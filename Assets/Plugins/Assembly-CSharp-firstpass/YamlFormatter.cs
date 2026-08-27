using System;
using System.Globalization;

internal static class YamlFormatter
{
	private static readonly NumberFormatInfo MNNCBNBBPHN = new NumberFormatInfo
	{
		CurrencyDecimalSeparator = ".",
		CurrencyGroupSeparator = "_",
		CurrencyGroupSizes = new int[1] { 3 },
		CurrencySymbol = string.Empty,
		CurrencyDecimalDigits = 99,
		NumberDecimalSeparator = ".",
		NumberGroupSeparator = "_",
		NumberGroupSizes = new int[1] { 3 },
		NumberDecimalDigits = 99
	};

	public static string DGIAFODNLNN(object number)
	{
		return Convert.ToString(number, MNNCBNBBPHN);
	}

	public static string NMBPLFHGICK(object CIGMFMBICLJ)
	{
		return (!CIGMFMBICLJ.Equals(true)) ? "false" : "true";
	}

	public static string AHNEOKMPCPD(object KLHLNCMNKDD)
	{
		return ((DateTime)KLHLNCMNKDD).ToString("o", CultureInfo.InvariantCulture);
	}

	public static string ALEIMPLLAHI(object NFBCAMOCHFG)
	{
		return ((TimeSpan)NFBCAMOCHFG/*cast due to constrained. prefix*/).ToString();
	}
}
