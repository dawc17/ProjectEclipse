using System.Diagnostics;
using SimpleJSON;
using SF2.Offline;

public class FIKEKJBAKBO
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string JOEPMODEEBN;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string FPBPEJHHBBN;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string BALMFJPGGLO;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string ODIBAGNGCIE;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string AEPPEGMGNEO;

	private Product KNLGJKLFLHF;

	public string LDFKMIOPLKA
	{
		get
		{
			return KBNGPOIINHL();
		}
		private set
		{
			NGMLJMAOAPA(value);
		}
	}

	public string GAJNJJELICI
	{
		get
		{
			return JICEOHCLPJP();
		}
		private set
		{
			OPLLCJDAOLF(value);
		}
	}

	public string LBAMJPCNCNK
	{
		get
		{
			return NLHGDFGNIHB();
		}
		private set
		{
			ODECIOLOGDP(value);
		}
	}

	public string KBPMGJPFEAG
	{
		get
		{
			return KGNGCPEGMJP();
		}
		private set
		{
			HLGMMPILEHO(value);
		}
	}

	public string FBFKEJEOELM
	{
		get
		{
			return MCDDGNJEKEO();
		}
		private set
		{
			PAHOBGBPBCG(value);
		}
	}

	public ProductDefinition HPPHLBFCCKB
	{
		get
		{
			return ILOONNDHLLI();
		}
	}

	public string FCCNPMNNGAN
	{
		get
		{
			return JLDEALIEEJI();
		}
	}

	public bool KJPIMCPCBBD
	{
		get
		{
			return LECDJMCDKJI();
		}
	}

	public FIKEKJBAKBO(Product KDOEGOIJKLG)
	{
		KNLGJKLFLHF = KDOEGOIJKLG;
		JSONNode jSONNode = JSONNode.Parse(KNLGJKLFLHF.receipt);
		NGMLJMAOAPA((!jSONNode.HasValue("Store")) ? string.Empty : jSONNode["Store"].Value);
		OPLLCJDAOLF((!jSONNode.HasValue("TransactionID")) ? string.Empty : jSONNode["TransactionID"].Value);
		ODECIOLOGDP((!jSONNode.HasValue("Payload")) ? string.Empty : jSONNode["Payload"].Value);
		if (!LECDJMCDKJI())
		{
			jSONNode = JSONNode.Parse(NLHGDFGNIHB());
			HLGMMPILEHO((!jSONNode.HasValue("json")) ? string.Empty : jSONNode["json"].Value);
			PAHOBGBPBCG((!jSONNode.HasValue("signature")) ? string.Empty : jSONNode["signature"].Value);
		}
		else
		{
			HLGMMPILEHO(string.Empty);
			PAHOBGBPBCG(string.Empty);
		}
	}

	public string KBNGPOIINHL()
	{
		return JOEPMODEEBN;
	}

	private void NGMLJMAOAPA(string value)
	{
		JOEPMODEEBN = value;
	}

	public string JICEOHCLPJP()
	{
		return FPBPEJHHBBN;
	}

	private void OPLLCJDAOLF(string value)
	{
		FPBPEJHHBBN = value;
	}

	public string NLHGDFGNIHB()
	{
		return BALMFJPGGLO;
	}

	private void ODECIOLOGDP(string value)
	{
		BALMFJPGGLO = value;
	}

	public string KGNGCPEGMJP()
	{
		return ODIBAGNGCIE;
	}

	private void HLGMMPILEHO(string value)
	{
		ODIBAGNGCIE = value;
	}

	public string MCDDGNJEKEO()
	{
		return AEPPEGMGNEO;
	}

	private void PAHOBGBPBCG(string value)
	{
		AEPPEGMGNEO = value;
	}

	public ProductDefinition ILOONNDHLLI()
	{
		return KNLGJKLFLHF.definition;
	}

	public string JLDEALIEEJI()
	{
		return ILOONNDHLLI().id;
	}

	public bool LECDJMCDKJI()
	{
		return KBNGPOIINHL().ToLower() == "fake";
	}
}
