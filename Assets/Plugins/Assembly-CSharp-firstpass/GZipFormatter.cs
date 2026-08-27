internal class GZipFormatter : IFileFormatWriter
{
	private byte[] headerBytes = new byte[10] { 31, 139, 8, 0, 0, 0, 0, 0, 4, 0 };

	private uint _crc32;

	private long _inputStreamSizeModulo;

	internal GZipFormatter()
		: this(3)
	{
	}

	internal GZipFormatter(int CPOCBHJGICD)
	{
		if (CPOCBHJGICD == 10)
		{
			headerBytes[8] = 2;
		}
	}

	public byte[] BOLKKEBKHEE()
	{
		return headerBytes;
	}

	public void UpdateWithBytesRead(byte[] buffer, int IPCOBJBKNAO, int OGAPEFFEHIH)
	{
		_crc32 = Crc32Helper.JDBNFCAIBHC(_crc32, buffer, IPCOBJBKNAO, OGAPEFFEHIH);
		long num = _inputStreamSizeModulo + (uint)OGAPEFFEHIH;
		if (num >= 4294967296L)
		{
			num %= 4294967296L;
		}
		_inputStreamSizeModulo = num;
	}

	public byte[] AIJJKADBLMG()
	{
		byte[] array = new byte[8];
		WriteUInt32(array, _crc32, 0);
		WriteUInt32(array, (uint)_inputStreamSizeModulo, 4);
		return array;
	}

	internal void WriteUInt32(byte[] AAOIAEJJINO, uint value, int CAILGDNIKJD)
	{
		AAOIAEJJINO[CAILGDNIKJD] = (byte)value;
		AAOIAEJJINO[CAILGDNIKJD + 1] = (byte)(value >> 8);
		AAOIAEJJINO[CAILGDNIKJD + 2] = (byte)(value >> 16);
		AAOIAEJJINO[CAILGDNIKJD + 3] = (byte)(value >> 24);
	}
}
