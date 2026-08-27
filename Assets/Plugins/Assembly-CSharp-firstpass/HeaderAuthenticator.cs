using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

internal class HeaderAuthenticator : IAuthenticationProvider
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string KGDNJPINPMN;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string JLGKGECFBII;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private LACLODBGJEI OnAuthenticationSucceded;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private BCHANFGJONF OnAuthenticationFailed;

	public string NKHIGHOCKOP
	{
		get
		{
			return MDBKLAILOEA();
		}
		private set
		{
			PEHJEBAJFKA(value);
		}
	}

	public string NNJOKOABFAL
	{
		get
		{
			return HEDILAFLPHJ();
		}
		private set
		{
			PELNBAHFEIO(value);
		}
	}

	public bool CBIBNHGGEBI
	{
		get
		{
			return MCHOHLKGMBI();
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

	public HeaderAuthenticator(string KEJDJHAGBMK, string NNMKKAKIJCP)
	{
		PEHJEBAJFKA(KEJDJHAGBMK);
		PELNBAHFEIO(NNMKKAKIJCP);
	}

	public string MDBKLAILOEA()
	{
		return KGDNJPINPMN;
	}

	private void PEHJEBAJFKA(string value)
	{
		KGDNJPINPMN = value;
	}

	public string HEDILAFLPHJ()
	{
		return JLGKGECFBII;
	}

	private void PELNBAHFEIO(string value)
	{
		JLGKGECFBII = value;
	}

	public bool MCHOHLKGMBI()
	{
		return false;
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
	}

	public void PrepareRequest(HTTPRequest ONOCIELLAPL, FHIEGKMHOCC LFLGCDNKNJI)
	{
		ONOCIELLAPL.MMPFBNNMGED("username", MDBKLAILOEA());
		ONOCIELLAPL.MMPFBNNMGED("roles", HEDILAFLPHJ());
	}
}
