using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Fight
{
	public class DisplayModel : MonoBehaviour
	{
		[SerializeField]
		private ResolutionImageAvatar avatar;

		[SerializeField]
		private ResolutionImage complete;

		[SerializeField]
		private LayoutElement layoutElement;

		private string texturePath = SF2Paths.BHCPOOOJAAK();

		public ResolutionImageAvatar HNKFHGOOKEG
		{
			get
			{
				return get_Avatar();
			}
		}

		public ResolutionImageAvatar get_Avatar()
		{
			return avatar;
		}

		public ResolutionImage get_Complete()
		{
			return complete;
		}

		public void SetAvatar(string OEJJMOBGPLG)
		{
			if (avatar != null)
			{
				avatar.set_TexturePath(texturePath);
				avatar.set_SpriteName(OEJJMOBGPLG);
				avatar.SetNativeSize();
			}
		}

		public void Completed()
		{
			if (complete != null)
			{
				complete.gameObject.SetActive(true);
			}
		}

		public void ScaleAvatar(Vector2 JPJGNKGEHPI)
		{
			if (avatar != null)
			{
				avatar.transform.localScale = JPJGNKGEHPI;
				RectTransform rectTransform = base.transform as RectTransform;
				if (rectTransform != null && layoutElement != null)
				{
					Vector2 size = avatar.rectTransform.rect.size;
					size.x *= JPJGNKGEHPI.x;
					size.y *= JPJGNKGEHPI.y;
					rectTransform.sizeDelta = size;
					layoutElement.minWidth = size.x;
					layoutElement.minHeight = size.y;
				}
			}
			if (complete != null)
			{
				complete.transform.localScale = JPJGNKGEHPI;
			}
		}

		public void SetSizeDelta(Vector2 MHLKKEPFMIF)
		{
			RectTransform rectTransform = base.transform as RectTransform;
			if (rectTransform != null && layoutElement != null)
			{
				rectTransform.sizeDelta = MHLKKEPFMIF;
				layoutElement.minWidth = MHLKKEPFMIF.x;
				layoutElement.minHeight = MHLKKEPFMIF.y;
			}
		}
	}
}
