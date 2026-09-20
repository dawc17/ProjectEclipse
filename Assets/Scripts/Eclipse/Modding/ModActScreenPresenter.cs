using System;
using System.Collections.Generic;
using Eclipse.UI.Modding;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eclipse.Modding
{
    public sealed class ModActScreenPresenter : MonoBehaviour, IDisposable
    {
        private static ModActScreenPresenter active;
        private EnterScreen screen;
        private IDisposable moduleLock, uiLock;
        private Action<bool> finished;
        private Func<bool> valid;
        private Scene scene;
        private bool disposed;

        public static void CancelActive() { if (active != null) active.Dispose(); }

        public static IDisposable TryOpen(IReadOnlyList<ModActScreenLine> lines, Action<bool> finished, Func<bool> valid)
        {
            if (active != null || !valid()) return null;
            if (lines == null || lines.Count < 1 || lines.Count > 32) throw new ArgumentException("Act screens require 1..32 lines.");
            var native = new List<KeyValuePair<string, int>>();
            int total = 0;
            foreach (var line in lines)
            {
                if (line == null) throw new ArgumentException("Null act-screen line.");
                total += line.Frames;
                if (total > 7200) throw new ArgumentException("Act-screen text duration exceeds 7200 frames.");
                native.Add(new KeyValuePair<string, int>(line.Text, line.Frames));
            }
            var screen = EnterScreen.Create();
            var owner = screen.gameObject.AddComponent<ModActScreenPresenter>();
            active = owner;
            owner.screen = screen; owner.valid = valid; owner.finished = finished;
            owner.scene = SceneManager.GetActiveScene();
            try
            {
                owner.moduleLock = Module.GetInstance().AcquirePresentationLock();
                owner.uiLock = ModUiGameBridge.AcquirePresentationBlock();
                screen.InitResolved(native, () => owner.Finish(owner.IsValid));
                return owner;
            }
            catch { owner.Dispose(); throw; }
        }

        private bool IsValid => !disposed && valid != null && valid() && SceneManager.GetActiveScene() == scene;
        private void Update() { if (!IsValid) Dispose(); }
        private void OnDestroy() { Dispose(); }
        private void OnDisable() { Dispose(); }
        public void Dispose() { Finish(false); }
        private void Finish(bool completed)
        {
            if (disposed) return;
            disposed = true;
            if (active == this) active = null;
            var notify = finished; finished = null; valid = null;
            try { if (screen != null) screen.Cancel(); }
            finally
            {
                moduleLock?.Dispose(); moduleLock = null;
                uiLock?.Dispose(); uiLock = null;
            }
            notify?.Invoke(completed);
        }
    }
}
