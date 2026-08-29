using UnityEngine;

public static class ResourcesAndBundles
{
	public static T Load<T>(string ONEIGMLOGDC) where T : Object
	{
		if (IsLooseLocationAsset(ONEIGMLOGDC))
		{
			return Resources.Load<T>(ONEIGMLOGDC);
		}

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
		if (IsLooseLocationAsset(ONEIGMLOGDC))
		{
			return Resources.LoadAll<T>(ONEIGMLOGDC);
		}

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

	private static bool IsLooseLocationAsset(string resourcePath)
	{
		if (string.IsNullOrEmpty(resourcePath))
		{
			return false;
		}

		string path = resourcePath.Replace((char)92, '/').TrimStart('/');
		return path.StartsWith("Textures/Locations/", System.StringComparison.OrdinalIgnoreCase) ||
			path.StartsWith("Textures/Location_effects/", System.StringComparison.OrdinalIgnoreCase) ||
			path.StartsWith("gamedata/locations/", System.StringComparison.OrdinalIgnoreCase);
	}
}
