using System.Collections.Generic;
using System.Xml;

public class QuestActionClearStack : QuestAction
{
	private List<string> NIKHAICFGNM = new List<string>();

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		string text = EPKLCPOEELO.Attributes["Name"].CIPOICEEIBK(string.Empty);
		string[] collection = text.Split('|');
		NIKHAICFGNM.AddRange(collection);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		ListSF.ELEBLBJKDBI().ClearQuestsStack(NIKHAICFGNM);
		OGIJONMKABB();
	}
}
