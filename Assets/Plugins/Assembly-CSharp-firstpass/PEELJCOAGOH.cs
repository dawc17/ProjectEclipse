using System.Collections.Generic;

public sealed class PEELJCOAGOH : AJAIAKCIJIJ
{
	public string Encode(object AOMLCBHAJJH)
	{
		JsonWriter iGOCJFDLBMG = new JsonWriter();
		JsonMapper.ToJson(AOMLCBHAJJH, iGOCJFDLBMG);
		return iGOCJFDLBMG.ToString();
	}

	public IDictionary<string, object> DecodeMessage(string EMDHMHOKGFP)
	{
		JsonReader iJIMLLIHKGN = new JsonReader(EMDHMHOKGFP);
		return JsonMapper.ToObject<Dictionary<string, object>>(iJIMLLIHKGN);
	}
}
