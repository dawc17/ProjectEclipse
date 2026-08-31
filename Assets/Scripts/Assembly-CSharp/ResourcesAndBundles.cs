using UnityEngine;

public static class ResourcesAndBundles
{
	public static T Load<T>(string ONEIGMLOGDC) where T : Object
	{
		T modAsset;
		if (Eclipse.Modding.ModRuntime.TryLoadQualified(ONEIGMLOGDC, out modAsset))
		{
			return modAsset;
		}

		if (IsLooseLocationAsset(ONEIGMLOGDC))
		{
			return Resources.Load<T>(ONEIGMLOGDC);
		}

		T val = Eclipse.Content.PackagedArtCatalog.Load<T>(ONEIGMLOGDC);
		if (val != null)
		{
			return val;
		}
		// Old downloaded bundles must not override the project-owned content set.
		return Resources.Load<T>(ONEIGMLOGDC);
	}

	public static T[] BNCMBJOICHI<T>(string ONEIGMLOGDC) where T : Object
	{
		T[] modAssets;
		if (Eclipse.Modding.ModRuntime.TryLoadQualifiedWithSubAssets(ONEIGMLOGDC, out modAssets))
		{
			return modAssets;
		}

		if (IsLooseLocationAsset(ONEIGMLOGDC))
		{
			return Resources.LoadAll<T>(ONEIGMLOGDC);
		}

		T[] array = Eclipse.Content.PackagedArtCatalog.LoadWithSubAssets<T>(ONEIGMLOGDC);
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
		// Location art is packaged like every other runtime visual. Location params stay
		// loose because stage layout/collision/music is part of the moddable config layer.
		return path.StartsWith("gamedata/locations/", System.StringComparison.OrdinalIgnoreCase);
	}
}
