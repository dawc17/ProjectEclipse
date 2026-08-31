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
}
