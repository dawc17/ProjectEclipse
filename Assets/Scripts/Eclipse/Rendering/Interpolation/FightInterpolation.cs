using UnityEngine;

namespace Eclipse.Rendering.Interpolation
{
	// Presentation-only interpolation facade for the custom fixed-step fight model.
	// Simulation, collision, AI and attack timing continue to read current model state.
	public static class FightInterpolation
	{
		private static int _cachedFrame = -1;

		private static float _cachedAlpha = 1f;

		public static bool Enabled
		{
			get { return SF2DisplayFrameRate.InterpolationEnabled; }
		}

		public static float CurrentAlpha
		{
			get
			{
				if (!Enabled)
				{
					return 1f;
				}
				int frame = Time.frameCount;
				if (_cachedFrame != frame)
				{
					_cachedFrame = frame;
					_cachedAlpha = CalculateAlpha(Time.timeAsDouble, Time.fixedTimeAsDouble, Time.fixedDeltaTime);
				}
				return _cachedAlpha;
			}
		}

		private static double _drawStep = double.NaN;
		private static double _fightStep = double.NaN;
		private static double _cameraStep = double.NaN;

		// Fight.Draw marks each fixed step it drives, then which subsystems
		// actually advanced. During pause or hit-stop the frozen poses still hold
		// unequal previous/current values; interpolating them would replay the
		// last tick over and over, so a frozen subsystem is shown at its current pose.
		public static void MarkDrawStep() { _drawStep = Time.fixedTimeAsDouble; }
		public static void MarkFightStep() { _fightStep = Time.fixedTimeAsDouble; }
		public static void MarkCameraStep() { _cameraStep = Time.fixedTimeAsDouble; }

		// True when an active fight drove the latest fixed step.
		private static bool FightDriven
		{
			get { return _drawStep == Time.fixedTimeAsDouble; }
		}

		// A fight is currently being simulated (it drove the latest fixed step).
		public static bool IsFightActive
		{
			get { return FightDriven; }
		}

		// Alpha for fighters, effects and other fight-simulation presentation.
		public static float FightAlpha
		{
			get { return FightDriven && _fightStep != _drawStep ? 1f : CurrentAlpha; }
		}

		// Alpha for the fight camera, which keeps running during hit-stop.
		public static float CameraAlpha
		{
			get { return FightDriven && _cameraStep != _drawStep ? 1f : CurrentAlpha; }
		}

		public static float CalculateAlpha(double renderTime, double fixedTime, float fixedDeltaTime)
		{
			if (fixedDeltaTime <= 0f || double.IsNaN(renderTime) || double.IsNaN(fixedTime))
			{
				return 1f;
			}
			return Mathf.Clamp01((float)((renderTime - fixedTime) / fixedDeltaTime));
		}

		public static void SamplePosition(ModelNode node, float alpha, out float x, out float y, out float z)
		{
			Vector3f current = node.GetStart();
			Vector3f previous = node.GetEnd();
			x = Mathf.Lerp(previous.GetX(), current.GetX(), alpha);
			y = Mathf.Lerp(previous.GetY(), current.GetY(), alpha);
			z = Mathf.Lerp(previous.GetZ(), current.GetZ(), alpha);
		}

		public static void SamplePosition(ModelNode node, float alpha, Vector3f result)
		{
			float x;
			float y;
			float z;
			SamplePosition(node, alpha, out x, out y, out z);
			result.SetX(x);
			result.SetY(y);
			result.SetZ(z);
		}
	}
}
