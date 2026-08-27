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

	public MANJCIGJPMK(XmlNode node, ushort CDCJKJNGPOE, ushort MCDAHGPLLDO)
	{
		POHFOGPKMMK = null;
		BDJKDCMHEBI = -1;
		CIKLDJLOFDJ = -1;
		POHFOGPKMMK = new Reward(node, CDCJKJNGPOE, MCDAHGPLLDO);
		KHPKDMGDMAB = node.Attributes["Image"].CIPOICEEIBK(string.Empty);
		MHOJBEKALLD = node.Attributes["CancellingItem"].CIPOICEEIBK(string.Empty);
		HLBHGHEJBKE = node.Attributes["ViewType"].CIPOICEEIBK(string.Empty);
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
