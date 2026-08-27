using System.Xml;

public class QuestActionHideMapButton : QuestAction
{
	private string _name = string.Empty;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		_name = EPKLCPOEELO.Attributes["Name"].CIPOICEEIBK(string.Empty);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		string name = string.Empty;
		GetValues(ref name);
		MapButtonController.ELEBLBJKDBI().DMCBGLJHBPA(name);
		OGIJONMKABB();
	}

	private void GetValues(ref string name)
	{
		ConditionExtension.CompareResult lNIDLHOIHIM = new ConditionExtension.CompareResult();
		QuestCondition kKDGLNECFHA = new QuestCondition();
		kKDGLNECFHA.LIMHBJBEEIA(PAJDEKLLFNJ);
		kKDGLNECFHA.MCPIOGALBMK(_name, lNIDLHOIHIM);
		name = lNIDLHOIHIM.ToString();
	}
}
