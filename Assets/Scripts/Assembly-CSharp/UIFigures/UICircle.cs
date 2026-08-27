using System;
using UnityEngine;
using UnityEngine.UI;

namespace UIFigures
{
	public class UICircle : UIFigure
	{
		[Range(3f, 150f)]
		[SerializeField]
		private int _Segments = 10;

		protected override void OnPopulateMesh(VertexHelper DHJBOKKAOJK)
		{
			base.OnPopulateMesh(DHJBOKKAOJK);
			Vector2 gIAEPIIIMDH = (OMPIACGGOAC + PMBHNNBJNKL) * 0.5f;
			Vector2 lPEMPCEJFIN = new Vector2(base.rectTransform.rect.width, base.rectTransform.rect.height) * 0.5f;
			DrawFunctions.CJOMPMCKCJP(DHJBOKKAOJK, gIAEPIIIMDH, lPEMPCEJFIN, 0f, (float)Math.PI * 2f, _Segments, color);
		}
	}
}
