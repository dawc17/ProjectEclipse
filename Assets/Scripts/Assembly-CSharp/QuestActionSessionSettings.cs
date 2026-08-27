using System.Xml;

public class QuestActionSessionSettings : QuestAction
{
	private string DDNHIFDLEPK = string.Empty;

	private string NILCJDPHEEE = string.Empty;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		DDNHIFDLEPK = EPKLCPOEELO.Attributes["Name"].CIPOICEEIBK(string.Empty);
		NILCJDPHEEE = EPKLCPOEELO.Attributes["Value"].CIPOICEEIBK(string.Empty);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		if (nKGLHEGIKKP != null)
		{
			nKGLHEGIKKP.SessionSettings(DDNHIFDLEPK, NILCJDPHEEE);
		}
		ListSF.ELEBLBJKDBI().EJANJEEGOOE();
		OGIJONMKABB();
	}
}
