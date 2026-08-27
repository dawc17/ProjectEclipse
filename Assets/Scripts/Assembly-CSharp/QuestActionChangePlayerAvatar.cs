using System.Xml;

public class QuestActionChangePlayerAvatar : QuestAction
{
	private string MBMAENOMLHF;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		MBMAENOMLHF = EPKLCPOEELO.Attributes["Avatar"].CIPOICEEIBK(string.Empty);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		string FHLFEBDNIFF = string.Empty;
		GetValues(ref FHLFEBDNIFF);
		if (GameUtils.CJKKIOIMGAC(FHLFEBDNIFF))
		{
			ListSF.CCDKHLAMKKO().BAOKBJGLKEF(FHLFEBDNIFF);
		}
		OGIJONMKABB();
	}

	private void GetValues(ref string FHLFEBDNIFF)
	{
		ConditionExtension.CompareResult lNIDLHOIHIM = new ConditionExtension.CompareResult();
		QuestCondition kKDGLNECFHA = new QuestCondition();
		kKDGLNECFHA.LIMHBJBEEIA(PAJDEKLLFNJ);
		kKDGLNECFHA.MCPIOGALBMK(MBMAENOMLHF, lNIDLHOIHIM);
		FHLFEBDNIFF = lNIDLHOIHIM.ToString();
	}
}
