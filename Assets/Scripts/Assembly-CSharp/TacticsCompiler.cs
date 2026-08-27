using System.Collections.Generic;
using System.Xml;

public class TacticsCompiler
{
	public static void CompileTacticsSettings(XmlDocument EELFNMOHGJL)
	{
		XmlNode xmlNode = EELFNMOHGJL["TacticsSettings"]["Tactics"];
		if (xmlNode == null)
		{
			return;
		}
		foreach (XmlNode childNode in xmlNode.ChildNodes)
		{
			if (!(childNode.Name == "Tactic"))
			{
				continue;
			}
			XmlAttribute xmlAttribute = childNode.Attributes["Template"];
			if (xmlAttribute == null)
			{
				continue;
			}
			string text = xmlAttribute.CIPOICEEIBK(string.Empty);
			string[] array = text.Split('|');
			List<string> list = new List<string>();
			list.AddRange(array);
			int num = array.Length;
			RemoveDuplicateTeplates(list);
			if (0 < num)
			{
				int iHPMGHJPLBP = 0;
				JKJLFOAOLFI(xmlNode, childNode, list, iHPMGHJPLBP);
			}
			if (list.Count == 0)
			{
				continue;
			}
			string text2 = string.Empty;
			foreach (string item in list)
			{
				text2 = text2 + item + "|";
			}
			if (text2 != string.Empty)
			{
				text2 = text2.Remove(text2.Length - 1);
			}
			xmlAttribute.Value = text2;
		}
	}

	private static void JKJLFOAOLFI(XmlNode AFHNINCKJEE, XmlNode OEMALIFPGPO, List<string> JIGEFEPNCIN, int index)
	{
		if (JIGEFEPNCIN.Count <= index)
		{
			return;
		}
		string pAGGPPPLPGC = JIGEFEPNCIN[index];
		XmlNode xmlNode = GetTemplateNode(AFHNINCKJEE, pAGGPPPLPGC);
		if (xmlNode != null)
		{
			KDMOFABEMLN(OEMALIFPGPO, xmlNode);
			OKLPIDNNLJK(OEMALIFPGPO, xmlNode);
			XmlAttribute xmlAttribute = xmlNode.Attributes["Template"];
			if (xmlAttribute != null)
			{
				string text = xmlAttribute.CIPOICEEIBK(string.Empty);
				List<string> list = new List<string>();
				string[] collection = text.Split('|');
				list.AddRange(collection);
				JIGEFEPNCIN.AddIfNotExist(list);
			}
		}
		JKJLFOAOLFI(AFHNINCKJEE, OEMALIFPGPO, JIGEFEPNCIN, index + 1);
	}

	private static XmlNode GetTemplateNode(XmlNode AFHNINCKJEE, string PAGGPPPLPGC)
	{
		foreach (XmlNode childNode in AFHNINCKJEE.ChildNodes)
		{
			if (childNode.Name == "Tactic")
			{
				string text = childNode.Attributes["Name"].CIPOICEEIBK(string.Empty);
				if (PAGGPPPLPGC == text)
				{
					return childNode;
				}
			}
		}
		LLLOJBFMONN.Error("TacticsSettings: tactics template " + PAGGPPPLPGC + " not found");
		return null;
	}

	public static void KDMOFABEMLN(XmlNode OEMALIFPGPO, XmlNode BBNKIBKPBLO, bool HPKBCMPEBJF = false)
	{
		foreach (XmlAttribute attribute in BBNKIBKPBLO.Attributes)
		{
			string name = attribute.Name;
			XmlAttribute xmlAttribute2 = OEMALIFPGPO.Attributes[name];
			if (xmlAttribute2 == null)
			{
				OEMALIFPGPO.LCOLFMJJDJE(attribute);
			}
			else if (HPKBCMPEBJF)
			{
				xmlAttribute2.Value = attribute.Value;
			}
		}
	}

	public static void OKLPIDNNLJK(XmlNode OEMALIFPGPO, XmlNode BBNKIBKPBLO)
	{
		foreach (XmlNode childNode in BBNKIBKPBLO.ChildNodes)
		{
			string name = childNode.Name;
			XmlNode xmlNode2 = OEMALIFPGPO[name];
			if (xmlNode2 == null)
			{
				OEMALIFPGPO.LCOLFMJJDJE(childNode);
			}
		}
	}

	public static void AEGPGGKJBLG(XmlNode OEMALIFPGPO, XmlNode BBNKIBKPBLO)
	{
		XmlNode mEEAKLDGLDF = OEMALIFPGPO["TacticsSettings"]["Tactics"];
		XmlNode xmlNode = BBNKIBKPBLO["TacticsSettings"]["Tactics"];
		foreach (XmlNode childNode in xmlNode.ChildNodes)
		{
			string name = childNode.Name;
			if (name != "Tactic")
			{
				continue;
			}
			string text = childNode.Attributes["Name"].CIPOICEEIBK(string.Empty);
			bool flag = false;
			foreach (XmlNode childNode2 in OEMALIFPGPO.ChildNodes)
			{
				if (!(name != "Tactic"))
				{
					string text2 = childNode2.Attributes["Name"].CIPOICEEIBK(string.Empty);
					if (text == text2)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				mEEAKLDGLDF.LCOLFMJJDJE(childNode);
			}
		}
	}

	private static void RemoveDuplicateTeplates(List<string> JIGEFEPNCIN)
	{
		for (int i = 0; i < JIGEFEPNCIN.Count; i++)
		{
			for (int j = i + 1; j < JIGEFEPNCIN.Count; j++)
			{
				if (JIGEFEPNCIN[i] == JIGEFEPNCIN[j])
				{
					JIGEFEPNCIN.RemoveAt(j);
					j--;
				}
			}
		}
	}
}
