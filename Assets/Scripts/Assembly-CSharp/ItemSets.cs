using System.Collections.Generic;
using System.Xml;

public class ItemSets
{
	private List<ItemSet> NNDONLBFICM = new List<ItemSet>();

	public IReadOnlyList<ItemSet> Sets => NNDONLBFICM.AsReadOnly();

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

	public ItemSet AddExternalSet(XmlNode node)
	{
		if (node == null) throw new System.ArgumentNullException("node");
		string name = node.Attributes?["Name"]?.Value ?? string.Empty;
		if (string.IsNullOrEmpty(name)) throw new System.InvalidOperationException("External item set requires a Name attribute.");
		if (IGHHCHBEHOH(name) != null) throw new System.InvalidOperationException("Item set already exists: " + name);
		ItemSet itemSet = new ItemSet(node);
		if (itemSet.OJIAKDDCGLB == null || itemSet.OJIAKDDCGLB.Count == 0)
			throw new System.InvalidOperationException("External item set requires at least one member: " + name);
		NNDONLBFICM.Add(itemSet);
		return itemSet;
	}

	public bool RemoveExternalSet(string name)
	{
		if (string.IsNullOrEmpty(name)) return false;
		ItemSet itemSet = IGHHCHBEHOH(name);
		return itemSet != null && NNDONLBFICM.Remove(itemSet);
	}
}
