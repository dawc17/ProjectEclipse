using System.Xml;

public class QuestActionDeliver : QuestAction
{
	private string DLKPBAJDHBO;

	private string KEHBCHJDCND;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		DLKPBAJDHBO = EPKLCPOEELO.Attributes["Item"].CIPOICEEIBK(string.Empty);
		KEHBCHJDCND = EPKLCPOEELO.Attributes["Enchantment"].CIPOICEEIBK(string.Empty);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		ConditionExtension.CompareResult lNIDLHOIHIM = new ConditionExtension.CompareResult();
		QuestCondition kKDGLNECFHA = new QuestCondition();
		kKDGLNECFHA.LIMHBJBEEIA(GFIHPBCEEOB);
		kKDGLNECFHA.MCPIOGALBMK(DLKPBAJDHBO, lNIDLHOIHIM);
		string text = lNIDLHOIHIM.ToString();
		if (!text.Equals("0"))
		{
			ItemBuyHelper.BuyImmediatelyDelivery(text);
		}
		else
		{
			string empty = string.Empty;
			kKDGLNECFHA.MCPIOGALBMK(KEHBCHJDCND, lNIDLHOIHIM);
			empty = lNIDLHOIHIM.ToString();
			GameUtils.PKMIJDBNNFK(empty);
		}
		OGIJONMKABB();
	}
}
