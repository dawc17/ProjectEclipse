using System.Collections.Generic;

public sealed class NLHENJCBLHC : AJAIAKCIJIJ
{
	public string Encode(object AOMLCBHAJJH)
	{
		return Json.Encode(AOMLCBHAJJH);
	}

	public IDictionary<string, object> DecodeMessage(string EMDHMHOKGFP)
	{
		bool IBFAPIMOMBA = false;
		IDictionary<string, object> dictionary = Json.Decode(EMDHMHOKGFP, ref IBFAPIMOMBA) as IDictionary<string, object>;
		return (!IBFAPIMOMBA) ? null : dictionary;
	}
}
