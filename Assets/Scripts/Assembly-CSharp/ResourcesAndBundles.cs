using UnityEngine;

public static class ResourcesAndBundles
{
	public static T Load<T>(string ONEIGMLOGDC) where T : Object
	{
		T val = Eclipse.Content.ResearchArtBundleOverride.Load<T>(ONEIGMLOGDC);
		if (val != null)
		{
			return val;
		}
		val = BundleManager.LoadAsset<T>(ONEIGMLOGDC);
		if (val != null)
		{
			return val;
		}
		return Resources.Load<T>(ONEIGMLOGDC);
	}

	public static T[] BNCMBJOICHI<T>(string ONEIGMLOGDC) where T : Object
	{
		T[] array = Eclipse.Content.ResearchArtBundleOverride.LoadWithSubAssets<T>(ONEIGMLOGDC);
		if (array != null)
		{
			return array;
		}
		array = BundleManager.LoadAssetWithSubAssets<T>(ONEIGMLOGDC);
		if (array != null)
		{
			return array;
		}
		return Resources.LoadAll<T>(ONEIGMLOGDC);
	}
}
