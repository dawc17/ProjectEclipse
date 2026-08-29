using System.Collections.Generic;
using System.Xml;

public class ConditionsParser
{
	public static ConditionAnimation Create(XmlNode node)
	{
		ConditionAnimation result = null;
		string name = node.Name;
		switch (MovesMaps.MHKNIEBONKD(name))
		{
		case ConditionAnimation.DGAGKLODADD.ROUND:
			result = new ConditionRound(node);
			break;
		case ConditionAnimation.DGAGKLODADD.KEYS:
			result = new ConditionKeys(node);
			break;
		case ConditionAnimation.DGAGKLODADD.LIST:
		{
			List<ConditionAnimation> list = new List<ConditionAnimation>();
			ParseInside(list, node);
			result = new ConditionList(node, list);
			break;
		}
		case ConditionAnimation.DGAGKLODADD.CURRENT_INTERVAL:
			result = new ConditionInterval(node);
			break;
		case ConditionAnimation.DGAGKLODADD.CURRENT_ANIMATION:
			result = new ConditionCurrentAnimation(node);
			break;
		case ConditionAnimation.DGAGKLODADD.PLAYER:
			result = new ConditionPlayer(node);
			break;
		case ConditionAnimation.DGAGKLODADD.PHYSICS_FRAME:
			result = new ConditionPhysics(node);
			break;
		case ConditionAnimation.DGAGKLODADD.HEALTH:
			result = new ConditionHealth(node);
			break;
		case ConditionAnimation.DGAGKLODADD.ROUND_RESULT:
			result = new ConditionRoundResult(node);
			break;
		case ConditionAnimation.DGAGKLODADD.ANIMATION:
			result = new ConditionCurrentAnimation(node);
			break;
		case ConditionAnimation.DGAGKLODADD.DISTANCE:
			result = new ConditionDistance(node);
			break;
		case ConditionAnimation.DGAGKLODADD.DIRECTION:
			result = new ConditionDirection(node);
			break;
		case ConditionAnimation.DGAGKLODADD.ITEM:
			result = new ConditionItemInfo(node);
			break;
		case ConditionAnimation.DGAGKLODADD.PERK:
			result = new ConditionPerk(node);
			break;
		case ConditionAnimation.DGAGKLODADD.WEAPONS:
			result = new ConditionWeapon(node);
			break;
		case ConditionAnimation.DGAGKLODADD.BULLETS:
			result = new ConditionBullets(node);
			break;
		case ConditionAnimation.DGAGKLODADD.BIRTH:
			result = new ConditionBirth(node);
			break;
		case ConditionAnimation.DGAGKLODADD.NAME:
			result = new ConditionName(node);
			break;
		case ConditionAnimation.DGAGKLODADD.SCREEN:
			result = new ConditionScene(node);
			break;
		case ConditionAnimation.DGAGKLODADD.MIRROR:
			result = new ConditionModelMirrored(node);
			break;
		case ConditionAnimation.DGAGKLODADD.MOD_EXISTS:
			result = new ConditionModExists(node);
			break;
		case ConditionAnimation.DGAGKLODADD.EVENT:
			result = new ConditionEvent(node);
			break;
		case ConditionAnimation.DGAGKLODADD.BATTLE_TYPE:
			result = new Eclipse.Content.BattleTypeMoveCondition(node);
			break;
		case ConditionAnimation.DGAGKLODADD.BOSS_ABILITY_STATE:
			result = new Eclipse.Content.BossAbilityStateMoveCondition(node);
			break;
		default:
			LLLOJBFMONN.Error("ERROR: ConditionsParser - no condition for \"{0}\"", name);
			break;
		}
		return result;
	}

	public static void ParseInside(List<ConditionAnimation> DCJLKCFKCOM, XmlNode nodes)
	{
		DCJLKCFKCOM.Clear();
		foreach (XmlNode childNode in nodes.ChildNodes)
		{
			ConditionAnimation iIDOLPHMOGA = Create(childNode);
			if (iIDOLPHMOGA != null)
			{
				iIDOLPHMOGA.Parse(childNode);
				DCJLKCFKCOM.Add(iIDOLPHMOGA);
			}
		}
	}
}
