using UnityEngine;

namespace SF2DE.Rendering.Interpolation
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
			Vector3f current = node.ICLEOFDKDIF();
			Vector3f previous = node.FOGHEPNAPLC();
			x = Mathf.Lerp(previous.GILCBJJPKBK(), current.GILCBJJPKBK(), alpha);
			y = Mathf.Lerp(previous.OBIMBNIBEFG(), current.OBIMBNIBEFG(), alpha);
			z = Mathf.Lerp(previous.KMFEKANLCFO(), current.KMFEKANLCFO(), alpha);
		}

		public static void SamplePosition(ModelNode node, float alpha, Vector3f result)
		{
			float x;
			float y;
			float z;
			SamplePosition(node, alpha, out x, out y, out z);
			result.JPFALPBDBAP(x);
			result.IBNFLLGPOLD(y);
			result.set_Z(z);
		}
	}
}
