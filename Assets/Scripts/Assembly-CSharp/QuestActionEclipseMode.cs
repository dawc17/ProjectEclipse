using System.Xml;

public class QuestActionEclipseMode : QuestAction
{
	private bool _enabled;

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		string value = node.Attributes["Toggle"].CIPOICEEIBK("Off");
		_enabled = value.Equals("On", System.StringComparison.OrdinalIgnoreCase) ||
			value.Equals("True", System.StringComparison.OrdinalIgnoreCase) || value == "1";
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		Roster roster = ListSF.CCDKHLAMKKO();
		if (roster != null)
		{
			roster.SetEclipseMode(_enabled);
			ListSF.ELEBLBJKDBI().EJANJEEGOOE();
		}
		OGIJONMKABB();
	}
}
