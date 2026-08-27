using System;
using System.Collections.Generic;
using System.Diagnostics;

public sealed class NegotiationData
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string KLKKKBPEPOI;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string JEKMJACHIBB;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string CFEFDADCKMF;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string FMLCDJCADML;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private TimeSpan? BAHGOFHKNKL;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private TimeSpan HGPAAOFDBID;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private TimeSpan MNFNJDDOLNN;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool CNKCCGOKOEB;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string KPKEAFHNIOA;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private TimeSpan MFEPPIMHBKP;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private TimeSpan MFHDCBOGDKF;

	public Action<NegotiationData> OnReceived;

	public Action<NegotiationData, string> OnError;

	private HTTPRequest IFLGEGDHGBD;

	private IConnection PEBFDIFIMBO;

	public string OCIMIOMMGNE
	{
		get
		{
			return IPPHLDGJADH();
		}
		private set
		{
			GCFBKKBNLDI(value);
		}
	}

	public string FFIPCFPNENI
	{
		get
		{
			return HKBCNJMMOOP();
		}
		private set
		{
			AKFLLIIIJPF(value);
		}
	}

	public string GBGLCJDKLID
	{
		get
		{
			return FNJEMLMFGEG();
		}
		private set
		{
			JFHGEPMMEEM(value);
		}
	}

	public TimeSpan? NDDJEFFPBOC
	{
		get
		{
			return FCIMGPJDODG();
		}
		private set
		{
			set_KeepAliveTimeout(value);
		}
	}

	public TimeSpan PKAGMCHJCLB
	{
		get
		{
			return BDMKAEDCGNL();
		}
		private set
		{
			OFLBEAPGAPI(value);
		}
	}

	public TimeSpan LHHIEGJPMCD
	{
		get
		{
			return LFLAILLBGOF();
		}
		private set
		{
			OICCAHABLBF(value);
		}
	}

	public bool GGHFMABJDLP
	{
		get
		{
			return AOKNIGBFMKG();
		}
		private set
		{
			set_TryWebSockets(value);
		}
	}

	public string BFJPAKFGFBH
	{
		get
		{
			return HMLNCKGEHMN();
		}
		private set
		{
			BNEOABLKJDF(value);
		}
	}

	public TimeSpan MECOAAGEOPI
	{
		get
		{
			return BICBAKIOCMM();
		}
		private set
		{
			MKEOKCINEBK(value);
		}
	}

	public TimeSpan BLMAHPJOFKK
	{
		get
		{
			return NCMIDNBFDID();
		}
		private set
		{
			IPMEBEILOEE(value);
		}
	}

	public NegotiationData(Connection MDGFGCDPGFI)
	{
		PEBFDIFIMBO = MDGFGCDPGFI;
	}

	public string KLMLKCKNNFD()
	{
		return KLKKKBPEPOI;
	}

	private void set_Url(string value)
	{
		KLKKKBPEPOI = value;
	}

	public string IPPHLDGJADH()
	{
		return JEKMJACHIBB;
	}

	private void GCFBKKBNLDI(string value)
	{
		JEKMJACHIBB = value;
	}

	public string HKBCNJMMOOP()
	{
		return CFEFDADCKMF;
	}

	private void AKFLLIIIJPF(string value)
	{
		CFEFDADCKMF = value;
	}

	public string FNJEMLMFGEG()
	{
		return FMLCDJCADML;
	}

	private void JFHGEPMMEEM(string value)
	{
		FMLCDJCADML = value;
	}

	public TimeSpan? FCIMGPJDODG()
	{
		return BAHGOFHKNKL;
	}

	private void set_KeepAliveTimeout(TimeSpan? value)
	{
		BAHGOFHKNKL = value;
	}

	public TimeSpan BDMKAEDCGNL()
	{
		return HGPAAOFDBID;
	}

	private void OFLBEAPGAPI(TimeSpan value)
	{
		HGPAAOFDBID = value;
	}

	public TimeSpan LFLAILLBGOF()
	{
		return MNFNJDDOLNN;
	}

	private void OICCAHABLBF(TimeSpan value)
	{
		MNFNJDDOLNN = value;
	}

	public bool AOKNIGBFMKG()
	{
		return CNKCCGOKOEB;
	}

	private void set_TryWebSockets(bool value)
	{
		CNKCCGOKOEB = value;
	}

	public string HMLNCKGEHMN()
	{
		return KPKEAFHNIOA;
	}

	private void BNEOABLKJDF(string value)
	{
		KPKEAFHNIOA = value;
	}

	public TimeSpan BICBAKIOCMM()
	{
		return MFEPPIMHBKP;
	}

	private void MKEOKCINEBK(TimeSpan value)
	{
		MFEPPIMHBKP = value;
	}

	public TimeSpan NCMIDNBFDID()
	{
		return MFHDCBOGDKF;
	}

	private void IPMEBEILOEE(TimeSpan value)
	{
		MFHDCBOGDKF = value;
	}

	public void Start()
	{
		IFLGEGDHGBD = new HTTPRequest(PEBFDIFIMBO.BuildUri(FHIEGKMHOCC.Negotiate), LAAFHDKKJFL.Get, true, true, LPMLIOMGAIO);
		PEBFDIFIMBO.PrepareRequest(IFLGEGDHGBD, FHIEGKMHOCC.Negotiate);
		IFLGEGDHGBD.Send();
		HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("NegotiationData", "Negotiation request sent");
	}

	public void AKLEEMEHBIC()
	{
		if (IFLGEGDHGBD != null)
		{
			OnReceived = null;
			OnError = null;
			IFLGEGDHGBD.AKLEEMEHBIC();
		}
	}

	private void LPMLIOMGAIO(HTTPRequest CGOIOKHEGOE, HTTPResponse BEIGFGCBICO)
	{
		IFLGEGDHGBD = null;
		switch (CGOIOKHEGOE.FLBBFDNHJAJ())
		{
		case CFGBMHKCENK.Finished:
			if (BEIGFGCBICO.AICKPAMONBH())
			{
				HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("NegotiationData", "Negotiation data arrived: " + BEIGFGCBICO.DPBLPGKOEJB());
				int num = BEIGFGCBICO.DPBLPGKOEJB().IndexOf("{");
				if (num < 0)
				{
					RaiseOnError("Invalid negotiation text: " + BEIGFGCBICO.DPBLPGKOEJB());
					break;
				}
				NegotiationData jNNJJJOPCKL = Parse(BEIGFGCBICO.DPBLPGKOEJB().Substring(num));
				if (jNNJJJOPCKL == null)
				{
					RaiseOnError("Parsing Negotiation data failed: " + BEIGFGCBICO.DPBLPGKOEJB());
				}
				else if (OnReceived != null)
				{
					OnReceived(this);
					OnReceived = null;
				}
			}
			else
			{
				RaiseOnError(string.Format("Negotiation request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2} Uri: {3}", BEIGFGCBICO.KNMDPGBPNED(), BEIGFGCBICO.DCKPMHKDLEJ(), BEIGFGCBICO.DPBLPGKOEJB(), CGOIOKHEGOE.DKAECMGPGOE()));
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
		HTTPManager.MBBMPNDDPIH().Error("NegotiationData", "Negotiation request failed with error: " + KEPBNIIECPN);
		if (OnError != null)
		{
			OnError(this, KEPBNIIECPN);
			OnError = null;
		}
	}

	private NegotiationData Parse(string IGGFGLLIGCG)
	{
		bool IBFAPIMOMBA = false;
		Dictionary<string, object> dictionary = Json.Decode(IGGFGLLIGCG, ref IBFAPIMOMBA) as Dictionary<string, object>;
		if (!IBFAPIMOMBA)
		{
			return null;
		}
		try
		{
			set_Url(GetString(dictionary, "Url"));
			if (dictionary.ContainsKey("webSocketServerUrl"))
			{
				GCFBKKBNLDI(GetString(dictionary, "webSocketServerUrl"));
			}
			AKFLLIIIJPF(Uri.EscapeDataString(GetString(dictionary, "ConnectionToken")));
			JFHGEPMMEEM(GetString(dictionary, "ConnectionId"));
			if (dictionary.ContainsKey("KeepAliveTimeout"))
			{
				set_KeepAliveTimeout(TimeSpan.FromSeconds(GetDouble(dictionary, "KeepAliveTimeout")));
			}
			OFLBEAPGAPI(TimeSpan.FromSeconds(GetDouble(dictionary, "DisconnectTimeout")));
			OICCAHABLBF(TimeSpan.FromSeconds(GetDouble(dictionary, "ConnectionTimeout")));
			set_TryWebSockets((bool)Get(dictionary, "TryWebSockets"));
			BNEOABLKJDF(GetString(dictionary, "ProtocolVersion"));
			MKEOKCINEBK(TimeSpan.FromSeconds(GetDouble(dictionary, "TransportConnectTimeout")));
			IPMEBEILOEE(TimeSpan.FromSeconds(GetDouble(dictionary, "LongPollDelay")));
			return this;
		}
		catch (Exception mPFFFAOGBJE)
		{
			HTTPManager.MBBMPNDDPIH().COHEDILAHFD("NegotiationData", "Parse", mPFFFAOGBJE);
			return null;
		}
	}

	private static object Get(Dictionary<string, object> IOFHCAAOELD, string KGBGENDIMBC)
	{
		object value;
		if (!IOFHCAAOELD.TryGetValue(KGBGENDIMBC, out value))
		{
			throw new Exception(string.Format("Can't get {0} from Negotiation data!", KGBGENDIMBC));
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

	private static double GetDouble(Dictionary<string, object> IOFHCAAOELD, string KGBGENDIMBC)
	{
		return (double)Get(IOFHCAAOELD, KGBGENDIMBC);
	}
}
