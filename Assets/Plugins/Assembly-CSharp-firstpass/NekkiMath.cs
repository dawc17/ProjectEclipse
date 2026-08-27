using System;
using System.Collections.Generic;

public class NekkiMath
{
	private const double EILBPJGCDPI = 180.0 / Math.PI;

	private const double HGLPFJOCBBJ = Math.PI / 180.0;

	private const double LMJAKBLNLHN = 1.1920929E-07;

	private static NekkiRandom OBGJHBNGOML = new NekkiRandom();

	public static float round(float number, int HCHKHLJLJBG)
	{
		return (float)(int)(number * (float)HCHKHLJLJBG) / (float)HCHKHLJLJBG;
	}

	public static float KOCMHLJOCPA(float value, int OMGDBOOMDKP)
	{
		return 0f - KAJCCKDDMHL(0f - value, OMGDBOOMDKP);
	}

	public static float KAJCCKDDMHL(float value, int OMGDBOOMDKP)
	{
		float num = EPOBPGPJPNG(value, OMGDBOOMDKP);
		if (value < num && value < 0f)
		{
			return num - (float)Math.Pow(10.0, -OMGDBOOMDKP);
		}
		return num;
	}

	public static float GAHKBAANMKL(float value, int OMGDBOOMDKP)
	{
		int num = ((value > 0f) ? 1 : (-1));
		return (float)num * KOCMHLJOCPA(Math.Abs(value), OMGDBOOMDKP);
	}

	public static float EPOBPGPJPNG(float number, int order)
	{
		int num = (int)Math.Pow(10.0, order);
		return (float)(int)(number * (float)num) / (float)num;
	}

	public static string ENHGDJPCALE(float number, int order)
	{
		return Math.Round(number, order).ToString();
	}

	public static float JONPNCONENM(float EHCLMBADLKH, float _base)
	{
		float result = 0f;
		if (_base > 0f && _base != 1f && EHCLMBADLKH > 0f)
		{
			result = (float)(Math.Log(EHCLMBADLKH) / Math.Log(_base));
		}
		return result;
	}

	public static float FEONOCCEKNO(float LMKNPGGDOCO)
	{
		return (float)((double)LMKNPGGDOCO * (180.0 / Math.PI));
	}

	public static float KHFNLLBANHE(float NGMLOAFJDAP)
	{
		return (float)((double)NGMLOAFJDAP * (Math.PI / 180.0));
	}

	public static float randomFloat()
	{
		return OBGJHBNGOML.randomFloat();
	}

	public static float randomFloat(float KAEPJHHLLPK)
	{
		return OBGJHBNGOML.randomFloat(KAEPJHHLLPK);
	}

	public static float randomFloat(float LHNCHOAEGEA, float KAEPJHHLLPK)
	{
		return OBGJHBNGOML.randomFloat(LHNCHOAEGEA, KAEPJHHLLPK);
	}

	public static int randomInt(int KAEPJHHLLPK)
	{
		return (int)OBGJHBNGOML.randomInt((uint)KAEPJHHLLPK);
	}

	public static int randomInt(int LHNCHOAEGEA, int KAEPJHHLLPK)
	{
		return (int)OBGJHBNGOML.randomInt((uint)LHNCHOAEGEA, (uint)KAEPJHHLLPK);
	}

	public static bool randomChance(float AMBMJABLPFE, float BCCEJBCHNHC = 100f)
	{
		return OBGJHBNGOML.randomChance(AMBMJABLPFE, BCCEJBCHNHC);
	}

	public static uint KACCBCCEPGB()
	{
		uint num = (uint)DateTime.UtcNow.Ticks;
		OBGJHBNGOML.setSeed(num);
		return num;
	}

	public static void KACCBCCEPGB(int OKGKLCLEDFN)
	{
		OBGJHBNGOML.setSeed((uint)OKGKLCLEDFN);
	}

	public static T FGFBKJLIADI<T>(List<T> HCMPBOCKJOP)
	{
		int count = HCMPBOCKJOP.Count;
		if (count == 0)
		{
			AdvLog.CCOFFJPPAKC("NekkiMath::randomElement - empty vector");
		}
		return HCMPBOCKJOP[randomInt(count)];
	}

	private static int OHBDPMGHNFM()
	{
		return (int)OBGJHBNGOML.OHBDPMGHNFM();
	}
}
