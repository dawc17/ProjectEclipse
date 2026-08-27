using System.Collections.Generic;
using Nekki.SF2.GUI.Map;

public class CountersFight : global::EventDispatcher<object>
{
	public enum OFMIJLDFHBO
	{
		OnCounterIncrement = 0
	}

	public class CurrentCounter
	{
		public Counter EOGLBDCLMBM;

		public int Value;

		public bool IsNot;

		public void KPPJMFDMFBK()
		{
			Value++;
		}
	}

	public class IONIIPMOMAO
	{
		public Dictionary<string, CurrentCounter> JFPJCGPONGM = new Dictionary<string, CurrentCounter>();
	}

	private bool isFirstBlock;

	private ModelParameters IHEFAMAFBIA;

	private Dictionary<string, Counter> CJEMDKDMOKN = new Dictionary<string, Counter>();

	private IONIIPMOMAO EJPOJJKKICO = new IONIIPMOMAO();

	private CounterConditions EMPNPOMEAPL = new CounterConditions();

	public void Init(Dictionary<string, Counter> GGOFNBMGJAF, ModelParameters KKNOCIPBIIK, BattleType JBJHPJMJNNF, float ratio)
	{
		EMPNPOMEAPL.BattleType = JBJHPJMJNNF;
		EMPNPOMEAPL.Ratio = ratio;
		CJEMDKDMOKN = GGOFNBMGJAF;
		IHEFAMAFBIA = KKNOCIPBIIK;
		isFirstBlock = false;
		foreach (KeyValuePair<string, Counter> item in CJEMDKDMOKN)
		{
			CurrentCounter pEMLBKDIDHA = new CurrentCounter();
			pEMLBKDIDHA.EOGLBDCLMBM = item.Value;
			pEMLBKDIDHA.Value = 0;
			EJPOJJKKICO.JFPJCGPONGM[item.Key] = pEMLBKDIDHA;
		}
	}

	public void GCDLFJGEPNI()
	{
		LLLOJBFMONN.INNGABABJPC("Counters ------------------------------------- ");
		foreach (KeyValuePair<string, CurrentCounter> item in EJPOJJKKICO.JFPJCGPONGM)
		{
			LLLOJBFMONN.INNGABABJPC("Counter: {0} -- Value: {1})", item.Key, item.Value.Value);
		}
	}

	public void OPLKJKPHHOH()
	{
		CallCountersByType("NoLose");
	}

	public void DFONENABHBO()
	{
		CallCountersByType("BossNoLose");
	}

	public void LCFPCDJLDLH()
	{
		CallCountersByType("Enchantments");
	}

	public void DMBJKBBFMPH()
	{
		CallCountersByType("PerfectRound");
	}

	public void PMKNEKPKFFA()
	{
		CallCountersByType("Losses");
	}

	public void NDJHKKLEGPC()
	{
		CallCountersByType("ShockWin");
	}

	public void GEAEKJJBMDG()
	{
		CallCountersByType("BodyguardsWin");
	}

	public void MGKKANDMALJ()
	{
		CallCountersByType("BossWin");
	}

	public void GFENMJJDLCL()
	{
		CallCountersByType("TournamentsBeaten");
	}

	public void HKBNCNMLAHK()
	{
		CallCountersByType("DailyBeaten");
	}

	public void MGANFEMKLPM()
	{
		CallCountersByType("ChallangesBeaten");
	}

	public void LAHGOBJIOOG()
	{
		CallCountersByType("Challanges2Beaten");
	}

	public void PMEOOPEEAEM()
	{
		CallCountersByType("MaximumLevel");
	}

	public void PIPGPHELPPK()
	{
		CallCountersByType("DifficultyWin");
	}

	public void MEFALNAFBNG(FightIDS DIAIIPCBMFL)
	{
		List<CurrentCounter> list = DNMGGIKGNNP("WinBattle");
		foreach (CurrentCounter item in list)
		{
			if (item.EOGLBDCLMBM.CAIPCEHIBOO(DIAIIPCBMFL))
			{
				CFCCEPKGEAH(item);
			}
		}
	}

	public void MKIPHHMHIOC(int PKHDLOGJKAD)
	{
		List<CurrentCounter> list = DNMGGIKGNNP("ComboCount");
		foreach (CurrentCounter item in list)
		{
			if ((float)PKHDLOGJKAD >= item.EOGLBDCLMBM.Value)
			{
				CFCCEPKGEAH(item);
			}
		}
	}

	public void HFCLLLHJBGH(int PKHDLOGJKAD)
	{
		List<CurrentCounter> list = DNMGGIKGNNP("Style");
		foreach (CurrentCounter item in list)
		{
			if ((float)PKHDLOGJKAD >= item.EOGLBDCLMBM.Value)
			{
				CFCCEPKGEAH(item);
			}
		}
	}

	public void IHANMCFEJJG(FightIDS DIAIIPCBMFL)
	{
		List<CurrentCounter> list = DNMGGIKGNNP("FightBeaten");
		foreach (CurrentCounter item in list)
		{
			if (DIAIIPCBMFL.Equals(item.EOGLBDCLMBM.IOJFIFODOKO) || DIAIIPCBMFL.Equals(item.EOGLBDCLMBM.FHAGEKGLJOI))
			{
				CFCCEPKGEAH(item);
			}
		}
	}

	public void JKOBOBJMDDE(int value)
	{
		List<CurrentCounter> list = DNMGGIKGNNP("SurvivalRounds");
		foreach (CurrentCounter item in list)
		{
			if ((float)value >= item.EOGLBDCLMBM.Value)
			{
				CFCCEPKGEAH(item);
			}
		}
	}

	public void SetLife(float value)
	{
		List<CurrentCounter> list = DNMGGIKGNNP("HealthRemained");
		foreach (CurrentCounter item in list)
		{
			if (value <= item.EOGLBDCLMBM.Value)
			{
				CFCCEPKGEAH(item);
			}
		}
	}

	public void SetTime(int value)
	{
		List<CurrentCounter> list = DNMGGIKGNNP("RoundQuicker");
		foreach (CurrentCounter item in list)
		{
			if ((float)value <= item.EOGLBDCLMBM.Value)
			{
				CFCCEPKGEAH(item);
			}
		}
		List<CurrentCounter> list2 = DNMGGIKGNNP("RoundLonger");
		foreach (CurrentCounter item2 in list2)
		{
			if ((float)value >= item2.EOGLBDCLMBM.Value)
			{
				CFCCEPKGEAH(item2);
			}
		}
	}

	public void OHHKIAMNCKI(bool OOCLHFGEPML)
	{
		isFirstBlock = true;
		CurrentCounter pEMLBKDIDHA = BECPDHFPNFC("BlockedRound");
		if (!OOCLHFGEPML && pEMLBKDIDHA != null && !pEMLBKDIDHA.IsNot)
		{
			pEMLBKDIDHA.IsNot = true;
		}
	}

	public void NELEDHIIDCG(InfoAnimation DBOLBEOCEME, bool APLJLFHDJIM, bool isFirstStrike, bool INDFLCGLJPP, bool LGNDOAHHHNP, bool OOCLHFGEPML, bool EPKEEMFHHFM)
	{
		if (!OOCLHFGEPML && isFirstStrike)
		{
			List<CurrentCounter> list = DNMGGIKGNNP("FirstHits");
			foreach (CurrentCounter item in list)
			{
				CFCCEPKGEAH(item);
			}
		}
		if (INDFLCGLJPP)
		{
			List<CurrentCounter> list2 = DNMGGIKGNNP("Disarm");
			foreach (CurrentCounter item2 in list2)
			{
				CFCCEPKGEAH(item2);
			}
		}
		List<CurrentCounter> list3 = DNMGGIKGNNP("RestrictedAnimation");
		foreach (CurrentCounter item3 in list3)
		{
			if (!item3.IsNot && !DBOLBEOCEME.LPPIKDGABOL(item3.EOGLBDCLMBM.FGICHADOEHF))
			{
				item3.IsNot = true;
			}
			string jIIFFJAJNNN = item3.EOGLBDCLMBM.JIIFFJAJNNN;
			if (IHEFAMAFBIA.JGMLKIPCFII != null && jIIFFJAJNNN != string.Empty && IHEFAMAFBIA.JGMLKIPCFII.Name != jIIFFJAJNNN)
			{
				item3.IsNot = true;
			}
			if (LGNDOAHHHNP && !item3.IsNot)
			{
				CFCCEPKGEAH(item3);
			}
		}
		if (LGNDOAHHHNP)
		{
			if (APLJLFHDJIM)
			{
				List<CurrentCounter> list4 = DNMGGIKGNNP("HeadHitRound");
				foreach (CurrentCounter item4 in list4)
				{
					CFCCEPKGEAH(item4);
				}
			}
			CurrentCounter pEMLBKDIDHA = BECPDHFPNFC("BlockedRound");
			if (pEMLBKDIDHA != null && !pEMLBKDIDHA.IsNot && isFirstBlock)
			{
				CFCCEPKGEAH(pEMLBKDIDHA);
			}
		}
		if (OOCLHFGEPML || !APLJLFHDJIM)
		{
			return;
		}
		List<CurrentCounter> list5 = DNMGGIKGNNP("HeadKick");
		foreach (CurrentCounter item5 in list5)
		{
			if (item5 != null && DBOLBEOCEME.LPPIKDGABOL(item5.EOGLBDCLMBM.FGICHADOEHF))
			{
				CFCCEPKGEAH(item5);
			}
		}
	}

	public void CallCountersByType(string KFLJDKNOPCE)
	{
		List<CurrentCounter> list = DNMGGIKGNNP(KFLJDKNOPCE);
		foreach (CurrentCounter item in list)
		{
			CFCCEPKGEAH(item);
		}
	}

	public void Complete(int roundTotal, bool CDCEOCEPMPK = false)
	{
		if (CDCEOCEPMPK)
		{
			return;
		}
		foreach (KeyValuePair<string, CurrentCounter> item in EJPOJJKKICO.JFPJCGPONGM)
		{
			CurrentCounter value = item.Value;
			if (value.EOGLBDCLMBM.CHDEIEMINPF(EMPNPOMEAPL) && value.EOGLBDCLMBM.KKNOICPMJPO == Counter.IPENPHOAEGL.SPAN_FIGHT)
			{
				value.Value = ((value.Value == roundTotal) ? 1 : 0);
				CallEvent(0, value);
			}
		}
	}

	public void HOCBEHCHOFL(bool CDCEOCEPMPK)
	{
		foreach (KeyValuePair<string, Counter> item in CJEMDKDMOKN)
		{
			if (!CDCEOCEPMPK || !item.Value.IsFightEnd)
			{
				item.Value.CompleteValue = EJPOJJKKICO.JFPJCGPONGM[item.Key].Value;
			}
		}
		GCDLFJGEPNI();
	}

	public void MLJCABABNDB()
	{
		isFirstBlock = false;
		foreach (KeyValuePair<string, CurrentCounter> item in EJPOJJKKICO.JFPJCGPONGM)
		{
			CurrentCounter value = item.Value;
			if (value.EOGLBDCLMBM.KKNOICPMJPO == Counter.IPENPHOAEGL.SPAN_ROUND)
			{
				value.IsNot = false;
			}
		}
	}

	private void CFCCEPKGEAH(CurrentCounter EPJGLECOIBG)
	{
		bool flag = CheckFightType(EPJGLECOIBG.EOGLBDCLMBM.DEGIADEEFGG);
		bool flag2 = CheckDifficult(EPJGLECOIBG.EOGLBDCLMBM.MJOJIPKLJOL, EPJGLECOIBG.EOGLBDCLMBM.GAHBCLAMANC);
		if (EPJGLECOIBG.EOGLBDCLMBM.CHDEIEMINPF(EMPNPOMEAPL) && flag && flag2)
		{
			EPJGLECOIBG.KPPJMFDMFBK();
			if (EPJGLECOIBG.EOGLBDCLMBM.KKNOICPMJPO != Counter.IPENPHOAEGL.SPAN_FIGHT)
			{
				CallEvent(0, EPJGLECOIBG);
			}
		}
	}

	private List<CurrentCounter> DNMGGIKGNNP(string LFLGCDNKNJI)
	{
		List<CurrentCounter> list = new List<CurrentCounter>();
		foreach (KeyValuePair<string, CurrentCounter> item in EJPOJJKKICO.JFPJCGPONGM)
		{
			if (item.Value.EOGLBDCLMBM.Type == LFLGCDNKNJI)
			{
				list.Add(item.Value);
			}
		}
		return list;
	}

	private CurrentCounter BECPDHFPNFC(string name)
	{
		if (EJPOJJKKICO.JFPJCGPONGM.ContainsKey(name))
		{
			return EJPOJJKKICO.JFPJCGPONGM[name];
		}
		return null;
	}

	private bool CheckFightType(string MPBIEONNLIJ)
	{
		if (MPBIEONNLIJ == null || MPBIEONNLIJ == string.Empty)
		{
			return true;
		}
		string[] array = MPBIEONNLIJ.Split('|');
		for (int i = 0; i < array.Length; i++)
		{
			BattleType pJMEMGHKKBM = ListSF.ELEBLBJKDBI().HIDKFHHJBDH(array[i]);
			if (pJMEMGHKKBM == EMPNPOMEAPL.BattleType)
			{
				return true;
			}
		}
		return false;
	}

	private bool CheckDifficult(string HBACJNBIFCH, string EBGLLCMNIED)
	{
		if (HBACJNBIFCH == string.Empty && EBGLLCMNIED == string.Empty)
		{
			return true;
		}
		bool flag = HBACJNBIFCH != string.Empty;
		bool flag2 = EBGLLCMNIED != string.Empty;
		float num = GetRationForDifficult(HBACJNBIFCH);
		float num2 = GetRationForDifficult(EBGLLCMNIED);
		if (flag && flag2)
		{
			return EMPNPOMEAPL.Ratio > num && EMPNPOMEAPL.Ratio < num2;
		}
		if (flag)
		{
			return EMPNPOMEAPL.Ratio > num;
		}
		if (flag2)
		{
			return EMPNPOMEAPL.Ratio < num2;
		}
		return false;
	}

	private float GetRationForDifficult(string DOACAKFFFPB)
	{
		List<global::Pair<string, float>> difficultyEvaluation = DifficultyPanel.get_DifficultyEvaluation();
		for (int i = 0; i < difficultyEvaluation.Count; i++)
		{
			global::Pair<string, float> cCKLNOPEKHO = difficultyEvaluation[i];
			if (cCKLNOPEKHO.First == DOACAKFFFPB)
			{
				return cCKLNOPEKHO.Second;
			}
		}
		return DOACAKFFFPB.ToFloat();
	}
}
