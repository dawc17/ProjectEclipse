using System.Collections.Generic;
using System.Xml;

public class MoneyBaseValues
{
	private List<CharProgLevel> JINJHDABECD = new List<CharProgLevel>();

	public void Parse(XmlNode node)
	{
		JINJHDABECD.Clear();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.Name == "Level")
			{
				CharProgLevel item = new CharProgLevel(childNode);
				JINJHDABECD.Add(item);
			}
		}
	}

	public long GetBaseValue(int OMHDLKNHNMJ)
	{
		foreach (CharProgLevel item in JINJHDABECD)
		{
			if (OMHDLKNHNMJ >= item.LHNCHOAEGEA && OMHDLKNHNMJ <= item.KAEPJHHLLPK)
			{
				return item.value;
			}
		}
		return 0L;
	}
}
