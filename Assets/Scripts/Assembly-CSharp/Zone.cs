using System.Collections.Generic;

public class Zone
{
	private bool _isStart;

	private string _name;

	private string _fileName;

	public ConditionStatus PGBKNLAEANJ;

	private int _index;

	public List<Battle> LGIIBNJFADA = new List<Battle>();

	private uint BELONIAAIEP;

	private uint JPMGAALMFKI;

	public bool FOCAALFKCGF
	{
		get
		{
			return AMBLIADMEOC();
		}
	}

	public string FileName
	{
		get
		{
			return EPDMGFELIMC();
		}
	}

	public int Index
	{
		get
		{
			return KDJNDHLHAFH();
		}
	}

	public uint ANHLAHFDDCE
	{
		get
		{
			return GIOJPNNLKKK();
		}
	}

	public uint LPMDOHPIEOP
	{
		get
		{
			return MCEGLDIFDBI();
		}
	}

	public Zone(string name, string PMFEIPCHENB, bool PENNHKHFEOM = false, ConditionStatus status = ConditionStatus.StatusOpen, int index = 0, uint CDCJKJNGPOE = 0u, uint MCDAHGPLLDO = 0u)
	{
		_name = name;
		_fileName = PMFEIPCHENB;
		_isStart = PENNHKHFEOM;
		PGBKNLAEANJ = status;
		_index = index;
		BELONIAAIEP = CDCJKJNGPOE;
		JPMGAALMFKI = MCDAHGPLLDO;
	}

	public bool AMBLIADMEOC()
	{
		return _isStart;
	}

	public string get_Name()
	{
		return _name;
	}

	public string EPDMGFELIMC()
	{
		return _fileName;
	}

	public int KDJNDHLHAFH()
	{
		return _index;
	}

	public uint GIOJPNNLKKK()
	{
		return BELONIAAIEP;
	}

	public uint MCEGLDIFDBI()
	{
		return JPMGAALMFKI;
	}

	public Battle MJINKOFNIAE(string name)
	{
		foreach (Battle lGIIBNJFADum in LGIIBNJFADA)
		{
			if (lGIIBNJFADum.get_Name() == name)
			{
				return lGIIBNJFADum;
			}
		}
		LLLOJBFMONN.Write("Error: battle with name=" + name + " not found");
		return null;
	}

	public List<Battle> NIAMMNJLEFI(BattleType LFLGCDNKNJI)
	{
		List<Battle> list = new List<Battle>();
		foreach (Battle lGIIBNJFADum in LGIIBNJFADA)
		{
			if (lGIIBNJFADum.get_Type() == LFLGCDNKNJI)
			{
				list.Add(lGIIBNJFADum);
			}
		}
		if (list.Count == 0)
		{
			LLLOJBFMONN.Write("Error: _battles with type={0} not found", LFLGCDNKNJI);
		}
		return list;
	}

	public void CGJCKGAFPED()
	{
		int num = 0;
		bool flag = false;
		int i = 0;
		for (int count = LGIIBNJFADA.Count; i < count; i++)
		{
			Battle cGJCGEBPCAF = LGIIBNJFADA[i];
			List<FightList> list = cGJCGEBPCAF.NAFMJGIGBGL();
			int j = 0;
			for (int count2 = list.Count; j < count2; j++)
			{
				num++;
				ConditionStatus pGBKNLAEANJ = list[j].PGBKNLAEANJ;
				if (pGBKNLAEANJ == ConditionStatus.StatusOpen || pGBKNLAEANJ == ConditionStatus.StatusComplete)
				{
					PGBKNLAEANJ = ConditionStatus.StatusOpen;
					flag = true;
					break;
				}
			}
		}
		if (num == 0 || !flag)
		{
			PGBKNLAEANJ = ConditionStatus.StatusIncomplete;
		}
	}

	public void SetTime(long time)
	{
		int i = 0;
		for (int count = LGIIBNJFADA.Count; i < count; i++)
		{
			LGIIBNJFADA[i].SetTime(time);
		}
	}
}
