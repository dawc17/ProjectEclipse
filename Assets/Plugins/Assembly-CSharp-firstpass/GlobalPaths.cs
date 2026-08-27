using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using Nekki.Yaml;
using UnityEngine;

public static class GlobalPaths
{
	public struct CJBHPHFBBKP
	{
		public string FKPKHCBIKBG;

		public string OIBCCCHONFF;

		public string PKLNJHNAPCA;

		public string JFMBIGDDPNG;

		public bool OHOKEGGOJBH;

		public bool CFELJJPPPFG;

		public bool JFGGPBKFAFP;

		public static CJBHPHFBBKP Empty = new CJBHPHFBBKP(string.Empty, string.Empty, string.Empty, string.Empty);

		public bool LPGLCGMMPHN
		{
			get
			{
				return DCIFFHJCIJD();
			}
		}

		public CJBHPHFBBKP(string IDGLPJGEFKB, string NEPCCFPPPIG, string bundle, string IMFLNPNECCO)
		{
			if (string.IsNullOrEmpty(IDGLPJGEFKB))
			{
				CFELJJPPPFG = false;
				FKPKHCBIKBG = string.Empty;
			}
			else
			{
				CFELJJPPPFG = true;
				FKPKHCBIKBG = IDGLPJGEFKB.Trim('/').Trim('\\');
			}
			if (string.IsNullOrEmpty(NEPCCFPPPIG))
			{
				JFGGPBKFAFP = false;
				OIBCCCHONFF = string.Empty;
			}
			else
			{
				JFGGPBKFAFP = true;
				OIBCCCHONFF = NEPCCFPPPIG.Trim('/').Trim('\\');
			}
			if (string.IsNullOrEmpty(bundle))
			{
				OHOKEGGOJBH = false;
				PKLNJHNAPCA = string.Empty;
			}
			else
			{
				OHOKEGGOJBH = true;
				PKLNJHNAPCA = bundle.Trim('/').Trim('\\');
			}
			JFMBIGDDPNG = IMFLNPNECCO.Trim('/').Trim('\\');
		}

		public bool DCIFFHJCIJD()
		{
			return !CFELJJPPPFG && !JFGGPBKFAFP && !OHOKEGGOJBH;
		}

		public CJBHPHFBBKP JJMAGODLBBI(string ALHKHJOJECK)
		{
			if (CFELJJPPPFG)
			{
				FKPKHCBIKBG = FKPKHCBIKBG + "/" + ALHKHJOJECK;
			}
			if (JFGGPBKFAFP)
			{
				OIBCCCHONFF = OIBCCCHONFF + "/" + ALHKHJOJECK;
			}
			if (OHOKEGGOJBH)
			{
				PKLNJHNAPCA = PKLNJHNAPCA + "/" + ALHKHJOJECK;
			}
			return this;
		}

		[SpecialName]
		public static string op_Explicit(CJBHPHFBBKP PIIEECCHMAC)
		{
			return PIIEECCHMAC.OIBCCCHONFF;
		}
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static YamlDocumentNekki LMGNAMMFLBF;

	private static readonly Dictionary<string, string> Pathes;

	private const string PathToResourcesFolder = "gamedata/Resources";

	private const string PIDKJDBEDJM = "gamedata/Bundles";

	public static YamlDocumentNekki PNLGKAMCBKF
	{
		get
		{
			return BAMNPCEMNCM();
		}
		private set
		{
			set_ConfigYaml(value);
		}
	}

	public static string POHBMFIKMCP
	{
		get
		{
			return IJNKMAPOJGJ();
		}
	}

	public static string EKBALHCKKFO
	{
		get
		{
			return MNACDIFKBDG();
		}
	}

	public static string BAAIALFLGJL
	{
		get
		{
			return CJAAKDKKHFD();
		}
	}

	public static string CALAEHKMGLE
	{
		get
		{
			return EPLPHDJLFBD();
		}
	}

	public static string OAOBEMPIMLM
	{
		get
		{
			return CNPDIADGBPA();
		}
	}

	static GlobalPaths()
	{
		Pathes = new Dictionary<string, string>();
		Pathes = new Dictionary<string, string>();
	}

	public static YamlDocumentNekki BAMNPCEMNCM()
	{
		return LMGNAMMFLBF;
	}

	private static void set_ConfigYaml(YamlDocumentNekki value)
	{
		LMGNAMMFLBF = value;
	}

	public static string IJNKMAPOJGJ()
	{
		return string.Format("{0}/{1}", Application.dataPath, "gamedata/Resources");
	}

	public static string MNACDIFKBDG()
	{
		if (SystemProperties.GAAMHGCDANB())
		{
			return string.Format("{0}/{1}", Application.persistentDataPath, "gamedata/Resources");
		}
		return string.Format("{0}/{1}", Application.dataPath.Replace("Assets", string.Empty).TrimEnd('/'), "gamedata/Resources");
	}

	public static string CJAAKDKKHFD()
	{
		if (SystemProperties.GAAMHGCDANB())
		{
			return string.Format("{0}/{1}", Application.persistentDataPath, "gamedata/Bundles");
		}
		return string.Format("{0}/{1}", Application.dataPath.Replace("Assets", string.Empty).TrimEnd('/'), "gamedata/Bundles");
	}

	public static string EPLPHDJLFBD()
	{
		return (SystemProperties.NICPICAMAOH().ACHKMBJANGN != SystemProperties.LOHALAKNGFB.PATH_BIG) ? "768" : "1536";
	}

	public static string LIJOKOHJBGP()
	{
		if (Application.isMobilePlatform)
		{
			return Application.persistentDataPath;
		}
		if (Application.isEditor)
		{
			return Environment.CurrentDirectory + Path.DirectorySeparatorChar + "gamedata";
		}
		return Application.dataPath + Path.DirectorySeparatorChar + "gamedata";
	}

	public static string CNPDIADGBPA()
	{
		return DILAKCHLHDL("AssetServer");
	}

	public static string KABLNHKINNG(string name)
	{
		if (Pathes.ContainsKey(name))
		{
			return Pathes[name].Replace("EXTERNAL_PATH", MNACDIFKBDG()).Replace("CURRENT_RESOLUTION", EPLPHDJLFBD()).TrimEnd('/')
				.Trim('\\');
		}
		AdvLog.CCOFFJPPAKC(string.Format("path not found: {0}", name));
		return string.Empty;
	}

	public static string DILAKCHLHDL(string name)
	{
		if (Pathes.ContainsKey(name))
		{
			return Pathes[name].Replace("EXTERNAL_PATH", MNACDIFKBDG()).Replace("CURRENT_RESOLUTION", EPLPHDJLFBD()).Trim('/')
				.Trim('\\');
		}
		AdvLog.CCOFFJPPAKC(string.Format("path not found: {0}", name));
		return string.Empty;
	}

	public static string MIMCIMJEGMA(string name)
	{
		if (Pathes.ContainsKey(name))
		{
			return Pathes[name].Trim('/').Trim('\\');
		}
		AdvLog.CCOFFJPPAKC(string.Format("path not found: {0}", name));
		return string.Empty;
	}

	public static CJBHPHFBBKP HFGMHHDBHMH(string name)
	{
		if (Pathes.ContainsKey(name))
		{
			string text = Pathes[name];
			return new CJBHPHFBBKP(text.Replace("EXTERNAL_PATH/", string.Empty).Replace("CURRENT_RESOLUTION/", string.Empty), text.Replace("EXTERNAL_PATH", MNACDIFKBDG()).Replace("CURRENT_RESOLUTION", EPLPHDJLFBD()), text.Replace("EXTERNAL_PATH/", string.Empty).Replace("CURRENT_RESOLUTION/", EPLPHDJLFBD()), text);
		}
		AdvLog.CCOFFJPPAKC(string.Format("path not found: {0}", name));
		return CJBHPHFBBKP.Empty;
	}

	public static CJBHPHFBBKP PAOGAEOEFLP(string OKJFMFILPOB)
	{
		OKJFMFILPOB = "EXTERNAL_PATH/" + OKJFMFILPOB;
		return new CJBHPHFBBKP(OKJFMFILPOB.Replace("EXTERNAL_PATH/", string.Empty).Replace("CURRENT_RESOLUTION/", string.Empty), OKJFMFILPOB.Replace("EXTERNAL_PATH", MNACDIFKBDG()).Replace("CURRENT_RESOLUTION", EPLPHDJLFBD()), OKJFMFILPOB.Replace("EXTERNAL_PATH/", string.Empty).Replace("CURRENT_RESOLUTION/", EPLPHDJLFBD()), OKJFMFILPOB);
	}
}
