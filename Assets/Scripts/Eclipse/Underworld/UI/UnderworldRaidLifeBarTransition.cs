using System;

namespace Eclipse.Underworld.UI
{
	public struct UnderworldRaidLifeBarUpdate
	{
		public bool Handled { get; internal set; }
		public bool ResetLife { get; internal set; }
		public bool SetHitDelay { get; internal set; }
		public float HitDelay { get; internal set; }
		public bool SetHealthBar { get; internal set; }
		public float HealthBarValue { get; internal set; }
		public int HealthBarFrames { get; internal set; }
		public bool SetHitBar { get; internal set; }
		public float HitBarValue { get; internal set; }
		public int HitBarFrames { get; internal set; }
	}

	public sealed class UnderworldRaidLifeBarTransition
	{
		private const int DamageDelayFrames = 60;
		private const int BarTransitionFrames = 10;
		private const int HitDrainFrames = 30;

		private enum TransitionState
		{
			None,
			DrainingSegment,
			FillingReplacement
		}

		private TransitionState _state;
		private int _remainingBars;
		private int _pendingSegmentsToDrain;
		private float _pendingHealthBarFraction;

		public void Reset(int remainingBars)
		{
			_state = TransitionState.None;
			_pendingSegmentsToDrain = 0;
			_pendingHealthBarFraction = 0f;
			_remainingBars = remainingBars;
		}

		public UnderworldRaidLifeBarUpdate Update(
			float targetFraction,
			int remainingHealthBars,
			int healthBarCount,
			float renderedHealthFraction,
			float renderedHitFraction,
			float currentTargetHealthFraction)
		{
			if (_state != TransitionState.None)
			{
				return UpdateTransition(
					targetFraction,
					remainingHealthBars,
					renderedHealthFraction,
					renderedHitFraction,
					currentTargetHealthFraction);
			}

			if (_remainingBars != remainingHealthBars)
			{
				if (healthBarCount > 1 && remainingHealthBars < _remainingBars)
				{
					return StartTransition(targetFraction, remainingHealthBars);
				}
				return ResetLifeUpdate();
			}

			return default(UnderworldRaidLifeBarUpdate);
		}

		private UnderworldRaidLifeBarUpdate StartTransition(float targetFraction, int remainingHealthBars)
		{
			_pendingSegmentsToDrain = Math.Max(1, _remainingBars - remainingHealthBars);
			_pendingHealthBarFraction = targetFraction;
			_remainingBars = remainingHealthBars;
			return BeginDrainingSegment();
		}

		private UnderworldRaidLifeBarUpdate UpdateTransition(
			float targetFraction,
			int remainingHealthBars,
			float renderedHealthFraction,
			float renderedHitFraction,
			float currentTargetHealthFraction)
		{
			// Damage may arrive while the previous segment is still handing off.
			// Fold it into the active transition instead of snapping to the model.
			if (remainingHealthBars > _remainingBars)
			{
				return ResetLifeUpdate();
			}
			if (remainingHealthBars < _remainingBars)
			{
				_pendingSegmentsToDrain += _remainingBars - remainingHealthBars;
				_remainingBars = remainingHealthBars;
			}
			_pendingHealthBarFraction = targetFraction;

			if (_state == TransitionState.DrainingSegment)
			{
				if (renderedHealthFraction != 0f || renderedHitFraction != 0f)
				{
					return HandledOnly();
				}
				if (_remainingBars <= 0)
				{
					return CompleteTransition(currentTargetHealthFraction);
				}
				return BeginFillingReplacement();
			}

			if (renderedHealthFraction != 1f || renderedHitFraction != 1f)
			{
				return HandledOnly();
			}
			if (_pendingSegmentsToDrain > 1)
			{
				_pendingSegmentsToDrain--;
				return BeginDrainingSegment();
			}
			return CompleteTransition(currentTargetHealthFraction);
		}

		private UnderworldRaidLifeBarUpdate BeginDrainingSegment()
		{
			_state = TransitionState.DrainingSegment;
			return new UnderworldRaidLifeBarUpdate
			{
				Handled = true,
				SetHitDelay = true,
				HitDelay = 0f,
				SetHealthBar = true,
				HealthBarValue = 0f,
				HealthBarFrames = BarTransitionFrames,
				SetHitBar = true,
				HitBarValue = 0f,
				HitBarFrames = HitDrainFrames
			};
		}

		private UnderworldRaidLifeBarUpdate BeginFillingReplacement()
		{
			_state = TransitionState.FillingReplacement;
			return new UnderworldRaidLifeBarUpdate
			{
				Handled = true,
				SetHealthBar = true,
				HealthBarValue = 1f,
				HealthBarFrames = BarTransitionFrames,
				SetHitBar = true,
				HitBarValue = 1f,
				HitBarFrames = BarTransitionFrames
			};
		}

		private UnderworldRaidLifeBarUpdate CompleteTransition(float currentTargetHealthFraction)
		{
			_state = TransitionState.None;
			_pendingSegmentsToDrain = 0;
			if (_remainingBars <= 0 || _pendingHealthBarFraction >= 1f)
			{
				return HandledOnly();
			}

			float damage = _pendingHealthBarFraction - currentTargetHealthFraction;
			if (damage >= 0f)
			{
				return HandledOnly();
			}

			return new UnderworldRaidLifeBarUpdate
			{
				Handled = true,
				SetHitDelay = true,
				HitDelay = Math.Abs(damage) < 0.01f ? 1f : DamageDelayFrames,
				SetHealthBar = true,
				HealthBarValue = _pendingHealthBarFraction,
				HealthBarFrames = BarTransitionFrames
			};
		}

		private static UnderworldRaidLifeBarUpdate HandledOnly()
		{
			return new UnderworldRaidLifeBarUpdate { Handled = true };
		}

		private static UnderworldRaidLifeBarUpdate ResetLifeUpdate()
		{
			return new UnderworldRaidLifeBarUpdate
			{
				Handled = true,
				ResetLife = true
			};
		}
	}
}
