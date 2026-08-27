using System;
using UnityEngine;

public static class GUIHelper
{
	private static GUIStyle PINOMKNHNLE;

	private static GUIStyle NEDJJLIEPBA;

	public static Rect ClientArea;

	private static void PAINOJOIGMC()
	{
		if (PINOMKNHNLE == null)
		{
			PINOMKNHNLE = new GUIStyle(GUI.skin.label);
			PINOMKNHNLE.alignment = TextAnchor.MiddleCenter;
			NEDJJLIEPBA = new GUIStyle(GUI.skin.label);
			NEDJJLIEPBA.alignment = TextAnchor.MiddleRight;
		}
	}

	public static void ECMOBPFHNPN(Rect FKAFENMANAB, bool CDKGMDNIECB, Action IBODMPMJELJ)
	{
		PAINOJOIGMC();
		GUI.Box(FKAFENMANAB, string.Empty);
		GUILayout.BeginArea(FKAFENMANAB);
		if (CDKGMDNIECB)
		{
			GECFPNNDHHJ(SampleSelector.SelectedSample.IFBOMKBDANN());
			GUILayout.Space(5f);
		}
		if (IBODMPMJELJ != null)
		{
			IBODMPMJELJ();
		}
		GUILayout.EndArea();
	}

	public static void GECFPNNDHHJ(string CKEHOEGLMBM)
	{
		PAINOJOIGMC();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label(CKEHOEGLMBM, PINOMKNHNLE);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	public static void IDPAKMFLODB(string KGBGENDIMBC, string value)
	{
		PAINOJOIGMC();
		GUILayout.BeginHorizontal();
		GUILayout.Label(KGBGENDIMBC);
		GUILayout.FlexibleSpace();
		GUILayout.Label(value, NEDJJLIEPBA);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}
}
