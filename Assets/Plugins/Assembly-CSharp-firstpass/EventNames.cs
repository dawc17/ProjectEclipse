using System;

public static class EventNames
{
	public const string NDCILHIAPIK = "connect";

	public const string Disconnect = "disconnect";

	public const string MOFKKABEFEB = "event";

	public const string FIGCHOHPPPI = "ack";

	public const string Error = "error";

	public const string AMLMOEEMOEO = "binaryevent";

	public const string FJLJMLNKCBG = "binaryack";

	private static string[] LCBKLNLLBOE = new string[8] { "unknown", "connect", "disconnect", "event", "ack", "error", "binaryevent", "binaryack" };

	private static string[] AGDHHPCMOGF = new string[8] { "unknown", "open", "close", "ping", "pong", "message", "upgrade", "noop" };

	private static string[] DMMFNNALHJN = new string[10] { "connect", "connect_error", "connect_timeout", "disconnect", "error", "reconnect", "reconnect_attempt", "reconnect_failed", "reconnect_error", "reconnecting" };

	public static string ICAIODPBKBO(ECDAJBEFCAH LFLGCDNKNJI)
	{
		return LCBKLNLLBOE[(int)(LFLGCDNKNJI + 1)];
	}

	public static string ICAIODPBKBO(HJDLGPHLPNF AJONKGOAHJH)
	{
		return AGDHHPCMOGF[(int)(AJONKGOAHJH + 1)];
	}

	public static bool IsBlacklisted(string DOPHKKGNAEF)
	{
		for (int i = 0; i < DMMFNNALHJN.Length; i++)
		{
			if (string.Compare(DMMFNNALHJN[i], DOPHKKGNAEF, StringComparison.OrdinalIgnoreCase) == 0)
			{
				return true;
			}
		}
		return false;
	}
}
