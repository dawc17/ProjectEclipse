using System.Collections.Generic;
using System.Xml;
using CodeStage.AntiCheat.ObscuredTypes;
using SF2.Offline;

public class ItemInfo
{
	public enum MEFIBHIDOLA
	{
		SPEND_TYPE_NONE = 0,
		SPEND_TYPE_ENERGY = 1
	}

	public enum HAIJKHDIFBC
	{
		DISCOUNT_NONE = 0,
		DISCOUNT_QUEST = 1,
		DISCOUNT_CONFIG = 2
	}

	public const string HMNIGMPCKFE = "NoneItem";

	public const string PKFHJMIEKFG = "Skeleton";

	public const string NKCPIOAGNAE = "Weapon";

	public const string PNMMEPBMMOC = "Armor";

	public const string OGPJODJEGLM = "Helm";

	public const string BIJMKPGBHMO = "Ranged";

	public const string NFLIMNFMJJI = "Magic";

	public const string BDFIIAGIKHM = "RealMoneyItem";

	public const string HNLBLBNJMPN = "Energy";

	public const string HEBJDAMBIME = "Dummy";

	public const string LEAFPMKEIMF = "Decorate";

	public const string FFFKBNMHDOP = "Cheat";

	public const string DPKFGDKOCMA = "Seal";

	public const string AOHPCDBCLBI = "Free";

	public const string DONHFCBOKKC = "Profile";

	public const string DGAMEBKKNPP = "Recipe";

	public const string HDNONAPIKGK = "Consumable";

	public const string KHOKCIKCNKA = "RaidConsumable";

	public const string OGKHEJBOPAH = "RaidItemPack";

	public const string CKLEEDONENO = "Gold";

	public const string DNBMMGPOBPC = "Bonus";

	public const string GEPKNMNBICG = "UnlimitedEnergy";

	public const string LDJPGMNAKFA = "StarterPack";

	public const string AMBOPJLFKJG = "TapJoy";

	public const string JMOCLCKLIBA = "SponsorPay";

	public const string LMNJLACPFJP = "Metaps";

	public const string PMIDDMDFEBM = "Video";

	public const string MDJNCBFJCJG = "Facebook_Like";

	public const string AMPCCNCIIFN = "PerkReset";

	public const string DLIHMBADCEO = "Currency";

	public const string JOGIHBIANNF = "RaidCurrency";

	public const string ICICDOHDFCB = "RaidCharge";

	public const string HHGPLLMEONG = "RaidPotion";

	public const string EEFPBKDFKAK = "RaidHorn";

	public string Name = string.Empty;

	public string FileName = string.Empty;

	// best guess for name
	public string ModelFileName = string.Empty;

	public string Type = string.Empty;

	// best guess for name
	public string SubType = string.Empty;

	private CombatSubtypeOverride _combatSubtypeOverride;
	private sealed class CombatSubtypeOverride : System.IDisposable
	{
		private ItemInfo _item;
		private readonly string _previous;
		public CombatSubtypeOverride(ItemInfo item) { _item = item; _previous = item.SubType; }
		public void Dispose()
		{
			ItemInfo item = _item;
			if (item == null) return;
			_item = null;
			if (!object.ReferenceEquals(item._combatSubtypeOverride, this)) return;
			item.SubType = _previous;
			item._combatSubtypeOverride = null;
		}
	}

	// Apply before profile/fight copies are built. Existing copies keep their snapshot.
	internal bool TryOverrideCombatSubtype(string subtype, out System.IDisposable lifetime)
	{
		lifetime = null;
		if ((Type != "Weapon" && Type != "Ranged" && Type != "Magic") ||
			_combatSubtypeOverride != null || string.IsNullOrEmpty(subtype) || subtype.Length > 128) return false;
		foreach (char c in subtype)
			if (!(c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c >= '0' && c <= '9' || c == '_')) return false;
		var replacement = new CombatSubtypeOverride(this);
		SubType = subtype;
		_combatSubtypeOverride = replacement;
		lifetime = replacement;
		return true;
	}

	private InitialProfileOverride _initialProfileOverride;
	private sealed class InitialProfileOverride : System.IDisposable
	{
		private ItemInfo _item;
		private readonly bool _hadLevel;
		private readonly int _level;
		private readonly int _upgradeLevel;
		private readonly string _upgradeTemplate;
		private readonly string _legacyPaidItem;
		private readonly List<UpgradeData> _localUpgrades;
		private readonly Attributes _attributes;
		public InitialProfileOverride(ItemInfo item)
		{
			_item = item;
			_hadLevel = item.HasAuthoredLevel;
			_level = item.ItemLevel;
			_upgradeLevel = item.UpgradeLevel;
			_upgradeTemplate = item.UpgradeTemplateName;
			_legacyPaidItem = item.LegacyPaidItem;
			_localUpgrades = item.LocalUpgrades;
			_attributes = new Attributes(item.ItemAttributes);
		}
		public void Dispose()
		{
			ItemInfo item = _item;
			if (item == null) return;
			_item = null;
			if (!object.ReferenceEquals(item._initialProfileOverride, this)) return;
			item.HasAuthoredLevel = _hadLevel;
			item.ItemLevel = _level;
			item.UpgradeLevel = _upgradeLevel;
			item.UpgradeTemplateName = _upgradeTemplate;
			item.LegacyPaidItem = _legacyPaidItem;
			item.LocalUpgrades = _localUpgrades;
			item.ItemAttributes = new Attributes(_attributes);
			item._initialProfileOverride = null;
		}
	}

	// The catalog item changes in place before new shop/fighter copies are built.
	// Its recovered NodeXML, price, shared upgrade tables and saved item identity are untouched.
	internal bool TryOverrideInitialProfile(int level, int upgradeLevel,
		System.Collections.Generic.IReadOnlyDictionary<string, int> stats, string upgradeTemplate, string legacyPaidItem,
		bool clearLocalUpgrades,
		out System.IDisposable lifetime)
	{
		lifetime = null;
		if ((Type != "Weapon" && Type != "Armor" && Type != "Helm" && Type != "Ranged" && Type != "Magic") ||
			_initialProfileOverride != null || level < 1 || level > 52 || upgradeLevel < 0 || upgradeLevel > 5200 || stats == null)
			return false;
		if (upgradeTemplate != null && upgradeTemplate != Type + "_Bonus" && upgradeTemplate != "Paid_" + Type + "_Bonus")
			return false;
		if (legacyPaidItem != null && legacyPaidItem != "None" && legacyPaidItem != "Paid" && legacyPaidItem != "SuperPaid")
			return false;
		var attributes = new Attributes();
		foreach (var stat in stats)
		{
			if (stat.Value < 0 || stat.Value > 1000000) return false;
			attributes.Set(stat.Key, stat.Value);
		}
		var replacement = new InitialProfileOverride(this);
		HasAuthoredLevel = true;
		ItemLevel = level;
		UpgradeLevel = upgradeLevel;
		if (upgradeTemplate != null) UpgradeTemplateName = upgradeTemplate;
		if (legacyPaidItem != null) LegacyPaidItem = legacyPaidItem;
		if (clearLocalUpgrades) LocalUpgrades = new List<UpgradeData>();
		ItemAttributes = attributes;
		_initialProfileOverride = replacement;
		lifetime = replacement;
		return true;
	}

	private ShopPriceOverride _shopPriceOverride;
	private sealed class ShopPriceOverride : System.IDisposable
	{
		private ItemInfo _item;
		private readonly ObscuredLong _coinPrice;
		private readonly ObscuredLong _gemPrice;
		public ShopPriceOverride(ItemInfo item)
		{
			_item = item;
			_coinPrice = item.CoinPrice;
			_gemPrice = item.GemPrice;
		}
		public void Dispose()
		{
			ItemInfo item = _item;
			if (item == null) return;
			_item = null;
			if (!object.ReferenceEquals(item._shopPriceOverride, this)) return;
			item.CoinPrice = _coinPrice;
			item.GemPrice = _gemPrice;
			item._shopPriceOverride = null;
		}
	}

	// Update the native catalog before shop copies are built. Both currency fields
	// are replaced so switching currency cannot leave the old price active.
	internal bool TryOverrideShopPrice(long coins, long gems, out System.IDisposable lifetime)
	{
		lifetime = null;
		if ((Type != "Weapon" && Type != "Armor" && Type != "Helm" && Type != "Ranged" && Type != "Magic") ||
			_shopPriceOverride != null || coins < 0 || gems < 0 || coins > int.MaxValue || gems > int.MaxValue ||
			(coins == 0 && gems == 0)) return false;
		var replacement = new ShopPriceOverride(this);
		CoinPrice = (ObscuredLong)coins;
		GemPrice = (ObscuredLong)gems;
		_shopPriceOverride = replacement;
		lifetime = replacement;
		return true;
	}

	private ItemPresentationOverride _presentationOverride;
	private sealed class ItemPresentationOverride : System.IDisposable
	{
		private ItemInfo _item;
		private readonly string _icon;
		private readonly string _model;
		public ItemPresentationOverride(ItemInfo item)
		{ _item = item; _icon = item.FileName; _model = item.ModelFileName; }
		public void Dispose()
		{
			ItemInfo item = _item;
			if (item == null) return;
			_item = null;
			if (!object.ReferenceEquals(item._presentationOverride, this)) return;
			item.FileName = _icon;
			item.ModelFileName = _model;
			item._presentationOverride = null;
		}
	}

	internal bool TryOverridePresentation(string icon, string model, out System.IDisposable lifetime)
	{
		lifetime = null;
		if ((Type != "Weapon" && Type != "Armor" && Type != "Helm" && Type != "Ranged" && Type != "Magic") ||
			_presentationOverride != null || (string.IsNullOrEmpty(icon) && string.IsNullOrEmpty(model))) return false;
		var replacement = new ItemPresentationOverride(this);
		if (!string.IsNullOrEmpty(icon)) FileName = icon;
		if (!string.IsNullOrEmpty(model)) ModelFileName = model;
		_presentationOverride = replacement;
		lifetime = replacement;
		return true;
	}

	// AI table grouping may differ from the subtype used by animations and conditions.
	internal string TacticSubtype { get; private set; } = string.Empty;
	internal string EffectiveTacticSubtype => string.IsNullOrEmpty(TacticSubtype) ? SubType : TacticSubtype;
	private TacticSubtypeOverride _tacticSubtypeOverride;

	private sealed class TacticSubtypeOverride : System.IDisposable
	{
		private ItemInfo _item;
		private readonly string _previous;
		public TacticSubtypeOverride(ItemInfo item) { _item = item; _previous = item.TacticSubtype; }
		public void Dispose()
		{
			ItemInfo item = _item;
			if (item == null) return;
			_item = null;
			if (!object.ReferenceEquals(item._tacticSubtypeOverride, this)) return;
			item.TacticSubtype = _previous;
			item._tacticSubtypeOverride = null;
		}
	}

	// Empty explicitly restores subtype fallback; null is not an override request.
	// Existing fight copies retain their snapshot until the next content lifecycle.
	internal bool TryOverrideTacticSubtype(string group, out System.IDisposable lifetime)
	{
		lifetime = null;
		if (Type != "Weapon" || _tacticSubtypeOverride != null || group == null || group.Length > 128) return false;
		foreach (char c in group)
			if (!(c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c >= '0' && c <= '9' || c == '_')) return false;
		var replacement = new TacticSubtypeOverride(this);
		TacticSubtype = group;
		_tacticSubtypeOverride = replacement;
		lifetime = replacement;
		return true;
	}

	public string IDFNCLPIIMA = string.Empty;

	public string HBCNKNFPAIM = string.Empty;

	public string DBJJONLCHND = string.Empty;

	public string MMHIKEIDDNB = string.Empty;

	private string IIKKHJIIFEE = string.Empty;

	private bool JFOKBAKDBDA;

	public string FPEIFLEBEAA = string.Empty;

	public string EGAJMELKANL = string.Empty;

	public string MIIJIMJDHFP = string.Empty;

	// best guess for name
	public string LegacyPaidItem = string.Empty;

	public string GGDJIPKMKFC = string.Empty;

	public string CGGDGCCNKJA = string.Empty;

	public string CMDJPAKOHMK = string.Empty;

	// best guess for name
	public string UpgradeTemplateName = string.Empty;

	public bool ANNCECNAEPN;

	// best guess for name
	public bool HasAuthoredLevel;

	private bool PPGBMODEAGD;

	public int Index;

	public int NLMDNOBHHKP;

	// best guess for name
	public int ItemLevel;

	public int GDCBBAHKCIE;

	public bool DCHJDPCEODD;

	private bool FOMPCNKEPJF;

	public int ICDIEHCJBGA;

	public int GKODCKNAAHB;

	// best guess for name
	public int UpgradeLevel;

	public long EHKNIKHPGDN;

	// best guess for name
	public ObscuredLong CoinPrice = (ObscuredLong)(0L);

	// best guess for name
	public ObscuredLong GemPrice = (ObscuredLong)(0L);

	public ObscuredLong KLHOKKPALOK = (ObscuredLong)(0L);

	public ObscuredLong NDCOLFHCNLD = (ObscuredLong)(0L);

	public bool MBLKNNAFCOB;

	public ObscuredLong HHIFKGOJFAC = (ObscuredLong)(0L);

	public ObscuredLong BBMLCBEFLGI = (ObscuredLong)(0L);

	public ItemInfo ParentItem;

	public XmlNode NodeXML;

	// best guess for name
	public bool IgnoreInventoryEnchantments;

	private bool DHDDDJFLDBD;

	// best guess for name
	public Attributes ItemAttributes = new Attributes();

	// best guess for name
	public List<PerkInfoItem> InnatePerks = new List<PerkInfoItem>();
	private InnatePerkOverride _innatePerkOverride;

	private sealed class InnatePerkOverride : System.IDisposable
	{
		private ItemInfo _item;
		private readonly List<PerkInfoItem> _previous;
		public InnatePerkOverride(ItemInfo item) { _item = item; _previous = item.InnatePerks; }
		public void Dispose()
		{
			ItemInfo item = _item;
			if (item == null) return;
			_item = null;
			if (!object.ReferenceEquals(item._innatePerkOverride, this)) return;
			item.InnatePerks = _previous;
			item._innatePerkOverride = null;
		}
	}

	internal bool TryOverrideInnatePerks(XmlNode perks, out System.IDisposable lifetime)
	{
		lifetime = null;
		if (_innatePerkOverride != null || perks == null || perks.Name != "Perks") return false;
		var resolved = new List<PerkInfoItem>();
		var names = new HashSet<string>(System.StringComparer.Ordinal);
		foreach (XmlNode child in perks.ChildNodes)
		{
			if (child.NodeType == XmlNodeType.Comment || child.NodeType == XmlNodeType.Whitespace) continue;
			if (child.NodeType != XmlNodeType.Element || child.Name != "Perk" || resolved.Count >= 64) return false;
			string name = child.Attributes?["Name"]?.Value;
			if (string.IsNullOrEmpty(name) || !names.Add(name)) return false;
			PerkInfoItem definition = GameUtils.FDEJIIDIPBI.ABAGJKMKCBA(name);
			if (definition == null) return false;
			// Combat marks equipment perks as weapon/non-weapon. Never share that mutable marker with the registry or another item.
			resolved.Add(definition.Clone(child["Set"], child["RatingEvaluation"]));
		}
		var replacement = new InnatePerkOverride(this);
		InnatePerks = resolved;
		_innatePerkOverride = replacement;
		lifetime = replacement;
		return true;
	}

	// best guess for name
	public List<UpgradeData> LocalUpgrades = new List<UpgradeData>();

	// best guess for name
	public List<PerkInfoItem> DefaultEnchantmentPreviews = new List<PerkInfoItem>();

	// best guess for name
	public List<PerkInfoItem> ParsedPerks = new List<PerkInfoItem>();

	// best guess for name
	public List<PerkStruct> DefaultEnchantments = new List<PerkStruct>();

	private DefaultEnchantmentOverride _defaultEnchantmentOverride;

	private sealed class DefaultEnchantmentOverride : System.IDisposable
	{
		private ItemInfo _item;
		private readonly List<PerkInfoItem> _previousPreview;
		private readonly List<PerkStruct> _previousGrants;
		public DefaultEnchantmentOverride(ItemInfo item)
		{ _item = item; _previousPreview = item.DefaultEnchantmentPreviews; _previousGrants = item.DefaultEnchantments; }
		public void Dispose()
		{
			ItemInfo item = _item;
			if (item == null) return;
			_item = null;
			if (!object.ReferenceEquals(item._defaultEnchantmentOverride, this)) return;
			item.DefaultEnchantmentPreviews = _previousPreview;
			item.DefaultEnchantments = _previousGrants;
			item._defaultEnchantmentOverride = null;
		}
	}

	// Update shop preview and acquisition defaults together. Existing UserItem save nodes are untouched.
	internal bool TryOverrideDefaultEnchantments(XmlNode enchantments, out System.IDisposable lifetime)
	{
		lifetime = null;
		if (_defaultEnchantmentOverride != null || enchantments == null || enchantments.Name != "Enchantments") return false;
		var previews = new List<PerkInfoItem>();
		var grants = new List<PerkStruct>();
		var names = new HashSet<string>(System.StringComparer.Ordinal);
		foreach (XmlNode child in enchantments.ChildNodes)
		{
			if (child.NodeType == XmlNodeType.Comment || child.NodeType == XmlNodeType.Whitespace) continue;
			if (child.NodeType != XmlNodeType.Element || child.Name != "Perk" || grants.Count >= 64) return false;
			string name = child.Attributes?["Name"]?.Value;
			if (string.IsNullOrEmpty(name) || !names.Add(name)) return false;
			PerkInfoItem preview = APPAODDDDKI(child);
			if (preview == null) return false;
			previews.Add(preview);
			grants.Add(new PerkStruct(child));
		}
		var replacement = new DefaultEnchantmentOverride(this);
		DefaultEnchantmentPreviews = previews;
		DefaultEnchantments = grants;
		_defaultEnchantmentOverride = replacement;
		lifetime = replacement;
		return true;
	}

	private bool DJNOJLDEHDD;

	public string FAEGJAEEMGH = string.Empty;

	public ObscuredInt CPODJDDPJHB = (ObscuredInt)(0);

	private int FMOJBFNFLNM;

	private int OJMODONDEHE;

	private bool DGOMAGNAMMD;

	public ObscuredInt FOLLHACLPNB = (ObscuredInt)(0);

	public bool ACOIHHPOBDH;

	public long NNLMNNAEDIE;

	public long PEGDPDINDDO;

	public string FCCNPMNNGAN
	{
		get
		{
			return JLDEALIEEJI();
		}
		set
		{
			set_MarketID(value);
		}
	}

	public bool LBPBADPNHLJ
	{
		get
		{
			return DFFFFIHOOKL();
		}
	}

	public bool IsNew
	{
		get
		{
			return DBHJGAGOLOB();
		}
		set
		{
			BEBDMOEIEJN(value);
		}
	}

	public ItemInfo(XmlNode node)
	{
		if (node != null)
		{
			Init();
			JKJLFOAOLFI(node);
		}
	}

	protected ItemInfo()
	{
		Init();
	}

	protected ItemInfo(ItemInfo item)
	{
		Name = item.Name;
		FileName = item.FileName;
		ModelFileName = item.ModelFileName;
		Type = item.Type;
		SubType = item.SubType;
		TacticSubtype = item.TacticSubtype;
		IDFNCLPIIMA = item.IDFNCLPIIMA;
		HBCNKNFPAIM = item.HBCNKNFPAIM;
		DBJJONLCHND = item.DBJJONLCHND;
		MMHIKEIDDNB = item.MMHIKEIDDNB;
		IIKKHJIIFEE = item.IIKKHJIIFEE;
		JFOKBAKDBDA = item.JFOKBAKDBDA;
		FPEIFLEBEAA = item.FPEIFLEBEAA;
		EGAJMELKANL = item.EGAJMELKANL;
		MIIJIMJDHFP = item.MIIJIMJDHFP;
		LegacyPaidItem = item.LegacyPaidItem;
		GGDJIPKMKFC = item.GGDJIPKMKFC;
		CGGDGCCNKJA = item.CGGDGCCNKJA;
		CMDJPAKOHMK = item.CMDJPAKOHMK;
		ANNCECNAEPN = item.ANNCECNAEPN;
		HasAuthoredLevel = item.HasAuthoredLevel;
		UpgradeTemplateName = item.UpgradeTemplateName;
		PPGBMODEAGD = item.PPGBMODEAGD;
		Index = item.Index;
		NLMDNOBHHKP = item.NLMDNOBHHKP;
		ItemLevel = item.ItemLevel;
		GDCBBAHKCIE = item.GDCBBAHKCIE;
		DCHJDPCEODD = item.DCHJDPCEODD;
		FOMPCNKEPJF = item.FOMPCNKEPJF;
		ICDIEHCJBGA = item.ICDIEHCJBGA;
		GKODCKNAAHB = item.GKODCKNAAHB;
		UpgradeLevel = item.UpgradeLevel;
		EHKNIKHPGDN = item.EHKNIKHPGDN;
		CoinPrice = item.CoinPrice;
		GemPrice = item.GemPrice;
		KLHOKKPALOK = item.KLHOKKPALOK;
		NDCOLFHCNLD = item.NDCOLFHCNLD;
		MBLKNNAFCOB = item.MBLKNNAFCOB;
		HHIFKGOJFAC = item.HHIFKGOJFAC;
		BBMLCBEFLGI = item.BBMLCBEFLGI;
		ParentItem = item.ParentItem;
		IgnoreInventoryEnchantments = item.IgnoreInventoryEnchantments;
		DJNOJLDEHDD = item.DJNOJLDEHDD;
		ItemAttributes = new Attributes(item.ItemAttributes);
		InnatePerks = new List<PerkInfoItem>(item.InnatePerks);
		LocalUpgrades = new List<UpgradeData>(item.LocalUpgrades);
		DefaultEnchantmentPreviews = new List<PerkInfoItem>(item.DefaultEnchantmentPreviews);
		ParsedPerks = new List<PerkInfoItem>(item.ParsedPerks);
		DefaultEnchantments = new List<PerkStruct>(item.DefaultEnchantments);
	}

	public string JLDEALIEEJI()
	{
		return IIKKHJIIFEE;
	}

	public void set_MarketID(string value)
	{
		IIKKHJIIFEE = value;
	}

	public bool DFFFFIHOOKL()
	{
		return JFOKBAKDBDA;
	}

	public bool DBHJGAGOLOB()
	{
		return DJNOJLDEHDD;
	}

	public void BEBDMOEIEJN(bool value)
	{
		if (!value || GKODCKNAAHB == 0)
		{
			DJNOJLDEHDD = value;
		}
	}

	public void RandomizeObscuredVars()
	{
		CPODJDDPJHB.GMCADPGOCHM();
		FOLLHACLPNB.GMCADPGOCHM();
		CoinPrice.GMCADPGOCHM();
		GemPrice.GMCADPGOCHM();
		KLHOKKPALOK.GMCADPGOCHM();
		NDCOLFHCNLD.GMCADPGOCHM();
		HHIFKGOJFAC.GMCADPGOCHM();
		BBMLCBEFLGI.GMCADPGOCHM();
		LocalUpgrades.ForEach((UpgradeData DHDMNHCIPEH) =>
		{
			DHDMNHCIPEH.RandomizeObscuredVars();
		});
	}

	private long GetPrice()
	{
		return (ObscuredLong)((!PLBFFNCCCGO()) ? CoinPrice : GemPrice);
	}

	private long HIPLKBCODAH()
	{
		return (!PLBFFNCCCGO()) ? OHBBLIMNIMJ() : MCNMMBCJADI();
	}

	private long MKEHOGFBMMA()
	{
		return (ObscuredLong)(CoinPrice) * (long)GameUtils.FPBFDNBDDIE;
	}

	public long OHBBLIMNIMJ()
	{
		return (ObscuredLong)(CoinPrice);
	}

	public long MCNMMBCJADI()
	{
		return (ObscuredLong)(GemPrice);
	}

	private int DNDHIHJPIEA()
	{
		return (int)((float)(ObscuredInt)(FOLLHACLPNB) * GECGFACDOBA());
	}

	public bool PLBFFNCCCGO()
	{
		return 0 < (ObscuredLong)(GemPrice);
	}

	public bool INCBGIDFIDN()
	{
		return 0 < (ObscuredLong)(CoinPrice);
	}

	public bool CAIEBJHILON()
	{
		return 0 < (ObscuredInt)(FOLLHACLPNB);
	}

	public void LEKDAILCFEG()
	{
		DCHJDPCEODD = FOMPCNKEPJF;
	}

	public bool GOKHJMOEGIJ()
	{
		return GDCBBAHKCIE > 0;
	}

	public bool INEOECGAGGD()
	{
		return (ParentItem != null) ? true : false;
	}

	public virtual ItemInfo Clone()
	{
		ItemInfo dJKEECEOCJB = new ItemInfo(this);
		dJKEECEOCJB.DefaultEnchantments.Clear();
		return dJKEECEOCJB;
	}

	private void ReadCombatClassification(XmlNode node)
	{
		if (!node.Attributes["Type"].Empty())
		{
			Type = node.Attributes["Type"].CIPOICEEIBK(string.Empty);
		}
		if (!node.Attributes["SubType"].Empty())
		{
			SubType = node.Attributes["SubType"].CIPOICEEIBK(string.Empty);
		}
		if (!node.Attributes["TacticSubtype"].Empty())
		{
			TacticSubtype = node.Attributes["TacticSubtype"].CIPOICEEIBK(string.Empty);
		}
	}

	private void JKJLFOAOLFI(XmlNode node)
	{
		if (!node.Attributes["Name"].Empty())
		{
			Name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		}
		if (!node.Attributes["PackLabel"].Empty())
		{
			MMHIKEIDDNB = node.Attributes["PackLabel"].CIPOICEEIBK(string.Empty);
		}
		if (!node.Attributes["GroupID"].Empty())
		{
			MMHIKEIDDNB = node.Attributes["GroupID"].CIPOICEEIBK(string.Empty);
		}
		if (!node.Attributes["Image"].Empty())
		{
			FileName = node.Attributes["Image"].CIPOICEEIBK(string.Empty);
		}
		if (!node.Attributes["Model"].Empty())
		{
			ModelFileName = node.Attributes["Model"].CIPOICEEIBK(string.Empty);
		}
		ReadCombatClassification(node);
		if (!node.Attributes["Text"].Empty())
		{
			GGDJIPKMKFC = node.Attributes["Text"].CIPOICEEIBK(string.Empty);
		}
		if (!node.Attributes["TextButton"].Empty())
		{
			CGGDGCCNKJA = node.Attributes["TextButton"].CIPOICEEIBK(string.Empty);
		}
		if (!node.Attributes["Price"].Empty())
		{
			CoinPrice = (ObscuredLong)(node.Attributes["Price"].ParseLong(0L));
		}
		if (!node.Attributes["PriceDigits"].Empty())
		{
			OJMODONDEHE = node.Attributes["PriceDigits"].ParseInt();
		}
		if (!node.Attributes["BonusPrice"].Empty())
		{
			GemPrice = (ObscuredLong)(node.Attributes["BonusPrice"].ParseLong(0L));
		}
		if (!node.Attributes["LotteryPrice"].Empty())
		{
			FOLLHACLPNB = (ObscuredInt)(node.Attributes["LotteryPrice"].ParseInt());
		}
		if (!node.Attributes["SilentRecieve"].Empty())
		{
			GKODCKNAAHB = node.Attributes["SilentRecieve"].ParseInt();
		}
		if (string.IsNullOrEmpty(IIKKHJIIFEE) && (SystemProperties.LHGPKEFEHDH() || SystemProperties.MEBGOGMJFLM()) && !node.Attributes["IphoneID"].Empty())
		{
			IIKKHJIIFEE = node.Attributes["IphoneID"].CIPOICEEIBK(string.Empty);
		}
		if (string.IsNullOrEmpty(IIKKHJIIFEE) && !AssemblyController.JONCCPLEIBE().BKGIFIPIHAL() && SystemProperties.IPJFCBAGMJJ() && !node.Attributes["AndroidID"].Empty())
		{
			IIKKHJIIFEE = node.Attributes["AndroidID"].CIPOICEEIBK(string.Empty);
		}
		if (string.IsNullOrEmpty(IIKKHJIIFEE) && AssemblyController.JONCCPLEIBE().BKGIFIPIHAL() && SystemProperties.IPJFCBAGMJJ() && !node.Attributes["ChineseID"].Empty())
		{
			IIKKHJIIFEE = node.Attributes["ChineseID"].CIPOICEEIBK(string.Empty);
		}
		if (string.IsNullOrEmpty(IIKKHJIIFEE) && SystemProperties.AFKGHBJPLOK() && !node.Attributes["WinPhoneID"].Empty())
		{
			IIKKHJIIFEE = node.Attributes["WinPhoneID"].CIPOICEEIBK(string.Empty);
		}
		if (!node.Attributes["ConsumableProduct"].Empty())
		{
			JFOKBAKDBDA = node.Attributes["ConsumableProduct"].ParseBool();
		}
		if (!AssemblyController.JONCCPLEIBE().BKGIFIPIHAL() && !node.Attributes["RealPrice"].Empty())
		{
			FPEIFLEBEAA = node.Attributes["RealPrice"].CIPOICEEIBK(string.Empty);
			EGAJMELKANL = FPEIFLEBEAA.Substring(1);
			MIIJIMJDHFP = "USD";
		}
		if (AssemblyController.JONCCPLEIBE().BKGIFIPIHAL() && !node.Attributes["RealPriceChina"].Empty())
		{
			FPEIFLEBEAA = node.Attributes["RealPriceChina"].CIPOICEEIBK(string.Empty);
			EGAJMELKANL = FPEIFLEBEAA.Substring(2);
			MIIJIMJDHFP = "CNY";
		}
		if (!node.Attributes["isPaid"].Empty())
		{
			MBLKNNAFCOB = node.Attributes["isPaid"].ParseBool();
		}
		if (!node.Attributes["RecieveGold"].Empty())
		{
			HHIFKGOJFAC = (ObscuredLong)(node.Attributes["RecieveGold"].ParseLong(0L));
		}
		if (!node.Attributes["RecieveBonus"].Empty())
		{
			BBMLCBEFLGI = (ObscuredLong)(node.Attributes["RecieveBonus"].ParseLong(0L));
		}
		if (!node.Attributes["CurrencyName"].Empty())
		{
			FAEGJAEEMGH = node.Attributes["CurrencyName"].CIPOICEEIBK(string.Empty);
		}
		if (!node.Attributes["CurrencyValue"].Empty())
		{
			CPODJDDPJHB = (ObscuredInt)(node.Attributes["CurrencyValue"].ParseInt());
		}
		if (!node.Attributes["ShopHide"].Empty())
		{
			bool flag = node.Attributes["ShopHide"].ParseBool();
			DCHJDPCEODD = !flag;
			FOMPCNKEPJF = DCHJDPCEODD;
		}
		if (!node.Attributes["Hidden"].Empty())
		{
			GDCBBAHKCIE = node.Attributes["Hidden"].ParseInt();
		}
		HasAuthoredLevel = !node.Attributes["Level"].Empty();
		if (!node.Attributes["Level"].Empty())
		{
			ItemLevel = node.Attributes["Level"].ParseInt();
		}
		if (!node.Attributes["UpgradeLevel"].Empty())
		{
			UpgradeLevel = node.Attributes["UpgradeLevel"].ParseInt();
		}
		if (!node.Attributes["SpendAfterUse"].Empty())
		{
			ANNCECNAEPN = node.Attributes["SpendAfterUse"].ParseBool();
		}
		if (!node.Attributes["DeliveryTime"].Empty())
		{
			EHKNIKHPGDN = node.Attributes["DeliveryTime"].ParseLong(0L);
		}
		if (!node.Attributes["DeliveryDescription"].Empty())
		{
			DHDDDJFLDBD = node.Attributes["DeliveryDescription"].ParseBool();
		}
		if (!node.Attributes["BonusDeliveryPrice"].Empty())
		{
			KLHOKKPALOK = (ObscuredLong)(node.Attributes["BonusDeliveryPrice"].ParseLong(0L));
		}
		if (!node.Attributes["Milestone"].Empty())
		{
			ICDIEHCJBGA = node.Attributes["Milestone"].ParseInt();
		}
		XmlNode xmlNode = node["Perks"];
		if (xmlNode != null)
		{
			DELGGDKPMKP(xmlNode);
		}
		XmlNode xmlNode2 = node["Enchantments"];
		if (xmlNode2 != null)
		{
			CKIBPGDJHNO(xmlNode2);
			BMAOLOLLBEI(xmlNode2);
		}
		if (!node.Attributes["AddPercent"].Empty())
		{
			FMOJBFNFLNM = node.Attributes["AddPercent"].ParseInt();
		}
		if (!node.Attributes["Icon"].Empty())
		{
			CMDJPAKOHMK = node.Attributes["Icon"].CIPOICEEIBK(string.Empty);
		}
		if (!node.Attributes["PaidItem"].Empty())
		{
			LegacyPaidItem = node.Attributes["PaidItem"].CIPOICEEIBK(string.Empty);
		}
		List<WarriorAttribute> iBLHIAHECLK = GameUtils.BGENALLCKII.IBLHIAHECLK;
		foreach (WarriorAttribute item in iBLHIAHECLK)
		{
			XmlAttribute cJBEMNNNHDM = node.Attributes[item.get_Name()];
			if (!cJBEMNNNHDM.Empty())
			{
				ItemAttributes.Set(item.get_Name(), cJBEMNNNHDM.ParseInt());
			}
		}
		XmlNode xmlNode3 = node["Upgrades"];
		if (xmlNode3 != null)
		{
			UpgradeTemplateName = xmlNode3.Attributes["Template"].CIPOICEEIBK(string.Empty);
		}
	}

	private float GECGFACDOBA()
	{
		return 1f;
	}

	public void MergeWithItem(ItemInfo item)
	{
		if (!string.IsNullOrEmpty(item.Type))
		{
			Type = item.Type;
		}
		if (!string.IsNullOrEmpty(item.SubType))
		{
			SubType = item.SubType;
		}
		if (!string.IsNullOrEmpty(item.TacticSubtype))
		{
			TacticSubtype = item.TacticSubtype;
		}
	}

	public void DELGGDKPMKP(XmlNode node)
	{
		InnatePerks.Clear();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			PerkInfoItem aCONCDFDNJH = APPAODDDDKI(childNode);
			if (aCONCDFDNJH != null)
			{
				InnatePerks.Add(aCONCDFDNJH);
			}
		}
	}

	public static PerkInfoItem APPAODDDDKI(XmlNode node)
	{
		string gOHIIMFFFJI = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		PerkInfoItem aCONCDFDNJH = GameUtils.FDEJIIDIPBI.ABAGJKMKCBA(gOHIIMFFFJI);
		if (aCONCDFDNJH != null)
		{
			if (node["Set"] != null || node["RatingEvaluation"] != null)
			{
				aCONCDFDNJH = aCONCDFDNJH.Clone(node["Set"], node["RatingEvaluation"]);
				string text = node.Attributes["Description"].CIPOICEEIBK(string.Empty);
				if (text != null && !text.Equals(string.Empty))
				{
					aCONCDFDNJH.MGNNJPBCOGD = text;
				}
			}
			return aCONCDFDNJH;
		}
		return null;
	}

	public void CKIBPGDJHNO(XmlNode node)
	{
		DefaultEnchantmentPreviews.Clear();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			PerkInfoItem aCONCDFDNJH = APPAODDDDKI(childNode);
			if (aCONCDFDNJH != null)
			{
				DefaultEnchantmentPreviews.Add(aCONCDFDNJH);
			}
		}
	}

	public void JCJKLMICDIC(XmlNode node)
	{
		APPEHIAIAAM();
		if (node == null)
		{
			return;
		}
		foreach (XmlNode childNode in node.ChildNodes)
		{
			PerkInfoItem aCONCDFDNJH = APPAODDDDKI(childNode);
			if (aCONCDFDNJH != null)
			{
				ParsedPerks.Add(aCONCDFDNJH);
				InnatePerks.Add(aCONCDFDNJH);
			}
		}
	}

	public void BMAOLOLLBEI(XmlNode node)
	{
		GAHFEAAHDCL();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			PerkStruct item = new PerkStruct(childNode);
			DefaultEnchantments.Add(item);
		}
	}

	public void GAHFEAAHDCL()
	{
		DefaultEnchantments.Clear();
	}

	private void FHILKOAPBKG(int NPFOBKBJAOB)
	{
	}

	private void BJBEKBECHAI(int NPFOBKBJAOB)
	{
	}

	public void HNMFDILOBMJ(UpgradeData IFOFMGAKHEP)
	{
		UpgradeData item = DBPHNGLCHHO(IFOFMGAKHEP);
		LocalUpgrades.Add(item);
	}

	public int FMHIKMNJHDL()
	{
		int LPINKLMDEEF = int.MinValue;
		LocalUpgrades.ForEach((UpgradeData DHDMNHCIPEH) =>
		{
			if (DHDMNHCIPEH.OGLHOJNMEBD.AKKLOMFOLNO > LPINKLMDEEF)
			{
				LPINKLMDEEF = DHDMNHCIPEH.OGLHOJNMEBD.AKKLOMFOLNO;
			}
		});
		return LPINKLMDEEF;
	}

	public List<UpgradeData> DNFDAGFAANJ(bool NNDOJGMBEDC = false, int JELPMBDMLAB = int.MaxValue)
	{
		List<UpgradeData> list = new List<UpgradeData>();
		List<UpgradeData> list2 = new List<UpgradeData>();
		int num = FMHIKMNJHDL();
		list2.AddRange(LocalUpgrades);
		UpgradeDataContainer aKHJNNDCKMK = ListSF.GetItems().GetUpgradeDataContainerByName(UpgradeTemplateName);
		if (aKHJNNDCKMK != null)
		{
			foreach (UpgradeData item in aKHJNNDCKMK.KPAPEBOAKIE)
			{
				if (item.OGLHOJNMEBD.AKKLOMFOLNO > num)
				{
					list2.Add(item);
				}
			}
		}
		list2.Sort();
		foreach (UpgradeData item2 in list2)
		{
			if ((!NNDOJGMBEDC || item2.OGLHOJNMEBD.AKKLOMFOLNO > UpgradeLevel) && item2.OGLHOJNMEBD.Level <= JELPMBDMLAB)
			{
				list.Add(item2);
			}
		}
		return list;
	}

	public void HPCGCMMGAAP(UpgradeData LILLEENHNCG)
	{
		List<WarriorAttribute> iBLHIAHECLK = GameUtils.BGENALLCKII.IBLHIAHECLK;
		foreach (WarriorAttribute item in iBLHIAHECLK)
		{
			int OEMALIFPGPO = 0;
			if (LILLEENHNCG.OGLHOJNMEBD.IBLHIAHECLK.Get(item.get_Name(), ref OEMALIFPGPO))
			{
				ItemAttributes.Set(item.get_Name(), OEMALIFPGPO);
			}
		}
		if (LILLEENHNCG.EBHOFBFKNMB.KLHOKKPALOK)
		{
			KLHOKKPALOK = LILLEENHNCG.OGLHOJNMEBD.KLHOKKPALOK;
		}
		if (LILLEENHNCG.EBHOFBFKNMB.FMHECGHHKGB)
		{
			GemPrice = LILLEENHNCG.OGLHOJNMEBD.FMHECGHHKGB;
		}
		if (LILLEENHNCG.EBHOFBFKNMB.EHKNIKHPGDN)
		{
			EHKNIKHPGDN = LILLEENHNCG.OGLHOJNMEBD.EHKNIKHPGDN;
		}
		if (LILLEENHNCG.EBHOFBFKNMB.Level)
		{
			ItemLevel = LILLEENHNCG.OGLHOJNMEBD.Level;
		}
		if (LILLEENHNCG.EBHOFBFKNMB.ICDIEHCJBGA)
		{
			ICDIEHCJBGA = LILLEENHNCG.OGLHOJNMEBD.ICDIEHCJBGA;
		}
		if (LILLEENHNCG.EBHOFBFKNMB.MDAAJFBENON)
		{
			CoinPrice = LILLEENHNCG.OGLHOJNMEBD.MDAAJFBENON;
		}
		if (LILLEENHNCG.EBHOFBFKNMB.AKKLOMFOLNO)
		{
			UpgradeLevel = LILLEENHNCG.OGLHOJNMEBD.AKKLOMFOLNO;
		}
	}

	public ItemInfo ILDOPPMOOOF(int GNLOCMLBNHF)
	{
		List<UpgradeData> list = DNFDAGFAANJ();
		foreach (UpgradeData item in list)
		{
			if (item.OGLHOJNMEBD.AKKLOMFOLNO == GNLOCMLBNHF)
			{
				return MPADIPJLMLH(item);
			}
		}
		return null;
	}

	public ItemInfo HIOBANJPMKF(int GNLOCMLBNHF)
	{
		List<UpgradeData> list = DNFDAGFAANJ();
		foreach (UpgradeData item in list)
		{
			if (item.OGLHOJNMEBD.AKKLOMFOLNO >= GNLOCMLBNHF)
			{
				return MPADIPJLMLH(item);
			}
		}
		return null;
	}

	public ItemInfo GetUpdateItemByLevel(int JHLGOAFNPNM, bool GHNLHKBJOIH = true)
	{
		UpgradeData fKFLGOCPFEB = null;
		UpgradeData fKFLGOCPFEB2 = null;
		UpgradeData fKFLGOCPFEB3 = null;
		bool flag = false;
		List<UpgradeData> list = DNFDAGFAANJ();
		foreach (UpgradeData item in list)
		{
			int gCAPLEJMMPM = item.OGLHOJNMEBD.Level;
			if (!flag && gCAPLEJMMPM == JHLGOAFNPNM)
			{
				fKFLGOCPFEB = item;
				flag = true;
			}
			if (gCAPLEJMMPM > JHLGOAFNPNM)
			{
				fKFLGOCPFEB2 = fKFLGOCPFEB3;
				break;
			}
			fKFLGOCPFEB3 = item;
		}
		if (fKFLGOCPFEB == null && fKFLGOCPFEB2 == null)
		{
			return null;
		}
		UpgradeData lILLEENHNCG = ((!GHNLHKBJOIH) ? fKFLGOCPFEB : fKFLGOCPFEB2);
		return MPADIPJLMLH(lILLEENHNCG);
	}

	public ItemInfo GJAMPOFICNK(int index)
	{
		List<UpgradeData> list = DNFDAGFAANJ();
		if (0 <= index && index < list.Count)
		{
			return MPADIPJLMLH(list[index]);
		}
		LLLOJBFMONN.Error("ItemInfo.getUpdateItemByIndex wrong index: {0}", index);
		return null;
	}

	public UpgradeIndexItem MJNILIJLCMI(int OMHDLKNHNMJ, int upgradeLevel)
	{
		UpgradeIndexItem aACAFOBANOH = new UpgradeIndexItem();
		int num = 0;
		if (ParentItem != null)
		{
			List<UpgradeData> list = ParentItem.DNFDAGFAANJ();
			foreach (UpgradeData item in list)
			{
				UpgradeData.AGKOBJMBAEC oGLHOJNMEBD = item.OGLHOJNMEBD;
				if (oGLHOJNMEBD.Level == ItemLevel && oGLHOJNMEBD.AKKLOMFOLNO < UpgradeLevel && oGLHOJNMEBD.AKKLOMFOLNO > ParentItem.UpgradeLevel)
				{
					num++;
				}
			}
			if (ParentItem.ItemLevel == ItemLevel)
			{
				num++;
			}
		}
		if (num == 0)
		{
			aACAFOBANOH.Type = UpgradeIndexItem.LIPHFAOKLCA.UPGRADE_INDEX_MILESTONE;
			aACAFOBANOH.Index = ItemLevel;
		}
		else
		{
			aACAFOBANOH.Index = num;
		}
		return aACAFOBANOH;
	}

	public void NHJAHNDOLAE(int OMHDLKNHNMJ, int upgradeLevel, ref ItemInfo HDMHCCKLLGK, ref ItemInfo JLNLOCNBGEK)
	{
		List<UpgradeData> list = DNFDAGFAANJ();
		List<UpgradeData> list2 = new List<UpgradeData>();
		UpgradeData fKFLGOCPFEB = null;
		UpgradeData fKFLGOCPFEB2 = null;
		UpgradeData fKFLGOCPFEB3 = null;
		float num = GameUtils.HPEBEOMLHKF.GetValue(Type);
		int num2 = upgradeLevel / 100;
		foreach (UpgradeData item in list)
		{
			int aKKLOMFOLNO = item.OGLHOJNMEBD.AKKLOMFOLNO;
			if (aKKLOMFOLNO == upgradeLevel)
			{
				fKFLGOCPFEB = item;
			}
			if (item.OGLHOJNMEBD.Level <= OMHDLKNHNMJ && aKKLOMFOLNO > upgradeLevel)
			{
				if (item.OGLHOJNMEBD.ICDIEHCJBGA > 0 && (float)item.OGLHOJNMEBD.Level >= (float)num2 + num && (fKFLGOCPFEB2 == null || fKFLGOCPFEB2.OGLHOJNMEBD.AKKLOMFOLNO < aKKLOMFOLNO))
				{
					fKFLGOCPFEB2 = item;
				}
				if (item.OGLHOJNMEBD.ICDIEHCJBGA <= 0 && (fKFLGOCPFEB3 == null || fKFLGOCPFEB3.OGLHOJNMEBD.AKKLOMFOLNO > aKKLOMFOLNO))
				{
					fKFLGOCPFEB3 = item;
				}
			}
		}
		if (fKFLGOCPFEB != null)
		{
			HDMHCCKLLGK = MPADIPJLMLH(fKFLGOCPFEB);
		}
		else
		{
			HDMHCCKLLGK = null;
		}
		if (fKFLGOCPFEB2 != null)
		{
			JLNLOCNBGEK = MPADIPJLMLH(fKFLGOCPFEB2);
		}
		else if (fKFLGOCPFEB3 != null)
		{
			JLNLOCNBGEK = MPADIPJLMLH(fKFLGOCPFEB3);
		}
		else
		{
			JLNLOCNBGEK = null;
		}
	}

	public static void DenominateItems(int NPFOBKBJAOB = 0)
	{
		List<ItemInfo> list = ListSF.GetItems().HCDLKHKBEPF();
		foreach (ItemInfo item in list)
		{
			item.CoinPrice = (ObscuredLong)(GameUtils.GetDenominatedValue((ObscuredLong)(item.CoinPrice), NPFOBKBJAOB));
			item.FHILKOAPBKG(NPFOBKBJAOB);
			item.BJBEKBECHAI(NPFOBKBJAOB);
			List<UpgradeData> kEFPALGDBOC = item.LocalUpgrades;
			foreach (UpgradeData item2 in kEFPALGDBOC)
			{
				item2.OGLHOJNMEBD.MDAAJFBENON = (ObscuredLong)(GameUtils.GetDenominatedValue((ObscuredLong)(item2.OGLHOJNMEBD.MDAAJFBENON), NPFOBKBJAOB));
			}
			if (item.Type.Equals("RealMoneyItem"))
			{
				item.HHIFKGOJFAC = (ObscuredLong)(GameUtils.GetDenominatedValue((ObscuredLong)(item.HHIFKGOJFAC), NPFOBKBJAOB));
			}
		}
		foreach (UpgradeDataContainer item3 in ListSF.GetItems().CKCGBCNMOOP())
		{
			foreach (UpgradeData item4 in item3.KPAPEBOAKIE)
			{
				item4.OGLHOJNMEBD.MDAAJFBENON = (ObscuredLong)(GameUtils.GetDenominatedValue((ObscuredLong)(item4.OGLHOJNMEBD.MDAAJFBENON), NPFOBKBJAOB));
			}
		}
		ListSF.CCDKHLAMKKO().KHCNHPCPFII().NHJAHNDOLAE();
	}

	public ItemInfo MPADIPJLMLH(UpgradeData LILLEENHNCG)
	{
		if (LILLEENHNCG == null)
		{
			return null;
		}
		ItemInfo dJKEECEOCJB = Clone();
		dJKEECEOCJB.ParentItem = this;
		dJKEECEOCJB.HPCGCMMGAAP(LILLEENHNCG);
		return dJKEECEOCJB;
	}

	private void APPEHIAIAAM()
	{
		ParsedPerks.ForEach((PerkInfoItem DHDMNHCIPEH) =>
		{
			MLOOKBFCOHM(DHDMNHCIPEH);
		});
		ParsedPerks.Clear();
	}

	private void MLOOKBFCOHM(PerkInfoItem DPLEGFCHOCE)
	{
		InnatePerks.Remove(DPLEGFCHOCE);
	}

	private void Init()
	{
		CoinPrice = (ObscuredLong)(0L);
		GemPrice = (ObscuredLong)(0L);
		FOLLHACLPNB = (ObscuredInt)(0);
		HHIFKGOJFAC = (ObscuredLong)(0L);
		BBMLCBEFLGI = (ObscuredLong)(0L);
		FAEGJAEEMGH = string.Empty;
		CPODJDDPJHB = (ObscuredInt)(0);
		DCHJDPCEODD = true;
		GDCBBAHKCIE = 0;
		ItemLevel = 0;
		UpgradeLevel = 0;
		ACOIHHPOBDH = false;
		ANNCECNAEPN = false;
		NNLMNNAEDIE = 0L;
		PEGDPDINDDO = 0L;
		MBLKNNAFCOB = false;
		EHKNIKHPGDN = 0L;
		DHDDDJFLDBD = false;
		KLHOKKPALOK = (ObscuredLong)(0L);
		NDCOLFHCNLD = (ObscuredLong)(0L);
		ICDIEHCJBGA = 0;
		GKODCKNAAHB = 0;
		ParentItem = null;
		FMOJBFNFLNM = 0;
		OJMODONDEHE = 0;
		DGOMAGNAMMD = false;
		CMDJPAKOHMK = string.Empty;
		IgnoreInventoryEnchantments = false;
		LegacyPaidItem = "None";
		PPGBMODEAGD = true;
	}

	private UpgradeData DBPHNGLCHHO(UpgradeData IFOFMGAKHEP)
	{
		UpgradeData fKFLGOCPFEB = new UpgradeData(IFOFMGAKHEP);
		List<WarriorAttribute> iBLHIAHECLK = GameUtils.BGENALLCKII.IBLHIAHECLK;
		foreach (WarriorAttribute item in iBLHIAHECLK)
		{
			int OEMALIFPGPO = 0;
			if (ItemAttributes.Get(item.get_Name(), ref OEMALIFPGPO) && !fKFLGOCPFEB.OGLHOJNMEBD.IBLHIAHECLK.Get(item.get_Name(), ref OEMALIFPGPO))
			{
				ItemAttributes.Get(item.get_Name(), ref OEMALIFPGPO);
				fKFLGOCPFEB.OGLHOJNMEBD.IBLHIAHECLK.Set(item.get_Name(), OEMALIFPGPO);
			}
		}
		if (!fKFLGOCPFEB.EBHOFBFKNMB.KLHOKKPALOK)
		{
			fKFLGOCPFEB.OGLHOJNMEBD.KLHOKKPALOK = KLHOKKPALOK;
		}
		if (!fKFLGOCPFEB.EBHOFBFKNMB.FMHECGHHKGB)
		{
			fKFLGOCPFEB.OGLHOJNMEBD.FMHECGHHKGB = GemPrice;
		}
		if (!fKFLGOCPFEB.EBHOFBFKNMB.EHKNIKHPGDN)
		{
			fKFLGOCPFEB.OGLHOJNMEBD.EHKNIKHPGDN = EHKNIKHPGDN;
		}
		if (!fKFLGOCPFEB.EBHOFBFKNMB.Level)
		{
			fKFLGOCPFEB.OGLHOJNMEBD.Level = ItemLevel;
		}
		if (!fKFLGOCPFEB.EBHOFBFKNMB.ICDIEHCJBGA)
		{
			fKFLGOCPFEB.OGLHOJNMEBD.ICDIEHCJBGA = ICDIEHCJBGA;
		}
		if (!fKFLGOCPFEB.EBHOFBFKNMB.MDAAJFBENON)
		{
			fKFLGOCPFEB.OGLHOJNMEBD.MDAAJFBENON = CoinPrice;
		}
		if (!fKFLGOCPFEB.EBHOFBFKNMB.AKKLOMFOLNO)
		{
			fKFLGOCPFEB.OGLHOJNMEBD.AKKLOMFOLNO = UpgradeLevel;
		}
		return fKFLGOCPFEB;
	}

	public void IEIKLANLOPL(ProductMetadata CFDFJHGLMNH)
	{
		FPEIFLEBEAA = CFDFJHGLMNH.localizedPriceString;
		EGAJMELKANL = CFDFJHGLMNH.localizedPrice.ToString();
		MIIJIMJDHFP = CFDFJHGLMNH.isoCurrencyCode;
	}

	public void GEEGNGNLPGO()
	{
		LocalUpgrades.Sort();
		int index = 0;
		LocalUpgrades.ForEach((UpgradeData DHDMNHCIPEH) =>
		{
			DHDMNHCIPEH.UpgradeIndex = index;
			index++;
		});
	}
}
