using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

public sealed class SampleCookieAuthentication : IAuthenticationProvider
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Uri HJCGILPDDIG;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string MHIOHGELAGB;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string FDPDIGOJOOG;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string HCBJOGDNOBK;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool IHBBEINDOJN;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private LACLODBGJEI OnAuthenticationSucceded;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private BCHANFGJONF OnAuthenticationFailed;

	private HTTPRequest KHHMDBBJJFO;

	private Cookie LLOHIGOHLMI;

	public Uri JLEHBPMKOCK
	{
		get
		{
			return BDJDPKFCGCH();
		}
		private set
		{
			set_AuthUri(value);
		}
	}

	public string OEDGKJNJCEA
	{
		get
		{
			return BFFCEKDPNAM();
		}
		private set
		{
			IHIOOLDEDBN(value);
		}
	}

	public string LCIENEOINCL
	{
		get
		{
			return LDEFEGOBBGO();
		}
		private set
		{
			EOMDIHIOGDO(value);
		}
	}

	public string NLAFONGMJNL
	{
		get
		{
			return NFJMHJJDDHC();
		}
		private set
		{
			JHPLDAEBAMH(value);
		}
	}

	public bool CBIBNHGGEBI
	{
		get
		{
			return MCHOHLKGMBI();
		}
		private set
		{
			set_IsPreAuthRequired(value);
		}
	}

	public event LACLODBGJEI EFCGDJPAJIG
	{
		add
		{
			IJPBAJDFAED(value);
		}
		remove
		{
			KFGAHIPDDOF(value);
		}
	}

	public event BCHANFGJONF HMIFKIFAFMK
	{
		add
		{
			NEAGLBOCLHI(value);
		}
		remove
		{
			BFANLHDOICD(value);
		}
	}

	public SampleCookieAuthentication(Uri EJLKINNHGHN, string KEJDJHAGBMK, string JMKKKMKEAMI, string NNMKKAKIJCP)
	{
		set_AuthUri(EJLKINNHGHN);
		IHIOOLDEDBN(KEJDJHAGBMK);
		EOMDIHIOGDO(JMKKKMKEAMI);
		JHPLDAEBAMH(NNMKKAKIJCP);
		set_IsPreAuthRequired(true);
	}

	public Uri BDJDPKFCGCH()
	{
		return HJCGILPDDIG;
	}

	private void set_AuthUri(Uri value)
	{
		HJCGILPDDIG = value;
	}

	public string BFFCEKDPNAM()
	{
		return MHIOHGELAGB;
	}

	private void IHIOOLDEDBN(string value)
	{
		MHIOHGELAGB = value;
	}

	public string LDEFEGOBBGO()
	{
		return FDPDIGOJOOG;
	}

	private void EOMDIHIOGDO(string value)
	{
		FDPDIGOJOOG = value;
	}

	public string NFJMHJJDDHC()
	{
		return HCBJOGDNOBK;
	}

	private void JHPLDAEBAMH(string value)
	{
		HCBJOGDNOBK = value;
	}

	public bool MCHOHLKGMBI()
	{
		return IHBBEINDOJN;
	}

	private void set_IsPreAuthRequired(bool value)
	{
		IHBBEINDOJN = value;
	}

	public void IJPBAJDFAED(LACLODBGJEI value)
	{
		LACLODBGJEI lACLODBGJEI = OnAuthenticationSucceded;
		LACLODBGJEI lACLODBGJEI2;
		do
		{
			lACLODBGJEI2 = lACLODBGJEI;
			lACLODBGJEI = Interlocked.CompareExchange(ref OnAuthenticationSucceded, (LACLODBGJEI)Delegate.Combine(lACLODBGJEI2, value), lACLODBGJEI);
		}
		while ((object)lACLODBGJEI != lACLODBGJEI2);
	}

	public void KFGAHIPDDOF(LACLODBGJEI value)
	{
		LACLODBGJEI lACLODBGJEI = OnAuthenticationSucceded;
		LACLODBGJEI lACLODBGJEI2;
		do
		{
			lACLODBGJEI2 = lACLODBGJEI;
			lACLODBGJEI = Interlocked.CompareExchange(ref OnAuthenticationSucceded, (LACLODBGJEI)Delegate.Remove(lACLODBGJEI2, value), lACLODBGJEI);
		}
		while ((object)lACLODBGJEI != lACLODBGJEI2);
	}

	public void NEAGLBOCLHI(BCHANFGJONF value)
	{
		BCHANFGJONF bCHANFGJONF = OnAuthenticationFailed;
		BCHANFGJONF bCHANFGJONF2;
		do
		{
			bCHANFGJONF2 = bCHANFGJONF;
			bCHANFGJONF = Interlocked.CompareExchange(ref OnAuthenticationFailed, (BCHANFGJONF)Delegate.Combine(bCHANFGJONF2, value), bCHANFGJONF);
		}
		while ((object)bCHANFGJONF != bCHANFGJONF2);
	}

	public void BFANLHDOICD(BCHANFGJONF value)
	{
		BCHANFGJONF bCHANFGJONF = OnAuthenticationFailed;
		BCHANFGJONF bCHANFGJONF2;
		do
		{
			bCHANFGJONF2 = bCHANFGJONF;
			bCHANFGJONF = Interlocked.CompareExchange(ref OnAuthenticationFailed, (BCHANFGJONF)Delegate.Remove(bCHANFGJONF2, value), bCHANFGJONF);
		}
		while ((object)bCHANFGJONF != bCHANFGJONF2);
	}

	public void MKODIGEMHFN()
	{
		KHHMDBBJJFO = new HTTPRequest(BDJDPKFCGCH(), LAAFHDKKJFL.Post, MNICDCAPDDC);
		KHHMDBBJJFO.AddField("userName", BFFCEKDPNAM());
		KHHMDBBJJFO.AddField("Password", LDEFEGOBBGO());
		KHHMDBBJJFO.AddField("roles", NFJMHJJDDHC());
		KHHMDBBJJFO.Send();
	}

	public void PrepareRequest(HTTPRequest ONOCIELLAPL, FHIEGKMHOCC LFLGCDNKNJI)
	{
		ONOCIELLAPL.HNDADBHDOID().Add(LLOHIGOHLMI);
	}

	private void MNICDCAPDDC(HTTPRequest CGOIOKHEGOE, HTTPResponse BEIGFGCBICO)
	{
		KHHMDBBJJFO = null;
		string nEPOLDCKNJL = string.Empty;
		switch (CGOIOKHEGOE.FLBBFDNHJAJ())
		{
		case CFGBMHKCENK.Finished:
			if (BEIGFGCBICO.AICKPAMONBH())
			{
				LLOHIGOHLMI = ((BEIGFGCBICO.HNDADBHDOID() == null) ? null : BEIGFGCBICO.HNDADBHDOID().Find((Cookie ILHDJDNPFKH) => ILHDJDNPFKH.get_Name().Equals(".ASPXAUTH")));
				if (LLOHIGOHLMI != null)
				{
					HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("CookieAuthentication", "Auth. Cookie found!");
					if (OnAuthenticationSucceded != null)
					{
						OnAuthenticationSucceded(this);
					}
					return;
				}
				HTTPManager.MBBMPNDDPIH().GLCKHLCAPIN("CookieAuthentication", nEPOLDCKNJL = "Auth. Cookie NOT found!");
			}
			else
			{
				HTTPManager.MBBMPNDDPIH().GLCKHLCAPIN("CookieAuthentication", nEPOLDCKNJL = string.Format("Request Finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}", BEIGFGCBICO.KNMDPGBPNED(), BEIGFGCBICO.DCKPMHKDLEJ(), BEIGFGCBICO.DPBLPGKOEJB()));
			}
			break;
		case CFGBMHKCENK.Error:
			HTTPManager.MBBMPNDDPIH().GLCKHLCAPIN("CookieAuthentication", nEPOLDCKNJL = "Request Finished with Error! " + ((CGOIOKHEGOE.IEFGFKFHNMD() == null) ? "No Exception" : (CGOIOKHEGOE.IEFGFKFHNMD().Message + "\n" + CGOIOKHEGOE.IEFGFKFHNMD().StackTrace)));
			break;
		case CFGBMHKCENK.Aborted:
			HTTPManager.MBBMPNDDPIH().GLCKHLCAPIN("CookieAuthentication", nEPOLDCKNJL = "Request Aborted!");
			break;
		case CFGBMHKCENK.ConnectionTimedOut:
			HTTPManager.MBBMPNDDPIH().Error("CookieAuthentication", nEPOLDCKNJL = "Connection Timed Out!");
			break;
		case CFGBMHKCENK.TimedOut:
			HTTPManager.MBBMPNDDPIH().Error("CookieAuthentication", nEPOLDCKNJL = "Processing the request Timed Out!");
			break;
		}
		if (OnAuthenticationFailed != null)
		{
			OnAuthenticationFailed(this, nEPOLDCKNJL);
		}
	}
}
