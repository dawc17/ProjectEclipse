using System.Diagnostics;
using System.Xml;

public class PerkActionChangeHitEffectScale : PerkAction
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private float LFMJEGFEAPD;

	public float JNKHFJJGJAF
	{
		get
		{
			return DNOILFCGCGD();
		}
		protected set
		{
			set_HitEffectScale(value);
		}
	}

	public PerkActionChangeHitEffectScale()
	{
		set_HitEffectScale(1f);
	}

	public PerkActionChangeHitEffectScale(PerkActionChangeHitEffectScale NOLFMPDGCOC)
		: base(NOLFMPDGCOC)
	{
		set_HitEffectScale(NOLFMPDGCOC.DNOILFCGCGD());
	}

	public float DNOILFCGCGD()
	{
		return LFMJEGFEAPD;
	}

	protected void set_HitEffectScale(float value)
	{
		LFMJEGFEAPD = value;
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		set_Type(ActionType.ACTION_CHANGE_HIT_EFFECT_SCALE);
		set_HitEffectScale(node.Attributes["Scale"].ParseFloat());
	}
}
