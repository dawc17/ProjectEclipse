using System.Collections.Generic;
using System.Xml;

public class UserItem
{
	private string _Name;

	private ItemInfo EOFKPLMPGLD;

	private bool NCDLPMFEEHG;

	private int DNIAOMIFPGD;

	private long _DeliveryTime;

	private int MLKADDDOCGH;

	private int IKNDJDEODFD;

	private XmlNode _Node;

	private bool JGPEOEDJMHH;

	private RecipeItemInfo LIONCGGHKLO;

	private ItemInfo ONNGDDFPBCA;

	private ItemInfo BIKNDEHPKGN;

	private bool CDFJMDKCAIL;

	private bool JLCNBIHFFEB;

	private bool KILMNBHEFGD;

	private bool GOPNLHKFOEG;

	private string BIOPPMKLLME;

	private List<PerkInfoItem> JCGBOOPPOLG = new List<PerkInfoItem>();

	public ItemInfo OFMCNLBFIDF
	{
		get
		{
			return BHKHOJPANHE();
		}
		set
		{
			KIGHKCOCJFJ(value);
		}
	}

	public bool BJKNOAGNBGM
	{
		get
		{
			return EFMFGEPDAOP();
		}
		set
		{
			JBLKCIBKMKB(value);
		}
	}

	public int Count
	{
		get
		{
			return OFOPFCJNEBL();
		}
		set
		{
			CHILOKHFALD(value);
		}
	}

	public long EHKNIKHPGDN
	{
		get
		{
			return IJGAOHJNLAH();
		}
		set
		{
			set_DeliveryTime(value);
		}
	}

	public int DIHHOAJJDAD
	{
		get
		{
			return EIMMBNNMBCN();
		}
		set
		{
			BAMLNLIDEBG(value);
		}
	}

	public int AKKLOMFOLNO
	{
		get
		{
			return DHNNCAEEMLL();
		}
		set
		{
			FMMDLMGHPIB(value);
		}
	}

	public XmlNode Node
	{
		get
		{
			return LIGMHKEOJBB();
		}
	}

	public bool FKPDJECLKDG
	{
		get
		{
			return FCKLNHEHDJE();
		}
	}

	public RecipeItemInfo LBOBJHNDLFD
	{
		get
		{
			return PHDBCIHJKON();
		}
	}

	public ItemInfo GAOFAAAIAAG
	{
		get
		{
			return AKKBIFEFDCI();
		}
	}

	public ItemInfo ICIJLBHLHIB
	{
		get
		{
			return HADDPFNDPDG();
		}
	}

	public bool MHHGNJHEFGL
	{
		get
		{
			return EPJAMDEFMFB();
		}
	}

	public bool INEOECGAGGD
	{
		get
		{
			return DBKKJGBJOEO();
		}
		set
		{
			IJCEKDCPBAG(value);
		}
	}

	public bool ICHDEOIBBNA
	{
		get
		{
			return JBCOAMLEBFG();
		}
	}

	public bool MHOGAHBFCHB
	{
		get
		{
			return CPBLPMAILGH();
		}
	}

	public bool IHIOPFHIPBK
	{
		get
		{
			return ALICFCFCCJG();
		}
	}

	public string GPNMNMNEPOP
	{
		get
		{
			return GAMAMIKGDKI();
		}
		set
		{
			HJONIDFKNJH(value);
		}
	}

	public List<PerkInfoItem> JAJNJAIJOPA
	{
		get
		{
			return IGACBNCNDBG();
		}
	}

	public bool OAKNAPGEBCD
	{
		get
		{
			return GKGIKMCMCPB();
		}
	}

	public UserItem(XmlNode EMOEJIOAKEG)
	{
		string gOHIIMFFFJI = EMOEJIOAKEG.Attributes["Name"].CIPOICEEIBK(string.Empty);
		int bLJGEOEHIGP = EMOEJIOAKEG.Attributes["Count"].ParseInt();
		int gNLOCMLBNHF = EMOEJIOAKEG.Attributes["UpgradeLevel"].ParseInt(-1);
		bool cBDBANOPFDM = EMOEJIOAKEG.Attributes["Equipped"].ParseBool();
		long bMNFPNBAMAF = EMOEJIOAKEG.Attributes["DeliveryTime"].ParseLong(0L);
		int gIPFIKDILKL = EMOEJIOAKEG.Attributes["DeliveryUpgradeLevel"].ParseInt(-1);
		string aFGFKAANGLL = EMOEJIOAKEG.Attributes["AcquireType"].CIPOICEEIBK("Item");
		if (EMOEJIOAKEG.Attributes["IsUpgrade"] != null)
		{
			bool flag = EMOEJIOAKEG.Attributes["IsUpgrade"].ParseBool();
			EMOEJIOAKEG.Attributes.RemoveNamedItem("IsUpgrade");
			aFGFKAANGLL = ((!flag) ? "Item" : "Upgrade");
		}
		Init(EMOEJIOAKEG, gOHIIMFFFJI, cBDBANOPFDM, bLJGEOEHIGP, gNLOCMLBNHF, bMNFPNBAMAF, gIPFIKDILKL, true, aFGFKAANGLL);
	}

	public UserItem(XmlNode FMBDAPOMFGN, string name, bool CBDBANOPFDM, int count, int GNLOCMLBNHF = -1, long time = 0L, int MDFLLEJODHJ = -1, bool KNGJACCPGPA = true, string AFGFKAANGLL = "Item")
	{
		XmlNode hKPPBKPJOEO = FMBDAPOMFGN.ACBPMPMPKJJ("Item");
		Init(hKPPBKPJOEO, name, CBDBANOPFDM, count, GNLOCMLBNHF, time, MDFLLEJODHJ, KNGJACCPGPA, AFGFKAANGLL);
	}

	public UserItem(ItemInfo PJDAGCBPLJE, bool CBDBANOPFDM, int count, int GNLOCMLBNHF = -1, long time = 0L, int MDFLLEJODHJ = -1, bool KNGJACCPGPA = true, string AFGFKAANGLL = "Item")
	{
		if (ListSF.CCDKHLAMKKO().BABKABBEFEL() != null)
		{
			XmlNode hKPPBKPJOEO = ListSF.CCDKHLAMKKO().BABKABBEFEL().ACBPMPMPKJJ("Item");
			Init(hKPPBKPJOEO, PJDAGCBPLJE.Name, CBDBANOPFDM, count, GNLOCMLBNHF, time, MDFLLEJODHJ, KNGJACCPGPA, AFGFKAANGLL);
		}
	}

	public string get_Name()
	{
		return _Name;
	}

	public void set_Name(string value)
	{
		_Name = value;
		if (JGPEOEDJMHH)
		{
			if (_Node.Attributes["Name"] == null)
			{
				_Node.LLIKNHNLGJJ("Name");
			}
			_Node.Attributes["Name"].Value = value;
		}
	}

	public ItemInfo BHKHOJPANHE()
	{
		return EOFKPLMPGLD;
	}

	public void KIGHKCOCJFJ(ItemInfo value)
	{
		EOFKPLMPGLD = value;
		EOFKPLMPGLD.DCHJDPCEODD = true;
		if (IKNDJDEODFD == -1)
		{
			FMMDLMGHPIB(EOFKPLMPGLD.OBJDGBBFJOO);
		}
		else
		{
			FMMDLMGHPIB(IKNDJDEODFD);
		}
	}

	public bool EFMFGEPDAOP()
	{
		return NCDLPMFEEHG;
	}

	public void JBLKCIBKMKB(bool value)
	{
		NCDLPMFEEHG = value;
		if (JGPEOEDJMHH)
		{
			if (_Node.Attributes["Equipped"] == null)
			{
				_Node.LLIKNHNLGJJ("Equipped");
			}
			_Node.Attributes["Equipped"].Value = ((!value) ? "0" : "1");
		}
	}

	public int OFOPFCJNEBL()
	{
		return DNIAOMIFPGD;
	}

	public void CHILOKHFALD(int value)
	{
		DNIAOMIFPGD = value;
		if (JGPEOEDJMHH)
		{
			if (_Node.Attributes["Count"] == null)
			{
				_Node.LLIKNHNLGJJ("Count");
			}
			_Node.Attributes["Count"].Value = value.ToString();
		}
	}

	public long IJGAOHJNLAH()
	{
		return _DeliveryTime;
	}

	public void set_DeliveryTime(long value)
	{
		_DeliveryTime = value;
		if (JGPEOEDJMHH)
		{
			if (_Node.Attributes["DeliveryTime"] == null)
			{
				_Node.LLIKNHNLGJJ("DeliveryTime");
			}
			_Node.Attributes["DeliveryTime"].Value = value.ToString();
		}
	}

	public int EIMMBNNMBCN()
	{
		return MLKADDDOCGH;
	}

	public void BAMLNLIDEBG(int value)
	{
		MLKADDDOCGH = value;
		if (JGPEOEDJMHH)
		{
			if (_Node.Attributes["DeliveryUpgradeLevel"] == null)
			{
				_Node.LLIKNHNLGJJ("DeliveryUpgradeLevel");
			}
			_Node.Attributes["DeliveryUpgradeLevel"].Value = value.ToString();
		}
	}

	public int DHNNCAEEMLL()
	{
		return IKNDJDEODFD;
	}

	public void FMMDLMGHPIB(int value)
	{
		IKNDJDEODFD = value;
		if (JGPEOEDJMHH)
		{
			if (_Node.Attributes["UpgradeLevel"] == null)
			{
				_Node.LLIKNHNLGJJ("UpgradeLevel");
			}
			_Node.Attributes["UpgradeLevel"].Value = value.ToString();
		}
	}

	public XmlNode LIGMHKEOJBB()
	{
		return _Node;
	}

	public bool FCKLNHEHDJE()
	{
		return JGPEOEDJMHH;
	}

	public RecipeItemInfo PHDBCIHJKON()
	{
		return LIONCGGHKLO;
	}

	public ItemInfo AKKBIFEFDCI()
	{
		if (ONNGDDFPBCA == null)
		{
			return EOFKPLMPGLD;
		}
		return ONNGDDFPBCA;
	}

	public ItemInfo HADDPFNDPDG()
	{
		return BIKNDEHPKGN;
	}

	public bool EPJAMDEFMFB()
	{
		return CDFJMDKCAIL;
	}

	public bool DBKKJGBJOEO()
	{
		return GAMAMIKGDKI().Equals("Upgrade");
	}

	public void IJCEKDCPBAG(bool value)
	{
		string bAINMLLIKOL = ((!value) ? "Item" : "Upgrade");
		HJONIDFKNJH(bAINMLLIKOL);
	}

	public bool JBCOAMLEBFG()
	{
		return JLCNBIHFFEB;
	}

	public bool CPBLPMAILGH()
	{
		return KILMNBHEFGD;
	}

	public bool ALICFCFCCJG()
	{
		return GOPNLHKFOEG;
	}

	public string GAMAMIKGDKI()
	{
		return BIOPPMKLLME;
	}

	public void HJONIDFKNJH(string value)
	{
		BIOPPMKLLME = value;
		if (JGPEOEDJMHH)
		{
			if (_Node.Attributes["AcquireType"] == null)
			{
				_Node.LLIKNHNLGJJ("AcquireType");
			}
			_Node.Attributes["AcquireType"].Value = BIOPPMKLLME;
		}
	}

	public List<PerkInfoItem> IGACBNCNDBG()
	{
		return JCGBOOPPOLG;
	}

	public bool GKGIKMCMCPB()
	{
		return OFOPFCJNEBL() > 0;
	}

	private void Init(XmlNode node, string name, bool CBDBANOPFDM, int count, int GNLOCMLBNHF, long BMNFPNBAMAF, int GIPFIKDILKL, bool KNGJACCPGPA, string AFGFKAANGLL)
	{
		_Node = node;
		JGPEOEDJMHH = KNGJACCPGPA;
		EOFKPLMPGLD = null;
		LIONCGGHKLO = null;
		ONNGDDFPBCA = null;
		BIKNDEHPKGN = null;
		CDFJMDKCAIL = false;
		JLCNBIHFFEB = false;
		KILMNBHEFGD = false;
		set_Name(name);
		JBLKCIBKMKB(CBDBANOPFDM);
		CHILOKHFALD(count);
		FMMDLMGHPIB(GNLOCMLBNHF);
		set_DeliveryTime(BMNFPNBAMAF);
		BAMLNLIDEBG(GIPFIKDILKL);
		HJONIDFKNJH(AFGFKAANGLL);
		if (node["Enchantments"] != null)
		{
			CHBPLGEDGAC(node["Enchantments"]);
		}
		if (node["RecipeDelivery"] != null)
		{
			OIAMNHOMACJ(node["RecipeDelivery"]);
		}
	}

	private void OIAMNHOMACJ(XmlNode node)
	{
		LIONCGGHKLO = new RecipeItemInfo(node, this);
	}

	private void CHBPLGEDGAC(XmlNode node)
	{
		LEFIBJHJAOD();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			PerkInfoItem aCONCDFDNJH = ItemInfo.APPAODDDDKI(childNode);
			if (aCONCDFDNJH != null)
			{
				JCGBOOPPOLG.Add(aCONCDFDNJH);
			}
		}
	}

	private void LEFIBJHJAOD(bool KBLMKFKJHCE = true, bool removeNodes = false)
	{
		List<PerkInfoItem> list = new List<PerkInfoItem>();
		foreach (PerkInfoItem item in JCGBOOPPOLG)
		{
			if (item.LELHEEDNMBP != PerkInfoItem.DNPGIEGCGKH.COMBO || KBLMKFKJHCE)
			{
				list.Add(item);
			}
		}
		list.ForEach((PerkInfoItem DHDMNHCIPEH) =>
		{
			JCGBOOPPOLG.Remove(DHDMNHCIPEH);
			if (removeNodes)
			{
				XmlNode xmlNode = _Node["Enchantments"];
				XmlNode oldChild = xmlNode.LJGLMGNAFHJ("Perk", "Name", DHDMNHCIPEH.Name);
				xmlNode.RemoveChild(oldChild);
			}
		});
	}

	public void CDFODJBJIPI(int OMHDLKNHNMJ)
	{
		ONNGDDFPBCA = null;
		BIKNDEHPKGN = null;
		CDFJMDKCAIL = false;
		JLCNBIHFFEB = false;
		KILMNBHEFGD = false;
		GOPNLHKFOEG = false;
		if (EOFKPLMPGLD != null)
		{
			EOFKPLMPGLD.NHJAHNDOLAE(OMHDLKNHNMJ, IKNDJDEODFD, ref ONNGDDFPBCA, ref BIKNDEHPKGN);
			CDFJMDKCAIL = EOFKPLMPGLD.DNFDAGFAANJ().Count > 0;
			JLCNBIHFFEB = CDFJMDKCAIL && ONNGDDFPBCA != null;
			KILMNBHEFGD = CDFJMDKCAIL && BIKNDEHPKGN == null;
			GOPNLHKFOEG = !KILMNBHEFGD && BIKNDEHPKGN != null;
		}
	}

	public ItemInfo DBLCMCEGJGI(bool EECHKLPPCKH)
	{
		ItemInfo dJKEECEOCJB = null;
		if (EECHKLPPCKH && EPJAMDEFMFB() && GKGIKMCMCPB())
		{
			dJKEECEOCJB = HADDPFNDPDG();
		}
		if (dJKEECEOCJB == null && DBKKJGBJOEO())
		{
			dJKEECEOCJB = AKKBIFEFDCI();
		}
		return (dJKEECEOCJB == null) ? BHKHOJPANHE() : dJKEECEOCJB;
	}

	public ItemInfo AENKEPCBHJG()
	{
		ItemInfo dJKEECEOCJB = null;
		if (EIMMBNNMBCN() > 0)
		{
			dJKEECEOCJB = EOFKPLMPGLD.ILDOPPMOOOF(EIMMBNNMBCN());
		}
		return (dJKEECEOCJB == null) ? DBLCMCEGJGI(ListSF.CCDKHLAMKKO().HFINDOBJHNK()) : dJKEECEOCJB;
	}

	public void PJEEGECBHMH()
	{
		int mHNCENBCECJ = ListSF.CCDKHLAMKKO().PINDEKDNCNL();
		int mHGODOLNDLE = EOFKPLMPGLD.MHGODOLNDLE;
		GDBFNNLHPOB(EOFKPLMPGLD.APMJCGBNEDI, mHGODOLNDLE, mHNCENBCECJ);
	}

	public void GDBFNNLHPOB(List<PerkStruct> HALHGEGADKA, int MPAGFAKIEJG, int MHNCENBCECJ)
	{
		if (!JGPEOEDJMHH)
		{
			return;
		}
		bool flag = false;
		bool flag2 = false;
		foreach (PerkStruct item in HALHGEGADKA)
		{
			PerkInfoItem aCONCDFDNJH = GameUtils.FDEJIIDIPBI.ABAGJKMKCBA(item.get_Name());
			if (aCONCDFDNJH != null)
			{
				if (aCONCDFDNJH.LELHEEDNMBP == PerkInfoItem.DNPGIEGCGKH.SINGLE)
				{
					flag = true;
				}
				if (aCONCDFDNJH.LELHEEDNMBP == PerkInfoItem.DNPGIEGCGKH.COMBO)
				{
					flag2 = true;
				}
			}
		}
		if (flag)
		{
			JNGJKFLJCML();
		}
		if (flag2)
		{
			AMCMLDINIOM();
		}
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		nKGLHEGIKKP.MMIMAJCKFKL = MHNCENBCECJ;
		nKGLHEGIKKP.CLODDOOGDBB = true;
		if (HALHGEGADKA.Count > 0)
		{
			XmlNode mEEAKLDGLDF = ((_Node["Enchantments"] != null) ? _Node["Enchantments"] : _Node.ACBPMPMPKJJ("Enchantments"));
			foreach (PerkStruct item2 in HALHGEGADKA)
			{
				XmlNode xmlNode = mEEAKLDGLDF.KDPLHGGPJHN("Perk");
				xmlNode.LLIKNHNLGJJ("Name").Value = item2.get_Name();
				if (item2.EOLPAHGCMHH().Count > 0)
				{
					XmlNode mEEAKLDGLDF2 = xmlNode.KDPLHGGPJHN("Set");
					PerkStruct jLFJOECODOF = new PerkStruct(item2);
					jLFJOECODOF.MLONLJGHDEA();
					foreach (KeyValuePair<string, string> item3 in jLFJOECODOF.EOLPAHGCMHH())
					{
						mEEAKLDGLDF2.LLIKNHNLGJJ(item3.Key).Value = item3.Value;
					}
				}
				PerkInfoItem aCONCDFDNJH2 = ItemInfo.APPAODDDDKI(xmlNode);
				if (aCONCDFDNJH2 != null)
				{
					JCGBOOPPOLG.Add(aCONCDFDNJH2);
				}
			}
		}
		nKGLHEGIKKP.CLODDOOGDBB = false;
	}

	private void JNGJKFLJCML()
	{
		int num = 0;
		while (num < JCGBOOPPOLG.Count)
		{
			PerkInfoItem aCONCDFDNJH = JCGBOOPPOLG[num];
			if (aCONCDFDNJH.LELHEEDNMBP != PerkInfoItem.DNPGIEGCGKH.COMBO)
			{
				JCGBOOPPOLG.Remove(aCONCDFDNJH);
				XmlNode xmlNode = _Node["Enchantments"];
				XmlNode oldChild = xmlNode.LJGLMGNAFHJ("Perk", "Name", aCONCDFDNJH.Name);
				xmlNode.RemoveChild(oldChild);
			}
			else
			{
				num++;
			}
		}
	}

	private void AMCMLDINIOM()
	{
		int num = 0;
		while (num < JCGBOOPPOLG.Count)
		{
			PerkInfoItem aCONCDFDNJH = JCGBOOPPOLG[num];
			if (aCONCDFDNJH.LELHEEDNMBP == PerkInfoItem.DNPGIEGCGKH.COMBO)
			{
				JCGBOOPPOLG.Remove(aCONCDFDNJH);
				XmlNode xmlNode = _Node["Enchantments"];
				XmlNode oldChild = xmlNode.LJGLMGNAFHJ("Perk", "Name", aCONCDFDNJH.Name);
				xmlNode.RemoveChild(oldChild);
			}
			else
			{
				num++;
			}
		}
	}

	public bool OJNNHFNPNEM(PerkInfoItem AEFFHJGMNFI)
	{
		for (int i = 0; i < JCGBOOPPOLG.Count; i++)
		{
			if (JCGBOOPPOLG[i].Name == AEFFHJGMNFI.Name)
			{
				return true;
			}
		}
		return false;
	}

	public void CFIDFHLBKGP(List<PerkStruct> HALHGEGADKA, int MPAGFAKIEJG, int MHNCENBCECJ)
	{
		DPDHAOPJFHD(HALHGEGADKA);
		GDBFNNLHPOB(HALHGEGADKA, MPAGFAKIEJG, MHNCENBCECJ);
	}

	private void DPDHAOPJFHD(List<PerkStruct> NIBJKBMNOKG)
	{
		XmlNode xmlNode = _Node["Enchantments"];
		for (int i = 0; i < NIBJKBMNOKG.Count; i++)
		{
			PerkStruct jLFJOECODOF = NIBJKBMNOKG[i];
			foreach (PerkInfoItem item in JCGBOOPPOLG)
			{
				if (JCGBOOPPOLG[i].Name == jLFJOECODOF.get_Name())
				{
					JCGBOOPPOLG.Remove(item);
					break;
				}
			}
			XmlNode xmlNode2 = xmlNode.LJGLMGNAFHJ("Perk", "Name", jLFJOECODOF.get_Name());
			if (xmlNode2 != null)
			{
				xmlNode.RemoveChild(xmlNode2);
			}
		}
	}
}
