using System.Collections.Generic;
using System.Xml;

public class QuestActionGivePerk : QuestAction
{
	private string BJHBHKKHENM;

	private string KLIDPJCCAME;

	private XmlDocument _node = new XmlDocument();

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		BJHBHKKHENM = EPKLCPOEELO.Attributes["ApplyTo"].CIPOICEEIBK(string.Empty);
		KLIDPJCCAME = EPKLCPOEELO.Attributes["Item"].CIPOICEEIBK(string.Empty);
		CopyNodeToNode(EPKLCPOEELO, _node);
	}

	private void CopyNodeToNode(XmlNode node, XmlNode EAFDAPNLMJD)
	{
		foreach (XmlNode childNode in node.ChildNodes)
		{
			EAFDAPNLMJD.LCOLFMJJDJE(childNode);
		}
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		try
		{
			ConditionExtension.CompareResult lNIDLHOIHIM = new ConditionExtension.CompareResult();
			QuestCondition kKDGLNECFHA = new QuestCondition();
			kKDGLNECFHA.LIMHBJBEEIA(GFIHPBCEEOB);
			lNIDLHOIHIM.Clear();
			kKDGLNECFHA.MCPIOGALBMK(BJHBHKKHENM, lNIDLHOIHIM);
			string text = lNIDLHOIHIM.ToString();
			if (text == "Player")
			{
				OCFGEOPIBKB(lNIDLHOIHIM, kKDGLNECFHA);
			}
			else if (text == "Item")
			{
				NLKFMJFJPLI(lNIDLHOIHIM, kKDGLNECFHA);
			}
		}
		catch (System.Exception exception)
		{
			// A malformed/already-maxed imported reward must not strand the global
			// quest queue.  That previously made unrelated map controls, including
			// Eclipse, appear completely unresponsive for the rest of the session.
			Roster roster = ListSF.CCDKHLAMKKO();
			if (roster != null)
			{
				roster.CLODDOOGDBB = false;
			}
			UnityEngine.Debug.LogWarning("[Quest] GivePerk skipped invalid imported reward: " + exception.Message);
		}
		finally
		{
			OGIJONMKABB();
		}
	}

	private void OCFGEOPIBKB(ConditionExtension.CompareResult DCJLKCFKCOM, QuestCondition IOFGGOCEIAM)
	{
		XmlDocument xmlDocument = new XmlDocument();
		CopyNodeToNode(_node, xmlDocument);
		FBFHOPPKPEB(xmlDocument, DCJLKCFKCOM, IOFGGOCEIAM);
		GameUtils.FDEJIIDIPBI.MHAEANEADOO(xmlDocument, false);
		EOCILJEGNFL(xmlDocument);
	}

	private void FBFHOPPKPEB(XmlNode node, ConditionExtension.CompareResult DCJLKCFKCOM, QuestCondition IOFGGOCEIAM)
	{
		foreach (XmlNode childNode in node.ChildNodes)
		{
			foreach (XmlAttribute attribute in childNode.Attributes)
			{
				DCJLKCFKCOM.Clear();
				IOFGGOCEIAM.MCPIOGALBMK(attribute.Value, DCJLKCFKCOM);
				attribute.Value = DCJLKCFKCOM.ToString();
			}
			FBFHOPPKPEB(childNode, DCJLKCFKCOM, IOFGGOCEIAM);
		}
	}

	private void EOCILJEGNFL(XmlNode node)
	{
		foreach (XmlNode childNode in node.ChildNodes)
		{
			string text = childNode.Attributes["Name"].CIPOICEEIBK(string.Empty);
			int gCAPLEJMMPM = childNode.Attributes["Level"].ParseInt();
			int aKKLOMFOLNO = childNode.Attributes["UpgradeLevel"].ParseInt();
			PerkInfoItem aCONCDFDNJH = LALIAKEJOON(text);
			if (aCONCDFDNJH != null)
			{
				RosterPerkInfo gAKDPKLHHFF = new RosterPerkInfo();
				gAKDPKLHHFF.Name = text;
				gAKDPKLHHFF.Level = gCAPLEJMMPM;
				gAKDPKLHHFF.AKKLOMFOLNO = aKKLOMFOLNO;
				AHLHDKNCPIC(childNode, gAKDPKLHHFF);
				ListSF.CCDKHLAMKKO().JLBDOBLHHAF().HGOLHMJEPIA(gAKDPKLHHFF);
			}
		}
	}

	private void AHLHDKNCPIC(XmlNode node, RosterPerkInfo EMBBNNBFODN)
	{
		XmlNode xmlNode = node["Set"];
		if (xmlNode == null)
		{
			return;
		}
		foreach (XmlAttribute attribute in xmlNode.Attributes)
		{
			EMBBNNBFODN.Pairs[attribute.Name] = attribute.Value;
		}
	}

	private PerkInfoItem LALIAKEJOON(string name)
	{
		PerkInfoItem aCONCDFDNJH = GameUtils.FDEJIIDIPBI.MNMFPCBNLJI(name);
		if (aCONCDFDNJH == null)
		{
			aCONCDFDNJH = GameUtils.FDEJIIDIPBI.LAAJJBEEDKL(name);
		}
		if (aCONCDFDNJH == null)
		{
			aCONCDFDNJH = GameUtils.FDEJIIDIPBI.ABAGJKMKCBA(name);
		}
		return aCONCDFDNJH;
	}

	private void NLKFMJFJPLI(ConditionExtension.CompareResult DCJLKCFKCOM, QuestCondition IOFGGOCEIAM)
	{
		XmlDocument xmlDocument = new XmlDocument();
		CopyNodeToNode(_node, xmlDocument);
		FBFHOPPKPEB(xmlDocument, DCJLKCFKCOM, IOFGGOCEIAM);
		List<PerkStruct> hALHGEGADKA = JAIBCJIKACF(xmlDocument);
		DCJLKCFKCOM.Clear();
		IOFGGOCEIAM.MCPIOGALBMK(KLIDPJCCAME, DCJLKCFKCOM);
		string gOHIIMFFFJI = DCJLKCFKCOM.ToString();
		UserItem dKCHDHMLKHN = ListSF.CCDKHLAMKKO().KHCNHPCPFII().CMGOCLGHNLH(gOHIIMFFFJI);
		if (dKCHDHMLKHN != null)
		{
			dKCHDHMLKHN.CFIDFHLBKGP(hALHGEGADKA, dKCHDHMLKHN.DHNNCAEEMLL(), ListSF.CCDKHLAMKKO().PINDEKDNCNL());
		}
	}

	private List<PerkStruct> JAIBCJIKACF(XmlNode node)
	{
		List<PerkStruct> list = new List<PerkStruct>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			PerkStruct item = new PerkStruct(childNode);
			list.Add(item);
		}
		return list;
	}
}
