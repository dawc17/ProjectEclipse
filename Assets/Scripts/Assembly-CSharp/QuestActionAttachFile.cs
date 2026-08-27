using System.Xml;

public class QuestActionAttachFile : QuestAction
{
	private string IEICEKFPADK = string.Empty;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		IEICEKFPADK = EPKLCPOEELO.Attributes["File"].CIPOICEEIBK(string.Empty);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		if (IEICEKFPADK != string.Empty)
		{
			ListSF.ELEBLBJKDBI().PDCHBPKOBFI(IEICEKFPADK);
		}
		OGIJONMKABB();
	}
}
