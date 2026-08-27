using System.Collections.Generic;
using UnityEngine;

public static class AtlasCache
{
	private static Dictionary<string, Sprite[]> _CachedAtlases = new Dictionary<string, Sprite[]>();

	public static Sprite[] ENFOJMFEGJH(string ONNKJLOGHGH)
	{
		if (!_CachedAtlases.ContainsKey(ONNKJLOGHGH))
		{
			Sprite[] array = ResourcesAndBundles.BNCMBJOICHI<Sprite>(ONNKJLOGHGH);
			if (array != null && array.Length != 0)
			{
				_CachedAtlases.Add(ONNKJLOGHGH, array);
				return array;
			}
			Sprite[] array2 = Resources.LoadAll<Sprite>(ONNKJLOGHGH);
			if (array2 != null && array2.Length != 0)
			{
				_CachedAtlases.Add(ONNKJLOGHGH, array2);
				return array2;
			}
			return array;
		}
		return _CachedAtlases[ONNKJLOGHGH];
	}

	public static Sprite GetSpriteFromAtlas(string ONNKJLOGHGH, string CMMPHNJDOCF)
	{
		Sprite[] array = ENFOJMFEGJH(ONNKJLOGHGH);
		if ((array == null || array.Length == 0) && !string.IsNullOrEmpty(ONNKJLOGHGH))
		{
			string text = ONNKJLOGHGH;
			int num = text.IndexOf('/');
			text = (num >= 0) ? text.Substring(num + 1) : text;
			array = ENFOJMFEGJH(text);
		}
		if (array == null || array.Length == 0)
		{
			array = ENFOJMFEGJH("ui/atlases");
		}
		Sprite[] array2 = array;
		foreach (Sprite sprite in array2)
		{
			if (sprite.name == CMMPHNJDOCF)
			{
				return sprite;
			}
		}
		return null;
	}

	public static void Clear()
	{
		_CachedAtlases.Clear();
	}
}
