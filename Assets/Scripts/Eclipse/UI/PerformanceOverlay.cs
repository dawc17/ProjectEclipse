using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;

namespace Eclipse.Diagnostics
{
	/// <summary>
	/// Player-facing performance overlay. F3 (or Options > Display) cycles
	/// Off / Compact / Detailed; F4 saves a report players can send with a bug.
	/// Fight code brackets its simulation and AI work with the static timing
	/// helpers so a frame drop can be attributed to AI, fight simulation,
	/// garbage collection or rendering.
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
		private const int GraphHeight = 60;

		private struct Sample
		{
			public float FrameMs;
			public float AiMs;
			public float SimMs;
			public int SimTicks;
			public bool Gc;
		}

		private struct Spike
		{
			public float Time;
			public float FrameMs;
			public float AiMs;
			public float SimMs;
			public bool Gc;
			public bool InFight;
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
		private static int _simTickCount;
		private static long _aiStart;
		private static long _simStart;

		private readonly Sample[] _samples = new Sample[SampleCount];
		private readonly float[] _sortScratch = new float[SampleCount];
		private readonly Spike[] _spikes = new Spike[SpikeCapacity];
		private readonly Color32[] _graphPixels = new Color32[SampleCount * GraphHeight];
		private int _sampleIndex;
		private int _sampleFilled;
		private int _spikeIndex;
		private int _spikeTotal;
		private int _lastGcCount;
		private int _gcStartCount;
		private float _sessionStart;
		private float _nextTextRefresh;
		private string _summary = string.Empty;
		private string _status = string.Empty;
		private float _statusUntil;
		private Texture2D _graph;
		private Texture2D _background;
		private GUIStyle _textStyle;
		private GUIStyle _smallStyle;

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
			_mode = mode;
			_collecting = mode != Mode.Off;
			PlayerPrefs.SetInt(ModePlayerPref, (int)mode);
			PlayerPrefs.Save();
			if (_instance != null && _collecting)
			{
				_instance.ResetStats();
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
			ResetStats();
		}

		private void OnDestroy()
		{
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

		private void ResetStats()
		{
			_sampleIndex = 0;
			_sampleFilled = 0;
			_spikeIndex = 0;
			_spikeTotal = 0;
			_aiTicks = 0;
			_simTicks = 0;
			_simTickCount = 0;
			_lastGcCount = GC.CollectionCount(0);
			_gcStartCount = _lastGcCount;
			_sessionStart = Time.unscaledTime;
			_nextTextRefresh = 0f;
		}

		private void Update()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.F3))
			{
				CycleMode();
				ShowStatus("Performance overlay: " + ModeLabel(_mode));
			}
			if (!_collecting)
			{
				return;
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.F4))
			{
				SaveReport();
			}

			// Runs first each frame, so the accumulators hold the previous frame.
			int gcCount = GC.CollectionCount(0);
			Sample sample = new Sample
			{
				FrameMs = Time.unscaledDeltaTime * 1000f,
				AiMs = (float)(_aiTicks * TicksToMs),
				SimMs = (float)(_simTicks * TicksToMs),
				SimTicks = _simTickCount,
				Gc = gcCount != _lastGcCount
			};
			_lastGcCount = gcCount;
			_aiTicks = 0;
			_simTicks = 0;
			_simTickCount = 0;

			// Skip the first frames after enabling or loading: they include the
			// hitch of the change itself.
			if (Time.frameCount > 2)
			{
				RecordSample(sample);
			}
			if (Time.unscaledTime >= _nextTextRefresh)
			{
				_nextTextRefresh = Time.unscaledTime + TextRefreshSeconds;
				_summary = BuildSummary(_mode == Mode.Detailed);
			}
		}

		private void RecordSample(Sample sample)
		{
			float median = _sampleFilled >= 30 ? Percentile(0.5f) : 0f;
			_samples[_sampleIndex] = sample;
			_sampleIndex = (_sampleIndex + 1) % SampleCount;
			_sampleFilled = Mathf.Min(_sampleFilled + 1, SampleCount);

			if (median > 0f && sample.FrameMs >= SpikeMinimumMs &&
				sample.FrameMs >= Mathf.Max(median * 2f, median + 8f))
			{
				_spikes[_spikeIndex] = new Spike
				{
					Time = Time.unscaledTime - _sessionStart,
					FrameMs = sample.FrameMs,
					AiMs = sample.AiMs,
					SimMs = sample.SimMs,
					Gc = sample.Gc,
					InFight = Fight.GetCurrentFight() != null,
					Scene = SceneManager.GetActiveScene().name
				};
				_spikeIndex = (_spikeIndex + 1) % SpikeCapacity;
				_spikeTotal++;
			}
		}

		private float Percentile(float fraction)
		{
			// Order is irrelevant for a percentile, so the ring buffer is copied as is.
			for (int i = 0; i < _sampleFilled; i++)
			{
				_sortScratch[i] = _samples[i].FrameMs;
			}
			Array.Sort(_sortScratch, 0, _sampleFilled);
			int index = Mathf.Clamp(Mathf.RoundToInt(fraction * (_sampleFilled - 1)), 0, _sampleFilled - 1);
			return _sortScratch[index];
		}

		private static string SpikeCause(float frameMs, float aiMs, float simMs, bool gc)
		{
			if (aiMs >= frameMs * 0.35f)
			{
				return "enemy AI";
			}
			if (simMs - aiMs >= frameMs * 0.35f)
			{
				return "fight simulation";
			}
			return gc ? "garbage collection" : "rendering / other";
		}

		private string BuildSummary(bool detailed)
		{
			if (_sampleFilled == 0)
			{
				return "Measuring...";
			}

			float total = 0f, worst = 0f, aiTotal = 0f, aiWorst = 0f, simTotal = 0f, simWorst = 0f;
			int simTicks = 0;
			for (int i = 0; i < _sampleFilled; i++)
			{
				Sample s = _samples[i];
				total += s.FrameMs;
				worst = Mathf.Max(worst, s.FrameMs);
				aiTotal += s.AiMs;
				aiWorst = Mathf.Max(aiWorst, s.AiMs);
				simTotal += s.SimMs;
				simWorst = Mathf.Max(simWorst, s.SimMs);
				simTicks += s.SimTicks;
			}
			float average = total / _sampleFilled;
			float onePercentLow = Percentile(0.99f);

			StringBuilder text = new StringBuilder(512);
			text.Append("FPS ").Append(Mathf.RoundToInt(1000f / Mathf.Max(average, 0.01f)))
				.Append("   ").Append(average.ToString("0.0")).Append(" ms");
			text.Append("\n1% low ").Append(Mathf.RoundToInt(1000f / Mathf.Max(onePercentLow, 0.01f)))
				.Append(" FPS   worst ").Append(worst.ToString("0.0")).Append(" ms");
			if (!detailed)
			{
				return text.ToString();
			}

			text.Append("\nEnemy AI  avg ").Append((aiTotal / _sampleFilled).ToString("0.00"))
				.Append(" / max ").Append(aiWorst.ToString("0.00")).Append(" ms");
			text.Append("\nFight sim avg ").Append((simTotal / _sampleFilled).ToString("0.00"))
				.Append(" / max ").Append(simWorst.ToString("0.00")).Append(" ms  (")
				.Append((simTicks / (float)_sampleFilled).ToString("0.0")).Append(" ticks/frame)");
			text.Append("\nGC ").Append(GC.CollectionCount(0) - _gcStartCount).Append(" collections   heap ")
				.Append((Profiler.GetMonoUsedSizeLong() / 1048576f).ToString("0.0")).Append(" MB");
			text.Append("\nSpikes ").Append(_spikeTotal);
			if (_spikeTotal > 0)
			{
				Spike last = _spikes[(_spikeIndex + SpikeCapacity - 1) % SpikeCapacity];
				text.Append("   last ").Append(last.FrameMs.ToString("0")).Append(" ms: ")
					.Append(SpikeCause(last.FrameMs, last.AiMs, last.SimMs, last.Gc));
			}
			text.Append("\n").Append(Screen.width).Append("x").Append(Screen.height)
				.Append("  vsync ").Append(QualitySettings.vSyncCount)
				.Append("  cap ").Append(Application.targetFrameRate <= 0 ? "none" : Application.targetFrameRate.ToString());
			return text.ToString();
		}

		private void SaveReport()
		{
			try
			{
				string directory = Path.Combine(Application.persistentDataPath, "Diagnostics");
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
			StringBuilder report = new StringBuilder(32768);
			report.AppendLine("Eclipse performance report");
			report.AppendLine("Created: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
			report.AppendLine("Game version: " + Application.version + "  Unity " + Application.unityVersion);
			report.AppendLine("OS: " + SystemInfo.operatingSystem);
			report.AppendLine("CPU: " + SystemInfo.processorType + " (" + SystemInfo.processorCount + " threads, " +
				SystemInfo.processorFrequency + " MHz)");
			report.AppendLine("RAM: " + SystemInfo.systemMemorySize + " MB");
			report.AppendLine("GPU: " + SystemInfo.graphicsDeviceName + " (" + SystemInfo.graphicsDeviceType + ", " +
				SystemInfo.graphicsMemorySize + " MB, " + SystemInfo.graphicsDeviceVersion + ")");
			report.AppendLine("Display: " + Screen.width + "x" + Screen.height + " " + Screen.fullScreenMode +
				" @ " + Screen.currentResolution.refreshRateRatio.value.ToString("0.##") + " Hz");
			report.AppendLine("Settings: vsync " + QualitySettings.vSyncCount + ", target FPS " + Application.targetFrameRate +
				", frame limit " + SF2DisplayFrameRate.MaxFrameRate + ", interpolation " + SF2DisplayFrameRate.InterpolationEnabled +
				", motion blur " + SF2DisplayFrameRate.MotionBlurEnabled + ", MSAA " + QualitySettings.antiAliasing +
				", quality " + QualitySettings.names[QualitySettings.GetQualityLevel()]);
			report.AppendLine("Scene: " + SceneManager.GetActiveScene().name + (Fight.GetCurrentFight() != null ? " (in fight)" : string.Empty));
			report.AppendLine("Session: " + (Time.unscaledTime - _sessionStart).ToString("0") + " s measured");
			report.AppendLine();
			report.AppendLine(BuildSummary(true));
			report.AppendLine();

			int spikeCount = Mathf.Min(_spikeTotal, SpikeCapacity);
			report.AppendLine("Frame spikes (latest " + spikeCount + " of " + _spikeTotal + "):");
			report.AppendLine("time_s,frame_ms,ai_ms,fight_sim_ms,gc,in_fight,scene,likely_cause");
			for (int i = 0; i < spikeCount; i++)
			{
				Spike s = _spikes[(_spikeIndex - spikeCount + i + SpikeCapacity) % SpikeCapacity];
				report.Append(s.Time.ToString("0.00")).Append(',').Append(s.FrameMs.ToString("0.00")).Append(',')
					.Append(s.AiMs.ToString("0.000")).Append(',').Append(s.SimMs.ToString("0.000")).Append(',')
					.Append(s.Gc ? 1 : 0).Append(',').Append(s.InFight ? 1 : 0).Append(',').Append(s.Scene).Append(',')
					.AppendLine(SpikeCause(s.FrameMs, s.AiMs, s.SimMs, s.Gc));
			}
			report.AppendLine();

			report.AppendLine("Last " + _sampleFilled + " frames (oldest first):");
			report.AppendLine("frame_ms,ai_ms,fight_sim_ms,sim_ticks,gc");
			for (int i = 0; i < _sampleFilled; i++)
			{
				Sample s = _samples[(_sampleIndex - _sampleFilled + i + SampleCount) % SampleCount];
				report.Append(s.FrameMs.ToString("0.00")).Append(',').Append(s.AiMs.ToString("0.000")).Append(',')
					.Append(s.SimMs.ToString("0.000")).Append(',').Append(s.SimTicks).Append(',')
					.Append(s.Gc ? 1 : 0).AppendLine();
			}
			return report.ToString();
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
			if (Event.current.type != EventType.Repaint && Event.current.type != EventType.MouseDown &&
				Event.current.type != EventType.MouseUp)
			{
				return;
			}
			EnsureStyles();

			Matrix4x4 oldMatrix = GUI.matrix;
			float scale = Mathf.Clamp(Screen.height / 900f, 0.85f, 1.35f);
			GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1f));
			float screenWidth = Screen.width / scale;

			if (_collecting)
			{
				bool detailed = _mode == Mode.Detailed;
				float width = detailed ? 330f : 190f;
				float height = detailed ? 128f + GraphHeight + 34f : 46f;
				Rect panel = new Rect(screenWidth - width - 12f, 12f, width, height);
				GUI.DrawTexture(panel, _background);
				GUI.Label(new Rect(panel.x + 8f, panel.y + 4f, width - 16f, 124f), _summary, _textStyle);
				if (detailed)
				{
					Rect graphRect = new Rect(panel.x + 8f, panel.y + 128f, width - 16f, GraphHeight);
					if (Event.current.type == EventType.Repaint)
					{
						UpdateGraph();
					}
					GUI.DrawTexture(graphRect, _graph);
					GUI.Label(new Rect(panel.x + 8f, graphRect.yMax + 2f, width - 16f, 30f),
						"green frame  magenta AI  blue fight sim  line 60 FPS\nF3 hide   F4 save report", _smallStyle);
				}
			}

			if (showStatus)
			{
				GUI.Label(new Rect(screenWidth - 612f, _collecting && _mode == Mode.Detailed ? 240f : 64f, 600f, 40f),
					_status, _smallStyle);
			}
			GUI.matrix = oldMatrix;
		}

		private void UpdateGraph()
		{
			// Each column is one frame, scaled so the top is 50 ms (20 FPS).
			const float maxMs = 50f;
			Color32 clear = new Color32(0, 0, 0, 0);
			Color32 frame = new Color32(90, 200, 110, 230);
			Color32 slow = new Color32(235, 80, 60, 240);
			Color32 ai = new Color32(230, 70, 220, 240);
			Color32 sim = new Color32(70, 140, 255, 240);
			Color32 guide = new Color32(255, 255, 255, 90);
			int guideRow = Mathf.RoundToInt(16.67f / maxMs * (GraphHeight - 1));
			for (int x = 0; x < SampleCount; x++)
			{
				int age = SampleCount - 1 - x;
				bool has = age < _sampleFilled;
				Sample s = has ? _samples[(_sampleIndex - 1 - age + SampleCount) % SampleCount] : default(Sample);
				int frameRows = has ? Mathf.Clamp(Mathf.CeilToInt(s.FrameMs / maxMs * GraphHeight), 1, GraphHeight) : 0;
				int aiRows = has ? Mathf.Clamp(Mathf.CeilToInt(s.AiMs / maxMs * GraphHeight), 0, frameRows) : 0;
				// Fight simulation time includes the AI it runs, so stack only the remainder.
				int simRows = has ? Mathf.Clamp(Mathf.CeilToInt((s.SimMs - s.AiMs) / maxMs * GraphHeight), 0, frameRows - aiRows) : 0;
				Color32 top = s.FrameMs > 33.4f ? slow : frame;
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
