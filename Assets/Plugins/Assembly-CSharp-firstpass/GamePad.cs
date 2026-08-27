using System;
using UnityEngine;

public static class GamePad
{
	public enum PFENLAPGKFM
	{
		A = 0,
		B = 1,
		Y = 2,
		X = 3,
		RightShoulder = 4,
		LeftShoulder = 5,
		RightStick = 6,
		LeftStick = 7,
		Back = 8,
		Start = 9
	}

	public enum HKKPDLMCPIF
	{
		LeftTrigger = 0,
		RightTrigger = 1
	}

	public enum LCNPGEANNDP
	{
		LeftStick = 0,
		RightStick = 1,
		Dpad = 2
	}

	public enum GGAKHLLMPMM
	{
		Any = 0,
		One = 1,
		Two = 2,
		Three = 3,
		Four = 4
	}

	public static bool JAHEECFCLHN(PFENLAPGKFM KLNKEPMAGKF, GGAKHLLMPMM EKFPHMLKDAP)
	{
		KeyCode key = KNBAPAJMFIN(KLNKEPMAGKF, EKFPHMLKDAP);
		return Input.GetKeyDown(key);
	}

	public static bool MGGDMBHADIP(PFENLAPGKFM KLNKEPMAGKF, GGAKHLLMPMM EKFPHMLKDAP)
	{
		KeyCode key = KNBAPAJMFIN(KLNKEPMAGKF, EKFPHMLKDAP);
		return Input.GetKeyUp(key);
	}

	public static bool NFCGBMHPKMA(PFENLAPGKFM KLNKEPMAGKF, GGAKHLLMPMM EKFPHMLKDAP)
	{
		KeyCode key = KNBAPAJMFIN(KLNKEPMAGKF, EKFPHMLKDAP);
		return Input.GetKey(key);
	}

	public static Vector2 CNNMBBLLGNE(LCNPGEANNDP NMADGDHJBGB, GGAKHLLMPMM EKFPHMLKDAP, bool IMFLNPNECCO = false)
	{
		string axisName = string.Empty;
		string axisName2 = string.Empty;
		switch (NMADGDHJBGB)
		{
		case LCNPGEANNDP.Dpad:
			axisName = "DPad_XAxis_" + (int)EKFPHMLKDAP;
			axisName2 = "DPad_YAxis_" + (int)EKFPHMLKDAP;
			break;
		case LCNPGEANNDP.LeftStick:
			axisName = "L_XAxis_" + (int)EKFPHMLKDAP;
			axisName2 = "L_YAxis_" + (int)EKFPHMLKDAP;
			break;
		case LCNPGEANNDP.RightStick:
			axisName = "R_XAxis_" + (int)EKFPHMLKDAP;
			axisName2 = "R_YAxis_" + (int)EKFPHMLKDAP;
			break;
		}
		Vector2 result = Vector3.zero;
		try
		{
			if (!IMFLNPNECCO)
			{
				result.x = Input.GetAxis(axisName);
				result.y = 0f - Input.GetAxis(axisName2);
			}
			else
			{
				result.x = Input.GetAxisRaw(axisName);
				result.y = 0f - Input.GetAxisRaw(axisName2);
			}
		}
		catch (Exception lIOGIBJBHAH)
		{
			AdvLog.CCOFFJPPAKC(lIOGIBJBHAH);
			AdvLog.LOPHFKMOPAA("Have you set up all axes correctly? \nThe easiest solution is to replace the InputManager.asset with version located in the GamepadInput package. \nWarning: do so will overwrite any existing input");
		}
		return result;
	}

	public static float MAJINGINCHM(HKKPDLMCPIF CPBHKJFPFJB, GGAKHLLMPMM EKFPHMLKDAP, bool IMFLNPNECCO = false)
	{
		string axisName = string.Empty;
		switch (CPBHKJFPFJB)
		{
		case HKKPDLMCPIF.LeftTrigger:
			axisName = "TriggersL_" + (int)EKFPHMLKDAP;
			break;
		case HKKPDLMCPIF.RightTrigger:
			axisName = "TriggersR_" + (int)EKFPHMLKDAP;
			break;
		}
		float result = 0f;
		try
		{
			result = (IMFLNPNECCO ? Input.GetAxisRaw(axisName) : Input.GetAxis(axisName));
		}
		catch (Exception lIOGIBJBHAH)
		{
			AdvLog.CCOFFJPPAKC(lIOGIBJBHAH);
			AdvLog.LOPHFKMOPAA("Have you set up all axes correctly? \nThe easiest solution is to replace the InputManager.asset with version located in the GamepadInput package. \nWarning: do so will overwrite any existing input");
		}
		return result;
	}

	private static KeyCode KNBAPAJMFIN(PFENLAPGKFM KLNKEPMAGKF, GGAKHLLMPMM EKFPHMLKDAP)
	{
		switch (EKFPHMLKDAP)
		{
		case GGAKHLLMPMM.One:
			switch (KLNKEPMAGKF)
			{
			case PFENLAPGKFM.A:
				return KeyCode.Joystick1Button0;
			case PFENLAPGKFM.B:
				return KeyCode.Joystick1Button1;
			case PFENLAPGKFM.X:
				return KeyCode.Joystick1Button2;
			case PFENLAPGKFM.Y:
				return KeyCode.Joystick1Button3;
			case PFENLAPGKFM.RightShoulder:
				return KeyCode.Joystick1Button5;
			case PFENLAPGKFM.LeftShoulder:
				return KeyCode.Joystick1Button4;
			case PFENLAPGKFM.Back:
				return KeyCode.Joystick1Button6;
			case PFENLAPGKFM.Start:
				return KeyCode.Joystick1Button7;
			case PFENLAPGKFM.LeftStick:
				return KeyCode.Joystick1Button8;
			case PFENLAPGKFM.RightStick:
				return KeyCode.Joystick1Button9;
			}
			break;
		case GGAKHLLMPMM.Two:
			switch (KLNKEPMAGKF)
			{
			case PFENLAPGKFM.A:
				return KeyCode.Joystick2Button0;
			case PFENLAPGKFM.B:
				return KeyCode.Joystick2Button1;
			case PFENLAPGKFM.X:
				return KeyCode.Joystick2Button2;
			case PFENLAPGKFM.Y:
				return KeyCode.Joystick2Button3;
			case PFENLAPGKFM.RightShoulder:
				return KeyCode.Joystick2Button5;
			case PFENLAPGKFM.LeftShoulder:
				return KeyCode.Joystick2Button4;
			case PFENLAPGKFM.Back:
				return KeyCode.Joystick2Button6;
			case PFENLAPGKFM.Start:
				return KeyCode.Joystick2Button7;
			case PFENLAPGKFM.LeftStick:
				return KeyCode.Joystick2Button8;
			case PFENLAPGKFM.RightStick:
				return KeyCode.Joystick2Button9;
			}
			break;
		case GGAKHLLMPMM.Three:
			switch (KLNKEPMAGKF)
			{
			case PFENLAPGKFM.A:
				return KeyCode.Joystick3Button0;
			case PFENLAPGKFM.B:
				return KeyCode.Joystick3Button1;
			case PFENLAPGKFM.X:
				return KeyCode.Joystick3Button2;
			case PFENLAPGKFM.Y:
				return KeyCode.Joystick3Button3;
			case PFENLAPGKFM.RightShoulder:
				return KeyCode.Joystick3Button5;
			case PFENLAPGKFM.LeftShoulder:
				return KeyCode.Joystick3Button4;
			case PFENLAPGKFM.Back:
				return KeyCode.Joystick3Button6;
			case PFENLAPGKFM.Start:
				return KeyCode.Joystick3Button7;
			case PFENLAPGKFM.LeftStick:
				return KeyCode.Joystick3Button8;
			case PFENLAPGKFM.RightStick:
				return KeyCode.Joystick3Button9;
			}
			break;
		case GGAKHLLMPMM.Four:
			switch (KLNKEPMAGKF)
			{
			case PFENLAPGKFM.A:
				return KeyCode.Joystick4Button0;
			case PFENLAPGKFM.B:
				return KeyCode.Joystick4Button1;
			case PFENLAPGKFM.X:
				return KeyCode.Joystick4Button2;
			case PFENLAPGKFM.Y:
				return KeyCode.Joystick4Button3;
			case PFENLAPGKFM.RightShoulder:
				return KeyCode.Joystick4Button5;
			case PFENLAPGKFM.LeftShoulder:
				return KeyCode.Joystick4Button4;
			case PFENLAPGKFM.Back:
				return KeyCode.Joystick4Button6;
			case PFENLAPGKFM.Start:
				return KeyCode.Joystick4Button7;
			case PFENLAPGKFM.LeftStick:
				return KeyCode.Joystick4Button8;
			case PFENLAPGKFM.RightStick:
				return KeyCode.Joystick4Button9;
			}
			break;
		case GGAKHLLMPMM.Any:
			switch (KLNKEPMAGKF)
			{
			case PFENLAPGKFM.A:
				return KeyCode.JoystickButton0;
			case PFENLAPGKFM.B:
				return KeyCode.JoystickButton1;
			case PFENLAPGKFM.X:
				return KeyCode.JoystickButton2;
			case PFENLAPGKFM.Y:
				return KeyCode.JoystickButton3;
			case PFENLAPGKFM.RightShoulder:
				return KeyCode.JoystickButton5;
			case PFENLAPGKFM.LeftShoulder:
				return KeyCode.JoystickButton4;
			case PFENLAPGKFM.Back:
				return KeyCode.JoystickButton6;
			case PFENLAPGKFM.Start:
				return KeyCode.JoystickButton7;
			case PFENLAPGKFM.LeftStick:
				return KeyCode.JoystickButton8;
			case PFENLAPGKFM.RightStick:
				return KeyCode.JoystickButton9;
			}
			break;
		}
		return KeyCode.None;
	}

	public static GamepadState GetState(GGAKHLLMPMM EKFPHMLKDAP, bool IMFLNPNECCO = false)
	{
		GamepadState iOIGCCPIJPN = new GamepadState();
		iOIGCCPIJPN.IEKADOOKFKG = NFCGBMHPKMA(PFENLAPGKFM.A, EKFPHMLKDAP);
		iOIGCCPIJPN.LDKCOIHONPG = NFCGBMHPKMA(PFENLAPGKFM.B, EKFPHMLKDAP);
		iOIGCCPIJPN.IHAHIEHHNCG = NFCGBMHPKMA(PFENLAPGKFM.Y, EKFPHMLKDAP);
		iOIGCCPIJPN.NPKMJMCLDAH = NFCGBMHPKMA(PFENLAPGKFM.X, EKFPHMLKDAP);
		iOIGCCPIJPN.CLIBGHJKICF = NFCGBMHPKMA(PFENLAPGKFM.RightShoulder, EKFPHMLKDAP);
		iOIGCCPIJPN.GGMOMECKAGP = NFCGBMHPKMA(PFENLAPGKFM.LeftShoulder, EKFPHMLKDAP);
		iOIGCCPIJPN.KDPBFODDKOJ = NFCGBMHPKMA(PFENLAPGKFM.RightStick, EKFPHMLKDAP);
		iOIGCCPIJPN.ELAPGGICPLB = NFCGBMHPKMA(PFENLAPGKFM.LeftStick, EKFPHMLKDAP);
		iOIGCCPIJPN.Start = NFCGBMHPKMA(PFENLAPGKFM.Start, EKFPHMLKDAP);
		iOIGCCPIJPN.AJLBHIHFFCE = NFCGBMHPKMA(PFENLAPGKFM.Back, EKFPHMLKDAP);
		iOIGCCPIJPN.HNPGBMGKGEB = CNNMBBLLGNE(LCNPGEANNDP.LeftStick, EKFPHMLKDAP, IMFLNPNECCO);
		iOIGCCPIJPN.IMMFMNIFNEH = CNNMBBLLGNE(LCNPGEANNDP.RightStick, EKFPHMLKDAP, IMFLNPNECCO);
		iOIGCCPIJPN.PGHJPABHPLP = CNNMBBLLGNE(LCNPGEANNDP.Dpad, EKFPHMLKDAP, IMFLNPNECCO);
		iOIGCCPIJPN.EDCHBILGFLD = iOIGCCPIJPN.PGHJPABHPLP.x < 0f;
		iOIGCCPIJPN.NNCHJCLKHHA = iOIGCCPIJPN.PGHJPABHPLP.x > 0f;
		iOIGCCPIJPN.FJBHJIFKOMF = iOIGCCPIJPN.PGHJPABHPLP.y > 0f;
		iOIGCCPIJPN.HHMEIEKKDAL = iOIGCCPIJPN.PGHJPABHPLP.y < 0f;
		iOIGCCPIJPN.CHJIELPPCOE = MAJINGINCHM(HKKPDLMCPIF.LeftTrigger, EKFPHMLKDAP, IMFLNPNECCO);
		iOIGCCPIJPN.ALEANDMIOJO = MAJINGINCHM(HKKPDLMCPIF.RightTrigger, EKFPHMLKDAP, IMFLNPNECCO);
		return iOIGCCPIJPN;
	}
}
