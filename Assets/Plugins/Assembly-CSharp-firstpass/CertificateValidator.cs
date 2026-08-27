using System.Text.RegularExpressions;

public class CertificateValidator
{
	private const string IIHOOJPKMGK = "https://";

	private const string KGGAPPPKGGC = "nekkimobile\\.ru";

	private const string EHGNKEFFJOE = "^https://([^\\/]+\\.|)nekkimobile\\.ru(:[0-9]*)*(\\/([a-zA-Z0-9\\-\\.\\?\\,\\'\\/\\\\\\+&amp;%\\$#_]*)?|)$";

	public static bool GLHLIEOFFLN(string BEPKJNKCKPH)
	{
		return PPMFHMDKNMG(BEPKJNKCKPH);
	}

	private static bool PPMFHMDKNMG(string BEPKJNKCKPH)
	{
		return new Regex("^https://([^\\/]+\\.|)nekkimobile\\.ru(:[0-9]*)*(\\/([a-zA-Z0-9\\-\\.\\?\\,\\'\\/\\\\\\+&amp;%\\$#_]*)?|)$").Matches(BEPKJNKCKPH).Count == 1;
	}
}
