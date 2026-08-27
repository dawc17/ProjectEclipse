using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

public class PerkEventAnimationStart : PerkEvent
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string HKGHEJDKCPI;

	public PerkEventAnimationStart()
	{
	}

	public PerkEventAnimationStart(PerkEventAnimationStart NOLFMPDGCOC)
		: base(NOLFMPDGCOC)
	{
		set_Name(NOLFMPDGCOC.get_Name());
	}

	public string get_Name()
	{
		return HKGHEJDKCPI;
	}

	protected void set_Name(string value)
	{
		HKGHEJDKCPI = value;
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		set_Name(node.Attributes["Name"].CIPOICEEIBK(string.Empty));
	}

	public override bool IsEqual(EventStruct EJMEALJNNIL)
	{
		if (!base.IsEqual(EJMEALJNNIL) || EJMEALJNNIL == null || EJMEALJNNIL.Info == null)
		{
			return false;
		}
		Dictionary<string, object> dictionary = (Dictionary<string, object>)EJMEALJNNIL.Info;
		InfoAnimation pJAHIOELGGD = ((!dictionary.ContainsKey("Animation")) ? null : ((InfoAnimation)dictionary["Animation"]));
		if (get_Name() != string.Empty && (pJAHIOELGGD == null || !pJAHIOELGGD.CNPFHBMGDFP(get_Name())))
		{
			return false;
		}
		return true;
	}
}
