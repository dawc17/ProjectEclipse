using System.Xml;

public class RosterAchievement
{
	private XmlNode _node;

	private string _name = string.Empty;

	private bool _reward;

	public bool HMKHIDIHDAK
	{
		get
		{
			return BLHBOBGKMBN();
		}
		set
		{
			set_Reward(value);
		}
	}

	public RosterAchievement(XmlNode node)
	{
		_node = node;
		_name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		_reward = node.Attributes["ObtainedReward"].CIPOICEEIBK(string.Empty) == "true" || node.Attributes["ObtainedReward"].CIPOICEEIBK(string.Empty) == "1";
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

	public bool BLHBOBGKMBN()
	{
		return _reward;
	}

	public void set_Reward(bool value)
	{
		_reward = value;
		if (_node.Attributes["ObtainedReward"] == null)
		{
			_node.LLIKNHNLGJJ("ObtainedReward");
		}
		_node.Attributes["ObtainedReward"].Value = ((!_reward) ? "false" : "true");
	}
}
