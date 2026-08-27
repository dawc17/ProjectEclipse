using UnityEngine;

namespace Nekki.SF2.GUI.Dialogs
{
	public class ImpossibleDialog : BaseDialog
	{
		public enum MAKDAMIONLL
		{
			A_NOT_ENOUGH_GOLD = 0,
			A_NOT_ENOUGH_RUBY = 1,
			A_NOT_ENOUGH_ENERGY = 2,
			A_NOT_ENOUGH_LEVEL = 3,
			A_NOT_NETWORK = 4,
			A_EQUIP_ERROR = 5,
			A_UNEQUIP_ERROR = 6,
			A_SELL_ERROR = 7
		}

		private const int MACACIPAONI = 135;

		private const int INECLDHJIML = 70;

		private const int MCOMHPPHDAP = 122;

		private MAKDAMIONLL CBFFIFKAHHN = MAKDAMIONLL.A_SELL_ERROR;

		private string _titleString = string.Empty;

		private object _contentData;

		[SerializeField]
		protected LabelAlias _text;

		public override void Init(object data)
		{
			ImpossibleDialogInfo fHBGDNBFPLG = (ImpossibleDialogInfo)data;
			CBFFIFKAHHN = fHBGDNBFPLG.AMKJNPOCODK;
			_contentData = fHBGDNBFPLG.Content;
			if (fHBGDNBFPLG.Dlg != null)
			{
				AddEventListener(0, fHBGDNBFPLG.Dlg);
			}
			switch (CBFFIFKAHHN)
			{
			case MAKDAMIONLL.A_NOT_ENOUGH_GOLD:
				IPECDAGDHHH();
				break;
			case MAKDAMIONLL.A_NOT_ENOUGH_RUBY:
				DGKLBMJPDDN();
				break;
			case MAKDAMIONLL.A_NOT_ENOUGH_ENERGY:
				ILOFFPLEJLO();
				break;
			default:
				LLOHNIADENP(CBFFIFKAHHN);
				break;
			}
			base.Init(_titleString, "dlgBuyButton", "Cancel", GBECKKCHAFI);
		}

		protected virtual void IPECDAGDHHH()
		{
			_titleString = "dlgNotEnoughGoldTitle";
			NEDJJMIHKPK(MAKDAMIONLL.A_NOT_ENOUGH_GOLD);
		}

		protected virtual void DGKLBMJPDDN()
		{
			_titleString = "dlgNotEnoughRubyTitle";
			NEDJJMIHKPK(MAKDAMIONLL.A_NOT_ENOUGH_RUBY);
		}

		protected virtual void NEDJJMIHKPK(MAKDAMIONLL IBODMPMJELJ)
		{
			BGJJDGOBPKA = "shopBuy";
			GBECKKCHAFI = KBDHPMOMJLL.FOOTER_BOTH;
			string empty = string.Empty;
			switch (IBODMPMJELJ)
			{
			default:
				return;
			case MAKDAMIONLL.A_NOT_ENOUGH_GOLD:
				empty = "dlgNotEnoughGoldMessage";
				break;
			case MAKDAMIONLL.A_NOT_ENOUGH_RUBY:
				empty = "dlgNotEnoughRubyMessage";
				break;
			case MAKDAMIONLL.A_NOT_ENOUGH_ENERGY:
				empty = "dlgNotEnoughEnergyMessage";
				break;
			}
			GHMKEENGCMI(empty);
		}

		protected virtual void LLOHNIADENP(MAKDAMIONLL IBODMPMJELJ)
		{
			BGJJDGOBPKA = "ok";
			GBECKKCHAFI = KBDHPMOMJLL.FOOTER_OK;
			_titleString = "dlgErrorTitle";
		}

		protected virtual void ILOFFPLEJLO()
		{
			if (_contentData != null)
			{
				_titleString = "dlgNotEnoughEnergyTitle";
				BGJJDGOBPKA = "dlgNotEnoughEnergyButton";
				GBECKKCHAFI = KBDHPMOMJLL.FOOTER_BOTH;
				string empty = string.Empty;
				string empty2 = string.Empty;
				int num = 0;
				NotEnoughEnergyDialogInfo oJJHNNJPMCI = (NotEnoughEnergyDialogInfo)_contentData;
				empty = TimerLabel.GetTimeString(oJJHNNJPMCI.CLDABPBDDGB, true, true, true, false, ":", string.Empty, true, true, true, true, true, string.Empty, string.Empty, string.Empty);
				num = oJJHNNJPMCI.Value;
				GameValueType hGIKOMLPBMJ = oJJHNNJPMCI.HGIKOMLPBMJ;
				string empty3 = string.Empty;
				switch (hGIKOMLPBMJ)
				{
				case GameValueType.Gems:
					empty3 = "MiscSprites.ruby";
					break;
				case GameValueType.Energy:
					empty3 = "MiscSprites.energy";
					break;
				default:
					empty3 = ListSF.CCDKHLAMKKO().OGJBDMNBMLJ();
					break;
				}
				empty2 = num.ToString();
				string text = LocalizationManager.GetString("dlgNotEnoughEnergyMessage1");
				text += "|[0]";
				text += LocalizationManager.GetString("dlgNotEnoughEnergyMessage2");
				text += " ";
				text += empty;
				text += "\n";
				text += LocalizationManager.GetString("dlgNotEnoughEnergyMessage3");
				text += "|[1]";
				text += empty2;
				text += " ";
				text += LocalizationManager.GetString("dlgNotEnoughEnergyMessage4");
				_text.alignment = TextAnchor.MiddleCenter;
				_text.transform.OKHPLHPBPKJ(0f);
				_text.transform.BGNJGIACJBG(70f);
				_text.set_LabelFontSize(122);
				_text.color = Constants.PJJIMHMJPAL;
				_text.set_text(text);
			}
		}

		protected override void SetupHeader(string HCPNFPMHFCM)
		{
			base.SetupHeader(HCPNFPMHFCM);
			_header.set_LabelFontSize(135);
		}

		protected virtual void GHMKEENGCMI(string APFHJLHOCEN)
		{
			_text.alignment = TextAnchor.MiddleCenter;
			_text.transform.OKHPLHPBPKJ(0f);
			_text.transform.BGNJGIACJBG(70f);
			_text.set_LabelFontSize(122);
			_text.color = Constants.PJJIMHMJPAL;
			_text.set_Alias(APFHJLHOCEN);
		}
	}
}
