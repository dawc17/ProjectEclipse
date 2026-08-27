using System.Collections.Generic;

public sealed class LJPCIAOIJNK : OOINGNLNJGM
{
	public List<object> Decode(string EMDHMHOKGFP)
	{
		JsonReader iJIMLLIHKGN = new JsonReader(EMDHMHOKGFP);
		return JsonMapper.ToObject<List<object>>(iJIMLLIHKGN);
	}

	public string Encode(List<object> AOMLCBHAJJH)
	{
		JsonWriter iGOCJFDLBMG = new JsonWriter();
		JsonMapper.ToJson(AOMLCBHAJJH, iGOCJFDLBMG);
		return iGOCJFDLBMG.ToString();
	}
}
