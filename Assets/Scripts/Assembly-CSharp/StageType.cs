public static class StageType
{
	public enum FDBBPEGEGMK
	{
		STAGE_NONE = 0,
		STAGE_START_STANCE = 1,
		STAGE_FIGHT = 2,
		STAGE_END_STANCE = 3,
		STAGE_SHOP_START = 4,
		STAGE_SHOP_PURCHASE = 5,
		STAGE_PEACEFUL_RESTORE = 6,
		STAGE_SHOP_TRY_ON = 7
	}

	public static FDBBPEGEGMK GetStageByName(string name)
	{
		switch (name)
		{
		case "StartStance":
			return FDBBPEGEGMK.STAGE_START_STANCE;
		case "Fight":
			return FDBBPEGEGMK.STAGE_FIGHT;
		case "EndStance":
			return FDBBPEGEGMK.STAGE_END_STANCE;
		case "PeacefulStart":
			return FDBBPEGEGMK.STAGE_SHOP_START;
		case "ShopPurchase":
			return FDBBPEGEGMK.STAGE_SHOP_PURCHASE;
		case "PeacefulRestore":
			return FDBBPEGEGMK.STAGE_PEACEFUL_RESTORE;
		case "TryOn":
			return FDBBPEGEGMK.STAGE_SHOP_TRY_ON;
		default:
			LLLOJBFMONN.Error("StageType::getStageByName - unknown stage: %s", name);
			return FDBBPEGEGMK.STAGE_NONE;
		}
	}
}
