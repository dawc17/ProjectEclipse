using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIFigures
{
	public class UIFrameRectBorder : UIFigure
	{
		[SerializeField]
		private float _AngelSize;

		[SerializeField]
		private float _Width = 10f;

		private List<Vector2> _Points = new List<Vector2>();

		protected override void OnPopulateMesh(VertexHelper DHJBOKKAOJK)
		{
			base.OnPopulateMesh(DHJBOKKAOJK);
			Vector2 vector = new Vector2(base.rectTransform.rect.width, base.rectTransform.rect.height) * 0.5f;
			vector.x -= _Width / 2f;
			vector.y -= _Width / 2f;
			_Points.Clear();
			_Points.Add(new Vector2(0f, vector.y));
			_Points.Add(new Vector2(vector.x - (_AngelSize - _Width / 2f * 0.585786f), vector.y));
			_Points.Add(new Vector2(vector.x, vector.y - (_AngelSize - _Width / 2f * 0.585786f)));
			_Points.Add(new Vector2(vector.x, 0f - vector.y + (_AngelSize - _Width / 2f * 0.585786f)));
			_Points.Add(new Vector2(vector.x - (_AngelSize - _Width / 2f * 0.585786f), 0f - vector.y));
			_Points.Add(new Vector2(0f - vector.x + (_AngelSize - _Width / 2f * 0.585786f), 0f - vector.y));
			_Points.Add(new Vector2(0f - vector.x, 0f - vector.y + (_AngelSize - _Width / 2f * 0.585786f)));
			_Points.Add(new Vector2(0f - vector.x, vector.y - (_AngelSize - _Width / 2f * 0.585786f)));
			_Points.Add(new Vector2(0f - vector.x + (_AngelSize - _Width / 2f * 0.585786f), vector.y));
			_Points.Add(new Vector2(0f, vector.y));
			DrawFunctions.FBFOFHOLLKI(DHJBOKKAOJK, _Points, _Width, color);
		}
	}
}
