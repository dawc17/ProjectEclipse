using System;

public static class FAEJIAODPEA
{
	private static long ANIOEODPPMN = 1262304000000L;

	public static long HOHOJLNPOGA
	{
		get
		{
			return IKJMBCFLHMC();
		}
	}

	public static TimeZone BBDAANPCHLL
	{
		get
		{
			return BCBCEAJCHLM();
		}
	}

	public static TimeSpan NOOIBEEIAFD
	{
		get
		{
			return HIMDGBKLCKE();
		}
	}

	public static long IKJMBCFLHMC()
	{
		return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
	}

	public static int EOMOODICAAG(long AIJGNBNAGGJ)
	{
		return (int)((double)(AIJGNBNAGGJ - ANIOEODPPMN) * 0.001);
	}

	public static long CACFFIJKJBI(int AIJGNBNAGGJ)
	{
		return (long)AIJGNBNAGGJ * 1000L + ANIOEODPPMN;
	}

	public static TimeZone BCBCEAJCHLM()
	{
		return TimeZone.CurrentTimeZone;
	}

	public static TimeSpan HIMDGBKLCKE()
	{
		return BCBCEAJCHLM().GetUtcOffset(DateTime.Now);
	}
}
