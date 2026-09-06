using System;
using UnityEngine;

namespace Eclipse.Input
{
    public static class FightControllerBindings
    {
        // 0..9 are the existing GamePad button enum; 10 and 11 are analog triggers.
        public static readonly int[] Defaults = { 3, 0, 1, 2, 4 };
        public static readonly string[] Names = { "Punch", "Kick", "Ranged weapon", "Magic", "Raid charge" };
        private static readonly string[] Labels = { "A / Cross", "B / Circle", "Y / Triangle", "X / Square",
            "RB / R1", "LB / L1", "Right stick click", "Left stick click", "Back / Share", "Start / Options", "LT / L2", "RT / R2" };
        private const string Prefix = "Eclipse.Controller.";
        public static int Get(int action)
        {
            int value = PlayerPrefs.GetInt(Prefix + Names[action], Defaults[action]);
            return value >= 0 && value < Labels.Length ? value : Defaults[action];
        }
        public static string Display(int value) { return Labels[value]; }
        public static bool IsPressed(int value)
        {
            return value < 10 ? GamePad.NFCGBMHPKMA((GamePad.PFENLAPGKFM)value, GamePad.GGAKHLLMPMM.One)
                : GamePad.MAJINGINCHM((GamePad.HKKPDLMCPIF)(value - 10), GamePad.GGAKHLLMPMM.One, true) > .5f;
        }
        public static bool TrySet(int action, int value, out string message)
        {
            if (action < 0 || action >= Names.Length || value < 0 || value >= Labels.Length)
                throw new ArgumentOutOfRangeException();
            for (int i = 0; i < Names.Length; i++)
                if (i != action && Get(i) == value)
                { message = Display(value) + " is assigned to " + Names[i] + ". Choose another input."; return false; }
            PlayerPrefs.SetInt(Prefix + Names[action], value);
            PlayerPrefs.Save();
            message = Names[action] + " saved.";
            return true;
        }
        public static GamePad.LCNPGEANNDP MovementStick { get { return PlayerPrefs.GetInt(Prefix + "RightStick", 0) == 1
            ? GamePad.LCNPGEANNDP.RightStick : GamePad.LCNPGEANNDP.LeftStick; } }
        public static void ToggleStick()
        {
            PlayerPrefs.SetInt(Prefix + "RightStick", MovementStick == GamePad.LCNPGEANNDP.LeftStick ? 1 : 0);
            PlayerPrefs.Save();
        }
        public static void Reset()
        {
            foreach (var name in Names) PlayerPrefs.DeleteKey(Prefix + name);
            PlayerPrefs.DeleteKey(Prefix + "RightStick");
            PlayerPrefs.Save();
        }
    }

    public static class FightKeyBindings
    {
        public static readonly KeyCode[] Defaults = {
            KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D,
            KeyCode.O, KeyCode.P, KeyCode.K, KeyCode.L, KeyCode.J
        };
        public static readonly string[] Names = {
            "Jump", "Crouch", "Move left", "Move right",
            "Punch", "Kick", "Ranged weapon", "Magic", "Raid charge"
        };
        private const string Prefix = "Eclipse.Key.";

        public static KeyCode Get(KeyCode original)
        {
            var key = (KeyCode)PlayerPrefs.GetInt(Prefix + original, (int)original);
            return IsAllowed(key) ? key : original;
        }

        // Numpad movement and the recovered debug shortcuts remain reserved.
        public static bool IsAllowed(KeyCode key)
        {
            return (key >= KeyCode.A && key <= KeyCode.Z && key != KeyCode.M && key != KeyCode.B && key != KeyCode.U)
                || key == KeyCode.Space || key == KeyCode.Tab
                || key == KeyCode.LeftShift || key == KeyCode.RightShift
                || key == KeyCode.LeftControl || key == KeyCode.RightControl
                || key == KeyCode.UpArrow || key == KeyCode.DownArrow || key == KeyCode.LeftArrow
                || key == KeyCode.Comma || key == KeyCode.Period || key == KeyCode.Slash
                || key == KeyCode.Semicolon || key == KeyCode.Quote
                || key == KeyCode.LeftBracket || key == KeyCode.RightBracket;
        }

        public static bool TrySet(int action, KeyCode key, out string message)
        {
            if (action < 0 || action >= Defaults.Length) throw new ArgumentOutOfRangeException("action");
            if (!IsAllowed(key)) { message = "That key is reserved. Choose another key, or Esc to cancel."; return false; }
            for (int i = 0; i < Defaults.Length; i++)
                if (i != action && Get(Defaults[i]) == key)
                { message = Display(key) + " is assigned to " + Names[i] + ". Choose another key."; return false; }
            PlayerPrefs.SetInt(Prefix + Defaults[action], (int)key);
            PlayerPrefs.Save();
            message = Names[action] + " saved.";
            return true;
        }

        public static void Reset()
        {
            foreach (var key in Defaults) PlayerPrefs.DeleteKey(Prefix + key);
            PlayerPrefs.Save();
        }

        public static string Display(KeyCode key)
        {
            return key.ToString().Replace("Left", "Left ").Replace("Right", "Right ").Replace("Arrow", " Arrow").Trim();
        }
    }
}
