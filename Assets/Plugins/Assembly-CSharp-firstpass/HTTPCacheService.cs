using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;

public static class HTTPCacheService
{
	private const int LibraryVersion = 2;

	private static bool BIJLAGAGJJP;

	private static bool PFEOIGKNHKB;

	private static Dictionary<Uri, HTTPCacheFileInfo> DJPHBDDJMOJ;

	private static Dictionary<ulong, HTTPCacheFileInfo> DALAFDNPDGB;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static string JEBOIEBCAJE;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static string OFLEPKHJLFB;

	private static bool GIDPNJIHGFN;

	private static bool PLPAFAKMOBF;

	private static ulong NextNameIDX;

	public static bool ABFAHBGHFOB
	{
		get
		{
			return EPACOIFEICA();
		}
	}

	private static Dictionary<Uri, HTTPCacheFileInfo> OHNIOLNCGPP
	{
		get
		{
			return MGHJNDBKNEC();
		}
	}

	internal static string KCHHBBGFGIG
	{
		get
		{
			return MJJLGGBAMJE();
		}
		private set
		{
			LMFJMCCBLBJ(value);
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

	static HTTPCacheService()
	{
		DALAFDNPDGB = new Dictionary<ulong, HTTPCacheFileInfo>();
		NextNameIDX = 1uL;
	}

	public static bool EPACOIFEICA()
	{
		if (PFEOIGKNHKB)
		{
			return BIJLAGAGJJP;
		}
		try
		{
			File.Exists(HTTPManager.DJHDCCJDJGJ());
			BIJLAGAGJJP = true;
		}
		catch
		{
			BIJLAGAGJJP = false;
			HTTPManager.MBBMPNDDPIH().GLCKHLCAPIN("HTTPCacheService", "Cache Service Disabled!");
		}
		finally
		{
			PFEOIGKNHKB = true;
		}
		return BIJLAGAGJJP;
	}

	private static Dictionary<Uri, HTTPCacheFileInfo> MGHJNDBKNEC()
	{
		IOBILFLFODD();
		return DJPHBDDJMOJ;
	}

	internal static string MJJLGGBAMJE()
	{
		return JEBOIEBCAJE;
	}

	private static void LMFJMCCBLBJ(string value)
	{
		JEBOIEBCAJE = value;
	}

	private static string HIJIPJKMFNH()
	{
		return OFLEPKHJLFB;
	}

	private static void DBLJMNJMAHF(string value)
	{
		OFLEPKHJLFB = value;
	}

	internal static void BEGBHCIIOAO()
	{
		if (!EPACOIFEICA())
		{
			return;
		}
		try
		{
			KDACPEKJHPP();
			IOBILFLFODD();
		}
		catch
		{
		}
	}

	internal static void KDACPEKJHPP()
	{
		if (!EPACOIFEICA())
		{
			return;
		}
		try
		{
			if (string.IsNullOrEmpty(MJJLGGBAMJE()) || string.IsNullOrEmpty(HIJIPJKMFNH()))
			{
				LMFJMCCBLBJ(Path.Combine(HTTPManager.DJHDCCJDJGJ(), "HTTPCache"));
				if (!Directory.Exists(MJJLGGBAMJE()))
				{
					Directory.CreateDirectory(MJJLGGBAMJE());
				}
				DBLJMNJMAHF(Path.Combine(HTTPManager.DJHDCCJDJGJ(), "Library"));
			}
		}
		catch
		{
		}
	}

	internal static ulong OKGJODFOLBM()
	{
		lock (MGHJNDBKNEC())
		{
			ulong kDIGFPPHJDL = NextNameIDX;
			do
			{
				NextNameIDX = ++NextNameIDX % ulong.MaxValue;
			}
			while (DALAFDNPDGB.ContainsKey(NextNameIDX));
			return kDIGFPPHJDL;
		}
	}

	internal static bool HasEntity(Uri KJHNCLAJMLO)
	{
		if (!EPACOIFEICA())
		{
			return false;
		}
		lock (MGHJNDBKNEC())
		{
			return MGHJNDBKNEC().ContainsKey(KJHNCLAJMLO);
		}
	}

	internal static bool DeleteEntity(Uri KJHNCLAJMLO, bool IPHPJPNKPMD = true)
	{
		if (!EPACOIFEICA())
		{
			return false;
		}
		object obj = HTTPCacheFileLock.Acquire(KJHNCLAJMLO);
		lock (obj)
		{
			try
			{
				lock (MGHJNDBKNEC())
				{
					HTTPCacheFileInfo value;
					bool flag = MGHJNDBKNEC().TryGetValue(KJHNCLAJMLO, out value);
					if (flag)
					{
						value.JEHBFCLLPCL();
					}
					if (flag && IPHPJPNKPMD)
					{
						MGHJNDBKNEC().Remove(KJHNCLAJMLO);
						DALAFDNPDGB.Remove(value.KDBDNGPOENN());
					}
					return true;
				}
			}
			finally
			{
			}
		}
	}

	internal static bool BEBFIMACMEK(HTTPRequest ONOCIELLAPL)
	{
		if (!EPACOIFEICA())
		{
			return false;
		}
		lock (MGHJNDBKNEC())
		{
			HTTPCacheFileInfo value;
			if (MGHJNDBKNEC().TryGetValue(ONOCIELLAPL.DKAECMGPGOE(), out value))
			{
				return value.IDGHLAOFMEO();
			}
		}
		return false;
	}

	internal static void JGLDNKPBBGC(HTTPRequest ONOCIELLAPL)
	{
		if (!EPACOIFEICA())
		{
			return;
		}
		lock (MGHJNDBKNEC())
		{
			HTTPCacheFileInfo value;
			if (MGHJNDBKNEC().TryGetValue(ONOCIELLAPL.DKAECMGPGOE(), out value))
			{
				value.EKLOPCNGGED(ONOCIELLAPL);
			}
		}
	}

	internal static Stream GetBody(Uri KJHNCLAJMLO, out int BDBOAEGELMC)
	{
		BDBOAEGELMC = 0;
		if (!EPACOIFEICA())
		{
			return null;
		}
		lock (MGHJNDBKNEC())
		{
			HTTPCacheFileInfo value;
			if (MGHJNDBKNEC().TryGetValue(KJHNCLAJMLO, out value))
			{
				return value.GetBodyStream(out BDBOAEGELMC);
			}
		}
		return null;
	}

	internal static HTTPResponse HLLKJACMILI(HTTPRequest ONOCIELLAPL)
	{
		if (!EPACOIFEICA())
		{
			return null;
		}
		lock (MGHJNDBKNEC())
		{
			HTTPCacheFileInfo value;
			if (MGHJNDBKNEC().TryGetValue(ONOCIELLAPL.DKAECMGPGOE(), out value))
			{
				return value.IFPJHJFHJDK(ONOCIELLAPL);
			}
		}
		return null;
	}

	internal static bool HCCGCAKPOGB(Uri KJHNCLAJMLO, LAAFHDKKJFL FJLOLCPJACB, HTTPResponse GIHDDAKBMHE)
	{
		if (!EPACOIFEICA())
		{
			return false;
		}
		if (FJLOLCPJACB != LAAFHDKKJFL.Get)
		{
			return false;
		}
		if (GIHDDAKBMHE == null)
		{
			return false;
		}
		if (GIHDDAKBMHE.KNMDPGBPNED() == 304)
		{
			return false;
		}
		if (GIHDDAKBMHE.KNMDPGBPNED() < 200 || GIHDDAKBMHE.KNMDPGBPNED() >= 400)
		{
			return false;
		}
		List<string> list = GIHDDAKBMHE.GetHeaderValues("cache-control");
		if (list != null && list.Exists((string PNJNBBFLCAH) =>
		{
			string text = PNJNBBFLCAH.ToLower();
			return text.Contains("no-store") || text.Contains("no-cache");
		}))
		{
			return false;
		}
		List<string> list2 = GIHDDAKBMHE.GetHeaderValues("pragma");
		if (list2 != null && list2.Exists((string PNJNBBFLCAH) =>
		{
			string text = PNJNBBFLCAH.ToLower();
			return text.Contains("no-store") || text.Contains("no-cache");
		}))
		{
			return false;
		}
		List<string> list3 = GIHDDAKBMHE.GetHeaderValues("content-range");
		if (list3 != null)
		{
			return false;
		}
		return true;
	}

	internal static HTTPCacheFileInfo LDFKMIOPLKA(Uri KJHNCLAJMLO, LAAFHDKKJFL FJLOLCPJACB, HTTPResponse GIHDDAKBMHE)
	{
		if (GIHDDAKBMHE == null || GIHDDAKBMHE.CHIGLEKCFFN() == null || GIHDDAKBMHE.CHIGLEKCFFN().Length == 0)
		{
			return null;
		}
		if (!EPACOIFEICA())
		{
			return null;
		}
		HTTPCacheFileInfo value = null;
		lock (MGHJNDBKNEC())
		{
			if (!MGHJNDBKNEC().TryGetValue(KJHNCLAJMLO, out value))
			{
				MGHJNDBKNEC().Add(KJHNCLAJMLO, value = new HTTPCacheFileInfo(KJHNCLAJMLO));
				DALAFDNPDGB.Add(value.KDBDNGPOENN(), value);
			}
			try
			{
				value.LDFKMIOPLKA(GIHDDAKBMHE);
				return value;
			}
			catch
			{
				DeleteEntity(KJHNCLAJMLO);
				throw;
			}
		}
	}

	internal static Stream KHGFDHOJOOG(Uri KJHNCLAJMLO, HTTPResponse GIHDDAKBMHE)
	{
		if (!EPACOIFEICA())
		{
			return null;
		}
		lock (MGHJNDBKNEC())
		{
			HTTPCacheFileInfo value;
			if (!MGHJNDBKNEC().TryGetValue(KJHNCLAJMLO, out value))
			{
				MGHJNDBKNEC().Add(KJHNCLAJMLO, value = new HTTPCacheFileInfo(KJHNCLAJMLO));
				DALAFDNPDGB.Add(value.KDBDNGPOENN(), value);
			}
			try
			{
				return value.GOGEGGPBPBP(GIHDDAKBMHE);
			}
			catch
			{
				DeleteEntity(KJHNCLAJMLO);
				throw;
			}
		}
	}

	public static void ACMOEBGCPML()
	{
		if (EPACOIFEICA() && !GIDPNJIHGFN)
		{
			GIDPNJIHGFN = true;
			KDACPEKJHPP();
			new Thread(ClearImpl).Start();
		}
	}

	private static void ClearImpl(object KKNOCIPBIIK)
	{
		if (!EPACOIFEICA())
		{
			return;
		}
		try
		{
			string[] files = Directory.GetFiles(MJJLGGBAMJE());
			for (int i = 0; i < files.Length; i++)
			{
				try
				{
					File.Delete(files[i]);
				}
				catch
				{
				}
			}
		}
		finally
		{
			DALAFDNPDGB.Clear();
			DJPHBDDJMOJ.Clear();
			NextNameIDX = 1uL;
			FIMLABMLKJF();
			GIDPNJIHGFN = false;
		}
	}

	public static void JJFFGOABNOA(HTTPCacheMaintananceParams HFBDNDCABLM)
	{
		if (HFBDNDCABLM == null)
		{
			throw new ArgumentNullException("maintananceParams == null");
		}
		if (!EPACOIFEICA() || PLPAFAKMOBF)
		{
			return;
		}
		PLPAFAKMOBF = true;
		KDACPEKJHPP();
		new Thread((object KKNOCIPBIIK) =>
		{
			try
			{
				lock (MGHJNDBKNEC())
				{
					DateTime dateTime = DateTime.UtcNow - HFBDNDCABLM.DKAPKJDOAEJ();
					List<HTTPCacheFileInfo> list = new List<HTTPCacheFileInfo>();
					foreach (KeyValuePair<Uri, HTTPCacheFileInfo> item in MGHJNDBKNEC())
					{
						if (item.Value.PPHKANGFLHJ() < dateTime && DeleteEntity(item.Key, false))
						{
							list.Add(item.Value);
						}
					}
					for (int i = 0; i < list.Count; i++)
					{
						MGHJNDBKNEC().Remove(list[i].OJBDMGBGJMA());
						DALAFDNPDGB.Remove(list[i].KDBDNGPOENN());
					}
					list.Clear();
					ulong num = NKOHKEGHKJN();
					if (num > HFBDNDCABLM.GAJGIIJPPBF())
					{
						List<HTTPCacheFileInfo> list2 = new List<HTTPCacheFileInfo>(DJPHBDDJMOJ.Count);
						foreach (KeyValuePair<Uri, HTTPCacheFileInfo> item2 in DJPHBDDJMOJ)
						{
							list2.Add(item2.Value);
						}
						list2.Sort();
						int num2 = 0;
						while (num >= HFBDNDCABLM.GAJGIIJPPBF() && num2 < list2.Count)
						{
							try
							{
								HTTPCacheFileInfo aEMMGBPFAHD = list2[num2];
								ulong num3 = (ulong)aEMMGBPFAHD.NHCAEHOHBEE();
								DeleteEntity(aEMMGBPFAHD.OJBDMGBGJMA());
								num -= num3;
							}
							catch
							{
							}
							finally
							{
								num2++;
							}
						}
					}
				}
			}
			finally
			{
				FIMLABMLKJF();
				PLPAFAKMOBF = false;
			}
		}).Start();
	}

	public static int MDIKHCGGACM()
	{
		if (!EPACOIFEICA())
		{
			return 0;
		}
		BEGBHCIIOAO();
		lock (MGHJNDBKNEC())
		{
			return MGHJNDBKNEC().Count;
		}
	}

	public static ulong NKOHKEGHKJN()
	{
		ulong num = 0uL;
		if (!EPACOIFEICA())
		{
			return num;
		}
		BEGBHCIIOAO();
		lock (MGHJNDBKNEC())
		{
			foreach (KeyValuePair<Uri, HTTPCacheFileInfo> item in MGHJNDBKNEC())
			{
				if (item.Value.NHCAEHOHBEE() > 0)
				{
					num += (ulong)item.Value.NHCAEHOHBEE();
				}
			}
			return num;
		}
	}

	private static void IOBILFLFODD()
	{
		if (DJPHBDDJMOJ != null || !EPACOIFEICA())
		{
			return;
		}
		DJPHBDDJMOJ = new Dictionary<Uri, HTTPCacheFileInfo>();
		if (!File.Exists(HIJIPJKMFNH()))
		{
			LKBJMMEEHPB();
			return;
		}
		try
		{
			int num;
			lock (DJPHBDDJMOJ)
			{
				using (FileStream input = new FileStream(HIJIPJKMFNH(), FileMode.Open))
				{
					using (BinaryReader binaryReader = new BinaryReader(input))
					{
						num = binaryReader.ReadInt32();
						if (num > 1)
						{
							NextNameIDX = binaryReader.ReadUInt64();
						}
						int num2 = binaryReader.ReadInt32();
						for (int i = 0; i < num2; i++)
						{
							Uri uri = new Uri(binaryReader.ReadString());
							HTTPCacheFileInfo aEMMGBPFAHD = new HTTPCacheFileInfo(uri, binaryReader, num);
							if (aEMMGBPFAHD.IsExists())
							{
								DJPHBDDJMOJ.Add(uri, aEMMGBPFAHD);
								if (num > 1)
								{
									DALAFDNPDGB.Add(aEMMGBPFAHD.KDBDNGPOENN(), aEMMGBPFAHD);
								}
							}
						}
					}
				}
			}
			if (num == 1)
			{
				ACMOEBGCPML();
			}
			else
			{
				LKBJMMEEHPB();
			}
		}
		catch
		{
		}
	}

	internal static void FIMLABMLKJF()
	{
		if (DJPHBDDJMOJ == null || !EPACOIFEICA())
		{
			return;
		}
		try
		{
			lock (MGHJNDBKNEC())
			{
				using (FileStream output = new FileStream(HIJIPJKMFNH(), FileMode.Create))
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(output))
					{
						binaryWriter.Write(2);
						binaryWriter.Write(NextNameIDX);
						binaryWriter.Write(MGHJNDBKNEC().Count);
						foreach (KeyValuePair<Uri, HTTPCacheFileInfo> item in MGHJNDBKNEC())
						{
							binaryWriter.Write(item.Key.ToString());
							item.Value.SaveTo(binaryWriter);
						}
					}
				}
			}
		}
		catch
		{
		}
	}

	internal static void SetBodyLength(Uri KJHNCLAJMLO, int DEFBMELCOHO)
	{
		if (!EPACOIFEICA())
		{
			return;
		}
		lock (MGHJNDBKNEC())
		{
			HTTPCacheFileInfo value;
			if (MGHJNDBKNEC().TryGetValue(KJHNCLAJMLO, out value))
			{
				value.set_BodyLength(DEFBMELCOHO);
				return;
			}
			MGHJNDBKNEC().Add(KJHNCLAJMLO, value = new HTTPCacheFileInfo(KJHNCLAJMLO, DateTime.UtcNow, DEFBMELCOHO));
			DALAFDNPDGB.Add(value.KDBDNGPOENN(), value);
		}
	}

	private static void LKBJMMEEHPB()
	{
		if (!EPACOIFEICA())
		{
			return;
		}
		BEGBHCIIOAO();
		string[] files = Directory.GetFiles(MJJLGGBAMJE());
		for (int i = 0; i < files.Length; i++)
		{
			try
			{
				string fileName = Path.GetFileName(files[i]);
				ulong result = 0uL;
				bool flag = false;
				if (ulong.TryParse(fileName, NumberStyles.AllowHexSpecifier, null, out result))
				{
					lock (MGHJNDBKNEC())
					{
						flag = !DALAFDNPDGB.ContainsKey(result);
					}
				}
				else
				{
					flag = true;
				}
				if (flag)
				{
					File.Delete(files[i]);
				}
			}
			catch
			{
			}
		}
	}
}
