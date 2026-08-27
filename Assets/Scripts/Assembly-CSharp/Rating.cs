using System;
using System.Xml;

public class Rating
{
	public string IHJJBIDMEMB;

	public string KFMJMBANIGF;

	public string GBOKABKLCFM;

	public float Multiplier;

	public string BIOIOGIBCOE;

	public Rating()
	{
		IHJJBIDMEMB = string.Empty;
		KFMJMBANIGF = string.Empty;
		GBOKABKLCFM = string.Empty;
		Multiplier = 0f;
		BIOIOGIBCOE = string.Empty;
	}

	public Rating(Rating NOLFMPDGCOC)
	{
		IHJJBIDMEMB = NOLFMPDGCOC.IHJJBIDMEMB;
		KFMJMBANIGF = NOLFMPDGCOC.KFMJMBANIGF;
		GBOKABKLCFM = NOLFMPDGCOC.GBOKABKLCFM;
		Multiplier = NOLFMPDGCOC.Multiplier;
		BIOIOGIBCOE = NOLFMPDGCOC.BIOIOGIBCOE;
	}

	public void Parse(XmlNode node, PerkSetAttributes CJILONFAJIK = null)
	{
		if (CJILONFAJIK != null)
		{
			foreach (XmlAttribute attribute in node.Attributes)
			{
				string text = XmlUtils.ParseString(attribute, string.Empty);
				if (text[0] == '_')
				{
					string gOHIIMFFFJI = text.Substring(1, text.Length - 1);
					string value = CJILONFAJIK.GetValue(gOHIIMFFFJI);
					attribute.Value = value;
				}
			}
		}
		IHJJBIDMEMB = XmlUtils.ParseString(node.Attributes["Player"], "Me");
		KFMJMBANIGF = XmlUtils.ParseString(node.Attributes["Damage"]);
		GBOKABKLCFM = XmlUtils.ParseString(node.Attributes["Defense"]);
		try
		{
			Multiplier = XmlUtils.ParseFloat(node.Attributes["Multiplier"]);
		}
		catch (Exception)
		{
			int num = 0;
		}
		BIOIOGIBCOE = XmlUtils.ParseString(node.Attributes["EnemyAttribute"]);
	}
}
