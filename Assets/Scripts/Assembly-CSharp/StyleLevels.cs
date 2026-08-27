using System.Collections.Generic;
using System.Xml;

public class StyleLevels
{
	public List<Style> Styles = new List<Style>();

	public float ENKMAPMCMCM;

	public float JOIJKPLCJAN;

	public float PKOFNMPOMKM;

	public Style BPDFOLFPBHO(int index)
	{
		if (Styles.Count > index)
		{
			return Styles[index];
		}
		return null;
	}

	public float GetStyleMultiplier(int index)
	{
		Style mHOJFHKHIIL = BPDFOLFPBHO(index);
		return (mHOJFHKHIIL == null) ? 0f : mHOJFHKHIIL.StyleMultiplier;
	}

	public void Parse(XmlNode node)
	{
		Styles.Clear();
		XmlAttribute xmlAttribute = node.Attributes["StylePerHit"];
		if (xmlAttribute != null)
		{
			ENKMAPMCMCM = xmlAttribute.ParseFloat();
		}
		else
		{
			LLLOJBFMONN.Error("Error: InternalSettings->StyleLevels: Attribute StylePerHit is absent!");
		}
		XmlAttribute xmlAttribute2 = node.Attributes["DecreaseSpeed"];
		if (xmlAttribute2 != null)
		{
			JOIJKPLCJAN = xmlAttribute2.ParseFloat();
		}
		else
		{
			LLLOJBFMONN.Error("Error: InternalSettings->StyleLevels: Attribute DecreaseSpeed is absent!");
		}
		XmlAttribute xmlAttribute3 = node.Attributes["Penalty"];
		if (xmlAttribute3 != null)
		{
			PKOFNMPOMKM = xmlAttribute3.ParseFloat();
		}
		else
		{
			LLLOJBFMONN.Error("Error: InternalSettings->StyleLevels: Attribute Penalty is absent!");
		}
		foreach (XmlNode childNode in node.ChildNodes)
		{
			Style mHOJFHKHIIL = new Style();
			mHOJFHKHIIL.Name = childNode.Attributes["Name"].CIPOICEEIBK(string.Empty);
			mHOJFHKHIIL.StyleMultiplier = childNode.Attributes["StyleMultiplier"].ParseFloat();
			mHOJFHKHIIL.PDJFODICKBP = childNode.Attributes["TextImage"].CIPOICEEIBK(string.Empty);
			mHOJFHKHIIL.MJGNPJMBNFK = childNode.Attributes["BarImage"].CIPOICEEIBK(string.Empty);
			Styles.Add(mHOJFHKHIIL);
		}
		if (Styles.Count == 0)
		{
			LLLOJBFMONN.Error("Error: InternalSettings->StyleLevels: Styles is absent!");
		}
	}
}
