using System;
using System.Collections.Generic;
using Eclipse.UI.TopBar;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Menu
{
	public class MenuMoneyPanel : SFMonoBehaviour<object>
	{
		public enum HGPAPGPHFFH
		{
			onRubyBtnClicked = 0
		}

		public enum BGOFFGFBDEJ
		{
			LocalValues = 0,
			ServerValues = 1
		}

		private enum GDGGJGEMEHE
		{
			NormalView = 0,
			ForgeView = 1
		}

		public const float SALE_ANIM_TIME = 0.03f;

		public const float SALE_ANIM_PAUSE = 5f;

		private GDGGJGEMEHE LBOOAELPANA;

		private long PLBBEIGAJEP = -1L;

		private float MLLLPDIIIHN;

		private float MALGIPGEANF;

		[SerializeField]
		private Text _infoCoins;

		[SerializeField]
		private ResolutionImage _iconCoins;

		[SerializeField]
		private Text _infoBonus;

		[SerializeField]
		private ResolutionImage _iconBonus;

		[SerializeField]
		private Button _btnRuby;

		[SerializeField]
		private Button _btnServerValues;

		[SerializeField]
		private Button _btnGoToShop;

		[SerializeField]
		private ImageAnimation _picRubySale;

		private long INLLPDEFCGI = -1L;

		private BGOFFGFBDEJ FNKPJFAMLEH;

		public void Init()
		{
			IPMGLHLKLHK();
			bool flag = SystemProperties.DBBOCENKMGD();
			bool flag2 = false;
			if (flag || flag2)
			{
				_btnServerValues.onClick.AddListener(() =>
				{
					IJDLFOINEIF();
				});
			}
			KPKHFKNGAMJ();
		}

		public void ConfigureCompactTopBar()
		{
			DesktopTopBarLayout.ConfigureMoneyPanel(this, _btnRuby, _btnGoToShop, _picRubySale,
				_iconCoins.rectTransform, _infoCoins.rectTransform, _iconBonus.rectTransform, _infoBonus.rectTransform);
		}

		private void CHILAIJNEHG()
		{
			_btnServerValues.onClick.RemoveListener(() =>
			{
				IJDLFOINEIF();
			});
		}

		private void KPKHFKNGAMJ()
		{
			if (_iconCoins != null && _iconCoins.get_SpriteName() != ListSF.CCDKHLAMKKO().OGJBDMNBMLJ())
			{
				_iconCoins.set_SpriteName(ListSF.CCDKHLAMKKO().OGJBDMNBMLJ());
			}
		}

		public void UpdateValues()
		{
			KPKHFKNGAMJ();
			long num = ListSF.CCDKHLAMKKO().BFBOEGMAMNF();
			_infoCoins.text = num.ToString();
			long pLBBEIGAJEP = PLBBEIGAJEP;
			PLBBEIGAJEP = ListSF.CCDKHLAMKKO().EHFJHFDACMP();
			if (FNKPJFAMLEH != BGOFFGFBDEJ.ServerValues)
			{
				_infoBonus.text = PLBBEIGAJEP.ToString();
			}
		}

		public void UpdateRuby()
		{
			string empty = string.Empty;
			if (FNKPJFAMLEH == BGOFFGFBDEJ.LocalValues)
			{
				if (MALGIPGEANF != 0f)
				{
					if (Math.Abs(MALGIPGEANF) > Math.Abs(MLLLPDIIIHN))
					{
						MALGIPGEANF -= MLLLPDIIIHN;
					}
					else
					{
						MALGIPGEANF = 0f;
					}
					empty = ((int)((float)PLBBEIGAJEP - MALGIPGEANF)/*cast due to constrained. prefix*/).ToString();
				}
				else
				{
					empty = PLBBEIGAJEP.ToString();
				}
			}
			else
			{
				bool flag = false;
				int num = 0;
				empty = ((!flag) ? (-1) : num).ToString();
			}
			if (_infoBonus.text != empty)
			{
				_infoBonus.text = empty;
			}
		}

		public void UpdateRubySale()
		{
			List<ItemInfo> list = new List<ItemInfo>();
			long bAINMLLIKOL = -1L;
			for (int i = 0; i < list.Count; i++)
			{
				ItemInfo dJKEECEOCJB = list[i];
			}
			SetSaleEndTime(bAINMLLIKOL);
		}

		public void SetRubyBtnPressType(NFOGOFFAPPP.HHGPKAJENGF LFLGCDNKNJI, bool GHJGPAEDIHG)
		{
			_btnRuby.OFPNNIBBNCE(LFLGCDNKNJI, GHJGPAEDIHG);
		}

		public void SetServerValuesBtnPressType(NFOGOFFAPPP.HHGPKAJENGF LFLGCDNKNJI, bool GHJGPAEDIHG)
		{
			_btnServerValues.OFPNNIBBNCE(LFLGCDNKNJI, GHJGPAEDIHG);
		}

		public void SetNormalViewMode()
		{
			LBOOAELPANA = GDGGJGEMEHE.NormalView;
			_iconCoins.gameObject.SetActive(true);
			_infoCoins.gameObject.SetActive(true);
		}

		public void SetForgeViewMode()
		{
			LBOOAELPANA = GDGGJGEMEHE.ForgeView;
			_iconCoins.gameObject.SetActive(false);
			_infoCoins.gameObject.SetActive(false);
		}

		public float GetRubyIconLeftEdgePosX()
		{
			float x = _iconBonus.transform.position.x;
			float x2 = base.transform.position.x;
			float num = x2 + x;
			float width = _iconBonus.rectTransform.rect.width;
			return num - width / 2f;
		}

		public Button GetRubyBtn()
		{
			return _btnRuby;
		}

		public Button GetServerValuesBtn()
		{
			return _btnServerValues;
		}

		public BGOFFGFBDEJ GetValuesSource()
		{
			return FNKPJFAMLEH;
		}

		public void SetValuesSource(BGOFFGFBDEJ PPJEFKEKAAC)
		{
			FNKPJFAMLEH = PPJEFKEKAAC;
			UpdateValues();
		}

		private void IPMGLHLKLHK()
		{
			_btnRuby.onClick.AddListener(FANJLLMEOEJ);
			_btnGoToShop.onClick.AddListener(FANJLLMEOEJ);
		}

		private void FANJLLMEOEJ()
		{
			_btnRuby.enabled = false;
			MainMenu.BGGGJCMEGPH bGGGJCMEGPH = MainMenu.BGGGJCMEGPH.MENU_MONEY;
			CallEvent(0, bGGGJCMEGPH);
		}

		private void IJDLFOINEIF()
		{
			if (FNKPJFAMLEH == BGOFFGFBDEJ.LocalValues)
			{
				SetValuesSource(BGOFFGFBDEJ.ServerValues);
			}
			else
			{
				SetValuesSource(BGOFFGFBDEJ.LocalValues);
			}
			UpdateRubySale();
		}

		private void SetSaleEndTime(long value)
		{
			if (INLLPDEFCGI != value)
			{
				INLLPDEFCGI = value;
				bool flag = false;
			}
		}

		private void FLICCNJBGDH(float data)
		{
		}
	}
}
