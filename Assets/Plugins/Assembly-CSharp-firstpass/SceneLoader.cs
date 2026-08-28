using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class SceneLoader : ExtentionBehaviour
{
	public const int OnSceneLoadDone = 0;

	public const int OnConfigLoadDone = 1;

	public const string Server = "http://127.0.0.1";

	private static SceneLoader EDAPJLKMFPC;

	private static readonly Dictionary<string, AssetBundle> CachedScenes = new Dictionary<string, AssetBundle>();

	private static readonly Dictionary<string, SceneConfig> CachedConfigs = new Dictionary<string, SceneConfig>();

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static string PFHJEFFEFFF;

	public const string DataFolderName = "Export";

	private static bool _inited;

	public static SceneLoader BPCBBHAKFDM
	{
		get
		{
			return get_Instance();
		}
	}

	public static string CGIMKCCFPII
	{
		get
		{
			return get_Datapath();
		}
		private set
		{
			set_Datapath(value);
		}
	}

	public static SceneLoader get_Instance()
	{
		if (!EDAPJLKMFPC)
		{
			SceneLoader sceneLoader = Object.FindObjectOfType<SceneLoader>();
			if (!sceneLoader)
			{
				EDAPJLKMFPC = new GameObject("_sceneLoader").AddComponent<SceneLoader>();
				StaticObjectsManager.AddObject(EDAPJLKMFPC.get_gameObject(), false);
			}
			else
			{
				EDAPJLKMFPC = sceneLoader;
			}
		}
		return EDAPJLKMFPC;
	}

	public static string get_Datapath()
	{
		return PFHJEFFEFFF;
	}

	private static void set_Datapath(string value)
	{
		PFHJEFFEFFF = value;
	}

	private static void Init()
	{
		string arg = ((!SystemProperties.GAAMHGCDANB()) ? Application.dataPath : Application.persistentDataPath);
		set_Datapath(string.Format("{0}/{1}", arg, "Export"));
		if (!Directory.Exists(get_Datapath()))
		{
			Directory.CreateDirectory(get_Datapath());
		}
	}

	internal void Start()
	{
		if (_inited)
		{
			Object.Destroy(get_gameObject());
			return;
		}
		Application.targetFrameRate = 60;
		_inited = true;
		Init();
		EDAPJLKMFPC = this;
		StaticObjectsManager.AddObject(EDAPJLKMFPC.get_gameObject(), false);
	}

	public static bool HasInstance()
	{
		return EDAPJLKMFPC != null;
	}

	private static string GPFPJDOHCKI()
	{
		switch (Application.platform)
		{
		case RuntimePlatform.OSXEditor:
			return "standaloneMacOSX";
		case RuntimePlatform.OSXPlayer:
			return "standaloneMacOSX";
		case RuntimePlatform.WindowsPlayer:
			return "standaloneWindows";
		case RuntimePlatform.WindowsEditor:
			return "standaloneWindows";
		case RuntimePlatform.IPhonePlayer:
			return "ios";
		case RuntimePlatform.Android:
			return "android";
		default:
			return "standaloneWindows";
		}
	}

	public static void GetScene(string MHOCFOODLLL)
	{
		if (CachedScenes.ContainsKey(MHOCFOODLLL))
		{
			get_Instance().callEvent(0, Object.Instantiate(CachedScenes[MHOCFOODLLL].mainAsset));
		}
		else if (File.Exists(string.Format("{0}/{1}/SceneRoot_{2}.ab", get_Datapath(), GPFPJDOHCKI(), MHOCFOODLLL)))
		{
			get_Instance().StartCoroutine(NGOOKDFGNOO(MHOCFOODLLL));
		}
		else
		{
			get_Instance().StartCoroutine(FAGFELJKCDF(MHOCFOODLLL));
		}
		if (CachedConfigs.ContainsKey(MHOCFOODLLL))
		{
			get_Instance().callEvent(1, CachedConfigs[MHOCFOODLLL]);
		}
		else if (File.Exists(string.Format("{0}/{1}/SceneRoot_{2}_config.ab", get_Datapath(), GPFPJDOHCKI(), MHOCFOODLLL)))
		{
			get_Instance().StartCoroutine(JFHLECHFDBI(MHOCFOODLLL));
		}
		else
		{
			get_Instance().StartCoroutine(BHELPLHFJOE(MHOCFOODLLL));
		}
	}

	private static IEnumerator JFHLECHFDBI(string MHOCFOODLLL)
	{
		string text = string.Format("file:///{0}/{1}/SceneRoot_{2}_config.ab", get_Datapath().Replace("\\", "/"), GPFPJDOHCKI(), MHOCFOODLLL);
		UnityEngine.Debug.Log(text);
		WWW wWW = new WWW(text);
		yield return wWW;
		if (string.IsNullOrEmpty(wWW.error))
		{
			if (!CachedConfigs.ContainsKey(MHOCFOODLLL))
			{
				CachedConfigs.Add(MHOCFOODLLL, ((GameObject)wWW.assetBundle.mainAsset).GetComponent<SceneConfig>());
			}
			else
			{
				CachedConfigs[MHOCFOODLLL] = ((GameObject)wWW.assetBundle.mainAsset).GetComponent<SceneConfig>();
			}
			get_Instance().callEvent(1, CachedConfigs[MHOCFOODLLL]);
		}
		else
		{
			get_Instance().CCOFFJPPAKC(string.Format("cant load scene config {0}: {1}", MHOCFOODLLL, wWW.error));
		}
	}

	private static IEnumerator NGOOKDFGNOO(string MHOCFOODLLL)
	{
		string text = string.Format("file:///{0}/{1}/SceneRoot_{2}.ab", get_Datapath().Replace("\\", "/"), GPFPJDOHCKI(), MHOCFOODLLL);
		UnityEngine.Debug.Log(text);
		WWW wWW = new WWW(text);
		yield return wWW;
		if (string.IsNullOrEmpty(wWW.error))
		{
			if (!CachedScenes.ContainsKey(MHOCFOODLLL))
			{
				CachedScenes.Add(MHOCFOODLLL, wWW.assetBundle);
			}
			else
			{
				CachedScenes[MHOCFOODLLL] = wWW.assetBundle;
			}
			get_Instance().callEvent(0, Object.Instantiate(CachedScenes[MHOCFOODLLL].mainAsset));
		}
		else
		{
			get_Instance().CCOFFJPPAKC(string.Format("cant load scene {0}: {1}", MHOCFOODLLL, wWW.error));
		}
	}

	private static IEnumerator FAGFELJKCDF(string MHOCFOODLLL)
	{
		GameObject gameObject = GlobalLoad.ACMMJDJDNDP(string.Format("Export/SceneRoot_{0}", MHOCFOODLLL));
		if ((bool)gameObject)
		{
			get_Instance().callEvent(0, Object.Instantiate(gameObject));
		}
		else
		{
			get_Instance().CCOFFJPPAKC(string.Format("cant load local scene {0}", MHOCFOODLLL));
		}
		yield break;
	}

	private static IEnumerator BHELPLHFJOE(string MHOCFOODLLL)
	{
		GameObject gameObject = GlobalLoad.ACMMJDJDNDP(string.Format("Export/SceneRoot_{0}_config", MHOCFOODLLL));
		if ((bool)gameObject)
		{
			get_Instance().callEvent(0, Object.Instantiate(gameObject));
		}
		else
		{
			get_Instance().LOPHFKMOPAA(string.Format("cant load local scene config {0}", MHOCFOODLLL));
		}
		yield break;
	}
}
