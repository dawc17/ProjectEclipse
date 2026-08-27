using System;
using System.Diagnostics;

public class BinaryReaderNekki : IDisposable
{
	private const int LMKPPMCBDLH = 1;

	private const int GBECGLPOLIG = 2;

	private const int LPAHFIHOJCG = 4;

	private const int BPNHNODECLM = 8;

	public readonly int Size;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int JCGAABDDAOK;

	private byte[] _bytes;

	public int JJCKADKCDIF
	{
		get
		{
			return ECJPLFFAMJO();
		}
		private set
		{
			set_Position(value);
		}
	}

	public BinaryReaderNekki(byte[] data)
	{
		_bytes = data;
		Size = _bytes.Length;
	}

	public int ECJPLFFAMJO()
	{
		return JCGAABDDAOK;
	}

	private void set_Position(int value)
	{
		JCGAABDDAOK = value;
	}

	public virtual bool CLBFCEJOKJC()
	{
		bool result = BitConverter.ToBoolean(_bytes, ECJPLFFAMJO());
		SetPosition(1);
		return result;
	}

	public virtual byte ReadByte()
	{
		byte result = _bytes[ECJPLFFAMJO()];
		SetPosition(1);
		return result;
	}

	public virtual short FOMNAMCAEPD()
	{
		short result = BitConverter.ToInt16(_bytes, ECJPLFFAMJO());
		SetPosition(2);
		return result;
	}

	public virtual int GDFKNFAHHKF()
	{
		int result = BitConverter.ToInt32(_bytes, ECJPLFFAMJO());
		SetPosition(4);
		return result;
	}

	public virtual long JPNNLGCAIGK()
	{
		long result = BitConverter.ToInt64(_bytes, ECJPLFFAMJO());
		SetPosition(8);
		return result;
	}

	public virtual float MMJAOEBFCLN()
	{
		float result = BitConverter.ToSingle(_bytes, ECJPLFFAMJO());
		SetPosition(4);
		return result;
	}

	public float[] ConvertByteArrayToFloat(int count)
	{
		int num = count * 4;
		float[] array = new float[count];
		Buffer.BlockCopy(_bytes, ECJPLFFAMJO(), array, 0, num);
		SetPosition(num);
		return array;
	}

	private void SetPosition(int value)
	{
		set_Position(ECJPLFFAMJO() + value);
	}

	public void Dispose()
	{
		_bytes = null;
	}
}
