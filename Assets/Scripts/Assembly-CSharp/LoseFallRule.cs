using System.Xml;

public class LoseFallRule : AnimationListRule
{
	public const float PGJLFIONACP = -100000f;

	public const float PLIKDGOFDAB = 100000f;

	private float GNBJFBPFAMM;

	private float MOLFLBBKAOE;

	private string _nodeName;

	private ModelNode _node;

	private float NCOIMBKECMD;

	private float IBLJKECKGKP;

	private float KEFGNKPIHKH;

	private float AHHGHKHMLDN;

	private bool _isCheckRender;

	public LoseFallRule(XmlNode node, RuleAppliance EJPOJJKKICO)
		: base(BCBLLMPAMLP.RuleLoseFall, EJPOJJKKICO, node)
	{
		_isCheckRender = false;
		IBLJKECKGKP = -100000f;
		NCOIMBKECMD = 100000f;
		KEFGNKPIHKH = -100000f;
		AHHGHKHMLDN = 100000f;
		GNBJFBPFAMM = 0f;
		MOLFLBBKAOE = 0f;
		_node = null;
		Parse(node);
		EBJIKKBLBEM(FightEvent.AnimationStartEvent);
		EBJIKKBLBEM(FightEvent.RenderEvent);
		if (CheckAnimation("Physical"))
		{
			EBJIKKBLBEM(FightEvent.PhysicsStartEvent);
		}
	}

	public float EJHLFJBJHAN()
	{
		return NCOIMBKECMD;
	}

	public float JFBOKNFDFDO()
	{
		return IBLJKECKGKP;
	}

	public bool GFIIDPIFMFJ()
	{
		if (_node == null || !_isCheckRender)
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

	public override void InitRule(object data)
	{
		Reset();
		RuleInitData oIFPCFEGFOB = (RuleInitData)data;
		if (oIFPCFEGFOB.LPJNEDFCBOI != null)
		{
			GNBJFBPFAMM = (0f - oIFPCFEGFOB.LPJNEDFCBOI.JMLAKAKDBBL) / 2f;
			MOLFLBBKAOE = 0f - oIFPCFEGFOB.LPJNEDFCBOI.JMBOGPILDNM;
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
			LLLOJBFMONN.Error("LoseFallRule::initRule error - no ModelNode found with name %s", _nodeName);
		}
	}

	public override void Reset()
	{
		_isCheckRender = false;
	}

	public override void Clear()
	{
		_isCheckRender = false;
	}

	protected override bool CompareSingle(object data)
	{
		FightData hCPJJKMNMCE = (FightData)data;
		switch (hCPJJKMNMCE.KOJNCHKPLLN)
		{
		case FightEvent.PhysicsStartEvent:
			_isCheckRender = true;
			return GFIIDPIFMFJ();
		case FightEvent.AnimationStartEvent:
			_isCheckRender = CheckAnimation(hCPJJKMNMCE.LKLHCEEMINM);
			return GFIIDPIFMFJ();
		case FightEvent.RenderEvent:
			return GFIIDPIFMFJ();
		default:
			return false;
		}
	}

	protected override void Parse(XmlNode node)
	{
		base.Parse(node);
		_nodeName = node.Attributes["Node"].CIPOICEEIBK(string.Empty);
		string text = node.Attributes["Axis"].CIPOICEEIBK(string.Empty);
		if (text == "X")
		{
			IBLJKECKGKP = node.Attributes["Max"].ParseFloat(-100000f);
			NCOIMBKECMD = node.Attributes["Min"].ParseFloat(100000f);
		}
		if (text == "Y")
		{
			KEFGNKPIHKH = node.Attributes["Max"].ParseFloat(-100000f);
			AHHGHKHMLDN = node.Attributes["Min"].ParseFloat(100000f);
		}
	}
}
