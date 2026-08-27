using System;
using System.IO;
using System.Text;

internal class Lexer
{
	private delegate bool MGLALMHHOGL(FsmContext IEBDPKGBOGJ);

	private static int[] fsm_return_table;

	private static MGLALMHHOGL[] MOCBIJNMDKH;

	private bool ILNBEIIEBMJ;

	private bool KIDINKPOHCD;

	private bool LGPODKLEMBP;

	private FsmContext GFHMIGANEPF;

	private int KCLLHLMNMKO;

	private int LJJGFHFKGHN;

	private TextReader reader;

	private int state;

	private StringBuilder string_buffer;

	private string string_value;

	private int JLFCBDKNAGP;

	private int FGCEBBKDFPE;

	public bool NIHDCFNOEMF
	{
		get
		{
			return CGHOOPPOBJO();
		}
		set
		{
			LEONKMNNHJC(value);
		}
	}

	public bool CBCDIFLPHAK
	{
		get
		{
			return MIPNPCEMOPG();
		}
		set
		{
			JKAFBNBJLCM(value);
		}
	}

	public bool GEPCEOKMALO
	{
		get
		{
			return ELOPMJBDCEN();
		}
	}

	public int CJPJNFFJNGN
	{
		get
		{
			return EACDJONMMAP();
		}
	}

	public string OGGIEEDEKEM
	{
		get
		{
			return EODMEFCBIOM();
		}
	}

	static Lexer()
	{
		NBOMPEKELBH();
	}

	public Lexer(TextReader reader)
	{
		ILNBEIIEBMJ = true;
		KIDINKPOHCD = true;
		KCLLHLMNMKO = 0;
		string_buffer = new StringBuilder(128);
		state = 1;
		LGPODKLEMBP = false;
		this.reader = reader;
		GFHMIGANEPF = new FsmContext();
		GFHMIGANEPF.PLHFFNOPLMM = this;
	}

	public bool CGHOOPPOBJO()
	{
		return ILNBEIIEBMJ;
	}

	public void LEONKMNNHJC(bool value)
	{
		ILNBEIIEBMJ = value;
	}

	public bool MIPNPCEMOPG()
	{
		return KIDINKPOHCD;
	}

	public void JKAFBNBJLCM(bool value)
	{
		KIDINKPOHCD = value;
	}

	public bool ELOPMJBDCEN()
	{
		return LGPODKLEMBP;
	}

	public int EACDJONMMAP()
	{
		return JLFCBDKNAGP;
	}

	public string EODMEFCBIOM()
	{
		return string_value;
	}

	private static int HexValue(int BMLBINLPOOE)
	{
		switch (BMLBINLPOOE)
		{
		case 65:
		case 97:
			return 10;
		case 66:
		case 98:
			return 11;
		case 67:
		case 99:
			return 12;
		case 68:
		case 100:
			return 13;
		case 69:
		case 101:
			return 14;
		case 70:
		case 102:
			return 15;
		default:
			return BMLBINLPOOE - 48;
		}
	}

	private static void NBOMPEKELBH()
	{
		MOCBIJNMDKH = new MGLALMHHOGL[28]
		{
			HJMEEAGDJCF, ANKHKJFGFPM, EJHIGDNFCDK, CLEGNLOABGG, HDCICLINCFJ, PNMFKCAGAOO, CEGCABJIFAC, BECIMMGHALF, AOMINPGADIM, DFNLBPEFAIM,
			PNLIDPLFJND, DFBHIODPHBG, LDKFPIDDHBD, MHCNLJAHDMO, EOJOLJMHMIE, LHEDIFJCNCF, AHPOCKCJPFD, MPBAAPKKHPA, BMJONAADIEA, FILDPJMEDAL,
			PEBHDBIELEI, HCLNFPJIEPH, LBIDLILELGF, DPLKFCBNBBI, OEIJIEICKGC, KODHDINLCML, KEHPOJHCJJI, HKPHGNANHDD
		};
		fsm_return_table = new int[28]
		{
			65542, 0, 65537, 65537, 0, 65537, 0, 65537, 0, 0,
			65538, 0, 0, 0, 65539, 0, 0, 65540, 65541, 65542,
			0, 0, 65541, 65542, 0, 0, 0, 0
		};
	}

	private static char ProcessEscChar(int NLHEKGPGAME)
	{
		switch (NLHEKGPGAME)
		{
		case 34:
		case 39:
		case 47:
		case 92:
			return Convert.ToChar(NLHEKGPGAME);
		case 110:
			return '\n';
		case 116:
			return '\t';
		case 114:
			return '\r';
		case 98:
			return '\b';
		case 102:
			return '\f';
		default:
			return '?';
		}
	}

	private static bool HJMEEAGDJCF(FsmContext IEBDPKGBOGJ)
	{
		while (IEBDPKGBOGJ.PLHFFNOPLMM.POLJELICCMK())
		{
			if (IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN == 32 || (IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN >= 9 && IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN <= 13))
			{
				continue;
			}
			if (IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN >= 49 && IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN <= 57)
			{
				IEBDPKGBOGJ.PLHFFNOPLMM.string_buffer.Append((char)IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN);
				IEBDPKGBOGJ.BKINLEDMLDJ = 3;
				return true;
			}
			switch (IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN)
			{
			case 34:
				IEBDPKGBOGJ.BKINLEDMLDJ = 19;
				IEBDPKGBOGJ.Return = true;
				return true;
			case 44:
			case 58:
			case 91:
			case 93:
			case 123:
			case 125:
				IEBDPKGBOGJ.BKINLEDMLDJ = 1;
				IEBDPKGBOGJ.Return = true;
				return true;
			case 45:
				IEBDPKGBOGJ.PLHFFNOPLMM.string_buffer.Append((char)IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN);
				IEBDPKGBOGJ.BKINLEDMLDJ = 2;
				return true;
			case 48:
				IEBDPKGBOGJ.PLHFFNOPLMM.string_buffer.Append((char)IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN);
				IEBDPKGBOGJ.BKINLEDMLDJ = 4;
				return true;
			case 102:
				IEBDPKGBOGJ.BKINLEDMLDJ = 12;
				return true;
			case 110:
				IEBDPKGBOGJ.BKINLEDMLDJ = 16;
				return true;
			case 116:
				IEBDPKGBOGJ.BKINLEDMLDJ = 9;
				return true;
			case 39:
				if (!IEBDPKGBOGJ.PLHFFNOPLMM.KIDINKPOHCD)
				{
					return false;
				}
				IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN = 34;
				IEBDPKGBOGJ.BKINLEDMLDJ = 23;
				IEBDPKGBOGJ.Return = true;
				return true;
			case 47:
				if (!IEBDPKGBOGJ.PLHFFNOPLMM.ILNBEIIEBMJ)
				{
					return false;
				}
				IEBDPKGBOGJ.BKINLEDMLDJ = 25;
				return true;
			default:
				return false;
			}
		}
		return true;
	}

	private static bool ANKHKJFGFPM(FsmContext IEBDPKGBOGJ)
	{
		IEBDPKGBOGJ.PLHFFNOPLMM.POLJELICCMK();
		if (IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN >= 49 && IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN <= 57)
		{
			IEBDPKGBOGJ.PLHFFNOPLMM.string_buffer.Append((char)IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN);
			IEBDPKGBOGJ.BKINLEDMLDJ = 3;
			return true;
		}
		int lJJGFHFKGHN = IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN;
		if (lJJGFHFKGHN == 48)
		{
			IEBDPKGBOGJ.PLHFFNOPLMM.string_buffer.Append((char)IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN);
			IEBDPKGBOGJ.BKINLEDMLDJ = 4;
			return true;
		}
		return false;
	}

	private static bool EJHIGDNFCDK(FsmContext IEBDPKGBOGJ)
	{
		while (IEBDPKGBOGJ.PLHFFNOPLMM.POLJELICCMK())
		{
			if (IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN >= 48 && IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN <= 57)
			{
				IEBDPKGBOGJ.PLHFFNOPLMM.string_buffer.Append((char)IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN);
				continue;
			}
			if (IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN == 32 || (IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN >= 9 && IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN <= 13))
			{
				IEBDPKGBOGJ.Return = true;
				IEBDPKGBOGJ.BKINLEDMLDJ = 1;
				return true;
			}
			switch (IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN)
			{
			case 44:
			case 93:
			case 125:
				IEBDPKGBOGJ.PLHFFNOPLMM.FMKBGGPKKEN();
				IEBDPKGBOGJ.Return = true;
				IEBDPKGBOGJ.BKINLEDMLDJ = 1;
				return true;
			case 46:
				IEBDPKGBOGJ.PLHFFNOPLMM.string_buffer.Append((char)IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN);
				IEBDPKGBOGJ.BKINLEDMLDJ = 5;
				return true;
			case 69:
			case 101:
				IEBDPKGBOGJ.PLHFFNOPLMM.string_buffer.Append((char)IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN);
				IEBDPKGBOGJ.BKINLEDMLDJ = 7;
				return true;
			default:
				return false;
			}
		}
		return true;
	}

	private static bool CLEGNLOABGG(FsmContext IEBDPKGBOGJ)
	{
		IEBDPKGBOGJ.PLHFFNOPLMM.POLJELICCMK();
		if (IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN == 32 || (IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN >= 9 && IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN <= 13))
		{
			IEBDPKGBOGJ.Return = true;
			IEBDPKGBOGJ.BKINLEDMLDJ = 1;
			return true;
		}
		switch (IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN)
		{
		case 44:
		case 93:
		case 125:
			IEBDPKGBOGJ.PLHFFNOPLMM.FMKBGGPKKEN();
			IEBDPKGBOGJ.Return = true;
			IEBDPKGBOGJ.BKINLEDMLDJ = 1;
			return true;
		case 46:
			IEBDPKGBOGJ.PLHFFNOPLMM.string_buffer.Append((char)IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN);
			IEBDPKGBOGJ.BKINLEDMLDJ = 5;
			return true;
		case 69:
		case 101:
			IEBDPKGBOGJ.PLHFFNOPLMM.string_buffer.Append((char)IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN);
			IEBDPKGBOGJ.BKINLEDMLDJ = 7;
			return true;
		default:
			return false;
		}
	}

	private static bool HDCICLINCFJ(FsmContext IEBDPKGBOGJ)
	{
		IEBDPKGBOGJ.PLHFFNOPLMM.POLJELICCMK();
		if (IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN >= 48 && IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN <= 57)
		{
			IEBDPKGBOGJ.PLHFFNOPLMM.string_buffer.Append((char)IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN);
			IEBDPKGBOGJ.BKINLEDMLDJ = 6;
			return true;
		}
		return false;
	}

	private static bool PNMFKCAGAOO(FsmContext IEBDPKGBOGJ)
	{
		while (IEBDPKGBOGJ.PLHFFNOPLMM.POLJELICCMK())
		{
			if (IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN >= 48 && IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN <= 57)
			{
				IEBDPKGBOGJ.PLHFFNOPLMM.string_buffer.Append((char)IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN);
				continue;
			}
			if (IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN == 32 || (IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN >= 9 && IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN <= 13))
			{
				IEBDPKGBOGJ.Return = true;
				IEBDPKGBOGJ.BKINLEDMLDJ = 1;
				return true;
			}
			switch (IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN)
			{
			case 44:
			case 93:
			case 125:
				IEBDPKGBOGJ.PLHFFNOPLMM.FMKBGGPKKEN();
				IEBDPKGBOGJ.Return = true;
				IEBDPKGBOGJ.BKINLEDMLDJ = 1;
				return true;
			case 69:
			case 101:
				IEBDPKGBOGJ.PLHFFNOPLMM.string_buffer.Append((char)IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN);
				IEBDPKGBOGJ.BKINLEDMLDJ = 7;
				return true;
			default:
				return false;
			}
		}
		return true;
	}

	private static bool CEGCABJIFAC(FsmContext IEBDPKGBOGJ)
	{
		IEBDPKGBOGJ.PLHFFNOPLMM.POLJELICCMK();
		if (IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN >= 48 && IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN <= 57)
		{
			IEBDPKGBOGJ.PLHFFNOPLMM.string_buffer.Append((char)IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN);
			IEBDPKGBOGJ.BKINLEDMLDJ = 8;
			return true;
		}
		int lJJGFHFKGHN = IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN;
		if (lJJGFHFKGHN == 43 || lJJGFHFKGHN == 45)
		{
			IEBDPKGBOGJ.PLHFFNOPLMM.string_buffer.Append((char)IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN);
			IEBDPKGBOGJ.BKINLEDMLDJ = 8;
			return true;
		}
		return false;
	}

	private static bool BECIMMGHALF(FsmContext IEBDPKGBOGJ)
	{
		while (IEBDPKGBOGJ.PLHFFNOPLMM.POLJELICCMK())
		{
			if (IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN >= 48 && IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN <= 57)
			{
				IEBDPKGBOGJ.PLHFFNOPLMM.string_buffer.Append((char)IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN);
				continue;
			}
			if (IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN == 32 || (IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN >= 9 && IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN <= 13))
			{
				IEBDPKGBOGJ.Return = true;
				IEBDPKGBOGJ.BKINLEDMLDJ = 1;
				return true;
			}
			int lJJGFHFKGHN = IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN;
			if (lJJGFHFKGHN == 44 || lJJGFHFKGHN == 93 || lJJGFHFKGHN == 125)
			{
				IEBDPKGBOGJ.PLHFFNOPLMM.FMKBGGPKKEN();
				IEBDPKGBOGJ.Return = true;
				IEBDPKGBOGJ.BKINLEDMLDJ = 1;
				return true;
			}
			return false;
		}
		return true;
	}

	private static bool AOMINPGADIM(FsmContext IEBDPKGBOGJ)
	{
		IEBDPKGBOGJ.PLHFFNOPLMM.POLJELICCMK();
		int lJJGFHFKGHN = IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN;
		if (lJJGFHFKGHN == 114)
		{
			IEBDPKGBOGJ.BKINLEDMLDJ = 10;
			return true;
		}
		return false;
	}

	private static bool DFNLBPEFAIM(FsmContext IEBDPKGBOGJ)
	{
		IEBDPKGBOGJ.PLHFFNOPLMM.POLJELICCMK();
		int lJJGFHFKGHN = IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN;
		if (lJJGFHFKGHN == 117)
		{
			IEBDPKGBOGJ.BKINLEDMLDJ = 11;
			return true;
		}
		return false;
	}

	private static bool PNLIDPLFJND(FsmContext IEBDPKGBOGJ)
	{
		IEBDPKGBOGJ.PLHFFNOPLMM.POLJELICCMK();
		int lJJGFHFKGHN = IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN;
		if (lJJGFHFKGHN == 101)
		{
			IEBDPKGBOGJ.Return = true;
			IEBDPKGBOGJ.BKINLEDMLDJ = 1;
			return true;
		}
		return false;
	}

	private static bool DFBHIODPHBG(FsmContext IEBDPKGBOGJ)
	{
		IEBDPKGBOGJ.PLHFFNOPLMM.POLJELICCMK();
		int lJJGFHFKGHN = IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN;
		if (lJJGFHFKGHN == 97)
		{
			IEBDPKGBOGJ.BKINLEDMLDJ = 13;
			return true;
		}
		return false;
	}

	private static bool LDKFPIDDHBD(FsmContext IEBDPKGBOGJ)
	{
		IEBDPKGBOGJ.PLHFFNOPLMM.POLJELICCMK();
		int lJJGFHFKGHN = IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN;
		if (lJJGFHFKGHN == 108)
		{
			IEBDPKGBOGJ.BKINLEDMLDJ = 14;
			return true;
		}
		return false;
	}

	private static bool MHCNLJAHDMO(FsmContext IEBDPKGBOGJ)
	{
		IEBDPKGBOGJ.PLHFFNOPLMM.POLJELICCMK();
		int lJJGFHFKGHN = IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN;
		if (lJJGFHFKGHN == 115)
		{
			IEBDPKGBOGJ.BKINLEDMLDJ = 15;
			return true;
		}
		return false;
	}

	private static bool EOJOLJMHMIE(FsmContext IEBDPKGBOGJ)
	{
		IEBDPKGBOGJ.PLHFFNOPLMM.POLJELICCMK();
		int lJJGFHFKGHN = IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN;
		if (lJJGFHFKGHN == 101)
		{
			IEBDPKGBOGJ.Return = true;
			IEBDPKGBOGJ.BKINLEDMLDJ = 1;
			return true;
		}
		return false;
	}

	private static bool LHEDIFJCNCF(FsmContext IEBDPKGBOGJ)
	{
		IEBDPKGBOGJ.PLHFFNOPLMM.POLJELICCMK();
		int lJJGFHFKGHN = IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN;
		if (lJJGFHFKGHN == 117)
		{
			IEBDPKGBOGJ.BKINLEDMLDJ = 17;
			return true;
		}
		return false;
	}

	private static bool AHPOCKCJPFD(FsmContext IEBDPKGBOGJ)
	{
		IEBDPKGBOGJ.PLHFFNOPLMM.POLJELICCMK();
		int lJJGFHFKGHN = IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN;
		if (lJJGFHFKGHN == 108)
		{
			IEBDPKGBOGJ.BKINLEDMLDJ = 18;
			return true;
		}
		return false;
	}

	private static bool MPBAAPKKHPA(FsmContext IEBDPKGBOGJ)
	{
		IEBDPKGBOGJ.PLHFFNOPLMM.POLJELICCMK();
		int lJJGFHFKGHN = IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN;
		if (lJJGFHFKGHN == 108)
		{
			IEBDPKGBOGJ.Return = true;
			IEBDPKGBOGJ.BKINLEDMLDJ = 1;
			return true;
		}
		return false;
	}

	private static bool BMJONAADIEA(FsmContext IEBDPKGBOGJ)
	{
		while (IEBDPKGBOGJ.PLHFFNOPLMM.POLJELICCMK())
		{
			switch (IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN)
			{
			case 34:
				IEBDPKGBOGJ.PLHFFNOPLMM.FMKBGGPKKEN();
				IEBDPKGBOGJ.Return = true;
				IEBDPKGBOGJ.BKINLEDMLDJ = 20;
				return true;
			case 92:
				IEBDPKGBOGJ.BFIEGKKGJDD = 19;
				IEBDPKGBOGJ.BKINLEDMLDJ = 21;
				return true;
			}
			IEBDPKGBOGJ.PLHFFNOPLMM.string_buffer.Append((char)IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN);
		}
		return true;
	}

	private static bool FILDPJMEDAL(FsmContext IEBDPKGBOGJ)
	{
		IEBDPKGBOGJ.PLHFFNOPLMM.POLJELICCMK();
		int lJJGFHFKGHN = IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN;
		if (lJJGFHFKGHN == 34)
		{
			IEBDPKGBOGJ.Return = true;
			IEBDPKGBOGJ.BKINLEDMLDJ = 1;
			return true;
		}
		return false;
	}

	private static bool PEBHDBIELEI(FsmContext IEBDPKGBOGJ)
	{
		IEBDPKGBOGJ.PLHFFNOPLMM.POLJELICCMK();
		switch (IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN)
		{
		case 117:
			IEBDPKGBOGJ.BKINLEDMLDJ = 22;
			return true;
		case 34:
		case 39:
		case 47:
		case 92:
		case 98:
		case 102:
		case 110:
		case 114:
		case 116:
			IEBDPKGBOGJ.PLHFFNOPLMM.string_buffer.Append(ProcessEscChar(IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN));
			IEBDPKGBOGJ.BKINLEDMLDJ = IEBDPKGBOGJ.BFIEGKKGJDD;
			return true;
		default:
			return false;
		}
	}

	private static bool HCLNFPJIEPH(FsmContext IEBDPKGBOGJ)
	{
		int num = 0;
		int num2 = 4096;
		IEBDPKGBOGJ.PLHFFNOPLMM.FGCEBBKDFPE = 0;
		while (IEBDPKGBOGJ.PLHFFNOPLMM.POLJELICCMK())
		{
			if ((IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN >= 48 && IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN <= 57) || (IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN >= 65 && IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN <= 70) || (IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN >= 97 && IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN <= 102))
			{
				IEBDPKGBOGJ.PLHFFNOPLMM.FGCEBBKDFPE += HexValue(IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN) * num2;
				num++;
				num2 /= 16;
				if (num == 4)
				{
					IEBDPKGBOGJ.PLHFFNOPLMM.string_buffer.Append(Convert.ToChar(IEBDPKGBOGJ.PLHFFNOPLMM.FGCEBBKDFPE));
					IEBDPKGBOGJ.BKINLEDMLDJ = IEBDPKGBOGJ.BFIEGKKGJDD;
					return true;
				}
				continue;
			}
			return false;
		}
		return true;
	}

	private static bool LBIDLILELGF(FsmContext IEBDPKGBOGJ)
	{
		while (IEBDPKGBOGJ.PLHFFNOPLMM.POLJELICCMK())
		{
			switch (IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN)
			{
			case 39:
				IEBDPKGBOGJ.PLHFFNOPLMM.FMKBGGPKKEN();
				IEBDPKGBOGJ.Return = true;
				IEBDPKGBOGJ.BKINLEDMLDJ = 24;
				return true;
			case 92:
				IEBDPKGBOGJ.BFIEGKKGJDD = 23;
				IEBDPKGBOGJ.BKINLEDMLDJ = 21;
				return true;
			}
			IEBDPKGBOGJ.PLHFFNOPLMM.string_buffer.Append((char)IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN);
		}
		return true;
	}

	private static bool DPLKFCBNBBI(FsmContext IEBDPKGBOGJ)
	{
		IEBDPKGBOGJ.PLHFFNOPLMM.POLJELICCMK();
		int lJJGFHFKGHN = IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN;
		if (lJJGFHFKGHN == 39)
		{
			IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN = 34;
			IEBDPKGBOGJ.Return = true;
			IEBDPKGBOGJ.BKINLEDMLDJ = 1;
			return true;
		}
		return false;
	}

	private static bool OEIJIEICKGC(FsmContext IEBDPKGBOGJ)
	{
		IEBDPKGBOGJ.PLHFFNOPLMM.POLJELICCMK();
		switch (IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN)
		{
		case 42:
			IEBDPKGBOGJ.BKINLEDMLDJ = 27;
			return true;
		case 47:
			IEBDPKGBOGJ.BKINLEDMLDJ = 26;
			return true;
		default:
			return false;
		}
	}

	private static bool KODHDINLCML(FsmContext IEBDPKGBOGJ)
	{
		while (IEBDPKGBOGJ.PLHFFNOPLMM.POLJELICCMK())
		{
			if (IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN == 10)
			{
				IEBDPKGBOGJ.BKINLEDMLDJ = 1;
				return true;
			}
		}
		return true;
	}

	private static bool KEHPOJHCJJI(FsmContext IEBDPKGBOGJ)
	{
		while (IEBDPKGBOGJ.PLHFFNOPLMM.POLJELICCMK())
		{
			if (IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN == 42)
			{
				IEBDPKGBOGJ.BKINLEDMLDJ = 28;
				return true;
			}
		}
		return true;
	}

	private static bool HKPHGNANHDD(FsmContext IEBDPKGBOGJ)
	{
		while (IEBDPKGBOGJ.PLHFFNOPLMM.POLJELICCMK())
		{
			if (IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN == 42)
			{
				continue;
			}
			if (IEBDPKGBOGJ.PLHFFNOPLMM.LJJGFHFKGHN == 47)
			{
				IEBDPKGBOGJ.BKINLEDMLDJ = 1;
				return true;
			}
			IEBDPKGBOGJ.BKINLEDMLDJ = 27;
			return true;
		}
		return true;
	}

	private bool POLJELICCMK()
	{
		if ((LJJGFHFKGHN = HPOFLMDBKKJ()) != -1)
		{
			return true;
		}
		LGPODKLEMBP = true;
		return false;
	}

	private int HPOFLMDBKKJ()
	{
		if (KCLLHLMNMKO != 0)
		{
			int kCLLHLMNMKO = KCLLHLMNMKO;
			KCLLHLMNMKO = 0;
			return kCLLHLMNMKO;
		}
		return reader.Read();
	}

	public bool NextToken()
	{
		GFHMIGANEPF.Return = false;
		while (true)
		{
			MGLALMHHOGL mGLALMHHOGL = MOCBIJNMDKH[state - 1];
			if (!mGLALMHHOGL(GFHMIGANEPF))
			{
				throw new JsonException(LJJGFHFKGHN);
			}
			if (LGPODKLEMBP)
			{
				return false;
			}
			if (GFHMIGANEPF.Return)
			{
				break;
			}
			state = GFHMIGANEPF.BKINLEDMLDJ;
		}
		string_value = string_buffer.ToString();
		string_buffer.Remove(0, string_buffer.Length);
		JLFCBDKNAGP = fsm_return_table[state - 1];
		if (JLFCBDKNAGP == 65542)
		{
			JLFCBDKNAGP = LJJGFHFKGHN;
		}
		state = GFHMIGANEPF.BKINLEDMLDJ;
		return true;
	}

	private void FMKBGGPKKEN()
	{
		KCLLHLMNMKO = LJJGFHFKGHN;
	}
}
