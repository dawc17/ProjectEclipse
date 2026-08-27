using System.Collections.Generic;
using System.Xml;

public class AchievCounter
{
	public string Name;

	public List<Achievement> FOICCCGPCMJ = new List<Achievement>();

	public AchievCounter(XmlNode node)
	{
		Name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		foreach (XmlNode childNode in node.ChildNodes)
		{
			Achievement item = new Achievement(childNode);
			FOICCCGPCMJ.Add(item);
		}
		FOICCCGPCMJ.Sort((Achievement LHBNIMGFKIB, Achievement AAOIAEJJINO) => LHBNIMGFKIB.EOGLBDCLMBM.CompareTo(AAOIAEJJINO.EOGLBDCLMBM));
	}
}
