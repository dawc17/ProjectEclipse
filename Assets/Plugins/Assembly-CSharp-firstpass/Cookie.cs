using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

public sealed class Cookie : IComparable<Cookie>, IEquatable<Cookie>
{
	private const int Version = 1;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string HKGHEJDKCPI;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string IELPCLONGKP;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private DateTime DLHGOFDMCHO;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private DateTime KLMBGHCOBPE;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private DateTime DCBEPAGIKAK;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private long DLAMLEBFCJO;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool FJNJIMKBHCJ;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string EEMFKBEMNAJ;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string AHAGEGAEDBD;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool FDLIOFMNGPG;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool IDGFJNJEJEL;

	public string MENAJEAJJBE
	{
		get
		{
			return get_Name();
		}
		private set
		{
			set_Name(value);
		}
	}

	public DateTime IPJJMANFBBF
	{
		get
		{
			return DJPNPAGCDKB();
		}
		internal set
		{
			MPHLCKEMAIL(value);
		}
	}

	public DateTime BINEABBDPDH
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

	public DateTime FCFCGJHCJJL
	{
		get
		{
			return EPCHAKMLDFN();
		}
		private set
		{
			PGAOJGPAMND(value);
		}
	}

	public long JHJOIHEHOEP
	{
		get
		{
			return FCBEBMKLFIO();
		}
		private set
		{
			set_MaxAge(value);
		}
	}

	public bool OPMIELPALOE
	{
		get
		{
			return HPBAMOOJLLF();
		}
		private set
		{
			IOAIJLIJEGM(value);
		}
	}

	public string COMLIHJDDPG
	{
		get
		{
			return PILMIFGDMCK();
		}
		private set
		{
			GCICCAKGEGO(value);
		}
	}

	public string NGMCEBMMKHP
	{
		get
		{
			return DEIEDODNANN();
		}
		private set
		{
			GEAMFLMNFBI(value);
		}
	}

	public bool EINFHMBOLBE
	{
		get
		{
			return KFPJIIHEAFJ();
		}
		private set
		{
			CNFIAGCLCLM(value);
		}
	}

	public bool ABIAINBOAKM
	{
		get
		{
			return BJGFJBHHAFA();
		}
		private set
		{
			KFBLGDFAHFH(value);
		}
	}

	public Cookie(string name, string value)
		: this(name, value, string.Empty, string.Empty)
	{
	}

	public Cookie(string name, string value, string path)
		: this(name, value, path, string.Empty)
	{
	}

	public Cookie(string name, string value, string path, string OKDDNOHODMN)
		: this()
	{
		set_Name(name);
		set_Value(value);
		GEAMFLMNFBI(path);
		GCICCAKGEGO(OKDDNOHODMN);
	}

	internal Cookie()
	{
		IOAIJLIJEGM(true);
		set_MaxAge(-1L);
		ABGLCGLPNKO(DateTime.UtcNow);
	}

	public string get_Name()
	{
		return HKGHEJDKCPI;
	}

	private void set_Name(string value)
	{
		HKGHEJDKCPI = value;
	}

	public string OEAKCOHMIHH()
	{
		return IELPCLONGKP;
	}

	private void set_Value(string value)
	{
		IELPCLONGKP = value;
	}

	public DateTime DJPNPAGCDKB()
	{
		return DLHGOFDMCHO;
	}

	internal void MPHLCKEMAIL(DateTime value)
	{
		DLHGOFDMCHO = value;
	}

	public DateTime PPHKANGFLHJ()
	{
		return KLMBGHCOBPE;
	}

	public void ABGLCGLPNKO(DateTime value)
	{
		KLMBGHCOBPE = value;
	}

	public DateTime EPCHAKMLDFN()
	{
		return DCBEPAGIKAK;
	}

	private void PGAOJGPAMND(DateTime value)
	{
		DCBEPAGIKAK = value;
	}

	public long FCBEBMKLFIO()
	{
		return DLAMLEBFCJO;
	}

	private void set_MaxAge(long value)
	{
		DLAMLEBFCJO = value;
	}

	public bool HPBAMOOJLLF()
	{
		return FJNJIMKBHCJ;
	}

	private void IOAIJLIJEGM(bool value)
	{
		FJNJIMKBHCJ = value;
	}

	public string PILMIFGDMCK()
	{
		return EEMFKBEMNAJ;
	}

	private void GCICCAKGEGO(string value)
	{
		EEMFKBEMNAJ = value;
	}

	public string DEIEDODNANN()
	{
		return AHAGEGAEDBD;
	}

	private void GEAMFLMNFBI(string value)
	{
		AHAGEGAEDBD = value;
	}

	public bool KFPJIIHEAFJ()
	{
		return FDLIOFMNGPG;
	}

	private void CNFIAGCLCLM(bool value)
	{
		FDLIOFMNGPG = value;
	}

	public bool BJGFJBHHAFA()
	{
		return IDGFJNJEJEL;
	}

	private void KFBLGDFAHFH(bool value)
	{
		IDGFJNJEJEL = value;
	}

	public bool IDGHLAOFMEO()
	{
		if (HPBAMOOJLLF())
		{
			return true;
		}
		return (FCBEBMKLFIO() == -1) ? (EPCHAKMLDFN() > DateTime.UtcNow) : (Math.Max(0L, (long)(DateTime.UtcNow - DJPNPAGCDKB()).TotalSeconds) < FCBEBMKLFIO());
	}

	public uint ADGKKEKOJBD()
	{
		return (uint)(((get_Name() != null) ? (get_Name().Length * 2) : 0) + ((OEAKCOHMIHH() != null) ? (OEAKCOHMIHH().Length * 2) : 0) + ((PILMIFGDMCK() != null) ? (PILMIFGDMCK().Length * 2) : 0) + ((DEIEDODNANN() != null) ? (DEIEDODNANN().Length * 2) : 0) + 32 + 3);
	}

	public static Cookie Parse(string HHAAFADDOJB, Uri BABJLNLFPPI)
	{
		Cookie eKAOIOLAGFH = new Cookie();
		try
		{
			List<KeyValuePair> list = HBAPIKCODAD(HHAAFADDOJB);
			foreach (KeyValuePair item in list)
			{
				switch (item.AENLBNDAEKB().ToLowerInvariant())
				{
				case "path":
				{
					object bAINMLLIKOL;
					if (string.IsNullOrEmpty(item.OEAKCOHMIHH()) || !item.OEAKCOHMIHH().StartsWith("/"))
					{
						bAINMLLIKOL = "/";
					}
					else
					{
						string text = item.OEAKCOHMIHH();
						eKAOIOLAGFH.GEAMFLMNFBI(text);
						bAINMLLIKOL = text;
					}
					eKAOIOLAGFH.GEAMFLMNFBI((string)bAINMLLIKOL);
					break;
				}
				case "domain":
					if (string.IsNullOrEmpty(item.OEAKCOHMIHH()))
					{
						return null;
					}
					eKAOIOLAGFH.GCICCAKGEGO((!item.OEAKCOHMIHH().StartsWith(".")) ? item.OEAKCOHMIHH() : item.OEAKCOHMIHH().Substring(1));
					break;
				case "expires":
					eKAOIOLAGFH.PGAOJGPAMND(item.OEAKCOHMIHH().ToDateTime(DateTime.FromBinary(0L)));
					eKAOIOLAGFH.IOAIJLIJEGM(false);
					break;
				case "max-age":
					eKAOIOLAGFH.set_MaxAge(item.OEAKCOHMIHH().ToInt64(-1L));
					eKAOIOLAGFH.IOAIJLIJEGM(false);
					break;
				case "secure":
					eKAOIOLAGFH.CNFIAGCLCLM(true);
					break;
				case "httponly":
					eKAOIOLAGFH.KFBLGDFAHFH(true);
					break;
				default:
					eKAOIOLAGFH.set_Name(item.AENLBNDAEKB());
					eKAOIOLAGFH.set_Value(item.OEAKCOHMIHH());
					break;
				}
			}
			if (HTTPManager.IMFEILECHFL())
			{
				eKAOIOLAGFH.IOAIJLIJEGM(true);
			}
			if (string.IsNullOrEmpty(eKAOIOLAGFH.PILMIFGDMCK()))
			{
				eKAOIOLAGFH.GCICCAKGEGO(BABJLNLFPPI.Host);
			}
			if (string.IsNullOrEmpty(eKAOIOLAGFH.DEIEDODNANN()))
			{
				eKAOIOLAGFH.GEAMFLMNFBI(BABJLNLFPPI.AbsolutePath);
			}
			DateTime utcNow = DateTime.UtcNow;
			eKAOIOLAGFH.ABGLCGLPNKO(utcNow);
			eKAOIOLAGFH.MPHLCKEMAIL(utcNow);
		}
		catch
		{
		}
		return eKAOIOLAGFH;
	}

	internal void SaveTo(BinaryWriter ABJIEFMMIEK)
	{
		ABJIEFMMIEK.Write(1);
		ABJIEFMMIEK.Write(get_Name() ?? string.Empty);
		ABJIEFMMIEK.Write(OEAKCOHMIHH() ?? string.Empty);
		ABJIEFMMIEK.Write(DJPNPAGCDKB().ToBinary());
		ABJIEFMMIEK.Write(PPHKANGFLHJ().ToBinary());
		ABJIEFMMIEK.Write(EPCHAKMLDFN().ToBinary());
		ABJIEFMMIEK.Write(FCBEBMKLFIO());
		ABJIEFMMIEK.Write(HPBAMOOJLLF());
		ABJIEFMMIEK.Write(PILMIFGDMCK() ?? string.Empty);
		ABJIEFMMIEK.Write(DEIEDODNANN() ?? string.Empty);
		ABJIEFMMIEK.Write(KFPJIIHEAFJ());
		ABJIEFMMIEK.Write(BJGFJBHHAFA());
	}

	internal void LoadFrom(BinaryReader ABJIEFMMIEK)
	{
		ABJIEFMMIEK.ReadInt32();
		set_Name(ABJIEFMMIEK.ReadString());
		set_Value(ABJIEFMMIEK.ReadString());
		MPHLCKEMAIL(DateTime.FromBinary(ABJIEFMMIEK.ReadInt64()));
		ABGLCGLPNKO(DateTime.FromBinary(ABJIEFMMIEK.ReadInt64()));
		PGAOJGPAMND(DateTime.FromBinary(ABJIEFMMIEK.ReadInt64()));
		set_MaxAge(ABJIEFMMIEK.ReadInt64());
		IOAIJLIJEGM(ABJIEFMMIEK.ReadBoolean());
		GCICCAKGEGO(ABJIEFMMIEK.ReadString());
		GEAMFLMNFBI(ABJIEFMMIEK.ReadString());
		CNFIAGCLCLM(ABJIEFMMIEK.ReadBoolean());
		KFBLGDFAHFH(ABJIEFMMIEK.ReadBoolean());
	}

	public override string ToString()
	{
		return get_Name() + "=" + OEAKCOHMIHH();
	}

	public override bool Equals(object AOMLCBHAJJH)
	{
		if (AOMLCBHAJJH == null)
		{
			return false;
		}
		return Equals(AOMLCBHAJJH as Cookie);
	}

	public bool Equals(Cookie FJKPPODBPJF)
	{
		if (FJKPPODBPJF == null)
		{
			return false;
		}
		if (object.ReferenceEquals(this, FJKPPODBPJF))
		{
			return true;
		}
		return get_Name().Equals(FJKPPODBPJF.get_Name(), StringComparison.Ordinal) && ((PILMIFGDMCK() == null && FJKPPODBPJF.PILMIFGDMCK() == null) || PILMIFGDMCK().Equals(FJKPPODBPJF.PILMIFGDMCK(), StringComparison.Ordinal)) && ((DEIEDODNANN() == null && FJKPPODBPJF.DEIEDODNANN() == null) || DEIEDODNANN().Equals(FJKPPODBPJF.DEIEDODNANN(), StringComparison.Ordinal));
	}

	public override int GetHashCode()
	{
		return ToString().GetHashCode();
	}

	private static string ReadValue(string IGGFGLLIGCG, ref int LCCLEFMKLPB)
	{
		string empty = string.Empty;
		if (IGGFGLLIGCG == null)
		{
			return empty;
		}
		return IGGFGLLIGCG.Read(ref LCCLEFMKLPB, ';');
	}

	private static List<KeyValuePair> HBAPIKCODAD(string IGGFGLLIGCG)
	{
		List<KeyValuePair> list = new List<KeyValuePair>();
		if (IGGFGLLIGCG == null)
		{
			return list;
		}
		int LCCLEFMKLPB = 0;
		while (LCCLEFMKLPB < IGGFGLLIGCG.Length)
		{
			string kGBGENDIMBC = IGGFGLLIGCG.Read(ref LCCLEFMKLPB, (char KDFCGMMKAME) => KDFCGMMKAME != '=' && KDFCGMMKAME != ';').Trim();
			KeyValuePair gGCJLGPPHKP = new KeyValuePair(kGBGENDIMBC);
			if (LCCLEFMKLPB < IGGFGLLIGCG.Length && IGGFGLLIGCG[LCCLEFMKLPB - 1] == '=')
			{
				gGCJLGPPHKP.set_Value(ReadValue(IGGFGLLIGCG, ref LCCLEFMKLPB));
			}
			list.Add(gGCJLGPPHKP);
		}
		return list;
	}

	public int CompareTo(Cookie NOLFMPDGCOC)
	{
		return PPHKANGFLHJ().CompareTo(NOLFMPDGCOC.PPHKANGFLHJ());
	}
}
