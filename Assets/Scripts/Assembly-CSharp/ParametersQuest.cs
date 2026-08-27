using System.Xml;

public class ParametersQuest
{
	private string MHAGBMLGJNB = string.Empty;

	private string MPFLHOFEOGI = string.Empty;

	private string ABEABNOGJDM = string.Empty;

	private int KJBLFMHGLPF;

	private int _power;

	private int PCJHKLGDFDH;

	private int DKMFBOAKFPH;

	private float _fightAvgFPS;

	public XmlNode Node;

	public string EAJKKMEDGEL
	{
		get
		{
			return HPELIEHPJCI();
		}
		set
		{
			ELKOGHKIDOG(value);
		}
	}

	public string JOGMEAACJED
	{
		get
		{
			return LIPMCBHCLKN();
		}
		set
		{
			AJBMLOLOFAN(value);
		}
	}

	public string BBFPIPJJCDH
	{
		get
		{
			return JOLAAOAFNFF();
		}
		set
		{
			CPONINMPIJL(value);
		}
	}

	public int LCDGAGJHGBJ
	{
		get
		{
			return OGIPFNNJOPK();
		}
		set
		{
			EFIFIPKDMIN(value);
		}
	}

	public int MFGLDPKEDJB
	{
		get
		{
			return NHKMGNPADKI();
		}
		set
		{
			MPFIPAANJON(value);
		}
	}

	public int OIOLMKNFHOP
	{
		get
		{
			return EDADICNDCKK();
		}
		set
		{
			MNNPHOAEMII(value);
		}
	}

	public int LHLMNEDCCDK
	{
		get
		{
			return CIDMJEKCDMP();
		}
		set
		{
			IBMNACPGMLL(value);
		}
	}

	public float LFAPNGHLANG
	{
		get
		{
			return OEDLLJIBHFN();
		}
		set
		{
			set_FightAvgFPS(value);
		}
	}

	public ParametersQuest(XmlNode PKHDLOGJKAD)
	{
		Node = PKHDLOGJKAD;
		if (Node.Attributes["ScreenIndex"] == null)
		{
			Node.LLIKNHNLGJJ("ScreenIndex").Value = "0";
		}
		if (Node.Attributes["ChekPointIndex"] == null)
		{
			Node.LLIKNHNLGJJ("ChekPointIndex").Value = "0";
		}
		if (Node["FightResult"] == null)
		{
			Node.ACBPMPMPKJJ("FightResult").LLIKNHNLGJJ("Name");
		}
		if (Node["RaidResult"] == null)
		{
			Node.ACBPMPMPKJJ("RaidResult").LLIKNHNLGJJ("Name");
		}
		if (Node["Fight"] == null)
		{
			Node.ACBPMPMPKJJ("Fight").LLIKNHNLGJJ("Name");
		}
		if (Node["LevelUp"] == null)
		{
			Node.ACBPMPMPKJJ("LevelUp").LLIKNHNLGJJ("Value").Value = "0";
		}
		if (Node["PowerAmount"] == null)
		{
			Node.ACBPMPMPKJJ("PowerAmount").LLIKNHNLGJJ("Value").Value = "0";
		}
		if (Node["FightAvgFPS"] == null)
		{
			Node.ACBPMPMPKJJ("FightAvgFPS").LLIKNHNLGJJ("Value").Value = "0";
		}
		PCJHKLGDFDH = Node.Attributes["ScreenIndex"].ParseInt();
		DKMFBOAKFPH = Node.Attributes["ChekPointIndex"].ParseInt();
		MPFLHOFEOGI = Node["FightResult"].Attributes["Name"].CIPOICEEIBK(string.Empty);
		ABEABNOGJDM = Node["RaidResult"].Attributes["Name"].CIPOICEEIBK(string.Empty);
		MHAGBMLGJNB = Node["Fight"].Attributes["Name"].CIPOICEEIBK(string.Empty);
		KJBLFMHGLPF = Node["LevelUp"].Attributes["Value"].ParseInt();
		_power = Node["PowerAmount"].Attributes["Value"].ParseInt();
		_fightAvgFPS = Node["FightAvgFPS"].Attributes["Value"].ParseFloat();
	}

	public string HPELIEHPJCI()
	{
		return MHAGBMLGJNB;
	}

	public void ELKOGHKIDOG(string value)
	{
		MHAGBMLGJNB = value;
		Node["Fight"].Attributes["Name"].Value = ((MHAGBMLGJNB == null) ? string.Empty : MHAGBMLGJNB);
	}

	public string LIPMCBHCLKN()
	{
		return MPFLHOFEOGI;
	}

	public void AJBMLOLOFAN(string value)
	{
		MPFLHOFEOGI = value;
		Node["FightResult"].Attributes["Name"].Value = ((MPFLHOFEOGI == null) ? string.Empty : MPFLHOFEOGI);
	}

	public string JOLAAOAFNFF()
	{
		return ABEABNOGJDM;
	}

	public void CPONINMPIJL(string value)
	{
		ABEABNOGJDM = value;
		Node["RaidResult"].Attributes["Name"].Value = ((ABEABNOGJDM == null) ? string.Empty : ABEABNOGJDM);
	}

	public int OGIPFNNJOPK()
	{
		return KJBLFMHGLPF;
	}

	public void EFIFIPKDMIN(int value)
	{
		KJBLFMHGLPF = value;
		Node["LevelUp"].Attributes["Value"].Value = KJBLFMHGLPF.ToString();
	}

	public int NHKMGNPADKI()
	{
		return _power;
	}

	public void MPFIPAANJON(int value)
	{
		_power = value;
		Node["PowerAmount"].Attributes["Value"].Value = KJBLFMHGLPF.ToString();
	}

	public int EDADICNDCKK()
	{
		return PCJHKLGDFDH;
	}

	public void MNNPHOAEMII(int value)
	{
		PCJHKLGDFDH = value;
		Node.Attributes["ScreenIndex"].Value = PCJHKLGDFDH.ToString();
	}

	public int CIDMJEKCDMP()
	{
		return DKMFBOAKFPH;
	}

	public void IBMNACPGMLL(int value)
	{
		DKMFBOAKFPH = value;
		Node.Attributes["ChekPointIndex"].Value = DKMFBOAKFPH.ToString();
	}

	public float OEDLLJIBHFN()
	{
		return _fightAvgFPS;
	}

	public void set_FightAvgFPS(float value)
	{
		_fightAvgFPS = value;
		Node["FightAvgFPS"].Attributes["Value"].Value = _fightAvgFPS.ToString();
	}
}
