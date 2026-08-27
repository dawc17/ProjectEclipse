using System.Collections.Generic;
using System.Xml;

public class ItemSets
{
	private List<ItemSet> NNDONLBFICM = new List<ItemSet>();

	public void Parse(XmlNode node)
	{
		foreach (XmlNode childNode in node.ChildNodes)
		{
			ItemSet item = new ItemSet(childNode);
			NNDONLBFICM.Add(item);
		}
	}

	public ItemSet JELBFNKKFFG(string OHCGEEEKEJH)
	{
		foreach (ItemSet item in NNDONLBFICM)
		{
			ItemSetItem bADHNGONFNC = item.HJNFOPNFFIJ(OHCGEEEKEJH);
			if (bADHNGONFNC != null)
			{
				return item;
			}
		}
		return null;
	}

	public ItemSet IGHHCHBEHOH(string JFGJBCGEGCN)
	{
		foreach (ItemSet item in NNDONLBFICM)
		{
			if (item.Name.Equals(JFGJBCGEGCN))
			{
				return item;
			}
		}
		return null;
	}
}
