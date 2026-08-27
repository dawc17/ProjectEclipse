using System.Collections.Generic;
using Nekki.SF2.GUI.Shop;
using UnityEngine;

namespace Nekki.SF2.GUI.Profile
{
	public class UserPerksSprite : SFMonoBehaviour<object>
	{
		private ProfileSliderItem NOELBEFDFFE;

		[SerializeField]
		private LabelAlias _label;

		private List<ProfilePerk> NDEOKNAOAKM = new List<ProfilePerk>();

		[SerializeField]
		protected Scroll _slider;

		[SerializeField]
		private BaseScrollContent _sliderContent;

		[SerializeField]
		private GameObject _perkItemPrefab;

		[SerializeField]
		private GameObject _profileSliderItemPrefab;

		public void Init()
		{
			_label.set_text(string.Empty);
			_label.set_LabelFontSize(83);
			_label.color = Constants.PJJIMHMJPAL;
			_label.alignment = TextAnchor.MiddleCenter;
			IEFFLCBGJJM();
			BDAJMIAPDOF();
			_label.rectTransform.sizeDelta = new Vector2(GetComponent<RectTransform>().rect.width - 120f, _label.rectTransform.rect.height);
			_label.set_Alias("ProfileNoPerks");
		}

		public void AddItem(PerkInfoItem ELHEKFLAIKM)
		{
			if (ELHEKFLAIKM == null)
			{
				return;
			}
			ProfilePerk pLKCIINIFMJ = ABAGJKMKCBA(ELHEKFLAIKM.Name);
			if (pLKCIINIFMJ != null)
			{
				pLKCIINIFMJ.NOLDHAFMOLF(ELHEKFLAIKM);
				pLKCIINIFMJ.set_Description(ELHEKFLAIKM.MGNNJPBCOGD);
				return;
			}
			ProfilePerk pLKCIINIFMJ2 = new ProfilePerk(ELHEKFLAIKM, 0, ProfilePerk.KMHBPKKCNPP.PERK_SELECTED, ProfilePerk.JHDKDOPHGOO.TYPE_PERK_SELETED);
			NDEOKNAOAKM.Add(pLKCIINIFMJ2);
			if (!(pLKCIINIFMJ2.CEENDGFFEFM() != string.Empty))
			{
				if (KMNLKGHEMIM())
				{
					GameObject gameObject = Object.Instantiate(_profileSliderItemPrefab);
					NOELBEFDFFE = gameObject.GetComponent<ProfileSliderItem>();
					NOELBEFDFFE.Init(15f);
					_slider.AddItem(NOELBEFDFFE);
				}
				GameObject gameObject2 = Object.Instantiate(_perkItemPrefab, NOELBEFDFFE.transform, false);
				PerkSubItem component = gameObject2.GetComponent<PerkSubItem>();
				component.Init(pLKCIINIFMJ2, 0);
				ProfileScene current = Scene<ProfileScene>.get_Current();
				if (current != null)
				{
					current.AddSubItem(component);
				}
				NOELBEFDFFE.AddIcons(component);
				LBPMCDKOFOE();
				if (_label != null)
				{
					_label.gameObject.SetActive(false);
				}
			}
		}

		public void Clear()
		{
			_slider.ClearItems();
			NDEOKNAOAKM.ForEach((ProfilePerk PIIEECCHMAC) =>
			{
				PIIEECCHMAC.RemoveAllEventListener();
			});
			NDEOKNAOAKM.Clear();
		}

		private void IEFFLCBGJJM()
		{
			_slider.Init(_sliderContent);
			_slider.get_ItemsScroll().AutoscrollIsOn = false;
		}

		private void BDAJMIAPDOF()
		{
			List<RosterPerk> list = ListSF.CCDKHLAMKKO().JLBDOBLHHAF().KEHFPLBNDHI();
			for (int i = 0; i < list.Count; i++)
			{
				AddItem(list[i].DFOELJAEEGG());
			}
		}

		private void LBPMCDKOFOE()
		{
			if (!(_slider == null))
			{
				int count = _slider.GetItems().Count;
				if (count > 3)
				{
					_slider.ScrollToItem(count - 2);
				}
			}
		}

		private bool KMNLKGHEMIM()
		{
			if (NOELBEFDFFE == null)
			{
				return true;
			}
			if (NOELBEFDFFE.GetIcons().Count >= 2)
			{
				return true;
			}
			return false;
		}

		private ProfilePerk ABAGJKMKCBA(string name)
		{
			ProfilePerk result = null;
			foreach (ProfilePerk item in NDEOKNAOAKM)
			{
				if (item.KAMBOKLFBEE() == name)
				{
					result = item;
					break;
				}
			}
			return result;
		}
	}
}
