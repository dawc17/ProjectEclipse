using System.Collections;
using Nekki.SF2.Core.Fights.Controller;
using Nekki.SF2.GUI.Menu;
using UnityEngine;

public class QuestActionStoryTutorialDoubleSweep : QuestAction
{
	private IEnumerator _WaitTimeCoroutine;

	private bool _LastAnimationIsDoubleSweep;

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		MainMenu.get_Instance().SetEnabled(false);
		Fight gDBOMJODDEA = Fight.OHNKFOHIAKG();
		Model fGCODGKLHED = gDBOMJODDEA.LNDLFINJHDB[0];
		fGCODGKLHED.AddEventListener(2, OnAnimationStart);
		Stick joystick = gDBOMJODDEA.KCJNBFLAMCC.GetJoystick();
		joystick.SetIsFlashing(true);
		SFButton buttonKick = gDBOMJODDEA.KCJNBFLAMCC.GetButtonKick();
		buttonKick.AddFlashImage("FightButtons.Kick_Highlight");
		buttonKick.FlashingImage.rectTransform.localScale = new Vector3(1.33f, 1.33f);
		buttonKick.set_IsFlashing(true);
		_WaitTimeCoroutine = IGIJPMDLDEL();
		CoroutineManager.get_Current().StartRoutine(_WaitTimeCoroutine);
	}

	private void OnAnimationStart(object data)
	{
		if (_LastAnimationIsDoubleSweep)
		{
			_LastAnimationIsDoubleSweep = false;
			LLJIIEDNJPF();
		}
		Model.EventModel oJDOHGBGPFK = (Model.EventModel)data;
		InfoAnimation pJAHIOELGGD = (InfoAnimation)oJDOHGBGPFK.Data;
		if ("DoubleSweep" == pJAHIOELGGD.Name)
		{
			_LastAnimationIsDoubleSweep = true;
		}
	}

	private IEnumerator IGIJPMDLDEL()
	{
		yield return new WaitForSeconds(GameUtils.AKPBNLKFONO.DefaultTutorialStepTimeout);
		LLJIIEDNJPF();
	}

	private void LLJIIEDNJPF()
	{
		if (_WaitTimeCoroutine != null)
		{
			CoroutineManager.get_Current().StopRoutine(_WaitTimeCoroutine);
		}
		MainMenu.get_Instance().SetEnabled(true);
		Fight gDBOMJODDEA = Fight.OHNKFOHIAKG();
		Model fGCODGKLHED = gDBOMJODDEA.LNDLFINJHDB[0];
		fGCODGKLHED.RemoveEventListener(2, OnAnimationStart);
		Stick joystick = gDBOMJODDEA.KCJNBFLAMCC.GetJoystick();
		joystick.SetIsFlashing(false);
		SFButton buttonKick = gDBOMJODDEA.KCJNBFLAMCC.GetButtonKick();
		buttonKick.set_IsFlashing(false);
		OGIJONMKABB();
	}
}
