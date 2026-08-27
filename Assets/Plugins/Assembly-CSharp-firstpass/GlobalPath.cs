using System;
using System.IO;
using UnityEngine;

public class GlobalPath
{
	private const string PathToResourcesFolder = "Resources";

	public static readonly string PathToLoaderFolder = "assets/gamedata/" + "Resources".ToLower();

	public static string FDIJFOKPBHM
	{
		get
		{
			return LCHPJEDOJKK();
		}
	}

	public static string EKBALHCKKFO
	{
		get
		{
			return MNACDIFKBDG();
		}
	}

	public static string NJIAIFAKOAF
	{
		get
		{
			return DPPLJPBLPKF();
		}
	}

	public static string BOMMODFGANC
	{
		get
		{
			return AIHMCHABGKO();
		}
	}

	public static string KIEDBJBBHOF
	{
		get
		{
			return ENDDFPLIPNK();
		}
	}

	public static string LCHPJEDOJKK()
	{
		return AIHMCHABGKO() + "/Logs";
	}

	public static string MNACDIFKBDG()
	{
		return AIHMCHABGKO() + "/Resources";
	}

	public static string DPPLJPBLPKF()
	{
		return ENDDFPLIPNK() + "/Resources";
	}

	public static string AIHMCHABGKO()
	{
		if (SystemProperties.GAAMHGCDANB())
		{
			return Application.persistentDataPath + "/gamedata";
		}
		string text = Application.dataPath.Replace("Assets", string.Empty).TrimEnd('/');
		return text + "/gamedata";
	}

	public static string ENDDFPLIPNK()
	{
		return Application.dataPath + "/gamedata";
	}

	public static int GetIndexExtension(string path, int BOGBPHDFGGB = 0)
	{
		int num = path.LastIndexOf(".", StringComparison.OrdinalIgnoreCase);
		return (num <= -1) ? (path.Length - BOGBPHDFGGB) : (num - BOGBPHDFGGB);
	}

	public static string FGGDOBKOCAN(string path)
	{
		return Path.Combine(AIHMCHABGKO(), path);
	}

	private static string HFGMHHDBHMH(string KGBGENDIMBC)
	{
		return InternalSettings.HFGMHHDBHMH(KGBGENDIMBC);
	}

	public static string GetInternalPath(string KGBGENDIMBC, string name = "")
	{
		return HFGMHHDBHMH(KGBGENDIMBC) + name;
	}

	public static string FLBGANCBDBB(string path)
	{
		return PathToLoaderFolder + "/" + path;
	}
}
