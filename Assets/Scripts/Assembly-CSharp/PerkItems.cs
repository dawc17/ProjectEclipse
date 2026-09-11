using System;
using System.Collections.Generic;
using System.Xml;

public class PerkItems
{
	private List<PerkInfoItem> BJHCPMLJOEK = new List<PerkInfoItem>();

	private List<PerkInfoItem> PAABAIILNEG = new List<PerkInfoItem>();

	private List<PerkInfoItem> NIOMJEOEMDL = new List<PerkInfoItem>();

	private HashSet<string> externalBasePerkNames = new HashSet<string>(StringComparer.Ordinal);
	private readonly HashSet<PerkInfoItem> externalUpgradeVariants = new HashSet<PerkInfoItem>();

	public void AddExternalPerkUpgrades(string name, XmlNode upgrades)
	{
		if (!externalBasePerkNames.Contains(name)) throw new InvalidOperationException("Upgrade owner is not an external perk: " + name);
		if (GAEHBOAPMLI(name).Count != 0) throw new InvalidOperationException("Progression variants already exist: " + name);
		PerkInfoItem original = ABAGJKMKCBA(name);
		var variants = new List<PerkInfoItem>();
		var baseline = original.Clone(null, null);
		baseline.AKKLOMFOLNO = 0;
		variants.Add(baseline);
		foreach (XmlNode node in upgrades.ChildNodes)
			if (node.Name == "UpgradeLevel") variants.Add(HDIPMKIGKDA(original, node));
		foreach (var variant in variants)
		{
			variant.GDCBBAHKCIE = false;
			PAABAIILNEG.Add(variant);
			externalUpgradeVariants.Add(variant);
		}
	}

	public List<PerkInfoItem> MHEJPIPKEFP
	{
		get
		{
			return CJJEPHDFOCJ();
		}
	}

	public List<PerkInfoItem> ICIFLAKCNBH
	{
		get
		{
			return GFPFNILGJML();
		}
	}

	public List<PerkInfoItem> FEPAABCBGGN
	{
		get
		{
			return BPBLIPKOJOP();
		}
	}

	public List<PerkInfoItem> CJJEPHDFOCJ()
	{
		return BJHCPMLJOEK;
	}

	public List<PerkInfoItem> GFPFNILGJML()
	{
		return PAABAIILNEG;
	}

	public List<PerkInfoItem> BPBLIPKOJOP()
	{
		return NIOMJEOEMDL;
	}

	public void Parse(XmlNode node)
	{
		BJHCPMLJOEK.Clear();
		externalBasePerkNames.Clear();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			PerkInfoItem aCONCDFDNJH = new PerkInfoItem();
			aCONCDFDNJH.Parse(childNode);
			aCONCDFDNJH.BGFEPJKDHFB = false;
			BJHCPMLJOEK.Add(aCONCDFDNJH);
		}
	}

	public PerkInfoItem AddExternalBasePerk(XmlNode node)
	{
		if (node == null)
		{
			throw new ArgumentNullException("node");
		}
		if (!node.Name.Equals("Perk", StringComparison.Ordinal))
		{
			throw new ArgumentException("External base perk node must be a complete Perk element.", "node");
		}
		string name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentException("External base perk requires a non-empty Name attribute.", "node");
		}
		if (ABAGJKMKCBA(name) != null)
		{
			throw new InvalidOperationException("Perk already exists: " + name);
		}

		PerkInfoItem perk = new PerkInfoItem();
		perk.Parse(node);
		perk.BGFEPJKDHFB = false;
		BJHCPMLJOEK.Add(perk);
		externalBasePerkNames.Add(name);
		return perk;
	}

	public bool RemoveExternalBasePerk(string name)
	{
		if (string.IsNullOrEmpty(name) || !externalBasePerkNames.Contains(name))
		{
			return false;
		}
		for (int i = 0; i < BJHCPMLJOEK.Count; i++)
		{
			if (BJHCPMLJOEK[i].Name.Equals(name, StringComparison.Ordinal))
			{
				BJHCPMLJOEK.RemoveAt(i);
				PAABAIILNEG.RemoveAll(perk => perk.Name == name && externalUpgradeVariants.Remove(perk));
				externalBasePerkNames.Remove(name);
				return true;
			}
		}
		return false;
	}

	public void NLLMCPOPFCI(XmlNode node)
	{
		PAABAIILNEG.Clear();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			string text = childNode.Attributes["Name"].CIPOICEEIBK(string.Empty);
			if (string.IsNullOrEmpty(text))
			{
				continue;
			}
			int gNLOCMLBNHF = childNode.Attributes["Level"].ParseInt();
			string eMDJGBHIAIA = childNode.Attributes["Description"].CIPOICEEIBK(string.Empty);
			string cMGIPKIPIPA = childNode.Attributes["Move"].CIPOICEEIBK(string.Empty);
			PerkInfoItem aCONCDFDNJH = ABAGJKMKCBA(text);
			if (aCONCDFDNJH == null)
			{
				continue;
			}
			if (childNode.ChildNodes.Count > 0)
			{
				PerkInfoItem aCONCDFDNJH2 = LEPHEFBINCL(aCONCDFDNJH, childNode);
				if (aCONCDFDNJH2 != aCONCDFDNJH)
				{
					PJIBJPOKIOC(aCONCDFDNJH2, gNLOCMLBNHF, eMDJGBHIAIA, cMGIPKIPIPA);
				}
				foreach (XmlNode childNode2 in childNode.ChildNodes)
				{
					if (childNode2.Name.Equals("UpgradeLevel"))
					{
						PerkInfoItem aEFFHJGMNFI = HDIPMKIGKDA(aCONCDFDNJH, childNode2);
						PJIBJPOKIOC(aEFFHJGMNFI, gNLOCMLBNHF, eMDJGBHIAIA, cMGIPKIPIPA);
					}
				}
			}
			else
			{
				PJIBJPOKIOC(aCONCDFDNJH, gNLOCMLBNHF, eMDJGBHIAIA, cMGIPKIPIPA);
			}
		}
	}

	public void MHAEANEADOO(XmlNode node, bool BBMAPFNKPBO = true, bool OPBAFPEJNNO = false)
	{
		if (BBMAPFNKPBO)
		{
			NIOMJEOEMDL.Clear();
		}
		foreach (XmlNode childNode in node.ChildNodes)
		{
			XmlNode xmlNode2 = childNode["Set"];
			XmlNode xmlNode3 = childNode["RatingEvaluation"];
			if (xmlNode2 == null && xmlNode3 == null)
			{
				continue;
			}
			string gOHIIMFFFJI = childNode.Attributes["Name"].CIPOICEEIBK(string.Empty);
			List<PerkInfoItem> list = GAEHBOAPMLI(gOHIIMFFFJI);
			foreach (PerkInfoItem item in list)
			{
				PerkInfoItem aCONCDFDNJH = item.Clone(xmlNode2, xmlNode3);
				if (OPBAFPEJNNO)
				{
					RemoveUsersPerkByName(aCONCDFDNJH.Name);
				}
				NIOMJEOEMDL.Add(aCONCDFDNJH);
			}
		}
	}

	private void RemoveUsersPerkByName(string name)
	{
		PerkInfoItem aCONCDFDNJH = NIOMJEOEMDL.Find((PerkInfoItem DHDMNHCIPEH) => DHDMNHCIPEH.Name.Equals(name));
		if (aCONCDFDNJH != null)
		{
			NIOMJEOEMDL.Remove(aCONCDFDNJH);
		}
	}

	private void PJIBJPOKIOC(PerkInfoItem AEFFHJGMNFI, int GNLOCMLBNHF, string EMDJGBHIAIA, string CMGIPKIPIPA)
	{
		if (!string.IsNullOrEmpty(EMDJGBHIAIA))
		{
			AEFFHJGMNFI.MGNNJPBCOGD = EMDJGBHIAIA;
		}
		AEFFHJGMNFI.GDCBBAHKCIE = false;
		AEFFHJGMNFI.Level = GNLOCMLBNHF;
		AEFFHJGMNFI.JNBECGKCNBB = CMGIPKIPIPA;
		PAABAIILNEG.Add(AEFFHJGMNFI);
	}

	private PerkInfoItem LEPHEFBINCL(PerkInfoItem AEFFHJGMNFI, XmlNode node)
	{
		PerkInfoItem result = AEFFHJGMNFI;
		XmlNode xmlNode = node["Set"];
		XmlNode xmlNode2 = node["RatingEvaluation"];
		if (xmlNode != null || xmlNode2 != null)
		{
			result = AEFFHJGMNFI.Clone(xmlNode, xmlNode2);
		}
		return result;
	}

	private PerkInfoItem HDIPMKIGKDA(PerkInfoItem AEFFHJGMNFI, XmlNode node)
	{
		PerkInfoItem aCONCDFDNJH = AEFFHJGMNFI.Clone(node["Set"], node["RatingEvaluation"]);
		string text = node.Attributes["Description"].CIPOICEEIBK(string.Empty);
		int aKKLOMFOLNO = node.Attributes["Value"].ParseInt();
		if (!string.IsNullOrEmpty(text))
		{
			aCONCDFDNJH.MGNNJPBCOGD = text;
		}
		aCONCDFDNJH.AKKLOMFOLNO = aKKLOMFOLNO;
		return aCONCDFDNJH;
	}

	public PerkInfoItem ABAGJKMKCBA(string name)
	{
		foreach (PerkInfoItem item in BJHCPMLJOEK)
		{
			if (item.Name.Equals(name))
			{
				return item;
			}
		}
		return null;
	}

	public PerkInfoItem MNMFPCBNLJI(string name)
	{
		return NIOMJEOEMDL.Find((PerkInfoItem DHDMNHCIPEH) => DHDMNHCIPEH.Name.Equals(name));
	}

	public PerkInfoItem LAAJJBEEDKL(string name, int upgradeLevel = -1)
	{
		return PAABAIILNEG.Find((PerkInfoItem DHDMNHCIPEH) =>
		{
			bool flag = DHDMNHCIPEH.Name.Equals(name);
			bool flag2 = upgradeLevel < 0 || DHDMNHCIPEH.AKKLOMFOLNO == upgradeLevel;
			return flag && flag2;
		});
	}

	public List<PerkInfoItem> GAEHBOAPMLI(string name)
	{
		return PAABAIILNEG.FindAll((PerkInfoItem DHDMNHCIPEH) => DHDMNHCIPEH.Name.Equals(name));
	}
}
