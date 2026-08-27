using System.Xml;

public class QuestActionVariable : QuestAction
{
	private string _name = string.Empty;

	private string _value = string.Empty;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		_name = EPKLCPOEELO.Attributes["Name"].CIPOICEEIBK(string.Empty);
		_value = EPKLCPOEELO.Attributes["Value"].CIPOICEEIBK(string.Empty);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		KPCNKLECCKB(GFIHPBCEEOB);
		OGIJONMKABB();
	}

	public void KPCNKLECCKB(QuestParameters JCICKLIMBEF)
	{
		ConditionExtension.CompareResult lNIDLHOIHIM = new ConditionExtension.CompareResult();
		QuestCondition kKDGLNECFHA = new QuestCondition();
		kKDGLNECFHA.LIMHBJBEEIA(JCICKLIMBEF);
		string bAINMLLIKOL = string.Empty;
		if (!string.IsNullOrEmpty(_value))
		{
			kKDGLNECFHA.MCPIOGALBMK(_value, lNIDLHOIHIM);
			bAINMLLIKOL = lNIDLHOIHIM.ToString();
		}
		lNIDLHOIHIM.Clear();
		kKDGLNECFHA.MCPIOGALBMK(_name, lNIDLHOIHIM);
		string gOHIIMFFFJI = lNIDLHOIHIM.ToString();
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		nKGLHEGIKKP.SetQuestVariable(gOHIIMFFFJI, bAINMLLIKOL);
		ListSF.ELEBLBJKDBI().EJANJEEGOOE();
	}
}
