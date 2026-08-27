using System.Collections.Generic;

public class PricesDataContainer
{
	private List<PricesData> NHDOHBCNLGB = new List<PricesData>();

	public List<PricesData> IHHMHNHOLCB
	{
		get
		{
			return GMCBGMPEHLF();
		}
	}

	public List<PricesData> GMCBGMPEHLF()
	{
		return NHDOHBCNLGB;
	}

	public PricesData CCFOOCDFGMF(string JKKKGIOHNMH)
	{
		return NHDOHBCNLGB.Find((PricesData DHDMNHCIPEH) => DHDMNHCIPEH.GNIJPFLLNIC == JKKKGIOHNMH || DHDMNHCIPEH.GFMKCJPKMOK == JKKKGIOHNMH);
	}

	public PricesData LIKBNIAJHKA(string BMCEHAPAJCA)
	{
		return NHDOHBCNLGB.Find((PricesData DHDMNHCIPEH) => DHDMNHCIPEH.name == BMCEHAPAJCA);
	}

	public bool LIKBNIAJHKA(string BMCEHAPAJCA, out float HCHKFOJEEBK)
	{
		HCHKFOJEEBK = 0f;
		PricesData bEOLBLGJCKA = LIKBNIAJHKA(BMCEHAPAJCA);
		if (bEOLBLGJCKA != null)
		{
			return float.TryParse(bEOLBLGJCKA.GFIMMDLCPMI(), out HCHKFOJEEBK);
		}
		return false;
	}
}
