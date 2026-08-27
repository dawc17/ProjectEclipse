using System.Collections.Generic;
using System.Linq;

public class FigureTopology
{
	public static List<int> NGPPLGNODNB(int LGKJBIEDKBO)
	{
		int[] array = new int[LGKJBIEDKBO * 3];
		for (int i = 0; i < LGKJBIEDKBO; i++)
		{
			array[3 * i] = 0;
			array[3 * i + 1] = 2 + i;
			array[3 * i + 2] = 1 + i;
		}
		return array.ToList();
	}

	public static List<int> OAPJCHAALCJ(int LGKJBIEDKBO)
	{
		int[] array = new int[LGKJBIEDKBO * 6];
		for (int i = 0; i < LGKJBIEDKBO * 6; i += 6)
		{
			array[i] = i;
			array[i + 1] = i + 1;
			array[i + 2] = i + 2;
			array[i + 3] = i + 2;
			array[i + 4] = i + 3;
			array[i + 5] = i;
		}
		return array.ToList();
	}

	public static List<int> AMJOJPPFIEB(int LGKJBIEDKBO)
	{
		int[] array = new int[LGKJBIEDKBO * 3];
		for (int i = 0; i < LGKJBIEDKBO; i++)
		{
			if (i % 2 == 0)
			{
				array[3 * i] = i;
				array[3 * i + 1] = i + 1;
				array[3 * i + 2] = i + 2;
			}
			else
			{
				array[3 * i] = i;
				array[3 * i + 1] = i + 2;
				array[3 * i + 2] = i + 1;
			}
		}
		return array.ToList();
	}
}
