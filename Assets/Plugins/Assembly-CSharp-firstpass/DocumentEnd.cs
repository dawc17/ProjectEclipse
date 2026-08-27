using System.Globalization;
using YamlDotNet.Core;

public class DocumentEnd : ParsingEvent
{
	private readonly bool isImplicit;

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

	public DocumentEnd(bool isImplicit, Mark ILENLCMAMBH, Mark PCLFFOBJJFO)
		: base(ILENLCMAMBH, PCLFFOBJJFO)
	{
		this.isImplicit = isImplicit;
	}

	public DocumentEnd(bool isImplicit)
		: this(isImplicit, Mark.Empty, Mark.Empty)
	{
	}

	public override int DPIMLJJFMCO()
	{
		return -1;
	}

	internal override BHBPOHDAGPH get_Type()
	{
		return BHBPOHDAGPH.DocumentEnd;
	}

	public bool BBBGHODAEIN()
	{
		return isImplicit;
	}

	public override string ToString()
	{
		return string.Format(CultureInfo.InvariantCulture, "Document end [isImplicit = {0}]", isImplicit);
	}

	public override void GPHIFFOGOGN(IParsingEventVisitor NKECMANOOEM)
	{
		NKECMANOOEM.Visit(this);
	}
}
