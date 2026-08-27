using System;
using Newtonsoft.Json;
using UnityEngine;

public class GlobalLoad : GlobalPath
{
	private static Sprite HMOOEGJGICC;

	public static Sprite HKDNGEKDOJB
	{
		get
		{
			return IHDKNNHOPFJ();
		}
	}

	public static Texture2D HKNGNEJNAJH
	{
		get
		{
			return CILCHLHBKGM();
		}
	}

	public static Sprite IHDKNNHOPFJ()
	{
		if (HMOOEGJGICC == null)
		{
			HMOOEGJGICC = PKCAELCFNDB<Sprite>(InternalSettings.NoImageTexture);
		}
		return HMOOEGJGICC;
	}

	public static Texture2D CILCHLHBKGM()
	{
		return IHDKNNHOPFJ().texture;
	}

	public static GameObject OCNHMFJIDNP(string CKANLCOICIL, string CBKHNNNCPLO = "")
	{
		GameObject dONFADGOEDE = DLHMMOPANAI(CKANLCOICIL, CBKHNNNCPLO);
		return GetGameObjectInstance(dONFADGOEDE);
	}

	public static GameObject PDIMNBOPLAB(string path)
	{
		GameObject dONFADGOEDE = ACMMJDJDNDP(path);
		return GetGameObjectInstance(dONFADGOEDE);
	}

	private static GameObject GetGameObjectInstance(GameObject DONFADGOEDE)
	{
		if (DONFADGOEDE != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(DONFADGOEDE);
			gameObject.name = gameObject.name.Replace("(Clone)", string.Empty);
			return gameObject;
		}
		return null;
	}

	public static GameObject DLHMMOPANAI(string CKANLCOICIL, string CBKHNNNCPLO = "")
	{
		return GFOGMCFOFLI<GameObject>(CKANLCOICIL, CBKHNNNCPLO);
	}

	public static GameObject ACMMJDJDNDP(string path)
	{
		return PKCAELCFNDB<GameObject>(path);
	}

	public static AudioClip GetLoadAudioClipInternal(string CKANLCOICIL, string CBKHNNNCPLO = "")
	{
		return GFOGMCFOFLI<AudioClip>(CKANLCOICIL, CBKHNNNCPLO);
	}

	public static AudioClip GetLoadAudioClip(string path)
	{
		return PKCAELCFNDB<AudioClip>(path);
	}

	public static Texture2D GetLoadTexture2DInternal(string CKANLCOICIL, string CBKHNNNCPLO = "")
	{
		Texture2D aOMLCBHAJJH = GFOGMCFOFLI<Texture2D>(CKANLCOICIL, CBKHNNNCPLO);
		return ObjecOrDefault(aOMLCBHAJJH, CILCHLHBKGM());
	}

	public static Texture2D GetLoadTexture2D(string path)
	{
		Texture2D aOMLCBHAJJH = PKCAELCFNDB<Texture2D>(path);
		return ObjecOrDefault(aOMLCBHAJJH, CILCHLHBKGM());
	}

	public static Sprite FIEMAKFOLDL(string CKANLCOICIL, string CBKHNNNCPLO = "")
	{
		Sprite aOMLCBHAJJH = GFOGMCFOFLI<Sprite>(CKANLCOICIL, CBKHNNNCPLO);
		return ObjecOrDefault(aOMLCBHAJJH, IHDKNNHOPFJ());
	}

	public static Sprite ODMFGCDIPMN(string path)
	{
		Sprite aOMLCBHAJJH = PKCAELCFNDB<Sprite>(path);
		return ObjecOrDefault(aOMLCBHAJJH, IHDKNNHOPFJ());
	}

	public static Sprite DKFODAECPGP(string CKANLCOICIL, string CBKHNNNCPLO = "")
	{
		Texture2D dAELKEKILOB = GetLoadTexture2DInternal(CKANLCOICIL, CBKHNNNCPLO);
		return TexturesUtils.CreateSprite(dAELKEKILOB);
	}

	public static Sprite OKNFNHMIENJ(string path)
	{
		Texture2D dAELKEKILOB = GetLoadTexture2D(path);
		return TexturesUtils.CreateSprite(dAELKEKILOB);
	}

	public static Sprite GetLoadSpriteFromAtlas(string NJKCBALJDMM, string KIKMPCLOBCK, string JGIGOMLGLPN)
	{
		return TexturesUtils.GetSpriteFromAtlas(NJKCBALJDMM, KIKMPCLOBCK, JGIGOMLGLPN);
	}

	public static Sprite GetLoadSpriteFromAtlas(string KIKMPCLOBCK, string JGIGOMLGLPN)
	{
		return TexturesUtils.GetSpriteFromAtlas(KIKMPCLOBCK, JGIGOMLGLPN);
	}

	public static byte[] GetLoadBytesInternal(string CKANLCOICIL, string CBKHNNNCPLO = "")
	{
		TextAsset textAsset = GFOGMCFOFLI<TextAsset>(CKANLCOICIL, CBKHNNNCPLO);
		return (!(textAsset == null)) ? textAsset.bytes : null;
	}

	public static byte[] GetLoadBytes(string path)
	{
		TextAsset textAsset = PKCAELCFNDB<TextAsset>(path);
		return (!(textAsset == null)) ? textAsset.bytes : null;
	}

	public static string GetLoadTextInternal(string CKANLCOICIL, string CBKHNNNCPLO = "")
	{
		TextAsset textAsset = GFOGMCFOFLI<TextAsset>(CKANLCOICIL, CBKHNNNCPLO);
		return (!(textAsset == null)) ? textAsset.text : null;
	}

	public static string GetLoadText(string path)
	{
		TextAsset textAsset = PKCAELCFNDB<TextAsset>(path);
		return (!(textAsset == null)) ? textAsset.text : null;
	}

	public static T DEMEAOOLCMJ<T>(string CKANLCOICIL, string CBKHNNNCPLO = "") where T : class
	{
		string dMNBDBJNKME = GetLoadTextInternal(CKANLCOICIL, CBKHNNNCPLO);
		return ACHBAJOFGHJ<T>(dMNBDBJNKME);
	}

	public static T AGJDOMMPOAD<T>(string path) where T : class
	{
		string dMNBDBJNKME = GetLoadText(path);
		return ACHBAJOFGHJ<T>(dMNBDBJNKME);
	}

	private static T ACHBAJOFGHJ<T>(string DMNBDBJNKME) where T : class
	{
		if (!DMNBDBJNKME.BKOIKMEEHDK())
		{
			try
			{
				return JsonConvert.DeserializeObject<T>(DMNBDBJNKME);
			}
			catch (Exception ex)
			{
				Debug.LogError("Error GetLoadJson [" + ex.Message + "]");
			}
		}
		return (T)null;
	}

	private static T ObjecOrDefault<T>(T AOMLCBHAJJH, T LKJLDJGIAOJ = null) where T : UnityEngine.Object
	{
		return AOMLCBHAJJH ?? LKJLDJGIAOJ;
	}

	public static T[] GetLoadObjectsInternal<T>(string CKANLCOICIL, string CBKHNNNCPLO = "") where T : UnityEngine.Object
	{
		return BNCMBJOICHI<T>(GlobalPath.GetInternalPath(CKANLCOICIL, CBKHNNNCPLO));
	}

	public static T[] DKIOHNLLACG<T>(string MFBENNFFKNC) where T : UnityEngine.Object
	{
		return BNCMBJOICHI<T>(GlobalPath.FLBGANCBDBB(MFBENNFFKNC));
	}

	public static T GFOGMCFOFLI<T>(string CKANLCOICIL, string CBKHNNNCPLO = "") where T : UnityEngine.Object
	{
		return Load<T>(GlobalPath.GetInternalPath(CKANLCOICIL, CBKHNNNCPLO));
	}

	public static T PKCAELCFNDB<T>(string MFBENNFFKNC) where T : UnityEngine.Object
	{
		return Load<T>(GlobalPath.FLBGANCBDBB(MFBENNFFKNC));
	}

	private static TResult LMJBJGKCCJC<TResult, Arg>(Arg EHCLMBADLKH, params Func<Arg, TResult>[] EFAICNOJJIP)
	{
		foreach (Func<Arg, TResult> func in EFAICNOJJIP)
		{
			TResult val = func(EHCLMBADLKH);
			if (val != null)
			{
				return val;
			}
		}
		return default(TResult);
	}

	private static T[] BNCMBJOICHI<T>(string path) where T : UnityEngine.Object
	{
		T[] array = LMJBJGKCCJC<T[], string>(path, MCGAJMCEEJA<T>, GetResources<T>);
		if (array == null)
		{
			Debug.LogError("LoadObject Not Found - " + path);
		}
		return array;
	}

	private static T Load<T>(string path) where T : UnityEngine.Object
	{
		T val = LMJBJGKCCJC<T, string>(path, LKEBDJCFPFK<T>, GetResource<T>);
		if (val == null)
		{
			Debug.LogError("LoadObject Not Found - " + path);
		}
		return val;
	}

	private static T[] GetResources<T>(string path) where T : UnityEngine.Object
	{
		return ResourcesUtil.GetResources<T>(path);
	}

	private static T GetResource<T>(string path) where T : UnityEngine.Object
	{
		return ResourcesUtil.GetResource<T>(path);
	}

	private static T[] MCGAJMCEEJA<T>(string path) where T : UnityEngine.Object
	{
		return BundlesUtil.GetObjects<T>(path);
	}

	private static T LKEBDJCFPFK<T>(string path) where T : UnityEngine.Object
	{
		return BundlesUtil.GetObject<T>(path);
	}

	public static void BPEDLFOKKNN(UnityEngine.Object AOMLCBHAJJH, bool OJCKACIMFEJ = true)
	{
		if (!(AOMLCBHAJJH == null))
		{
			if (AOMLCBHAJJH.GetInstanceID() <= 0)
			{
				CHILAIJNEHG(AOMLCBHAJJH, OJCKACIMFEJ);
			}
			else
			{
				ResourcesUtil.UnloadAsset(AOMLCBHAJJH, OJCKACIMFEJ);
			}
		}
	}

	public static void CHILAIJNEHG(UnityEngine.Object AOMLCBHAJJH, bool OJCKACIMFEJ = true)
	{
		try
		{
			if (!OJCKACIMFEJ)
			{
				UnityEngine.Object.Destroy(AOMLCBHAJJH);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(AOMLCBHAJJH);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	public static void KGNLHIKNDLL()
	{
		BundlesUtil.KGNLHIKNDLL();
		ResourcesUtil.KGNLHIKNDLL();
	}

	public static void NHKOKHGGDKH()
	{
		GC.Collect();
		GC.WaitForPendingFinalizers();
	}

	public static string GetFileOrResourcesText(string path, string IDGLPJGEFKB, string name = "")
	{
		string text = HCEPBIAOJKG.AOLDPEFEBEK(path);
		if (!text.BKOIKMEEHDK())
		{
			return text;
		}
		return GetLoadTextInternal(IDGLPJGEFKB, name);
	}
}
