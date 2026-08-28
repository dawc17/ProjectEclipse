using Nekki.SF2.Core;

public class PreInitializationModule : LoadingModule
{
	private static bool NDHHFHHBFEC;

	public override void JLPMOKPFECK()
	{
		if (!CHIHBINEGFL)
		{
			Init();
			CHIHBINEGFL = true;
		}
	}

	private static void Init()
	{
		if (!NDHHFHHBFEC)
		{
			NDHHFHHBFEC = true;
			CUDLRConsole.Init();
			ApplicationController.Init();
			RaidCheatManager.Init();
			SF2Paths.Init();
		}
	}
}
