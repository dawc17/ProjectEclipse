using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class IntervalAttack : IntervalAnimation
{
	public class Factors
	{
		public bool FNDCJJNDNJC;

		public bool DJGAHEOIHGG;

		public float Factor;

		public float HJIIIBHAOMJ;

		public Factors(float IOGMPFJOCPE = 1f, float BEMFHMKCJOK = 1f, bool BBBPANLHLEM = false, bool LNJNGGBCPFH = false)
		{
			FNDCJJNDNJC = BBBPANLHLEM;
			DJGAHEOIHGG = LNJNGGBCPFH;
			Factor = IOGMPFJOCPE;
			HJIIIBHAOMJ = BEMFHMKCJOK;
		}

		public void UpdateFactor()
		{
			if (FNDCJJNDNJC && DJGAHEOIHGG)
			{
				Factor *= HJIIIBHAOMJ;
			}
		}
	}

	public class Reaction
	{
		public string Name = string.Empty;

		public int Start = -1;

		public int PLHPGFGAGKJ = -1;

		public Reaction()
		{
		}

		public Reaction(string _name, int IKJGMFDONPK, int BBFNPCJLJIM)
		{
			Name = _name;
			Start = IKJGMFDONPK;
			PLHPGFGAGKJ = BBFNPCJLJIM;
		}
	}

	private bool LLCADDDKFKH;

	private Factors MHEPKEFOGCP = new Factors();

	private Factors NGDOFKDAIOD = new Factors();

	private bool DPCBBGHKAMA;

	private bool MMIBGOBLIDF;

	private bool HCEACAPBEHG;

	private RuleAppliance MPJMABFAJJM;

	private float _Damage;

	private bool DAAIAHEALOC;

	private string _BodyPart;

	private Vector3f FCFLHDNEICG = new Vector3f();

	private int _ComboTime;

	private List<Reaction> HBLDJNLGILG = new List<Reaction>();

	private bool KOJJEJFODEG;

	private bool NPKNOBDPPMC;

	private List<string> FODLDCDBJHG;

	private List<string> IBANLECAADN;

	private List<string> IHCJJKDOGGL;

	private List<string> OBNBCOLPNDF;

	private List<string> KAFFLGLEKPG = new List<string>();

	private List<global::Pair<string, float>> AJNCNCFDLKL = new List<global::Pair<string, float>>();

	private List<string> OJPDIHOCDGO = new List<string>();

	public bool KPDHBAPFMDB
	{
		get
		{
			return MOILKOLCNBP();
		}
	}

	public bool FHHPPFJGEIP
	{
		get
		{
			return CFADPGIEKDN();
		}
	}

	public RuleAppliance AFPHMCDIFIO
	{
		get
		{
			return JLNCBPPNPCI();
		}
		set
		{
			JMBPMOIBLDF(value);
		}
	}

	public float KFMJMBANIGF
	{
		get
		{
			return GHGGNMBCMNM();
		}
	}

	public bool JHAJPODLMEH
	{
		get
		{
			return HPLOFLKCLHG();
		}
	}

	public string EMMANKFGLLL
	{
		get
		{
			return ELHIBCEADCG();
		}
	}

	public Vector3f LEHLBAGKMKH
	{
		get
		{
			return GIFLLJFAJCO();
		}
	}

	public int HIGBAPPOOKJ
	{
		get
		{
			return KCBHAMHLGBC();
		}
	}

	public bool FIEBIONJCCI
	{
		get
		{
			return PIKCMLIAFOI();
		}
	}

	public bool HHFIEAABPAA
	{
		get
		{
			return NPHDDMAIGKN();
		}
	}

	public List<string> OKHHAPIKBII
	{
		get
		{
			return KBENFIOADCG();
		}
	}

	public List<string> PKDFDIICGFK
	{
		get
		{
			return DNPLIFOABPB();
		}
	}

	public List<string> MHNFFFIOIDH
	{
		get
		{
			return IKPJJAEIOCG();
		}
	}

	public List<global::Pair<string, float>> ILFHIDLMHFB
	{
		get
		{
			return ACCOBHPHDDN();
		}
	}

	public List<string> CKJBFNJEDHH
	{
		get
		{
			return DONAJGIBKCC();
		}
	}

	public IntervalAttack()
		: base(NGAJJDIEDGF.INTERVAL_ATTACK)
	{
	}

	public bool MOILKOLCNBP()
	{
		return LLCADDDKFKH;
	}

	public bool CFADPGIEKDN()
	{
		return DPCBBGHKAMA;
	}

	public RuleAppliance JLNCBPPNPCI()
	{
		return MPJMABFAJJM;
	}

	public void JMBPMOIBLDF(RuleAppliance value)
	{
		MPJMABFAJJM = value;
	}

	public float GHGGNMBCMNM()
	{
		return _Damage;
	}

	public bool HPLOFLKCLHG()
	{
		return DAAIAHEALOC;
	}

	public string ELHIBCEADCG()
	{
		return _BodyPart;
	}

	public Vector3f GIFLLJFAJCO()
	{
		return FCFLHDNEICG;
	}

	public int KCBHAMHLGBC()
	{
		return _ComboTime;
	}

	public bool PIKCMLIAFOI()
	{
		return KOJJEJFODEG;
	}

	public bool NPHDDMAIGKN()
	{
		return NPKNOBDPPMC;
	}

	public List<string> KBENFIOADCG()
	{
		return FODLDCDBJHG;
	}

	public List<string> DNPLIFOABPB()
	{
		return IBANLECAADN;
	}

	public List<string> IKPJJAEIOCG()
	{
		return KAFFLGLEKPG;
	}

	public List<global::Pair<string, float>> ACCOBHPHDDN()
	{
		return AJNCNCFDLKL;
	}

	public List<string> DONAJGIBKCC()
	{
		return OJPDIHOCDGO;
	}

	public string GetReactionName(int BJNCGLPAMMF)
	{
		foreach (Reaction item in HBLDJNLGILG)
		{
			int bOPAEEBGFAN = item.Start;
			int pLHPGFGAGKJ = item.PLHPGFGAGKJ;
			if (bOPAEEBGFAN <= BJNCGLPAMMF && BJNCGLPAMMF <= pLHPGFGAGKJ)
			{
				return item.Name;
			}
		}
		return string.Empty;
	}

	public static float GetItemFactor(float CKKFKEIELCP, float JMMCOMOIDNN, float KEOGNFIOEIB)
	{
		return Mathf.Pow(2f, (CKKFKEIELCP - JMMCOMOIDNN) / KEOGNFIOEIB);
	}

	public void UpdateFactor(RuleAppliance EJPOJJKKICO)
	{
		switch (EJPOJJKKICO)
		{
		case RuleAppliance.AppliancePlayer:
			MHEPKEFOGCP.UpdateFactor();
			break;
		case RuleAppliance.ApplianceOpponent:
			NGDOFKDAIOD.UpdateFactor();
			break;
		case RuleAppliance.ApplianceAll:
			MHEPKEFOGCP.UpdateFactor();
			NGDOFKDAIOD.UpdateFactor();
			break;
		}
	}

	public Factors GetFactors(RuleAppliance EJPOJJKKICO)
	{
		switch (EJPOJJKKICO)
		{
		case RuleAppliance.AppliancePlayer:
			return MHEPKEFOGCP;
		case RuleAppliance.ApplianceOpponent:
			return NGDOFKDAIOD;
		default:
			return NGDOFKDAIOD;
		}
	}

	protected override void ParseInside()
	{
		KOJJEJFODEG = !NodeInterval.Attributes["NoEffect"].ParseBool();
		XmlNode xmlNode = NodeInterval["IgnoresBlock"];
		if (xmlNode != null)
		{
			NPKNOBDPPMC = true;
			string text = xmlNode.Attributes["Name"].CIPOICEEIBK(string.Empty);
			FODLDCDBJHG = new List<string>(text.Split('|'));
		}
		XmlNode xmlNode2 = NodeInterval["IgnoresInvulnerable"];
		if (xmlNode2 != null)
		{
			LLCADDDKFKH = true;
			string text2 = xmlNode2.Attributes["Name"].CIPOICEEIBK(string.Empty);
			IBANLECAADN = new List<string>(text2.Split('|'));
		}
		XmlNode xmlNode3 = NodeInterval["AttackingParts"];
		if (xmlNode3 != null)
		{
			foreach (XmlNode childNode in xmlNode3.ChildNodes)
			{
				string item = childNode.Attributes["Name"].CIPOICEEIBK(string.Empty);
				KAFFLGLEKPG.Add(item);
			}
		}
		DPCBBGHKAMA = KAFFLGLEKPG.Count > 0;
		foreach (XmlNode childNode2 in NodeInterval.ChildNodes)
		{
			if (childNode2.Name == "Hit")
			{
				string text3 = childNode2.Attributes["Name"].CIPOICEEIBK(string.Empty);
				int num = childNode2.Attributes["Start"].ParseInt(Start);
				int num2 = childNode2.Attributes["End"].ParseInt(GEJLNPIEDPF);
				if (num < Start || GEJLNPIEDPF < num)
				{
					LLLOJBFMONN.Error("StartFrame ({0}) is outside of attack interval ({1}-{2}) - {3}", num, Start, GEJLNPIEDPF, text3);
				}
				if (num2 < Start || GEJLNPIEDPF < num2)
				{
					LLLOJBFMONN.Error("EndFrame ({0}) is outside of attack interval ({1}-{2}) - {3}", num2, Start, GEJLNPIEDPF, text3);
				}
				Reaction item2 = new Reaction(text3, num, num2);
				HBLDJNLGILG.Add(item2);
			}
		}
		if (NodeInterval["Impulse"] != null)
		{
			FCFLHDNEICG.JPFALPBDBAP(NodeInterval["Impulse"].Attributes["X"].ParseFloat());
			FCFLHDNEICG.IBNFLLGPOLD(NodeInterval["Impulse"].Attributes["Y"].ParseFloat());
			FCFLHDNEICG.set_Z(NodeInterval["Impulse"].Attributes["Z"].ParseFloat());
		}
		_ComboTime = ((NodeInterval["Combo"] != null) ? NodeInterval["Combo"].Attributes["Time"].ParseInt() : 0);
		XmlNode xmlNode6 = NodeInterval["Damage"];
		_Damage = xmlNode6.Attributes["Value"].ParseFloat();
		DAAIAHEALOC = xmlNode6.Attributes["NoCritical"].ParseBool();
		_BodyPart = xmlNode6.Attributes["BodyPart"].CIPOICEEIBK(string.Empty);
		ParseFactorAndDefenseItems(xmlNode6);
	}

	private void ParseFactorAndDefenseItems(XmlNode KPOOAIGIDPL)
	{
		foreach (XmlNode childNode in KPOOAIGIDPL.ChildNodes)
		{
			string name = childNode.Name;
			string text = childNode.Attributes["Type"].CIPOICEEIBK(string.Empty);
			float pOFHDGJAFMP = childNode.Attributes["Shift"].ParseFloat();
			if (name == "Damage")
			{
				AJNCNCFDLKL.Add(new global::Pair<string, float>(text, pOFHDGJAFMP));
				continue;
			}
			if (name == "Defense")
			{
				OJPDIHOCDGO.Add(text);
				continue;
			}
			LLLOJBFMONN.Error("Strange node name xml {0}", name);
		}
	}
}
