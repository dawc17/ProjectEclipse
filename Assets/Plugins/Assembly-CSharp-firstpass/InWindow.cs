using System.IO;

public class InWindow
{
	public byte[] _bufferBase;

	private Stream _stream;

	private uint MOPCNHCGPNC;

	private bool _streamEndWasReached;

	private uint BABLHKEJPEL;

	public uint HGGJBAEEKJN;

	public uint EILJMLGCJAH;

	public uint _pos;

	private uint DBHKMKMKFMJ;

	private uint ECELIIKJLHM;

	public uint OONMJHCFEHO;

	public void DFKNDKFDELB()
	{
		uint num = HGGJBAEEKJN + _pos - DBHKMKMKFMJ;
		if (num != 0)
		{
			num--;
		}
		uint num2 = HGGJBAEEKJN + OONMJHCFEHO - num;
		for (uint num3 = 0u; num3 < num2; num3++)
		{
			_bufferBase[num3] = _bufferBase[num + num3];
		}
		HGGJBAEEKJN -= num;
	}

	public virtual void ONDIFDJPMDM()
	{
		if (_streamEndWasReached)
		{
			return;
		}
		while (true)
		{
			int num = (int)(0 - HGGJBAEEKJN + EILJMLGCJAH - OONMJHCFEHO);
			if (num == 0)
			{
				return;
			}
			int num2 = _stream.Read(_bufferBase, (int)(HGGJBAEEKJN + OONMJHCFEHO), num);
			if (num2 == 0)
			{
				break;
			}
			OONMJHCFEHO += (uint)num2;
			if (OONMJHCFEHO >= _pos + ECELIIKJLHM)
			{
				MOPCNHCGPNC = OONMJHCFEHO - ECELIIKJLHM;
			}
		}
		MOPCNHCGPNC = OONMJHCFEHO;
		uint num3 = HGGJBAEEKJN + MOPCNHCGPNC;
		if (num3 > BABLHKEJPEL)
		{
			MOPCNHCGPNC = BABLHKEJPEL - HGGJBAEEKJN;
		}
		_streamEndWasReached = true;
	}

	private void PJNFHNFLNNO()
	{
		_bufferBase = null;
	}

	public void Create(uint CMNIBPLKJEA, uint ABFKEDIJFPN, uint IKHIOAIPBNL)
	{
		DBHKMKMKFMJ = CMNIBPLKJEA;
		ECELIIKJLHM = ABFKEDIJFPN;
		uint num = CMNIBPLKJEA + ABFKEDIJFPN + IKHIOAIPBNL;
		if (_bufferBase == null || EILJMLGCJAH != num)
		{
			PJNFHNFLNNO();
			EILJMLGCJAH = num;
			_bufferBase = new byte[EILJMLGCJAH];
		}
		BABLHKEJPEL = EILJMLGCJAH - ABFKEDIJFPN;
	}

	public void SetStream(Stream ABJIEFMMIEK)
	{
		_stream = ABJIEFMMIEK;
	}

	public void IAIFCIAAHOE()
	{
		_stream = null;
	}

	public void Init()
	{
		HGGJBAEEKJN = 0u;
		_pos = 0u;
		OONMJHCFEHO = 0u;
		_streamEndWasReached = false;
		ONDIFDJPMDM();
	}

	public void MHEJFMDCOHI()
	{
		_pos++;
		if (_pos > MOPCNHCGPNC)
		{
			uint num = HGGJBAEEKJN + _pos;
			if (num > BABLHKEJPEL)
			{
				DFKNDKFDELB();
			}
			ONDIFDJPMDM();
		}
	}

	public byte GetIndexByte(int index)
	{
		return _bufferBase[HGGJBAEEKJN + _pos + index];
	}

	public uint GetMatchLen(int index, uint OIOMNNFMDOO, uint LOHCIKNKDEI)
	{
		if (_streamEndWasReached && _pos + index + LOHCIKNKDEI > OONMJHCFEHO)
		{
			LOHCIKNKDEI = OONMJHCFEHO - (uint)(int)(_pos + index);
		}
		OIOMNNFMDOO++;
		uint num = HGGJBAEEKJN + _pos + (uint)index;
		uint num2;
		for (num2 = 0u; num2 < LOHCIKNKDEI && _bufferBase[num + num2] == _bufferBase[num + num2 - OIOMNNFMDOO]; num2++)
		{
		}
		return num2;
	}

	public uint HBJMPBCHFJB()
	{
		return OONMJHCFEHO - _pos;
	}

	public void ReduceOffsets(int BALBEBAOPMP)
	{
		HGGJBAEEKJN += (uint)BALBEBAOPMP;
		MOPCNHCGPNC -= (uint)BALBEBAOPMP;
		_pos -= (uint)BALBEBAOPMP;
		OONMJHCFEHO -= (uint)BALBEBAOPMP;
	}
}
