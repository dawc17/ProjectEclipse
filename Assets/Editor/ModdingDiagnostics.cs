using Eclipse.Modding;
using UnityEditor;
using UnityEngine;

public static class ModdingDiagnostics
{
    [MenuItem("SF2/Modding/Validate Loose Mods")]
    public static void ValidateLooseMods()
    {
        ModHost host = ModHost.BuildDefault();
        if (host.HasErrors) Debug.LogWarning("[ModHost]\n" + host.FormatReport());
        else Debug.Log("[ModHost]\n" + host.FormatReport());
    }

    [MenuItem("SF2/Modding/Run Loose Mod Scripts")]
    public static void RunLooseModScripts()
    {
        using (ModHost host = ModHost.BuildDefault())
        using (ModScriptSession scripts = host.StartScripts(new MoonSharpScriptRuntime(), LogScript))
        {
            if (scripts.HasErrors) Debug.LogWarning("[ModScripts]\n" + scripts.FormatReport());
            else Debug.Log("[ModScripts]\n" + scripts.FormatReport());
        }
    }

    private static void LogScript(ModLogEntry entry)
    {
        string message = "[Mod:" + entry.ModId + "] " + entry.Message;
        if (entry.Level == ModLogLevel.Error) Debug.LogError(message);
        else if (entry.Level == ModLogLevel.Warning) Debug.LogWarning(message);
        else Debug.Log(message);
    }
}
