using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Rendering.Interpolation
{
	// Smooths fight HUD values that the fixed-step simulation writes directly
	// (a label's localPosition, a bar's fillAmount). Before every fixed step the
	// authoritative values are restored, so simulation code keeps reading and
	// writing exactly what it did; LateUpdate shows them blended between the
	// value before and after the latest step.
	[DefaultExecutionOrder(-10000)]
	public sealed class TickPresentationSmoother : MonoBehaviour
	{
		public enum Clock { Fight, Camera }

		private Clock _clock;
		private bool _smoothPosition;
		private Image _fill;

		private bool _displayed;
		private bool _initialized;
		private Vector3 _previousPosition, _currentPosition;
		private float _previousFill, _currentFill;

		public static TickPresentationSmoother AttachPosition(GameObject target, Clock clock)
		{
			var smoother = Attach(target, clock);
			smoother._smoothPosition = true;
			return smoother;
		}

		public static TickPresentationSmoother AttachFill(Image image, Clock clock)
		{
			if (image == null) return null;
			var smoother = Attach(image.gameObject, clock);
			smoother._fill = image;
			return smoother;
		}

		private static TickPresentationSmoother Attach(GameObject target, Clock clock)
		{
			var smoother = target.GetComponent<TickPresentationSmoother>();
			if (smoother == null) smoother = target.AddComponent<TickPresentationSmoother>();
			smoother._clock = clock;
			return smoother;
		}

		private void FixedUpdate()
		{
			if (_displayed)
			{
				if (_smoothPosition) transform.localPosition = _currentPosition;
				if (_fill != null) _fill.fillAmount = _currentFill;
				_displayed = false;
			}
			if (_smoothPosition) _previousPosition = transform.localPosition;
			if (_fill != null) _previousFill = _fill.fillAmount;
			_initialized = true;
		}

		private void LateUpdate()
		{
			if (!_initialized || !FightInterpolation.Enabled) return;
			float alpha = _clock == Clock.Camera ? FightInterpolation.CameraAlpha : FightInterpolation.FightAlpha;
			// Read the authoritative values only once per fixed step; later frames
			// would otherwise read back the blended value shown last frame.
			if (!_displayed)
			{
				if (_smoothPosition) _currentPosition = transform.localPosition;
				if (_fill != null) _currentFill = _fill.fillAmount;
			}
			if (_smoothPosition) transform.localPosition = Vector3.LerpUnclamped(_previousPosition, _currentPosition, alpha);
			if (_fill != null) _fill.fillAmount = Mathf.LerpUnclamped(_previousFill, _currentFill, alpha);
			_displayed = true;
		}
	}
}
