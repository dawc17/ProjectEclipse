using System;
using System.Runtime.CompilerServices;

public class VersionContainer
{
	private enum LOICEAFFHDO
	{
		Equally = 0,
		More = 1,
		Less = 2
	}

	public static readonly VersionContainer Zero = new VersionContainer();

	private readonly int[] _versionSource = new int[4];

	public int JCKJHNFINJL
	{
		get
		{
			return FAOHNABGKFH();
		}
		set
		{
			BINKMFGNMKA(value);
		}
	}

	public int OJEABAEBFIB
	{
		get
		{
			return ELEBDJHKBPL();
		}
		set
		{
			LBPFMNJKNNJ(value);
		}
	}

	public int CGACCLHNJNP
	{
		get
		{
			return FMHLIFBPFBN();
		}
		set
		{
			KOLMOIIPCEC(value);
		}
	}

	public int OGMKHOHOGPD
	{
		get
		{
			return DFJEJKJECBI();
		}
		set
		{
			DPHPJFGOLMJ(value);
		}
	}

	public VersionContainer()
	{
		SetVersion(0, 0, 0, 0);
	}

	public VersionContainer(string version)
	{
		SetVersion(version);
	}

	public VersionContainer(int IGIOOCIDFIN, int IBGMIGIFNJM, int LDKAECLLDNG, int JJCDPPFGPDO = -1)
	{
		SetVersion(IGIOOCIDFIN, IBGMIGIFNJM, LDKAECLLDNG, JJCDPPFGPDO);
	}

	public int FAOHNABGKFH()
	{
		return _versionSource[0];
	}

	public void BINKMFGNMKA(int value)
	{
		_versionSource[0] = value;
	}

	public int ELEBDJHKBPL()
	{
		return _versionSource[1];
	}

	public void LBPFMNJKNNJ(int value)
	{
		_versionSource[1] = value;
	}

	public int FMHLIFBPFBN()
	{
		return _versionSource[2];
	}

	public void KOLMOIIPCEC(int value)
	{
		_versionSource[2] = value;
	}

	public int DFJEJKJECBI()
	{
		return _versionSource[3];
	}

	public void DPHPJFGOLMJ(int value)
	{
		_versionSource[3] = value;
	}

	public void SetVersion(string version)
	{
		version = version.Trim();
		if (string.IsNullOrEmpty(version))
		{
			SetVersion(0, 0, 0, 0);
			return;
		}
		string[] array = version.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries);
		SetVersion(Zero);
		try
		{
			for (int i = 0; i < array.Length; i++)
			{
				_versionSource[i] = int.Parse(array[i]);
			}
		}
		catch (Exception)
		{
			SetVersion(0, 0, 0, 0);
		}
	}

	public void SetVersion(VersionContainer version)
	{
		SetVersion(version.FAOHNABGKFH(), version.ELEBDJHKBPL(), version.FMHLIFBPFBN(), version.DFJEJKJECBI());
	}

	public void SetVersion(int IGIOOCIDFIN, int IBGMIGIFNJM = -1, int LDKAECLLDNG = -1, int JJCDPPFGPDO = -1)
	{
		BINKMFGNMKA(IGIOOCIDFIN);
		LBPFMNJKNNJ(IBGMIGIFNJM);
		KOLMOIIPCEC(LDKAECLLDNG);
		DPHPJFGOLMJ(JJCDPPFGPDO);
	}

	public static VersionContainer CreateVersion(string version)
	{
		return new VersionContainer(version);
	}

	public static VersionContainer CreateVersion(VersionContainer version)
	{
		return new VersionContainer(version.FAOHNABGKFH(), version.ELEBDJHKBPL(), version.FMHLIFBPFBN(), version.DFJEJKJECBI());
	}

	public static VersionContainer CreateVersion(int IGIOOCIDFIN, int IBGMIGIFNJM = -1, int LDKAECLLDNG = -1, int JJCDPPFGPDO = -1)
	{
		return new VersionContainer(IGIOOCIDFIN, IBGMIGIFNJM, LDKAECLLDNG, JJCDPPFGPDO);
	}

	[SpecialName]
	public static bool LFPMCJPCJBD(VersionContainer LHBNIMGFKIB, VersionContainer AAOIAEJJINO)
	{
		LOICEAFFHDO lOICEAFFHDO = Compare(LHBNIMGFKIB, AAOIAEJJINO);
		return lOICEAFFHDO == LOICEAFFHDO.Equally;
	}

	[SpecialName]
	public static bool LFPMCJPCJBD(VersionContainer LHBNIMGFKIB, string AAOIAEJJINO)
	{
		return LFPMCJPCJBD(LHBNIMGFKIB, CreateVersion(AAOIAEJJINO));
	}

	[SpecialName]
	public static bool GLCJKGIOIEC(VersionContainer LHBNIMGFKIB, VersionContainer AAOIAEJJINO)
	{
		return !LFPMCJPCJBD(LHBNIMGFKIB, AAOIAEJJINO);
	}

	[SpecialName]
	public static bool GLCJKGIOIEC(VersionContainer LHBNIMGFKIB, string AAOIAEJJINO)
	{
		return GLCJKGIOIEC(LHBNIMGFKIB, CreateVersion(AAOIAEJJINO));
	}

	[SpecialName]
	public static bool CGMHEDJDOEK(VersionContainer LHBNIMGFKIB, VersionContainer AAOIAEJJINO)
	{
		LOICEAFFHDO lOICEAFFHDO = Compare(LHBNIMGFKIB, AAOIAEJJINO);
		return lOICEAFFHDO == LOICEAFFHDO.More && lOICEAFFHDO != LOICEAFFHDO.Equally;
	}

	[SpecialName]
	public static bool CGMHEDJDOEK(VersionContainer LHBNIMGFKIB, string AAOIAEJJINO)
	{
		return CGMHEDJDOEK(LHBNIMGFKIB, CreateVersion(AAOIAEJJINO));
	}

	[SpecialName]
	public static bool GLLHGKILFFH(VersionContainer LHBNIMGFKIB, VersionContainer AAOIAEJJINO)
	{
		LOICEAFFHDO lOICEAFFHDO = Compare(LHBNIMGFKIB, AAOIAEJJINO);
		return lOICEAFFHDO == LOICEAFFHDO.Less && lOICEAFFHDO != LOICEAFFHDO.Equally;
	}

	[SpecialName]
	public static bool GLLHGKILFFH(VersionContainer LHBNIMGFKIB, string AAOIAEJJINO)
	{
		return GLLHGKILFFH(LHBNIMGFKIB, CreateVersion(AAOIAEJJINO));
	}

	[SpecialName]
	public static bool BCCGLNMPHCE(VersionContainer LHBNIMGFKIB, VersionContainer AAOIAEJJINO)
	{
		LOICEAFFHDO lOICEAFFHDO = Compare(LHBNIMGFKIB, AAOIAEJJINO);
		return lOICEAFFHDO == LOICEAFFHDO.More || lOICEAFFHDO == LOICEAFFHDO.Equally;
	}

	[SpecialName]
	public static bool BCCGLNMPHCE(VersionContainer LHBNIMGFKIB, string AAOIAEJJINO)
	{
		return BCCGLNMPHCE(LHBNIMGFKIB, CreateVersion(AAOIAEJJINO));
	}

	[SpecialName]
	public static bool CDOCLICKACF(VersionContainer LHBNIMGFKIB, VersionContainer AAOIAEJJINO)
	{
		LOICEAFFHDO lOICEAFFHDO = Compare(LHBNIMGFKIB, AAOIAEJJINO);
		return lOICEAFFHDO == LOICEAFFHDO.Less || lOICEAFFHDO == LOICEAFFHDO.Equally;
	}

	[SpecialName]
	public static bool CDOCLICKACF(VersionContainer LHBNIMGFKIB, string AAOIAEJJINO)
	{
		return CDOCLICKACF(LHBNIMGFKIB, CreateVersion(AAOIAEJJINO));
	}

	public string ToString(bool MJHLPGDFEHA)
	{
		if (MJHLPGDFEHA)
		{
			return string.Format("{0}.{1}.{2}.{3}", FAOHNABGKFH(), ELEBDJHKBPL(), FMHLIFBPFBN(), DFJEJKJECBI());
		}
		return string.Format("{0}.{1}.{2}", FAOHNABGKFH(), ELEBDJHKBPL(), FMHLIFBPFBN());
	}

	public override bool Equals(object AOMLCBHAJJH)
	{
		return AOMLCBHAJJH is VersionContainer && LFPMCJPCJBD((VersionContainer)AOMLCBHAJJH, this);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override string ToString()
	{
		return ToString(false);
	}

	[SpecialName]
	public static string op_Implicit(VersionContainer AFIEJABPAKA)
	{
		return AFIEJABPAKA.ToString(true);
	}

	public bool Empty(bool MJHLPGDFEHA = false)
	{
		if (FAOHNABGKFH() == 0 && ELEBDJHKBPL() == 0 && FMHLIFBPFBN() == 0 && (!MJHLPGDFEHA || DFJEJKJECBI() == 0))
		{
			return true;
		}
		return false;
	}

	private static LOICEAFFHDO Compare(VersionContainer LHBNIMGFKIB, VersionContainer AAOIAEJJINO, int IGIEDFIPIAN = 4)
	{
		int[] mOCMENBOJJF = LHBNIMGFKIB._versionSource;
		int[] mOCMENBOJJF2 = AAOIAEJJINO._versionSource;
		for (int i = 0; i < IGIEDFIPIAN; i++)
		{
			if (mOCMENBOJJF[i] != mOCMENBOJJF2[i])
			{
				return (mOCMENBOJJF[i] > mOCMENBOJJF2[i]) ? LOICEAFFHDO.More : LOICEAFFHDO.Less;
			}
		}
		return LOICEAFFHDO.Equally;
	}

	public bool ForCurrentVersion(string JJCDPPFGPDO)
	{
		return ForCurrentVersion(new VersionContainer(JJCDPPFGPDO));
	}

	public bool ForCurrentVersion(VersionContainer JJCDPPFGPDO)
	{
		return Compare(this, JJCDPPFGPDO, 3) == LOICEAFFHDO.Equally;
	}

	public bool CMMIDNOLKPG(VersionContainer JJCDPPFGPDO)
	{
		return Compare(this, JJCDPPFGPDO, 3) == LOICEAFFHDO.Less;
	}
}
