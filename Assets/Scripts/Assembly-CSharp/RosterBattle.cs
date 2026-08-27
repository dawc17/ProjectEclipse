using System;
using System.Xml;

public class RosterBattle
{
	private XmlNode _node;

	public Battle EDHMHFONDAI;

	private bool LNKJGCAAJHN;

	private bool FMFGNJBDGKG;

	public bool DFMKJHFFCMD;

	public bool EFJKJPCHFMB;

	private FightIDS FMOAFHBHOJD = new FightIDS();

	private long _randomRuleSeed;

	private long NBNPGBNOALB;

	private int JENKECCDPGP;

	private bool JONEMNHBCHA;

	private bool IMEMIMFNKFD;

	private bool IKCHAEKOMBN;

	private int HGCCKCCJBGG = -1;

	public bool CNNCIENODGE
	{
		get
		{
			return NLIJBCHAEBK();
		}
		set
		{
			HLNEICNJDCF(value);
		}
	}

	public bool GDCBBAHKCIE
	{
		get
		{
			return KAPIELMDIIK();
		}
		set
		{
			HCEOCBOFIGC(value);
		}
	}

	public FightIDS ODNEGHOCMDE
	{
		get
		{
			return KHGCEFNBDDG();
		}
		set
		{
			GEGKFFGACDI(value);
		}
	}

	public long BHNAKDJODEB
	{
		get
		{
			return BKDOAOCGJLJ();
		}
		set
		{
			OEKFMKDLLHE(value);
		}
	}

	public long GLLHEJIFGOJ
	{
		get
		{
			return PFJKCOPFNHB();
		}
		set
		{
			ELJAOONAOHJ(value);
		}
	}

	public int EPKLHKJHAPL
	{
		get
		{
			return ODCFKCJJDKN();
		}
		set
		{
			FHCHCHPPMEI(value);
		}
	}

	public bool EAIODKMKPGB
	{
		get
		{
			return FBIHFOCDCAA();
		}
	}

	public bool LCNAHCKMMFL
	{
		get
		{
			return NFOFEJPJDLL();
		}
	}

	public bool PBKCGOBMCBP
	{
		get
		{
			return GCMLGEGODDB();
		}
	}

	public int JBAGJHBMKBK
	{
		get
		{
			return PHCFNACJAAJ();
		}
		set
		{
			EAONJGHNJGB(value);
		}
	}

	public RosterBattle(XmlNode node)
	{
		_node = node;
		EDHMHFONDAI = null;
		DFMKJHFFCMD = false;
		EFJKJPCHFMB = false;
		NBNPGBNOALB = 0L;
		_randomRuleSeed = 0L;
		JENKECCDPGP = 0;
		IMEMIMFNKFD = false;
		JONEMNHBCHA = false;
		IKCHAEKOMBN = false;
		HGCCKCCJBGG = 1;
		if (_node.Attributes["Name"].Empty())
		{
			_node.LLIKNHNLGJJ("Name").Value = string.Empty;
		}
		if (_node.Attributes["Locked"].Empty())
		{
			_node.LLIKNHNLGJJ("Locked").Value = "0";
		}
		LNKJGCAAJHN = _node.Attributes["Locked"].ParseBool();
		FMFGNJBDGKG = _node.Attributes["Hidden"].ParseBool();
		FMOAFHBHOJD = new FightIDS();
		FMOAFHBHOJD.SetFightIDSByString(_node.Attributes["Name"].CIPOICEEIBK(string.Empty));
		IMEMIMFNKFD = !node.Attributes["RandomGroupSeed"].Empty();
		JONEMNHBCHA = !node.Attributes["RandomRuleSeed"].Empty();
		if (IMEMIMFNKFD)
		{
			NBNPGBNOALB = node.Attributes["RandomGroupSeed"].ParseInt();
		}
		if (JONEMNHBCHA)
		{
			_randomRuleSeed = node.Attributes["RandomRuleSeed"].ParseInt();
		}
		JENKECCDPGP = node.Attributes["ReplayCount"].ParseInt();
		IKCHAEKOMBN = !node.Attributes["Fight"].Empty();
		if (IKCHAEKOMBN)
		{
			HGCCKCCJBGG = node.Attributes["Fight"].ParseInt();
		}
	}

	public bool NLIJBCHAEBK()
	{
		return LNKJGCAAJHN;
	}

	public void HLNEICNJDCF(bool value)
	{
		LNKJGCAAJHN = value;
		_node.Attributes["Locked"].Value = Convert.ToInt32(LNKJGCAAJHN).ToString();
	}

	public bool KAPIELMDIIK()
	{
		return FMFGNJBDGKG;
	}

	public void HCEOCBOFIGC(bool value)
	{
		FMFGNJBDGKG = value;
		if (_node.Attributes["Hidden"] == null)
		{
			_node.LLIKNHNLGJJ("Hidden");
		}
		_node.Attributes["Hidden"].Value = Convert.ToInt32(FMFGNJBDGKG).ToString();
	}

	public FightIDS KHGCEFNBDDG()
	{
		return FMOAFHBHOJD;
	}

	public void GEGKFFGACDI(FightIDS value)
	{
		FMOAFHBHOJD = value;
		_node.Attributes["Name"].Value = FMOAFHBHOJD.ToString();
	}

	public long BKDOAOCGJLJ()
	{
		return _randomRuleSeed;
	}

	public void OEKFMKDLLHE(long value)
	{
		JONEMNHBCHA = true;
		_randomRuleSeed = value;
		if (_node.Attributes["RandomRuleSeed"] == null)
		{
			_node.LLIKNHNLGJJ("RandomRuleSeed");
		}
		_node.Attributes["RandomRuleSeed"].Value = _randomRuleSeed.ToString();
	}

	public long PFJKCOPFNHB()
	{
		return NBNPGBNOALB;
	}

	public void ELJAOONAOHJ(long value)
	{
		IMEMIMFNKFD = true;
		NBNPGBNOALB = value;
		if (_node.Attributes["RandomGroupSeed"] == null)
		{
			_node.LLIKNHNLGJJ("RandomGroupSeed");
		}
		_node.Attributes["RandomGroupSeed"].Value = NBNPGBNOALB.ToString();
	}

	public int ODCFKCJJDKN()
	{
		return JENKECCDPGP;
	}

	public void FHCHCHPPMEI(int value)
	{
		JENKECCDPGP = value;
		if (_node.Attributes["ReplayCount"].Empty())
		{
			_node.LLIKNHNLGJJ("ReplayCount");
		}
		_node.Attributes["ReplayCount"].Value = JENKECCDPGP.ToString();
	}

	public bool FBIHFOCDCAA()
	{
		return JONEMNHBCHA;
	}

	public bool NFOFEJPJDLL()
	{
		return IMEMIMFNKFD;
	}

	public bool GCMLGEGODDB()
	{
		return IKCHAEKOMBN;
	}

	public int PHCFNACJAAJ()
	{
		return HGCCKCCJBGG;
	}

	public void EAONJGHNJGB(int value)
	{
		IKCHAEKOMBN = true;
		HGCCKCCJBGG = value;
		if (_node.Attributes["Fight"].Empty())
		{
			_node.LLIKNHNLGJJ("Fight");
		}
		_node.Attributes["Fight"].Value = HGCCKCCJBGG.ToString();
	}
}
