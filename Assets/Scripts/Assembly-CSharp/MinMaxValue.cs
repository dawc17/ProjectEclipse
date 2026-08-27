using System.Xml;

public class MinMaxValue
{
	public float DPGMCKCDMBC;

	public float EBDBPJNBHGI;

	public MinMaxValue(float NOFALOKFBEM = 0f, float MFODOCNLNPH = 0f)
	{
		DPGMCKCDMBC = NOFALOKFBEM;
		EBDBPJNBHGI = MFODOCNLNPH;
	}

	public void Parse(XmlNode node, float PCEKHCGCHFH = 0f, float JCKIAGACDMA = 0f)
	{
		DPGMCKCDMBC = node.Attributes["Min"].ParseFloat(PCEKHCGCHFH);
		EBDBPJNBHGI = node.Attributes["Max"].ParseFloat(JCKIAGACDMA);
	}
}
