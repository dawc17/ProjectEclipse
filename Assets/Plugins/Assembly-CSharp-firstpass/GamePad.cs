using System;
using UnityEngine;

public static class GamePad
{
	public enum Button // best guess for name
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

	public enum Trigger // best guess for name
	{
		LeftTrigger = 0,
		RightTrigger = 1
	}

	public enum Stick // best guess for name
	{
		LeftStick = 0,
		RightStick = 1,
		Dpad = 2
	}

	public enum Player // best guess for name
	{
		Any = 0,
		One = 1,
		Two = 2,
		Three = 3,
		Four = 4
	}

	public static bool GetButtonDown(Button KLNKEPMAGKF, Player EKFPHMLKDAP) // best guess for name
	{
		KeyCode key = KNBAPAJMFIN(KLNKEPMAGKF, EKFPHMLKDAP);
		return Input.GetKeyDown(key);
	}

	public static bool GetButtonUp(Button KLNKEPMAGKF, Player EKFPHMLKDAP) // best guess for name
	{
		KeyCode key = KNBAPAJMFIN(KLNKEPMAGKF, EKFPHMLKDAP);
		return Input.GetKeyUp(key);
	}

	public static bool GetButton(Button KLNKEPMAGKF, Player EKFPHMLKDAP) // best guess for name
	{
		KeyCode key = KNBAPAJMFIN(KLNKEPMAGKF, EKFPHMLKDAP);
		return Input.GetKey(key);
	}

	public static Vector2 GetStick(Stick NMADGDHJBGB, Player EKFPHMLKDAP, bool IMFLNPNECCO = false) // best guess for name
	{
		// Only numbered axes are configured. Pick the strongest connected pad
		// for menus instead of looking up nonexistent *_0 axes.
		if (EKFPHMLKDAP == Player.Any)
		{
			Vector2 strongest = Vector2.zero;
			string[] devices = Input.GetJoystickNames();
			for (int i = 0; i < devices.Length && i < 2; i++)
			{
				if (string.IsNullOrEmpty(devices[i])) continue;
				Vector2 stick = GetStick(NMADGDHJBGB, (Player)(i + 1), IMFLNPNECCO);
				if (stick.sqrMagnitude > strongest.sqrMagnitude) strongest = stick;
			}
			return strongest;
		}
		string axisName = string.Empty;
		string axisName2 = string.Empty;
		switch (NMADGDHJBGB)
		{
		case Stick.Dpad:
			axisName = "DPad_XAxis_" + (int)EKFPHMLKDAP;
			axisName2 = "DPad_YAxis_" + (int)EKFPHMLKDAP;
			break;
		case Stick.LeftStick:
			axisName = "L_XAxis_" + (int)EKFPHMLKDAP;
			axisName2 = "L_YAxis_" + (int)EKFPHMLKDAP;
			break;
		case Stick.RightStick:
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

	public static float GetTrigger(Trigger CPBHKJFPFJB, Player EKFPHMLKDAP, bool IMFLNPNECCO = false) // best guess for name
	{
		string axisName = string.Empty;
		switch (CPBHKJFPFJB)
		{
		case Trigger.LeftTrigger:
			axisName = "TriggersL_" + (int)EKFPHMLKDAP;
			break;
		case Trigger.RightTrigger:
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

	private static KeyCode KNBAPAJMFIN(Button KLNKEPMAGKF, Player EKFPHMLKDAP)
	{
		switch (EKFPHMLKDAP)
		{
		case Player.One:
			switch (KLNKEPMAGKF)
			{
			case Button.A:
				return KeyCode.Joystick1Button0;
			case Button.B:
				return KeyCode.Joystick1Button1;
			case Button.X:
				return KeyCode.Joystick1Button2;
			case Button.Y:
				return KeyCode.Joystick1Button3;
			case Button.RightShoulder:
				return KeyCode.Joystick1Button5;
			case Button.LeftShoulder:
				return KeyCode.Joystick1Button4;
			case Button.Back:
				return KeyCode.Joystick1Button6;
			case Button.Start:
				return KeyCode.Joystick1Button7;
			case Button.LeftStick:
				return KeyCode.Joystick1Button8;
			case Button.RightStick:
				return KeyCode.Joystick1Button9;
			}
			break;
		case Player.Two:
			switch (KLNKEPMAGKF)
			{
			case Button.A:
				return KeyCode.Joystick2Button0;
			case Button.B:
				return KeyCode.Joystick2Button1;
			case Button.X:
				return KeyCode.Joystick2Button2;
			case Button.Y:
				return KeyCode.Joystick2Button3;
			case Button.RightShoulder:
				return KeyCode.Joystick2Button5;
			case Button.LeftShoulder:
				return KeyCode.Joystick2Button4;
			case Button.Back:
				return KeyCode.Joystick2Button6;
			case Button.Start:
				return KeyCode.Joystick2Button7;
			case Button.LeftStick:
				return KeyCode.Joystick2Button8;
			case Button.RightStick:
				return KeyCode.Joystick2Button9;
			}
			break;
		case Player.Three:
			switch (KLNKEPMAGKF)
			{
			case Button.A:
				return KeyCode.Joystick3Button0;
			case Button.B:
				return KeyCode.Joystick3Button1;
			case Button.X:
				return KeyCode.Joystick3Button2;
			case Button.Y:
				return KeyCode.Joystick3Button3;
			case Button.RightShoulder:
				return KeyCode.Joystick3Button5;
			case Button.LeftShoulder:
				return KeyCode.Joystick3Button4;
			case Button.Back:
				return KeyCode.Joystick3Button6;
			case Button.Start:
				return KeyCode.Joystick3Button7;
			case Button.LeftStick:
				return KeyCode.Joystick3Button8;
			case Button.RightStick:
				return KeyCode.Joystick3Button9;
			}
			break;
		case Player.Four:
			switch (KLNKEPMAGKF)
			{
			case Button.A:
				return KeyCode.Joystick4Button0;
			case Button.B:
				return KeyCode.Joystick4Button1;
			case Button.X:
				return KeyCode.Joystick4Button2;
			case Button.Y:
				return KeyCode.Joystick4Button3;
			case Button.RightShoulder:
				return KeyCode.Joystick4Button5;
			case Button.LeftShoulder:
				return KeyCode.Joystick4Button4;
			case Button.Back:
				return KeyCode.Joystick4Button6;
			case Button.Start:
				return KeyCode.Joystick4Button7;
			case Button.LeftStick:
				return KeyCode.Joystick4Button8;
			case Button.RightStick:
				return KeyCode.Joystick4Button9;
			}
			break;
		case Player.Any:
			switch (KLNKEPMAGKF)
			{
			case Button.A:
				return KeyCode.JoystickButton0;
			case Button.B:
				return KeyCode.JoystickButton1;
			case Button.X:
				return KeyCode.JoystickButton2;
			case Button.Y:
				return KeyCode.JoystickButton3;
			case Button.RightShoulder:
				return KeyCode.JoystickButton5;
			case Button.LeftShoulder:
				return KeyCode.JoystickButton4;
			case Button.Back:
				return KeyCode.JoystickButton6;
			case Button.Start:
				return KeyCode.JoystickButton7;
			case Button.LeftStick:
				return KeyCode.JoystickButton8;
			case Button.RightStick:
				return KeyCode.JoystickButton9;
			}
			break;
		}
		return KeyCode.None;
	}

	public static GamepadState GetState(Player EKFPHMLKDAP, bool IMFLNPNECCO = false)
	{
		GamepadState iOIGCCPIJPN = new GamepadState();
		iOIGCCPIJPN.IEKADOOKFKG = GetButton(Button.A, EKFPHMLKDAP);
		iOIGCCPIJPN.LDKCOIHONPG = GetButton(Button.B, EKFPHMLKDAP);
		iOIGCCPIJPN.IHAHIEHHNCG = GetButton(Button.Y, EKFPHMLKDAP);
		iOIGCCPIJPN.NPKMJMCLDAH = GetButton(Button.X, EKFPHMLKDAP);
		iOIGCCPIJPN.CLIBGHJKICF = GetButton(Button.RightShoulder, EKFPHMLKDAP);
		iOIGCCPIJPN.GGMOMECKAGP = GetButton(Button.LeftShoulder, EKFPHMLKDAP);
		iOIGCCPIJPN.KDPBFODDKOJ = GetButton(Button.RightStick, EKFPHMLKDAP);
		iOIGCCPIJPN.ELAPGGICPLB = GetButton(Button.LeftStick, EKFPHMLKDAP);
		iOIGCCPIJPN.Start = GetButton(Button.Start, EKFPHMLKDAP);
		iOIGCCPIJPN.AJLBHIHFFCE = GetButton(Button.Back, EKFPHMLKDAP);
		iOIGCCPIJPN.HNPGBMGKGEB = GetStick(Stick.LeftStick, EKFPHMLKDAP, IMFLNPNECCO);
		iOIGCCPIJPN.IMMFMNIFNEH = GetStick(Stick.RightStick, EKFPHMLKDAP, IMFLNPNECCO);
		iOIGCCPIJPN.PGHJPABHPLP = GetStick(Stick.Dpad, EKFPHMLKDAP, IMFLNPNECCO);
		iOIGCCPIJPN.EDCHBILGFLD = iOIGCCPIJPN.PGHJPABHPLP.x < 0f;
		iOIGCCPIJPN.NNCHJCLKHHA = iOIGCCPIJPN.PGHJPABHPLP.x > 0f;
		iOIGCCPIJPN.FJBHJIFKOMF = iOIGCCPIJPN.PGHJPABHPLP.y > 0f;
		iOIGCCPIJPN.HHMEIEKKDAL = iOIGCCPIJPN.PGHJPABHPLP.y < 0f;
		iOIGCCPIJPN.CHJIELPPCOE = GetTrigger(Trigger.LeftTrigger, EKFPHMLKDAP, IMFLNPNECCO);
		iOIGCCPIJPN.ALEANDMIOJO = GetTrigger(Trigger.RightTrigger, EKFPHMLKDAP, IMFLNPNECCO);
		return iOIGCCPIJPN;
	}
}
