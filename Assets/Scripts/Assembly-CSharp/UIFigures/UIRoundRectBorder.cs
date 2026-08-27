using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIFigures
{
	public class UIRoundRectBorder : UIFigure
	{
		[SerializeField]
		protected float _Width = 10f;

		[SerializeField]
		protected float _ChangeAllRadius = 10f;

		private float HIOFLBEBMAN = 10f;

		[SerializeField]
		protected float _RadiusUpLeft = 10f;

		[SerializeField]
		protected float _RadiusUpRight = 10f;

		[SerializeField]
		protected float _RadiusBottomRight = 10f;

		[SerializeField]
		protected float _RadiusBottomLeft = 10f;

		[Range(1f, 50f)]
		[SerializeField]
		protected int _Sectors = 10;

		protected List<UIVertex> _Vertexes = new List<UIVertex>();

		protected override void OnPopulateMesh(VertexHelper DHJBOKKAOJK)
		{
			base.OnPopulateMesh(DHJBOKKAOJK);
			Vector2 eFOBPHOJNJF = (OMPIACGGOAC + PMBHNNBJNKL) * 0.5f;
			Vector2 dGHIGGGFNLP = new Vector2(base.rectTransform.rect.width, base.rectTransform.rect.height) * 0.5f;
			Draw(DHJBOKKAOJK, eFOBPHOJNJF, dGHIGGGFNLP);
		}

		public void Draw(VertexHelper DHJBOKKAOJK, Vector2 EFOBPHOJNJF, Vector2 DGHIGGGFNLP)
		{
			DHJBOKKAOJK.Clear();
			_Vertexes.Clear();
			AddArc(new Vector2(DGHIGGGFNLP.x - _RadiusUpRight, DGHIGGGFNLP.y - _RadiusUpRight), _RadiusUpRight, 0f);
			AddArc(new Vector2(0f - DGHIGGGFNLP.x + _RadiusUpLeft, DGHIGGGFNLP.y - _RadiusUpLeft), _RadiusUpLeft, (float)Math.PI / 2f);
			AddArc(new Vector2(0f - DGHIGGGFNLP.x + _RadiusBottomLeft, 0f - DGHIGGGFNLP.y + _RadiusBottomLeft), _RadiusBottomLeft, (float)Math.PI);
			AddArc(new Vector2(DGHIGGGFNLP.x - _RadiusBottomRight, 0f - DGHIGGGFNLP.y + _RadiusBottomRight), _RadiusBottomRight, 4.712389f);
			AddVertex(new Vector2(DGHIGGGFNLP.x, DGHIGGGFNLP.y - _RadiusUpRight));
			AddVertex(new Vector2(DGHIGGGFNLP.x - _Width, DGHIGGGFNLP.y - _RadiusUpRight));
			DHJBOKKAOJK.AddUIVertexStream(_Vertexes, FigureTopology.AMJOJPPFIEB((_Sectors + 1) * 8));
		}

		protected void AddArc(Vector3 EFOBPHOJNJF, float LPEMPCEJFIN, float AMNCLCPADOO, bool MEFMAPOEPNE = true)
		{
			float num = (float)Math.PI / 2f / (float)_Sectors;
			for (int i = 0; i < _Sectors + 1; i++)
			{
				float jIGOJGPKGPO = AMNCLCPADOO + (float)i * num;
				AddSegment(EFOBPHOJNJF, LPEMPCEJFIN, jIGOJGPKGPO, MEFMAPOEPNE);
			}
		}

		protected void AddSegment(Vector2 EFOBPHOJNJF, float LPEMPCEJFIN, float JIGOJGPKGPO, bool MEFMAPOEPNE = true)
		{
			float num = Mathf.Cos(JIGOJGPKGPO);
			float num2 = Mathf.Sin(JIGOJGPKGPO);
			float x = num * LPEMPCEJFIN + EFOBPHOJNJF.x;
			float y = num2 * LPEMPCEJFIN + EFOBPHOJNJF.y;
			AddVertex(new Vector2(x, y));
			if (MEFMAPOEPNE)
			{
				x = num * (LPEMPCEJFIN - _Width) + EFOBPHOJNJF.x;
				y = num2 * (LPEMPCEJFIN - _Width) + EFOBPHOJNJF.y;
				AddVertex(new Vector2(x, y));
			}
		}

		protected void AddVertex(Vector3 GIAEPIIIMDH)
		{
			UIVertex simpleVert = UIVertex.simpleVert;
			simpleVert.position = GIAEPIIIMDH;
			simpleVert.uv0 = new Vector2(0.5f + GIAEPIIIMDH.x / base.rectTransform.rect.width, 0.5f + GIAEPIIIMDH.y / base.rectTransform.rect.height);
			simpleVert.color = color;
			_Vertexes.Add(simpleVert);
		}

		protected void Update()
		{
			if (Math.Abs(HIOFLBEBMAN - _ChangeAllRadius) > 0.01f)
			{
				HIOFLBEBMAN = _ChangeAllRadius;
				_RadiusBottomLeft = _ChangeAllRadius;
				_RadiusBottomRight = _ChangeAllRadius;
				_RadiusUpLeft = _ChangeAllRadius;
				_RadiusUpRight = _ChangeAllRadius;
			}
		}
	}
}
