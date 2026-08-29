using UnityEngine;

public class CameraRenderInterpolationDriver : MonoBehaviour
{
	private global::Camera _camera;

	public void Init(global::Camera camera)
	{
		_camera = camera;
	}

	private void LateUpdate()
	{
		if (_camera != null && SF2DisplayFrameRate.InterpolationEnabled)
		{
			_camera.RenderInterpolatedPresentation();
		}
	}
}
