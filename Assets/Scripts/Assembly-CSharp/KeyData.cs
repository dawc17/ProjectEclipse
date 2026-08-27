using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class KeyData
{
	public enum MCIDLLKHKDE
	{
		BOTH = 0,
		SEQUENCE = 1
	}

	public class JABEIBOFKDM
	{
		public float DPGMCKCDMBC;

		public float EBDBPJNBHGI;
	}

	public class Distances
	{
		public JABEIBOFKDM GAIBPAGPEGK;

		public JABEIBOFKDM DEJPGJNPKFC;

		public JABEIBOFKDM DJJJNIEAKGJ;

		public Distances()
		{
		}

		public Distances(Distances NBMGOEMJJAF)
		{
			GAIBPAGPEGK = NBMGOEMJJAF.GAIBPAGPEGK;
			DEJPGJNPKFC = NBMGOEMJJAF.DEJPGJNPKFC;
			DJJJNIEAKGJ = NBMGOEMJJAF.DJJJNIEAKGJ;
		}
	}

	public List<int> IGEEOAGOMEM = new List<int>();

	public List<int> CEPODJDDLBF = new List<int>();

	public List<int> HPEOJLAMIHC = new List<int>();

	public Distances CAGHDJNDFLJ = new Distances();

	public MCIDLLKHKDE HGPMABCJGGN;

	public bool IsInverted;

	public KeyData()
	{
		IsInverted = false;
		HGPMABCJGGN = MCIDLLKHKDE.BOTH;
	}

	public KeyData(KeyData NBMGOEMJJAF)
	{
		IGEEOAGOMEM = new List<int>(NBMGOEMJJAF.IGEEOAGOMEM);
		CEPODJDDLBF = new List<int>(NBMGOEMJJAF.CEPODJDDLBF);
		HPEOJLAMIHC = new List<int>(NBMGOEMJJAF.HPEOJLAMIHC);
		CAGHDJNDFLJ = new Distances(NBMGOEMJJAF.CAGHDJNDFLJ);
		HGPMABCJGGN = NBMGOEMJJAF.HGPMABCJGGN;
		IsInverted = NBMGOEMJJAF.IsInverted;
	}

	public void Set(KeyData NBMGOEMJJAF)
	{
		Clear();
		IGEEOAGOMEM.Clear();
		IGEEOAGOMEM.AddRange(NBMGOEMJJAF.IGEEOAGOMEM);
		CEPODJDDLBF.AddRange(NBMGOEMJJAF.CEPODJDDLBF);
		HPEOJLAMIHC.AddRange(NBMGOEMJJAF.HPEOJLAMIHC);
		HGPMABCJGGN = NBMGOEMJJAF.HGPMABCJGGN;
	}

	public KeyData Copy()
	{
		return new KeyData(this);
	}

	public void Reverse(int AOJJBKLCHJO)
	{
		if (AOJJBKLCHJO < 0)
		{
			MirrorKeys(CEPODJDDLBF);
			MirrorKeys(IGEEOAGOMEM);
			MirrorKeys(HPEOJLAMIHC);
		}
	}

	public bool IsVariable(KeyData OKJABKNIBEO)
	{
		return CompareKeys(IGEEOAGOMEM, OKJABKNIBEO.IGEEOAGOMEM) && CompareKeys(CEPODJDDLBF, OKJABKNIBEO.CEPODJDDLBF) && CompareKeys(HPEOJLAMIHC, OKJABKNIBEO.HPEOJLAMIHC);
	}

	public void Clear()
	{
		CEPODJDDLBF.Clear();
		HPEOJLAMIHC.Clear();
		HGPMABCJGGN = MCIDLLKHKDE.BOTH;
	}

	public string EIHLEEHGEEO()
	{
		if (IGEEOAGOMEM.Count == 0 && CEPODJDDLBF.Count == 0)
		{
			return string.Empty;
		}
		int num = 0;
		string empty = string.Empty;
		string text = "KeyData: ";
		text += "starter:";
		foreach (int item in IGEEOAGOMEM)
		{
			empty = ((num < IGEEOAGOMEM.Count - 1) ? "," : string.Empty);
			text = text + item + empty;
			num++;
		}
		num = 0;
		text += " additional:";
		foreach (int item2 in CEPODJDDLBF)
		{
			empty = ((num < CEPODJDDLBF.Count - 1) ? "," : string.Empty);
			text = text + item2 + empty;
			num++;
		}
		text += " pressType:";
		switch (HGPMABCJGGN)
		{
		case MCIDLLKHKDE.BOTH:
			return text + "BOTH";
		case MCIDLLKHKDE.SEQUENCE:
			return text + "SEQUENCE";
		default:
			return text + "? ";
		}
	}

	public string HDIPIBOONDG()
	{
		string text = string.Empty;
		int i = 0;
		for (int count = IGEEOAGOMEM.Count; i < count; i++)
		{
			int mJGKGLGJHHK = IGEEOAGOMEM[i];
			text += KeyToString(mJGKGLGJHHK);
			if (i < count - 1)
			{
				text += " + ";
			}
		}
		text += " ADD: ";
		int j = 0;
		for (int count2 = CEPODJDDLBF.Count; j < count2; j++)
		{
			int mJGKGLGJHHK2 = CEPODJDDLBF[j];
			text += KeyToString(mJGKGLGJHHK2);
			if (j < count2 - 1)
			{
				text += " + ";
			}
		}
		text += " PressType: ";
		switch (HGPMABCJGGN)
		{
		case MCIDLLKHKDE.BOTH:
			text += "Both";
			break;
		case MCIDLLKHKDE.SEQUENCE:
			text += "Sequence";
			break;
		}
		return text;
	}

	public void ResetPressType()
	{
		if (IGEEOAGOMEM.Count == 1)
		{
			HGPMABCJGGN = MCIDLLKHKDE.BOTH;
			return;
		}
		foreach (int item in IGEEOAGOMEM)
		{
			foreach (int item2 in CEPODJDDLBF)
			{
				if (item == item2)
				{
					HGPMABCJGGN = MCIDLLKHKDE.BOTH;
					return;
				}
			}
		}
		HGPMABCJGGN = MCIDLLKHKDE.SEQUENCE;
	}

	[SpecialName]
	public static bool LFPMCJPCJBD(KeyData LHBNIMGFKIB, KeyData AAOIAEJJINO)
	{
		if (LHBNIMGFKIB.IsInverted == AAOIAEJJINO.IsInverted && LHBNIMGFKIB.HGPMABCJGGN == AAOIAEJJINO.HGPMABCJGGN && LHBNIMGFKIB.IGEEOAGOMEM.Count == AAOIAEJJINO.IGEEOAGOMEM.Count && LHBNIMGFKIB.CEPODJDDLBF.Count == AAOIAEJJINO.CEPODJDDLBF.Count && LHBNIMGFKIB.IGEEOAGOMEM == AAOIAEJJINO.IGEEOAGOMEM && LHBNIMGFKIB.CEPODJDDLBF == AAOIAEJJINO.CEPODJDDLBF)
		{
			return true;
		}
		return false;
	}

	[SpecialName]
	public static bool GLCJKGIOIEC(KeyData LHBNIMGFKIB, KeyData AAOIAEJJINO)
	{
		return !LFPMCJPCJBD(LHBNIMGFKIB, AAOIAEJJINO);
	}

	private static void MirrorKeys(List<int> EGJHGBCEPHO)
	{
		for (int i = 0; i < EGJHGBCEPHO.Count; i++)
		{
			switch ((FightCID)EGJHGBCEPHO[i])
			{
			case FightCID.QuadrantUpForward:
				EGJHGBCEPHO[i] = 8;
				break;
			case FightCID.QuadrantForward:
				EGJHGBCEPHO[i] = 7;
				break;
			case FightCID.QuadrantDownForward:
				EGJHGBCEPHO[i] = 6;
				break;
			case FightCID.QuadrantDownBack:
				EGJHGBCEPHO[i] = 4;
				break;
			case FightCID.QuadrantBack:
				EGJHGBCEPHO[i] = 3;
				break;
			case FightCID.QuadrantUpBack:
				EGJHGBCEPHO[i] = 2;
				break;
			}
		}
	}

	private static bool CompareKeys(List<int> BMKNHNOGIHO, List<int> GDOOLJGKOMG)
	{
		return GDOOLJGKOMG.ANNPHPHLNEH(BMKNHNOGIHO);
	}

	private static string KeyToString(int MJGKGLGJHHK)
	{
		switch ((FightCID)MJGKGLGJHHK)
		{
		case FightCID.QuadrantUp:
			return "U";
		case FightCID.QuadrantUpForward:
			return "UF";
		case FightCID.QuadrantForward:
			return "F";
		case FightCID.QuadrantDownForward:
			return "DF";
		case FightCID.QuadrantDown:
			return "D";
		case FightCID.QuadrantDownBack:
			return "DB";
		case FightCID.QuadrantBack:
			return "B";
		case FightCID.QuadrantUpBack:
			return "UB";
		case FightCID.Punch:
			return "Punch";
		case FightCID.Kick:
			return "Kick";
		case FightCID.MissileButton:
			return "Throw";
		case FightCID.MagicButton:
			return "Magic";
		case FightCID.RaidChargeButton:
			return "RaidCharge";
		default:
			return string.Empty;
		}
	}
}
