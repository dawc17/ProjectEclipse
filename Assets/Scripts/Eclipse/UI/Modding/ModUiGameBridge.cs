using Eclipse.Modding;
using UnityEngine;

namespace Eclipse.UI.Modding
{
    [DefaultExecutionOrder(-10000)]
    public sealed class ModUiGameBridge : MonoBehaviour
    {
        private static ModUiGameBridge current;
        private static bool nativeBlocked;
        private static int consumedFrame = -1;
        private static int backHandledFrame = -1;
        private ModUiCoordinator coordinator;
        private bool capturing;
        private bool waitForNeutral;
        private int direction;
        private int horizontalDirection;
        private float horizontalRepeatAt;
        private float repeatAt;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetSession()
        { current = null; nativeBlocked = false; consumedFrame = backHandledFrame = -1; }

        public static bool BlocksGameplayInput => consumedFrame == Time.frameCount ||
            (current != null && current.coordinator != null && current.coordinator.CapturesInput);

        internal static bool NativeInputBlocked => nativeBlocked || TitleScreen.IsOpen || GameSessionRestart.IsRestarting;

        public static void Attach(ModUiSurface surface)
        {
            if (surface == null || surface.IsClosed || surface.IsMounted)
                throw new System.ArgumentException("An open, unmounted UI surface is required.");
            if (current == null)
            {
                var coordinator = ModUiCoordinator.Create();
                current = coordinator.gameObject.AddComponent<ModUiGameBridge>();
                current.coordinator = coordinator;
            }
            current.RefreshNativeBlock();
            current.coordinator.Attach(surface);
        }

        public static void SetNativeBlocked(bool value)
        {
            nativeBlocked = value;
            if (current != null) current.RefreshNativeBlock();
        }

        private void RefreshNativeBlock()
        {
            if (coordinator != null)
                coordinator.SetNativeBlocked(NativeInputBlocked);
        }

        public static bool TryHandleBack()
        {
            if (backHandledFrame == Time.frameCount) return true;
            if (current == null) return consumedFrame == Time.frameCount;
            current.RefreshNativeBlock();
            if (!current.coordinator.CapturesInput) return consumedFrame == Time.frameCount;
            consumedFrame = Time.frameCount;
            backHandledFrame = Time.frameCount;
            return current.coordinator.Back();
        }

        // Also used by the input fixture; actual device polling remains below.
        public static bool Route(int move, bool submit, bool back, int adjust = 0)
        {
            if (current == null) return false;
            current.RefreshNativeBlock();
            if (!current.coordinator.CapturesInput) return false;
            consumedFrame = Time.frameCount;
            if (back)
            {
                if (backHandledFrame != Time.frameCount)
                { backHandledFrame = Time.frameCount; current.coordinator.Back(); }
            }
            else if (submit) current.coordinator.ActivateSelected();
            else if (move != 0) current.coordinator.MoveFocus(move);
            else if (adjust != 0) current.coordinator.AdjustSelected(adjust);
            return true;
        }

        private void Update()
        {
            RefreshNativeBlock();
            bool now = coordinator.CapturesInput;
            if (!now) { capturing = false; direction = horizontalDirection = 0; return; }
            consumedFrame = Time.frameCount;
            if (!capturing) { capturing = true; waitForNeutral = true; direction = horizontalDirection = 0; }
            var pad = GamePad.CNNMBBLLGNE(GamePad.LCNPGEANNDP.Dpad, GamePad.GGAKHLLMPMM.One, true);
            var stick = GamePad.CNNMBBLLGNE(GamePad.LCNPGEANNDP.LeftStick, GamePad.GGAKHLLMPMM.One, true);
            float vertical = Mathf.Abs(pad.y) > .5f ? pad.y : stick.y;
            float horizontal = Mathf.Abs(pad.x) > .5f ? pad.x : stick.x;
            int nextHorizontal = UnityEngine.Input.GetKey(KeyCode.LeftArrow) ? -1 : UnityEngine.Input.GetKey(KeyCode.RightArrow) ? 1 :
                horizontal > .5f ? 1 : horizontal < -.5f ? -1 : 0;
            bool down = UnityEngine.Input.GetKey(KeyCode.DownArrow) || UnityEngine.Input.GetKey(KeyCode.Tab);
            bool up = UnityEngine.Input.GetKey(KeyCode.UpArrow);
            int next = down ? 1 : up ? -1 : vertical > .5f ? -1 : vertical < -.5f ? 1 : 0;
            bool submitHeld = UnityEngine.Input.GetKey(KeyCode.Return) || UnityEngine.Input.GetKey(KeyCode.Space) ||
                GamePad.NFCGBMHPKMA(GamePad.PFENLAPGKFM.A, GamePad.GGAKHLLMPMM.One);
            bool backHeld = UnityEngine.Input.GetKey(KeyCode.Escape) ||
                GamePad.NFCGBMHPKMA(GamePad.PFENLAPGKFM.B, GamePad.GGAKHLLMPMM.One);
            if (waitForNeutral)
            {
                if (next == 0 && nextHorizontal == 0 && !submitHeld && !backHeld) waitForNeutral = false;
                return;
            }
            bool submit = UnityEngine.Input.GetKeyDown(KeyCode.Return) || UnityEngine.Input.GetKeyDown(KeyCode.Space) ||
                GamePad.JAHEECFCLHN(GamePad.PFENLAPGKFM.A, GamePad.GGAKHLLMPMM.One);
            bool back = UnityEngine.Input.GetKeyDown(KeyCode.Escape) ||
                GamePad.JAHEECFCLHN(GamePad.PFENLAPGKFM.B, GamePad.GGAKHLLMPMM.One);
            int move = 0;
            if (next != 0 && (next != direction || Time.unscaledTime >= repeatAt))
            { move = next; repeatAt = Time.unscaledTime + (next != direction ? .35f : .1f); }
            direction = next;
            int adjust = 0;
            if (nextHorizontal != 0 && (nextHorizontal != horizontalDirection || Time.unscaledTime >= horizontalRepeatAt))
            { adjust = nextHorizontal; horizontalRepeatAt = Time.unscaledTime + (nextHorizontal != horizontalDirection ? .35f : .1f); }
            horizontalDirection = nextHorizontal;
            Route(move, submit, back, adjust);
        }

        private void OnDestroy()
        {
            if (current == this) current = null;
        }
    }
}
