using System.Collections.Generic;
using UnityEngine;

public static class LocationSpriteCache
{
	private static Dictionary<string, Sprite[]> _CachedAtlases = new Dictionary<string, Sprite[]>();

	private static Dictionary<string, Sprite> _CachedSingleSprite = new Dictionary<string, Sprite>();

	public static Sprite[] ENFOJMFEGJH(string ONNKJLOGHGH)
	{
		if (!_CachedAtlases.ContainsKey(ONNKJLOGHGH))
		{
			Sprite[] array = ResourcesAndBundles.BNCMBJOICHI<Sprite>(ONNKJLOGHGH);
			if (array != null)
			{
				_CachedAtlases.Add(ONNKJLOGHGH, array);
			}
			return array;
		}
		return _CachedAtlases[ONNKJLOGHGH];
	}

	private static Sprite OPHFAHOKBOK(string PPAJIHNNNDG, string CMMPHNJDOCF)
	{
		string text = string.Format("{0}/{1}", PPAJIHNNNDG, CMMPHNJDOCF);
		if (_CachedSingleSprite.ContainsKey(text))
		{
			return _CachedSingleSprite[text];
		}
		Sprite sprite;
		if (!Eclipse.Modding.ModAssetBinding.TryLoadSprite(text, out sprite))
		{
			sprite = ResourcesAndBundles.Load<Sprite>(text);
		}
		_CachedSingleSprite[text] = sprite;
		return sprite;
	}

	public static Sprite PPBEKKDIJKC(string PPAJIHNNNDG, string CMMPHNJDOCF, string BBPGNOBFECF)
	{
		if (Eclipse.Modding.AssetId.TryParse(PPAJIHNNNDG, out _))
		{
			// Qualified core/mod location art is addressed per sprite. Legacy atlas sub-assets
			// remain available for installed/core locations through the branch below.
			return OPHFAHOKBOK(PPAJIHNNNDG, CMMPHNJDOCF);
		}
        if (Eclipse.Modding.ModRuntime.TryResolveCoreReplacement(PPAJIHNNNDG + "/" + CMMPHNJDOCF, out var replacement))
            return Eclipse.Modding.ModRuntime.Host.TypedAssets.LoadSprite(replacement);
		if (!string.IsNullOrEmpty(BBPGNOBFECF))
		{
			string oNNKJLOGHGH = string.Format("{0}/{1}", PPAJIHNNNDG, BBPGNOBFECF);
			Sprite[] array = ENFOJMFEGJH(oNNKJLOGHGH);
			Sprite[] array2 = array ?? new Sprite[0];
			foreach (Sprite sprite in array2)
			{
				if (sprite.name == CMMPHNJDOCF)
				{
					return sprite;
				}
			}
		}
		return OPHFAHOKBOK(PPAJIHNNNDG, CMMPHNJDOCF);
	}

	public static void Clear()
	{
		_CachedAtlases.Clear();
		_CachedSingleSprite.Clear();
		CocosAnimationData.DECIILEPLDM();
	}
}
