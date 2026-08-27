using System.Collections.Generic;
using System.Xml;

public class LevelThresholds
{
	public List<global::Pair<int, uint>> PEDIMBMABIG = new List<global::Pair<int, uint>>();

	public void Parse(XmlNode EBLIGDMALEA)
	{
		PEDIMBMABIG.Clear();
		foreach (XmlNode childNode in EBLIGDMALEA.ChildNodes)
		{
			int gBCLEDJAOBM = childNode.Attributes["Level"].ParseInt();
			uint pOFHDGJAFMP = childNode.Attributes["Exp"].ParseUint();
			PEDIMBMABIG.Add(new global::Pair<int, uint>(gBCLEDJAOBM, pOFHDGJAFMP));
		}
	}
}
