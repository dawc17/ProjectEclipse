using System.Diagnostics;
using System.Xml;

public class PerkObject
{
	public bool IsNot;

	public PlayerType IHJJBIDMEMB = PlayerType.PLAYER_ME;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private PerkInfoItem DHNEANFLBJI;

	public PerkInfoItem JMDBPECCFDF
	{
		get
		{
			return JMDLAMHAJLN();
		}
		protected set
		{
			JMOIMIHPBOM(value);
		}
	}

	public PerkObject()
	{
	}

	public PerkObject(PerkObject NOLFMPDGCOC)
	{
		IsNot = NOLFMPDGCOC.IsNot;
		IHJJBIDMEMB = NOLFMPDGCOC.IHJJBIDMEMB;
		JMOIMIHPBOM(NOLFMPDGCOC.JMDLAMHAJLN());
	}

	public PerkInfoItem JMDLAMHAJLN()
	{
		return DHNEANFLBJI;
	}

	protected void JMOIMIHPBOM(PerkInfoItem value)
	{
		DHNEANFLBJI = value;
	}

	public virtual void Parse(XmlNode node)
	{
		string text = node.Attributes["Player"].CIPOICEEIBK(string.Empty);
		if (text == null || text.Equals(string.Empty) || text.Equals("Me"))
		{
			IHJJBIDMEMB = PlayerType.PLAYER_ME;
		}
		else if (text.Equals("Enemy"))
		{
			IHJJBIDMEMB = PlayerType.PLAYER_ENEMY;
		}
		else
		{
			IHJJBIDMEMB = PlayerType.PLAYER_NONE;
		}
		IsNot = node.Attributes["Not"].ParseInt() > 0;
	}

	public int GetRoundStage(string name)
	{
		switch (name)
		{
		case "StartStance":
			return 1;
		case "Fight":
			return 2;
		case "EndStance":
			return 3;
		default:
			return 0;
		}
	}
}
