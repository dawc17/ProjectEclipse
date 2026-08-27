using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public class UserInfo
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string MACAPENACJP;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string CMIPHMMCMDH;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string EOICEMDPEEO;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string GGBOJPFJBJH;

	public string BMFLPBLAFLK
	{
		get
		{
			return FJANLLCDPCP();
		}
		private set
		{
			DJJONJEKILE(value);
		}
	}

	public string FMOKLKFCCKF
	{
		get
		{
			return GKKFLFIACMN();
		}
		private set
		{
			HEMAEFLGOOH(value);
		}
	}

	public string CPJJFKNOCJE
	{
		get
		{
			return CIHLLDHJLON();
		}
		private set
		{
			NFDAFCJJDCO(value);
		}
	}

	public string UserID
	{
		get
		{
			return NDLJPNCIJIP();
		}
		private set
		{
			MNNHKMOFMMK(value);
		}
	}

	internal UserInfo(string BBNKIBKPBLO)
	{
		string[] array = BBNKIBKPBLO.Split('|');
		MNNHKMOFMMK(array[0]);
		NFDAFCJJDCO(array[1]);
		if (array[2].Contains(" "))
		{
			string[] array2 = array[2].Split(' ');
			DJJONJEKILE(array2[0]);
			HEMAEFLGOOH(array2[1]);
		}
		else
		{
			DJJONJEKILE(array[2]);
			HEMAEFLGOOH(string.Empty);
		}
	}

	internal UserInfo(string MEEFALMGOMC, string NEEAGKJGKDM, string HGLELKMAJMJ, string PDJEDKAFEAK)
	{
		DJJONJEKILE(PDJEDKAFEAK);
		HEMAEFLGOOH(HGLELKMAJMJ);
		NFDAFCJJDCO(NEEAGKJGKDM);
		MNNHKMOFMMK(MEEFALMGOMC);
	}

	private UserInfo()
	{
	}

	public string FJANLLCDPCP()
	{
		return MACAPENACJP;
	}

	private void DJJONJEKILE(string value)
	{
		MACAPENACJP = value;
	}

	public string GKKFLFIACMN()
	{
		return CMIPHMMCMDH;
	}

	private void HEMAEFLGOOH(string value)
	{
		CMIPHMMCMDH = value;
	}

	public string CIHLLDHJLON()
	{
		return EOICEMDPEEO;
	}

	private void NFDAFCJJDCO(string value)
	{
		EOICEMDPEEO = value;
	}

	public string NDLJPNCIJIP()
	{
		return GGBOJPFJBJH;
	}

	private void MNNHKMOFMMK(string value)
	{
		GGBOJPFJBJH = value;
	}

	internal static Dictionary<string, UserInfo> GetInfos(string BBNKIBKPBLO)
	{
		Dictionary<string, UserInfo> dictionary = new Dictionary<string, UserInfo>();
		string[] array = BBNKIBKPBLO.Split(new char[1] { '^' }, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			UserInfo jPKEEFNNAAP = new UserInfo(array[i]);
			if (!dictionary.ContainsKey(jPKEEFNNAAP.NDLJPNCIJIP()))
			{
				dictionary.Add(jPKEEFNNAAP.NDLJPNCIJIP(), jPKEEFNNAAP);
			}
			else
			{
				dictionary[jPKEEFNNAAP.NDLJPNCIJIP()] = jPKEEFNNAAP;
			}
		}
		return dictionary;
	}

	[SpecialName]
	public static bool op_Implicit(UserInfo KEJDJHAGBMK)
	{
		return KEJDJHAGBMK != null;
	}
}
