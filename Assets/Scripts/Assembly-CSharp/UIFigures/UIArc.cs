using System;
using UnityEngine;
using UnityEngine.UI;

namespace UIFigures
{
	public class UIArc : UIFigure
	{
		[SerializeField]
		[Range(2f, 50f)]
		private int _Segments = 10;

		[SerializeField]
		[Range(0f, (float)Math.PI * 2f)]
		private float _From;

		[SerializeField]
		[Range(0f, (float)Math.PI * 2f)]
		private float _To = (float)Math.PI / 2f;

		protected override void OnPopulateMesh(VertexHelper DHJBOKKAOJK)
		{
			base.OnPopulateMesh(DHJBOKKAOJK);
			Vector2 gIAEPIIIMDH = (OMPIACGGOAC + PMBHNNBJNKL) * 0.5f;
			Vector2 lPEMPCEJFIN = new Vector2(base.rectTransform.rect.width, base.rectTransform.rect.height) * 0.5f;
			DrawFunctions.CJOMPMCKCJP(DHJBOKKAOJK, gIAEPIIIMDH, lPEMPCEJFIN, _To, _From, _Segments, color);
		}
	}
}
