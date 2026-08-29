using System.Collections.Generic;
using System.Xml;

public class ConditionAnimation
{
	public enum DGAGKLODADD
	{
		NONE = 0,
		ROUND = 1,
		DISTANCE = 2,
		ANIMATION = 3,
		KEYS = 4,
		WEAPONS = 5,
		PLAYER = 6,
		HEALTH = 7,
		LIST = 8,
		CURRENT_INTERVAL = 9,
		CURRENT_ANIMATION = 10,
		PHYSICS_FRAME = 11,
		ROUND_RESULT = 12,
		ITEM = 13,
		BULLETS = 14,
		PERK = 15,
		BIRTH = 16,
		NAME = 17,
		SCREEN = 18,
		MIRROR = 19,
		MOD_EXISTS = 20,
		EVENT = 21,
		DIRECTION = 22,
		BATTLE_TYPE = 23
	}

	protected ModelType.KEIDBIOIFGA OOFFOILONLO;

	public DGAGKLODADD Type;

	public bool IsNot;

	public ModelType.KEIDBIOIFGA NPEAOKLDJHA
	{
		get
		{
			return FHBAPKNECOM();
		}
		set
		{
			GNPMNEDOFPB(value);
		}
	}

	public ConditionAnimation(DGAGKLODADD LFLGCDNKNJI)
	{
		Type = LFLGCDNKNJI;
		IsNot = false;
	}

	public ModelType.KEIDBIOIFGA FHBAPKNECOM()
	{
		return OOFFOILONLO;
	}

	public void GNPMNEDOFPB(ModelType.KEIDBIOIFGA value)
	{
		OOFFOILONLO = value;
	}

	public virtual void Init()
	{
	}

	public virtual bool IsEqual(ModelConditions conditions)
	{
		LLLOJBFMONN.Error("ERROR: Unknown condition type checked: " + Type);
		return false;
	}

	public virtual bool IsEqual(Model ACENLMONNPA, InfoAnimation DBOLBEOCEME)
	{
		return IsEqual(ACENLMONNPA.EBABHGHPLFK());
	}

	public virtual void Parse(XmlNode BGPKIKNPIKP)
	{
		IsNot = XmlUtils.ParseBool(BGPKIKNPIKP.Attributes["Not"]);
		OOFFOILONLO = ModelType.EHFNOBFLAHI(XmlUtils.ParseString(BGPKIKNPIKP.Attributes["Player"], "Me"));
		Init();
	}

	private static int IOFDJJIABEO(List<ConditionAnimation> BBNKIBKPBLO, DGAGKLODADD KLFPAELMPJL, List<ConditionAnimation> GKHEPKGMEFI)
	{
		int count = GKHEPKGMEFI.Count;
		foreach (ConditionAnimation item in BBNKIBKPBLO)
		{
			if (KLFPAELMPJL == item.Type)
			{
				GKHEPKGMEFI.Add(item);
			}
			if (item.Type == DGAGKLODADD.LIST)
			{
				ConditionList eLFKOGJJNMN = item as ConditionList;
				if (eLFKOGJJNMN != null)
				{
					List<ConditionAnimation> bBNKIBKPBLO = eLFKOGJJNMN.KJILOMLMMEN();
					IOFDJJIABEO(bBNKIBKPBLO, KLFPAELMPJL, GKHEPKGMEFI);
				}
			}
		}
		return GKHEPKGMEFI.Count - count;
	}

	public virtual Model DKDAKGDMHAL(Model BPBMKGHEEBI, ModelType.KEIDBIOIFGA LFLGCDNKNJI)
	{
		return BPBMKGHEEBI.NMGNPBMFJKP(LFLGCDNKNJI);
	}

	public virtual void MJFKNEHGNMB(ModelType.KEIDBIOIFGA LFLGCDNKNJI)
	{
		OOFFOILONLO = LFLGCDNKNJI;
	}
}
