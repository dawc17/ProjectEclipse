using System;
using UnityEngine;

namespace Eclipse.Input
{
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

	public sealed class FightGamepadInput
	{
		private const float DeadZone = 0.35f;

		private readonly Func<FightCID, bool> _isControlEnabled;
		private readonly Action<int, FightCID> _emitControlEvent;
		private readonly GamePad.Player _player;

		private FightCID _direction = FightCID.QuadrantZero;
		private bool _punchPressed;
		private bool _kickPressed;
		private bool _rangedPressed;
		private bool _magicPressed;
		private bool _chargePressed;

		public FightGamepadInput(Func<FightCID, bool> isControlEnabled, Action<int, FightCID> emitControlEvent,
			GamePad.Player player = GamePad.Player.One)
		{
			_isControlEnabled = isControlEnabled;
			_emitControlEvent = emitControlEvent;
			_player = player;
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
                    (UnityEngine.Input.GetKey(FightKeyBindings.Get(KeyCode.D)) ? 1 : 0) - (UnityEngine.Input.GetKey(FightKeyBindings.Get(KeyCode.A)) ? 1 : 0),
                    (UnityEngine.Input.GetKey(FightKeyBindings.Get(KeyCode.W)) ? 1 : 0) - (UnityEngine.Input.GetKey(FightKeyBindings.Get(KeyCode.S)) ? 1 : 0));
                if (keyboard != Vector2.zero) movement = keyboard;
            }
            SetDirection(GetDirection(movement));

				SetButton(ref _punchPressed, ReadButton(0, KeyCode.O, keyboardActions, gamepadEnabled), FightCID.Punch);
				SetButton(ref _kickPressed, ReadButton(1, KeyCode.P, keyboardActions, gamepadEnabled), FightCID.Kick);
				SetButton(ref _rangedPressed, ReadButton(2, KeyCode.K, keyboardActions, gamepadEnabled), FightCID.MissileButton);
				SetButton(ref _magicPressed, ReadButton(3, KeyCode.L, keyboardActions, gamepadEnabled), FightCID.MagicButton);
				SetButton(ref _chargePressed, ReadButton(4, KeyCode.J, keyboardActions, gamepadEnabled), FightCID.RaidChargeButton);
			}

			public static bool IsConnected(GamePad.Player player)
			{
				var devices = UnityEngine.Input.GetJoystickNames();
				int index = (int)player - 1;
				return index >= 0 && index < devices.Length && !string.IsNullOrEmpty(devices[index]);
			}

			private bool ReadButton(int action, KeyCode key, bool keyboard, bool gamepad)
			{
				return (keyboard && UnityEngine.Input.GetKey(FightKeyBindings.Get(key))) ||
					(gamepad && FightControllerBindings.IsPressed(FightControllerBindings.Get(action), _player));
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
