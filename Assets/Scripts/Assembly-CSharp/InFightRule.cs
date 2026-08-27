using System.Collections.Generic;
using System.Xml;

public class InFightRule : Rule
{
	protected RuleAppliance NDBMMPENJNJ;

	protected HashSet<FightEvent> PDIDNJAAPIH;

	protected bool HBDGMDPIAJD;

	protected bool KOKHKAFELGL;

	public InFightRule(BCBLLMPAMLP LFLGCDNKNJI, RuleAppliance EJPOJJKKICO, XmlNode node, InFightRule CEFOMFMPHJM = null)
		: base(LFLGCDNKNJI, node)
	{
		NDBMMPENJNJ = EJPOJJKKICO;
		KOKHKAFELGL = true;
		HBDGMDPIAJD = false;
		PDIDNJAAPIH = new HashSet<FightEvent>();
		HBDGMDPIAJD = node.Attributes["Death"].ParseBool();
	}

	public virtual InFightRule Copy()
	{
		return null;
	}

	public void EBJIKKBLBEM(FightEvent KOJNCHKPLLN)
	{
		PDIDNJAAPIH.Add(KOJNCHKPLLN);
	}

	public bool PMBJPCMHJOA(FightEvent KOJNCHKPLLN)
	{
		return PDIDNJAAPIH.Contains(KOJNCHKPLLN);
	}

	public virtual void Reset()
	{
	}

	public override bool Compare(object data)
	{
		AGCBHKBNMKL(data);
		PlayersFightData jNGGHELCPFM = (PlayersFightData)data;
		if (NDBMMPENJNJ == RuleAppliance.AppliancePlayer)
		{
			return CompareSingle(jNGGHELCPFM.MPLPEMOFHGI);
		}
		if (NDBMMPENJNJ == RuleAppliance.ApplianceOpponent)
		{
			return CompareSingle(jNGGHELCPFM.EKBMBILHBMC);
		}
		return false;
	}

	protected virtual bool CompareSingle(object data)
	{
		return false;
	}

	public virtual void InitRule(object data)
	{
	}

	public virtual void Clear()
	{
	}

	public virtual void Stop()
	{
	}

	public bool JKDDBGJKEMC()
	{
		return KOKHKAFELGL;
	}

	public bool OBDNDAEPPNN()
	{
		return HBDGMDPIAJD;
	}

	public RuleAppliance EDAKADCHOLE()
	{
		return NDBMMPENJNJ;
	}

	public void MOEAPHGDNAB(RuleAppliance IGFNCCEHFEK)
	{
		NDBMMPENJNJ = IGFNCCEHFEK;
	}

	public virtual RuleAppliance IMINMDOFHMG()
	{
		switch (NDBMMPENJNJ)
		{
		case RuleAppliance.AppliancePlayer:
			return (!KOKHKAFELGL) ? RuleAppliance.AppliancePlayer : RuleAppliance.ApplianceOpponent;
		case RuleAppliance.ApplianceOpponent:
			return KOKHKAFELGL ? RuleAppliance.AppliancePlayer : RuleAppliance.ApplianceOpponent;
		default:
			LLLOJBFMONN.Error("InFightRule::getWinnerAppliance ERROR  - wrong playerAppliance " + NDBMMPENJNJ);
			return RuleAppliance.ApplianceAll;
		}
	}

	protected virtual void AGCBHKBNMKL(object data)
	{
	}
}
