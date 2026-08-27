using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

public abstract class PerkCondition : PerkObject
{
	public enum NHDGLPNNNLH
	{
		CONDITION_NONE = 0,
		CONDITION_RANDOM = 1,
		CONDITION_STYLE = 2,
		CONDITION_COMBO = 3,
		CONDITION_ROUND_STAGE = 4,
		CONDITION_CURRENT_ANIMATION = 5,
		CONDITION_CURRENT_INTERVAL = 6,
		CONDITION_HEALTH = 7,
		CONDITION_ITEM = 8,
		CONDITION_ROUND = 9,
		CONDITION_BULLETS = 10,
		CONDITION_MAGIC_CHARGE = 11,
		CONDITION_MOD_EXISTS = 12,
		CONDITION_PAIN = 13,
		CONDITION_OPERATOR = 14,
		CONDITION_IN_THE_AREA = 15,
		CONDITION_PERK_START = 16,
		CONDITION_PERK_COMPARISON = 17
	}

	protected object Info;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private NHDGLPNNNLH KAHHEBMBCFA;

	public PerkCondition()
	{
		set_Type(NHDGLPNNNLH.CONDITION_NONE);
	}

	public NHDGLPNNNLH get_Type()
	{
		return KAHHEBMBCFA;
	}

	protected void set_Type(NHDGLPNNNLH value)
	{
		KAHHEBMBCFA = value;
	}

	public static List<PerkCondition> Create(XmlNode node, PerkInfoItem AEFFHJGMNFI)
	{
		List<PerkCondition> list = new List<PerkCondition>();
		if (node == null)
		{
			return list;
		}
		foreach (XmlNode childNode in node.ChildNodes)
		{
			PerkCondition iDJILNODHAD = null;
			string name = childNode.Name;
			if (FunctionExtension.MHKNIEBONKD(name) != FunctionExtension.DLLJOIFFBPL.COMPARE_NONE)
			{
				iDJILNODHAD = new PerkConditionComparison();
			}
			else
			{
				switch (name)
				{
				case "Random":
					iDJILNODHAD = new PerkConditionRandom();
					break;
				case "Style":
					iDJILNODHAD = new PerkConditionStyle();
					break;
				case "Combo":
					iDJILNODHAD = new PerkConditionCombo();
					break;
				case "RoundStage":
				case "RoundStageStart":
					iDJILNODHAD = new PerkConditionRoundStage();
					break;
				case "CurrentAnimation":
					iDJILNODHAD = new PerkConditionCurrentAnimation();
					break;
				case "CurrentInterval":
					iDJILNODHAD = new PerkConditionCurrentInterval();
					break;
				case "Health":
					iDJILNODHAD = new PerkConditionHealth();
					break;
				case "Item":
					iDJILNODHAD = new PerkConditionItem();
					break;
				case "Round":
					iDJILNODHAD = new PerkConditionRound();
					break;
				case "Bullets":
					iDJILNODHAD = new PerkConditionBullets();
					break;
				case "MagicCharge":
					iDJILNODHAD = new PerkConditionMagicCharge();
					break;
				case "ModExists":
					iDJILNODHAD = new PerkConditionModExists();
					break;
				case "Pain":
					iDJILNODHAD = new PerkConditionPain();
					break;
				case "Operator":
					iDJILNODHAD = new PerkConditionOperator();
					break;
				case "InTheArea":
					iDJILNODHAD = new PerkConditionInTheArea();
					break;
				case "PerkStart":
					iDJILNODHAD = new PerkConditionPerkStart(AEFFHJGMNFI.Name, false);
					break;
				}
			}
			if (iDJILNODHAD != null)
			{
				iDJILNODHAD.JMOIMIHPBOM(AEFFHJGMNFI);
				iDJILNODHAD.Parse(childNode);
				list.Add(iDJILNODHAD);
			}
		}
		return list;
	}

	public abstract bool IsEqual(Model ACENLMONNPA, List<string> NIKHAICFGNM);

	protected Model EPCPGEPPHLO(Model ACENLMONNPA)
	{
		if (IHJJBIDMEMB == PlayerType.PLAYER_ME)
		{
			return ACENLMONNPA;
		}
		if (IHJJBIDMEMB == PlayerType.PLAYER_ENEMY)
		{
			return ACENLMONNPA.EGGEACCDAEK();
		}
		return null;
	}
}
