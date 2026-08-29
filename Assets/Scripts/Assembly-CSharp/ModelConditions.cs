using System.Collections.Generic;
using UnityEngine;

public class ModelConditions
{
	public class ModelPositions
	{
		public Vector2 BOGHNBAKCEL = default(Vector2);

		public Vector2 PCIBKEOCFAO = default(Vector2);

		public ModelObject CBAECAAKAIA;

		public void Clear()
		{
			CBAECAAKAIA = null;
		}
	}

	public KeyData BJACLIMKPAE;

	public List<ItemInfo> OJIAKDDCGLB;

	public List<IntervalAnimation> Intervals;

	public List<IntervalAnimation> FJFOIEFFMEM;

	public List<IntervalAnimation> JLCFPNDDGCJ;

	public List<string> PDKPGKPBBIL = new List<string>();

	public List<string> NNPJJLPCOHD = new List<string>();

	public List<string> MGFNFEHILNF = new List<string>();

	public List<string> DHHADKMMOHP = new List<string>();

	public List<string> NKPMIACBKDE = new List<string>();

	public Dictionary<string, float> PerkVariables = new Dictionary<string, float>();

	public Dictionary<string, string> PerkStringVariables = new Dictionary<string, string>();

	public string ModelName;

	public SceneTypes IBBALIJOJMC;

	public ModelNode AFLPHBDFMGA;

	public ModelNode CJELGCJHMHI;

	public ModelNode EJFOAKCDPHH;

	public ModelNode JMMGJGCDPGE;

	public EventAnimation HFCIDBJJINB;

	public EndRoundType EndRoundType;

	public int BossAbilityState;

	public int PCAOCHAIBJC;

	public int FOIHIKCEBJF;

	public int GFHOIKMBNHF;

	public int OLNDCCIPJAE;

	public int CDPEPJDJIPK;

	public int CNNMAMCKCMO;

	public float BFLPOMAHPJD;

	public float KGCJIBCACBH;

	public int JMHJDHLBHLK;

	public bool IDCHHGHAENM;

	public bool IsPlayer;

	public bool FAHHBNIFAMB;

	public bool FDELMAHAAJD;

	public bool NCBPMBJCFBK;

	public bool EKFCILFBDPO;

	public bool LFLDHGKEDEH;

	public int KAKMANLHJOA;

	public int BOECCPNHAII;

	public bool BHHLEBHLBLH;

	public bool IsWinner;

	public object StrikeResult;

	public List<PerkInfoItem> POBNMMADAJJ;

	public List<PerkInfoItem> CFPLPALGCMK;

	public List<PerksStage.ActionPerk> LPGJIICFIKF = new List<PerksStage.ActionPerk>();

	public List<PerksStage.ActionPerk> CBMFGJHKKMJ = new List<PerksStage.ActionPerk>();

	public List<PerksStage.ActionPerk> FPFKABHOEHP = new List<PerksStage.ActionPerk>();

	public List<PerksStage.ActionPerk> ENBHOAKMCIG = new List<PerksStage.ActionPerk>();

	public int PKMHOICGDIM;

	public int JJDNDOLCMMN;

	public int KHDBLNPFDPE;

	public ModelPositions JBNPEMEEMLK = new ModelPositions();

	public ModelPositions IHJJBIDMEMB = new ModelPositions();

	public ModelPositions GAIBPAGPEGK = new ModelPositions();

	public ModelPositions NECEKOMIPIB = new ModelPositions();

	public void Reset()
	{
		PerkVariables.Clear();
		PerkStringVariables.Clear();
		JBNPEMEEMLK.Clear();
		IHJJBIDMEMB.Clear();
		GAIBPAGPEGK.Clear();
		NECEKOMIPIB.Clear();
	}
}
