using System.Collections.Generic;
using System.Xml;

public class ItemSet
{
	public string Name;

	public string Title;

	public string GGDJIPKMKFC;

	public string LHOJGHFGLFD;

	public List<ItemSetItem> OJIAKDDCGLB;

	public ItemSet(XmlNode node)
	{
		Name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		Title = node.Attributes["Title"].CIPOICEEIBK(string.Empty);
		GGDJIPKMKFC = node.Attributes["Text"].CIPOICEEIBK(string.Empty);
		LHOJGHFGLFD = node.Attributes["Brief"].CIPOICEEIBK(string.Empty);
		OJIAKDDCGLB = new List<ItemSetItem>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			ItemSetItem item = new ItemSetItem(childNode);
			OJIAKDDCGLB.Add(item);
		}
	}

	public ItemSetItem HJNFOPNFFIJ(string OHCGEEEKEJH)
	{
		foreach (ItemSetItem item in OJIAKDDCGLB)
		{
			if (item.Name.Equals(OHCGEEEKEJH))
			{
				return item;
			}
		}
		return null;
	}

	public bool BAOEOHJOIDK()
	{
		foreach (ItemSetItem item in OJIAKDDCGLB)
		{
			ItemInfo oFMCNLBFIDF = item.OFMCNLBFIDF;
			if (oFMCNLBFIDF == null)
			{
				return false;
			}
			UserItem dKCHDHMLKHN = ListSF.CCDKHLAMKKO().KHCNHPCPFII().CMGOCLGHNLH(oFMCNLBFIDF);
			if (dKCHDHMLKHN == null)
			{
				return false;
			}
		}
		return true;
	}
}
