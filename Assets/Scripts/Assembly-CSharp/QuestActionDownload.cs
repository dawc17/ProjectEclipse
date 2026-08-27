using System.IO;
using System.Xml;
using Nekki.SF2.GUI.Dialogs;

public class QuestActionDownload : QuestAction
{
	private QuestActionsSequence DBONDAIEBPN = new QuestActionsSequence();

	private QuestActionsSequence LDDDPGLPHCO = new QuestActionsSequence();

	private string name;

	private string KPBNOBNELAH;

	private JBKAOMLJCEL COPKLEDMPPD;

	private bool isRewriteHashes;

	private DownloadingScreen downloadingScreen;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		name = EPKLCPOEELO.Attributes["Pack"].CIPOICEEIBK();
		KPBNOBNELAH = EPKLCPOEELO.Attributes["ProgressBarTitle"].CIPOICEEIBK();
		COPKLEDMPPD = null;
		isRewriteHashes = EPKLCPOEELO.Attributes["RewriteHashes"].ParseInt() > 0;
		XmlNode ePKLCPOEELO = EPKLCPOEELO["Success"];
		XmlNode ePKLCPOEELO2 = EPKLCPOEELO["Error"];
		APKBANHAEGN(ePKLCPOEELO, DBONDAIEBPN, OnActionComplete);
		APKBANHAEGN(ePKLCPOEELO2, LDDDPGLPHCO, OnActionComplete);
	}

	private void OnActionComplete(object data)
	{
		OGIJONMKABB();
		if (COPKLEDMPPD != null && COPKLEDMPPD.EFJLHFFGCIF)
		{
			PJGEOIKPGFH();
			GameUtils.BKFMHANNIEF();
		}
	}

	public override void GKFMJKAAJCA()
	{
		base.GKFMJKAAJCA();
		DBONDAIEBPN.FHPKJMMLIEG();
		LDDDPGLPHCO.FHPKJMMLIEG();
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		GKFMJKAAJCA();
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		ConditionExtension.CompareResult lNIDLHOIHIM = new ConditionExtension.CompareResult();
		QuestCondition kKDGLNECFHA = new QuestCondition();
		kKDGLNECFHA.LIMHBJBEEIA(GFIHPBCEEOB);
		kKDGLNECFHA.MCPIOGALBMK(name, lNIDLHOIHIM);
		COPKLEDMPPD = GeneralConfig.NNFMKNJJDDD.OCKOCHAINHG(lNIDLHOIHIM.resultSTR);
		if (COPKLEDMPPD == null)
		{
			LLLOJBFMONN.Error("QuestActionDownload noName: {0}", lNIDLHOIHIM.resultSTR);
			LDDDPGLPHCO.DEJMHFMLKIC(GFIHPBCEEOB);
			return;
		}
		string text = NekkiMath.randomInt(1000000).ToString();
		string text2 = ((COPKLEDMPPD == null) ? string.Empty : COPKLEDMPPD.Url);
		text2 += "?";
		text2 += text;
		if (AssemblyController.AOIJKOFDHIC() && (SystemProperties.PPFPHAKMNLC() || SystemProperties.CEJMCBKCPOH() || SystemProperties.AOJIOMDCEKN()))
		{
			Complete();
			return;
		}
		downloadingScreen = DownloadingScreen.get_Instance();
		downloadingScreen.set_TitleAlias(KPBNOBNELAH);
		downloadingScreen.set_Progress(0f);
		FileDownloader.ELEBLBJKDBI().EMANDFAOCNO(text2, COPKLEDMPPD.Name, SF2Paths.MEKBAHBKMNB(), OnLoadContent, OnProgressContent, COPKLEDMPPD.HKPOAABOLHN);
	}

	private void OnProgressContent(float progress)
	{
		if (downloadingScreen != null)
		{
			downloadingScreen.set_Progress(progress);
		}
	}

	private void OnLoadContent(bool DCJLKCFKCOM)
	{
		bool flag = false;
		string text = string.Format("{0}/{1}", SF2Paths.MEKBAHBKMNB(), COPKLEDMPPD.Name);
		if (DCJLKCFKCOM && File.Exists(text))
		{
			string text2 = MD5Utils.PIFDHBHOMJL(text);
			flag = text2.Equals(COPKLEDMPPD.NDDHELJHHKI.ToUpper());
		}
		if (flag)
		{
			Complete();
		}
		else
		{
			LDDDPGLPHCO.DEJMHFMLKIC(PAJDEKLLFNJ);
		}
		if (downloadingScreen != null)
		{
			DownloadingScreen.Destroy();
			downloadingScreen = null;
		}
	}

	private void Complete()
	{
		string aHLPODLKBEP = SystemProperties.KCJMMIEBLHL().ToString();
		PacksController.ELEBLBJKDBI().DDKKLHDOFNG(COPKLEDMPPD.Name, COPKLEDMPPD.Url, aHLPODLKBEP, -1L, COPKLEDMPPD.NBEEINKJMPK);
		if (isRewriteHashes)
		{
			ListSF.ELEBLBJKDBI().EMJLEBDAALP();
		}
		DBONDAIEBPN.DEJMHFMLKIC(PAJDEKLLFNJ);
	}
}
