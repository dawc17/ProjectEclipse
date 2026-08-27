using System.Collections.Generic;
using System.Xml;

public class CurrencyBaseValues
{
	public class CurrencyBaseValue
	{
		public string currencyName;

		private List<CharProgLevel> JINJHDABECD;

		public CurrencyBaseValue(XmlNode EBLIGDMALEA)
		{
			currencyName = EBLIGDMALEA.Attributes["Name"].CIPOICEEIBK(string.Empty);
			JINJHDABECD = new List<CharProgLevel>();
			foreach (XmlNode childNode in EBLIGDMALEA.ChildNodes)
			{
				if (childNode.Name == "Level")
				{
					CharProgLevel item = new CharProgLevel(childNode);
					JINJHDABECD.Add(item);
				}
			}
		}

		public float GetBaseValue(int OMHDLKNHNMJ)
		{
			foreach (CharProgLevel item in JINJHDABECD)
			{
				if (OMHDLKNHNMJ >= item.LHNCHOAEGEA && OMHDLKNHNMJ <= item.KAEPJHHLLPK)
				{
					return item.value;
				}
			}
			return 0f;
		}
	}

	public List<CurrencyBaseValue> PPJIHOCADPA = new List<CurrencyBaseValue>();

	public void Parse(XmlNode node)
	{
		PPJIHOCADPA.Clear();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.Name == "Currency")
			{
				CurrencyBaseValue item = new CurrencyBaseValue(childNode);
				PPJIHOCADPA.Add(item);
			}
		}
	}

	public float GetBaseValue(string currencyName)
	{
		foreach (CurrencyBaseValue item in PPJIHOCADPA)
		{
			if (item.currencyName == currencyName)
			{
				int oMHDLKNHNMJ = ListSF.CCDKHLAMKKO().PINDEKDNCNL();
				return item.GetBaseValue(oMHDLKNHNMJ);
			}
		}
		return 0f;
	}
}
