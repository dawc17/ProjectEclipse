using System;
using System.Collections.Generic;

public class QuestActionsSequence : global::EventDispatcher<object>
{
	public enum EBMFHMPKBCI
	{
		onRun = 0,
		onComplete = 1
	}

	private Action<object> JEDPEBLEGDM;

	public int JJIHOMLLAOL;

	public QuestParameters GFIHPBCEEOB;

	public List<QuestAction> AFENHJFICNN;

	public QuestActionsSequence()
	{
		JEDPEBLEGDM = OnActionComplete;
		JJIHOMLLAOL = 0;
		GFIHPBCEEOB = null;
		AFENHJFICNN = new List<QuestAction>();
	}

	public void NLJLHHNPCAO(QuestAction IBODMPMJELJ)
	{
		AFENHJFICNN.Add(IBODMPMJELJ);
	}

	public void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		CallEvent(0, GFIHPBCEEOB);
		this.GFIHPBCEEOB = GFIHPBCEEOB;
		int count = AFENHJFICNN.Count;
		if (count > 0)
		{
			if (JJIHOMLLAOL < count)
			{
				QuestAction mBAAKHELFKL = AFENHJFICNN[JJIHOMLLAOL];
				mBAAKHELFKL.AddEventListener(1, JEDPEBLEGDM);
				mBAAKHELFKL.DEJMHFMLKIC(GFIHPBCEEOB);
			}
		}
		else
		{
			CallEvent(1, GFIHPBCEEOB);
		}
	}

	public void OnActionComplete(object data)
	{
		if (data != null)
		{
			GFIHPBCEEOB = (QuestParameters)data;
		}
		QuestAction mBAAKHELFKL = AFENHJFICNN[JJIHOMLLAOL];
		mBAAKHELFKL.RemoveEventListener(1, JEDPEBLEGDM);
		JJIHOMLLAOL++;
		if (JJIHOMLLAOL < AFENHJFICNN.Count)
		{
			DEJMHFMLKIC(GFIHPBCEEOB);
		}
		else
		{
			CallEvent(1, GFIHPBCEEOB);
		}
	}

	public void FHPKJMMLIEG()
	{
		JJIHOMLLAOL = 0;
		foreach (QuestAction item in AFENHJFICNN)
		{
			item.GKFMJKAAJCA();
		}
	}
}
