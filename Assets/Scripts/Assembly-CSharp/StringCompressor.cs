using System.IO;
using System.Text;

public class StringCompressor
{
	public static void CopyTo(Stream LPCJOFBJBIM, Stream BMFJEKAJGOL)
	{
		byte[] array = new byte[4096];
		int count;
		while ((count = LPCJOFBJBIM.Read(array, 0, array.Length)) != 0)
		{
			BMFJEKAJGOL.Write(array, 0, count);
		}
	}

	public static byte[] Compress(string JDCCBCNFENK)
	{
		byte[] bytes = Encoding.Unicode.GetBytes(JDCCBCNFENK);
		return Compressor.Compress(bytes);
	}

	public static string EFJJNIMIBEO(byte[] OIOHECBCFJA)
	{
		byte[] bytes = Compressor.EFJJNIMIBEO(OIOHECBCFJA);
		return Encoding.Unicode.GetString(bytes);
	}
}
