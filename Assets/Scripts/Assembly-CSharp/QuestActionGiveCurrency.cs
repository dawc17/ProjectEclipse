using System.Xml;

public class QuestActionGiveCurrency : QuestAction
{
	private string Type;

	private string Value;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		Type = EPKLCPOEELO.Attributes["Type"].CIPOICEEIBK(string.Empty);
		Value = EPKLCPOEELO.Attributes["Value"].CIPOICEEIBK(string.Empty);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		string LFLGCDNKNJI = string.Empty;
		long value = 0L;
		GetValues(ref LFLGCDNKNJI, ref value);
		if (LFLGCDNKNJI == "Gold")
		{
			nKGLHEGIKKP.OIOOMAKNIOB(nKGLHEGIKKP.BFBOEGMAMNF() + value);
		}
		else if (LFLGCDNKNJI == "Bonus")
		{
			nKGLHEGIKKP.LLNELLFMMBB(nKGLHEGIKKP.EHFJHFDACMP() + value, Roster.HPOIJPGPOCF.CHANGE_QUEST);
		}
		else if (LFLGCDNKNJI != string.Empty)
		{
			nKGLHEGIKKP.AddCurrencyCount(LFLGCDNKNJI, (int)value);
		}
		MenuController.OPPMFDNNBDE();
		OGIJONMKABB();
	}

	private void GetValues(ref string LFLGCDNKNJI, ref long value)
	{
		ConditionExtension.CompareResult lNIDLHOIHIM = new ConditionExtension.CompareResult();
		QuestCondition kKDGLNECFHA = new QuestCondition();
		kKDGLNECFHA.LIMHBJBEEIA(PAJDEKLLFNJ);
		kKDGLNECFHA.MCPIOGALBMK(Type, lNIDLHOIHIM);
		LFLGCDNKNJI = lNIDLHOIHIM.ToString();
		lNIDLHOIHIM.Clear();
		kKDGLNECFHA.MCPIOGALBMK(Value, lNIDLHOIHIM);
		value = (long)lNIDLHOIHIM.resultNumber;
	}
}
