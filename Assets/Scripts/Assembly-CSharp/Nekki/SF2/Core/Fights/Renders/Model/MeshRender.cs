using Eclipse.Rendering.Interpolation;
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

		private MeshRenderer _Renderer;

		private MaterialPropertyBlock _TintBlock;

		private Eclipse.Rendering.ModelPresentation _Presentation;

		private int _TintVersion = -1;

		private MeshRenderer _RimRenderer;

		private MaterialPropertyBlock _RimBlock;

		public Color get_Color()
		{
			return _Color;
		}

		// Per-renderer colour. The material is shared by every fighter, so never
		// write colours into it; a ModelPresentation tint takes precedence.
		public void set_Color(Color value)
		{
			_Color = value;
			_TintVersion = -1;
			ApplyTint();
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
			_Renderer = meshRenderer;
			_Presentation = GetComponentInParent<Eclipse.Rendering.ModelPresentation>();
			// Experimental rim light: an offset twin sharing this mesh.
			GameObject rim = new GameObject("Rim");
			rim.transform.SetParent(base.transform, false);
			rim.AddComponent<MeshFilter>().sharedMesh = _Mesh;
			_RimRenderer = rim.AddComponent<MeshRenderer>();
			_RimRenderer.sharedMaterial = _SharedMaterial;
			_RimRenderer.shadowCastingMode = ShadowCastingMode.Off;
			_RimRenderer.receiveShadows = false;
			_RimRenderer.lightProbeUsage = LightProbeUsage.Off;
			_RimRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
			rim.SetActive(false);
			meshRenderer.sharedMaterial = _SharedMaterial;
			meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
			meshRenderer.receiveShadows = false;
			meshRenderer.lightProbeUsage = LightProbeUsage.Off;
			meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
			Init();
		}

		private void UpdateRim()
		{
			if (_RimRenderer == null) return;
			bool active = Eclipse.Rendering.RimLight.Active;
			if (_RimRenderer.gameObject.activeSelf != active) _RimRenderer.gameObject.SetActive(active);
			if (!active) return;
			_RimRenderer.transform.localPosition = Eclipse.Rendering.RimLight.LocalOffset(base.transform);
			if (_RimBlock == null) _RimBlock = new MaterialPropertyBlock();
			_RimBlock.SetVector("_Color", Eclipse.Rendering.RimLight.ColorFor(base.transform));
			_RimRenderer.SetPropertyBlock(_RimBlock);
		}

		// Per-renderer override of the shared material's colour; clearing it
		// restores the shared (original) colour.
		private void ApplyTint()
		{
			if (_Renderer == null) return;
			int version = _Presentation != null ? _Presentation.TintVersion : 0;
			if (_TintVersion == version) return;
			_TintVersion = version;
			if (_TintBlock == null) _TintBlock = new MaterialPropertyBlock();
			Color? tint = _Presentation != null ? _Presentation.Tint : null;
			_TintBlock.SetVector("_Color", tint ?? _Color);
			_Renderer.SetPropertyBlock(_TintBlock);
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
				JBLMEBBICJI.Render(Eclipse.Rendering.ModelPresentation.AlphaFor(_Presentation));
				ApplyTint();
				_Mesh.vertices = JBLMEBBICJI.Vertices;
				_Mesh.RecalculateBounds();
				UpdateRim();
			}
		}
	}
}
