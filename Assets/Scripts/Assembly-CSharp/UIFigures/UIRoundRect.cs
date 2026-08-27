using System;
using UnityEngine;
using UnityEngine.UI;

namespace UIFigures
{
	public class UIRoundRect : UIRoundRectBorder
	{
		protected override void OnPopulateMesh(VertexHelper DHJBOKKAOJK)
		{
			base.OnPopulateMesh(DHJBOKKAOJK);
			Vector2 eFOBPHOJNJF = (OMPIACGGOAC + PMBHNNBJNKL) * 0.5f;
			Vector2 dGHIGGGFNLP = new Vector2(base.rectTransform.rect.width, base.rectTransform.rect.height) * 0.5f;
			Draw(DHJBOKKAOJK, eFOBPHOJNJF, dGHIGGGFNLP);
		}

		public new void Draw(VertexHelper DHJBOKKAOJK, Vector2 EFOBPHOJNJF, Vector2 DGHIGGGFNLP)
		{
			DHJBOKKAOJK.Clear();
			_Vertexes.Clear();
			AddVertex(EFOBPHOJNJF);
			AddArc(new Vector2(DGHIGGGFNLP.x - _RadiusUpRight, DGHIGGGFNLP.y - _RadiusUpRight), _RadiusUpRight, 0f, false);
			AddArc(new Vector2(0f - DGHIGGGFNLP.x + _RadiusUpLeft, DGHIGGGFNLP.y - _RadiusUpLeft), _RadiusUpLeft, (float)Math.PI / 2f, false);
			AddArc(new Vector2(0f - DGHIGGGFNLP.x + _RadiusBottomLeft, 0f - DGHIGGGFNLP.y + _RadiusBottomLeft), _RadiusBottomLeft, (float)Math.PI, false);
			AddArc(new Vector2(DGHIGGGFNLP.x - _RadiusBottomRight, 0f - DGHIGGGFNLP.y + _RadiusBottomRight), _RadiusBottomRight, 4.712389f, false);
			AddVertex(new Vector2(DGHIGGGFNLP.x, DGHIGGGFNLP.y - _RadiusUpRight));
			DHJBOKKAOJK.AddUIVertexStream(_Vertexes, FigureTopology.NGPPLGNODNB((_Sectors + 1) * 4));
		}
	}
}
