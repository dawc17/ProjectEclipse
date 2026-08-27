using System.Xml;
using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Map;

public class QuestActionGotoZone : QuestAction
{
	private string _name = string.Empty;

	private int _frames;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		_name = EPKLCPOEELO.Attributes["Name"].CIPOICEEIBK(string.Empty);
		_frames = EPKLCPOEELO.Attributes["Frames"].ParseInt();
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		MapScene current = Scene<MapScene>.get_Current();
		if (current != null)
		{
			current.GotoZoneByName(_name, _frames);
		}
		OGIJONMKABB();
	}
}
