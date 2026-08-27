using System.Collections.Generic;
using System.Xml;

public class BarScale
{
	public List<Limit> PJGDPHKNCIG = new List<Limit>();

	public List<Limit> PBMLLNANNKA = new List<Limit>();

	public string Name;

	public string Type;

	public float MFGLDPKEDJB;

	public float DPGMCKCDMBC;

	public Limit EHKJEKAIDFF(int GNLOCMLBNHF)
	{
		return PBMLLNANNKA.Find((Limit DHDMNHCIPEH) => DHDMNHCIPEH.Levels.Contains(GNLOCMLBNHF));
	}

	public Limit NMMHOKHKFEE()
	{
		return PBMLLNANNKA.Find((Limit DHDMNHCIPEH) => DHDMNHCIPEH.Levels.Count == 0);
	}

	public Limit GPBFMLDPOKH(int GNLOCMLBNHF)
	{
		return PJGDPHKNCIG.Find((Limit DHDMNHCIPEH) => DHDMNHCIPEH.Levels.Contains(GNLOCMLBNHF));
	}

	public Limit IKEBHGKBGHO()
	{
		return PJGDPHKNCIG.Find((Limit DHDMNHCIPEH) => DHDMNHCIPEH.Levels.Count == 0);
	}

	public void LIJGBNNAMKK(XmlNode OEOOHNMCBOC, List<Limit> AJKECEDPPDC)
	{
		if (OEOOHNMCBOC == null)
		{
			return;
		}
		foreach (XmlNode childNode in OEOOHNMCBOC.ChildNodes)
		{
			Limit pEKGEPHFCMN = new Limit();
			XmlAttribute cJBEMNNNHDM = childNode.Attributes["LeftLimit"];
			XmlAttribute cJBEMNNNHDM2 = childNode.Attributes["RightLimit"];
			XmlAttribute xmlAttribute = childNode.Attributes["Level"];
			XmlAttribute cJBEMNNNHDM3 = childNode.Attributes["LevelMultiplier"];
			XmlAttribute cJBEMNNNHDM4 = childNode.Attributes["Shift"];
			pEKGEPHFCMN.OBGGBMDABAD = cJBEMNNNHDM.ParseInt(-1);
			pEKGEPHFCMN.NGPJDHKOEJC = cJBEMNNNHDM2.ParseInt(-1);
			if (xmlAttribute != null)
			{
				string text = xmlAttribute.CIPOICEEIBK();
				if (!string.IsNullOrEmpty(text))
				{
					string[] array = text.Split('|');
					string[] array2 = array;
					foreach (string s in array2)
					{
						int result;
						if (int.TryParse(s, out result))
						{
							pEKGEPHFCMN.Levels.Add(result);
						}
					}
				}
			}
			pEKGEPHFCMN.LevelMultiplier = cJBEMNNNHDM3.ParseFloat(-1f);
			pEKGEPHFCMN.Shift = cJBEMNNNHDM4.ParseInt(-1);
			AJKECEDPPDC.Add(pEKGEPHFCMN);
		}
	}
}
