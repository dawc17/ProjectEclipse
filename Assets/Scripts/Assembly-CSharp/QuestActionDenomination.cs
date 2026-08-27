using System.Xml;

public class QuestActionDenomination : QuestAction
{
	private int CFNNEGHPCMN;

	private string NBBNANIILBL = "MiscSprites.gold";

	public override void Parse(XmlNode EPKLCPOEELO)
	{
		base.Parse(EPKLCPOEELO);
		CFNNEGHPCMN = EPKLCPOEELO.Attributes["DenominationDigits"].ParseInt(-1);
		NBBNANIILBL = EPKLCPOEELO.Attributes["CoinIcon"].CIPOICEEIBK(string.Empty);
	}

	public override void DEJMHFMLKIC(QuestParameters GFIHPBCEEOB)
	{
		base.DEJMHFMLKIC(GFIHPBCEEOB);
		int nPFOBKBJAOB = ListSF.CCDKHLAMKKO().NPGECMDDNFO();
		ListSF.CCDKHLAMKKO().KGEHCFADNLI(CFNNEGHPCMN);
		ListSF.CCDKHLAMKKO().HEIPPEGBOCK(NBBNANIILBL);
		JEGCABAHHHJ(nPFOBKBJAOB);
		MenuController.OPPMFDNNBDE();
		ScreenType iPKNDMINFMJ = Module.ELEBLBJKDBI().DMCJGOMOJEF.ScreenType;
		if (iPKNDMINFMJ != ScreenType.ModuleFight)
		{
			Module.DLOKJOHNDID(iPKNDMINFMJ);
		}
		ListSF.ELEBLBJKDBI().OnAuthenticate(true);
		OGIJONMKABB();
	}

	private void JEGCABAHHHJ(int NPFOBKBJAOB)
	{
		ItemInfo.DenominateItems(NPFOBKBJAOB);
		ListSF.CCDKHLAMKKO().FHCPEIGMGMK(NPFOBKBJAOB);
	}
}
