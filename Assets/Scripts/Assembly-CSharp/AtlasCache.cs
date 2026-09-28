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
			_CachedAtlases[ONNKJLOGHGH] = array2 ?? new Sprite[0];
            return _CachedAtlases[ONNKJLOGHGH];
		}
		return _CachedAtlases[ONNKJLOGHGH];
	}

	public static Sprite GetSpriteFromAtlas(string ONNKJLOGHGH, string CMMPHNJDOCF)
	{
        if (Eclipse.Modding.ModRuntime.TryLoadCoreSpriteReplacement(ONNKJLOGHGH, CMMPHNJDOCF, out var replacement)) return replacement;
        // Selected on-screen control texture pack (FightButtons only); sized from the recovered sprite.
        if (Eclipse.UI.ControlTexturePacks.TryGetSprite(ONNKJLOGHGH, CMMPHNJDOCF, RecoveredSpriteLoader, out var packed)) return packed;
		return LoadRecoveredSprite(ONNKJLOGHGH, CMMPHNJDOCF);
	}

	private static readonly System.Func<string, string, Sprite> RecoveredSpriteLoader = LoadRecoveredSprite;

	private static Sprite LoadRecoveredSprite(string ONNKJLOGHGH, string CMMPHNJDOCF)
	{
		if (ONNKJLOGHGH == Eclipse.UI.ControlTexturePacks.AtlasPath)
		{
			Sprite standalone = Resources.Load<Sprite>("ui/atlases/" + CMMPHNJDOCF);
			if (standalone != null) return standalone;
		}
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
			return ResourcesAndBundles.Load<Sprite>("ui/atlases/" + CMMPHNJDOCF);
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
