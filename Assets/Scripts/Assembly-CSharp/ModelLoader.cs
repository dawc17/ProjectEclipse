using System.Collections.Generic;
using System.Xml;

public class ModelLoader
{
	public class CacheModelDocuments
	{
		private Dictionary<string, XmlDocument> NEECAHBNFMF = new Dictionary<string, XmlDocument>();

		public XmlDocument JBJDPDOEGFO(string EFGLOMANJHN, string PMFEIPCHENB)
		{
			XmlDocument value = null;
			if (NEECAHBNFMF.TryGetValue(PMFEIPCHENB, out value))
			{
				return value;
			}
			value = XmlUtils.OpenXMLDocument(EFGLOMANJHN, PMFEIPCHENB, XmlUtils.EBLFEPIOMOL.ForcedResourced);
			NEECAHBNFMF.Add(PMFEIPCHENB, value);
			return value;
		}

		public void NJOFIMLLJLP()
		{
			NEECAHBNFMF.Clear();
		}
	}

	private const string HAJMOBCFAGO = "ChildNode";

	private const string KGCLFDMPPMI = "LCC";

	public static CacheModelDocuments FHGHPCACAKJ = new CacheModelDocuments();

	public static void PAGDHDKNBPK()
	{
		FHGHPCACAKJ.NJOFIMLLJLP();
	}

	private static void DDPHOCNGAHE(List<ModelNode> nodes)
	{
	}

	public static void Load(ModelObject ACENLMONNPA, List<string> CBHAEPCLDFG)
	{
		if (CBHAEPCLDFG.Count == 0)
		{
			return;
		}
		XmlDocument xmlDocument = null;
		string text = "assets/models/.xml";
		foreach (string item in CBHAEPCLDFG)
		{
			if (!(item == text))
			{
				xmlDocument = FHGHPCACAKJ.JBJDPDOEGFO(SF2Paths.BNHLPKEDMOM(), item);
				if (xmlDocument != null)
				{
					Parse(ACENLMONNPA, xmlDocument);
					continue;
				}
				LLLOJBFMONN.Error("File '{0}' not found", item);
			}
		}
		ACENLMONNPA.LKFBKGPOHPI();
		ACENLMONNPA.GINBBKBGMDC();
		DDPHOCNGAHE(ACENLMONNPA.NAMKCLGOPDD());
		ACENLMONNPA.SetFileNames(CBHAEPCLDFG);
		ACENLMONNPA.MDDBGGPHNLF();
		ACENLMONNPA.KJIEPFHIIKM();
	}

	private static void Parse(ModelObject ACENLMONNPA, XmlDocument EELFNMOHGJL)
	{
		XmlNode eELFNMOHGJL = EELFNMOHGJL["Scene"];
		if (!PICNEPHDGGG(ACENLMONNPA, eELFNMOHGJL))
		{
			LLLOJBFMONN.Write("Nodes was not parsed");
		}
		if (!FOLCCFCGFPG(ACENLMONNPA, eELFNMOHGJL))
		{
			LLLOJBFMONN.Write("Edges was not parsed");
		}
		if (!LDPLPKPLNEJ(ACENLMONNPA, eELFNMOHGJL))
		{
			LLLOJBFMONN.Write("Capsules was not parsed");
		}
		if (!KJNNNOLBFPJ(ACENLMONNPA, eELFNMOHGJL))
		{
			LLLOJBFMONN.Write("Triangles was not parsed");
		}
	}

	private static bool PICNEPHDGGG(ModelObject ACENLMONNPA, XmlNode EELFNMOHGJL)
	{
		XmlNode xmlNode = EELFNMOHGJL["Nodes"];
		if (xmlNode == null)
		{
			return true;
		}
		List<global::Pair<string, float>> mFIEGKAMKNJ = new List<global::Pair<string, float>>();
		foreach (XmlNode childNode in xmlNode.ChildNodes)
		{
			GLNMJNFLLIN(ACENLMONNPA, childNode, mFIEGKAMKNJ);
		}
		ACENLMONNPA.LEOMLPGGLNA(mFIEGKAMKNJ);
		if (ACENLMONNPA.DFKIHADCFKG() == 0)
		{
			ACENLMONNPA.set_NodesCount(ACENLMONNPA.NAMKCLGOPDD().Count);
		}
		return true;
	}

	private static bool FOLCCFCGFPG(ModelObject ACENLMONNPA, XmlNode EELFNMOHGJL)
	{
		XmlNode xmlNode = EELFNMOHGJL["Edges"];
		if (xmlNode == null)
		{
			return true;
		}
		ACENLMONNPA.BKAPPJMGPKP().Capacity = ACENLMONNPA.BKAPPJMGPKP().Count + xmlNode.ChildNodes.Count;
		foreach (XmlNode childNode in xmlNode.ChildNodes)
		{
			int num = childNode.Attributes["Iterations"].ParseInt(1);
			string name = childNode.Name;
			for (int i = 0; i < num; i++)
			{
				string empty = string.Empty;
				empty += name;
				if (i > 0)
				{
					empty = empty + "CI" + i;
				}
				NLINKGAAKJD(ACENLMONNPA, childNode, empty);
			}
		}
		return true;
	}

	private static bool LDPLPKPLNEJ(ModelObject ACENLMONNPA, XmlNode EELFNMOHGJL)
	{
		XmlNode xmlNode = EELFNMOHGJL["Figures"];
		foreach (XmlNode childNode in xmlNode.ChildNodes)
		{
			string value = childNode.Attributes["Type"].Value;
			if (value == "Capsule")
			{
				IJAMECONFML(ACENLMONNPA, childNode);
			}
		}
		return true;
	}

	private static bool KJNNNOLBFPJ(ModelObject ACENLMONNPA, XmlNode EELFNMOHGJL)
	{
		XmlNode xmlNode = EELFNMOHGJL["Figures"];
		foreach (XmlNode childNode in xmlNode.ChildNodes)
		{
			string value = childNode.Attributes["Type"].Value;
			if (value == "Triangle")
			{
				DOMOFJMFLGK(ACENLMONNPA, childNode);
			}
		}
		return true;
	}

	private static void GLNMJNFLLIN(ModelObject ACENLMONNPA, XmlNode node, List<global::Pair<string, float>> MFIEGKAMKNJ)
	{
		ModelNode lCDGOCIAIDK = null;
		Vector3f eMAFACPEPDK = new Vector3f(node.Attributes["X"].ParseFloat(), 0f - node.Attributes["Y"].ParseFloat(), node.Attributes["Z"].ParseFloat());
		string value = node.Attributes["Type"].Value;
		string name = node.Name;
		bool flag = value == "CenterOfMass";
		if (value == "Node" || flag)
		{
			lCDGOCIAIDK = new ModelNode(name, eMAFACPEPDK);
			lCDGOCIAIDK.CNNKFMNKDNE(node.Attributes["Cloth"].ParseBool());
			lCDGOCIAIDK.BDFIDDLGDNM(node.Attributes["Attenuation"].ParseFloat());
			if (flag)
			{
				MFIEGKAMKNJ.Clear();
				DPMFEKBBPIL(MFIEGKAMKNJ, node, false);
			}
			ACENLMONNPA.LMBNDIPLBJA().Add(lCDGOCIAIDK);
		}
		else if (value == "MacroNode")
		{
			eMAFACPEPDK.JPFALPBDBAP(eMAFACPEPDK.GILCBJJPKBK() * -1f);
			ModelMacroNode gDNAJOODAGP = new ModelMacroNode(name, eMAFACPEPDK);
			lCDGOCIAIDK = gDNAJOODAGP;
			EADLCHAFKDC(gDNAJOODAGP, node);
			ACENLMONNPA.BLFJJAEFKKP().Add(gDNAJOODAGP);
		}
		if (lCDGOCIAIDK != null)
		{
			lCDGOCIAIDK.set_ID(ACENLMONNPA.NAMKCLGOPDD().Count);
			lCDGOCIAIDK.NPKACGCHOLK(node.Attributes["Mass"].ParseFloat());
			lCDGOCIAIDK.MGPLABIFCAH(node.Attributes["Fixed"].ParseBool());
			lCDGOCIAIDK.NNHPOJFKEID(node.Attributes["Visible"].ParseBool());
			lCDGOCIAIDK.set_IsShock(node.Attributes["Shock"].ParseBool());
			lCDGOCIAIDK.KMBHEMMJACN(node.Attributes["Collisible"].ParseBool());
			lCDGOCIAIDK.LBLPDPJGPHL(node.Attributes["Weak"].ParseBool());
			ACENLMONNPA.NAMKCLGOPDD().Add(lCDGOCIAIDK);
			ACENLMONNPA.HKCFFKKFFFE().Add(lCDGOCIAIDK.get_Name(), lCDGOCIAIDK);
		}
	}

	private static void NLINKGAAKJD(ModelObject ACENLMONNPA, XmlNode node, string IMGCANJHPND)
	{
		ModelEdge nAKBKCDKEHF = null;
		ModelNode iLENLCMAMBH = ACENLMONNPA.KLAPIGGACMM(node.Attributes["End1"].Value);
		ModelNode bFDAHEHCAGK = ACENLMONNPA.KLAPIGGACMM(node.Attributes["End2"].Value);
		float bAINMLLIKOL = node.Attributes["Length"].ParseFloat();
		float bAINMLLIKOL2 = node.Attributes["Radius"].ParseFloat();
		float bAINMLLIKOL3 = node.Attributes["Margin1"].ParseFloat();
		float bAINMLLIKOL4 = node.Attributes["Margin2"].ParseFloat();
		string text = node.Attributes["Type"].CIPOICEEIBK(string.Empty);
		string text2 = node.Attributes["SubType"].CIPOICEEIBK(string.Empty);
		string bAINMLLIKOL5 = node.Attributes["BodyPart"].CIPOICEEIBK(string.Empty);
		string bAINMLLIKOL6 = node.Attributes["Defense"].CIPOICEEIBK(string.Empty);
		int num = node.Attributes["Collisible"].ParseInt();
		bool bAINMLLIKOL7 = node.Attributes["Blood"].ParseBool();
		bool bAINMLLIKOL8 = node.Attributes["Shock"].ParseBool();
		nAKBKCDKEHF = new ModelEdge(iLENLCMAMBH, bFDAHEHCAGK);
		nAKBKCDKEHF.set_Length(bAINMLLIKOL);
		nAKBKCDKEHF.set_Name(IMGCANJHPND);
		nAKBKCDKEHF.set_Collisible(num);
		nAKBKCDKEHF.MCDGHEAJGGP(bAINMLLIKOL5);
		nAKBKCDKEHF.CFFCAJLFBEM(bAINMLLIKOL6);
		nAKBKCDKEHF.DIIBABHCHFP(bAINMLLIKOL7);
		nAKBKCDKEHF.set_IsShock(bAINMLLIKOL8);
		nAKBKCDKEHF.OIEJCNEODGC(bAINMLLIKOL2);
		nAKBKCDKEHF.LADPGJPABHO(bAINMLLIKOL3);
		nAKBKCDKEHF.EJIOOIMBAEA(bAINMLLIKOL4);
		if (text == "Edge")
		{
			nAKBKCDKEHF.set_Type(EdgeType.Edge);
			ACENLMONNPA.HABIIJGLCMA().Add(nAKBKCDKEHF);
			if (num > 0)
			{
				ACENLMONNPA.ODDEMLAODPM().Add(nAKBKCDKEHF);
			}
		}
		else
		{
			if (!(text == "Muscle"))
			{
				LLLOJBFMONN.Error("Wring type edge: {0}", text);
				return;
			}
			nAKBKCDKEHF.set_Type(EdgeType.Muscle);
			ACENLMONNPA.EKOGCJAAKDN().Add(nAKBKCDKEHF);
		}
		if (text2 == "None")
		{
			nAKBKCDKEHF.JIDPIOJGNBP(EdgeSubType.None);
		}
		else if (text2 == "Blade")
		{
			nAKBKCDKEHF.JIDPIOJGNBP(EdgeSubType.Blade);
		}
		ACENLMONNPA.BKAPPJMGPKP().Add(nAKBKCDKEHF);
	}

	private static void IJAMECONFML(ModelObject ACENLMONNPA, XmlNode node)
	{
		string value = node.Attributes["Edge"].Value;
		ModelEdge nAKBKCDKEHF = ACENLMONNPA.CLBHEMEAAEN(value);
		if (nAKBKCDKEHF != null)
		{
			Capsule cOGLBFKLNFC = new Capsule(nAKBKCDKEHF);
			cOGLBFKLNFC.set_Name(node.Name);
			cOGLBFKLNFC.CNEEGAJGBEI(node.Attributes["Radius1"].ParseFloat());
			cOGLBFKLNFC.BLHHLPDEAKF(node.Attributes["Radius2"].ParseFloat());
			cOGLBFKLNFC.GKBFHLAHCFG(node.Attributes["Margin1"].ParseFloat());
			cOGLBFKLNFC.HCCIGEIFEOF(node.Attributes["Margin2"].ParseFloat());
			cOGLBFKLNFC.IJIGFKFDKGM(node.Attributes["Radius1"].ParseFloat() * 2f);
			cOGLBFKLNFC.CreateUI(ACENLMONNPA.get_Model().MJNPBMOAFML().transform).Render();
			ACENLMONNPA.DPIFMDIKDBC().Add(cOGLBFKLNFC);
		}
	}

	private static void DOMOFJMFLGK(ModelObject ACENLMONNPA, XmlNode node)
	{
		string value = node.Attributes["Node1"].Value;
		ModelNode lCDGOCIAIDK = ACENLMONNPA.EGHIDHMENEF(value);
		if (lCDGOCIAIDK == null)
		{
			return;
		}
		value = node.Attributes["Node2"].Value;
		ModelNode lCDGOCIAIDK2 = ACENLMONNPA.EGHIDHMENEF(value);
		if (lCDGOCIAIDK2 != null)
		{
			value = node.Attributes["Node3"].Value;
			ModelNode lCDGOCIAIDK3 = ACENLMONNPA.EGHIDHMENEF(value);
			if (lCDGOCIAIDK3 != null)
			{
				Triangle item = new Triangle(lCDGOCIAIDK, lCDGOCIAIDK2, lCDGOCIAIDK3, node.Name);
				ACENLMONNPA.ELOGKMHEBGA().Add(item);
				ACENLMONNPA.get_Model()._MeshRender.get_Base().LPEPFNNPCBK(lCDGOCIAIDK, lCDGOCIAIDK2, lCDGOCIAIDK3);
			}
		}
	}

	private static void EADLCHAFKDC(ModelMacroNode AHJOLBKABMC, XmlNode node)
	{
		if (AHJOLBKABMC.get_Type() == ModelNode.KOJNBGALAHM.MacroNode)
		{
			DPMFEKBBPIL(AHJOLBKABMC.LMPPCKACMNB, node, true);
		}
	}

	private static void DPMFEKBBPIL(List<global::Pair<string, float>> NBAGKJAPCFD, XmlNode node, bool OGFKPCPEDAK)
	{
		int num = node.Attributes["NodesCount"].ParseInt();
		if (0 >= num)
		{
			return;
		}
		string empty = string.Empty;
		string empty2 = string.Empty;
		NBAGKJAPCFD.Capacity = num;
		for (int i = 0; i < num; i++)
		{
			empty2 = (i + 1).ToString();
			empty = "ChildNode";
			empty += empty2;
			string gBCLEDJAOBM = node.Attributes[empty].CIPOICEEIBK(string.Empty);
			float pOFHDGJAFMP = 0f;
			if (OGFKPCPEDAK)
			{
				empty = "LCC";
				empty += empty2;
				pOFHDGJAFMP = node.Attributes[empty].ParseFloat();
			}
			NBAGKJAPCFD.Add(new global::Pair<string, float>(gBCLEDJAOBM, pOFHDGJAFMP));
		}
	}

	private static void DNDNHBKDPHI(ModelObject ACENLMONNPA)
	{
		List<ModelNode> list = ACENLMONNPA.NAMKCLGOPDD();
		List<global::Pair<int, int>> list2 = ACENLMONNPA.DJNNIKHGGFO();
		foreach (global::Pair<int, int> item in list2)
		{
			ModelNode lCDGOCIAIDK = list[item.First];
			ModelNode lCDGOCIAIDK2 = list[item.Second];
			lCDGOCIAIDK.set_PairNode(lCDGOCIAIDK2);
			lCDGOCIAIDK2.set_PairNode(lCDGOCIAIDK);
		}
	}
}
