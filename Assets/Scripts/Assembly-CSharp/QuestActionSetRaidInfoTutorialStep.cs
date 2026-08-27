using System.Xml;

public class QuestActionSetRaidInfoTutorialStep : QuestAction
{
	private string NOFMEBBEDKK = string.Empty;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		NOFMEBBEDKK = EPKLCPOEELO.Attributes["Value"].CIPOICEEIBK(string.Empty);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		ListSF.CCDKHLAMKKO().BKBHIMEEDBG().PECCKNJMNJP(GameUtils.PHHOCKCCGMM(NOFMEBBEDKK));
		OGIJONMKABB();
	}
}
