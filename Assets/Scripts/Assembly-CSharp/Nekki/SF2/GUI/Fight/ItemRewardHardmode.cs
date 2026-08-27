using UnityEngine;

namespace Nekki.SF2.GUI.Fight
{
	public class ItemRewardHardmode : MonoBehaviour
	{
		[SerializeField]
		private ResolutionImage icon;

		private void Start()
		{
		}

		private void Update()
		{
		}

		public void setIcon(ItemInfo item)
		{
			if (icon != null)
			{
				icon.set_TexturePath(SF2Paths.LFIIMPEAMFG());
				icon.set_SpriteName(item.FileName);
			}
		}
	}
}
