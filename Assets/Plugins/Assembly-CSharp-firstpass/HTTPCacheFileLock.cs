using System;
using System.Collections.Generic;

internal sealed class HTTPCacheFileLock
{
	private static Dictionary<Uri, object> FileLocks = new Dictionary<Uri, object>();

	private static object SyncRoot = new object();

	internal static object Acquire(Uri KJHNCLAJMLO)
	{
		lock (SyncRoot)
		{
			object value;
			if (!FileLocks.TryGetValue(KJHNCLAJMLO, out value))
			{
				FileLocks.Add(KJHNCLAJMLO, value = new object());
			}
			return value;
		}
	}

	internal static void Remove(Uri KJHNCLAJMLO)
	{
		lock (SyncRoot)
		{
			if (FileLocks.ContainsKey(KJHNCLAJMLO))
			{
				FileLocks.Remove(KJHNCLAJMLO);
			}
		}
	}

	internal static void Clear()
	{
		lock (SyncRoot)
		{
			FileLocks.Clear();
		}
	}
}
