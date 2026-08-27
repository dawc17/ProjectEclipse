using System.Collections.Generic;
using UnityEngine;

public class ModelAnimation : global::EventDispatcher<object>
{
	public enum CEEJIMAJDMN
	{
		ON_START_ANIMATION_EVENT = 0,
		ON_STOP_ANIMATION_EVENT = 1,
		ON_START_INTERVAL_EVENT = 2,
		ON_STOP_INTERVAL_EVENT = 3,
		ON_ACTION_START = 4
	}

	private ModelAnimation CDMBBKPCMPP;

	private ModelAnimation LOOMFPAEADA;

	private HashSet<IntervalAnimation.NGAJJDIEDGF> PAICIELHBHA;

	private bool MDLBEBOGOGK;

	private bool MHEIFCGAOHP;

	private bool ONHMMDAOGIM;

	private bool KJODFLDMCFP;

	private int JMKAHNADIOI;

	private int LBHPMJDHAEM;

	private ModelNode KFGEBGBEJBC;

	private int GKGEBAKLIDH;

	private int EJJNHDCIEAD;

	private int HHDIJIMGOKM;

	private int IDKMDLCEHBK;

	private int OLFJCLJKHIF;

	private int CNFABIDNBOH;

	private int BCGHHIIPCBJ;

	private int FGFGGGKDJLN;

	private bool IPGAAGOANDE;

	private bool INFAGPDFGNL;

	private bool AEKEELJMLDC;

	private bool KGIEFJNJFOH;

	private float BDHBFDMBMFM;

	private float DCJLLPNFIMF;

	private float PMIELLEMGJK;

	private int GEPFNKEMDEE;

	private bool NLANLJEDHKJ;

	private float NBLBKLANDNC;

	private Vector3f JCLKMEAJOLO = new Vector3f();

	private Vector3f BFDLFAHGKHP = new Vector3f();

	private Vector3f AGAFKHLPLCA = new Vector3f();

	private Vector3f DBCBOPONOBE = new Vector3f();

	private float GLOJGJIBABF;

	private int MIDMNJKJOFO;

	private int NINBIDKEHKD;

	private int ALKLIKKIDCM;

	private ModelObject _Model;

	private float DNOJAJNAFAF;

	private float DPEOGNBGKML;

	private KeyFrames _Frames = new KeyFrames();

	private List<List<Vector3f>> AHOBIIMFNEP = new List<List<Vector3f>>();

	private int PLLOJCCNDOH;

	private int ECNLDOICIND;

	private List<IntervalAnimation> KKNKJMCFIJK = new List<IntervalAnimation>();

	private List<IntervalAnimation> OACCPPMJMML = new List<IntervalAnimation>();

	private InfoAnimation BAOONIGFBMB;

	private InfoAnimation FJDKCIHGLLM;

	private List<ModelEdge> ECNLLKIJIGP = new List<ModelEdge>();

	public ModelNode LNGDODBCMEB;

	public ModelNode NMGJKMEDDCB;

	public ModelNode HDILJAAKKDL;

	public InfoAnimation GJGDKFAAGOD;

	public ModelAnimation KNFBNMGCKFO
	{
		get
		{
			return ODHOECEPOFK();
		}
		set
		{
			CBKLDPIBGHD(value);
		}
	}

	public ModelAnimation PBDLLNEOIDG
	{
		get
		{
			return OJKLPPNCONP();
		}
		set
		{
			NFEGCGJIICB(value);
		}
	}

	public bool EKEPPACCCPI
	{
		get
		{
			return NMEEPBDJHMG();
		}
	}

	public int GFHOIKMBNHF
	{
		get
		{
			return KFCNPADAMHA();
		}
		set
		{
			set_Sign(value);
		}
	}

	public int CLIPMJNJDKI
	{
		get
		{
			return BOIDEOFKBMK();
		}
	}

	public ModelNode BKODDBIOLLD
	{
		get
		{
			return PKKDMELGFBE();
		}
	}

	public int HALDIEBDJLG
	{
		get
		{
			return LOIJGOPOGMO();
		}
	}

	public int LHHAGECFIOL
	{
		get
		{
			return BNALDMNOMHH();
		}
	}

	public bool KFFBBLOCLEL
	{
		get
		{
			return ANHGOGDEFCO();
		}
	}

	public float StartPosition
	{
		get
		{
			return FHGNPPBLIIL();
		}
	}

	public float DGGMKIEPDCE
	{
		get
		{
			return LPNPNBJGKJM();
		}
	}

	public int BANAFKEMLJC
	{
		get
		{
			return JFGEHNHLDJM();
		}
	}

	public float ICCPGFDJFMN
	{
		get
		{
			return LIBBBOCOCNP();
		}
		set
		{
			set_ShiftWallDelta(value);
		}
	}

	public Vector3f Shift
	{
		get
		{
			return HOPJDHOEKEN();
		}
		set
		{
			JCNEDHMGLKE(value);
		}
	}

	public int IFEMCGMHADD
	{
		get
		{
			return KOCKCMNHPMC();
		}
	}

	public int OGCIGFBOBMD
	{
		get
		{
			return IHKKEEOCOOF();
		}
	}

	public int FICBMNCMJIG
	{
		get
		{
			return INAOPLIFJEJ();
		}
	}

	public ModelObject KJDFJPBIGJC
	{
		get
		{
			return get_Model();
		}
	}

	public float BOGHNBAKCEL
	{
		get
		{
			return KJFIBMMOEPI();
		}
	}

	public float PCIBKEOCFAO
	{
		get
		{
			return PHHHEGOBAPB();
		}
	}

	public List<IntervalAnimation> GDAGDHGLKPB
	{
		get
		{
			return PCKKMNHDDMP();
		}
	}

	public InfoAnimation FGICHADOEHF
	{
		get
		{
			return NNMAFFCCMHC();
		}
		set
		{
			DBDJHIHLCFD(value);
		}
	}

	public InfoAnimation OGOCAHDGAKO
	{
		get
		{
			return JJBEAOPDGCO();
		}
	}

	public List<ModelEdge> JNCNOKPDMCM
	{
		get
		{
			return CPNOFKIMMCK();
		}
	}

	public int BJDFMKOCNBN
	{
		get
		{
			return LPFPGDJALED();
		}
	}

	public int NFIMGNEINNI
	{
		get
		{
			return NEBJGKODIKP();
		}
	}

	public int BBGDBGIPBJP
	{
		get
		{
			return NODAINEDAKJ();
		}
	}

	public int CAIOJLHLNAE
	{
		get
		{
			return MEOKDFJHEKC();
		}
	}

	public int HFOEPOPBGBB
	{
		get
		{
			return HILLKPNMCIP();
		}
	}

	public int MCMNPBAIMFN
	{
		get
		{
			return PIDJIKLOKJC();
		}
	}

	public ModelNode AFLPHBDFMGA
	{
		get
		{
			return CJELIBMCCMA();
		}
	}

	public ModelAnimation(ModelObject ACENLMONNPA)
	{
		LNGDODBCMEB = null;
		NMGJKMEDDCB = null;
		LOOMFPAEADA = null;
		CDMBBKPCMPP = null;
		CNFABIDNBOH = -3;
		_Model = ACENLMONNPA;
		MDLBEBOGOGK = false;
		JMKAHNADIOI = 1;
		LBHPMJDHAEM = 0;
		KFGEBGBEJBC = null;
		GKGEBAKLIDH = 0;
		IDKMDLCEHBK = 0;
		OLFJCLJKHIF = 0;
		FGFGGGKDJLN = int.MaxValue;
		DCJLLPNFIMF = 0f;
		BAOONIGFBMB = null;
		FJDKCIHGLLM = null;
		IPGAAGOANDE = false;
		NINBIDKEHKD = 0;
		ALKLIKKIDCM = 0;
		MHEIFCGAOHP = false;
		GJGDKFAAGOD = null;
		PAICIELHBHA = null;
		MIDMNJKJOFO = 0;
		INFAGPDFGNL = false;
		KGIEFJNJFOH = ACENLMONNPA.get_Model().NJDJHGDMCIJ() != null;
		GLOJGJIBABF = 1f;
		NBLBKLANDNC = 0f;
		ONHMMDAOGIM = false;
		KJODFLDMCFP = false;
		PMIELLEMGJK = 0f;
		GEPFNKEMDEE = 0;
		NLANLJEDHKJ = false;
		Stop();
	}

	public ModelAnimation ODHOECEPOFK()
	{
		return CDMBBKPCMPP;
	}

	public void CBKLDPIBGHD(ModelAnimation value)
	{
		CDMBBKPCMPP = value;
	}

	public ModelAnimation OJKLPPNCONP()
	{
		return LOOMFPAEADA;
	}

	public void NFEGCGJIICB(ModelAnimation value)
	{
		LOOMFPAEADA = value;
	}

	public bool NMEEPBDJHMG()
	{
		return MDLBEBOGOGK;
	}

	public int KFCNPADAMHA()
	{
		return JMKAHNADIOI;
	}

	public void set_Sign(int value)
	{
		if (value < 0)
		{
			JMKAHNADIOI = -1;
			return;
		}
		if (value > 0)
		{
			JMKAHNADIOI = 1;
			return;
		}
		JMKAHNADIOI = 1;
		Debug.LogError("set sign value != -1 or 1");
	}

	public int BOIDEOFKBMK()
	{
		return LBHPMJDHAEM;
	}

	public ModelNode PKKDMELGFBE()
	{
		return KFGEBGBEJBC;
	}

	public int LOIJGOPOGMO()
	{
		return GKGEBAKLIDH;
	}

	public int BNALDMNOMHH()
	{
		return EJJNHDCIEAD;
	}

	public bool ANHGOGDEFCO()
	{
		return IPGAAGOANDE;
	}

	public float FHGNPPBLIIL()
	{
		if (MDLBEBOGOGK)
		{
			return DCJLLPNFIMF;
		}
		return 0f;
	}

	public float LPNPNBJGKJM()
	{
		return PMIELLEMGJK;
	}

	public int JFGEHNHLDJM()
	{
		return GEPFNKEMDEE;
	}

	public float LIBBBOCOCNP()
	{
		return NBLBKLANDNC;
	}

	public void set_ShiftWallDelta(float value)
	{
		NBLBKLANDNC = value;
	}

	public Vector3f HOPJDHOEKEN()
	{
		return JCLKMEAJOLO;
	}

	public void JCNEDHMGLKE(Vector3f value)
	{
		JCLKMEAJOLO.Set(value);
	}

	public int KOCKCMNHPMC()
	{
		return MIDMNJKJOFO;
	}

	public int IHKKEEOCOOF()
	{
		return NINBIDKEHKD;
	}

	public int INAOPLIFJEJ()
	{
		return ALKLIKKIDCM;
	}

	public ModelObject get_Model()
	{
		return _Model;
	}

	public float KJFIBMMOEPI()
	{
		return DNOJAJNAFAF;
	}

	public float PHHHEGOBAPB()
	{
		return DPEOGNBGKML;
	}

	public List<IntervalAnimation> PCKKMNHDDMP()
	{
		return KKNKJMCFIJK;
	}

	public InfoAnimation NNMAFFCCMHC()
	{
		return BAOONIGFBMB;
	}

	public void DBDJHIHLCFD(InfoAnimation value)
	{
		BAOONIGFBMB = value;
	}

	public InfoAnimation JJBEAOPDGCO()
	{
		return FJDKCIHGLLM;
	}

	public List<ModelEdge> CPNOFKIMMCK()
	{
		return ECNLLKIJIGP;
	}

	public void Render()
	{
		GEPFNKEMDEE++;
		if (MDLBEBOGOGK)
		{
			if (CNFABIDNBOH != -3)
			{
				CNFABIDNBOH++;
			}
			else if (IDKMDLCEHBK == 0)
			{
				BCGHHIIPCBJ = 0;
			}
			ShiftWall();
			int num = _Frames.FNEPPBAKIDP();
			if (isBuffer())
			{
				DrawFrame();
				BCGHHIIPCBJ++;
				CheckActionsOnFrame();
				if (KGIEFJNJFOH && !isBuffer() && OLFJCLJKHIF + 2 >= num)
				{
					if (INFAGPDFGNL)
					{
						SetBufferFrame(OLFJCLJKHIF, GKGEBAKLIDH + 1);
						OLFJCLJKHIF = GKGEBAKLIDH;
					}
					else
					{
						StopAnimation();
						LDFKBJAHGII(BAOONIGFBMB);
						DeleteAnimation();
					}
				}
			}
			else if (!KGIEFJNJFOH && OLFJCLJKHIF + 2 >= num)
			{
				if (INFAGPDFGNL)
				{
					SetBufferFrame(OLFJCLJKHIF, GKGEBAKLIDH + 1);
					OLFJCLJKHIF = GKGEBAKLIDH + 1;
					DrawFrame();
					BCGHHIIPCBJ++;
				}
				else
				{
					StopAnimation();
					LDFKBJAHGII(BAOONIGFBMB);
					DeleteAnimation();
				}
			}
			else
			{
				if (OLFJCLJKHIF + 2 < num && !isBuffer())
				{
					SetBufferFrame();
				}
				if (isBuffer())
				{
					DrawFrame();
				}
				BCGHHIIPCBJ++;
				IDKMDLCEHBK++;
				OLFJCLJKHIF++;
				NewFrame();
			}
		}
		else if (MHEIFCGAOHP && BAOONIGFBMB != null)
		{
			OLFJCLJKHIF += 3;
			NewFrame();
			MHEIFCGAOHP = false;
		}
	}

	public void RenderPhysics()
	{
		IDKMDLCEHBK++;
		OLFJCLJKHIF++;
		NewFrame();
	}

	public void StopAnimation()
	{
		MDLBEBOGOGK = false;
		Stop();
	}

	public void DeleteAnimation()
	{
		MDLBEBOGOGK = false;
		MHEIFCGAOHP = true;
	}

	public int LPFPGDJALED()
	{
		if (!BAOONIGFBMB.FBKGDALBNDJ)
		{
			return ((OLFJCLJKHIF > 2) ? (OLFJCLJKHIF - 2) : 0) + GKGEBAKLIDH;
		}
		return OLFJCLJKHIF;
	}

	public int NEBJGKODIKP()
	{
		return (!MDLBEBOGOGK) ? IDKMDLCEHBK : (((IDKMDLCEHBK > 2) ? (IDKMDLCEHBK - 2) : 0) + GKGEBAKLIDH);
	}

	public int NODAINEDAKJ()
	{
		return (IDKMDLCEHBK != 0) ? (IDKMDLCEHBK + GKGEBAKLIDH - 2) : (-3);
	}

	public int MEOKDFJHEKC()
	{
		return (IDKMDLCEHBK != 0) ? (IDKMDLCEHBK + GKGEBAKLIDH - 2) : (-3);
	}

	public int HILLKPNMCIP()
	{
		if (MDLBEBOGOGK)
		{
			return BCGHHIIPCBJ;
		}
		return 0;
	}

	public void Reset()
	{
		MDLBEBOGOGK = false;
		JMKAHNADIOI = 1;
		LBHPMJDHAEM = 0;
		KFGEBGBEJBC = null;
		GKGEBAKLIDH = 0;
		IDKMDLCEHBK = 0;
		OLFJCLJKHIF = 0;
		MHEIFCGAOHP = false;
		FGFGGGKDJLN = int.MaxValue;
		Stop();
	}

	public int PIDJIKLOKJC()
	{
		return BAOONIGFBMB.MNHGBPOIHKG;
	}

	public ModelNode AAPLMJGHIGI(int OKNNNLIPODI)
	{
		return _Model.NAMKCLGOPDD()[OKNNNLIPODI];
	}

	public ModelNode CJELIBMCCMA()
	{
		if (MDLBEBOGOGK)
		{
			return KFGEBGBEJBC;
		}
		return null;
	}

	public ModelNode EGHIDHMENEF(string name, int AOJJBKLCHJO)
	{
		if (!string.IsNullOrEmpty(name))
		{
			ModelNode lCDGOCIAIDK = _Model.EGHIDHMENEF(name);
			if (lCDGOCIAIDK != null)
			{
				ModelNode lCDGOCIAIDK2 = lCDGOCIAIDK.PKOPJAHFNJG();
				if (lCDGOCIAIDK2 == null)
				{
					return lCDGOCIAIDK;
				}
				char c = name[name.Length - 1];
				switch (AOJJBKLCHJO)
				{
				case 1:
					switch (c)
					{
					case '1':
						if (lCDGOCIAIDK.ICLEOFDKDIF().GILCBJJPKBK() < lCDGOCIAIDK2.ICLEOFDKDIF().GILCBJJPKBK())
						{
							return lCDGOCIAIDK2;
						}
						return lCDGOCIAIDK;
					case '2':
						if (lCDGOCIAIDK.ICLEOFDKDIF().GILCBJJPKBK() < lCDGOCIAIDK2.ICLEOFDKDIF().GILCBJJPKBK())
						{
							return lCDGOCIAIDK;
						}
						return lCDGOCIAIDK2;
					}
					break;
				case -1:
					switch (c)
					{
					case '1':
						if (lCDGOCIAIDK.ICLEOFDKDIF().GILCBJJPKBK() < lCDGOCIAIDK2.ICLEOFDKDIF().GILCBJJPKBK())
						{
							return lCDGOCIAIDK;
						}
						return lCDGOCIAIDK2;
					case '2':
						if (lCDGOCIAIDK.ICLEOFDKDIF().GILCBJJPKBK() < lCDGOCIAIDK2.ICLEOFDKDIF().GILCBJJPKBK())
						{
							return lCDGOCIAIDK2;
						}
						return lCDGOCIAIDK;
					}
					break;
				default:
					LLLOJBFMONN.Error("strange sign {0}", AOJJBKLCHJO);
					return lCDGOCIAIDK;
				}
			}
		}
		return null;
	}

	public void SetAligns(float JDJNFNGEDDP, float CCDPMGGMDLP, int CDNFFEFGLKN, int JNKHDFNCOGK)
	{
		NINBIDKEHKD = CDNFFEFGLKN;
		ALKLIKKIDCM = JNKHDFNCOGK;
		DNOJAJNAFAF = JDJNFNGEDDP;
		DPEOGNBGKML = CCDPMGGMDLP;
	}

	public void ShiftSequence(float HLBMDDOPKKL, float ELAKEOGEDPN = 0f, float PIIFLHIBODE = 0f)
	{
		_Frames.Shift(HLBMDDOPKKL, ELAKEOGEDPN, PIIFLHIBODE);
	}

	public void MirrorSequence()
	{
		if (JMKAHNADIOI == -1)
		{
			_Frames.NKHEGNLGJIG();
		}
	}

	public void ShiftBuffer(Vector3f OPNPKNEOALJ)
	{
		for (int i = 0; i < AHOBIIMFNEP.Count; i++)
		{
			List<Vector3f> list = AHOBIIMFNEP[i];
			for (int j = 0; j < list.Count; j++)
			{
				list[j].Add(OPNPKNEOALJ);
			}
		}
	}

	public bool PlayInfo(InfoAnimation DBOLBEOCEME, int AOJJBKLCHJO, bool NAJJLNDPNJC = true, bool HHJGACBCGBP = false, int BADKABIKMBD = -1)
	{
		if (DBOLBEOCEME != null)
		{
			if (GJGDKFAAGOD != null && GJGDKFAAGOD != DBOLBEOCEME && GJGDKFAAGOD != DBOLBEOCEME.IMFGMAAEMIC())
			{
				LLLOJBFMONN.Error("Animation error need: '{0}' (Priority {1}); Animation played: '{2}' (Priority {3})", GJGDKFAAGOD.Name, GJGDKFAAGOD.Priority, DBOLBEOCEME.Name, DBOLBEOCEME.Priority);
			}
			if (KGIEFJNJFOH)
			{
				ClearIntervals();
				ClearAttackingEdges();
			}
			int num = DBOLBEOCEME.GOBJCKFGIPA;
			if (HHJGACBCGBP)
			{
				int num2 = LPFPGDJALED();
				num = num2 + 1 + BADKABIKMBD;
			}
			else if (-1 < BADKABIKMBD)
			{
				num = BADKABIKMBD;
			}
			Stop();
			set_Sign(AOJJBKLCHJO);
			BAOONIGFBMB = DBOLBEOCEME;
			GKGEBAKLIDH = num;
			EJJNHDCIEAD = BAOONIGFBMB.LHHAGECFIOL;
			INFAGPDFGNL = BAOONIGFBMB.NCEKKNIMHAG();
			if (GKGEBAKLIDH > EJJNHDCIEAD - 1)
			{
				GKGEBAKLIDH = EJJNHDCIEAD - 1;
			}
			if (BAOONIGFBMB.DIHJOPGKGFO().Length == 0)
			{
				LLLOJBFMONN.Error("ModelAnimation::playInfo - empty animation \"{0}\"", BAOONIGFBMB.Name);
				MDLBEBOGOGK = false;
				return false;
			}
			AEKEELJMLDC = NAJJLNDPNJC;
			if (NAJJLNDPNJC)
			{
				SetInterruptFrames(BAOONIGFBMB.DFKIHADCFKG());
			}
			BAOONIGFBMB.HAILLLEPCHP(_Frames, num, !AEKEELJMLDC);
			PhysicsNodes();
			SetCurrentNode();
			MirrorNodes();
			ShiftPoints();
			int gNDPBMIJEMH = DBOLBEOCEME.DFKIHADCFKG();
			AHOBIIMFNEP.CPCAJIKOIEE(gNDPBMIJEMH);
			MDLBEBOGOGK = true;
			IDKMDLCEHBK = 0;
			OLFJCLJKHIF = 0;
			FGFGGGKDJLN = int.MaxValue;
			BCGHHIIPCBJ = 0;
			PLLOJCCNDOH = int.MaxValue;
			CNFABIDNBOH = -3;
			MHEIFCGAOHP = false;
			BFDLFAHGKHP.Reset();
			if (!BAOONIGFBMB.HOPDDLNABCG())
			{
				AGAFKHLPLCA.Set(BAOONIGFBMB.LBJFGCFGMDI());
				Vector3f aGAFKHLPLCA = AGAFKHLPLCA;
				aGAFKHLPLCA.JPFALPBDBAP(aGAFKHLPLCA.GILCBJJPKBK() * (float)KFCNPADAMHA());
			}
			DBCBOPONOBE.Set(BAOONIGFBMB.NCENGIOMKOF());
			Vector3f dBCBOPONOBE = DBCBOPONOBE;
			dBCBOPONOBE.JPFALPBDBAP(dBCBOPONOBE.GILCBJJPKBK() * (float)KFCNPADAMHA());
			SetDistanceAlign();
			PAMICDLAMHC(BAOONIGFBMB);
			GJGDKFAAGOD = null;
			return true;
		}
		return false;
	}

	public void SetIntervals()
	{
		ClearAttackingEdges();
		int num = LPFPGDJALED();
		BAOONIGFBMB.GetIntervals(num, KKNKJMCFIJK, OACCPPMJMML, PAICIELHBHA);
		foreach (IntervalAnimation item in KKNKJMCFIJK)
		{
			if (item.Type == IntervalAnimation.NGAJJDIEDGF.INTERVAL_ATTACK)
			{
				SetAttackingEdges(item);
			}
			int num2 = ((GKGEBAKLIDH > item.Start) ? GKGEBAKLIDH : item.Start);
			if (num2 == num)
			{
				OnStartIntervals(item);
			}
		}
		foreach (IntervalAnimation item2 in OACCPPMJMML)
		{
			OnEndIntervals(item2);
		}
	}

	public void ClearIntervals()
	{
		foreach (IntervalAnimation item in KKNKJMCFIJK)
		{
			OnEndIntervals(item);
		}
	}

	public void ResetIntervals()
	{
		KKNKJMCFIJK.Clear();
	}

	public void OnStartIntervals(IntervalAnimation CHCGJBLDPML)
	{
		CallEvent(2, CHCGJBLDPML);
	}

	public void OnEndIntervals(IntervalAnimation CHCGJBLDPML)
	{
		CallEvent(3, CHCGJBLDPML);
	}

	public void PAMICDLAMHC(InfoAnimation DBOLBEOCEME)
	{
		CallEvent(0, DBOLBEOCEME);
	}

	public void LDFKBJAHGII(InfoAnimation DBOLBEOCEME)
	{
		if (!KGIEFJNJFOH)
		{
			ClearIntervals();
			ClearAttackingEdges();
		}
		CallEvent(1, DBOLBEOCEME);
	}

	public void SetAttackingEdges(IntervalAnimation CHCGJBLDPML)
	{
		IntervalAttack hFIIPNLCIEE = CHCGJBLDPML as IntervalAttack;
		List<string> list = hFIIPNLCIEE.IKPJJAEIOCG();
		foreach (string item in list)
		{
			string text = item;
			if (ANHGOGDEFCO())
			{
				char c = text[item.Length - 2];
				char c2 = text[item.Length - 1];
				if (c == '_')
				{
					switch (c2)
					{
					case '1':
						text = text.Remove(text.Length - 1, 1) + "2";
						break;
					case '2':
						text = text.Remove(text.Length - 1, 1) + "1";
						break;
					}
				}
			}
			ModelEdge nAKBKCDKEHF = _Model.CLBHEMEAAEN(text);
			if (nAKBKCDKEHF != null)
			{
				ECNLLKIJIGP.Add(nAKBKCDKEHF);
			}
		}
	}

	public void ClearAttackingEdges()
	{
		ECNLLKIJIGP.Clear();
	}

	public IntervalAnimation HDJBHPOGKNJ(IntervalAnimation.NGAJJDIEDGF LFLGCDNKNJI)
	{
		for (int i = 0; i < KKNKJMCFIJK.Count; i++)
		{
			if (LFLGCDNKNJI == KKNKJMCFIJK[i].Type)
			{
				return KKNKJMCFIJK[i];
			}
		}
		return null;
	}

	public IntervalAnimation HDJBHPOGKNJ(string JDPPEBHEJPI)
	{
		for (int i = 0; i < KKNKJMCFIJK.Count; i++)
		{
			if (KKNKJMCFIJK[i].Name == JDPPEBHEJPI)
			{
				return KKNKJMCFIJK[i];
			}
		}
		return null;
	}

	public void RemoveInterval(IntervalAnimation.NGAJJDIEDGF LFLGCDNKNJI)
	{
		for (int num = KKNKJMCFIJK.Count - 1; num >= 0; num--)
		{
			if (LFLGCDNKNJI == KKNKJMCFIJK[num].Type)
			{
				KKNKJMCFIJK.RemoveAt(num);
			}
		}
	}

	public void RemoveInterval(string name)
	{
		for (int num = KKNKJMCFIJK.Count - 1; num >= 0; num--)
		{
			if (name == KKNKJMCFIJK[num].Name)
			{
				KKNKJMCFIJK.RemoveAt(num);
			}
		}
	}

	public void RemoveIntervals(List<string> NFLDEGMEJAK)
	{
		foreach (string item in NFLDEGMEJAK)
		{
			RemoveInterval(item);
		}
	}

	public bool CheckIntervals(List<string> NFLDEGMEJAK)
	{
		for (int i = 0; i < NFLDEGMEJAK.Count; i++)
		{
			if (HDJBHPOGKNJ(NFLDEGMEJAK[i]) != null)
			{
				return true;
			}
		}
		return false;
	}

	public void Init()
	{
		LNGDODBCMEB = _Model.EGHIDHMENEF("NHeel_1");
		NMGJKMEDDCB = _Model.EGHIDHMENEF("NHeel_2");
	}

	public void PONNDMHBGJK(IntervalAnimation.NGAJJDIEDGF LFLGCDNKNJI)
	{
		if (PAICIELHBHA == null)
		{
			PAICIELHBHA = new HashSet<IntervalAnimation.NGAJJDIEDGF>();
		}
		PAICIELHBHA.Add(LFLGCDNKNJI);
	}

	public void FDDONCMEAHA()
	{
		PAICIELHBHA = null;
	}

	public void PJDPCLCOGFP(EventAnimation.EECEJKADLCK LFLGCDNKNJI)
	{
		if (BAOONIGFBMB == null)
		{
			return;
		}
		List<ActionAnimation> dJBAIAKOIHM = BAOONIGFBMB.ODACDCDONJE.DJBAIAKOIHM;
		if (dJBAIAKOIHM.Count <= 0)
		{
			return;
		}
		List<ActionAnimation> list = new List<ActionAnimation>();
		foreach (ActionAnimation item in dJBAIAKOIHM)
		{
			if (item.NeedStart(LFLGCDNKNJI))
			{
				list.Add(item);
			}
		}
		if (list.Count > 0)
		{
			CallEvent(4, list);
		}
	}

	public void MoveByVelocity(Vector3f BLPIMOCGMKJ)
	{
		List<ModelNode> list = _Model.NAMKCLGOPDD();
		foreach (ModelNode item in list)
		{
			item.OIEPNGBEECN();
			item.ICLEOFDKDIF().Add(BLPIMOCGMKJ);
		}
	}

	public static bool CalcIsMirror(ModelObject IPKAHIMPMEG, string PCAMJGFDBID, int AOJJBKLCHJO, Vector3[] MCPABOHDBLO, bool AAGPPKAOHGI = true)
	{
		ModelNode lCDGOCIAIDK = IPKAHIMPMEG.EGHIDHMENEF(PCAMJGFDBID);
		ModelNode lCDGOCIAIDK2 = ((lCDGOCIAIDK == null) ? null : lCDGOCIAIDK.PKOPJAHFNJG());
		if (lCDGOCIAIDK != null && lCDGOCIAIDK2 != null)
		{
			return CalcIsMirror(lCDGOCIAIDK, lCDGOCIAIDK2, AOJJBKLCHJO, MCPABOHDBLO, AAGPPKAOHGI);
		}
		LLLOJBFMONN.Error("ModelAnimation::mirrorNodes - nodes not found: \"{0}\"", PCAMJGFDBID);
		return false;
	}

	public static bool CalcIsMirror(ModelNode NMBEADHHHFH, ModelNode OKCKNALOCCK, int AOJJBKLCHJO, Vector3[] MCPABOHDBLO, bool AAGPPKAOHGI = true)
	{
		if (AOJJBKLCHJO == -1)
		{
			Util.Swap(ref NMBEADHHHFH, ref OKCKNALOCCK);
		}
		int FBENKEEDIKJ = NMBEADHHHFH.ANAECCFDHMI();
		int PGKPNBGIGEI = OKCKNALOCCK.ANAECCFDHMI();
		if (!AAGPPKAOHGI && AOJJBKLCHJO == -1)
		{
			Util.Swap(ref FBENKEEDIKJ, ref PGKPNBGIGEI);
		}
		return NMBEADHHHFH.ICLEOFDKDIF().GILCBJJPKBK() >= OKCKNALOCCK.ICLEOFDKDIF().GILCBJJPKBK() != MCPABOHDBLO[FBENKEEDIKJ].x >= MCPABOHDBLO[PGKPNBGIGEI].x;
	}

	public static bool CalcIsMirror(ModelObject IPKAHIMPMEG, string PCAMJGFDBID, int AOJJBKLCHJO, List<Vector3f> MCPABOHDBLO, bool AAGPPKAOHGI = true)
	{
		ModelNode lCDGOCIAIDK = IPKAHIMPMEG.EGHIDHMENEF(PCAMJGFDBID);
		ModelNode lCDGOCIAIDK2 = ((lCDGOCIAIDK == null) ? null : lCDGOCIAIDK.PKOPJAHFNJG());
		if (lCDGOCIAIDK != null && lCDGOCIAIDK2 != null)
		{
			return CalcIsMirror(lCDGOCIAIDK, lCDGOCIAIDK2, AOJJBKLCHJO, MCPABOHDBLO, AAGPPKAOHGI);
		}
		LLLOJBFMONN.Error("ModelAnimation::mirrorNodes - nodes not found: \"{0}\"", PCAMJGFDBID);
		return false;
	}

	public static bool CalcIsMirror(ModelNode NMBEADHHHFH, ModelNode OKCKNALOCCK, int AOJJBKLCHJO, List<Vector3f> MCPABOHDBLO, bool AAGPPKAOHGI = true)
	{
		if (AOJJBKLCHJO == -1)
		{
			Util.Swap(ref NMBEADHHHFH, ref OKCKNALOCCK);
		}
		int FBENKEEDIKJ = NMBEADHHHFH.ANAECCFDHMI();
		int PGKPNBGIGEI = OKCKNALOCCK.ANAECCFDHMI();
		if (!AAGPPKAOHGI && AOJJBKLCHJO == -1)
		{
			Util.Swap(ref FBENKEEDIKJ, ref PGKPNBGIGEI);
		}
		return NMBEADHHHFH.ICLEOFDKDIF().GILCBJJPKBK() >= OKCKNALOCCK.ICLEOFDKDIF().GILCBJJPKBK() != MCPABOHDBLO[FBENKEEDIKJ].GILCBJJPKBK() >= MCPABOHDBLO[PGKPNBGIGEI].GILCBJJPKBK();
	}

	private void SetDistanceAlign()
	{
		ModelNode lCDGOCIAIDK = CJELIBMCCMA();
		float bDHBFDMBMFM = BDHBFDMBMFM;
		int gOBJCKFGIPA = BAOONIGFBMB.GOBJCKFGIPA;
		int num = LOIJGOPOGMO();
		MIDMNJKJOFO = (num - gOBJCKFGIPA) * (BAOONIGFBMB.MNHGBPOIHKG + 1);
		float num2 = 0f;
		if (0 < MIDMNJKJOFO && BAOONIGFBMB.ODACDCDONJE.ILOEBFFAEAN.CKBGFODEBAJ == InfoAnimation.DOLCEABGNGA.ObjectNodes)
		{
			int num3;
			if (ANHGOGDEFCO())
			{
				ModelNode lCDGOCIAIDK2 = lCDGOCIAIDK.PKOPJAHFNJG();
				num3 = ((lCDGOCIAIDK2 == null) ? BOIDEOFKBMK() : lCDGOCIAIDK2.ANAECCFDHMI());
			}
			else
			{
				num3 = BOIDEOFKBMK();
			}
			num2 = BAOONIGFBMB.DIHJOPGKGFO()[num][num3].x - BAOONIGFBMB.DIHJOPGKGFO()[gOBJCKFGIPA][num3].x;
		}
		bDHBFDMBMFM = (DCJLLPNFIMF = bDHBFDMBMFM + num2 * (float)KFCNPADAMHA());
		if (!NLANLJEDHKJ)
		{
			PMIELLEMGJK = bDHBFDMBMFM;
			FJDKCIHGLLM = BAOONIGFBMB;
			GEPFNKEMDEE = -4;
			NLANLJEDHKJ = true;
		}
	}

	private void Stop()
	{
		_Frames.Reset();
		PLLOJCCNDOH = int.MaxValue;
	}

	private void DrawFrame()
	{
		List<ModelNode> list = _Model.NAMKCLGOPDD();
		PGBLJDCEIOM();
		Vector3f eMAFACPEPDK = new Vector3f();
		for (int i = 0; i < AHOBIIMFNEP.Count; i++)
		{
			ModelNode lCDGOCIAIDK = list[i];
			if (!_Model.EDJFLMILEBA() || (_Model.EDJFLMILEBA() && !lCDGOCIAIDK.EDJFLMILEBA()))
			{
				lCDGOCIAIDK.OIEPNGBEECN();
				eMAFACPEPDK.Set(AHOBIIMFNEP[i][PLLOJCCNDOH]);
				eMAFACPEPDK.Add(BFDLFAHGKHP);
				lCDGOCIAIDK.AMPCKAIPIHH(eMAFACPEPDK);
				lCDGOCIAIDK.OHMNDOKBGGA(true);
			}
		}
		if (BAOONIGFBMB.NBOLIGLFFEL() != 0f)
		{
			CKNIEFMEDDA();
		}
		int count = AHOBIIMFNEP[0].Count;
		if (PLLOJCCNDOH == count - 1)
		{
			PLLOJCCNDOH++;
		}
		else
		{
			PLLOJCCNDOH += ((ECNLDOICIND <= 1) ? 1 : (ECNLDOICIND / GameUtils.GGBABPJBGJB()));
			if (PLLOJCCNDOH > count - 1)
			{
				PLLOJCCNDOH = count - 1;
			}
		}
		if (PLLOJCCNDOH >= ECNLDOICIND)
		{
			KJODFLDMCFP = true;
		}
		else
		{
			KJODFLDMCFP = false;
		}
		KGOGFKAPIOC();
	}

	private void SetInterruptFrames(int JEJGJGLMKDM)
	{
		_Frames.InterruptFramesSeted(JEJGJGLMKDM);
		int num = (BAOONIGFBMB.MNHGBPOIHKG + 1) / 2;
		KeyFrames.Frame cJMFONMNFBI = _Frames.KLNOLPIADNN(0);
		KeyFrames.Frame cJMFONMNFBI2 = _Frames.KLNOLPIADNN(1);
		List<ModelNode> list = _Model.NAMKCLGOPDD();
		for (int i = 0; i < cJMFONMNFBI.Size; i++)
		{
			Vector3f eMAFACPEPDK = list[i].ICLEOFDKDIF();
			Vector3f eMAFACPEPDK2 = list[i].FOGHEPNAPLC();
			float lHNJJFDIJKK = (eMAFACPEPDK.GILCBJJPKBK() - eMAFACPEPDK2.GILCBJJPKBK()) * (float)num;
			float fFFHIOALHGM = (eMAFACPEPDK.OBIMBNIBEFG() - eMAFACPEPDK2.OBIMBNIBEFG()) * (float)num;
			float pDCENMEKIAP = (eMAFACPEPDK.KMFEKANLCFO() - eMAFACPEPDK2.KMFEKANLCFO()) * (float)num;
			cJMFONMNFBI.Data[i].Set(eMAFACPEPDK);
			cJMFONMNFBI.Data[i].EHGLHOGAIDI(lHNJJFDIJKK, fFFHIOALHGM, pDCENMEKIAP);
			cJMFONMNFBI2.Data[i].Set(eMAFACPEPDK);
			cJMFONMNFBI2.Data[i].Add(lHNJJFDIJKK, fFFHIOALHGM, pDCENMEKIAP);
		}
	}

	private void ShiftPoints()
	{
		InfoAnimation.MovePivot iLOEBFFAEAN = BAOONIGFBMB.ODACDCDONJE.ILOEBFFAEAN;
		ModelAnimation oJIEPADIEDE = DMDKINMOAKM(iLOEBFFAEAN.BAFGOANMBMI);
		ModelAnimation oJIEPADIEDE2 = DMDKINMOAKM(iLOEBFFAEAN.EDBLMNIEKBD);
		Vector3f eMAFACPEPDK = new Vector3f();
		Vector3f eMAFACPEPDK2 = new Vector3f();
		if (oJIEPADIEDE2 == null)
		{
			oJIEPADIEDE2 = this;
		}
		switch (iLOEBFFAEAN.CKBGFODEBAJ)
		{
		case InfoAnimation.DOLCEABGNGA.ObjectPivot:
			eMAFACPEPDK.Set(_Frames.KLNOLPIADNN(2).Data[LBHPMJDHAEM]);
			break;
		case InfoAnimation.DOLCEABGNGA.ObjectNodes:
		{
			int num = ((!oJIEPADIEDE.ANHGOGDEFCO() || iLOEBFFAEAN.BAHKGNNELBL <= -1) ? iLOEBFFAEAN.CLIPMJNJDKI : iLOEBFFAEAN.BAHKGNNELBL);
			num = ((num >= 0 && num < _Frames.KLNOLPIADNN(2).Size) ? num : 0);
			eMAFACPEPDK.Set(_Frames.KLNOLPIADNN(2).Data[num]);
			break;
		}
		case InfoAnimation.DOLCEABGNGA.ObjectAnimation:
			eMAFACPEPDK.Reset();
			break;
		case InfoAnimation.DOLCEABGNGA.ObjectWall:
			eMAFACPEPDK.Reset();
			eMAFACPEPDK.JPFALPBDBAP((KFCNPADAMHA() == 1 != (iLOEBFFAEAN.BLODCIGDJFK == "Back")) ? (0f - DPEOGNBGKML) : (0f - DNOJAJNAFAF));
			break;
		}
		switch (iLOEBFFAEAN.HHPAGAOGGLP)
		{
		case InfoAnimation.DOLCEABGNGA.ObjectPivot:
			if (oJIEPADIEDE2.PKKDMELGFBE() != null)
			{
				eMAFACPEPDK2.Set(oJIEPADIEDE2.PKKDMELGFBE().ICLEOFDKDIF());
			}
			break;
		case InfoAnimation.DOLCEABGNGA.ObjectNodes:
		{
			int oKNNNLIPODI = ((!oJIEPADIEDE2.ANHGOGDEFCO() || iLOEBFFAEAN.KFMGKDOLKGN <= -1) ? iLOEBFFAEAN.JPKDOHPGEBA : iLOEBFFAEAN.KFMGKDOLKGN);
			eMAFACPEPDK2.Set(oJIEPADIEDE2.AAPLMJGHIGI(oKNNNLIPODI).ICLEOFDKDIF());
			break;
		}
		case InfoAnimation.DOLCEABGNGA.ObjectAnimation:
			eMAFACPEPDK2.Set(oJIEPADIEDE2.HOPJDHOEKEN());
			break;
		case InfoAnimation.DOLCEABGNGA.ObjectWall:
			eMAFACPEPDK2.Reset();
			eMAFACPEPDK2.JPFALPBDBAP((KFCNPADAMHA() == 1 != (iLOEBFFAEAN.PMILDGBBLMF == "Back")) ? DPEOGNBGKML : DNOJAJNAFAF);
			break;
		}
		eMAFACPEPDK2.JPFALPBDBAP(eMAFACPEPDK2.GILCBJJPKBK() + (float)KFCNPADAMHA() * iLOEBFFAEAN.LDNPHPGEOPJ.GILCBJJPKBK());
		eMAFACPEPDK2.IBNFLLGPOLD(eMAFACPEPDK2.OBIMBNIBEFG() + iLOEBFFAEAN.LDNPHPGEOPJ.OBIMBNIBEFG());
		BDHBFDMBMFM = eMAFACPEPDK2.GILCBJJPKBK();
		JCLKMEAJOLO.Set(Vector3f.MJOKEBGPHKB(eMAFACPEPDK2, eMAFACPEPDK));
		ShiftSequence((!iLOEBFFAEAN.HNDMMOGMOAN) ? 0f : JCLKMEAJOLO.GILCBJJPKBK(), (!iLOEBFFAEAN.IMCDDINEFKC) ? 0f : JCLKMEAJOLO.OBIMBNIBEFG(), (!iLOEBFFAEAN.GHKGPDMMHHK) ? 0f : JCLKMEAJOLO.KMFEKANLCFO());
		if (string.IsNullOrEmpty(iLOEBFFAEAN.BONDKHGGCDD))
		{
			return;
		}
		ModelNode lCDGOCIAIDK = _Model.EGHIDHMENEF(iLOEBFFAEAN.BONDKHGGCDD);
		int index = lCDGOCIAIDK.ANAECCFDHMI();
		eMAFACPEPDK = _Frames.KLNOLPIADNN(2).Data[index];
		Vector3f bEHOPOPCJGB = new Vector3f(Vector3f.MJOKEBGPHKB(eMAFACPEPDK, lCDGOCIAIDK.ICLEOFDKDIF()));
		Vector3f bEHOPOPCJGB2 = new Vector3f(Vector3f.MJOKEBGPHKB(eMAFACPEPDK, lCDGOCIAIDK.FOGHEPNAPLC()));
		List<ModelNode> list = _Model.NAMKCLGOPDD();
		int count = list.Count;
		foreach (ModelNode item in list)
		{
			item.ICLEOFDKDIF().Add(bEHOPOPCJGB);
			item.FOGHEPNAPLC().Add(bEHOPOPCJGB2);
		}
	}

	private float IHPGHCDAHKF(List<Vector3f> frame)
	{
		return KFGEBGBEJBC.ICLEOFDKDIF().GILCBJJPKBK() - frame[LBHPMJDHAEM].GILCBJJPKBK();
	}

	private float EDJHNDGAFMJ(List<Vector3f> frame)
	{
		return KFGEBGBEJBC.ICLEOFDKDIF().OBIMBNIBEFG() - frame[LBHPMJDHAEM].OBIMBNIBEFG();
	}

	private Vector3f ONFBGCBIFJL(List<Vector3f> frame)
	{
		return Vector3f.MJOKEBGPHKB(KFGEBGBEJBC.ICLEOFDKDIF(), frame[LBHPMJDHAEM]);
	}

	private Vector3f ONFBGCBIFJL()
	{
		return Vector3f.op_Implicit(default(Vector3));
	}

	private void MirrorNodes()
	{
		MirrorSequence();
		InfoAnimation.MirrorNode cIEEMJJCABC = BAOONIGFBMB.ECCLELFHNHE();
		if (cIEEMJJCABC.DAIAOBAEDCB())
		{
			return;
		}
		IPGAAGOANDE = CalcIsMirror(_Model, cIEEMJJCABC.FJANLLCDPCP(), KFCNPADAMHA(), _Frames.KLNOLPIADNN(2).Data);
		if (IPGAAGOANDE)
		{
			int num = _Model.GetNodeIDByPairName(LBHPMJDHAEM);
			if (num > -1)
			{
				LBHPMJDHAEM = num;
				KFGEBGBEJBC = _Model.NAMKCLGOPDD()[LBHPMJDHAEM];
			}
			BAOONIGFBMB.FNGJFDNAPPH(_Model.DJNNIKHGGFO(), _Frames, 2);
		}
	}

	private bool isBuffer()
	{
		return 0 < AHOBIIMFNEP.Count && PLLOJCCNDOH < AHOBIIMFNEP[0].Count;
	}

	private void SetBufferFrame(int PADOCECKBPE = -1, int AGOKCKGOKLI = -1)
	{
		if (PADOCECKBPE == -1 || AGOKCKGOKLI == -1)
		{
			PADOCECKBPE = OLFJCLJKHIF;
			AGOKCKGOKLI = OLFJCLJKHIF + 1;
		}
		CNFABIDNBOH = 1;
		ECNLDOICIND = GameUtils.GGBABPJBGJB();
		GLOJGJIBABF = 1f / (float)ECNLDOICIND;
		int bLJGEOEHIGP = (PIDJIKLOKJC() + 1) * ECNLDOICIND;
		List<Vector3f> aLAKNMCKLFI = _Frames.KLNOLPIADNN(PADOCECKBPE).Data;
		List<Vector3f> aLAKNMCKLFI2 = _Frames.KLNOLPIADNN(AGOKCKGOKLI).Data;
		List<Vector3f> aLAKNMCKLFI3 = _Frames.KLNOLPIADNN(AGOKCKGOKLI + 1).Data;
		Bezier kHKJJAKJPAJ = new Bezier(bLJGEOEHIGP);
		for (int i = 0; i < AHOBIIMFNEP.Count; i++)
		{
			kHKJJAKJPAJ.CFCFNHONDML(aLAKNMCKLFI[i], aLAKNMCKLFI2[i], aLAKNMCKLFI3[i], AHOBIIMFNEP[i]);
		}
		PLLOJCCNDOH = 0;
	}

	private void ShiftWall()
	{
		bool flag = false;
		float num = 0f;
		if (CDMBBKPCMPP != null && BAOONIGFBMB.ALFPDPEEJFO)
		{
			num = CDMBBKPCMPP.LIBBBOCOCNP();
			flag = true;
		}
		int num2 = OLFJCLJKHIF + 2;
		if (num2 > _Frames.OLINNGEMHMG() - 1)
		{
			set_ShiftWallDelta(0f);
			return;
		}
		int num3 = ((JMKAHNADIOI != -1) ? ALKLIKKIDCM : NINBIDKEHKD);
		int num4 = ((JMKAHNADIOI != 1) ? ALKLIKKIDCM : NINBIDKEHKD);
		int index = ((_Model.CJELIBMCCMA() == null) ? _Model.NAMKCLGOPDD()[0].ANAECCFDHMI() : _Model.CJELIBMCCMA().ANAECCFDHMI());
		float num5 = _Frames.KLNOLPIADNN(num2).Data[index].GILCBJJPKBK();
		if (BAOONIGFBMB.HFBOLCPHMBB && !flag)
		{
			return;
		}
		if (flag)
		{
			num5 = num;
		}
		else
		{
			num5 = ((num5 < DNOJAJNAFAF + (float)num3) ? (num5 - (DNOJAJNAFAF + (float)num3)) : ((!(num5 > DPEOGNBGKML - (float)num4)) ? 0f : (num5 - (DPEOGNBGKML - (float)num4))));
			set_ShiftWallDelta(num5);
			if (num5 == 0f)
			{
				return;
			}
		}
		int num6 = ((_Frames.OLINNGEMHMG() >= num2 + 2) ? (num2 + 2) : _Frames.OLINNGEMHMG());
		for (int i = num2; i < num6; i++)
		{
			KeyFrames.Frame cJMFONMNFBI = _Frames.KLNOLPIADNN(i);
			for (int j = 0; j < cJMFONMNFBI.Size; j++)
			{
				Vector3f eMAFACPEPDK = cJMFONMNFBI.Data[j];
				eMAFACPEPDK.JPFALPBDBAP(eMAFACPEPDK.GILCBJJPKBK() - num5);
			}
		}
	}

	private void NewFrame()
	{
		if (BAOONIGFBMB != null)
		{
			int num = LPFPGDJALED();
			if (num != FGFGGGKDJLN)
			{
				FGFGGGKDJLN = num;
				SetIntervals();
				ONHMMDAOGIM = true;
			}
		}
		CheckActionsOnFrame();
	}

	private void SetCurrentNode()
	{
		if (BAOONIGFBMB.ODACDCDONJE.ILOEBFFAEAN.CLIPMJNJDKI > -1)
		{
			LBHPMJDHAEM = BAOONIGFBMB.ODACDCDONJE.ILOEBFFAEAN.CLIPMJNJDKI;
			KFGEBGBEJBC = _Model.NAMKCLGOPDD()[LBHPMJDHAEM];
		}
		else
		{
			KFGEBGBEJBC = null;
			LLLOJBFMONN.Write("_AnimationInfo.moveInside.align.pivotID == -1 " + BAOONIGFBMB.Name);
		}
	}

	private void PhysicsNodes()
	{
		_Model.FLPIFFOGDBF();
		List<ModelNode> list = _Model.NAMKCLGOPDD();
		int num = BAOONIGFBMB.DFKIHADCFKG();
		if (list.Count < num)
		{
			LLLOJBFMONN.Error("In {0} animation {1} nodes, but in model only {2}", BAOONIGFBMB.Name, num, list.Count);
		}
		for (int i = 0; i < num; i++)
		{
			list[i].KCDIAMOLAKB();
		}
	}

	private ModelAnimation DMDKINMOAKM(ModelType.KEIDBIOIFGA HJMMACIELFG)
	{
		switch (HJMMACIELFG)
		{
		case ModelType.KEIDBIOIFGA.MODEL_NULL:
		case ModelType.KEIDBIOIFGA.MODEL_THIS:
			return this;
		case ModelType.KEIDBIOIFGA.MODEL_PARENT:
			return CDMBBKPCMPP;
		case ModelType.KEIDBIOIFGA.MODEL_OTHER:
			return LOOMFPAEADA;
		default:
			LLLOJBFMONN.Error("ModelAnimation::getPlayerAnimation - unknown type: {0}", HJMMACIELFG);
			return null;
		}
	}

	private void CheckActionsOnFrame()
	{
		if (!ONHMMDAOGIM || !KJODFLDMCFP)
		{
			return;
		}
		ONHMMDAOGIM = false;
		List<ActionAnimation> dJBAIAKOIHM = BAOONIGFBMB.ODACDCDONJE.DJBAIAKOIHM;
		if (dJBAIAKOIHM.Count <= 0)
		{
			return;
		}
		int dBEDGEMEFNB = LPFPGDJALED();
		List<ActionAnimation> list = new List<ActionAnimation>();
		foreach (ActionAnimation item in dJBAIAKOIHM)
		{
			if (item.NeedStart(dBEDGEMEFNB))
			{
				list.Add(item);
			}
		}
		if (list.Count > 0)
		{
			CallEvent(4, list);
		}
	}

	private void PGBLJDCEIOM()
	{
		PGBLJDCEIOM(AGAFKHLPLCA);
	}

	private void PGBLJDCEIOM(Vector3f BLPIMOCGMKJ)
	{
		Vector3f eMAFACPEPDK = new Vector3f(BLPIMOCGMKJ);
		if (GLOJGJIBABF != 1f)
		{
			eMAFACPEPDK.Multiply(GLOJGJIBABF);
		}
		BFDLFAHGKHP.Add(eMAFACPEPDK);
	}

	private void KGOGFKAPIOC()
	{
		KGOGFKAPIOC(DBCBOPONOBE);
	}

	private void KGOGFKAPIOC(Vector3f IALBIAFLGFI)
	{
		Vector3f eMAFACPEPDK = new Vector3f(IALBIAFLGFI);
		if (GLOJGJIBABF != 1f)
		{
			eMAFACPEPDK.Multiply(GLOJGJIBABF);
		}
		AGAFKHLPLCA.Add(eMAFACPEPDK);
	}

	private void OBOOKFFKMIB()
	{
	}

	private void CKNIEFMEDDA()
	{
	}
}
