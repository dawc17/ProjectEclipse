using System.Collections.Generic;
using Nekki.SF2.Core.Tutorials;
using Nekki.SF2.GUI;
using UnityEngine;

public class QuestActionStoryTutorialShowBlock : QuestAction
{
	private LabelButton showButon;
	private ProfileScene profile;
	private TutorialCanvas tutorialCanvas;
	private bool running;

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		running = true;
		profile = Scene<ProfileScene>.get_Current();
		if (profile == null)
		{
			Complete();
			return;
		}
		// Use the profile's block demonstration, not whichever fight move sorts first.
		List<Trick> list = GameUtils.KLLGJKHALGH(SceneTypes.SceneProfile);
		Trick block = list.Find(trick => trick.KJHMOGGECBN != null && trick.KJHMOGGECBN.Name == "HighBlockProfile");
		if (block == null)
		{
			Debug.LogWarning("[Tutorial] Block demonstration unavailable; releasing tutorial input.");
			Complete();
			return;
		}
		profile.ScrollToItemByName(SliderType.SliderTricks, block.Name);
		showButon = profile.GetBtnStrikeShow();
		if (showButon == null || showButon.GetComponent<TutorialComponent>() == null)
		{
			Debug.LogWarning("[Tutorial] Block Show button unavailable; releasing tutorial input.");
			Complete();
			return;
		}
		profile.TrickPreviewCompleted += PCOFDIIBLCB;
		profile.ProfileClosing += OnProfileClosing;
		tutorialCanvas = TutorialCanvas.get_Instance();
		tutorialCanvas.set_BlockOn(true);
		showButon.set_IsFlashing(true);
		showButon.GetComponent<TutorialComponent>().IsActive = true;
		showButon.onClick.AddListener(CDABOCGCPOH);
	}

	private void CDABOCGCPOH()
	{
		// ProfileScene owns the temporary preview lock from here. The persistent
		// tutorial canvas must not follow the player into another scene.
		if (tutorialCanvas != null) tutorialCanvas.set_BlockOn(false);
		ClearButton();
	}

	private void PCOFDIIBLCB(object data)
	{
		Complete();
	}

	private void OnProfileClosing(object data)
	{
		if (!running) return;
		GKFMJKAAJCA();
		// Cancel this run without advancing SHOW_BLOCK or opening the next
		// tutorial on a scene being destroyed. Re-entering Profile can retry it.
		PJGEOIKPGFH();
	}

	private void Complete()
	{
		if (!running) return;
		running = false;
		Cleanup();
		OGIJONMKABB();
	}

	private void ClearButton()
	{
		if (showButon == null) return;
		showButon.set_IsFlashing(false);
		TutorialComponent component = showButon.GetComponent<TutorialComponent>();
		if (component != null) component.IsActive = false;
		showButon.onClick.RemoveListener(CDABOCGCPOH);
		showButon = null;
	}

	private void Cleanup()
	{
		ClearButton();
		if (profile != null)
		{
			profile.TrickPreviewCompleted -= PCOFDIIBLCB;
			profile.ProfileClosing -= OnProfileClosing;
		}
		profile = null;
		if (tutorialCanvas != null) tutorialCanvas.set_BlockOn(false);
		tutorialCanvas = null;
	}

	public override void GKFMJKAAJCA()
	{
		running = false;
		Cleanup();
	}
}
