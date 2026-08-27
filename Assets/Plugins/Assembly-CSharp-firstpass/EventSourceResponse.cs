using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

internal sealed class EventSourceResponse : HTTPResponse, IProtocol
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool KCCENMDNOOK;

	public Action<EventSourceResponse, Message> OnMessage;

	public Action<EventSourceResponse> OnClosed;

	private System.Threading.Thread ReceiverThread;

	private object FrameLock = new object();

	private byte[] LineBuffer = new byte[1024];

	private int LineBufferPos;

	private Message BCHHNNNFDGI;

	private List<Message> PJLMDEGNKJK = new List<Message>();

	public bool BILHEJLBKMF
	{
		get
		{
			return HDDABMLNDPK();
		}
		private set
		{
			set_IsClosed(value);
		}
	}

	internal EventSourceResponse(HTTPRequest ONOCIELLAPL, Stream ABJIEFMMIEK, bool IBIIADCLKCH, bool PEAJIKCANHP)
		: base(ONOCIELLAPL, ABJIEFMMIEK, IBIIADCLKCH, PEAJIKCANHP)
	{
		DFIAKBONHGB(true);
	}

	public bool HDDABMLNDPK()
	{
		return KCCENMDNOOK;
	}

	private void set_IsClosed(bool value)
	{
		KCCENMDNOOK = value;
	}

	internal override bool Receive(int JHFPNBPNHEH = -1, bool NDCKHEGBAGO = true)
	{
		bool flag = base.Receive(JHFPNBPNHEH, false);
		GCDKHOCDONK(flag && KNMDPGBPNED() == 200 && HasHeaderWithValue("content-type", "text/event-stream"));
		if (!ODOHODEENIB())
		{
			ReadPayload(JHFPNBPNHEH);
		}
		return flag;
	}

	internal void PBAFKNHCJHD()
	{
		if (ODOHODEENIB())
		{
			ReceiverThread = new System.Threading.Thread(PCFDLMGIEKG);
			ReceiverThread.Name = "EventSource Receiver Thread";
			ReceiverThread.IsBackground = true;
			ReceiverThread.Start();
		}
	}

	private void PCFDLMGIEKG()
	{
		try
		{
			if (HasHeaderWithValue("transfer-encoding", "chunked"))
			{
				ReadChunked(Stream);
			}
			else
			{
				ReadRaw(Stream, -1);
			}
		}
		catch (ThreadAbortException)
		{
			KEEGKCNNPGM.set_State(CFGBMHKCENK.Aborted);
		}
		catch (Exception bAINMLLIKOL)
		{
			KEEGKCNNPGM.set_Exception(bAINMLLIKOL);
			KEEGKCNNPGM.set_State(CFGBMHKCENK.Error);
		}
		finally
		{
			set_IsClosed(true);
		}
	}

	private new void ReadChunked(Stream ABJIEFMMIEK)
	{
		int num = ReadChunkLength(ABJIEFMMIEK);
		byte[] array = new byte[num];
		while (num != 0)
		{
			if (array.Length < num)
			{
				Array.Resize(ref array, num);
			}
			int num2 = 0;
			do
			{
				int num3 = ABJIEFMMIEK.Read(array, num2, num - num2);
				if (num3 == 0)
				{
					throw new Exception("The remote server closed the connection unexpectedly!");
				}
				num2 += num3;
			}
			while (num2 < num);
			KMDCCLDLOJL(array, num2);
			HTTPResponse.JJFJFNEFOHK(ABJIEFMMIEK, 10);
			num = ReadChunkLength(ABJIEFMMIEK);
		}
		NEECNIHNFGI(ABJIEFMMIEK);
	}

	private new void ReadRaw(Stream ABJIEFMMIEK, int HDIIBKGCCNB)
	{
		byte[] array = new byte[1024];
		int num;
		do
		{
			num = ABJIEFMMIEK.Read(array, 0, array.Length);
			KMDCCLDLOJL(array, num);
		}
		while (num > 0);
	}

	public void KMDCCLDLOJL(byte[] buffer, int count)
	{
		if (count == -1)
		{
			count = buffer.Length;
		}
		if (count == 0)
		{
			return;
		}
		int num = 0;
		int num2;
		do
		{
			num2 = -1;
			int num3 = 1;
			for (int i = num; i < count; i++)
			{
				if (num2 != -1)
				{
					break;
				}
				if (buffer[i] == 13)
				{
					if (i + 1 < count && buffer[i + 1] == 10)
					{
						num3 = 2;
					}
					num2 = i;
				}
				else if (buffer[i] == 10)
				{
					num2 = i;
				}
			}
			int num4 = ((num2 != -1) ? num2 : count);
			if (LineBuffer.Length < LineBufferPos + (num4 - num))
			{
				Array.Resize(ref LineBuffer, LineBufferPos + (num4 - num));
			}
			Array.Copy(buffer, num, LineBuffer, LineBufferPos, num4 - num);
			LineBufferPos += num4 - num;
			if (num2 == -1)
			{
				break;
			}
			HPDBMAIMJKJ(LineBuffer, LineBufferPos);
			LineBufferPos = 0;
			num += num2 + num3;
		}
		while (num2 != -1 && num < count);
	}

	private void HPDBMAIMJKJ(byte[] buffer, int count)
	{
		if (count == 0)
		{
			if (BCHHNNNFDGI != null)
			{
				lock (FrameLock)
				{
					PJLMDEGNKJK.Add(BCHHNNNFDGI);
				}
				BCHHNNNFDGI = null;
			}
		}
		else
		{
			if (buffer[0] == 58)
			{
				return;
			}
			int num = -1;
			for (int i = 0; i < count; i++)
			{
				if (num != -1)
				{
					break;
				}
				if (buffer[i] == 58)
				{
					num = i;
				}
			}
			string text;
			string text2;
			if (num != -1)
			{
				text = Encoding.UTF8.GetString(buffer, 0, num);
				if (num + 1 < count && buffer[num + 1] == 32)
				{
					num++;
				}
				num++;
				if (num >= count)
				{
					return;
				}
				text2 = Encoding.UTF8.GetString(buffer, num, count - num);
			}
			else
			{
				text = Encoding.UTF8.GetString(buffer, 0, count);
				text2 = string.Empty;
			}
			if (BCHHNNNFDGI == null)
			{
				BCHHNNNFDGI = new Message();
			}
			switch (text)
			{
			case "id":
				BCHHNNNFDGI.MKAMABIPHEN(text2);
				break;
			case "event":
				BCHHNNNFDGI.set_Event(text2);
				break;
			case "data":
			{
				if (BCHHNNNFDGI.CHIGLEKCFFN() != null)
				{
					Message bCHHNNNFDGI = BCHHNNNFDGI;
					bCHHNNNFDGI.set_Data(bCHHNNNFDGI.CHIGLEKCFFN() + Environment.NewLine);
				}
				Message bCHHNNNFDGI2 = BCHHNNNFDGI;
				bCHHNNNFDGI2.set_Data(bCHHNNNFDGI2.CHIGLEKCFFN() + text2);
				break;
			}
			case "retry":
			{
				int result;
				if (int.TryParse(text2, out result))
				{
					BCHHNNNFDGI.set_Retry(TimeSpan.FromMilliseconds(result));
				}
				break;
			}
			}
		}
	}

	void IProtocol.HandleEvents()
	{
		lock (FrameLock)
		{
			if (PJLMDEGNKJK.Count > 0)
			{
				if (OnMessage != null)
				{
					for (int i = 0; i < PJLMDEGNKJK.Count; i++)
					{
						try
						{
							OnMessage(this, PJLMDEGNKJK[i]);
						}
						catch (Exception mPFFFAOGBJE)
						{
							HTTPManager.MBBMPNDDPIH().COHEDILAHFD("EventSourceMessage", "HandleEvents - OnMessage", mPFFFAOGBJE);
						}
					}
				}
				PJLMDEGNKJK.Clear();
			}
		}
		if (!HDDABMLNDPK())
		{
			return;
		}
		PJLMDEGNKJK.Clear();
		if (OnClosed == null)
		{
			return;
		}
		try
		{
			OnClosed(this);
		}
		catch (Exception mPFFFAOGBJE2)
		{
			HTTPManager.MBBMPNDDPIH().COHEDILAHFD("EventSourceMessage", "HandleEvents - OnClosed", mPFFFAOGBJE2);
		}
		finally
		{
			OnClosed = null;
		}
	}
}
