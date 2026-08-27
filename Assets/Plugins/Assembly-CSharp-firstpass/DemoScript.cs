using System;
using UnityEngine;

public class DemoScript : MonoBehaviour
{
	private void OEEIEAMDKIG()
	{
		GamePad.JAHEECFCLHN(GamePad.PFENLAPGKFM.A, GamePad.GGAKHLLMPMM.One);
		GamePad.CNNMBBLLGNE(GamePad.LCNPGEANNDP.LeftStick, GamePad.GGAKHLLMPMM.One);
		GamePad.MAJINGINCHM(GamePad.HKKPDLMCPIF.RightTrigger, GamePad.GGAKHLLMPMM.One);
		GamepadState iOIGCCPIJPN = GamePad.GetState(GamePad.GGAKHLLMPMM.One);
		MonoBehaviour.print("A: " + iOIGCCPIJPN.IEKADOOKFKG);
	}

	private void OnGUI()
	{
		GUILayout.BeginArea(new Rect(0f, 20f, Screen.width, Screen.height));
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		AKMACMCIKGK();
		for (int i = 0; i < 5; i++)
		{
			LCBBJNJDHOM((GamePad.GGAKHLLMPMM)i);
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	private void LCBBJNJDHOM(GamePad.GGAKHLLMPMM OJINMMFLEEB)
	{
		GUILayout.Space(45f);
		GUILayout.BeginVertical();
		GamepadState iOIGCCPIJPN = GamePad.GetState(OJINMMFLEEB);
		GUILayout.Label("Gamepad " + OJINMMFLEEB);
		GUILayout.Label(string.Empty + iOIGCCPIJPN.IEKADOOKFKG);
		GUILayout.Label(string.Empty + iOIGCCPIJPN.LDKCOIHONPG);
		GUILayout.Label(string.Empty + iOIGCCPIJPN.NPKMJMCLDAH);
		GUILayout.Label(string.Empty + iOIGCCPIJPN.IHAHIEHHNCG);
		GUILayout.Label(string.Empty + iOIGCCPIJPN.Start);
		GUILayout.Label(string.Empty + iOIGCCPIJPN.AJLBHIHFFCE);
		GUILayout.Label(string.Empty + iOIGCCPIJPN.GGMOMECKAGP);
		GUILayout.Label(string.Empty + iOIGCCPIJPN.CLIBGHJKICF);
		GUILayout.Label(string.Empty + iOIGCCPIJPN.EDCHBILGFLD);
		GUILayout.Label(string.Empty + iOIGCCPIJPN.NNCHJCLKHHA);
		GUILayout.Label(string.Empty + iOIGCCPIJPN.FJBHJIFKOMF);
		GUILayout.Label(string.Empty + iOIGCCPIJPN.HHMEIEKKDAL);
		GUILayout.Label(string.Empty + iOIGCCPIJPN.ELAPGGICPLB);
		GUILayout.Label(string.Empty + iOIGCCPIJPN.KDPBFODDKOJ);
		GUILayout.Label(string.Empty);
		GUILayout.Label(string.Empty + Math.Round(iOIGCCPIJPN.CHJIELPPCOE, 2));
		GUILayout.Label(string.Empty + Math.Round(iOIGCCPIJPN.ALEANDMIOJO, 2));
		GUILayout.Label(string.Empty);
		GUILayout.Label(string.Empty + iOIGCCPIJPN.HNPGBMGKGEB);
		GUILayout.Label(string.Empty + iOIGCCPIJPN.IMMFMNIFNEH);
		GUILayout.Label(string.Empty + iOIGCCPIJPN.PGHJPABHPLP);
		GUILayout.EndVertical();
	}

	private void AKMACMCIKGK()
	{
		GUILayout.BeginVertical();
		GUILayout.Label(" ", GUILayout.Width(80f));
		GUILayout.Label("A");
		GUILayout.Label("B");
		GUILayout.Label("X");
		GUILayout.Label("Y");
		GUILayout.Label("Start");
		GUILayout.Label("Back");
		GUILayout.Label("Left Shoulder");
		GUILayout.Label("Right Shoulder");
		GUILayout.Label("Left");
		GUILayout.Label("Right");
		GUILayout.Label("Up");
		GUILayout.Label("Down");
		GUILayout.Label("LeftStick");
		GUILayout.Label("RightStick");
		GUILayout.Label(string.Empty);
		GUILayout.Label("LeftTrigger");
		GUILayout.Label("RightTrigger");
		GUILayout.Label(string.Empty);
		GUILayout.Label("LeftStickAxis");
		GUILayout.Label("rightStickAxis");
		GUILayout.Label("dPadAxis");
		GUILayout.EndVertical();
	}
}
