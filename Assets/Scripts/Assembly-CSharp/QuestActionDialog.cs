using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Nekki.SF2.GUI.Dialogs;
using UnityEngine;

public class QuestActionDialog : QuestAction
{
	public enum BEMAILNFOHL
	{
		BUTTON_TYPE_NONE = 0,
		BUTTON_TYPE_LEFT = 1,
		BUTTON_TYPE_RIGHT = 2,
		BUTTON_TYPE_MIDDLE = 3
	}

	public class FFIBFAFPEGF
	{
		public BEMAILNFOHL Type;

		public string GGDJIPKMKFC = string.Empty;

		public QuestActionsSequence DJBAIAKOIHM = new QuestActionsSequence();

		public string Color = string.Empty;
	}

	public class CLNIMHCJIAL
	{
		public int Id;

		public QuestActionsSequence DJBAIAKOIHM = new QuestActionsSequence();
	}

	public class DialogCheckBox
	{
		public string JKHGBEJBCDG = string.Empty;

		public string GGDJIPKMKFC = string.Empty;

		public QuestActionsSequence LCBILMMHDGP = new QuestActionsSequence();

		public QuestActionsSequence ENHDNOBHIHA = new QuestActionsSequence();
	}

	private string PEMOECLNECD = string.Empty;

	private string KHPKDMGDMAB = string.Empty;

	private string LFLGCDNKNJI = string.Empty;

	private string FKKMHPFLIME = string.Empty;

	private string KGHEOKCBOLP = string.Empty;

	private string LCJIJENEPMC = string.Empty;

	private bool NHIFNDHPKLJ;

	private bool CBAANFEMIOP;

	private bool IDIHEPPFMMF;

	private float FFLLNCBOGJJ;

	private List<StoryDialogContent> IGLEKOAILHD = new List<StoryDialogContent>();

	private FFIBFAFPEGF GEIEBHILDME;

	private FFIBFAFPEGF JFKGFNHJLLE;

	private FFIBFAFPEGF KJBKIDGAPJH;

	private List<CLNIMHCJIAL> KEJEJIIBBGM = new List<CLNIMHCJIAL>();

	private string EBGIGEGKIBD = string.Empty;

	private DialogCheckBox GDOAGJEBCFK;

	private int timersID = 5;

	private string KOGNLFOPACB = string.Empty;

	private QuestParameters EHMMGHDNIJL;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		PEMOECLNECD = EPKLCPOEELO.Attributes["Title"].CIPOICEEIBK(string.Empty);
		KHPKDMGDMAB += EPKLCPOEELO.Attributes["Image"].CIPOICEEIBK(string.Empty);
		FKKMHPFLIME = EPKLCPOEELO.Attributes["ItemIcon"].CIPOICEEIBK(string.Empty);
		KGHEOKCBOLP = EPKLCPOEELO.Attributes["ItemBefore"].CIPOICEEIBK(string.Empty);
		LCJIJENEPMC = EPKLCPOEELO.Attributes["ItemAfter"].CIPOICEEIBK(string.Empty);
		LFLGCDNKNJI = EPKLCPOEELO.Attributes["Type"].CIPOICEEIBK("Regular");
		CBAANFEMIOP = EPKLCPOEELO.Attributes["IgnoreBack"].ParseBool();
		NHIFNDHPKLJ = EPKLCPOEELO.Attributes["Mirrored"].ParseBool();
		FFLLNCBOGJJ = EPKLCPOEELO.Attributes["ReadTime"].ParseFloat(BasicGUI.KMJDBLBFEMF());
		foreach (XmlNode childNode in EPKLCPOEELO.ChildNodes)
		{
			switch (childNode.Name)
			{
			case "Line":
			case "DeliveryDelay":
			case "PriceLine":
				OECMOJEDJHP(childNode);
				break;
			case "Button":
				KDFIOMJEFAC(childNode);
				break;
			case "DifficultyOf":
				KPINGLIDIGI(childNode);
				break;
			case "CheckBox":
				BCLEMHHFPDO(childNode);
				break;
			case "Timer":
				EBGIGEGKIBD = childNode.Attributes["Name"].CIPOICEEIBK(string.Empty);
				break;
			}
		}
	}

	public override void DEJMHFMLKIC(QuestParameters JCICKLIMBEF)
	{
		GKFMJKAAJCA();
		base.DEJMHFMLKIC(JCICKLIMBEF);
		EHMMGHDNIJL = JCICKLIMBEF;
		IDIHEPPFMMF = false;
		float lDHEHCLPMOK = MPNBGBIMEIP();
		Action<object> action = EGCODMJHDBI;
		string pMDPPGNJAFE = ((GEIEBHILDME == null) ? "dlgStoryNegative" : GEIEBHILDME.GGDJIPKMKFC);
		string pMDPPGNJAFE2 = ((JFKGFNHJLLE == null) ? "dlgStoryPositive" : JFKGFNHJLLE.GGDJIPKMKFC);
		string pMDPPGNJAFE3 = ((KJBKIDGAPJH == null) ? "dlgButtonFight" : KJBKIDGAPJH.GGDJIPKMKFC);
		pMDPPGNJAFE = ABMMAALFNFD.KGIEIAJLAGI(pMDPPGNJAFE, JCICKLIMBEF);
		pMDPPGNJAFE2 = ABMMAALFNFD.KGIEIAJLAGI(pMDPPGNJAFE2, JCICKLIMBEF);
		pMDPPGNJAFE3 = ABMMAALFNFD.KGIEIAJLAGI(pMDPPGNJAFE3, JCICKLIMBEF);
		LabelButton.FBMGEHJPPIK fBMGEHJPPIK = ((GEIEBHILDME == null || !(GEIEBHILDME.Color != string.Empty)) ? LabelButton.GetBtnColor("Red") : LabelButton.GetBtnColor(GEIEBHILDME.Color));
		LabelButton.FBMGEHJPPIK fBMGEHJPPIK2 = ((JFKGFNHJLLE == null || !(JFKGFNHJLLE.Color != string.Empty)) ? LabelButton.GetBtnColor("Beige") : LabelButton.GetBtnColor(JFKGFNHJLLE.Color));
		LabelButton.FBMGEHJPPIK nFDONPAIONH = ((KJBKIDGAPJH == null || !(KJBKIDGAPJH.Color != string.Empty)) ? LabelButton.GetBtnColor("Beige") : LabelButton.GetBtnColor(KJBKIDGAPJH.Color));
		string text = ((JFKGFNHJLLE == null) ? string.Empty : pMDPPGNJAFE2);
		string text2 = ((GEIEBHILDME == null) ? string.Empty : pMDPPGNJAFE);
		string bFNHNNFIBNM = ((KJBKIDGAPJH == null) ? string.Empty : pMDPPGNJAFE3);
		ConditionExtension.CompareResult lNIDLHOIHIM = new ConditionExtension.CompareResult();
		QuestCondition kKDGLNECFHA = new QuestCondition();
		kKDGLNECFHA.LIMHBJBEEIA(JCICKLIMBEF);
		kKDGLNECFHA.MCPIOGALBMK(PEMOECLNECD, lNIDLHOIHIM);
		string text3 = ABMMAALFNFD.KGIEIAJLAGI(lNIDLHOIHIM.resultSTR, JCICKLIMBEF);
		kKDGLNECFHA.MCPIOGALBMK(KHPKDMGDMAB, lNIDLHOIHIM);
		string iBBAMMHHBFE = lNIDLHOIHIM.resultSTR;
		string empty = string.Empty;
		if (!iBBAMMHHBFE.Contains("/"))
		{
		}
		empty += iBBAMMHHBFE;
		if (!iBBAMMHHBFE.Contains("."))
		{
			empty += ".png";
		}
		if (NHIFNDHPKLJ)
		{
			empty += "|Flip";
		}
		BaseDialog baseDialog = null;
		List<StoryDialogContent> list = new List<StoryDialogContent>();
		for (int i = 0; i < IGLEKOAILHD.Count; i++)
		{
			StoryDialogContent nJEPNCJLPPF = new StoryDialogContent(IGLEKOAILHD[i]);
			if (nJEPNCJLPPF.DLKPBAJDHBO != string.Empty)
			{
				kKDGLNECFHA.MCPIOGALBMK(nJEPNCJLPPF.DLKPBAJDHBO, lNIDLHOIHIM);
				nJEPNCJLPPF.DLKPBAJDHBO = lNIDLHOIHIM.resultSTR;
			}
			if (nJEPNCJLPPF.KEHBCHJDCND != string.Empty)
			{
				kKDGLNECFHA.MCPIOGALBMK(nJEPNCJLPPF.KEHBCHJDCND, lNIDLHOIHIM);
				nJEPNCJLPPF.KEHBCHJDCND = lNIDLHOIHIM.resultSTR;
			}
			kKDGLNECFHA.MCPIOGALBMK(nJEPNCJLPPF.GGDJIPKMKFC, lNIDLHOIHIM);
			nJEPNCJLPPF.GGDJIPKMKFC = lNIDLHOIHIM.ToString();
			nJEPNCJLPPF.GGDJIPKMKFC = ABMMAALFNFD.KGIEIAJLAGI(nJEPNCJLPPF.GGDJIPKMKFC, JCICKLIMBEF);
			list.Add(nJEPNCJLPPF);
		}
		if (LFLGCDNKNJI == "Regular")
		{
			baseDialog = DialogsOpener.EHMEIJCOOKP(empty, text3, list, action, pMDPPGNJAFE, GEIEBHILDME != null && JFKGFNHJLLE != null, pMDPPGNJAFE2, fBMGEHJPPIK2, fBMGEHJPPIK);
		}
		else if (LFLGCDNKNJI == "Stranger")
		{
			baseDialog = DialogsOpener.DKBFJMGFEEB(empty, text3, list, lDHEHCLPMOK, action, text, text2, bFNHNNFIBNM, fBMGEHJPPIK2, fBMGEHJPPIK, nFDONPAIONH, true, false, false, false, string.Empty);
		}
		else if (LFLGCDNKNJI == "NoAvatar")
		{
			string dOEEIGAHKEN = ((GDOAGJEBCFK == null) ? string.Empty : GDOAGJEBCFK.GGDJIPKMKFC);
			bool ePHHGNKDPEG = false;
			if (GDOAGJEBCFK != null)
			{
				ConditionExtension.CompareResult lNIDLHOIHIM2 = new ConditionExtension.CompareResult();
				QuestCondition kKDGLNECFHA2 = new QuestCondition();
				kKDGLNECFHA2.LIMHBJBEEIA(JCICKLIMBEF);
				kKDGLNECFHA2.MCPIOGALBMK(GDOAGJEBCFK.JKHGBEJBCDG, lNIDLHOIHIM2);
				ePHHGNKDPEG = lNIDLHOIHIM2.resultNumber == 1.0;
			}
			bool lMAFOFCILBL = GDOAGJEBCFK != null;
			StoryDialogContent nJEPNCJLPPF2 = list[0];
			string hCPNFPMHFCM = ((list.Count <= 0) ? string.Empty : ABMMAALFNFD.KGIEIAJLAGI(nJEPNCJLPPF2.GGDJIPKMKFC, JCICKLIMBEF));
			baseDialog = DialogsOpener.PEDJMOMBJJI(text3, hCPNFPMHFCM, text, text2, action, fBMGEHJPPIK2, fBMGEHJPPIK, lMAFOFCILBL, ePHHGNKDPEG, dOEEIGAHKEN);
		}
		else if (LFLGCDNKNJI == "ThreeButtons")
		{
			baseDialog = DialogsOpener.DKBFJMGFEEB(empty, text3, list, 0f, action, text, text2, bFNHNNFIBNM, fBMGEHJPPIK2, fBMGEHJPPIK, nFDONPAIONH, false, false, false, false, string.Empty);
		}
		else if (LFLGCDNKNJI == "Multiline")
		{
			string iAHHOEJJJHP = ((GDOAGJEBCFK == null) ? string.Empty : GDOAGJEBCFK.GGDJIPKMKFC);
			bool cJJBDGPDOFF = false;
			if (GDOAGJEBCFK != null)
			{
				ConditionExtension.CompareResult lNIDLHOIHIM3 = new ConditionExtension.CompareResult();
				QuestCondition kKDGLNECFHA3 = new QuestCondition();
				kKDGLNECFHA3.LIMHBJBEEIA(JCICKLIMBEF);
				kKDGLNECFHA3.MCPIOGALBMK(GDOAGJEBCFK.JKHGBEJBCDG, lNIDLHOIHIM3);
				cJJBDGPDOFF = lNIDLHOIHIM3.resultNumber == 1.0;
			}
			bool nKPIIFBDEIB = GDOAGJEBCFK != null;
			baseDialog = DialogsOpener.DKBFJMGFEEB(empty, text3, list, 0f, action, text, text2, bFNHNNFIBNM, fBMGEHJPPIK2, fBMGEHJPPIK, nFDONPAIONH, false, true, nKPIIFBDEIB, cJJBDGPDOFF, iAHHOEJJJHP);
		}
		else if (LFLGCDNKNJI == "Notification")
		{
			NotificationsGame.get_Instance().OpenNotification(empty, list, action, text, fBMGEHJPPIK2, FFLLNCBOGJJ);
		}
		if (baseDialog != null)
		{
			baseDialog.IsIgnoreBack = CBAANFEMIOP;
			baseDialog.IsQuestDialog = true;
		}
		else
		{
			OGIJONMKABB();
		}
	}

	public override void GKFMJKAAJCA()
	{
		if (GEIEBHILDME != null)
		{
			GEIEBHILDME.DJBAIAKOIHM.FHPKJMMLIEG();
		}
		if (JFKGFNHJLLE != null)
		{
			JFKGFNHJLLE.DJBAIAKOIHM.FHPKJMMLIEG();
		}
		if (KJBKIDGAPJH != null)
		{
			KJBKIDGAPJH.DJBAIAKOIHM.FHPKJMMLIEG();
		}
		for (int i = 0; i < KEJEJIIBBGM.Count; i++)
		{
			KEJEJIIBBGM[i].DJBAIAKOIHM.FHPKJMMLIEG();
		}
		BCDBAPMJOJD();
	}

	private void OECMOJEDJHP(XmlNode EPKLCPOEELO)
	{
		StoryDialogContent nJEPNCJLPPF = new StoryDialogContent(string.Empty, string.Empty, string.Empty, string.Empty);
		nJEPNCJLPPF.GGDJIPKMKFC = EPKLCPOEELO.Attributes["Text"].CIPOICEEIBK(string.Empty);
		nJEPNCJLPPF.AJELOOEBCPO = EPKLCPOEELO.Attributes["ButtonText"].CIPOICEEIBK(string.Empty);
		nJEPNCJLPPF.LKIKHJNCBEI = EPKLCPOEELO.Attributes["FontName"].CIPOICEEIBK(string.Empty);
		nJEPNCJLPPF.DLKPBAJDHBO = EPKLCPOEELO.Attributes["Item"].CIPOICEEIBK(string.Empty);
		nJEPNCJLPPF.KEHBCHJDCND = EPKLCPOEELO.Attributes["Enchantment"].CIPOICEEIBK(string.Empty);
		string name = EPKLCPOEELO.Name;
		if (name == "PriceLine")
		{
			nJEPNCJLPPF.NGEPEDCCMAI = StoryDialogContent.MFHMNFAPAOH.CONTENT_TYPE_PRICELINE;
		}
		else
		{
			nJEPNCJLPPF.NGEPEDCCMAI = StoryDialogContent.MFHMNFAPAOH.CONTENT_TYPE_REGULAR;
		}
		string text = EPKLCPOEELO.Attributes["TextColor"].CIPOICEEIBK(string.Empty);
		text = text.Replace("0x", string.Empty);
		text = text.Replace("#", string.Empty);
		if (text.Length == 8)
		{
			byte r = byte.Parse(text.Substring(0, 2), NumberStyles.HexNumber);
			byte g = byte.Parse(text.Substring(2, 2), NumberStyles.HexNumber);
			byte b = byte.Parse(text.Substring(4, 2), NumberStyles.HexNumber);
			byte a = byte.Parse(text.Substring(6, 2), NumberStyles.HexNumber);
			nJEPNCJLPPF.FontColor = new Color32(r, g, b, a);
		}
		foreach (XmlNode childNode in EPKLCPOEELO.ChildNodes)
		{
			if (childNode.Name == "Timer")
			{
				nJEPNCJLPPF.MALKNOOGNBA = POAECPMINAM(childNode, nJEPNCJLPPF.LKIKHJNCBEI);
			}
		}
		if (nJEPNCJLPPF.DLKPBAJDHBO != null || nJEPNCJLPPF.KEHBCHJDCND != null)
		{
			CLNIMHCJIAL cLNIMHCJIAL = new CLNIMHCJIAL();
			cLNIMHCJIAL.DJBAIAKOIHM.AddEventListener(1, OnActionComplete);
			cLNIMHCJIAL.Id = timersID++;
			nJEPNCJLPPF.Id = cLNIMHCJIAL.Id;
			foreach (XmlNode childNode2 in EPKLCPOEELO.ChildNodes)
			{
				QuestAction mBAAKHELFKL = QuestAction.GetClassActionByName(childNode2.Name);
				mBAAKHELFKL.ONGHPGEIJEN = ONGHPGEIJEN;
				mBAAKHELFKL.Parse(childNode2);
				cLNIMHCJIAL.DJBAIAKOIHM.NLJLHHNPCAO(mBAAKHELFKL);
			}
			KEJEJIIBBGM.Add(cLNIMHCJIAL);
		}
		IGLEKOAILHD.Add(nJEPNCJLPPF);
	}

	private void KDFIOMJEFAC(XmlNode EPKLCPOEELO)
	{
		string text = EPKLCPOEELO.Attributes["Type"].CIPOICEEIBK(string.Empty);
		string gGDJIPKMKFC = EPKLCPOEELO.Attributes["Text"].CIPOICEEIBK(string.Empty);
		string mDADHHOFCNG = EPKLCPOEELO.Attributes["Color"].CIPOICEEIBK(string.Empty);
		FFIBFAFPEGF fFIBFAFPEGF = null;
		switch (text)
		{
		case "Left":
			GEIEBHILDME = new FFIBFAFPEGF();
			GEIEBHILDME.Type = BEMAILNFOHL.BUTTON_TYPE_LEFT;
			fFIBFAFPEGF = GEIEBHILDME;
			break;
		case "Right":
			JFKGFNHJLLE = new FFIBFAFPEGF();
			JFKGFNHJLLE.Type = BEMAILNFOHL.BUTTON_TYPE_RIGHT;
			fFIBFAFPEGF = JFKGFNHJLLE;
			break;
		case "Middle":
			KJBKIDGAPJH = new FFIBFAFPEGF();
			KJBKIDGAPJH.Type = BEMAILNFOHL.BUTTON_TYPE_MIDDLE;
			fFIBFAFPEGF = KJBKIDGAPJH;
			break;
		default:
			LLLOJBFMONN.Error("Strange typeName %s", text);
			break;
		}
		if (fFIBFAFPEGF != null)
		{
			fFIBFAFPEGF.GGDJIPKMKFC = gGDJIPKMKFC;
			fFIBFAFPEGF.Color = mDADHHOFCNG;
			fFIBFAFPEGF.DJBAIAKOIHM.AddEventListener(1, OnActionComplete);
			NLJLHHNPCAO(EPKLCPOEELO, fFIBFAFPEGF.DJBAIAKOIHM);
		}
		else
		{
			LLLOJBFMONN.Error("button is null");
		}
	}

	private void KPINGLIDIGI(XmlNode EPKLCPOEELO)
	{
		KOGNLFOPACB = EPKLCPOEELO.Attributes["Fight"].CIPOICEEIBK(string.Empty);
	}

	private void BCLEMHHFPDO(XmlNode EPKLCPOEELO)
	{
		GDOAGJEBCFK = new DialogCheckBox();
		GDOAGJEBCFK.JKHGBEJBCDG = EPKLCPOEELO.Attributes["InitialValue"].CIPOICEEIBK(string.Empty);
		GDOAGJEBCFK.GGDJIPKMKFC = EPKLCPOEELO.Attributes["Text"].CIPOICEEIBK(string.Empty);
		foreach (XmlNode childNode in EPKLCPOEELO.ChildNodes)
		{
			string name = childNode.Name;
			if (name == "On")
			{
				NLJLHHNPCAO(childNode, GDOAGJEBCFK.LCBILMMHDGP);
			}
			else if (name == "Off")
			{
				NLJLHHNPCAO(childNode, GDOAGJEBCFK.ENHDNOBHIHA);
			}
		}
	}

	private TextTimer POAECPMINAM(XmlNode node, string IFHPLGGBDPM)
	{
		string text = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		TextTimer dKPAACCMAPO = new TextTimer();
		dKPAACCMAPO.Color = Constants.KLLKHFKHCGK;
		switch (text)
		{
		case "EnergyRefillTimer":
			dKPAACCMAPO.Delegate = ListSF.ELEBLBJKDBI().BBDOJLNOHLO;
			break;
		case "DuelAccessibilityTimer":
			dKPAACCMAPO.Delegate = ListSF.ELEBLBJKDBI().JNKBLMLEJGE;
			break;
		case "DeliveryTimer":
		{
			dKPAACCMAPO.Delegate = ListSF.ELEBLBJKDBI().ENMEBKHLCHF;
			string gOHIIMFFFJI = node.Attributes["Item"].CIPOICEEIBK(string.Empty);
			UserItem bAINMLLIKOL = ListSF.CMGOCLGHNLH(gOHIIMFFFJI);
			dKPAACCMAPO.set_Data(bAINMLLIKOL);
			break;
		}
		case "StarterPackTimer":
			dKPAACCMAPO.Delegate = ListSF.ELEBLBJKDBI().OKNJMHBIIGJ;
			break;
		default:
			dKPAACCMAPO.Delegate = ListSF.ELEBLBJKDBI().IAKAPNOBAMJ;
			dKPAACCMAPO.set_Data(text);
			break;
		}
		dKPAACCMAPO.set_Label(null);
		return dKPAACCMAPO;
	}

	private void BCDBAPMJOJD()
	{
		if (GDOAGJEBCFK != null)
		{
			GDOAGJEBCFK.LCBILMMHDGP.FHPKJMMLIEG();
			GDOAGJEBCFK.ENHDNOBHIHA.FHPKJMMLIEG();
		}
	}

	private float MPNBGBIMEIP()
	{
		float result = -1f;
		if (KOGNLFOPACB != string.Empty)
		{
			ConditionExtension.CompareResult lNIDLHOIHIM = new ConditionExtension.CompareResult();
			QuestCondition kKDGLNECFHA = new QuestCondition();
			kKDGLNECFHA.LIMHBJBEEIA(EHMMGHDNIJL);
			kKDGLNECFHA.MCPIOGALBMK(KOGNLFOPACB, lNIDLHOIHIM);
			FightIDS mOCEDDJOAEB = new FightIDS();
			mOCEDDJOAEB.SetFightIDSByString(lNIDLHOIHIM.resultSTR);
			FightList jDIPBIHBGPF = ListSF.CHMCKGCDGCM(mOCEDDJOAEB);
			if (jDIPBIHBGPF != null)
			{
				List<ModelParameters> list = GameUtils.IGNNMAKHBFF(jDIPBIHBGPF.OFKJMHPMCCD());
				ModelParameters aCENLMONNPA = GameUtils.LBMPHBNJMGG();
				result = jDIPBIHBGPF.MPNBGBIMEIP(aCENLMONNPA, list);
				list.Clear();
			}
		}
		return result;
	}

	private void EGCODMJHDBI(object data)
	{
		if (IDIHEPPFMMF)
		{
			return;
		}
		int num = ((data != null) ? ((int)data) : 0);
		if (num == 0 && GEIEBHILDME != null)
		{
			IDIHEPPFMMF = true;
			GEIEBHILDME.DJBAIAKOIHM.DEJMHFMLKIC(EHMMGHDNIJL);
		}
		else if (num == 0 && GEIEBHILDME == null)
		{
			IDIHEPPFMMF = true;
			OGIJONMKABB();
		}
		else if (num == 1 && JFKGFNHJLLE != null)
		{
			IDIHEPPFMMF = true;
			JFKGFNHJLLE.DJBAIAKOIHM.DEJMHFMLKIC(EHMMGHDNIJL);
		}
		else if (num == 2 && KJBKIDGAPJH != null)
		{
			IDIHEPPFMMF = true;
			KJBKIDGAPJH.DJBAIAKOIHM.DEJMHFMLKIC(EHMMGHDNIJL);
		}
		else if (num == 3 && GDOAGJEBCFK != null)
		{
			GDOAGJEBCFK.LCBILMMHDGP.DEJMHFMLKIC(EHMMGHDNIJL);
			BCDBAPMJOJD();
		}
		else if (num == 4 && GDOAGJEBCFK != null)
		{
			GDOAGJEBCFK.ENHDNOBHIHA.DEJMHFMLKIC(EHMMGHDNIJL);
			BCDBAPMJOJD();
		}
		else
		{
			if (num >= timersID)
			{
				return;
			}
			for (int i = 0; i < KEJEJIIBBGM.Count; i++)
			{
				CLNIMHCJIAL cLNIMHCJIAL = KEJEJIIBBGM[i];
				if (cLNIMHCJIAL.Id == num)
				{
					cLNIMHCJIAL.DJBAIAKOIHM.DEJMHFMLKIC(EHMMGHDNIJL);
					break;
				}
			}
		}
	}

	private void OnActionComplete(object data)
	{
		OGIJONMKABB();
	}

	protected virtual void NLJLHHNPCAO(XmlNode EPKLCPOEELO, QuestActionsSequence AFENHJFICNN)
	{
		foreach (XmlNode childNode in EPKLCPOEELO.ChildNodes)
		{
			string name = childNode.Name;
			QuestAction mBAAKHELFKL = QuestAction.GetClassActionByName(name);
			mBAAKHELFKL.ONGHPGEIJEN = ONGHPGEIJEN;
			mBAAKHELFKL.Parse(childNode);
			AFENHJFICNN.NLJLHHNPCAO(mBAAKHELFKL);
		}
	}
}
