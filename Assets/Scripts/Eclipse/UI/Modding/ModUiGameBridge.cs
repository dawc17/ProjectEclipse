using Eclipse.Modding;
using UnityEngine;

namespace Eclipse.UI.Modding
{
    [DefaultExecutionOrder(-10000)]
    public sealed class ModUiGameBridge : MonoBehaviour
    {
        private static ModUiGameBridge current;
        private static bool nativeBlocked;
        private static readonly System.Collections.Generic.HashSet<object> presentationBlocks = new System.Collections.Generic.HashSet<object>();
        public static System.IDisposable AcquirePresentationBlock()
        {
            var lease = new PresentationBlock(); presentationBlocks.Add(lease);
            if (current != null) current.RefreshNativeBlock();
            return lease;
        }
        private sealed class PresentationBlock : System.IDisposable
        {
            public void Dispose() { presentationBlocks.Remove(this); if (current != null) current.RefreshNativeBlock(); }
        }
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
        { current = null; nativeBlocked = false; presentationBlocks.Clear(); consumedFrame = backHandledFrame = -1; }

        public static bool BlocksGameplayInput => consumedFrame == Time.frameCount ||
            (current != null && current.coordinator != null && current.coordinator.CapturesInput);

        internal static bool NativeInputBlocked => nativeBlocked || presentationBlocks.Count != 0 || TitleScreen.IsOpen || GameSessionRestart.IsRestarting;

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
        public static bool Route(int move, bool submit, bool back, int adjust = 0, bool sequential = true)
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
            else if (move != 0)
            {
                if (sequential) current.coordinator.MoveFocus(move);
                else current.coordinator.NavigateFocus(0, move);
            }
            else if (adjust != 0) current.coordinator.NavigateFocus(adjust, 0);
            return true;
        }

        private void Update()
        {
            RefreshNativeBlock();
            bool now = coordinator.CapturesInput;
            if (!now) { capturing = false; direction = horizontalDirection = 0; return; }
            consumedFrame = Time.frameCount;
            if (!capturing) { capturing = true; waitForNeutral = true; direction = horizontalDirection = 0; }
            var pad = GamePad.GetStick(GamePad.Stick.Dpad, GamePad.Player.One, true);
            var stick = GamePad.GetStick(GamePad.Stick.LeftStick, GamePad.Player.One, true);
            float vertical = Mathf.Abs(pad.y) > .5f ? pad.y : stick.y;
            float horizontal = Mathf.Abs(pad.x) > .5f ? pad.x : stick.x;
            int nextHorizontal = UnityEngine.Input.GetKey(KeyCode.LeftArrow) ? -1 : UnityEngine.Input.GetKey(KeyCode.RightArrow) ? 1 :
                horizontal > .5f ? 1 : horizontal < -.5f ? -1 : 0;
            bool tab = UnityEngine.Input.GetKey(KeyCode.Tab);
            bool reverseTab = UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift);
            bool down = UnityEngine.Input.GetKey(KeyCode.DownArrow);
            bool up = UnityEngine.Input.GetKey(KeyCode.UpArrow);
            int next = tab ? (reverseTab ? -1 : 1) : down ? 1 : up ? -1 : vertical > .5f ? -1 : vertical < -.5f ? 1 : 0;
            bool submitHeld = UnityEngine.Input.GetKey(KeyCode.Return) || UnityEngine.Input.GetKey(KeyCode.Space) ||
                GamePad.GetButton(GamePad.Button.A, GamePad.Player.One);
            bool backHeld = UnityEngine.Input.GetKey(KeyCode.Escape) ||
                GamePad.GetButton(GamePad.Button.B, GamePad.Player.One);
            if (waitForNeutral)
            {
                if (next == 0 && nextHorizontal == 0 && !submitHeld && !backHeld) waitForNeutral = false;
                return;
            }
            bool submit = UnityEngine.Input.GetKeyDown(KeyCode.Return) || UnityEngine.Input.GetKeyDown(KeyCode.Space) ||
                GamePad.GetButtonDown(GamePad.Button.A, GamePad.Player.One);
            bool back = UnityEngine.Input.GetKeyDown(KeyCode.Escape) ||
                GamePad.GetButtonDown(GamePad.Button.B, GamePad.Player.One);
            int move = 0;
            if (next != 0 && (next != direction || Time.unscaledTime >= repeatAt))
            { move = next; repeatAt = Time.unscaledTime + (next != direction ? .35f : .1f); }
            direction = next;
            int adjust = 0;
            if (nextHorizontal != 0 && (nextHorizontal != horizontalDirection || Time.unscaledTime >= horizontalRepeatAt))
            { adjust = nextHorizontal; horizontalRepeatAt = Time.unscaledTime + (nextHorizontal != horizontalDirection ? .35f : .1f); }
            horizontalDirection = nextHorizontal;
            Route(move, submit, back, adjust, sequential: tab);
        }

        private void OnDestroy()
        {
            if (current == this) current = null;
        }
    }
}
