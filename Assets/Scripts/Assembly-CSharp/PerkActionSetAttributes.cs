using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

public class PerkActionSetAttributes : PerkActionModificator
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Dictionary<string, FunctionExtension> LGLIMFLAKFK;

	public Dictionary<string, FunctionExtension> PGPKNHDMNBD
	{
		get
		{
			return NNBFJDJAAGI();
		}
		protected set
		{
			PIOBIGEOKHN(value);
		}
	}

	public PerkActionSetAttributes()
	{
	}

	public PerkActionSetAttributes(PerkActionSetAttributes NOLFMPDGCOC)
		: base(NOLFMPDGCOC)
	{
		PIOBIGEOKHN(NOLFMPDGCOC.NNBFJDJAAGI());
	}

	public Dictionary<string, FunctionExtension> NNBFJDJAAGI()
	{
		return LGLIMFLAKFK;
	}

	protected void PIOBIGEOKHN(Dictionary<string, FunctionExtension> value)
	{
		LGLIMFLAKFK = value;
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		set_Type(ActionType.ACTION_SET_ATTRIBUTES);
		PIOBIGEOKHN(new Dictionary<string, FunctionExtension>());
		foreach (WarriorAttribute item in GameUtils.BGENALLCKII.IBLHIAHECLK)
		{
			XmlAttribute xmlAttribute = node.Attributes[item.get_Name()];
			if (xmlAttribute != null)
			{
				string key = item.get_Name();
				string bLLCOEAOJGF = xmlAttribute.CIPOICEEIBK(string.Empty);
				FunctionExtension oPIFBDJNMKD = new FunctionExtension();
				oPIFBDJNMKD.Parse(bLLCOEAOJGF);
				oPIFBDJNMKD.PBPBNENGLPA(JMDLAMHAJLN().HJFEFJIEINN);
				oPIFBDJNMKD.DMPCFMACDJM(JMDLAMHAJLN().OKPFNCJFLDL);
				oPIFBDJNMKD.set_Target(this);
				NNBFJDJAAGI()[key] = oPIFBDJNMKD;
			}
		}
	}
}
