using System;
using UnityEngine;

public class ResourcesUtil
{
	public static T[] GetResources<T>(string path) where T : UnityEngine.Object
	{
		path = FKJBCOEMBOC(path);
		return Resources.LoadAll<T>(path);
	}

	public static T GetResource<T>(string path) where T : UnityEngine.Object
	{
		path = FKJBCOEMBOC(path);
		return Resources.Load<T>(path);
	}

	public static void UnloadAsset(UnityEngine.Object value, bool OJCKACIMFEJ = true)
	{
		if (value is GameObject)
		{
			GlobalLoad.CHILAIJNEHG(value, OJCKACIMFEJ);
		}
		else
		{
			Resources.UnloadAsset(value);
		}
	}

	public static void KGNLHIKNDLL()
	{
		Resources.UnloadUnusedAssets();
	}

	private static string FKJBCOEMBOC(string path)
	{
		string text = "resources/";
		int num = path.LastIndexOf(text, StringComparison.OrdinalIgnoreCase);
		if (num > -1)
		{
			num += text.Length;
			int length = GlobalPath.GetIndexExtension(path, num);
			return path.Substring(num, length);
		}
		return path;
	}
}
