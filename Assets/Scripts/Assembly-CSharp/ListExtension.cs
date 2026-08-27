using System;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtension
{
	public static int AddIfNotExist<T>(this List<T> OMKIGJOLJJE, T FAKOMBAIFPP)
	{
		int num = OMKIGJOLJJE.IndexOf(FAKOMBAIFPP);
		if (num == -1)
		{
			OMKIGJOLJJE.Add(FAKOMBAIFPP);
			return OMKIGJOLJJE.Count - 1;
		}
		return num;
	}

	public static int AddIfNotExist<T>(this List<T> OMKIGJOLJJE, List<T> FNGODBOFAJD) where T : class
	{
		int count = OMKIGJOLJJE.Count;
		for (int i = 0; i < FNGODBOFAJD.Count; i++)
		{
			OMKIGJOLJJE.AddIfNotExist(FNGODBOFAJD[i]);
		}
		return OMKIGJOLJJE.Count - count;
	}

	public static bool ANNPHPHLNEH<T>(this List<T> EGJHGBCEPHO, List<T> BPLIHEIIBFP) where T : IComparable
	{
		int count = BPLIHEIIBFP.Count;
		int count2 = EGJHGBCEPHO.Count;
		if (count <= count2)
		{
			List<bool> list = new List<bool>(count2);
			for (int i = 0; i < count2; i++)
			{
				list.Add(false);
			}
			foreach (T item in BPLIHEIIBFP)
			{
				bool flag = false;
				for (int j = 0; j < EGJHGBCEPHO.Count; j++)
				{
					if (!list[j] && item.Equals(EGJHGBCEPHO[j]))
					{
						flag = true;
						list[j] = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}

	public static T CJBCAIOBHMP<T>(this List<T> OMKIGJOLJJE) where T : class
	{
		int count = OMKIGJOLJJE.Count;
		if (count == 0)
		{
			return (T)null;
		}
		return OMKIGJOLJJE[UnityEngine.Random.Range(0, count)];
	}

	public static void CPCAJIKOIEE<T>(this List<T> OMKIGJOLJJE, int GNDPBMIJEMH) where T : new()
	{
		int count = OMKIGJOLJJE.Count;
		if (count == GNDPBMIJEMH)
		{
			return;
		}
		if (count > GNDPBMIJEMH)
		{
			for (int num = count - 1; num >= GNDPBMIJEMH; num--)
			{
				OMKIGJOLJJE.RemoveAt(num);
			}
		}
		else
		{
			for (int i = count; i < GNDPBMIJEMH; i++)
			{
				OMKIGJOLJJE.Add(new T());
			}
		}
	}

	public static List<T> KJCJIHJOLFC<T>(this List<T> OMKIGJOLJJE)
	{
		List<T> list = new List<T>();
		foreach (T item in OMKIGJOLJJE)
		{
			if (!list.Contains(item))
			{
				list.Add(item);
			}
		}
		return list;
	}
}
