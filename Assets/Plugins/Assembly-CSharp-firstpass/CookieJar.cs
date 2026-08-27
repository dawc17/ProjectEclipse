using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

public static class CookieJar
{
	private const int Version = 1;

	private static List<Cookie> FPFLODAGEFD = new List<Cookie>();

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static string LMJPLLHPNAI;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static string OFLEPKHJLFB;

	private static object Locker = new object();

	private static bool MPHCKKBNFPO;

	private static bool PFEOIGKNHKB;

	private static bool JHEGDCAEDFM;

	public static bool DLBFPOLIHFF
	{
		get
		{
			return NOMOAENPKCP();
		}
	}

	private static string GFLCLIIOALH
	{
		get
		{
			return BMMIHMPFOOA();
		}
		set
		{
			EAGHMBOHBED(value);
		}
	}

	private static string OHOPINMBPPJ
	{
		get
		{
			return HIJIPJKMFNH();
		}
		set
		{
			DBLJMNJMAHF(value);
		}
	}

	public static bool NOMOAENPKCP()
	{
		if (PFEOIGKNHKB)
		{
			return MPHCKKBNFPO;
		}
		try
		{
			File.Exists(HTTPManager.DJHDCCJDJGJ());
			MPHCKKBNFPO = true;
		}
		catch
		{
			MPHCKKBNFPO = false;
			HTTPManager.MBBMPNDDPIH().GLCKHLCAPIN("CookieJar", "Cookie saving and loading disabled!");
		}
		finally
		{
			PFEOIGKNHKB = true;
		}
		return MPHCKKBNFPO;
	}

	private static string BMMIHMPFOOA()
	{
		return LMJPLLHPNAI;
	}

	private static void EAGHMBOHBED(string value)
	{
		LMJPLLHPNAI = value;
	}

	private static string HIJIPJKMFNH()
	{
		return OFLEPKHJLFB;
	}

	private static void DBLJMNJMAHF(string value)
	{
		OFLEPKHJLFB = value;
	}

	internal static void ELIJOFFHEBP()
	{
		if (!NOMOAENPKCP())
		{
			return;
		}
		try
		{
			if (string.IsNullOrEmpty(BMMIHMPFOOA()) || string.IsNullOrEmpty(HIJIPJKMFNH()))
			{
				EAGHMBOHBED(Path.Combine(HTTPManager.DJHDCCJDJGJ(), "Cookies"));
				DBLJMNJMAHF(Path.Combine(BMMIHMPFOOA(), "Library"));
			}
		}
		catch
		{
		}
	}

	internal static void Set(HTTPResponse GIHDDAKBMHE)
	{
		if (GIHDDAKBMHE == null)
		{
			return;
		}
		lock (Locker)
		{
			try
			{
				DPDIOCGIKEO();
				List<Cookie> list = new List<Cookie>();
				List<string> list2 = GIHDDAKBMHE.GetHeaderValues("set-cookie");
				if (list2 == null)
				{
					return;
				}
				foreach (string item in list2)
				{
					try
					{
						Cookie eKAOIOLAGFH = Cookie.Parse(item, GIHDDAKBMHE.KEEGKCNNPGM.DKAECMGPGOE());
						if (eKAOIOLAGFH == null)
						{
							continue;
						}
						int OOPOEMNCCGH;
						Cookie eKAOIOLAGFH2 = EKGMPKKAAKB(eKAOIOLAGFH, out OOPOEMNCCGH);
						if (!string.IsNullOrEmpty(eKAOIOLAGFH.OEAKCOHMIHH()) && eKAOIOLAGFH.IDGHLAOFMEO())
						{
							if (eKAOIOLAGFH2 == null)
							{
								FPFLODAGEFD.Add(eKAOIOLAGFH);
								list.Add(eKAOIOLAGFH);
							}
							else
							{
								eKAOIOLAGFH.MPHLCKEMAIL(eKAOIOLAGFH2.DJPNPAGCDKB());
								FPFLODAGEFD[OOPOEMNCCGH] = eKAOIOLAGFH;
								list.Add(eKAOIOLAGFH);
							}
						}
						else if (OOPOEMNCCGH != -1)
						{
							FPFLODAGEFD.RemoveAt(OOPOEMNCCGH);
						}
					}
					catch
					{
					}
				}
				GIHDDAKBMHE.PPLAPHMALFL(list);
			}
			catch
			{
			}
		}
	}

	internal static void DPDIOCGIKEO()
	{
		lock (Locker)
		{
			try
			{
				uint num = 0u;
				TimeSpan timeSpan = TimeSpan.FromDays(7.0);
				int num2 = 0;
				while (num2 < FPFLODAGEFD.Count)
				{
					Cookie eKAOIOLAGFH = FPFLODAGEFD[num2];
					if (!eKAOIOLAGFH.IDGHLAOFMEO() || eKAOIOLAGFH.PPHKANGFLHJ() + timeSpan < DateTime.UtcNow)
					{
						FPFLODAGEFD.RemoveAt(num2);
						continue;
					}
					if (!eKAOIOLAGFH.HPBAMOOJLLF())
					{
						num += eKAOIOLAGFH.ADGKKEKOJBD();
					}
					num2++;
				}
				if (num > HTTPManager.CFPIDMJOENK())
				{
					FPFLODAGEFD.Sort();
					while (num > HTTPManager.CFPIDMJOENK() && FPFLODAGEFD.Count > 0)
					{
						Cookie eKAOIOLAGFH2 = FPFLODAGEFD[0];
						FPFLODAGEFD.RemoveAt(0);
						num -= eKAOIOLAGFH2.ADGKKEKOJBD();
					}
				}
			}
			catch
			{
			}
		}
	}

	internal static void AENFMDELLBM()
	{
		if (!NOMOAENPKCP())
		{
			return;
		}
		lock (Locker)
		{
			try
			{
				DPDIOCGIKEO();
				if (!Directory.Exists(BMMIHMPFOOA()))
				{
					Directory.CreateDirectory(BMMIHMPFOOA());
				}
				using (FileStream output = new FileStream(HIJIPJKMFNH(), FileMode.Create))
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(output))
					{
						binaryWriter.Write(1);
						int num = 0;
						foreach (Cookie item in FPFLODAGEFD)
						{
							if (!item.HPBAMOOJLLF())
							{
								num++;
							}
						}
						binaryWriter.Write(num);
						foreach (Cookie item2 in FPFLODAGEFD)
						{
							if (!item2.HPBAMOOJLLF())
							{
								item2.SaveTo(binaryWriter);
							}
						}
					}
				}
			}
			catch
			{
			}
		}
	}

	internal static void Load()
	{
		if (!NOMOAENPKCP())
		{
			return;
		}
		lock (Locker)
		{
			if (JHEGDCAEDFM)
			{
				return;
			}
			ELIJOFFHEBP();
			try
			{
				FPFLODAGEFD.Clear();
				if (!Directory.Exists(BMMIHMPFOOA()))
				{
					Directory.CreateDirectory(BMMIHMPFOOA());
				}
				if (!File.Exists(HIJIPJKMFNH()))
				{
					return;
				}
				using (FileStream input = new FileStream(HIJIPJKMFNH(), FileMode.Open))
				{
					using (BinaryReader binaryReader = new BinaryReader(input))
					{
						binaryReader.ReadInt32();
						int num = binaryReader.ReadInt32();
						for (int i = 0; i < num; i++)
						{
							Cookie eKAOIOLAGFH = new Cookie();
							eKAOIOLAGFH.LoadFrom(binaryReader);
							if (eKAOIOLAGFH.IDGHLAOFMEO())
							{
								FPFLODAGEFD.Add(eKAOIOLAGFH);
							}
						}
					}
				}
			}
			catch
			{
				FPFLODAGEFD.Clear();
			}
			finally
			{
				JHEGDCAEDFM = true;
			}
		}
	}

	public static List<Cookie> Get(Uri KJHNCLAJMLO)
	{
		lock (Locker)
		{
			Load();
			List<Cookie> list = null;
			for (int i = 0; i < FPFLODAGEFD.Count; i++)
			{
				Cookie eKAOIOLAGFH = FPFLODAGEFD[i];
				if (eKAOIOLAGFH.IDGHLAOFMEO() && KJHNCLAJMLO.Host.IndexOf(eKAOIOLAGFH.PILMIFGDMCK()) != -1 && KJHNCLAJMLO.AbsolutePath.StartsWith(eKAOIOLAGFH.DEIEDODNANN()))
				{
					if (list == null)
					{
						list = new List<Cookie>();
					}
					list.Add(eKAOIOLAGFH);
				}
			}
			return list;
		}
	}

	public static void Set(Uri KJHNCLAJMLO, Cookie ILHDJDNPFKH)
	{
		lock (Locker)
		{
			Load();
			Cookie eKAOIOLAGFH = new Cookie(ILHDJDNPFKH.get_Name(), ILHDJDNPFKH.OEAKCOHMIHH(), KJHNCLAJMLO.AbsolutePath, KJHNCLAJMLO.Host);
			int OOPOEMNCCGH;
			EKGMPKKAAKB(eKAOIOLAGFH, out OOPOEMNCCGH);
			if (OOPOEMNCCGH >= 0)
			{
				FPFLODAGEFD[OOPOEMNCCGH] = eKAOIOLAGFH;
			}
			else
			{
				FPFLODAGEFD.Add(eKAOIOLAGFH);
			}
		}
	}

	public static List<Cookie> CDFJFIJHDOM()
	{
		lock (Locker)
		{
			Load();
			return FPFLODAGEFD;
		}
	}

	public static void Clear()
	{
		lock (Locker)
		{
			Load();
			FPFLODAGEFD.Clear();
		}
	}

	public static void Clear(TimeSpan HJALDNILENB)
	{
		lock (Locker)
		{
			Load();
			int num = 0;
			while (num < FPFLODAGEFD.Count)
			{
				Cookie eKAOIOLAGFH = FPFLODAGEFD[num];
				if (!eKAOIOLAGFH.IDGHLAOFMEO() || eKAOIOLAGFH.DJPNPAGCDKB() + HJALDNILENB < DateTime.UtcNow)
				{
					FPFLODAGEFD.RemoveAt(num);
				}
				else
				{
					num++;
				}
			}
		}
	}

	public static void Clear(string OKDDNOHODMN)
	{
		lock (Locker)
		{
			Load();
			int num = 0;
			while (num < FPFLODAGEFD.Count)
			{
				Cookie eKAOIOLAGFH = FPFLODAGEFD[num];
				if (!eKAOIOLAGFH.IDGHLAOFMEO() || eKAOIOLAGFH.PILMIFGDMCK().IndexOf(OKDDNOHODMN) != -1)
				{
					FPFLODAGEFD.RemoveAt(num);
				}
				else
				{
					num++;
				}
			}
		}
	}

	public static void Remove(Uri KJHNCLAJMLO, string name)
	{
		lock (Locker)
		{
			Load();
			int num = 0;
			while (num < FPFLODAGEFD.Count)
			{
				Cookie eKAOIOLAGFH = FPFLODAGEFD[num];
				if (eKAOIOLAGFH.get_Name().Equals(name, StringComparison.OrdinalIgnoreCase) && KJHNCLAJMLO.Host.IndexOf(eKAOIOLAGFH.PILMIFGDMCK()) != -1)
				{
					FPFLODAGEFD.RemoveAt(num);
				}
				else
				{
					num++;
				}
			}
		}
	}

	private static Cookie EKGMPKKAAKB(Cookie FJKPPODBPJF, out int OOPOEMNCCGH)
	{
		for (int i = 0; i < FPFLODAGEFD.Count; i++)
		{
			Cookie eKAOIOLAGFH = FPFLODAGEFD[i];
			if (eKAOIOLAGFH.Equals(FJKPPODBPJF))
			{
				OOPOEMNCCGH = i;
				return eKAOIOLAGFH;
			}
		}
		OOPOEMNCCGH = -1;
		return null;
	}
}
