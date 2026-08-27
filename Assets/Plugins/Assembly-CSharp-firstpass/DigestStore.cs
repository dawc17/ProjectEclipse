using System;
using System.Collections.Generic;

internal static class DigestStore
{
	private static Dictionary<string, Digest> MNHMKFLCJHI = new Dictionary<string, Digest>();

	private static object Locker = new object();

	private static string[] SupportedAlgorithms = new string[2] { "digest", "basic" };

	public static Digest Get(Uri KJHNCLAJMLO)
	{
		lock (Locker)
		{
			Digest value = null;
			if (MNHMKFLCJHI.TryGetValue(KJHNCLAJMLO.Host, out value) && !value.IsUriProtected(KJHNCLAJMLO))
			{
				return null;
			}
			return value;
		}
	}

	public static Digest NLJEDHBBPKK(Uri KJHNCLAJMLO)
	{
		lock (Locker)
		{
			Digest value = null;
			if (!MNHMKFLCJHI.TryGetValue(KJHNCLAJMLO.Host, out value))
			{
				MNHMKFLCJHI.Add(KJHNCLAJMLO.Host, value = new Digest(KJHNCLAJMLO));
			}
			return value;
		}
	}

	public static void Remove(Uri KJHNCLAJMLO)
	{
		lock (Locker)
		{
			MNHMKFLCJHI.Remove(KJHNCLAJMLO.Host);
		}
	}

	public static string FindBest(List<string> DBMBCAIIJAD)
	{
		if (DBMBCAIIJAD == null || DBMBCAIIJAD.Count == 0)
		{
			return string.Empty;
		}
		List<string> list = new List<string>(DBMBCAIIJAD.Count);
		for (int i = 0; i < DBMBCAIIJAD.Count; i++)
		{
			list.Add(DBMBCAIIJAD[i].ToLower());
		}
		for (int j = 0; j < SupportedAlgorithms.Length; j++)
		{
			int num = list.FindIndex((string HHAAFADDOJB) => HHAAFADDOJB.StartsWith(SupportedAlgorithms[j]));
			if (num != -1)
			{
				return DBMBCAIIJAD[num];
			}
		}
		return string.Empty;
	}
}
