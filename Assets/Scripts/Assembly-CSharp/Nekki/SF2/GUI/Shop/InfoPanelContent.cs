using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using Nekki.SF2.GUI.Dialogs;
using UnityEngine;
using UnityEngine.Events;

namespace Nekki.SF2.GUI.Shop
{
	public class InfoPanelContent : SidePanelContent
	{
		public class EHMHJDBJJPL : UnityEvent
		{
		}

		public EHMHJDBJJPL updateEvent = new EHMHJDBJJPL();

		[SerializeField]
		private LabelAlias _header;

		[SerializeField]
		private LabelAlias _description;

		[SerializeField]
		private LabelAlias _descriptionDonate;

		[SerializeField]
		private ParametersPanel _parametersPanel;

		[SerializeField]
		private GameObject _buttonPanel;

		[SerializeField]
		private GameObject _buttonPrefab;

		private IconLabelButton NDMPMIEKGAA;

		private IconLabelButton EPDPCAKGBBO;

		private IconLabelButton FNEMFKKBHLC;

		private IconLabelButton ABACDHNMNLJ;

		private IconLabelButton MAANHOHMEEO;

		private IconLabelButton MJOLEHNABAM;

		private IconLabelButton KFJFPNAFKLO;

		private List<IconLabelButton> _buttons = new List<IconLabelButton>();

		private ItemInfo JMPPBCFDOLL;

		private ItemInfo HLOPCCHHGHB;

		private UserItem NKBIOFJMONB;

		private bool FNFHDIPECME;

		private bool PPEJCMNKIGP;

		private bool KAPLEKBLLFO;

		private bool CJALINCMNCJ;

		public override void Init()
		{
			if (_buttonPrefab != null && _buttonPanel != null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(_buttonPrefab);
				gameObject.transform.SetParent(_buttonPanel.transform, false);
				NDMPMIEKGAA = gameObject.GetComponent<IconLabelButton>();
				_buttons.Add(NDMPMIEKGAA);
				gameObject = UnityEngine.Object.Instantiate(_buttonPrefab);
				gameObject.transform.SetParent(_buttonPanel.transform, false);
				EPDPCAKGBBO = gameObject.GetComponent<IconLabelButton>();
				_buttons.Add(EPDPCAKGBBO);
				gameObject = UnityEngine.Object.Instantiate(_buttonPrefab);
				gameObject.transform.SetParent(_buttonPanel.transform, false);
				FNEMFKKBHLC = gameObject.GetComponent<IconLabelButton>();
				_buttons.Add(FNEMFKKBHLC);
				gameObject = UnityEngine.Object.Instantiate(_buttonPrefab);
				gameObject.transform.SetParent(_buttonPanel.transform, false);
				ABACDHNMNLJ = gameObject.GetComponent<IconLabelButton>();
				_buttons.Add(ABACDHNMNLJ);
				gameObject = UnityEngine.Object.Instantiate(_buttonPrefab);
				gameObject.transform.SetParent(_buttonPanel.transform, false);
				MAANHOHMEEO = gameObject.GetComponent<IconLabelButton>();
				_buttons.Add(MAANHOHMEEO);
				gameObject = UnityEngine.Object.Instantiate(_buttonPrefab);
				gameObject.transform.SetParent(_buttonPanel.transform, false);
				MJOLEHNABAM = gameObject.GetComponent<IconLabelButton>();
				_buttons.Add(MJOLEHNABAM);
				gameObject = UnityEngine.Object.Instantiate(_buttonPrefab);
				gameObject.transform.SetParent(_buttonPanel.transform, false);
				KFJFPNAFKLO = gameObject.GetComponent<IconLabelButton>();
				_buttons.Add(KFJFPNAFKLO);
			}
			if (NDMPMIEKGAA != null)
			{
				NDMPMIEKGAA.get_Icon().set_SpriteName(ListSF.CCDKHLAMKKO().OGJBDMNBMLJ());
				NDMPMIEKGAA.SetColor(LabelButton.FBMGEHJPPIK.BUTTON_GREEN);
				NDMPMIEKGAA.SetText("0");
				NDMPMIEKGAA.ButtonId = 1;
				NDMPMIEKGAA.AddEventListener(2, MDLLDINIIKM);
			}
			if (EPDPCAKGBBO != null)
			{
				EPDPCAKGBBO.get_Icon().set_SpriteName("TopPanel.ruby");
				EPDPCAKGBBO.SetColor(LabelButton.FBMGEHJPPIK.BUTTON_GREEN);
				EPDPCAKGBBO.SetText("0");
				EPDPCAKGBBO.ButtonId = 2;
				EPDPCAKGBBO.AddEventListener(2, MDLLDINIIKM);
			}
			if (FNEMFKKBHLC != null)
			{
				FNEMFKKBHLC.get_Icon().set_SpriteName(ListSF.CCDKHLAMKKO().OGJBDMNBMLJ());
				FNEMFKKBHLC.SetColor(LabelButton.FBMGEHJPPIK.BUTTON_YELLOW);
				FNEMFKKBHLC.SetText("0");
				FNEMFKKBHLC.ButtonId = 7;
				FNEMFKKBHLC.AddEventListener(2, MDLLDINIIKM);
			}
			if (ABACDHNMNLJ != null)
			{
				ABACDHNMNLJ.get_Icon().set_SpriteName("TopPanel.ruby");
				ABACDHNMNLJ.SetColor(LabelButton.FBMGEHJPPIK.BUTTON_YELLOW);
				ABACDHNMNLJ.SetText("0");
				ABACDHNMNLJ.ButtonId = 8;
				ABACDHNMNLJ.AddEventListener(2, MDLLDINIIKM);
			}
			if (MAANHOHMEEO != null)
			{
				MAANHOHMEEO.get_Icon().set_SpriteName("TopPanel.ruby");
				MAANHOHMEEO.SetColor(LabelButton.FBMGEHJPPIK.BUTTON_GREEN);
				MAANHOHMEEO.SetText("0");
				MAANHOHMEEO.ButtonId = 10;
				MAANHOHMEEO.AddEventListener(2, MDLLDINIIKM);
			}
			if (MJOLEHNABAM != null)
			{
				MJOLEHNABAM.get_Icon().set_SpriteName("TopPanel.ruby");
				MJOLEHNABAM.SetColor(LabelButton.FBMGEHJPPIK.BUTTON_GREEN);
				MJOLEHNABAM.SetText("0");
				MJOLEHNABAM.ButtonId = 17;
				MJOLEHNABAM.AddEventListener(2, MDLLDINIIKM);
			}
			if (KFJFPNAFKLO != null)
			{
				KFJFPNAFKLO.get_Icon().gameObject.SetActive(false);
				KFJFPNAFKLO.SetColor(LabelButton.FBMGEHJPPIK.BUTTON_GREEN);
				KFJFPNAFKLO.SetText("0.00 USD");
				KFJFPNAFKLO.ButtonId = 3;
				KFJFPNAFKLO.AddEventListener(2, MDLLDINIIKM);
			}
			LNBNDFOMKCP();
		}

		private void MDLLDINIIKM(object data)
		{
			if (!PNJCNLJNGEH())
			{
				ItemAction pCKPFBFHKJH = (ItemAction)data;
				TradeDialog.LBGFOGHMBED iBODMPMJELJ = TradeDialog.LBGFOGHMBED.A_BUY;
				GameValueType bAINMLLIKOL = GameValueType.Gold;
				long num = 0L;
				long cNIOCCCBDBJ = 0L;
				bool flag = false;
				ListSF.BKDHBIDPKLK dDEDNPLHOJH = ListSF.BKDHBIDPKLK.CHECK_ITEM_NONE;
				Action<object> oDDEOFKLIAG = null;
				UserItem dKCHDHMLKHN = ListSF.CMGOCLGHNLH(JMPPBCFDOLL.Name);
				switch (pCKPFBFHKJH)
				{
				case ItemAction.Item_Buy_Gold:
					iBODMPMJELJ = TradeDialog.LBGFOGHMBED.A_BUY;
					bAINMLLIKOL = GameValueType.Gold;
					num = JMPPBCFDOLL.OHBBLIMNIMJ();
					cNIOCCCBDBJ = 0L;
					oDDEOFKLIAG = LJLECGPEDFA;
					flag = num > ListSF.CCDKHLAMKKO().BFBOEGMAMNF();
					dDEDNPLHOJH = ListSF.BKDHBIDPKLK.CHECK_ITEM_MONEY;
					break;
				case ItemAction.Item_Buy_Ruby:
					iBODMPMJELJ = TradeDialog.LBGFOGHMBED.A_BUY;
					bAINMLLIKOL = GameValueType.Gems;
					num = JMPPBCFDOLL.MCNMMBCJADI();
					cNIOCCCBDBJ = 0L;
					oDDEOFKLIAG = AEOLDJPPIGM;
					flag = num > ListSF.CCDKHLAMKKO().EHFJHFDACMP();
					dDEDNPLHOJH = ListSF.BKDHBIDPKLK.CHECK_ITEM_BONUS;
					break;
				case ItemAction.Item_Upgrade_Gold:
					iBODMPMJELJ = TradeDialog.LBGFOGHMBED.A_UPGRADE;
					bAINMLLIKOL = GameValueType.Gold;
					num = dKCHDHMLKHN.HADDPFNDPDG().OHBBLIMNIMJ();
					cNIOCCCBDBJ = 0L;
					oDDEOFKLIAG = EGLHCOAOPGL;
					flag = num > ListSF.CCDKHLAMKKO().BFBOEGMAMNF();
					dDEDNPLHOJH = ListSF.BKDHBIDPKLK.CHECK_ITEM_MONEY;
					break;
				case ItemAction.Item_Upgrade_Ruby:
					iBODMPMJELJ = TradeDialog.LBGFOGHMBED.A_UPGRADE;
					bAINMLLIKOL = GameValueType.Gems;
					num = dKCHDHMLKHN.HADDPFNDPDG().MCNMMBCJADI();
					cNIOCCCBDBJ = 0L;
					oDDEOFKLIAG = BLMDCAOBIJD;
					flag = num > ListSF.CCDKHLAMKKO().EHFJHFDACMP();
					dDEDNPLHOJH = ListSF.BKDHBIDPKLK.CHECK_ITEM_BONUS;
					break;
				case ItemAction.Item_Delivery_Ruby:
					MHPLBNJCDOP();
					return;
				case ItemAction.Item_Consumable:
					iBODMPMJELJ = TradeDialog.LBGFOGHMBED.A_BUY;
					bAINMLLIKOL = GameValueType.Gems;
					num = JMPPBCFDOLL.MCNMMBCJADI();
					cNIOCCCBDBJ = 0L;
					oDDEOFKLIAG = JFBJPMDFJKK;
					flag = num > ListSF.CCDKHLAMKKO().EHFJHFDACMP();
					dDEDNPLHOJH = ListSF.BKDHBIDPKLK.CHECK_ITEM_BONUS;
					break;
				case ItemAction.Item_Buy_Real:
					ShopScene.get_Instance().get_PaymentUI().MakePurchase(JMPPBCFDOLL);
					return;
				}
				HLOPCCHHGHB = JMPPBCFDOLL;
				if (flag)
				{
					GameUtils.EEKHDNNBDCH(JMPPBCFDOLL, dDEDNPLHOJH);
				}
				else
				{
					DialogsOpener.NGAMLDNIJID(iBODMPMJELJ, bAINMLLIKOL, num, oDDEOFKLIAG, cNIOCCCBDBJ);
				}
			}
		}

		private bool PNJCNLJNGEH()
		{
			bool result = false;
			QuestParameters hHKLFIIBIFF = ListSF.ELEBLBJKDBI().BNMLDPNCMLB();
			FightIDS jLGLBLDPAAF = hHKLFIIBIFF.JLGLBLDPAAF;
			hHKLFIIBIFF.JLGLBLDPAAF = FightIDS.Empty();
			hHKLFIIBIFF.HEIADONEACH = string.Empty;
			hHKLFIIBIFF.AIEHNBBFNPF = string.Empty;
			hHKLFIIBIFF.DLKPBAJDHBO = JMPPBCFDOLL;
			if (ListSF.ELEBLBJKDBI().FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_PREPURCHASE))
			{
				ListSF.ELEBLBJKDBI().MHHNIPBJNAD();
				result = true;
			}
			hHKLFIIBIFF.JLGLBLDPAAF = jLGLBLDPAAF;
			return result;
		}

		private void LJLECGPEDFA(object data)
		{
			int num = ((data != null) ? ((int)data) : 0);
			if (num > 0 && HLOPCCHHGHB != null)
			{
				if (ItemBuyHelper.IHHKNBPKGHD(HLOPCCHHGHB))
				{
					ListSF.CCDKHLAMKKO().KHCNHPCPFII().EEDJEDBMIMI(HLOPCCHHGHB, true);
				}
				UpdateContent();
			}
		}

		private void AEOLDJPPIGM(object data)
		{
			int num = ((data != null) ? ((int)data) : 0);
			if (num > 0 && HLOPCCHHGHB != null)
			{
				if (ItemBuyHelper.MGMAJHLAICA(HLOPCCHHGHB))
				{
					ListSF.CCDKHLAMKKO().KHCNHPCPFII().EEDJEDBMIMI(HLOPCCHHGHB, true);
				}
				UpdateContent();
			}
		}

		private void EGLHCOAOPGL(object data)
		{
			int num = ((data != null) ? ((int)data) : 0);
			if (num > 0 && HLOPCCHHGHB != null)
			{
				if (ItemBuyHelper.APICBINEPGJ(HLOPCCHHGHB))
				{
					ListSF.CCDKHLAMKKO().KHCNHPCPFII().EEDJEDBMIMI(HLOPCCHHGHB, true);
				}
				UpdateContent();
			}
		}

		private void BLMDCAOBIJD(object data)
		{
			int num = ((data != null) ? ((int)data) : 0);
			if (num > 0 && HLOPCCHHGHB != null)
			{
				if (ItemBuyHelper.JAJLOABHIMA(HLOPCCHHGHB))
				{
					ListSF.CCDKHLAMKKO().KHCNHPCPFII().EEDJEDBMIMI(HLOPCCHHGHB, true);
				}
				UpdateContent();
			}
		}

		private void MHPLBNJCDOP()
		{
			if (HLOPCCHHGHB != null)
			{
				if (ItemBuyHelper.BuyImmediatelyDelivery(HLOPCCHHGHB))
				{
					ListSF.CCDKHLAMKKO().KHCNHPCPFII().EEDJEDBMIMI(HLOPCCHHGHB, true);
				}
				UpdateContent();
			}
		}

		private void JFBJPMDFJKK(object data)
		{
			int num = ((data != null) ? ((int)data) : 0);
			if (num > 0 && HLOPCCHHGHB != null)
			{
				bool flag = ItemBuyHelper.NIEAANPCGLC(HLOPCCHHGHB);
				UpdateContent();
			}
		}

		private void HPEKMDCMPBO()
		{
			float opacity = 0.7f;
			foreach (IconLabelButton item in _buttons)
			{
				if (item != null)
				{
					item.SetOpacity(opacity);
					item.interactable = false;
				}
			}
		}

		private void ACGIOEAPGPH()
		{
			float opacity = 1f;
			foreach (IconLabelButton item in _buttons)
			{
				if (item != null)
				{
					item.SetOpacity(opacity);
					item.interactable = true;
				}
			}
		}

		private void LNBNDFOMKCP()
		{
			foreach (IconLabelButton item in _buttons)
			{
				if (item != null)
				{
					item.gameObject.SetActive(false);
				}
			}
		}

		private void SetButton(IconLabelButton AMACDAACGCA, long GGPEGMLPBKA, Color OHJKNABLCMF)
		{
			if (GGPEGMLPBKA > 0)
			{
				SetButton(AMACDAACGCA, GGPEGMLPBKA.ToString(), OHJKNABLCMF);
			}
		}

		private void SetButton(IconLabelButton AMACDAACGCA, string NGEPNAJJHCD, Color OHJKNABLCMF)
		{
			if (AMACDAACGCA != null)
			{
				AMACDAACGCA.gameObject.SetActive(true);
				AMACDAACGCA.SetText(NGEPNAJJHCD);
				AMACDAACGCA.SetColor(OHJKNABLCMF);
			}
		}

		public void ShowButton()
		{
			if (JMPPBCFDOLL != null)
			{
				switch (JMPPBCFDOLL.Type)
				{
				case "Consumable":
					ShowConsumableButton();
					break;
				case "RealMoneyItem":
					ShowPaymentButton();
					break;
				default:
					ShowDefaultButton();
					break;
				}
			}
		}

		public void ShowConsumableButton()
		{
			Color oHJKNABLCMF = Color.black;
			if (ListSF.CCDKHLAMKKO().EHFJHFDACMP() < (ObscuredLong)(JMPPBCFDOLL.FMHECGHHKGB))
			{
				oHJKNABLCMF = Constants.GJKMPOAJDCF;
			}
			SetButton(MJOLEHNABAM, (ObscuredLong)(JMPPBCFDOLL.FMHECGHHKGB), oHJKNABLCMF);
		}

		public void ShowPaymentButton()
		{
			SetButton(KFJFPNAFKLO, JMPPBCFDOLL.EGAJMELKANL + " " + JMPPBCFDOLL.MIIJIMJDHFP, Color.black);
		}

		public void ShowDefaultButton()
		{
			if (!FNFHDIPECME)
			{
				Color oHJKNABLCMF = Color.black;
				if (ListSF.CCDKHLAMKKO().BFBOEGMAMNF() < (ObscuredLong)(JMPPBCFDOLL.KJFAOKLILOC))
				{
					oHJKNABLCMF = Constants.GJKMPOAJDCF;
				}
				SetButton(NDMPMIEKGAA, (ObscuredLong)(JMPPBCFDOLL.KJFAOKLILOC), oHJKNABLCMF);
				Color oHJKNABLCMF2 = Color.black;
				if (ListSF.CCDKHLAMKKO().EHFJHFDACMP() < (ObscuredLong)(JMPPBCFDOLL.FMHECGHHKGB))
				{
					oHJKNABLCMF2 = Constants.GJKMPOAJDCF;
				}
				SetButton(EPDPCAKGBBO, (ObscuredLong)(JMPPBCFDOLL.FMHECGHHKGB), oHJKNABLCMF2);
			}
			else if (KAPLEKBLLFO)
			{
				ItemInfo dJKEECEOCJB = NKBIOFJMONB.HADDPFNDPDG();
				if (dJKEECEOCJB == null)
				{
					dJKEECEOCJB = NKBIOFJMONB.AKKBIFEFDCI();
				}
				Color oHJKNABLCMF3 = Color.black;
				if (ListSF.CCDKHLAMKKO().EHFJHFDACMP() < (ObscuredLong)(dJKEECEOCJB.KLHOKKPALOK))
				{
					oHJKNABLCMF3 = Constants.GJKMPOAJDCF;
				}
				SetButton(MAANHOHMEEO, (ObscuredLong)(dJKEECEOCJB.KLHOKKPALOK), oHJKNABLCMF3);
			}
			else if (CJALINCMNCJ && ListSF.CCDKHLAMKKO().HFINDOBJHNK())
			{
				ItemInfo dJKEECEOCJB2 = NKBIOFJMONB.HADDPFNDPDG();
				Color oHJKNABLCMF4 = Color.black;
				if (ListSF.CCDKHLAMKKO().BFBOEGMAMNF() < (ObscuredLong)(dJKEECEOCJB2.KJFAOKLILOC))
				{
					oHJKNABLCMF4 = Constants.GJKMPOAJDCF;
				}
				SetButton(FNEMFKKBHLC, (ObscuredLong)(dJKEECEOCJB2.KJFAOKLILOC), oHJKNABLCMF4);
				Color oHJKNABLCMF5 = Color.black;
				if (ListSF.CCDKHLAMKKO().EHFJHFDACMP() < (ObscuredLong)(dJKEECEOCJB2.FMHECGHHKGB))
				{
					oHJKNABLCMF5 = Constants.GJKMPOAJDCF;
				}
				SetButton(ABACDHNMNLJ, (ObscuredLong)(dJKEECEOCJB2.FMHECGHHKGB), oHJKNABLCMF5);
			}
		}

		public void SetDescription()
		{
			if (_description != null)
			{
				_description.gameObject.SetActive(false);
			}
			if (_descriptionDonate != null)
			{
				_descriptionDonate.gameObject.SetActive(false);
			}
			if (JMPPBCFDOLL.Type.Equals("Consumable"))
			{
				SetConsumableDescription();
			}
			else if (JMPPBCFDOLL.Type.Equals("RealMoneyItem"))
			{
				SetRealMoneyItemDescription();
			}
			else
			{
				SetDefaultDescription();
			}
		}

		public void SetConsumableDescription()
		{
			if (!(_descriptionDonate == null))
			{
				_descriptionDonate.gameObject.SetActive(true);
				_descriptionDonate.SetAlias(JMPPBCFDOLL.GGDJIPKMKFC);
			}
		}

		public void SetRealMoneyItemDescription()
		{
			if (!(_descriptionDonate == null))
			{
				string text = string.Empty;
				if ((ObscuredLong)(JMPPBCFDOLL.BBMLCBEFLGI) > 0)
				{
					text = string.Format("<quad name={0} size=106 width=1 /> {1}", "TopPanel.ruby", JMPPBCFDOLL.BBMLCBEFLGI);
				}
				if ((ObscuredLong)(JMPPBCFDOLL.HHIFKGOJFAC) > 0)
				{
					text = string.Format("<quad name={0} size=106 width=1 /> {1}", ListSF.CCDKHLAMKKO().OGJBDMNBMLJ(), JMPPBCFDOLL.HHIFKGOJFAC);
				}
				if (!string.IsNullOrEmpty(text))
				{
					_descriptionDonate.gameObject.SetActive(true);
					_descriptionDonate.set_text(text);
				}
			}
		}

		public void SetDefaultDescription()
		{
			if (_description == null)
			{
				return;
			}
			_description.gameObject.SetActive(true);
			if (KAPLEKBLLFO)
			{
				_description.SetAlias("shopMaking");
			}
			else if (PPEJCMNKIGP)
			{
				_description.SetAlias("shopOrder");
			}
			else if (CJALINCMNCJ && ListSF.CCDKHLAMKKO().HFINDOBJHNK())
			{
				ItemInfo dJKEECEOCJB = ((NKBIOFJMONB == null) ? null : NKBIOFJMONB.HADDPFNDPDG());
				if (dJKEECEOCJB != null)
				{
					int oMHDLKNHNMJ = ListSF.CCDKHLAMKKO().PINDEKDNCNL();
					int oBJDGBBFJOO = dJKEECEOCJB.OBJDGBBFJOO;
					UpgradeIndexItem aACAFOBANOH = dJKEECEOCJB.MJNILIJLCMI(oMHDLKNHNMJ, oBJDGBBFJOO);
					int num = ((aACAFOBANOH != null) ? aACAFOBANOH.Index : 0);
					if (aACAFOBANOH.Type == UpgradeIndexItem.LIPHFAOKLCA.UPGRADE_INDEX_MILESTONE)
					{
						string alias = "shopUpgrade{img::MiscSprites.star}{" + num + "}";
						_description.SetAlias(alias);
					}
					else
					{
						_description.SetAlias("shopUpgrade{}{" + num + "}");
					}
				}
			}
			else
			{
				_description.set_text(string.Empty);
			}
		}

		public void SetItemInfo(ItemInfo item)
		{
			NKBIOFJMONB = ListSF.CCDKHLAMKKO().KHCNHPCPFII().CMGOCLGHNLH(item);
			FNFHDIPECME = NKBIOFJMONB != null;
			JMPPBCFDOLL = ((!FNFHDIPECME) ? item : NKBIOFJMONB.AKKBIFEFDCI());
			PPEJCMNKIGP = InputDeviceExtension.CBIECFPMNKC(item, NKBIOFJMONB);
			KAPLEKBLLFO = InputDeviceExtension.GMCENJHBIDF(item, NKBIOFJMONB);
			CJALINCMNCJ = InputDeviceExtension.ACOIHHPOBDH(item, NKBIOFJMONB);
			_header.SetAlias(item.Name);
			SetDescription();
			LNBNDFOMKCP();
			ShowButton();
			if (item.MHGODOLNDLE > ListSF.CCDKHLAMKKO().PINDEKDNCNL())
			{
				HPEKMDCMPBO();
			}
			else
			{
				ACGIOEAPGPH();
			}
			if (_parametersPanel != null)
			{
				ItemInfo dJKEECEOCJB = ((NKBIOFJMONB == null) ? null : NKBIOFJMONB.HADDPFNDPDG());
				bool oGMLCLNEAIJ = ListSF.CCDKHLAMKKO().HFINDOBJHNK() && FNFHDIPECME && dJKEECEOCJB != null;
				_parametersPanel.SetParameters(JMPPBCFDOLL, dJKEECEOCJB, oGMLCLNEAIJ);
			}
		}

		public void UpdateContent()
		{
			ShopScene.get_Instance().RememberFocus();
			SetItemInfo(JMPPBCFDOLL);
			updateEvent.Invoke();
			ShopScene.get_Instance().FocusOnLastFocus();
		}

		public IconLabelButton GetGoldButton()
		{
			return NDMPMIEKGAA;
		}
	}
}
