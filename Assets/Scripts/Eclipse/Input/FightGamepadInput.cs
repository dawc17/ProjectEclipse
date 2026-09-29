using System;
using UnityEngine;

namespace Eclipse.Input
{
    // Native restrictions and each script instance compose; clearing one source
    // never grants an action still blocked by another source.
    public sealed class FightControlRestrictions
    {
        public const int MaximumScriptSources = 256;
        private readonly System.Collections.Generic.HashSet<FightCID> native = new System.Collections.Generic.HashSet<FightCID>();
        private readonly System.Collections.Generic.Dictionary<object, System.Collections.Generic.HashSet<FightCID>> scripts =
            new System.Collections.Generic.Dictionary<object, System.Collections.Generic.HashSet<FightCID>>();
        public void SetNative(FightCID control, bool blocked)
        {
            if (blocked) native.Add(control); else native.Remove(control);
        }
        public void SetScript(object owner, FightCID control, bool blocked)
        {
            if (owner == null) throw new System.ArgumentNullException(nameof(owner));
            if (!scripts.TryGetValue(owner, out var controls))
            {
                if (!blocked) return;
                if (scripts.Count >= MaximumScriptSources) throw new System.InvalidOperationException("Too many control restriction sources in this round.");
                controls = new System.Collections.Generic.HashSet<FightCID>();
                scripts.Add(owner, controls);
            }
            if (blocked) controls.Add(control); else controls.Remove(control);
            if (controls.Count == 0) scripts.Remove(owner);
        }
        public bool IsBlocked(FightCID control)
        {
            if (native.Contains(control)) return true;
            foreach (var controls in scripts.Values) if (controls.Contains(control)) return true;
            return false;
        }
        public void ClearScripts() => scripts.Clear();
    }

    // Rule suppression is independent of touch visibility and physical input source.
    public sealed class FightControlRuleGate
    {
        private readonly System.Collections.Generic.HashSet<FightCID> blocked = new System.Collections.Generic.HashSet<FightCID>();
        private readonly System.Collections.Generic.HashSet<FightCID> held = new System.Collections.Generic.HashSet<FightCID>();
        private readonly System.Collections.Generic.HashSet<FightCID> suppressed = new System.Collections.Generic.HashSet<FightCID>();

        public bool SetBlocked(FightCID control, bool value)
        {
            if (!value) { blocked.Remove(control); return false; }
            blocked.Add(control);
            if (!held.Remove(control)) return false;
            suppressed.Add(control);
            return true; // The caller must release the previously accepted press.
        }

        public bool Press(FightCID control)
        {
            if (blocked.Contains(control)) { suppressed.Add(control); return false; }
            if (suppressed.Contains(control)) return false;
            held.Add(control);
            return true;
        }

        public bool Release(FightCID control)
        {
            held.Remove(control);
            return !suppressed.Remove(control);
        }
    }

	/// <summary>
	/// A fixed keyboard layout that replaces the player's rebindable keys, so two
	/// players can share one keyboard. Each control accepts any of its keys.
	/// </summary>
	public sealed class FightKeyboardLayout
	{
		public KeyCode[] Up, Left, Down, Right, Punch, Kick, Ranged, Magic;

		public static bool Held(KeyCode[] keys)
		{
			if (keys == null) return false;
			foreach (var key in keys)
				if (UnityEngine.Input.GetKey(key)) return true;
			return false;
		}
	}

	public sealed class FightGamepadInput
	{
		private const float DeadZone = 0.35f;

		private readonly Func<FightCID, bool> _isControlEnabled;
		private readonly Action<int, FightCID> _emitControlEvent;
		private readonly GamePad.Player _player;
		private readonly FightKeyboardLayout _layout;

		private FightCID _direction = FightCID.QuadrantZero;
		private bool _punchPressed;
		private bool _kickPressed;
		private bool _rangedPressed;
		private bool _magicPressed;
		private bool _chargePressed;

		public FightGamepadInput(Func<FightCID, bool> isControlEnabled, Action<int, FightCID> emitControlEvent,
			GamePad.Player player = GamePad.Player.One, FightKeyboardLayout layout = null)
		{
			_isControlEnabled = isControlEnabled;
			_emitControlEvent = emitControlEvent;
			_player = player;
			_layout = layout;
		}

		public void Reset()
		{
			_direction = FightCID.QuadrantZero;
			_punchPressed = false;
			_kickPressed = false;
			_rangedPressed = false;
			_magicPressed = false;
			_chargePressed = false;
		}

			public void Poll(bool keyboardMovement = true, bool keyboardActions = false, bool gamepadEnabled = true)
			{
				gamepadEnabled = gamepadEnabled && IsConnected(_player);
				Vector2 dpad = gamepadEnabled ? GamePad.GetStick(GamePad.Stick.Dpad, _player, true) : Vector2.zero;
				Vector2 leftStick = gamepadEnabled ? GamePad.GetStick(FightControllerBindings.MovementStick, _player, true) : Vector2.zero;
			Vector2 movement = dpad.sqrMagnitude >= DeadZone * DeadZone ? dpad : leftStick;
			// Resolve both axes together; opposite keys cancel, and releasing one
            // half of a diagonal immediately restores the remaining direction.
            if (keyboardMovement)
            {
                var keyboard = new Vector2(
                    (Key(KeyCode.D, _layout?.Right) ? 1 : 0) - (Key(KeyCode.A, _layout?.Left) ? 1 : 0),
                    (Key(KeyCode.W, _layout?.Up) ? 1 : 0) - (Key(KeyCode.S, _layout?.Down) ? 1 : 0));
                if (keyboard != Vector2.zero) movement = keyboard;
            }
            SetDirection(GetDirection(movement));

				SetButton(ref _punchPressed, ReadButton(0, KeyCode.O, _layout?.Punch, keyboardActions, gamepadEnabled), FightCID.Punch);
				SetButton(ref _kickPressed, ReadButton(1, KeyCode.P, _layout?.Kick, keyboardActions, gamepadEnabled), FightCID.Kick);
				SetButton(ref _rangedPressed, ReadButton(2, KeyCode.K, _layout?.Ranged, keyboardActions, gamepadEnabled), FightCID.MissileButton);
				SetButton(ref _magicPressed, ReadButton(3, KeyCode.L, _layout?.Magic, keyboardActions, gamepadEnabled), FightCID.MagicButton);
				// A shared-keyboard layout has no raid charge key.
				SetButton(ref _chargePressed, _layout == null && ReadButton(4, KeyCode.J, null, keyboardActions, gamepadEnabled), FightCID.RaidChargeButton);
			}

			public static bool IsConnected(GamePad.Player player)
			{
				var devices = UnityEngine.Input.GetJoystickNames();
				int index = (int)player - 1;
				return index >= 0 && index < devices.Length && !string.IsNullOrEmpty(devices[index]);
			}

			private bool ReadButton(int action, KeyCode key, KeyCode[] layoutKeys, bool keyboard, bool gamepad)
			{
				return (keyboard && Key(key, layoutKeys)) ||
					(gamepad && FightControllerBindings.IsPressed(FightControllerBindings.Get(action), _player));
			}

			// The player's rebindable key, unless a fixed layout replaces it.
			private bool Key(KeyCode defaultKey, KeyCode[] layoutKeys)
			{
				return _layout != null ? FightKeyboardLayout.Held(layoutKeys) : UnityEngine.Input.GetKey(FightKeyBindings.Get(defaultKey));
			}

		public void ReleaseAll()
		{
			SetDirection(FightCID.QuadrantZero);
			ReleaseButton(ref _punchPressed, FightCID.Punch);
			ReleaseButton(ref _kickPressed, FightCID.Kick);
			ReleaseButton(ref _rangedPressed, FightCID.MissileButton);
			ReleaseButton(ref _magicPressed, FightCID.MagicButton);
            ReleaseButton(ref _chargePressed, FightCID.RaidChargeButton);
		}

		private static FightCID GetDirection(Vector2 direction)
		{
			if (direction.sqrMagnitude < DeadZone * DeadZone)
			{
				return FightCID.QuadrantZero;
			}

			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			if (angle < 0f)
			{
				angle += 360f;
			}

			if (angle < 27.5f || angle >= 332.5f)
			{
				return FightCID.QuadrantForward;
			}
			if (angle < 62.5f)
			{
				return FightCID.QuadrantUpForward;
			}
			if (angle < 117.5f)
			{
				return FightCID.QuadrantUp;
			}
			if (angle < 152.5f)
			{
				return FightCID.QuadrantUpBack;
			}
			if (angle < 207.5f)
			{
				return FightCID.QuadrantBack;
			}
			if (angle < 242.5f)
			{
				return FightCID.QuadrantDownBack;
			}
			if (angle < 297.5f)
			{
				return FightCID.QuadrantDown;
			}
			return FightCID.QuadrantDownForward;
		}

		private void SetDirection(FightCID direction)
		{
			if (direction != FightCID.QuadrantZero && !_isControlEnabled(direction))
			{
				direction = FightCID.QuadrantZero;
			}
			if (_direction == direction)
			{
				return;
			}

			if (_direction != FightCID.QuadrantZero)
			{
				_emitControlEvent(1, _direction);
			}
			_direction = direction;
			if (_direction != FightCID.QuadrantZero)
			{
				_emitControlEvent(0, _direction);
			}
		}

		private void SetButton(ref bool state, bool pressed, FightCID control)
		{
			bool value = pressed && _isControlEnabled(control);
			if (state == value)
			{
				return;
			}
			state = value;
			_emitControlEvent(value ? 0 : 1, control);
		}

		private void ReleaseButton(ref bool state, FightCID control)
		{
			if (!state)
			{
				return;
			}
			state = false;
			_emitControlEvent(1, control);
		}
	}
}
