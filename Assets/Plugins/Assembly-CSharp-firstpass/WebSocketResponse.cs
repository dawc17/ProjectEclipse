using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

public sealed class WebSocketResponse : HTTPResponse, IHeartbeat, IProtocol
{
	public Action<WebSocketResponse, string> OnText;

	public Action<WebSocketResponse, byte[]> OnBinary;

	public Action<WebSocketResponse, WebSocketFrameReader> GJADNPIKFEL;

	public Action<WebSocketResponse, ushort, string> OnClosed;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private TimeSpan HEDIFJHLGPB;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private ushort OILAMPHCBBD;

	private List<WebSocketFrameReader> OCKKDCBJLAK = new List<WebSocketFrameReader>();

	private List<WebSocketFrameReader> IDBIIDBEJMF = new List<WebSocketFrameReader>();

	private WebSocketFrameReader HHFKHCLMIPO;

	private System.Threading.Thread ReceiverThread;

	private object FrameLock = new object();

	private object AINBLIFIDOL = new object();

	private bool MPDJPMMICIK;

	private bool BHJMGNNGEPC;

	private DateTime lastPing = DateTime.MinValue;

	public bool BILHEJLBKMF
	{
		get
		{
			return HDDABMLNDPK();
		}
	}

	public TimeSpan OJIEBBAHBII
	{
		get
		{
			return BKCALOLNDNA();
		}
		private set
		{
			set_PingFrequnecy(value);
		}
	}

	public ushort PMDGMFOEDNA
	{
		get
		{
			return DCLJBIMGBOE();
		}
		private set
		{
			set_MaxFragmentSize(value);
		}
	}

	internal WebSocketResponse(HTTPRequest ONOCIELLAPL, Stream ABJIEFMMIEK, bool IBIIADCLKCH, bool PEAJIKCANHP)
		: base(ONOCIELLAPL, ABJIEFMMIEK, IBIIADCLKCH, PEAJIKCANHP)
	{
		DFIAKBONHGB(true);
		BHJMGNNGEPC = false;
		set_MaxFragmentSize(32767);
	}

	public bool HDDABMLNDPK()
	{
		return BHJMGNNGEPC;
	}

	public TimeSpan BKCALOLNDNA()
	{
		return HEDIFJHLGPB;
	}

	private void set_PingFrequnecy(TimeSpan value)
	{
		HEDIFJHLGPB = value;
	}

	public ushort DCLJBIMGBOE()
	{
		return OILAMPHCBBD;
	}

	private void set_MaxFragmentSize(ushort value)
	{
		OILAMPHCBBD = value;
	}

	internal void PBAFKNHCJHD()
	{
		if (ODOHODEENIB())
		{
			ReceiverThread = new System.Threading.Thread(PCFDLMGIEKG);
			ReceiverThread.Name = "WebSocket Receiver Thread";
			ReceiverThread.IsBackground = true;
			ReceiverThread.Start();
		}
	}

	public void Send(string LIOGIBJBHAH)
	{
		if (LIOGIBJBHAH == null)
		{
			throw new ArgumentNullException("message must not be null!");
		}
		Send(new AEGCCCNBCML(LIOGIBJBHAH));
	}

	public void Send(byte[] data)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data must not be null!");
		}
		if ((long)data.Length > (long)(int)DCLJBIMGBOE())
		{
			lock (AINBLIFIDOL)
			{
				Send(new WebSocketBinaryFrame(data, 0uL, DCLJBIMGBOE(), false));
				ulong num2;
				for (ulong num = DCLJBIMGBOE(); num < (ulong)data.Length; num += num2)
				{
					num2 = Math.Min(DCLJBIMGBOE(), (ulong)data.Length - num);
					Send(new WebSocketContinuationFrame(data, num, num2, num + num2 >= (ulong)data.Length));
				}
				return;
			}
		}
		Send(new WebSocketBinaryFrame(data));
	}

	public void Send(byte[] data, ulong IPCOBJBKNAO, ulong count)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data must not be null!");
		}
		if (IPCOBJBKNAO + count > (ulong)data.Length)
		{
			throw new ArgumentOutOfRangeException("offset + count >= data.Length");
		}
		if ((long)count > (long)(int)DCLJBIMGBOE())
		{
			lock (AINBLIFIDOL)
			{
				Send(new WebSocketBinaryFrame(data, IPCOBJBKNAO, DCLJBIMGBOE(), false));
				ulong num2;
				for (ulong num = IPCOBJBKNAO + DCLJBIMGBOE(); num < count; num += num2)
				{
					num2 = Math.Min(DCLJBIMGBOE(), count - num);
					Send(new WebSocketContinuationFrame(data, num, num2, num + num2 >= count));
				}
				return;
			}
		}
		Send(new WebSocketBinaryFrame(data, IPCOBJBKNAO, count, true));
	}

	public void Send(IWebSocketFrameWriter frame)
	{
		if (frame == null)
		{
			throw new ArgumentNullException("frame is null!");
		}
		if (!BHJMGNNGEPC)
		{
			byte[] array = frame.Get();
			lock (AINBLIFIDOL)
			{
				Stream.Write(array, 0, array.Length);
				Stream.Flush();
			}
			if (frame.get_Type() == BECKAHJIEGE.ConnectionClose)
			{
				MPDJPMMICIK = true;
			}
		}
	}

	public void Close()
	{
		Close(1000, "Bye!");
	}

	public void Close(ushort KJPGKHJNOMC, string CKEHOEGLMBM)
	{
		if (!BHJMGNNGEPC)
		{
			Send(new WebSocketClose(KJPGKHJNOMC, CKEHOEGLMBM));
		}
	}

	public void StartPinging(int ONDDDDCAPFG)
	{
		if (ONDDDDCAPFG < 100)
		{
			throw new ArgumentException("frequency must be at least 100 millisec!");
		}
		set_PingFrequnecy(TimeSpan.FromMilliseconds(ONDDDDCAPFG));
		HTTPManager.MAMNLAJACOD().ELAHFBCGAGL(this);
	}

	private void PCFDLMGIEKG()
	{
		try
		{
			while (!BHJMGNNGEPC)
			{
				try
				{
					WebSocketFrameReader hENOIJFGGOF = new WebSocketFrameReader();
					hENOIJFGGOF.Read(Stream);
					if (hENOIJFGGOF.FIDNGEELBPG())
					{
						Close(1002, "Protocol Error: masked frame received from server!");
						continue;
					}
					if (!hENOIJFGGOF.MOOCLIBIPBI())
					{
						if (GJADNPIKFEL == null)
						{
							OCKKDCBJLAK.Add(hENOIJFGGOF);
							continue;
						}
						lock (FrameLock)
						{
							IDBIIDBEJMF.Add(hENOIJFGGOF);
						}
						continue;
					}
					switch (hENOIJFGGOF.get_Type())
					{
					case BECKAHJIEGE.Continuation:
						if (GJADNPIKFEL == null)
						{
							hENOIJFGGOF.Assemble(OCKKDCBJLAK);
							OCKKDCBJLAK.Clear();
							goto case BECKAHJIEGE.Text;
						}
						lock (FrameLock)
						{
							IDBIIDBEJMF.Add(hENOIJFGGOF);
						}
						break;
					case BECKAHJIEGE.Text:
					case BECKAHJIEGE.Binary:
						lock (FrameLock)
						{
							IDBIIDBEJMF.Add(hENOIJFGGOF);
						}
						break;
					case BECKAHJIEGE.Ping:
						if (!MPDJPMMICIK && !BHJMGNNGEPC)
						{
							Send(new WebSocketPong(hENOIJFGGOF));
						}
						break;
					case BECKAHJIEGE.ConnectionClose:
						HHFKHCLMIPO = hENOIJFGGOF;
						if (!MPDJPMMICIK)
						{
							Send(new WebSocketClose());
						}
						BHJMGNNGEPC = MPDJPMMICIK;
						break;
					}
				}
				catch (ThreadAbortException)
				{
					OCKKDCBJLAK.Clear();
					KEEGKCNNPGM.set_State(CFGBMHKCENK.Aborted);
					BHJMGNNGEPC = true;
				}
				catch (Exception bAINMLLIKOL)
				{
					KEEGKCNNPGM.set_Exception(bAINMLLIKOL);
					KEEGKCNNPGM.set_State(CFGBMHKCENK.Error);
					BHJMGNNGEPC = true;
				}
			}
		}
		finally
		{
			HTTPManager.MAMNLAJACOD().HKMBDKKHPCB(this);
		}
	}

	void IProtocol.HandleEvents()
	{
		lock (FrameLock)
		{
			for (int i = 0; i < IDBIIDBEJMF.Count; i++)
			{
				WebSocketFrameReader hENOIJFGGOF = IDBIIDBEJMF[i];
				try
				{
					BECKAHJIEGE bECKAHJIEGE = hENOIJFGGOF.get_Type();
					if (bECKAHJIEGE == BECKAHJIEGE.Continuation)
					{
						goto IL_0041;
					}
					if (bECKAHJIEGE != BECKAHJIEGE.Text)
					{
						if (bECKAHJIEGE == BECKAHJIEGE.Binary)
						{
							if (!hENOIJFGGOF.MOOCLIBIPBI())
							{
								goto IL_0041;
							}
							if (OnBinary != null)
							{
								OnBinary(this, hENOIJFGGOF.CHIGLEKCFFN());
							}
						}
					}
					else
					{
						if (!hENOIJFGGOF.MOOCLIBIPBI())
						{
							goto IL_0041;
						}
						if (OnText != null)
						{
							OnText(this, Encoding.UTF8.GetString(hENOIJFGGOF.CHIGLEKCFFN(), 0, hENOIJFGGOF.CHIGLEKCFFN().Length));
						}
					}
					goto end_IL_0021;
					IL_0041:
					if (GJADNPIKFEL != null)
					{
						GJADNPIKFEL(this, hENOIJFGGOF);
					}
					end_IL_0021:;
				}
				catch (Exception mPFFFAOGBJE)
				{
					HTTPManager.MBBMPNDDPIH().COHEDILAHFD("WebSocketResponse", "HandleEvents", mPFFFAOGBJE);
				}
			}
			IDBIIDBEJMF.Clear();
		}
		if (!HDDABMLNDPK() || OnClosed == null || KEEGKCNNPGM.FLBBFDNHJAJ() != CFGBMHKCENK.Processing)
		{
			return;
		}
		try
		{
			ushort arg = 0;
			string arg2 = string.Empty;
			if (HHFKHCLMIPO != null && HHFKHCLMIPO.CHIGLEKCFFN() != null && HHFKHCLMIPO.CHIGLEKCFFN().Length >= 2)
			{
				if (BitConverter.IsLittleEndian)
				{
					Array.Reverse(HHFKHCLMIPO.CHIGLEKCFFN(), 0, 2);
				}
				arg = BitConverter.ToUInt16(HHFKHCLMIPO.CHIGLEKCFFN(), 0);
				if (HHFKHCLMIPO.CHIGLEKCFFN().Length > 2)
				{
					arg2 = Encoding.UTF8.GetString(HHFKHCLMIPO.CHIGLEKCFFN(), 2, HHFKHCLMIPO.CHIGLEKCFFN().Length - 2);
				}
			}
			OnClosed(this, arg, arg2);
		}
		catch (Exception mPFFFAOGBJE2)
		{
			HTTPManager.MBBMPNDDPIH().COHEDILAHFD("WebSocketResponse", "HandleEvents - OnClosed", mPFFFAOGBJE2);
		}
	}

	void IHeartbeat.OnHeartbeatUpdate(TimeSpan OJOKANCMPLG)
	{
		if (lastPing == DateTime.MinValue)
		{
			lastPing = DateTime.UtcNow;
		}
		else if (DateTime.UtcNow - lastPing >= BKCALOLNDNA())
		{
			Send(new HKALCPMGELL(string.Empty));
			lastPing = DateTime.UtcNow;
		}
	}
}
