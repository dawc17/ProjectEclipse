using System.Xml;
using UnityEngine;

public class ComparisonExpression
{
	public enum HBHFGHDGOBI
	{
		COMPARISON_NONE = 0,
		COMPARISON_EQUAL = 1,
		COMPARISON_GREATER = 2,
		COMPARISON_GREATER_EQUAL = 3,
		COMPARISON_LESS = 4,
		COMPARISON_LESS_EQUAL = 5
	}

	private const float FLT_VALUE_DELTA = 1E-05f;

	protected bool _isTrue;

	protected HBHFGHDGOBI MDLIBPIHOMD;

	protected float MAFCNMOAIDA;

	protected float DIKPCBMONEH;

	public ComparisonExpression(XmlNode node)
	{
		MDLIBPIHOMD = GGNIBBDFBED(node.Name);
		_isTrue = node.Attributes["Not"] == null || (!(node.Attributes["Not"].Value == "True") && !(node.Attributes["Not"].Value == "1"));
	}

	public bool Compare()
	{
		bool flag = true;
		switch (MDLIBPIHOMD)
		{
		case HBHFGHDGOBI.COMPARISON_EQUAL:
			flag = Mathf.Abs(MAFCNMOAIDA - DIKPCBMONEH) < 1E-05f;
			break;
		case HBHFGHDGOBI.COMPARISON_GREATER:
			flag = MAFCNMOAIDA - DIKPCBMONEH > 1E-05f;
			break;
		case HBHFGHDGOBI.COMPARISON_GREATER_EQUAL:
			flag = MAFCNMOAIDA - DIKPCBMONEH > -1E-05f;
			break;
		case HBHFGHDGOBI.COMPARISON_LESS:
			flag = MAFCNMOAIDA - DIKPCBMONEH < -1E-05f;
			break;
		case HBHFGHDGOBI.COMPARISON_LESS_EQUAL:
			flag = MAFCNMOAIDA - DIKPCBMONEH < 1E-05f;
			break;
		}
		return (!_isTrue) ? (!flag) : flag;
	}

	public static HBHFGHDGOBI GGNIBBDFBED(string name)
	{
		switch (name)
		{
		case "Equal":
			return HBHFGHDGOBI.COMPARISON_EQUAL;
		case "Greater":
			return HBHFGHDGOBI.COMPARISON_GREATER;
		case "GreaterEqual":
			return HBHFGHDGOBI.COMPARISON_GREATER_EQUAL;
		case "Less":
			return HBHFGHDGOBI.COMPARISON_LESS;
		case "LessEqual":
			return HBHFGHDGOBI.COMPARISON_LESS_EQUAL;
		default:
			return HBHFGHDGOBI.COMPARISON_NONE;
		}
	}
}
