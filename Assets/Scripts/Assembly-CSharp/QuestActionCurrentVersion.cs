using System.Xml;

public class QuestActionCurrentVersion : QuestAction
{
	private string IGIOOCIDFIN = string.Empty;

	private string IBGMIGIFNJM = string.Empty;

	private string LDKAECLLDNG = string.Empty;

	private string JJCDPPFGPDO = string.Empty;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		IGIOOCIDFIN = EPKLCPOEELO.Attributes["Production"].CIPOICEEIBK(string.Empty);
		IBGMIGIFNJM = EPKLCPOEELO.Attributes["Major"].CIPOICEEIBK(string.Empty);
		LDKAECLLDNG = EPKLCPOEELO.Attributes["Minor"].CIPOICEEIBK(string.Empty);
		JJCDPPFGPDO = EPKLCPOEELO.Attributes["DataVersion"].CIPOICEEIBK(string.Empty);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		ConditionExtension.CompareResult lNIDLHOIHIM = new ConditionExtension.CompareResult();
		ConditionExtension.CompareResult lNIDLHOIHIM2 = new ConditionExtension.CompareResult();
		ConditionExtension.CompareResult lNIDLHOIHIM3 = new ConditionExtension.CompareResult();
		ConditionExtension.CompareResult lNIDLHOIHIM4 = new ConditionExtension.CompareResult();
		QuestCondition kKDGLNECFHA = new QuestCondition();
		kKDGLNECFHA.LIMHBJBEEIA(GFIHPBCEEOB);
		kKDGLNECFHA.MCPIOGALBMK(IGIOOCIDFIN, lNIDLHOIHIM);
		kKDGLNECFHA.MCPIOGALBMK(IBGMIGIFNJM, lNIDLHOIHIM2);
		kKDGLNECFHA.MCPIOGALBMK(LDKAECLLDNG, lNIDLHOIHIM3);
		kKDGLNECFHA.MCPIOGALBMK(JJCDPPFGPDO, lNIDLHOIHIM4);
		string empty = string.Empty;
		empty += lNIDLHOIHIM.ToString();
		empty += ".";
		empty += lNIDLHOIHIM2.ToString();
		empty += ".";
		empty += lNIDLHOIHIM3.ToString();
		empty += ".";
		empty += lNIDLHOIHIM4.ToString();
		SystemProperties.DFJEJKJECBI().SetVersion(empty);
		ListSF.ELEBLBJKDBI().DLAJNCEILEH(empty);
		OGIJONMKABB();
	}
}
