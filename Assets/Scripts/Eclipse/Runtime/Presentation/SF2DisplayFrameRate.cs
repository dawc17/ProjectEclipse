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

	private const string AntiAliasingPlayerPref = "SF2.AntiAliasing";

	private const string BackgroundDepthPlayerPref = "SF2.BackgroundDepth";

	public const bool DefaultBackgroundDepthEnabled = false;

	// Exponent applied to background layer factors (factor^(1+strength)).
	// Always <= the authored factor, so layers never travel further than their art allows.
	public const float BackgroundDepthStrength = 0.6f;

	private static bool _backgroundDepthEnabled;

	// MSAA sample counts offered by the settings UIs (0 = off).
	public static readonly int[] AntiAliasingOptions = { 0, 2, 4, 8 };

	private static int _antiAliasing;

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

	// Desktop defaults to 4x so the flat silhouettes and limb edges stay smooth
	// at high resolutions; mobile keeps its quality level's own setting.
	public static int DefaultAntiAliasing
	{
		get { return Application.isMobilePlatform ? QualitySettings.antiAliasing : 4; }
	}

	public static bool BackgroundDepthEnabled
	{
		get
		{
			LoadSettings();
			return _backgroundDepthEnabled;
		}
	}

	public static void SetBackgroundDepthEnabled(bool enabled)
	{
		LoadSettings();
		_backgroundDepthEnabled = enabled;
		PlayerPrefs.SetInt(BackgroundDepthPlayerPref, enabled ? 1 : 0);
	}

	public static void ToggleBackgroundDepth()
	{
		SetBackgroundDepthEnabled(!BackgroundDepthEnabled);
	}

	// Spreads background layers further apart in depth: far layers (small
	// factors) move proportionally less than near ones. Screen-fixed (0),
	// camera-locked (>= 1) and game/foreground layers are left as authored.
	public static float BackgroundLayerFactor(float factor)
	{
		if (!BackgroundDepthEnabled || factor <= 0f || factor >= 1f) return factor;
		return Mathf.Pow(factor, 1f + BackgroundDepthStrength);
	}

	public static int AntiAliasing
	{
		get
		{
			LoadSettings();
			return _antiAliasing;
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
		QualitySettings.antiAliasing = _antiAliasing;
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

	public static void SetAntiAliasing(int samples)
	{
		LoadSettings();
		_antiAliasing = NormalizeAntiAliasing(samples);
		PlayerPrefs.SetInt(AntiAliasingPlayerPref, _antiAliasing);
		QualitySettings.antiAliasing = _antiAliasing;
	}

	public static void CycleAntiAliasing()
	{
		int index = System.Array.IndexOf(AntiAliasingOptions, AntiAliasing);
		SetAntiAliasing(AntiAliasingOptions[(index + 1) % AntiAliasingOptions.Length]);
	}

	public static string AntiAliasingLabel(int samples)
	{
		return samples <= 0 ? "Off" : samples + "x";
	}

	private static int NormalizeAntiAliasing(int samples)
	{
		return samples >= 8 ? 8 : samples >= 4 ? 4 : samples >= 2 ? 2 : 0;
	}

	public static void ResetRenderSettings()
	{
		_settingsLoaded = true;
		_interpolationEnabled = DefaultInterpolationEnabled;
		_maxFrameRate = DefaultMaxFrameRate;
		_motionBlurEnabled = DefaultMotionBlurEnabled;
		_antiAliasing = NormalizeAntiAliasing(DefaultAntiAliasing);
		_backgroundDepthEnabled = DefaultBackgroundDepthEnabled;
		PlayerPrefs.DeleteKey(AntiAliasingPlayerPref);
		PlayerPrefs.DeleteKey(BackgroundDepthPlayerPref);
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
		_antiAliasing = NormalizeAntiAliasing(PlayerPrefs.GetInt(AntiAliasingPlayerPref, DefaultAntiAliasing));
		_backgroundDepthEnabled = PlayerPrefs.GetInt(BackgroundDepthPlayerPref, DefaultBackgroundDepthEnabled ? 1 : 0) != 0;
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
