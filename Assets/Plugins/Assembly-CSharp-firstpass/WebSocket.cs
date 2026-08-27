using System;
using System.Diagnostics;

public sealed class WebSocket
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private HTTPRequest HMKDGNFLBMB;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool HKJBEBPDOOK;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int JICCALEELHL;

	public BNIEFDKHAJN HKBKFMIBCED;

	public KCEBOGOANEH OnMessage;

	public OnWebSocketBinaryDelegate OnBinary;

	public OnWebSocketClosedDelegate OnClosed;

	public OnWebSocketErrorDelegate OnError;

	public JFCEKCIHELB NKNFDKGPPAJ;

	public PDFHPHPODBK GJADNPIKFEL;

	private bool PAMNPBEHNCF;

	private WebSocketResponse ILNFPNFEOCL;

	public HTTPRequest OOHLFJNPGGA
	{
		get
		{
			return KGBEGJJPCKC();
		}
		private set
		{
			HMPIGPEAMPM(value);
		}
	}

	public bool PLCIGHLBOPP
	{
		get
		{
			return DJKKJPNLOAE();
		}
	}

	public bool HDEBMDAFLNC
	{
		get
		{
			return ALNKFAEMIBD();
		}
		set
		{
			set_StartPingThread(value);
		}
	}

	public int CINAOLMJCEJ
	{
		get
		{
			return IGDLFKDLPDK();
		}
		set
		{
			set_PingFrequency(value);
		}
	}

	public WebSocket(Uri KJHNCLAJMLO)
		: this(KJHNCLAJMLO, string.Empty, string.Empty)
	{
	}

	public WebSocket(Uri KJHNCLAJMLO, string IKOOJMAOFOD, string ENLHAIGCCBO = "")
	{
		set_PingFrequency(1000);
		if (KJHNCLAJMLO.Port == -1)
		{
			KJHNCLAJMLO = new Uri(KJHNCLAJMLO.Scheme + "://" + KJHNCLAJMLO.Host + ":" + ((!KJHNCLAJMLO.Scheme.Equals("wss", StringComparison.OrdinalIgnoreCase)) ? "80" : "443") + KJHNCLAJMLO.PathAndQuery);
		}
		HMPIGPEAMPM(new HTTPRequest(KJHNCLAJMLO, DLHNDCMFPCJ));
		KGBEGJJPCKC().GFFABFBMJAO = FFCPMNOMFPA;
		KGBEGJJPCKC().MMPFBNNMGED("Host", KJHNCLAJMLO.Host + ":" + KJHNCLAJMLO.Port);
		KGBEGJJPCKC().MMPFBNNMGED("Upgrade", "websocket");
		KGBEGJJPCKC().MMPFBNNMGED("Connection", "keep-alive, Upgrade");
		KGBEGJJPCKC().MMPFBNNMGED("Sec-WebSocket-Key", GetSecKey(new object[4]
		{
			this,
			KGBEGJJPCKC(),
			KJHNCLAJMLO,
			new object()
		}));
		if (!string.IsNullOrEmpty(IKOOJMAOFOD))
		{
			KGBEGJJPCKC().MMPFBNNMGED("Origin", IKOOJMAOFOD);
		}
		KGBEGJJPCKC().MMPFBNNMGED("Sec-WebSocket-Version", "13");
		if (!string.IsNullOrEmpty(ENLHAIGCCBO))
		{
			KGBEGJJPCKC().MMPFBNNMGED("Sec-WebSocket-Protocol", ENLHAIGCCBO);
		}
		KGBEGJJPCKC().MMPFBNNMGED("Cache-Control", "no-cache");
		KGBEGJJPCKC().MMPFBNNMGED("Pragma", "no-cache");
		KGBEGJJPCKC().JJCLPAGJEBJ(true);
		if (HTTPManager.FHGBKFBCGCO() != null)
		{
			KGBEGJJPCKC().PNGMAECJHID(new HTTPProxy(HTTPManager.FHGBKFBCGCO().DNIJHGFINDG(), HTTPManager.FHGBKFBCGCO().HPKPFEOBIOC(), false, false, HTTPManager.FHGBKFBCGCO().OHCGKBPPMEN()));
		}
	}

	public HTTPRequest KGBEGJJPCKC()
	{
		return HMKDGNFLBMB;
	}

	private void HMPIGPEAMPM(HTTPRequest value)
	{
		HMKDGNFLBMB = value;
	}

	public bool DJKKJPNLOAE()
	{
		return ILNFPNFEOCL != null && !ILNFPNFEOCL.HDDABMLNDPK();
	}

	public bool ALNKFAEMIBD()
	{
		return HKJBEBPDOOK;
	}

	public void set_StartPingThread(bool value)
	{
		HKJBEBPDOOK = value;
	}

	public int IGDLFKDLPDK()
	{
		return JICCALEELHL;
	}

	public void set_PingFrequency(int value)
	{
		JICCALEELHL = value;
	}

	private void DLHNDCMFPCJ(HTTPRequest CGOIOKHEGOE, HTTPResponse BEIGFGCBICO)
	{
		string empty = string.Empty;
		switch (CGOIOKHEGOE.FLBBFDNHJAJ())
		{
		default:
			return;
		case CFGBMHKCENK.Finished:
			if (BEIGFGCBICO.AICKPAMONBH() || BEIGFGCBICO.KNMDPGBPNED() == 101)
			{
				HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("WebSocket", string.Format("Request finished. Status Code: {0} Message: {1}", BEIGFGCBICO.KNMDPGBPNED().ToString(), BEIGFGCBICO.DCKPMHKDLEJ()));
				return;
			}
			empty = string.Format("Request Finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}", BEIGFGCBICO.KNMDPGBPNED(), BEIGFGCBICO.DCKPMHKDLEJ(), BEIGFGCBICO.DPBLPGKOEJB());
			break;
		case CFGBMHKCENK.Error:
			empty = "Request Finished with Error! " + ((CGOIOKHEGOE.IEFGFKFHNMD() == null) ? string.Empty : ("Exception: " + CGOIOKHEGOE.IEFGFKFHNMD().Message + CGOIOKHEGOE.IEFGFKFHNMD().StackTrace));
			break;
		case CFGBMHKCENK.Aborted:
			empty = "Request Aborted!";
			break;
		case CFGBMHKCENK.ConnectionTimedOut:
			empty = "Connection Timed Out!";
			break;
		case CFGBMHKCENK.TimedOut:
			empty = "Processing the request Timed Out!";
			break;
		}
		if (OnError != null)
		{
			OnError(this, CGOIOKHEGOE.IEFGFKFHNMD());
		}
		if (NKNFDKGPPAJ != null)
		{
			NKNFDKGPPAJ(this, empty);
		}
		if (OnError == null && NKNFDKGPPAJ == null)
		{
			HTTPManager.MBBMPNDDPIH().Error("WebSocket", empty);
		}
	}

	private void FFCPMNOMFPA(HTTPRequest CGOIOKHEGOE, HTTPResponse BEIGFGCBICO)
	{
		ILNFPNFEOCL = BEIGFGCBICO as WebSocketResponse;
		if (ILNFPNFEOCL == null)
		{
			if (OnError != null)
			{
				OnError(this, CGOIOKHEGOE.IEFGFKFHNMD());
			}
			if (NKNFDKGPPAJ != null)
			{
				string nEPOLDCKNJL = string.Empty;
				if (CGOIOKHEGOE.IEFGFKFHNMD() != null)
				{
					nEPOLDCKNJL = CGOIOKHEGOE.IEFGFKFHNMD().Message + " " + CGOIOKHEGOE.IEFGFKFHNMD().StackTrace;
				}
				NKNFDKGPPAJ(this, nEPOLDCKNJL);
			}
			return;
		}
		if (HKBKFMIBCED != null)
		{
			try
			{
				HKBKFMIBCED(this);
			}
			catch (Exception mPFFFAOGBJE)
			{
				HTTPManager.MBBMPNDDPIH().COHEDILAHFD("WebSocket", "OnOpen", mPFFFAOGBJE);
			}
		}
		ILNFPNFEOCL.OnText = (WebSocketResponse IIBIPJJLEGJ, string CKEHOEGLMBM) =>
		{
			if (OnMessage != null)
			{
				OnMessage(this, CKEHOEGLMBM);
			}
		};
		ILNFPNFEOCL.OnBinary = (WebSocketResponse IIBIPJJLEGJ, byte[] DOEJIOEKACH) =>
		{
			if (OnBinary != null)
			{
				OnBinary(this, DOEJIOEKACH);
			}
		};
		ILNFPNFEOCL.OnClosed = (WebSocketResponse IIBIPJJLEGJ, ushort KJPGKHJNOMC, string CKEHOEGLMBM) =>
		{
			if (OnClosed != null)
			{
				OnClosed(this, KJPGKHJNOMC, CKEHOEGLMBM);
			}
		};
		if (GJADNPIKFEL != null)
		{
			ILNFPNFEOCL.GJADNPIKFEL = (WebSocketResponse IIBIPJJLEGJ, WebSocketFrameReader frame) =>
			{
				if (GJADNPIKFEL != null)
				{
					GJADNPIKFEL(this, frame);
				}
			};
		}
		if (ALNKFAEMIBD())
		{
			ILNFPNFEOCL.StartPinging(Math.Min(IGDLFKDLPDK(), 100));
		}
		ILNFPNFEOCL.PBAFKNHCJHD();
	}

	public void LAJCMNNNIIM()
	{
		if (!PAMNPBEHNCF && KGBEGJJPCKC() != null)
		{
			KGBEGJJPCKC().Send();
			PAMNPBEHNCF = true;
		}
	}

	public void Send(string LIOGIBJBHAH)
	{
		if (DJKKJPNLOAE())
		{
			ILNFPNFEOCL.Send(LIOGIBJBHAH);
		}
	}

	public void Send(byte[] buffer)
	{
		if (DJKKJPNLOAE())
		{
			ILNFPNFEOCL.Send(buffer);
		}
	}

	public void Send(byte[] buffer, ulong IPCOBJBKNAO, ulong count)
	{
		if (DJKKJPNLOAE())
		{
			ILNFPNFEOCL.Send(buffer, IPCOBJBKNAO, count);
		}
	}

	public void Send(IWebSocketFrameWriter frame)
	{
		if (DJKKJPNLOAE())
		{
			ILNFPNFEOCL.Send(frame);
		}
	}

	public void Close()
	{
		if (DJKKJPNLOAE())
		{
			ILNFPNFEOCL.Close();
		}
	}

	public void Close(ushort KJPGKHJNOMC, string LIOGIBJBHAH)
	{
		if (DJKKJPNLOAE())
		{
			ILNFPNFEOCL.Close(KJPGKHJNOMC, LIOGIBJBHAH);
		}
	}

	private string GetSecKey(object[] IOFHCAAOELD)
	{
		byte[] array = new byte[16];
		int num = 0;
		for (int i = 0; i < IOFHCAAOELD.Length; i++)
		{
			byte[] bytes = BitConverter.GetBytes(IOFHCAAOELD[i].GetHashCode());
			for (int j = 0; j < bytes.Length; j++)
			{
				if (num >= array.Length)
				{
					break;
				}
				array[num++] = bytes[j];
			}
		}
		return Convert.ToBase64String(array);
	}
}
