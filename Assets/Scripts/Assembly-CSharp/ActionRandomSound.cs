using System.Collections.Generic;
using System.Xml;

public class ActionRandomSound : ActionAnimation
{
	private List<string> _Names = new List<string>();

	private bool _AnyGender;

	private string _Gender;

	public ActionRandomSound(XmlNode node)
		: base(FADAJCEEKIO.RANDOM_SOUND)
	{
		Parse(node);
	}

	public string get_Name()
	{
		return _Names.CJBCAIOBHMP();
	}

	public override void Visit(Model ACENLMONNPA)
	{
		ACENLMONNPA.OPPIKLBKMPN(this);
	}

	public bool SameGender(string EMENABICHED)
	{
		return _AnyGender || EMENABICHED == _Gender;
	}

	protected override void Parse(XmlNode node)
	{
		base.Parse(node);
		XmlAttribute xmlAttribute = node.Attributes["Voice"];
		_AnyGender = xmlAttribute == null;
		_Gender = xmlAttribute.CIPOICEEIBK(string.Empty);
		foreach (XmlNode childNode in node.ChildNodes)
		{
			_Names.Add(childNode.Attributes["Name"].CIPOICEEIBK("ERR_RAND_SOUND_NO_NAME"));
		}
	}
}
