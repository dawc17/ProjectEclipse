using System.Diagnostics;
using System.Xml;

public class PerkActionChangeImpulse : PerkAction
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private float BPKFKDMGJPO;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private float GCJFHFFIBEB;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private float PAEPBIJKLOJ;

	public float JMDNHEBNGKD
	{
		get
		{
			return NBECOMENIEH();
		}
		protected set
		{
			KMIBMHIIFDD(value);
		}
	}

	public float IDMCNBHKHFB
	{
		get
		{
			return LEAGBJCDLLA();
		}
		protected set
		{
			IIMEGJODOLL(value);
		}
	}

	public float MCEMNPAGNHH
	{
		get
		{
			return HODMHJNNFFG();
		}
		protected set
		{
			DIMJPGEPCMF(value);
		}
	}

	public PerkActionChangeImpulse()
	{
		KMIBMHIIFDD(1f);
		IIMEGJODOLL(1f);
		DIMJPGEPCMF(1f);
	}

	public PerkActionChangeImpulse(PerkActionChangeImpulse NOLFMPDGCOC)
		: base(NOLFMPDGCOC)
	{
		KMIBMHIIFDD(NOLFMPDGCOC.NBECOMENIEH());
		IIMEGJODOLL(NOLFMPDGCOC.LEAGBJCDLLA());
		DIMJPGEPCMF(NOLFMPDGCOC.HODMHJNNFFG());
	}

	public float NBECOMENIEH()
	{
		return BPKFKDMGJPO;
	}

	protected void KMIBMHIIFDD(float value)
	{
		BPKFKDMGJPO = value;
	}

	public float LEAGBJCDLLA()
	{
		return GCJFHFFIBEB;
	}

	protected void IIMEGJODOLL(float value)
	{
		GCJFHFFIBEB = value;
	}

	public float HODMHJNNFFG()
	{
		return PAEPBIJKLOJ;
	}

	protected void DIMJPGEPCMF(float value)
	{
		PAEPBIJKLOJ = value;
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		set_Type(ActionType.ACTION_CHANGE_IMPULSE);
		KMIBMHIIFDD(node.Attributes["MultiplierX"].ParseFloat());
		IIMEGJODOLL(node.Attributes["MultiplierY"].ParseFloat());
		DIMJPGEPCMF(node.Attributes["MultiplierZ"].ParseFloat());
	}
}
