using System.Collections.Generic;

public static class MovesMaps
{
	public enum NHKAHBBOIHG
	{
		KEY_TYPE = 0,
		DISTANCE_OBJECT_TYPE = 1
	}

	private static Dictionary<string, ConditionAnimation.DGAGKLODADD> EDIIJIFPPAP;

	private static Dictionary<string, FightCID> IMIDLONNDLK;

	private static Dictionary<string, DistancePoint.JJIAEPLMBFF> JJENJCAHOJC;

	public static void Init()
	{
		Clear();
		EDIIJIFPPAP = new Dictionary<string, ConditionAnimation.DGAGKLODADD>();
		IMIDLONNDLK = new Dictionary<string, FightCID>();
		JJENJCAHOJC = new Dictionary<string, DistancePoint.JJIAEPLMBFF>();
		EDIIJIFPPAP.Add("RoundStage", ConditionAnimation.DGAGKLODADD.ROUND);
		EDIIJIFPPAP.Add("Keys", ConditionAnimation.DGAGKLODADD.KEYS);
		EDIIJIFPPAP.Add("Distance", ConditionAnimation.DGAGKLODADD.DISTANCE);
		EDIIJIFPPAP.Add("Direction", ConditionAnimation.DGAGKLODADD.DIRECTION);
		EDIIJIFPPAP.Add("Weapon", ConditionAnimation.DGAGKLODADD.WEAPONS);
		EDIIJIFPPAP.Add("Player", ConditionAnimation.DGAGKLODADD.PLAYER);
		EDIIJIFPPAP.Add("Health", ConditionAnimation.DGAGKLODADD.HEALTH);
		EDIIJIFPPAP.Add("Operator", ConditionAnimation.DGAGKLODADD.LIST);
		EDIIJIFPPAP.Add("CurrentInterval", ConditionAnimation.DGAGKLODADD.CURRENT_INTERVAL);
		EDIIJIFPPAP.Add("CurrentAnimation", ConditionAnimation.DGAGKLODADD.CURRENT_ANIMATION);
		EDIIJIFPPAP.Add("PhysicsFrameNumber", ConditionAnimation.DGAGKLODADD.PHYSICS_FRAME);
		EDIIJIFPPAP.Add("RoundResult", ConditionAnimation.DGAGKLODADD.ROUND_RESULT);
		EDIIJIFPPAP.Add("Item", ConditionAnimation.DGAGKLODADD.ITEM);
		EDIIJIFPPAP.Add("Perk", ConditionAnimation.DGAGKLODADD.PERK);
		EDIIJIFPPAP.Add("Bullets", ConditionAnimation.DGAGKLODADD.BULLETS);
		EDIIJIFPPAP.Add("Birth", ConditionAnimation.DGAGKLODADD.BIRTH);
		EDIIJIFPPAP.Add("Name", ConditionAnimation.DGAGKLODADD.NAME);
		EDIIJIFPPAP.Add("Screen", ConditionAnimation.DGAGKLODADD.SCREEN);
		EDIIJIFPPAP.Add("ModelMirrored", ConditionAnimation.DGAGKLODADD.MIRROR);
		EDIIJIFPPAP.Add("ModExists", ConditionAnimation.DGAGKLODADD.MOD_EXISTS);
		IMIDLONNDLK.Add("Up", FightCID.QuadrantUp);
		IMIDLONNDLK.Add("Up-Forward", FightCID.QuadrantUpForward);
		IMIDLONNDLK.Add("Forward", FightCID.QuadrantForward);
		IMIDLONNDLK.Add("Down-Forward", FightCID.QuadrantDownForward);
		IMIDLONNDLK.Add("Down", FightCID.QuadrantDown);
		IMIDLONNDLK.Add("Down-Back", FightCID.QuadrantDownBack);
		IMIDLONNDLK.Add("Back", FightCID.QuadrantBack);
		IMIDLONNDLK.Add("Up-Back", FightCID.QuadrantUpBack);
		IMIDLONNDLK.Add("Punch", FightCID.Punch);
		IMIDLONNDLK.Add("Kick", FightCID.Kick);
		IMIDLONNDLK.Add("Ranged", FightCID.MissileButton);
		IMIDLONNDLK.Add("Magic", FightCID.MagicButton);
		IMIDLONNDLK.Add("RaidCharge", FightCID.RaidChargeButton);
		IMIDLONNDLK.Add("Super", FightCID.Super);
		JJENJCAHOJC.Add("Nodes", DistancePoint.JJIAEPLMBFF.OBJECT_NODES);
		JJENJCAHOJC.Add("Pivot", DistancePoint.JJIAEPLMBFF.OBJECT_PIVOT);
		JJENJCAHOJC.Add("Wall", DistancePoint.JJIAEPLMBFF.OBJECT_WALL);
		JJENJCAHOJC.Add("Floor", DistancePoint.JJIAEPLMBFF.OBJECT_FLOOR);
		JJENJCAHOJC.Add("COM", DistancePoint.JJIAEPLMBFF.OBJECT_COM);
	}

	public static void Clear()
	{
		if (EDIIJIFPPAP != null)
		{
			EDIIJIFPPAP.Clear();
		}
		if (IMIDLONNDLK != null)
		{
			IMIDLONNDLK.Clear();
		}
		if (JJENJCAHOJC != null)
		{
			JJENJCAHOJC.Clear();
		}
		EDIIJIFPPAP = null;
		IMIDLONNDLK = null;
		JJENJCAHOJC = null;
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
			return (int)(IMIDLONNDLK.ContainsKey(value) ? IMIDLONNDLK[value] : FightCID.QuadrantZero);
		case NHKAHBBOIHG.DISTANCE_OBJECT_TYPE:
			if (value == null)
			{
				return 0;
			}
			return (int)(JJENJCAHOJC.ContainsKey(value) ? JJENJCAHOJC[value] : DistancePoint.JJIAEPLMBFF.OBJECT_NULL);
		default:
			LLLOJBFMONN.Error("ERROR: MovesMaps::getIndex - no map for index: " + BLGLGNGBADH);
			return -1;
		}
	}

	public static ConditionAnimation.DGAGKLODADD MHKNIEBONKD(string value)
	{
		if (EDIIJIFPPAP.ContainsKey(value))
		{
			return EDIIJIFPPAP[value];
		}
		return ConditionAnimation.DGAGKLODADD.EVENT;
	}
}
