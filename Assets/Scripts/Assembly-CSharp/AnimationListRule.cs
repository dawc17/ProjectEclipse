using System.Collections.Generic;
using System.Xml;

public class AnimationListRule : InFightRule
{
	protected List<InfoAnimation> KABLHHCPDPD = new List<InfoAnimation>();

	public AnimationListRule(BCBLLMPAMLP LFLGCDNKNJI, RuleAppliance EJPOJJKKICO, XmlNode node)
		: base(LFLGCDNKNJI, EJPOJJKKICO, node)
	{
		FillAnimations(node);
	}

	public bool CheckAnimation(InfoAnimation DBOLBEOCEME)
	{
		if (DBOLBEOCEME == null)
		{
			return false;
		}
		return CheckAnimation(DBOLBEOCEME.Name);
	}

	public bool CheckAnimation(string name)
	{
		foreach (InfoAnimation item in KABLHHCPDPD)
		{
			if (item.Name == name || item.LPPIKDGABOL(name))
			{
				return true;
			}
		}
		return false;
	}

	protected void FillAnimations(XmlNode node)
	{
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.Name == "Animation")
			{
				string gOHIIMFFFJI = childNode.Attributes["Name"].CIPOICEEIBK(string.Empty);
				AnimationData.NEBELEFIDMB(gOHIIMFFFJI, KABLHHCPDPD);
			}
		}
	}

	public override InFightRule Copy()
	{
		AnimationListRule kCGODLBLCDJ = null;
		RuleAppliance eJPOJJKKICO = EDAKADCHOLE();
		XmlNode hKPPBKPJOEO = GIFDJEEGCJI().IOJIGDNFCFL();
		kCGODLBLCDJ = new AnimationListRule(_type, eJPOJJKKICO, hKPPBKPJOEO);
		kCGODLBLCDJ.IsRandom = IsRandom;
		return kCGODLBLCDJ;
	}
}
