using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

public class Compressor
{
	public static void Compress(List<string> EDAAOBPEKJF, string NOLDJLJIPOG)
	{
		if (EDAAOBPEKJF == null || EDAAOBPEKJF.Count == 0)
		{
			return;
		}
		CustomBinaryWriter nKLNBIIKNBA = new CustomBinaryWriter();
		foreach (string item in EDAAOBPEKJF)
		{
			nKLNBIIKNBA.Write(Path.GetFileName(item));
			nKLNBIIKNBA.Write(File.ReadAllText(item));
		}
		byte[] array = nKLNBIIKNBA.IBOIAEAAEGD();
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (DeflateStream oIPMMMLGOAJ = new DeflateStream(memoryStream, System.IO.Compression.CompressionMode.Compress))
			{
				oIPMMMLGOAJ.Write(array, 0, array.Length);
			}
			byte[] bytes = memoryStream.ToArray();
			File.WriteAllBytes(NOLDJLJIPOG, bytes);
		}
	}

	public static void Uncompress(string BBNKIBKPBLO, string AHGDCAFCELI = "")
	{
		using (MemoryStream aBJIEFMMIEK = new MemoryStream(File.ReadAllBytes(BBNKIBKPBLO)))
		{
			MemoryStream memoryStream = new MemoryStream();
			using (DeflateStream oIPMMMLGOAJ = new DeflateStream(aBJIEFMMIEK, System.IO.Compression.CompressionMode.Decompress))
			{
				byte[] array = new byte[2048];
				int num = 0;
				while ((num = oIPMMMLGOAJ.Read(array, 0, array.Length)) > 0)
				{
					memoryStream.Write(array, 0, num);
				}
				memoryStream.Position = 0L;
				StreamReader streamReader = new StreamReader(memoryStream);
				bool flag = false;
				while (!flag)
				{
					string text = streamReader.ReadLine();
					string text2 = streamReader.ReadLine();
					if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text2))
					{
						flag = true;
						continue;
					}
					string path = AHGDCAFCELI + text;
					File.WriteAllText(path, text2);
				}
			}
		}
	}

	public static byte[] Compress(byte[] BPLIHEIIBFP)
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (DeflateStream defl = new DeflateStream(memoryStream, System.IO.Compression.CompressionMode.Compress))
			{
				defl.Write(BPLIHEIIBFP, 0, BPLIHEIIBFP.Length);
			}
			return memoryStream.ToArray();
		}
	}

	public static byte[] EFJJNIMIBEO(byte[] BPLIHEIIBFP)
	{
		try
		{
			using (MemoryStream input = new MemoryStream(BPLIHEIIBFP))
			using (DeflateStream defl = new DeflateStream(input, System.IO.Compression.CompressionMode.Decompress))
			using (MemoryStream output = new MemoryStream())
			{
				byte[] array = new byte[4096];
				int num;
				while ((num = defl.Read(array, 0, array.Length)) > 0)
				{
					output.Write(array, 0, num);
				}
				return output.ToArray();
			}
		}
		catch
		{
			return BPLIHEIIBFP;
		}
	}
}