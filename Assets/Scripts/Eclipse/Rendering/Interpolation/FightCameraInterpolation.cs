using UnityEngine;

namespace Eclipse.Rendering.Interpolation
{
	public sealed class FightCameraInterpolation
	{
		private readonly Vector3f _cameraPosition = new Vector3f();
		private readonly Vector3f _cameraTarget = new Vector3f();
		private readonly Vector3f _focusPosition = new Vector3f();
		private float _previousZoomScale;
		private float _currentZoomScale;

		public Vector3f CameraPosition
		{
			get { return _cameraPosition; }
		}

		public Vector3f CameraTarget
		{
			get { return _cameraTarget; }
		}

		public Vector3f FocusPosition
		{
			get { return _focusPosition; }
		}

		public void SamplePositions(
			ModelNode cameraPosition,
			ModelNode cameraTarget,
			ModelNode focusPosition,
			float alpha)
		{
			FightInterpolation.SamplePosition(cameraPosition, alpha, _cameraPosition);
			FightInterpolation.SamplePosition(cameraTarget, alpha, _cameraTarget);
			FightInterpolation.SamplePosition(focusPosition, alpha, _focusPosition);
		}

		public void ResetZoomScale(float zoomScale)
		{
			_previousZoomScale = zoomScale;
			_currentZoomScale = zoomScale;
		}

		public void PushZoomScale(float zoomScale)
		{
			_previousZoomScale = _currentZoomScale;
			_currentZoomScale = zoomScale;
		}

		public float SampleZoomScale(float alpha)
		{
			return Mathf.Lerp(_previousZoomScale, _currentZoomScale, alpha);
		}
	}
}
