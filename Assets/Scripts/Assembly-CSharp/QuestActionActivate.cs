using System.Xml;

public class QuestActionActivate : QuestAction
{
	private string EKAMIDAEKMK = string.Empty;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		EKAMIDAEKMK = EPKLCPOEELO.Attributes["ActionID"].CIPOICEEIBK(string.Empty);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		QuestParameters hHKLFIIBIFF = ListSF.ELEBLBJKDBI().BNMLDPNCMLB();
		hHKLFIIBIFF.NPMDMOIHBFP = new RosterQuest.NOKCOAHJIPB(EKAMIDAEKMK, EKAMIDAEKMK);
		ListSF.ELEBLBJKDBI().FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_ACTIVATE);
		hHKLFIIBIFF.NPMDMOIHBFP = null;
		OGIJONMKABB();
	}
}
