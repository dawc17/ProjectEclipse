using UnityEngine;

namespace Eclipse.UI
{
    public static class AccessibilitySettings
    {
        public static float CriticalPause { get { return Mathf.Clamp01(PlayerPrefs.GetFloat("Eclipse.CriticalPause", .8f)); } }
        public static float CriticalShake { get { return Mathf.Clamp01(PlayerPrefs.GetFloat("Eclipse.CriticalShake", .65f)); } }
        public static void SetCriticalPause(float value) { PlayerPrefs.SetFloat("Eclipse.CriticalPause", Mathf.Clamp01(value)); PlayerPrefs.Save(); }
        public static void SetCriticalShake(float value) { PlayerPrefs.SetFloat("Eclipse.CriticalShake", Mathf.Clamp01(value)); PlayerPrefs.Save(); }
    }
}
