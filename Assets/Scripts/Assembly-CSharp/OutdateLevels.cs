using System.Collections.Generic;
using System.Xml;

public class OutdateLevels
{
	private List<OutdateLevelItem> Types = new List<OutdateLevelItem>();

	public void Parse(XmlNode node)
	{
		Types.Clear();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			OutdateLevelItem nCIJAJGCAPA = new OutdateLevelItem();
			nCIJAJGCAPA.Parse(childNode);
			Types.Add(nCIJAJGCAPA);
		}
	}

	public float GetValue(string LFLGCDNKNJI)
	{
		foreach (OutdateLevelItem item in Types)
		{
			if (item.IsType(LFLGCDNKNJI))
			{
				return item.Value;
			}
		}
		return (Types.Count <= 0) ? 0f : Types[0].Value;
	}
}
