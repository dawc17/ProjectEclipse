using Nekki.SF2.Core.Tutorials;
using Nekki.SF2.GUI;

public class QuestActionStoryTutorialLearnPerk : QuestAction
{
	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		TutorialCanvas.get_Instance().set_BlockOn(true);
		ProfileScene current = Scene<ProfileScene>.get_Current();
		if (current == null)
		{
			OGIJONMKABB();
			return;
		}
		string oHCGEEEKEJH = "PERK_DOUBLE_SWEEP";
		current.ScrollToItemByName(SliderType.SliderPerks, oHCGEEEKEJH);
		LabelButton btnPerkImprove = current.GetBtnPerkImprove();
		btnPerkImprove.set_IsFlashing(true);
		btnPerkImprove.GetComponent<TutorialComponent>().IsActive = true;
		btnPerkImprove.onClick.AddListener(OnButtonClick);
	}

	private void OnButtonClick()
	{
		TutorialCanvas.get_Instance().set_BlockOn(false);
		ProfileScene current = Scene<ProfileScene>.get_Current();
		LabelButton btnPerkImprove = current.GetBtnPerkImprove();
		btnPerkImprove.set_IsFlashing(false);
		btnPerkImprove.GetComponent<TutorialComponent>().IsActive = false;
		btnPerkImprove.onClick.RemoveListener(OnButtonClick);
		OGIJONMKABB();
	}
}
