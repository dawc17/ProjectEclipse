using System;
using UnityEngine;

namespace Eclipse.Input
{
	public sealed class FightGamepadInput
	{
		private const float DeadZone = 0.35f;

		private readonly Func<FightCID, bool> _isControlEnabled;
		private readonly Action<int, FightCID> _emitControlEvent;

		private FightCID _direction = FightCID.QuadrantZero;
		private bool _punchPressed;
		private bool _kickPressed;
		private bool _rangedPressed;
		private bool _magicPressed;

		public FightGamepadInput(Func<FightCID, bool> isControlEnabled, Action<int, FightCID> emitControlEvent)
		{
			_isControlEnabled = isControlEnabled;
			_emitControlEvent = emitControlEvent;
		}

		public void Reset()
		{
			_direction = FightCID.QuadrantZero;
			_punchPressed = false;
			_kickPressed = false;
			_rangedPressed = false;
			_magicPressed = false;
		}

		public void Poll()
		{
			Vector2 dpad = GamePad.CNNMBBLLGNE(GamePad.LCNPGEANNDP.Dpad, GamePad.GGAKHLLMPMM.One, true);
			Vector2 leftStick = GamePad.CNNMBBLLGNE(GamePad.LCNPGEANNDP.LeftStick, GamePad.GGAKHLLMPMM.One, true);
			Vector2 movement = dpad.sqrMagnitude >= DeadZone * DeadZone ? dpad : leftStick;
			SetDirection(GetDirection(movement));

			SetButton(ref _punchPressed,
				GamePad.NFCGBMHPKMA(GamePad.PFENLAPGKFM.X, GamePad.GGAKHLLMPMM.One), FightCID.Punch);
			SetButton(ref _kickPressed,
				GamePad.NFCGBMHPKMA(GamePad.PFENLAPGKFM.A, GamePad.GGAKHLLMPMM.One), FightCID.Kick);
			SetButton(ref _rangedPressed,
				GamePad.NFCGBMHPKMA(GamePad.PFENLAPGKFM.B, GamePad.GGAKHLLMPMM.One), FightCID.MissileButton);
			SetButton(ref _magicPressed,
				GamePad.NFCGBMHPKMA(GamePad.PFENLAPGKFM.Y, GamePad.GGAKHLLMPMM.One), FightCID.MagicButton);
		}

		public void ReleaseAll()
		{
			SetDirection(FightCID.QuadrantZero);
			ReleaseButton(ref _punchPressed, FightCID.Punch);
			ReleaseButton(ref _kickPressed, FightCID.Kick);
			ReleaseButton(ref _rangedPressed, FightCID.MissileButton);
			ReleaseButton(ref _magicPressed, FightCID.MagicButton);
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
