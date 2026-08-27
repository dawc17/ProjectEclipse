using System;
using System.Collections.Generic;
using System.Diagnostics;

public static class ConsoleDatabase
{
	public delegate string CommandFunction(params string[] LKIOKGCNKHE);

	private class CommandData
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private CommandFunction GNLOEEPMBAE;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool BNOGJDKNPNK;

		public CommandFunction LFGMKDBLKIM
		{
			get
			{
				return GELBGNNLCPL();
			}
			private set
			{
				BOMEKPKGNGF(value);
			}
		}

		public bool DCEDOJMGMFD
		{
			get
			{
				return NKMNCHFLCOB();
			}
			private set
			{
				set_IgnoreCase(value);
			}
		}

		public CommandData(CommandFunction MHAEIBNCMPL, bool DLEJFILIEGL)
		{
			BOMEKPKGNGF(MHAEIBNCMPL);
			set_IgnoreCase(DLEJFILIEGL);
		}

		public CommandFunction GELBGNNLCPL()
		{
			return GNLOEEPMBAE;
		}

		private void BOMEKPKGNGF(CommandFunction value)
		{
			GNLOEEPMBAE = value;
		}

		public bool NKMNCHFLCOB()
		{
			return BNOGJDKNPNK;
		}

		private void set_IgnoreCase(bool value)
		{
			BNOGJDKNPNK = value;
		}
	}

	private const string _CommandArgsSeparator = " ";

	private static readonly char[] _ArgsSeparators = new char[1] { ' ' };

	private static readonly Dictionary<string, CommandData> CBJMDPIEOBC = new Dictionary<string, CommandData>();

	public static bool HasCommand(string name)
	{
		name = name.ToLower();
		return CBJMDPIEOBC.ContainsKey(name) && CBJMDPIEOBC[name] != null;
	}

	public static string ExecuteCommand(string LEKEGLMDAHA)
	{
		if (!string.IsNullOrEmpty(LEKEGLMDAHA))
		{
			LEKEGLMDAHA = LEKEGLMDAHA.Trim(' ');
			int num = ((!LEKEGLMDAHA.Contains(" ")) ? LEKEGLMDAHA.Length : LEKEGLMDAHA.IndexOf(" ", StringComparison.Ordinal));
			string text = LEKEGLMDAHA.Substring(0, num).ToLower();
			LEKEGLMDAHA = LEKEGLMDAHA.Remove(0, num);
			LEKEGLMDAHA = LEKEGLMDAHA.Trim(' ');
			if (HasCommand(text))
			{
				CommandData kHGKFJFOEBE = CBJMDPIEOBC[text];
				if (kHGKFJFOEBE.NKMNCHFLCOB())
				{
					LEKEGLMDAHA = LEKEGLMDAHA.ToLower();
				}
				string[] lKIOKGCNKHE = LEKEGLMDAHA.Split(_ArgsSeparators, StringSplitOptions.RemoveEmptyEntries);
				return kHGKFJFOEBE.GELBGNNLCPL()(lKIOKGCNKHE);
			}
		}
		return "Unknown command!";
	}

	public static void IMEAIJNKBOP(string name, CommandFunction LEPDMLGJCKI, bool DLEJFILIEGL = true)
	{
		CBJMDPIEOBC[name.ToLower()] = new CommandData(LEPDMLGJCKI, DLEJFILIEGL);
	}

	public static void UnregisterCommand(string name)
	{
		CBJMDPIEOBC.Remove(name.ToLower());
	}
}
