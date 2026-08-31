using System.IO;
using Eclipse.Content;
using UnityEditor;
using UnityEngine;

public static class LocalAnimationPreviewMenu
{
    private const string Menu = "SF2/Animation Preview/Enable Local Move";

    [MenuItem(Menu)]
    private static void Toggle()
    {
        string directory = LocalAnimationPreview.DirectoryPath;
        string enabled = Path.Combine(directory, "enabled");
        if (File.Exists(enabled)) File.Delete(enabled);
        else
        {
            if (!File.Exists(Path.Combine(directory, "Move.xml")))
            {
                Debug.LogWarning("Install a local move first; see Tools/Animation/InstallFf3Preview.py.");
                return;
            }
            File.WriteAllText(enabled, "Local animation preview enabled");
        }
        Debug.Log("[AnimationPreview] " + (File.Exists(enabled) ? "Enabled" : "Disabled") + "; takes effect on the next Play session.");
    }

    [MenuItem(Menu, true)]
    private static bool ValidateToggle()
    {
        UnityEditor.Menu.SetChecked(Menu, File.Exists(Path.Combine(LocalAnimationPreview.DirectoryPath, "enabled")));
        return !EditorApplication.isPlayingOrWillChangePlaymode;
    }
}
