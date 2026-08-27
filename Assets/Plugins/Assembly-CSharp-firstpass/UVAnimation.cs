using UnityEngine;

public class UVAnimation : MonoBehaviour
{
	public Vector2 ScrollSpeed = new Vector2(0.1f, 0f);

	private Mesh _mesh;

	private void Start()
	{
		_mesh = base.transform.GetComponent<MeshFilter>().sharedMesh;
	}

	private void Update()
	{
		JAGIBPBOMBL();
	}

	private void JAGIBPBOMBL()
	{
		Vector2[] uv = _mesh.uv;
		for (int i = 0; i < uv.Length; i++)
		{
			uv[i] += ScrollSpeed * Time.deltaTime;
		}
		_mesh.uv = uv;
	}
}
