using System.Xml;

public class RepostAchievement
{
	private XmlNode _node;

	private string _name = string.Empty;

	public RepostAchievement(XmlNode node)
	{
		_node = node;
		_name = node.Attributes["Name"].CIPOICEEIBK();
	}

	public RepostAchievement(XmlNode FMBDAPOMFGN, string name)
	{
		_node = FMBDAPOMFGN.ACBPMPMPKJJ("RepostAchievement");
		set_Name(name);
	}

	public string get_Name()
	{
		return _name;
	}

	public void set_Name(string value)
	{
		_name = value;
		if (_node.Attributes["Name"] == null)
		{
			_node.LLIKNHNLGJJ("Name");
		}
		_node.Attributes["Name"].Value = _name;
	}
}
