using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Shop
{
	public class ButtonPanel : MonoBehaviour
	{
		[SerializeField]
		private List<SectionButton> _buttons;

		[SerializeField]
		private ShopScene _shopScene;

		public void Init()
		{
			foreach (SectionButton button in _buttons)
			{
				if (!(button == null))
				{
					int i = _buttons.IndexOf(button);
					button.onClick.AddListener(() =>
					{
						OnClick((ShopSection)i);
					});
				}
			}
		}

		public void OnClick(ShopSection KGDHCBNKLMF)
		{
			if (_shopScene != null)
			{
				_shopScene.SetShopSection(KGDHCBNKLMF);
			}
		}

		public void HideButton(int index)
		{
			if (index < _buttons.Count)
			{
				Button button = _buttons[index];
				if ((bool)button)
				{
					button.gameObject.SetActive(false);
				}
			}
		}

		public void ShowButton(int index)
		{
			if (index < _buttons.Count)
			{
				Button button = _buttons[index];
				if ((bool)button)
				{
					button.gameObject.SetActive(true);
				}
			}
		}

		public void DisableButton(int index)
		{
			if (index < _buttons.Count)
			{
				Button button = _buttons[index];
				if ((bool)button)
				{
					button.interactable = false;
				}
			}
		}

		public void EnableAllButtons()
		{
			foreach (SectionButton button in _buttons)
			{
				if (!(button == null))
				{
					button.interactable = true;
				}
			}
		}

		public void UpdateNewItemsCounter()
		{
			foreach (SectionButton button in _buttons)
			{
				if (!(button == null))
				{
					int kGDHCBNKLMF = _buttons.IndexOf(button);
					string lFLGCDNKNJI = KHNCOFCHCCD((ShopSection)kGDHCBNKLMF);
					button.set_NewItemsCount(ListSF.DJBOFEEKJMP().GetCountNewItemsByType(lFLGCDNKNJI));
				}
			}
		}

		private string KHNCOFCHCCD(ShopSection KGDHCBNKLMF)
		{
			switch (KGDHCBNKLMF)
			{
			case ShopSection.Weapon:
				return "Weapon";
			case ShopSection.Armor:
				return "Armor";
			case ShopSection.Helmet:
				return "Helm";
			case ShopSection.Ranged:
				return "Ranged";
			case ShopSection.Magic:
				return "Magic";
			case ShopSection.Payment:
				return "RealMoneyItem";
			case ShopSection.Free:
				return "Free";
			default:
				return null;
			}
		}
	}
}
