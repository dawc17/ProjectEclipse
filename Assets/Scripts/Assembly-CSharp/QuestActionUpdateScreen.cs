public class QuestActionUpdateScreen : QuestAction
{
	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		Module module = Module.ELEBLBJKDBI();
		ScreenType currentScreen = module.NMCNDOPKFJD();
		// UpdateScreen can be resumed from the save before the first real module
		// has been selected. ModuleNone is a sentinel (enum value 8), not a scene
		// build index; attempting to reload it strands the Loader scene.
		if (currentScreen == ScreenType.ModuleNone)
		{
			UnityEngine.Debug.LogWarning("[Quest] Ignoring UpdateScreen before a screen is initialized.");
			OGIJONMKABB();
			return;
		}
		module.AddEventListener(1, DOHEMBEEHBB);
		if (!Module.DLOKJOHNDID(currentScreen))
		{
			module.RemoveEventListener(1, DOHEMBEEHBB);
			OGIJONMKABB();
		}
	}

	private void DOHEMBEEHBB(object data)
	{
		Module.ELEBLBJKDBI().RemoveEventListener(1, DOHEMBEEHBB);
		OGIJONMKABB();
	}
}
