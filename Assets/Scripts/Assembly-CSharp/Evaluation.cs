using System.Collections.Generic;
using System.Xml;

public class Evaluation
{
	public string Name;

	public float Shift;

	public static int ParseAttributes(XmlNode BLLNKKNDNII, List<Evaluation> PNKJPOHEOJB)
	{
		return Parse(BLLNKKNDNII, PNKJPOHEOJB, "Attribute");
	}

	public static float BEFJGPEAJCF(XmlNode BLLNKKNDNII, RatingEvaluation PNKJPOHEOJB)
	{
		PNKJPOHEOJB.BEAGNAOOHBP = XmlUtils.ParseFloat(BLLNKKNDNII.Attributes["AverageQuantity"]);
		return PNKJPOHEOJB.BEAGNAOOHBP;
	}

	public static float MNFPBOPIHDE(XmlNode BLLNKKNDNII, RatingEvaluation PNKJPOHEOJB)
	{
		PNKJPOHEOJB.CDCIEOFCKNO = XmlUtils.ParseFloat(BLLNKKNDNII.Attributes["AverageBaseDamage"]);
		PNKJPOHEOJB.GCOFCDFHMGL = XmlUtils.ParseFloat(BLLNKKNDNII.Attributes["RechargeRate"]);
		PNKJPOHEOJB.OFHGAJDLIDB = XmlUtils.ParseFloat(BLLNKKNDNII.Attributes["MagicRechargeRate"]);
		return PNKJPOHEOJB.CDCIEOFCKNO;
	}

	private void Parse(XmlNode MEEAKLDGLDF)
	{
		Name = XmlUtils.ParseString(MEEAKLDGLDF.Attributes["Name"]);
		Shift = XmlUtils.ParseFloat(MEEAKLDGLDF.Attributes["Shift"]);
	}

	private static int Parse(XmlNode BLLNKKNDNII, List<Evaluation> PNKJPOHEOJB, string JLEKBBJBLOE)
	{
		int count = PNKJPOHEOJB.Count;
		foreach (XmlNode childNode in BLLNKKNDNII.ChildNodes)
		{
			if (childNode.Name == JLEKBBJBLOE)
			{
				Evaluation bAEELPHJKBD = new Evaluation();
				bAEELPHJKBD.Parse(childNode);
				PNKJPOHEOJB.Add(bAEELPHJKBD);
			}
		}
		return PNKJPOHEOJB.Count - count;
	}
}
