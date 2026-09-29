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
		private double _sampledFixedTime;

		public void Sample(Vector3 rawStart, Vector3 rawEnd, out Vector3 start, out Vector3 end)
		{
			Sample(rawStart, rawEnd, FightInterpolation.FightAlpha, out start, out end);
		}

		public void Sample(Vector3 rawStart, Vector3 rawEnd, float alpha, out Vector3 start, out Vector3 end)
		{
			if (!_initialized)
			{
				_previousStart = _currentStart = rawStart;
				_previousEnd = _currentEnd = rawEnd;
				_initialized = true;
			}
			else if (rawStart != _currentStart || rawEnd != _currentEnd)
			{
				// The previous endpoint is the one last drawn. When a slow frame ran several
				// ticks, blending from it would trail the rest of the body (which blends from
				// the previous tick), so show the new endpoints directly.
				bool severalTicks = Time.fixedTimeAsDouble - _sampledFixedTime > Time.fixedDeltaTime * 1.5;
				_previousStart = severalTicks ? rawStart : _currentStart;
				_previousEnd = severalTicks ? rawEnd : _currentEnd;
				_currentStart = rawStart;
				_currentEnd = rawEnd;
			}
			_sampledFixedTime = Time.fixedTimeAsDouble;

			start = Vector3.LerpUnclamped(_previousStart, _currentStart, alpha);
			end = Vector3.LerpUnclamped(_previousEnd, _currentEnd, alpha);
		}
	}
}
