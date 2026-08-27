using System.Collections.Generic;
using System.Xml;

public class UserPerks
{
	private XmlNode _node;

	private XmlNode BJJELGMJHJM;

	private XmlNode HFCPHEDFGEN;

	private List<RosterPerk> GBMLFKHHLCC = new List<RosterPerk>();

	public PerkHistory GIAEMMLABDL = new PerkHistory();

	protected ModelParameters HEGIABHIPHA;

	public List<RosterPerk> AOMHAONDFIP
	{
		get
		{
			return KEHFPLBNDHI();
		}
	}

	public int JNDHGMFINGH
	{
		get
		{
			return OPPFMFKAOIG();
		}
	}

	public int IFNOOAECKHD
	{
		get
		{
			return IBAOKPECDLF();
		}
	}

	public int MMFJLIFFJIP
	{
		get
		{
			return ICIAGDCEMEM();
		}
	}

	public UserPerks(ModelParameters JCICKLIMBEF)
	{
		HEGIABHIPHA = JCICKLIMBEF;
	}

	public List<RosterPerk> KEHFPLBNDHI()
	{
		return GBMLFKHHLCC;
	}

	public int OPPFMFKAOIG()
	{
		int num = ListSF.CCDKHLAMKKO().PINDEKDNCNL() - GIAEMMLABDL.JOGBKOJCINM.Count - 1;
		int num2 = PerkTree.GBPBIPFIOJH().KBIOIHPNLIM();
		int count = GIAEMMLABDL.JOGBKOJCINM.Count;
		return num2 - count;
	}

	public void Parse(XmlNode node)
	{
		if (node == null)
		{
			return;
		}
		_node = node;
		HFCPHEDFGEN = node["PerkHistory"];
		GIAEMMLABDL.Parse(HFCPHEDFGEN);
		BJJELGMJHJM = node["Perks"];
		if (BJJELGMJHJM == null)
		{
			return;
		}
		GameUtils.FDEJIIDIPBI.MHAEANEADOO(BJJELGMJHJM);
		foreach (XmlNode childNode in BJJELGMJHJM.ChildNodes)
		{
			DDBGEFPKAPN(new RosterPerk(childNode));
		}
	}

	public RosterPerk HGOLHMJEPIA(RosterPerkInfo AEFFHJGMNFI)
	{
		foreach (RosterPerk item in GBMLFKHHLCC)
		{
			bool flag = item.get_Name().Equals(AEFFHJGMNFI.Name);
			if (flag)
			{
				int aKKLOMFOLNO = AEFFHJGMNFI.AKKLOMFOLNO;
				if (aKKLOMFOLNO > 0)
				{
					item.FMMDLMGHPIB(aKKLOMFOLNO);
				}
				item.AppendNodeChild(AEFFHJGMNFI.Pairs);
				item.NOLDHAFMOLF(null);
				DDBGEFPKAPN(item, AEFFHJGMNFI);
				return item;
			}
			bool flag2 = string.IsNullOrEmpty(AEFFHJGMNFI.Name);
			if (flag || flag2)
			{
				return item;
			}
		}
		string text = "Perks";
		string name = "Perk";
		XmlNode xmlNode = _node[text];
		if (xmlNode == null)
		{
			xmlNode = _node.ACBPMPMPKJJ(text);
		}
		XmlNode newChild = _node.OwnerDocument.CreateNode(XmlNodeType.Element, name, null);
		newChild = xmlNode.PrependChild(newChild);
		RosterPerk hOGDBKBFFDJ = new RosterPerk(newChild);
		hOGDBKBFFDJ.DLDMOHEGENM(AEFFHJGMNFI.Level);
		hOGDBKBFFDJ.set_Name(AEFFHJGMNFI.Name);
		int aKKLOMFOLNO2 = AEFFHJGMNFI.AKKLOMFOLNO;
		if (aKKLOMFOLNO2 > 0)
		{
			hOGDBKBFFDJ.FMMDLMGHPIB(aKKLOMFOLNO2);
		}
		hOGDBKBFFDJ.AppendNodeChild(AEFFHJGMNFI.Pairs);
		DDBGEFPKAPN(hOGDBKBFFDJ, AEFFHJGMNFI);
		return hOGDBKBFFDJ;
	}

	public RosterPerk HGOLHMJEPIA(PerkInfoItem AEFFHJGMNFI)
	{
		XmlNode hKPPBKPJOEO = (_node["Perks"] ?? _node.ACBPMPMPKJJ("Perks")).ACBPMPMPKJJ("Perk");
		RosterPerk hOGDBKBFFDJ = new RosterPerk(hKPPBKPJOEO);
		hOGDBKBFFDJ.set_Name(AEFFHJGMNFI.Name);
		hOGDBKBFFDJ.DLDMOHEGENM(AEFFHJGMNFI.Level);
		hOGDBKBFFDJ.FMMDLMGHPIB(AEFFHJGMNFI.AKKLOMFOLNO);
		return hOGDBKBFFDJ;
	}

	public RosterPerk HGOLHMJEPIA(ProfilePerk AEFFHJGMNFI)
	{
		foreach (RosterPerk item in GBMLFKHHLCC)
		{
			bool flag = item.get_Name() == AEFFHJGMNFI.KAMBOKLFBEE();
			bool flag2 = AEFFHJGMNFI.get_Type() == ProfilePerk.JHDKDOPHGOO.TYPE_UPGRADE;
			bool flag3 = AEFFHJGMNFI.KAMBOKLFBEE() == string.Empty;
			if (flag && flag2)
			{
				int num = AEFFHJGMNFI.LMGGMMFEODJ();
				if (num > 0)
				{
					item.FMMDLMGHPIB(num);
				}
				item.NOLDHAFMOLF(null);
				DDBGEFPKAPN(item, AEFFHJGMNFI);
				return item;
			}
			if (flag || flag3)
			{
				return item;
			}
		}
		string text = "Perks";
		string jLEKBBJBLOE = "Perk";
		XmlNode mEEAKLDGLDF = ((_node[text] == null) ? _node.ACBPMPMPKJJ(text) : _node[text]);
		XmlNode hKPPBKPJOEO = mEEAKLDGLDF.ACBPMPMPKJJ(jLEKBBJBLOE);
		RosterPerk hOGDBKBFFDJ = new RosterPerk(hKPPBKPJOEO);
		hOGDBKBFFDJ.DLDMOHEGENM(AEFFHJGMNFI.PINDEKDNCNL());
		hOGDBKBFFDJ.set_Name(AEFFHJGMNFI.KAMBOKLFBEE());
		int num2 = AEFFHJGMNFI.LMGGMMFEODJ();
		if (num2 > 0)
		{
			hOGDBKBFFDJ.FMMDLMGHPIB(num2);
		}
		DDBGEFPKAPN(hOGDBKBFFDJ, AEFFHJGMNFI);
		return hOGDBKBFFDJ;
	}

	public void PBPAOBKIMKK(PerkHistory.Perk AEFFHJGMNFI)
	{
		if (AEFFHJGMNFI != null)
		{
			LLLOJBFMONN.Write("Save " + _node.Name);
			XmlNode mEEAKLDGLDF = _node["PerkHistory"] ?? _node.ACBPMPMPKJJ("PerkHistory");
			XmlNode mEEAKLDGLDF2 = mEEAKLDGLDF.ACBPMPMPKJJ("Level");
			mEEAKLDGLDF2.LLIKNHNLGJJ("Value").Value = AEFFHJGMNFI.Level.ToString();
			mEEAKLDGLDF2.LLIKNHNLGJJ("Perk").Value = AEFFHJGMNFI.Name;
			ListSF.CCDKHLAMKKO().GGGEHAGCLGC();
		}
	}

	public void DDBGEFPKAPN(RosterPerk AEFFHJGMNFI)
	{
		if (AEFFHJGMNFI != null)
		{
			CAGEDGLJAKF(AEFFHJGMNFI);
		}
	}

	public void DDBGEFPKAPN(RosterPerk PPPNCJLGJPE, ProfilePerk AEFFHJGMNFI)
	{
		LHCFFCKNMOO(AEFFHJGMNFI.DFOELJAEEGG());
		DDBGEFPKAPN(PPPNCJLGJPE);
		ListSF.CCDKHLAMKKO().GGGEHAGCLGC();
		Sound.IFKCCDAIADF("snd_learn");
	}

	public void DDBGEFPKAPN(RosterPerk PPPNCJLGJPE, RosterPerkInfo BPANICNCIAO)
	{
		LHCFFCKNMOO(BPANICNCIAO.GEFLIFEPDNG);
		if (PPPNCJLGJPE != null)
		{
			CAGEDGLJAKF(PPPNCJLGJPE, true);
		}
		ListSF.CCDKHLAMKKO().GGGEHAGCLGC();
	}

	public RosterPerk LKIEAGLHNON(string name)
	{
		for (int i = 0; i < GBMLFKHHLCC.Count; i++)
		{
			RosterPerk hOGDBKBFFDJ = GBMLFKHHLCC[i];
			if (hOGDBKBFFDJ.get_Name() == name)
			{
				return hOGDBKBFFDJ;
			}
		}
		return null;
	}

	public int IBAOKPECDLF()
	{
		int num = 0;
		foreach (RosterPerk item in GBMLFKHHLCC)
		{
			num += item.DHNNCAEEMLL();
		}
		return num;
	}

	public void LCDFOLAAEGM()
	{
		XmlNode xmlNode = _node["Perks"];
		if (xmlNode != null)
		{
			_node.RemoveChild(xmlNode);
		}
		XmlNode xmlNode2 = _node["PerkHistory"];
		if (xmlNode2 != null)
		{
			_node.RemoveChild(xmlNode2);
		}
		XmlNode xmlNode3 = _node["OpenTricks"];
		if (xmlNode3 != null)
		{
			_node.RemoveChild(xmlNode3);
		}
		ListSF.CCDKHLAMKKO().GGGEHAGCLGC();
		GBMLFKHHLCC.Clear();
		GIAEMMLABDL.JOGBKOJCINM.Clear();
		PerkTree.GBPBIPFIOJH().LJHPGKAOIAE();
		HEGIABHIPHA.JGCNPHDGHAK.Clear();
		GameUtils.FDEJIIDIPBI.BPBLIPKOJOP().Clear();
	}

	public int ICIAGDCEMEM()
	{
		UserItem dKCHDHMLKHN = ListSF.CCDKHLAMKKO().KHCNHPCPFII().CMGOCLGHNLH("Perk_Reset");
		if (dKCHDHMLKHN != null)
		{
			return dKCHDHMLKHN.OFOPFCJNEBL();
		}
		return 0;
	}

	public void CAGEDGLJAKF(RosterPerk value, bool PGDIBFDIEIB = false)
	{
		List<PerkInfoItem> list = GameUtils.FDEJIIDIPBI.BPBLIPKOJOP();
		foreach (PerkInfoItem item in list)
		{
			bool flag = item.Name.Equals(value.get_Name());
			bool flag2 = item.AKKLOMFOLNO == value.DHNNCAEEMLL();
			if (flag && flag2)
			{
				value.NOLDHAFMOLF(item);
			}
		}
		if (value.DFOELJAEEGG() == null)
		{
			List<PerkInfoItem> list2 = GameUtils.FDEJIIDIPBI.GFPFNILGJML();
			foreach (PerkInfoItem item2 in list2)
			{
				bool flag3 = item2.Name.Equals(value.get_Name());
				bool flag4 = item2.AKKLOMFOLNO == value.DHNNCAEEMLL();
				if (flag3 && flag4)
				{
					value.NOLDHAFMOLF(item2);
				}
			}
		}
		if (value.DFOELJAEEGG() == null)
		{
			List<PerkInfoItem> list3 = GameUtils.FDEJIIDIPBI.CJJEPHDFOCJ();
			foreach (PerkInfoItem item3 in list3)
			{
				if (item3.Name.Equals(value.get_Name()))
				{
					value.NOLDHAFMOLF(item3);
				}
			}
		}
		if (value.DFOELJAEEGG() != null)
		{
			RosterPerk hOGDBKBFFDJ = GBMLFKHHLCC.Find((RosterPerk DHDMNHCIPEH) => DHDMNHCIPEH.Equals(value.get_Name()));
			if (hOGDBKBFFDJ != null)
			{
				GBMLFKHHLCC.Remove(hOGDBKBFFDJ);
			}
			if (PGDIBFDIEIB)
			{
				GBMLFKHHLCC.Insert(0, value);
			}
			else
			{
				GBMLFKHHLCC.Add(value);
			}
			DEHDIDFCECL(value.DFOELJAEEGG());
		}
	}

	private void DEHDIDFCECL(PerkInfoItem value)
	{
		if (HEGIABHIPHA != null)
		{
			HEGIABHIPHA.JGCNPHDGHAK.Add(value);
		}
	}

	private void LHCFFCKNMOO(PerkInfoItem value)
	{
		if (HEGIABHIPHA != null && value != null)
		{
			PerkInfoItem aCONCDFDNJH = HEGIABHIPHA.JGCNPHDGHAK.Find((PerkInfoItem DHDMNHCIPEH) => DHDMNHCIPEH.Name.Equals(value.Name));
			if (aCONCDFDNJH != null)
			{
				HEGIABHIPHA.JGCNPHDGHAK.Remove(aCONCDFDNJH);
			}
		}
	}
}
