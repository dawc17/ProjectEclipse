using System;
using System.Collections.Generic;
using Nekki.SF2.GUI.Dialogs;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eclipse.Modding
{
    // Presents a mod request through the native StoryDialog used by quest
    // Dialog Type="Regular". The native dialog lives on a DontDestroyOnLoad canvas,
    // so this owner closes it itself when the scene or profile it belongs to ends.
    public sealed class ModStoryDialogPresenter : MonoBehaviour, IDisposable
    {
        private static ModStoryDialogPresenter active;
        private BaseDialog dialog;
        private Action<bool> finished;
        private Func<bool> valid;
        private Scene scene;
        private bool done;

        public static void CancelActive() { if (active != null) active.Dispose(); }

        public static IDisposable TryOpen(ModStoryDialogRequest request, Action<bool> finished, Func<bool> valid)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (active != null || !valid()) return null;
            var content = new List<StoryDialogContent>();
            foreach (var line in request.Lines)
            {
                var entry = new StoryDialogContent(string.Empty, string.Empty, string.Empty, string.Empty);
                entry.GGDJIPKMKFC = line.Text;
                entry.AJELOOEBCPO = line.Button;
                entry.NGEPEDCCMAI = StoryDialogContent.MFHMNFAPAOH.CONTENT_TYPE_REGULAR;
                content.Add(entry);
            }
            var host = new GameObject("Mod story dialog").AddComponent<ModStoryDialogPresenter>();
            DontDestroyOnLoad(host.gameObject);
            active = host;
            host.finished = finished; host.valid = valid; host.scene = SceneManager.GetActiveScene();
            try
            {
                // Same arguments as QuestActionDialog's Regular branch with one Right button.
                string image = request.Portrait + (request.Mirrored ? "|Flip" : string.Empty);
                // An empty portrait hides the native portrait (quest dialogs without Image).
                host.dialog = DialogsOpener.EHMEIJCOOKP(image, request.Title, content, host.OnNativeClose, "CANCEL", false,
                    request.Button, LabelButton.GetBtnColor("Beige"), LabelButton.GetBtnColor("Red"), request.Portrait.Length != 0);
                if (host.dialog == null) { host.Finish(false, false); return null; }
                host.dialog.IsIgnoreBack = request.IgnoreBack;
                host.dialog.IsQuestDialog = true;
                return host;
            }
            catch { host.Finish(false, true); throw; }
        }

        private void OnNativeClose(object data) { Finish(true, false); }

        private void Update()
        {
            if (done) return;
            if (dialog == null || !dialog.gameObject.activeInHierarchy) { Finish(false, false); return; }
            if (valid == null || !valid() || SceneManager.GetActiveScene() != scene) Dispose();
        }

        private void OnDestroy() { Finish(false, true); }
        public void Dispose() { Finish(false, true); }

        private void Finish(bool acknowledged, bool closeNative)
        {
            if (done) return;
            done = true;
            if (active == this) active = null;
            var notify = finished; finished = null; valid = null;
            try
            {
                // Closing reports through OnNativeClose, which is ignored once done.
                if (closeNative && dialog != null && dialog.gameObject.activeInHierarchy) dialog.OnClose(null);
            }
            finally
            {
                dialog = null;
                if (this != null) Destroy(gameObject);
                notify?.Invoke(acknowledged);
            }
        }
    }
}
