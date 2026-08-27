using UnityEngine;
using UnityEngine.UI;

namespace UIFigures
{
	[ExecuteInEditMode]
	public class UIFigure : MaskableGraphic
	{
		public Sprite _Sprite;

		protected Vector2 OMPIACGGOAC;

		protected Vector2 PMBHNNBJNKL;

		public override Texture mainTexture
		{
			get
			{
				return base.mainTexture;
			}
		}

		protected override void OnPopulateMesh(VertexHelper DHJBOKKAOJK)
		{
			OMPIACGGOAC = new Vector2(0f - base.rectTransform.pivot.x, 0f - base.rectTransform.pivot.y);
			PMBHNNBJNKL = new Vector2(1f - base.rectTransform.pivot.x, 1f - base.rectTransform.pivot.y);
			OMPIACGGOAC.x *= base.rectTransform.rect.width;
			OMPIACGGOAC.y *= base.rectTransform.rect.height;
			PMBHNNBJNKL.x *= base.rectTransform.rect.width;
			PMBHNNBJNKL.y *= base.rectTransform.rect.height;
		}

		public void Refresh()
		{
			SetVerticesDirty();
		}
	}
}
