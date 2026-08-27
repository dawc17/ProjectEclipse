using System.Xml;

public class Shock
{
	public float LILMAHHANIL;

	public float LPHBGLLAEOG;

	public float NIPKAAEFMNG;

	public string ADAOLENDOME;

	public float PAKGFJEEJLD;

	public string POJAOGMJBDC;

	public int JKOMIENEACF;

	public string JIIFFJAJNNN;

	public string APJJEFJHJGK;

	public int OMPDIOBDAKB;

	public Vector3f IIIDIKABLOJ = new Vector3f();

	public void Parse(XmlNode node)
	{
		LILMAHHANIL = node["Treshold"].Attributes["Value"].ParseFloat();
		LPHBGLLAEOG = node["FrameReduction"].Attributes["Value"].ParseFloat();
		JKOMIENEACF = node["LooseningDelay"].Attributes["Frames"].ParseInt();
		JIIFFJAJNNN = node["Weapon"].Attributes["Name"].CIPOICEEIBK(string.Empty);
		APJJEFJHJGK = node["SetAttribute"].Attributes["Name"].CIPOICEEIBK(string.Empty);
		OMPDIOBDAKB = node["SetAttribute"].Attributes["Value"].ParseInt();
		NIPKAAEFMNG = node["CriticalHitChance"].Attributes["Base"].ParseFloat();
		ADAOLENDOME = node["CriticalHitChance"].Attributes["Attribute"].CIPOICEEIBK(string.Empty);
		PAKGFJEEJLD = node["HeadHitChance"].Attributes["Base"].ParseFloat();
		POJAOGMJBDC = node["HeadHitChance"].Attributes["Attribute"].CIPOICEEIBK(string.Empty);
		IIIDIKABLOJ.JPFALPBDBAP(node["Impulse"].Attributes["X"].ParseFloat());
		IIIDIKABLOJ.IBNFLLGPOLD(node["Impulse"].Attributes["Y"].ParseFloat());
		IIIDIKABLOJ.set_Z(node["Impulse"].Attributes["Z"].ParseFloat());
	}
}
