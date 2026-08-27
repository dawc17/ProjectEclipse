using System.Collections.Generic;
using System.Xml;

public class ModifiedAlignFormula
{
	public class DamageAttribute
	{
		public string Name;

		public float OPHGJJGKIHE;

		public float BPPJAMCGICA;

		public float GFHOHECBODM;

		public string KLAIAPBONFM;

		public DamageAttribute(XmlNode node)
		{
			Name = XmlUtils.ParseString(node.Attributes["Name"]);
			OPHGJJGKIHE = XmlUtils.ParseFloat(node.Attributes["DamageMultiplier"], 2f);
			BPPJAMCGICA = XmlUtils.ParseFloat(node.Attributes["NetDamage"], 0.1f);
			GFHOHECBODM = XmlUtils.ParseFloat(node.Attributes["MinAttributeDifference"]);
			KLAIAPBONFM = XmlUtils.ParseString(node.Attributes["ApplyTo"], "Player");
			if (KLAIAPBONFM != "Player" && KLAIAPBONFM != "Enemy")
			{
				KLAIAPBONFM = "Player";
			}
		}
	}

	private List<DamageAttribute> AJNCNCFDLKL = new List<DamageAttribute>();

	public void Parse(XmlNode node)
	{
		AJNCNCFDLKL.Clear();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			DamageAttribute item = new DamageAttribute(childNode);
			AJNCNCFDLKL.Add(item);
		}
	}

	public DamageAttribute NOADKFMGODA(string name)
	{
		for (int i = 0; i < AJNCNCFDLKL.Count; i++)
		{
			if (AJNCNCFDLKL[i].Name == name)
			{
				return AJNCNCFDLKL[i];
			}
		}
		return null;
	}
}
