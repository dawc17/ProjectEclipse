using System.Diagnostics;
using YamlDotNet.Core;

public class Comment : ParsingEvent
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string IELPCLONGKP;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool DGLPEGPPNJJ;

	public bool BAAFBDAEGOJ
	{
		get
		{
			return IGLENNPMPDJ();
		}
		private set
		{
			set_IsInline(value);
		}
	}

	public Comment(string value, bool EKOKIGANOMO)
		: this(value, EKOKIGANOMO, Mark.Empty, Mark.Empty)
	{
	}

	public Comment(string value, bool EKOKIGANOMO, Mark ILENLCMAMBH, Mark PCLFFOBJJFO)
		: base(ILENLCMAMBH, PCLFFOBJJFO)
	{
		set_Value(value);
		set_IsInline(EKOKIGANOMO);
	}

	public string OEAKCOHMIHH()
	{
		return IELPCLONGKP;
	}

	private void set_Value(string value)
	{
		IELPCLONGKP = value;
	}

	public bool IGLENNPMPDJ()
	{
		return DGLPEGPPNJJ;
	}

	private void set_IsInline(bool value)
	{
		DGLPEGPPNJJ = value;
	}

	internal override BHBPOHDAGPH get_Type()
	{
		return BHBPOHDAGPH.Comment;
	}

	public override void GPHIFFOGOGN(IParsingEventVisitor NKECMANOOEM)
	{
		NKECMANOOEM.Visit(this);
	}
}
