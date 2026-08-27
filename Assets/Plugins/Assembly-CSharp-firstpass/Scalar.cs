using System.Globalization;
using YamlDotNet.Core;

public class Scalar : NodeEvent
{
	private readonly string value;

	private readonly IBEOFCPMMJJ KIGNIBIMLKK;

	private readonly bool OCBIEJBMFJN;

	private readonly bool FAKBCOKEHGP;

	public string Value
	{
		get
		{
			return OEAKCOHMIHH();
		}
	}

	public IBEOFCPMMJJ HCJPMGKAAMN
	{
		get
		{
			return HALCJLMJDII();
		}
	}

	public bool GGEHMNEDANI
	{
		get
		{
			return BIDLJMEAFMI();
		}
	}

	public bool HFFBJBBEEKA
	{
		get
		{
			return NIENIKOPKOG();
		}
	}

	public override bool MKCFJALADOA
	{
		get
		{
			return DOHAHEHOCLN();
		}
	}

	public Scalar(string KOLNNNLOCFE, string EDLADAAKMDF, string value, IBEOFCPMMJJ KIGNIBIMLKK, bool OCBIEJBMFJN, bool FAKBCOKEHGP, Mark ILENLCMAMBH, Mark PCLFFOBJJFO)
		: base(KOLNNNLOCFE, EDLADAAKMDF, ILENLCMAMBH, PCLFFOBJJFO)
	{
		this.value = value;
		this.KIGNIBIMLKK = KIGNIBIMLKK;
		this.OCBIEJBMFJN = OCBIEJBMFJN;
		this.FAKBCOKEHGP = FAKBCOKEHGP;
	}

	public Scalar(string KOLNNNLOCFE, string EDLADAAKMDF, string value, IBEOFCPMMJJ KIGNIBIMLKK, bool OCBIEJBMFJN, bool FAKBCOKEHGP)
		: this(KOLNNNLOCFE, EDLADAAKMDF, value, KIGNIBIMLKK, OCBIEJBMFJN, FAKBCOKEHGP, Mark.Empty, Mark.Empty)
	{
	}

	public Scalar(string value)
		: this(null, null, value, IBEOFCPMMJJ.Any, true, true, Mark.Empty, Mark.Empty)
	{
	}

	public Scalar(string EDLADAAKMDF, string value)
		: this(null, EDLADAAKMDF, value, IBEOFCPMMJJ.Any, true, true, Mark.Empty, Mark.Empty)
	{
	}

	public Scalar(string KOLNNNLOCFE, string EDLADAAKMDF, string value)
		: this(KOLNNNLOCFE, EDLADAAKMDF, value, IBEOFCPMMJJ.Any, true, true, Mark.Empty, Mark.Empty)
	{
	}

	internal override BHBPOHDAGPH get_Type()
	{
		return BHBPOHDAGPH.Scalar;
	}

	public string OEAKCOHMIHH()
	{
		return value;
	}

	public IBEOFCPMMJJ HALCJLMJDII()
	{
		return KIGNIBIMLKK;
	}

	public bool BIDLJMEAFMI()
	{
		return OCBIEJBMFJN;
	}

	public bool NIENIKOPKOG()
	{
		return FAKBCOKEHGP;
	}

	public override bool DOHAHEHOCLN()
	{
		return !OCBIEJBMFJN && !FAKBCOKEHGP;
	}

	public override string ToString()
	{
		return string.Format(CultureInfo.InvariantCulture, "Scalar [anchor = {0}, tag = {1}, value = {2}, style = {3}, isPlainImplicit = {4}, isQuotedImplicit = {5}]", HCPOJDFJFMM(), LOIGCKFONHJ(), value, KIGNIBIMLKK, OCBIEJBMFJN, FAKBCOKEHGP);
	}

	public override void GPHIFFOGOGN(IParsingEventVisitor NKECMANOOEM)
	{
		NKECMANOOEM.Visit(this);
	}
}
