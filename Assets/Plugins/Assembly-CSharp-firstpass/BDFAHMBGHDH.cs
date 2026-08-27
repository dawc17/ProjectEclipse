using System;
using System.Text.RegularExpressions;

internal static class BDFAHMBGHDH
{
	private static string ToCamelOrPascalCase(string IGGFGLLIGCG, Func<char, char> BBGFDLJEEEL)
	{
		string text = Regex.Replace(IGGFGLLIGCG, "([_\\-])(?<char>[a-z])", (System.Text.RegularExpressions.Match MLPEJKLNAKF) => MLPEJKLNAKF.Groups["char"].Value.ToUpperInvariant(), RegexOptions.IgnoreCase);
		return BBGFDLJEEEL(text[0]) + text.Substring(1);
	}

	public static string KFPGHEEOKBK(this string IGGFGLLIGCG)
	{
		return ToCamelOrPascalCase(IGGFGLLIGCG, char.ToLowerInvariant);
	}

	public static string KMIGNCKKEJH(this string IGGFGLLIGCG)
	{
		return ToCamelOrPascalCase(IGGFGLLIGCG, char.ToUpperInvariant);
	}

	public static string FromCamelCase(this string IGGFGLLIGCG, string LHCEONCBNPP)
	{
		IGGFGLLIGCG = char.ToLower(IGGFGLLIGCG[0]) + IGGFGLLIGCG.Substring(1);
		IGGFGLLIGCG = Regex.Replace(IGGFGLLIGCG.KFPGHEEOKBK(), "(?<char>[A-Z])", (System.Text.RegularExpressions.Match MLPEJKLNAKF) => LHCEONCBNPP + MLPEJKLNAKF.Groups["char"].Value.ToLowerInvariant());
		return IGGFGLLIGCG;
	}
}
