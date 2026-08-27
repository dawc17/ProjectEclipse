using System.Xml;
using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Map;

public class QuestActionShowRaidToggleBtn : QuestAction
{
	private bool _visible = true;

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		_visible = node.Attributes["Value"].ParseInt(1) != 0;
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		MapScene current = Scene<MapScene>.get_Current();
		if (current != null)
		{
			current.SetRaidToggleVisible(_visible);
		}
		OGIJONMKABB();
	}
}
