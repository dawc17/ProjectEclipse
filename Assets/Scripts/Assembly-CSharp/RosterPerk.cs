using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

public class RosterPerk
{
	private int _level;

	private int FEAJIHEBIGL;

	private string _name = string.Empty;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private PerkInfoItem AAJEBIINPNL;

	private XmlNode _node;

	public int Level
	{
		get
		{
			return PINDEKDNCNL();
		}
		set
		{
			DLDMOHEGENM(value);
		}
	}

	public int AKKLOMFOLNO
	{
		get
		{
			return DHNNCAEEMLL();
		}
		set
		{
			FMMDLMGHPIB(value);
		}
	}

	public PerkInfoItem MBDDKGIOOGD
	{
		get
		{
			return DFOELJAEEGG();
		}
		set
		{
			NOLDHAFMOLF(value);
		}
	}

	public RosterPerk(XmlNode node)
	{
		_node = node;
		XmlAttribute xmlAttribute = _node.Attributes["Level"];
		if (xmlAttribute == null || string.IsNullOrEmpty(xmlAttribute.Value))
		{
			xmlAttribute = _node.LLIKNHNLGJJ("Level");
			xmlAttribute.Value = "0";
		}
		XmlAttribute xmlAttribute2 = _node.Attributes["Name"];
		if (xmlAttribute2 == null || string.IsNullOrEmpty(xmlAttribute2.Value))
		{
			xmlAttribute2 = _node.LLIKNHNLGJJ("Name");
		}
		_level = xmlAttribute.ParseInt();
		_name = xmlAttribute2.CIPOICEEIBK(string.Empty);
		FEAJIHEBIGL = _node.Attributes["UpgradeLevel"].ParseInt();
	}

	public int PINDEKDNCNL()
	{
		return _level;
	}

	public void DLDMOHEGENM(int value)
	{
		_level = value;
		XmlAttribute xmlAttribute = _node.Attributes["Level"];
		if (xmlAttribute == null)
		{
			xmlAttribute = _node.LLIKNHNLGJJ("Level");
		}
		xmlAttribute.Value = _level.ToString();
	}

	public int DHNNCAEEMLL()
	{
		return FEAJIHEBIGL;
	}

	public void FMMDLMGHPIB(int value)
	{
		FEAJIHEBIGL = value;
		XmlAttribute xmlAttribute = _node.Attributes["UpgradeLevel"];
		if (xmlAttribute == null)
		{
			xmlAttribute = _node.LLIKNHNLGJJ("UpgradeLevel");
		}
		xmlAttribute.Value = FEAJIHEBIGL.ToString();
	}

	public string get_Name()
	{
		return _name;
	}

	public void set_Name(string value)
	{
		_name = value;
		XmlAttribute xmlAttribute = _node.Attributes["Name"];
		if (xmlAttribute == null)
		{
			xmlAttribute = _node.LLIKNHNLGJJ("Name");
		}
		xmlAttribute.Value = _name;
	}

	public PerkInfoItem DFOELJAEEGG()
	{
		return AAJEBIINPNL;
	}

	public void NOLDHAFMOLF(PerkInfoItem value)
	{
		AAJEBIINPNL = value;
	}

	public void AppendNodeChild(Dictionary<string, string> MPEHGKBJPEN)
	{
		XmlNode xmlNode = _node["Set"];
		if (xmlNode != null)
		{
			_node.RemoveChild(xmlNode);
		}
		if (MPEHGKBJPEN.Count == 0)
		{
			return;
		}
		XmlNode mEEAKLDGLDF = _node.ACBPMPMPKJJ("Set");
		foreach (KeyValuePair<string, string> item in MPEHGKBJPEN)
		{
			mEEAKLDGLDF.LLIKNHNLGJJ(item.Key).Value = item.Value;
		}
	}
}
