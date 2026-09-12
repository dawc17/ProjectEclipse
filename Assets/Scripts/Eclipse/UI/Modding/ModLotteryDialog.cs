using System;
using Eclipse.Modding;

namespace Eclipse.UI.Modding
{
    // Presentation lifecycle only. The caller owns persisted claim and quest identity.
    internal sealed class ModLotteryDialog : IDisposable
    {
        private readonly ModUiScope scope;
        private bool claimed;
        private bool claiming;
        private bool notified;
        private ModUiCloseReason closeReason;
        private readonly Action completed;
        private readonly Action<ModUiCloseReason> deferred;
        internal ModUiSurface Surface { get; }

        internal ModLotteryDialog(string title, string rewardText, Func<bool> claim,
            Action completed, Action<ModUiCloseReason> deferred, Action<Exception> report,
            Func<string, string, string> localize = null, AssetId? artwork = null)
        {
            if (claim == null) throw new ArgumentNullException(nameof(claim));
            if (completed == null) throw new ArgumentNullException(nameof(completed));
            this.completed = completed;
            this.deferred = deferred;
            localize = localize ?? ((key, fallback) => fallback);
            scope = new ModUiScope(ModId.Parse("core"), report);
            int lines = 0;
            foreach (string line in (rewardText ?? string.Empty).Split('\n')) lines += Math.Max(1, (line.Length + 31) / 32);
            int textHeight = Math.Min(7800, Math.Max(64, lines * 32));
            var body = new ModUiNode("reward_text", ModUiKind.Text, 548, textHeight, text: rewardText,
                style: new ModUiStyle(fontSize: 24, textAlign: "center"));
            ModUiNode rewardContent = body;
            if (artwork.HasValue)
                rewardContent = new ModUiNode("reward_content", ModUiKind.Column, 548, textHeight + 168, gap: 8, children: new[] {
                    new ModUiNode("reward_art", ModUiKind.Image, 548, 160, sprite: artwork.Value), body
                });
            var root = new ModUiNode("lottery", ModUiKind.Column, 600, 480, gap: 8, children: new[] {
                new ModUiNode("title", ModUiKind.Text, 560, 54, text: title,
                    style: new ModUiStyle(fontSize: 32, textAlign: "center")),
                new ModUiNode("reward", ModUiKind.Scroll, 560, 240, children: new[] { rewardContent }),
                new ModUiNode("status", ModUiKind.Text, 560, 58, text: localize("eclipse.lottery.saved", "Your reward is saved until you claim it."),
                    style: new ModUiStyle(fontSize: 20, textAlign: "center")),
                new ModUiNode("actions", ModUiKind.Row, 560, 64, gap: 16, children: new[] {
                    new ModUiNode("claim", ModUiKind.Button, 264, 56, text: localize("btnObtain", "CLAIM")),
                    new ModUiNode("later", ModUiKind.Button, 264, 56, text: localize("dlgLaterBtn", "LATER"))
                })
            });
            Surface = scope.Open("lottery", ModUiMount.Modal, root, id => {
                if (id == "later") { Surface.Close(ModUiCloseReason.Back); return; }
                if (id != "claim" || claimed) return;
                Surface.SetEnabled("claim", false);
                claiming = true;
                try
                {
                    if (!claim())
                    {
                        if (!Surface.IsClosed) Surface.SetText("status", localize("eclipse.lottery.unavailable", "This reward is no longer available. Close and reopen the lottery."));
                        return;
                    }
                    claimed = true;
                    Surface.Close();
                }
                catch (Exception error)
                {
                    if (!Surface.IsClosed) Surface.SetText("status", localize("eclipse.lottery.failed", "The reward could not be saved. Reload your profile before continuing."));
                    report?.Invoke(error);
                }
                finally
                {
                    claiming = false;
                    if (Surface.IsClosed) NotifyClosed();
                }
            }, onClose: reason => {
                closeReason = reason;
                scope.Dispose();
                if (!claiming) NotifyClosed();
            });
        }

        private void NotifyClosed()
        {
            if (notified) return;
            notified = true;
            if (claimed) completed();
            else deferred?.Invoke(closeReason);
        }

        internal void Show() => ModUiGameBridge.Attach(Surface);
        public void Dispose() => Surface.Close(ModUiCloseReason.Destroyed);
    }
}
