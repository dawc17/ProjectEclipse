using System.Collections.Generic;

public static class AnimationData
{
	private static Dictionary<string, TemplateAnimation> KKANPMPHNNA = new Dictionary<string, TemplateAnimation>();

	private static readonly Dictionary<string, InfoAnimation> BKCEJLOBCNB = new Dictionary<string, InfoAnimation>();

	private static readonly List<InfoAnimation> LNKJIIGBEDA = new List<InfoAnimation>();

	private static readonly List<Trick> AGBJABJNGEA = new List<Trick>();

	private static readonly List<Trigger> NMILPLHGCMA = new List<Trigger>();

	private static List<string> _WeaponTypeList = new List<string>();

	public static List<InfoAnimation> KGPMGOBAOFG
	{
		get
		{
			return CCANGHENJAE();
		}
	}

	public static int BCIGIMOHBJH
	{
		get
		{
			return DJDLCMCLOJN();
		}
	}

	public static List<Trick> GCBKPAHELLI
	{
		get
		{
			return BFNFDDLNHPA();
		}
	}

	public static List<Trigger> ANPKEANHHGE
	{
		get
		{
			return GFPPKEAMEBO();
		}
	}

	public static List<string> EDIPADGDGPM
	{
		get
		{
			return LOJEMPOAAKF();
		}
	}

	public static List<InfoAnimation> CCANGHENJAE()
	{
		return LNKJIIGBEDA;
	}

	public static int DJDLCMCLOJN()
	{
		return LNKJIIGBEDA.Count;
	}

	public static List<Trick> BFNFDDLNHPA()
	{
		return AGBJABJNGEA;
	}

	public static List<Trigger> GFPPKEAMEBO()
	{
		return NMILPLHGCMA;
	}

	public static List<string> LOJEMPOAAKF()
	{
		if (_WeaponTypeList.Count != 0)
		{
			return _WeaponTypeList;
		}
		for (int i = 0; i < LNKJIIGBEDA.Count; i++)
		{
			List<string> list = LNKJIIGBEDA[i].OIDIJEOMJCB();
			if (list.Count == 0)
			{
				continue;
			}
			bool flag = true;
			foreach (string item in _WeaponTypeList)
			{
				if (list.IndexOf(item) != -1)
				{
					flag = false;
					break;
				}
			}
			if (!flag)
			{
				continue;
			}
			foreach (string item2 in list)
			{
				_WeaponTypeList.Add(item2);
			}
		}
		return _WeaponTypeList;
	}

	public static void Load(string PMFEIPCHENB, bool OOJAEKEOEFJ)
	{
		MovesParser.Parse(PMFEIPCHENB, LNKJIIGBEDA, KKANPMPHNNA, AGBJABJNGEA, NMILPLHGCMA, OOJAEKEOEFJ);
		InfoAnimation pJAHIOELGGD = null;
		for (int i = 0; i < LNKJIIGBEDA.Count; i++)
		{
			pJAHIOELGGD = LNKJIIGBEDA[i];
			BKCEJLOBCNB[pJAHIOELGGD.Name] = pJAHIOELGGD;
		}
		CreateCapabilityTables();
	}

	public static void BCILLFEBJHK()
	{
		LNKJIIGBEDA.Clear();
		BKCEJLOBCNB.Clear();
		InfoAnimation.EGLKBMCHPNN();
		AGBJABJNGEA.Clear();
		KKANPMPHNNA.Clear();
		NMILPLHGCMA.Clear();
		MovesParser.CHILAIJNEHG();
	}

	public static void CreateCapabilityTables()
	{
		foreach (InfoAnimation lNKJIIGBEDum in LNKJIIGBEDA)
		{
			CreateCapabilityTable(lNKJIIGBEDum, LNKJIIGBEDA);
		}
	}

	public static void CreateCapabilityTable(InfoAnimation DBOLBEOCEME, List<InfoAnimation> MAHEJFLCCHP)
	{
		List<ConditionKeys> list = DBOLBEOCEME.MOPMGFIIFGA();
		int count = list.Count;
		if (0 >= count)
		{
			return;
		}
		foreach (InfoAnimation item in MAHEJFLCCHP)
		{
			if (DBOLBEOCEME.Priority >= item.Priority)
			{
				continue;
			}
			List<ConditionKeys> list2 = item.MOPMGFIIFGA();
			int count2 = list2.Count;
			if (0 >= count2)
			{
				continue;
			}
			bool flag = false;
			foreach (ConditionKeys item2 in list)
			{
				KeyData fONEJOKEIEN = item2.FONEJOKEIEN;
				foreach (ConditionKeys item3 in list2)
				{
					KeyData fONEJOKEIEN2 = item3.FONEJOKEIEN;
					if (fONEJOKEIEN2.IsVariable(fONEJOKEIEN))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					break;
				}
			}
			if (flag)
			{
				DBOLBEOCEME.ICANLHJKKNE.NINJLLDJLFI.Add(item);
			}
		}
	}

	public static void AKJLPGMEFFD(List<InfoAnimation> MAHEJFLCCHP, List<ItemInfo> HELFDCAIJNE, bool ABGINCCBACK = false, List<string> JHJPMONBIDI = null, SceneTypes NFNJJIGAKNN = SceneTypes.SceneFight, List<PerkInfoItem> MAFPBEFKNGE = null, List<PerkInfoItem> CFKCGBEONAM = null)
	{
		List<ConditionAnimation> list = new List<ConditionAnimation>();
		ModelConditions dGJJDPIAEAO = new ModelConditions();
		dGJJDPIAEAO.OJIAKDDCGLB = HELFDCAIJNE;
		dGJJDPIAEAO.FDELMAHAAJD = ABGINCCBACK;
		dGJJDPIAEAO.IBBALIJOJMC = NFNJJIGAKNN;
		dGJJDPIAEAO.POBNMMADAJJ = MAFPBEFKNGE;
		dGJJDPIAEAO.CFPLPALGCMK = CFKCGBEONAM;
		foreach (InfoAnimation lNKJIIGBEDum in LNKJIIGBEDA)
		{
			list = lNKJIIGBEDum.ODACDCDONJE.HIFPHBNGIPO;
			if (lNKJIIGBEDum.HPPGNJJCEGF(dGJJDPIAEAO, list) && (JHJPMONBIDI == null || !lNKJIIGBEDum.CheckAnimationName(JHJPMONBIDI)))
			{
				MAHEJFLCCHP.Add(lNKJIIGBEDum);
			}
		}
	}

	public static void FMDFKKEDMJG(List<Trigger> CMHFKBKKKOK, List<ItemInfo> HELFDCAIJNE, bool ABGINCCBACK = false, SceneTypes NFNJJIGAKNN = SceneTypes.SceneFight, List<PerkInfoItem> MAFPBEFKNGE = null, List<PerkInfoItem> CFKCGBEONAM = null)
	{
		CMHFKBKKKOK.Clear();
		List<ConditionAnimation> list = new List<ConditionAnimation>();
		ModelConditions dGJJDPIAEAO = new ModelConditions();
		dGJJDPIAEAO.OJIAKDDCGLB = HELFDCAIJNE;
		dGJJDPIAEAO.FDELMAHAAJD = ABGINCCBACK;
		dGJJDPIAEAO.IBBALIJOJMC = NFNJJIGAKNN;
		dGJJDPIAEAO.POBNMMADAJJ = MAFPBEFKNGE;
		dGJJDPIAEAO.CFPLPALGCMK = CFKCGBEONAM;
		foreach (Trigger item in NMILPLHGCMA)
		{
			list = item.IDEMFOLJIFE.HIFPHBNGIPO;
			if (item.HPPGNJJCEGF(dGJJDPIAEAO, list))
			{
				CMHFKBKKKOK.Add(item);
			}
		}
	}

	public static void OCMIKNOMINM(List<string> NIKHAICFGNM, List<InfoAnimation> OEMALIFPGPO)
	{
		for (int i = 0; i < NIKHAICFGNM.Count; i++)
		{
			NEBELEFIDMB(NIKHAICFGNM[i], OEMALIFPGPO);
		}
	}

	public static void NEBELEFIDMB(string name, List<InfoAnimation> OEMALIFPGPO)
	{
		if (KKANPMPHNNA.ContainsKey(name))
		{
			if (OEMALIFPGPO.Count == 0)
			{
				OEMALIFPGPO.AddRange(KKANPMPHNNA[name].LDEBJOPLCKO());
			}
			else
			{
				OEMALIFPGPO.AddIfNotExist(KKANPMPHNNA[name].LDEBJOPLCKO());
			}
		}
	}

	public static TemplateAnimation ANEMJNGKFDB(string name)
	{
		if (KKANPMPHNNA.ContainsKey(name))
		{
			return KKANPMPHNNA[name];
		}
		return null;
	}

	public static InfoAnimation KCHIFIDKLOC(ItemInfo LGCMGHAFEDD)
	{
		if (LGCMGHAFEDD == null)
		{
			return BCIFKBJAFEC("StanceIdle");
		}
		string item = "Stance";
		string mENAJEAJJBE = LGCMGHAFEDD.Name;
		foreach (InfoAnimation lNKJIIGBEDum in LNKJIIGBEDA)
		{
			List<string> list = lNKJIIGBEDum.FOLOOGCLPNE();
			List<string> list2 = lNKJIIGBEDum.OIDIJEOMJCB();
			if (((list2.Count == 0 && string.IsNullOrEmpty(mENAJEAJJBE)) || (list2.Count != 0 && list2.IndexOf(mENAJEAJJBE) != -1)) && list.IndexOf(item) != -1)
			{
				return lNKJIIGBEDum;
			}
		}
		return BCIFKBJAFEC("StanceIdle");
	}

	public static void PHNMANPDPKG(List<Trick> IAGDAAPCDNI, List<ItemInfo> HELFDCAIJNE, bool ABGINCCBACK = false, List<string> JHJPMONBIDI = null, List<PerkInfoItem> JOGBKOJCINM = null, SceneTypes NFNJJIGAKNN = SceneTypes.SceneFight)
	{
		List<InfoAnimation> list = new List<InfoAnimation>();
		AKJLPGMEFFD(list, HELFDCAIJNE, ABGINCCBACK, JHJPMONBIDI, NFNJJIGAKNN, JOGBKOJCINM);
		foreach (Trick item in AGBJABJNGEA)
		{
			foreach (InfoAnimation item2 in list)
			{
				if (item.KJHMOGGECBN == item2)
				{
					IAGDAAPCDNI.Add(item);
				}
			}
		}
	}

	public static InfoAnimation BCIFKBJAFEC(string name, bool ADCNNABFIDL = true)
	{
		InfoAnimation value = null;
		if (BKCEJLOBCNB.TryGetValue(name, out value))
		{
			return value;
		}
		if (ADCNNABFIDL)
		{
			LLLOJBFMONN.Error("Animation " + name + " not found");
		}
		else
		{
			LLLOJBFMONN.Write("Animation " + name + " not found");
		}
		return null;
	}

	public static void GAPACJBBJKL(List<string> GKHEPKGMEFI, List<InfoAnimation> FKFEKLNOAGE = null)
	{
		List<InfoAnimation> list = ((FKFEKLNOAGE != null) ? FKFEKLNOAGE : LNKJIIGBEDA);
		foreach (InfoAnimation item in list)
		{
			InfoAnimation.MovePivot iLOEBFFAEAN = item.ODACDCDONJE.ILOEBFFAEAN;
			if (iLOEBFFAEAN.CKBGFODEBAJ != InfoAnimation.DOLCEABGNGA.ObjectNodes || iLOEBFFAEAN.EDBLMNIEKBD != ModelType.KEIDBIOIFGA.MODEL_THIS || iLOEBFFAEAN.HHPAGAOGGLP != InfoAnimation.DOLCEABGNGA.ObjectPivot)
			{
				continue;
			}
			string bLODCIGDJFK = item.ODACDCDONJE.ILOEBFFAEAN.BLODCIGDJFK;
			if (string.IsNullOrEmpty(bLODCIGDJFK))
			{
				continue;
			}
			bool flag = true;
			foreach (string item2 in GKHEPKGMEFI)
			{
				if (item2 == bLODCIGDJFK)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				GKHEPKGMEFI.Add(bLODCIGDJFK);
			}
		}
	}
}
