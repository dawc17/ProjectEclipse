using System.IO;
using System.Xml;
using Nekki.SF2.Core.Exceptions;

public static class UserDataValidator
{
	private const string JKMJBLCHICB = ".hash";

	private const string APEMILJBKBG = "wqO+Qchj|r*QXg7o_KNmLYvpGHdSwqxwlQI2vy618KaD^Pwt-h3H8*uJ";

	private static bool _IsValid = true;

	public static bool GJHHGDAOHGK
	{
		get
		{
			return DINANCBOIMJ();
		}
	}

	public static string CEMABBPLAGK
	{
		get
		{
			return GHEHGBBNMNO();
		}
	}

	public static bool DINANCBOIMJ()
	{
		return _IsValid;
	}

	public static string GHEHGBBNMNO()
	{
		return SystemProperties.GLLJKPBHELE() + "wqO+Qchj|r*QXg7o_KNmLYvpGHdSwqxwlQI2vy618KaD^Pwt-h3H8*uJ";
	}

	public static bool CheckFileHash(XmlDocument LOBFDOKFJIP, string ONEIGMLOGDC)
	{
		if (!GameSettings.HCAJHNKLLGB())
		{
			return true;
		}
		string text = ReadHash(ONEIGMLOGDC + ".hash");
		if (string.IsNullOrEmpty(text))
		{
			_IsValid = false;
			throw new HackDetectedException("[UserDataValidator]: file is missing - " + Path.GetFileName(ONEIGMLOGDC + ".hash") + " !");
		}
		_IsValid = MD5Utils.HGHDINBJBAD(LOBFDOKFJIP.OuterXml, text, GHEHGBBNMNO());
		if (!_IsValid)
		{
			throw new HackDetectedException("[UserDataValidator]: incorrect file hash - " + Path.GetFileName(ONEIGMLOGDC) + " !");
		}
		return _IsValid;
	}

	private static string ReadHash(string ONEIGMLOGDC)
	{
		if (!File.Exists(ONEIGMLOGDC))
		{
			return null;
		}
		return File.ReadAllText(ONEIGMLOGDC);
	}

	public static void UpdateFileHash(string LOBFDOKFJIP)
	{
		if (GameSettings.HCAJHNKLLGB())
		{
			string iMMGBGKAMPK = MD5Utils.PIFDHBHOMJL(LOBFDOKFJIP, GHEHGBBNMNO());
			ECBPHPHKBPD(LOBFDOKFJIP + ".hash", iMMGBGKAMPK);
		}
	}

	public static void UpdateFileHash(XmlDocument LOBFDOKFJIP, string ONEIGMLOGDC)
	{
		if (GameSettings.HCAJHNKLLGB())
		{
			string iMMGBGKAMPK = MD5Utils.INPENHNJBGJ(LOBFDOKFJIP.OuterXml, GHEHGBBNMNO());
			ECBPHPHKBPD(ONEIGMLOGDC + ".hash", iMMGBGKAMPK);
		}
	}

	private static void ECBPHPHKBPD(string ONEIGMLOGDC, string IMMGBGKAMPK)
	{
		File.WriteAllText(ONEIGMLOGDC, IMMGBGKAMPK);
	}

	public static void KAFMCNCGOJH(string ONEIGMLOGDC)
	{
		if (GameSettings.HCAJHNKLLGB())
		{
			ONEIGMLOGDC += ".hash";
			if (File.Exists(ONEIGMLOGDC))
			{
				File.Delete(ONEIGMLOGDC);
			}
		}
	}

	public static void NLIJEIGOALP(string AMNCLCPADOO, string IFIOLDFCLIE)
	{
		if (GameSettings.HCAJHNKLLGB() && File.Exists(AMNCLCPADOO + ".hash"))
		{
			File.Copy(AMNCLCPADOO + ".hash", IFIOLDFCLIE + ".hash", true);
		}
	}
}
