using System;
using UnityEngine;

public static class ColorUtils
{
	public static Color DAAIIECAAFO(string OHJKNABLCMF, float KGJALFLDIBG = 1f)
	{
		OHJKNABLCMF = OHJKNABLCMF.Replace("#", string.Empty);
		OHJKNABLCMF = OHJKNABLCMF.Replace("0x", string.Empty);
		if (OHJKNABLCMF.Length != 6 && OHJKNABLCMF.Length != 8)
		{
			return new Color(0f, 0f, 0f, 1f);
		}
		int num = OHJKNABLCMF.Length / 2;
		byte[] array = new byte[4]
		{
			0,
			0,
			0,
			(byte)(255f * KGJALFLDIBG)
		};
		for (int i = 0; i < num; i++)
		{
			array[i] = Convert.ToByte(OHJKNABLCMF.Substring(i * 2, 2), 16);
		}
		return new Color((float)(int)array[0] / 255f, (float)(int)array[1] / 255f, (float)(int)array[2] / 255f, (float)(int)array[3] / 255f);
	}
}
