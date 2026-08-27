using System;
using UnityEngine;

namespace GooglePlayGames.OurUtils
{
	public class Logger
	{
		private static bool debugLogEnabled;

		private static bool warningLogEnabled = true;

		public static bool DebugLogEnabled
		{
			get
			{
				return debugLogEnabled;
			}
			set
			{
				debugLogEnabled = value;
			}
		}

		public static bool WarningLogEnabled
		{
			get
			{
				return warningLogEnabled;
			}
			set
			{
				warningLogEnabled = value;
			}
		}

		public static void d(string msg)
		{
			if (debugLogEnabled)
			{
				PlayGamesHelperObject.RunOnGameThread(() =>
				{
					Debug.Log(ToLogMessage(string.Empty, "DEBUG", msg));
				});
			}
		}

		public static void w(string msg)
		{
			if (warningLogEnabled)
			{
				PlayGamesHelperObject.RunOnGameThread(() =>
				{
					Debug.LogWarning(ToLogMessage("!!!", "WARNING", msg));
				});
			}
		}

		public static void e(string msg)
		{
			if (warningLogEnabled)
			{
				PlayGamesHelperObject.RunOnGameThread(() =>
				{
					Debug.LogWarning(ToLogMessage("***", "ERROR", msg));
				});
			}
		}

		public static string describe(byte[] b)
		{
			return (b != null) ? ("byte[" + b.Length + "]") : "(null)";
		}

		private static string ToLogMessage(string prefix, string logType, string msg)
		{
			return string.Format("{0} [Play Games Plugin DLL] {1} {2}: {3}", prefix, DateTime.Now.ToString("MM/dd/yy H:mm:ss zzz"), logType, msg);
		}
	}
}
