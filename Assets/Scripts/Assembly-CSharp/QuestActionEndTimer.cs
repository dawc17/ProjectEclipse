using System.Xml;

public class QuestActionEndTimer : QuestAction
{
	private string GAADCGKKMEN;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		GAADCGKKMEN = EPKLCPOEELO.Attributes["Name"].CIPOICEEIBK(string.Empty);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		RosterTimerContainer kCMICMHCEBB = nKGLHEGIKKP.AEMFLPNDDKL();
		kCMICMHCEBB.IPKMLCMAINI(GAADCGKKMEN);
		OGIJONMKABB();
	}
}
