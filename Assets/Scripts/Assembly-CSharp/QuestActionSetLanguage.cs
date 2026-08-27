using System.Xml;

public class QuestActionSetLanguage : QuestAction
{
	private string BGMCCMHOMJL;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		BGMCCMHOMJL = EPKLCPOEELO.Attributes["Name"].CIPOICEEIBK(string.Empty);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		ConditionExtension.CompareResult lNIDLHOIHIM = new ConditionExtension.CompareResult();
		QuestCondition kKDGLNECFHA = new QuestCondition();
		kKDGLNECFHA.LIMHBJBEEIA(GFIHPBCEEOB);
		kKDGLNECFHA.MCPIOGALBMK(BGMCCMHOMJL, lNIDLHOIHIM);
		string kEEACJILEEK = lNIDLHOIHIM.ToString();
		LocalizationManager.Language pPNFBAFOOAH = LocalizationManager.NLFKNPBICED(kEEACJILEEK);
		if (pPNFBAFOOAH != null)
		{
			LocalizationManager.BJPNKAGDKFL(pPNFBAFOOAH);
		}
		OGIJONMKABB();
	}
}
