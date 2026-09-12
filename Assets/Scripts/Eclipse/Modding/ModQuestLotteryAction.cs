using System;
using Eclipse.UI.Modding;
using UnityEngine;

namespace Eclipse.Modding
{
    // Keeps quest completion separate from dismissing a scene-owned reward view.
    internal sealed class ModQuestLotteryAction : IDisposable
    {
        private readonly ModRuntime.LotteryClaim claim;
        private readonly Action complete;
        private readonly Module module;
        private ModLotteryDialog dialog;
        private bool disposed;

        internal ModQuestLotteryAction(QuestAction action, string fightName, Action complete)
            : this(ModRuntime.PrepareQuestLotteryClaim(action.NOFNJFOCIMK(), action.Index,
                fightName, Math.Min(UnityEngine.Random.value, 0.9999999999999999)), complete)
        { }

        internal ModQuestLotteryAction(ModRuntime.LotteryClaim claim, Action complete)
        {
            this.complete = complete ?? throw new ArgumentNullException(nameof(complete));
            this.claim = claim;
            module = Module.ELEBLBJKDBI();
            if (claim != null)
            {
                module.AddEventListener((int)Module.FKHIMIAOCJL.OnOpenScene, OnOpenScene);
                module.AddEventListener((int)Module.FKHIMIAOCJL.OnCloseScene, OnCloseScene);
            }
        }

        internal void Show()
        {
            if (disposed) return;
            if (claim == null) { Finish(); return; }
            if (!claim.IsCurrent) { Dispose(); return; }
            if (dialog != null && !dialog.Surface.IsClosed) return;
            if (module.NMCNDOPKFJD() == ScreenType.ModuleFight) return;
            dialog = new ModLotteryDialog(LocalizationManager.GetStringOrDefault("ClanRewardTxt", "Reward"), claim.PreviewText, claim.TryClaim,
                Finish, _ => { }, error => Debug.LogException(error),
                (key, fallback) => LocalizationManager.GetStringOrDefault(key, fallback), ModRuntime.ResolveLotteryArtwork(claim));
            dialog.Show();
        }

        private void OnOpenScene(object _) { if (!disposed) Show(); }
        private void OnCloseScene(object _) { dialog?.Dispose(); }

        private void Finish()
        {
            if (disposed) return;
            Dispose();
            complete();
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            module.RemoveEventListener((int)Module.FKHIMIAOCJL.OnOpenScene, OnOpenScene);
            module.RemoveEventListener((int)Module.FKHIMIAOCJL.OnCloseScene, OnCloseScene);
            dialog?.Dispose();
        }
    }
}
