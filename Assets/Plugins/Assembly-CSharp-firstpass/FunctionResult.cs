using System.Collections.Generic;

public class FunctionResult
{
	public string DCJLKCFKCOM;

	public int ToInt()
	{
		float result;
		if (!float.TryParse(DCJLKCFKCOM, out result))
		{
			Dictionary<string, RpnParser.PHNLIHEJEPK> pPEABEJMCPI = new Dictionary<string, RpnParser.PHNLIHEJEPK>();
			Dictionary<string, RpnParser.ParameterDelegate> gIOGAJGIGMO = new Dictionary<string, RpnParser.ParameterDelegate>();
			RpnParser.init(pPEABEJMCPI, gIOGAJGIGMO);
			RpnParser.Formula lANLKOHCGEJ = new RpnParser.Formula(DCJLKCFKCOM);
			object obj = lANLKOHCGEJ.ODHJHHMEEOI();
			if (!float.TryParse(obj.ToString(), out result))
			{
				result = 0f;
			}
		}
		return (int)result;
	}

	public float ToFloat()
	{
		float result;
		if (!float.TryParse(DCJLKCFKCOM, out result))
		{
			Dictionary<string, RpnParser.PHNLIHEJEPK> pPEABEJMCPI = new Dictionary<string, RpnParser.PHNLIHEJEPK>();
			Dictionary<string, RpnParser.ParameterDelegate> gIOGAJGIGMO = new Dictionary<string, RpnParser.ParameterDelegate>();
			RpnParser.init(pPEABEJMCPI, gIOGAJGIGMO);
			RpnParser.Formula lANLKOHCGEJ = new RpnParser.Formula(DCJLKCFKCOM);
			object obj = lANLKOHCGEJ.ODHJHHMEEOI();
			if (!float.TryParse(obj.ToString(), out result))
			{
				return 0f;
			}
		}
		return result;
	}
}
