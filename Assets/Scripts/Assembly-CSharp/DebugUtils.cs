using System;
using System.Collections.Generic;
using UnityEngine;

public static class DebugUtils
{
	private static Dictionary<string, double> _TimeStart = new Dictionary<string, double>();

	private static double _SceneLoadTime;

	private static long _Memory;

	public static double DDNPHALMBJM
	{
		get
		{
			return EHCEPAEEGAI();
		}
	}

	public static double KDJCKPLKJMI()
	{
		DateTime dateTime = new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Utc);
		return (DateTime.UtcNow - dateTime).TotalMilliseconds;
	}

	public static void ELDNFPLKKPN(string HMDBGGEMICE = "")
	{
		if (_TimeStart.ContainsKey(HMDBGGEMICE))
		{
			_TimeStart[HMDBGGEMICE] = KDJCKPLKJMI();
		}
		else
		{
			_TimeStart.Add(HMDBGGEMICE, KDJCKPLKJMI());
		}
	}

	public static double NEEKBHAEDON(string HMDBGGEMICE = "")
	{
		double value;
		_TimeStart.TryGetValue(HMDBGGEMICE, out value);
		return KDJCKPLKJMI() - value;
	}

	public static double CMFAPHAAHFK(string HMDBGGEMICE = "")
	{
		double num = NEEKBHAEDON(HMDBGGEMICE);
		Debug.Log("Time: " + num);
		return num;
	}

	public static void StopAndStartTimer(string BKODCLIDPCP, string HMDBGGEMICE = "")
	{
		StopTimerWithMessage(BKODCLIDPCP, HMDBGGEMICE);
		ELDNFPLKKPN(HMDBGGEMICE);
	}

	public static double StopTimerWithMessage(string BKODCLIDPCP, string HMDBGGEMICE = "")
	{
		double num = NEEKBHAEDON(HMDBGGEMICE);
		Debug.Log("Time(" + BKODCLIDPCP + "): " + num);
		return num;
	}

	public static double EHCEPAEEGAI()
	{
		return _SceneLoadTime;
	}

	public static void PEGAFGCKLIC()
	{
		ELDNFPLKKPN("SceneLoadTime");
	}

	public static void NOBALMGNJFJ()
	{
		_SceneLoadTime = NEEKBHAEDON("SceneLoadTime");
	}

	public static void AFKKMKLHGDH()
	{
		GC.Collect();
		_Memory = GC.GetTotalMemory(true);
	}

	public static void IPMOBAJJIJG(string HNBKNLGDHFF = null)
	{
		GC.Collect();
		Debug.Log("Mem" + ((HNBKNLGDHFF == null) ? " :" : ("(" + HNBKNLGDHFF + ") :")) + (float)(GC.GetTotalMemory(true) - _Memory) / 1048576f);
	}
}
