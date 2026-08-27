using System.Diagnostics;
using Nekki.SF2.GUI.Scenes;
using UnityEngine.SceneManagement;

public static class SceneManagerSF
{
	private static ScreenType MHOCFOODLLL;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static bool BPBEHJPPEOO;

	public static ScreenType DAINBPONGAB
	{
		get
		{
			return EKFBDMBCDMB();
		}
		set
		{
			DJKMOGJMHLO(value);
		}
	}

	public static bool IOGMEGLBNIJ
	{
		get
		{
			return IANJCHNLMHC();
		}
		private set
		{
			set_IsInitialized(value);
		}
	}

	public static ScreenType EKFBDMBCDMB()
	{
		return MHOCFOODLLL;
	}

	public static void DJKMOGJMHLO(ScreenType value)
	{
		MHOCFOODLLL = value;
	}

	public static bool IANJCHNLMHC()
	{
		return BPBEHJPPEOO;
	}

	private static void set_IsInitialized(bool value)
	{
		BPBEHJPPEOO = value;
	}

	public static bool Init(ScreenType DAINBPONGAB)
	{
		if (!IANJCHNLMHC())
		{
			set_IsInitialized(true);
			if (DAINBPONGAB != ScreenType.ModulePreloader)
			{
				Reset();
				return false;
			}
		}
		return true;
	}

	public static void Reset()
	{
		GameLoaderScene.Stop();
		Load(ScreenType.ModulePreloader);
	}

	public static void Load(ScreenType MHOCFOODLLL)
	{
		if (MHOCFOODLLL != ScreenType.Loader)
		{
			LoaderScene.set_PrevScene(EKFBDMBCDMB());
			LoaderScene.set_NextScene(MHOCFOODLLL);
		}
		SceneManager.LoadSceneAsync(1);
	}

	public static Scene GAFDMIPPIAL()
	{
		return SceneManager.GetActiveScene();
	}
}
