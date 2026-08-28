using System.Collections.Generic;
using SF2.Offline;

public static class ICFMIHIKGOD
{
	private static ADEKACKLIJG PKKAGCHKEKH = new ADEKACKLIJG();

	private static FNEEAGNNFNN IJDIOGKIAMM;

	private static List<JLDHCFFAIPK> OEHDEFBEPKP = new List<JLDHCFFAIPK>();

	private static List<JLDHCFFAIPK> HPJDBKGAHPM = new List<JLDHCFFAIPK>();

	public static ADEKACKLIJG KOKELGMOCJD
	{
		get
		{
			return OFFDIMCJOIC();
		}
	}

	public static List<JLDHCFFAIPK> KBPCOJPFLAJ
	{
		get
		{
			return MDLJADJGDOL();
		}
	}

	public static List<JLDHCFFAIPK> OCPPIHGIACB
	{
		get
		{
			return JJIKCAEFPIO();
		}
	}

	public static bool AEHOPJPBMHH
	{
		get
		{
			return GPGDLGFJBHJ();
		}
	}

	public static bool ODDGLIEHGFK
	{
		get
		{
			return LHGPKEFEHDH();
		}
	}

	public static ADEKACKLIJG OFFDIMCJOIC()
	{
		return PKKAGCHKEKH;
	}

	public static void Init(FNEEAGNNFNN ONDHILAOLIM, JNEBPDNJFJG IHLKACMLEGK, ProductDefinition[] OCMDJBDPLJK, Dictionary<string, object> PCJAKPJMKGN = null)
	{
		IJDIOGKIAMM = ONDHILAOLIM;
		// Keep UI subscriptions on the inert local facade; no store or verification service.
	}

	public static List<JLDHCFFAIPK> MDLJADJGDOL()
	{
		return OEHDEFBEPKP;
	}

	public static List<JLDHCFFAIPK> JJIKCAEFPIO()
	{
		return HPJDBKGAHPM;
	}

	public static bool GPGDLGFJBHJ()
	{
		if (HPJDBKGAHPM.Count == 0)
		{
			return false;
		}
		foreach (JLDHCFFAIPK item in HPJDBKGAHPM)
		{
			if (item.IIHBCPBNCCB())
			{
				return true;
			}
		}
		return false;
	}

	public static bool LHGPKEFEHDH()
	{
		return false;
	}

	public static JLDHCFFAIPK GDFHMOEGPMD(string ODJCLFJHKFP, string BGMLFNGKDHI, string DNHKNDPBGNM, string BGLGHEMMANM)
	{
		JLDHCFFAIPK jLDHCFFAIPK = JLDHCFFAIPK.KMIMHNOGDBI(ODJCLFJHKFP, BGMLFNGKDHI, DNHKNDPBGNM, BGLGHEMMANM);
		OEHDEFBEPKP.Add(jLDHCFFAIPK);
		IJDIOGKIAMM.GGGEHAGCLGC(true);
		return jLDHCFFAIPK;
	}

	public static JLDHCFFAIPK PHLBLFONMEP(string ODJCLFJHKFP, string BGMLFNGKDHI, string DNHKNDPBGNM, string BGLGHEMMANM)
	{
		JLDHCFFAIPK jLDHCFFAIPK = JLDHCFFAIPK.KADDCOFNAEC(ODJCLFJHKFP, BGMLFNGKDHI, DNHKNDPBGNM, BGLGHEMMANM);
		HPJDBKGAHPM.Add(jLDHCFFAIPK);
		IJDIOGKIAMM.GGGEHAGCLGC(true);
		return jLDHCFFAIPK;
	}

	public static JLDHCFFAIPK LGFAKOHJOGK(string ODJCLFJHKFP, string BGMLFNGKDHI, string DNHKNDPBGNM, string BGLGHEMMANM, string PPJBKHKCONC)
	{
		JLDHCFFAIPK jLDHCFFAIPK = JLDHCFFAIPK.PCDJBFCLKED(ODJCLFJHKFP, BGMLFNGKDHI, DNHKNDPBGNM, BGLGHEMMANM, PPJBKHKCONC);
		HPJDBKGAHPM.Add(jLDHCFFAIPK);
		IJDIOGKIAMM.GGGEHAGCLGC(true);
		return jLDHCFFAIPK;
	}

	public static JLDHCFFAIPK NBNKJBANGPD(string ODJCLFJHKFP, string BGMLFNGKDHI, string DNHKNDPBGNM, string BGLGHEMMANM, bool BGBMBECEGFH)
	{
		JLDHCFFAIPK jLDHCFFAIPK = JLDHCFFAIPK.DBNJNPBIJGD(ODJCLFJHKFP, BGMLFNGKDHI, DNHKNDPBGNM, BGLGHEMMANM);
		jLDHCFFAIPK.CCLPNJMEMCG(BGBMBECEGFH);
		HPJDBKGAHPM.Add(jLDHCFFAIPK);
		IJDIOGKIAMM.GGGEHAGCLGC(true);
		return jLDHCFFAIPK;
	}

	public static void DCPEBKEGOHG()
	{
		// Preserve any recovered pending transactions without contacting a store
		// or verification backend when the payment UI opens.
	}

	public static void AGLGFEGPGHH(JLDHCFFAIPK PAENLDALDGB)
	{
		OEHDEFBEPKP.Remove(PAENLDALDGB);
		PAENLDALDGB.AGLGFEGPGHH();
		HPJDBKGAHPM.Add(PAENLDALDGB);
		IJDIOGKIAMM.GGGEHAGCLGC(true);
	}

	public static void KMCDDMLIAOP(JLDHCFFAIPK PAENLDALDGB)
	{
		PAENLDALDGB.KMCDDMLIAOP();
		IJDIOGKIAMM.GGGEHAGCLGC(true);
	}

	public static void NHMONODAIFA(JLDHCFFAIPK PAENLDALDGB)
	{
		OEHDEFBEPKP.Remove(PAENLDALDGB);
		HPJDBKGAHPM.Add(PAENLDALDGB);
		IJDIOGKIAMM.GGGEHAGCLGC(true);
	}

	public static void OICIDLANHHN(JLDHCFFAIPK PAENLDALDGB)
	{
		OEHDEFBEPKP.Remove(PAENLDALDGB);
		PAENLDALDGB.EEKOCPGOMEB();
		HPJDBKGAHPM.Add(PAENLDALDGB);
		IJDIOGKIAMM.GGGEHAGCLGC(true);
	}

	public static bool AHJBMFHMAAK(JLDHCFFAIPK PAENLDALDGB)
	{
		for (int i = 0; i < OEHDEFBEPKP.Count; i++)
		{
			if (OEHDEFBEPKP[i].EJFAHFANGFM() == PAENLDALDGB.EJFAHFANGFM())
			{
				return false;
			}
		}
		return true;
	}

	public static bool JBLCINPOOEM(string ODJCLFJHKFP, string BGMLFNGKDHI = null)
	{
		foreach (JLDHCFFAIPK item in OEHDEFBEPKP)
		{
			if (item.JLDEALIEEJI() == ODJCLFJHKFP && (BGMLFNGKDHI == null || item.EJFAHFANGFM() == BGMLFNGKDHI))
			{
				return true;
			}
		}
		return false;
	}

	public static bool MNONELGNFNM(string ODJCLFJHKFP, string BGMLFNGKDHI = null)
	{
		foreach (JLDHCFFAIPK item in HPJDBKGAHPM)
		{
			if (item.JLDEALIEEJI() == ODJCLFJHKFP && (BGMLFNGKDHI == null || item.EJFAHFANGFM() == BGMLFNGKDHI))
			{
				return true;
			}
		}
		return false;
	}
}
