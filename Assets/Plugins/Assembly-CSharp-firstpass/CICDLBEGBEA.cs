using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class CICDLBEGBEA
{
	public static bool BKOIKMEEHDK(this string value)
	{
		return string.IsNullOrEmpty(value);
	}

	public static string OEEPJABEJFO(this string value)
	{
		return new RpnParser.Formula(value).ODHJHHMEEOI().ToString();
	}

	public static IEnumerable<string> NCFOFFBNDFJ(this string JDCCBCNFENK, char[] delims)
	{
		int num = 0;
		while (true)
		{
			int num3;
			int num2 = (num3 = JDCCBCNFENK.IndexOfAny(delims, num));
			if (num3 == -1)
			{
				break;
			}
			if (num2 - num > 0)
			{
				yield return JDCCBCNFENK.Substring(num, num2 - num);
			}
			yield return JDCCBCNFENK.Substring(num2, 1);
			num = num2 + 1;
		}
		if (num < JDCCBCNFENK.Length)
		{
			yield return JDCCBCNFENK.Substring(num);
		}
	}

	public static void CKDJKCLCKDD(this object HCPNFPMHFCM, string color = "green")
	{
		Debug.Log(string.Concat("$<color=", color, "><b>", HCPNFPMHFCM, "</b></color>"));
	}

	public static void BFBILPIHOHN(this object HCPNFPMHFCM)
	{
		HCPNFPMHFCM.CKDJKCLCKDD("red");
	}

	public static void OFKCMLNBEAH(this object HCPNFPMHFCM)
	{
		HCPNFPMHFCM.CKDJKCLCKDD("blue");
	}

	public static string MGMLMIJOFFL(this string HCPNFPMHFCM, string ABHINJEKNLG)
	{
		return "<color=#" + ABHINJEKNLG + ">" + HCPNFPMHFCM + "</color>";
	}

	public static string MGMLMIJOFFL(this int HCPNFPMHFCM, string ABHINJEKNLG)
	{
		return HCPNFPMHFCM.ToString().MGMLMIJOFFL(ABHINJEKNLG);
	}

	public static string MGMLMIJOFFL(this float HCPNFPMHFCM, string ABHINJEKNLG)
	{
		return HCPNFPMHFCM.ToString(CultureInfo.InvariantCulture).MGMLMIJOFFL(ABHINJEKNLG);
	}
}
