using System.Collections.Generic;
using System.Xml;

public class Defense
{
	public string AOAPDHDACPJ;

	public float Weight;

	public string GMODDPGBGHM;

	public List<Evaluation> IBLHIAHECLK = new List<Evaluation>();

	public static int Parse(XmlNode BLLNKKNDNII, List<Defense> PNKJPOHEOJB)
	{
		int count = PNKJPOHEOJB.Count;
		foreach (XmlNode childNode in BLLNKKNDNII.ChildNodes)
		{
			if (childNode.Name == "Defense")
			{
				Defense dFKIEMBKKGA = new Defense();
				dFKIEMBKKGA.Parse(childNode);
				PNKJPOHEOJB.Add(dFKIEMBKKGA);
			}
		}
		return PNKJPOHEOJB.Count - count;
	}

	public void Parse(XmlNode MEEAKLDGLDF)
	{
		AOAPDHDACPJ = XmlUtils.ParseString(MEEAKLDGLDF.Attributes["Name"]);
		Weight = XmlUtils.ParseFloat(MEEAKLDGLDF.Attributes["Weight"]);
		GMODDPGBGHM = XmlUtils.ParseString(MEEAKLDGLDF.Attributes["CancellingItem"]);
		Evaluation.ParseAttributes(MEEAKLDGLDF, IBLHIAHECLK);
	}
}
