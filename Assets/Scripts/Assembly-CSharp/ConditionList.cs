using System.Collections.Generic;
using System.Xml;

public class ConditionList : ConditionAnimation
{
	// best guess for name
	public enum OperatorType
	{
		AND = 0,
		OR = 1
	}

	private OperatorType DGHJEHMPAOP;

	private List<ConditionAnimation> KEJBANPKCFA = new List<ConditionAnimation>();

	public List<ConditionAnimation> JIFAHHGNPFH
	{
		get
		{
			return GetConditions();
		}
	}

	public ConditionList(XmlNode node, List<ConditionAnimation> conditions)
		: base(ConditionType.LIST)
	{
		string text = XmlUtils.ParseString(node.Attributes["Type"]);
		DGHJEHMPAOP = ((text == "Or") ? OperatorType.OR : OperatorType.AND);
		KEJBANPKCFA = conditions;
	}

	public OperatorType get_Type()
	{
		return DGHJEHMPAOP;
	}

	// best guess for name
	public List<ConditionAnimation> GetConditions()
	{
		return KEJBANPKCFA;
	}

	public override bool IsEqual(ModelConditions conditions)
	{
		bool flag = OIBMEHKCPKB(conditions);
		return (!IsNot) ? flag : (!flag);
	}

	public bool DJEJMGCMPPH(ModelConditions conditions, Model ACENLMONNPA = null, EventAnimation DOANBADPBGH = null)
	{
		bool flag = OIBMEHKCPKB(conditions, ACENLMONNPA, DOANBADPBGH);
		return (!IsNot) ? flag : (!flag);
	}

	private bool OIBMEHKCPKB(ModelConditions conditions, Model ACENLMONNPA = null, EventAnimation DOANBADPBGH = null)
	{
		foreach (ConditionAnimation item in KEJBANPKCFA)
		{
			bool flag = false;
			if (item.Type == ConditionType.EVENT && ACENLMONNPA != null)
			{
				ModelType.KEIDBIOIFGA lFLGCDNKNJI = item.FHBAPKNECOM();
				Model fGCODGKLHED = item.DKDAKGDMHAL(ACENLMONNPA, lFLGCDNKNJI);
				ModelConditions dGJJDPIAEAO = fGCODGKLHED.EBABHGHPLFK();
				if (DOANBADPBGH != null)
				{
					dGJJDPIAEAO.HFCIDBJJINB = DOANBADPBGH;
					DOANBADPBGH.JIFAHHGNPFH = dGJJDPIAEAO;
				}
				flag = item.IsEqual(dGJJDPIAEAO);
			}
			else
			{
				flag = item.IsEqual(conditions);
			}
			if (flag && DGHJEHMPAOP == OperatorType.OR)
			{
				return true;
			}
			if (!flag && DGHJEHMPAOP == OperatorType.AND)
			{
				return false;
			}
		}
		return OperatorType.AND == DGHJEHMPAOP;
	}
}
