internal class DeflateInput
{
	internal struct BKLHEBEBFFD
	{
		internal int count;

		internal int CAILGDNIKJD;
	}

	private byte[] buffer;

	private int count;

	private int CAILGDNIKJD;

	internal int Count
	{
		get
		{
			return OFOPFCJNEBL();
		}
		set
		{
			CHILOKHFALD(value);
		}
	}

	internal int KCJJIDMLJNK
	{
		get
		{
			return JHGJIJNGNBO();
		}
		set
		{
			MOFAGMEDPNM(value);
		}
	}

	internal byte[] FAJIIIFCCPD()
	{
		return buffer;
	}

	internal void set_Buffer(byte[] value)
	{
		buffer = value;
	}

	internal int OFOPFCJNEBL()
	{
		return count;
	}

	internal void CHILOKHFALD(int value)
	{
		count = value;
	}

	internal int JHGJIJNGNBO()
	{
		return CAILGDNIKJD;
	}

	internal void MOFAGMEDPNM(int value)
	{
		CAILGDNIKJD = value;
	}

	internal void MBODOPCOFFE(int HDKKKCDKFEE)
	{
		CAILGDNIKJD += HDKKKCDKFEE;
		count -= HDKKKCDKFEE;
	}

	internal BKLHEBEBFFD ENBODKKOALL()
	{
		BKLHEBEBFFD result = default(BKLHEBEBFFD);
		result.count = count;
		result.CAILGDNIKJD = CAILGDNIKJD;
		return result;
	}

	internal void BIDLPPIPACF(BKLHEBEBFFD state)
	{
		count = state.count;
		CAILGDNIKJD = state.CAILGDNIKJD;
	}
}
