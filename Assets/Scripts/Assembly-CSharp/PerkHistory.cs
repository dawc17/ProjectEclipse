using System;
using System.Collections.Generic;
using System.Xml;

public class PerkHistory
{
	public class Perk : IComparable<Perk>
	{
		public string Name;

		public int Level;

		public Perk(string name, int GNLOCMLBNHF)
		{
			Name = name;
			Level = GNLOCMLBNHF;
		}

		public int CompareTo(Perk NOLFMPDGCOC)
		{
			return Level.CompareTo(NOLFMPDGCOC.Level);
		}
	}

	public List<Perk> JOGBKOJCINM = new List<Perk>();

	public void Parse(XmlNode node)
	{
		JOGBKOJCINM.Clear();
		if (node == null)
		{
			return;
		}
		foreach (XmlNode item in node)
		{
			string gOHIIMFFFJI = item.Attributes["Perk"].CIPOICEEIBK();
			int gNLOCMLBNHF = item.Attributes["Value"].ParseInt();
			JOGBKOJCINM.Add(new Perk(gOHIIMFFFJI, gNLOCMLBNHF));
		}
		JOGBKOJCINM.Sort();
	}

	public Perk GNIICEKAJKC(int GNLOCMLBNHF)
	{
		foreach (Perk item in JOGBKOJCINM)
		{
			if (item.Level == GNLOCMLBNHF)
			{
				return item;
			}
		}
		return null;
	}

	private bool IsExistPerkWithLevel(int GNLOCMLBNHF)
	{
		return GNIICEKAJKC(GNLOCMLBNHF) != null;
	}

	public Perk CBGCAPIMCFH(string name, int GNLOCMLBNHF)
	{
		if (IsExistPerkWithLevel(GNLOCMLBNHF) || name == string.Empty)
		{
			return null;
		}
		Perk hNHILOOIIMO = new Perk(name, GNLOCMLBNHF);
		JOGBKOJCINM.Add(hNHILOOIIMO);
		ListSF.CCDKHLAMKKO().JLBDOBLHHAF().PBPAOBKIMKK(hNHILOOIIMO);
		ListSF.ELEBLBJKDBI().EJANJEEGOOE();
		return hNHILOOIIMO;
	}
}
