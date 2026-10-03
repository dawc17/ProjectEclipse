using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;

namespace Eclipse.Diagnostics
{
	/// <summary>
	/// Player-facing performance overlay. F3 (or Options > Display) cycles
	/// Off / Compact / Detailed; F4 saves a report players can send with a bug.
	/// Fight code brackets its simulation step and each fighter's AI with the
	/// static timing helpers (AI time is nested inside simulation time). Frame
	/// timing stats split the rest into CPU main thread, render thread and GPU,
	/// and the GC allocation counter shows how much garbage each frame creates.
	/// Loading screens are kept out of the gameplay statistics.
	/// </summary>
	[DefaultExecutionOrder(-32000)]
	public sealed class PerformanceOverlay : MonoBehaviour
	{
		public enum Mode
		{
			Off = 0,
			Compact = 1,
			Detailed = 2
		}

		private const string ModePlayerPref = "Eclipse.PerformanceOverlay";
		private const int SampleCount = 300;
		private const int SpikeCapacity = 64;
		private const float TextRefreshSeconds = 0.25f;
		private const float SpikeMinimumMs = 20f;
		// Frames this soon after a scene change still belong to its loading work.
		private const float SceneSettleSeconds = 1f;
		private const int GraphHeight = 60;
		private const float GraphTopMs = 25f;
		// Whole-session fight histogram: 0.1 ms buckets up to 100 ms, then one overflow bucket.
		private const float HistogramBucketMs = 0.1f;
		private const int HistogramBuckets = 1001;

		private struct Sample
		{
			public float FrameMs;
			public float AiMs;
			public float SimMs;
			public float SnapshotMs;
			public float RestoreMs;
			public int SimTicks;
			public float CpuMainMs;
			public float RenderThreadMs;
			public float GpuMs;
			public float AllocKb;
			public bool Gc;
			public bool Loading;
			public bool InFight;
		}

		private struct Spike
		{
			public float Time;
			public Sample Frame;
			public string Scene;
		}

		private static readonly double TicksToMs = 1000.0 / Stopwatch.Frequency;
		private static PerformanceOverlay _instance;
		private static bool _modeLoaded;
		private static Mode _mode;
		private static bool _collecting;

		// Accumulated by the fight hooks during the current frame.
		private static long _aiTicks;
		private static long _simTicks;
		private static long _snapshotTicks, _restoreTicks;
		private static int _simTickCount;
		private static long _aiStart;
		private static long _simStart;

		private readonly Sample[] _samples = new Sample[SampleCount];
		private readonly float[] _sortScratch = new float[SampleCount];
		private readonly Spike[] _spikes = new Spike[SpikeCapacity];
		private readonly Color32[] _graphPixels = new Color32[SampleCount * GraphHeight];
		private readonly int[] _fightHistogram = new int[HistogramBuckets];
		private readonly FrameTiming[] _frameTimings = new FrameTiming[1];
		private ProfilerRecorder _allocRecorder;
		private long _lastThreadAllocBytes = -1;
		private int _sampleIndex;
		private int _sampleFilled;
		private int _spikeIndex;
		private int _spikeTotal;
		private int _gameplaySpikeTotal;
		private int _lastGcCount;
		private int _gcStartCount;
		private int _fightFrames;
		private double _fightFrameMsTotal;
		private double _fightAllocKbTotal;
		private double _fightSnapshotMsTotal, _fightRestoreMsTotal;
		private int _fightGcCount;
		private float _sessionStart;
		private float _sceneChangedAt;
		private float _nextTextRefresh;
		private bool _graphDirty;
		private string _summary = string.Empty;
		private string _status = string.Empty;
		private float _statusUntil;
		private Texture2D _graph;
		private Texture2D _background;
		private GUIStyle _textStyle;
		private GUIStyle _smallStyle;

		internal static void RecordRollbackSave(long ticks)
		{
			if (_collecting) _snapshotTicks += ticks;
		}

		internal static void RecordRollbackRestore(long ticks)
		{
			if (_collecting) _restoreTicks += ticks;
		}

		public static Mode CurrentMode
		{
			get
			{
				LoadMode();
				return _mode;
			}
		}

		public static string ModeLabel(Mode mode)
		{
			return mode == Mode.Off ? "Off" : mode == Mode.Compact ? "Compact" : "Detailed";
		}

		public static void CycleMode()
		{
			SetMode((Mode)(((int)CurrentMode + 1) % 3));
		}

		public static void SetMode(Mode mode)
		{
			LoadMode();
			bool wasCollecting = _collecting;
			_mode = mode;
			_collecting = mode != Mode.Off;
			if (Eclipse.Modding.ModRuntime.Scripts != null)
				Eclipse.Modding.ModRuntime.Scripts.CallbackDiagnostics.Recording = _collecting;
			PlayerPrefs.SetInt(ModePlayerPref, (int)mode);
			PlayerPrefs.Save();
			if (_instance != null && _collecting != wasCollecting)
			{
				_instance.SetRecording(_collecting);
				if (_collecting)
				{
					_instance.ResetStats();
				}
			}
		}

		public static void BeginAi()
		{
			if (_collecting)
			{
				_aiStart = Stopwatch.GetTimestamp();
			}
		}

		public static void EndAi()
		{
			if (_collecting && _aiStart != 0)
			{
				_aiTicks += Stopwatch.GetTimestamp() - _aiStart;
				_aiStart = 0;
			}
		}

		public static void BeginFightSimulation()
		{
			if (_collecting)
			{
				_simStart = Stopwatch.GetTimestamp();
			}
		}

		public static void EndFightSimulation()
		{
			if (_collecting && _simStart != 0)
			{
				_simTicks += Stopwatch.GetTimestamp() - _simStart;
				_simTickCount++;
				_simStart = 0;
			}
		}

		private static void LoadMode()
		{
			if (_modeLoaded)
			{
				return;
			}
			_modeLoaded = true;
			_mode = (Mode)Mathf.Clamp(PlayerPrefs.GetInt(ModePlayerPref, 0), 0, 2);
			_collecting = _mode != Mode.Off;
		}

		private static bool IsLoadingScene(string scene)
		{
			return scene == "Loader" || scene == "GameLoader" || scene == "Preloader";
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void EnsureInstance()
		{
			if (_instance != null)
			{
				return;
			}
			GameObject host = new GameObject("Eclipse Performance Overlay");
			DontDestroyOnLoad(host);
			_instance = host.AddComponent<PerformanceOverlay>();
		}

		private void Awake()
		{
			LoadMode();
			SceneManager.activeSceneChanged += OnActiveSceneChanged;
			SetRecording(_collecting);
			ResetStats();
		}

		private void OnDestroy()
		{
			SceneManager.activeSceneChanged -= OnActiveSceneChanged;
			SetRecording(false);
			if (_instance == this)
			{
				_instance = null;
			}
			if (_graph != null)
			{
				Destroy(_graph);
			}
			if (_background != null)
			{
				Destroy(_background);
			}
		}

		private void OnActiveSceneChanged(Scene from, Scene to)
		{
			_sceneChangedAt = Time.unscaledTime;
		}

		private void SetRecording(bool recording)
		{
			// The allocation counter is available in release players; it only
			// records while the overlay is on.
			if (recording && !_allocRecorder.Valid)
			{
				_allocRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Allocated In Frame");
			}
			else if (!recording && _allocRecorder.Valid)
			{
				_allocRecorder.Dispose();
			}
		}

		// Profiler counters are missing from release players, so fall back to the
		// main thread's allocation total (where the game allocates).
		private float ReadAllocKb()
		{
			if (_allocRecorder.Valid && _allocRecorder.LastValue > 0)
			{
				return _allocRecorder.LastValue / 1024f;
			}
			long bytes;
			try
			{
				bytes = GC.GetAllocatedBytesForCurrentThread();
			}
			catch (Exception)
			{
				return -1f;
			}
			long previous = _lastThreadAllocBytes;
			_lastThreadAllocBytes = bytes;
			return previous < 0 || bytes <= 0 ? -1f : (bytes - previous) / 1024f;
		}

		private void ResetStats()
		{
			_sampleIndex = 0;
			_sampleFilled = 0;
			_spikeIndex = 0;
			_spikeTotal = 0;
			_gameplaySpikeTotal = 0;
			_aiTicks = 0;
			_simTicks = 0;
			_simTickCount = 0;
			_snapshotTicks = _restoreTicks = 0;
			_lastGcCount = GC.CollectionCount(0);
			_gcStartCount = _lastGcCount;
			Array.Clear(_fightHistogram, 0, _fightHistogram.Length);
			_fightFrames = 0;
			_fightFrameMsTotal = 0;
			_fightAllocKbTotal = 0;
			_fightSnapshotMsTotal = _fightRestoreMsTotal = 0;
			_fightGcCount = 0;
			_lastThreadAllocBytes = -1;
			_sessionStart = Time.unscaledTime;
			_nextTextRefresh = 0f;
		}

		private void Update()
		{
			if (Eclipse.Input.EclipseInput.GetKeyDown(KeyCode.F3))
			{
				CycleMode();
				ShowStatus("Performance overlay: " + ModeLabel(_mode));
			}
			if (!_collecting)
			{
				return;
			}
			if (Eclipse.Input.EclipseInput.GetKeyDown(KeyCode.F4))
			{
				SaveReport();
			}

			// Runs first each frame, so the accumulators hold the previous frame.
			// Frame timing stats arrive a few frames late; they are close enough
			// for averages and for attributing sustained drops.
			float cpuMain = -1f, renderThread = -1f, gpu = -1f;
			FrameTimingManager.CaptureFrameTimings();
			if (FrameTimingManager.GetLatestTimings(1, _frameTimings) > 0)
			{
				cpuMain = (float)_frameTimings[0].cpuMainThreadFrameTime;
				renderThread = (float)_frameTimings[0].cpuRenderThreadFrameTime;
				gpu = (float)_frameTimings[0].gpuFrameTime;
			}

			int gcCount = GC.CollectionCount(0);
			string scene = SceneManager.GetActiveScene().name;
			Sample sample = new Sample
			{
				FrameMs = Time.unscaledDeltaTime * 1000f,
				AiMs = (float)(_aiTicks * TicksToMs),
				SimMs = (float)(_simTicks * TicksToMs),
				SnapshotMs = (float)(_snapshotTicks * TicksToMs),
				RestoreMs = (float)(_restoreTicks * TicksToMs),
				SimTicks = _simTickCount,
				CpuMainMs = cpuMain,
				RenderThreadMs = renderThread,
				GpuMs = gpu,
				AllocKb = ReadAllocKb(),
				Gc = gcCount != _lastGcCount,
				Loading = IsLoadingScene(scene) || Time.unscaledTime - _sceneChangedAt < SceneSettleSeconds,
				InFight = Fight.GetCurrentFight() != null
			};
			_lastGcCount = gcCount;
			_snapshotTicks = _restoreTicks = 0;
			_aiTicks = 0;
			_simTicks = 0;
			_simTickCount = 0;

			// Skip the first frames after start: they include startup itself.
			if (Time.frameCount > 2)
			{
				RecordSample(sample, scene);
			}
			if (Time.unscaledTime >= _nextTextRefresh)
			{
				_nextTextRefresh = Time.unscaledTime + TextRefreshSeconds;
				_summary = BuildSummary(_mode == Mode.Detailed);
				_graphDirty = true;
			}
		}

		private void RecordSample(Sample sample, string scene)
		{
			// Most frames cannot be spikes. Avoid sorting 300 samples every
			// rendered frame just to reject them against the 20 ms minimum.
			int gameplayCount = 0;
			float median = sample.FrameMs >= SpikeMinimumMs ? Percentile(0.5f, out gameplayCount) : 0f;
			_samples[_sampleIndex] = sample;
			_sampleIndex = (_sampleIndex + 1) % SampleCount;
			_sampleFilled = Mathf.Min(_sampleFilled + 1, SampleCount);

			if (sample.InFight && !sample.Loading)
			{
				int bucket = Mathf.Min((int)(sample.FrameMs / HistogramBucketMs), HistogramBuckets - 1);
				_fightHistogram[bucket]++;
				_fightFrames++;
				_fightFrameMsTotal += sample.FrameMs;
				_fightSnapshotMsTotal += sample.SnapshotMs;
				_fightRestoreMsTotal += sample.RestoreMs;
				if (sample.AllocKb > 0f)
				{
					_fightAllocKbTotal += sample.AllocKb;
				}
				if (sample.Gc)
				{
					_fightGcCount++;
				}
			}

			bool spike = sample.FrameMs >= SpikeMinimumMs &&
				(gameplayCount < 30 || sample.FrameMs >= Mathf.Max(median * 2f, median + 8f));
			if (spike)
			{
				_spikes[_spikeIndex] = new Spike
				{
					Time = Time.unscaledTime - _sessionStart,
					Frame = sample,
					Scene = scene
				};
				_spikeIndex = (_spikeIndex + 1) % SpikeCapacity;
				_spikeTotal++;
				if (!sample.Loading)
				{
					_gameplaySpikeTotal++;
				}
			}
		}

		// Percentile of the frame times in the window, ignoring loading frames.
		private float Percentile(float fraction, out int count)
		{
			count = 0;
			for (int i = 0; i < _sampleFilled; i++)
			{
				if (!_samples[i].Loading)
				{
					_sortScratch[count++] = _samples[i].FrameMs;
				}
			}
			if (count == 0)
			{
				return 0f;
			}
			Array.Sort(_sortScratch, 0, count);
			int index = Mathf.Clamp(Mathf.RoundToInt(fraction * (count - 1)), 0, count - 1);
			return _sortScratch[index];
		}

		private float FightPercentile(float fraction)
		{
			int target = Mathf.CeilToInt(fraction * _fightFrames);
			int seen = 0;
			for (int i = 0; i < HistogramBuckets; i++)
			{
				seen += _fightHistogram[i];
				if (seen >= target)
				{
					return (i + 0.5f) * HistogramBucketMs;
				}
			}
			return 0f;
		}

		private static string SpikeCause(Sample s)
		{
			if (s.Loading)
			{
				return "loading";
			}
			if (s.AiMs >= s.FrameMs * 0.35f)
			{
				return "enemy AI";
			}
			if (s.SnapshotMs + s.RestoreMs >= s.FrameMs * 0.35f)
			{
				return "rollback snapshots / restores";
			}
			if (s.SimMs - s.AiMs >= s.FrameMs * 0.35f)
			{
				return "fight simulation";
			}
			if (s.Gc)
			{
				return "garbage collection";
			}
			if (s.GpuMs > 0f && s.GpuMs >= s.FrameMs * 0.6f && s.GpuMs > s.CpuMainMs)
			{
				return "GPU";
			}
			if (s.CpuMainMs > 0f)
			{
				return "CPU main thread (scripts / rendering setup)";
			}
			return "rendering / other";
		}

		private string BuildSummary(bool detailed)
		{
			float total = 0f, worst = 0f, aiTotal = 0f, aiWorst = 0f, simTotal = 0f, simWorst = 0f;
			float cpuTotal = 0f, renderTotal = 0f, gpuTotal = 0f, allocTotal = 0f;
			int count = 0, simTicks = 0, timingCount = 0, allocCount = 0;
			for (int i = 0; i < _sampleFilled; i++)
			{
				Sample s = _samples[i];
				if (s.Loading)
				{
					continue;
				}
				count++;
				total += s.FrameMs;
				worst = Mathf.Max(worst, s.FrameMs);
				aiTotal += s.AiMs;
				aiWorst = Mathf.Max(aiWorst, s.AiMs);
				simTotal += s.SimMs;
				simWorst = Mathf.Max(simWorst, s.SimMs);
				simTicks += s.SimTicks;
				if (s.CpuMainMs > 0f)
				{
					timingCount++;
					cpuTotal += s.CpuMainMs;
					renderTotal += Mathf.Max(s.RenderThreadMs, 0f);
					gpuTotal += Mathf.Max(s.GpuMs, 0f);
				}
				if (s.AllocKb >= 0f)
				{
					allocCount++;
					allocTotal += s.AllocKb;
				}
			}
			if (count == 0)
			{
				var loadingCallbacks = Eclipse.Modding.ModRuntime.Scripts?.CallbackDiagnostics;
				return "Loading..." + (detailed && loadingCallbacks != null ? "\n" + loadingCallbacks.FormatSummary() : string.Empty);
			}
			float average = total / count;
			float onePercentLow = Percentile(0.99f, out _);

			StringBuilder text = new StringBuilder(768);
			text.Append("FPS ").Append(Mathf.RoundToInt(1000f / Mathf.Max(average, 0.01f)))
				.Append("   ").Append(average.ToString("0.00", CultureInfo.InvariantCulture)).Append(" ms");
			text.Append("\n1% low ").Append(Mathf.RoundToInt(1000f / Mathf.Max(onePercentLow, 0.01f)))
				.Append(" FPS   worst ").Append(worst.ToString("0.0", CultureInfo.InvariantCulture)).Append(" ms");
			if (!detailed)
			{
				return text.ToString();
			}

			if (timingCount > 0)
			{
				text.Append("\nCPU main ").Append((cpuTotal / timingCount).ToString("0.00", CultureInfo.InvariantCulture))
					.Append("  render ").Append((renderTotal / timingCount).ToString("0.00", CultureInfo.InvariantCulture))
					.Append("  GPU ").Append((gpuTotal / timingCount).ToString("0.00", CultureInfo.InvariantCulture)).Append(" ms");
			}
			else
			{
				text.Append("\nCPU/GPU split unavailable on this device");
			}
			text.Append("\nEnemy AI ").Append((aiTotal / count).ToString("0.00", CultureInfo.InvariantCulture))
				.Append(" / max ").Append(aiWorst.ToString("0.00", CultureInfo.InvariantCulture))
				.Append("   fight sim ").Append((simTotal / count).ToString("0.00", CultureInfo.InvariantCulture))
				.Append(" / max ").Append(simWorst.ToString("0.00", CultureInfo.InvariantCulture)).Append(" ms");
			text.Append("\nSim ticks/frame ").Append((simTicks / (float)count).ToString("0.00", CultureInfo.InvariantCulture));
			if (allocCount > 0)
			{
				float allocPerFrame = allocTotal / allocCount;
				text.Append("   garbage ").Append(allocPerFrame.ToString("0.0", CultureInfo.InvariantCulture)).Append(" KB/frame (")
					.Append((allocPerFrame * 1000f / average / 1024f).ToString("0.0", CultureInfo.InvariantCulture)).Append(" MB/s)");
			}
			text.Append("\nGC ").Append(GC.CollectionCount(0) - _gcStartCount).Append(" collections   heap ")
				.Append((Profiler.GetMonoUsedSizeLong() / 1048576f).ToString("0", CultureInfo.InvariantCulture)).Append(" MB");
			text.Append("\nSpikes ").Append(_gameplaySpikeTotal).Append(" (+")
				.Append(_spikeTotal - _gameplaySpikeTotal).Append(" loading)");
			Spike? last = LastGameplaySpike();
			if (last.HasValue)
			{
				text.Append("   last ").Append(last.Value.Frame.FrameMs.ToString("0", CultureInfo.InvariantCulture)).Append(" ms: ")
					.Append(SpikeCause(last.Value.Frame));
			}
			if (_fightFrames > 0)
			{
				float fightAverage = (float)(_fightFrameMsTotal / _fightFrames);
				text.Append("\nAll fights: ").Append(Mathf.RoundToInt(1000f / fightAverage)).Append(" FPS avg, 1% low ")
					.Append(Mathf.RoundToInt(1000f / Mathf.Max(FightPercentile(0.99f), 0.01f))).Append(" FPS");
			}
			var callbacks = Eclipse.Modding.ModRuntime.Scripts?.CallbackDiagnostics;
			if (callbacks != null) text.Append('\n').Append(callbacks.FormatSummary());
			return text.ToString();
		}

		private Spike? LastGameplaySpike()
		{
			int count = Mathf.Min(_spikeTotal, SpikeCapacity);
			for (int i = 1; i <= count; i++)
			{
				Spike spike = _spikes[(_spikeIndex - i + SpikeCapacity) % SpikeCapacity];
				if (!spike.Frame.Loading)
				{
					return spike;
				}
			}
			return null;
		}

		private void SaveReport()
		{
			try
			{
				string directory = Path.Combine(Eclipse.Runtime.EditorPlayModeContext.PersistentDataPath, "Diagnostics");
				Directory.CreateDirectory(directory);
				string path = Path.Combine(directory, "performance-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".txt");
				File.WriteAllText(path, BuildReport());
				GUIUtility.systemCopyBuffer = path;
				ShowStatus("Report saved (path copied): " + path);
			}
			catch (Exception e)
			{
				ShowStatus("Could not save report: " + e.Message);
			}
		}

		private string BuildReport()
		{
			StringBuilder report = new StringBuilder(49152);
			report.AppendLine("Eclipse performance report (format 5)");
			report.AppendLine("Created: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
			report.AppendLine("Game version: " + Application.version + "  Unity " + Application.unityVersion);
			report.AppendLine("OS: " + SystemInfo.operatingSystem);
			report.AppendLine("CPU: " + SystemInfo.processorType + " (" + SystemInfo.processorCount + " threads, " +
				SystemInfo.processorFrequency + " MHz)");
			report.AppendLine("RAM: " + SystemInfo.systemMemorySize + " MB");
			report.AppendLine("Memory at report: Unity allocated " + MemoryMb(Profiler.GetTotalAllocatedMemoryLong()) +
				" MB, reserved " + MemoryMb(Profiler.GetTotalReservedMemoryLong()) +
				" MB; managed used " + MemoryMb(Profiler.GetMonoUsedSizeLong()) +
				" MB, reserved " + MemoryMb(Profiler.GetMonoHeapSizeLong()) + " MB");
			// Query the OS only when saving a report, never from the per-frame overlay.
			try
			{
				using (var process = Process.GetCurrentProcess())
					report.AppendLine("Process at report: working set " + MemoryMb(process.WorkingSet64) +
						" MB, private committed " + MemoryMb(process.PrivateMemorySize64) + " MB");
			}
			catch (Exception) { report.AppendLine("Process memory counters unavailable on this runtime."); }
			report.AppendLine("GPU: " + SystemInfo.graphicsDeviceName + " (" + SystemInfo.graphicsDeviceType + ", " +
				SystemInfo.graphicsMemorySize + " MB, " + SystemInfo.graphicsDeviceVersion + ")");
			report.AppendLine("Display: " + Screen.width + "x" + Screen.height + " " + Screen.fullScreenMode +
				" @ " + Screen.currentResolution.refreshRateRatio.value.ToString("0.##", CultureInfo.InvariantCulture) + " Hz");
			report.AppendLine("Settings: vsync " + QualitySettings.vSyncCount + ", target FPS " + Application.targetFrameRate +
				", frame limit " + SF2DisplayFrameRate.MaxFrameRate + ", interpolation " + SF2DisplayFrameRate.InterpolationEnabled +
				", motion blur " + SF2DisplayFrameRate.MotionBlurEnabled + ", MSAA " + QualitySettings.antiAliasing +
				", quality " + QualitySettings.names[QualitySettings.GetQualityLevel()]);
			report.AppendLine("Frame timing stats: " + (FrameTimingManager.IsFeatureEnabled() ? "on" : "off") +
				"   GC counter: " + (_allocRecorder.Valid ? "profiler" : "thread allocations"));
			report.AppendLine("Scene: " + SceneManager.GetActiveScene().name + (Fight.GetCurrentFight() != null ? " (in fight)" : string.Empty));
			report.AppendLine("Session: " + (Time.unscaledTime - _sessionStart).ToString("0", CultureInfo.InvariantCulture) + " s measured");
			report.AppendLine();
			report.AppendLine("Current window (last " + SampleCount + " frames, loading excluded):");
			report.AppendLine(BuildSummary(true));
			report.AppendLine();
			var scripts = Eclipse.Modding.ModRuntime.Scripts;
			if (scripts != null)
			{
				report.AppendLine("Active mod session (resolved load order):");
				report.AppendLine(scripts.FormatReport());
				report.AppendLine(scripts.CallbackDiagnostics.FormatReport());
			}

			if (_fightFrames > 0)
			{
				report.AppendLine("All fights this session (" + _fightFrames + " frames, loading excluded):");
				float fightAverage = (float)(_fightFrameMsTotal / _fightFrames);
				report.AppendLine("  average " + fightAverage.ToString("0.00", CultureInfo.InvariantCulture) + " ms (" + Mathf.RoundToInt(1000f / fightAverage) + " FPS)");
				report.AppendLine("  median " + FightPercentile(0.5f).ToString("0.00", CultureInfo.InvariantCulture) + " ms, 90% " + FightPercentile(0.9f).ToString("0.00", CultureInfo.InvariantCulture) +
					" ms, 99% " + FightPercentile(0.99f).ToString("0.00", CultureInfo.InvariantCulture) + " ms, 99.9% " + FightPercentile(0.999f).ToString("0.00", CultureInfo.InvariantCulture) + " ms");
				if (Application.targetFrameRate > 0)
				{
					float budget = 1000f / Application.targetFrameRate * 1.05f;
					int over = 0;
					for (int i = Mathf.Min((int)(budget / HistogramBucketMs), HistogramBuckets - 1); i < HistogramBuckets; i++)
					{
						over += _fightHistogram[i];
					}
					report.AppendLine("  frames slower than the " + Application.targetFrameRate + " FPS cap: " +
						(100f * over / _fightFrames).ToString("0.0", CultureInfo.InvariantCulture) + "%");
				}
				report.AppendLine("  garbage " + (_fightAllocKbTotal / _fightFrames).ToString("0.0", CultureInfo.InvariantCulture) + " KB/frame, " +
					_fightGcCount + " GC frames");
				report.AppendLine("  rollback saves " + (_fightSnapshotMsTotal / _fightFrames).ToString("0.000", CultureInfo.InvariantCulture) +
					" ms/frame, restores " + (_fightRestoreMsTotal / _fightFrames).ToString("0.000", CultureInfo.InvariantCulture) + " ms/frame (additional to fight sim)");
				report.AppendLine();
			}

			int spikeCount = Mathf.Min(_spikeTotal, SpikeCapacity);
			report.AppendLine("Frame spikes (latest " + spikeCount + " of " + _spikeTotal + ", " + _gameplaySpikeTotal + " outside loading):");
			report.AppendLine("time_s,frame_ms,ai_ms,fight_sim_ms,cpu_main_ms,render_thread_ms,gpu_ms,alloc_kb,gc,loading,in_fight,rollback_save_ms,rollback_restore_ms,scene,likely_cause");
			for (int i = 0; i < spikeCount; i++)
			{
				Spike s = _spikes[(_spikeIndex - spikeCount + i + SpikeCapacity) % SpikeCapacity];
				report.Append(s.Time.ToString("0.00", CultureInfo.InvariantCulture)).Append(',');
				AppendSample(report, s.Frame);
				report.Append(',').Append(s.Scene).Append(',').AppendLine(SpikeCause(s.Frame));
			}
			report.AppendLine();

			report.AppendLine("Last " + _sampleFilled + " frames (oldest first; CPU/GPU timings lag a few frames):");
			report.AppendLine("frame_ms,ai_ms,fight_sim_ms,cpu_main_ms,render_thread_ms,gpu_ms,alloc_kb,gc,loading,in_fight,rollback_save_ms,rollback_restore_ms,sim_ticks");
			for (int i = 0; i < _sampleFilled; i++)
			{
				Sample s = _samples[(_sampleIndex - _sampleFilled + i + SampleCount) % SampleCount];
				AppendSample(report, s);
				report.Append(',').Append(s.SimTicks).AppendLine();
			}
			return report.ToString();
		}

		private static string MemoryMb(long bytes) => (bytes / 1048576.0).ToString("0.0", CultureInfo.InvariantCulture);

		private static void AppendSample(StringBuilder report, Sample s)
		{
			report.Append(s.FrameMs.ToString("0.00", CultureInfo.InvariantCulture)).Append(',')
				.Append(s.AiMs.ToString("0.000", CultureInfo.InvariantCulture)).Append(',')
				.Append(s.SimMs.ToString("0.000", CultureInfo.InvariantCulture)).Append(',')
				.Append(s.CpuMainMs.ToString("0.00", CultureInfo.InvariantCulture)).Append(',')
				.Append(s.RenderThreadMs.ToString("0.00", CultureInfo.InvariantCulture)).Append(',')
				.Append(s.GpuMs.ToString("0.00", CultureInfo.InvariantCulture)).Append(',')
				.Append(s.AllocKb.ToString("0.0", CultureInfo.InvariantCulture)).Append(',')
				.Append(s.Gc ? 1 : 0).Append(',')
				.Append(s.Loading ? 1 : 0).Append(',')
				.Append(s.InFight ? 1 : 0).Append(',')
				.Append(s.SnapshotMs.ToString("0.000", CultureInfo.InvariantCulture)).Append(',')
				.Append(s.RestoreMs.ToString("0.000", CultureInfo.InvariantCulture));
		}

		private void ShowStatus(string message)
		{
			_status = message;
			_statusUntil = Time.unscaledTime + 5f;
		}

		private void OnGUI()
		{
			bool showStatus = Time.unscaledTime < _statusUntil;
			if (!_collecting && !showStatus)
			{
				return;
			}
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			EnsureStyles();

			Matrix4x4 oldMatrix = GUI.matrix;
			float scale = Mathf.Clamp(Screen.height / 900f, 0.85f, 1.35f);
			GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1f));
			float screenWidth = Screen.width / scale;

			const float detailedTextHeight = 210f;
			if (_collecting)
			{
				bool detailed = _mode == Mode.Detailed;
				float width = detailed ? 420f : 190f;
				float height = detailed ? detailedTextHeight + GraphHeight + 34f : 46f;
				Rect panel = new Rect(screenWidth - width - 12f, 12f, width, height);
				GUI.DrawTexture(panel, _background);
				GUI.Label(new Rect(panel.x + 8f, panel.y + 4f, width - 16f, detailedTextHeight), _summary, _textStyle);
				if (detailed)
				{
					Rect graphRect = new Rect(panel.x + 8f, panel.y + detailedTextHeight, width - 16f, GraphHeight);
					// Redrawn with the text so the overlay costs little itself.
					if (_graphDirty)
					{
						_graphDirty = false;
						UpdateGraph();
					}
					GUI.DrawTexture(graphRect, _graph);
					GUI.Label(new Rect(panel.x + 8f, graphRect.yMax + 2f, width - 16f, 30f),
						"green frame  magenta AI  blue fight sim  grey loading  line = cap\nF3 hide   F4 save report", _smallStyle);
				}
			}

			if (showStatus)
			{
				GUI.Label(new Rect(screenWidth - 612f, _collecting && _mode == Mode.Detailed ? detailedTextHeight + GraphHeight + 52f : 64f,
					600f, 40f), _status, _smallStyle);
			}
			GUI.matrix = oldMatrix;
		}

		private void UpdateGraph()
		{
			// Each column is one frame; the top is GraphTopMs (40 FPS).
			Color32 clear = new Color32(0, 0, 0, 0);
			Color32 frame = new Color32(90, 200, 110, 230);
			Color32 slow = new Color32(235, 80, 60, 240);
			Color32 loading = new Color32(120, 120, 120, 200);
			Color32 ai = new Color32(230, 70, 220, 240);
			Color32 sim = new Color32(70, 140, 255, 240);
			Color32 guide = new Color32(255, 255, 255, 110);
			float budgetMs = Application.targetFrameRate > 0 ? 1000f / Application.targetFrameRate : 1000f / 60f;
			int guideRow = Mathf.Clamp(Mathf.RoundToInt(budgetMs / GraphTopMs * GraphHeight) - 1, 0, GraphHeight - 1);
			for (int x = 0; x < SampleCount; x++)
			{
				int age = SampleCount - 1 - x;
				bool has = age < _sampleFilled;
				Sample s = has ? _samples[(_sampleIndex - 1 - age + SampleCount) % SampleCount] : default(Sample);
				int frameRows = has ? Mathf.Clamp(Mathf.CeilToInt(s.FrameMs / GraphTopMs * GraphHeight), 1, GraphHeight) : 0;
				int aiRows = has ? Mathf.Clamp(Mathf.CeilToInt(s.AiMs / GraphTopMs * GraphHeight), 0, frameRows) : 0;
				// Fight simulation time includes the AI it runs, so stack only the remainder.
				int simRows = has ? Mathf.Clamp(Mathf.CeilToInt((s.SimMs - s.AiMs) / GraphTopMs * GraphHeight), 0, frameRows - aiRows) : 0;
				Color32 top = s.Loading ? loading : s.FrameMs > budgetMs * 1.5f ? slow : frame;
				for (int y = 0; y < GraphHeight; y++)
				{
					Color32 color = y < aiRows ? ai : y < aiRows + simRows ? sim : y < frameRows ? top : clear;
					if (y == guideRow && color.a == 0)
					{
						color = guide;
					}
					_graphPixels[y * SampleCount + x] = color;
				}
			}
			_graph.SetPixels32(_graphPixels);
			_graph.Apply(false);
		}

		private void EnsureStyles()
		{
			if (_textStyle != null)
			{
				return;
			}
			_background = new Texture2D(1, 1, TextureFormat.RGBA32, false);
			_background.SetPixel(0, 0, new Color(0.04f, 0.05f, 0.07f, 0.78f));
			_background.Apply();
			_background.hideFlags = HideFlags.HideAndDontSave;
			_graph = new Texture2D(SampleCount, GraphHeight, TextureFormat.RGBA32, false);
			_graph.filterMode = FilterMode.Point;
			_graph.wrapMode = TextureWrapMode.Clamp;
			_graph.hideFlags = HideFlags.HideAndDontSave;
			_textStyle = new GUIStyle(GUI.skin.label)
			{
				fontSize = 13,
				richText = false,
				normal = { textColor = new Color(0.92f, 0.95f, 1f) }
			};
			_smallStyle = new GUIStyle(_textStyle)
			{
				fontSize = 11,
				wordWrap = true,
				normal = { textColor = new Color(0.72f, 0.77f, 0.85f) }
			};
		}
	}
}
