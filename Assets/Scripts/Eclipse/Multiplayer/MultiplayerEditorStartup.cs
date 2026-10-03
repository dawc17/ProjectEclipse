#if UNITY_EDITOR
using System.Collections;
using Eclipse.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Eclipse.Multiplayer
{
    /// <summary>Boots tagged editor clients into the existing versus workflow.</summary>
    public sealed class MultiplayerEditorStartup : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Boot()
        {
            if (EditorPlayModeContext.Role == null) return;
            var host = new GameObject("Eclipse Multiplayer Editor Startup");
            DontDestroyOnLoad(host);
            host.AddComponent<MultiplayerEditorStartup>();
        }

        private IEnumerator Start()
        {
            Application.runInBackground = true;
            float deadline = Time.realtimeSinceStartup + 120f;
            while (!Eclipse.UI.TitleScreen.IsOpen && Time.realtimeSinceStartup < deadline) yield return null;
            var title = Object.FindFirstObjectByType<Eclipse.UI.TitleScreen>();
            if (title == null) { Fail("The title screen did not load."); yield break; }
            title.EnterMultiplayerForTesting();
            while (!LocalVersusSession.IsReady && Time.realtimeSinceStartup < deadline) yield return null;
            if (!LocalVersusSession.IsReady) { Fail("Versus data did not finish loading."); yield break; }
            var role = EditorPlayModeContext.Role;
            try
            {
                if (role == EditorPlayModeContext.Host) OnlineVersusSession.Host("Editor Host", 7311);
                else if (role == EditorPlayModeContext.Guest) OnlineVersusSession.Join("Editor Guest", "127.0.0.1:7311");
                else LocalVersusMenu.Ensure().ShowOnlineHome();
            }
            catch (System.Exception error) { Fail(role + ": " + error.Message); yield break; }
            Debug.Log("[EclipseMPPM] " + role + " ready; writable data: " + EditorPlayModeContext.PersistentDataPath);
            if (!Application.isBatchMode && InputSystem.devices.Count == 0)
                Debug.LogWarning("[EclipseMPPM] No Input System devices detected. Fully quit and reopen Unity after changing Active Input Handling; stopping and restarting Play does not initialize the native input backend.");
            Destroy(gameObject);
        }

        private void Fail(string message)
        {
            Debug.LogError("[EclipseMPPM] " + message);
            Destroy(gameObject);
        }
    }
}
#endif
