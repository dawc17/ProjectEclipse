using System;
using System.Collections.Generic;
using Nekki.SF2.Core;
using Nekki.SF2.GUI.Dialogs;
using UnityEngine;

public class DialogsOpener
{
	public const float HFHFJGIMFED = 65f;

	public const float OFMFMJCFEPF = 90f;

	public const float OJCFPNJFKEO = 470f;

	private static bool IJEDODAJGDD;

	public static bool GLLHPJKFCDM
	{
		get
		{
			return MOAEBPJBDCD();
		}
	}

	public static void NGAMLDNIJID(TradeDialog.LBGFOGHMBED IBODMPMJELJ, GameValueType value, long GLGKKGBLFPH, Action<object> ODDEOFKLIAG, long CNIOCCCBDBJ = 0L)
	{
		TradeDialogInfo jGMLAFOPBBC = new TradeDialogInfo(IBODMPMJELJ, value, GLGKKGBLFPH, ODDEOFKLIAG, CNIOCCCBDBJ);
		DialogsManager.LAEGPJHIGAM(DialogType.DialogBuy, jGMLAFOPBBC);
	}

	public static void ENBLMFGOCEL(ImpossibleDialog.MAKDAMIONLL IBODMPMJELJ, Action<object> ODDEOFKLIAG = null, object DMNBDBJNKME = null)
	{
		ImpossibleDialogInfo jGMLAFOPBBC = new ImpossibleDialogInfo(IBODMPMJELJ, ODDEOFKLIAG, DMNBDBJNKME);
		DialogsManager.LAEGPJHIGAM(DialogType.DialogImpossible, jGMLAFOPBBC);
	}

	public static BaseDialog DKBFJMGFEEB(string JIAKJEOEIMF, string GIBEOPMGOPG, List<StoryDialogContent> PBCJDMAPOOB, float ratio, Action<object> ODDEOFKLIAG = null, string FGJCMOLFFGH = "", string NMFJJEJEHMC = "", string BFNHNNFIBNM = "", LabelButton.FBMGEHJPPIK IDHJGMKHNOP = LabelButton.FBMGEHJPPIK.BUTTON_WHITE, LabelButton.FBMGEHJPPIK CPKKNLPKBIO = LabelButton.FBMGEHJPPIK.BUTTON_DARK, LabelButton.FBMGEHJPPIK NFDONPAIONH = LabelButton.FBMGEHJPPIK.BUTTON_WHITE, bool HNNKHNCELDA = true, bool MHLJPGALMFO = false, bool NKPIIFBDEIB = false, bool CJJBDGPDOFF = false, string IAHHOEJJJHP = "")
	{
		bool flag = true;
		for (int i = 0; i < PBCJDMAPOOB.Count; i++)
		{
			StoryDialogContent nJEPNCJLPPF = PBCJDMAPOOB[i];
			if (!nJEPNCJLPPF.JHOPPPIADHN())
			{
				flag = false;
			}
		}
		if ((PBCJDMAPOOB.Count > 0 && PBCJDMAPOOB[0].CheckTimer && PBCJDMAPOOB[0].Timer <= 0) || (MHLJPGALMFO && !flag))
		{
			if (ODDEOFKLIAG != null)
			{
				int oNNLBFAOMMB = PBCJDMAPOOB[0].Id;
				ODDEOFKLIAG(oNNLBFAOMMB);
			}
			return null;
		}
		StrangerDialogInfo jGMLAFOPBBC = new StrangerDialogInfo(JIAKJEOEIMF, GIBEOPMGOPG, PBCJDMAPOOB, ratio, ODDEOFKLIAG, FGJCMOLFFGH, NMFJJEJEHMC, BFNHNNFIBNM, IDHJGMKHNOP, CPKKNLPKBIO, NFDONPAIONH, HNNKHNCELDA, MHLJPGALMFO, NKPIIFBDEIB, CJJBDGPDOFF, IAHHOEJJJHP);
		return DialogsManager.LAEGPJHIGAM(DialogType.DialogStranger, jGMLAFOPBBC);
	}

	public static BaseDialog PEDJMOMBJJI(string HHAAFADDOJB, string HCPNFPMHFCM, string ALOJJLCOGMP, string PAJIOGEINPI = "", Action<object> ODDEOFKLIAG = null, LabelButton.FBMGEHJPPIK HGAGMJENCNM = LabelButton.FBMGEHJPPIK.BUTTON_WHITE, LabelButton.FBMGEHJPPIK PHBOACBIMMF = LabelButton.FBMGEHJPPIK.BUTTON_DARK, bool LMAFOFCILBL = false, bool EPHHGNKDPEG = false, string DOEEIGAHKEN = "")
	{
		BaseDialog.KBDHPMOMJLL hJNAHNICGMH = BaseDialog.KBDHPMOMJLL.FOOTER_BOTH;
		if (ALOJJLCOGMP == string.Empty || PAJIOGEINPI == string.Empty)
		{
			hJNAHNICGMH = BaseDialog.KBDHPMOMJLL.FOOTER_NONE;
			hJNAHNICGMH = ((ALOJJLCOGMP != string.Empty) ? BaseDialog.KBDHPMOMJLL.FOOTER_OK : ((PAJIOGEINPI != string.Empty) ? BaseDialog.KBDHPMOMJLL.FOOTER_CANCEL : BaseDialog.KBDHPMOMJLL.FOOTER_NONE));
		}
		SimpleDialogInfo jGMLAFOPBBC = new SimpleDialogInfo(HHAAFADDOJB, HCPNFPMHFCM, hJNAHNICGMH, ALOJJLCOGMP, PAJIOGEINPI, HGAGMJENCNM, PHBOACBIMMF, LMAFOFCILBL, EPHHGNKDPEG, DOEEIGAHKEN, ODDEOFKLIAG);
		BaseDialog baseDialog = DialogsManager.LAEGPJHIGAM(DialogType.DialogSimple, jGMLAFOPBBC);
		if (ODDEOFKLIAG != null)
		{
			baseDialog.AddEventListener(0, ODDEOFKLIAG);
		}
		return baseDialog;
	}

	public static bool MOAEBPJBDCD()
	{
		return IJEDODAJGDD;
	}

	public static BaseDialog FEAHBJGCNLC(Action JPCNFOHPAOB)
	{
		string hHAAFADDOJB = "dlgWarning";
		string hCPNFPMHFCM = "dlg_appleID_required";
		string aLOJJLCOGMP = "OK";
		string empty = string.Empty;
		return PEDJMOMBJJI(hHAAFADDOJB, hCPNFPMHFCM, aLOJJLCOGMP, empty, (object KFBMKMCEMGG) =>
		{
			IJEDODAJGDD = true;
			JPCNFOHPAOB();
		}, LabelButton.FBMGEHJPPIK.BUTTON_WHITE, LabelButton.FBMGEHJPPIK.BUTTON_DARK, false, false, string.Empty);
	}

	public static BaseDialog OFIOGLOLIJP(Action JPCNFOHPAOB)
	{
		string hHAAFADDOJB = "Error";
		string hCPNFPMHFCM = "Error_validation_nointernet";
		string aLOJJLCOGMP = "OK";
		string empty = string.Empty;
		return PEDJMOMBJJI(hHAAFADDOJB, hCPNFPMHFCM, aLOJJLCOGMP, empty, (object KFBMKMCEMGG) =>
		{
			JPCNFOHPAOB();
		}, LabelButton.FBMGEHJPPIK.BUTTON_WHITE, LabelButton.FBMGEHJPPIK.BUTTON_DARK, false, false, string.Empty);
	}

	public static BaseDialog FPCPKGBNEPD()
	{
		string hHAAFADDOJB = "Error";
		string hCPNFPMHFCM = "Error_validation_failed";
		string aLOJJLCOGMP = "OK";
		string empty = string.Empty;
		return PEDJMOMBJJI(hHAAFADDOJB, hCPNFPMHFCM, aLOJJLCOGMP, empty, (object KFBMKMCEMGG) =>
		{
			ApplicationController.Quit();
		}, LabelButton.FBMGEHJPPIK.BUTTON_WHITE, LabelButton.FBMGEHJPPIK.BUTTON_DARK, false, false, string.Empty);
	}

	public static BaseDialog BGHNEGGJIJC()
	{
		string hHAAFADDOJB = "dlgNotNetworkTitle";
		string hCPNFPMHFCM = "dlgNotNetworkMessage";
		string aLOJJLCOGMP = "OK";
		string empty = string.Empty;
		return PEDJMOMBJJI(hHAAFADDOJB, hCPNFPMHFCM, aLOJJLCOGMP, empty, null, LabelButton.FBMGEHJPPIK.BUTTON_WHITE, LabelButton.FBMGEHJPPIK.BUTTON_DARK, false, false, string.Empty);
	}

	public static BaseDialog DNFMECAEDLJ()
	{
		string hHAAFADDOJB = "dlgDuelLockedTitle";
		string hCPNFPMHFCM = "dlgDuelLockedMessage";
		string aLOJJLCOGMP = "OK";
		string empty = string.Empty;
		return PEDJMOMBJJI(hHAAFADDOJB, hCPNFPMHFCM, aLOJJLCOGMP, empty, null, LabelButton.FBMGEHJPPIK.BUTTON_WHITE, LabelButton.FBMGEHJPPIK.BUTTON_DARK, false, false, string.Empty);
	}

	public static BaseDialog PBKDJENGJKB()
	{
		string hHAAFADDOJB = "dlgNotAvaliableTitle";
		string hCPNFPMHFCM = "dlgNotAvaliableMessage";
		string aLOJJLCOGMP = "OK";
		string empty = string.Empty;
		return PEDJMOMBJJI(hHAAFADDOJB, hCPNFPMHFCM, aLOJJLCOGMP, empty, null, LabelButton.FBMGEHJPPIK.BUTTON_WHITE, LabelButton.FBMGEHJPPIK.BUTTON_DARK, false, false, string.Empty);
	}

	public static BaseDialog PMMOGEADGNL()
	{
		return DialogsManager.LAEGPJHIGAM(DialogType.DialogExit, null);
	}

	public static BaseDialog OEDGOIHPJJK(Action<object> _dlg)
	{
		return DialogsManager.LAEGPJHIGAM(DialogType.DialogExit, new GBAEHLPNDAC(true, _dlg));
	}

	public static BaseDialog DBHBIMGMIEH()
	{
		return DialogsManager.LAEGPJHIGAM(DialogType.DialogSettings, null);
	}

	public static BaseDialog CLOCBDBIAEF()
	{
		return DialogsManager.LAEGPJHIGAM(DialogType.DialogSettingsAdvenced, null);
	}

	public static BaseDialog CNDJILOPFJC(NewsDialogInfo EMBBNNBFODN)
	{
		return DialogsManager.LAEGPJHIGAM(DialogType.DialogNews, EMBBNNBFODN);
	}

	public static void CNDJILOPFJC()
	{
		if (!GameUtils.GCDIGFODNFO || GeneralConfig.FNHPCBEDKFO.MEFNHIALOED().Count == 0)
		{
			return;
		}
		List<NewsItem> list = new List<NewsItem>();
		foreach (NewsItem item in GeneralConfig.FNHPCBEDKFO.MEFNHIALOED())
		{
			bool flag = item.EndDate < 0 || item.EndDate > GameUtils.ECCPJAPIABG();
			bool flag2 = !item.CIKJHDEGHGD;
			bool flag3 = ListSF.ELEBLBJKDBI().NKLCAPEMDIO(item.KJHMHHBJEDH);
			if (item.DCHJDPCEODD && item.GAHGCJNGDMH && flag && flag2 && flag3)
			{
				item.CIKJHDEGHGD = true;
				list.Add(item);
			}
		}
		if (list.Count != 0)
		{
			NewsDialogInfo eMBBNNBFODN = new NewsDialogInfo(list);
			CNDJILOPFJC(eMBBNNBFODN);
		}
	}

	public static BaseDialog EHMEIJCOOKP(string GDLKNAOPKIL, string PEMOECLNECD, List<StoryDialogContent> PBCJDMAPOOB, Action<object> ODDEOFKLIAG = null, string AGEBBHHPFME = "CANCEL", bool IJCBBJHLGFI = false, string NGJFMFPMAFL = "", LabelButton.FBMGEHJPPIK FHNFKIHDCPC = LabelButton.FBMGEHJPPIK.BUTTON_WHITE, LabelButton.FBMGEHJPPIK ICLJIMNHGMN = LabelButton.FBMGEHJPPIK.BUTTON_DARK, bool JLBJMEGPNPF = true, bool MHLJPGALMFO = false)
	{
		for (int i = 0; i < PBCJDMAPOOB.Count; i++)
		{
			StoryDialogContent nJEPNCJLPPF = PBCJDMAPOOB[i];
			nJEPNCJLPPF.JHOPPPIADHN();
		}
		if (PBCJDMAPOOB.Count > 0 && PBCJDMAPOOB[0].CheckTimer && PBCJDMAPOOB[0].Timer <= 0)
		{
			if (ODDEOFKLIAG != null)
			{
				int oNNLBFAOMMB = PBCJDMAPOOB[0].Id;
				ODDEOFKLIAG(oNNLBFAOMMB);
			}
			return null;
		}
		StoryDialogInfo jGMLAFOPBBC = new StoryDialogInfo(GDLKNAOPKIL, PEMOECLNECD, PBCJDMAPOOB, ODDEOFKLIAG, NGJFMFPMAFL, AGEBBHHPFME, IJCBBJHLGFI, FHNFKIHDCPC, ICLJIMNHGMN, JLBJMEGPNPF, MHLJPGALMFO);
		return DialogsManager.LAEGPJHIGAM(DialogType.DialogStory, jGMLAFOPBBC);
	}

	public static void LMHIIMALDKF()
	{
		Application.OpenURL(InternetController.DMFANLAIJMN());
	}
}
