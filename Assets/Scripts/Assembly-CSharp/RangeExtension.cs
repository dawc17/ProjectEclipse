using System;
using UnityEngine.SocialPlatforms;
using Range = UnityEngine.SocialPlatforms.Range;

public static class RangeExtension
{
	public static int GEMHMCFOIMJ(this Range JMPCNIOBPAI)
	{
		if (JMPCNIOBPAI.count == 0)
		{
			throw new InvalidOperationException("Range is invalid");
		}
		return JMPCNIOBPAI.from + JMPCNIOBPAI.count - 1;
	}

	public static bool Contains(this Range JMPCNIOBPAI, int OMEDGJMNGKE)
	{
		return OMEDGJMNGKE >= JMPCNIOBPAI.from && OMEDGJMNGKE < JMPCNIOBPAI.from + JMPCNIOBPAI.count;
	}
}
