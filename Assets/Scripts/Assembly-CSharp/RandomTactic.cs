using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class RandomTactic
{
	public class EILEKOMGNOP
	{
		public string FGICHADOEHF;

		public int DPGMCKCDMBC;

		public int EBDBPJNBHGI;
	}

	public List<string> Intervals = new List<string>();

	public List<EILEKOMGNOP> IPCDOLMOJIF = new List<EILEKOMGNOP>();

	public float BeginnerCheat;

	public void Parse(XmlNode node)
	{
		IPCDOLMOJIF.Clear();
		XmlNode xmlNode = node["Intervals"];
		foreach (XmlNode childNode in xmlNode.ChildNodes)
		{
			Intervals.Add(childNode.Attributes["Name"].CIPOICEEIBK(string.Empty));
		}
		XmlNode xmlNode3 = node["Delays"];
		foreach (XmlNode childNode2 in xmlNode3.ChildNodes)
		{
			EILEKOMGNOP eILEKOMGNOP = new EILEKOMGNOP();
			eILEKOMGNOP.FGICHADOEHF = childNode2.Attributes["Animation"].CIPOICEEIBK(string.Empty);
			eILEKOMGNOP.DPGMCKCDMBC = childNode2.Attributes["Min"].ParseInt();
			eILEKOMGNOP.EBDBPJNBHGI = childNode2.Attributes["Max"].ParseInt();
			IPCDOLMOJIF.Add(eILEKOMGNOP);
		}
		XmlNode xmlNode5 = node["BeginnerCheat"];
		BeginnerCheat = xmlNode5.Attributes["Treshold"].ParseFloat();
	}

	public bool IsIntervalByName(string name)
	{
		return Intervals.Contains(name);
	}

	public int GetDelayByName(List<string> NIKHAICFGNM)
	{
		foreach (EILEKOMGNOP item in IPCDOLMOJIF)
		{
			foreach (string item2 in NIKHAICFGNM)
			{
				if (item.FGICHADOEHF == item2)
				{
					return Random.Range(item.DPGMCKCDMBC, item.EBDBPJNBHGI) + 1;
				}
			}
		}
		return 0;
	}
}
