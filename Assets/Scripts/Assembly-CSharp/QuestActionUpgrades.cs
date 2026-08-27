using Nekki.SF2.GUI.Shop;

public class QuestActionUpgrades : QuestAction
{
	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		nKGLHEGIKKP.MJLDOEOMLEG(true);
		ShopScene instance = ShopScene.get_Instance();
		if (instance != null)
		{
			instance.UpdateScene(null);
		}
		OGIJONMKABB();
	}
}
