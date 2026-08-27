using System.Collections.Generic;
using System.Xml;

public class PerkTree
{
	public enum AAAIBJGLPAI
	{
		TYPE_NONE = 0,
		TYPE_PERK = 1,
		TYPE_UPGRADE = 2
	}

	public class PerkItem
	{
		public AAAIBJGLPAI Type;

		public string Name;

		public int Level;

		public PerkItem(AAAIBJGLPAI _type, string _name, int _level)
		{
			Type = _type;
			Name = _name;
			Level = _level;
		}

		public bool KHNMKGFDPHD()
		{
			bool flag = false;
			RosterPerk hOGDBKBFFDJ = ListSF.CCDKHLAMKKO().JLBDOBLHHAF().LKIEAGLHNON(Name);
			bool flag2 = hOGDBKBFFDJ != null;
			switch (Type)
			{
			case AAAIBJGLPAI.TYPE_PERK:
				return !flag2 || hOGDBKBFFDJ.PINDEKDNCNL() >= Level;
			case AAAIBJGLPAI.TYPE_UPGRADE:
				return flag2 && hOGDBKBFFDJ.PINDEKDNCNL() <= Level;
			default:
				return false;
			}
		}
	}

	public class PerkBranch
	{
		public List<PerkItem> OJIAKDDCGLB = new List<PerkItem>();

		public int Level;

		public PerkBranch(int _level)
		{
			Level = _level;
		}

		public List<PerkItem> HNGFNDIKHEC(int CCBEHBMOPMC = 2)
		{
			List<PerkItem> list = new List<PerkItem>();
			for (int i = 0; i < OJIAKDDCGLB.Count; i++)
			{
				PerkItem pJOFNPMOJJA = OJIAKDDCGLB[i];
				if (pJOFNPMOJJA.KHNMKGFDPHD())
				{
					list.Add(pJOFNPMOJJA);
				}
				if (list.Count >= CCBEHBMOPMC)
				{
					break;
				}
			}
			return list;
		}
	}

	private static PerkTree _instance;

	private List<PerkBranch> COPMNJGPPIH = new List<PerkBranch>();

	private List<ProfilePerkContainer> HBDGGFOPFFB = new List<ProfilePerkContainer>();

	private List<ProfilePerk> DPPMNFCIIGP = new List<ProfilePerk>();

	private List<PerkInfoItem> PPHJHENDCLL = new List<PerkInfoItem>();

	public static PerkTree GBPBIPFIOJH()
	{
		if (_instance == null)
		{
			_instance = new PerkTree();
		}
		return _instance;
	}

	public static bool Compare(PerkBranch MKICABFAHFA, PerkBranch JMLKHIPBCLI)
	{
		return MKICABFAHFA.Level < JMLKHIPBCLI.Level;
	}

	public void Clear()
	{
		COPMNJGPPIH.Clear();
		FAGKACLCCPE();
	}

	public void FAGKACLCCPE()
	{
		HBDGGFOPFFB.Clear();
		DPPMNFCIIGP.Clear();
		PPHJHENDCLL.Clear();
	}

	public void LJHPGKAOIAE()
	{
		FAGKACLCCPE();
		PPHJHENDCLL = GameUtils.FDEJIIDIPBI.GFPFNILGJML();
		List<PerkBranch> list = GBPBIPFIOJH().LGGMDGDHJJP();
		for (int i = 0; i < list.Count; i++)
		{
			PerkBranch gOOLLBPEFJM = list[i];
			int bLJGEOEHIGP = ((gOOLLBPEFJM.OJIAKDDCGLB.Count == 1) ? 1 : 2);
			AHPDPEDGJLM(gOOLLBPEFJM.Level, bLJGEOEHIGP);
		}
		if (list.Count != 0)
		{
			AEOKBBBAANA(list[0]);
		}
		DCBCNJJCAMP();
		List<PerkHistory.Perk> jOGBKOJCINM = ListSF.CCDKHLAMKKO().JLBDOBLHHAF().GIAEMMLABDL.JOGBKOJCINM;
		for (int j = 0; j < jOGBKOJCINM.Count; j++)
		{
			AEOKBBBAANA(jOGBKOJCINM[j]);
		}
	}

	public void AEOKBBBAANA(PerkHistory.Perk AEFFHJGMNFI)
	{
		if (AEFFHJGMNFI != null)
		{
			RemovePerkInfoItemIfExist(AEFFHJGMNFI.Name);
			PerkBranch gOOLLBPEFJM = FMNLBLFHJFB(AEFFHJGMNFI.Level);
			if (gOOLLBPEFJM != null)
			{
				AEOKBBBAANA(gOOLLBPEFJM);
			}
			KCOMDIKHFCH(AEFFHJGMNFI);
		}
	}

	public void Parse(XmlNode node)
	{
		Clear();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			FBCMCLEBAKC(childNode);
		}
		COPMNJGPPIH.Sort((PerkBranch LHBNIMGFKIB, PerkBranch AAOIAEJJINO) => LHBNIMGFKIB.Level.CompareTo(AAOIAEJJINO.Level));
	}

	public int KBIOIHPNLIM()
	{
		int num = 0;
		int num2 = ListSF.CCDKHLAMKKO().PINDEKDNCNL();
		for (int i = 0; i < COPMNJGPPIH.Count; i++)
		{
			PerkBranch gOOLLBPEFJM = COPMNJGPPIH[i];
			if (gOOLLBPEFJM.Level > num2)
			{
				break;
			}
			num++;
		}
		return num;
	}

	public List<PerkBranch> LGGMDGDHJJP()
	{
		return COPMNJGPPIH;
	}

	public List<ProfilePerk> JGCHDCOOGII()
	{
		return DPPMNFCIIGP;
	}

	public List<ProfilePerk> HBODBIBFIKM(int GNLOCMLBNHF)
	{
		List<ProfilePerk> list = new List<ProfilePerk>();
		for (int i = 0; i < DPPMNFCIIGP.Count; i++)
		{
			ProfilePerk pLKCIINIFMJ = DPPMNFCIIGP[i];
			if (pLKCIINIFMJ.PINDEKDNCNL() == GNLOCMLBNHF)
			{
				list.Add(pLKCIINIFMJ);
			}
			else if (pLKCIINIFMJ.PINDEKDNCNL() > GNLOCMLBNHF)
			{
				break;
			}
		}
		return list;
	}

	public ProfilePerk LAAJJBEEDKL(string name)
	{
		for (int i = 0; i < DPPMNFCIIGP.Count; i++)
		{
			ProfilePerk pLKCIINIFMJ = DPPMNFCIIGP[i];
			if (pLKCIINIFMJ.KAMBOKLFBEE() == name)
			{
				return pLKCIINIFMJ;
			}
		}
		return null;
	}

	public List<ProfilePerkContainer> KGKJCLDFIHA()
	{
		return HBDGGFOPFFB;
	}

	public ProfilePerkContainer HKCIFHMLKKM(int GNLOCMLBNHF)
	{
		for (int i = 0; i < HBDGGFOPFFB.Count; i++)
		{
			ProfilePerkContainer fHPJJGPJLHD = HBDGGFOPFFB[i];
			if (fHPJJGPJLHD.Level == GNLOCMLBNHF)
			{
				return fHPJJGPJLHD;
			}
		}
		return null;
	}

	public ProfilePerkContainer HPKLHAAFPHK(int GNLOCMLBNHF)
	{
		for (int i = 0; i < HBDGGFOPFFB.Count; i++)
		{
			ProfilePerkContainer fHPJJGPJLHD = HBDGGFOPFFB[i];
			if (fHPJJGPJLHD.Level > GNLOCMLBNHF)
			{
				return fHPJJGPJLHD;
			}
		}
		return null;
	}

	public PerkBranch FMNLBLFHJFB(int GNLOCMLBNHF)
	{
		for (int i = 0; i < COPMNJGPPIH.Count; i++)
		{
			PerkBranch gOOLLBPEFJM = COPMNJGPPIH[i];
			if (gOOLLBPEFJM.Level > GNLOCMLBNHF)
			{
				return gOOLLBPEFJM;
			}
		}
		return null;
	}

	private void FBCMCLEBAKC(XmlNode node)
	{
		int iBMNBEGDMBJ = node.Attributes["Value"].ParseInt();
		PerkBranch gOOLLBPEFJM = new PerkBranch(iBMNBEGDMBJ);
		foreach (XmlNode childNode in node.ChildNodes)
		{
			AAAIBJGLPAI aMKFIJMKLIB = MLFLOGKMLJI(childNode.Name);
			string pHJCCJNOCGJ = childNode.Attributes["Name"].CIPOICEEIBK(string.Empty);
			PerkItem item = new PerkItem(aMKFIJMKLIB, pHJCCJNOCGJ, iBMNBEGDMBJ);
			gOOLLBPEFJM.OJIAKDDCGLB.Add(item);
		}
		COPMNJGPPIH.Add(gOOLLBPEFJM);
	}

	private void RemoveProfilePerksWithLevel(int GNLOCMLBNHF)
	{
		int num = 0;
		while (num != DPPMNFCIIGP.Count)
		{
			if (DPPMNFCIIGP[num].PINDEKDNCNL() == GNLOCMLBNHF)
			{
				DPPMNFCIIGP.RemoveAt(num);
			}
			else
			{
				num++;
			}
		}
		ProfilePerkContainer fHPJJGPJLHD = HKCIFHMLKKM(GNLOCMLBNHF);
		if (fHPJJGPJLHD != null)
		{
			List<ProfilePerk> jOGBKOJCINM = fHPJJGPJLHD.JOGBKOJCINM;
			jOGBKOJCINM.Clear();
		}
	}

	private void EKJFPAJMIIF(PerkBranch JBEIKKDKINI)
	{
		List<PerkItem> list = JBEIKKDKINI.HNGFNDIKHEC();
		ProfilePerkContainer fHPJJGPJLHD = HKCIFHMLKKM(JBEIKKDKINI.Level);
		for (int i = 0; i < list.Count; i++)
		{
			PerkItem pJOFNPMOJJA = list[i];
			PerkInfoItem aCONCDFDNJH = CIHOCOBECNP(pJOFNPMOJJA.Name);
			if (aCONCDFDNJH != null)
			{
				ProfilePerk item = new ProfilePerk(aCONCDFDNJH, pJOFNPMOJJA.Level, ProfilePerk.KMHBPKKCNPP.PERK_LOCK, HCHAAHKHCFN(pJOFNPMOJJA.Type));
				DPPMNFCIIGP.Add(item);
				if (fHPJJGPJLHD != null)
				{
					fHPJJGPJLHD.JOGBKOJCINM.Add(item);
				}
			}
		}
	}

	private void AHPDPEDGJLM(int GNLOCMLBNHF, int count = 2)
	{
		ProfilePerkContainer fHPJJGPJLHD = new ProfilePerkContainer(GNLOCMLBNHF);
		HBDGGFOPFFB.Add(fHPJJGPJLHD);
		for (int i = 0; i < count; i++)
		{
			ProfilePerk item = new ProfilePerk(null, GNLOCMLBNHF, ProfilePerk.KMHBPKKCNPP.PERK_LOCK);
			DPPMNFCIIGP.Add(item);
			fHPJJGPJLHD.JOGBKOJCINM.Add(item);
		}
	}

	private void AEOKBBBAANA(PerkBranch JBEIKKDKINI)
	{
		RemoveProfilePerksWithLevel(JBEIKKDKINI.Level);
		EKJFPAJMIIF(JBEIKKDKINI);
		DPPMNFCIIGP.Sort((ProfilePerk LHBNIMGFKIB, ProfilePerk AAOIAEJJINO) => LHBNIMGFKIB.PINDEKDNCNL().CompareTo(AAOIAEJJINO.PINDEKDNCNL()));
	}

	private void KCOMDIKHFCH(PerkHistory.Perk AEFFHJGMNFI)
	{
		List<ProfilePerk> list = HBODBIBFIKM(AEFFHJGMNFI.Level);
		for (int i = 0; i < list.Count; i++)
		{
			ProfilePerk pLKCIINIFMJ = list[i];
			ProfilePerk.KMHBPKKCNPP bAINMLLIKOL = ((!(pLKCIINIFMJ.KAMBOKLFBEE() == AEFFHJGMNFI.Name)) ? ProfilePerk.KMHBPKKCNPP.PERK_UNAVAILABLE : ProfilePerk.KMHBPKKCNPP.PERK_SELECTED);
			pLKCIINIFMJ.set_State(bAINMLLIKOL);
		}
		List<ProfilePerk> list2 = ENFMEDEFINB(AEFFHJGMNFI.Level);
		for (int j = 0; j < list2.Count; j++)
		{
			list2[j].set_State(ProfilePerk.KMHBPKKCNPP.PERK_AVAILABLE);
		}
	}

	private void DCBCNJJCAMP()
	{
		if (DPPMNFCIIGP.Count > 0)
		{
			int num = DPPMNFCIIGP[0].PINDEKDNCNL();
			int count = DPPMNFCIIGP.Count;
			for (int i = 0; i < count && num == DPPMNFCIIGP[i].PINDEKDNCNL(); i++)
			{
				DPPMNFCIIGP[i].set_State(ProfilePerk.KMHBPKKCNPP.PERK_AVAILABLE);
			}
		}
	}

	private PerkInfoItem CIHOCOBECNP(string name)
	{
		PerkInfoItem aCONCDFDNJH = null;
		foreach (PerkInfoItem item in PPHJHENDCLL)
		{
			if (item.Name == name)
			{
				aCONCDFDNJH = item;
				break;
			}
		}
		if (aCONCDFDNJH == null)
		{
			aCONCDFDNJH = GameUtils.FDEJIIDIPBI.ABAGJKMKCBA(name);
		}
		return aCONCDFDNJH;
	}

	private void RemovePerkInfoItemIfExist(string name)
	{
		for (int i = 0; i < PPHJHENDCLL.Count; i++)
		{
			if (PPHJHENDCLL[i].Name == name)
			{
				PPHJHENDCLL.RemoveAt(i);
				break;
			}
		}
	}

	private AAAIBJGLPAI MLFLOGKMLJI(string CNKBLODAFDO)
	{
		AAAIBJGLPAI result = AAAIBJGLPAI.TYPE_NONE;
		if (CNKBLODAFDO == "Perk")
		{
			result = AAAIBJGLPAI.TYPE_PERK;
		}
		else if (CNKBLODAFDO == "Upgrade")
		{
			result = AAAIBJGLPAI.TYPE_UPGRADE;
		}
		return result;
	}

	private ProfilePerk.JHDKDOPHGOO HCHAAHKHCFN(AAAIBJGLPAI LFLGCDNKNJI)
	{
		switch (LFLGCDNKNJI)
		{
		case AAAIBJGLPAI.TYPE_PERK:
			return ProfilePerk.JHDKDOPHGOO.TYPE_PERK;
		case AAAIBJGLPAI.TYPE_UPGRADE:
			return ProfilePerk.JHDKDOPHGOO.TYPE_UPGRADE;
		default:
			return ProfilePerk.JHDKDOPHGOO.TYPE_NONE;
		}
	}

	private List<ProfilePerk> ENFMEDEFINB(int GNLOCMLBNHF)
	{
		List<ProfilePerk> list = new List<ProfilePerk>();
		int num = GNLOCMLBNHF;
		for (int i = 0; i < DPPMNFCIIGP.Count; i++)
		{
			ProfilePerk pLKCIINIFMJ = DPPMNFCIIGP[i];
			int num2 = pLKCIINIFMJ.PINDEKDNCNL();
			if (num2 > num && num > GNLOCMLBNHF)
			{
				break;
			}
			if (num2 > GNLOCMLBNHF)
			{
				num = num2;
				list.Add(pLKCIINIFMJ);
			}
		}
		return list;
	}
}
