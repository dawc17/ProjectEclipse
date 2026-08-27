using UnityEngine.UI;

public static class NFOGOFFAPPP
{
	public enum HHGPKAJENGF
	{
		PressNormal = 0,
		PressSelected = 1,
		PressInactive = 2,
		PressPressed = 3
	}

	public static void OFPNNIBBNCE(this Button GAMILDJHFDB, HHGPKAJENGF BGFHMJMEGEE, bool GHJGPAEDIHG = true)
	{
		switch (BGFHMJMEGEE)
		{
		case HHGPKAJENGF.PressNormal:
			GAMILDJHFDB.interactable = true;
			break;
		case HHGPKAJENGF.PressSelected:
			break;
		case HHGPKAJENGF.PressInactive:
			GAMILDJHFDB.interactable = false;
			break;
		case HHGPKAJENGF.PressPressed:
			GAMILDJHFDB.Select();
			break;
		}
	}

	public static HHGPKAJENGF GetPressType(this Button GAMILDJHFDB)
	{
		if (GAMILDJHFDB.interactable)
		{
			return HHGPKAJENGF.PressNormal;
		}
		return HHGPKAJENGF.PressInactive;
	}
}
