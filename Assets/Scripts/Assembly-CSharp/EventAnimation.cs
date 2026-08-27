using System.Xml;

public class EventAnimation
{
	public enum EECEJKADLCK
	{
		EVENT_NONE = 0,
		EVENT_ROUND_STAGE = 1,
		EVENT_KEY_PRESSED = 2,
		EVENT_KEY_RELEASED = 3,
		EVENT_ROUND_START = 4,
		EVENT_ROUND_END = 5,
		EVENT_HIT = 6,
		EVENT_STRIKE = 7,
		EVENT_WALL_HIT = 8,
		EVENT_ANIMATION_START = 9,
		EVENT_ANIMATION_END = 10,
		EVENT_INTERVAL_START = 11,
		EVENT_INTERVAL_END = 12,
		EVENT_EVERY_FRAME = 13,
		EVENT_BIRTH = 14,
		EVENT_MOD_EXPIRES = 15
	}

	public ModelConditions JIFAHHGNPFH;

	public EECEJKADLCK Type;

	public string LJICHLHMBFA;

	public string LONCGFHLFKA;

	public string PLNBENLPIBD;

	public bool IsNot;

	public ModelType.KEIDBIOIFGA IHJJBIDMEMB;

	public EventAnimation(EECEJKADLCK LFLGCDNKNJI = EECEJKADLCK.EVENT_NONE)
	{
		Type = LFLGCDNKNJI;
		JIFAHHGNPFH = null;
	}

	public static string NJFGAJFCCGD(EECEJKADLCK LFLGCDNKNJI)
	{
		switch (LFLGCDNKNJI)
		{
		case EECEJKADLCK.EVENT_NONE:
			return "None";
		case EECEJKADLCK.EVENT_ROUND_STAGE:
			return "RoundStage";
		case EECEJKADLCK.EVENT_KEY_PRESSED:
			return "KeyPressed";
		case EECEJKADLCK.EVENT_KEY_RELEASED:
			return "KeyReleased";
		case EECEJKADLCK.EVENT_ROUND_START:
			return "RoundStart";
		case EECEJKADLCK.EVENT_ROUND_END:
			return "RoundEnd";
		case EECEJKADLCK.EVENT_HIT:
			return "Hit";
		case EECEJKADLCK.EVENT_STRIKE:
			return "Strike";
		case EECEJKADLCK.EVENT_WALL_HIT:
			return "WallHit";
		case EECEJKADLCK.EVENT_ANIMATION_START:
			return "AnimationStart";
		case EECEJKADLCK.EVENT_ANIMATION_END:
			return "AnimationEnd";
		case EECEJKADLCK.EVENT_INTERVAL_START:
			return "IntervalStart";
		case EECEJKADLCK.EVENT_INTERVAL_END:
			return "IntervalEnd";
		case EECEJKADLCK.EVENT_EVERY_FRAME:
			return "EveryFrame";
		case EECEJKADLCK.EVENT_BIRTH:
			return "Birth";
		case EECEJKADLCK.EVENT_MOD_EXPIRES:
			return "ModExpires";
		default:
			return "None";
		}
	}

	public static EECEJKADLCK IOPCBLBFLKB(string name)
	{
		switch (name)
		{
		case "None":
		case "":
		case null:
			return EECEJKADLCK.EVENT_NONE;
		case "RoundStage":
			return EECEJKADLCK.EVENT_ROUND_STAGE;
		case "KeyPressed":
			return EECEJKADLCK.EVENT_KEY_PRESSED;
		case "RoundStart":
			return EECEJKADLCK.EVENT_ROUND_START;
		case "RoundEnd":
			return EECEJKADLCK.EVENT_ROUND_END;
		case "Hit":
			return EECEJKADLCK.EVENT_HIT;
		case "Strike":
			return EECEJKADLCK.EVENT_STRIKE;
		case "WallHit":
			return EECEJKADLCK.EVENT_WALL_HIT;
		case "AnimationStart":
			return EECEJKADLCK.EVENT_ANIMATION_START;
		case "AnimationEnd":
			return EECEJKADLCK.EVENT_ANIMATION_END;
		case "IntervalStart":
			return EECEJKADLCK.EVENT_INTERVAL_START;
		case "IntervalEnd":
			return EECEJKADLCK.EVENT_INTERVAL_END;
		case "EveryFrame":
			return EECEJKADLCK.EVENT_EVERY_FRAME;
		case "Birth":
			return EECEJKADLCK.EVENT_BIRTH;
		case "KeyReleased":
			return EECEJKADLCK.EVENT_KEY_RELEASED;
		case "ModExpires":
			return EECEJKADLCK.EVENT_MOD_EXPIRES;
		default:
			LLLOJBFMONN.Error("getEventTypeByName - unknown type: \"{0}\"", name);
			return EECEJKADLCK.EVENT_NONE;
		}
	}

	public bool IsEqual(EventAnimation JHJEPJJOCAE)
	{
		if (Type == JHJEPJJOCAE.Type)
		{
			return Compare(JHJEPJJOCAE);
		}
		return false;
	}

	public void Init(XmlNode EIGDDPDGIAN)
	{
		LJICHLHMBFA = EIGDDPDGIAN.Attributes["Name"].CIPOICEEIBK(string.Empty);
		LONCGFHLFKA = EIGDDPDGIAN.Attributes["Type"].CIPOICEEIBK(string.Empty);
		PLNBENLPIBD = EIGDDPDGIAN.Attributes["Stage"].CIPOICEEIBK(string.Empty);
		IsNot = EIGDDPDGIAN.Attributes["Not"].ParseBool();
		IHJJBIDMEMB = ModelType.EHFNOBFLAHI(EIGDDPDGIAN.Attributes["Player"].CIPOICEEIBK("Me"));
		Parse(EIGDDPDGIAN);
	}

	protected virtual bool Compare(EventAnimation FOPOKALJIIJ)
	{
		if (Type == EECEJKADLCK.EVENT_HIT)
		{
			bool flag = false;
			if (!string.IsNullOrEmpty(LJICHLHMBFA) && (!(LJICHLHMBFA == FOPOKALJIIJ.LJICHLHMBFA) || 1 == 0))
			{
				return false;
			}
			if (string.IsNullOrEmpty(LONCGFHLFKA))
			{
				return true;
			}
			string[] array = LONCGFHLFKA.Split('|');
			for (int i = 0; i < array.Length; i++)
			{
				if (LONCGFHLFKA == array[i])
				{
					return true;
				}
			}
		}
		return false;
	}

	protected virtual void Parse(XmlNode MEEAKLDGLDF)
	{
	}
}
