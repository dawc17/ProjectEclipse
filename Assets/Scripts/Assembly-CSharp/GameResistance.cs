using System.Xml;

public class GameResistance
{
	public int EFANAIIGEMO;

	public string Name;

	public string MJBPMLCLMFN;

	public string Color;

	public GameResistance(XmlNode node)
	{
		Name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		EFANAIIGEMO = node.Attributes["MaxValue"].ParseInt();
		MJBPMLCLMFN = node.Attributes["Icon"].CIPOICEEIBK(string.Empty);
		Color = node.Attributes["Color"].CIPOICEEIBK(string.Empty);
	}

	public string CIOKDNDHFBE()
	{
		return MJBPMLCLMFN;
	}
}
