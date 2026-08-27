using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class Crypto
{
	private static byte[] FPEMKGPELNH = Encoding.ASCII.GetBytes("o6806642kbM7c5");

	public static string APAIFKLIICN(string NOGCALGLPCH, string MGOACNLKMPB)
	{
		if (string.IsNullOrEmpty(NOGCALGLPCH))
		{
			throw new ArgumentNullException("plainText");
		}
		if (string.IsNullOrEmpty(MGOACNLKMPB))
		{
			throw new ArgumentNullException("sharedSecret");
		}
		string text = null;
		RijndaelManaged rijndaelManaged = null;
		try
		{
			Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(MGOACNLKMPB, FPEMKGPELNH);
			rijndaelManaged = new RijndaelManaged();
			rijndaelManaged.Key = rfc2898DeriveBytes.GetBytes(rijndaelManaged.KeySize / 8);
			ICryptoTransform transform = rijndaelManaged.CreateEncryptor(rijndaelManaged.Key, rijndaelManaged.IV);
			using (MemoryStream memoryStream = new MemoryStream())
			{
				memoryStream.Write(BitConverter.GetBytes(rijndaelManaged.IV.Length), 0, 4);
				memoryStream.Write(rijndaelManaged.IV, 0, rijndaelManaged.IV.Length);
				using (CryptoStream stream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
				{
					using (StreamWriter streamWriter = new StreamWriter(stream))
					{
						streamWriter.Write(NOGCALGLPCH);
					}
				}
				return Convert.ToBase64String(memoryStream.ToArray());
			}
		}
		finally
		{
			if (rijndaelManaged != null)
			{
				rijndaelManaged.Clear();
			}
		}
	}

	public static string BLFFIJJHOPA(string PKJCKPENLIN, string MGOACNLKMPB)
	{
		if (string.IsNullOrEmpty(PKJCKPENLIN))
		{
			throw new ArgumentNullException("cipherText");
		}
		if (string.IsNullOrEmpty(MGOACNLKMPB))
		{
			throw new ArgumentNullException("sharedSecret");
		}
		RijndaelManaged rijndaelManaged = null;
		string text = null;
		try
		{
			Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(MGOACNLKMPB, FPEMKGPELNH);
			byte[] buffer = Convert.FromBase64String(PKJCKPENLIN);
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				rijndaelManaged = new RijndaelManaged();
				rijndaelManaged.Key = rfc2898DeriveBytes.GetBytes(rijndaelManaged.KeySize / 8);
				rijndaelManaged.IV = EOKDGDGEBJA(memoryStream);
				ICryptoTransform transform = rijndaelManaged.CreateDecryptor(rijndaelManaged.Key, rijndaelManaged.IV);
				using (CryptoStream stream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Read))
				{
					using (StreamReader streamReader = new StreamReader(stream))
					{
						return streamReader.ReadToEnd();
					}
				}
			}
		}
		finally
		{
			if (rijndaelManaged != null)
			{
				rijndaelManaged.Clear();
			}
		}
	}

	private static byte[] EOKDGDGEBJA(Stream JDCCBCNFENK)
	{
		byte[] array = new byte[4];
		if (JDCCBCNFENK.Read(array, 0, array.Length) != array.Length)
		{
			throw new SystemException("Stream did not contain properly formatted byte array");
		}
		byte[] array2 = new byte[BitConverter.ToInt32(array, 0)];
		if (JDCCBCNFENK.Read(array2, 0, array2.Length) != array2.Length)
		{
			throw new SystemException("Did not read byte array properly");
		}
		return array2;
	}
}
