using System.Diagnostics;
using System.Xml;
using UnityEngine;

public static class PerkGUI
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Vector2 JPLBKPPAOBD;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Vector2 HPCEDDMDEMJ;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Vector2 HBIMMOBDEME;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Vector2 GKJGKHFHANJ;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static float PMEIDFGIEHL;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static float HGKCCKMPNIJ;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static float LNAHKGCNGLB;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static float FGCOIKOICIF;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static float BJHMGAGELOO;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static float GCOKELCDOKK;

	public static Vector2 KINCPIGBJJN
	{
		get
		{
			return PDNIHJMHKBI();
		}
		private set
		{
			JFEEFEGOKHM(value);
		}
	}

	public static Vector2 MNOFJIPNHDK
	{
		get
		{
			return PELLCOKIJMM();
		}
		private set
		{
			AAFOFMLICPN(value);
		}
	}

	public static Vector2 BHAKGKHLAKK
	{
		get
		{
			return HGHGEAIOHJA();
		}
		private set
		{
			BPCMKNCBNDN(value);
		}
	}

	public static Vector2 EPDFGFIACAF
	{
		get
		{
			return FEHBEIFACMG();
		}
		private set
		{
			set_Spacing(value);
		}
	}

	public static float MHNDBNCHEEJ
	{
		get
		{
			return CPPMFFCKHJI();
		}
		private set
		{
			HHDMIFECNNE(value);
		}
	}

	public static float KPIBFDBDLAJ
	{
		get
		{
			return EHJFNLLGMOG();
		}
		private set
		{
			LMIKMMMLNGJ(value);
		}
	}

	public static float CFFPHNNEPAI
	{
		get
		{
			return OOAHAJJKHKI();
		}
		private set
		{
			LKHJKOOFAAN(value);
		}
	}

	public static float IKBBJCPPPOI
	{
		get
		{
			return IKONKNEHCPB();
		}
		private set
		{
			AEIGGPAGFHF(value);
		}
	}

	public static float DJGCPGHCJHI
	{
		get
		{
			return FKMDJBBMJFM();
		}
		private set
		{
			BODHOAEAHHG(value);
		}
	}

	public static float LJJBGGLDBDI
	{
		get
		{
			return MOLPKLGMBJH();
		}
		private set
		{
			GIPHCNHAADM(value);
		}
	}

	public static Vector2 PDNIHJMHKBI()
	{
		return JPLBKPPAOBD;
	}

	private static void JFEEFEGOKHM(Vector2 value)
	{
		JPLBKPPAOBD = value;
	}

	public static Vector2 PELLCOKIJMM()
	{
		return HPCEDDMDEMJ;
	}

	private static void AAFOFMLICPN(Vector2 value)
	{
		HPCEDDMDEMJ = value;
	}

	public static Vector2 HGHGEAIOHJA()
	{
		return HBIMMOBDEME;
	}

	private static void BPCMKNCBNDN(Vector2 value)
	{
		HBIMMOBDEME = value;
	}

	public static Vector2 FEHBEIFACMG()
	{
		return GKJGKHFHANJ;
	}

	private static void set_Spacing(Vector2 value)
	{
		GKJGKHFHANJ = value;
	}

	public static float CPPMFFCKHJI()
	{
		return PMEIDFGIEHL;
	}

	private static void HHDMIFECNNE(float value)
	{
		PMEIDFGIEHL = value;
	}

	public static float EHJFNLLGMOG()
	{
		return HGKCCKMPNIJ;
	}

	private static void LMIKMMMLNGJ(float value)
	{
		HGKCCKMPNIJ = value;
	}

	public static float OOAHAJJKHKI()
	{
		return LNAHKGCNGLB;
	}

	private static void LKHJKOOFAAN(float value)
	{
		LNAHKGCNGLB = value;
	}

	public static float IKONKNEHCPB()
	{
		return FGCOIKOICIF;
	}

	private static void AEIGGPAGFHF(float value)
	{
		FGCOIKOICIF = value;
	}

	public static float FKMDJBBMJFM()
	{
		return BJHMGAGELOO;
	}

	private static void BODHOAEAHHG(float value)
	{
		BJHMGAGELOO = value;
	}

	public static float MOLPKLGMBJH()
	{
		return GCOKELCDOKK;
	}

	private static void GIPHCNHAADM(float value)
	{
		GCOKELCDOKK = value;
	}

	public static void Parse(XmlNode node)
	{
		JFEEFEGOKHM(node["FadeFrames"].JIIENECAAEH());
		AAFOFMLICPN(node["PulseAccel"].JIIENECAAEH());
		BPCMKNCBNDN(node["PulseFrames"].JIIENECAAEH());
		set_Spacing(new Vector2
		{
			x = node["Spacing"].Attributes["X"].ParseFloat(),
			y = node["Spacing"].Attributes["Y"].ParseFloat()
		});
		HHDMIFECNNE(node["PulseAmp"].PNJPEDPDMCP().ParseFloat());
		LMIKMMMLNGJ(node["RowCapacity"].Attributes["Value"].ParseFloat());
		LKHJKOOFAAN(node["ExpirationOpacity"].Attributes["Value"].ParseFloat());
		AEIGGPAGFHF(node["StackShiftX"].Attributes["Value"].ParseFloat());
		BODHOAEAHHG(node["StackShiftY"].Attributes["Value"].ParseFloat());
		GIPHCNHAADM(node["FontScale"].Attributes["Value"].ParseFloat());
	}
}
