using System.Collections.Generic;
using System.Xml;

public static class CounterConditionsParser
{
	public static List<ConditionCounter> EPCNPJEALBH(XmlNode node)
	{
		List<ConditionCounter> list = new List<ConditionCounter>();
		EPCNPJEALBH(node, list);
		return list;
	}

	public static void EPCNPJEALBH(XmlNode EBLIGDMALEA, List<ConditionCounter> DCJLKCFKCOM)
	{
		foreach (XmlNode childNode in EBLIGDMALEA.ChildNodes)
		{
			DCJLKCFKCOM.Add(DKPIKJMJPPH(childNode));
		}
	}

	public static ConditionCounter DKPIKJMJPPH(XmlNode node)
	{
		string name = node.Name;
		if (name == "Battle")
		{
			return new ConditionBattle(node);
		}
		if (name == "Operator")
		{
			return new ConditionOperator(node);
		}
		LLLOJBFMONN.Error(string.Format("CounterConditionsParser::parseCondition - %s", name));
		return null;
	}

	public static void FDABJKODMAI(XmlNode EBLIGDMALEA, ConditionOperator FMFMOPOJBOH, ConditionOfCompletionInspector GLKOKIOFOMD)
	{
		foreach (XmlNode childNode in EBLIGDMALEA.ChildNodes)
		{
			string name = childNode.Name;
			switch (name)
			{
			case "Battle":
			{
				ConditionBattle ePJGLECOIBG2 = new ConditionBattle(childNode);
				FMFMOPOJBOH.BFPIIJDAEME(ePJGLECOIBG2);
				break;
			}
			case "Operator":
			{
				ConditionOperator ePJGLECOIBG = new ConditionOperator(childNode);
				FMFMOPOJBOH.BFPIIJDAEME(ePJGLECOIBG);
				break;
			}
			case "WinBattle":
			{
				ConditionOfCompletionBattle iOFGGOCEIAM = new ConditionOfCompletionBattle(childNode);
				GLKOKIOFOMD.CHDLHMGPDHL(iOFGGOCEIAM);
				break;
			}
			default:
				LLLOJBFMONN.Error(string.Format("CounterConditionsParser::parseCondition - %s", name));
				break;
			}
		}
	}
}
