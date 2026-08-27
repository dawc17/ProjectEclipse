using System.Collections;
using Nekki.SF2.Core.Fights.Controller;
using Nekki.SF2.GUI.Menu;
using UnityEngine;

public class QuestActionStoryTutorialMove : QuestAction
{
	private int CAGOGNNAONE;

	private int BABODCPGPEN = 3;

	private IEnumerator _WaitTimeCoroutine;

	private bool _LastAnimationIsMove;

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		MainMenu.get_Instance().SetEnabled(false);
		Fight gDBOMJODDEA = Fight.OHNKFOHIAKG();
		Stick joystick = gDBOMJODDEA.KCJNBFLAMCC.GetJoystick();
		joystick.SetIsFlashing(true);
		Model fGCODGKLHED = gDBOMJODDEA.LNDLFINJHDB[0];
		fGCODGKLHED.AddEventListener(2, OnAnimationStart);
		_WaitTimeCoroutine = IGIJPMDLDEL();
		CoroutineManager.get_Current().StartRoutine(_WaitTimeCoroutine);
	}

	private void OnAnimationStart(object data)
	{
		if (_LastAnimationIsMove)
		{
			_LastAnimationIsMove = false;
			CAGOGNNAONE++;
			if (CAGOGNNAONE >= BABODCPGPEN)
			{
				DPAAINCBKBF();
			}
		}
		Fight gDBOMJODDEA = Fight.OHNKFOHIAKG();
		Model fGCODGKLHED = gDBOMJODDEA.LNDLFINJHDB[0];
		InfoAnimation.MGHNBEPCKIF dFLPNNBIFFN = fGCODGKLHED.DFLPNNBIFFN;
		if (dFLPNNBIFFN == InfoAnimation.MGHNBEPCKIF.AnimationMove)
		{
			_LastAnimationIsMove = true;
		}
	}

	private IEnumerator IGIJPMDLDEL()
	{
		yield return new WaitForSeconds(GameUtils.AKPBNLKFONO.DefaultTutorialStepTimeout);
		DPAAINCBKBF();
	}

	private void DPAAINCBKBF()
	{
		if (_WaitTimeCoroutine != null)
		{
			CoroutineManager.get_Current().StopRoutine(_WaitTimeCoroutine);
		}
		MainMenu.get_Instance().SetEnabled(true);
		Fight gDBOMJODDEA = Fight.OHNKFOHIAKG();
		Stick joystick = gDBOMJODDEA.KCJNBFLAMCC.GetJoystick();
		joystick.SetIsFlashing(false);
		Model fGCODGKLHED = gDBOMJODDEA.LNDLFINJHDB[0];
		fGCODGKLHED.RemoveEventListener(2, OnAnimationStart);
		OGIJONMKABB();
	}
}
