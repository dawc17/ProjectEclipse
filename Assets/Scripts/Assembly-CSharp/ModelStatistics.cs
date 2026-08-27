using System.Collections.Generic;
using UnityEngine;

public class ModelStatistics
{
	private class AttackStatistics
	{
		private InfoAnimation IBMJEFJBOIK;

		private float CJOJDEOLBND;

		private float JOGLLIIGDMN;

		private float _count;

		private float ONOLEKBJIEJ;

		private float AFFJHCAGHGI;

		private int _strikeIndex;

		public InfoAnimation FGICHADOEHF
		{
			get
			{
				return NNMAFFCCMHC();
			}
		}

		public float EMHIAONANOJ
		{
			get
			{
				return PANEJCCDFFP();
			}
			set
			{
				ENCKIHKPKCO(value);
			}
		}

		public float KFMJMBANIGF
		{
			get
			{
				return GHGGNMBCMNM();
			}
			set
			{
				PJEADIKBIGL(value);
			}
		}

		public float Count
		{
			get
			{
				return OFOPFCJNEBL();
			}
			set
			{
				CHILOKHFALD(value);
			}
		}

		public float AAKOCIPFDNM
		{
			get
			{
				return POKBOKHJJPL();
			}
			set
			{
				BEHDBJOCHGM(value);
			}
		}

		public AttackStatistics(InfoAnimation DBOLBEOCEME)
		{
			IBMJEFJBOIK = DBOLBEOCEME;
			JOGLLIIGDMN = 0f;
			CJOJDEOLBND = 0f;
			_count = 0f;
			AFFJHCAGHGI = 0f;
			ONOLEKBJIEJ = 0f;
			_strikeIndex = 0;
		}

		public AttackStatistics(AttackStatistics NBMGOEMJJAF)
		{
			IBMJEFJBOIK = NBMGOEMJJAF.IBMJEFJBOIK;
			JOGLLIIGDMN = NBMGOEMJJAF.JOGLLIIGDMN;
			CJOJDEOLBND = NBMGOEMJJAF.CJOJDEOLBND;
			_count = NBMGOEMJJAF._count;
			AFFJHCAGHGI = NBMGOEMJJAF.AFFJHCAGHGI;
			ONOLEKBJIEJ = NBMGOEMJJAF.ONOLEKBJIEJ;
			_strikeIndex = 0;
		}

		public InfoAnimation NNMAFFCCMHC()
		{
			return IBMJEFJBOIK;
		}

		public float PANEJCCDFFP()
		{
			return CJOJDEOLBND;
		}

		public void ENCKIHKPKCO(float value)
		{
			CJOJDEOLBND = value;
		}

		public float GHGGNMBCMNM()
		{
			return JOGLLIIGDMN;
		}

		public void PJEADIKBIGL(float value)
		{
			JOGLLIIGDMN = value;
		}

		public float OFOPFCJNEBL()
		{
			return _count;
		}

		public void CHILOKHFALD(float value)
		{
			_count = value;
		}

		public float POKBOKHJJPL()
		{
			return AFFJHCAGHGI;
		}

		public void BEHDBJOCHGM(float value)
		{
			AFFJHCAGHGI = value;
		}

		private void NNNFJNLCMMO(int BCAOGKPNMFG, float DAIGFEOMFIE)
		{
			int num = BCAOGKPNMFG - _strikeIndex;
			if (0 < num)
			{
				float num2 = Mathf.Pow(2f, (0f - (float)num) / DAIGFEOMFIE);
				JOGLLIIGDMN *= num2;
				CJOJDEOLBND *= num2;
				_count *= num2;
				AFFJHCAGHGI *= num2;
				ONOLEKBJIEJ *= num2;
			}
			_strikeIndex = BCAOGKPNMFG;
		}

		public void LPCJBPFDFLD(float CKKFKEIELCP, int BCAOGKPNMFG, float DAIGFEOMFIE)
		{
			NNNFJNLCMMO(BCAOGKPNMFG, DAIGFEOMFIE);
			CJOJDEOLBND += CKKFKEIELCP;
			ONOLEKBJIEJ++;
		}

		public void PIOIIIMCFMJ()
		{
			JOGLLIIGDMN += CJOJDEOLBND;
			CJOJDEOLBND = 0f;
			AFFJHCAGHGI += ONOLEKBJIEJ;
			ONOLEKBJIEJ = 0f;
		}

		public void NFPKBOGGPBA(int BCAOGKPNMFG, float DAIGFEOMFIE)
		{
			NNNFJNLCMMO(BCAOGKPNMFG, DAIGFEOMFIE);
			_count++;
		}

		public float KDPAKCJCNMI(int BCAOGKPNMFG, float DAIGFEOMFIE)
		{
			NNNFJNLCMMO(BCAOGKPNMFG, DAIGFEOMFIE);
			return JOGLLIIGDMN;
		}

		public float GCGFLOAGMHP(int BCAOGKPNMFG, float DAIGFEOMFIE)
		{
			NNNFJNLCMMO(BCAOGKPNMFG, DAIGFEOMFIE);
			return _count;
		}

		public float KELJBLCCOMH(int BCAOGKPNMFG, float DAIGFEOMFIE)
		{
			NNNFJNLCMMO(BCAOGKPNMFG, DAIGFEOMFIE);
			return AFFJHCAGHGI;
		}

		public void CIJNPJFOBHI(float ratio)
		{
			JOGLLIIGDMN *= ratio;
			CJOJDEOLBND *= ratio;
			_count *= ratio;
			AFFJHCAGHGI *= ratio;
			ONOLEKBJIEJ *= ratio;
		}
	}

	private Model _model;

	private Dictionary<InfoAnimation, AttackStatistics> OJMNFIAGGFI = new Dictionary<InfoAnimation, AttackStatistics>();

	private Dictionary<InfoAnimation, AttackStatistics> HIJMNFKPJOJ = new Dictionary<InfoAnimation, AttackStatistics>();

	private List<InfoAnimation> PIHGLDCAEIF = new List<InfoAnimation>();

	private static AttackStatistics FINNGOADIDP;

	private int IKBOCLPGIEP;

	private int KCFFFLKIDLN;

	public bool ECMICCIOLLM
	{
		get
		{
			return FMGDKLFNKGM();
		}
	}

	public ModelStatistics(Model ACENLMONNPA)
	{
		_model = ACENLMONNPA;
	}

	private AttackStatistics IAMEMEDKMOB(bool MNJPFPLKNFA, InfoAnimation DBOLBEOCEME)
	{
		Dictionary<InfoAnimation, AttackStatistics> dictionary = ((!MNJPFPLKNFA) ? HIJMNFKPJOJ : OJMNFIAGGFI);
		if (dictionary.ContainsKey(DBOLBEOCEME))
		{
			return dictionary[DBOLBEOCEME];
		}
		AttackStatistics iINOIHKEDDJ = new AttackStatistics(DBOLBEOCEME);
		dictionary.Add(DBOLBEOCEME, iINOIHKEDDJ);
		return iINOIHKEDDJ;
	}

	public void LPCJBPFDFLD(bool MNJPFPLKNFA, InfoAnimation DBOLBEOCEME, float CKKFKEIELCP)
	{
		int bCAOGKPNMFG = _model.EJJIGHLCKEN();
		float dAIGFEOMFIE = KDGHCGHAIDA();
		AttackStatistics iINOIHKEDDJ = IAMEMEDKMOB(MNJPFPLKNFA, DBOLBEOCEME);
		iINOIHKEDDJ.LPCJBPFDFLD(CKKFKEIELCP, bCAOGKPNMFG, dAIGFEOMFIE);
	}

	public void PIOIIIMCFMJ(bool MNJPFPLKNFA, InfoAnimation DBOLBEOCEME)
	{
		AttackStatistics iINOIHKEDDJ = IAMEMEDKMOB(MNJPFPLKNFA, DBOLBEOCEME);
		iINOIHKEDDJ.PIOIIIMCFMJ();
	}

	public void NFPKBOGGPBA(bool MNJPFPLKNFA, InfoAnimation DBOLBEOCEME)
	{
		int bCAOGKPNMFG = _model.EJJIGHLCKEN();
		float dAIGFEOMFIE = KDGHCGHAIDA();
		AttackStatistics iINOIHKEDDJ = IAMEMEDKMOB(MNJPFPLKNFA, DBOLBEOCEME);
		iINOIHKEDDJ.NFPKBOGGPBA(bCAOGKPNMFG, dAIGFEOMFIE);
	}

	public void Reset()
	{
		OJMNFIAGGFI.Clear();
		HIJMNFKPJOJ.Clear();
	}

	public void MLJCABABNDB()
	{
		foreach (AttackStatistics value in OJMNFIAGGFI.Values)
		{
			value.CHILOKHFALD(value.OFOPFCJNEBL() * GGLHHPCOABD());
			value.PJEADIKBIGL(value.GHGGNMBCMNM() * GGLHHPCOABD());
			value.ENCKIHKPKCO(value.PANEJCCDFFP() * GGLHHPCOABD());
			value.BEHDBJOCHGM(value.POKBOKHJJPL() * GGLHHPCOABD());
			value.CIJNPJFOBHI(GGLHHPCOABD());
		}
		foreach (AttackStatistics value2 in HIJMNFKPJOJ.Values)
		{
			value2.CHILOKHFALD(value2.OFOPFCJNEBL() * GGLHHPCOABD());
			value2.PJEADIKBIGL(value2.GHGGNMBCMNM() * GGLHHPCOABD());
			value2.ENCKIHKPKCO(value2.PANEJCCDFFP() * GGLHHPCOABD());
			value2.BEHDBJOCHGM(value2.POKBOKHJJPL() * GGLHHPCOABD());
			value2.CIJNPJFOBHI(GGLHHPCOABD());
		}
	}

	public void GetCountAndDamage(bool MNJPFPLKNFA, string KCAIJCBMNKP, ref float count, ref float CKKFKEIELCP, ref float JOOJIMPEPOJ)
	{
		count = 0f;
		CKKFKEIELCP = 0f;
		PIHGLDCAEIF.Clear();
		AnimationData.NEBELEFIDMB(KCAIJCBMNKP, PIHGLDCAEIF);
		float BLJGEOEHIGP2 = 0f;
		float CKKFKEIELCP2 = 0f;
		float JOOJIMPEPOJ2 = 0f;
		for (int i = 0; i < PIHGLDCAEIF.Count; i++)
		{
			GetCountAndDamage(MNJPFPLKNFA, PIHGLDCAEIF[i], ref BLJGEOEHIGP2, ref CKKFKEIELCP2, ref JOOJIMPEPOJ2);
			count += BLJGEOEHIGP2;
			CKKFKEIELCP += CKKFKEIELCP2;
			JOOJIMPEPOJ += JOOJIMPEPOJ2;
		}
	}

	public void GetCountAndDamage(bool MNJPFPLKNFA, InfoAnimation DBOLBEOCEME, ref float count, ref float CKKFKEIELCP, ref float JOOJIMPEPOJ)
	{
		AttackStatistics iINOIHKEDDJ = IAMEMEDKMOB(MNJPFPLKNFA, DBOLBEOCEME);
		int bCAOGKPNMFG = _model.EJJIGHLCKEN();
		float dAIGFEOMFIE = KDGHCGHAIDA();
		count = iINOIHKEDDJ.GCGFLOAGMHP(bCAOGKPNMFG, dAIGFEOMFIE);
		CKKFKEIELCP = iINOIHKEDDJ.KDPAKCJCNMI(bCAOGKPNMFG, dAIGFEOMFIE);
		JOOJIMPEPOJ = iINOIHKEDDJ.KELJBLCCOMH(bCAOGKPNMFG, dAIGFEOMFIE);
	}

	private float KDGHCGHAIDA()
	{
		float result = 0f;
		ModelAi pCFGKAFOCDO = _model.EEIGOJBKFGE();
		if (pCFGKAFOCDO != null)
		{
			Tactic eEJNOAKLOLG = pCFGKAFOCDO.get_Tactic();
			if (eEJNOAKLOLG != null)
			{
				result = eEJNOAKLOLG.DHPIKOMPJEK.CJKKOJCLIGK;
			}
		}
		return result;
	}

	private float GGLHHPCOABD()
	{
		float result = 0f;
		ModelAi pCFGKAFOCDO = _model.EEIGOJBKFGE();
		if (pCFGKAFOCDO != null)
		{
			Tactic eEJNOAKLOLG = pCFGKAFOCDO.get_Tactic();
			if (eEJNOAKLOLG != null)
			{
				result = eEJNOAKLOLG.DHPIKOMPJEK.HHGKEGHMMCP;
			}
		}
		return result;
	}

	public void AddRaidHitInfo(bool OOCLHFGEPML, bool OOGIBOBMGJA)
	{
		if (!OOCLHFGEPML)
		{
			IKBOCLPGIEP++;
		}
		if (OOGIBOBMGJA)
		{
			KCFFFLKIDLN++;
		}
	}

	public bool FMGDKLFNKGM()
	{
		int pOJMKEEPBJK = QuestUtils.BKBHIHMEMEH().JOPIIDEIJEF().CritAdditional;
		float iMPHONCGFGP = QuestUtils.BKBHIHMEMEH().JOPIIDEIJEF().CritProbablity;
		int num = pOJMKEEPBJK + (int)((float)IKBOCLPGIEP * iMPHONCGFGP);
		if (KCFFFLKIDLN + 1 <= num)
		{
			return true;
		}
		return false;
	}
}
