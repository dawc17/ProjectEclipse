using System.Xml;

public class RingOutRule : InFightRule
{
	public const float PGJLFIONACP = 100000f;

	public const float PLIKDGOFDAB = -100000f;

	public const float MNCFCHNOCGI = 3f;

	public const string DEFAULT_SEQUENTION_NAME = "ringout";

	private float GNBJFBPFAMM;

	private float MOLFLBBKAOE;

	private string _nodeName;

	private string IEGGNJMGIMN;

	private ModelNode _node;

	private float NCOIMBKECMD;

	private float IBLJKECKGKP;

	private float KEFGNKPIHKH;

	private float AHHGHKHMLDN;

	private float AAHGGDAHNLP;

	public RingOutRule(XmlNode node, RuleAppliance EJPOJJKKICO)
		: base(BCBLLMPAMLP.RuleRingout, EJPOJJKKICO, node)
	{
		_nodeName = string.Empty;
		KEFGNKPIHKH = 100000f;
		AHHGHKHMLDN = -100000f;
		NCOIMBKECMD = -100000f;
		IBLJKECKGKP = 100000f;
		_node = null;
		GNBJFBPFAMM = 0f;
		AAHGGDAHNLP = 3f;
		EBJIKKBLBEM(FightEvent.RenderEvent);
		Parse(node);
	}

	public float EJHLFJBJHAN()
	{
		return NCOIMBKECMD;
	}

	public float JFBOKNFDFDO()
	{
		return IBLJKECKGKP;
	}

	public float IOCBNKAFHKL()
	{
		return AAHGGDAHNLP;
	}

	public override void InitRule(object data)
	{
		RuleInitData oIFPCFEGFOB = (RuleInitData)data;
		if (oIFPCFEGFOB.LPJNEDFCBOI != null)
		{
			GNBJFBPFAMM = (0f - oIFPCFEGFOB.LPJNEDFCBOI.JMLAKAKDBBL) / 2f;
			MOLFLBBKAOE = 0f - oIFPCFEGFOB.LPJNEDFCBOI.GBNPHCHGKDO;
		}
		switch (NDBMMPENJNJ)
		{
		case RuleAppliance.AppliancePlayer:
			if (oIFPCFEGFOB.DLPKDAIDCBF != null)
			{
				_node = oIFPCFEGFOB.DLPKDAIDCBF.CLDMEJKGLBA().EGHIDHMENEF(_nodeName);
			}
			break;
		case RuleAppliance.ApplianceOpponent:
			if (oIFPCFEGFOB.OGBHDKKOIGH != null)
			{
				_node = oIFPCFEGFOB.OGBHDKKOIGH.CLDMEJKGLBA().EGHIDHMENEF(_nodeName);
			}
			break;
		}
		if (_node == null)
		{
			LLLOJBFMONN.Error("RingoutRule::initRule error - no ModelNode found with name %s", _nodeName);
		}
	}

	public string OCJHHNFNHMK()
	{
		return IEGGNJMGIMN;
	}

	protected override bool CompareSingle(object data)
	{
		if (_node == null)
		{
			return false;
		}
		Vector3f eMAFACPEPDK = _node.ICLEOFDKDIF();
		eMAFACPEPDK = new Vector3f(eMAFACPEPDK.GILCBJJPKBK() + GNBJFBPFAMM, 0f - eMAFACPEPDK.OBIMBNIBEFG() + MOLFLBBKAOE, eMAFACPEPDK.KMFEKANLCFO());
		bool flag = eMAFACPEPDK.GILCBJJPKBK() > IBLJKECKGKP || eMAFACPEPDK.GILCBJJPKBK() < NCOIMBKECMD || eMAFACPEPDK.OBIMBNIBEFG() > KEFGNKPIHKH || eMAFACPEPDK.OBIMBNIBEFG() < AHHGHKHMLDN;
		if (flag)
		{
			SetActive(false);
		}
		return flag;
	}

	protected override void Parse(XmlNode node)
	{
		base.Parse(node);
		_nodeName = node.Attributes["Node"].CIPOICEEIBK(string.Empty);
		string text = node.Attributes["Axis"].CIPOICEEIBK(string.Empty);
		if (text == "X")
		{
			IBLJKECKGKP = node.Attributes["Max"].ParseFloat(100000f);
			NCOIMBKECMD = node.Attributes["Min"].ParseFloat(-100000f);
		}
		if (text == "Y")
		{
			KEFGNKPIHKH = node.Attributes["Max"].ParseFloat(100000f);
			AHHGHKHMLDN = node.Attributes["Min"].ParseFloat(-100000f);
		}
		AAHGGDAHNLP = node.Attributes["SequentionSpeed"].ParseFloat(3f);
		IEGGNJMGIMN = node.Attributes["Sequence"].CIPOICEEIBK("ringout");
	}

	public override InFightRule Copy()
	{
		InFightRule aAJIFBJLJOA = null;
		RuleAppliance eJPOJJKKICO = EDAKADCHOLE();
		XmlNode hKPPBKPJOEO = GIFDJEEGCJI().IOJIGDNFCFL();
		aAJIFBJLJOA = new RingOutRule(hKPPBKPJOEO, eJPOJJKKICO);
		aAJIFBJLJOA.IsRandom = IsRandom;
		return aAJIFBJLJOA;
	}
}
