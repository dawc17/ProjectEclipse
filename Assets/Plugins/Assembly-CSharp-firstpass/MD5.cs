using System.Runtime.InteropServices;
using System.Security.Cryptography;

[ComVisible(true)]
public abstract class MD5 : HashAlgorithm
{
	protected MD5()
	{
		HashSizeValue = 128;
	}

	public static MD5 Create()
	{
		return new MD5CryptoServiceProvider();
	}
}
