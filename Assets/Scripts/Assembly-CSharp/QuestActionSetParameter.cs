using System.Xml;

public class QuestActionSetParameter : QuestAction
{
	private class KJHEFADMIOA
	{
		public ConditionExtension.CompareResult OGCLLIKKLGN;

		public ConditionExtension.CompareResult GAPPCKCCBGO;

		public ConditionExtension.CompareResult CLEFMDPKIDK;
	}

	private string name;

	private string LECBBPAOJGF;

	private string value;

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		name = EPKLCPOEELO.Attributes["Name"].CIPOICEEIBK(string.Empty);
		LECBBPAOJGF = EPKLCPOEELO.Attributes["Parameter"].CIPOICEEIBK(string.Empty);
		value = EPKLCPOEELO.Attributes["Value"].CIPOICEEIBK(string.Empty);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		KJHEFADMIOA kJHEFADMIOA = new KJHEFADMIOA();
		QuestCondition kKDGLNECFHA = new QuestCondition();
		kKDGLNECFHA.LIMHBJBEEIA(GFIHPBCEEOB);
		kKDGLNECFHA.MCPIOGALBMK(name, kJHEFADMIOA.OGCLLIKKLGN);
		kKDGLNECFHA.MCPIOGALBMK(LECBBPAOJGF, kJHEFADMIOA.GAPPCKCCBGO);
		kKDGLNECFHA.MCPIOGALBMK(value, kJHEFADMIOA.CLEFMDPKIDK);
		KBNCBICGFEK(kJHEFADMIOA);
		OGIJONMKABB();
	}

	private void KBNCBICGFEK(KJHEFADMIOA DCJLKCFKCOM)
	{
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		UserItem dKCHDHMLKHN = nKGLHEGIKKP.KHCNHPCPFII().CMGOCLGHNLH(DCJLKCFKCOM.OGCLLIKKLGN.resultSTR);
		if (dKCHDHMLKHN != null)
		{
			string iBBAMMHHBFE = DCJLKCFKCOM.GAPPCKCCBGO.resultSTR;
			if (iBBAMMHHBFE.Equals("UpgradeLevel"))
			{
				dKCHDHMLKHN.FMMDLMGHPIB((int)DCJLKCFKCOM.CLEFMDPKIDK.resultNumber);
				dKCHDHMLKHN.CDFODJBJIPI(nKGLHEGIKKP.PINDEKDNCNL());
			}
		}
	}
}
