using System.Xml;

public class DamageFactorRule : AnimationListRule
{
	public const float KJBDNLFDCJA = 1f;

	public const float HLPMOPAJKKD = 1f;

	private float IOGMPFJOCPE;

	private float BEMFHMKCJOK;

	public DamageFactorRule(XmlNode node, RuleAppliance EJPOJJKKICO)
		: base(BCBLLMPAMLP.RuleDamageFactor, EJPOJJKKICO, node)
	{
		Parse(node);
	}

	protected override bool CompareSingle(object data)
	{
		FightData hCPJJKMNMCE = (FightData)data;
		return CheckAnimation(hCPJJKMNMCE.LKLHCEEMINM);
	}

	public override void InitRule(object data)
	{
		foreach (InfoAnimation item in KABLHHCPDPD)
		{
			foreach (IntervalAnimation item2 in item.ODACDCDONJE.Intervals)
			{
				if (item2.Type == IntervalAnimation.NGAJJDIEDGF.INTERVAL_ATTACK)
				{
					IntervalAttack hFIIPNLCIEE = (IntervalAttack)item2;
					IntervalAttack.Factors bPLPKPIBEIF = hFIIPNLCIEE.GetFactors(NDBMMPENJNJ);
					if (bPLPKPIBEIF.FNDCJJNDNJC || bPLPKPIBEIF.DJGAHEOIHGG)
					{
						break;
					}
					bPLPKPIBEIF.FNDCJJNDNJC = true;
					bPLPKPIBEIF.Factor = IOGMPFJOCPE;
					bPLPKPIBEIF.DJGAHEOIHGG = true;
					bPLPKPIBEIF.HJIIIBHAOMJ = BEMFHMKCJOK;
				}
			}
		}
	}

	public override void Reset()
	{
		Clear();
	}

	public override void Clear()
	{
		foreach (InfoAnimation item in KABLHHCPDPD)
		{
			foreach (IntervalAnimation item2 in item.ODACDCDONJE.Intervals)
			{
				if (item2.Type == IntervalAnimation.NGAJJDIEDGF.INTERVAL_ATTACK)
				{
					IntervalAttack hFIIPNLCIEE = (IntervalAttack)item2;
					IntervalAttack.Factors bPLPKPIBEIF = hFIIPNLCIEE.GetFactors(NDBMMPENJNJ);
					if (bPLPKPIBEIF.FNDCJJNDNJC || bPLPKPIBEIF.DJGAHEOIHGG)
					{
						bPLPKPIBEIF.FNDCJJNDNJC = false;
						bPLPKPIBEIF.Factor = 1f;
						bPLPKPIBEIF.DJGAHEOIHGG = false;
						bPLPKPIBEIF.HJIIIBHAOMJ = 1f;
					}
				}
			}
		}
	}

	protected override void Parse(XmlNode node)
	{
		base.Parse(node);
		string gOHIIMFFFJI = node.Attributes["Animation"].CIPOICEEIBK(string.Empty);
		AnimationData.NEBELEFIDMB(gOHIIMFFFJI, KABLHHCPDPD);
		IOGMPFJOCPE = node.Attributes["Factor"].ParseFloat(1f);
		BEMFHMKCJOK = node.Attributes["RepeatFactor"].ParseFloat(1f);
	}
}
