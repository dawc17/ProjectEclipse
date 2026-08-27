using System.Xml;

public class QuestActionToggleGroup : QuestAction
{
	private string _toggle;

	private string _name;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		_toggle = EPKLCPOEELO.Attributes["Toggle"].CIPOICEEIBK("on");
		_name = EPKLCPOEELO.Attributes["Name"].CIPOICEEIBK(string.Empty);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		nKGLHEGIKKP.AOBEHOILNOG(_name, _toggle.Equals("on"));
		ListSF.ELEBLBJKDBI().OnAuthenticate(true);
		OGIJONMKABB();
	}
}
