public class FightIDS
{
	private string NHIIEPAGPPA;

	private string NAMJEBLOKNM;

	private string JEAHLOHOAAF;

	private string PJOMDOEMFFB;

	public string GEIAEIKDLMP
	{
		get
		{
			return PELHCAEAOFE();
		}
	}

	public string AMHIKINPPFN
	{
		get
		{
			return CPHDPCAECJN();
		}
	}

	public string FECNKMAHIOO
	{
		get
		{
			return EJPNIFANKDG();
		}
	}

	public FightIDS()
	{
		Clear();
	}

	public FightIDS(string DIAIIPCBMFL)
	{
		SetFightIDSByString(DIAIIPCBMFL);
	}

	public FightIDS(FightIDS MMEJHKCKFDD)
	{
		SetFightIDSByString(MMEJHKCKFDD.ToString());
	}

	public FightIDS(string HLJKOKMKMLM, string DPOOIONCEOA, string fight)
	{
		SetFightIDSByZBF(HLJKOKMKMLM, DPOOIONCEOA, fight);
	}

	public string PELHCAEAOFE()
	{
		return NHIIEPAGPPA;
	}

	public string CPHDPCAECJN()
	{
		return NAMJEBLOKNM;
	}

	public string EJPNIFANKDG()
	{
		return JEAHLOHOAAF;
	}

	public override string ToString()
	{
		return PJOMDOEMFFB;
	}

	public string OOBHBGJIBGP()
	{
		return NHIIEPAGPPA + '|' + NAMJEBLOKNM;
	}

	public void SetFightIDSByString(string value)
	{
		if (value != null)
		{
			string[] array = value.Split('|');
			int num = array.Length;
			NHIIEPAGPPA = ((num <= 0) ? string.Empty : array[0]);
			NAMJEBLOKNM = ((num <= 1) ? string.Empty : array[1]);
			JEAHLOHOAAF = ((num <= 2) ? string.Empty : array[2]);
			KNEGDCJNEED();
		}
	}

	public void SetFightIDSByZBF(string HLJKOKMKMLM, string DPOOIONCEOA, string fight)
	{
		NHIIEPAGPPA = ((HLJKOKMKMLM == null) ? string.Empty : HLJKOKMKMLM);
		NAMJEBLOKNM = ((DPOOIONCEOA == null) ? string.Empty : DPOOIONCEOA);
		JEAHLOHOAAF = ((fight == null) ? string.Empty : fight);
		KNEGDCJNEED();
	}

	public bool Equals(string DIAIIPCBMFL)
	{
		return ToString() == DIAIIPCBMFL;
	}

	public bool Equals(FightIDS DIAIIPCBMFL)
	{
		return Equals(DIAIIPCBMFL.ToString());
	}

	public bool Equals(string HLJKOKMKMLM, string DPOOIONCEOA, string fight)
	{
		return HLJKOKMKMLM.Equals(NHIIEPAGPPA) && DPOOIONCEOA.Equals(NAMJEBLOKNM) && fight.Equals(JEAHLOHOAAF);
	}

	public bool OLAJNGPILGL(string DIAIIPCBMFL)
	{
		string[] array = DIAIIPCBMFL.Split('|');
		int num = array.Length;
		string hLJKOKMKMLM = ((num <= 0) ? string.Empty : array[0]);
		string dPOOIONCEOA = ((num <= 1) ? string.Empty : array[1]);
		return OLAJNGPILGL(hLJKOKMKMLM, dPOOIONCEOA);
	}

	public bool OLAJNGPILGL(string HLJKOKMKMLM, string DPOOIONCEOA)
	{
		return HLJKOKMKMLM.Equals(NHIIEPAGPPA) && DPOOIONCEOA.Equals(NAMJEBLOKNM);
	}

	public void Clear()
	{
		NHIIEPAGPPA = string.Empty;
		NAMJEBLOKNM = string.Empty;
		JEAHLOHOAAF = string.Empty;
		PJOMDOEMFFB = string.Empty;
	}

	public bool OOPMAAHJMCE()
	{
		return NHIIEPAGPPA.Equals(string.Empty) && NAMJEBLOKNM.Equals(string.Empty);
	}

	public static FightIDS Empty()
	{
		FightIDS mOCEDDJOAEB = new FightIDS();
		mOCEDDJOAEB.Clear();
		return mOCEDDJOAEB;
	}

	private void KNEGDCJNEED()
	{
		PJOMDOEMFFB = NHIIEPAGPPA + "|" + NAMJEBLOKNM + "|" + JEAHLOHOAAF;
	}
}
