using System.Diagnostics;
using System.Xml;

public class PerkActionShowIcon : PerkActionModificator
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string JDDPALEMDNE;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool KKDOCIKMKDB;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int KOCNAIJGEMK;

	public string NHKMCLPOMFK
	{
		get
		{
			return AJAEJNGLKOK();
		}
		protected set
		{
			set_Image(value);
		}
	}

	public bool FLNCPBKBJBL
	{
		get
		{
			return ECKEHGCGBBP();
		}
		protected set
		{
			set_ShowExpiration(value);
		}
	}

	public int MGDCIODPHCH
	{
		get
		{
			return NKHNFHIKGIG();
		}
		protected set
		{
			set_ExpirationVer(value);
		}
	}

	public PerkActionShowIcon()
	{
	}

	public PerkActionShowIcon(PerkActionShowIcon NOLFMPDGCOC)
		: base(NOLFMPDGCOC)
	{
		set_Image(NOLFMPDGCOC.AJAEJNGLKOK());
		set_ShowExpiration(NOLFMPDGCOC.ECKEHGCGBBP());
		set_ExpirationVer(NOLFMPDGCOC.NKHNFHIKGIG());
	}

	public string AJAEJNGLKOK()
	{
		return JDDPALEMDNE;
	}

	protected void set_Image(string value)
	{
		JDDPALEMDNE = value;
	}

	public bool ECKEHGCGBBP()
	{
		return KKDOCIKMKDB;
	}

	protected void set_ShowExpiration(bool value)
	{
		KKDOCIKMKDB = value;
	}

	public int NKHNFHIKGIG()
	{
		return KOCNAIJGEMK;
	}

	protected void set_ExpirationVer(int value)
	{
		KOCNAIJGEMK = value;
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		set_Type(ActionType.ACTION_SHOW_ICONS);
		set_Image(node.Attributes["Image"].CIPOICEEIBK(string.Empty));
		set_ShowExpiration(node.Attributes["ShowExpiration"].ParseBool());
		set_ExpirationVer(node.Attributes["ExpirationVer"].ParseInt());
	}
}
