using SimpleJSON;

public static class BOBHMFOINCO
{
	public static void BJPGPFIPBGH(bool AMKKLMOONEP, string GHDPPHAAPCA, object JHJDJOFPHPH)
	{
		if (!AMKKLMOONEP)
		{
			return;
		}
		JSONNode jSONNode = JSON.Parse(GHDPPHAAPCA);
		if (!(jSONNode == null) && jSONNode["data"] != null && jSONNode["data"].Value == "user")
		{
			JSONNode jSONNode2 = jSONNode["value"];
			int asInt = jSONNode2["user_id"].AsInt;
			bool flag = jSONNode2["should_log"].AsInt == 1;
			string empty = string.Empty;
			string empty2 = string.Empty;
			JSONArray asArray = jSONNode2["ab_group"].AsArray;
			if (asArray != null && asArray.Count > 0)
			{
				empty = asArray[0]["group"];
				empty2 = asArray[0]["hash"];
			}
		}
	}

	public static void KIPNAAKJKBG(bool AMKKLMOONEP, string GHDPPHAAPCA, object JHJDJOFPHPH)
	{
	}
}
