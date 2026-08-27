using System.Collections.Generic;
using System.Xml;

public class ConditionList : ConditionAnimation
{
	public enum PJDDCKKJBNB
	{
		AND = 0,
		OR = 1
	}

	private PJDDCKKJBNB DGHJEHMPAOP;

	private List<ConditionAnimation> KEJBANPKCFA = new List<ConditionAnimation>();

	public List<ConditionAnimation> JIFAHHGNPFH
	{
		get
		{
			return KJILOMLMMEN();
		}
	}

	public ConditionList(XmlNode node, List<ConditionAnimation> conditions)
		: base(DGAGKLODADD.LIST)
	{
		string text = XmlUtils.ParseString(node.Attributes["Type"]);
		DGHJEHMPAOP = ((text == "Or") ? PJDDCKKJBNB.OR : PJDDCKKJBNB.AND);
		KEJBANPKCFA = conditions;
	}

	public PJDDCKKJBNB get_Type()
	{
		return DGHJEHMPAOP;
	}

	public List<ConditionAnimation> KJILOMLMMEN()
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
			if (item.Type == DGAGKLODADD.EVENT && ACENLMONNPA != null)
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
			if (flag && DGHJEHMPAOP == PJDDCKKJBNB.OR)
			{
				return true;
			}
			if (!flag && DGHJEHMPAOP == PJDDCKKJBNB.AND)
			{
				return false;
			}
		}
		return PJDDCKKJBNB.AND == DGHJEHMPAOP;
	}
}
