using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.Core.Tokens;

public class DocumentStart : ParsingEvent
{
	private readonly TagDirectiveCollection CPAIGLNDIOK;

	private readonly VersionDirective version;

	private readonly bool isImplicit;

	public override int OHJMGKADENE
	{
		get
		{
			return DPIMLJJFMCO();
		}
	}

	public TagDirectiveCollection DCCLDDMHOBH
	{
		get
		{
			return FNNKPBJDMDF();
		}
	}

	public VersionDirective Version
	{
		get
		{
			return KCJMMIEBLHL();
		}
	}

	public bool KIOLMKCLEEB
	{
		get
		{
			return BBBGHODAEIN();
		}
	}

	public DocumentStart(VersionDirective version, TagDirectiveCollection CPAIGLNDIOK, bool isImplicit, Mark ILENLCMAMBH, Mark PCLFFOBJJFO)
		: base(ILENLCMAMBH, PCLFFOBJJFO)
	{
		this.version = version;
		this.CPAIGLNDIOK = CPAIGLNDIOK;
		this.isImplicit = isImplicit;
	}

	public DocumentStart(VersionDirective version, TagDirectiveCollection CPAIGLNDIOK, bool isImplicit)
		: this(version, CPAIGLNDIOK, isImplicit, Mark.Empty, Mark.Empty)
	{
	}

	public DocumentStart(Mark ILENLCMAMBH, Mark PCLFFOBJJFO)
		: this(null, null, true, ILENLCMAMBH, PCLFFOBJJFO)
	{
	}

	public DocumentStart()
		: this(null, null, true, Mark.Empty, Mark.Empty)
	{
	}

	public override int DPIMLJJFMCO()
	{
		return 1;
	}

	internal override BHBPOHDAGPH get_Type()
	{
		return BHBPOHDAGPH.DocumentStart;
	}

	public TagDirectiveCollection FNNKPBJDMDF()
	{
		return CPAIGLNDIOK;
	}

	public VersionDirective KCJMMIEBLHL()
	{
		return version;
	}

	public bool BBBGHODAEIN()
	{
		return isImplicit;
	}

	public override string ToString()
	{
		return string.Format(CultureInfo.InvariantCulture, "Document start [isImplicit = {0}]", isImplicit);
	}

	public override void GPHIFFOGOGN(IParsingEventVisitor NKECMANOOEM)
	{
		NKECMANOOEM.Visit(this);
	}
}
