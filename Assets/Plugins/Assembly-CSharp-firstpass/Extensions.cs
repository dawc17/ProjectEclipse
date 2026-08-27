using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class Extensions
{
	public static string JBAOFMBHJND(this byte[] KPAMPCLHCEN)
	{
		StringBuilder stringBuilder = new StringBuilder(KPAMPCLHCEN.Length);
		foreach (byte b in KPAMPCLHCEN)
		{
			stringBuilder.Append((char)((b > 127) ? 63 : b));
		}
		return stringBuilder.ToString();
	}

	public static byte[] GetASCIIBytes(this string IGGFGLLIGCG)
	{
		byte[] array = new byte[IGGFGLLIGCG.Length];
		for (int i = 0; i < IGGFGLLIGCG.Length; i++)
		{
			char c = IGGFGLLIGCG[i];
			array[i] = (byte)((c >= '\u0080') ? '?' : c);
		}
		return array;
	}

	public static void SendAsASCII(this BinaryWriter ABJIEFMMIEK, string IGGFGLLIGCG)
	{
		foreach (char c in IGGFGLLIGCG)
		{
			ABJIEFMMIEK.Write((byte)((c >= '\u0080') ? '?' : c));
		}
	}

	public static void WriteLine(this FileStream MEHMICNAPMK)
	{
		MEHMICNAPMK.Write(HTTPRequest.HGBANJPCEPF, 0, 2);
	}

	public static void WriteLine(this FileStream MEHMICNAPMK, string MGPBPJOHMLH)
	{
		byte[] array = MGPBPJOHMLH.GetASCIIBytes();
		MEHMICNAPMK.Write(array, 0, array.Length);
		MEHMICNAPMK.WriteLine();
	}

	public static void WriteLine(this FileStream MEHMICNAPMK, string LBOHOKIBHOH, params object[] AMMFNLMJJFM)
	{
		byte[] array = string.Format(LBOHOKIBHOH, AMMFNLMJJFM).GetASCIIBytes();
		MEHMICNAPMK.Write(array, 0, array.Length);
		MEHMICNAPMK.WriteLine();
	}

	public static string[] FindOption(this string IGGFGLLIGCG, string LFJBBPIDBCL)
	{
		string[] array = IGGFGLLIGCG.ToLower().Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
		LFJBBPIDBCL = LFJBBPIDBCL.ToLower();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].Contains(LFJBBPIDBCL))
			{
				return array[i].Split(new char[1] { '=' }, StringSplitOptions.RemoveEmptyEntries);
			}
		}
		return null;
	}

	public static int ToInt32(this string IGGFGLLIGCG, int OBPKDHBJKJL = 0)
	{
		if (IGGFGLLIGCG == null)
		{
			return OBPKDHBJKJL;
		}
		try
		{
			return int.Parse(IGGFGLLIGCG);
		}
		catch
		{
			return OBPKDHBJKJL;
		}
	}

	public static long ToInt64(this string IGGFGLLIGCG, long OBPKDHBJKJL = 0L)
	{
		if (IGGFGLLIGCG == null)
		{
			return OBPKDHBJKJL;
		}
		try
		{
			return long.Parse(IGGFGLLIGCG);
		}
		catch
		{
			return OBPKDHBJKJL;
		}
	}

	public static DateTime ToDateTime(this string IGGFGLLIGCG, DateTime OBPKDHBJKJL = default(DateTime))
	{
		if (IGGFGLLIGCG == null)
		{
			return OBPKDHBJKJL;
		}
		try
		{
			DateTime.TryParse(IGGFGLLIGCG, out OBPKDHBJKJL);
			return OBPKDHBJKJL.ToUniversalTime();
		}
		catch
		{
			return OBPKDHBJKJL;
		}
	}

	public static string PKBHGNMGNNO(this string IGGFGLLIGCG)
	{
		if (IGGFGLLIGCG == null)
		{
			return string.Empty;
		}
		return IGGFGLLIGCG;
	}

	public static string DAOJIBHMOJK(this string NILNDHEKNLJ)
	{
		return NILNDHEKNLJ.GetASCIIBytes().DAOJIBHMOJK();
	}

	public static string DAOJIBHMOJK(this byte[] NILNDHEKNLJ)
	{
		byte[] array = MD5.Create().ComputeHash(NILNDHEKNLJ);
		StringBuilder stringBuilder = new StringBuilder();
		byte[] array2 = array;
		foreach (byte b in array2)
		{
			stringBuilder.Append(b.ToString("x2"));
		}
		return stringBuilder.ToString();
	}

	internal static string Read(this string IGGFGLLIGCG, ref int LCCLEFMKLPB, char JILGHDDEMPE, bool EEIONCIPIIE = true)
	{
		return IGGFGLLIGCG.Read(ref LCCLEFMKLPB, (char KDFCGMMKAME) => KDFCGMMKAME != JILGHDDEMPE, EEIONCIPIIE);
	}

	internal static string Read(this string IGGFGLLIGCG, ref int LCCLEFMKLPB, Func<char, bool> JILGHDDEMPE, bool EEIONCIPIIE = true)
	{
		if (LCCLEFMKLPB >= IGGFGLLIGCG.Length)
		{
			return string.Empty;
		}
		IGGFGLLIGCG.SkipWhiteSpace(ref LCCLEFMKLPB);
		int num = LCCLEFMKLPB;
		while (LCCLEFMKLPB < IGGFGLLIGCG.Length && JILGHDDEMPE(IGGFGLLIGCG[LCCLEFMKLPB]))
		{
			LCCLEFMKLPB++;
		}
		string result = ((!EEIONCIPIIE) ? null : IGGFGLLIGCG.Substring(num, LCCLEFMKLPB - num));
		LCCLEFMKLPB++;
		return result;
	}

	internal static string ReadQuotedText(this string IGGFGLLIGCG, ref int LCCLEFMKLPB)
	{
		string empty = string.Empty;
		if (IGGFGLLIGCG == null)
		{
			return empty;
		}
		if (IGGFGLLIGCG[LCCLEFMKLPB] == '"')
		{
			IGGFGLLIGCG.Read(ref LCCLEFMKLPB, '"', false);
			empty = IGGFGLLIGCG.Read(ref LCCLEFMKLPB, '"');
			IGGFGLLIGCG.Read(ref LCCLEFMKLPB, ',', false);
		}
		else
		{
			empty = IGGFGLLIGCG.Read(ref LCCLEFMKLPB, ',');
		}
		return empty;
	}

	internal static void SkipWhiteSpace(this string IGGFGLLIGCG, ref int LCCLEFMKLPB)
	{
		if (LCCLEFMKLPB < IGGFGLLIGCG.Length)
		{
			while (LCCLEFMKLPB < IGGFGLLIGCG.Length && char.IsWhiteSpace(IGGFGLLIGCG[LCCLEFMKLPB]))
			{
				LCCLEFMKLPB++;
			}
		}
	}

	internal static string JONPEPOKJFC(this string IGGFGLLIGCG)
	{
		if (IGGFGLLIGCG == null)
		{
			return null;
		}
		char[] array = new char[IGGFGLLIGCG.Length];
		int length = 0;
		foreach (char c in IGGFGLLIGCG)
		{
			if (!char.IsWhiteSpace(c) && !char.IsControl(c))
			{
				array[length++] = char.ToLowerInvariant(c);
			}
		}
		return new string(array, 0, length);
	}

	internal static List<KeyValuePair> IAFOBCFEJPH(this string IGGFGLLIGCG)
	{
		List<KeyValuePair> list = new List<KeyValuePair>();
		if (IGGFGLLIGCG == null)
		{
			return list;
		}
		int LCCLEFMKLPB = 0;
		while (LCCLEFMKLPB < IGGFGLLIGCG.Length)
		{
			string kGBGENDIMBC = IGGFGLLIGCG.Read(ref LCCLEFMKLPB, (char KDFCGMMKAME) => KDFCGMMKAME != '=' && KDFCGMMKAME != ',').JONPEPOKJFC();
			KeyValuePair gGCJLGPPHKP = new KeyValuePair(kGBGENDIMBC);
			if (IGGFGLLIGCG[LCCLEFMKLPB - 1] == '=')
			{
				gGCJLGPPHKP.set_Value(IGGFGLLIGCG.ReadQuotedText(ref LCCLEFMKLPB));
			}
			list.Add(gGCJLGPPHKP);
		}
		return list;
	}

	internal static List<KeyValuePair> MNFHBLMMGPE(this string IGGFGLLIGCG)
	{
		List<KeyValuePair> list = new List<KeyValuePair>();
		if (IGGFGLLIGCG == null)
		{
			return list;
		}
		int LCCLEFMKLPB = 0;
		while (LCCLEFMKLPB < IGGFGLLIGCG.Length)
		{
			string kGBGENDIMBC = IGGFGLLIGCG.Read(ref LCCLEFMKLPB, (char KDFCGMMKAME) => KDFCGMMKAME != ',' && KDFCGMMKAME != ';').JONPEPOKJFC();
			KeyValuePair gGCJLGPPHKP = new KeyValuePair(kGBGENDIMBC);
			if (IGGFGLLIGCG[LCCLEFMKLPB - 1] == ';')
			{
				IGGFGLLIGCG.Read(ref LCCLEFMKLPB, '=', false);
				gGCJLGPPHKP.set_Value(IGGFGLLIGCG.Read(ref LCCLEFMKLPB, ','));
			}
			list.Add(gGCJLGPPHKP);
		}
		return list;
	}

	public static void ReadBuffer(this Stream ABJIEFMMIEK, byte[] buffer)
	{
		int num = 0;
		do
		{
			num += ABJIEFMMIEK.Read(buffer, num, buffer.Length - num);
		}
		while (num < buffer.Length);
	}

	public static void WriteAll(this MemoryStream PKGAJCFLOLA, byte[] buffer)
	{
		PKGAJCFLOLA.Write(buffer, 0, buffer.Length);
	}

	public static void IHOOAEHGMFO(this MemoryStream PKGAJCFLOLA, string IGGFGLLIGCG)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(IGGFGLLIGCG);
		PKGAJCFLOLA.WriteAll(bytes);
	}

	public static void WriteLine(this MemoryStream PKGAJCFLOLA)
	{
		PKGAJCFLOLA.WriteAll(HTTPRequest.HGBANJPCEPF);
	}

	public static void WriteLine(this MemoryStream PKGAJCFLOLA, string IGGFGLLIGCG)
	{
		PKGAJCFLOLA.IHOOAEHGMFO(IGGFGLLIGCG);
		PKGAJCFLOLA.WriteLine();
	}
}
