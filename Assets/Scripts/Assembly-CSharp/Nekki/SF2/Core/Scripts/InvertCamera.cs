using UnityEngine;

namespace Nekki.SF2.Core.Scripts
{
	public class InvertCamera : MonoBehaviour
	{
		[SerializeField]
		private Vector3 _Scale = new Vector3(1f, -1f, 1f);

		private void OnPreCull()
		{
			GetComponent<UnityEngine.Camera>().ResetWorldToCameraMatrix();
			GetComponent<UnityEngine.Camera>().ResetProjectionMatrix();
			GetComponent<UnityEngine.Camera>().projectionMatrix = GetComponent<UnityEngine.Camera>().projectionMatrix * Matrix4x4.Scale(_Scale);
		}

		private void OnPreRender()
		{
			GL.invertCulling = true;
		}

		private void OnPostRender()
		{
			GL.invertCulling = false;
		}
	}
}
