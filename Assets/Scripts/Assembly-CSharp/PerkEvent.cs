using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

public class PerkEvent : PerkObject
{
	public enum KNKIIEPDCPN
	{
		EVENT_NONE = 0,
		EVENT_ROUND_STAGE_START = 1,
		EVENT_EVERY_FRAME = 2,
		EVENT_STYLE = 3,
		EVENT_COMBO = 4,
		EVENT_HIT_PRECRIT = 5,
		EVENT_HIT_POSTCRIT = 6,
		EVENT_POST_HIT = 7,
		EVENT_MAGIC_CHARGED = 8,
		EVENT_ANIMATION_START = 9,
		EVENT_ANIMATION_END = 10,
		EVENT_MOD_EXPIRES = 11,
		EVENT_AREA_ENTER = 12,
		EVENT_AREA_EXIT = 13,
		EVENT_INTERVAL_END = 14
	}

	public class EventStruct
	{
		public KNKIIEPDCPN Type;

		public object Info;

		public Model BMIGEFANCCC;

		public Model BIKLKJMNGKP;

		public string Namespace = string.Empty;
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private KNKIIEPDCPN KAHHEBMBCFA;

	public PerkEvent()
	{
	}

	public PerkEvent(PerkEvent NOLFMPDGCOC)
		: base(NOLFMPDGCOC)
	{
		set_Type(NOLFMPDGCOC.get_Type());
	}

	public KNKIIEPDCPN get_Type()
	{
		return KAHHEBMBCFA;
	}

	protected void set_Type(KNKIIEPDCPN value)
	{
		KAHHEBMBCFA = value;
	}

	public static List<PerkEvent> Create(XmlNode node, PerkInfoItem AEFFHJGMNFI)
	{
		List<PerkEvent> list = new List<PerkEvent>();
		if (node != null)
		{
			foreach (XmlNode childNode in node.ChildNodes)
			{
				string name = childNode.Name;
				PerkEvent gBMAKFJNAPG = null;
				switch (name)
				{
				case "RoundStageStart":
					gBMAKFJNAPG = new PerkEventRoundStage();
					break;
				case "HitPreCrit":
					gBMAKFJNAPG = new PerkEventPostHit();
					break;
				case "HitPostCrit":
					gBMAKFJNAPG = new PerkEventPostHit();
					break;
				case "PostHit":
					gBMAKFJNAPG = new PerkEventPostHit();
					break;
				case "AnimationStart":
					gBMAKFJNAPG = new PerkEventAnimationStart();
					break;
				case "AnimationEnd":
					gBMAKFJNAPG = new PerkEventAnimationStart();
					break;
				case "ModExpires":
					gBMAKFJNAPG = new PerkEventModExpires();
					break;
				case "EveryFrame":
					gBMAKFJNAPG = new PerkEventEveryFrame();
					break;
				case "AreaEnter":
					gBMAKFJNAPG = new PerkEventAreaEnter();
					break;
				case "AreaExit":
					gBMAKFJNAPG = new PerkEventAreaEnter();
					break;
				case "MagicCharged":
					gBMAKFJNAPG = new PerkEventAreaEnter();
					break;
				case "IntervalEnd":
					gBMAKFJNAPG = new PerkEventIntervalEnd();
					break;
				default:
					gBMAKFJNAPG = new PerkEvent();
					break;
				}
				gBMAKFJNAPG.JMOIMIHPBOM(AEFFHJGMNFI);
				gBMAKFJNAPG.Parse(childNode);
				if (gBMAKFJNAPG.get_Type() != KNKIIEPDCPN.EVENT_NONE)
				{
					list.Add(gBMAKFJNAPG);
				}
			}
		}
		return list;
	}

	public static PerkEvent Clone(PerkEvent BBLOGNPCPKI, PerkInfoItem AEFFHJGMNFI)
	{
		PerkEvent gBMAKFJNAPG = null;
		if (BBLOGNPCPKI != null)
		{
			switch (BBLOGNPCPKI.get_Type())
			{
			case KNKIIEPDCPN.EVENT_ROUND_STAGE_START:
				gBMAKFJNAPG = new PerkEventRoundStage((PerkEventRoundStage)BBLOGNPCPKI);
				break;
			case KNKIIEPDCPN.EVENT_HIT_PRECRIT:
				gBMAKFJNAPG = new PerkEventPostHit((PerkEventPostHit)BBLOGNPCPKI);
				break;
			case KNKIIEPDCPN.EVENT_HIT_POSTCRIT:
				gBMAKFJNAPG = new PerkEventPostHit((PerkEventPostHit)BBLOGNPCPKI);
				break;
			case KNKIIEPDCPN.EVENT_POST_HIT:
				gBMAKFJNAPG = new PerkEventPostHit((PerkEventPostHit)BBLOGNPCPKI);
				break;
			case KNKIIEPDCPN.EVENT_ANIMATION_START:
				gBMAKFJNAPG = new PerkEventAnimationStart((PerkEventAnimationStart)BBLOGNPCPKI);
				break;
			case KNKIIEPDCPN.EVENT_ANIMATION_END:
				gBMAKFJNAPG = new PerkEventAnimationStart((PerkEventAnimationStart)BBLOGNPCPKI);
				break;
			case KNKIIEPDCPN.EVENT_MOD_EXPIRES:
				gBMAKFJNAPG = new PerkEventModExpires((PerkEventModExpires)BBLOGNPCPKI);
				break;
			case KNKIIEPDCPN.EVENT_EVERY_FRAME:
				gBMAKFJNAPG = new PerkEventEveryFrame((PerkEventEveryFrame)BBLOGNPCPKI);
				break;
			case KNKIIEPDCPN.EVENT_AREA_ENTER:
				gBMAKFJNAPG = new PerkEventAreaEnter((PerkEventAreaEnter)BBLOGNPCPKI);
				break;
			case KNKIIEPDCPN.EVENT_AREA_EXIT:
				gBMAKFJNAPG = new PerkEventAreaEnter((PerkEventAreaEnter)BBLOGNPCPKI);
				break;
			case KNKIIEPDCPN.EVENT_MAGIC_CHARGED:
				gBMAKFJNAPG = new PerkEventAreaEnter((PerkEventAreaEnter)BBLOGNPCPKI);
				break;
			case KNKIIEPDCPN.EVENT_INTERVAL_END:
				gBMAKFJNAPG = new PerkEventIntervalEnd((PerkEventIntervalEnd)BBLOGNPCPKI);
				break;
			default:
				gBMAKFJNAPG = new PerkEvent(BBLOGNPCPKI);
				LLLOJBFMONN.Error("PerkEvent.Clone PerkEvent type is EventType.EVENT_NONE");
				break;
			}
			gBMAKFJNAPG.JMOIMIHPBOM(AEFFHJGMNFI);
		}
		return gBMAKFJNAPG;
	}

	public virtual bool IsEqual(EventStruct EJMEALJNNIL)
	{
		if (IHJJBIDMEMB == PlayerType.PLAYER_ME && EJMEALJNNIL.BMIGEFANCCC != EJMEALJNNIL.BIKLKJMNGKP)
		{
			return false;
		}
		if (IHJJBIDMEMB == PlayerType.PLAYER_ENEMY && EJMEALJNNIL.BMIGEFANCCC == EJMEALJNNIL.BIKLKJMNGKP)
		{
			return false;
		}
		return EJMEALJNNIL.Type == get_Type();
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		switch (node.Name)
		{
		case "RoundStageStart":
			set_Type(KNKIIEPDCPN.EVENT_ROUND_STAGE_START);
			break;
		case "EveryFrame":
			set_Type(KNKIIEPDCPN.EVENT_EVERY_FRAME);
			break;
		case "Style":
			set_Type(KNKIIEPDCPN.EVENT_STYLE);
			break;
		case "Combo":
			set_Type(KNKIIEPDCPN.EVENT_COMBO);
			break;
		case "HitPreCrit":
			set_Type(KNKIIEPDCPN.EVENT_HIT_PRECRIT);
			break;
		case "HitPostCrit":
			set_Type(KNKIIEPDCPN.EVENT_HIT_POSTCRIT);
			break;
		case "PostHit":
			set_Type(KNKIIEPDCPN.EVENT_POST_HIT);
			break;
		case "MagicCharged":
			set_Type(KNKIIEPDCPN.EVENT_MAGIC_CHARGED);
			break;
		case "AnimationStart":
			set_Type(KNKIIEPDCPN.EVENT_ANIMATION_START);
			break;
		case "AnimationEnd":
			set_Type(KNKIIEPDCPN.EVENT_ANIMATION_END);
			break;
		case "ModExpires":
			set_Type(KNKIIEPDCPN.EVENT_MOD_EXPIRES);
			break;
		case "AreaEnter":
			set_Type(KNKIIEPDCPN.EVENT_AREA_ENTER);
			break;
		case "AreaExit":
			set_Type(KNKIIEPDCPN.EVENT_AREA_EXIT);
			break;
		case "IntervalEnd":
			set_Type(KNKIIEPDCPN.EVENT_INTERVAL_END);
			break;
		default:
			set_Type(KNKIIEPDCPN.EVENT_NONE);
			break;
		}
	}
}

// Newer enchantments can proc when a named/type animation interval ends.
// The old fight runtime already emits this information, but its perk layer did
// not expose a corresponding event.
public class PerkEventIntervalEnd : PerkEvent
{
	private string _name = string.Empty;
	private IntervalAnimation.NGAJJDIEDGF _intervalType = IntervalAnimation.NGAJJDIEDGF.INTERVAL_NONE;

	public PerkEventIntervalEnd()
	{
	}

	public PerkEventIntervalEnd(PerkEventIntervalEnd other) : base(other)
	{
		_name = other._name;
		_intervalType = other._intervalType;
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		_name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		_intervalType = IntervalAnimation.LAJMDAFFPJE(node.Attributes["Type"].CIPOICEEIBK(string.Empty));
	}

	public override bool IsEqual(EventStruct eventInfo)
	{
		if (!base.IsEqual(eventInfo) || eventInfo == null || eventInfo.Info == null)
			return false;
		Dictionary<string, object> info = (Dictionary<string, object>)eventInfo.Info;
		IntervalAnimation interval = info.ContainsKey("Interval") ? info["Interval"] as IntervalAnimation : null;
		if (interval == null)
			return false;
		return (_name == string.Empty || interval.Name == _name) &&
			(_intervalType == IntervalAnimation.NGAJJDIEDGF.INTERVAL_NONE || interval.Type == _intervalType);
	}
}
