using System.Collections.Generic;
using System.Xml;

public class HotGroundRule : AnimationListRule
{
	public class LimitedNode
	{
		public ModelNode node;

		public string name;

		public float MMIKMPNLJGN;

		public float GGINDPCJGEI;

		public float HPIKHPEIJHM;

		public float JELMLMPIGHN;

		public LimitedNode(XmlNode EABJIAHGLEO)
		{
			MMIKMPNLJGN = float.MaxValue;
			HPIKHPEIJHM = float.MinValue;
			GGINDPCJGEI = float.MaxValue;
			JELMLMPIGHN = float.MinValue;
			name = string.Empty;
			node = null;
			name = EABJIAHGLEO.Attributes["Name"].CIPOICEEIBK(string.Empty);
			string text = EABJIAHGLEO.Attributes["Axis"].CIPOICEEIBK(string.Empty);
			if (text == "X")
			{
				MMIKMPNLJGN = EABJIAHGLEO.Attributes["Max"].ParseFloat(float.MaxValue);
				HPIKHPEIJHM = EABJIAHGLEO.Attributes["Min"].ParseFloat(float.MinValue);
			}
			if (text == "Y")
			{
				GGINDPCJGEI = EABJIAHGLEO.Attributes["Max"].ParseFloat(float.MaxValue);
				JELMLMPIGHN = EABJIAHGLEO.Attributes["Min"].ParseFloat(float.MinValue);
			}
		}
	}

	public const int FRAMES_IN_SECOND = 60;

	private List<LimitedNode> CFPIOKDFJCH = new List<LimitedNode>();

	private string _sequenceName;

	private bool DGNPODNAMDA;

	private float LNDJNKGFDII;

	public bool HADLDHHEOKM;

	private bool CIGIBFFPICM;

	private bool OIKCMPFPACL;

	private int HJOHKOEICAP;

	private int LAHIFBDMIJC;

	private float _frames;

	private float OHNHMIDDOPA;

	private float GNBJFBPFAMM;

	private float MOLFLBBKAOE;

	private int GPBICDOPGAC;

	public HotGroundRule(XmlNode node, RuleAppliance EJPOJJKKICO)
		: base(BCBLLMPAMLP.RuleHotGround, EJPOJJKKICO, node)
	{
		HADLDHHEOKM = true;
		GNBJFBPFAMM = 0f;
		MOLFLBBKAOE = 0f;
		CIGIBFFPICM = false;
		OIKCMPFPACL = false;
		GPBICDOPGAC = 1;
		LNDJNKGFDII = 0f;
		DGNPODNAMDA = false;
		EBJIKKBLBEM(FightEvent.AnimationStartEvent);
		EBJIKKBLBEM(FightEvent.RenderEvent);
		Parse(node);
		Reset();
	}

	public int NNOHILNKJEN()
	{
		return HJOHKOEICAP;
	}

	public int KFKJEMCAMNF()
	{
		return LAHIFBDMIJC;
	}

	public override void Reset()
	{
		HJOHKOEICAP = LAHIFBDMIJC;
		_frames = 0f;
		HADLDHHEOKM = true;
	}

	public override void InitRule(object data)
	{
		RuleInitData oIFPCFEGFOB = (RuleInitData)data;
		if (oIFPCFEGFOB.LPJNEDFCBOI != null)
		{
			GNBJFBPFAMM = (0f - oIFPCFEGFOB.LPJNEDFCBOI.JMLAKAKDBBL) / 2f;
			MOLFLBBKAOE = 0f - oIFPCFEGFOB.LPJNEDFCBOI.GBNPHCHGKDO;
		}
		Model fGCODGKLHED = null;
		switch (NDBMMPENJNJ)
		{
		case RuleAppliance.AppliancePlayer:
			fGCODGKLHED = oIFPCFEGFOB.DLPKDAIDCBF;
			break;
		case RuleAppliance.ApplianceOpponent:
			fGCODGKLHED = oIFPCFEGFOB.OGBHDKKOIGH;
			break;
		}
		if (fGCODGKLHED != null)
		{
			foreach (LimitedNode item in CFPIOKDFJCH)
			{
				item.node = fGCODGKLHED.CLDMEJKGLBA().EGHIDHMENEF(item.name);
				if (item.node == null)
				{
					LLLOJBFMONN.Error("RingoutRule::initRule error - no ModelNode found with name " + item.name);
				}
			}
		}
		Reset();
	}

	public bool BFOJOGLCIBB()
	{
		return DGNPODNAMDA;
	}

	public string OCJHHNFNHMK()
	{
		return _sequenceName;
	}

	public float APDIONCLEDH()
	{
		return LNDJNKGFDII;
	}

	protected override bool CompareSingle(object data)
	{
		FightData hCPJJKMNMCE = (FightData)data;
		switch (hCPJJKMNMCE.KOJNCHKPLLN)
		{
		case FightEvent.RenderEvent:
			if (CIGIBFFPICM && KEPMMNOBIIL())
			{
				if (!OIKCMPFPACL)
				{
					HJOHKOEICAP = LAHIFBDMIJC;
					_frames = 0f;
					HADLDHHEOKM = true;
					OIKCMPFPACL = true;
				}
				break;
			}
			_frames += 1f / (float)GPBICDOPGAC;
			if (_frames >= 60f)
			{
				_frames = 0f;
				if (HJOHKOEICAP > 0)
				{
					HJOHKOEICAP--;
					HADLDHHEOKM = true;
				}
			}
			break;
		case FightEvent.AnimationStartEvent:
			OIKCMPFPACL = false;
			CIGIBFFPICM = CheckAnimation(hCPJJKMNMCE.LKLHCEEMINM);
			break;
		}
		return HJOHKOEICAP <= 0;
	}

	protected override void AGCBHKBNMKL(object data)
	{
		PlayersFightData jNGGHELCPFM = (PlayersFightData)data;
		GPBICDOPGAC = jNGGHELCPFM.SlowMode;
	}

	protected override void Parse(XmlNode node)
	{
		base.Parse(node);
		PICNEPHDGGG(node);
		OHNHMIDDOPA = node.Attributes["Frames"].ParseFloat();
		LAHIFBDMIJC = (int)(OHNHMIDDOPA / 60f);
		DGNPODNAMDA = !node.Attributes["Sequence"].Empty();
		if (DGNPODNAMDA)
		{
			_sequenceName = node.Attributes["Sequence"].CIPOICEEIBK(string.Empty);
			LNDJNKGFDII = node.Attributes["SequenceWidth"].ParseFloat();
		}
	}

	protected void PICNEPHDGGG(XmlNode node)
	{
		foreach (XmlNode item in node.SelectNodes("Node"))
		{
			CFPIOKDFJCH.Add(new LimitedNode(item));
		}
	}

	protected bool KEPMMNOBIIL()
	{
		foreach (LimitedNode item in CFPIOKDFJCH)
		{
			Vector3f eMAFACPEPDK = item.node.ICLEOFDKDIF();
			eMAFACPEPDK = new Vector3f(eMAFACPEPDK.GILCBJJPKBK() + GNBJFBPFAMM, 0f - eMAFACPEPDK.OBIMBNIBEFG(), eMAFACPEPDK.KMFEKANLCFO());
			if (!(eMAFACPEPDK.GILCBJJPKBK() >= item.MMIKMPNLJGN) && !(eMAFACPEPDK.GILCBJJPKBK() <= item.HPIKHPEIJHM) && !(eMAFACPEPDK.OBIMBNIBEFG() >= item.GGINDPCJGEI) && !(eMAFACPEPDK.OBIMBNIBEFG() <= item.JELMLMPIGHN))
			{
				return false;
			}
		}
		return true;
	}
}
