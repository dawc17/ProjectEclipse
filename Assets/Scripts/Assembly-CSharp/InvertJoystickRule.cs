using System.Xml;

public class InvertJoystickRule : InFightRule
{
	public InvertJoystickRule(XmlNode node)
		: base(BCBLLMPAMLP.RuleInvertJoystick, RuleAppliance.AppliancePlayer, node)
	{
		Parse(node);
	}

	public override InFightRule Copy()
	{
		InFightRule aAJIFBJLJOA = null;
		XmlNode hKPPBKPJOEO = GIFDJEEGCJI().IOJIGDNFCFL();
		aAJIFBJLJOA = new InvertJoystickRule(hKPPBKPJOEO);
		aAJIFBJLJOA.IsRandom = IsRandom;
		return aAJIFBJLJOA;
	}
}
