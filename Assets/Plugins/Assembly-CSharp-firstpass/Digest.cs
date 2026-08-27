using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

internal sealed class Digest
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Uri NHCOGAAPOAB;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private BMBGFBGIAPL KAHHEBMBCFA;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string OPEKCOFPAKK;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool DAPBCAMFDNK;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string ILFNAADCHNH;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string HBNIABHGCLL;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string KAGNAOMBBEM;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private List<string> EBAINKEOJPE;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string EHEBENAMEGP;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int GFDDCEIJDCN;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string CBPAMELDDOK;

	public Uri GJIGOCNEPME
	{
		get
		{
			return OJBDMGBGJMA();
		}
		private set
		{
			set_Uri(value);
		}
	}

	public string CLACHGOIEHH
	{
		get
		{
			return LAEOPDGLBMO();
		}
		private set
		{
			NDLNONPPIJL(value);
		}
	}

	public bool BCNLCFEBBEH
	{
		get
		{
			return OCBMLPLDMOO();
		}
		private set
		{
			set_Stale(value);
		}
	}

	private string DNLHOEFAPOG
	{
		get
		{
			return CINJMLJPGGE();
		}
		set
		{
			MLKDAKOIOEC(value);
		}
	}

	private string HKLBDMFKKGH
	{
		get
		{
			return NBDLOHFGNHI();
		}
		set
		{
			JBDMKFHHGHE(value);
		}
	}

	private string DFAEENLMOLN
	{
		get
		{
			return EFJHDNIJDNN();
		}
		set
		{
			HCAIPEONAMO(value);
		}
	}

	public List<string> LIPMKBKNOAF
	{
		get
		{
			return DGAJBCGFAKI();
		}
		private set
		{
			set_ProtectedUris(value);
		}
	}

	private string IGIFOLFOALA
	{
		get
		{
			return OKNGMJFJPJM();
		}
		set
		{
			KFNMAJDFPJK(value);
		}
	}

	private int KFOCJOCPJNC
	{
		get
		{
			return PFLKBJDJLIK();
		}
		set
		{
			set_NonceCount(value);
		}
	}

	private string KMJHEHFGBGM
	{
		get
		{
			return ADPBKDPPKIK();
		}
		set
		{
			HPIBIHECJJP(value);
		}
	}

	internal Digest(Uri KJHNCLAJMLO)
	{
		set_Uri(KJHNCLAJMLO);
		HCAIPEONAMO("md5");
	}

	public Uri OJBDMGBGJMA()
	{
		return NHCOGAAPOAB;
	}

	private void set_Uri(Uri value)
	{
		NHCOGAAPOAB = value;
	}

	public BMBGFBGIAPL get_Type()
	{
		return KAHHEBMBCFA;
	}

	private void set_Type(BMBGFBGIAPL value)
	{
		KAHHEBMBCFA = value;
	}

	public string LAEOPDGLBMO()
	{
		return OPEKCOFPAKK;
	}

	private void NDLNONPPIJL(string value)
	{
		OPEKCOFPAKK = value;
	}

	public bool OCBMLPLDMOO()
	{
		return DAPBCAMFDNK;
	}

	private void set_Stale(bool value)
	{
		DAPBCAMFDNK = value;
	}

	private string CINJMLJPGGE()
	{
		return ILFNAADCHNH;
	}

	private void MLKDAKOIOEC(string value)
	{
		ILFNAADCHNH = value;
	}

	private string NBDLOHFGNHI()
	{
		return HBNIABHGCLL;
	}

	private void JBDMKFHHGHE(string value)
	{
		HBNIABHGCLL = value;
	}

	private string EFJHDNIJDNN()
	{
		return KAGNAOMBBEM;
	}

	private void HCAIPEONAMO(string value)
	{
		KAGNAOMBBEM = value;
	}

	public List<string> DGAJBCGFAKI()
	{
		return EBAINKEOJPE;
	}

	private void set_ProtectedUris(List<string> value)
	{
		EBAINKEOJPE = value;
	}

	private string OKNGMJFJPJM()
	{
		return EHEBENAMEGP;
	}

	private void KFNMAJDFPJK(string value)
	{
		EHEBENAMEGP = value;
	}

	private int PFLKBJDJLIK()
	{
		return GFDDCEIJDCN;
	}

	private void set_NonceCount(int value)
	{
		GFDDCEIJDCN = value;
	}

	private string ADPBKDPPKIK()
	{
		return CBPAMELDDOK;
	}

	private void HPIBIHECJJP(string value)
	{
		CBPAMELDDOK = value;
	}

	public void CKNNIILGPNN(string HHAAFADDOJB)
	{
		set_Type(BMBGFBGIAPL.Unknown);
		set_Stale(false);
		JBDMKFHHGHE(null);
		HPIBIHECJJP(null);
		set_NonceCount(0);
		KFNMAJDFPJK(null);
		if (DGAJBCGFAKI() != null)
		{
			DGAJBCGFAKI().Clear();
		}
		WWWAuthenticateHeaderParser iIGMPGDLCCK = new WWWAuthenticateHeaderParser(HHAAFADDOJB);
		foreach (KeyValuePair item2 in iIGMPGDLCCK.CCEDNLIDAND())
		{
			switch (item2.AENLBNDAEKB())
			{
			case "basic":
				set_Type(BMBGFBGIAPL.Basic);
				break;
			case "digest":
				set_Type(BMBGFBGIAPL.Digest);
				break;
			case "realm":
				NDLNONPPIJL(item2.OEAKCOHMIHH());
				break;
			case "domain":
				if (!string.IsNullOrEmpty(item2.OEAKCOHMIHH()) && item2.OEAKCOHMIHH().Length != 0)
				{
					if (DGAJBCGFAKI() == null)
					{
						set_ProtectedUris(new List<string>());
					}
					int LCCLEFMKLPB = 0;
					string item = item2.OEAKCOHMIHH().Read(ref LCCLEFMKLPB, ' ');
					do
					{
						DGAJBCGFAKI().Add(item);
						item = item2.OEAKCOHMIHH().Read(ref LCCLEFMKLPB, ' ');
					}
					while (LCCLEFMKLPB < item2.OEAKCOHMIHH().Length);
				}
				break;
			case "nonce":
				MLKDAKOIOEC(item2.OEAKCOHMIHH());
				break;
			case "qop":
				KFNMAJDFPJK(item2.OEAKCOHMIHH());
				break;
			case "stale":
				set_Stale(bool.Parse(item2.OEAKCOHMIHH()));
				break;
			case "opaque":
				JBDMKFHHGHE(item2.OEAKCOHMIHH());
				break;
			case "algorithm":
				HCAIPEONAMO(item2.OEAKCOHMIHH());
				break;
			}
		}
	}

	public string CIIGLAEHAOJ(HTTPRequest ONOCIELLAPL, Credentials JKBAHGNLECO)
	{
		try
		{
			switch (get_Type())
			{
			case BMBGFBGIAPL.Basic:
				return "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", JKBAHGNLECO.BFFCEKDPNAM(), JKBAHGNLECO.LDEFEGOBBGO())));
			case BMBGFBGIAPL.Digest:
			{
				set_NonceCount(PFLKBJDJLIK() + 1);
				string empty = string.Empty;
				string text = new Random(ONOCIELLAPL.GetHashCode()).Next(int.MinValue, int.MaxValue).ToString("X8");
				string text2 = PFLKBJDJLIK().ToString("X8");
				switch (EFJHDNIJDNN().JONPEPOKJFC())
				{
				case "md5":
					empty = string.Format("{0}:{1}:{2}", JKBAHGNLECO.BFFCEKDPNAM(), LAEOPDGLBMO(), JKBAHGNLECO.LDEFEGOBBGO()).DAOJIBHMOJK();
					break;
				case "md5-sess":
					if (string.IsNullOrEmpty(ADPBKDPPKIK()))
					{
						HPIBIHECJJP(string.Format("{0}:{1}:{2}:{3}:{4}", JKBAHGNLECO.BFFCEKDPNAM(), LAEOPDGLBMO(), JKBAHGNLECO.LDEFEGOBBGO(), CINJMLJPGGE(), text2).DAOJIBHMOJK());
					}
					empty = ADPBKDPPKIK();
					break;
				default:
					return string.Empty;
				}
				string empty2 = string.Empty;
				string text3 = ((OKNGMJFJPJM() == null) ? null : OKNGMJFJPJM().JONPEPOKJFC());
				if (text3 == null)
				{
					string arg = (ONOCIELLAPL.JCHNIGKBBMI().ToString().ToUpper() + ":" + ONOCIELLAPL.DKAECMGPGOE().PathAndQuery).DAOJIBHMOJK();
					empty2 = string.Format("{0}:{1}:{2}", empty, CINJMLJPGGE(), arg).DAOJIBHMOJK();
				}
				else if (text3.Contains("auth-int"))
				{
					text3 = "auth-int";
					byte[] array = ONOCIELLAPL.JLLCKEFOEBF();
					if (array == null)
					{
						array = string.Empty.GetASCIIBytes();
					}
					string text4 = string.Format("{0}:{1}:{2}", ONOCIELLAPL.JCHNIGKBBMI().ToString().ToUpper(), ONOCIELLAPL.DKAECMGPGOE().PathAndQuery, array.DAOJIBHMOJK()).DAOJIBHMOJK();
					empty2 = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", empty, CINJMLJPGGE(), text2, text, text3, text4).DAOJIBHMOJK();
				}
				else
				{
					if (!text3.Contains("auth"))
					{
						return string.Empty;
					}
					text3 = "auth";
					string text5 = (ONOCIELLAPL.JCHNIGKBBMI().ToString().ToUpper() + ":" + ONOCIELLAPL.DKAECMGPGOE().PathAndQuery).DAOJIBHMOJK();
					empty2 = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", empty, CINJMLJPGGE(), text2, text, text3, text5).DAOJIBHMOJK();
				}
				string text6 = string.Format("Digest username=\"{0}\", realm=\"{1}\", nonce=\"{2}\", uri=\"{3}\", cnonce=\"{4}\", response=\"{5}\"", JKBAHGNLECO.BFFCEKDPNAM(), LAEOPDGLBMO(), CINJMLJPGGE(), ONOCIELLAPL.OJBDMGBGJMA().PathAndQuery, text, empty2);
				if (text3 != null)
				{
					text6 = string.Concat(text6, ", qop=\"" + text3 + "\", nc=" + text2);
				}
				if (!string.IsNullOrEmpty(NBDLOHFGNHI()))
				{
					text6 = text6 + ", opaque=\"" + NBDLOHFGNHI() + "\"";
				}
				return text6;
			}
			}
		}
		catch
		{
		}
		return string.Empty;
	}

	public bool IsUriProtected(Uri KJHNCLAJMLO)
	{
		if (string.CompareOrdinal(KJHNCLAJMLO.Host, OJBDMGBGJMA().Host) != 0)
		{
			return false;
		}
		string text = KJHNCLAJMLO.ToString();
		if (DGAJBCGFAKI() != null && DGAJBCGFAKI().Count > 0)
		{
			for (int i = 0; i < DGAJBCGFAKI().Count; i++)
			{
				if (text.Contains(DGAJBCGFAKI()[i]))
				{
					return true;
				}
			}
		}
		return true;
	}
}
