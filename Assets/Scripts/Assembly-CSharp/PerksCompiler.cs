using System.Collections.Generic;
using System.Xml;

public static class PerksCompiler
{
	public static void CompilePerks(ref XmlDocument EELFNMOHGJL, string PMFEIPCHENB)
	{
		EELFNMOHGJL = XmlUtils.OpenXMLDocument(PMFEIPCHENB, string.Empty, XmlUtils.EBLFEPIOMOL.ForcedResourced);
		if (EELFNMOHGJL != null)
		{
			XmlNode xmlNode = EELFNMOHGJL["Perks"];
			if (xmlNode != null)
			{
				foreach (XmlNode childNode in xmlNode.ChildNodes)
				{
					if (!childNode.Name.Equals("Perk"))
					{
						continue;
					}
					List<string> list = new List<string>();
					MDPJALEPMFI(childNode, xmlNode, list);
					XmlAttribute xmlAttribute = childNode.Attributes["Template"];
					if (xmlAttribute != null && list.Count > 0)
					{
						string text = string.Empty;
						foreach (string item in list)
						{
							text = string.Format("{0}{1}|", text, item);
						}
						if (!string.IsNullOrEmpty(text))
						{
							text = text.Remove(text.Length - 1);
						}
						xmlAttribute.Value = text;
					}
					list.Clear();
				}
			}
		}
		AddIDs(EELFNMOHGJL);
		if (SystemProperties.DBBOCENKMGD())
		{
			EELFNMOHGJL.Save(string.Format("{0}/{1}", SF2Paths.GBOFOFGDMBN(), "perks_result.xml"));
		}
	}

	private static XmlNode GetTemplateNode(XmlNode AFHNINCKJEE, string PAGGPPPLPGC)
	{
		foreach (XmlNode childNode in AFHNINCKJEE.ChildNodes)
		{
			if (childNode.Name.Equals("Perk"))
			{
				string value = childNode.Attributes["Name"].CIPOICEEIBK();
				if (PAGGPPPLPGC.Equals(value))
				{
					return childNode;
				}
			}
		}
		LLLOJBFMONN.Error("Perks: tactics template '{0}' not found", PAGGPPPLPGC);
		return null;
	}

	private static void KDMOFABEMLN(XmlNode OEMALIFPGPO, XmlNode BBNKIBKPBLO)
	{
		foreach (XmlAttribute attribute in BBNKIBKPBLO.Attributes)
		{
			string name = attribute.Name;
			XmlAttribute xmlAttribute2 = OEMALIFPGPO.Attributes[name];
			if (xmlAttribute2 == null)
			{
				OEMALIFPGPO.LCOLFMJJDJE(attribute);
			}
		}
	}

	private static void OKLPIDNNLJK(XmlNode OEMALIFPGPO, XmlNode BBNKIBKPBLO)
	{
		foreach (XmlNode childNode in BBNKIBKPBLO.ChildNodes)
		{
			string name = childNode.Name;
			XmlNode xmlNode2 = OEMALIFPGPO["Set"];
			if (name == "Trigger" || (name == "Set" && xmlNode2 == null))
			{
				OEMALIFPGPO.LCOLFMJJDJE(childNode);
			}
			else if (name == "Set" && xmlNode2 != null)
			{
				KDMOFABEMLN(xmlNode2, childNode);
			}
		}
	}

	private static void MDPJALEPMFI(XmlNode OEMALIFPGPO, XmlNode BBNKIBKPBLO, List<string> KLFLOKHIPLN)
	{
		List<string> list = new List<string>();
		XmlAttribute xmlAttribute = OEMALIFPGPO.Attributes["Template"];
		if (xmlAttribute == null)
		{
			return;
		}
		string text = xmlAttribute.CIPOICEEIBK();
		list.AddRange(text.Split('|'));
		list = list.KJCJIHJOLFC();
		foreach (string item in list)
		{
			XmlNode xmlNode = GetTemplateNode(BBNKIBKPBLO, item);
			if (xmlNode != null)
			{
				XmlDocument mEEAKLDGLDF = new XmlDocument();
				XmlNode xmlNode2 = mEEAKLDGLDF.LCOLFMJJDJE(xmlNode);
				MDPJALEPMFI(xmlNode2, BBNKIBKPBLO, KLFLOKHIPLN);
				KDMOFABEMLN(OEMALIFPGPO, xmlNode2);
				OKLPIDNNLJK(OEMALIFPGPO, xmlNode2);
			}
			KLFLOKHIPLN.Add(item);
		}
	}

	private static void AddIDs(XmlDocument EELFNMOHGJL)
	{
		XmlNode xmlNode = EELFNMOHGJL["Perks"];
		int num = 0;
		if (xmlNode == null)
		{
			return;
		}
		foreach (XmlNode childNode in xmlNode.ChildNodes)
		{
			if (childNode.Name.Equals("Perk"))
			{
				childNode.IHNEFFHCDDJ("ID").Value = num.ToString();
				num++;
			}
		}
	}
}
