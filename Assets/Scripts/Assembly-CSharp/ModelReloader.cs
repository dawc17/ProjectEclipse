using System.Collections.Generic;
using System.Xml;

public static class ModelReloader
{
	public static void NPMIHDFCBBH(ModelObject ACENLMONNPA, List<string> CBHAEPCLDFG)
	{
		if (CBHAEPCLDFG.Count > 0)
		{
			List<XmlNode> iLCCDINCICK = new List<XmlNode>();
			if (Concat(ACENLMONNPA, iLCCDINCICK, CBHAEPCLDFG))
			{
				Parse(ACENLMONNPA, iLCCDINCICK);
			}
		}
	}

	private static bool Concat(ModelObject GIAMLEDNFJD, List<XmlNode> nodes, List<string> CBHAEPCLDFG)
	{
		int i = 0;
		for (int count = CBHAEPCLDFG.Count; i < count; i++)
		{
			XmlDocument xmlDocument = ModelLoader.FHGHPCACAKJ.JBJDPDOEGFO(SF2Paths.BNHLPKEDMOM(), CBHAEPCLDFG[i]);
			XmlNode xmlNode = ((xmlDocument == null) ? null : xmlDocument["Scene"]);
			XmlNode xmlNode2 = ((xmlNode == null) ? null : xmlNode["Nodes"]);
			if (xmlNode2 == null)
			{
				// Overlay model files are allowed to contain only figures, edges or
				// materials. They contribute no reset positions and must not add a null
				// Nodes entry to the reload pass (mdl_punching_bag is one such file).
				continue;
			}
			nodes.Add(xmlNode2);
		}
		return nodes.Count > 0;
	}

	private static void Parse(ModelObject ACENLMONNPA, List<XmlNode> nodes)
	{
		if (!PICNEPHDGGG(ACENLMONNPA, nodes))
		{
			LLLOJBFMONN.Error("Nodes was not parsed");
		}
	}

	private static bool PICNEPHDGGG(ModelObject ACENLMONNPA, List<XmlNode> BMGDKMNOLLL)
	{
		for (int i = 0; i < BMGDKMNOLLL.Count; i++)
		{
			foreach (XmlNode childNode in BMGDKMNOLLL[i].ChildNodes)
			{
				ModelNode modelNode = ACENLMONNPA.EGHIDHMENEF(childNode.Name);
				if (modelNode != null)
				{
					GLNMJNFLLIN(ACENLMONNPA, modelNode, childNode);
				}
			}
		}
		ACENLMONNPA.KJIEPFHIIKM();
		return true;
	}

	private static void GLNMJNFLLIN(ModelObject ACENLMONNPA, ModelNode NPDJNAMFIKD, XmlNode EABJIAHGLEO)
	{
		if (NPDJNAMFIKD.get_Name() != EABJIAHGLEO.Name)
		{
			LLLOJBFMONN.Error("Model reload: {0} -- {1}", NPDJNAMFIKD.get_Name(), EABJIAHGLEO.Name);
		}
		float lHNJJFDIJKK = EABJIAHGLEO.Attributes["X"].ParseFloat();
		float fFFHIOALHGM = 0f - EABJIAHGLEO.Attributes["Y"].ParseFloat();
		float pDCENMEKIAP = EABJIAHGLEO.Attributes["Z"].ParseFloat();
		Vector3f bAINMLLIKOL = new Vector3f(lHNJJFDIJKK, fFFHIOALHGM, pDCENMEKIAP);
		string text = EABJIAHGLEO.Attributes["Type"].CIPOICEEIBK();
		NPDJNAMFIKD.AMPCKAIPIHH(bAINMLLIKOL);
		NPDJNAMFIKD.LAHLFIKENPP(bAINMLLIKOL);
		if (text == "Node")
		{
			NPDJNAMFIKD.CNNKFMNKDNE(EABJIAHGLEO.Attributes["Cloth"].ParseBool());
			NPDJNAMFIKD.BDFIDDLGDNM(EABJIAHGLEO.Attributes["Attenuation"].ParseFloat());
		}
		NPDJNAMFIKD.NPKACGCHOLK(EABJIAHGLEO.Attributes["Mass"].ParseFloat());
		NPDJNAMFIKD.MGPLABIFCAH(EABJIAHGLEO.Attributes["Fixed"].ParseBool());
		NPDJNAMFIKD.NNHPOJFKEID(EABJIAHGLEO.Attributes["Visible"].ParseBool());
	}
}
