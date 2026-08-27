using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

public class PerkEventModExpires : PerkEvent
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string OPAFELFOFFB;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string HCHALPNMNMK;

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

	public PerkEventModExpires()
	{
	}

	public PerkEventModExpires(PerkEventModExpires NOLFMPDGCOC)
		: base(NOLFMPDGCOC)
	{
		set_ModName(NOLFMPDGCOC.CMKKGFDBBJF());
		set_Namespace(NOLFMPDGCOC.IONIEDIPEGB());
	}

	public string CMKKGFDBBJF()
	{
		return OPAFELFOFFB;
	}

	protected void set_ModName(string value)
	{
		OPAFELFOFFB = value;
	}

	public string IONIEDIPEGB()
	{
		return HCHALPNMNMK;
	}

	protected void set_Namespace(string value)
	{
		HCHALPNMNMK = value;
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		set_ModName(node.Attributes["Name"].CIPOICEEIBK(string.Empty));
		set_Namespace(node.Attributes["Namespace"].CIPOICEEIBK(string.Empty));
	}

	public override bool IsEqual(EventStruct EJMEALJNNIL)
	{
		if (IONIEDIPEGB() == null || IONIEDIPEGB() == string.Empty)
		{
			if (!base.IsEqual(EJMEALJNNIL) || EJMEALJNNIL == null || EJMEALJNNIL.Info == null)
			{
				return false;
			}
		}
		else if (IONIEDIPEGB().Equals(EJMEALJNNIL.Namespace))
		{
			return false;
		}
		Dictionary<string, object> dictionary = (Dictionary<string, object>)EJMEALJNNIL.Info;
		if (dictionary != null)
		{
			string value = ((!dictionary.ContainsKey("ModExpires")) ? null : ((string)dictionary["ModExpires"]));
			PerkInfoItem aCONCDFDNJH = ((!dictionary.ContainsKey("ParentPerk")) ? null : ((PerkInfoItem)dictionary["ParentPerk"]));
			if (aCONCDFDNJH == JMDLAMHAJLN() && (CMKKGFDBBJF() == null || CMKKGFDBBJF() == string.Empty || CMKKGFDBBJF().Equals(value)))
			{
				return true;
			}
		}
		return false;
	}
}
