using System.Diagnostics;
using System.Xml;

public class PerkActionChangeAdditionalDamageValue : PerkActionModificator
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private float FEAEEMLEBKI;

	public float OMKEMNJKFEO
	{
		get
		{
			return JKEKBCJHANF();
		}
		protected set
		{
			set_AdditionalDamageValue(value);
		}
	}

	public PerkActionChangeAdditionalDamageValue()
	{
		set_AdditionalDamageValue(0f);
	}

	public PerkActionChangeAdditionalDamageValue(PerkActionChangeAdditionalDamageValue NOLFMPDGCOC)
		: base(NOLFMPDGCOC)
	{
		set_AdditionalDamageValue(NOLFMPDGCOC.JKEKBCJHANF());
	}

	public float JKEKBCJHANF()
	{
		return FEAEEMLEBKI;
	}

	protected void set_AdditionalDamageValue(float value)
	{
		FEAEEMLEBKI = value;
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		set_Type(ActionType.ACTION_CHANGE_ADD_DAMAGE_VALUE);
		set_AdditionalDamageValue(node.Attributes["Value"].ParseFloat());
	}
}
