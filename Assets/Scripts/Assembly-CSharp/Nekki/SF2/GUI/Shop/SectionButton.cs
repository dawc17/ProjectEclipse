using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Shop
{
	public class SectionButton : SFButton
	{
		[SerializeField]
		private ResolutionImage _newItemsCircle;

		[SerializeField]
		private ResolutionImage _newItemsEllipse;

		[SerializeField]
		private LabelAlias _newItemsLabel;

		private bool FGDFEHNEGCF;

		private int BONAMONOIIC;

		public int HMFKGPPNJEP
		{
			get
			{
				return get_NewItemsCount();
			}
			set
			{
				set_NewItemsCount(value);
			}
		}

		public int get_NewItemsCount()
		{
			return BONAMONOIIC;
		}

		public void set_NewItemsCount(int value)
		{
			BONAMONOIIC = value;
			PFLEKAHHIDB();
		}

		private void PFLEKAHHIDB()
		{
			if (_newItemsCircle == null || _newItemsEllipse == null || _newItemsLabel == null)
			{
				LLLOJBFMONN.Error("SectionButton.UpdateNewItemsIndicator some field is null");
				return;
			}
			if (1 > BONAMONOIIC)
			{
				_newItemsCircle.gameObject.SetActive(false);
				_newItemsEllipse.gameObject.SetActive(false);
				_newItemsLabel.gameObject.SetActive(false);
			}
			else if (10 > BONAMONOIIC)
			{
				_newItemsCircle.gameObject.SetActive(true);
				_newItemsEllipse.gameObject.SetActive(false);
				_newItemsLabel.gameObject.SetActive(true);
			}
			else
			{
				_newItemsCircle.gameObject.SetActive(false);
				_newItemsEllipse.gameObject.SetActive(true);
				_newItemsLabel.gameObject.SetActive(true);
			}
			_newItemsLabel.set_text(BONAMONOIIC.ToString());
		}

		private void BMJDFBAGEDG()
		{
			SpriteState spriteState = base.spriteState;
			Sprite sprite = PPBEKKDIJKC(spriteState.highlightedSprite);
			if (sprite != null)
			{
				spriteState.highlightedSprite = sprite;
			}
			sprite = PPBEKKDIJKC(spriteState.disabledSprite);
			if (sprite != null)
			{
				spriteState.disabledSprite = sprite;
			}
			sprite = PPBEKKDIJKC(spriteState.pressedSprite);
			if (sprite != null)
			{
				spriteState.pressedSprite = sprite;
			}
			FGDFEHNEGCF = true;
		}

		private Sprite PPBEKKDIJKC(Sprite GBIOHMNNEJI)
		{
			if (GBIOHMNNEJI != null)
			{
				Sprite sprite = ResolutionImage.GetSprite(string.Empty, GBIOHMNNEJI.name);
				if (sprite != null && GBIOHMNNEJI != sprite)
				{
					return sprite;
				}
			}
			return null;
		}

		protected override void DoStateTransition(SelectionState state, bool PJHFBFHIGNN)
		{
			if (!FGDFEHNEGCF)
			{
				BMJDFBAGEDG();
			}
			base.DoStateTransition(state, PJHFBFHIGNN);
		}
	}
}
