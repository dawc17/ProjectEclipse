using System.Xml;

public class RosterFight
{
	protected XmlNode _node;

	public FightList GAHNGDBKFNO;

	protected int JOBFHJKBJKN;

	protected int GDFACJKNGCB;

	protected long DEJHGDMHGAA;

	protected int _level;

	private int _id;

	private int DCBBBLMIHKN;

	private int FGEHEIHHPJD;

	private int EGADIFEELGM;

	private int KOJONNKHOGP;

	private int NBNPGBNOALB;

	private int _randomRuleSeed;

	public bool HasRandomSeeds;

	private long OCNKJAGPNJH;

	private long HJOHKOEICAP;

	private long NCFEAANPKMI;

	private string _fightIDS = string.Empty;

	public XmlNode Node
	{
		get
		{
			return LIGMHKEOJBB();
		}
	}

	public int EDDMIFJFKBM
	{
		get
		{
			return HCMBHIGGMDF();
		}
		set
		{
			NGGKBEKOJGI(value);
		}
	}

	public int CPPGPAILEDF
	{
		get
		{
			return JAJNIKDMPPO();
		}
		set
		{
			OBFNFKPHJIN(value);
		}
	}

	public long CLDABPBDDGB
	{
		get
		{
			return ILBNPNIPEHO();
		}
		set
		{
			CKJFJFPBIFF(value);
		}
	}

	public int Level
	{
		get
		{
			return PINDEKDNCNL();
		}
		set
		{
			DLDMOHEGENM(value);
		}
	}

	public int Id
	{
		set
		{
			MKAMABIPHEN(value);
		}
	}

	public int LCDHKPEPEOB
	{
		get
		{
			return PEHLNNEFFLI();
		}
		set
		{
			BIINCAKDHLP(value);
		}
	}

	public int KCNPAIGGBNE
	{
		get
		{
			return PHKCBMAOHIF();
		}
		set
		{
			PFFHCBCFMCM(value);
		}
	}

	public int GKNHMGEEFNE
	{
		get
		{
			return NIAHMHPBEAA();
		}
	}

	public int HBLEGOFKICB
	{
		get
		{
			return LNMPOLNHCIJ();
		}
		set
		{
			LCFFEFOFJOM(value);
		}
	}

	public int GLLHEJIFGOJ
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

	public int BHNAKDJODEB
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

	public long BDOMCCPCLNN
	{
		get
		{
			return FDAEBPDIEEE();
		}
		set
		{
			NAAHEPJIFAD(value);
		}
	}

	public long Time
	{
		get
		{
			return CCCIFDLEMPI();
		}
		set
		{
			ABIELBGOLCA(value);
		}
	}

	public long EIIOLMBDBCF
	{
		set
		{
			CLCBNOCDIPF(value);
		}
	}

	public string JLGLBLDPAAF
	{
		get
		{
			return EKOIBAIIKHL();
		}
		set
		{
			set_FightIDS(value);
		}
	}

	public RosterFight(XmlNode node)
	{
		HJOHKOEICAP = -1L;
		NCFEAANPKMI = 0L;
		GAHNGDBKFNO = null;
		_node = node;
		if (_node.Attributes["ID"].Empty())
		{
			_node.LLIKNHNLGJJ("ID").Value = "-1";
		}
		if (_node.Attributes["IDS"].Empty())
		{
			_node.LLIKNHNLGJJ("IDS").Value = "-1|-1|-1";
		}
		if (_node.Attributes["CompletedCount"].Empty())
		{
			_node.LLIKNHNLGJJ("CompletedCount").Value = "0";
		}
		if (_node.Attributes["LossCount"].Empty())
		{
			_node.LLIKNHNLGJJ("LossCount").Value = "0";
		}
		if (_node.Attributes["EclipseCompletedCount"].Empty())
		{
			_node.LLIKNHNLGJJ("EclipseCompletedCount").Value = "0";
		}
		if (_node.Attributes["EclipseLossCount"].Empty())
		{
			_node.LLIKNHNLGJJ("EclipseLossCount").Value = "0";
		}
		if (_node.Attributes["StoryCount"].Empty())
		{
			_node.LLIKNHNLGJJ("StoryCount").Value = "0";
		}
		if (_node.Attributes["CompletedTime"].Empty())
		{
			_node.LLIKNHNLGJJ("CompletedTime").Value = "0";
		}
		if (_node.Attributes["TimeLeft"].Empty())
		{
			_node.LLIKNHNLGJJ("TimeLeft").Value = "0";
		}
		if (_node.Attributes["RandomizeTimeLeft"].Empty())
		{
			_node.LLIKNHNLGJJ("RandomizeTimeLeft").Value = "0";
		}
		if (_node.Attributes["Level"].Empty())
		{
			_node.LLIKNHNLGJJ("Level").Value = "0";
		}
		_fightIDS = _node.Attributes["IDS"].CIPOICEEIBK(string.Empty);
		_id = _node.Attributes["ID"].ParseInt();
		GDFACJKNGCB = _node.Attributes["CompletedCount"].ParseInt();
		JOBFHJKBJKN = _node.Attributes["LossCount"].ParseInt();
		DCBBBLMIHKN = _node.Attributes["EclipseCompletedCount"].ParseInt();
		FGEHEIHHPJD = _node.Attributes["EclipseLossCount"].ParseInt();
		DEJHGDMHGAA = _node.Attributes["TimeLeft"].ParseLong(0L);
		OCNKJAGPNJH = _node.Attributes["RandomizeTimeLeft"].ParseLong(0L);
		KOJONNKHOGP = _node.Attributes["StoryCount"].ParseInt();
		_level = _node.Attributes["Level"].ParseInt();
		NBNPGBNOALB = _node.Attributes["RandomGroupSeed"].ParseInt();
		_randomRuleSeed = _node.Attributes["RandomRuleSeed"].ParseInt();
		HasRandomSeeds = NBNPGBNOALB != 0 || _randomRuleSeed != 0;
		EGADIFEELGM = 0;
	}

	public XmlNode LIGMHKEOJBB()
	{
		return _node;
	}

	public int HCMBHIGGMDF()
	{
		return JOBFHJKBJKN;
	}

	public void NGGKBEKOJGI(int value)
	{
		JOBFHJKBJKN = value;
		_node.Attributes["LossCount"].Value = JOBFHJKBJKN.ToString();
	}

	public int JAJNIKDMPPO()
	{
		return GDFACJKNGCB;
	}

	public void OBFNFKPHJIN(int value)
	{
		GDFACJKNGCB = value;
		_node.Attributes["CompletedCount"].Value = GDFACJKNGCB.ToString();
	}

	public long ILBNPNIPEHO()
	{
		return DEJHGDMHGAA;
	}

	public void CKJFJFPBIFF(long value)
	{
		DEJHGDMHGAA = value;
		_node.Attributes["TimeLeft"].Value = DEJHGDMHGAA.ToString();
	}

	public int PINDEKDNCNL()
	{
		return _level;
	}

	public void DLDMOHEGENM(int value)
	{
		_level = value;
		_node.Attributes["Level"].Value = _level.ToString();
	}

	public void MKAMABIPHEN(int value)
	{
		_id = value;
		_node.Attributes["ID"].Value = _id.ToString();
	}

	public int PEHLNNEFFLI()
	{
		return DCBBBLMIHKN;
	}

	public void BIINCAKDHLP(int value)
	{
		DCBBBLMIHKN = value;
		_node.Attributes["EclipseCompletedCount"].Value = DCBBBLMIHKN.ToString();
	}

	public void LOEBHEODPAH()
	{
		DCBBBLMIHKN++;
		_node.Attributes["EclipseCompletedCount"].Value = DCBBBLMIHKN.ToString();
	}

	public int PHKCBMAOHIF()
	{
		return FGEHEIHHPJD;
	}

	public void PFFHCBCFMCM(int value)
	{
		FGEHEIHHPJD = value;
		_node.Attributes["EclipseLossCount"].Value = FGEHEIHHPJD.ToString();
	}

	public void HBIAOHGMLDK()
	{
		FGEHEIHHPJD++;
		_node.Attributes["EclipseLossCount"].Value = FGEHEIHHPJD.ToString();
	}

	public int NIAHMHPBEAA()
	{
		return EGADIFEELGM;
	}

	public int LNMPOLNHCIJ()
	{
		return KOJONNKHOGP;
	}

	public void LCFFEFOFJOM(int value)
	{
		KOJONNKHOGP = value;
		_node.Attributes["StoryCount"].Value = KOJONNKHOGP.ToString();
	}

	private void PMIDKMKNOHM()
	{
		KOJONNKHOGP++;
		_node.Attributes["StoryCount"].Value = KOJONNKHOGP.ToString();
	}

	public int PFJKCOPFNHB()
	{
		return NBNPGBNOALB;
	}

	public void ELJAOONAOHJ(int value)
	{
		NBNPGBNOALB = value;
		if (_node.Attributes["RandomGroupSeed"].Empty())
		{
			_node.LLIKNHNLGJJ("RandomGroupSeed").Value = NBNPGBNOALB.ToString();
		}
		else
		{
			_node.Attributes["RandomGroupSeed"].Value = NBNPGBNOALB.ToString();
		}
	}

	public int BKDOAOCGJLJ()
	{
		return _randomRuleSeed;
	}

	public void OEKFMKDLLHE(int value)
	{
		_randomRuleSeed = value;
		if (_node.Attributes["RandomRuleSeed"].Empty())
		{
			_node.LLIKNHNLGJJ("RandomRuleSeed").Value = _randomRuleSeed.ToString();
		}
		else
		{
			_node.Attributes["RandomRuleSeed"].Value = _randomRuleSeed.ToString();
		}
		if (GAHNGDBKFNO != null)
		{
			GAHNGDBKFNO.ResetRandomRules();
		}
	}

	public long FDAEBPDIEEE()
	{
		return OCNKJAGPNJH;
	}

	public void NAAHEPJIFAD(long value)
	{
		OCNKJAGPNJH = value;
		_node.Attributes["RandomizeTimeLeft"].Value = OCNKJAGPNJH.ToString();
	}

	public long CCCIFDLEMPI()
	{
		return HJOHKOEICAP;
	}

	public void ABIELBGOLCA(long value)
	{
		if (DEJHGDMHGAA <= 0)
		{
			HJOHKOEICAP = -1L;
		}
		else
		{
			HJOHKOEICAP = value - DEJHGDMHGAA;
		}
	}

	public void CLCBNOCDIPF(long value)
	{
		if (OCNKJAGPNJH <= 0)
		{
			NCFEAANPKMI = -1L;
		}
		else
		{
			NCFEAANPKMI = value - OCNKJAGPNJH;
		}
	}

	public string EKOIBAIIKHL()
	{
		return _fightIDS;
	}

	public void set_FightIDS(string value)
	{
		_fightIDS = value;
		_node.Attributes["IDS"].Value = _fightIDS.ToString();
	}

	public string GIDNOKCJLPL()
	{
		FightIDS mOCEDDJOAEB = new FightIDS();
		mOCEDDJOAEB.SetFightIDSByString(_fightIDS);
		return mOCEDDJOAEB.CPHDPCAECJN();
	}

	public void GICDABHEMML()
	{
		EGADIFEELGM = 0;
		GDFACJKNGCB++;
		_node.Attributes["CompletedCount"].Value = GDFACJKNGCB.ToString();
	}

	public void ICAKCDMOMDF()
	{
		EGADIFEELGM++;
		JOBFHJKBJKN++;
		_node.Attributes["LossCount"].Value = JOBFHJKBJKN.ToString();
	}

	public bool GHCHJIBBBOK(long value)
	{
		return HJOHKOEICAP < 0 || HJOHKOEICAP >= value;
	}

	public void BABOCEFFPII()
	{
		ELJAOONAOHJ(NekkiMath.randomInt(int.MaxValue));
		OEKFMKDLLHE(NekkiMath.randomInt(int.MaxValue));
		HasRandomSeeds = true;
		NAAHEPJIFAD(ListSF.IDMJOMOMDOJ());
		ListSF.ELEBLBJKDBI().EJANJEEGOOE();
	}

	public bool AANKNHJKJII(long LHLPFBOAEPA)
	{
		if (NCFEAANPKMI >= LHLPFBOAEPA || NCFEAANPKMI == -1)
		{
			BABOCEFFPII();
			return true;
		}
		return false;
	}
}
