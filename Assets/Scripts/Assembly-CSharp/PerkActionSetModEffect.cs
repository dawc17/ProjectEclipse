using System.Diagnostics;
using System.Xml;

internal class PerkActionSetModEffect : PerkAction
{
	public enum COLPJOBKGEI
	{
		EFFECT_NONE = 0,
		EFFECT_PULSE = 1
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string OPAFELFOFFB;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private COLPJOBKGEI KLKJOHCLJCH;

	public string POLPHCDNLEL
	{
		get
		{
			return CMKKGFDBBJF();
		}
		protected set
		{
			set_ModName(value);
		}
	}

	public COLPJOBKGEI EOKLCEHFGDC
	{
		get
		{
			return CKEDENENELC();
		}
		protected set
		{
			GKKKNLKBHNJ(value);
		}
	}

	public PerkActionSetModEffect()
	{
	}

	public PerkActionSetModEffect(PerkActionSetModEffect NOLFMPDGCOC)
		: base(NOLFMPDGCOC)
	{
		set_ModName(NOLFMPDGCOC.CMKKGFDBBJF());
		GKKKNLKBHNJ(NOLFMPDGCOC.CKEDENENELC());
	}

	public string CMKKGFDBBJF()
	{
		return OPAFELFOFFB;
	}

	protected void set_ModName(string value)
	{
		OPAFELFOFFB = value;
	}

	public COLPJOBKGEI CKEDENENELC()
	{
		return KLKJOHCLJCH;
	}

	protected void GKKKNLKBHNJ(COLPJOBKGEI value)
	{
		KLKJOHCLJCH = value;
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		set_Type(ActionType.ACTION_MOD_EFFECT);
		set_ModName(node.Attributes["Name"].CIPOICEEIBK(string.Empty));
		GKKKNLKBHNJ(AFFIICKHKKN(node.Attributes["Type"].CIPOICEEIBK(string.Empty)));
	}

	private COLPJOBKGEI AFFIICKHKKN(string CNKBLODAFDO)
	{
		if (CNKBLODAFDO.Equals("Pulse"))
		{
			return COLPJOBKGEI.EFFECT_PULSE;
		}
		return COLPJOBKGEI.EFFECT_NONE;
	}
}
