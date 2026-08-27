using Nekki.SF2.Core.Tutorials;
using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Menu;
using Nekki.SF2.GUI.Shop;
using UnityEngine.UI;

public class QuestActionStoryTutorialBuyItem : QuestAction
{
	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		string cDNCPBKAHKJ = GameUtils.AKPBNLKFONO.CDNCPBKAHKJ;
		UserItem dKCHDHMLKHN = ListSF.CCDKHLAMKKO().KHCNHPCPFII().CMGOCLGHNLH(cDNCPBKAHKJ);
		if (dKCHDHMLKHN != null)
		{
			OGIJONMKABB();
		}
		TutorialCanvas.get_Instance().set_BlockOn(true);
		ShopScene current = Scene<ShopScene>.get_Current();
		current.ScrollToItemByName(ShopSection.Weapon, cDNCPBKAHKJ);
		IconLabelButton goldButton = current.GetInfoPanel().GetGoldButton();
		goldButton.set_IsFlashing(true);
		goldButton.RemoveAllEventListener();
		goldButton.onClick.RemoveAllListeners();
		goldButton.onClick.AddListener(OnButtonClick);
		TutorialComponent component = goldButton.gameObject.GetComponent<TutorialComponent>();
		component.IsActive = true;
		Button skipBtn = MainMenu.get_Instance().GetSkipBtn();
		TutorialComponent tutorialComponent = ((!(skipBtn != null)) ? null : skipBtn.gameObject.GetComponent<TutorialComponent>());
		if (tutorialComponent != null)
		{
			tutorialComponent.IsActive = true;
			skipBtn.onClick.AddListener(JPMFAFMCCLP);
		}
	}

	private void JPMFAFMCCLP()
	{
		Button skipBtn = MainMenu.get_Instance().GetSkipBtn();
		skipBtn.onClick.AddListener(JPMFAFMCCLP);
		OnButtonClick();
	}

	private void OnButtonClick()
	{
		TutorialCanvas.get_Instance().set_BlockOn(false);
		ShopScene instance = ShopScene.get_Instance();
		IconLabelButton goldButton = instance.GetInfoPanel().GetGoldButton();
		goldButton.set_IsFlashing(false);
		goldButton.onClick.RemoveListener(OnButtonClick);
		TutorialComponent component = goldButton.gameObject.GetComponent<TutorialComponent>();
		component.IsActive = false;
		string cDNCPBKAHKJ = GameUtils.AKPBNLKFONO.CDNCPBKAHKJ;
		ItemInfo dJKEECEOCJB = ListSF.DJBOFEEKJMP().KCCDBEEKBCG(cDNCPBKAHKJ);
		if (dJKEECEOCJB != null)
		{
			if (ItemBuyHelper.IHHKNBPKGHD(dJKEECEOCJB))
			{
				ListSF.CCDKHLAMKKO().KHCNHPCPFII().EEDJEDBMIMI(dJKEECEOCJB, true);
			}
			instance.GetInfoPanel().UpdateContent();
		}
		OGIJONMKABB();
	}
}
