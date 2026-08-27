public static class InputDeviceExtension
{
	public static bool CBIECFPMNKC(ItemInfo item, UserItem NDMCFNGEPOA)
	{
		if (item == null)
		{
			return false;
		}
		if (NDMCFNGEPOA == null && item.EHKNIKHPGDN > 0)
		{
			return true;
		}
		return false;
	}

	public static bool GMCENJHBIDF(ItemInfo item, UserItem NDMCFNGEPOA)
	{
		return NDMCFNGEPOA != null && NDMCFNGEPOA.IJGAOHJNLAH() > 0;
	}

	public static bool ACOIHHPOBDH(ItemInfo item, UserItem NDMCFNGEPOA)
	{
		if (NDMCFNGEPOA == null)
		{
			return false;
		}
		return NDMCFNGEPOA.EPJAMDEFMFB() && !NDMCFNGEPOA.CPBLPMAILGH();
	}

	public static void AOGLFIHGKCN(ref bool PNKJLPDJOJF, ref bool CBDBANOPFDM, ItemInfo item)
	{
		if (item != null)
		{
			UserItem dKCHDHMLKHN = ListSF.CCDKHLAMKKO().KHCNHPCPFII().CMGOCLGHNLH(item);
			if (dKCHDHMLKHN != null)
			{
				PNKJLPDJOJF = true;
				CBDBANOPFDM = dKCHDHMLKHN.EFMFGEPDAOP();
			}
		}
	}
}
