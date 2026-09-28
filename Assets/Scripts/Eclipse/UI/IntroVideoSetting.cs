using UnityEngine;

namespace Eclipse.UI
{
    // Whether the intro video plays when a save is entered (IntroModule). Off by default,
    // which keeps the behaviour Eclipse shipped with.
    public static class IntroVideoSetting
    {
        private const string Key = "Eclipse.IntroVideo";
        public static bool Enabled { get { return PlayerPrefs.GetInt(Key, 0) == 1; } }
        public static void Toggle() { PlayerPrefs.SetInt(Key, Enabled ? 0 : 1); PlayerPrefs.Save(); }
    }
}
