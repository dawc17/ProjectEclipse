using System;
using UnityEngine;

namespace Nekki.SF2.GUI.Dialogs
{
	public class TradeDialog : BaseDialog
	{
		public enum LBGFOGHMBED
		{
			A_BUY = 0,
			A_UPGRADE = 1
		}

		[SerializeField]
		private LabelAlias _text;

		private string _titleString = string.Empty;

		private string HEGPBBJKKCJ = string.Empty;

		private long LFEDAOPJBHP;

		private GameValueType _value;

		private LBGFOGHMBED CBFFIFKAHHN;

		private long IGMDKDOGGNA;

		public override void Init(object data)
		{
			TradeDialogInfo fOIIPALJAMM = (TradeDialogInfo)data;
			if (fOIIPALJAMM == null)
			{
				_value = GameValueType.Gold;
				LFEDAOPJBHP = 0L;
				CBFFIFKAHHN = LBGFOGHMBED.A_BUY;
				IGMDKDOGGNA = 0L;
			}
			else
			{
				_value = fOIIPALJAMM.Value;
				LFEDAOPJBHP = fOIIPALJAMM.IJJJDFHBLNN;
				CBFFIFKAHHN = fOIIPALJAMM.AMKJNPOCODK;
				IGMDKDOGGNA = fOIIPALJAMM.JNNKILFJKPB;
			}
			string text;
			switch (CBFFIFKAHHN)
			{
			case LBGFOGHMBED.A_BUY:
				text = "shopBuy";
				_titleString = "dlgBuyTitle";
				HEGPBBJKKCJ = "dlgBuyMessage";
				break;
			case LBGFOGHMBED.A_UPGRADE:
				text = "dlgUpgradeButton";
				_titleString = "dlgUpgradeTitle";
				HEGPBBJKKCJ = "dlgUpgradeMessage";
				break;
			default:
				text = "OK";
				break;
			}
			if (IGMDKDOGGNA > 0)
			{
				if (CBFFIFKAHHN != LBGFOGHMBED.A_UPGRADE)
				{
					text = "dlgOrderButton";
					_titleString = "dlgOrderTitle";
					HEGPBBJKKCJ = "dlgOrderMessage";
				}
				else
				{
					text = "dlgUpgradeButton";
					_titleString = "dlgUpgradeTitle";
					HEGPBBJKKCJ = "dlgUpgradeMessage";
				}
			}
			BGJJDGOBPKA = text;
			if (fOIIPALJAMM != null && fOIIPALJAMM.Dlg != null)
			{
				AddEventListener(0, fOIIPALJAMM.Dlg);
			}
			base.Init(_titleString, text, "CANCEL", KBDHPMOMJLL.FOOTER_BOTH);
		}

		protected override void HLJBLAPMDCB()
		{
			string text = "img::";
			switch (_value)
			{
			case GameValueType.Gold:
				text += ListSF.CCDKHLAMKKO().OGJBDMNBMLJ();
				break;
			case GameValueType.Gems:
				text += "MiscSprites.ruby";
				break;
			}
			string pEMOECLNECD = "dlgCurrencyQuestion{" + text + "}{" + LFEDAOPJBHP + "}";
			string text2 = LocalizationManager.GetString(HEGPBBJKKCJ) + "\n" + LocalizationManager.GetString(pEMOECLNECD);
			_text.set_text(text2);
			if (IGMDKDOGGNA > 0)
			{
				string empty = string.Empty;
				empty = ((CBFFIFKAHHN != LBGFOGHMBED.A_BUY) ? "dlgTimeUpgrade" : "dlgTimeDelivery");
				TimeSpan timeSpan = TimeSpan.FromSeconds(IGMDKDOGGNA);
				string empty2 = string.Empty;
				empty2 = ((timeSpan.Hours <= 0) ? string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds) : string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds));
				string text3 = LocalizationManager.GetString(empty, empty2);
				text2 = text2 + "\n" + text3;
				_text.set_text(text2);
			}
		}
	}
}
