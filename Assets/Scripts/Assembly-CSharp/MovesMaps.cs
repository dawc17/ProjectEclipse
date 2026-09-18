using System.Collections.Generic;

public static class MovesMaps
{
	public enum NHKAHBBOIHG
	{
		KEY_TYPE = 0,
		DISTANCE_OBJECT_TYPE = 1
	}

	private static Dictionary<string, ConditionAnimation.ConditionType> _Conditions;

	private static Dictionary<string, FightCID> _ControlIDs;

	private static Dictionary<string, DistancePoint.Object> _Objects;

	public static void Init()
	{
		Clear();
		_Conditions = new Dictionary<string, ConditionAnimation.ConditionType>();
		_ControlIDs = new Dictionary<string, FightCID>();
		_Objects = new Dictionary<string, DistancePoint.Object>();
		_Conditions.Add("RoundStage", ConditionAnimation.ConditionType.ROUND);
		_Conditions.Add("Keys", ConditionAnimation.ConditionType.KEYS);
		_Conditions.Add("Distance", ConditionAnimation.ConditionType.DISTANCE);
		_Conditions.Add("Direction", ConditionAnimation.ConditionType.DIRECTION);
		_Conditions.Add("Weapon", ConditionAnimation.ConditionType.WEAPONS);
		_Conditions.Add("Player", ConditionAnimation.ConditionType.PLAYER);
		_Conditions.Add("Health", ConditionAnimation.ConditionType.HEALTH);
		_Conditions.Add("Operator", ConditionAnimation.ConditionType.LIST);
		_Conditions.Add("CurrentInterval", ConditionAnimation.ConditionType.CURRENT_INTERVAL);
		_Conditions.Add("CurrentAnimation", ConditionAnimation.ConditionType.CURRENT_ANIMATION);
		_Conditions.Add("PhysicsFrameNumber", ConditionAnimation.ConditionType.PHYSICS_FRAME);
		_Conditions.Add("RoundResult", ConditionAnimation.ConditionType.ROUND_RESULT);
		_Conditions.Add("Item", ConditionAnimation.ConditionType.ITEM);
		_Conditions.Add("Perk", ConditionAnimation.ConditionType.PERK);
		_Conditions.Add("Bullets", ConditionAnimation.ConditionType.BULLETS);
		_Conditions.Add("Birth", ConditionAnimation.ConditionType.BIRTH);
		_Conditions.Add("Name", ConditionAnimation.ConditionType.NAME);
		_Conditions.Add("Screen", ConditionAnimation.ConditionType.SCREEN);
		_Conditions.Add("ModelMirrored", ConditionAnimation.ConditionType.MIRROR);
		_Conditions.Add("ModExists", ConditionAnimation.ConditionType.MOD_EXISTS);
		_Conditions.Add("BattleType", ConditionAnimation.ConditionType.BATTLE_TYPE);
		_Conditions.Add("BossAbilityState", ConditionAnimation.ConditionType.BOSS_ABILITY_STATE);
		_ControlIDs.Add("Up", FightCID.QuadrantUp);
		_ControlIDs.Add("Up-Forward", FightCID.QuadrantUpForward);
		_ControlIDs.Add("Forward", FightCID.QuadrantForward);
		_ControlIDs.Add("Down-Forward", FightCID.QuadrantDownForward);
		_ControlIDs.Add("Down", FightCID.QuadrantDown);
		_ControlIDs.Add("Down-Back", FightCID.QuadrantDownBack);
		_ControlIDs.Add("Back", FightCID.QuadrantBack);
		_ControlIDs.Add("Up-Back", FightCID.QuadrantUpBack);
		_ControlIDs.Add("Punch", FightCID.Punch);
		_ControlIDs.Add("Kick", FightCID.Kick);
		_ControlIDs.Add("Ranged", FightCID.MissileButton);
		_ControlIDs.Add("Magic", FightCID.MagicButton);
		_ControlIDs.Add("RaidCharge", FightCID.RaidChargeButton);
		_ControlIDs.Add("Super", FightCID.Super);
		_Objects.Add("Nodes", DistancePoint.Object.OBJECT_NODES);
		_Objects.Add("Pivot", DistancePoint.Object.OBJECT_PIVOT);
		_Objects.Add("Wall", DistancePoint.Object.OBJECT_WALL);
		_Objects.Add("Floor", DistancePoint.Object.OBJECT_FLOOR);
		_Objects.Add("COM", DistancePoint.Object.OBJECT_COM);
	}

	public static void Clear()
	{
		if (_Conditions != null)
		{
			_Conditions.Clear();
		}
		if (_ControlIDs != null)
		{
			_ControlIDs.Clear();
		}
		if (_Objects != null)
		{
			_Objects.Clear();
		}
		_Conditions = null;
		_ControlIDs = null;
		_Objects = null;
	}

	public static int HHBMBMNLJIE(NHKAHBBOIHG BLGLGNGBADH, string value)
	{
		switch (BLGLGNGBADH)
		{
		case NHKAHBBOIHG.KEY_TYPE:
			if (value == null)
			{
				return 0;
			}
			return (int)(_ControlIDs.ContainsKey(value) ? _ControlIDs[value] : FightCID.QuadrantZero);
		case NHKAHBBOIHG.DISTANCE_OBJECT_TYPE:
			if (value == null)
			{
				return 0;
			}
			return (int)(_Objects.ContainsKey(value) ? _Objects[value] : DistancePoint.Object.OBJECT_NULL);
		default:
			LLLOJBFMONN.Error("ERROR: MovesMaps::getIndex - no map for index: " + BLGLGNGBADH);
			return -1;
		}
	}

	public static ConditionAnimation.ConditionType MHKNIEBONKD(string value)
	{
		if (_Conditions.ContainsKey(value))
		{
			return _Conditions[value];
		}
		return ConditionAnimation.ConditionType.EVENT;
	}
}
