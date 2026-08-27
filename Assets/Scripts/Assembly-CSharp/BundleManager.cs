using System.Collections.Generic;
using UnityEngine;

public static class BundleManager
{
	private static Dictionary<string, BundleRef> DHNMJFPPGJL = new Dictionary<string, BundleRef>();

	private static BundleRef EBKGJFPJDJJ = null;

	public static void AddBundle(string name)
	{
		BundleRef fJMBOHIMAMI = new BundleRef(name);
		fJMBOHIMAMI.Load();
		if (fJMBOHIMAMI.DFDMGDDLDNB())
		{
			string[] array = fJMBOHIMAMI.OEEOEGEBBAL();
			string[] array2 = array;
			foreach (string jHEMALDDIFN in array2)
			{
				string key = AssetBundleExtension.NCDIGHNCMFH(jHEMALDDIFN);
				DHNMJFPPGJL[key] = fJMBOHIMAMI;
			}
		}
		fJMBOHIMAMI.BPEDLFOKKNN();
	}

	public static T LoadAsset<T>(string name) where T : Object
	{
		name = AssetBundleExtension.NCDIGHNCMFH(name);
		BundleRef value = null;
		if (!DHNMJFPPGJL.TryGetValue(name, out value))
		{
			return (T)null;
		}
		EICEEPBNHOG(value);
		return (!value.DFDMGDDLDNB()) ? ((T)null) : value.LoadAsset<T>(name);
	}

	public static T[] LoadAssetWithSubAssets<T>(string name) where T : Object
	{
		name = AssetBundleExtension.NCDIGHNCMFH(name);
		BundleRef value = null;
		if (!DHNMJFPPGJL.TryGetValue(name, out value))
		{
			return null;
		}
		EICEEPBNHOG(value);
		return (!value.DFDMGDDLDNB()) ? null : value.LoadAssetWithSubAssets<T>(name);
	}

	private static void EICEEPBNHOG(BundleRef bundle)
	{
		if (bundle != null && EBKGJFPJDJJ != bundle)
		{
			if (EBKGJFPJDJJ != null)
			{
				EBKGJFPJDJJ.BPEDLFOKKNN();
			}
			bundle.Load();
			if (bundle.DFDMGDDLDNB())
			{
				EBKGJFPJDJJ = bundle;
			}
		}
	}

	public static void Reset()
	{
		if (EBKGJFPJDJJ != null)
		{
			EBKGJFPJDJJ.BPEDLFOKKNN();
			EBKGJFPJDJJ = null;
		}
		DHNMJFPPGJL.Clear();
	}
}
