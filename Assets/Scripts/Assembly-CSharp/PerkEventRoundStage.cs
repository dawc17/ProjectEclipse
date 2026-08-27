using System.Diagnostics;
using System.Xml;

public class PerkEventRoundStage : PerkEvent
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int ELDMMPIAAHG;

	public int JMHJDHLBHLK
	{
		get
		{
			return OLFNNDNPDNH();
		}
		protected set
		{
			set_RoundStage(value);
		}
	}

	public PerkEventRoundStage()
	{
	}

	public PerkEventRoundStage(PerkEventRoundStage NOLFMPDGCOC)
		: base(NOLFMPDGCOC)
	{
		set_RoundStage(NOLFMPDGCOC.OLFNNDNPDNH());
	}

	public int OLFNNDNPDNH()
	{
		return ELDMMPIAAHG;
	}

	protected void set_RoundStage(int value)
	{
		ELDMMPIAAHG = value;
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		set_RoundStage(GetRoundStage(node.Attributes["Name"].CIPOICEEIBK(string.Empty)));
	}

	public override bool IsEqual(EventStruct EJMEALJNNIL)
	{
		if (!base.IsEqual(EJMEALJNNIL))
		{
			return false;
		}
		int jMHJDHLBHLK = EJMEALJNNIL.BIKLKJMNGKP.JMHJDHLBHLK;
		if (OLFNNDNPDNH() != 0 && OLFNNDNPDNH() != jMHJDHLBHLK)
		{
			return false;
		}
		return true;
	}
}
