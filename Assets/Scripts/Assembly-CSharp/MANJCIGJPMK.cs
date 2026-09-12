using System.Xml;

public struct MANJCIGJPMK
{
	public enum PNMPEPEMDHH
	{
		prizeCountNone = 0,
		prizeCountMoney = 1,
		prizeCountCurrency = 2
	}

	private PNMPEPEMDHH LJOALDGDOFP;

	private Reward POHFOGPKMMK;

	public int BDJKDCMHEBI;

	public int CIKLDJLOFDJ;

	private string KHPKDMGDMAB;

	private string MHOJBEKALLD;

	private string HLBHGHEJBKE;

	internal float Weight { get; private set; }
	internal string Image => KHPKDMGDMAB;
	internal string CancellingItem => MHOJBEKALLD;
	internal string ViewType => HLBHGHEJBKE;
	internal bool IsAvailableAtLevel(int level) => POHFOGPKMMK != null && GDHOMAGHADB(level);

	internal bool TryEvaluateAtLevel(int level, out RewardPrize prize)
	{
		prize = null;
		if (!IsAvailableAtLevel(level)) return false;
		prize = POHFOGPKMMK.KOBOIFJNPMO(level);
		return true;
	}

	public MANJCIGJPMK(XmlNode node, ushort CDCJKJNGPOE, ushort MCDAHGPLLDO)
	{
		POHFOGPKMMK = null;
		BDJKDCMHEBI = -1;
		CIKLDJLOFDJ = -1;
		POHFOGPKMMK = new Reward(node, CDCJKJNGPOE, MCDAHGPLLDO);
		KHPKDMGDMAB = node.Attributes["Image"].CIPOICEEIBK(string.Empty);
		MHOJBEKALLD = node.Attributes["CancellingItem"].CIPOICEEIBK(string.Empty);
		HLBHGHEJBKE = node.Attributes["ViewType"].CIPOICEEIBK(string.Empty);
		Weight = node.Attributes["Weight"].ParseFloat(1f);
		if (node["Money"] != null)
		{
			LJOALDGDOFP = PNMPEPEMDHH.prizeCountMoney;
		}
		else if (node["Currency"] != null)
		{
			LJOALDGDOFP = PNMPEPEMDHH.prizeCountCurrency;
		}
		else
		{
			LJOALDGDOFP = PNMPEPEMDHH.prizeCountNone;
		}
	}

	private bool GDHOMAGHADB(int GNLOCMLBNHF)
	{
		if (BDJKDCMHEBI < 0 && CIKLDJLOFDJ < 0)
		{
			return true;
		}
		if (BDJKDCMHEBI <= GNLOCMLBNHF && CIKLDJLOFDJ >= GNLOCMLBNHF)
		{
			return true;
		}
		if (BDJKDCMHEBI < 0 && CIKLDJLOFDJ >= GNLOCMLBNHF)
		{
			return true;
		}
		if (BDJKDCMHEBI <= GNLOCMLBNHF && CIKLDJLOFDJ < 0)
		{
			return true;
		}
		return false;
	}
}
