using System.Xml;

public class QuestActionSetCurrentZone : QuestAction
{
	private string CODCAENBFHK = string.Empty;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		CODCAENBFHK = EPKLCPOEELO.Attributes["Name"].CIPOICEEIBK(string.Empty);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		QuestCondition kKDGLNECFHA = new QuestCondition();
		kKDGLNECFHA.LIMHBJBEEIA(GFIHPBCEEOB);
		ConditionExtension.CompareResult lNIDLHOIHIM = new ConditionExtension.CompareResult();
		kKDGLNECFHA.MCPIOGALBMK(CODCAENBFHK, lNIDLHOIHIM);
		ListSF.CCDKHLAMKKO().AOIBKCOBABL(lNIDLHOIHIM.ToString());
		OGIJONMKABB();
	}
}
