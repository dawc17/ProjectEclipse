using UnityEngine;

namespace Eclipse.UI
{
    // Home is replaced by these pages while the release check runs or after it finds a newer build.
    public sealed partial class TitleScreen
    {
        // Usually resolved during the splash; otherwise this shows until the check finishes or times out.
        private void DrawReleaseCheck()
        {
            Clear("Checking");
            Label(page, "Checking for updates…", 100, 300, 1080, 70, 36, Ink, TextAnchor.MiddleCenter);
            FocusFirst();
        }

        private void DrawOutdated()
        {
            Clear("Outdated");
            Label(page, "This build is out of date", 100, 200, 1080, 70, 46, Ink);
            Label(page, "Eclipse " + ReleaseCheck.Installed + " is installed, but " + ReleaseCheck.Latest +
                " is available on the " + ReleaseCheck.Channel + " channel. Update through the Eclipse Launcher to keep playing.",
                100, 290, 1080, 100, 26, Ink);
            if (ReleaseCheck.FromLauncher)
                Button(page, "Update in launcher", 100, 440, 470, 64, () => QuitWith(ReleaseCheck.OutdatedExitCode));
            else if (ReleaseCheck.LauncherRoot != null)
                Button(page, "Open launcher", 100, 440, 470, 64, () =>
                {
                    if (ReleaseCheck.OpenLauncher()) QuitWith(0);
                    else Application.OpenURL(ReleaseCheck.ReleasesPage);
                });
            else
                Button(page, "Get the launcher", 100, 440, 470, 64, () => Application.OpenURL(ReleaseCheck.ReleasesPage));
            Button(page, "Quit game", 650, 440, 470, 64, () => QuitWith(0), UiSound.Back);
            FocusFirst();
        }

        private static void QuitWith(int exitCode)
        {
            PlayerPrefs.Save();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit(exitCode);
#endif
        }
    }
}
