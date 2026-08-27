using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public static class MovesParser
{
	private const string PNMKCPIEPCO = "/moves.xml";

	private static Dictionary<string, XmlNode> _TemplateTemp;

	public static void Parse(string path, List<InfoAnimation> DPPDBCBFHIL, Dictionary<string, TemplateAnimation> CBNKICJENCB, List<Trick> IAGDAAPCDNI, List<Trigger> CMHFKBKKKOK, bool OOJAEKEOEFJ)
	{
		MovesMaps.Init();
		XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(path + "/moves.xml", string.Empty);
		XmlNode aFHNINCKJEE = xmlDocument["Movesxml"]["Templates"];
		XmlNode iLCCDINCICK = xmlDocument["Movesxml"]["Moves"];
		XmlNode hKPPBKPJOEO = xmlDocument["Movesxml"]["Triggers"];
		AKGCKOGKJBD(aFHNINCKJEE, CBNKICJENCB);
		MNCBOOGMKGB(iLCCDINCICK, CBNKICJENCB, DPPDBCBFHIL, IAGDAAPCDNI);
		KOKCNPLBFAG(hKPPBKPJOEO, CMHFKBKKKOK);
		_TemplateTemp.Clear();
		_TemplateTemp = null;
	}

	private static void SetMoveTemplate(XmlNode KIKPDADFBDM, XmlNode LFKJDMIPCEA, List<XmlNode> HKIBBEPJGCH)
	{
		XmlAttribute xmlAttribute = LFKJDMIPCEA.Attributes["Template"];
		if (xmlAttribute == null)
		{
			return;
		}
		bool flag = false;
		string[] array = xmlAttribute.Value.Split('|');
		for (int i = 0; i < array.Length; i++)
		{
			flag = false;
			for (int j = 0; j < HKIBBEPJGCH.Count; j++)
			{
				if (array[i] == HKIBBEPJGCH[j].Attributes["Name"].Value)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				XmlNode value = null;
				if (_TemplateTemp.TryGetValue(array[i], out value))
				{
					HKIBBEPJGCH.Add(value);
					AddAttributes(KIKPDADFBDM, value);
					SetMoveTemplate(KIKPDADFBDM, value, HKIBBEPJGCH);
				}
				else
				{
					LLLOJBFMONN.Write("Move template don't find: " + array[i]);
				}
			}
		}
	}

	private static void AddAttributes(XmlNode FFFLNOBCBGL, XmlNode PEPPBJKBBOG)
	{
		foreach (XmlAttribute attribute in PEPPBJKBBOG.Attributes)
		{
			string name = attribute.Name;
			XmlAttribute xmlAttribute2 = FFFLNOBCBGL.Attributes[name];
			if (xmlAttribute2 == null)
			{
				FFFLNOBCBGL.LCOLFMJJDJE(attribute);
			}
		}
	}

	private static List<InfoAnimation> MNCBOOGMKGB(XmlNode nodes, Dictionary<string, TemplateAnimation> JIGEFEPNCIN, List<InfoAnimation> OEMALIFPGPO, List<Trick> IAGDAAPCDNI)
	{
		List<global::Pair<InfoAnimation, string>> list = new List<global::Pair<InfoAnimation, string>>();
		list.Capacity = 100;
		List<XmlNode> list2 = new List<XmlNode>();
		foreach (XmlNode childNode in nodes.ChildNodes)
		{
			list2.Clear();
			SetMoveTemplate(childNode, childNode, list2);
			InfoAnimation pJAHIOELGGD = new InfoAnimation();
			pJAHIOELGGD.Name = childNode.Attributes["Name"].CIPOICEEIBK(string.Empty);
			pJAHIOELGGD.Id = childNode.Attributes["ID"].ParseInt();
			pJAHIOELGGD.FileName = childNode.Attributes["FileName"].CIPOICEEIBK(string.Empty);
			pJAHIOELGGD.MNHGBPOIHKG = childNode.Attributes["MidFrames"].ParseInt();
			pJAHIOELGGD.GOBJCKFGIPA = childNode.Attributes["FirstFrame"].ParseInt();
			pJAHIOELGGD.LHHAGECFIOL = childNode.Attributes["EndFrame"].ParseInt();
			pJAHIOELGGD.Priority = childNode.Attributes["Priority"].ParseInt();
			pJAHIOELGGD.PFELBJBNEEK(childNode.Attributes["NoMagicRecharge"].ParseBool());
			pJAHIOELGGD.Type = InfoAnimation.MGHNBEPCKIF.AnimationNone;
			pJAHIOELGGD.OFADIIPBEKI = InfoAnimation.EOJCAKOHCHA.TutorialNone;
			pJAHIOELGGD.HFBOLCPHMBB = childNode.Attributes["NoWallRepulsion"].ParseBool();
			pJAHIOELGGD.DCLGDANCGHC = childNode.Attributes["StyleFactor"].ParseFloat(1f);
			pJAHIOELGGD.FBKGDALBNDJ = childNode.Attributes["Physics"].ParseBool();
			pJAHIOELGGD.HECHJGBMHIC = childNode.Attributes["EndsStage"].ParseBool();
			pJAHIOELGGD.LLELLFKJKGE(childNode.Attributes["Looped"].ParseBool());
			pJAHIOELGGD.JEADCBJMEGC = childNode.Attributes["NoInterpolationFrames"].ParseBool();
			pJAHIOELGGD.ALFPDPEEJFO = childNode.Attributes["AlignOnParentWallCollision"].ParseBool();
			pJAHIOELGGD.ABEGFBOKPOI();
			pJAHIOELGGD.AddTemplateName(pJAHIOELGGD.Name);
			XmlAttribute xmlAttribute = childNode.Attributes["MirrorNode"];
			if (xmlAttribute != null)
			{
				pJAHIOELGGD.ECCLELFHNHE().HHACPELEPAK(xmlAttribute.CIPOICEEIBK(string.Empty));
			}
			xmlAttribute = childNode.Attributes["CameraCOMAlignStage"];
			if (xmlAttribute != null)
			{
				StageType.FDBBPEGEGMK bAINMLLIKOL = StageType.GetStageByName(xmlAttribute.CIPOICEEIBK(string.Empty));
				pJAHIOELGGD.POOOFPBAJDM(bAINMLLIKOL);
			}
			pJAHIOELGGD.IBMFCIFKGOO(childNode.Attributes["TacticWeapon"].CIPOICEEIBK(string.Empty));
			string text = childNode.Attributes["TacticEquivalent"].CIPOICEEIBK(string.Empty);
			if (!string.IsNullOrEmpty(text))
			{
				list.Add(new global::Pair<InfoAnimation, string>(pJAHIOELGGD, text));
			}
			pJAHIOELGGD.Priority = childNode.Attributes["Priority"].ParseInt();
			xmlAttribute = childNode.Attributes["Type"];
			if (xmlAttribute != null)
			{
				string text2 = xmlAttribute.CIPOICEEIBK(string.Empty);
				if (text2 == "MOVE")
				{
					pJAHIOELGGD.Type = InfoAnimation.MGHNBEPCKIF.AnimationMove;
				}
				else if (text2 == "ATTACK")
				{
					pJAHIOELGGD.Type = InfoAnimation.MGHNBEPCKIF.AnimationAttack;
				}
			}
			xmlAttribute = childNode.Attributes["Delay"];
			if (xmlAttribute != null)
			{
				pJAHIOELGGD.AddDelay(xmlAttribute.ParseInt());
			}
			NEONAMKOFPN(list2, pJAHIOELGGD, JIGEFEPNCIN);
			OOCDLNMDLKE(childNode, list2, pJAHIOELGGD);
			JGLOLDJFFKC(pJAHIOELGGD, childNode, list2);
			GHMBNFDNMCH(pJAHIOELGGD, childNode["Rotation"]);
			pJAHIOELGGD.Init();
			OEMALIFPGPO.Add(pJAHIOELGGD);
			XmlNode xmlNode2 = childNode["Profile"];
			if (xmlNode2 != null && xmlNode2.Attributes["Show"].ParseBool())
			{
				pJAHIOELGGD.Rank = xmlNode2.Attributes["Rank"].ParseInt();
				Trick item = new Trick(xmlNode2, pJAHIOELGGD);
				IAGDAAPCDNI.Add(item);
			}
		}
		foreach (global::Pair<InfoAnimation, string> item2 in list)
		{
			InfoAnimation lLHEDBIEHAA = item2.First;
			string nFNBFHCDEGG = item2.Second;
			InfoAnimation pJAHIOELGGD2 = null;
			foreach (InfoAnimation item3 in OEMALIFPGPO)
			{
				if (item3.Name == nFNBFHCDEGG)
				{
					pJAHIOELGGD2 = item3;
					break;
				}
			}
			if (pJAHIOELGGD2 != null)
			{
				lLHEDBIEHAA.set_TacticEquivalent(pJAHIOELGGD2);
				continue;
			}
			LLLOJBFMONN.Error("{0} tactic equivalent {1} not found", lLHEDBIEHAA.Name, nFNBFHCDEGG);
		}
		return OEMALIFPGPO;
	}

	private static void KOKCNPLBFAG(XmlNode node, List<Trigger> OEMALIFPGPO)
	{
		foreach (XmlNode childNode in node.ChildNodes)
		{
			Trigger cPFMGFAFAFB = new Trigger();
			cPFMGFAFAFB.Name = childNode.Attributes["Name"].CIPOICEEIBK(string.Empty);
			FDHENMIHJBC(childNode, cPFMGFAFAFB);
			cPFMGFAFAFB.Init();
			OEMALIFPGPO.Add(cPFMGFAFAFB);
		}
	}

	private static void AKGCKOGKJBD(XmlNode AFHNINCKJEE, Dictionary<string, TemplateAnimation> JIGEFEPNCIN)
	{
		JIGEFEPNCIN.Clear();
		_TemplateTemp = new Dictionary<string, XmlNode>();
		if (AFHNINCKJEE == null)
		{
			return;
		}
		foreach (XmlNode childNode in AFHNINCKJEE.ChildNodes)
		{
			if (childNode.Name == "Template")
			{
				TemplateAnimation bHIDAHDCPHM = new TemplateAnimation(childNode);
				JIGEFEPNCIN.Add(bHIDAHDCPHM.get_Name(), bHIDAHDCPHM);
				_TemplateTemp.Add(bHIDAHDCPHM.get_Name(), childNode);
			}
		}
	}

	private static void NEONAMKOFPN(List<XmlNode> MMLFAGGGINF, InfoAnimation INOIBNOMNEO, Dictionary<string, TemplateAnimation> JIGEFEPNCIN)
	{
		string text = null;
		for (int i = 0; i < MMLFAGGGINF.Count; i++)
		{
			text = MMLFAGGGINF[i].Attributes["Name"].Value;
			if (JIGEFEPNCIN.ContainsKey(text))
			{
				JIGEFEPNCIN[text].MBJCDIDIBDJ(INOIBNOMNEO);
			}
		}
		if (!JIGEFEPNCIN.ContainsKey(INOIBNOMNEO.Name))
		{
			TemplateAnimation bHIDAHDCPHM = new TemplateAnimation(INOIBNOMNEO);
			JIGEFEPNCIN.Add(bHIDAHDCPHM.get_Name(), bHIDAHDCPHM);
		}
	}

	private static bool PPDEKBCNBDM(string name, List<TemplateAnimation> OEMALIFPGPO)
	{
		foreach (TemplateAnimation item in OEMALIFPGPO)
		{
			if (item.get_Name() == name)
			{
				return true;
			}
		}
		return false;
	}

	private static TemplateAnimation CNFBCBDPKCI(string name, List<TemplateAnimation> JIGEFEPNCIN)
	{
		int i = 0;
		for (int count = JIGEFEPNCIN.Count; i < count; i++)
		{
			if (JIGEFEPNCIN[i].get_Name() == name)
			{
				return JIGEFEPNCIN[i];
			}
		}
		return null;
	}

	private static void OOCDLNMDLKE(XmlNode node, List<XmlNode> MMLFAGGGINF, InfoAnimation DBOLBEOCEME)
	{
		InfoAnimation.MoveInside cNPOIHDPBPB = new InfoAnimation.MoveInside();
		cNPOIHDPBPB.AJCMBMJGJEG = JBNILFIHMMK(node, MMLFAGGGINF);
		cNPOIHDPBPB.JIFAHHGNPFH = EPCNPJEALBH("Conditions", node, MMLFAGGGINF);
		cNPOIHDPBPB.HIFPHBNGIPO = EPCNPJEALBH("Locks", node, MMLFAGGGINF);
		cNPOIHDPBPB.Intervals = MAFNIEICKGN(node, MMLFAGGGINF);
		cNPOIHDPBPB.DJBAIAKOIHM = EFJHONIPBOC(node, MMLFAGGGINF);
		cNPOIHDPBPB.NIDNJFOGBFO = OJJDCDGJAHO(node, MMLFAGGGINF);
		XmlNode xmlNode = node["Transitions"];
		if (xmlNode != null)
		{
			cNPOIHDPBPB.ELFBPNOBDKC = KFPHEGLHEMM(xmlNode);
		}
		xmlNode = node["Shop"];
		if (xmlNode != null)
		{
			cNPOIHDPBPB.DFLNENOIMPO = ABAFCBFPAON(xmlNode);
		}
		NAAEEHHHOFG(cNPOIHDPBPB, node, MMLFAGGGINF);
		FPOCAAILLAM(cNPOIHDPBPB, node, MMLFAGGGINF);
		DBOLBEOCEME.NHAEHLFMPNK(cNPOIHDPBPB);
	}

	private static void FDHENMIHJBC(XmlNode node, Trigger CPBHKJFPFJB)
	{
		Trigger.TriggerInside pCFAHEOAJLB = new Trigger.TriggerInside();
		pCFAHEOAJLB.AJCMBMJGJEG = JBNILFIHMMK(node);
		pCFAHEOAJLB.JIFAHHGNPFH = EPCNPJEALBH("Conditions", node);
		pCFAHEOAJLB.HIFPHBNGIPO = EPCNPJEALBH("Locks", node);
		pCFAHEOAJLB.DJBAIAKOIHM = EFJHONIPBOC(node);
		CPBHKJFPFJB.NHAEHLFMPNK(pCFAHEOAJLB);
	}

	private static List<EventAnimation> JBNILFIHMMK(XmlNode nodes, List<XmlNode> MMLFAGGGINF = null)
	{
		List<EventAnimation> list = new List<EventAnimation>();
		JBNILFIHMMK(nodes["Events"], list);
		if (MMLFAGGGINF != null)
		{
			for (int i = 0; i < MMLFAGGGINF.Count; i++)
			{
				JBNILFIHMMK(MMLFAGGGINF[i]["Events"], list);
			}
		}
		return list;
	}

	private static void JBNILFIHMMK(XmlNode MEEAKLDGLDF, List<EventAnimation> FFFLNOBCBGL)
	{
		if (MEEAKLDGLDF == null)
		{
			return;
		}
		FFFLNOBCBGL.Capacity = FFFLNOBCBGL.Count + MEEAKLDGLDF.ChildNodes.Count;
		EventAnimation nFCCFMOMPHG = null;
		foreach (XmlNode childNode in MEEAKLDGLDF.ChildNodes)
		{
			nFCCFMOMPHG = EventParser.Create(childNode);
			if (nFCCFMOMPHG != null)
			{
				nFCCFMOMPHG.Init(childNode);
				FFFLNOBCBGL.Add(nFCCFMOMPHG);
			}
		}
	}

	private static List<ConditionAnimation> EPCNPJEALBH(string IMGCANJHPND, XmlNode nodes, List<XmlNode> MMLFAGGGINF = null)
	{
		List<ConditionAnimation> list = new List<ConditionAnimation>();
		EPCNPJEALBH(nodes[IMGCANJHPND], list);
		if (MMLFAGGGINF != null)
		{
			for (int i = 0; i < MMLFAGGGINF.Count; i++)
			{
				EPCNPJEALBH(MMLFAGGGINF[i][IMGCANJHPND], list);
			}
		}
		return list;
	}

	private static void EPCNPJEALBH(XmlNode MEEAKLDGLDF, List<ConditionAnimation> FFFLNOBCBGL)
	{
		if (MEEAKLDGLDF == null)
		{
			return;
		}
		FFFLNOBCBGL.Capacity = FFFLNOBCBGL.Count + MEEAKLDGLDF.ChildNodes.Count;
		foreach (XmlNode childNode in MEEAKLDGLDF.ChildNodes)
		{
			ConditionAnimation iIDOLPHMOGA = ConditionsParser.Create(childNode);
			if (iIDOLPHMOGA != null)
			{
				iIDOLPHMOGA.Parse(childNode);
				FFFLNOBCBGL.Add(iIDOLPHMOGA);
			}
		}
	}

	private static List<ConditionAnimation> OJJDCDGJAHO(XmlNode node, List<XmlNode> MMLFAGGGINF)
	{
		List<ConditionAnimation> list = new List<ConditionAnimation>();
		XmlNode xmlNode = node["Tactics"];
		if (xmlNode != null)
		{
			EPCNPJEALBH(xmlNode["Conditions"], list);
		}
		for (int i = 0; i < MMLFAGGGINF.Count; i++)
		{
			xmlNode = node["Tactics"];
			if (xmlNode != null)
			{
				EPCNPJEALBH(xmlNode["Conditions"], list);
			}
		}
		return list;
	}

	private static InfoAnimation.MoveInside.ShopAnimation ABAFCBFPAON(XmlNode nodes)
	{
		InfoAnimation.MoveInside.ShopAnimation oNLLFHPLBFL = new InfoAnimation.MoveInside.ShopAnimation();
		oNLLFHPLBFL.FGMBMNFANHF = nodes["RunOnStart"] != null;
		oNLLFHPLBFL.AnimationName = nodes["NextAnimation"].Attributes["Name"].CIPOICEEIBK(string.Empty);
		oNLLFHPLBFL.IsExists = true;
		return oNLLFHPLBFL;
	}

	private static List<TransitionAnimation> KFPHEGLHEMM(XmlNode nodes)
	{
		List<TransitionAnimation> list = new List<TransitionAnimation>(nodes.ChildNodes.Count);
		foreach (XmlNode childNode in nodes.ChildNodes)
		{
			TransitionAnimation nIHOEJAKIJK = new TransitionAnimation();
			nIHOEJAKIJK.AJKANHBOADL(EPCNPJEALBH("Conditions", childNode));
			if (childNode.Attributes["FirstFrame"] != null)
			{
				nIHOEJAKIJK.IsFrameShift = false;
				nIHOEJAKIJK.FrameShift = childNode.Attributes["FirstFrame"].ParseInt();
			}
			if (childNode.Attributes["FrameShift"] != null)
			{
				nIHOEJAKIJK.IsFrameShift = true;
				nIHOEJAKIJK.FrameShift = childNode.Attributes["FrameShift"].ParseInt();
			}
			list.Add(nIHOEJAKIJK);
		}
		return list;
	}

	private static List<IntervalAnimation> MAFNIEICKGN(XmlNode nodes, List<XmlNode> MMLFAGGGINF = null)
	{
		List<IntervalAnimation> list = new List<IntervalAnimation>();
		MAFNIEICKGN(nodes["Intervals"], list);
		if (MMLFAGGGINF != null)
		{
			for (int i = 0; i < MMLFAGGGINF.Count; i++)
			{
				MAFNIEICKGN(MMLFAGGGINF[i]["Intervals"], list);
			}
		}
		return list;
	}

	private static void MAFNIEICKGN(XmlNode MEEAKLDGLDF, List<IntervalAnimation> FFFLNOBCBGL)
	{
		if (MEEAKLDGLDF == null)
		{
			return;
		}
		FFFLNOBCBGL.Capacity = FFFLNOBCBGL.Count + MEEAKLDGLDF.ChildNodes.Count;
		foreach (XmlNode childNode in MEEAKLDGLDF.ChildNodes)
		{
			string lFLGCDNKNJI = childNode.Attributes["Type"].CIPOICEEIBK(string.Empty);
			IntervalAnimation.NGAJJDIEDGF nGAJJDIEDGF = IntervalAnimation.LAJMDAFFPJE(lFLGCDNKNJI);
			IntervalAnimation mNOIEOBBCMI = ((nGAJJDIEDGF != IntervalAnimation.NGAJJDIEDGF.INTERVAL_ATTACK) ? new IntervalAnimation(nGAJJDIEDGF) : new IntervalAttack());
			mNOIEOBBCMI.Parse(childNode);
			FFFLNOBCBGL.Add(mNOIEOBBCMI);
		}
	}

	private static List<ActionAnimation> EFJHONIPBOC(XmlNode nodes, List<XmlNode> MMLFAGGGINF = null)
	{
		List<ActionAnimation> list = new List<ActionAnimation>();
		EFJHONIPBOC(nodes["Actions"], list);
		if (MMLFAGGGINF != null)
		{
			for (int i = 0; i < MMLFAGGGINF.Count; i++)
			{
				EFJHONIPBOC(MMLFAGGGINF[i]["Actions"], list);
			}
		}
		return list;
	}

	private static void EFJHONIPBOC(XmlNode MEEAKLDGLDF, List<ActionAnimation> FFFLNOBCBGL)
	{
		if (MEEAKLDGLDF == null)
		{
			return;
		}
		FFFLNOBCBGL.Capacity = FFFLNOBCBGL.Count + MEEAKLDGLDF.ChildNodes.Count;
		foreach (XmlNode childNode in MEEAKLDGLDF.ChildNodes)
		{
			ActionAnimation gELPMIAIGDF = ActionsParser.Create(childNode);
			if (gELPMIAIGDF != null)
			{
				FFFLNOBCBGL.Add(gELPMIAIGDF);
			}
		}
	}

	private static void NAAEEHHHOFG(InfoAnimation.MoveInside ODACDCDONJE, XmlNode node, List<XmlNode> MMLFAGGGINF)
	{
		ODACDCDONJE.ILOEBFFAEAN.IsExists = false;
		XmlNode xmlNode = node["Align"];
		if (xmlNode != null)
		{
			ODACDCDONJE.ILOEBFFAEAN = PLMHJKGIHLB(xmlNode);
			ODACDCDONJE.ILOEBFFAEAN.IsExists = true;
			return;
		}
		for (int i = 0; i < MMLFAGGGINF.Count; i++)
		{
			xmlNode = MMLFAGGGINF[i]["Align"];
			if (xmlNode != null)
			{
				ODACDCDONJE.ILOEBFFAEAN = PLMHJKGIHLB(xmlNode);
				ODACDCDONJE.ILOEBFFAEAN.IsExists = true;
				break;
			}
		}
	}

	private static InfoAnimation.MovePivot PLMHJKGIHLB(XmlNode node)
	{
		InfoAnimation.MovePivot jKHNOAFIHKP = new InfoAnimation.MovePivot();
		XmlNode xmlNode = node["Pivot"];
		XmlNode xmlNode2 = node["Position"];
		string text = xmlNode.Attributes["Object"].CIPOICEEIBK(string.Empty);
		string text2 = xmlNode2.Attributes["Object"].CIPOICEEIBK(string.Empty);
		XmlAttribute cJBEMNNNHDM = xmlNode.Attributes["Player"];
		string lFLGCDNKNJI = cJBEMNNNHDM.CIPOICEEIBK("Me");
		XmlAttribute cJBEMNNNHDM2 = xmlNode2.Attributes["Player"];
		string lFLGCDNKNJI2 = cJBEMNNNHDM2.CIPOICEEIBK("Me");
		XmlAttribute xmlAttribute = node.Attributes["Axis"];
		jKHNOAFIHKP.HNDMMOGMOAN = (jKHNOAFIHKP.IMCDDINEFKC = (jKHNOAFIHKP.GHKGPDMMHHK = false));
		if (xmlAttribute == null)
		{
			jKHNOAFIHKP.HNDMMOGMOAN = true;
			jKHNOAFIHKP.IMCDDINEFKC = true;
			jKHNOAFIHKP.GHKGPDMMHHK = true;
		}
		else
		{
			string text3 = xmlAttribute.CIPOICEEIBK(string.Empty);
			string[] array = text3.Split('|');
			string[] array2 = array;
			foreach (string text4 in array2)
			{
				switch (text4)
				{
				case "X":
					jKHNOAFIHKP.HNDMMOGMOAN = true;
					continue;
				case "Y":
					jKHNOAFIHKP.IMCDDINEFKC = true;
					continue;
				case "Z":
					jKHNOAFIHKP.GHKGPDMMHHK = true;
					continue;
				}
				LLLOJBFMONN.Error("ERROR: alignParse - wrong axis \"{0}\" in \"{1}\"", text4, text3);
			}
		}
		XmlAttribute xmlAttribute2 = node.Attributes["ShiftModelNode"];
		if (xmlAttribute2 != null)
		{
			jKHNOAFIHKP.BONDKHGGCDD = xmlAttribute2.CIPOICEEIBK(string.Empty);
		}
		jKHNOAFIHKP.BAFGOANMBMI = ModelType.EHFNOBFLAHI(lFLGCDNKNJI);
		jKHNOAFIHKP.EDBLMNIEKBD = ModelType.EHFNOBFLAHI(lFLGCDNKNJI2);
		jKHNOAFIHKP.BLODCIGDJFK = xmlNode.Attributes["Part"].CIPOICEEIBK(string.Empty);
		jKHNOAFIHKP.PMILDGBBLMF = xmlNode2.Attributes["Part"].CIPOICEEIBK(string.Empty);
		jKHNOAFIHKP.LDNPHPGEOPJ.JPFALPBDBAP(xmlNode2.Attributes["ShiftX"].ParseFloat());
		jKHNOAFIHKP.LDNPHPGEOPJ.IBNFLLGPOLD(xmlNode2.Attributes["ShiftY"].ParseFloat());
		switch (text)
		{
		case "Nodes":
			jKHNOAFIHKP.CKBGFODEBAJ = InfoAnimation.DOLCEABGNGA.ObjectNodes;
			break;
		case "Animation":
			jKHNOAFIHKP.CKBGFODEBAJ = InfoAnimation.DOLCEABGNGA.ObjectAnimation;
			break;
		case "Wall":
			jKHNOAFIHKP.CKBGFODEBAJ = InfoAnimation.DOLCEABGNGA.ObjectWall;
			break;
		case "Pivot":
			jKHNOAFIHKP.CKBGFODEBAJ = InfoAnimation.DOLCEABGNGA.ObjectPivot;
			break;
		}
		switch (text2)
		{
		case "Nodes":
			jKHNOAFIHKP.HHPAGAOGGLP = InfoAnimation.DOLCEABGNGA.ObjectNodes;
			break;
		case "Animation":
			jKHNOAFIHKP.HHPAGAOGGLP = InfoAnimation.DOLCEABGNGA.ObjectAnimation;
			break;
		case "Wall":
			jKHNOAFIHKP.HHPAGAOGGLP = InfoAnimation.DOLCEABGNGA.ObjectWall;
			break;
		case "Pivot":
			jKHNOAFIHKP.HHPAGAOGGLP = InfoAnimation.DOLCEABGNGA.ObjectPivot;
			break;
		}
		return jKHNOAFIHKP;
	}

	private static void FPOCAAILLAM(InfoAnimation.MoveInside ODACDCDONJE, XmlNode node, List<XmlNode> MMLFAGGGINF)
	{
		ODACDCDONJE.IHJEKBAEIKK.IsExists = false;
		XmlNode xmlNode = node["SetDirection"];
		if (xmlNode != null)
		{
			ODACDCDONJE.IHJEKBAEIKK = JOLJIHDPADK(xmlNode);
			ODACDCDONJE.IHJEKBAEIKK.IsExists = true;
			return;
		}
		for (int i = 0; i < MMLFAGGGINF.Count; i++)
		{
			xmlNode = MMLFAGGGINF[i]["SetDirection"];
			if (xmlNode != null)
			{
				ODACDCDONJE.IHJEKBAEIKK = JOLJIHDPADK(xmlNode);
				ODACDCDONJE.IHJEKBAEIKK.IsExists = true;
				break;
			}
		}
	}

	public static InfoAnimation.MoveInside.Direction JOLJIHDPADK(XmlNode node)
	{
		InfoAnimation.MoveInside.Direction nMLHMNAEJDH = new InfoAnimation.MoveInside.Direction();
		XmlNode hKPPBKPJOEO = node["From"];
		nMLHMNAEJDH.CLCFLPDNBNL.Create(hKPPBKPJOEO);
		hKPPBKPJOEO = node["To"];
		nMLHMNAEJDH.KAEAKHIEIHH.Create(hKPPBKPJOEO);
		hKPPBKPJOEO = node["Impulse"];
		nMLHMNAEJDH.IIIDIKABLOJ = InfoAnimation.MoveInside.Direction.BBAGKNMNONO(hKPPBKPJOEO);
		return nMLHMNAEJDH;
	}

	public static void CHILAIJNEHG()
	{
		MovesMaps.Clear();
	}

	private static void JGLOLDJFFKC(InfoAnimation DBOLBEOCEME, XmlNode MEEAKLDGLDF, List<XmlNode> MMLFAGGGINF)
	{
		XmlNode xmlNode = MEEAKLDGLDF["Velocity"];
		if (xmlNode == null)
		{
			for (int i = 0; i < MMLFAGGGINF.Count; i++)
			{
				xmlNode = MEEAKLDGLDF["Velocity"];
				if (xmlNode != null)
				{
					break;
				}
			}
		}
		Vector3 bEHOPOPCJGB = new Vector3(0f, 0f, 0f);
		Vector3 bEHOPOPCJGB2 = new Vector3(0f, 0f, 0f);
		if (xmlNode != null)
		{
			bEHOPOPCJGB.x = xmlNode.Attributes["X"].ParseFloat();
			bEHOPOPCJGB.y = xmlNode.Attributes["Y"].ParseFloat();
			bEHOPOPCJGB.z = xmlNode.Attributes["Z"].ParseFloat();
			bEHOPOPCJGB2.x = xmlNode.Attributes["Ax"].ParseFloat();
			bEHOPOPCJGB2.y = xmlNode.Attributes["Ay"].ParseFloat();
			bEHOPOPCJGB2.z = xmlNode.Attributes["Az"].ParseFloat();
			DBOLBEOCEME.NFMLONEIJEJ(xmlNode.Attributes["SaveVelocity"].ParseBool());
		}
		DBOLBEOCEME.DIGCECPPHOH(Vector3f.op_Implicit(bEHOPOPCJGB));
		DBOLBEOCEME.PICBLJDLDDN(Vector3f.op_Implicit(bEHOPOPCJGB2));
	}

	private static void GHMBNFDNMCH(InfoAnimation DBOLBEOCEME, XmlNode node)
	{
		float bAINMLLIKOL = 0f;
		if (node != null)
		{
			bAINMLLIKOL = node.Attributes["Angle"].ParseFloat();
			XmlNode xmlNode = node["Position"];
			if (xmlNode != null)
			{
				DistancePoint bAINMLLIKOL2 = new DistancePoint(xmlNode);
				DBOLBEOCEME.HGJPLKKCKHM(bAINMLLIKOL2);
			}
		}
		DBOLBEOCEME.set_RotationAngle(bAINMLLIKOL);
	}
}
