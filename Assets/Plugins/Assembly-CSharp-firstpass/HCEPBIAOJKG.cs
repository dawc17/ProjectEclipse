using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

public class HCEPBIAOJKG
{
	public static void OOMKKNBMFDG(string path, byte[] KPAMPCLHCEN)
	{
		KOIHJKJFAMJ(Path.GetDirectoryName(path));
		File.WriteAllBytes(path, KPAMPCLHCEN);
	}

	public static void BJKNGNMEDOI(string path, string DMNBDBJNKME = "")
	{
		OMNHJLFLCNL(path, DMNBDBJNKME, true);
	}

	public static void OMNHJLFLCNL(string path, string DMNBDBJNKME = "", bool OGOAJGKFMPF = false)
	{
		KOIHJKJFAMJ(Path.GetDirectoryName(path));
		if (!OGOAJGKFMPF && GFBMBNAIJEJ(path))
		{
			File.AppendAllText(path, DMNBDBJNKME);
		}
		else
		{
			File.WriteAllText(path, DMNBDBJNKME);
		}
	}

	public static void EDAIFPALMAP(string path, Action<TextWriter> IBODMPMJELJ, bool FBLOBGLPAFJ = false)
	{
		using (TextWriter bAINMLLIKOL = EDAIFPALMAP(path, FBLOBGLPAFJ))
		{
			IBODMPMJELJ.FEEGJDJIFEF(bAINMLLIKOL);
		}
	}

	public static TextWriter EDAIFPALMAP(string path, bool FBLOBGLPAFJ = false)
	{
		KOIHJKJFAMJ(Path.GetDirectoryName(path));
		return new StreamWriter(path, FBLOBGLPAFJ, Encoding.UTF8);
	}

	public static FileStream BNIOOOBANEN(string path)
	{
		FileMode nMMPBADCFHK = ((!GFBMBNAIJEJ(path)) ? FileMode.OpenOrCreate : FileMode.Append);
		return CMGHOEJJOPN(path, nMMPBADCFHK);
	}

	public static void CMGHOEJJOPN(string path, Action<FileStream> IBODMPMJELJ, FileMode NMMPBADCFHK = FileMode.OpenOrCreate, FileAccess HIOFGOHJANN = FileAccess.Write)
	{
		using (FileStream bAINMLLIKOL = CMGHOEJJOPN(path, NMMPBADCFHK, HIOFGOHJANN))
		{
			IBODMPMJELJ.FEEGJDJIFEF(bAINMLLIKOL);
		}
	}

	public static FileStream CMGHOEJJOPN(string path, FileMode NMMPBADCFHK = FileMode.OpenOrCreate, FileAccess HIOFGOHJANN = FileAccess.Write)
	{
		KOIHJKJFAMJ(Path.GetDirectoryName(path));
		return new FileStream(path, NMMPBADCFHK, HIOFGOHJANN);
	}

	public static void CFMNELHADHL(string path, object AOMLCBHAJJH, Formatting LFHMGPBFEPI = Formatting.None)
	{
		try
		{
			string dMNBDBJNKME = JsonConvert.SerializeObject(AOMLCBHAJJH, LFHMGPBFEPI);
			BJKNGNMEDOI(path, dMNBDBJNKME);
		}
		catch (Exception ex)
		{
			Debug.LogError("Error SaveJSON config [" + ex.Message + "]");
		}
	}

	public static T ECLCGODJFBM<T>(string path) where T : class
	{
		string text = AOLDPEFEBEK(path);
		if (!text.BKOIKMEEHDK())
		{
			try
			{
				return JsonConvert.DeserializeObject<T>(text);
			}
			catch (Exception ex)
			{
				Debug.LogError("Error ReadFileJson [" + path + " " + ex.Message + "]");
			}
		}
		return (T)null;
	}

	public static string AOLDPEFEBEK(string path)
	{
		if (GFBMBNAIJEJ(path))
		{
			return File.ReadAllText(path);
		}
		return null;
	}

	public static string[] AFGFEBDBOBH(string path)
	{
		if (GFBMBNAIJEJ(path))
		{
			return File.ReadAllLines(path);
		}
		return null;
	}

	public static byte[] OEPBCILIGPI(string path)
	{
		if (GFBMBNAIJEJ(path))
		{
			return File.ReadAllBytes(path);
		}
		return null;
	}

	public static bool GFBMBNAIJEJ(string path)
	{
		return File.Exists(path);
	}

	public static bool OAKKAIDCHCO(string path)
	{
		return Directory.Exists(path);
	}

	public static bool BKACCHENJPK(string path)
	{
		FileInfo fileInfo = new FileInfo(path);
		FileStream fileStream = null;
		try
		{
			fileStream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.None);
		}
		catch (FileNotFoundException)
		{
			return false;
		}
		catch (IOException)
		{
			return true;
		}
		finally
		{
			if (fileStream != null)
			{
				fileStream.Close();
			}
		}
		return false;
	}

	public static bool BKLIKICKDPH(string path)
	{
		if (GFBMBNAIJEJ(path) && !BKACCHENJPK(path))
		{
			File.Delete(path);
			return true;
		}
		return false;
	}

	public static bool JALLPPJHCEA(string OOFLNBMPPID, string FMEOELPPAFJ)
	{
		if (GFBMBNAIJEJ(OOFLNBMPPID) && !BKACCHENJPK(OOFLNBMPPID))
		{
			BKLIKICKDPH(FMEOELPPAFJ);
			File.Move(OOFLNBMPPID, FMEOELPPAFJ);
			return true;
		}
		return false;
	}

	public static bool OCCBJAMEGNJ(string OOFLNBMPPID, string FMEOELPPAFJ)
	{
		if (GFBMBNAIJEJ(OOFLNBMPPID))
		{
			KOIHJKJFAMJ(Path.GetDirectoryName(FMEOELPPAFJ));
			File.Copy(OOFLNBMPPID, FMEOELPPAFJ);
			return true;
		}
		return false;
	}

	public static bool JOOGEILKLIC(string path)
	{
		if (OAKKAIDCHCO(path))
		{
			Directory.Delete(path, true);
			return true;
		}
		return false;
	}

	public static void KOIHJKJFAMJ(string path)
	{
		if (!OAKKAIDCHCO(path))
		{
			Directory.CreateDirectory(path);
		}
	}
}
