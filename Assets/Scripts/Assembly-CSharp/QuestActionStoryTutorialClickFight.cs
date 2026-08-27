using Nekki.SF2.Core.Tutorials;
using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Map;

public class QuestActionStoryTutorialClickFight : QuestAction
{
	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		TutorialCanvas.get_Instance().set_BlockOn(true);
		MapScene current = Scene<MapScene>.get_Current();
		current.SelectFight("ZONE_1|BOSS_LYNX|1");
		LabelButton btnFight = current.GetBtnFight();
		btnFight.set_IsFlashing(true);
		btnFight.RemoveAllEventListener();
		btnFight.onClick.RemoveAllListeners();
		btnFight.onClick.AddListener(OnButtonClick);
		TutorialComponent component = btnFight.gameObject.GetComponent<TutorialComponent>();
		component.IsActive = true;
	}

	private void OnButtonClick()
	{
		TutorialCanvas.get_Instance().set_BlockOn(false);
		MapScene current = Scene<MapScene>.get_Current();
		LabelButton btnFight = current.GetBtnFight();
		btnFight.set_IsFlashing(false);
		btnFight.onClick.RemoveListener(OnButtonClick);
		TutorialComponent component = btnFight.gameObject.GetComponent<TutorialComponent>();
		component.IsActive = false;
		OGIJONMKABB();
	}
}
