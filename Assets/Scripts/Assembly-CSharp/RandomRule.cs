using System.Collections.Generic;
using System.Xml;

public class RandomRule : Rule
{
	public enum EOAOMBKFMPF
	{
		REFRESH_NONE = 0,
		REFRESH_EACH_FIGHT = 1,
		REFRESH_EACH_ROUND = 2
	}

	public const string KAHFLCIMJBO = "EachFight";

	public const string KAPJDNNLKHE = "EachRound";

	private Rule ALJIKCOCHCL;

	private List<Rule> _rules = new List<Rule>();

	private List<Rule> FLPJJMJHBBK = new List<Rule>();

	private List<Rule> JNMJJPEBECE = new List<Rule>();

	private bool _noDoubles;

	private EOAOMBKFMPF BJIBDCEMHCH;

	public RandomRule(XmlNode node)
		: base(BCBLLMPAMLP.RuleRandom, node)
	{
		ALJIKCOCHCL = null;
		_noDoubles = false;
		BJIBDCEMHCH = EOAOMBKFMPF.REFRESH_NONE;
		Parse(node);
		JNMJJPEBECE = new List<Rule>();
	}

	public void OIOJKNKDFJM()
	{
		List<Rule> list = new List<Rule>();
		int num = ListSF.CCDKHLAMKKO().PINDEKDNCNL();
		foreach (Rule item in FLPJJMJHBBK)
		{
			if (item.CHDEIEMINPF())
			{
				list.Add(item);
			}
		}
		int count = list.Count;
		if (count > 0)
		{
			int index = NekkiMath.randomInt(count);
			ALJIKCOCHCL = list[index];
			if (_noDoubles)
			{
				FLPJJMJHBBK.RemoveAt(index);
				JNMJJPEBECE.Add(ALJIKCOCHCL);
			}
		}
		else if (JNMJJPEBECE.Count > 0)
		{
			GDDNBFMCEJO();
			OIOJKNKDFJM();
		}
		else
		{
			LLLOJBFMONN.Error("RandomRule::resetRandom ERROR - RandomRule is empty");
		}
	}

	public void GDDNBFMCEJO()
	{
		FLPJJMJHBBK.Clear();
		FLPJJMJHBBK.AddRange(_rules);
		JNMJJPEBECE.Clear();
		ALJIKCOCHCL = null;
	}

	public Rule GHLEKCGJAEP()
	{
		return ALJIKCOCHCL;
	}

	public override void SetActive(bool value)
	{
		base.SetActive(value);
		foreach (Rule item in FLPJJMJHBBK)
		{
			item.SetActive(value);
		}
	}

	public bool CheckReset(int round)
	{
		switch (BJIBDCEMHCH)
		{
		case EOAOMBKFMPF.REFRESH_EACH_ROUND:
			return true;
		case EOAOMBKFMPF.REFRESH_EACH_FIGHT:
			return round == 1;
		default:
			LLLOJBFMONN.Error("RandomRule::checkReset ERROR - invalid Refresh value");
			return false;
		}
	}

	public EOAOMBKFMPF EPMBMBMNJIA()
	{
		return BJIBDCEMHCH;
	}

	public List<Rule> BONNMLEJBJH()
	{
		return _rules;
	}

	protected new void Parse(XmlNode node)
	{
		RuleParser.EEPPJEMHBCK(node, _rules);
		FLPJJMJHBBK.Clear();
		FLPJJMJHBBK.AddRange(_rules);
		foreach (Rule item in _rules)
		{
			item.IsRandom = true;
		}
		_noDoubles = node.Attributes["NoDoubles"].ParseBool();
		string text = node.Attributes["Refresh"].CIPOICEEIBK(string.Empty);
		if (text == "EachRound")
		{
			BJIBDCEMHCH = EOAOMBKFMPF.REFRESH_EACH_ROUND;
		}
		else if (text == "EachFight")
		{
			BJIBDCEMHCH = EOAOMBKFMPF.REFRESH_EACH_FIGHT;
		}
		else
		{
			BJIBDCEMHCH = EOAOMBKFMPF.REFRESH_EACH_FIGHT;
		}
	}
}
