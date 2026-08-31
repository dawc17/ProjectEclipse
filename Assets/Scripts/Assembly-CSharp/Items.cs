using System.Collections.Generic;
using System.Xml;
using SF2.Offline;

public class Items
{
	private string _fileName = "/list.xml";

	private List<ItemInfo> GMCMEPIKDOI = new List<ItemInfo>();

	private List<ItemInfo> HOOPHODHKPB = new List<ItemInfo>();

	private List<ItemInfo> CKONDOACLHG = new List<ItemInfo>();

	private List<ItemInfo> FJPBGMBAEFP = new List<ItemInfo>();

	private List<ItemInfo> CAANAADPJBF = new List<ItemInfo>();

	private List<ItemInfo> OMMJCLFJLIF = new List<ItemInfo>();

	private List<ItemInfo> OHPGLEFNBID = new List<ItemInfo>();

	private List<ItemInfo> POAPGJKKPMK = new List<ItemInfo>();

	private List<ItemInfo> EDHACKOGGAA = new List<ItemInfo>();

	private List<ItemInfo> DEEGAJNPJCI = new List<ItemInfo>();

	private List<UpgradeDataContainer> EJDLNCGFACO = new List<UpgradeDataContainer>();

	private ItemSets MPOPIPMCPOJ = new ItemSets();

	public List<ItemInfo> JIIFFJAJNNN
	{
		get
		{
			return MJKFCBMNNGJ();
		}
	}

	public List<ItemInfo> KGGEFNMBLDK
	{
		get
		{
			return MCGKNJPLIIH();
		}
	}

	public List<ItemInfo> NDJOHPHKJAD
	{
		get
		{
			return EKKIBLDGNHH();
		}
	}

	public List<ItemInfo> JLKLNBDHJDD
	{
		get
		{
			return LKGPBHADANE();
		}
	}

	public List<ItemInfo> MBANHMHBAJJ
	{
		get
		{
			return OGFOBKIEGKA();
		}
	}

	public List<ItemInfo> PICCOILOGOB
	{
		get
		{
			return DBGMLKGEJDD();
		}
	}

	public List<ItemInfo> EOHJMKFDBEI
	{
		get
		{
			return KCIHHGCHEKM();
		}
	}

	public List<ItemInfo> GLHNHHCHLHL
	{
		get
		{
			return BFFNOIPELKC();
		}
	}

	public List<ItemInfo> PJNFHNFLNNO
	{
		get
		{
			return KEFJPEOEPBN();
		}
	}

	public List<ItemInfo> KLJFJJJPPJJ
	{
		get
		{
			return HCDLKHKBEPF();
		}
	}

	public List<UpgradeDataContainer> CEKOFEFDMLJ
	{
		get
		{
			return CKCGBCNMOOP();
		}
	}

	public ItemSets KJPHFJLDMPC
	{
		get
		{
			return DGKMILIPLLF();
		}
	}

	private List<ItemInfo> HLMEPLOEPEL
	{
		get
		{
			return HKHHDDKGMIA();
		}
	}

	public int LIEGFJCKPLA
	{
		get
		{
			return EFEJPENECKN();
		}
	}

	public List<ItemInfo> MJKFCBMNNGJ()
	{
		return GMCMEPIKDOI;
	}

	public List<ItemInfo> MCGKNJPLIIH()
	{
		return HOOPHODHKPB;
	}

	public List<ItemInfo> EKKIBLDGNHH()
	{
		return CKONDOACLHG;
	}

	public List<ItemInfo> LKGPBHADANE()
	{
		return FJPBGMBAEFP;
	}

	public List<ItemInfo> OGFOBKIEGKA()
	{
		return CAANAADPJBF;
	}

	public List<ItemInfo> DBGMLKGEJDD()
	{
		return OMMJCLFJLIF;
	}

	public List<ItemInfo> KCIHHGCHEKM()
	{
		return OHPGLEFNBID;
	}

	public List<ItemInfo> BFFNOIPELKC()
	{
		return POAPGJKKPMK;
	}

	public List<ItemInfo> KEFJPEOEPBN()
	{
		return EDHACKOGGAA;
	}

	public List<ItemInfo> HCDLKHKBEPF()
	{
		return DEEGAJNPJCI;
	}

	public List<UpgradeDataContainer> CKCGBCNMOOP()
	{
		return EJDLNCGFACO;
	}

	public ItemSets DGKMILIPLLF()
	{
		return MPOPIPMCPOJ;
	}

	private List<ItemInfo> HKHHDDKGMIA()
	{
		List<ItemInfo> list = new List<ItemInfo>();
		foreach (ItemInfo item in DEEGAJNPJCI)
		{
			if (item.Type == "RealMoneyItem" && !string.IsNullOrEmpty(item.JLDEALIEEJI()))
			{
				list.Add(item);
			}
		}
		return list;
	}

	public int EFEJPENECKN()
	{
		int count = 0;
		DEEGAJNPJCI.ForEach((ItemInfo DHDMNHCIPEH) =>
		{
			if (!DHDMNHCIPEH.Type.Equals("Seal") && DHDMNHCIPEH.DBHJGAGOLOB())
			{
				count++;
			}
		});
		return count;
	}

	public int GetCountNewItemsByType(string LFLGCDNKNJI)
	{
		return DEEGAJNPJCI.FindAll((ItemInfo DHDMNHCIPEH) => DHDMNHCIPEH.Type.Equals(LFLGCDNKNJI) && DHDMNHCIPEH.DBHJGAGOLOB()).Count;
	}

	public ItemInfo KCCDBEEKBCG(string name)
	{
		ItemInfo item = DEEGAJNPJCI.Find((ItemInfo DHDMNHCIPEH) => DHDMNHCIPEH.Name.Equals(name));
		if (item != null) return item;
		Eclipse.Modding.DefinitionId id;
		Eclipse.Modding.ItemDefinition core;
		Eclipse.Modding.ModScriptSession scripts = Eclipse.Modding.ModRuntime.Scripts;
		if (scripts != null && Eclipse.Modding.DefinitionId.TryParse(name, out id) && id.Namespace.Value == "core" &&
			scripts.Content.TryGetItem(id, out core) && core.LegacyName != null)
		{
			ItemInfo legacy = DEEGAJNPJCI.Find(value => value.Name == core.LegacyName && value.NodeXML != null &&
				core.LegacyItemXml != null && value.NodeXML.OuterXml == core.LegacyItemXml);
			return legacy ?? DEEGAJNPJCI.Find(value => value.Name == core.LegacyName);
		}
		return null;
	}

	public List<ItemInfo> ONFMAJEAACM(string LFLGCDNKNJI)
	{
		switch (LFLGCDNKNJI)
		{
		case "Weapon":
			return MJKFCBMNNGJ();
		case "Armor":
			return MCGKNJPLIIH();
		case "Helm":
			return EKKIBLDGNHH();
		case "Ranged":
			return LKGPBHADANE();
		case "Magic":
			return OGFOBKIEGKA();
		case "RealMoneyItem":
			return KCIHHGCHEKM();
		case "Consumable":
			return BFFNOIPELKC();
		case "Free":
			return KEFJPEOEPBN();
		case "Seal":
			return DBGMLKGEJDD();
		default:
			return null;
		}
	}

	public List<ItemInfo> CKCMJAJAELO(string FDKNIPNGFNF)
	{
		List<ItemInfo> list = new List<ItemInfo>();
		int i = 0;
		for (int count = DEEGAJNPJCI.Count; i < count; i++)
		{
			if (DEEGAJNPJCI[i].JLDEALIEEJI() == FDKNIPNGFNF)
			{
				list.Add(DEEGAJNPJCI[i]);
			}
		}
		return list;
	}

	private ItemInfo HOBNJMONDKB(XmlNode node, int JDEHLOMDDOH)
	{
		ItemInfo dJKEECEOCJB = new ItemInfo(node);
		dJKEECEOCJB.NodeXML = node.CloneNode(true);
		XmlNode xmlNode = node["Upgrades"];
		if (xmlNode != null)
		{
			string lFLGCDNKNJI = xmlNode.Attributes["Template"].CIPOICEEIBK(string.Empty);
			foreach (XmlNode item in xmlNode)
			{
				UpgradeData iFOFMGAKHEP = new UpgradeData(item, lFLGCDNKNJI);
				dJKEECEOCJB.HNMFDILOBMJ(iFOFMGAKHEP);
			}
		}
		dJKEECEOCJB.GEEGNGNLPGO();
		dJKEECEOCJB.Index = JDEHLOMDDOH;
		return dJKEECEOCJB;
	}

	// Eclipse-owned mod content is validated before reaching this recovered container.
	// Keep the integration seam here deliberately tiny so the original item parser remains
	// authoritative for ItemInfo defaults, attributes and upgrade-template semantics.
	public ItemInfo AddExternalWeapon(XmlNode node)
	{
		if (node == null)
		{
			throw new System.ArgumentNullException("node");
		}
		XmlAttribute nameAttribute = node.Attributes["Name"];
		string name = (nameAttribute == null) ? string.Empty : nameAttribute.Value;
		if (string.IsNullOrEmpty(name))
		{
			throw new System.InvalidOperationException("External weapon requires a Name attribute.");
		}
		if (KCCDBEEKBCG(name) != null)
		{
			throw new System.InvalidOperationException("Item already exists: " + name);
		}

		ItemInfo item = HOBNJMONDKB(node, HCDLKHKBEPF().Count);
		if (item.Type != "Weapon")
		{
			throw new System.InvalidOperationException("External item seam currently accepts Weapon only: " + name);
		}
		HCDLKHKBEPF().Add(item);
		MJKFCBMNNGJ().Add(item);
		return item;
	}

	public bool RemoveExternalWeapon(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return false;
		}
		ItemInfo item = KCCDBEEKBCG(name);
		if (item == null || item.Type != "Weapon")
		{
			return false;
		}
		MJKFCBMNNGJ().Remove(item);
		HCDLKHKBEPF().Remove(item);
		return true;
	}

	private void ParseUpgradeList(XmlNode node)
	{
		foreach (XmlNode childNode in node.ChildNodes)
		{
			UpgradeDataContainer aKHJNNDCKMK = new UpgradeDataContainer();
			aKHJNNDCKMK.Type = childNode.Attributes["Name"].CIPOICEEIBK(string.Empty);
			foreach (XmlNode childNode2 in childNode.ChildNodes)
			{
				UpgradeData item = new UpgradeData(childNode2, aKHJNNDCKMK.Type);
				aKHJNNDCKMK.KPAPEBOAKIE.Add(item);
			}
			EJDLNCGFACO.Add(aKHJNNDCKMK);
		}
	}

	public UpgradeDataContainer BKPOCLGODDM(string LFLGCDNKNJI)
	{
		foreach (UpgradeDataContainer item in EJDLNCGFACO)
		{
			if (item.Type.Equals(LFLGCDNKNJI))
			{
				return item;
			}
		}
		return null;
	}

	public void NMMBHENGDJO(string path)
	{
		XmlDocument xmlDocument = null;
		xmlDocument = XmlUtils.OpenXMLDocument(path + _fileName, string.Empty, XmlUtils.EBLFEPIOMOL.Normal, true, XmlCryptoUtils.NNLGALNDJCL());
		if (xmlDocument == null)
		{
			LLLOJBFMONN.Error("Items.ParseItems xmlDocument == null");
			return;
		}
		XmlNode hKPPBKPJOEO = xmlDocument["List"]["UpgradeList"];
		ParseUpgradeList(hKPPBKPJOEO);
		XmlNode xmlNode = xmlDocument["List"]["Items"];
		int num = 0;
		foreach (XmlNode childNode in xmlNode.ChildNodes)
		{
			ItemInfo dJKEECEOCJB = HOBNJMONDKB(childNode, num);
			HCDLKHKBEPF().Add(dJKEECEOCJB);
			num++;
			switch (dJKEECEOCJB.Type)
			{
			case "Weapon":
				MJKFCBMNNGJ().Add(dJKEECEOCJB);
				break;
			case "Armor":
				MCGKNJPLIIH().Add(dJKEECEOCJB);
				break;
			case "Helm":
				EKKIBLDGNHH().Add(dJKEECEOCJB);
				break;
			case "Ranged":
				LKGPBHADANE().Add(dJKEECEOCJB);
				break;
			case "Magic":
				OGFOBKIEGKA().Add(dJKEECEOCJB);
				break;
			case "RealMoneyItem":
				KCIHHGCHEKM().Add(dJKEECEOCJB);
				break;
			case "Consumable":
				BFFNOIPELKC().Add(dJKEECEOCJB);
				break;
			case "Free":
				KEFJPEOEPBN().Add(dJKEECEOCJB);
				break;
			case "Seal":
				DBGMLKGEJDD().Add(dJKEECEOCJB);
				break;
			}
		}
		XmlNode hKPPBKPJOEO3 = xmlDocument["List"]["ItemSets"];
		DGKMILIPLLF().Parse(hKPPBKPJOEO3);
	}

	public void HGAKAILEHJO()
	{
		foreach (ItemInfo item in DEEGAJNPJCI)
		{
			if (item.DBHJGAGOLOB())
			{
				item.BEBDMOEIEJN(false);
			}
		}
	}

	public void SetNewAddItem(string OHCGEEEKEJH, bool value, int OMHDLKNHNMJ)
	{
		SetNewAddItem(KCCDBEEKBCG(OHCGEEEKEJH), value, OMHDLKNHNMJ);
	}

	public void SetNewAddItem(ItemInfo item, bool value, int OMHDLKNHNMJ)
	{
		bool flag = item.MMHIKEIDDNB == string.Empty || ListSF.CCDKHLAMKKO().FLFKOIPCEPI(item.MMHIKEIDDNB);
		bool flag2 = OMHDLKNHNMJ == item.MHGODOLNDLE;
		if (!item.GOKHJMOEGIJ() && flag && flag2)
		{
			item.BEBDMOEIEJN(value);
		}
	}

	public void MJICEAIDCGP(string EADBPKMABML)
	{
		List<ItemInfo> list = HCDLKHKBEPF();
		foreach (ItemInfo item in list)
		{
			if (item.DCHJDPCEODD && item.DBHJGAGOLOB() && item.MMHIKEIDDNB == EADBPKMABML)
			{
				item.BEBDMOEIEJN(false);
			}
		}
	}

	public ProductDefinition[] FOEGEPKLGJN()
	{
		List<ProductDefinition> list = new List<ProductDefinition>();
		HashSet<string> hashSet = new HashSet<string>();
		foreach (ItemInfo item in DEEGAJNPJCI)
		{
			if (item.Type == "RealMoneyItem" && !string.IsNullOrEmpty(item.JLDEALIEEJI()) && !hashSet.Contains(item.JLDEALIEEJI()))
			{
				hashSet.Add(item.JLDEALIEEJI());
				list.Add(new ProductDefinition(item.JLDEALIEEJI(), (!item.DFFFFIHOOKL()) ? ProductType.NonConsumable : ProductType.Consumable));
			}
		}
		return list.ToArray();
	}

	public void HAHLCEBCPLJ(Product[] OCMDJBDPLJK)
	{
		if (OCMDJBDPLJK != null && OCMDJBDPLJK.Length != 0)
		{
			List<ItemInfo> gBBJICINGDF = HKHHDDKGMIA();
			foreach (Product pANEMFIIOGB in OCMDJBDPLJK)
			{
				KGNJPJKFEEB(pANEMFIIOGB, gBBJICINGDF);
			}
		}
	}

	private void KGNJPJKFEEB(Product PANEMFIIOGB, List<ItemInfo> GBBJICINGDF)
	{
		foreach (ItemInfo item in GBBJICINGDF)
		{
			if (item.JLDEALIEEJI() == PANEMFIIOGB.definition.id)
			{
				item.IEIKLANLOPL(PANEMFIIOGB.metadata);
			}
		}
	}

	public void RandomizeObscuredVars()
	{
		HCDLKHKBEPF().ForEach((ItemInfo DHDMNHCIPEH) =>
		{
			DHDMNHCIPEH.RandomizeObscuredVars();
		});
		CKCGBCNMOOP().ForEach((UpgradeDataContainer DHDMNHCIPEH) =>
		{
			DHDMNHCIPEH.RandomizeObscuredVars();
		});
	}
}
