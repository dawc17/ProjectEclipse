using System.Xml;

public class Achievement
{
	public int Priority;

	public int EOGLBDCLMBM;

	public int ANCDKCFLHOL;

	public int LBJFKGAHBBG;

	public string Name;

	public string MGNNJPBCOGD;

	public string MJBPMLCLMFN;

	public string EIEBHLJCOKE;

	public bool HGMHEOGJDMM;

	public bool NMCBAKACIGK;

	public bool GDCBBAHKCIE;

	private bool ENNNIICHPBM;

	public bool IsNew
	{
		get
		{
			return DBHJGAGOLOB();
		}
		set
		{
			BEBDMOEIEJN(value);
		}
	}

	public Achievement(XmlNode node)
	{
		EOGLBDCLMBM = node.Attributes["CounterValue"].ParseInt();
		Priority = node.Attributes["Priority"].ParseInt();
		Name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		MGNNJPBCOGD = node.Attributes["Description"].CIPOICEEIBK(string.Empty);
		MJBPMLCLMFN = node.Attributes["Icon"].CIPOICEEIBK(string.Empty);
		ANCDKCFLHOL = node.Attributes["MoneyPrize"].ParseInt();
		LBJFKGAHBBG = node.Attributes["BonusPrize"].ParseInt();
		GDCBBAHKCIE = node.Attributes["Hidden"].ParseBool();
		HGMHEOGJDMM = false;
		NMCBAKACIGK = false;
		ENNNIICHPBM = false;
		if (SystemProperties.IPJFCBAGMJJ())
		{
			EIEBHLJCOKE = node.Attributes["GooglePlayID"].CIPOICEEIBK(string.Empty);
		}
		else
		{
			EIEBHLJCOKE = node.Attributes["GameCenterID"].CIPOICEEIBK(string.Empty);
		}
	}

	public void BEBDMOEIEJN(bool value)
	{
		ENNNIICHPBM = value && (ANCDKCFLHOL > 0 || LBJFKGAHBBG > 0);
	}

	public bool DBHJGAGOLOB()
	{
		return ENNNIICHPBM;
	}

	public string CIOKDNDHFBE()
	{
		return MJBPMLCLMFN;
	}
}
