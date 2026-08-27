using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class MD5Utils
{
	public static string PIFDHBHOMJL(string DCOPLCIFCFL, string JBAPBBGCOGG = null)
	{
		try
		{
			byte[] oIOHECBCFJA = File.ReadAllBytes(DCOPLCIFCFL);
			return MD5HashBytes(oIOHECBCFJA, JBAPBBGCOGG);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		return string.Empty;
	}

	public static string INPENHNJBGJ(string DMKMNOINKFC, string JBAPBBGCOGG = null)
	{
		using (MD5 mD = MD5.Create())
		{
			byte[] nCHAHPLJBMD = mD.ComputeHash(StringToByteArray(DMKMNOINKFC));
			string text = ByteArrayToString(nCHAHPLJBMD);
			if (JBAPBBGCOGG == null)
			{
				return text;
			}
			byte[] nCHAHPLJBMD2 = mD.ComputeHash(StringToByteArray(text + JBAPBBGCOGG));
			return ByteArrayToString(nCHAHPLJBMD2);
		}
	}

	public static string MD5HashBytes(byte[] OIOHECBCFJA, string JBAPBBGCOGG = null)
	{
		using (MD5 mD = MD5.Create())
		{
			byte[] nCHAHPLJBMD = mD.ComputeHash(OIOHECBCFJA);
			string text = ByteArrayToString(nCHAHPLJBMD);
			if (JBAPBBGCOGG == null)
			{
				return text;
			}
			byte[] nCHAHPLJBMD2 = mD.ComputeHash(StringToByteArray(text + JBAPBBGCOGG));
			return ByteArrayToString(nCHAHPLJBMD2);
		}
	}

	public static bool CheckFileHash(string DCOPLCIFCFL, string IMMGBGKAMPK, string JBAPBBGCOGG = null)
	{
		return PIFDHBHOMJL(DCOPLCIFCFL, JBAPBBGCOGG) == IMMGBGKAMPK;
	}

	public static bool HGHDINBJBAD(string DMKMNOINKFC, string IMMGBGKAMPK, string JBAPBBGCOGG = null)
	{
		return INPENHNJBGJ(DMKMNOINKFC, JBAPBBGCOGG) == IMMGBGKAMPK;
	}

	public static bool CheckBytesHash(byte[] OIOHECBCFJA, string IMMGBGKAMPK, string JBAPBBGCOGG = null)
	{
		return MD5HashBytes(OIOHECBCFJA, JBAPBBGCOGG) == IMMGBGKAMPK;
	}

	public static string ByteArrayToString(byte[] NCHAHPLJBMD)
	{
		string text = BitConverter.ToString(NCHAHPLJBMD);
		return text.Replace("-", string.Empty);
	}

	public static byte[] StringToByteArray(string DMKMNOINKFC)
	{
		return Encoding.UTF8.GetBytes(DMKMNOINKFC);
	}
}
