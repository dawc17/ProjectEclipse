using System.Collections.Generic;

public class PerksStage : global::EventDispatcher<PerksStage.PerkEventStruct>
{
	public enum CECEBJIFBHD
	{
		PERK_PARENT_NONE = 0,
		PERK_PARENT_ITEM = 1,
		PERK_PARENT_RULE = 2,
		PERK_PARENT_MODEL = 3
	}

	public class ActionPerk
	{
		public bool PLNNKKBPDJK;

		public Model BIKLKJMNGKP;

		public Model KJDFJPBIGJC;

		public PerkAction AMKJNPOCODK;

		public int KGNDJOLBBJF;

		public int FLNLMIHEDCI;

		public string NHKMCLPOMFK = string.Empty;

		public string GJONJADIAJM = string.Empty;

		public bool FLNCPBKBJBL;

		public int MGDCIODPHCH;

		public ItemInfo PreviousMagic;

		public string KGPDHIKOEKF
		{
			get
			{
				return LGMFEIFGGDG();
			}
		}

		public string AOOJOKOHAHA
		{
			get
			{
				return DDBPICENEJE();
			}
		}

		public ActionPerk()
		{
		}

		public ActionPerk(ActionPerk IBODMPMJELJ)
		{
			PLNNKKBPDJK = IBODMPMJELJ.PLNNKKBPDJK;
			BIKLKJMNGKP = IBODMPMJELJ.BIKLKJMNGKP;
			KJDFJPBIGJC = IBODMPMJELJ.KJDFJPBIGJC;
			AMKJNPOCODK = IBODMPMJELJ.AMKJNPOCODK;
			KGNDJOLBBJF = IBODMPMJELJ.KGNDJOLBBJF;
			FLNLMIHEDCI = IBODMPMJELJ.FLNLMIHEDCI;
			NHKMCLPOMFK = IBODMPMJELJ.NHKMCLPOMFK;
			GJONJADIAJM = IBODMPMJELJ.GJONJADIAJM;
			FLNCPBKBJBL = IBODMPMJELJ.FLNCPBKBJBL;
			MGDCIODPHCH = IBODMPMJELJ.MGDCIODPHCH;
			PreviousMagic = IBODMPMJELJ.PreviousMagic;
		}

		public string LGMFEIFGGDG()
		{
			return AMKJNPOCODK.JMDLAMHAJLN().Name;
		}

		public string DDBPICENEJE()
		{
			return AMKJNPOCODK.get_Name();
		}
	}

	public class PerkEventStruct
	{
		public Model KJDFJPBIGJC;

		public object Data;

		public PerkEvent.KNKIIEPDCPN DJPLGDJCMPI;
	}

	private Dictionary<string, object> _PerkMap = new Dictionary<string, object>();

	private List<PerkModelStruct> MPJMCCGKEOD = new List<PerkModelStruct>();

	private List<ActionPerk> JLAKGOEOHMN = new List<ActionPerk>();

	private static Dictionary<string, List<ActionPerk>> PNAALKAHAKG = new Dictionary<string, List<ActionPerk>>();

	private static Dictionary<string, int> PerkUsesLeft = new Dictionary<string, int>();

	public Dictionary<string, object> OILMNCIGCNI
	{
		get
		{
			return OFKIKABKDFD();
		}
	}

	public Dictionary<string, object> OFKIKABKDFD()
	{
		return _PerkMap;
	}

	public void Run()
	{
		foreach (PerkModelStruct item in MPJMCCGKEOD)
		{
			foreach (InfoPerk item2 in item.HIPOGANEPMI())
			{
				item2.Run();
			}
		}
	}

	public void MIPABIOGDBH(List<Model> INNLAFHKJNI)
	{
		JBOGMAPDLHG();
		foreach (Model item in INNLAFHKJNI)
		{
			AddModel(item);
		}
	}

	public void AddModel(Model ACENLMONNPA)
	{
		RemoveModel(ACENLMONNPA);
		PerkModelStruct iAIBLEELGNK = new PerkModelStruct();
		iAIBLEELGNK.set_Model(ACENLMONNPA);
		MPJMCCGKEOD.Add(iAIBLEELGNK);
		foreach (PerkInfoItem item in ACENLMONNPA.KMMJCHDKBDO.NHBIJEEKALC)
		{
			OPACOCIKEOL(iAIBLEELGNK, item);
		}
	}

	public void RemoveModel(Model ACENLMONNPA)
	{
		foreach (PerkModelStruct item in MPJMCCGKEOD)
		{
			if (item.get_Model() == ACENLMONNPA)
			{
				MPJMCCGKEOD.Remove(item);
				break;
			}
		}
	}

	public void JBOGMAPDLHG()
	{
		MPJMCCGKEOD.Clear();
	}

	public void Reset()
	{
		PerkUsesLeft.Clear();
		foreach (PerkModelStruct item in MPJMCCGKEOD)
		{
			foreach (InfoPerk item2 in item.HIPOGANEPMI())
			{
				bool gIBIGPCELOB = true;
				item2.ClearActions(gIBIGPCELOB);
			}
		}
	}

	public bool JALOHCICLGN(Model FAJBDBKEHJL, PerkEvent.KNKIIEPDCPN LFLGCDNKNJI, bool GMFCKPBJNLC = false, PerkTrigger CPBHKJFPFJB = null)
	{
		DBHDFPCPHEH(FAJBDBKEHJL, LFLGCDNKNJI);
		object obj = ((!OFKIKABKDFD().ContainsKey("Namespace")) ? null : OFKIKABKDFD()["Namespace"]);
		string fILIJOFBNMA = string.Empty;
		if (obj != null)
		{
			fILIJOFBNMA = (string)obj;
		}
		foreach (PerkModelStruct item in MPJMCCGKEOD)
		{
			if (CPBHKJFPFJB != null)
			{
				if (FAJBDBKEHJL == item.get_Model())
				{
					FOMKDCMBDJD(item, FAJBDBKEHJL, null, CPBHKJFPFJB, true);
				}
				continue;
			}
			List<PerkTrigger> list = item.ILLIKOPBPIK(LFLGCDNKNJI);
			if (list == null)
			{
				continue;
			}
			PerkEvent.EventStruct pJEJIOPNBIJ = new PerkEvent.EventStruct();
			pJEJIOPNBIJ.Type = LFLGCDNKNJI;
			pJEJIOPNBIJ.Info = ((!GMFCKPBJNLC) ? null : OFKIKABKDFD());
			pJEJIOPNBIJ.BMIGEFANCCC = item.get_Model();
			pJEJIOPNBIJ.BIKLKJMNGKP = FAJBDBKEHJL;
			pJEJIOPNBIJ.Namespace = fILIJOFBNMA;
			foreach (PerkTrigger item2 in list)
			{
				FOMKDCMBDJD(item, FAJBDBKEHJL, pJEJIOPNBIJ, item2);
			}
		}
		if (CPBHKJFPFJB == null)
		{
			Run();
		}
		return true;
	}

	private void FOMKDCMBDJD(PerkModelStruct MAEPLNACFKD, Model FAJBDBKEHJL, PerkEvent.EventStruct EJMEALJNNIL, PerkTrigger CPBHKJFPFJB, bool CAPNMPNNBHF = false)
	{
		PerkData mFKICNALNFB = MAEPLNACFKD.DCGNMCFLDFD(CPBHKJFPFJB.JMDLAMHAJLN());
		if (mFKICNALNFB != null && mFKICNALNFB.Enabled)
		{
			InfoPerk bPDFFLADJMJ = BELALEGDCDM(MAEPLNACFKD, CPBHKJFPFJB.JMDLAMHAJLN());
			List<string> nIKHAICFGNM = ((bPDFFLADJMJ == null) ? new List<string>() : bPDFFLADJMJ.BFKDLIMHGFA());
			CPBHKJFPFJB.JMDLAMHAJLN().LPHBKEKMPEH(MAEPLNACFKD.get_Model());
			if ((EJMEALJNNIL == null || CPBHKJFPFJB.MIMBCGNGGHO(EJMEALJNNIL)) && CPBHKJFPFJB.IPFOGLIBLLB(MAEPLNACFKD.get_Model(), nIKHAICFGNM))
			{
				MHHNIPBJNAD(MAEPLNACFKD, FAJBDBKEHJL, CPBHKJFPFJB, CAPNMPNNBHF);
			}
		}
	}

	public void Render()
	{
		GGKJFBGBDGM();
		foreach (PerkModelStruct item in MPJMCCGKEOD)
		{
			foreach (InfoPerk item2 in item.HIPOGANEPMI())
			{
				item2.Render();
			}
		}
	}

	public void AINGCNFDFMM(Model ACENLMONNPA, List<ActionPerk> FFFLNOBCBGL)
	{
		FFFLNOBCBGL.Clear();
		PerkModelStruct iAIBLEELGNK = null;
		InfoPerk bPDFFLADJMJ = null;
		ActionPerk oAJGINIDKJD = null;
		for (int i = 0; i < MPJMCCGKEOD.Count; i++)
		{
			iAIBLEELGNK = MPJMCCGKEOD[i];
			for (int j = 0; j < iAIBLEELGNK.HIPOGANEPMI().Count; j++)
			{
				bPDFFLADJMJ = iAIBLEELGNK.HIPOGANEPMI()[j];
				for (int k = 0; k < bPDFFLADJMJ.HIPOGANEPMI().Count; k++)
				{
					oAJGINIDKJD = bPDFFLADJMJ.HIPOGANEPMI()[k];
					if (oAJGINIDKJD.KJDFJPBIGJC == ACENLMONNPA)
					{
						FFFLNOBCBGL.Add(oAJGINIDKJD);
					}
				}
			}
		}
	}

	public void KCEBAJBMJGF(Model ACENLMONNPA, List<ActionPerk> FFFLNOBCBGL)
	{
		FFFLNOBCBGL.Clear();
		ActionPerk oAJGINIDKJD = null;
		for (int i = 0; i < JLAKGOEOHMN.Count; i++)
		{
			oAJGINIDKJD = JLAKGOEOHMN[i];
			if (oAJGINIDKJD.KJDFJPBIGJC == ACENLMONNPA)
			{
				FFFLNOBCBGL.Add(oAJGINIDKJD);
			}
		}
	}

	public InfoPerk BELALEGDCDM(Model ACENLMONNPA, PerkInfoItem AEFFHJGMNFI)
	{
		foreach (PerkModelStruct item in MPJMCCGKEOD)
		{
			if (ACENLMONNPA != item.get_Model())
			{
				continue;
			}
			foreach (InfoPerk item2 in item.HIPOGANEPMI())
			{
				if (item2.DCMHONAFOGI.MBDDKGIOOGD == AEFFHJGMNFI)
				{
					return item2;
				}
			}
		}
		return null;
	}

	public InfoPerk BELALEGDCDM(PerkModelStruct ACENLMONNPA, PerkInfoItem AEFFHJGMNFI)
	{
		return BELALEGDCDM(ACENLMONNPA.get_Model(), AEFFHJGMNFI);
	}

	public void HLIOEELKFCP(object data)
	{
		Model.DisarmData aADFODEJPHG = (Model.DisarmData)data;
		PerkModelStruct iAIBLEELGNK = FMHMIJIPBFG(aADFODEJPHG.KJDFJPBIGJC);
		foreach (PerkInfoItem item in aADFODEJPHG.NHBIJEEKALC)
		{
			foreach (InfoPerk item2 in iAIBLEELGNK.HIPOGANEPMI())
			{
				if (item2.DCMHONAFOGI.MBDDKGIOOGD == item)
				{
					item2.ClearActions(true);
				}
			}
			iAIBLEELGNK.ANHEJBMHGIL(item, false);
		}
	}

	public void DEHPKPPDIIA()
	{
		foreach (PerkModelStruct item in MPJMCCGKEOD)
		{
			item.ANPCFJGEJPO().ForEach((PerkData DHDMNHCIPEH) =>
			{
				DHDMNHCIPEH.Enabled = true;
			});
		}
	}

	public void PAHPCIFKDEA()
	{
		foreach (PerkModelStruct item in MPJMCCGKEOD)
		{
			item.HIPOGANEPMI().ForEach((InfoPerk DHDMNHCIPEH) =>
			{
				DHDMNHCIPEH.PANKENFPNPN();
			});
		}
	}

	public static void HKMMGCLNJCN(ActionPerk IBODMPMJELJ)
	{
		if (IBODMPMJELJ.AMKJNPOCODK.NKAEEFNNBEN())
		{
			PerkActionModificator cKCICHAIMFL = (PerkActionModificator)IBODMPMJELJ.AMKJNPOCODK;
			if (cKCICHAIMFL != null && !string.IsNullOrEmpty(cKCICHAIMFL.IONIEDIPEGB()))
			{
				if (!PNAALKAHAKG.ContainsKey(cKCICHAIMFL.IONIEDIPEGB()))
					PNAALKAHAKG.Add(cKCICHAIMFL.IONIEDIPEGB(), new List<ActionPerk>());
				List<ActionPerk> oMKIGJOLJJE = PNAALKAHAKG[cKCICHAIMFL.IONIEDIPEGB()];
				oMKIGJOLJJE.AddIfNotExist(IBODMPMJELJ);
			}
		}
	}

	public static void AEMBNMFGDBN(ActionPerk IBODMPMJELJ)
	{
		if (IBODMPMJELJ.AMKJNPOCODK.NKAEEFNNBEN())
		{
			PerkActionModificator cKCICHAIMFL = (PerkActionModificator)IBODMPMJELJ.AMKJNPOCODK;
			if (cKCICHAIMFL != null && PNAALKAHAKG.ContainsKey(cKCICHAIMFL.IONIEDIPEGB()))
			{
				List<ActionPerk> list = PNAALKAHAKG[cKCICHAIMFL.IONIEDIPEGB()];
				list.Remove(IBODMPMJELJ);
			}
		}
	}

	public static void EHFKNCOOCAA()
	{
		PNAALKAHAKG.Clear();
	}

	public static bool CheckModNameInNamespace(string GBHAIILPKFC, string PJPJIBOAFKF)
	{
		if (PNAALKAHAKG.ContainsKey(PJPJIBOAFKF))
		{
			List<ActionPerk> list = PNAALKAHAKG[PJPJIBOAFKF];
			foreach (ActionPerk item in list)
			{
				if (item.AMKJNPOCODK.get_Name().Equals(GBHAIILPKFC))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static ActionPerk AFAGHKFHHIF(string GBHAIILPKFC, string PJPJIBOAFKF)
	{
		if (!PNAALKAHAKG.ContainsKey(PJPJIBOAFKF))
		{
			return null;
		}
		List<ActionPerk> list = PNAALKAHAKG[PJPJIBOAFKF];
		foreach (ActionPerk item in list)
		{
			if (item.AMKJNPOCODK.get_Name().Equals(GBHAIILPKFC))
			{
				return item;
			}
		}
		return null;
	}

	public static List<ActionPerk> DOAECFNPKIO(string PJPJIBOAFKF)
	{
		return (!PNAALKAHAKG.ContainsKey(PJPJIBOAFKF)) ? null : PNAALKAHAKG[PJPJIBOAFKF];
	}

	public static void ANPAFFMJMNG(string name)
	{
		if (string.IsNullOrEmpty(name))
			return;
		int used;
		PerkUsesLeft.TryGetValue(name, out used);
		PerkUsesLeft[name] = used + 1;
	}

	public static bool CanUsePerk(string name)
	{
		// Special Edition shipped this live-service usage hook stubbed to false.
		// Modern perk XML puts <PerkStart/> on normal enchantment procs, so the
		// stub disabled virtually every migrated enchantment.  There is no local
		// per-fight usage cap in this edition; cooldown/mod conditions in the XML
		// are authoritative.  Keep the counter for diagnostics/statistics only.
		return true;
	}

	public void MHHNIPBJNAD(Model FAJBDBKEHJL, PerkTrigger CPBHKJFPFJB, bool CAPNMPNNBHF = false)
	{
		foreach (PerkModelStruct item in MPJMCCGKEOD)
		{
			if (FAJBDBKEHJL == item.get_Model())
			{
				MHHNIPBJNAD(item, FAJBDBKEHJL, CPBHKJFPFJB, CAPNMPNNBHF);
				break;
			}
		}
	}

	public void MHHNIPBJNAD(PerkModelStruct MAEPLNACFKD, Model FAJBDBKEHJL, PerkTrigger CPBHKJFPFJB, bool CAPNMPNNBHF = false)
	{
		InfoPerk bPDFFLADJMJ = BELALEGDCDM(MAEPLNACFKD, CPBHKJFPFJB.JMDLAMHAJLN());
		if (bPDFFLADJMJ == null)
		{
			bPDFFLADJMJ = new InfoPerk();
			bPDFFLADJMJ.DCMHONAFOGI = new PerkData(CPBHKJFPFJB.JMDLAMHAJLN());
			MAEPLNACFKD.HIPOGANEPMI().Add(bPDFFLADJMJ);
		}
		List<ActionPerk> list = new List<ActionPerk>();
		List<PerkAction> list2 = CPBHKJFPFJB.HIPOGANEPMI();
		foreach (PerkAction item in list2)
		{
			ActionPerk oAJGINIDKJD = new ActionPerk();
			oAJGINIDKJD.KJDFJPBIGJC = item.NKLMKGFAGFG(MAEPLNACFKD.get_Model());
			oAJGINIDKJD.BIKLKJMNGKP = FAJBDBKEHJL;
			oAJGINIDKJD.AMKJNPOCODK = item;
			oAJGINIDKJD.PLNNKKBPDJK = false;
			oAJGINIDKJD.KGNDJOLBBJF = 0;
			oAJGINIDKJD.FLNLMIHEDCI = 0;
			if (item.BFJEFNHKPJI() != null)
			{
				FunctionResult dEIHAOLOPLC = item.BFJEFNHKPJI().IBCPKBBAFNH();
				oAJGINIDKJD.FLNLMIHEDCI = dEIHAOLOPLC.ToInt();
			}
			if (CAPNMPNNBHF)
			{
				list.Add(oAJGINIDKJD);
			}
			else
			{
				bPDFFLADJMJ.MNLNLKOJPHO().Add(oAJGINIDKJD);
			}
		}
		if (list.Count > 0)
		{
			bPDFFLADJMJ.MHHNIPBJNAD(list);
		}
	}

	public void OPACOCIKEOL(PerkModelStruct ACENLMONNPA, PerkInfoItem AEFFHJGMNFI)
	{
		if (AEFFHJGMNFI != null)
		{
			AEFFHJGMNFI.EIKAGOOJOCN(ACENLMONNPA.GBMCKIKMDNH(), PerkEvent.KNKIIEPDCPN.EVENT_COMBO);
			AEFFHJGMNFI.EIKAGOOJOCN(ACENLMONNPA.DEAADEMBDGN(), PerkEvent.KNKIIEPDCPN.EVENT_EVERY_FRAME);
			AEFFHJGMNFI.EIKAGOOJOCN(ACENLMONNPA.GMCIFKOLPDH(), PerkEvent.KNKIIEPDCPN.EVENT_HIT_PRECRIT);
			AEFFHJGMNFI.EIKAGOOJOCN(ACENLMONNPA.MPMNMFBBDJF(), PerkEvent.KNKIIEPDCPN.EVENT_HIT_POSTCRIT);
			AEFFHJGMNFI.EIKAGOOJOCN(ACENLMONNPA.CPIGPEHCCAJ(), PerkEvent.KNKIIEPDCPN.EVENT_POST_HIT);
			AEFFHJGMNFI.EIKAGOOJOCN(ACENLMONNPA.KBNLBEPMEHH(), PerkEvent.KNKIIEPDCPN.EVENT_MAGIC_CHARGED);
			AEFFHJGMNFI.EIKAGOOJOCN(ACENLMONNPA.GHMGNHPKPGF(), PerkEvent.KNKIIEPDCPN.EVENT_ROUND_STAGE_START);
			AEFFHJGMNFI.EIKAGOOJOCN(ACENLMONNPA.CDLEEPDEFJP(), PerkEvent.KNKIIEPDCPN.EVENT_STYLE);
			AEFFHJGMNFI.EIKAGOOJOCN(ACENLMONNPA.NJOFBEBLCCB(), PerkEvent.KNKIIEPDCPN.EVENT_ANIMATION_START);
			AEFFHJGMNFI.EIKAGOOJOCN(ACENLMONNPA.DOAAGFIINIE(), PerkEvent.KNKIIEPDCPN.EVENT_ANIMATION_END);
			AEFFHJGMNFI.EIKAGOOJOCN(ACENLMONNPA.CFFBHLIPDDF(), PerkEvent.KNKIIEPDCPN.EVENT_MOD_EXPIRES);
			AEFFHJGMNFI.EIKAGOOJOCN(ACENLMONNPA.HHMPEODCJBI(), PerkEvent.KNKIIEPDCPN.EVENT_AREA_ENTER);
			AEFFHJGMNFI.EIKAGOOJOCN(ACENLMONNPA.EIJABICDDFO(), PerkEvent.KNKIIEPDCPN.EVENT_AREA_EXIT);
			AEFFHJGMNFI.EIKAGOOJOCN(ACENLMONNPA.GetIntervalEndTriggers(), PerkEvent.KNKIIEPDCPN.EVENT_INTERVAL_END);
			PerkData item = new PerkData(AEFFHJGMNFI);
			ACENLMONNPA.ANPCFJGEJPO().Add(item);
		}
	}

	public void CLBPEANCNOA(ActionPerk DIMEFLGFIME)
	{
		JLAKGOEOHMN.Add(DIMEFLGFIME);
	}

	private void GGKJFBGBDGM()
	{
		JLAKGOEOHMN.Clear();
	}

	private PerkModelStruct FMHMIJIPBFG(Model ACENLMONNPA)
	{
		return MPJMCCGKEOD.Find((PerkModelStruct DHDMNHCIPEH) => DHDMNHCIPEH.get_Model() == ACENLMONNPA);
	}

	private void DBHDFPCPHEH(Model ACENLMONNPA, PerkEvent.KNKIIEPDCPN LFLGCDNKNJI)
	{
		PerkEventStruct nFFNFAAPEPF = new PerkEventStruct();
		nFFNFAAPEPF.KJDFJPBIGJC = ACENLMONNPA;
		nFFNFAAPEPF.DJPLGDJCMPI = LFLGCDNKNJI;
		if (LFLGCDNKNJI == PerkEvent.KNKIIEPDCPN.EVENT_MOD_EXPIRES)
		{
			nFFNFAAPEPF.Data = ((!OFKIKABKDFD().ContainsKey("ModExpires")) ? null : OFKIKABKDFD()["ModExpires"]);
			CallEvent((int)LFLGCDNKNJI, nFFNFAAPEPF);
		}
	}
}
