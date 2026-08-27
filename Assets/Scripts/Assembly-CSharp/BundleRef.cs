using System.IO;
using UnityEngine;

public class BundleRef
{
	private string _BundleId;

	private AssetBundle _UnityBundle;

	public string KDPHJBCIHEM
	{
		get
		{
			return NAIOABAGJCN();
		}
	}

	public string FAMAOJIIPBL
	{
		get
		{
			return ALJBPGLCKBP();
		}
	}

	public bool MOBGLOMMCJJ
	{
		get
		{
			return DFDMGDDLDNB();
		}
	}

	public BundleRef(string BKKKEENLEDP)
	{
		_BundleId = BKKKEENLEDP;
	}

	public string NAIOABAGJCN()
	{
		return SF2Paths.MEKBAHBKMNB() + "/" + _BundleId;
	}

	public string ALJBPGLCKBP()
	{
		return _BundleId;
	}

	public bool DFDMGDDLDNB()
	{
		return _UnityBundle != null;
	}

	public string[] OEEOEGEBBAL()
	{
		if (_UnityBundle != null)
		{
			return _UnityBundle.GetAllAssetNames();
		}
		return new string[0];
	}

	public void Load()
	{
		if (!(_UnityBundle != null) && File.Exists(NAIOABAGJCN()))
		{
			_UnityBundle = AssetBundle.LoadFromFile(NAIOABAGJCN());
		}
	}

	public void BPEDLFOKKNN()
	{
		if (!(_UnityBundle == null))
		{
			_UnityBundle.Unload(false);
			_UnityBundle = null;
		}
	}

	public T LoadAsset<T>(string JHEMALDDIFN) where T : Object
	{
		if (_UnityBundle == null)
		{
			return (T)null;
		}
		return _UnityBundle.LoadAsset<T>(JHEMALDDIFN);
	}

	public T[] LoadAssetWithSubAssets<T>(string JHEMALDDIFN) where T : Object
	{
		if (_UnityBundle == null)
		{
			return null;
		}
		return _UnityBundle.LoadAssetWithSubAssets<T>(JHEMALDDIFN);
	}
}
