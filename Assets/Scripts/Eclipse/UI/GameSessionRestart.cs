using System;
using Eclipse.Modding;
using UnityEngine;

namespace Eclipse.UI
{
    public static class GameSessionRestart
    {
        public static bool IsRestarting { get; private set; }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void ArrivedAtTitle() { IsRestarting = false; }

        public static bool TryRestart(Action savePreferences, out string error)
        {
            error = null;
            if (IsRestarting) return false;
            try
            {
                // Preserve the old content context until the native save has completed.
                ListSF.CCDKHLAMKKO()?.GGGEHAGCLGC();
                savePreferences?.Invoke();
                PlayerPrefs.Save();
                IsRestarting = true;
                Time.timeScale = 1f;
                AudioListener.pause = false;
                TitleScreen.PrepareForRestart();
                // Stop/reset runs in the new preloader, after old scene objects are gone.
                SceneManagerSF.Load(ScreenType.ModulePreloader);
                return true;
            }
            catch (Exception exception)
            {
                IsRestarting = false;
                error = "Could not restart: " + exception.Message;
                Debug.LogException(exception);
                return false;
            }
        }
    }
}
