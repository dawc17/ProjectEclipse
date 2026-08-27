using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

internal class HTTPCacheFileInfo : IComparable<HTTPCacheFileInfo>
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Uri NHCOGAAPOAB;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private DateTime KLMBGHCOBPE;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int MHLKLAEBFCD;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string KCEOHKJOHFB;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string FLJCNLNAENP;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private DateTime DCBEPAGIKAK;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private long NGMJOLHOLKL;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private long DLAMLEBFCJO;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private DateTime DLHGOFDMCHO;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool LEBBJPJPCLA;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private DateTime EOHJKHPPCCN;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string NIGDFDADGKD;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private ulong HDEPIILJKIN;

	internal Uri GJIGOCNEPME
	{
		get
		{
			return OJBDMGBGJMA();
		}
		set
		{
			set_Uri(value);
		}
	}

	internal DateTime BINEABBDPDH
	{
		get
		{
			return PPHKANGFLHJ();
		}
		set
		{
			ABGLCGLPNKO(value);
		}
	}

	internal int MADCPBFOEOG
	{
		get
		{
			return NHCAEHOHBEE();
		}
		set
		{
			set_BodyLength(value);
		}
	}

	private string MFCFBMCBJMI
	{
		get
		{
			return HEIJBOKAMJI();
		}
		set
		{
			FKPPFMHIBJM(value);
		}
	}

	private string LastModified
	{
		get
		{
			return FKJJDHNKOLH();
		}
		set
		{
			ICHJGCIBBGC(value);
		}
	}

	private DateTime FCFCGJHCJJL
	{
		get
		{
			return EPCHAKMLDFN();
		}
		set
		{
			PGAOJGPAMND(value);
		}
	}

	private long IDGJLEBFHOK
	{
		get
		{
			return DMCMJJCMLGP();
		}
		set
		{
			FFGJFMNGBDK(value);
		}
	}

	private long JHJOIHEHOEP
	{
		get
		{
			return FCBEBMKLFIO();
		}
		set
		{
			set_MaxAge(value);
		}
	}

	private DateTime IPJJMANFBBF
	{
		get
		{
			return DJPNPAGCDKB();
		}
		set
		{
			MPHLCKEMAIL(value);
		}
	}

	private bool OBNKAIAGJFG
	{
		get
		{
			return EJCIFAGMCIE();
		}
		set
		{
			set_MustRevalidate(value);
		}
	}

	private DateTime LDFMDJGCLGM
	{
		get
		{
			return EHEDLEAKOHD();
		}
		set
		{
			JBEBONEKNJP(value);
		}
	}

	private string KMEPDJGCKKL
	{
		get
		{
			return IBMNGGCCEOL();
		}
		set
		{
			FKIBPCNPPJA(value);
		}
	}

	internal ulong DIFCEKMFBBL
	{
		get
		{
			return KDBDNGPOENN();
		}
		set
		{
			set_MappedNameIDX(value);
		}
	}

	internal HTTPCacheFileInfo(Uri KJHNCLAJMLO)
		: this(KJHNCLAJMLO, DateTime.UtcNow, -1)
	{
	}

	internal HTTPCacheFileInfo(Uri KJHNCLAJMLO, DateTime HMHMICPJKOF, int DEFBMELCOHO)
	{
		set_Uri(KJHNCLAJMLO);
		ABGLCGLPNKO(HMHMICPJKOF);
		set_BodyLength(DEFBMELCOHO);
		set_MaxAge(-1L);
		set_MappedNameIDX(HTTPCacheService.OKGJODFOLBM());
	}

	internal HTTPCacheFileInfo(Uri KJHNCLAJMLO, BinaryReader reader, int version)
	{
		set_Uri(KJHNCLAJMLO);
		ABGLCGLPNKO(DateTime.FromBinary(reader.ReadInt64()));
		set_BodyLength(reader.ReadInt32());
		switch (version)
		{
		default:
			return;
		case 2:
			set_MappedNameIDX(reader.ReadUInt64());
			break;
		case 1:
			break;
		}
		FKPPFMHIBJM(reader.ReadString());
		ICHJGCIBBGC(reader.ReadString());
		PGAOJGPAMND(DateTime.FromBinary(reader.ReadInt64()));
		FFGJFMNGBDK(reader.ReadInt64());
		set_MaxAge(reader.ReadInt64());
		MPHLCKEMAIL(DateTime.FromBinary(reader.ReadInt64()));
		set_MustRevalidate(reader.ReadBoolean());
		JBEBONEKNJP(DateTime.FromBinary(reader.ReadInt64()));
	}

	internal Uri OJBDMGBGJMA()
	{
		return NHCOGAAPOAB;
	}

	internal void set_Uri(Uri value)
	{
		NHCOGAAPOAB = value;
	}

	internal DateTime PPHKANGFLHJ()
	{
		return KLMBGHCOBPE;
	}

	internal void ABGLCGLPNKO(DateTime value)
	{
		KLMBGHCOBPE = value;
	}

	internal int NHCAEHOHBEE()
	{
		return MHLKLAEBFCD;
	}

	internal void set_BodyLength(int value)
	{
		MHLKLAEBFCD = value;
	}

	private string HEIJBOKAMJI()
	{
		return KCEOHKJOHFB;
	}

	private void FKPPFMHIBJM(string value)
	{
		KCEOHKJOHFB = value;
	}

	private string FKJJDHNKOLH()
	{
		return FLJCNLNAENP;
	}

	private void ICHJGCIBBGC(string value)
	{
		FLJCNLNAENP = value;
	}

	private DateTime EPCHAKMLDFN()
	{
		return DCBEPAGIKAK;
	}

	private void PGAOJGPAMND(DateTime value)
	{
		DCBEPAGIKAK = value;
	}

	private long DMCMJJCMLGP()
	{
		return NGMJOLHOLKL;
	}

	private void FFGJFMNGBDK(long value)
	{
		NGMJOLHOLKL = value;
	}

	private long FCBEBMKLFIO()
	{
		return DLAMLEBFCJO;
	}

	private void set_MaxAge(long value)
	{
		DLAMLEBFCJO = value;
	}

	private DateTime DJPNPAGCDKB()
	{
		return DLHGOFDMCHO;
	}

	private void MPHLCKEMAIL(DateTime value)
	{
		DLHGOFDMCHO = value;
	}

	private bool EJCIFAGMCIE()
	{
		return LEBBJPJPCLA;
	}

	private void set_MustRevalidate(bool value)
	{
		LEBBJPJPCLA = value;
	}

	private DateTime EHEDLEAKOHD()
	{
		return EOHJKHPPCCN;
	}

	private void JBEBONEKNJP(DateTime value)
	{
		EOHJKHPPCCN = value;
	}

	private string IBMNGGCCEOL()
	{
		return NIGDFDADGKD;
	}

	private void FKIBPCNPPJA(string value)
	{
		NIGDFDADGKD = value;
	}

	internal ulong KDBDNGPOENN()
	{
		return HDEPIILJKIN;
	}

	internal void set_MappedNameIDX(ulong value)
	{
		HDEPIILJKIN = value;
	}

	internal void SaveTo(BinaryWriter writer)
	{
		writer.Write(PPHKANGFLHJ().ToBinary());
		writer.Write(NHCAEHOHBEE());
		writer.Write(KDBDNGPOENN());
		writer.Write(HEIJBOKAMJI());
		writer.Write(FKJJDHNKOLH());
		writer.Write(EPCHAKMLDFN().ToBinary());
		writer.Write(DMCMJJCMLGP());
		writer.Write(FCBEBMKLFIO());
		writer.Write(DJPNPAGCDKB().ToBinary());
		writer.Write(EJCIFAGMCIE());
		writer.Write(EHEDLEAKOHD().ToBinary());
	}

	private string HFGMHHDBHMH()
	{
		if (IBMNGGCCEOL() != null)
		{
			return IBMNGGCCEOL();
		}
		string text = Path.Combine(HTTPCacheService.MJJLGGBAMJE(), KDBDNGPOENN().ToString("X"));
		FKIBPCNPPJA(text);
		return text;
	}

	internal bool IsExists()
	{
		if (!HTTPCacheService.EPACOIFEICA())
		{
			return false;
		}
		return File.Exists(HFGMHHDBHMH());
	}

	internal void JEHBFCLLPCL()
	{
		if (!HTTPCacheService.EPACOIFEICA())
		{
			return;
		}
		string path = HFGMHHDBHMH();
		try
		{
			File.Delete(path);
		}
		catch
		{
		}
		finally
		{
			Reset();
		}
	}

	private void Reset()
	{
		set_MappedNameIDX(0uL);
		set_BodyLength(-1);
		FKPPFMHIBJM(string.Empty);
		PGAOJGPAMND(DateTime.FromBinary(0L));
		ICHJGCIBBGC(string.Empty);
		FFGJFMNGBDK(0L);
		set_MaxAge(-1L);
		MPHLCKEMAIL(DateTime.FromBinary(0L));
		set_MustRevalidate(false);
		JBEBONEKNJP(DateTime.FromBinary(0L));
	}

	private void NDENMNGKBPG(HTTPResponse GIHDDAKBMHE)
	{
		FKPPFMHIBJM(GIHDDAKBMHE.GetFirstHeaderValue("ETag").PKBHGNMGNNO());
		PGAOJGPAMND(GIHDDAKBMHE.GetFirstHeaderValue("Expires").ToDateTime(DateTime.FromBinary(0L)));
		ICHJGCIBBGC(GIHDDAKBMHE.GetFirstHeaderValue("Last-Modified").PKBHGNMGNNO());
		FFGJFMNGBDK(GIHDDAKBMHE.GetFirstHeaderValue("Age").ToInt64(0L));
		MPHLCKEMAIL(GIHDDAKBMHE.GetFirstHeaderValue("Date").ToDateTime(DateTime.FromBinary(0L)));
		string text = GIHDDAKBMHE.GetFirstHeaderValue("cache-control");
		if (!string.IsNullOrEmpty(text))
		{
			string[] array = text.FindOption("Max-Age");
			double result;
			if (array != null && double.TryParse(array[1], out result))
			{
				set_MaxAge((int)result);
			}
			set_MustRevalidate(text.ToLower().Contains("must-revalidate"));
		}
		JBEBONEKNJP(DateTime.UtcNow);
	}

	internal bool IDGHLAOFMEO()
	{
		if (!IsExists())
		{
			return false;
		}
		if (EJCIFAGMCIE())
		{
			return false;
		}
		if (FCBEBMKLFIO() != -1)
		{
			long val = Math.Max(0L, (long)(EHEDLEAKOHD() - DJPNPAGCDKB()).TotalSeconds);
			long num = Math.Max(val, DMCMJJCMLGP());
			long num2 = (long)(DateTime.UtcNow - DJPNPAGCDKB()).TotalSeconds;
			long num3 = num + num2;
			return num3 < FCBEBMKLFIO();
		}
		return EPCHAKMLDFN() > DateTime.UtcNow;
	}

	internal void EKLOPCNGGED(HTTPRequest ONOCIELLAPL)
	{
		if (IsExists())
		{
			if (!string.IsNullOrEmpty(HEIJBOKAMJI()))
			{
				ONOCIELLAPL.AddHeader("If-None-Match", HEIJBOKAMJI());
			}
			if (!string.IsNullOrEmpty(FKJJDHNKOLH()))
			{
				ONOCIELLAPL.AddHeader("If-Modified-Since", FKJJDHNKOLH());
			}
		}
	}

	internal Stream GetBodyStream(out int BDBOAEGELMC)
	{
		if (!IsExists())
		{
			BDBOAEGELMC = 0;
			return null;
		}
		BDBOAEGELMC = NHCAEHOHBEE();
		ABGLCGLPNKO(DateTime.UtcNow);
		FileStream fileStream = new FileStream(HFGMHHDBHMH(), FileMode.Open);
		fileStream.Seek(-BDBOAEGELMC, SeekOrigin.End);
		return fileStream;
	}

	internal HTTPResponse IFPJHJFHJDK(HTTPRequest ONOCIELLAPL)
	{
		if (!IsExists())
		{
			return null;
		}
		ABGLCGLPNKO(DateTime.UtcNow);
		using (FileStream aBJIEFMMIEK = new FileStream(HFGMHHDBHMH(), FileMode.Open))
		{
			HTTPResponse iLGKJGGJHAJ = new HTTPResponse(ONOCIELLAPL, aBJIEFMMIEK, ONOCIELLAPL.MDEPOKKKKCL(), true);
			iLGKJGGJHAJ.Receive(NHCAEHOHBEE());
			return iLGKJGGJHAJ;
		}
	}

	internal void LDFKMIOPLKA(HTTPResponse GIHDDAKBMHE)
	{
		if (!HTTPCacheService.EPACOIFEICA())
		{
			return;
		}
		string text = HFGMHHDBHMH();
		if (text.Length > HTTPManager.LACBBEFPIPO())
		{
			return;
		}
		if (File.Exists(text))
		{
			JEHBFCLLPCL();
		}
		using (FileStream fileStream = new FileStream(text, FileMode.Create))
		{
			fileStream.WriteLine("HTTP/1.1 {0} {1}", GIHDDAKBMHE.KNMDPGBPNED(), GIHDDAKBMHE.DCKPMHKDLEJ());
			foreach (KeyValuePair<string, List<string>> item in GIHDDAKBMHE.AJCCGKHBNML())
			{
				for (int i = 0; i < item.Value.Count; i++)
				{
					fileStream.WriteLine("{0}: {1}", item.Key, item.Value[i]);
				}
			}
			fileStream.WriteLine();
			fileStream.Write(GIHDDAKBMHE.CHIGLEKCFFN(), 0, GIHDDAKBMHE.CHIGLEKCFFN().Length);
		}
		set_BodyLength(GIHDDAKBMHE.CHIGLEKCFFN().Length);
		ABGLCGLPNKO(DateTime.UtcNow);
		NDENMNGKBPG(GIHDDAKBMHE);
	}

	internal Stream GOGEGGPBPBP(HTTPResponse GIHDDAKBMHE)
	{
		if (!HTTPCacheService.EPACOIFEICA())
		{
			return null;
		}
		ABGLCGLPNKO(DateTime.UtcNow);
		string text = HFGMHHDBHMH();
		if (File.Exists(text))
		{
			JEHBFCLLPCL();
		}
		if (text.Length > HTTPManager.LACBBEFPIPO())
		{
			return null;
		}
		using (FileStream mEHMICNAPMK = new FileStream(text, FileMode.Create))
		{
			mEHMICNAPMK.WriteLine("HTTP/1.1 {0} {1}", GIHDDAKBMHE.KNMDPGBPNED(), GIHDDAKBMHE.DCKPMHKDLEJ());
			foreach (KeyValuePair<string, List<string>> item in GIHDDAKBMHE.AJCCGKHBNML())
			{
				for (int i = 0; i < item.Value.Count; i++)
				{
					mEHMICNAPMK.WriteLine("{0}: {1}", item.Key, item.Value[i]);
				}
			}
			mEHMICNAPMK.WriteLine();
		}
		if (GIHDDAKBMHE.LOHDBJLLKEE() && !GIHDDAKBMHE.AJCCGKHBNML().ContainsKey("content-length"))
		{
			GIHDDAKBMHE.AJCCGKHBNML().Add("content-length", new List<string> { NHCAEHOHBEE().ToString() });
		}
		NDENMNGKBPG(GIHDDAKBMHE);
		return new FileStream(HFGMHHDBHMH(), FileMode.Append);
	}

	public int CompareTo(HTTPCacheFileInfo NOLFMPDGCOC)
	{
		return PPHKANGFLHJ().CompareTo(NOLFMPDGCOC.PPHKANGFLHJ());
	}
}
