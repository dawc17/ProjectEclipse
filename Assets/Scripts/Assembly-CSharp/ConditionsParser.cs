using System.Collections.Generic;
using System.Xml;

public class ConditionsParser
{
	public static ConditionAnimation Create(XmlNode node)
	{
		ConditionAnimation result = null;
		string name = node.Name;
        if (name == "EclipseCharacter") return new Eclipse.Modding.ModCharacterCondition(node.Attributes["Name"]?.Value);
		switch (MovesMaps.MHKNIEBONKD(name))
		{
		case ConditionAnimation.ConditionType.ROUND:
			result = new ConditionRound(node);
			break;
		case ConditionAnimation.ConditionType.KEYS:
			result = new ConditionKeys(node);
			break;
		case ConditionAnimation.ConditionType.LIST:
		{
			List<ConditionAnimation> list = new List<ConditionAnimation>();
			ParseInside(list, node);
			result = new ConditionList(node, list);
			break;
		}
		case ConditionAnimation.ConditionType.CURRENT_INTERVAL:
			result = new ConditionInterval(node);
			break;
		case ConditionAnimation.ConditionType.CURRENT_ANIMATION:
			result = new ConditionCurrentAnimation(node);
			break;
		case ConditionAnimation.ConditionType.PLAYER:
			result = new ConditionPlayer(node);
			break;
		case ConditionAnimation.ConditionType.PHYSICS_FRAME:
			result = new ConditionPhysics(node);
			break;
		case ConditionAnimation.ConditionType.HEALTH:
			result = new ConditionHealth(node);
			break;
		case ConditionAnimation.ConditionType.ROUND_RESULT:
			result = new ConditionRoundResult(node);
			break;
		case ConditionAnimation.ConditionType.ANIMATION:
			result = new ConditionCurrentAnimation(node);
			break;
		case ConditionAnimation.ConditionType.DISTANCE:
			result = new ConditionDistance(node);
			break;
		case ConditionAnimation.ConditionType.DIRECTION:
			result = new ConditionDirection(node);
			break;
		case ConditionAnimation.ConditionType.ITEM:
			result = new ConditionItemInfo(node);
			break;
		case ConditionAnimation.ConditionType.PERK:
			result = new ConditionPerk(node);
			break;
		case ConditionAnimation.ConditionType.WEAPONS:
			result = new ConditionWeapon(node);
			break;
		case ConditionAnimation.ConditionType.BULLETS:
			result = new ConditionBullets(node);
			break;
		case ConditionAnimation.ConditionType.BIRTH:
			result = new ConditionBirth(node);
			break;
		case ConditionAnimation.ConditionType.NAME:
			result = new ConditionName(node);
			break;
		case ConditionAnimation.ConditionType.SCREEN:
			result = new ConditionScene(node);
			break;
		case ConditionAnimation.ConditionType.MIRROR:
			result = new ConditionModelMirrored(node);
			break;
		case ConditionAnimation.ConditionType.MOD_EXISTS:
			result = new ConditionModExists(node);
			break;
		case ConditionAnimation.ConditionType.EVENT:
			result = new ConditionEvent(node);
			break;
		case ConditionAnimation.ConditionType.BATTLE_TYPE:
			result = new Eclipse.Content.BattleTypeMoveCondition(node);
			break;
		case ConditionAnimation.ConditionType.BOSS_ABILITY_STATE:
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
