using SimpleJSON;

public static class JsonUtils
{
	public static int ParseInt(this JSONNode MEEAKLDGLDF, int KDLNPAGLMHF = 0)
	{
		if (MEEAKLDGLDF == null)
		{
			return KDLNPAGLMHF;
		}
		int result;
		return (!int.TryParse(MEEAKLDGLDF.Value, out result)) ? KDLNPAGLMHF : result;
	}

	public static long ParseLong(this JSONNode MEEAKLDGLDF, long KDLNPAGLMHF = 0L)
	{
		if (MEEAKLDGLDF == null)
		{
			return KDLNPAGLMHF;
		}
		long result;
		return (!long.TryParse(MEEAKLDGLDF.Value, out result)) ? KDLNPAGLMHF : result;
	}

	public static uint ParseUint(this JSONNode MEEAKLDGLDF, uint KDLNPAGLMHF = 0u)
	{
		if (MEEAKLDGLDF == null)
		{
			return KDLNPAGLMHF;
		}
		uint result;
		return (!uint.TryParse(MEEAKLDGLDF.Value, out result)) ? KDLNPAGLMHF : result;
	}

	public static float ParseFloat(this JSONNode MEEAKLDGLDF, float KDLNPAGLMHF = 0f)
	{
		if (MEEAKLDGLDF == null)
		{
			return KDLNPAGLMHF;
		}
		float result;
		return (!float.TryParse(MEEAKLDGLDF.Value, out result)) ? KDLNPAGLMHF : result;
	}

	public static bool ParseBool(this JSONNode MEEAKLDGLDF, bool KDLNPAGLMHF = false)
	{
		if (MEEAKLDGLDF == null)
		{
			return KDLNPAGLMHF;
		}
		int result;
		return (!int.TryParse(MEEAKLDGLDF.Value, out result)) ? KDLNPAGLMHF : (result > 0);
	}

	public static string ParseString(JSONNode MEEAKLDGLDF, string KDLNPAGLMHF = null)
	{
		if (MEEAKLDGLDF == null)
		{
			return KDLNPAGLMHF;
		}
		return MEEAKLDGLDF.Value;
	}

	public static string CIPOICEEIBK(this JSONNode MEEAKLDGLDF, string KDLNPAGLMHF = null)
	{
		if (MEEAKLDGLDF == null)
		{
			return KDLNPAGLMHF;
		}
		return MEEAKLDGLDF.Value;
	}

	public static JSONNode GetNode(this JSONNode MEEAKLDGLDF, string PEMMNLHBHIA)
	{
		if (MEEAKLDGLDF != null && !string.IsNullOrEmpty(PEMMNLHBHIA))
		{
			return MEEAKLDGLDF[PEMMNLHBHIA];
		}
		return null;
	}
}
