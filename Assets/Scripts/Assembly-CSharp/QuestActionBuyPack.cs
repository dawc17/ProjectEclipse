using System.Xml;

public class QuestActionBuyPack : QuestAction
{
	private string _PackName;

	private QuestActionsSequence BCMBLGBENNN = new QuestActionsSequence();

	private QuestActionsSequence AKGCHOFDMDN = new QuestActionsSequence();

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		_PackName = EPKLCPOEELO.Attributes["PackName"].CIPOICEEIBK(string.Empty);
		XmlNode ePKLCPOEELO = EPKLCPOEELO["Success"];
		XmlNode ePKLCPOEELO2 = EPKLCPOEELO["Error"];
		APKBANHAEGN(ePKLCPOEELO, BCMBLGBENNN, OnActionComplete);
		APKBANHAEGN(ePKLCPOEELO2, AKGCHOFDMDN, OnActionComplete);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		GKFMJKAAJCA();
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		ConditionExtension.CompareResult bMDEBHIHIAJ = new ConditionExtension.CompareResult();
		QuestCondition kKDGLNECFHA = new QuestCondition();
		kKDGLNECFHA.LIMHBJBEEIA(GFIHPBCEEOB);
		kKDGLNECFHA.MCPIOGALBMK(_PackName, bMDEBHIHIAJ);
	}

	public override void GKFMJKAAJCA()
	{
		BCMBLGBENNN.FHPKJMMLIEG();
		AKGCHOFDMDN.FHPKJMMLIEG();
	}

	private void OnActionComplete(object data)
	{
		OGIJONMKABB();
	}
}
