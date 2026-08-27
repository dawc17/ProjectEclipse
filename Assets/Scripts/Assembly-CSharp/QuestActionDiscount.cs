using System.Collections.Generic;
using System.Xml;

public class QuestActionDiscount : QuestAction
{
	private string ABKKEDPPPCI = string.Empty;

	private string BJEMAJAOFKE = "0";

	private string CAEBEDLOMBE = "0";

	private string AOIKBGKHJMC = string.Empty;

	private string PPNHCAMHNKJ = string.Empty;

	private string LPENEFLIBEF = string.Empty;

	private string JGPCLKNPCLG = string.Empty;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		ABKKEDPPPCI = EPKLCPOEELO.Attributes["Item"].CIPOICEEIBK(string.Empty);
		BJEMAJAOFKE = EPKLCPOEELO.Attributes["Percent"].CIPOICEEIBK(string.Empty);
		CAEBEDLOMBE = EPKLCPOEELO.Attributes["Toggle"].CIPOICEEIBK(string.Empty);
		PPNHCAMHNKJ = EPKLCPOEELO.Attributes["NewAmount"].CIPOICEEIBK("0");
		LPENEFLIBEF = EPKLCPOEELO.Attributes["NewPrice"].CIPOICEEIBK(string.Empty);
		AOIKBGKHJMC = EPKLCPOEELO.Attributes["Period"].CIPOICEEIBK("0");
		JGPCLKNPCLG = EPKLCPOEELO.Attributes["Sale"].CIPOICEEIBK(string.Empty);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		OGIJONMKABB();
	}

	private void PMMCIFDFCLJ(QuestParameters GFIHPBCEEOB, ItemInfo item, ref int upgradeLevel, ref float IFKAJHEOAEG, ref bool LPPNCLBEAFA, ref long AJKMNFGEHIJ, ref long GKIHFPFHKCI, ref string DDHOJFFGBKM, ref bool GEPBMEMMLEA)
	{
		string empty = string.Empty;
		ConditionExtension.CompareResult lNIDLHOIHIM = new ConditionExtension.CompareResult();
		QuestCondition kKDGLNECFHA = new QuestCondition();
		kKDGLNECFHA.LIMHBJBEEIA(GFIHPBCEEOB);
		kKDGLNECFHA.MCPIOGALBMK(ABKKEDPPPCI, lNIDLHOIHIM);
		empty = lNIDLHOIHIM.ToString();
		string text = string.Empty;
		string text2 = string.Empty;
		List<string> list = new List<string>(empty.Split('|'));
		int count = list.Count;
		if (count > 0)
		{
			text = list[0];
		}
		if (count >= 2)
		{
			text2 = list[1];
		}
		if (text2 != string.Empty)
		{
			kKDGLNECFHA.MCPIOGALBMK(text2, lNIDLHOIHIM);
			upgradeLevel = (int)lNIDLHOIHIM.resultNumber;
		}
		else
		{
			upgradeLevel = -1;
		}
		lNIDLHOIHIM.Clear();
		kKDGLNECFHA.MCPIOGALBMK(BJEMAJAOFKE, lNIDLHOIHIM);
		IFKAJHEOAEG = (float)lNIDLHOIHIM.resultNumber;
		lNIDLHOIHIM.Clear();
		kKDGLNECFHA.MCPIOGALBMK(CAEBEDLOMBE, lNIDLHOIHIM);
		LPPNCLBEAFA = lNIDLHOIHIM.resultNumber > 0.0;
		lNIDLHOIHIM.Clear();
		kKDGLNECFHA.MCPIOGALBMK(PPNHCAMHNKJ, lNIDLHOIHIM);
		AJKMNFGEHIJ = (long)lNIDLHOIHIM.resultNumber;
		lNIDLHOIHIM.Clear();
		kKDGLNECFHA.MCPIOGALBMK(AOIKBGKHJMC, lNIDLHOIHIM);
		GKIHFPFHKCI = (long)lNIDLHOIHIM.resultNumber;
		lNIDLHOIHIM.Clear();
		kKDGLNECFHA.MCPIOGALBMK(LPENEFLIBEF, lNIDLHOIHIM);
		if (LPENEFLIBEF != string.Empty && lNIDLHOIHIM.INCOIAANDCO())
		{
			DDHOJFFGBKM = lNIDLHOIHIM.resultNumber.ToString();
		}
		else
		{
			DDHOJFFGBKM = lNIDLHOIHIM.resultSTR;
		}
		lNIDLHOIHIM.Clear();
		kKDGLNECFHA.MCPIOGALBMK(JGPCLKNPCLG, lNIDLHOIHIM);
		GEPBMEMMLEA = lNIDLHOIHIM.resultNumber > 0.0;
		item = ListSF.DJBOFEEKJMP().KCCDBEEKBCG(text);
		if (item == null)
		{
			LLLOJBFMONN.Error("QuestActionDiscount - cant find item \"%s\" from name \"%s\"", text, ABKKEDPPPCI);
		}
	}
}
