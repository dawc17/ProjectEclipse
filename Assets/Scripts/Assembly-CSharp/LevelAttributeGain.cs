using System.Xml;

public class LevelAttributeGain
{
	public Attributes KGMDIGIONNB = new Attributes();

	public void Parse(XmlNode node)
	{
		KGMDIGIONNB.Clear();
		foreach (XmlAttribute attribute in node.Attributes)
		{
			string name = attribute.Name;
			int bAINMLLIKOL = XmlUtils.ParseInt(attribute);
			KGMDIGIONNB.Set(name, bAINMLLIKOL);
		}
	}
}
