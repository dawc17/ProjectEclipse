using System.Collections.Generic;
using Nekki.SF2.Core.Tutorials;
using Nekki.SF2.GUI;

public class QuestActionStoryTutorialShowBlock : QuestAction
{
	private LabelButton showButon;

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
		List<Trick> list = GameUtils.KLLGJKHALGH();
		list.Sort((Trick KOOLDHKJHNH, Trick MHFCMOONCHB) => KOOLDHKJHNH.Rank.CompareTo(MHFCMOONCHB.Rank));
		Trick iHNIKIHKFHC = list[0];
		string mENAJEAJJBE = iHNIKIHKFHC.KJHMOGGECBN.Name;
		current.ScrollToItemByName(SliderType.SliderTricks, mENAJEAJJBE);
		LabelButton btnStrikeShow = current.GetBtnStrikeShow();
		btnStrikeShow.set_IsFlashing(true);
		btnStrikeShow.GetComponent<TutorialComponent>().IsActive = true;
		btnStrikeShow.onClick.AddListener(CDABOCGCPOH);
	}

	private void CDABOCGCPOH()
	{
		ProfileScene current = Scene<ProfileScene>.get_Current();
		current.ModelContainer.AddEventListener(0, PCOFDIIBLCB);
		LabelButton btnStrikeShow = current.GetBtnStrikeShow();
		btnStrikeShow.set_IsFlashing(false);
		btnStrikeShow.GetComponent<TutorialComponent>().IsActive = false;
		btnStrikeShow.onClick.RemoveListener(CDABOCGCPOH);
	}

	private void PCOFDIIBLCB(object data)
	{
		ProfileScene current = Scene<ProfileScene>.get_Current();
		current.ModelContainer.RemoveEventListener(0, PCOFDIIBLCB);
		OGIJONMKABB();
	}
}
