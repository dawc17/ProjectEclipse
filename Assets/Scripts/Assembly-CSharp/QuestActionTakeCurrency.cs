using System.Xml;

public class QuestActionTakeCurrency : QuestAction
{
	private string _type = string.Empty;

	private string _name = string.Empty;

	private string _value = string.Empty;

	private QuestActionsSequence DBONDAIEBPN = new QuestActionsSequence();

	private QuestActionsSequence LDDDPGLPHCO = new QuestActionsSequence();

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		_type = EPKLCPOEELO.Attributes["Type"].CIPOICEEIBK(string.Empty);
		_name = EPKLCPOEELO.Attributes["Name"].CIPOICEEIBK(string.Empty);
		_value = EPKLCPOEELO.Attributes["Value"].CIPOICEEIBK(string.Empty);
		XmlNode ePKLCPOEELO = EPKLCPOEELO["Success"];
		XmlNode ePKLCPOEELO2 = EPKLCPOEELO["Error"];
		APKBANHAEGN(ePKLCPOEELO, DBONDAIEBPN, OnActionComplete);
		APKBANHAEGN(ePKLCPOEELO2, LDDDPGLPHCO, OnActionComplete);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		GKFMJKAAJCA();
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		string LFLGCDNKNJI = string.Empty;
		string name = string.Empty;
		long value = 0L;
		GetValues(ref LFLGCDNKNJI, ref name, ref value);
		bool flag = GetIsCurrencyExist(LFLGCDNKNJI, name);
		bool flag2 = GetCurrencyCount(LFLGCDNKNJI, name) >= value;
		if (flag && flag2)
		{
			AddCurrencyCount(LFLGCDNKNJI, name, value);
			MenuController.IAMGKKOINFC();
			DBONDAIEBPN.DEJMHFMLKIC(GFIHPBCEEOB);
		}
		else
		{
			LDDDPGLPHCO.DEJMHFMLKIC(GFIHPBCEEOB);
		}
	}

	private bool GetIsCurrencyExist(string LFLGCDNKNJI, string name)
	{
		bool result = false;
		switch (LFLGCDNKNJI)
		{
		case "Gold":
		case "Bonus":
			result = true;
			break;
		case "Currency":
			result = ListSF.CCDKHLAMKKO().GetIsCurrencyExist(name);
			break;
		default:
			if (LFLGCDNKNJI != string.Empty)
			{
				result = ListSF.CCDKHLAMKKO().GetIsCurrencyExist(LFLGCDNKNJI);
			}
			break;
		}
		return result;
	}

	private long GetCurrencyCount(string LFLGCDNKNJI, string name)
	{
		long num = 0L;
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		switch (LFLGCDNKNJI)
		{
		case "Gold":
			return nKGLHEGIKKP.BFBOEGMAMNF();
		case "Bonus":
			return nKGLHEGIKKP.EHFJHFDACMP();
		case "Currency":
			return nKGLHEGIKKP.GetCurrencyCount(name);
		default:
			return nKGLHEGIKKP.GetCurrencyCount(LFLGCDNKNJI);
		}
	}

	private void AddCurrencyCount(string LFLGCDNKNJI, string name, long value)
	{
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		switch (LFLGCDNKNJI)
		{
		case "Gold":
			nKGLHEGIKKP.OIOOMAKNIOB(nKGLHEGIKKP.BFBOEGMAMNF() - value);
			return;
		case "Bonus":
			nKGLHEGIKKP.LLNELLFMMBB(nKGLHEGIKKP.EHFJHFDACMP() - value, Roster.HPOIJPGPOCF.CHANGE_QUEST);
			return;
		case "Currency":
			nKGLHEGIKKP.AddCurrencyCount(name, (int)(-value));
			return;
		}
		if (LFLGCDNKNJI != string.Empty)
		{
			nKGLHEGIKKP.AddCurrencyCount(LFLGCDNKNJI, (int)(-value));
		}
	}

	private void GetValues(ref string LFLGCDNKNJI, ref string name, ref long value)
	{
		ConditionExtension.CompareResult lNIDLHOIHIM = new ConditionExtension.CompareResult();
		QuestCondition kKDGLNECFHA = new QuestCondition();
		kKDGLNECFHA.LIMHBJBEEIA(PAJDEKLLFNJ);
		kKDGLNECFHA.MCPIOGALBMK(_type, lNIDLHOIHIM);
		LFLGCDNKNJI = lNIDLHOIHIM.ToString();
		lNIDLHOIHIM.Clear();
		kKDGLNECFHA.MCPIOGALBMK(_name, lNIDLHOIHIM);
		name = lNIDLHOIHIM.ToString();
		lNIDLHOIHIM.Clear();
		kKDGLNECFHA.MCPIOGALBMK(_value, lNIDLHOIHIM);
		value = lNIDLHOIHIM.ToString().ToLong(0L);
	}

	private void OnActionComplete(object data)
	{
		OGIJONMKABB();
	}

	public override void GKFMJKAAJCA()
	{
		DBONDAIEBPN.FHPKJMMLIEG();
		LDDDPGLPHCO.FHPKJMMLIEG();
	}
}
