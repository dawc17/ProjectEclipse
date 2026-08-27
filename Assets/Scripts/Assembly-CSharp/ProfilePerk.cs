public class ProfilePerk : global::EventDispatcher<object>
{
	public enum OLBACIJLKBI
	{
		PerkStateUpdate = 0,
		PerkDesroyed = 1,
		PerkChanged = 2,
		PerkInfoChanged = 3
	}

	public enum KMHBPKKCNPP
	{
		PERK_AVAILABLE = 0,
		PERK_UNAVAILABLE = 1,
		PERK_SELECTED = 2,
		PERK_LOCK = 3
	}

	public enum JHDKDOPHGOO
	{
		TYPE_NONE = 0,
		TYPE_PERK = 1,
		TYPE_UPGRADE = 2,
		TYPE_PERK_SELETED = 3
	}

	protected KMHBPKKCNPP MAFFNGPOMJD;

	protected int _level;

	protected PerkInfoItem ILJJPHHDIJI;

	public bool IsNew;

	protected JHDKDOPHGOO _type;

	protected string _description;

	public KMHBPKKCNPP AFINHOBCHMC
	{
		get
		{
			return FLBBFDNHJAJ();
		}
		set
		{
			set_State(value);
		}
	}

	public int Level
	{
		get
		{
			return PINDEKDNCNL();
		}
	}

	public PerkInfoItem MBDDKGIOOGD
	{
		get
		{
			return DFOELJAEEGG();
		}
		set
		{
			NOLDHAFMOLF(value);
		}
	}

	public string MGNNJPBCOGD
	{
		get
		{
			return GJOAJAIJHOE();
		}
		set
		{
			set_Description(value);
		}
	}

	public ProfilePerk(PerkInfoItem AEFFHJGMNFI, int GNLOCMLBNHF, KMHBPKKCNPP state = KMHBPKKCNPP.PERK_AVAILABLE, JHDKDOPHGOO LFLGCDNKNJI = JHDKDOPHGOO.TYPE_NONE)
	{
		MAFFNGPOMJD = state;
		_level = GNLOCMLBNHF;
		ILJJPHHDIJI = AEFFHJGMNFI;
		IsNew = false;
		_type = LFLGCDNKNJI;
		_description = ((ILJJPHHDIJI == null) ? string.Empty : ILJJPHHDIJI.MGNNJPBCOGD);
	}

	public KMHBPKKCNPP FLBBFDNHJAJ()
	{
		return MAFFNGPOMJD;
	}

	public void set_State(KMHBPKKCNPP value)
	{
		MAFFNGPOMJD = value;
		CallEvent(0, MAFFNGPOMJD);
	}

	public int PINDEKDNCNL()
	{
		return _level;
	}

	public PerkInfoItem DFOELJAEEGG()
	{
		return ILJJPHHDIJI;
	}

	public void NOLDHAFMOLF(PerkInfoItem value)
	{
		ILJJPHHDIJI = value;
		CallEvent(2, 0);
	}

	public JHDKDOPHGOO get_Type()
	{
		return _type;
	}

	public string GJOAJAIJHOE()
	{
		return _description;
	}

	public void set_Description(string value)
	{
		_description = value;
		CallEvent(3, 0);
	}

	private void ANIDBLANMIC()
	{
		CallEvent(1, null);
	}

	public bool Islevel(int OMHDLKNHNMJ)
	{
		return OMHDLKNHNMJ < _level;
	}

	public int LMGGMMFEODJ()
	{
		return (ILJJPHHDIJI != null) ? ILJJPHHDIJI.AKKLOMFOLNO : 0;
	}

	public string CEENDGFFEFM()
	{
		return (ILJJPHHDIJI == null) ? string.Empty : ILJJPHHDIJI.JNBECGKCNBB;
	}

	public string KAMBOKLFBEE()
	{
		return (ILJJPHHDIJI == null) ? string.Empty : ILJJPHHDIJI.Name;
	}

	public string OPIOIHAPMDG()
	{
		return (ILJJPHHDIJI == null) ? string.Empty : ILJJPHHDIJI.NHKMCLPOMFK;
	}
}
