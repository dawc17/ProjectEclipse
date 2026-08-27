using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

public class Battle
{
	public const string DFNPDBOMFAA = "base_";

	public const string DACECAKFPFJ = "active_";

	public const string NBINNAJPDHC = "pressed_";

	public const string CJGHIGMIEML = "locked_";

	public const string KJCAEHBBODC = "locked_active_";

	protected RosterBattle MEOMPEEPCJJ;

	protected string _name = string.Empty;

	protected BattleType _type;

	protected Zone GIKMINGBAAK;

	protected Vector2 _pos;

	protected string NBJPNBAGMDD = string.Empty;

	protected string HFFLJODJMMM = string.Empty;

	protected string KENGIGPHMPK = string.Empty;

	// Optional atlas family used by newer map/raid battle icons. The original
	// runtime assumed every icon lived in BattleBtnBase/BattleBtnActive.
	protected string _iconAtlas = string.Empty;

	protected string MINCIJJDCCF = string.Empty;

	protected string _description;

	protected string _location = string.Empty;

	protected string OGIKEKHPFBN = string.Empty;

	protected string DABIBOMAABO = string.Empty;

	protected string IMCKALOIEHB = string.Empty;

	protected List<string> _fightsNames = new List<string>();

	protected List<FightList> JNPMCNMEOLE = new List<FightList>();

	protected DeflatedString CMDDPMAAJOF = new DeflatedString();

	public bool DCHJDPCEODD;

	protected bool NLLECKHLMAN;

	public ushort LEGLFDDINKO;

	protected ushort BELONIAAIEP;

	protected ushort JPMGAALMFKI;

	protected ushort AIKIOPMGCEG;

	public RosterBattle AMBOOABHBAN
	{
		get
		{
			return NNPNEABKHPP();
		}
		set
		{
			FOMHAGJJCLJ(value);
		}
	}

	public Zone OAEIILGHJMG
	{
		get
		{
			return LKDFFCADHNO();
		}
		set
		{
			EENNGGIMMMI(value);
		}
	}

	public Vector2 JJCKADKCDIF
	{
		get
		{
			return ECJPLFFAMJO();
		}
	}

	public string HBCNKNFPAIM
	{
		get
		{
			return MIDPFGENBCF();
		}
	}

	public string BEEELCFNOKB
	{
		get
		{
			return IGPOHDHPIIL();
		}
	}

	public string MJBPMLCLMFN
	{
		get
		{
			return GKEFFHLOHDK();
		}
	}

	public string LBPEGKPKFJH
	{
		get
		{
			return FGPAPMGHBDE();
		}
	}

	public string MGNNJPBCOGD
	{
		get
		{
			return GJOAJAIJHOE();
		}
	}

	public string JKMJHIIMHPG
	{
		get
		{
			return CBABFGDMLIH();
		}
	}

	public string NPPIFKKLNCN
	{
		get
		{
			return MOADJJNKFKB();
		}
	}

	public string JKCHHOMGGBN
	{
		get
		{
			return FOPCGEDOJKC();
		}
	}

	public string BODCLFIBHFK
	{
		get
		{
			return NCJLECDEDMH();
		}
	}

	public List<FightList> EGPLAMMOKHK
	{
		get
		{
			return NAFMJGIGBGL();
		}
	}

	public ushort ANHLAHFDDCE
	{
		get
		{
			return GIOJPNNLKKK();
		}
	}

	public ushort LPMDOHPIEOP
	{
		get
		{
			return MCEGLDIFDBI();
		}
	}

	public ushort ECJBEEODABC
	{
		get
		{
			return KCIKELGFHOA();
		}
	}

	public virtual List<FightList> AOBAHGFPPII
	{
		get
		{
			return ANNHMNIHKCC();
		}
	}

	public Battle(string LFLGCDNKNJI, Vector2 MGMMDGFPBLP, string name, string ADONPNOBBDE, string LHCFHAIDNDP, string EMDJGBHIAIA, ushort CDCJKJNGPOE, ushort MCDAHGPLLDO, string LOKLDPLAPOL, string PEMOECLNECD, string LPJNEDFCBOI, string PINIIFIOECE, string OAPKHNPPGHP, string IHBMPGKIBAN)
	{
		_pos = MGMMDGFPBLP;
		_name = name;
		NBJPNBAGMDD = LOKLDPLAPOL;
		HFFLJODJMMM = PEMOECLNECD;
		KENGIGPHMPK = ADONPNOBBDE;
		MINCIJJDCCF = LHCFHAIDNDP;
		_description = EMDJGBHIAIA;
		GIKMINGBAAK = null;
		DCHJDPCEODD = false;
		MEOMPEEPCJJ = null;
		BELONIAAIEP = CDCJKJNGPOE;
		JPMGAALMFKI = MCDAHGPLLDO;
		NLLECKHLMAN = false;
		LEGLFDDINKO = 0;
		_location = LPJNEDFCBOI;
		OGIKEKHPFBN = PINIIFIOECE;
		AIKIOPMGCEG = 0;
		DABIBOMAABO = OAPKHNPPGHP;
		IMCKALOIEHB = IHBMPGKIBAN;
		ParseTypeBattle(LFLGCDNKNJI);
	}

	public RosterBattle NNPNEABKHPP()
	{
		return MEOMPEEPCJJ;
	}

	public void FOMHAGJJCLJ(RosterBattle value)
	{
		MEOMPEEPCJJ = value;
	}

	public string get_Name()
	{
		return _name;
	}

	public BattleType get_Type()
	{
		return _type;
	}

	public Zone LKDFFCADHNO()
	{
		return GIKMINGBAAK;
	}

	public void EENNGGIMMMI(Zone value)
	{
		GIKMINGBAAK = value;
	}

	public Vector2 ECJPLFFAMJO()
	{
		return _pos;
	}

	public string MIDPFGENBCF()
	{
		return NBJPNBAGMDD;
	}

	public string IGPOHDHPIIL()
	{
		return HFFLJODJMMM;
	}

	public string GKEFFHLOHDK()
	{
		return KENGIGPHMPK;
	}

	public string GetIconAtlas()
	{
		return _iconAtlas;
	}

	public string FGPAPMGHBDE()
	{
		return MINCIJJDCCF;
	}

	public string GJOAJAIJHOE()
	{
		return _description;
	}

	public string CBABFGDMLIH()
	{
		return _location;
	}

	public string MOADJJNKFKB()
	{
		return OGIKEKHPFBN;
	}

	public string FOPCGEDOJKC()
	{
		return DABIBOMAABO;
	}

	public string NCJLECDEDMH()
	{
		return IMCKALOIEHB;
	}

	public List<FightList> NAFMJGIGBGL()
	{
		return JNPMCNMEOLE;
	}

	public ushort GIOJPNNLKKK()
	{
		return BELONIAAIEP;
	}

	public ushort MCEGLDIFDBI()
	{
		return JPMGAALMFKI;
	}

	public ushort KCIKELGFHOA()
	{
		return AIKIOPMGCEG;
	}

	public string OJDNDADJBID()
	{
		return string.Format("{0}|{1}|", GIKMINGBAAK.get_Name(), get_Name());
	}

	public FightList LPHHPIJLJBM(string name)
	{
		foreach (FightList item in JNPMCNMEOLE)
		{
			if (item.Name == name)
			{
				return item;
			}
		}
		return null;
	}

	public virtual FightList OEJCNHOEFIJ(string name)
	{
		if (!_fightsNames.Contains(name))
		{
			return null;
		}
		FightList jDIPBIHBGPF = LPHHPIJLJBM(name);
		if (jDIPBIHBGPF == null)
		{
			XmlNode xmlNode = CMDDPMAAJOF.IOJIGDNFCFL();
			int num = 0;
			foreach (XmlNode item in xmlNode.SelectNodes("Fight"))
			{
				string text = item.Attributes["Name"].CIPOICEEIBK(string.Empty);
				if (text == name)
				{
					jDIPBIHBGPF = LNIDPNHGEHC(item, num);
					break;
				}
				num++;
			}
		}
		return jDIPBIHBGPF;
	}

	public virtual List<FightList> ANNHMNIHKCC()
	{
		if (!NLLECKHLMAN)
		{
			PDFECMAJIEC();
		}
		return JNPMCNMEOLE;
	}

	public virtual FightList FBFHBKPFLJC()
	{
		for (int i = 0; i < AIKIOPMGCEG; i++)
		{
			FightList jDIPBIHBGPF = OAJCBGAKHJJ(i);
			if (jDIPBIHBGPF.PGBKNLAEANJ == ConditionStatus.StatusOpen)
			{
				return jDIPBIHBGPF;
			}
		}
		return null;
	}

	public virtual FightList OAJCBGAKHJJ(int index)
	{
		if (index > AIKIOPMGCEG - 1)
		{
			return null;
		}
		foreach (FightList item in JNPMCNMEOLE)
		{
			if (item.Index == index)
			{
				return item;
			}
		}
		return MKNDLKPFMJK(index);
	}

	public string CCALOKFBLMC()
	{
		return "base_" + KENGIGPHMPK;
	}

	public string OAIJONICMKL()
	{
		return "active_" + KENGIGPHMPK;
	}

	public string DMFMLCMEAGE()
	{
		return "pressed_" + KENGIGPHMPK;
	}

	public string JCBOGEGKLKB()
	{
		return "locked_" + KENGIGPHMPK;
	}

	public string GMBFCAIINAD()
	{
		return "locked_active_" + KENGIGPHMPK;
	}

	public string FKPENLDOHPC()
	{
		return "pressed_" + KENGIGPHMPK;
	}

	public virtual ConditionStatus MNHLGELMOEJ()
	{
		uint aIKIOPMGCEG = AIKIOPMGCEG;
		if (!NLLECKHLMAN)
		{
			PDFECMAJIEC();
		}
		if (BALPDBPBPND(ConditionStatus.StatusComplete) == aIKIOPMGCEG)
		{
			return ConditionStatus.StatusComplete;
		}
		if (BALPDBPBPND(ConditionStatus.StatusIncomplete) == aIKIOPMGCEG)
		{
			return ConditionStatus.StatusIncomplete;
		}
		return ConditionStatus.StatusOpen;
	}

	public virtual void IMCJJPCABOF(ConditionStatus status)
	{
		foreach (FightList item in JNPMCNMEOLE)
		{
			item.PGBKNLAEANJ = status;
		}
	}

	public virtual void SetTime(long time)
	{
		foreach (FightList item in JNPMCNMEOLE)
		{
			item.SetTime(time);
		}
	}

	public virtual void JLPMOKPFECK(long time)
	{
		LLLOJBFMONN.Error("Battle::update ERROR - calling ancestor method. Must call BattleDaily::update or BattlePeriodic::update instead");
	}

	public virtual void EMFABIGKAHC(FightList KGKDKENMAOA, bool FFIBGBMOMPD)
	{
		KGKDKENMAOA.HOCFLEMFFKC(ListSF.IKHJKHMIPEP(KGKDKENMAOA, FFIBGBMOMPD));
	}

	public virtual void BKGJCODJHKF()
	{
		FightList jDIPBIHBGPF = null;
		if (!NLLECKHLMAN)
		{
			PDFECMAJIEC();
		}
		foreach (FightList item in JNPMCNMEOLE)
		{
			if (item.PGBKNLAEANJ == ConditionStatus.StatusOpen)
			{
				if (item.Index > 0)
				{
					jDIPBIHBGPF = JNPMCNMEOLE[item.Index - 1];
				}
				break;
			}
		}
		if (jDIPBIHBGPF == null)
		{
			jDIPBIHBGPF = JNPMCNMEOLE[JNPMCNMEOLE.Count - 1];
		}
		RosterFight pIGKOIFBOME = jDIPBIHBGPF.FLKFFDLLBKA();
		if (pIGKOIFBOME != null)
		{
			pIGKOIFBOME.OBFNFKPHJIN(0);
			pIGKOIFBOME.BIINCAKDHLP(0);
		}
	}

	public bool CHLIJGLJAOA()
	{
		foreach (FightList item in JNPMCNMEOLE)
		{
			if (item.PGBKNLAEANJ == ConditionStatus.StatusComplete)
			{
				return true;
			}
		}
		return false;
	}

	public bool BACJPLBBCKL()
	{
		if (MEOMPEEPCJJ == null)
		{
			return false;
		}
		return MEOMPEEPCJJ.NLIJBCHAEBK();
	}

	public void JNPDHAFMKID()
	{
		MEOMPEEPCJJ = null;
	}

	public bool KBPNDJPMCCG()
	{
		if (MEOMPEEPCJJ != null)
		{
			return MEOMPEEPCJJ.KAPIELMDIIK();
		}
		return false;
	}

	public DeflatedString MMLPEMNIFBD()
	{
		return CMDDPMAAJOF;
	}

	public void JNIIGKNBCCL(XmlNode node)
	{
		_iconAtlas = node.Attributes["IconAtlas"].CIPOICEEIBK(string.Empty);
		KBLMDLKHMEO(node);
		CMDDPMAAJOF.Set(node);
	}

	public void MHMGONPIPKG()
	{
		QuestParameters hHKLFIIBIFF = ListSF.ELEBLBJKDBI().BNMLDPNCMLB();
		QuestParameters hAOHNNFLOGK = ListSF.ELEBLBJKDBI().HAOHNNFLOGK;
		int num = 0;
		while (num != JNPMCNMEOLE.Count)
		{
			FightList jDIPBIHBGPF = JNPMCNMEOLE[num];
			if (hHKLFIIBIFF.LBGOMJFFEPP() != jDIPBIHBGPF && hAOHNNFLOGK.LBGOMJFFEPP() != jDIPBIHBGPF)
			{
				JNPMCNMEOLE.RemoveAt(num);
				ListSF.ELEBLBJKDBI().KINHMMGJEMP(jDIPBIHBGPF);
			}
			else
			{
				num++;
			}
		}
		JNPMCNMEOLE.Clear();
		NLLECKHLMAN = false;
		LEGLFDDINKO = 0;
	}

	public virtual void PDFECMAJIEC()
	{
		for (int i = 0; i < AIKIOPMGCEG; i++)
		{
			bool flag = false;
			foreach (FightList item in JNPMCNMEOLE)
			{
				if (item.Index == i)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				flag = UseAllredyParsedFight(i);
			}
			if (!flag)
			{
				MKNDLKPFMJK(i);
			}
		}
	}

	public virtual void AJKBFMLOCOF(FightList KGKDKENMAOA, int index)
	{
		KGKDKENMAOA.CNAOMDMIGLJ = this;
		KGKDKENMAOA.Index = index;
		JNPMCNMEOLE.Add(KGKDKENMAOA);
		ListSF.ELEBLBJKDBI().AJKBFMLOCOF(KGKDKENMAOA);
		LEGLFDDINKO++;
		if (LEGLFDDINKO >= AIKIOPMGCEG)
		{
			NLLECKHLMAN = true;
		}
		JNPMCNMEOLE = JNPMCNMEOLE.OrderBy((FightList fight) => fight.Index).ToList();
	}

	public virtual void BDAELBFECAJ()
	{
	}

	protected void ParseTypeBattle(string LFLGCDNKNJI)
	{
		_type = ListSF.ELEBLBJKDBI().HIDKFHHJBDH(LFLGCDNKNJI);
	}

	protected bool UseAllredyParsedFight(int index)
	{
		List<FightList> list = ListSF.JEBHJOKNENP(this);
		foreach (FightList item in list)
		{
			if (item.Index == index)
			{
				AJKBFMLOCOF(item, index);
				return true;
			}
		}
		return false;
	}

	protected virtual FightList MKNDLKPFMJK(int index)
	{
		XmlNode xmlNode = CMDDPMAAJOF.IOJIGDNFCFL();
		int num = 0;
		XmlNode hKPPBKPJOEO = null;
		foreach (XmlNode item in xmlNode.SelectNodes("Fight"))
		{
			if (index == num)
			{
				hKPPBKPJOEO = item;
				break;
			}
			num++;
		}
		return LNIDPNHGEHC(hKPPBKPJOEO, index);
	}

	protected virtual FightList LNIDPNHGEHC(XmlNode node, int index)
	{
		ListSF.ELEBLBJKDBI().JGFGMICMBKL = false;
		FightList jDIPBIHBGPF = new FightList();
		ListSF.ELEBLBJKDBI().FOKCPLOMLOK(jDIPBIHBGPF, node, _type, _location, OGIKEKHPFBN, this);
		AJKBFMLOCOF(jDIPBIHBGPF, index);
		ListSF.ELEBLBJKDBI().JGFGMICMBKL = true;
		return jDIPBIHBGPF;
	}

	protected virtual uint BALPDBPBPND(ConditionStatus status)
	{
		uint num = 0u;
		foreach (FightList item in JNPMCNMEOLE)
		{
			if (item.PGBKNLAEANJ == status)
			{
				num++;
			}
		}
		return num;
	}

	protected void KBLMDLKHMEO(XmlNode IHKJDPGFDOE)
	{
		AIKIOPMGCEG = 0;
		_fightsNames.Clear();
		if (IHKJDPGFDOE == null)
		{
			return;
		}
		XmlNodeList xmlNodeList = IHKJDPGFDOE.SelectNodes("Fight");
		AIKIOPMGCEG = (ushort)xmlNodeList.Count;
		foreach (XmlNode item2 in xmlNodeList)
		{
			string item = item2.Attributes["Name"].CIPOICEEIBK(string.Empty);
			_fightsNames.Add(item);
		}
	}
}
