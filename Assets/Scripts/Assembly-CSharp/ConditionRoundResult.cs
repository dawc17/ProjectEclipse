using System.Xml;

public class ConditionRoundResult : ConditionAnimation
{
	public enum JBDNJGGKFDI
	{
		RESULT_TYPE_NONE = 0,
		RESULT_TYPE_VICTORY = 1,
		RESULT_TYPE_DEFEAT = 2
	}

	public enum DBLIMJMOEPB
	{
		RESULT_SUBTYPE_NONE = 0,
		RESULT_SUBTYPE_TIMEOUT = 1,
		RESULT_SUBTYPE_RINGOUT = 2,
		RESULT_SUBTYPE_LOSE = 3
	}

	private JBDNJGGKFDI KCIIELDOBOM;

	private DBLIMJMOEPB GPOHKJPLLGH;

	public ConditionRoundResult(XmlNode node)
		: base(DGAGKLODADD.ROUND_RESULT)
	{
		string text = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		string text2 = node.Attributes["Type"].CIPOICEEIBK(string.Empty);
		if (text == "Victory")
		{
			KCIIELDOBOM = JBDNJGGKFDI.RESULT_TYPE_VICTORY;
		}
		else if (text == "Defeat")
		{
			KCIIELDOBOM = JBDNJGGKFDI.RESULT_TYPE_DEFEAT;
		}
		else
		{
			KCIIELDOBOM = JBDNJGGKFDI.RESULT_TYPE_NONE;
		}
		if (text2 == "Timeout")
		{
			GPOHKJPLLGH = DBLIMJMOEPB.RESULT_SUBTYPE_TIMEOUT;
		}
		else if (text2 == "Ringout")
		{
			GPOHKJPLLGH = DBLIMJMOEPB.RESULT_SUBTYPE_RINGOUT;
		}
		else
		{
			GPOHKJPLLGH = DBLIMJMOEPB.RESULT_SUBTYPE_NONE;
		}
	}

	public override bool IsEqual(ModelConditions conditions)
	{
		bool flag = false;
		if (conditions.BHHLEBHLBLH && (KCIIELDOBOM == JBDNJGGKFDI.RESULT_TYPE_NONE || IsWinner(conditions.IsWinner)) && (GPOHKJPLLGH == DBLIMJMOEPB.RESULT_SUBTYPE_NONE || KPKMAGJMIMJ(conditions.EndRoundType)))
		{
			flag = true;
		}
		return (!IsNot) ? flag : (!flag);
	}

	private bool IsWinner(bool PKHDLOGJKAD)
	{
		return (PKHDLOGJKAD && KCIIELDOBOM == JBDNJGGKFDI.RESULT_TYPE_VICTORY) || (!PKHDLOGJKAD && KCIIELDOBOM == JBDNJGGKFDI.RESULT_TYPE_DEFEAT);
	}

	private bool KPKMAGJMIMJ(EndRoundType LFLGCDNKNJI)
	{
		return (LFLGCDNKNJI == EndRoundType.EndRoundTypeTimeOut && GPOHKJPLLGH == DBLIMJMOEPB.RESULT_SUBTYPE_TIMEOUT) || (LFLGCDNKNJI == EndRoundType.EndRoundTypeRingOut && GPOHKJPLLGH == DBLIMJMOEPB.RESULT_SUBTYPE_TIMEOUT) || (LFLGCDNKNJI == EndRoundType.EndRoundTypeLose && GPOHKJPLLGH == DBLIMJMOEPB.RESULT_SUBTYPE_TIMEOUT) || (LFLGCDNKNJI == EndRoundType.EndRoundTypeZeroHealth && GPOHKJPLLGH == DBLIMJMOEPB.RESULT_SUBTYPE_TIMEOUT);
	}
}
