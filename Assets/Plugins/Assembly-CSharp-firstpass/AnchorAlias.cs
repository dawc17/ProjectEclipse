using System.Globalization;
using YamlDotNet.Core;

public class AnchorAlias : ParsingEvent
{
	private readonly string value;

	public string Value
	{
		get
		{
			return OEAKCOHMIHH();
		}
	}

	public AnchorAlias(string value, Mark ILENLCMAMBH, Mark PCLFFOBJJFO)
		: base(ILENLCMAMBH, PCLFFOBJJFO)
	{
		if (string.IsNullOrEmpty(value))
		{
			throw new YamlException(ILENLCMAMBH, PCLFFOBJJFO, "Anchor value must not be empty.");
		}
		if (!NodeEvent.anchorValidator.IsMatch(value))
		{
			throw new YamlException(ILENLCMAMBH, PCLFFOBJJFO, "Anchor value must contain alphanumerical characters only.");
		}
		this.value = value;
	}

	public AnchorAlias(string value)
		: this(value, Mark.Empty, Mark.Empty)
	{
	}

	internal override BHBPOHDAGPH get_Type()
	{
		return BHBPOHDAGPH.Alias;
	}

	public string OEAKCOHMIHH()
	{
		return value;
	}

	public override string ToString()
	{
		return string.Format(CultureInfo.InvariantCulture, "Alias [value = {0}]", value);
	}

	public override void GPHIFFOGOGN(IParsingEventVisitor NKECMANOOEM)
	{
		NKECMANOOEM.Visit(this);
	}
}
