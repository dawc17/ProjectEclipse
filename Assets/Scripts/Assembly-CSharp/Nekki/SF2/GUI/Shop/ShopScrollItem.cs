using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Shop
{
	public class ShopScrollItem : BaseScrollItem, IComparable<ShopScrollItem>
	{
		private string _texturePath = SF2Paths.LFIIMPEAMFG();

		[SerializeField]
		private ResolutionImage _lockIcon;

		[SerializeField]
		private ResolutionImage _jackdawIcon;

		[SerializeField]
		private ResolutionImage _equppiedIcon;

		[SerializeField]
		private ResolutionImage _levelIcon;

		[SerializeField]
		private Text _levelLabel;

		[SerializeField]
		private ResolutionImage _image;

		[SerializeField]
		private PerksPanel _perksPanel;

		[SerializeField]
		private LayoutElement _iconPanel;

		[SerializeField]
		private LayoutElement _layoutElement;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Vector2 LMFNOHNBCMC;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int LKDFKFHAOOC;

		private ItemInfo JMPPBCFDOLL;

		private float _incraseHeight;

		public PerksPanel FNKBLODMEKA
		{
			get
			{
				return get_PerksPanel();
			}
		}

		public LayoutElement CMFIABIFDDD
		{
			get
			{
				return get_LayoutElement();
			}
		}

		public Vector2 MLIBBLGEHJI
		{
			get
			{
				return get_BaseSize();
			}
			set
			{
				set_BaseSize(value);
			}
		}

		public bool FNNPPCNDLNK
		{
			get
			{
				return get_IconPanelActive();
			}
			set
			{
				set_IconPanelActive(value);
			}
		}

		public override Vector3 GEHBDCJNJMJ
		{
			get
			{
				return get_CenterPosition();
			}
		}

		public override float NLLBLGNNFBA
		{
			set
			{
				set_Opacity(value);
			}
		}

		public ItemInfo OFMCNLBFIDF
		{
			get
			{
				return get_ItemInfo();
			}
		}

		public float PJOANAMPNNO
		{
			get
			{
				return get_IncraseHeight();
			}
			set
			{
				set_IncraseHeight(value);
			}
		}

		public PerksPanel get_PerksPanel()
		{
			return _perksPanel;
		}

		public LayoutElement get_LayoutElement()
		{
			return _layoutElement;
		}

		public Vector2 get_BaseSize()
		{
			return LMFNOHNBCMC;
		}

		public void set_BaseSize(Vector2 value)
		{
			LMFNOHNBCMC = value;
		}

		public bool get_IconPanelActive()
		{
			return _iconPanel != null && _iconPanel.gameObject.activeSelf;
		}

		public void set_IconPanelActive(bool value)
		{
			if (_iconPanel != null)
			{
				_iconPanel.gameObject.SetActive(value);
				GNBDBHEPKML();
			}
		}

		public int get_Index()
		{
			return LKDFKFHAOOC;
		}

		public void set_Index(int value)
		{
			LKDFKFHAOOC = value;
		}

		public override Vector3 get_CenterPosition()
		{
			Vector3 position = base.transform.position;
			if (_image != null)
			{
				position.y = _image.transform.position.y;
			}
			return position;
		}

		public override void set_Opacity(float value)
		{
			MGPPBIADMJM = value;
			if (MGPPBIADMJM < PNOCLNNCEBB)
			{
				MGPPBIADMJM = PNOCLNNCEBB;
			}
			if (MGPPBIADMJM > BFOEJPPDBAA)
			{
				MGPPBIADMJM = BFOEJPPDBAA;
			}
			GIMBAAJGDEN();
		}

		public ItemInfo get_ItemInfo()
		{
			return JMPPBCFDOLL;
		}

		public float get_IncraseHeight()
		{
			return _incraseHeight;
		}

		public void set_IncraseHeight(float value)
		{
			_incraseHeight = value;
			GNBDBHEPKML();
		}

		public void SetItemInfo(ItemInfo item)
		{
			if (item == null)
			{
				LLLOJBFMONN.Error("ShopScrollItem.SetItemInfo item is null");
				return;
			}
			set_Name(item.Name);
			base.gameObject.name = string.Format("ShopScrollItem({0})", get_Name());
			UserItem dKCHDHMLKHN = ListSF.CCDKHLAMKKO().KHCNHPCPFII().CMGOCLGHNLH(item);
			JMPPBCFDOLL = ((dKCHDHMLKHN == null) ? item : dKCHDHMLKHN.AKKBIFEFDCI());
			set_MaxOpacity((item.MHGODOLNDLE <= ListSF.CCDKHLAMKKO().PINDEKDNCNL()) ? 1f : 0.5f);
			if (_lockIcon != null)
			{
				if (item.MHGODOLNDLE > ListSF.CCDKHLAMKKO().PINDEKDNCNL())
				{
					_lockIcon.gameObject.SetActive(true);
				}
				else
				{
					_lockIcon.gameObject.SetActive(false);
				}
			}
			if (_jackdawIcon != null)
			{
				bool flag = dKCHDHMLKHN != null;
				_jackdawIcon.gameObject.SetActive(flag && item.Type != "Seal");
			}
			if (_equppiedIcon != null)
			{
				bool active = dKCHDHMLKHN != null && dKCHDHMLKHN.EFMFGEPDAOP();
				_equppiedIcon.gameObject.SetActive(active);
			}
			bool active2 = JMPPBCFDOLL.MHGODOLNDLE > 0;
			if (_levelLabel != null)
			{
				Font font = LocalizationManager.MBPJIKFOEBJ();
				if (font != null)
				{
					_levelLabel.font = font;
				}
				_levelLabel.text = JMPPBCFDOLL.MHGODOLNDLE.ToString();
				_levelLabel.gameObject.SetActive(active2);
			}
			if (_levelIcon != null)
			{
				_levelIcon.gameObject.SetActive(active2);
			}
			if (_image != null)
			{
				if (item.Type == "Seal")
				{
					_image.set_TexturePath(SF2Paths.BHCPOOOJAAK());
				}
				else
				{
					_image.set_TexturePath(_texturePath);
				}
				_image.set_SpriteName(item.FileName);
				_image.SetNativeSize();
			}
			else
			{
				LLLOJBFMONN.Error("ShopScrollItem.SetItemInfo _image is null");
			}
			if (_perksPanel != null)
			{
				_perksPanel.SetPerks(ListSF.EIMKEJNJMEJ(JMPPBCFDOLL));
			}
			GNBDBHEPKML();
			GIMBAAJGDEN();
		}

		private void GIMBAAJGDEN()
		{
			if (_image != null)
			{
				UIExtensions.HNIHBGAOAIH(_image, MGPPBIADMJM);
			}
			if (_jackdawIcon != null)
			{
				UIExtensions.HNIHBGAOAIH(_jackdawIcon, MGPPBIADMJM);
			}
			if (_equppiedIcon != null)
			{
				UIExtensions.HNIHBGAOAIH(_equppiedIcon, MGPPBIADMJM);
			}
			if (_levelIcon != null)
			{
				UIExtensions.HNIHBGAOAIH(_levelIcon, MGPPBIADMJM);
			}
			if (_levelLabel != null)
			{
				_levelLabel.HNIHBGAOAIH(MGPPBIADMJM);
			}
		}

		private void GNBDBHEPKML()
		{
			if (_layoutElement != null && _iconPanel != null)
			{
				RectTransform rectTransform = (RectTransform)base.transform;
				Vector2 vector = get_BaseSize();
				if (vector == new Vector2(0f, 0f) && _image != null)
				{
					vector = ((RectTransform)_image.transform).sizeDelta;
				}
				vector.y += _incraseHeight;
				if (_iconPanel.gameObject.activeSelf)
				{
					vector.y += _iconPanel.minHeight;
				}
				rectTransform.sizeDelta = vector;
				_layoutElement.minWidth = vector.x;
				_layoutElement.minHeight = vector.y;
			}
			else
			{
				LLLOJBFMONN.Error("ShopScrollItem.SetItemInfo _layoutElement or rectTransform is null");
			}
		}

		public void UpdateItem()
		{
			SetItemInfo(JMPPBCFDOLL);
		}

		public int CompareTo(ShopScrollItem NOLFMPDGCOC)
		{
			int num = ((JMPPBCFDOLL != null) ? JMPPBCFDOLL.MHGODOLNDLE : 0);
			int value = ((NOLFMPDGCOC.JMPPBCFDOLL != null) ? NOLFMPDGCOC.JMPPBCFDOLL.MHGODOLNDLE : 0);
			int num2 = ((JMPPBCFDOLL != null) ? JMPPBCFDOLL.Index : 0);
			int value2 = ((NOLFMPDGCOC.JMPPBCFDOLL == null) ? 1 : NOLFMPDGCOC.JMPPBCFDOLL.Index);
			int num3 = num.CompareTo(value);
			if (num3 != 0)
			{
				return num3;
			}
			return num2.CompareTo(value2);
		}
	}
}
