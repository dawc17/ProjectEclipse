using System.Xml;

public class RatingEvaluationRule : Rule
{
	protected float LIIBMLCBOEL;

	protected float GGKOKNNPJFC;

	protected float ICGKDBJLMMD;

	protected float OFEHMDKHFPC;

	protected float BIEBOJEAKMO;

	protected float CKJHMIPEPAN;

	protected float INMBDMJFLLJ;

	public RatingEvaluationRule(XmlNode node)
		: base(BCBLLMPAMLP.RuleRatingEvaluation, node)
	{
		LIIBMLCBOEL = node.Attributes["PlayerRating"].ParseFloat();
		GGKOKNNPJFC = node.Attributes["EnemyRating"].ParseFloat();
		ICGKDBJLMMD = node.Attributes["PlayerRatingMagic"].ParseFloat();
		OFEHMDKHFPC = node.Attributes["EnemyRatingMagic"].ParseFloat();
		BIEBOJEAKMO = node.Attributes["PlayerRatingRanged"].ParseFloat();
		CKJHMIPEPAN = node.Attributes["EnemyRatingRanged"].ParseFloat();
		INMBDMJFLLJ = node.Attributes["RatingCorrection"].ParseFloat();
	}

	public float JLDBFIKOALE()
	{
		return LIIBMLCBOEL;
	}

	public float IJCNLEOEFAG()
	{
		return GGKOKNNPJFC;
	}

	public float KFGKCOKGBFB()
	{
		return ICGKDBJLMMD;
	}

	public float EJCAKIHPNDG()
	{
		return OFEHMDKHFPC;
	}

	public float EJJFJELMIMP()
	{
		return BIEBOJEAKMO;
	}

	public float OOCNGFFGFPD()
	{
		return CKJHMIPEPAN;
	}

	public float FIOPALJIOEC()
	{
		return INMBDMJFLLJ;
	}
}
