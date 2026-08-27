using System;
using UnityEngine;
using UnityEngine.UI;

namespace UIFigures
{
	public class UIArcBorder : UIFigure
	{
		[SerializeField]
		private bool _UseEndColor;

		[SerializeField]
		private Color _EndColor;

		[SerializeField]
		private float _Width = 10f;

		[SerializeField]
		[Range(3f, 50f)]
		private int _Segments = 10;

		[SerializeField]
		[Range(0f, (float)Math.PI * 2f)]
		private float _From;

		[SerializeField]
		[Range(0f, (float)Math.PI * 2f)]
		private float _To = (float)Math.PI / 2f;

		public float KBGFAKKBMCN
		{
			get
			{
				return get_Width();
			}
			set
			{
				set_Width(value);
			}
		}

		public int MENNILNNPHH
		{
			set
			{
				set_Segments(value);
			}
		}

		public float CLCFLPDNBNL
		{
			get
			{
				return get_From();
			}
			set
			{
				set_From(value);
			}
		}

		public float KAEAKHIEIHH
		{
			get
			{
				return get_To();
			}
			set
			{
				set_To(value);
			}
		}

		public Color BOHEMLDLGFP
		{
			set
			{
				set_SetAllColor(value);
			}
		}

		public float get_Width()
		{
			return _Width;
		}

		public void set_Width(float value)
		{
			_Width = value;
		}

		public void set_Segments(int value)
		{
			_Segments = value;
		}

		public void set_From(float value)
		{
			_From = value;
		}

		public float get_From()
		{
			return _From;
		}

		public void set_To(float value)
		{
			_To = value;
		}

		public float get_To()
		{
			return _To;
		}

		public void set_SetAllColor(Color value)
		{
			color = new Color(value.r, value.g, value.b, value.a);
			_EndColor = new Color(value.r, value.g, value.b, value.a);
		}

		protected override void OnPopulateMesh(VertexHelper DHJBOKKAOJK)
		{
			base.OnPopulateMesh(DHJBOKKAOJK);
			Vector2 gIAEPIIIMDH = (OMPIACGGOAC + PMBHNNBJNKL) * 0.5f;
			Vector2 lPEMPCEJFIN = new Vector2(base.rectTransform.rect.width, base.rectTransform.rect.height) * 0.5f;
			DrawFunctions.GAGFKBFLHHE(DHJBOKKAOJK, gIAEPIIIMDH, lPEMPCEJFIN, _Width, _To, _From, _Segments, color, (!_UseEndColor) ? color : _EndColor);
		}
	}
}
