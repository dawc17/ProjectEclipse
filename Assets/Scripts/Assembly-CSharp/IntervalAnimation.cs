using System.Xml;

public class IntervalAnimation
{
	public enum NGAJJDIEDGF
	{
		INTERVAL_NONE = 0,
		INTERVAL_UNSTABLE = 1,
		INTERVAL_UNINTERRUPT = 2,
		INTERVAL_SELF_UNINTERRUPT = 3,
		INTERVAL_ATTACK = 4,
		INTERVAL_BLOCK = 5,
		INTERVAL_INVULNERABLE = 6,
		INTERVAL_INVISIBLE = 7
	}

	private int _Id;

	private int MLIGJHGMOAA;

	public int Start;

	public int GEJLNPIEDPF;

	public string Name;

	public NGAJJDIEDGF Type;

	public XmlNode NodeInterval;

	public int GJCOGFOJAEB
	{
		get
		{
			return ANAECCFDHMI();
		}
	}

	public int LJNBFJKKLKM
	{
		set
		{
			set_AnimationFinishFrame(value);
		}
	}

	public IntervalAnimation(NGAJJDIEDGF AFGJECLDAIG)
	{
		_Id = -1;
		MLIGJHGMOAA = int.MaxValue;
		Type = AFGJECLDAIG;
	}

	public int ANAECCFDHMI()
	{
		return _Id;
	}

	public void set_AnimationFinishFrame(int value)
	{
		MLIGJHGMOAA = value;
	}

	public virtual void Parse(XmlNode MEEAKLDGLDF)
	{
		NodeInterval = MEEAKLDGLDF;
		_Id = XmlUtils.ParseInt(MEEAKLDGLDF.Attributes["ID"], -1);
	}

	public virtual void Init()
	{
		Name = XmlUtils.ParseString(NodeInterval.Attributes["Name"]);
		if (Name == "Unstable")
		{
			Type = NGAJJDIEDGF.INTERVAL_UNSTABLE;
		}
		else if (Name == "Uninterrupt")
		{
			Type = NGAJJDIEDGF.INTERVAL_UNINTERRUPT;
		}
		else if (Name == "SelfUninterrupt")
		{
			Type = NGAJJDIEDGF.INTERVAL_SELF_UNINTERRUPT;
		}
		Start = XmlUtils.ParseInt(NodeInterval.Attributes["Start"]);
		bool flag = NodeInterval.Attributes["End"] != null;
		GEJLNPIEDPF = ((!flag) ? (MLIGJHGMOAA + 2) : XmlUtils.ParseInt(NodeInterval.Attributes["End"], int.MaxValue));
		if (Start > GEJLNPIEDPF)
		{
			// Newer templates can override Start while inheriting an older End.
			// The modern merger treats that as an open-ended interval; this legacy
			// merger leaves the stale End behind. Preserve a valid one-frame window.
			GEJLNPIEDPF = Start;
		}
		ParseInside();
		NodeInterval = null;
	}

	public static NGAJJDIEDGF LAJMDAFFPJE(string LFLGCDNKNJI)
	{
		switch (LFLGCDNKNJI)
		{
		case "Attack":
			return NGAJJDIEDGF.INTERVAL_ATTACK;
		case "Block":
			return NGAJJDIEDGF.INTERVAL_BLOCK;
		case "Invulnerable":
			return NGAJJDIEDGF.INTERVAL_INVULNERABLE;
		case "Invisible":
			return NGAJJDIEDGF.INTERVAL_INVISIBLE;
		default:
			return NGAJJDIEDGF.INTERVAL_NONE;
		}
	}

	private static string KNCNACGPAMA(NGAJJDIEDGF LFLGCDNKNJI)
	{
		switch (LFLGCDNKNJI)
		{
		case NGAJJDIEDGF.INTERVAL_NONE:
			return string.Empty;
		case NGAJJDIEDGF.INTERVAL_UNSTABLE:
			return "Unstable";
		case NGAJJDIEDGF.INTERVAL_UNINTERRUPT:
			return "Uninterrupt";
		case NGAJJDIEDGF.INTERVAL_SELF_UNINTERRUPT:
			return "SelfUninterrupt";
		case NGAJJDIEDGF.INTERVAL_ATTACK:
			return "Attack";
		case NGAJJDIEDGF.INTERVAL_BLOCK:
			return "Block";
		case NGAJJDIEDGF.INTERVAL_INVULNERABLE:
			return "Invulnerable";
		case NGAJJDIEDGF.INTERVAL_INVISIBLE:
			return "Invisible";
		default:
			return string.Empty;
		}
	}

	protected virtual void ParseInside()
	{
	}
}
