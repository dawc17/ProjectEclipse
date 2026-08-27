using System.Xml;

public class UserTutorials
{
	private XmlAttribute KJDFLFIBLGP;

	private string _storyTutorialStep = string.Empty;

	private XmlAttribute JHNKGJPCNIN;

	private RaidTutorialStepCode CEOBGDPEDOO = RaidTutorialStepCode.RaidTutNotStarted;

	private XmlAttribute FEDMOPFADNJ;

	private bool JFNNHJHCOKL;

	private XmlAttribute EBNMHICBFJJ;

	private bool LABBDBLNEDJ;

	public string HDIDABICNNI
	{
		get
		{
			return JILGHNPIHME();
		}
		set
		{
			set_StoryTutorialStep(value);
		}
	}

	public bool OMDLOOFIJDF
	{
		get
		{
			return JBPHIAEPHAH();
		}
	}

	public RaidTutorialStepCode GLOLFNEKHBH
	{
		get
		{
			return NAGDMOLMLGH();
		}
		set
		{
			PECCKNJMNJP(value);
		}
	}

	public bool KGBJGLLNPPF
	{
		get
		{
			return IJOGJBEIIJD();
		}
		set
		{
			KLJDICMKAAF(value);
		}
	}

	public bool MCILJIFPJHJ
	{
		get
		{
			return JAOBNPABAIF();
		}
		set
		{
			NNDIAFDINFC(value);
		}
	}

	public string JILGHNPIHME()
	{
		return _storyTutorialStep;
	}

	public void set_StoryTutorialStep(string value)
	{
		if (!(_storyTutorialStep == value))
		{
			_storyTutorialStep = value;
			KJDFLFIBLGP.Value = _storyTutorialStep;
			ListSF.CCDKHLAMKKO().GGGEHAGCLGC();
		}
	}

	public bool JBPHIAEPHAH()
	{
		return _storyTutorialStep != "END";
	}

	public RaidTutorialStepCode NAGDMOLMLGH()
	{
		return CEOBGDPEDOO;
	}

	public void PECCKNJMNJP(RaidTutorialStepCode value)
	{
		if (CEOBGDPEDOO <= value)
		{
			CEOBGDPEDOO = value;
			JHNKGJPCNIN.Value = GameUtils.HEJIFIHLLJF[CEOBGDPEDOO];
			ListSF.CCDKHLAMKKO().GGGEHAGCLGC();
		}
	}

	public bool IJOGJBEIIJD()
	{
		return JFNNHJHCOKL;
	}

	public void KLJDICMKAAF(bool value)
	{
		if (JFNNHJHCOKL != value)
		{
			JFNNHJHCOKL = value;
			FEDMOPFADNJ.Value = ((!JFNNHJHCOKL) ? "0" : "1");
			ListSF.CCDKHLAMKKO().GGGEHAGCLGC();
		}
	}

	public bool JAOBNPABAIF()
	{
		return LABBDBLNEDJ;
	}

	public void NNDIAFDINFC(bool value)
	{
		LABBDBLNEDJ = value;
		EBNMHICBFJJ.Value = ((!LABBDBLNEDJ) ? "0" : "1");
		ListSF.CCDKHLAMKKO().GGGEHAGCLGC();
	}

	public void Parse(XmlNode node)
	{
		KJDFLFIBLGP = node.Attributes["Tutorial"];
		if (KJDFLFIBLGP == null)
		{
			KJDFLFIBLGP = node.LLIKNHNLGJJ("Tutorial");
			KJDFLFIBLGP.Value = GameUtils.AKPBNLKFONO.StepsNames[0];
		}
		string text = KJDFLFIBLGP.CIPOICEEIBK();
		_storyTutorialStep = ((!GameUtils.AKPBNLKFONO.IsStepName(text)) ? GameUtils.AKPBNLKFONO.StepsNames[0] : text);
		JHNKGJPCNIN = node.Attributes["RaidTutorialStep"];
		if (JHNKGJPCNIN == null)
		{
			JHNKGJPCNIN = node.LLIKNHNLGJJ("RaidTutorialStep");
		}
		CEOBGDPEDOO = GameUtils.PHHOCKCCGMM(JHNKGJPCNIN.CIPOICEEIBK("NotStarted"));
		FEDMOPFADNJ = node.Attributes["RaidTutorialGemsTaken"];
		if (FEDMOPFADNJ == null)
		{
			FEDMOPFADNJ = node.LLIKNHNLGJJ("RaidTutorialGemsTaken");
		}
		JFNNHJHCOKL = FEDMOPFADNJ.ParseBool();
		EBNMHICBFJJ = node.Attributes["ForgeTutorialMaterialsGiven"];
		if (EBNMHICBFJJ == null)
		{
			EBNMHICBFJJ = node.LLIKNHNLGJJ("ForgeTutorialMaterialsGiven");
		}
		LABBDBLNEDJ = EBNMHICBFJJ.ParseBool();
	}
}
