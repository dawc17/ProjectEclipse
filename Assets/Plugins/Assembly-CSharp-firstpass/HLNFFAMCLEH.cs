using System;
using System.Collections.Generic;
using System.Diagnostics;

internal sealed class HLNFFAMCLEH : ITransport
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private FGMEPPMFFKG MKHEFCIEOCA;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private SocketManager JNNOJIEMLEK;

	private HTTPRequest GMEHLGCCBIP;

	private HTTPRequest IBEJDFINCHF;

	private Packet IOPFFNBKCPH;

	public FGMEPPMFFKG AFINHOBCHMC
	{
		get
		{
			return FLBBFDNHJAJ();
		}
		private set
		{
			set_State(value);
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

	public bool KNCJBAHIAGI
	{
		get
		{
			return LILBDKKEHCE();
		}
	}

	public HLNFFAMCLEH(SocketManager BJGMPDIKEJC)
	{
		CMOJGLBBCKC(BJGMPDIKEJC);
	}

	public FGMEPPMFFKG FLBBFDNHJAJ()
	{
		return MKHEFCIEOCA;
	}

	private void set_State(FGMEPPMFFKG value)
	{
		MKHEFCIEOCA = value;
	}

	public SocketManager HLBNHJADOMP()
	{
		return JNNOJIEMLEK;
	}

	private void CMOJGLBBCKC(SocketManager value)
	{
		JNNOJIEMLEK = value;
	}

	public bool LILBDKKEHCE()
	{
		return GMEHLGCCBIP != null;
	}

	public void LAJCMNNNIIM()
	{
		object[] obj = new object[6]
		{
			HLBNHJADOMP().OJBDMGBGJMA().ToString(),
			4,
			HLBNHJADOMP().GPEEDKOHFIG().ToString(),
			null,
			null,
			null
		};
		SocketManager mFANOMMMCFG = HLBNHJADOMP();
		ulong num;
		mFANOMMMCFG.set_RequestCounter((num = mFANOMMMCFG.EKBGNBPGFNG()) + 1);
		num = num;
		obj[3] = num.ToString();
		obj[4] = HLBNHJADOMP().EIOHJJFBIAL().EDLOIOOBPAJ();
		obj[5] = (HLBNHJADOMP().HLHJJJGJEEL().DKJAFHAOKDB() ? string.Empty : HLBNHJADOMP().HLHJJJGJEEL().LEKAOBKGMPF());
		HTTPRequest iPLGNIDJDCF = new HTTPRequest(new Uri(string.Format("{0}?EIO={1}&transport=polling&t={2}-{3}&sid={4}{5}&b64=true", obj)), GCGGFEIEJBN);
		iPLGNIDJDCF.JJCLPAGJEBJ(true);
		iPLGNIDJDCF.LADBBAMKLPJ(true);
		iPLGNIDJDCF.Send();
		set_State(FGMEPPMFFKG.Opening);
	}

	public void Close()
	{
		if (FLBBFDNHJAJ() != FGMEPPMFFKG.Closed)
		{
			set_State(FGMEPPMFFKG.Closed);
		}
	}

	public void Send(Packet NPKADBPBKIG)
	{
		Send(new List<Packet> { NPKADBPBKIG });
	}

	public void Send(List<Packet> DPGGBKDLDJE)
	{
		if (FLBBFDNHJAJ() != FGMEPPMFFKG.Open)
		{
			throw new Exception("Transport is not in Open state!");
		}
		if (LILBDKKEHCE())
		{
			throw new Exception("Sending packets are still in progress!");
		}
		byte[] array = null;
		try
		{
			array = DPGGBKDLDJE[0].KOLJHOEKHLI();
			for (int i = 1; i < DPGGBKDLDJE.Count; i++)
			{
				byte[] array2 = DPGGBKDLDJE[i].KOLJHOEKHLI();
				Array.Resize(ref array, array.Length + array2.Length);
				Array.Copy(array2, 0, array, array.Length - array2.Length, array2.Length);
			}
			DPGGBKDLDJE.Clear();
		}
		catch (Exception ex)
		{
			((IManager)HLBNHJADOMP()).EmitError(CCCOMMIFIMB.Internal, ex.Message + " " + ex.StackTrace);
			return;
		}
		object[] obj = new object[6]
		{
			HLBNHJADOMP().OJBDMGBGJMA().ToString(),
			4,
			HLBNHJADOMP().GPEEDKOHFIG().ToString(),
			null,
			null,
			null
		};
		SocketManager mFANOMMMCFG = HLBNHJADOMP();
		ulong num;
		mFANOMMMCFG.set_RequestCounter((num = mFANOMMMCFG.EKBGNBPGFNG()) + 1);
		num = num;
		obj[3] = num.ToString();
		obj[4] = HLBNHJADOMP().EIOHJJFBIAL().EDLOIOOBPAJ();
		obj[5] = (HLBNHJADOMP().HLHJJJGJEEL().DKJAFHAOKDB() ? string.Empty : HLBNHJADOMP().HLHJJJGJEEL().LEKAOBKGMPF());
		GMEHLGCCBIP = new HTTPRequest(new Uri(string.Format("{0}?EIO={1}&transport=polling&t={2}-{3}&sid={4}{5}&b64=true", obj)), LAAFHDKKJFL.Post, GCGGFEIEJBN);
		GMEHLGCCBIP.JJCLPAGJEBJ(true);
		GMEHLGCCBIP.MMPFBNNMGED("Content-Type", "application/octet-stream");
		GMEHLGCCBIP.set_RawData(array);
		GMEHLGCCBIP.Send();
	}

	private void GCGGFEIEJBN(HTTPRequest CGOIOKHEGOE, HTTPResponse BEIGFGCBICO)
	{
		GMEHLGCCBIP = null;
		if (FLBBFDNHJAJ() == FGMEPPMFFKG.Closed)
		{
			return;
		}
		string text = null;
		switch (CGOIOKHEGOE.FLBBFDNHJAJ())
		{
		case CFGBMHKCENK.Finished:
			if (HTTPManager.MBBMPNDDPIH().PINDEKDNCNL() <= BFNKPHDJNII.All)
			{
				HTTPManager.MBBMPNDDPIH().JMHHKELODIO("PollingTransport", "OnRequestFinished: " + BEIGFGCBICO.DPBLPGKOEJB());
			}
			if (BEIGFGCBICO.AICKPAMONBH())
			{
				ParseResponse(BEIGFGCBICO);
				break;
			}
			text = string.Format("Polling - Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2} Uri: {3}", BEIGFGCBICO.KNMDPGBPNED(), BEIGFGCBICO.DCKPMHKDLEJ(), BEIGFGCBICO.DPBLPGKOEJB(), CGOIOKHEGOE.DKAECMGPGOE());
			break;
		case CFGBMHKCENK.Error:
			text = ((CGOIOKHEGOE.IEFGFKFHNMD() == null) ? "No Exception" : (CGOIOKHEGOE.IEFGFKFHNMD().Message + "\n" + CGOIOKHEGOE.IEFGFKFHNMD().StackTrace));
			break;
		case CFGBMHKCENK.Aborted:
			text = string.Format("Polling - Request({0}) Aborted!", CGOIOKHEGOE.DKAECMGPGOE());
			break;
		case CFGBMHKCENK.ConnectionTimedOut:
			text = string.Format("Polling - Connection Timed Out! Uri: {0}", CGOIOKHEGOE.DKAECMGPGOE());
			break;
		case CFGBMHKCENK.TimedOut:
			text = string.Format("Polling - Processing the request({0}) Timed Out!", CGOIOKHEGOE.DKAECMGPGOE());
			break;
		}
		if (!string.IsNullOrEmpty(text))
		{
			((IManager)HLBNHJADOMP()).OnTransportError((ITransport)this, text);
		}
	}

	public void GNGIDEJLNCF()
	{
		if (IBEJDFINCHF == null && FLBBFDNHJAJ() != FGMEPPMFFKG.Paused)
		{
			object[] obj = new object[6]
			{
				HLBNHJADOMP().OJBDMGBGJMA().ToString(),
				4,
				HLBNHJADOMP().GPEEDKOHFIG().ToString(),
				null,
				null,
				null
			};
			SocketManager mFANOMMMCFG = HLBNHJADOMP();
			ulong num;
			mFANOMMMCFG.set_RequestCounter((num = mFANOMMMCFG.EKBGNBPGFNG()) + 1);
			num = num;
			obj[3] = num.ToString();
			obj[4] = HLBNHJADOMP().EIOHJJFBIAL().EDLOIOOBPAJ();
			obj[5] = (HLBNHJADOMP().HLHJJJGJEEL().DKJAFHAOKDB() ? string.Empty : HLBNHJADOMP().HLHJJJGJEEL().LEKAOBKGMPF());
			IBEJDFINCHF = new HTTPRequest(new Uri(string.Format("{0}?EIO={1}&transport=polling&t={2}-{3}&sid={4}{5}&b64=true", obj)), LAAFHDKKJFL.Get, BMOLBAJPGDJ);
			IBEJDFINCHF.JJCLPAGJEBJ(true);
			IBEJDFINCHF.LADBBAMKLPJ(true);
			IBEJDFINCHF.Send();
		}
	}

	private void BMOLBAJPGDJ(HTTPRequest CGOIOKHEGOE, HTTPResponse BEIGFGCBICO)
	{
		IBEJDFINCHF = null;
		if (FLBBFDNHJAJ() == FGMEPPMFFKG.Closed)
		{
			return;
		}
		string text = null;
		switch (CGOIOKHEGOE.FLBBFDNHJAJ())
		{
		case CFGBMHKCENK.Finished:
			if (HTTPManager.MBBMPNDDPIH().PINDEKDNCNL() <= BFNKPHDJNII.All)
			{
				HTTPManager.MBBMPNDDPIH().JMHHKELODIO("PollingTransport", "OnPollRequestFinished: " + BEIGFGCBICO.DPBLPGKOEJB());
			}
			if (BEIGFGCBICO.AICKPAMONBH())
			{
				ParseResponse(BEIGFGCBICO);
				break;
			}
			text = string.Format("Polling - Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2} Uri: {3}", BEIGFGCBICO.KNMDPGBPNED(), BEIGFGCBICO.DCKPMHKDLEJ(), BEIGFGCBICO.DPBLPGKOEJB(), CGOIOKHEGOE.DKAECMGPGOE());
			break;
		case CFGBMHKCENK.Error:
			text = ((CGOIOKHEGOE.IEFGFKFHNMD() == null) ? "No Exception" : (CGOIOKHEGOE.IEFGFKFHNMD().Message + "\n" + CGOIOKHEGOE.IEFGFKFHNMD().StackTrace));
			break;
		case CFGBMHKCENK.Aborted:
			text = string.Format("Polling - Request({0}) Aborted!", CGOIOKHEGOE.DKAECMGPGOE());
			break;
		case CFGBMHKCENK.ConnectionTimedOut:
			text = string.Format("Polling - Connection Timed Out! Uri: {0}", CGOIOKHEGOE.DKAECMGPGOE());
			break;
		case CFGBMHKCENK.TimedOut:
			text = string.Format("Polling - Processing the request({0}) Timed Out!", CGOIOKHEGOE.DKAECMGPGOE());
			break;
		}
		if (!string.IsNullOrEmpty(text))
		{
			((IManager)HLBNHJADOMP()).OnTransportError((ITransport)this, text);
		}
	}

	private void OnPacket(Packet NPKADBPBKIG)
	{
		if (NPKADBPBKIG.AIHPGOGLBCE() != 0 && !NPKADBPBKIG.DGDMLLFEAME())
		{
			IOPFFNBKCPH = NPKADBPBKIG;
			return;
		}
		HJDLGPHLPNF hJDLGPHLPNF = NPKADBPBKIG.FFJBNPEOAHI();
		if (hJDLGPHLPNF == HJDLGPHLPNF.Message && NPKADBPBKIG.CMEHGNCCCIN() == ECDAJBEFCAH.Connect && FLBBFDNHJAJ() == FGMEPPMFFKG.Opening)
		{
			set_State(FGMEPPMFFKG.Open);
			if (!((IManager)HLBNHJADOMP()).OnTransportConnected((ITransport)this))
			{
				return;
			}
		}
		((IManager)HLBNHJADOMP()).OnPacket(NPKADBPBKIG);
	}

	private void ParseResponse(HTTPResponse BEIGFGCBICO)
	{
		try
		{
			if (BEIGFGCBICO == null || BEIGFGCBICO.CHIGLEKCFFN() == null || BEIGFGCBICO.CHIGLEKCFFN().Length < 1)
			{
				return;
			}
			string text = BEIGFGCBICO.DPBLPGKOEJB();
			if (text == "ok")
			{
				return;
			}
			int num = text.IndexOf(':', 0);
			int num2 = 0;
			while (num >= 0 && num < text.Length)
			{
				int num3 = int.Parse(text.Substring(num2, num - num2));
				string text2 = text.Substring(++num, num3);
				if (text2.Length > 2 && text2[0] == 'b' && text2[1] == '4')
				{
					byte[] jGMLAFOPBBC = Convert.FromBase64String(text2.Substring(2));
					if (IOPFFNBKCPH != null)
					{
						IOPFFNBKCPH.AddAttachmentFromServer(jGMLAFOPBBC, true);
						if (IOPFFNBKCPH.DGDMLLFEAME())
						{
							try
							{
								OnPacket(IOPFFNBKCPH);
							}
							catch (Exception ex)
							{
								HTTPManager.MBBMPNDDPIH().COHEDILAHFD("PollingTransport", "ParseResponse - OnPacket with attachment", ex);
								((IManager)HLBNHJADOMP()).EmitError(CCCOMMIFIMB.Internal, ex.Message + " " + ex.StackTrace);
							}
							finally
							{
								IOPFFNBKCPH = null;
							}
						}
					}
				}
				else
				{
					try
					{
						Packet nPKADBPBKIG = new Packet(text2);
						OnPacket(nPKADBPBKIG);
					}
					catch (Exception ex2)
					{
						HTTPManager.MBBMPNDDPIH().COHEDILAHFD("PollingTransport", "ParseResponse - OnPacket", ex2);
						((IManager)HLBNHJADOMP()).EmitError(CCCOMMIFIMB.Internal, ex2.Message + " " + ex2.StackTrace);
					}
				}
				num2 = num + num3;
				num = text.IndexOf(':', num2);
			}
		}
		catch (Exception ex3)
		{
			((IManager)HLBNHJADOMP()).EmitError(CCCOMMIFIMB.Internal, ex3.Message + " " + ex3.StackTrace);
			HTTPManager.MBBMPNDDPIH().COHEDILAHFD("PollingTransport", "ParseResponse", ex3);
		}
	}
}
