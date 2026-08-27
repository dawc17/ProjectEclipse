using System.IO;
using System.Text;

internal class SharedUtils
{
	public static int AMEAMGBOINH(int number, int HLFOKLCKNEE)
	{
		return (int)((uint)number >> HLFOKLCKNEE);
	}

	public static int OEFEKLAOOLO(TextReader INLPHNJBHCP, byte[] target, int ILENLCMAMBH, int count)
	{
		if (target.Length == 0)
		{
			return 0;
		}
		char[] array = new char[target.Length];
		int num = INLPHNJBHCP.Read(array, ILENLCMAMBH, count);
		if (num == 0)
		{
			return -1;
		}
		for (int i = ILENLCMAMBH; i < ILENLCMAMBH + num; i++)
		{
			target[i] = (byte)array[i];
		}
		return num;
	}

	internal static byte[] ToByteArray(string NCBHBAKKMJO)
	{
		return Encoding.UTF8.GetBytes(NCBHBAKKMJO);
	}

	internal static char[] ToCharArray(byte[] HFADMOEOHFA)
	{
		return Encoding.UTF8.GetChars(HFADMOEOHFA);
	}
}
