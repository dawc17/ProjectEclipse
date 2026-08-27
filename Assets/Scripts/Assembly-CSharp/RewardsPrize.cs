using System.Collections.Generic;
using System.Xml;

public class RewardsPrize
{
	private string NameAttr = "Value";

	public float NJAIKCKFMNN;

	public float LOONMILKCFK;

	public float GKAEJDCDMHC;

	public float MLNBGDHDKLL;

	public float PMPDAOIGCLP;

	public float APCAKCCOMLO;

	public List<float> Styles = new List<float>();

	public void Parse(XmlNode node)
	{
		PMPDAOIGCLP = node["DefaultPrizeBaseFactor"].Attributes[NameAttr].ParseFloat();
		NJAIKCKFMNN = node["Perfect"].Attributes[NameAttr].ParseFloat();
		LOONMILKCFK = node["FirstStrike"].Attributes[NameAttr].ParseFloat();
		GKAEJDCDMHC = node["ComboCount"].Attributes[NameAttr].ParseFloat();
		MLNBGDHDKLL = node["HeadShot"].Attributes[NameAttr].ParseFloat();
		APCAKCCOMLO = node["Shock"].Attributes[NameAttr].ParseFloat();
		GKGMGLOHEGO(node);
	}

	private void GKGMGLOHEGO(XmlNode node)
	{
		Styles.Clear();
		XmlNode xmlNode = node["Styles"];
		Styles.Add(xmlNode["Turtle"].Attributes[NameAttr].ParseFloat());
		Styles.Add(xmlNode["Hard"].Attributes[NameAttr].ParseFloat());
		Styles.Add(xmlNode["Brutal"].Attributes[NameAttr].ParseFloat());
		Styles.Add(xmlNode["Agressive"].Attributes[NameAttr].ParseFloat());
		Styles.Add(xmlNode["Crazy"].Attributes[NameAttr].ParseFloat());
		Styles.Add(xmlNode["Fantastic"].Attributes[NameAttr].ParseFloat());
	}
}
