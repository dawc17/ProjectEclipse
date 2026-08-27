using System.Diagnostics;
using System.Text;

public class HTTPFieldData
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string HKGHEJDKCPI;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string NICJKIEBEOP;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string GGCOFGAKFJF;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Encoding KCEDIFIGHJD;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string MDBPCPHCLLC;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private byte[] NNNFENBIMPM;

	public string MENAJEAJJBE
	{
		get
		{
			return get_Name();
		}
		set
		{
			set_Name(value);
		}
	}

	public string FileName
	{
		get
		{
			return EPDMGFELIMC();
		}
		set
		{
			IMMLGNKJPKA(value);
		}
	}

	public string GLJIAOAHJNE
	{
		get
		{
			return DIHKMAKOHGN();
		}
		set
		{
			KLHEONGIINC(value);
		}
	}

	public Encoding LNGLPHJJIMC
	{
		get
		{
			return PGBGEOMJDJK();
		}
		set
		{
			set_Encoding(value);
		}
	}

	public string GGDJIPKMKFC
	{
		get
		{
			return ILMJJEMPKCN();
		}
		set
		{
			MHMDIMIEPLL(value);
		}
	}

	public byte[] CIABLGOJJAN
	{
		get
		{
			return CLBEEBOFBMA();
		}
		set
		{
			set_Binary(value);
		}
	}

	public byte[] LBAMJPCNCNK
	{
		get
		{
			return NLHGDFGNIHB();
		}
	}

	public string get_Name()
	{
		return HKGHEJDKCPI;
	}

	public void set_Name(string value)
	{
		HKGHEJDKCPI = value;
	}

	public string EPDMGFELIMC()
	{
		return NICJKIEBEOP;
	}

	public void IMMLGNKJPKA(string value)
	{
		NICJKIEBEOP = value;
	}

	public string DIHKMAKOHGN()
	{
		return GGCOFGAKFJF;
	}

	public void KLHEONGIINC(string value)
	{
		GGCOFGAKFJF = value;
	}

	public Encoding PGBGEOMJDJK()
	{
		return KCEDIFIGHJD;
	}

	public void set_Encoding(Encoding value)
	{
		KCEDIFIGHJD = value;
	}

	public string ILMJJEMPKCN()
	{
		return MDBPCPHCLLC;
	}

	public void MHMDIMIEPLL(string value)
	{
		MDBPCPHCLLC = value;
	}

	public byte[] CLBEEBOFBMA()
	{
		return NNNFENBIMPM;
	}

	public void set_Binary(byte[] value)
	{
		NNNFENBIMPM = value;
	}

	public byte[] NLHGDFGNIHB()
	{
		if (CLBEEBOFBMA() != null)
		{
			return CLBEEBOFBMA();
		}
		if (PGBGEOMJDJK() == null)
		{
			set_Encoding(Encoding.UTF8);
		}
		byte[] bytes = PGBGEOMJDJK().GetBytes(ILMJJEMPKCN());
		set_Binary(bytes);
		return bytes;
	}
}
