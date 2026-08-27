using System.Xml;

public class RandomAreaRule : InFightRule
{
	public const string FILE_PATH_PERK_ACTIVATION_AREA = "Textures/fight/rules/randomarea/";

	private NekkiRandom OOPIJCGCFAP = new NekkiRandom();

	private float GEFOLNHPJMI;

	private float MPBBJKDIKDD;

	private float DMKOKMCMCDE;

	private float GOGMIBDBJPG;

	private string NCKCDCODNHA;

	private string KENGIGPHMPK;

	public bool HADLDHHEOKM;

	private LocationSelectorDarknessData EHIDGLBHJCH = new LocationSelectorDarknessData();

	private bool _active;

	private bool PNJBGMPOCBM;

	private int _currentFrame;

	private float MFBOIBHHOHE;

	private float GJEHCPKKOLA;

	private float GJENDIFGOAF;

	public RandomAreaRule(XmlNode node)
		: base(BCBLLMPAMLP.RuleRandomArea, RuleAppliance.AppliancePlayer, node)
	{
		GEFOLNHPJMI = 0f;
		NCKCDCODNHA = string.Empty;
		KENGIGPHMPK = string.Empty;
		MPBBJKDIKDD = 0f;
		_currentFrame = 0;
		DMKOKMCMCDE = -50f;
		GOGMIBDBJPG = 50f;
		HADLDHHEOKM = false;
		_active = true;
		PNJBGMPOCBM = false;
		MFBOIBHHOHE = 0f;
		GJEHCPKKOLA = 0f;
		GJENDIFGOAF = 0f;
		EHIDGLBHJCH = new LocationSelectorDarknessData();
		EBJIKKBLBEM(FightEvent.RenderEvent);
		Parse(node);
	}

	public override void InitRule(object data)
	{
		RuleInitData oIFPCFEGFOB = (RuleInitData)data;
		OOPIJCGCFAP.setSeed((uint)ListSF.IDMJOMOMDOJ());
		DMKOKMCMCDE = (0f - oIFPCFEGFOB.LPJNEDFCBOI.JMLAKAKDBBL) / 2f + oIFPCFEGFOB.LPJNEDFCBOI.MFAPMDDJBBL + GEFOLNHPJMI / 2f;
		GOGMIBDBJPG = oIFPCFEGFOB.LPJNEDFCBOI.JMLAKAKDBBL / 2f - oIFPCFEGFOB.LPJNEDFCBOI.MFAPMDDJBBL - GEFOLNHPJMI / 2f;
		_currentFrame = 0;
		MFBOIBHHOHE = 0f;
		PNJBGMPOCBM = false;
	}

	public float BOCHPMJBLGA()
	{
		return MPBBJKDIKDD;
	}

	public float HFDJFADIAEP()
	{
		return GEFOLNHPJMI;
	}

	public string BPMABAFDFJK()
	{
		return NCKCDCODNHA;
	}

	public string AJIAFONPDKE()
	{
		return KENGIGPHMPK;
	}

	public bool JFFONEBNBMP()
	{
		return PNJBGMPOCBM;
	}

	public float CFNAMMODOAA()
	{
		return MFBOIBHHOHE;
	}

	public override void Stop()
	{
		_active = false;
	}

	public override void Reset()
	{
		_active = true;
	}

	public override void SetActive(bool value)
	{
		base.SetActive(value);
		_active = value;
	}

	protected override bool CompareSingle(object data)
	{
		_currentFrame++;
		if (_currentFrame <= EHIDGLBHJCH.KCANPMPILKI)
		{
			if (!_active)
			{
				_currentFrame = 0;
			}
			MFBOIBHHOHE = 0f;
			PNJBGMPOCBM = false;
		}
		else if (_currentFrame <= EHIDGLBHJCH.GFDMINCFBID)
		{
			MFBOIBHHOHE = (float)(_currentFrame - EHIDGLBHJCH.KCANPMPILKI) * GJEHCPKKOLA;
			PNJBGMPOCBM = true;
		}
		else if (_currentFrame <= EHIDGLBHJCH.NDBJNFHDGOA)
		{
			MFBOIBHHOHE = 255f;
			PNJBGMPOCBM = true;
		}
		else if (_currentFrame <= EHIDGLBHJCH.NJBHKDBOEAI)
		{
			MFBOIBHHOHE = 255f - (float)(_currentFrame - EHIDGLBHJCH.NDBJNFHDGOA) * GJENDIFGOAF;
			PNJBGMPOCBM = true;
		}
		else
		{
			_currentFrame = 0;
			MFBOIBHHOHE = 0f;
			PNJBGMPOCBM = false;
			FNJCCOBCGBI();
		}
		return true;
	}

	protected override void Parse(XmlNode node)
	{
		base.Parse(node);
		GEFOLNHPJMI = node.Attributes["Width"].ParseFloat();
		NCKCDCODNHA += "Textures/fight/rules/randomarea/";
		NCKCDCODNHA += node.Attributes["Image"].CIPOICEEIBK(string.Empty);
		if (!node.Attributes["Icon"].Empty())
		{
			KENGIGPHMPK += "Textures/fight/rules/randomarea/";
			KENGIGPHMPK += node.Attributes["Icon"].CIPOICEEIBK(string.Empty);
		}
		EHIDGLBHJCH.KCANPMPILKI = node.Attributes["FadeIn"].ParseInt();
		EHIDGLBHJCH.GFDMINCFBID = EHIDGLBHJCH.KCANPMPILKI + node.Attributes["FramesOn"].ParseInt();
		EHIDGLBHJCH.NDBJNFHDGOA = EHIDGLBHJCH.GFDMINCFBID + node.Attributes["FadeOut"].ParseInt();
		EHIDGLBHJCH.NJBHKDBOEAI = EHIDGLBHJCH.NDBJNFHDGOA + node.Attributes["FramesOff"].ParseInt();
		GJEHCPKKOLA = 255f / (float)(EHIDGLBHJCH.GFDMINCFBID - EHIDGLBHJCH.KCANPMPILKI);
		GJENDIFGOAF = 255f / (float)(EHIDGLBHJCH.NJBHKDBOEAI - EHIDGLBHJCH.NDBJNFHDGOA);
	}

	protected void FNJCCOBCGBI()
	{
		MPBBJKDIKDD = OOPIJCGCFAP.randomFloat(DMKOKMCMCDE, GOGMIBDBJPG);
	}

	public override InFightRule Copy()
	{
		InFightRule aAJIFBJLJOA = null;
		XmlNode hKPPBKPJOEO = GIFDJEEGCJI().IOJIGDNFCFL();
		aAJIFBJLJOA = new RandomAreaRule(hKPPBKPJOEO);
		aAJIFBJLJOA.IsRandom = IsRandom;
		return aAJIFBJLJOA;
	}
}
