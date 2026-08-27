using System.Collections.Generic;
using System.Xml;

public class BarScales
{
	public List<BarScale> JLIHKBCLKFH = new List<BarScale>();

	public void Parse(XmlNode node)
	{
		JLIHKBCLKFH.Clear();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			BarScale bABKPEHINKF = new BarScale();
			bABKPEHINKF.Name = childNode.Attributes["Name"].CIPOICEEIBK();
			XmlNode oEOOHNMCBOC = childNode["AttributeLimits"];
			bABKPEHINKF.LIJGBNNAMKK(oEOOHNMCBOC, bABKPEHINKF.PJGDPHKNCIG);
			XmlNode oEOOHNMCBOC2 = childNode["ItemLimits"];
			bABKPEHINKF.LIJGBNNAMKK(oEOOHNMCBOC2, bABKPEHINKF.PBMLLNANNKA);
			XmlAttribute cJBEMNNNHDM = childNode.Attributes["Power"];
			bABKPEHINKF.MFGLDPKEDJB = cJBEMNNNHDM.ParseFloat(-1f);
			XmlAttribute cJBEMNNNHDM2 = childNode.Attributes["Min"];
			bABKPEHINKF.DPGMCKCDMBC = cJBEMNNNHDM2.ParseFloat(-1f);
			XmlAttribute cJBEMNNNHDM3 = childNode.Attributes["Type"];
			bABKPEHINKF.Type = cJBEMNNNHDM3.CIPOICEEIBK();
			JLIHKBCLKFH.Add(bABKPEHINKF);
		}
	}

	public BarScale HNECOCDPENN(string name)
	{
		return JLIHKBCLKFH.Find((BarScale DHDMNHCIPEH) => DHDMNHCIPEH.Name.Equals(name));
	}
}
