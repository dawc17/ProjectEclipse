using System;
using System.Collections;
using System.Diagnostics;

namespace Nekki.SF2.Core.Network
{
	public class ServerProvider : ServerProviderBase, JNEBPDNJFJG
	{
		private static ServerProvider _Instance;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static string OFLIMNFAFHN;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static string BBNDHEMGHHK;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static string KPBKNDNFHEM;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static string DGMBMOLNBHL;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static string GGBOJPFJBJH;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static string BDMEDOELLNO;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static string LCKGFBDBCOF;

		private static int PIINNMAEFIJ;

		public string GKPACGEBJFP
		{
			get
			{
				return get_PutServer();
			}
		}

		public new static ServerProvider BPCBBHAKFDM
		{
			get
			{
				return get_Instance();
			}
		}

		public static string LBGNJDOKHEH
		{
			get
			{
				return get_PutURL();
			}
			set
			{
				set_PutURL(value);
			}
		}

		public static string CMGMOIJANNL
		{
			get
			{
				return get_GetURL();
			}
			set
			{
				set_GetURL(value);
			}
		}

		public static string GJBPJGLDPIJ
		{
			get
			{
				return get_ConfigURL();
			}
			set
			{
				set_ConfigURL(value);
			}
		}

		public static string EEEKJKIKLMD
		{
			get
			{
				return get_TimeServerURL();
			}
			set
			{
				set_TimeServerURL(value);
			}
		}

		public static string IIHIMJFONIL
		{
			get
			{
				return get_DumpPutURL();
			}
			set
			{
				set_DumpPutURL(value);
			}
		}

		public static string AHNMIPKNAKO
		{
			get
			{
				return get_DumpGetURL();
			}
			set
			{
				set_DumpGetURL(value);
			}
		}

		public static int LBHOGGEBNKD
		{
			get
			{
				return get_LoginInterval();
			}
			set
			{
				set_LoginInterval(value);
			}
		}

		protected override string NFKOPHMCLFF()
		{
			return get_GetURL();
		}

		public string get_PutServer()
		{
			return get_PutURL();
		}

		public new static ServerProvider get_Instance()
		{
			if (_Instance == null)
			{
				_Instance = ServerProviderBase.Init<ServerProvider>();
				_Instance.Init();
			}
			return _Instance;
		}

		public static string get_PutURL()
		{
			return OFLIMNFAFHN;
		}

		public static void set_PutURL(string value)
		{
			OFLIMNFAFHN = value;
		}

		public static string get_GetURL()
		{
			return BBNDHEMGHHK;
		}

		public static void set_GetURL(string value)
		{
			BBNDHEMGHHK = value;
		}

		public static string get_ConfigURL()
		{
			return KPBKNDNFHEM;
		}

		public static void set_ConfigURL(string value)
		{
			KPBKNDNFHEM = value;
		}

		public static string get_TimeServerURL()
		{
			return DGMBMOLNBHL;
		}

		public static void set_TimeServerURL(string value)
		{
			DGMBMOLNBHL = value;
		}

		public static string get_UserID()
		{
			return GGBOJPFJBJH;
		}

		public static void set_UserID(string value)
		{
			GGBOJPFJBJH = value;
		}

		public static string get_DumpPutURL()
		{
			return BDMEDOELLNO;
		}

		public static void set_DumpPutURL(string value)
		{
			BDMEDOELLNO = value;
		}

		public static string get_DumpGetURL()
		{
			return LCKGFBDBCOF;
		}

		public static void set_DumpGetURL(string value)
		{
			LCKGFBDBCOF = value;
		}

		public static int get_LoginInterval()
		{
			return PIINNMAEFIJ;
		}

		public static void set_LoginInterval(int value)
		{
			PIINNMAEFIJ = value;
		}

		public static void Reset()
		{
			PIINNMAEFIJ = 0;
		}

		protected override void Init()
		{
		}

		protected override IEnumerator TimeSyncRoutine(Action<long> onDone, Action<string> onError)
		{
			onDone?.Invoke((long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
			yield break;
		}

		public void VerifyPurchaseAction(JLDHCFFAIPK PAENLDALDGB, string DBKFOHCPLDB, Action<bool, string, object> p_delegate)
		{
			StartCoroutine(FailRequest(p_delegate, PAENLDALDGB));
		}

		public void ConfirmVerificationAction(JLDHCFFAIPK PAENLDALDGB, string DBKFOHCPLDB, Action<bool, string, object> p_delegate)
		{
			StartCoroutine(FailRequest(p_delegate, PAENLDALDGB));
		}

		public void SendGiveLogin(Action<bool, string, object> IKFMKMEHJFF)
		{
			StartCoroutine(FailRequest(IKFMKMEHJFF, null));
		}

		public void CheckLedger(string BEPKJNKCKPH, Action<bool, string, object> IKFMKMEHJFF)
		{
			StartCoroutine(FailRequest(IKFMKMEHJFF, null));
		}

		public void ConfirmLedger(string BEPKJNKCKPH, Action<bool, string, object> IKFMKMEHJFF, string DIAIIPCBMFL)
		{
			StartCoroutine(FailRequest(IKFMKMEHJFF, null));
		}

		// Retain callback timing and caller state without constructing remote payloads.
		private IEnumerator FailRequest(Action<bool, string, object> callback, object state)
		{
			callback?.Invoke(false, "offline build", state);
			yield break;
		}

		public const bool OFFLINE = true;

		public void DownloadFile(string p_url, Action<byte[], string, string> p_onDownloadComplete, Action<float> MDJEOHMECHA = null, int DGDKHFPEHOG = 0)
		{
			p_onDownloadComplete?.Invoke(new byte[0], "offline build", p_url);
		}
	}
}
