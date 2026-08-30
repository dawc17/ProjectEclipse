using System.Collections.Generic;
using System.Xml;

public static class MovesCompiler
{
	public static void Run(string PMFEIPCHENB, XmlDocument AMOPOGFKMCG, bool OOJAEKEOEFJ)
	{
		XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(PMFEIPCHENB, string.Empty);
		if (xmlDocument == null)
		{
			return;
		}
		List<global::Pair<string, XmlNode>> jIGEFEPNCIN = new List<global::Pair<string, XmlNode>>();
		List<string> kDJLIKCGHIG = new List<string>();
		XmlNode xmlNode = xmlDocument["Movesxml"]["Templates"];
		XmlNode nBMGOEMJJAF = xmlDocument["Movesxml"]["Moves"];
		XmlNode bBNKIBKPBLO = xmlDocument["Movesxml"]["Triggers"];
		XmlNode xmlNode2 = AMOPOGFKMCG.ACBPMPMPKJJ("Movesxml").LCOLFMJJDJE(nBMGOEMJJAF);
		foreach (XmlNode childNode in xmlNode2.ChildNodes)
		{
			GGGLIDNJNCJ(childNode, xmlNode, jIGEFEPNCIN, kDJLIKCGHIG);
		}
		XmlNode oEMALIFPGPO = AMOPOGFKMCG["Movesxml"].ACBPMPMPKJJ("Templates");
		FOOEPLIMKCB(oEMALIFPGPO, xmlNode);
		XmlNode oEMALIFPGPO2 = AMOPOGFKMCG["Movesxml"].ACBPMPMPKJJ("Triggers");
		CGHCNIJGLEC(oEMALIFPGPO2, bBNKIBKPBLO);
		AddIDs(AMOPOGFKMCG);
		if (OOJAEKEOEFJ)
		{
			AMOPOGFKMCG.Save(PMFEIPCHENB.Replace("moves.xml", "moves_tmp.xml"));
		}
	}

	private static void GGGLIDNJNCJ(XmlNode HBHJHDBOCBC, XmlNode NBODGGMDOIC, List<global::Pair<string, XmlNode>> JIGEFEPNCIN, List<string> KDJLIKCGHIG)
	{
		JIGEFEPNCIN.Clear();
		NEONAMKOFPN(HBHJHDBOCBC, NBODGGMDOIC, JIGEFEPNCIN, KDJLIKCGHIG);
		for (int i = 0; i < JIGEFEPNCIN.Count; i++)
		{
			NEONAMKOFPN(JIGEFEPNCIN[i].Second, NBODGGMDOIC, JIGEFEPNCIN, KDJLIKCGHIG);
		}
		ABPICBNBIJA(JIGEFEPNCIN);
		if (JIGEFEPNCIN.Count == 0)
		{
			return;
		}
		string text = string.Empty;
		foreach (global::Pair<string, XmlNode> item in JIGEFEPNCIN)
		{
			text = text + item.First + "|";
			GFFBIJIEIEM(HBHJHDBOCBC, item.Second);
		}
		text = text.Substring(0, text.Length - 1);
		HBHJHDBOCBC.Attributes["Template"].Value = text;
	}

	private static void NEONAMKOFPN(XmlNode HBHJHDBOCBC, XmlNode NBODGGMDOIC, List<global::Pair<string, XmlNode>> JIGEFEPNCIN, List<string> KDJLIKCGHIG)
	{
		KDJLIKCGHIG.Clear();
		ReadTemplatesName(HBHJHDBOCBC, KDJLIKCGHIG);
		foreach (string item in KDJLIKCGHIG)
		{
			foreach (XmlNode childNode in NBODGGMDOIC.ChildNodes)
			{
				string name = childNode.Name;
				string text = childNode.Attributes["Name"].CIPOICEEIBK(string.Empty);
				if ("Template" == name && item == text)
				{
					JIGEFEPNCIN.Add(new global::Pair<string, XmlNode>(item, childNode));
					break;
				}
			}
		}
	}

	private static void ABPICBNBIJA(List<global::Pair<string, XmlNode>> JIGEFEPNCIN)
	{
		for (int i = 0; i < JIGEFEPNCIN.Count; i++)
		{
			for (int j = i + 1; j < JIGEFEPNCIN.Count; j++)
			{
				if (JIGEFEPNCIN[i].First == JIGEFEPNCIN[j].First && JIGEFEPNCIN[i].Second == JIGEFEPNCIN[j].Second)
				{
					JIGEFEPNCIN.RemoveAt(j);
					j--;
				}
			}
		}
	}

	private static void GFFBIJIEIEM(XmlNode HBHJHDBOCBC, XmlNode LFDMCKCLNKG)
	{
		AddAttributes(HBHJHDBOCBC, LFDMCKCLNKG);
		AELEJBHOBOL(HBHJHDBOCBC, LFDMCKCLNKG, "Events");
		AELEJBHOBOL(HBHJHDBOCBC, LFDMCKCLNKG, "Conditions");
		AELEJBHOBOL(HBHJHDBOCBC, LFDMCKCLNKG, "Locks");
		AELEJBHOBOL(HBHJHDBOCBC, LFDMCKCLNKG, "Intervals");
		AELEJBHOBOL(HBHJHDBOCBC, LFDMCKCLNKG, "Actions");
		OPNMLCICLBE(HBHJHDBOCBC, LFDMCKCLNKG, "SetDirection");
		OPNMLCICLBE(HBHJHDBOCBC, LFDMCKCLNKG, "Align");
		OPNMLCICLBE(HBHJHDBOCBC, LFDMCKCLNKG, "Velocity");
		OPNMLCICLBE(HBHJHDBOCBC, LFDMCKCLNKG, "Tactics");
		AELEJBHOBOL(HBHJHDBOCBC["Tactics"], LFDMCKCLNKG["Tactics"], "Conditions");
	}

	private static void AddAttributes(XmlNode HBHJHDBOCBC, XmlNode LFDMCKCLNKG)
	{
		foreach (XmlAttribute attribute in LFDMCKCLNKG.Attributes)
		{
			string name = attribute.Name;
			XmlAttribute xmlAttribute2 = HBHJHDBOCBC.Attributes[name];
			if (xmlAttribute2 == null)
			{
				HBHJHDBOCBC.LCOLFMJJDJE(attribute);
			}
		}
	}

	private static void OPNMLCICLBE(XmlNode HBHJHDBOCBC, XmlNode LFDMCKCLNKG, string name)
	{
		XmlNode xmlNode = HBHJHDBOCBC[name];
		if (xmlNode == null)
		{
			xmlNode = LFDMCKCLNKG[name];
			if (xmlNode != null)
			{
				HBHJHDBOCBC.LCOLFMJJDJE(xmlNode);
			}
		}
	}

	private static void AELEJBHOBOL(XmlNode HBHJHDBOCBC, XmlNode LFDMCKCLNKG, string name)
	{
		if (LFDMCKCLNKG == null)
		{
			return;
		}
		XmlNode xmlNode = LFDMCKCLNKG[name];
		XmlNode xmlNode2 = HBHJHDBOCBC[name];
		bool flag = xmlNode2 != null && xmlNode2.Attributes["RewriteTemplates"].ParseBool();
		if (xmlNode == null || flag)
		{
			return;
		}
		XmlNode xmlNode3 = HBHJHDBOCBC[name];
		if (xmlNode3 == null)
		{
			HBHJHDBOCBC.LCOLFMJJDJE(xmlNode);
			return;
		}
		foreach (XmlNode childNode in xmlNode.ChildNodes)
		{
			xmlNode3.LCOLFMJJDJE(childNode);
		}
	}

	private static int ReadTemplatesName(XmlNode node, List<string> OEMALIFPGPO)
	{
		string text = node.Attributes["Template"].CIPOICEEIBK(string.Empty);
		string[] array = text.Split('|');
		OEMALIFPGPO.AddRange(array);
		return array.Length;
	}

	private static void FOOEPLIMKCB(XmlNode OEMALIFPGPO, XmlNode BBNKIBKPBLO)
	{
		foreach (XmlNode childNode in BBNKIBKPBLO.ChildNodes)
		{
			XmlElement xmlElement = OEMALIFPGPO.ACBPMPMPKJJ("Template");
			xmlElement.SetAttribute("Name", childNode.Attributes["Name"].CIPOICEEIBK(string.Empty));
		}
	}

	private static void CGHCNIJGLEC(XmlNode OEMALIFPGPO, XmlNode BBNKIBKPBLO)
	{
		foreach (XmlNode childNode in BBNKIBKPBLO.ChildNodes)
		{
			OEMALIFPGPO.LCOLFMJJDJE(childNode);
		}
	}

	private static void AddIDs(XmlDocument EELFNMOHGJL)
	{
		XmlNode xmlNode = EELFNMOHGJL["Movesxml"]["Moves"];
		int num = 0;
		foreach (XmlNode childNode in xmlNode.ChildNodes)
		{
			XmlNode xmlNode3 = childNode["Intervals"];
			if (xmlNode3 == null)
			{
				continue;
			}
			foreach (XmlNode childNode2 in xmlNode3.ChildNodes)
			{
				if (childNode2.Attributes["Type"].CIPOICEEIBK(string.Empty) == "Attack")
				{
					((XmlElement)childNode2).SetAttribute("ID", num.ToString());
					num++;
				}
			}
		}
	}
}
