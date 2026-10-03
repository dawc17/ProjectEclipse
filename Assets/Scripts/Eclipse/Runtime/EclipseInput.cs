using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Eclipse.Input
{
    /// <summary>Input System device reads with the recovered KeyCode/save contract.</summary>
    public static class EclipseInput
    {
        private static readonly Gamepad[] Slots = new Gamepad[4];
        private static readonly int[] LegacyJoystickButtons = { 0, 1, 3, 2, 5, 4, 8, 9, 7, 6 };
        private static bool initialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset()
        {
            InputSystem.onDeviceChange -= DeviceChanged;
            Array.Clear(Slots, 0, Slots.Length);
            initialized = false;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Boot()
        {
            Initialize();
            if (!EnhancedTouchSupport.enabled) EnhancedTouchSupport.Enable();
        }

        private static void Initialize()
        {
            if (initialized) return;
            initialized = true;
            InputSystem.onDeviceChange += DeviceChanged;
            foreach (var pad in Gamepad.all) Assign(pad);
        }

        private static void Assign(Gamepad pad)
        {
            // Never compact slots: unplugging P1 must not turn P2 into P1.
            for (int i = 0; i < Slots.Length; i++) if (Slots[i] == pad) return;
            for (int i = 0; i < Slots.Length; i++)
                if (Slots[i] == null || !Slots[i].added) { Slots[i] = pad; return; }
        }

        private static void DeviceChanged(InputDevice device, InputDeviceChange change)
        {
            if (device is Gamepad pad && (change == InputDeviceChange.Added || change == InputDeviceChange.Reconnected)) Assign(pad);
        }

        private static Gamepad Pad(int player)
        {
            Initialize();
            int slot = player - 1;
            var pad = slot >= 0 && slot < Slots.Length ? Slots[slot] : null;
            return pad != null && pad.added && pad.enabled ? pad : null;
        }

        public static bool IsGamepadConnected(int player) => Pad(player) != null;
        public static string[] GetJoystickNames()
        {
            var names = new string[Slots.Length];
            for (int i = 0; i < names.Length; i++) names[i] = Pad(i + 1)?.displayName ?? string.Empty;
            return names;
        }

        public static Vector2 GetGamepadStick(int stick, int player, bool raw = false)
        {
            if (player == 0)
            {
                var strongest = Vector2.zero;
                for (int i = 1; i <= Slots.Length; i++)
                {
                    var value = GetGamepadStick(stick, i, raw);
                    if (value.sqrMagnitude > strongest.sqrMagnitude) strongest = value;
                }
                return strongest;
            }
            var pad = Pad(player);
            if (pad == null) return Vector2.zero;
            Vector2Control control = stick == 0 ? pad.leftStick : stick == 1 ? pad.rightStick : stick == 2 ? pad.dpad : null;
            return control == null ? Vector2.zero : raw ? control.ReadUnprocessedValue() : control.ReadValue();
        }

        public static float GetGamepadTrigger(int trigger, int player, bool raw = false)
        {
            if (player == 0)
            {
                float strongest = 0;
                for (int i = 1; i <= Slots.Length; i++) strongest = Mathf.Max(strongest, GetGamepadTrigger(trigger, i, raw));
                return strongest;
            }
            var pad = Pad(player);
            var control = pad == null ? null : trigger == 0 ? pad.leftTrigger : trigger == 1 ? pad.rightTrigger : null;
            return control == null ? 0 : raw ? control.ReadUnprocessedValue() : control.ReadValue();
        }

        // Semantic GamePad.Button order retained for existing saved bindings.
        private static ButtonControl Button(Gamepad pad, int button)
        {
            if (pad == null) return null;
            switch (button)
            {
                case 0: return pad.buttonSouth;
                case 1: return pad.buttonEast;
                case 2: return pad.buttonNorth;
                case 3: return pad.buttonWest;
                case 4: return pad.rightShoulder;
                case 5: return pad.leftShoulder;
                case 6: return pad.rightStickButton;
                case 7: return pad.leftStickButton;
                case 8: return pad.selectButton;
                case 9: return pad.startButton;
                default: return null;
            }
        }

        private static bool Read(ButtonControl control, int edge) => control != null &&
            (edge == 1 ? control.wasPressedThisFrame : edge == 2 ? control.wasReleasedThisFrame : control.isPressed);

        public static bool GetGamepadButton(int button, int player, int edge = 0)
        {
            if (player != 0) return Read(Button(Pad(player), button), edge);
            for (int i = 1; i <= Slots.Length; i++) if (Read(Button(Pad(i), button), edge)) return true;
            return false;
        }

        private static bool ReadKey(KeyCode code, int edge)
        {
            // Recovered title code scans legacy joystick KeyCodes to detect the input modality.
            int value = (int)code - (int)KeyCode.JoystickButton0;
            if (value >= 0 && value < 100)
            {
                int button = value % 20;
                return button < LegacyJoystickButtons.Length && GetGamepadButton(LegacyJoystickButtons[button], value / 20, edge);
            }
            var keyboard = ActiveKeyboard;
            var key = MapKey(code);
            return keyboard != null && key != Key.None && Read(keyboard[key], edge);
        }

        public static bool GetKey(KeyCode code) => ReadKey(code, 0);
        public static bool GetKeyDown(KeyCode code) => ReadKey(code, 1);
        public static bool GetKeyUp(KeyCode code) => ReadKey(code, 2);

        public static Key MapKey(KeyCode code)
        {
            if (code >= KeyCode.A && code <= KeyCode.Z) return Key.A + (code - KeyCode.A);
            if (code >= KeyCode.F1 && code <= KeyCode.F12) return Key.F1 + (code - KeyCode.F1);
            if (code >= KeyCode.Alpha1 && code <= KeyCode.Alpha9) return Key.Digit1 + (code - KeyCode.Alpha1);
            if (code >= KeyCode.Keypad1 && code <= KeyCode.Keypad9) return Key.Numpad1 + (code - KeyCode.Keypad1);
            switch (code)
            {
                case KeyCode.Alpha0: return Key.Digit0;
                case KeyCode.Keypad0: return Key.Numpad0;
                case KeyCode.Return: return Key.Enter;
                case KeyCode.KeypadEnter: return Key.NumpadEnter;
                case KeyCode.Space: return Key.Space;
                case KeyCode.Escape: return Key.Escape;
                case KeyCode.Tab: return Key.Tab;
                case KeyCode.Backspace: return Key.Backspace;
                case KeyCode.UpArrow: return Key.UpArrow;
                case KeyCode.DownArrow: return Key.DownArrow;
                case KeyCode.LeftArrow: return Key.LeftArrow;
                case KeyCode.RightArrow: return Key.RightArrow;
                case KeyCode.LeftShift: return Key.LeftShift;
                case KeyCode.RightShift: return Key.RightShift;
                case KeyCode.LeftControl: return Key.LeftCtrl;
                case KeyCode.RightControl: return Key.RightCtrl;
                case KeyCode.LeftAlt: return Key.LeftAlt;
                case KeyCode.RightAlt: return Key.RightAlt;
                case KeyCode.LeftCommand: case KeyCode.LeftWindows: return Key.LeftMeta;
                case KeyCode.RightCommand: case KeyCode.RightWindows: return Key.RightMeta;
                case KeyCode.Minus: return Key.Minus;
                case KeyCode.Equals: return Key.Equals;
                case KeyCode.Comma: return Key.Comma;
                case KeyCode.Period: return Key.Period;
                case KeyCode.Slash: return Key.Slash;
                case KeyCode.Semicolon: return Key.Semicolon;
                case KeyCode.Quote: return Key.Quote;
                case KeyCode.LeftBracket: return Key.LeftBracket;
                case KeyCode.RightBracket: return Key.RightBracket;
                case KeyCode.Backslash: return Key.Backslash;
                case KeyCode.BackQuote: return Key.Backquote;
                case KeyCode.KeypadMinus: return Key.NumpadMinus;
                case KeyCode.KeypadPlus: return Key.NumpadPlus;
                case KeyCode.KeypadPeriod: return Key.NumpadPeriod;
                case KeyCode.KeypadDivide: return Key.NumpadDivide;
                case KeyCode.KeypadMultiply: return Key.NumpadMultiply;
                case KeyCode.KeypadEquals: return Key.NumpadEquals;
                case KeyCode.PageUp: return Key.PageUp;
                case KeyCode.PageDown: return Key.PageDown;
                case KeyCode.Home: return Key.Home;
                case KeyCode.End: return Key.End;
                case KeyCode.Insert: return Key.Insert;
                case KeyCode.Delete: return Key.Delete;
                case KeyCode.CapsLock: return Key.CapsLock;
                case KeyCode.Numlock: return Key.NumLock;
                case KeyCode.ScrollLock: return Key.ScrollLock;
                case KeyCode.Print: return Key.PrintScreen;
                case KeyCode.Pause: return Key.Pause;
                default: return Key.None;
            }
        }

        // Disabled devices can retain state in the other editor/player buffer.
        // Apply the same enabled-device rule as the stable gamepad slots.
        private static Keyboard ActiveKeyboard => Keyboard.current != null && Keyboard.current.enabled ? Keyboard.current : null;
        private static Mouse ActiveMouse => Mouse.current != null && Mouse.current.enabled ? Mouse.current : null;

        private static ButtonControl MouseButton(int button)
        {
            var mouse = ActiveMouse;
            return mouse == null ? null : button == 0 ? mouse.leftButton : button == 1 ? mouse.rightButton : button == 2 ? mouse.middleButton : null;
        }
        public static bool GetMouseButtonDown(int button) => Read(MouseButton(button), 1);
        public static bool GetMouseButtonUp(int button) => Read(MouseButton(button), 2);
        public static Vector3 mousePosition => ActiveMouse == null ? Vector3.zero : (Vector3)ActiveMouse.position.ReadValue();
        public static Vector2 mouseScrollDelta => ActiveMouse == null ? Vector2.zero : ActiveMouse.scroll.ReadValue() / 120f;
        public static int touchCount => EnhancedTouchSupport.enabled ? Touch.activeTouches.Count : 0;
        public static float GetAxisRaw(string axis) => ActiveMouse == null ? 0 : axis == "Mouse X" ? ActiveMouse.delta.x.ReadValue() : axis == "Mouse Y" ? ActiveMouse.delta.y.ReadValue() : 0;
        public static bool anyKey => AnyKey(0);
        public static bool anyKeyDown => AnyKey(1);
        private static bool AnyKey(int edge)
        {
            if (Read(ActiveKeyboard?.anyKey, edge)) return true;
            for (int i = 0; i < 3; i++) if (Read(MouseButton(i), edge)) return true;
            for (int i = 0; i < 10; i++) if (GetGamepadButton(i, 0, edge)) return true;
            return false;
        }
    }
}
