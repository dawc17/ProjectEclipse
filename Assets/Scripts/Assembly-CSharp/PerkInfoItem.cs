using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using UnityEngine;

public class PerkInfoItem
{
	public enum DNPGIEGCGKH
	{
		COMBO = 0,
		SINGLE = 1
	}

	public enum JFOFGHPCIBE
	{
		RT_FLOOR = 0,
		RT_CEIL = 1,
		RT_TRUNC = 2,
		RT_INF = 3
	}

	private bool PGECFFBLKJL;

	private bool OPAHEMMNCOK;

	private XmlDocument defaultDocument = new XmlDocument();

	private XmlNode defaultNode;

	public string NHKMCLPOMFK;

	public string Name;

	public string MGNNJPBCOGD;

	public string HBCNKNFPAIM;

	public string HCCKLLOEPJN;

	public string FEECJKOAKBE;

	public string JNBECGKCNBB;

	public string DIJBDEJFKKF;

	public int AKKLOMFOLNO;

	public int Id;

	public int Level;

	public int PDBNJHEBECL;

	public int EAIDMBHDPPO;

	public bool GDCBBAHKCIE;

	public bool BGFEPJKDHFB;

	public DNPGIEGCGKH LELHEEDNMBP;

	private PerkSetAttributes MJGGAHJDPMN;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Model BGNHAFPAPGL;

	public Attributes IBLHIAHECLK;

	public List<Rating> MLMLENHGNDJ;

	private List<string> Names;

	private List<string> IONLOHAKLFF;

	private List<PerkTrigger> NMILPLHGCMA = new List<PerkTrigger>();

	private static Dictionary<string, JFOFGHPCIBE> CCNDJMNINEM = new Dictionary<string, JFOFGHPCIBE>
	{
		{
			"floor",
			JFOFGHPCIBE.RT_FLOOR
		},
		{
			"ceil",
			JFOFGHPCIBE.RT_CEIL
		},
		{
			"trunc",
			JFOFGHPCIBE.RT_TRUNC
		},
		{
			"inf",
			JFOFGHPCIBE.RT_INF
		}
	};

	public bool GDCAEDIIIBG
	{
		get
		{
			return DLEAKGFKDBH();
		}
		set
		{
			HILDOOOKHGN(value);
		}
	}

	public bool DCHJDPCEODD
	{
		get
		{
			return OPIAGHNCFAM();
		}
		set
		{
			DLDANNALFEA(value);
		}
	}

	public XmlNode HAAKMBKCMCO
	{
		get
		{
			return FHCNBDOKIML();
		}
	}

	public PerkSetAttributes LJOGKGCEKAN
	{
		get
		{
			return EPBADFHIJAH();
		}
	}

	public Model IHGLMAHLBPJ
	{
		get
		{
			return ELPJBGIPEIB();
		}
		set
		{
			LPHBKEKMPEH(value);
		}
	}

	public List<PerkTrigger> GIFPBBKCKIK
	{
		get
		{
			return NOJEIGNOPII();
		}
	}

	public PerkInfoItem()
	{
		PGECFFBLKJL = false;
		OPAHEMMNCOK = false;
		Name = string.Empty;
		MGNNJPBCOGD = string.Empty;
		HBCNKNFPAIM = string.Empty;
		HCCKLLOEPJN = string.Empty;
		FEECJKOAKBE = string.Empty;
		JNBECGKCNBB = string.Empty;
		DIJBDEJFKKF = string.Empty;
		Id = -1;
		Level = 0;
		PDBNJHEBECL = 0;
		EAIDMBHDPPO = 0;
		AKKLOMFOLNO = 0;
		GDCBBAHKCIE = false;
		BGFEPJKDHFB = false;
		LELHEEDNMBP = DNPGIEGCGKH.SINGLE;
		MJGGAHJDPMN = new PerkSetAttributes();
		IBLHIAHECLK = new Attributes();
		MLMLENHGNDJ = new List<Rating>();
		Names = new List<string>();
		IONLOHAKLFF = new List<string>();
	}

	public bool DLEAKGFKDBH()
	{
		return PGECFFBLKJL;
	}

	public void HILDOOOKHGN(bool value)
	{
		PGECFFBLKJL = value;
	}

	public bool OPIAGHNCFAM()
	{
		return OPAHEMMNCOK;
	}

	public void DLDANNALFEA(bool value)
	{
		OPAHEMMNCOK = value;
	}

	public XmlNode FHCNBDOKIML()
	{
		return defaultNode;
	}

	public PerkSetAttributes EPBADFHIJAH()
	{
		return MJGGAHJDPMN;
	}

	public Model ELPJBGIPEIB()
	{
		return BGNHAFPAPGL;
	}

	public void LPHBKEKMPEH(Model value)
	{
		BGNHAFPAPGL = value;
	}

	public List<PerkTrigger> NOJEIGNOPII()
	{
		return NMILPLHGCMA;
	}

	public bool IsPerkByNames(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return true;
		}
		for (int i = 0; i < Names.Count; i++)
		{
			if (Names[i] == value)
			{
				return true;
			}
		}
		return false;
	}

	public static float DODDEPEMBMC(float FIJMPFHAKPB)
	{
		float lFIPMCAHODJ = GameUtils.JDHBJMHAJOG().LFIPMCAHODJ;
		float jHJAFHLMOBJ = GameUtils.JDHBJMHAJOG().JHJAFHLMOBJ;
		float gPEPDPOJJLM = GameUtils.JDHBJMHAJOG().GPEPDPOJJLM;
		float num = 0f;
		if (FIJMPFHAKPB >= 0f)
		{
			return gPEPDPOJJLM - (gPEPDPOJJLM - 1f) * Mathf.Pow(2f, (0f - FIJMPFHAKPB) / jHJAFHLMOBJ);
		}
		return lFIPMCAHODJ + Mathf.Pow(2f, FIJMPFHAKPB / jHJAFHLMOBJ);
	}

	public string NGNJGOJJPLD(string name)
	{
		return MJGGAHJDPMN.GetValue(name);
	}

	public PerkInfoItem Clone(XmlNode HKCGPHLLOEA, XmlNode KKBODEIBPAK)
	{
		PerkInfoItem aCONCDFDNJH = GameUtils.FDEJIIDIPBI.ABAGJKMKCBA(Name);
		XmlDocument xmlDocument = new XmlDocument();
		if (aCONCDFDNJH != null && aCONCDFDNJH.FHCNBDOKIML() != null)
		{
			xmlDocument.LCOLFMJJDJE(aCONCDFDNJH.FHCNBDOKIML());
		}
		if (HKCGPHLLOEA != null)
		{
			XmlNode xmlNode = null;
			if (xmlDocument["Perk"] != null)
			{
				xmlNode = xmlDocument["Perk"]["Set"];
			}
			if (xmlNode == null)
			{
				if (xmlDocument["Perk"] == null)
				{
					xmlDocument.ACBPMPMPKJJ("Perk");
				}
				xmlNode = xmlDocument["Perk"].ACBPMPMPKJJ("Set");
			}
			foreach (XmlAttribute attribute in HKCGPHLLOEA.Attributes)
			{
				string name = attribute.Name;
				string value = attribute.Value;
				XmlAttribute xmlAttribute2 = xmlNode.Attributes[name];
				if (xmlAttribute2 == null)
				{
					xmlAttribute2 = xmlNode.OwnerDocument.CreateAttribute(name);
					xmlNode.Attributes.Append(xmlAttribute2);
				}
				xmlAttribute2.Value = value;
			}
		}
		if (KKBODEIBPAK != null)
		{
			XmlNode xmlNode2 = null;
			if (xmlDocument["Perk"] != null)
			{
				xmlNode2 = xmlDocument["Perk"]["RatingEvaluation"];
			}
			if (xmlNode2 == null)
			{
				if (xmlDocument["Perk"] == null)
				{
					xmlDocument.ACBPMPMPKJJ("Perk");
				}
				xmlNode2 = xmlDocument["Perk"].LCOLFMJJDJE(KKBODEIBPAK);
			}
		}
		PerkInfoItem aCONCDFDNJH2 = new PerkInfoItem();
		XmlNode xmlNode3 = xmlDocument["Perk"];
		if (xmlNode3 != null)
		{
			aCONCDFDNJH2.Parse(xmlNode3);
		}
		aCONCDFDNJH2.AKKLOMFOLNO = AKKLOMFOLNO;
		aCONCDFDNJH2.BGFEPJKDHFB = true;
		aCONCDFDNJH2.OEBOFOCIPBH();
		return aCONCDFDNJH2;
	}

	public void OEBOFOCIPBH()
	{
		defaultNode = null;
		defaultDocument = null;
	}

	public void Parse(XmlNode node)
	{
		XmlDocument mEEAKLDGLDF = new XmlDocument();
		defaultNode = defaultDocument.LCOLFMJJDJE(node);
		XmlNode xmlNode = mEEAKLDGLDF.LCOLFMJJDJE(node);
		Id = node.Attributes["ID"].ParseInt(-1);
		Level = node.Attributes["Level"].ParseInt();
		Name = node.Attributes["Name"].CIPOICEEIBK();
		HBCNKNFPAIM = node.Attributes["Alias"].CIPOICEEIBK();
		HCCKLLOEPJN = node.Attributes["BarScale"].CIPOICEEIBK();
		PDBNJHEBECL = node.Attributes["BarShift"].ParseInt();
		FEECJKOAKBE = node.Attributes["BarSetAttribute"].CIPOICEEIBK();
		NHKMCLPOMFK = node.Attributes["Image"].CIPOICEEIBK();
		MGNNJPBCOGD = xmlNode.Attributes["Description"].CIPOICEEIBK();
		JNBECGKCNBB = xmlNode.Attributes["Move"].CIPOICEEIBK();
		GDCBBAHKCIE = xmlNode.Attributes["Hidden"].ParseBool();
		DIJBDEJFKKF = xmlNode.Attributes["ItemSet"].CIPOICEEIBK();
		string text = node.Attributes["Template"].CIPOICEEIBK(string.Empty);
		Names.AddRange(text.Split('|'));
		Names.Add(Name);
		LELHEEDNMBP = DNPGIEGCGKH.SINGLE;
		if (node.Attributes["PerkType"].CIPOICEEIBK(string.Empty).Equals("Combo"))
		{
			LELHEEDNMBP = DNPGIEGCGKH.COMBO;
		}
		List<WarriorAttribute> iBLHIAHECLK = GameUtils.BGENALLCKII.IBLHIAHECLK;
		foreach (WarriorAttribute item in iBLHIAHECLK)
		{
			XmlAttribute xmlAttribute = xmlNode.Attributes[item.get_Name()];
			if (xmlAttribute != null)
			{
				IBLHIAHECLK.Set(item.get_Name(), xmlAttribute.ParseInt());
			}
		}
		XmlNode xmlNode2 = xmlNode["Set"];
		if (xmlNode2 != null)
		{
			EAOJJOKKFFB(xmlNode2);
		}
		EAIDMBHDPPO = 0;
		foreach (KeyValuePair<string, string> item2 in MJGGAHJDPMN.IBLHIAHECLK)
		{
			if (item2.Key == FEECJKOAKBE)
			{
				float result;
				if (float.TryParse(item2.Value, out result))
				{
					EAIDMBHDPPO = (int)result;
				}
				EAIDMBHDPPO += PDBNJHEBECL;
			}
		}
		foreach (XmlNode childNode in xmlNode.ChildNodes)
		{
			string name = childNode.Name;
			if (name == "Trigger")
			{
				BHPEFFEMCKN(childNode);
				FBDBIALEOJA(childNode);
			}
		}
		XmlNode xmlNode4 = xmlNode["RatingEvaluation"];
		if (xmlNode4 != null)
		{
			NDMCMKNCGMD(xmlNode4, MJGGAHJDPMN);
		}
		mEEAKLDGLDF = null;
	}

	private void ParseAttributes(XmlNode node, PerkInfoItem AEFFHJGMNFI)
	{
		Name = node.Attributes["Name"].CIPOICEEIBK((AEFFHJGMNFI == null) ? Name : AEFFHJGMNFI.Name);
		Id = node.Attributes["ID"].ParseInt((AEFFHJGMNFI == null) ? Id : AEFFHJGMNFI.Id);
		Level = node.Attributes["Level"].ParseInt((AEFFHJGMNFI == null) ? Level : AEFFHJGMNFI.Level);
		HBCNKNFPAIM = node.Attributes["Alias"].CIPOICEEIBK((AEFFHJGMNFI == null) ? HBCNKNFPAIM : AEFFHJGMNFI.HBCNKNFPAIM);
		HCCKLLOEPJN = node.Attributes["BarScale"].CIPOICEEIBK((AEFFHJGMNFI == null) ? HCCKLLOEPJN : AEFFHJGMNFI.HCCKLLOEPJN);
		PDBNJHEBECL = node.Attributes["BarShift"].ParseInt((AEFFHJGMNFI == null) ? PDBNJHEBECL : AEFFHJGMNFI.PDBNJHEBECL);
		FEECJKOAKBE = node.Attributes["BarSetAttribute"].CIPOICEEIBK((AEFFHJGMNFI == null) ? FEECJKOAKBE : AEFFHJGMNFI.FEECJKOAKBE);
		NHKMCLPOMFK = node.Attributes["Image"].CIPOICEEIBK((AEFFHJGMNFI == null) ? NHKMCLPOMFK : AEFFHJGMNFI.NHKMCLPOMFK);
		MGNNJPBCOGD = node.Attributes["Description"].CIPOICEEIBK((AEFFHJGMNFI == null) ? MGNNJPBCOGD : AEFFHJGMNFI.MGNNJPBCOGD);
		JNBECGKCNBB = node.Attributes["Move"].CIPOICEEIBK((AEFFHJGMNFI == null) ? JNBECGKCNBB : AEFFHJGMNFI.JNBECGKCNBB);
		GDCBBAHKCIE = node.Attributes["Hidden"].ParseBool((AEFFHJGMNFI == null) ? GDCBBAHKCIE : AEFFHJGMNFI.GDCBBAHKCIE);
		DIJBDEJFKKF = node.Attributes["ItemSet"].CIPOICEEIBK((AEFFHJGMNFI == null) ? DIJBDEJFKKF : AEFFHJGMNFI.DIJBDEJFKKF);
		string text = node.Attributes["PerkType"].CIPOICEEIBK(string.Empty);
		LELHEEDNMBP = ((!text.Equals("COMBO")) ? DNPGIEGCGKH.SINGLE : DNPGIEGCGKH.COMBO);
		if (AEFFHJGMNFI != null)
		{
			IBLHIAHECLK.AddRange(AEFFHJGMNFI.IBLHIAHECLK);
		}
		List<WarriorAttribute> iBLHIAHECLK = GameUtils.BGENALLCKII.IBLHIAHECLK;
		foreach (WarriorAttribute item in iBLHIAHECLK)
		{
			XmlAttribute xmlAttribute = node.Attributes[item.get_Name()];
			if (xmlAttribute != null)
			{
				IBLHIAHECLK.Set(item.get_Name(), xmlAttribute.ParseInt());
			}
		}
	}

	private void CMPJBHLCPJA()
	{
		IONLOHAKLFF.Clear();
		IONLOHAKLFF.AddRange(Names);
		IONLOHAKLFF.Remove(Name);
		List<string> list = new List<string>();
		foreach (string item in IONLOHAKLFF)
		{
			PerkInfoItem aCONCDFDNJH = GameUtils.FDEJIIDIPBI.ABAGJKMKCBA(item);
			if (aCONCDFDNJH != null)
			{
				list.AddRange(aCONCDFDNJH.IONLOHAKLFF);
			}
		}
		list.ForEach((string DHDMNHCIPEH) =>
		{
			IONLOHAKLFF.AddIfNotExist(DHDMNHCIPEH);
		});
	}

	private void CPLKAPAOBIA()
	{
		foreach (string item in IONLOHAKLFF)
		{
			PerkInfoItem aCONCDFDNJH = GameUtils.FDEJIIDIPBI.ABAGJKMKCBA(item);
			if (aCONCDFDNJH == null)
			{
				continue;
			}
			XmlNode xmlNode = aCONCDFDNJH.FHCNBDOKIML().Clone();
			foreach (XmlNode childNode in xmlNode.ChildNodes)
			{
				string name = childNode.Name;
				if (name.Equals("Trigger"))
				{
					BHPEFFEMCKN(childNode);
					FBDBIALEOJA(childNode);
				}
			}
		}
	}

	private void NDMCMKNCGMD(XmlNode node)
	{
		if (node != null)
		{
			NDMCMKNCGMD(node, EPBADFHIJAH());
			return;
		}
		foreach (string item in IONLOHAKLFF)
		{
			PerkInfoItem aCONCDFDNJH = GameUtils.FDEJIIDIPBI.ABAGJKMKCBA(item);
			if (aCONCDFDNJH != null)
			{
				XmlNode xmlNode = aCONCDFDNJH.FHCNBDOKIML()["RatingEvaluation"];
				if (xmlNode != null)
				{
					NDMCMKNCGMD(xmlNode, EPBADFHIJAH());
					break;
				}
			}
		}
	}

	private void POHPBCCCIPJ(XmlNode node)
	{
		List<string> list = new List<string>(Names);
		list.Reverse();
		foreach (string item in list)
		{
			PerkInfoItem aCONCDFDNJH = GameUtils.FDEJIIDIPBI.ABAGJKMKCBA(item);
			if (aCONCDFDNJH == null)
			{
				continue;
			}
			foreach (KeyValuePair<string, string> item2 in aCONCDFDNJH.EPBADFHIJAH().IBLHIAHECLK)
			{
				MJGGAHJDPMN.ENDOOADOLEO(item2.Key, item2.Value);
			}
		}
		if (node != null)
		{
			EAOJJOKKFFB(node);
		}
	}

	private void FBDBIALEOJA(XmlNode node)
	{
		PerkTrigger eICIICPBDMC = new PerkTrigger();
		eICIICPBDMC.JMOIMIHPBOM(this);
		eICIICPBDMC.Parse(node);
		NOJEIGNOPII().Add(eICIICPBDMC);
	}

	private void BHPEFFEMCKN(XmlNode node)
	{
		foreach (XmlNode childNode in node.ChildNodes)
		{
			foreach (XmlAttribute attribute in childNode.Attributes)
			{
				SetAttributes(attribute);
			}
			if (childNode.ChildNodes.Count > 0)
			{
				BHPEFFEMCKN(childNode);
			}
		}
	}

	private void SetAttributes(XmlAttribute CJEPEDKEEGF)
	{
		string name = CJEPEDKEEGF.Name;
		string text = CJEPEDKEEGF.Value;
		FunctionExtension oPIFBDJNMKD = new FunctionExtension();
		oPIFBDJNMKD.Parse(text);
		List<FunctionExtension.FunctionObject> list = oPIFBDJNMKD.COGDGCDPOBJ();
		foreach (FunctionExtension.FunctionObject item in list)
		{
			if (item.body[0] == '_')
			{
				string gOHIIMFFFJI = item.body.Substring(1, item.body.Length - 1);
				string newValue = MJGGAHJDPMN.GetValue(gOHIIMFFFJI);
				text = text.Replace(item.body, newValue);
			}
		}
		CJEPEDKEEGF.Value = text;
	}

	private void EAOJJOKKFFB(XmlNode node)
	{
		foreach (XmlAttribute attribute in node.Attributes)
		{
			string name = attribute.Name;
			string value = attribute.Value;
			MJGGAHJDPMN.ENDOOADOLEO(name, value);
		}
	}

	private void NDMCMKNCGMD(XmlNode MGOANJIJHGB, PerkSetAttributes CJILONFAJIK)
	{
		int count = MGOANJIJHGB.ChildNodes.Count;
		if (count > 0)
		{
			MLMLENHGNDJ.Clear();
		}
		foreach (XmlNode childNode in MGOANJIJHGB.ChildNodes)
		{
			string name = childNode.Name;
			if (name.Equals("Rating"))
			{
				Rating cNLOJEAEGLG = new Rating();
				cNLOJEAEGLG.Parse(childNode, CJILONFAJIK);
				MLMLENHGNDJ.Add(cNLOJEAEGLG);
			}
		}
	}

	public void EIKAGOOJOCN(List<PerkTrigger> DCJLKCFKCOM, PerkEvent.KNKIIEPDCPN LFLGCDNKNJI)
	{
		foreach (PerkTrigger item in NOJEIGNOPII())
		{
			List<PerkEvent> list = item.PHLLJJNCEIH();
			foreach (PerkEvent item2 in list)
			{
				if (item2.get_Type() == LFLGCDNKNJI)
				{
					DCJLKCFKCOM.Add(item);
					break;
				}
			}
		}
	}

	public void OKPFNCJFLDL(FunctionExtension.CallbackResult DCJLKCFKCOM)
	{
	}

	public void HJFEFJIEINN(FunctionExtension.CallbackResult DCJLKCFKCOM)
	{
		FunctionExtension.GLBAFLLMOOH gLBAFLLMOOH = DCJLKCFKCOM.data as FunctionExtension.GLBAFLLMOOH;
		FunctionResult nAGGNMIFFGK = DCJLKCFKCOM.NAGGNMIFFGK;
		PerkObject iNCAIGLKDIE = DCJLKCFKCOM.target as PerkObject;
		Model fGCODGKLHED = ((ELPJBGIPEIB() == null) ? null : ELPJBGIPEIB());
		if (fGCODGKLHED != null && nAGGNMIFFGK.DCJLKCFKCOM.Equals("Enemy"))
		{
			fGCODGKLHED = fGCODGKLHED.EGGEACCDAEK();
		}
		switch (gLBAFLLMOOH.FJLOLCPJACB)
		{
		case "UniformFloatRandom":
			KGHGLIONOPM(fGCODGKLHED, gLBAFLLMOOH, iNCAIGLKDIE, nAGGNMIFFGK);
			return;
		case "PlayerAttribute":
			OBEGJAABGBJ(fGCODGKLHED, gLBAFLLMOOH, iNCAIGLKDIE, nAGGNMIFFGK);
			return;
		case "PlayerParameter":
			BGBEMEEJGDC(fGCODGKLHED, gLBAFLLMOOH, iNCAIGLKDIE, nAGGNMIFFGK);
			return;
		case "RoundParameter":
			CJPELOMKFOO(fGCODGKLHED, gLBAFLLMOOH, iNCAIGLKDIE, nAGGNMIFFGK);
			return;
		case "Hit":
			EDDGIAMBDKA(fGCODGKLHED, gLBAFLLMOOH, iNCAIGLKDIE, nAGGNMIFFGK);
			return;
		case "StringInArray":
			BKOOJLFFJNG(fGCODGKLHED, gLBAFLLMOOH, iNCAIGLKDIE, nAGGNMIFFGK);
			return;
		case "CoordX":
			DIHHHLGCDJN(fGCODGKLHED, gLBAFLLMOOH, iNCAIGLKDIE, nAGGNMIFFGK);
			return;
		case "CoordY":
			LHEFMGCHEBI(fGCODGKLHED, gLBAFLLMOOH, iNCAIGLKDIE, nAGGNMIFFGK);
			return;
		case "CoordZ":
			MAFKCCHKPGC(fGCODGKLHED, gLBAFLLMOOH, iNCAIGLKDIE, nAGGNMIFFGK);
			return;
		case "RandomAspect":
			IEJMALPPDCN(fGCODGKLHED, gLBAFLLMOOH, iNCAIGLKDIE, nAGGNMIFFGK);
			return;
		case "Abs":
			NKAENNIIBFJ(fGCODGKLHED, gLBAFLLMOOH, iNCAIGLKDIE, nAGGNMIFFGK);
			return;
		case "Aspect":
			JPNNFEFLNME(fGCODGKLHED, gLBAFLLMOOH, iNCAIGLKDIE, nAGGNMIFFGK);
			return;
		case "Round":
			MPHLCCNOALE(fGCODGKLHED, gLBAFLLMOOH, iNCAIGLKDIE, nAGGNMIFFGK);
			return;
		case "Variable":
			JADKFPGJAJP(fGCODGKLHED, gLBAFLLMOOH, iNCAIGLKDIE, nAGGNMIFFGK);
			return;
		case "MovesVariable":
			JADKFPGJAJP(fGCODGKLHED, gLBAFLLMOOH, iNCAIGLKDIE, nAGGNMIFFGK);
			return;
		case "Player":
			BJAOOMLBIHK(fGCODGKLHED, gLBAFLLMOOH, iNCAIGLKDIE, nAGGNMIFFGK);
			return;
		case "CurrentFight":
			BKLKFLGMIBD(fGCODGKLHED, gLBAFLLMOOH, iNCAIGLKDIE, nAGGNMIFFGK);
			return;
		}
		FunctionExtension.DLLJOIFFBPL dLLJOIFFBPL = FunctionExtension.MHKNIEBONKD(gLBAFLLMOOH.FJLOLCPJACB);
		bool flag = gLBAFLLMOOH.FJLOLCPJACB.Equals("Compare");
		if (flag || dLLJOIFFBPL != FunctionExtension.DLLJOIFFBPL.COMPARE_NONE)
		{
			CGDKLCKFDMI(flag, fGCODGKLHED, gLBAFLLMOOH, iNCAIGLKDIE, nAGGNMIFFGK);
		}
	}

	private void BKLKFLGMIBD(Model ACENLMONNPA, FunctionExtension.GLBAFLLMOOH KJFKPMCPIBH, PerkObject INCAIGLKDIE, FunctionResult DCJLKCFKCOM)
	{
		if (Fight.OHNKFOHIAKG() != null && KJFKPMCPIBH.HBDLDIKHFEG.Equals("isRaid"))
		{
			bool flag = Fight.OHNKFOHIAKG().OGNINOBBHIG().get_Type() == BattleType.FightRaid;
			DCJLKCFKCOM.DCJLKCFKCOM = ((!flag) ? "0" : "1");
		}
	}

	private void JADKFPGJAJP(Model ACENLMONNPA, FunctionExtension.GLBAFLLMOOH KJFKPMCPIBH, PerkObject INCAIGLKDIE, FunctionResult DCJLKCFKCOM)
	{
		if (KJFKPMCPIBH.EIALKNELNMB.Count == 0)
		{
			DCJLKCFKCOM.DCJLKCFKCOM = string.Empty;
		}
		string mJOCMMIBOGJ = KJFKPMCPIBH.EIALKNELNMB[0].body;
		Dictionary<string, float> cNOPDMEAODG = ACENLMONNPA.EBABHGHPLFK().PerkVariables;
		Dictionary<string, string> stringVariables = ACENLMONNPA.EBABHGHPLFK().PerkStringVariables;
		if (stringVariables.ContainsKey(mJOCMMIBOGJ))
		{
			DCJLKCFKCOM.DCJLKCFKCOM = stringVariables[mJOCMMIBOGJ];
		}
		else if (cNOPDMEAODG.ContainsKey(mJOCMMIBOGJ))
		{
			DCJLKCFKCOM.DCJLKCFKCOM = cNOPDMEAODG[mJOCMMIBOGJ].ToString();
		}
		else
		{
			DCJLKCFKCOM.DCJLKCFKCOM = string.Empty;
		}
	}

	private void BJAOOMLBIHK(Model ACENLMONNPA, FunctionExtension.GLBAFLLMOOH KJFKPMCPIBH, PerkObject INCAIGLKDIE, FunctionResult DCJLKCFKCOM)
	{
		if (KJFKPMCPIBH.HBDLDIKHFEG.Equals("Level"))
		{
			DCJLKCFKCOM.DCJLKCFKCOM = ListSF.CCDKHLAMKKO().PINDEKDNCNL().ToString();
		}
	}

	private void KGHGLIONOPM(Model ACENLMONNPA, FunctionExtension.GLBAFLLMOOH KJFKPMCPIBH, PerkObject INCAIGLKDIE, FunctionResult DCJLKCFKCOM)
	{
		DCJLKCFKCOM.DCJLKCFKCOM = NekkiMath.randomFloat(1f).ToString();
	}

	private void OBEGJAABGBJ(Model ACENLMONNPA, FunctionExtension.GLBAFLLMOOH KJFKPMCPIBH, PerkObject INCAIGLKDIE, FunctionResult DCJLKCFKCOM)
	{
		if (ACENLMONNPA != null)
		{
			ModelParameters kMMJCHDKBDO = ACENLMONNPA.KMMJCHDKBDO;
			if (kMMJCHDKBDO != null)
			{
				int OEMALIFPGPO = 0;
				kMMJCHDKBDO.IBLHIAHECLK.Get(KJFKPMCPIBH.HBDLDIKHFEG, ref OEMALIFPGPO);
				DCJLKCFKCOM.DCJLKCFKCOM = OEMALIFPGPO.ToString();
			}
		}
	}

	private void BGBEMEEJGDC(Model ACENLMONNPA, FunctionExtension.GLBAFLLMOOH KJFKPMCPIBH, PerkObject INCAIGLKDIE, FunctionResult DCJLKCFKCOM)
	{
		if (ACENLMONNPA != null)
		{
			switch (KJFKPMCPIBH.HBDLDIKHFEG)
			{
			case "Health":
				DCJLKCFKCOM.DCJLKCFKCOM = ACENLMONNPA.KKMCHCNOHMB().ToString();
				break;
			case "Pain":
				DCJLKCFKCOM.DCJLKCFKCOM = ACENLMONNPA.IDFIBPDPFLK().ToString();
				break;
			case "Shock":
				DCJLKCFKCOM.DCJLKCFKCOM = ((!ACENLMONNPA.EDJFLMILEBA()) ? "0" : "1");
				break;
			case "Disarm":
				DCJLKCFKCOM.DCJLKCFKCOM = ((!ACENLMONNPA.HFHJFOEFPCD()) ? "0" : "1");
				break;
			case "Style":
				DCJLKCFKCOM.DCJLKCFKCOM = ACENLMONNPA.LDLLJHEDCPD;
				break;
			case "StyleGain":
				DCJLKCFKCOM.DCJLKCFKCOM = ACENLMONNPA.CDBOONBLDBK.ToString();
				break;
			case "Combo":
				DCJLKCFKCOM.DCJLKCFKCOM = ACENLMONNPA.NPDOLGNNINO().ToString();
				break;
			case "MagicBullet":
				DCJLKCFKCOM.DCJLKCFKCOM = ACENLMONNPA.LPOJKGLFMAL().ToString();
				break;
			case "MagicCharge":
				DCJLKCFKCOM.DCJLKCFKCOM = ACENLMONNPA.EKAFGLHNMCN().ToString();
				break;
			case "Magic":
				DCJLKCFKCOM.DCJLKCFKCOM = (ACENLMONNPA.KMMJCHDKBDO.ADBKGIBBNHJ == null) ?
					string.Empty : ACENLMONNPA.KMMJCHDKBDO.ADBKGIBBNHJ.Name;
				break;
			case "Ranged":
				DCJLKCFKCOM.DCJLKCFKCOM = (ACENLMONNPA.KMMJCHDKBDO.LGHMILECPLA == null) ?
					string.Empty : ACENLMONNPA.KMMJCHDKBDO.LGHMILECPLA.Name;
				break;
			case "Weapon":
				DCJLKCFKCOM.DCJLKCFKCOM = (ACENLMONNPA.KMMJCHDKBDO.JGMLKIPCFII == null) ?
					string.Empty : ACENLMONNPA.KMMJCHDKBDO.JGMLKIPCFII.Name;
				break;
			case "Skeleton":
				DCJLKCFKCOM.DCJLKCFKCOM = (ACENLMONNPA.KMMJCHDKBDO.PILJCAOFAED == null) ?
					string.Empty : ACENLMONNPA.KMMJCHDKBDO.PILJCAOFAED.Name;
				break;
			case "RaidChargeBullet":
				break;
			case "DamageConverter":
				DCJLKCFKCOM.DCJLKCFKCOM = ACENLMONNPA.LJCFIOPBNKD().ToString();
				break;
			case "DefaultPerksAspect":
				DCJLKCFKCOM.DCJLKCFKCOM = ACENLMONNPA.FGNCFGDOELL().ToString();
				break;
			case "isPlayer":
				DCJLKCFKCOM.DCJLKCFKCOM = ((!ACENLMONNPA.EPCNJLEHJCB()) ? "0" : "1");
				break;
			}
		}
	}

	private void CJPELOMKFOO(Model ACENLMONNPA, FunctionExtension.GLBAFLLMOOH KJFKPMCPIBH, PerkObject INCAIGLKDIE, FunctionResult DCJLKCFKCOM)
	{
		if (Fight.OHNKFOHIAKG() != null)
		{
			switch (KJFKPMCPIBH.HBDLDIKHFEG)
			{
			case "Number":
				DCJLKCFKCOM.DCJLKCFKCOM = Fight.OHNKFOHIAKG().get_RoundNumber().ToString();
				break;
			case "TimeLeft":
				DCJLKCFKCOM.DCJLKCFKCOM = Fight.OHNKFOHIAKG().get_RoundTimeLeftFrames().ToString();
				break;
			case "TimePassed":
				DCJLKCFKCOM.DCJLKCFKCOM = Fight.OHNKFOHIAKG().get_RoundTimePassedFrames().ToString();
				break;
			case "RoundTime":
				DCJLKCFKCOM.DCJLKCFKCOM = Fight.OHNKFOHIAKG().get_RoundTimeTotalFrames().ToString();
				break;
			}
		}
	}

	private void CGDKLCKFDMI(bool GOIOEAHAAIA, Model ACENLMONNPA, FunctionExtension.GLBAFLLMOOH KJFKPMCPIBH, PerkObject INCAIGLKDIE, FunctionResult DCJLKCFKCOM)
	{
		if (DCJLKCFKCOM != null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(DCJLKCFKCOM.DCJLKCFKCOM);
			if (!GOIOEAHAAIA)
			{
				stringBuilder.Append(",");
				stringBuilder.Append(KJFKPMCPIBH.FJLOLCPJACB);
			}
			bool flag = FunctionExtension.IsCompare(stringBuilder.ToString());
			DCJLKCFKCOM.DCJLKCFKCOM = ((!flag) ? "0" : "1");
		}
	}

	private void DIHHHLGCDJN(Model ACENLMONNPA, FunctionExtension.GLBAFLLMOOH KJFKPMCPIBH, PerkObject INCAIGLKDIE, FunctionResult DCJLKCFKCOM)
	{
		if (KJFKPMCPIBH.EIALKNELNMB.Count < 2)
		{
			DCJLKCFKCOM.DCJLKCFKCOM = "-10000";
			return;
		}
		string mJOCMMIBOGJ = KJFKPMCPIBH.EIALKNELNMB[0].body;
		string mJOCMMIBOGJ2 = KJFKPMCPIBH.EIALKNELNMB[1].body;
		string text = string.Empty;
		if (KJFKPMCPIBH.EIALKNELNMB.Count >= 3)
		{
			text = KJFKPMCPIBH.EIALKNELNMB[2].body;
		}
		DistancePoint oGHICEHKFOL = new DistancePoint();
		oGHICEHKFOL.Create(mJOCMMIBOGJ, mJOCMMIBOGJ2, text);
		float num = 0f;
		if (oGHICEHKFOL.HLGJJGHDEAP != DistancePoint.JJIAEPLMBFF.OBJECT_NODES)
		{
			num = oGHICEHKFOL.ILIKNABGPNK(ACENLMONNPA.EBABHGHPLFK());
		}
		else
		{
			if (oGHICEHKFOL.OOFFOILONLO == ModelType.KEIDBIOIFGA.MODEL_THIS)
			{
				ModelNode lCDGOCIAIDK = ACENLMONNPA.CLDMEJKGLBA().EGHIDHMENEF(text);
				num = lCDGOCIAIDK.ICLEOFDKDIF().GILCBJJPKBK();
			}
			if (oGHICEHKFOL.OOFFOILONLO == ModelType.KEIDBIOIFGA.MODEL_OTHER)
			{
				ModelNode lCDGOCIAIDK2 = ACENLMONNPA.EGGEACCDAEK().CLDMEJKGLBA().EGHIDHMENEF(text);
				num = lCDGOCIAIDK2.ICLEOFDKDIF().GILCBJJPKBK();
			}
		}
		DCJLKCFKCOM.DCJLKCFKCOM = num.ToString();
	}

	private void LHEFMGCHEBI(Model ACENLMONNPA, FunctionExtension.GLBAFLLMOOH KJFKPMCPIBH, PerkObject INCAIGLKDIE, FunctionResult DCJLKCFKCOM)
	{
		if (KJFKPMCPIBH.EIALKNELNMB.Count < 2)
		{
			DCJLKCFKCOM.DCJLKCFKCOM = "-10000";
			return;
		}
		string mJOCMMIBOGJ = KJFKPMCPIBH.EIALKNELNMB[0].body;
		string mJOCMMIBOGJ2 = KJFKPMCPIBH.EIALKNELNMB[1].body;
		string text = string.Empty;
		if (KJFKPMCPIBH.EIALKNELNMB.Count >= 3)
		{
			text = KJFKPMCPIBH.EIALKNELNMB[2].body;
		}
		DistancePoint oGHICEHKFOL = new DistancePoint();
		oGHICEHKFOL.Create(mJOCMMIBOGJ, mJOCMMIBOGJ2, text);
		float num = 0f;
		if (oGHICEHKFOL.HLGJJGHDEAP != DistancePoint.JJIAEPLMBFF.OBJECT_NODES)
		{
			num = oGHICEHKFOL.MJPKHPNIJGK(ACENLMONNPA.EBABHGHPLFK());
		}
		else
		{
			if (oGHICEHKFOL.OOFFOILONLO == ModelType.KEIDBIOIFGA.MODEL_THIS)
			{
				ModelNode lCDGOCIAIDK = ACENLMONNPA.CLDMEJKGLBA().EGHIDHMENEF(text);
				num = lCDGOCIAIDK.ICLEOFDKDIF().OBIMBNIBEFG();
			}
			if (oGHICEHKFOL.OOFFOILONLO == ModelType.KEIDBIOIFGA.MODEL_OTHER)
			{
				ModelNode lCDGOCIAIDK2 = ACENLMONNPA.EGGEACCDAEK().CLDMEJKGLBA().EGHIDHMENEF(text);
				num = lCDGOCIAIDK2.ICLEOFDKDIF().OBIMBNIBEFG();
			}
		}
		DCJLKCFKCOM.DCJLKCFKCOM = num.ToString();
	}

	private void MAFKCCHKPGC(Model ACENLMONNPA, FunctionExtension.GLBAFLLMOOH KJFKPMCPIBH, PerkObject INCAIGLKDIE, FunctionResult DCJLKCFKCOM)
	{
		if (KJFKPMCPIBH.EIALKNELNMB.Count < 2)
		{
			DCJLKCFKCOM.DCJLKCFKCOM = "-10000";
			return;
		}
		string mJOCMMIBOGJ = KJFKPMCPIBH.EIALKNELNMB[0].body;
		string mJOCMMIBOGJ2 = KJFKPMCPIBH.EIALKNELNMB[1].body;
		string text = string.Empty;
		if (KJFKPMCPIBH.EIALKNELNMB.Count >= 3)
		{
			text = KJFKPMCPIBH.EIALKNELNMB[2].body;
		}
		DistancePoint oGHICEHKFOL = new DistancePoint();
		oGHICEHKFOL.Create(mJOCMMIBOGJ, mJOCMMIBOGJ2, text);
		float num = 0f;
		if (oGHICEHKFOL.HLGJJGHDEAP != DistancePoint.JJIAEPLMBFF.OBJECT_NODES)
		{
			num = oGHICEHKFOL.CHBKDOCBKFJ(ACENLMONNPA.EBABHGHPLFK());
		}
		else
		{
			if (oGHICEHKFOL.OOFFOILONLO == ModelType.KEIDBIOIFGA.MODEL_THIS)
			{
				ModelNode lCDGOCIAIDK = ACENLMONNPA.CLDMEJKGLBA().EGHIDHMENEF(text);
				num = lCDGOCIAIDK.ICLEOFDKDIF().KMFEKANLCFO();
			}
			if (oGHICEHKFOL.OOFFOILONLO == ModelType.KEIDBIOIFGA.MODEL_OTHER)
			{
				ModelNode lCDGOCIAIDK2 = ACENLMONNPA.EGGEACCDAEK().CLDMEJKGLBA().EGHIDHMENEF(text);
				num = lCDGOCIAIDK2.ICLEOFDKDIF().KMFEKANLCFO();
			}
		}
		DCJLKCFKCOM.DCJLKCFKCOM = num.ToString();
	}

	private void NKAENNIIBFJ(Model ACENLMONNPA, FunctionExtension.GLBAFLLMOOH KJFKPMCPIBH, PerkObject INCAIGLKDIE, FunctionResult DCJLKCFKCOM)
	{
		string dCJLKCFKCOM = DCJLKCFKCOM.DCJLKCFKCOM;
		double value = 0.0;
		Dictionary<string, RpnParser.PHNLIHEJEPK> pPEABEJMCPI = new Dictionary<string, RpnParser.PHNLIHEJEPK>();
		Dictionary<string, RpnParser.ParameterDelegate> gIOGAJGIGMO = new Dictionary<string, RpnParser.ParameterDelegate>();
		RpnParser.init(pPEABEJMCPI, gIOGAJGIGMO);
		RpnParser.Formula lANLKOHCGEJ = new RpnParser.Formula(dCJLKCFKCOM);
		if (lANLKOHCGEJ.OJEHEKMJJBL() == 0)
		{
			double result;
			if (double.TryParse(lANLKOHCGEJ.ODHJHHMEEOI().ToString(), out result))
			{
				value = result;
			}
		}
		else
		{
			LLLOJBFMONN.Error("Abs function error! Argument is not valid expression: {0}", DCJLKCFKCOM.DCJLKCFKCOM);
		}
		DCJLKCFKCOM.DCJLKCFKCOM = Math.Abs(value).ToString();
	}

	private void IEJMALPPDCN(Model ACENLMONNPA, FunctionExtension.GLBAFLLMOOH KJFKPMCPIBH, PerkObject INCAIGLKDIE, FunctionResult DCJLKCFKCOM)
	{
		if (KJFKPMCPIBH.EIALKNELNMB.Count < 2)
		{
			DCJLKCFKCOM.DCJLKCFKCOM = "0";
			return;
		}
		string mJOCMMIBOGJ = KJFKPMCPIBH.EIALKNELNMB[0].body;
		string mJOCMMIBOGJ2 = KJFKPMCPIBH.EIALKNELNMB[1].body;
		int lHNCHOAEGEA = mJOCMMIBOGJ.ToInt();
		int kAEPJHHLLPK = mJOCMMIBOGJ2.ToInt() + 1;
		NekkiMath.KACCBCCEPGB();
		int num = NekkiMath.randomInt(lHNCHOAEGEA, kAEPJHHLLPK);
		int mHNCENBCECJ = ListSF.CCDKHLAMKKO().PINDEKDNCNL();
		int num2 = ForgeManager.ELEBLBJKDBI().GetAspectValueByLevel(mHNCENBCECJ);
		DCJLKCFKCOM.DCJLKCFKCOM = (num2 + num).ToString();
	}

	private void JPNNFEFLNME(Model ACENLMONNPA, FunctionExtension.GLBAFLLMOOH KJFKPMCPIBH, PerkObject INCAIGLKDIE, FunctionResult DCJLKCFKCOM)
	{
		if (KJFKPMCPIBH.EIALKNELNMB.Count != 1)
		{
			DCJLKCFKCOM.DCJLKCFKCOM = "0";
			LLLOJBFMONN.Error("Aspect function error! Number of argument is not 1: {0}", DCJLKCFKCOM.DCJLKCFKCOM);
		}
		else
		{
			string mJOCMMIBOGJ = KJFKPMCPIBH.EIALKNELNMB[0].body;
			float fIJMPFHAKPB = mJOCMMIBOGJ.ToFloat();
			DCJLKCFKCOM.DCJLKCFKCOM = DODDEPEMBMC(fIJMPFHAKPB).ToString();
		}
	}

	private void MPHLCCNOALE(Model ACENLMONNPA, FunctionExtension.GLBAFLLMOOH KJFKPMCPIBH, PerkObject INCAIGLKDIE, FunctionResult DCJLKCFKCOM)
	{
		float num = 0f;
		int num2 = 0;
		string key = "trunc";
		JFOFGHPCIBE jFOFGHPCIBE = JFOFGHPCIBE.RT_TRUNC;
		if (KJFKPMCPIBH.EIALKNELNMB.Count < 1)
		{
			DCJLKCFKCOM.DCJLKCFKCOM = "RoundingError";
			return;
		}
		if (KJFKPMCPIBH.EIALKNELNMB.Count > 3)
		{
			DCJLKCFKCOM.DCJLKCFKCOM = "RoundingError";
			return;
		}
		if (KJFKPMCPIBH.EIALKNELNMB.Count >= 1)
		{
			string mJOCMMIBOGJ = KJFKPMCPIBH.EIALKNELNMB[0].body;
			num = mJOCMMIBOGJ.ToFloat();
		}
		if (KJFKPMCPIBH.EIALKNELNMB.Count >= 2)
		{
			string mJOCMMIBOGJ2 = KJFKPMCPIBH.EIALKNELNMB[1].body;
			num2 = mJOCMMIBOGJ2.ToInt();
		}
		if (KJFKPMCPIBH.EIALKNELNMB.Count == 3)
		{
			string mJOCMMIBOGJ3 = KJFKPMCPIBH.EIALKNELNMB[2].body;
			key = mJOCMMIBOGJ3;
		}
		if (!CCNDJMNINEM.ContainsKey(key))
		{
			DCJLKCFKCOM.DCJLKCFKCOM = "RoundingError";
			return;
		}
		jFOFGHPCIBE = CCNDJMNINEM[key];
		float num3 = 0f;
		switch (jFOFGHPCIBE)
		{
		case JFOFGHPCIBE.RT_FLOOR:
			DCJLKCFKCOM.DCJLKCFKCOM = NekkiMath.KAJCCKDDMHL(num, num2).ToString();
			break;
		case JFOFGHPCIBE.RT_CEIL:
			DCJLKCFKCOM.DCJLKCFKCOM = NekkiMath.KOCMHLJOCPA(num, num2).ToString();
			break;
		case JFOFGHPCIBE.RT_TRUNC:
			DCJLKCFKCOM.DCJLKCFKCOM = NekkiMath.EPOBPGPJPNG(num, num2).ToString();
			break;
		case JFOFGHPCIBE.RT_INF:
			DCJLKCFKCOM.DCJLKCFKCOM = NekkiMath.GAHKBAANMKL(num, num2).ToString();
			break;
		default:
			DCJLKCFKCOM.DCJLKCFKCOM = string.Empty;
			break;
		}
	}

	private void BKOOJLFFJNG(Model ACENLMONNPA, FunctionExtension.GLBAFLLMOOH KJFKPMCPIBH, PerkObject INCAIGLKDIE, FunctionResult DCJLKCFKCOM)
	{
		List<string> list = new List<string>(DCJLKCFKCOM.DCJLKCFKCOM.Split(','));
		if (list.Count > 1)
		{
			string text = list[0];
			string value = list[1];
			List<string> list2 = new List<string>(text.Split('|'));
			int i = 0;
			for (int count = list2.Count; i < count; i++)
			{
				if (list2[i].Equals(value))
				{
					DCJLKCFKCOM.DCJLKCFKCOM = "1";
					return;
				}
			}
		}
		DCJLKCFKCOM.DCJLKCFKCOM = "0";
	}

	private void EDDGIAMBDKA(Model ACENLMONNPA, FunctionExtension.GLBAFLLMOOH KJFKPMCPIBH, PerkObject INCAIGLKDIE, FunctionResult DCJLKCFKCOM)
	{
		if (Fight.OHNKFOHIAKG() == null)
		{
			return;
		}
		Model.StrikeResult fKGAAFNNCNE = Fight.OHNKFOHIAKG().FKGAAFNNCNE;
		if (fKGAAFNNCNE == null)
		{
			return;
		}
		switch (KJFKPMCPIBH.HBDLDIKHFEG)
		{
		case "Player":
			DCJLKCFKCOM.DCJLKCFKCOM = ((fKGAAFNNCNE.KJDFJPBIGJC != ACENLMONNPA) ? "Enemy" : "Me");
			break;
		case "DefenseAttribute":
			DCJLKCFKCOM.DCJLKCFKCOM = fKGAAFNNCNE.DefenceAttribute;
			break;
		case "Block":
			DCJLKCFKCOM.DCJLKCFKCOM = ((!fKGAAFNNCNE.DFOHNJEBDED) ? "0" : "1");
			break;
		case "Critical":
			DCJLKCFKCOM.DCJLKCFKCOM = ((!fKGAAFNNCNE.DNGKOMPMPCD) ? "0" : "1");
			break;
		case "Shock":
			DCJLKCFKCOM.DCJLKCFKCOM = ((!fKGAAFNNCNE.APCAKCCOMLO) ? "0" : "1");
			break;
		case "Animations":
		{
			StringBuilder stringBuilder = new StringBuilder();
			List<string> list = fKGAAFNNCNE.PBPDKJNKFCJ.FOLOOGCLPNE();
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				stringBuilder.Append(list[i]);
				if (i < count - 1)
				{
					stringBuilder.Append("|");
				}
			}
			DCJLKCFKCOM.DCJLKCFKCOM = stringBuilder.ToString();
			break;
		}
		case "Damage":
			DCJLKCFKCOM.DCJLKCFKCOM = fKGAAFNNCNE.EEDJBBOCFNL.ToString();
			break;
		case "BaseDamage":
		{
			float hMOLHIEDINK = fKGAAFNNCNE.HMOLHIEDINK;
			float num = 1f;
			if (fKGAAFNNCNE.DFOHNJEBDED)
			{
				int OEMALIFPGPO = 0;
				string nJFGLOECJEK = GameUtils.DAMKDJINILI().Attribute;
				fKGAAFNNCNE.KJDFJPBIGJC.KMMJCHDKBDO.IBLHIAHECLK.Get(nJFGLOECJEK, ref OEMALIFPGPO);
				float aMKPAGCFMIN = GameUtils.DAMKDJINILI().Base;
				num = Mathf.Pow(2f, (float)OEMALIFPGPO * aMKPAGCFMIN);
			}
			float num2 = 1f;
			if (fKGAAFNNCNE.DNGKOMPMPCD && fKGAAFNNCNE.GAIBPAGPEGK != null)
			{
				int OEMALIFPGPO2 = 0;
				string nJFGLOECJEK2 = GameUtils.IOGOPCABLON().Attribute;
				fKGAAFNNCNE.GAIBPAGPEGK.KMMJCHDKBDO.IBLHIAHECLK.Get(nJFGLOECJEK2, ref OEMALIFPGPO2);
				float aMKPAGCFMIN2 = GameUtils.IOGOPCABLON().Base;
				num2 = Mathf.Pow(2f, (float)OEMALIFPGPO2 * aMKPAGCFMIN2);
			}
			DCJLKCFKCOM.DCJLKCFKCOM = (hMOLHIEDINK * num * num2).ToString();
			break;
		}
		}
	}

	public string PDLPHLNCOMJ(string PMDPPGNJAFE)
	{
		if (string.IsNullOrEmpty(PMDPPGNJAFE))
		{
			return string.Empty;
		}
		int num = PMDPPGNJAFE.IndexOf('{');
		if (num == -1)
		{
			return PMDPPGNJAFE;
		}
		int num2 = PMDPPGNJAFE.LastIndexOf('}');
		if (num2 == -1)
		{
			return PMDPPGNJAFE;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(PMDPPGNJAFE);
		QuestParameters jCICKLIMBEF = ListSF.ELEBLBJKDBI().BNMLDPNCMLB();
		ConditionExtension.CompareResult lNIDLHOIHIM = new ConditionExtension.CompareResult();
		QuestCondition kKDGLNECFHA = new QuestCondition();
		kKDGLNECFHA.LIMHBJBEEIA(jCICKLIMBEF);
		while (num <= num2)
		{
			string newValue = string.Empty;
			if (PMDPPGNJAFE[num] == '{')
			{
				int num3 = PMDPPGNJAFE.IndexOf('}', num);
				string text = PMDPPGNJAFE.Substring(num + 1, num3 - num - 1);
				if (!string.IsNullOrEmpty(text))
				{
					FunctionExtension oPIFBDJNMKD = new FunctionExtension();
					oPIFBDJNMKD.DMPCFMACDJM(OKPFNCJFLDL);
					oPIFBDJNMKD.PBPBNENGLPA(HJFEFJIEINN);
					foreach (KeyValuePair<string, string> item in MJGGAHJDPMN.IBLHIAHECLK)
					{
						oPIFBDJNMKD.SetVariable(item.Key, item.Value);
					}
					oPIFBDJNMKD.Parse(text);
					FunctionResult dEIHAOLOPLC = oPIFBDJNMKD.IBCPKBBAFNH();
					newValue = dEIHAOLOPLC.DCJLKCFKCOM;
				}
				stringBuilder.Replace(text, newValue);
				num = num3 + 1;
			}
			else
			{
				num++;
			}
		}
		return stringBuilder.ToString();
	}
}
