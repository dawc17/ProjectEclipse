using System.Collections.Generic;
using UnityEngine;

public class FHHECMPNKHC : global::EventDispatcher<object>
{
	public enum GLOKGJPJGNH
	{
		OnKeyboardKeyPress = 0,
		OnKeyboardKeyRelease = 1
	}

	public List<List<CBBEIGACPPD.GIPHMILLKGA>> BFEBNHGFIHB = new List<List<CBBEIGACPPD.GIPHMILLKGA>>();

	public bool DCHJDPCEODD;

	public void Init()
	{
		BFEBNHGFIHB.Clear();
		BFEBNHGFIHB.CPCAJIKOIEE(2);
	}

	public void Render()
	{
		if (!DCHJDPCEODD)
		{
			return;
		}
		int i = 0;
		for (int count = BFEBNHGFIHB.Count; i < count; i++)
		{
			foreach (CBBEIGACPPD.GIPHMILLKGA item in BFEBNHGFIHB[i])
			{
				IJKEJMLLMNA(item);
			}
		}
	}

	public void NGHDGMNEPJB(KeyCode KGBGENDIMBC, FightCID index, int JAMPAODJGGL = 0)
	{
		CBBEIGACPPD.GIPHMILLKGA item = new CBBEIGACPPD.GIPHMILLKGA(KGBGENDIMBC, index, JAMPAODJGGL);
		BFEBNHGFIHB[JAMPAODJGGL].Add(item);
	}

	public void Clear()
	{
		int i = 0;
		for (int count = BFEBNHGFIHB.Count; i < count; i++)
		{
			BFEBNHGFIHB[i].Clear();
		}
		BFEBNHGFIHB.Clear();
	}

	private void IJKEJMLLMNA(CBBEIGACPPD.GIPHMILLKGA KGBGENDIMBC)
	{
		if (Input.GetKeyDown(KGBGENDIMBC.EDEEELJMHLG) || Input.GetKey(KGBGENDIMBC.EDEEELJMHLG))
		{
			if (!KGBGENDIMBC.isActive)
			{
				KGBGENDIMBC.isActive = true;
				FCKDDEIIPEN(0, KGBGENDIMBC);
			}
		}
		else if (KGBGENDIMBC.isActive)
		{
			KGBGENDIMBC.isActive = false;
			FCKDDEIIPEN(1, KGBGENDIMBC);
		}
	}

	private void FCKDDEIIPEN(int DOPHKKGNAEF, CBBEIGACPPD.GIPHMILLKGA KGBGENDIMBC)
	{
		CBBEIGACPPD cBBEIGACPPD = new CBBEIGACPPD();
		cBBEIGACPPD.Index = KGBGENDIMBC.count;
		cBBEIGACPPD.KMOPCKPBHIA = KGBGENDIMBC.Index;
		CallEvent(DOPHKKGNAEF, cBBEIGACPPD);
	}
}
