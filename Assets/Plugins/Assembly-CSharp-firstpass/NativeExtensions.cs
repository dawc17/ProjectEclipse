using System;
using System.Linq;
using UnityEngine;

public static class NativeExtensions
{
	public static void FEEGJDJIFEF(this Action IBODMPMJELJ)
	{
		if (IBODMPMJELJ != null)
		{
			IBODMPMJELJ();
		}
	}

	public static void FEEGJDJIFEF<T>(this Action<T> IBODMPMJELJ, T value)
	{
		if (IBODMPMJELJ != null)
		{
			IBODMPMJELJ(value);
		}
	}

	public static string LDNABOKCAFL<T>(this Action<T> IBODMPMJELJ)
	{
		return "(" + IBODMPMJELJ.GetInvocationList().Length + " total) " + string.Join(", ", (from d in IBODMPMJELJ.GetInvocationList()
			select d.Method.Name).ToArray());
	}

	public static bool BKOIKMEEHDK<T>(this Action<T> IBODMPMJELJ)
	{
		return IBODMPMJELJ == null || IBODMPMJELJ.GetInvocationList().Length == 0;
	}

	public static float DBKEJBHHKBM(this float FINAMGBHHDL)
	{
		return FINAMGBHHDL / 1000f;
	}

	public static bool IsEqual(this int number, Enum FOPOKALJIIJ)
	{
		try
		{
			return number == Convert.ToInt32(FOPOKALJIIJ);
		}
		catch (Exception message)
		{
			Debug.LogError(message);
			return false;
		}
	}
}
