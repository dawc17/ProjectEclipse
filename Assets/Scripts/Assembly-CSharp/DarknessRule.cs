using System.Xml;

public class DarknessRule : InFightRule
{
	public enum CPIDLPIBLHE
	{
		STAGE_BLACKOUT = 0,
		STAGE_LASTING = 1,
		STAGE_LIGHT = 2,
		STAGE_PAUSE = 3
	}

	private LocationSelectorDarknessData HNPFNOPIIMJ = new LocationSelectorDarknessData();

	private bool _active;

	private int _currentFrame;

	private float MFBOIBHHOHE;

	private float GJEHCPKKOLA;

	private float GJENDIFGOAF;

	protected CPIDLPIBLHE HGCCKCCJBGG;

	public DarknessRule(XmlNode node, RuleAppliance EJPOJJKKICO)
		: base(BCBLLMPAMLP.RuleDarkness, EJPOJJKKICO, node)
	{
		HNPFNOPIIMJ = new LocationSelectorDarknessData();
		HGCCKCCJBGG = CPIDLPIBLHE.STAGE_PAUSE;
		_currentFrame = 0;
		MFBOIBHHOHE = 0f;
		_active = false;
		EBJIKKBLBEM(FightEvent.RenderEvent);
		Parse(node);
	}

	public override void InitRule(object data)
	{
		_currentFrame = 0;
		MFBOIBHHOHE = 0f;
	}

	public float CFNAMMODOAA()
	{
		return MFBOIBHHOHE;
	}

	public LocationSelectorDarknessData DFBGCBOKCBG()
	{
		return HNPFNOPIIMJ;
	}

	public override void SetActive(bool value)
	{
		base.SetActive(value);
		_active = value;
	}

	protected override bool CompareSingle(object data)
	{
		_currentFrame++;
		if (_currentFrame <= HNPFNOPIIMJ.KCANPMPILKI)
		{
			if (!_active)
			{
				_currentFrame = 0;
			}
			MFBOIBHHOHE = 0f;
		}
		else if (_currentFrame <= HNPFNOPIIMJ.GFDMINCFBID)
		{
			MFBOIBHHOHE = (float)(_currentFrame - HNPFNOPIIMJ.KCANPMPILKI) * GJEHCPKKOLA;
		}
		else if (_currentFrame <= HNPFNOPIIMJ.NDBJNFHDGOA)
		{
			MFBOIBHHOHE = 255f;
		}
		else if (_currentFrame <= HNPFNOPIIMJ.NJBHKDBOEAI)
		{
			MFBOIBHHOHE = 255f - (float)(_currentFrame - HNPFNOPIIMJ.NDBJNFHDGOA) * GJENDIFGOAF;
		}
		else
		{
			_currentFrame = 0;
			MFBOIBHHOHE = 0f;
		}
		return true;
	}

	protected override void Parse(XmlNode node)
	{
		base.Parse(node);
		HNPFNOPIIMJ.KCANPMPILKI = node.Attributes["LightLasting"].ParseInt();
		HNPFNOPIIMJ.GFDMINCFBID = HNPFNOPIIMJ.KCANPMPILKI + node.Attributes["DarkOn"].ParseInt();
		HNPFNOPIIMJ.NDBJNFHDGOA = HNPFNOPIIMJ.GFDMINCFBID + node.Attributes["DarkLasting"].ParseInt();
		HNPFNOPIIMJ.NJBHKDBOEAI = HNPFNOPIIMJ.NDBJNFHDGOA + node.Attributes["LightOn"].ParseInt();
		GJEHCPKKOLA = 255f / (float)(HNPFNOPIIMJ.GFDMINCFBID - HNPFNOPIIMJ.KCANPMPILKI);
		GJENDIFGOAF = 255f / (float)(HNPFNOPIIMJ.NJBHKDBOEAI - HNPFNOPIIMJ.NDBJNFHDGOA);
	}

	public override void Stop()
	{
		SetActive(false);
	}

	public override void Reset()
	{
		SetActive(true);
	}

	public override InFightRule Copy()
	{
		InFightRule aAJIFBJLJOA = null;
		RuleAppliance eJPOJJKKICO = EDAKADCHOLE();
		XmlNode hKPPBKPJOEO = GIFDJEEGCJI().IOJIGDNFCFL();
		aAJIFBJLJOA = new DarknessRule(hKPPBKPJOEO, eJPOJJKKICO);
		aAJIFBJLJOA.IsRandom = IsRandom;
		return aAJIFBJLJOA;
	}
}
