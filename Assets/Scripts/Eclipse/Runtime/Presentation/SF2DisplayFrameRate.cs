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
