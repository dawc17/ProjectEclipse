using UnityEngine;
using UnityEngine.SceneManagement;

public static class SF2DisplayFrameRate
{
	public const bool DefaultInterpolationEnabled = true;

	public const bool DefaultMotionBlurEnabled = false;

	public const float DefaultMotionBlurStrength = 0.35f;

	// 0 = follow the display/v-sync behavior. Set 120/144/165/240/etc for a hard cap.
	public const int DefaultMaxFrameRate = 0;

	private const string InterpolationPlayerPref = "SF2.RenderInterpolation";

	private const string MaxFrameRatePlayerPref = "SF2.MaxFrameRate";

	private const string MotionBlurPlayerPref = "SF2.MotionBlur";

	private static bool _settingsLoaded;

	private static bool _interpolationEnabled;

	private static int _maxFrameRate;

	private static bool _motionBlurEnabled;

	private static int _vSyncCountBeforeCustomCap = -1;

	public static bool InterpolationEnabled
	{
		get
		{
			LoadSettings();
			return _interpolationEnabled;
		}
	}

	public static int MaxFrameRate
	{
		get
		{
			LoadSettings();
			return _maxFrameRate;
		}
	}

	public static bool MotionBlurEnabled
	{
		get
		{
			LoadSettings();
			return _motionBlurEnabled;
		}
	}

	public static float MotionBlurStrength
	{
		get
		{
			return DefaultMotionBlurStrength;
		}
	}

	public static void Apply()
	{
		LoadSettings();
		SF2MotionBlur.EnsureOnMainCamera();
		if (_maxFrameRate > 0)
		{
			if (_vSyncCountBeforeCustomCap < 0)
			{
				_vSyncCountBeforeCustomCap = QualitySettings.vSyncCount;
			}
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = _maxFrameRate;
			return;
		}
		if (_vSyncCountBeforeCustomCap >= 0)
		{
			QualitySettings.vSyncCount = _vSyncCountBeforeCustomCap;
			_vSyncCountBeforeCustomCap = -1;
		}
		// Desktop rendering follows the active display/v-sync setting instead of a
		// hard 60 FPS cap. Mobile needs an explicit target, otherwise Unity may
		// choose 30 FPS when targetFrameRate is -1.
		Application.targetFrameRate = Application.isMobilePlatform ? GetRefreshRate() : -1;
	}

	public static void SetInterpolationEnabled(bool enabled)
	{
		LoadSettings();
		_interpolationEnabled = enabled;
		PlayerPrefs.SetInt(InterpolationPlayerPref, enabled ? 1 : 0);
	}

	public static void ToggleInterpolation()
	{
		SetInterpolationEnabled(!InterpolationEnabled);
	}

	public static void SetMaxFrameRate(int maxFrameRate)
	{
		LoadSettings();
		_maxFrameRate = Mathf.Max(0, maxFrameRate);
		PlayerPrefs.SetInt(MaxFrameRatePlayerPref, _maxFrameRate);
		Apply();
	}

	public static void SetMotionBlurEnabled(bool enabled)
	{
		LoadSettings();
		_motionBlurEnabled = enabled;
		PlayerPrefs.SetInt(MotionBlurPlayerPref, enabled ? 1 : 0);
		SF2MotionBlur.EnsureOnMainCamera();
	}

	public static void ToggleMotionBlur()
	{
		SetMotionBlurEnabled(!MotionBlurEnabled);
	}

	public static void ResetRenderSettings()
	{
		_settingsLoaded = true;
		_interpolationEnabled = DefaultInterpolationEnabled;
		_maxFrameRate = DefaultMaxFrameRate;
		_motionBlurEnabled = DefaultMotionBlurEnabled;
		PlayerPrefs.DeleteKey(InterpolationPlayerPref);
		PlayerPrefs.DeleteKey(MaxFrameRatePlayerPref);
		PlayerPrefs.DeleteKey(MotionBlurPlayerPref);
		Apply();
	}

	private static void LoadSettings()
	{
		if (_settingsLoaded)
		{
			return;
		}
		_interpolationEnabled = PlayerPrefs.GetInt(InterpolationPlayerPref, DefaultInterpolationEnabled ? 1 : 0) != 0;
		_maxFrameRate = Mathf.Max(0, PlayerPrefs.GetInt(MaxFrameRatePlayerPref, DefaultMaxFrameRate));
		_motionBlurEnabled = PlayerPrefs.GetInt(MotionBlurPlayerPref, DefaultMotionBlurEnabled ? 1 : 0) != 0;
		_settingsLoaded = true;
	}

	public static int GetRefreshRate()
	{
		double refreshRate = Screen.currentResolution.refreshRateRatio.value;
		if (double.IsNaN(refreshRate) || double.IsInfinity(refreshRate) || refreshRate < 60.0)
		{
			return 60;
		}
		return Mathf.RoundToInt((float)refreshRate);
	}
}

// Lightweight temporal motion blur for the recovered built-in render pipeline.
// This is presentation-only: it blends completed camera frames and never feeds
// data back into the fixed-step fight simulation.
[RequireComponent(typeof(Camera))]
public sealed class SF2MotionBlur : MonoBehaviour
{
	private const string ShaderName = "Hidden/SF2/FrameBlendMotionBlur";

	private Material _material;

	private RenderTexture _history;

	private bool _historyValid;

	private int _historyWidth;

	private int _historyHeight;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void RegisterSceneHook()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		EnsureOnMainCamera();
	}

	public static void EnsureOnMainCamera()
	{
		string sceneName = SceneManager.GetActiveScene().name;
		if (sceneName != "Fight" && sceneName != "Dojo")
		{
			return;
		}
		Camera camera = Camera.main;
		if (camera != null && camera.GetComponent<SF2MotionBlur>() == null)
		{
			camera.gameObject.AddComponent<SF2MotionBlur>();
		}
	}

	private void OnEnable()
	{
		_historyValid = false;
	}

	private void OnDisable()
	{
		ReleaseResources();
	}

	private void OnDestroy()
	{
		ReleaseResources();
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!SF2DisplayFrameRate.MotionBlurEnabled || SF2DisplayFrameRate.MotionBlurStrength <= 0.001f)
		{
			Graphics.Blit(source, destination);
			_historyValid = false;
			return;
		}

		if (!EnsureResources(source))
		{
			Graphics.Blit(source, destination);
			return;
		}

		float deltaTime = Time.unscaledDeltaTime;
		if (!_historyValid || deltaTime <= 0f || deltaTime > 0.1f)
		{
			Graphics.Blit(source, destination);
			Graphics.Blit(source, _history);
			_historyValid = true;
			return;
		}

		// Convert the user-facing strength to an exposure/decay time. Using time
		// rather than a fixed per-frame blend keeps the appearance stable as the
		// display rate changes.
		float shutterSeconds = Mathf.Lerp(1f / 480f, 1f / 60f, SF2DisplayFrameRate.MotionBlurStrength);
		float historyWeight = Mathf.Exp(-deltaTime / shutterSeconds);
			_material.SetTexture("_HistoryTex", _history);
			_material.SetFloat("_HistoryWeight", Mathf.Clamp01(historyWeight));
			Graphics.Blit(source, destination, _material);

			// Keep an unblurred previous camera frame. Besides avoiding recursive
			// trails, using the image-effect source keeps history orientation
			// consistent across render-target/back-buffer transitions.
			Graphics.Blit(source, _history);
		}

	private bool EnsureResources(RenderTexture source)
	{
		if (_material == null)
		{
			Shader shader = Resources.Load<Shader>("shaders/SF2FrameBlendMotionBlur");
			if (shader == null)
			{
				shader = Shader.Find(ShaderName);
			}
			if (shader == null || !shader.isSupported)
			{
				return false;
			}
			_material = new Material(shader);
			_material.hideFlags = HideFlags.HideAndDontSave;
		}

		if (_history == null || _historyWidth != source.width || _historyHeight != source.height)
		{
			ReleaseHistory();
			_history = new RenderTexture(source.width, source.height, 0, source.format);
			_history.name = "SF2 Motion Blur History";
			_history.hideFlags = HideFlags.HideAndDontSave;
			_history.filterMode = FilterMode.Bilinear;
			_history.wrapMode = TextureWrapMode.Clamp;
			_history.Create();
			_historyWidth = source.width;
			_historyHeight = source.height;
			_historyValid = false;
		}

		return _history != null && _history.IsCreated();
	}

	private void ReleaseResources()
	{
		ReleaseHistory();
		if (_material != null)
		{
			DestroyRuntimeObject(_material);
			_material = null;
		}
	}

	private void ReleaseHistory()
	{
		if (_history != null)
		{
			_history.Release();
			DestroyRuntimeObject(_history);
			_history = null;
		}
		_historyWidth = 0;
		_historyHeight = 0;
		_historyValid = false;
	}

	private static void DestroyRuntimeObject(UnityEngine.Object value)
	{
		if (value == null)
		{
			return;
		}
		if (Application.isPlaying)
		{
			UnityEngine.Object.Destroy(value);
		}
		else
		{
			UnityEngine.Object.DestroyImmediate(value);
		}
	}
}
