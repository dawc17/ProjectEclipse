// Historical native fixtures test untagged sessions without installing MPPM.
// Real tagged-client behavior is covered by TestUnity6Workflows.ps1.
namespace Eclipse.Runtime
{
    public static class EditorPlayModeContext
    {
        public static string Role => null;
        public static string PersistentDataPath => UnityEngine.Application.persistentDataPath;
    }
}
