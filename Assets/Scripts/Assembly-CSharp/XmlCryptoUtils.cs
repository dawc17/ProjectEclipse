using System;
using System.IO;
using UnityEngine;

public class XmlCryptoUtils
{
	private static byte[] ENFBNOGCCBH = new byte[32]
	{
		235, 183, 115, 127, 190, 102, 8, 38, 143, 83,
		145, 154, 166, 165, 4, 62, 149, 139, 150, 98,
		116, 121, 168, 0, 229, 206, 150, 77, 179, 200,
		129, 115
	};

	private static byte[] GBMFFFDKANM = new byte[16]
	{
		169, 159, 94, 17, 115, 17, 231, 206, 86, 58,
		123, 161, 50, 39, 189, 209
	};

	public static bool CJFDNPFGHDE
	{
		get
		{
			return NNLGALNDJCL();
		}
	}

	public static bool NNLGALNDJCL()
	{
		if (Application.isEditor)
		{
			return true;
		}
		return SystemProperties.IPJFCBAGMJJ() || SystemProperties.MEBGOGMJFLM();
	}

	public static string HJGLMILACAA(string ONEIGMLOGDC)
	{
		ONEIGMLOGDC = NNBCLAEKMIO(ONEIGMLOGDC);
		TextAsset textAsset = ResourcesAndBundles.Load<TextAsset>(ONEIGMLOGDC);
		string result = null;
		try
		{
			if (textAsset != null)
			{
				result = AESUtils.AGDCAGCACKL(textAsset.text, ENFBNOGCCBH, GBMFFFDKANM);
			}
		}
		catch (Exception ex)
		{
			LLLOJBFMONN.Error(ex.ToString());
		}
		return result;
	}

	public static string AHIMJFNGENL(string ONEIGMLOGDC)
	{
		ONEIGMLOGDC = NNBCLAEKMIO(ONEIGMLOGDC);
		TextAsset textAsset = ResourcesAndBundles.Load<TextAsset>(ONEIGMLOGDC);
		string result = null;
		try
		{
			if (textAsset != null)
			{
				result = AESUtils.JLONJPHLPAL(textAsset.text, ENFBNOGCCBH, GBMFFFDKANM);
			}
		}
		catch (Exception ex)
		{
			LLLOJBFMONN.Error(ex.ToString());
		}
		return result;
	}

	public static string PFCKNMLJPGL(string HCPNFPMHFCM)
	{
		string result = null;
		try
		{
			result = AESUtils.AGDCAGCACKL(HCPNFPMHFCM, ENFBNOGCCBH, GBMFFFDKANM);
		}
		catch (Exception ex)
		{
			LLLOJBFMONN.Error(ex.ToString());
		}
		return result;
	}

	public static string OIMNHACBGNH(string HCPNFPMHFCM)
	{
		string result = null;
		try
		{
			result = AESUtils.JLONJPHLPAL(HCPNFPMHFCM, ENFBNOGCCBH, GBMFFFDKANM);
		}
		catch (Exception ex)
		{
			LLLOJBFMONN.Error(ex.ToString());
		}
		if (string.IsNullOrEmpty(result))
		{
			result = HCPNFPMHFCM;
		}
		return result;
	}

	private static string NNBCLAEKMIO(string ONEIGMLOGDC)
	{
		if (Path.HasExtension(ONEIGMLOGDC))
		{
			return Path.ChangeExtension(ONEIGMLOGDC, null);
		}
		return ONEIGMLOGDC;
	}
}
