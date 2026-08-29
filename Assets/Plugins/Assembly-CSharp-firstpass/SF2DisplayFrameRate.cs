using UnityEngine;

public static class SF2DisplayFrameRate
{
	public static void Apply()
	{
		// Desktop rendering follows the active display/v-sync setting instead of a
		// hard 60 FPS cap. Mobile needs an explicit target, otherwise Unity may
		// choose 30 FPS when targetFrameRate is -1.
		Application.targetFrameRate = Application.isMobilePlatform ? GetRefreshRate() : -1;
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
