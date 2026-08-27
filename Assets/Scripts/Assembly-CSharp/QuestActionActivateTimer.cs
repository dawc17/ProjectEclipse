using System.Xml;

public class QuestActionActivateTimer : QuestAction
{
	private string GAADCGKKMEN;

	private string GHGKCHMAEKC;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		GAADCGKKMEN = EPKLCPOEELO.Attributes["Name"].CIPOICEEIBK(string.Empty);
		GHGKCHMAEKC = EPKLCPOEELO.Attributes["Value"].CIPOICEEIBK(string.Empty);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		ConditionExtension.CompareResult lNIDLHOIHIM = new ConditionExtension.CompareResult();
		QuestCondition kKDGLNECFHA = new QuestCondition();
		kKDGLNECFHA.LIMHBJBEEIA(GFIHPBCEEOB);
		kKDGLNECFHA.MCPIOGALBMK(GHGKCHMAEKC, lNIDLHOIHIM);
		long num = (long)lNIDLHOIHIM.resultNumber;
		long num2 = ListSF.IDMJOMOMDOJ();
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		RosterTimerContainer kCMICMHCEBB = nKGLHEGIKKP.AEMFLPNDDKL();
		kCMICMHCEBB.POEJBJOHFDP(GAADCGKKMEN, num2 + num);
		OGIJONMKABB();
	}
}
