using System.Collections.Generic;
using System.Xml;

public class ShopOverrideConteyner
{
	private List<ShopOverride> OOBPFGKIPHK = new List<ShopOverride>();

	public void Parse(XmlNode node)
	{
		OOBPFGKIPHK.Clear();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			ShopOverride jHJPEFFBMFM = new ShopOverride();
			jHJPEFFBMFM.Type = childNode.Attributes["Type"].CIPOICEEIBK();
			jHJPEFFBMFM.JEAJJFEEOCL = childNode.Attributes["Screen"].CIPOICEEIBK();
			jHJPEFFBMFM.DAOMBPLCBMN = childNode.Attributes["Name"].CIPOICEEIBK();
			OOBPFGKIPHK.Add(jHJPEFFBMFM);
		}
	}

	public ShopOverride GetOverrideByScreen(string JPDNPODKKJP)
	{
		return OOBPFGKIPHK.Find((ShopOverride DHDMNHCIPEH) => DHDMNHCIPEH.JEAJJFEEOCL.Equals(JPDNPODKKJP));
	}
}
