using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI
{
	[AddComponentMenu("UI_Nekki/ResolutionButton")]
	public class ResolutionButton : SFButton
	{
		protected override void Awake()
		{
			if (base.targetGraphic == null)
			{
				base.targetGraphic = base.gameObject.GetComponent<ResolutionImage>();
			}
			FMMBMFBAFKJ();
		}

		private void FMMBMFBAFKJ()
		{
			SpriteState spriteState = base.spriteState;
			GCOCMIMOIBH(spriteState.disabledSprite);
			GCOCMIMOIBH(spriteState.highlightedSprite);
			GCOCMIMOIBH(spriteState.pressedSprite);
			base.spriteState = spriteState;
		}

		private void GCOCMIMOIBH(Sprite GBIOHMNNEJI)
		{
			if (!(GBIOHMNNEJI == null))
			{
				string texturePath = ResolutionImage.GetTexturePath(GBIOHMNNEJI);
				string jGIGOMLGLPN = GBIOHMNNEJI.name;
				GBIOHMNNEJI = ResolutionImage.GetSprite(texturePath, jGIGOMLGLPN);
			}
		}

		public void SetDisabledSprite(Sprite GBIOHMNNEJI)
		{
			SpriteState spriteState = base.spriteState;
			spriteState.disabledSprite = GBIOHMNNEJI;
			GCOCMIMOIBH(spriteState.disabledSprite);
			base.spriteState = spriteState;
		}

		public void SetDisabledSprite(string texturePath, string JGIGOMLGLPN)
		{
			SpriteState spriteState = base.spriteState;
			spriteState.disabledSprite = ResolutionImage.GetSprite(texturePath, JGIGOMLGLPN);
			base.spriteState = spriteState;
		}

		public void SetHighlightedSprite(Sprite GBIOHMNNEJI)
		{
			SpriteState spriteState = base.spriteState;
			spriteState.highlightedSprite = GBIOHMNNEJI;
			GCOCMIMOIBH(spriteState.highlightedSprite);
			base.spriteState = spriteState;
		}

		public void SetHighlightedSprite(string texturePath, string JGIGOMLGLPN)
		{
			SpriteState spriteState = base.spriteState;
			spriteState.highlightedSprite = ResolutionImage.GetSprite(texturePath, JGIGOMLGLPN);
			base.spriteState = spriteState;
		}

		public void SetPressedSprite(Sprite GBIOHMNNEJI)
		{
			SpriteState spriteState = base.spriteState;
			spriteState.pressedSprite = GBIOHMNNEJI;
			GCOCMIMOIBH(spriteState.pressedSprite);
			base.spriteState = spriteState;
		}

		public void SetPressedSprite(string texturePath, string JGIGOMLGLPN)
		{
			SpriteState spriteState = base.spriteState;
			spriteState.pressedSprite = ResolutionImage.GetSprite(texturePath, JGIGOMLGLPN);
			base.spriteState = spriteState;
		}

		public void SetNormalSprite(string texturePath, string JGIGOMLGLPN)
		{
			ResolutionImage resolutionImage = base.targetGraphic as ResolutionImage;
			resolutionImage.set_TexturePath(texturePath);
			resolutionImage.set_SpriteName(JGIGOMLGLPN);
		}
	}
}
