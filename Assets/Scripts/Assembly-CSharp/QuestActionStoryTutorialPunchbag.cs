using System.Collections;
using Nekki.SF2.GUI.Menu;
using UnityEngine;

public class QuestActionStoryTutorialPunchbag : QuestAction
{
	private int FBNIMJAEJNH;

	private int KAIPMDJFBPN = 3;

	private IEnumerator _WaitTimeCoroutine;

	private bool _LastAnimationIsKick;

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		MainMenu.get_Instance().SetEnabled(false);
		Fight gDBOMJODDEA = Fight.OHNKFOHIAKG();
		SFButton buttonPunch = gDBOMJODDEA.KCJNBFLAMCC.GetButtonPunch();
		buttonPunch.AddFlashImage("FightButtons.Kick_Highlight");
		buttonPunch.FlashingImage.rectTransform.localScale = new Vector3(1.33f, 1.33f);
		buttonPunch.set_IsFlashing(true);
		SFButton buttonKick = gDBOMJODDEA.KCJNBFLAMCC.GetButtonKick();
		buttonKick.AddFlashImage("FightButtons.Kick_Highlight");
		buttonKick.FlashingImage.rectTransform.localScale = new Vector3(1.33f, 1.33f);
		buttonKick.set_IsFlashing(true);
		Model fGCODGKLHED = gDBOMJODDEA.LNDLFINJHDB[0];
		fGCODGKLHED.AddEventListener(2, OnAnimationStart);
		_WaitTimeCoroutine = IGIJPMDLDEL();
		CoroutineManager.get_Current().StartRoutine(_WaitTimeCoroutine);
	}

	private void OnAnimationStart(object data)
	{
		if (_LastAnimationIsKick)
		{
			_LastAnimationIsKick = false;
			FBNIMJAEJNH++;
			if (FBNIMJAEJNH >= KAIPMDJFBPN)
			{
				DPAAINCBKBF();
			}
		}
		Fight gDBOMJODDEA = Fight.OHNKFOHIAKG();
		Model fGCODGKLHED = gDBOMJODDEA.LNDLFINJHDB[0];
		InfoAnimation.MGHNBEPCKIF dFLPNNBIFFN = fGCODGKLHED.DFLPNNBIFFN;
		if (dFLPNNBIFFN == InfoAnimation.MGHNBEPCKIF.AnimationAttack)
		{
			_LastAnimationIsKick = true;
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
		SFButton buttonPunch = gDBOMJODDEA.KCJNBFLAMCC.GetButtonPunch();
		buttonPunch.set_IsFlashing(false);
		SFButton buttonKick = gDBOMJODDEA.KCJNBFLAMCC.GetButtonKick();
		buttonKick.set_IsFlashing(false);
		Model fGCODGKLHED = gDBOMJODDEA.LNDLFINJHDB[0];
		fGCODGKLHED.RemoveEventListener(2, OnAnimationStart);
		OGIJONMKABB();
	}
}
