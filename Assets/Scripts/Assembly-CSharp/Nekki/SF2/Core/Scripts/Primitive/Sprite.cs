using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Nekki.SF2.Core.Scripts.Primitive
{
	public class Sprite : MonoBehaviour
	{
		private Color _Color = new Color(1f, 1f, 1f, 1f);

		private Texture2D _Texture;

		private Rect _FrameRect = new Rect(0f, 0f, 1f, 1f);

		private float MIHBPJAFIOC = 1f;

		private float PHJHMBHCDEB = 1f;

		private static Shader _Shader = Shader.Find("Sprites/Legacy/Default");

		private static Shader CKOPCEGGGDJ = Shader.Find("Sprites/Legacy/Multiply");

		private static Material _SharedMaterial = new Material(_Shader);

		private static Dictionary<Texture, Material> MLLLJIJMCGH = new Dictionary<Texture, Material>();

		private static Dictionary<Texture, Material> MIEMECLJFIG = new Dictionary<Texture, Material>();

		private Material JIBGIDGLPKK;

		private MeshRenderer _MeshRender;

		public Mesh _Mesh;

		public Texture2D IFGDJMHCDDE
		{
			get
			{
				return get_Texture();
			}
			set
			{
				set_Texture(value);
			}
		}

		public Rect LODDPEBLFIB
		{
			set
			{
				set_FrameRect(value);
			}
		}

		public Color get_Color()
		{
			return _Color;
		}

		public void set_Color(Color value)
		{
			_Color = value;
			if (_Mesh != null)
			{
				int num = _Mesh.vertices.Length;
				Color[] array = new Color[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = _Color;
				}
				_Mesh.colors = array;
			}
		}

		public Texture2D get_Texture()
		{
			return _Texture;
		}

		public void set_Texture(Texture2D value)
		{
			_Texture = value;
			if (JIBGIDGLPKK == null && _MeshRender != null)
			{
				_MeshRender.sharedMaterial = FMHMKAEFLMF(false, _Texture);
			}
			if (JIBGIDGLPKK != null)
			{
				JIBGIDGLPKK.mainTexture = _Texture;
			}
		}

		private static Material FMHMKAEFLMF(bool IPEKLPADIMF, Texture AOHHPLGIPDA)
		{
			Dictionary<Texture, Material> dictionary = ((!IPEKLPADIMF) ? MLLLJIJMCGH : MIEMECLJFIG);
			Material material = null;
			if (dictionary.ContainsKey(AOHHPLGIPDA))
			{
				material = dictionary[AOHHPLGIPDA];
			}
			else
			{
				material = new Material((!IPEKLPADIMF) ? _Shader : CKOPCEGGGDJ);
				material.mainTexture = AOHHPLGIPDA;
				dictionary.Add(AOHHPLGIPDA, material);
			}
			return material;
		}

		public void set_FrameRect(Rect value)
		{
			_FrameRect = value;
			GOKLJDNDLFG();
		}

		public void SetWidthHeight(float JGAPNGHPJGJ, float ANEFPJNALLK)
		{
			MIHBPJAFIOC = JGAPNGHPJGJ;
			PHJHMBHCDEB = ANEFPJNALLK;
			GCAOLAHLFBM();
		}

		public void Start()
		{
			if (_Mesh == null)
			{
				_Mesh = new Mesh();
				GCAOLAHLFBM();
				GOKLJDNDLFG();
				_Mesh.triangles = new int[6] { 0, 1, 2, 1, 3, 2 };
				_Mesh.colors = new Color[4] { _Color, _Color, _Color, _Color };
			}
			base.gameObject.AddComponent<MeshFilter>().mesh = _Mesh;
			_MeshRender = base.gameObject.AddComponent<MeshRenderer>();
			_MeshRender.shadowCastingMode = ShadowCastingMode.Off;
			_MeshRender.receiveShadows = false;
			_MeshRender.lightProbeUsage = LightProbeUsage.Off;
			_MeshRender.reflectionProbeUsage = ReflectionProbeUsage.Off;
			if (_Texture != null)
			{
				set_Texture(_Texture);
			}
			else
			{
				_MeshRender.sharedMaterial = _SharedMaterial;
			}
		}

		private void OnDestroy()
		{
			if (_Texture != null && MLLLJIJMCGH.ContainsKey(_Texture))
			{
				MLLLJIJMCGH.Remove(_Texture);
			}
			_Texture = null;
			JIBGIDGLPKK = null;
		}

		private void GOKLJDNDLFG()
		{
			if (!(_Mesh == null))
			{
				Vector2[] uv = new Vector2[4]
				{
					new Vector2(_FrameRect.xMin, 1f - _FrameRect.yMax),
					new Vector2(_FrameRect.xMax, 1f - _FrameRect.yMax),
					new Vector2(_FrameRect.xMin, 1f - _FrameRect.yMin),
					new Vector2(_FrameRect.xMax, 1f - _FrameRect.yMin)
				};
				_Mesh.uv = uv;
			}
		}

		private void GCAOLAHLFBM()
		{
			if (!(_Mesh == null))
			{
				_Mesh.vertices = new Vector3[4]
				{
					new Vector3(0f, PHJHMBHCDEB, 0f),
					new Vector3(MIHBPJAFIOC, PHJHMBHCDEB, 0f),
					new Vector3(0f, 0f, 0f),
					new Vector3(MIHBPJAFIOC, 0f, 0f)
				};
				_Mesh.RecalculateBounds();
			}
		}
	}
}
