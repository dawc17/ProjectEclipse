using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIFigures
{
	public class UISkewRect : UIFigure
	{
		[SerializeField]
		protected float _SkewAngle;

		protected List<UIVertex> _Vertexes = new List<UIVertex>();

		protected override void OnPopulateMesh(VertexHelper DHJBOKKAOJK)
		{
			base.OnPopulateMesh(DHJBOKKAOJK);
			Vector2 pEEOEOMEBFG = new Vector2(base.rectTransform.rect.width, base.rectTransform.rect.height);
			Draw(DHJBOKKAOJK, pEEOEOMEBFG, base.rectTransform.pivot);
		}

		protected void Draw(VertexHelper DHJBOKKAOJK, Vector2 PEEOEOMEBFG, Vector2 BBKPOIGBHPI)
		{
			DHJBOKKAOJK.Clear();
			_Vertexes.Clear();
			float f = (float)Math.PI * _SkewAngle / 180f;
			float num = PEEOEOMEBFG.y * Mathf.Tan(f);
			Vector2 vector = new Vector2((0f - PEEOEOMEBFG.x) * BBKPOIGBHPI.x, (0f - PEEOEOMEBFG.y) * BBKPOIGBHPI.y);
			Vector2 vector2 = new Vector2((0f - PEEOEOMEBFG.x) * BBKPOIGBHPI.x + num, PEEOEOMEBFG.y * (1f - BBKPOIGBHPI.y));
			Vector2 vector3 = new Vector2(PEEOEOMEBFG.x * (1f - BBKPOIGBHPI.x), PEEOEOMEBFG.y * (1f - BBKPOIGBHPI.y));
			Vector2 vector4 = new Vector2(PEEOEOMEBFG.x * (1f - BBKPOIGBHPI.x) - num, (0f - PEEOEOMEBFG.y) * BBKPOIGBHPI.y);
			AddVertex(vector);
			AddVertex(vector2);
			AddVertex(vector3);
			AddVertex(vector4);
			DHJBOKKAOJK.AddUIVertexQuad(_Vertexes.ToArray());
		}

		protected void AddVertex(Vector3 GIAEPIIIMDH)
		{
			UIVertex simpleVert = UIVertex.simpleVert;
			simpleVert.position = GIAEPIIIMDH;
			simpleVert.uv0 = new Vector2(0.5f + GIAEPIIIMDH.x / base.rectTransform.rect.width, 0.5f + GIAEPIIIMDH.y / base.rectTransform.rect.height);
			simpleVert.color = color;
			_Vertexes.Add(simpleVert);
		}
	}
}
