using System.Globalization;
using YamlDotNet.Core;

public class MappingStart : NodeEvent
{
	private readonly bool isImplicit;

	private readonly FGDKNBEFPFN KIGNIBIMLKK;

	public override int OHJMGKADENE
	{
		get
		{
			return DPIMLJJFMCO();
		}
	}

	public bool KIOLMKCLEEB
	{
		get
		{
			return BBBGHODAEIN();
		}
	}

	public override bool MKCFJALADOA
	{
		get
		{
			return DOHAHEHOCLN();
		}
	}

	public FGDKNBEFPFN HCJPMGKAAMN
	{
		get
		{
			return HALCJLMJDII();
		}
	}

	public MappingStart(string KOLNNNLOCFE, string EDLADAAKMDF, bool isImplicit, FGDKNBEFPFN KIGNIBIMLKK, Mark ILENLCMAMBH, Mark PCLFFOBJJFO)
		: base(KOLNNNLOCFE, EDLADAAKMDF, ILENLCMAMBH, PCLFFOBJJFO)
	{
		this.isImplicit = isImplicit;
		this.KIGNIBIMLKK = KIGNIBIMLKK;
	}

	public MappingStart(string KOLNNNLOCFE, string EDLADAAKMDF, bool isImplicit, FGDKNBEFPFN KIGNIBIMLKK)
		: this(KOLNNNLOCFE, EDLADAAKMDF, isImplicit, KIGNIBIMLKK, Mark.Empty, Mark.Empty)
	{
	}

	public MappingStart()
		: this(null, null, true, FGDKNBEFPFN.Any, Mark.Empty, Mark.Empty)
	{
	}

	public override int DPIMLJJFMCO()
	{
		return 1;
	}

	internal override BHBPOHDAGPH get_Type()
	{
		return BHBPOHDAGPH.MappingStart;
	}

	public bool BBBGHODAEIN()
	{
		return isImplicit;
	}

	public override bool DOHAHEHOCLN()
	{
		return !isImplicit;
	}

	public FGDKNBEFPFN HALCJLMJDII()
	{
		return KIGNIBIMLKK;
	}

	public override string ToString()
	{
		return string.Format(CultureInfo.InvariantCulture, "Mapping start [anchor = {0}, tag = {1}, isImplicit = {2}, style = {3}]", HCPOJDFJFMM(), LOIGCKFONHJ(), isImplicit, KIGNIBIMLKK);
	}

	public override void GPHIFFOGOGN(IParsingEventVisitor NKECMANOOEM)
	{
		NKECMANOOEM.Visit(this);
	}
}
