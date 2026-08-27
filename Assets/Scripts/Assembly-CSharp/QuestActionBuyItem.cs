using System.Xml;

public class QuestActionBuyItem : QuestAction
{
	private string _name = string.Empty;

	private string JJPFBOKGIEF = string.Empty;

	private ItemAction _itemAction;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		_name = EPKLCPOEELO.Attributes["Name"].CIPOICEEIBK(string.Empty);
		JJPFBOKGIEF = EPKLCPOEELO.Attributes["Currency"].CIPOICEEIBK(string.Empty);
		if (JJPFBOKGIEF == "Coins")
		{
			_itemAction = ItemAction.Item_Buy_Gold;
		}
		else if (JJPFBOKGIEF == "Ruby")
		{
			_itemAction = ItemAction.Item_Buy_Ruby;
		}
		else if (JJPFBOKGIEF == "Real")
		{
			_itemAction = ItemAction.Item_Buy_Real;
		}
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		ConditionExtension.CompareResult lNIDLHOIHIM = new ConditionExtension.CompareResult();
		QuestCondition kKDGLNECFHA = new QuestCondition();
		kKDGLNECFHA.LIMHBJBEEIA(GFIHPBCEEOB);
		kKDGLNECFHA.MCPIOGALBMK(_name, lNIDLHOIHIM);
		string gOHIIMFFFJI = lNIDLHOIHIM.ToString();
		ItemInfo dJKEECEOCJB = ListSF.DJBOFEEKJMP().KCCDBEEKBCG(gOHIIMFFFJI);
		if (dJKEECEOCJB != null)
		{
			GameUtils.KBHDKPAMOJN(dJKEECEOCJB, _itemAction);
		}
		OGIJONMKABB();
	}
}
