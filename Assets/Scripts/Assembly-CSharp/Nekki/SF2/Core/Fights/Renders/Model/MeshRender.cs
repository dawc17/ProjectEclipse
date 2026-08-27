using UnityEngine;
using UnityEngine.Rendering;

namespace Nekki.SF2.Core.Fights.Renders.Model
{
	public class MeshRender : MonoBehaviour
	{
		private Color _Color = new Color(0f, 0f, 0f, 1f);

		protected MeshNode JBLMEBBICJI = new MeshNode();

		private Mesh _Mesh;

		private static Shader _Shader;

		private static Material _SharedMaterial;

		public Color get_Color()
		{
			return _Color;
		}

		public void set_Color(Color value)
		{
			_Color = value;
			if (_SharedMaterial != null)
			{
				_SharedMaterial.SetVector("_Color", _Color);
			}
		}

		public MeshNode get_Base()
		{
			return JBLMEBBICJI;
		}

		private void Start()
		{
			if (_Shader == null)
			{
				_Shader = Shader.Find("Mesh/Colored");
				_SharedMaterial = new Material(_Shader);
			}
			_Mesh = new Mesh();
			base.gameObject.AddComponent<MeshFilter>().mesh = _Mesh;
			MeshRenderer meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
			meshRenderer.sharedMaterial = _SharedMaterial;
			meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
			meshRenderer.receiveShadows = false;
			meshRenderer.lightProbeUsage = LightProbeUsage.Off;
			meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
			_SharedMaterial.SetVector("_Color", _Color);
			Init();
		}

		private void Init()
		{
			JBLMEBBICJI.Init();
			_Mesh.vertices = JBLMEBBICJI.Vertices;
			_Mesh.triangles = JBLMEBBICJI.Triangles;
		}

		private void Update()
		{
			if (JBLMEBBICJI != null)
			{
				JBLMEBBICJI.Render();
				_Mesh.vertices = JBLMEBBICJI.Vertices;
				_Mesh.RecalculateBounds();
			}
		}
	}
}
