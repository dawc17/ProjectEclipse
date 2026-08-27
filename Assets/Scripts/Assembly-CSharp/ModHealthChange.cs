using System.Diagnostics;
using System.Xml;

public class ModHealthChange : PerkActionModificator
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private float CKNAHAEAPAJ;

	public float NGKLOKPGGMF
	{
		get
		{
			return JMPIBKKAHJP();
		}
		protected set
		{
			set_PerFrameValue(value);
		}
	}

	public ModHealthChange()
	{
	}

	public ModHealthChange(ModHealthChange NOLFMPDGCOC)
		: base(NOLFMPDGCOC)
	{
		set_PerFrameValue(NOLFMPDGCOC.JMPIBKKAHJP());
	}

	public float JMPIBKKAHJP()
	{
		return CKNAHAEAPAJ;
	}

	protected void set_PerFrameValue(float value)
	{
		CKNAHAEAPAJ = value;
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		set_Type(ActionType.ACTION_MOD_HEALTH_CHANGE);
		set_PerFrameValue(node.Attributes["PerFrameValue"].ParseFloat());
	}
}
