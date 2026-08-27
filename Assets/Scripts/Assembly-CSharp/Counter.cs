using System.Xml;

public class Counter
{
	public enum IPENPHOAEGL
	{
		SPAN_NONE = 0,
		SPAN_ROUND = 1,
		SPAN_FIGHT = 2
	}

	public enum NENOEMHAEFH
	{
		ECLIPSE_MODE = 0,
		NORMAL_MODE = 1,
		RAID_MODE = 2,
		NONE_MODE = 3
	}

	public string Name;

	public string Type;

	public string DEGIADEEFGG;

	public string FGICHADOEHF;

	public string JIIFFJAJNNN;

	public string IOJFIFODOKO;

	public string FHAGEKGLJOI;

	public string GAHBCLAMANC;

	public string MJOJIPKLJOL;

	public IPENPHOAEGL KKNOICPMJPO;

	public NENOEMHAEFH NHDPMIGHKPF;

	public float Value;

	public int CompleteValue;

	public bool IsFightEnd;

	public ConditionOperator JIFAHHGNPFH = new ConditionOperator();

	public ConditionOfCompletionInspector DCIAEOCNHNO = new ConditionOfCompletionInspector();

	public Counter(XmlNode node)
	{
		Name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		Value = node.Attributes["Value"].ParseFloat();
		FGICHADOEHF = node.Attributes["Animation"].CIPOICEEIBK(string.Empty);
		JIIFFJAJNNN = node.Attributes["Weapon"].CIPOICEEIBK(string.Empty);
		IOJFIFODOKO = node.Attributes["Fight"].CIPOICEEIBK(string.Empty);
		FHAGEKGLJOI = node.Attributes["Fight2"].CIPOICEEIBK(string.Empty);
		Type = node.Attributes["Type"].CIPOICEEIBK(string.Empty);
		DEGIADEEFGG = node.Attributes["FightType"].CIPOICEEIBK(string.Empty);
		GAHBCLAMANC = node.Attributes["MaxDifficulty"].CIPOICEEIBK(string.Empty);
		MJOJIPKLJOL = node.Attributes["MinDifficulty"].CIPOICEEIBK(string.Empty);
		IsFightEnd = node.Attributes["OnFightEnd"].ParseBool();
		CompleteValue = 0;
		SetSpan(node.Attributes["CounterSpan"].CIPOICEEIBK(string.Empty));
		CCKKOCDBPBE(node);
		JIFAHHGNPFH.Type = ConditionOperator.EENJGHHIHIH.TYPE_AND;
		CounterConditionsParser.FDABJKODMAI(node, JIFAHHGNPFH, DCIAEOCNHNO);
	}

	public void SetSpan(string name)
	{
		if (name == "Round")
		{
			KKNOICPMJPO = IPENPHOAEGL.SPAN_ROUND;
		}
		else if (name == "Fight")
		{
			KKNOICPMJPO = IPENPHOAEGL.SPAN_FIGHT;
		}
		else
		{
			KKNOICPMJPO = IPENPHOAEGL.SPAN_NONE;
		}
	}

	public void CCKKOCDBPBE(XmlNode node)
	{
		if (node.Attributes["RaidMode"].ParseBool())
		{
			NHDPMIGHKPF = NENOEMHAEFH.RAID_MODE;
			return;
		}
		string text = node.Attributes["EclipseMode"].CIPOICEEIBK();
		if (text == "1")
		{
			NHDPMIGHKPF = NENOEMHAEFH.ECLIPSE_MODE;
		}
		else if (text == "0")
		{
			NHDPMIGHKPF = NENOEMHAEFH.NORMAL_MODE;
		}
		else
		{
			NHDPMIGHKPF = NENOEMHAEFH.NONE_MODE;
		}
	}

	public void EPCNPJEALBH(XmlNode node)
	{
		JIFAHHGNPFH.Type = ConditionOperator.EENJGHHIHIH.TYPE_AND;
		JIFAHHGNPFH.DIJNEIJHDIN(node);
	}

	public bool CHDEIEMINPF(CounterConditions conditions)
	{
		return JIFAHHGNPFH.IsEqual(conditions);
	}

	public bool CAIPCEHIBOO(FightIDS DIAIIPCBMFL)
	{
		return DCIAEOCNHNO.OPKPFKJPHNN(DIAIIPCBMFL);
	}

	public void AEPHNNABOEK()
	{
		JIFAHHGNPFH.AEPHNNABOEK();
	}

	public void CPMPOPHBFKJ()
	{
		CompleteValue = 0;
	}
}
