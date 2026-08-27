using System.Collections.Generic;
using System.Xml;
using CodeStage.AntiCheat.ObscuredTypes;

public class Reward
{
	private RewardPrize GMNOFEBABLM = new RewardPrize();

	private List<LogMessage> PCAMNBMIHIN = new List<LogMessage>();

	public Reward(XmlNode node, ushort CDCJKJNGPOE = 0, ushort MCDAHGPLLDO = 0)
	{
		GMNOFEBABLM.Parse(node, CDCJKJNGPOE, MCDAHGPLLDO);
		foreach (XmlNode item2 in node.SelectNodes("Level"))
		{
			LogMessage item = default(LogMessage);
			item.DPIIJICBGGA = new RewardPrize();
			item.DPIIJICBGGA.Parse(item2, CDCJKJNGPOE, MCDAHGPLLDO);
			item.BDJKDCMHEBI = item2.Attributes["Min"].ParseInt(int.MinValue);
			item.CIKLDJLOFDJ = item2.Attributes["Max"].ParseInt(int.MaxValue);
			item.DPIIJICBGGA.IsCloned = true;
			PCAMNBMIHIN.Add(item);
		}
	}

	public RewardPrize KOBOIFJNPMO(int GNLOCMLBNHF)
	{
		RewardPrize cMHHEHILIIH = new RewardPrize();
		cMHHEHILIIH.IsCloned = true;
		cMHHEHILIIH.HNJGHOKCDJF(GMNOFEBABLM);
		foreach (LogMessage item in PCAMNBMIHIN)
		{
			if (item.BDJKDCMHEBI <= GNLOCMLBNHF && GNLOCMLBNHF <= item.CIKLDJLOFDJ)
			{
				cMHHEHILIIH.HNJGHOKCDJF(item.DPIIJICBGGA);
			}
		}
		return cMHHEHILIIH;
	}

	public void ApplyDenomination(int NPFOBKBJAOB)
	{
		GMNOFEBABLM.GBGNFPNCGED = (ObscuredLong)(GameUtils.GetDenominatedValue((ObscuredLong)(GMNOFEBABLM.GBGNFPNCGED), NPFOBKBJAOB));
		foreach (LogMessage item in PCAMNBMIHIN)
		{
			item.DPIIJICBGGA.GBGNFPNCGED = (ObscuredLong)(GameUtils.GetDenominatedValue((ObscuredLong)(item.DPIIJICBGGA.GBGNFPNCGED), NPFOBKBJAOB));
		}
	}

	public void RandomizeObscuredVars()
	{
		GMNOFEBABLM.RandomizeObscuredVars();
		PCAMNBMIHIN.ForEach((LogMessage DHDMNHCIPEH) =>
		{
			if (DHDMNHCIPEH.DPIIJICBGGA != null)
			{
				DHDMNHCIPEH.DPIIJICBGGA.RandomizeObscuredVars();
			}
		});
	}
}
