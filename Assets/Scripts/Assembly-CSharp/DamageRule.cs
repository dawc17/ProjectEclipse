using System.Xml;

public class DamageRule : InFightRule
{
	protected bool BACMFNLDDMM;

	public DamageRule(XmlNode node, RuleAppliance EJPOJJKKICO, BCBLLMPAMLP LFLGCDNKNJI)
		: base(LFLGCDNKNJI, EJPOJJKKICO, node)
	{
		BACMFNLDDMM = false;
		EBJIKKBLBEM(FightEvent.DamageCheckEvent);
	}

	public bool BKEAKKCDMMN()
	{
		return BACMFNLDDMM;
	}

	public override void InitRule(object data)
	{
		RuleInitData oIFPCFEGFOB = (RuleInitData)data;
		Compare(oIFPCFEGFOB.KNKNPEADHOF);
	}

	protected override bool CompareSingle(object data)
	{
		return false;
	}

	protected virtual bool CheckIsNoDamageChange(bool EGDPHJKMGAB)
	{
		bool result = BACMFNLDDMM != EGDPHJKMGAB;
		BACMFNLDDMM = EGDPHJKMGAB;
		return result;
	}

	protected virtual void GGBCFJJPOAB()
	{
		if (NDBMMPENJNJ == RuleAppliance.AppliancePlayer)
		{
			NDBMMPENJNJ = RuleAppliance.ApplianceOpponent;
		}
		else if (NDBMMPENJNJ == RuleAppliance.ApplianceOpponent)
		{
			NDBMMPENJNJ = RuleAppliance.AppliancePlayer;
		}
	}

	public override InFightRule Copy()
	{
		InFightRule aAJIFBJLJOA = null;
		RuleAppliance eJPOJJKKICO = EDAKADCHOLE();
		XmlNode hKPPBKPJOEO = GIFDJEEGCJI().IOJIGDNFCFL();
		aAJIFBJLJOA = new DamageRule(hKPPBKPJOEO, eJPOJJKKICO, _type);
		aAJIFBJLJOA.IsRandom = IsRandom;
		return aAJIFBJLJOA;
	}
}
