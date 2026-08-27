using System.Collections.Generic;
using System.Xml;

public class Tactic
{
	public enum GKJKJFJALCA
	{
		TacticNone = 0,
		TacticRandom = 1,
		TacticTabular = 2
	}

	public class Memory
	{
		public float CJKKOJCLIGK = 10f;

		public float HHGKEGHMMCP = 10f;

		public Memory()
		{
		}

		public Memory(Memory MOLELAFGIPG)
		{
			CJKKOJCLIGK = MOLELAFGIPG.CJKKOJCLIGK;
			HHGKEGHMMCP = MOLELAFGIPG.HHGKEGHMMCP;
		}
	}

	private GKJKJFJALCA _type;

	public Memory DHPIKOMPJEK = new Memory();

	private string _name = string.Empty;

	private TacticValue EFMIJNCGFOK = new TacticValue();

	private TacticValue LJDANFGALGN = new TacticValue();

	private TacticValue NGDPJOAKAGI = new TacticValue();

	private TacticValue PNHJFDEANCA = new TacticValue();

	private TacticValue MOONHMCDILM = new TacticValue();

	private List<global::Pair<string, TacticValue>> EDMBGBEKOKC = new List<global::Pair<string, TacticValue>>();

	private List<global::Pair<string, TacticValue>> CCKEOHKCGKI = new List<global::Pair<string, TacticValue>>();

	private TacticValue MONEJNIBIIO = new TacticValue();

	private TacticValue IIGKMHEOEGP = new TacticValue();

	private TacticValue NKFHMEKFJOD = new TacticValue();

	private global::Pair<TacticValue, TacticValue> GLOELCCHOEP = new global::Pair<TacticValue, TacticValue>(new TacticValue(), new TacticValue());

	private global::Pair<TacticValue, TacticValue> LMNIHDCLFOI = new global::Pair<TacticValue, TacticValue>(new TacticValue(), new TacticValue());

	private global::Pair<TacticValue, TacticValue> BOFOIBJDMJB = new global::Pair<TacticValue, TacticValue>(new TacticValue(), new TacticValue());

	private global::Pair<TacticValue, TacticValue> EKABOOMHLDJ = new global::Pair<TacticValue, TacticValue>(new TacticValue(), new TacticValue());

	private List<global::Pair<string, TacticValue>> KEPIPBINEDC = new List<global::Pair<string, TacticValue>>();

	private List<global::Pair<string, TacticValue>> FBNEKGKGLLP = new List<global::Pair<string, TacticValue>>();

	public List<global::Pair<string, TacticValue>> NAOLEADGNND
	{
		get
		{
			return get_QuickAttacks();
		}
	}

	public List<global::Pair<string, TacticValue>> NIHGHAHGNDI
	{
		get
		{
			return get_Evades();
		}
	}

	public Tactic()
	{
	}

	public Tactic(XmlNode AFHNINCKJEE)
	{
		_name = AFHNINCKJEE.Attributes["Name"].CIPOICEEIBK(string.Empty);
		_type = GetType(AFHNINCKJEE.Attributes["Type"].CIPOICEEIBK(string.Empty));
		XmlNode xmlNode = AFHNINCKJEE["Memory"];
		if (xmlNode != null)
		{
			DHPIKOMPJEK.CJKKOJCLIGK = xmlNode.Attributes["Strikes"].ParseFloat();
			DHPIKOMPJEK.HHGKEGHMMCP = xmlNode.Attributes["RoundFactor"].ParseFloat();
		}
		XmlNode xmlNode2 = AFHNINCKJEE["UseDefense"];
		if (xmlNode2 != null)
		{
			EFMIJNCGFOK.Parse(xmlNode2["CounterAttackChance"]);
			LJDANFGALGN.Parse(xmlNode2["DodgeChance"]);
			NGDPJOAKAGI.Parse(xmlNode2["BlockChance"]);
		}
		PNHJFDEANCA.Parse(AFHNINCKJEE["UseSafeAttackChance"]);
		MOONHMCDILM.Parse(AFHNINCKJEE["TableAttackChance"]);
		ParseQuickAttacks(AFHNINCKJEE["QuickAttacks"]);
		ParseEvades(AFHNINCKJEE["Evades"]);
		MONEJNIBIIO.Parse(AFHNINCKJEE["CautiousMovementsChance"]);
		IIGKMHEOEGP.Parse(AFHNINCKJEE["DodgeMissilesChance"]);
		NKFHMEKFJOD.Parse(AFHNINCKJEE["DodgeMagicChance"]);
		ParseInterval(AFHNINCKJEE["DistanceError"], GLOELCCHOEP);
		ParseInterval(AFHNINCKJEE["FrameError"], LMNIHDCLFOI);
		ParseInterval(AFHNINCKJEE["ResponseDelay"], BOFOIBJDMJB);
		ParseInterval(AFHNINCKJEE["EnemyResponseDelay"], EKABOOMHLDJ);
		XmlNode xmlNode3 = AFHNINCKJEE["AnimationWeights"];
		if (xmlNode3 != null)
		{
			foreach (XmlNode childNode in xmlNode3.ChildNodes)
			{
				if (childNode.Name == "Animation")
				{
					string gBCLEDJAOBM = childNode.Attributes["Name"].CIPOICEEIBK(string.Empty);
					TacticValue pOFHDGJAFMP = new TacticValue(childNode);
					KEPIPBINEDC.Add(new global::Pair<string, TacticValue>(gBCLEDJAOBM, pOFHDGJAFMP));
				}
			}
		}
		XmlNode xmlNode5 = AFHNINCKJEE["ExpectedWait"];
		if (xmlNode5 == null)
		{
			return;
		}
		foreach (XmlNode childNode2 in xmlNode5.ChildNodes)
		{
			if (childNode2.Name == "Animation")
			{
				string gBCLEDJAOBM2 = childNode2.Attributes["Name"].CIPOICEEIBK(string.Empty);
				TacticValue pOFHDGJAFMP2 = new TacticValue(childNode2);
				FBNEKGKGLLP.Add(new global::Pair<string, TacticValue>(gBCLEDJAOBM2, pOFHDGJAFMP2));
			}
		}
	}

	public Tactic(Tactic BJBIGPGJKIE)
	{
		_name = BJBIGPGJKIE._name;
		_type = BJBIGPGJKIE._type;
		EFMIJNCGFOK = BJBIGPGJKIE.EFMIJNCGFOK;
		LJDANFGALGN = BJBIGPGJKIE.LJDANFGALGN;
		NGDPJOAKAGI = BJBIGPGJKIE.NGDPJOAKAGI;
		PNHJFDEANCA = BJBIGPGJKIE.PNHJFDEANCA;
		MOONHMCDILM = BJBIGPGJKIE.MOONHMCDILM;
		MONEJNIBIIO = BJBIGPGJKIE.MONEJNIBIIO;
		IIGKMHEOEGP = BJBIGPGJKIE.IIGKMHEOEGP;
		NKFHMEKFJOD = BJBIGPGJKIE.NKFHMEKFJOD;
		GLOELCCHOEP = BJBIGPGJKIE.GLOELCCHOEP;
		LMNIHDCLFOI = BJBIGPGJKIE.LMNIHDCLFOI;
		BOFOIBJDMJB = BJBIGPGJKIE.BOFOIBJDMJB;
		EKABOOMHLDJ = BJBIGPGJKIE.EKABOOMHLDJ;
		KEPIPBINEDC = BJBIGPGJKIE.KEPIPBINEDC;
		FBNEKGKGLLP = BJBIGPGJKIE.FBNEKGKGLLP;
		EDMBGBEKOKC = BJBIGPGJKIE.EDMBGBEKOKC;
		CCKEOHKCGKI = BJBIGPGJKIE.CCKEOHKCGKI;
		DHPIKOMPJEK = BJBIGPGJKIE.DHPIKOMPJEK;
	}

	public GKJKJFJALCA get_Type()
	{
		return _type;
	}

	public string get_Name()
	{
		return _name;
	}

	public List<global::Pair<string, TacticValue>> get_QuickAttacks()
	{
		return EDMBGBEKOKC;
	}

	public List<global::Pair<string, TacticValue>> get_Evades()
	{
		return CCKEOHKCGKI;
	}

	public float GetCounterAttackChance(TacticFactors FJCBLOKOBBD)
	{
		return EFMIJNCGFOK.GetValue(FJCBLOKOBBD);
	}

	public float GetDodgeChance(TacticFactors FJCBLOKOBBD)
	{
		return LJDANFGALGN.GetValue(FJCBLOKOBBD);
	}

	public float GetBlockChance(TacticFactors FJCBLOKOBBD)
	{
		return NGDPJOAKAGI.GetValue(FJCBLOKOBBD);
	}

	public float GetUseSafeAttackChance(TacticFactors FJCBLOKOBBD)
	{
		return PNHJFDEANCA.GetValue(FJCBLOKOBBD);
	}

	public float GetTableAttackChance(TacticFactors FJCBLOKOBBD)
	{
		return MOONHMCDILM.GetValue(FJCBLOKOBBD);
	}

	public float GetCautiousMovementsChance(TacticFactors FJCBLOKOBBD)
	{
		return MONEJNIBIIO.GetValue(FJCBLOKOBBD);
	}

	public float GetDodgeMissileChance(TacticFactors FJCBLOKOBBD)
	{
		return IIGKMHEOEGP.GetValue(FJCBLOKOBBD);
	}

	public float GetDodgeMagicChance(TacticFactors FJCBLOKOBBD)
	{
		return NKFHMEKFJOD.GetValue(FJCBLOKOBBD);
	}

	public float GetExpectedWait(InfoAnimation DBOLBEOCEME, TacticFactors FJCBLOKOBBD)
	{
		if (DBOLBEOCEME != null)
		{
			foreach (global::Pair<string, TacticValue> item in FBNEKGKGLLP)
			{
				if (string.IsNullOrEmpty(item.First) || DBOLBEOCEME.CNPFHBMGDFP(item.First))
				{
					return item.Second.GetValue(FJCBLOKOBBD);
				}
			}
		}
		else
		{
			foreach (global::Pair<string, TacticValue> item2 in FBNEKGKGLLP)
			{
				if (string.IsNullOrEmpty(item2.First))
				{
					return item2.Second.GetValue(FJCBLOKOBBD);
				}
			}
		}
		LLLOJBFMONN.Error("Expected Wait ERROR");
		return 1f;
	}

	public int SelectAnimationWithWeights(List<InfoAnimation> MAHEJFLCCHP, InfoAnimation HNCCGJECKLL, TacticFactors FJCBLOKOBBD)
	{
		int count = MAHEJFLCCHP.Count;
		if (0 < count)
		{
			float num = 0f;
			for (int i = 0; i < MAHEJFLCCHP.Count; i++)
			{
				InfoAnimation pJAHIOELGGD = MAHEJFLCCHP[i];
				if (pJAHIOELGGD == null && HNCCGJECKLL != null)
				{
					pJAHIOELGGD = HNCCGJECKLL;
				}
				if (pJAHIOELGGD != null)
				{
					float num2 = GetWeight(pJAHIOELGGD, FJCBLOKOBBD);
					num += num2;
				}
			}
			if (0f < num)
			{
				float num3 = NekkiMath.randomFloat(num);
				int num4 = 0;
				for (int j = 0; j < MAHEJFLCCHP.Count; j++)
				{
					InfoAnimation pJAHIOELGGD2 = MAHEJFLCCHP[j];
					if (pJAHIOELGGD2 == null && HNCCGJECKLL != null)
					{
						pJAHIOELGGD2 = HNCCGJECKLL;
					}
					if (pJAHIOELGGD2 != null)
					{
						float num5 = GetWeight(pJAHIOELGGD2, FJCBLOKOBBD);
						float num6 = num3 - num5;
						if (num6 < 0f)
						{
							return num4;
						}
						num3 = num6;
						num4++;
					}
				}
			}
		}
		return -1;
	}

	public float GetWeight(InfoAnimation DBOLBEOCEME, TacticFactors JCICKLIMBEF)
	{
		foreach (global::Pair<string, TacticValue> item in KEPIPBINEDC)
		{
			string lLHEDBIEHAA = item.First;
			if (lLHEDBIEHAA == string.Empty || DBOLBEOCEME.CNPFHBMGDFP(lLHEDBIEHAA))
			{
				return item.Second.GetValue(JCICKLIMBEF);
			}
		}
		return 0f;
	}

	public float GetDistanceError(TacticFactors FJCBLOKOBBD)
	{
		float lHNCHOAEGEA = GLOELCCHOEP.First.GetValue(FJCBLOKOBBD);
		float kAEPJHHLLPK = GLOELCCHOEP.Second.GetValue(FJCBLOKOBBD);
		return GetValueFromInterval(lHNCHOAEGEA, kAEPJHHLLPK);
	}

	public int GetFrameError(TacticFactors FJCBLOKOBBD)
	{
		float lHNCHOAEGEA = LMNIHDCLFOI.First.GetValue(FJCBLOKOBBD);
		float kAEPJHHLLPK = LMNIHDCLFOI.Second.GetValue(FJCBLOKOBBD);
		return (int)GetValueFromInterval(lHNCHOAEGEA, kAEPJHHLLPK);
	}

	public int GetResponseDelay(TacticFactors FJCBLOKOBBD)
	{
		float lHNCHOAEGEA = BOFOIBJDMJB.First.GetValue(FJCBLOKOBBD);
		float kAEPJHHLLPK = BOFOIBJDMJB.Second.GetValue(FJCBLOKOBBD);
		return (int)GetValueFromInterval(lHNCHOAEGEA, kAEPJHHLLPK);
	}

	public int GetEnemyResponseDelay(TacticFactors FJCBLOKOBBD)
	{
		float lHNCHOAEGEA = EKABOOMHLDJ.First.GetValue(FJCBLOKOBBD);
		float kAEPJHHLLPK = EKABOOMHLDJ.Second.GetValue(FJCBLOKOBBD);
		return (int)GetValueFromInterval(lHNCHOAEGEA, kAEPJHHLLPK);
	}

	private void ParseQuickAttacks(XmlNode AFHNINCKJEE)
	{
		int num = EDMBGBEKOKC.Count;
		foreach (XmlNode childNode in AFHNINCKJEE.ChildNodes)
		{
			if (childNode.Name == "QuickAttackChance")
			{
				string lLHEDBIEHAA = childNode.Attributes["Animation"].CIPOICEEIBK(string.Empty);
				EDMBGBEKOKC.Add(new global::Pair<string, TacticValue>(string.Empty, new TacticValue()));
				EDMBGBEKOKC[num].First = lLHEDBIEHAA;
				EDMBGBEKOKC[num].Second.Parse(childNode);
				num++;
			}
		}
	}

	private void ParseEvades(XmlNode AFHNINCKJEE)
	{
		int num = CCKEOHKCGKI.Count;
		foreach (XmlNode childNode in AFHNINCKJEE.ChildNodes)
		{
			if (childNode.Name == "EvadeChance")
			{
				string lLHEDBIEHAA = childNode.Attributes["Animation"].CIPOICEEIBK(string.Empty);
				CCKEOHKCGKI.Add(new global::Pair<string, TacticValue>(string.Empty, new TacticValue()));
				CCKEOHKCGKI[num].First = lLHEDBIEHAA;
				CCKEOHKCGKI[num].Second.Parse(childNode);
				num++;
			}
		}
	}

	private static void ParseInterval(XmlNode AFHNINCKJEE, global::Pair<TacticValue, TacticValue> CHCGJBLDPML)
	{
		if (AFHNINCKJEE != null)
		{
			CHCGJBLDPML.First.Parse(AFHNINCKJEE["Min"]);
			CHCGJBLDPML.Second.Parse(AFHNINCKJEE["Max"]);
		}
	}

	private static GKJKJFJALCA GetType(string CNKBLODAFDO)
	{
		if (CNKBLODAFDO == "Random")
		{
			return GKJKJFJALCA.TacticRandom;
		}
		if (CNKBLODAFDO == "Tabular")
		{
			return GKJKJFJALCA.TacticTabular;
		}
		LLLOJBFMONN.Error("Strange tactic type: %s", CNKBLODAFDO);
		return GKJKJFJALCA.TacticNone;
	}

	private static float GetValueFromInterval(float LHNCHOAEGEA, float KAEPJHHLLPK)
	{
		float num = NekkiMath.randomFloat() * (KAEPJHHLLPK - LHNCHOAEGEA);
		return LHNCHOAEGEA + num;
	}
}
