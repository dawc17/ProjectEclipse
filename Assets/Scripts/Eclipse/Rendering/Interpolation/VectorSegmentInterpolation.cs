using UnityEngine;

namespace Eclipse.Rendering.Interpolation
{
	// Tracks a pair of presentation endpoints whose authoritative values are
	// updated by fixed-step simulation and sampled at render-frame alpha.
	public sealed class VectorSegmentInterpolation
	{
		private Vector3 _previousStart;
		private Vector3 _currentStart;
		private Vector3 _previousEnd;
		private Vector3 _currentEnd;
		private bool _initialized;

		public void Sample(Vector3 rawStart, Vector3 rawEnd, out Vector3 start, out Vector3 end)
		{
			if (!_initialized)
			{
				_previousStart = _currentStart = rawStart;
				_previousEnd = _currentEnd = rawEnd;
				_initialized = true;
			}
			else if (rawStart != _currentStart || rawEnd != _currentEnd)
			{
				_previousStart = _currentStart;
				_previousEnd = _currentEnd;
				_currentStart = rawStart;
				_currentEnd = rawEnd;
			}

			float alpha = FightInterpolation.CurrentAlpha;
			start = Vector3.LerpUnclamped(_previousStart, _currentStart, alpha);
			end = Vector3.LerpUnclamped(_previousEnd, _currentEnd, alpha);
		}
	}
}
