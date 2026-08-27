using System.Xml;

public class RegenerationRule : InFightRule
{
	private float _frames;

	private float GGJKPLJLFLC;

	private float AAIJMKAHNKP;

	private bool _isWeaponStrike;

	public RegenerationRule(XmlNode node, RuleAppliance EJPOJJKKICO)
		: base(BCBLLMPAMLP.RuleRegeneration, EJPOJJKKICO, node)
	{
		_frames = 0f;
		GGJKPLJLFLC = 0f;
		AAIJMKAHNKP = 0f;
		_isWeaponStrike = false;
		EBJIKKBLBEM(FightEvent.HitEvent);
		EBJIKKBLBEM(FightEvent.RenderEvent);
		Parse(node);
		Reset();
	}

	public override void InitRule(object data)
	{
		_frames = 0f;
	}

	public float BIGCPKBIJNA()
	{
		return AAIJMKAHNKP;
	}

	protected override bool CompareSingle(object data)
	{
		FightData hCPJJKMNMCE = (FightData)data;
		switch (hCPJJKMNMCE.KOJNCHKPLLN)
		{
		case FightEvent.RenderEvent:
			_frames++;
			if (_isWeaponStrike && hCPJJKMNMCE.CBLNOFELDOE)
			{
				return false;
			}
			if (_frames >= GGJKPLJLFLC)
			{
				return true;
			}
			break;
		case FightEvent.HitEvent:
			_frames = 0f;
			break;
		}
		return false;
	}

	protected override void Parse(XmlNode node)
	{
		base.Parse(node);
		GGJKPLJLFLC = node.Attributes["FramesAfterHit"].ParseFloat();
		AAIJMKAHNKP = node.Attributes["Rate"].ParseFloat();
		_isWeaponStrike = node.Attributes["WeaponStrike"].ParseBool();
	}

	public override InFightRule Copy()
	{
		InFightRule aAJIFBJLJOA = null;
		RuleAppliance eJPOJJKKICO = EDAKADCHOLE();
		XmlNode hKPPBKPJOEO = GIFDJEEGCJI().IOJIGDNFCFL();
		aAJIFBJLJOA = new RegenerationRule(hKPPBKPJOEO, eJPOJJKKICO);
		aAJIFBJLJOA.IsRandom = IsRandom;
		return aAJIFBJLJOA;
	}
}
