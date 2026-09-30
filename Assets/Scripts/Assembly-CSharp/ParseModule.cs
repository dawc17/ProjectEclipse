public class ParseModule : LoadingModule
{
	public override void JLPMOKPFECK()
	{
		if (!CHIHBINEGFL)
		{
			GameUtils.InitVariables();
			bool reusedTitleContent = Eclipse.UI.TitleScreen.TryResumeGameDataPreview();
			if (!reusedTitleContent)
			{
				GameSettings.OCIPKAONMOP();
				GameLoader.BJLLJHDFMOO();
				GameLoader.POLKDKOOACO();
				ListSF.GetInstance().IIKDNMBIHCM();
			}
			PerkTree.GBPBIPFIOJH().LJHPGKAOIAE();
			GameSettings.LNNLDPLDABI();
			if (!reusedTitleContent)
			{
				GameLoader.SetSound();
				LocalizationManager.Init();
			}
			Eclipse.Modding.ModRuntime.ApplyLocaleMetadata();
			Eclipse.Modding.ModRuntime.ApplyLegacyLocalization();
			ListSF.CCDKHLAMKKO().AFAKCAMAACM();
			GameUtils.OEKOKKCILAG();
			CHIHBINEGFL = true;
		}
	}
}
