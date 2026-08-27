using System.Xml;

public class ActionEffect : ActionAnimation
{
	private string _Name;

	private string KHHJNHKEHPM;

	private float BPGEHHFLEOM;

	private float _ScaleX;

	private float _ScaleY;

	private float _StartRotation;

	private int _Priority;

	private float ECDOPLHOFOC;

	private bool INFAGPDFGNL;

	private bool EDNLDFKKLGL;

	private DistancePoint LEPIOCGGDIC = new DistancePoint();

	private bool EMPMLDKGEEG;

	private int _StopFollowFrame;

	private DistanceVector GOPEDNFNPJF = new DistanceVector();

	public string FileName
	{
		get
		{
			return EPDMGFELIMC();
		}
	}

	public float FOAHMAOBFEA
	{
		get
		{
			return JKEBPLCOOID();
		}
	}

	public float PJLNOLDINHA
	{
		get
		{
			return EHJCPFIELAN();
		}
	}

	public bool LBGCFNKKJJL
	{
		get
		{
			return NCEKKNIMHAG();
		}
	}

	public bool FGBGEHKFACJ
	{
		get
		{
			return JNAALMFCPCN();
		}
	}

	public DistancePoint JJCKADKCDIF
	{
		get
		{
			return ECJPLFFAMJO();
		}
	}

	public bool FFPAFCIMFNB
	{
		get
		{
			return DIGCODDLDAD();
		}
		set
		{
			set_IsFollowObject(value);
		}
	}

	public DistanceVector PILEOJJHOOB
	{
		get
		{
			return MABFDDNEOGO();
		}
	}

	public ActionEffect(XmlNode node)
		: base(FADAJCEEKIO.EFFECT)
	{
		Parse(node);
	}

	public string get_Name()
	{
		return _Name;
	}

	public string EPDMGFELIMC()
	{
		return KHHJNHKEHPM;
	}

	public float JKEBPLCOOID()
	{
		return BPGEHHFLEOM;
	}

	public float GetScaleX()
	{
		return _ScaleX;
	}

	public float GetScaleY()
	{
		return _ScaleY;
	}

	public float GetStartRotation()
	{
		return _StartRotation;
	}

	public int GetPriority()
	{
		return _Priority;
	}

	public float EHJCPFIELAN()
	{
		return ECDOPLHOFOC;
	}

	public bool NCEKKNIMHAG()
	{
		return INFAGPDFGNL;
	}

	public bool JNAALMFCPCN()
	{
		return EDNLDFKKLGL;
	}

	public DistancePoint ECJPLFFAMJO()
	{
		return LEPIOCGGDIC;
	}

	public bool DIGCODDLDAD()
	{
		return EMPMLDKGEEG;
	}

	public void set_IsFollowObject(bool value)
	{
		EMPMLDKGEEG = value;
	}

	public DistanceVector MABFDDNEOGO()
	{
		return GOPEDNFNPJF;
	}

	public override void Visit(Model ACENLMONNPA)
	{
		ACENLMONNPA.OPPIKLBKMPN(this);
	}

	public void KJHPCLOFDJB(ModelObject OECPEDPMKCD, bool EKBOGDKIHIH, ModelNode AECCPADGGPG, bool PHADJMAONJG, ModelObject MJCGOJBGFIE = null)
	{
		LEPIOCGGDIC.UpdateNode(OECPEDPMKCD, EKBOGDKIHIH, AECCPADGGPG, PHADJMAONJG, MJCGOJBGFIE);
		GOPEDNFNPJF.KJHPCLOFDJB(OECPEDPMKCD, EKBOGDKIHIH, AECCPADGGPG, PHADJMAONJG, MJCGOJBGFIE);
	}

	public void MGCNPBCBMHB()
	{
		LEPIOCGGDIC.GPGKANDFLNB();
		GOPEDNFNPJF.JKDMJGNOKCA();
	}

	protected override void Parse(XmlNode node)
	{
		base.Parse(node);
		_Name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		KHHJNHKEHPM = node.Attributes["Sequence"].CIPOICEEIBK(string.Empty);
		BPGEHHFLEOM = node.Attributes["Scale"].ParseFloat(1f);
		_ScaleX = node.Attributes["ScaleX"].ParseFloat(BPGEHHFLEOM);
		_ScaleY = node.Attributes["ScaleY"].ParseFloat(BPGEHHFLEOM);
		_StartRotation = node.Attributes["StartRotation"].ParseFloat();
		ECDOPLHOFOC = node.Attributes["TimeScale"].ParseFloat(1f);
		INFAGPDFGNL = node.Attributes["Looped"].ParseBool();
		EDNLDFKKLGL = node.Attributes["OnBackground"].ParseBool();
		_Priority = node.Attributes["Priority"].ParseInt(EDNLDFKKLGL ? -10 : 0);
		XmlNode xmlNode = node["Position"];
		if (xmlNode != null)
		{
			LEPIOCGGDIC.Create(xmlNode);
			EMPMLDKGEEG = xmlNode.Attributes["Follow"].ParseBool();
			_StopFollowFrame = xmlNode.Attributes["StopFollowframe"].ParseInt(-1);
		}
		else
		{
			// Newer screen-space and model-owned effects intentionally omit Position.
			// DistancePoint's default OBJECT_NULL is the legacy representation of
			// that behavior, so this is valid data rather than a parser error.
		}
		XmlNode xmlNode2 = node["Vector"];
		if (xmlNode2 != null)
		{
			GOPEDNFNPJF.Parse(xmlNode2);
		}
	}
}
