using System.Xml;

public class QuestActionRemovePack : QuestAction
{
	private string GAFGMNPOEGE = string.Empty;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		GAFGMNPOEGE = EPKLCPOEELO.Attributes["Name"].CIPOICEEIBK(string.Empty);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		if (GAFGMNPOEGE != string.Empty)
		{
			PacksController.ELEBLBJKDBI().DeletePack(GAFGMNPOEGE);
			ListSF.ELEBLBJKDBI().EMJLEBDAALP();
		}
		OGIJONMKABB();
	}
}
