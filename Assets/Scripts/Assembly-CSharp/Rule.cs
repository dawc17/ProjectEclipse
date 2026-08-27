using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

public class Rule : global::EventDispatcher<object>
{
	public enum BCBLLMPAMLP
	{
		RuleItem = 0,
		RuleEquipItem = 1,
		RuleRandomAquiredItem = 2,
		RuleNoButton = 3,
		RuleNoAnimation = 4,
		RuleRingout = 5,
		RuleDarkness = 6,
		RuleHotGround = 7,
		RuleLoseFall = 8,
		RuleRegeneration = 9,
		RuleAttributes = 10,
		RuleDamageFactor = 11,
		RuleRemoveInterval = 12,
		RuleCrazy = 13,
		RuleLifeSteal = 14,
		RuleNoHealthBar = 15,
		RuleCombo = 16,
		RuleTimeoutWin = 17,
		RulePoints = 18,
		RuleRechargeMagicEachRound = 19,
		RuleNoBulletsReplenishment = 20,
		RuleRandom = 21,
		RuleComplex = 22,
		RuleDescription = 23,
		RulePerk = 24,
		RuleNoPerks = 25,
		RuleWinStyle = 26,
		RuleWinCombo = 27,
		RuleWinShock = 28,
		RuleChangeFight = 29,
		RuleTactic = 30,
		RuleInvertJoystick = 31,
		RuleRandomArea = 32,
		RuleRatingEvaluation = 33,
		RuleInvulnerability = 34,
		RuleCurrencyCost = 35,
		RuleResistance = 36,
		RuleRaidCurrencyCost = 37,
		RuleAvatar = 38,
		RuleName = 39
	}

	public enum DIMPPDKCBLE
	{
		MODE_ECLIPSE = 0,
		MODE_NORMAL = 1,
		MODE_ALL = 2
	}

	public Rule ParentRule;

	protected DeflatedString HEPAHAKDDGC = new DeflatedString();

	protected BCBLLMPAMLP _type;

	public DIMPPDKCBLE PGOPBNMFAAG;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool HNDCEAGNNMG;

	protected bool DBOIEEKHBOD;

	public bool IsRandom;

	protected List<int> APNNBCCKAJA = new List<int>();

	protected List<int> _rounds = new List<int>();

	public int KLJOBCIINOF;

	public int NMPCMFDGOKA;

	public DeflatedString KPMOBOPDMDO
	{
		get
		{
			return GIFDJEEGCJI();
		}
	}

	public bool MMAIGKHJPMI
	{
		get
		{
			return HHHPGLLBPMF();
		}
		private set
		{
			set_Active(value);
		}
	}

	public Rule(BCBLLMPAMLP LFLGCDNKNJI, XmlNode node)
	{
		_type = LFLGCDNKNJI;
		set_Active(true);
		DBOIEEKHBOD = true;
		ParentRule = null;
		IsRandom = false;
		PGOPBNMFAAG = DIMPPDKCBLE.MODE_ALL;
		KLJOBCIINOF = 0;
		NMPCMFDGOKA = int.MaxValue;
		HEPAHAKDDGC.Set(node);
		MIJDEAIEEMM(node);
		EALGLHDGAAH(node);
	}

	public Rule(Rule HNBFMAKFJAM)
	{
		_type = HNBFMAKFJAM._type;
		set_Active(HNBFMAKFJAM.HHHPGLLBPMF());
		DBOIEEKHBOD = HNBFMAKFJAM.DBOIEEKHBOD;
		ParentRule = HNBFMAKFJAM.ParentRule;
		IsRandom = HNBFMAKFJAM.IsRandom;
		PGOPBNMFAAG = HNBFMAKFJAM.PGOPBNMFAAG;
		KLJOBCIINOF = HNBFMAKFJAM.KLJOBCIINOF;
		NMPCMFDGOKA = HNBFMAKFJAM.NMPCMFDGOKA;
		_rounds = HNBFMAKFJAM._rounds;
		HEPAHAKDDGC = HNBFMAKFJAM.HEPAHAKDDGC;
	}

	public DeflatedString GIFDJEEGCJI()
	{
		return HEPAHAKDDGC;
	}

	public BCBLLMPAMLP get_Type()
	{
		return _type;
	}

	public bool HHHPGLLBPMF()
	{
		return HNDCEAGNNMG;
	}

	private void set_Active(bool value)
	{
		HNDCEAGNNMG = value;
	}

	public virtual void SetActive(bool value)
	{
		set_Active(value);
	}

	public virtual bool Compare(object data)
	{
		return true;
	}

	public bool MIFEDJNJHNF()
	{
		return DBOIEEKHBOD;
	}

	public bool HAKHBAOJBON(int round)
	{
		if (!DBOIEEKHBOD)
		{
			foreach (int item in _rounds)
			{
				if (item == round)
				{
					return true;
				}
			}
			return false;
		}
		return true;
	}

	public bool CHDEIEMINPF()
	{
		return JKAHFDFNLPM();
	}

	protected bool JKAHFDFNLPM(int MHNCENBCECJ)
	{
		return MHNCENBCECJ >= KLJOBCIINOF && MHNCENBCECJ <= NMPCMFDGOKA;
	}

	protected bool JKAHFDFNLPM()
	{
		int mHNCENBCECJ = ListSF.CCDKHLAMKKO().PINDEKDNCNL();
		return JKAHFDFNLPM(mHNCENBCECJ);
	}

	protected virtual void Parse(XmlNode node)
	{
		MIJDEAIEEMM(node);
		EALGLHDGAAH(node);
	}

	protected void MIJDEAIEEMM(XmlNode node)
	{
		XmlAttribute cJBEMNNNHDM = node.Attributes["Round"];
		if (!cJBEMNNNHDM.Empty())
		{
			DBOIEEKHBOD = false;
			string text = cJBEMNNNHDM.CIPOICEEIBK(string.Empty);
			string[] array = text.Split('|');
			string[] array2 = array;
			foreach (string value in array2)
			{
				int item = Convert.ToInt32(value);
				_rounds.Add(item);
			}
		}
	}

	protected void EALGLHDGAAH(XmlNode node)
	{
		bool flag = node.Attributes["Eclipse"].Empty();
		bool flag2 = node.Attributes["Eclipse"].ParseBool();
		if (flag)
		{
			PGOPBNMFAAG = DIMPPDKCBLE.MODE_ALL;
		}
		else if (flag2)
		{
			PGOPBNMFAAG = DIMPPDKCBLE.MODE_ECLIPSE;
		}
		else
		{
			PGOPBNMFAAG = DIMPPDKCBLE.MODE_NORMAL;
		}
	}
}
