using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

public class PerkTrigger
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string HKGHEJDKCPI;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private PerkInfoItem DHNEANFLBJI;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private List<PerkEvent> PFFNNPMLBPD;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private List<PerkCondition> LEFCMOOIOCA;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private List<PerkAction> KEIGDAGJNLI;

	public PerkInfoItem JMDBPECCFDF
	{
		get
		{
			return JMDLAMHAJLN();
		}
		set
		{
			JMOIMIHPBOM(value);
		}
	}

	public List<PerkEvent> AJCMBMJGJEG
	{
		get
		{
			return PHLLJJNCEIH();
		}
		protected set
		{
			FHNHLFIKICP(value);
		}
	}

	public List<PerkCondition> JIFAHHGNPFH
	{
		get
		{
			return KJILOMLMMEN();
		}
		protected set
		{
			AJKANHBOADL(value);
		}
	}

	public List<PerkAction> DJBAIAKOIHM
	{
		get
		{
			return HIPOGANEPMI();
		}
		protected set
		{
			CLNPKBIMKJC(value);
		}
	}

	public string get_Name()
	{
		return HKGHEJDKCPI;
	}

	protected void set_Name(string value)
	{
		HKGHEJDKCPI = value;
	}

	public PerkInfoItem JMDLAMHAJLN()
	{
		return DHNEANFLBJI;
	}

	public void JMOIMIHPBOM(PerkInfoItem value)
	{
		DHNEANFLBJI = value;
	}

	public List<PerkEvent> PHLLJJNCEIH()
	{
		return PFFNNPMLBPD;
	}

	protected void FHNHLFIKICP(List<PerkEvent> value)
	{
		PFFNNPMLBPD = value;
	}

	public List<PerkCondition> KJILOMLMMEN()
	{
		return LEFCMOOIOCA;
	}

	protected void AJKANHBOADL(List<PerkCondition> value)
	{
		LEFCMOOIOCA = value;
	}

	public List<PerkAction> HIPOGANEPMI()
	{
		return KEIGDAGJNLI;
	}

	protected void CLNPKBIMKJC(List<PerkAction> value)
	{
		KEIGDAGJNLI = value;
	}

	public void Parse(XmlNode node)
	{
		set_Name(node.Attributes["Name"].CIPOICEEIBK(string.Empty));
		XmlNode hKPPBKPJOEO = node["Events"];
		XmlNode hKPPBKPJOEO2 = node["Conditions"];
		XmlNode hKPPBKPJOEO3 = node["Actions"];
		FHNHLFIKICP(PerkEvent.Create(hKPPBKPJOEO, JMDLAMHAJLN()));
		AJKANHBOADL(PerkCondition.Create(hKPPBKPJOEO2, JMDLAMHAJLN()));
		CLNPKBIMKJC(PerkAction.Create(hKPPBKPJOEO3, JMDLAMHAJLN(), this));
	}

	public bool MIMBCGNGGHO(PerkEvent.EventStruct EJMEALJNNIL)
	{
		PerkEvent gBMAKFJNAPG = null;
		for (int i = 0; i < PHLLJJNCEIH().Count; i++)
		{
			gBMAKFJNAPG = PHLLJJNCEIH()[i];
			bool flag = gBMAKFJNAPG.IsEqual(EJMEALJNNIL);
			if ((!gBMAKFJNAPG.IsNot) ? flag : (!flag))
			{
				return true;
			}
		}
		return false;
	}

	public bool IPFOGLIBLLB(Model ACENLMONNPA, List<string> NIKHAICFGNM)
	{
		PerkCondition iDJILNODHAD = null;
		for (int i = 0; i < KJILOMLMMEN().Count; i++)
		{
			iDJILNODHAD = KJILOMLMMEN()[i];
			bool flag = iDJILNODHAD.IsEqual(ACENLMONNPA, NIKHAICFGNM);
			if (!((!iDJILNODHAD.IsNot) ? flag : (!flag)))
			{
				return false;
			}
		}
		return true;
	}
}
