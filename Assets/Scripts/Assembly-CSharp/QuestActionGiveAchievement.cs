using System.Xml;

public class QuestActionGiveAchievement : QuestAction
{
	private string KPONJNPDLCE = string.Empty;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		KPONJNPDLCE = EPKLCPOEELO.Attributes["Name"].CIPOICEEIBK();
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		Achievement jNPIOKEKMII = GameUtils.POFHBHOIMAI(KPONJNPDLCE);
		OGIJONMKABB();
	}
}
