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
			if (Eclipse.Modding.ModRuntime.TryLoadCore(ONEIGMLOGDC, out modAsset))
			{
				return modAsset;
			}

		if (IsLooseLocationAsset(ONEIGMLOGDC))
		{
			return Resources.Load<T>(ONEIGMLOGDC);
		}

			// Core assets are owned by the reserved core namespace. The provider currently
			// reads TAR/LZ4 through PackagedArtCatalog; callers do not depend on that storage.
			return Resources.Load<T>(ONEIGMLOGDC);
	}

	public static T[] BNCMBJOICHI<T>(string ONEIGMLOGDC) where T : Object
	{
		T[] modAssets;
			if (Eclipse.Modding.ModRuntime.TryLoadQualifiedWithSubAssets(ONEIGMLOGDC, out modAssets))
			{
				return modAssets;
			}
			if (Eclipse.Modding.ModRuntime.TryLoadCoreWithSubAssets(ONEIGMLOGDC, out modAssets))
			{
				return modAssets;
			}

		if (IsLooseLocationAsset(ONEIGMLOGDC))
		{
			return Resources.LoadAll<T>(ONEIGMLOGDC);
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
