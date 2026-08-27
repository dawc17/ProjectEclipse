using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Shop
{
	public class ShopTableViewCell : TableViewCell, IComparable<ShopTableViewCell>
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

		public Vector3 GEHBDCJNJMJ
		{
			get
			{
				return get_CenterPosition();
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

		public Vector3 get_CenterPosition()
		{
			Vector3 position = base.transform.position;
			if (_image != null)
			{
				position.y = _image.transform.position.y;
			}
			return position;
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
				LLLOJBFMONN.Error("ShopTableViewCell.SetItemInfo item is null");
				return;
			}
			base.gameObject.name = string.Format("ShopTableViewCell({0})", item.Name);
			UserItem dKCHDHMLKHN = ListSF.CCDKHLAMKKO().KHCNHPCPFII().CMGOCLGHNLH(item);
			JMPPBCFDOLL = ((dKCHDHMLKHN == null) ? item : dKCHDHMLKHN.AKKBIFEFDCI());
			_lockIcon.gameObject.SetActive(item.MHGODOLNDLE > ListSF.CCDKHLAMKKO().PINDEKDNCNL());
			_jackdawIcon.gameObject.SetActive(dKCHDHMLKHN != null && item.Type != "Seal");
			_equppiedIcon.gameObject.SetActive(dKCHDHMLKHN != null && dKCHDHMLKHN.EFMFGEPDAOP());
			bool active = JMPPBCFDOLL.MHGODOLNDLE > 0;
			Font font = LocalizationManager.MBPJIKFOEBJ();
			if (font != null)
			{
				_levelLabel.font = font;
			}
			_levelLabel.text = JMPPBCFDOLL.MHGODOLNDLE.ToString();
			_levelLabel.gameObject.SetActive(active);
			_levelIcon.gameObject.SetActive(active);
			_image.set_TexturePath((!(item.Type == "Seal")) ? _texturePath : SF2Paths.BHCPOOOJAAK());
			_image.set_SpriteName(item.FileName);
			if (item.Type == "Seal")
			{
				_image.rectTransform.sizeDelta = Constants.SEAL_SIZE;
			}
			else
			{
				_image.SetNativeSize();
			}
			_perksPanel.SetPerks(ListSF.EIMKEJNJMEJ(JMPPBCFDOLL));
			GNBDBHEPKML();
			GIMBAAJGDEN();
		}

		private void GIMBAAJGDEN()
		{
		}

		private void GNBDBHEPKML()
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

		public void UpdateItem()
		{
			SetItemInfo(JMPPBCFDOLL);
		}

		public int CompareTo(ShopTableViewCell NOLFMPDGCOC)
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

		public override void SetHighlighted()
		{
		}

		public override void SetSelected()
		{
		}

		public override void Display()
		{
		}
	}
}
