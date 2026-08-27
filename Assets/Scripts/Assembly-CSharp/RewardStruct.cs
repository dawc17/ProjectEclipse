using System.Xml;

public class RewardStruct
{
	public Reward BBCCBPIIELF;

	public Reward FMOGFMIGLNP;

	public Reward LJLIFMOIAJJ;

	public RewardStruct(XmlNode node, ushort CDCJKJNGPOE, ushort MCDAHGPLLDO)
	{
		BBCCBPIIELF = new Reward(node, CDCJKJNGPOE, MCDAHGPLLDO);
		XmlNode xmlNode = node["NormalModeReward"];
		if (xmlNode != null)
		{
			FMOGFMIGLNP = new Reward(xmlNode, CDCJKJNGPOE, MCDAHGPLLDO);
		}
		else
		{
			FMOGFMIGLNP = null;
		}
		XmlNode xmlNode2 = node["EclipseModeReward"];
		if (xmlNode2 != null)
		{
			LJLIFMOIAJJ = new Reward(xmlNode2, CDCJKJNGPOE, MCDAHGPLLDO);
		}
		else
		{
			LJLIFMOIAJJ = null;
		}
	}

	public void RandomizeObscuredVars()
	{
		if (BBCCBPIIELF != null)
		{
			BBCCBPIIELF.RandomizeObscuredVars();
		}
		if (FMOGFMIGLNP != null)
		{
			FMOGFMIGLNP.RandomizeObscuredVars();
		}
		if (LJLIFMOIAJJ != null)
		{
			LJLIFMOIAJJ.RandomizeObscuredVars();
		}
	}

	public RewardPrize KOBOIFJNPMO(int GNLOCMLBNHF)
	{
		RewardPrize cMHHEHILIIH = new RewardPrize();
		if (BBCCBPIIELF != null)
		{
			cMHHEHILIIH = BBCCBPIIELF.KOBOIFJNPMO(GNLOCMLBNHF);
			cMHHEHILIIH.IsCloned = true;
		}
		Reward lOELDGJGPIF = ((!ListSF.CCDKHLAMKKO().JPMPIDFGCJL()) ? FMOGFMIGLNP : LJLIFMOIAJJ);
		if (lOELDGJGPIF != null)
		{
			RewardPrize cMHHEHILIIH2 = lOELDGJGPIF.KOBOIFJNPMO(GNLOCMLBNHF);
			cMHHEHILIIH2.IsCloned = true;
			cMHHEHILIIH.HNJGHOKCDJF(cMHHEHILIIH2);
		}
		return cMHHEHILIIH;
	}
}
