using System.Diagnostics;

public sealed class Error
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private CCCOMMIFIMB OAMDJDNMMCH;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string AGEPKFMGHGA;

	public CCCOMMIFIMB EDEEELJMHLG
	{
		get
		{
			return AKOMBGCDEHG();
		}
		private set
		{
			KMPELENHJBK(value);
		}
	}

	public Error(CCCOMMIFIMB KJPGKHJNOMC, string CKEHOEGLMBM)
	{
		KMPELENHJBK(KJPGKHJNOMC);
		set_Message(CKEHOEGLMBM);
	}

	public CCCOMMIFIMB AKOMBGCDEHG()
	{
		return OAMDJDNMMCH;
	}

	private void KMPELENHJBK(CCCOMMIFIMB value)
	{
		OAMDJDNMMCH = value;
	}

	public string DCKPMHKDLEJ()
	{
		return AGEPKFMGHGA;
	}

	private void set_Message(string value)
	{
		AGEPKFMGHGA = value;
	}

	public override string ToString()
	{
		return string.Format("Code: {0} Message: \"{1}\"", AKOMBGCDEHG().ToString(), DCKPMHKDLEJ());
	}
}
