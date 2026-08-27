using System.IO;
using UnityEngine;

public static class AssetBundleExtension
{
	public static string KBCDKMKHIBB(string JHEMALDDIFN)
	{
		if (!JHEMALDDIFN.Contains(SF2Paths.ECJMHJOMMBC()))
		{
			JHEMALDDIFN = string.Format("{0}/{1}.png", SF2Paths.ECJMHJOMMBC(), JHEMALDDIFN);
		}
		return JHEMALDDIFN.ToLower();
	}

	public static string NCDIGHNCMFH(string JHEMALDDIFN)
	{
		return Path.GetFileNameWithoutExtension(JHEMALDDIFN.ToLower());
	}

	public static string[] GetAllSimplifiedAssetNames(this AssetBundle ACOAHGKKMFC)
	{
		string[] allAssetNames = ACOAHGKKMFC.GetAllAssetNames();
		string[] array = new string[allAssetNames.Length];
		int i = 0;
		for (int num = allAssetNames.Length; i < num; i++)
		{
			array[i] = NCDIGHNCMFH(allAssetNames[i]);
		}
		return array;
	}
}
