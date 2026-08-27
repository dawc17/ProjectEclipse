using System.Collections.Generic;
using System.Xml;

public class OutdateLevelItem
{
	public float Value;

	public List<string> Types = new List<string>();

	public void Parse(XmlNode node)
	{
		Value = node.Attributes["Value"].ParseFloat();
		string text = node.Attributes["Type"].CIPOICEEIBK(string.Empty);
		if (text != null)
		{
			string[] collection = text.Split('|');
			Types.AddRange(collection);
		}
	}

	public bool IsType(string value)
	{
		foreach (string item in Types)
		{
			if (item.Equals(value))
			{
				return true;
			}
		}
		return false;
	}
}
