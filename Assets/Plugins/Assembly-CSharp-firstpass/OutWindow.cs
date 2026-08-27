using System.IO;

public class OutWindow
{
	private byte[] _buffer;

	private uint _pos;

	private uint PHCCJGNNFII;

	private uint OONMJHCFEHO;

	private Stream _stream;

	public uint FAJMEIBMEDF;

	public void Create(uint AKOEOKJFINO)
	{
		if (PHCCJGNNFII != AKOEOKJFINO)
		{
			_buffer = new byte[AKOEOKJFINO];
		}
		PHCCJGNNFII = AKOEOKJFINO;
		_pos = 0u;
		OONMJHCFEHO = 0u;
	}

	public void Init(Stream ABJIEFMMIEK, bool POOADOMADDK)
	{
		IAIFCIAAHOE();
		_stream = ABJIEFMMIEK;
		if (!POOADOMADDK)
		{
			OONMJHCFEHO = 0u;
			_pos = 0u;
			FAJMEIBMEDF = 0u;
		}
	}

	public bool Train(Stream ABJIEFMMIEK)
	{
		long length = ABJIEFMMIEK.Length;
		uint num = (FAJMEIBMEDF = (uint)((length >= PHCCJGNNFII) ? PHCCJGNNFII : length));
		ABJIEFMMIEK.Position = length - num;
		OONMJHCFEHO = (_pos = 0u);
		while (num != 0)
		{
			uint num2 = PHCCJGNNFII - _pos;
			if (num < num2)
			{
				num2 = num;
			}
			int num3 = ABJIEFMMIEK.Read(_buffer, (int)_pos, (int)num2);
			if (num3 == 0)
			{
				return false;
			}
			num -= (uint)num3;
			_pos += (uint)num3;
			OONMJHCFEHO += (uint)num3;
			if (_pos == PHCCJGNNFII)
			{
				OONMJHCFEHO = (_pos = 0u);
			}
		}
		return true;
	}

	public void IAIFCIAAHOE()
	{
		MKPBJGMJPMI();
		_stream = null;
	}

	public void MKPBJGMJPMI()
	{
		uint num = _pos - OONMJHCFEHO;
		if (num != 0)
		{
			_stream.Write(_buffer, (int)OONMJHCFEHO, (int)num);
			if (_pos >= PHCCJGNNFII)
			{
				_pos = 0u;
			}
			OONMJHCFEHO = _pos;
		}
	}

	public void CopyBlock(uint OIOMNNFMDOO, uint JCAJDBOMGOM)
	{
		uint num = _pos - OIOMNNFMDOO - 1;
		if (num >= PHCCJGNNFII)
		{
			num += PHCCJGNNFII;
		}
		while (JCAJDBOMGOM != 0)
		{
			if (num >= PHCCJGNNFII)
			{
				num = 0u;
			}
			_buffer[_pos++] = _buffer[num++];
			if (_pos >= PHCCJGNNFII)
			{
				MKPBJGMJPMI();
			}
			JCAJDBOMGOM--;
		}
	}

	public void PutByte(byte AAOIAEJJINO)
	{
		_buffer[_pos++] = AAOIAEJJINO;
		if (_pos >= PHCCJGNNFII)
		{
			MKPBJGMJPMI();
		}
	}

	public byte GetByte(uint OIOMNNFMDOO)
	{
		uint num = _pos - OIOMNNFMDOO - 1;
		if (num >= PHCCJGNNFII)
		{
			num += PHCCJGNNFII;
		}
		return _buffer[num];
	}
}
