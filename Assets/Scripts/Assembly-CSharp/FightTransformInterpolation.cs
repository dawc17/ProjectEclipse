using UnityEngine;

// Presentation-only interpolation for fight objects that bypass ModelNode and
// are advanced directly by the fixed-step fight simulation.
public class FightTransformInterpolation : MonoBehaviour
{
	private Vector3 _previousPosition;

	private Vector3 _currentPosition;

	private Quaternion _previousRotation = Quaternion.identity;

	private Quaternion _currentRotation = Quaternion.identity;

	private bool _initialized;

	public Vector3 CurrentPosition
	{
		get { return _currentPosition; }
	}

	public Quaternion CurrentRotation
	{
		get { return _currentRotation; }
	}

	public void Snap(Vector3 position, Quaternion rotation)
	{
		_previousPosition = position;
		_currentPosition = position;
		_previousRotation = rotation;
		_currentRotation = rotation;
		_initialized = true;
		Apply(1f);
	}

	public void Push(Vector3 position, Quaternion rotation)
	{
		if (!_initialized)
		{
			Snap(position, rotation);
			return;
		}
		_previousPosition = _currentPosition;
		_previousRotation = _currentRotation;
		_currentPosition = position;
		_currentRotation = rotation;
		// Keep the Unity transform authoritative between simulation and LateUpdate.
		// LateUpdate only substitutes the visible interpolated pose before rendering.
		Apply(1f);
	}

	private void LateUpdate()
	{
		if (_initialized)
		{
			Apply(ModelRenderInterpolation.GetCurrentAlpha());
		}
	}

	private void Apply(float alpha)
	{
		transform.localPosition = Vector3.LerpUnclamped(_previousPosition, _currentPosition, alpha);
		transform.localRotation = Quaternion.SlerpUnclamped(_previousRotation, _currentRotation, alpha);
	}
}
