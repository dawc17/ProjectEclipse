using System;
using System.Collections.Generic;
using System.Diagnostics;

public sealed class HandshakeData
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string NHKGFBIECIP;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private List<string> JLFGFPBGLBD;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private TimeSpan MLFPNHJCAFN;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private TimeSpan CIOJBNFOFAP;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private SocketManager JNNOJIEMLEK;

	public Action<HandshakeData> OnReceived;

	public Action<HandshakeData, string> OnError;

	private HTTPRequest EKJLAHPCEJM;

	public string LLPJGICMGNP
	{
		get
		{
			return EDLOIOOBPAJ();
		}
		private set
		{
			set_Sid(value);
		}
	}

	public List<string> EKJFMJBHMKK
	{
		get
		{
			return BLCLIKIBIPE();
		}
		private set
		{
			set_Upgrades(value);
		}
	}

	public TimeSpan PingInterval
	{
		get
		{
			return CMFJFNKMJIP();
		}
		private set
		{
			HKOAJEPLLJO(value);
		}
	}

	public TimeSpan OIPKMBICKLH
	{
		get
		{
			return EPFLHIKEBFO();
		}
		private set
		{
			FGBNJKNGKBF(value);
		}
	}

	public SocketManager CPOHGNDIBJD
	{
		get
		{
			return HLBNHJADOMP();
		}
		private set
		{
			CMOJGLBBCKC(value);
		}
	}

	public HandshakeData(SocketManager BJGMPDIKEJC)
	{
		CMOJGLBBCKC(BJGMPDIKEJC);
	}

	public string EDLOIOOBPAJ()
	{
		return NHKGFBIECIP;
	}

	private void set_Sid(string value)
	{
		NHKGFBIECIP = value;
	}

	public List<string> BLCLIKIBIPE()
	{
		return JLFGFPBGLBD;
	}

	private void set_Upgrades(List<string> value)
	{
		JLFGFPBGLBD = value;
	}

	public TimeSpan CMFJFNKMJIP()
	{
		return MLFPNHJCAFN;
	}

	private void HKOAJEPLLJO(TimeSpan value)
	{
		MLFPNHJCAFN = value;
	}

	public TimeSpan EPFLHIKEBFO()
	{
		return CIOJBNFOFAP;
	}

	private void FGBNJKNGKBF(TimeSpan value)
	{
		CIOJBNFOFAP = value;
	}

	public SocketManager HLBNHJADOMP()
	{
		return JNNOJIEMLEK;
	}

	private void CMOJGLBBCKC(SocketManager value)
	{
		JNNOJIEMLEK = value;
	}

	internal void Start()
	{
		if (EKJLAHPCEJM == null)
		{
			object[] obj = new object[5]
			{
				HLBNHJADOMP().OJBDMGBGJMA().ToString(),
				4,
				HLBNHJADOMP().GPEEDKOHFIG(),
				null,
				null
			};
			SocketManager mFANOMMMCFG = HLBNHJADOMP();
			ulong num;
			mFANOMMMCFG.set_RequestCounter((num = mFANOMMMCFG.EKBGNBPGFNG()) + 1);
			obj[3] = num;
			obj[4] = HLBNHJADOMP().HLHJJJGJEEL().LEKAOBKGMPF();
			EKJLAHPCEJM = new HTTPRequest(new Uri(string.Format("{0}?EIO={1}&transport=polling&t={2}-{3}{4}&b64=true", obj)), FHIJDHEAOLD);
			EKJLAHPCEJM.JJCLPAGJEBJ(true);
			EKJLAHPCEJM.Send();
			HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("HandshakeData", "Handshake request sent");
		}
	}

	internal void AKLEEMEHBIC()
	{
		if (EKJLAHPCEJM != null)
		{
			EKJLAHPCEJM.AKLEEMEHBIC();
		}
		EKJLAHPCEJM = null;
		OnReceived = null;
		OnError = null;
	}

	private void FHIJDHEAOLD(HTTPRequest CGOIOKHEGOE, HTTPResponse BEIGFGCBICO)
	{
		EKJLAHPCEJM = null;
		switch (CGOIOKHEGOE.FLBBFDNHJAJ())
		{
		case CFGBMHKCENK.Finished:
			if (BEIGFGCBICO.AICKPAMONBH())
			{
				HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("HandshakeData", "Handshake data arrived: " + BEIGFGCBICO.DPBLPGKOEJB());
				int num = BEIGFGCBICO.DPBLPGKOEJB().IndexOf("{");
				if (num < 0)
				{
					RaiseOnError("Invalid handshake text: " + BEIGFGCBICO.DPBLPGKOEJB());
					break;
				}
				HandshakeData pNAFNLKDFKD = Parse(BEIGFGCBICO.DPBLPGKOEJB().Substring(num));
				if (pNAFNLKDFKD == null)
				{
					RaiseOnError("Parsing Handshake data failed: " + BEIGFGCBICO.DPBLPGKOEJB());
				}
				else if (OnReceived != null)
				{
					OnReceived(this);
					OnReceived = null;
				}
			}
			else
			{
				RaiseOnError(string.Format("Handshake request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2} Uri: {3}", BEIGFGCBICO.KNMDPGBPNED(), BEIGFGCBICO.DCKPMHKDLEJ(), BEIGFGCBICO.DPBLPGKOEJB(), CGOIOKHEGOE.DKAECMGPGOE()));
			}
			break;
		case CFGBMHKCENK.Error:
			RaiseOnError((CGOIOKHEGOE.IEFGFKFHNMD() == null) ? string.Empty : (CGOIOKHEGOE.IEFGFKFHNMD().Message + " " + CGOIOKHEGOE.IEFGFKFHNMD().StackTrace));
			break;
		default:
			RaiseOnError(CGOIOKHEGOE.FLBBFDNHJAJ().ToString());
			break;
		}
	}

	private void RaiseOnError(string KEPBNIIECPN)
	{
		HTTPManager.MBBMPNDDPIH().Error("HandshakeData", "Handshake request failed with error: " + KEPBNIIECPN);
		if (OnError != null)
		{
			OnError(this, KEPBNIIECPN);
			OnError = null;
		}
	}

	private HandshakeData Parse(string IGGFGLLIGCG)
	{
		bool IBFAPIMOMBA = false;
		Dictionary<string, object> iOFHCAAOELD = Json.Decode(IGGFGLLIGCG, ref IBFAPIMOMBA) as Dictionary<string, object>;
		if (!IBFAPIMOMBA)
		{
			return null;
		}
		try
		{
			set_Sid(GetString(iOFHCAAOELD, "sid"));
			set_Upgrades(GetStringList(iOFHCAAOELD, "upgrades"));
			HKOAJEPLLJO(TimeSpan.FromMilliseconds(GetInt(iOFHCAAOELD, "pingInterval")));
			FGBNJKNGKBF(TimeSpan.FromMilliseconds(GetInt(iOFHCAAOELD, "pingTimeout")));
			return this;
		}
		catch
		{
			return null;
		}
	}

	private static object Get(Dictionary<string, object> IOFHCAAOELD, string KGBGENDIMBC)
	{
		object value;
		if (!IOFHCAAOELD.TryGetValue(KGBGENDIMBC, out value))
		{
			throw new Exception(string.Format("Can't get {0} from Handshake data!", KGBGENDIMBC));
		}
		return value;
	}

	private static string GetString(Dictionary<string, object> IOFHCAAOELD, string KGBGENDIMBC)
	{
		return Get(IOFHCAAOELD, KGBGENDIMBC) as string;
	}

	private static List<string> GetStringList(Dictionary<string, object> IOFHCAAOELD, string KGBGENDIMBC)
	{
		List<object> list = Get(IOFHCAAOELD, KGBGENDIMBC) as List<object>;
		List<string> list2 = new List<string>(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			string text = list[i] as string;
			if (text != null)
			{
				list2.Add(text);
			}
		}
		return list2;
	}

	private static int GetInt(Dictionary<string, object> IOFHCAAOELD, string KGBGENDIMBC)
	{
		return (int)(double)Get(IOFHCAAOELD, KGBGENDIMBC);
	}
}
