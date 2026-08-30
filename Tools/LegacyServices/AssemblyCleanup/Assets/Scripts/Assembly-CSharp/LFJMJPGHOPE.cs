using System.Xml;

public struct LFJMJPGHOPE
{
	private int JCFMELKLOCN;

	private float OIOKBGAOBJC;

	private string FNPLBLBCCKB;

	private LFJMJPGHOPE(XmlNode node)
	{
		JCFMELKLOCN = node.Attributes["PosTo"].ParseInt();
		OIOKBGAOBJC = node.Attributes["PosRatingScaler"].ParseFloat();
		FNPLBLBCCKB = node.Attributes["LootContainerName"].CIPOICEEIBK(string.Empty);
	}

	private LFJMJPGHOPE(int FLGBCBIMBHA, float MBILMCOJJPM, string ACBNCCEPMHO)
	{
		JCFMELKLOCN = FLGBCBIMBHA;
		OIOKBGAOBJC = MBILMCOJJPM;
		FNPLBLBCCKB = ACBNCCEPMHO;
	}
}
