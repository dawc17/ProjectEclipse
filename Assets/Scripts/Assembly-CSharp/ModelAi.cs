using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class ModelAi
{
	public enum CBGBLIPAMGA
	{
		SetWaitNone = 0,
		SetWaitRandAttack = 1,
		SetWaitRandUnint = 2,
		SetWaitAnimationLength = 3
	}

	public enum CNDJFAIJOOH
	{
		Nostate = 0,
		Move = 1,
		ConterAttack = 2
	}

	public enum BHIOPDNPEPA
	{
		Standard = 0,
		Missile = 1
	}

	private enum FOOLLKCDGFP
	{
		DefenceUseNone = 0,
		DefenceUseRandom = 1,
		DefenceUseCounterAttack = 2,
		DefenceUseDodge = 3,
		DefenceUseBlock = 4
	}

	private enum MFHIONPNAGO
	{
		SimpleMissile = 0,
		MagicMissile = 1
	}

	private enum CIDKMPMMEDB
	{
		AttackUseRandom = 0,
		AttackUseTableAttack = 1
	}

	private class Decision
	{
		public InfoAnimation FGICHADOEHF;

		public int Wait;

		public bool MNOFIOIEIBE
		{
			get
			{
				return OPLCCACOEGL();
			}
		}

		public bool GDHDPELHMOF
		{
			get
			{
				return JPDLFMAEOED();
			}
		}

		public Decision()
		{
			FGICHADOEHF = null;
			Wait = 0;
		}

		public Decision(InfoAnimation DBOLBEOCEME, int JOHDCPNACOC)
		{
			FGICHADOEHF = DBOLBEOCEME;
			Wait = JOHDCPNACOC;
		}

		public bool OPLCCACOEGL()
		{
			return FGICHADOEHF != null;
		}

		public bool JPDLFMAEOED()
		{
			return Wait != -1;
		}
	}

	private class BHDKGLJIOJD
	{
		public float PAIFCMNKFCP;

		public float NKFHNNFHLMM;

		public bool Flag;
	}

	private CBGBLIPAMGA PLDABIGHHFG;

	private CNDJFAIJOOH EBMMKHGDFCE;

	private FOOLLKCDGFP MKMJONLDKEM;

	private CIDKMPMMEDB IMFEKHJDABH;

	private static bool LCAHJDGGJPD = true;

	private bool OMECHFOGFBO;

	private int MEHOEEIGCEP;

	private int NJPDFMHHIDE;

	private Model _Model;

	private List<int> _InterframesList = new List<int>();

	private ModelAnimation _ModelAnimation;

	private ModelPhysics _ModelPhysics;

	private ModelParameters NHDAJBADMND;

	private string EIMKBOMDAAE;

	private string HCJOIHLKOKJ;

	private int NHIPFEIIPKG;

	private int EBEHPENMJLK;

	private bool EOGGOEGGONJ;

	private bool BEEPJNOFDCK;

	private InfoAnimation COKFBIJAFLH;

	private InfoAnimation CGPDPHJIDPA;

	private float OCCHDKNLAON;

	private int KHMLCMMKMBD;

	private int NFMNJOPBCBC;

	private AiData.HDHPLDFCDOF DOPHKPJFBJN;

	private Tactic GIBODDNLGJH;

	private bool PMOBMNECIBC;

	private float DGEFLLCBODI;

	private bool NHBNECINMPM;

	private float CAGEDGKGHEL;

	private bool EHAPKBCGPIA;

	private bool FHBHNNIIBDD;

	private bool EKOFADCNGAF;

	private List<BHDKGLJIOJD> OAPNPEFCBJB = new List<BHDKGLJIOJD>();

	private List<BHDKGLJIOJD> JNOMCJLDJGE = new List<BHDKGLJIOJD>();

	private bool JOOELCENMFC;

	private float CAONFMOKPKA;

	private float CIIDKJINKJG;

	private float JLKJCIMKKOH;

	private float BDEKBEEKIKA;

	private float IHOMGPBJEEG;

	private float GIKAFLJDHHJ;

	private bool BECCDKJJDAC;

	private List<Decision> NCBNEMAOHJE = new List<Decision>();

	private List<InfoAnimation> PIOGLIJBPLL = new List<InfoAnimation>();

	private float CJBHLGHFEGC;

	private float DDLPBPJJJNC;

	private float FKPMMOGNMLK;

	private float HFGEBCOFPCA;

	private float PMLLPOAPOFA;

	private float MFJFKFOJJEK;

	public static bool PBKOKPCKMPK
	{
		get
		{
			return get_AiOn();
		}
		set
		{
			set_AiOn(value);
		}
	}

	public Model KJDFJPBIGJC
	{
		get
		{
			return get_Model();
		}
		set
		{
			set_Model(value);
		}
	}

	public AiData.HDHPLDFCDOF IOPJIIJDHOE
	{
		get
		{
			return get_ResultSource();
		}
	}

	public Tactic HBFMBOHLKPJ
	{
		get
		{
			return get_Tactic();
		}
	}

	private bool HAMEBOCOHHJ
	{
		get
		{
			return get_IsEnabled();
		}
	}

	public ModelAi(ModelAnimation MEKLGEGJPFP, ModelPhysics LBELNKIDMIB, string PPIEODBOOJA, ModelParameters JCICKLIMBEF)
	{
		_ModelAnimation = MEKLGEGJPFP;
		_ModelPhysics = LBELNKIDMIB;
		COKFBIJAFLH = null;
		CGPDPHJIDPA = null;
		EIMKBOMDAAE = AiData.GetItemEquivalent(PPIEODBOOJA);
		OCCHDKNLAON = 0f;
		KHMLCMMKMBD = 0;
		NFMNJOPBCBC = 0;
		NHDAJBADMND = JCICKLIMBEF;
		DOPHKPJFBJN = AiData.HDHPLDFCDOF.noneTable;
		OMECHFOGFBO = false;
		EBMMKHGDFCE = CNDJFAIJOOH.Nostate;
		BEEPJNOFDCK = false;
		EBEHPENMJLK = 0;
		MKMJONLDKEM = FOOLLKCDGFP.DefenceUseRandom;
		IMFEKHJDABH = CIDKMPMMEDB.AttackUseRandom;
		MEHOEEIGCEP = 0;
		NJPDFMHHIDE = 0;
		BECCDKJJDAC = false;
		NHIPFEIIPKG = 1;
		EOGGOEGGONJ = false;
		PLDABIGHHFG = CBGBLIPAMGA.SetWaitNone;
		_Model = null;
		EHAPKBCGPIA = false;
		FHBHNNIIBDD = false;
		EKOFADCNGAF = false;
		JOOELCENMFC = false;
		GIBODDNLGJH = null;
		CAONFMOKPKA = 0f;
		CIIDKJINKJG = 0f;
		JLKJCIMKKOH = 0f;
		BDEKBEEKIKA = 0f;
		LoadParameters();
	}

	public static bool get_AiOn()
	{
		return LCAHJDGGJPD;
	}

	public static void set_AiOn(bool value)
	{
		LCAHJDGGJPD = value;
	}

	public void set_Model(Model value)
	{
		_Model = value;
	}

	public Model get_Model()
	{
		return _Model;
	}

	public AiData.HDHPLDFCDOF get_ResultSource()
	{
		return DOPHKPJFBJN;
	}

	public Tactic get_Tactic()
	{
		return GIBODDNLGJH;
	}

	public void setAvailableAnimations(List<InfoAnimation> MAHEJFLCCHP)
	{
	}

	public InfoAnimation Render(Model FNKFIMEDNLP, int JLLPJLEDBPG)
	{
		if (!get_IsEnabled())
		{
			return null;
		}
		ModelAnimation oJIEPADIEDE = FNKFIMEDNLP.OCPMJKIEPIG();
		TacticFactors fJCBLOKOBBD = SetFactors(FNKFIMEDNLP);
		if (oJIEPADIEDE.NMEEPBDJHMG())
		{
			int num = oJIEPADIEDE.HILLKPNMCIP();
			int num2 = oJIEPADIEDE.KOCKCMNHPMC();
			int num3 = GetFrameError(fJCBLOKOBBD);
			MEHOEEIGCEP = num + num2 + num3;
		}
		else
		{
			MEHOEEIGCEP = -1;
		}
		if (_ModelAnimation.NMEEPBDJHMG())
		{
			int num4 = _ModelAnimation.HILLKPNMCIP();
			int num5 = _ModelAnimation.KOCKCMNHPMC();
			NJPDFMHHIDE = num4 + num5;
		}
		else
		{
			NJPDFMHHIDE = -1;
		}
		if (BEEPJNOFDCK)
		{
			BEEPJNOFDCK = false;
			if (_ModelAnimation.NMEEPBDJHMG() && oJIEPADIEDE.NMEEPBDJHMG())
			{
				InfoAnimation pJAHIOELGGD = _ModelAnimation.NNMAFFCCMHC();
				InfoAnimation pJAHIOELGGD2 = oJIEPADIEDE.NNMAFFCCMHC();
				if (pJAHIOELGGD != null && pJAHIOELGGD2 != null)
				{
					int num6 = pJAHIOELGGD.HGMPJJACFHN();
					switch (PLDABIGHHFG)
					{
					case CBGBLIPAMGA.SetWaitRandAttack:
						NHIPFEIIPKG = pJAHIOELGGD2.MLLLLMFLOBG(true) - MEHOEEIGCEP + 1;
						NHIPFEIIPKG = Mathf.Min(NHIPFEIIPKG, pJAHIOELGGD.JMIDABBAKEP());
						if (num6 > NHIPFEIIPKG)
						{
							NHIPFEIIPKG = num6;
						}
						NHIPFEIIPKG--;
						break;
					case CBGBLIPAMGA.SetWaitRandUnint:
						NHIPFEIIPKG = pJAHIOELGGD2.IKFCNCLKDGD(true) - MEHOEEIGCEP + 1;
						NHIPFEIIPKG = Mathf.Min(NHIPFEIIPKG, pJAHIOELGGD.JMIDABBAKEP());
						if (num6 > NHIPFEIIPKG)
						{
							NHIPFEIIPKG = num6;
						}
						NHIPFEIIPKG--;
						break;
					case CBGBLIPAMGA.SetWaitAnimationLength:
						NHIPFEIIPKG = num6;
						NHIPFEIIPKG--;
						break;
					}
				}
			}
		}
		else
		{
			CBGBLIPAMGA pLDABIGHHFG = PLDABIGHHFG;
			if (pLDABIGHHFG == CBGBLIPAMGA.SetWaitRandAttack || pLDABIGHHFG == CBGBLIPAMGA.SetWaitRandUnint || pLDABIGHHFG == CBGBLIPAMGA.SetWaitAnimationLength)
			{
				LLLOJBFMONN.Error("!");
			}
		}
		PLDABIGHHFG = CBGBLIPAMGA.SetWaitNone;
		if (1 < NHIPFEIIPKG)
		{
			NHIPFEIIPKG--;
			return null;
		}
		InfoAnimation pJAHIOELGGD3 = oJIEPADIEDE.NNMAFFCCMHC();
		if (pJAHIOELGGD3 != null)
		{
			bool flag = false;
			List<TemplateAnimation> list = AiData.get_RandomizingEnemyAnimation();
			foreach (TemplateAnimation item in list)
			{
				List<InfoAnimation> list2 = item.LDEBJOPLCKO();
				if (list2.Contains(pJAHIOELGGD3))
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				RandomizeBehavior(FNKFIMEDNLP);
			}
		}
		SetQuickAttackRnd();
		SetEvadesRnd();
		if (!IsFitIntervalAndMove())
		{
			return null;
		}
		MKMJONLDKEM = SelectDefenceMode(FNKFIMEDNLP);
		HFGEBCOFPCA = GIBODDNLGJH.GetUseSafeAttackChance(fJCBLOKOBBD);
		FHBHNNIIBDD = CIIDKJINKJG < HFGEBCOFPCA;
		PMLLPOAPOFA = GIBODDNLGJH.GetTableAttackChance(fJCBLOKOBBD);
		EKOFADCNGAF = JLKJCIMKKOH < PMLLPOAPOFA;
		SetQuickAttackChances(fJCBLOKOBBD);
		SetEvadesChances(fJCBLOKOBBD);
		MFJFKFOJJEK = GIBODDNLGJH.GetCautiousMovementsChance(fJCBLOKOBBD);
		JOOELCENMFC = BDEKBEEKIKA < MFJFKFOJJEK;
		DGEFLLCBODI = GIBODDNLGJH.GetDodgeMissileChance(fJCBLOKOBBD);
		PMOBMNECIBC = IHOMGPBJEEG < DGEFLLCBODI;
		CAGEDGKGHEL = GIBODDNLGJH.GetDodgeMagicChance(fJCBLOKOBBD);
		NHBNECINMPM = GIKAFLJDHHJ < CAGEDGKGHEL;
		int num7 = SetDecisionList(FNKFIMEDNLP, JLLPJLEDBPG);
		if (0 < num7)
		{
			EOGGOEGGONJ = false;
			GetPlayableAnimations(NCBNEMAOHJE);
			DecisionsToAimationsList();
			int num8 = SelectAnimationWithWeights(PIOGLIJBPLL);
			if (-1 < num8)
			{
				NHIPFEIIPKG = _InterframesList[num8];
				InfoAnimation dBOLBEOCEME = PIOGLIJBPLL[num8];
				LogTable(FNKFIMEDNLP.OCPMJKIEPIG(), JLLPJLEDBPG, num7, dBOLBEOCEME, NHIPFEIIPKG);
				return PIOGLIJBPLL[num8];
			}
		}
		else if (BEEPJNOFDCK)
		{
			LogTable(FNKFIMEDNLP.OCPMJKIEPIG(), JLLPJLEDBPG - 1, 1, null, 0);
			EOGGOEGGONJ = false;
		}
		return null;
	}

	public void RandomizeBehavior(Model OGBHDKKOIGH)
	{
		TacticFactors oHKCJDCMOKN = new TacticFactors(_Model.FGACEEPJBIF(), _Model.GLEKCPCMINJ(), _Model.LPOJKGLFMAL());
		_Model.FGACEEPJBIF().GetCountAndDamage(true, COKFBIJAFLH, ref oHKCJDCMOKN.EOGLBDCLMBM, ref oHKCJDCMOKN.KFMJMBANIGF, ref oHKCJDCMOKN.AAKOCIPFDNM);
		oHKCJDCMOKN.MGICNNKKCAN = (ObscuredFloat)(_Model.KMMJCHDKBDO.KKMCHCNOHMB());
		oHKCJDCMOKN.DDGNCMJGDAG = (ObscuredFloat)(OGBHDKKOIGH.KMMJCHDKBDO.KKMCHCNOHMB());
		oHKCJDCMOKN.OLCKGMBDGOG = OGBHDKKOIGH.OCPMJKIEPIG().HILLKPNMCIP();
		oHKCJDCMOKN.NGMLGDJGBCD = ChildMaxModelFrame(OGBHDKKOIGH);
		CAONFMOKPKA = NekkiMath.randomFloat();
		CIIDKJINKJG = NekkiMath.randomFloat();
		JLKJCIMKKOH = NekkiMath.randomFloat();
		BDEKBEEKIKA = NekkiMath.randomFloat();
		IHOMGPBJEEG = NekkiMath.randomFloat();
		GIKAFLJDHHJ = NekkiMath.randomFloat();
		OCCHDKNLAON = GetDistanceError(oHKCJDCMOKN);
		KHMLCMMKMBD = GetFrameError(oHKCJDCMOKN);
	}

	private int ChildMaxModelFrame(Model ACENLMONNPA)
	{
		int num = 0;
		int i = 0;
		for (int count = ACENLMONNPA.KGGIDBLBMDJ().Count; i < count; i++)
		{
			WeaponModel gKIANLDJFCH = ACENLMONNPA.KGGIDBLBMDJ()[i];
			if (gKIANLDJFCH != null)
			{
				int num2 = gKIANLDJFCH.OCPMJKIEPIG().JFGEHNHLDJM();
				if (num2 > num)
				{
					num = num2;
				}
			}
		}
		return num;
	}

	public void StartAnimationBot(InfoAnimation DBOLBEOCEME)
	{
		if (!get_IsEnabled())
		{
			return;
		}
		if (_ModelAnimation.NMEEPBDJHMG() && DBOLBEOCEME != null)
		{
			InfoAnimation pJAHIOELGGD = DBOLBEOCEME.IMFGMAAEMIC();
			if (pJAHIOELGGD != null)
			{
				CGPDPHJIDPA = pJAHIOELGGD;
			}
			else
			{
				CGPDPHJIDPA = DBOLBEOCEME;
			}
		}
		else
		{
			CGPDPHJIDPA = null;
		}
		if (IsFitStartAnimation(CGPDPHJIDPA))
		{
			NHIPFEIIPKG = 1;
		}
	}

	public void StartAnimationEnemy(Model OGBHDKKOIGH)
	{
		if (!get_IsEnabled())
		{
			return;
		}
		ModelAnimation oJIEPADIEDE = OGBHDKKOIGH.OCPMJKIEPIG();
		InfoAnimation pJAHIOELGGD = oJIEPADIEDE.NNMAFFCCMHC();
		if (oJIEPADIEDE.NMEEPBDJHMG() && pJAHIOELGGD != null)
		{
			InfoAnimation pJAHIOELGGD2 = pJAHIOELGGD.IMFGMAAEMIC();
			if (pJAHIOELGGD2 == null)
			{
				COKFBIJAFLH = pJAHIOELGGD;
			}
			else
			{
				COKFBIJAFLH = pJAHIOELGGD2;
			}
			InfoAnimation cOKFBIJAFLH = COKFBIJAFLH;
			RandomizeBehavior(OGBHDKKOIGH);
			if (!IsIgnoredEnemyAnimation(cOKFBIJAFLH))
			{
				TacticFactors oHKCJDCMOKN = new TacticFactors(_Model.FGACEEPJBIF(), _Model.GLEKCPCMINJ(), _Model.LPOJKGLFMAL());
				_Model.FGACEEPJBIF().GetCountAndDamage(true, COKFBIJAFLH, ref oHKCJDCMOKN.EOGLBDCLMBM, ref oHKCJDCMOKN.KFMJMBANIGF, ref oHKCJDCMOKN.AAKOCIPFDNM);
				oHKCJDCMOKN.MGICNNKKCAN = (ObscuredFloat)(_Model.KMMJCHDKBDO.KKMCHCNOHMB());
				oHKCJDCMOKN.DDGNCMJGDAG = (ObscuredFloat)(OGBHDKKOIGH.KMMJCHDKBDO.KKMCHCNOHMB());
				oHKCJDCMOKN.OLCKGMBDGOG = OGBHDKKOIGH.OCPMJKIEPIG().HILLKPNMCIP();
				oHKCJDCMOKN.NGMLGDJGBCD = ChildMaxModelFrame(OGBHDKKOIGH);
				EBEHPENMJLK = GetResponseDelay(oHKCJDCMOKN);
			}
		}
	}

	private bool IsIgnoredEnemyAnimation(InfoAnimation DBOLBEOCEME)
	{
		List<string> list = AiData.get_IgnoredEnemyAnimations();
		foreach (string item in list)
		{
			if (DBOLBEOCEME.CNPFHBMGDFP(item))
			{
				return true;
			}
		}
		return false;
	}

	public void StartRangedEnemy()
	{
		if (get_IsEnabled())
		{
			NHIPFEIIPKG = 1;
		}
	}

	public void SetWeaponEnemy(string PPIEODBOOJA)
	{
		HCJOIHLKOKJ = AiData.GetItemEquivalent(PPIEODBOOJA);
	}

	public void SetWeaponBot(string PPIEODBOOJA)
	{
		HCJOIHLKOKJ = AiData.GetItemEquivalent(PPIEODBOOJA);
	}

	public void OnGetHit()
	{
		if (get_IsEnabled())
		{
		}
	}

	public void OnHitEnemy()
	{
		if (get_IsEnabled())
		{
		}
	}

	public int SelectAnimationWithWeights(List<InfoAnimation> MAHEJFLCCHP)
	{
		Model fGCODGKLHED = _Model.EGGEACCDAEK();
		if (fGCODGKLHED != null)
		{
			Model fNKFIMEDNLP = fGCODGKLHED.BDJBNOPNCNB();
			TacticFactors fJCBLOKOBBD = SetFactors(fNKFIMEDNLP);
			return GIBODDNLGJH.SelectAnimationWithWeights(MAHEJFLCCHP, CGPDPHJIDPA, fJCBLOKOBBD);
		}
		return -1;
	}

	public void ChangeTactic(string name)
	{
		Tactic bJBIGPGJKIE = AiData.GetTacticByName(name);
		ChangeTactic(bJBIGPGJKIE);
	}

	public void ChangeTactic(Tactic BJBIGPGJKIE)
	{
		if (BJBIGPGJKIE != null)
		{
			GIBODDNLGJH = BJBIGPGJKIE;
		}
	}

	private int GetResponseDelay(TacticFactors FJCBLOKOBBD)
	{
		return GIBODDNLGJH.GetResponseDelay(FJCBLOKOBBD) + 1;
	}

	private float GetDistanceError(TacticFactors FJCBLOKOBBD)
	{
		return GIBODDNLGJH.GetDistanceError(FJCBLOKOBBD);
	}

	private int GetFrameError(TacticFactors FJCBLOKOBBD)
	{
		return GIBODDNLGJH.GetFrameError(FJCBLOKOBBD);
	}

	private int GetEnemyResponseDelay(TacticFactors FJCBLOKOBBD)
	{
		return GIBODDNLGJH.GetEnemyResponseDelay(FJCBLOKOBBD);
	}

	private static bool GetRandomFlag(float KFJGPCLOMIG)
	{
		float num = NekkiMath.randomFloat();
		return num < KFJGPCLOMIG;
	}

	private void LoadParameters()
	{
		Tactic hBFMBOHLKPJ = NHDAJBADMND.HBFMBOHLKPJ;
		if (hBFMBOHLKPJ != null)
		{
			GIBODDNLGJH = hBFMBOHLKPJ;
		}
	}

	private void LogTable(ModelAnimation IPNLKNLBLIE, int JLLPJLEDBPG, int count, InfoAnimation DBOLBEOCEME, int JOHDCPNACOC)
	{
	}

	private void LogStart()
	{
	}

	private int GetNearestKeyFrameId(int KKEGODOKGCB)
	{
		if (KKEGODOKGCB % AiData.KINPOOFGAGD == 0)
		{
			return KKEGODOKGCB;
		}
		if (0 < KKEGODOKGCB)
		{
			return KKEGODOKGCB - KKEGODOKGCB % AiData.KINPOOFGAGD + AiData.KINPOOFGAGD;
		}
		return KKEGODOKGCB - KKEGODOKGCB % AiData.KINPOOFGAGD;
	}

	private bool IsFitStartAnimation(InfoAnimation DBOLBEOCEME)
	{
		if (_ModelAnimation.NMEEPBDJHMG() && DBOLBEOCEME != null)
		{
			List<string> list = AiData.get_UnexpectedMoves();
			foreach (string item in list)
			{
				if (DBOLBEOCEME.CNPFHBMGDFP(item))
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool IsFitBotAnimation(InfoAnimation DBOLBEOCEME, BHIOPDNPEPA LFLGCDNKNJI = BHIOPDNPEPA.Standard)
	{
		if (DBOLBEOCEME != null)
		{
			List<string> list = null;
			switch (LFLGCDNKNJI)
			{
			case BHIOPDNPEPA.Standard:
				list = AiData.get_MovesLastIteration();
				break;
			case BHIOPDNPEPA.Missile:
				list = AiData.get_MissilesLastIteration();
				break;
			}
			foreach (string item in list)
			{
				if (DBOLBEOCEME.CNPFHBMGDFP(item))
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool IsSafeDodges(InfoAnimation DBOLBEOCEME)
	{
		List<string> list = AiData.get_SafeDodgesAnimations();
		foreach (string item in list)
		{
			if (DBOLBEOCEME.CNPFHBMGDFP(item))
			{
				return true;
			}
		}
		return false;
	}

	private bool IsFitIntervalAndMove()
	{
		List<string> list = AiData.get_NoDecisionIntervals();
		foreach (string item in list)
		{
			IntervalAnimation mNOIEOBBCMI = _ModelAnimation.HDJBHPOGKNJ(item);
			if (mNOIEOBBCMI != null)
			{
				return false;
			}
		}
		if (_ModelAnimation.NMEEPBDJHMG() && CGPDPHJIDPA != null)
		{
			InfoAnimation cGPDPHJIDPA = CGPDPHJIDPA;
			List<string> list2 = AiData.get_NoDecisionMoves();
			foreach (string item2 in list2)
			{
				if (cGPDPHJIDPA.CNPFHBMGDFP(item2))
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}

	private bool IsMissileAnimation(InfoAnimation DBOLBEOCEME)
	{
		List<TemplateAnimation> nNGPIGIMNPD = AiData.get_MissileAnimations();
		return IsGivenTemplateAnimation(DBOLBEOCEME, nNGPIGIMNPD);
	}

	private bool IsMagicAnimation(InfoAnimation DBOLBEOCEME)
	{
		List<TemplateAnimation> nNGPIGIMNPD = AiData.get_MagicAnimations();
		return IsGivenTemplateAnimation(DBOLBEOCEME, nNGPIGIMNPD);
	}

	private bool IsGivenTemplateAnimation(InfoAnimation DBOLBEOCEME, List<TemplateAnimation> NNGPIGIMNPD)
	{
		bool result = false;
		if (DBOLBEOCEME != null)
		{
			foreach (TemplateAnimation item in NNGPIGIMNPD)
			{
				List<InfoAnimation> list = item.LDEBJOPLCKO();
				if (list.Contains(DBOLBEOCEME))
				{
					result = true;
					break;
				}
			}
		}
		return result;
	}

	private bool GetUseChildrenDodge(Model FNKFIMEDNLP, MFHIONPNAGO LFLGCDNKNJI)
	{
		if (FNKFIMEDNLP.KGGIDBLBMDJ().Count == 0 || _Model == null)
		{
			return false;
		}
		bool result = false;
		int num = GetModelDirection(_Model, FNKFIMEDNLP);
		foreach (WeaponModel item in FNKFIMEDNLP.KGGIDBLBMDJ())
		{
			if (item.OCPMJKIEPIG() != null && (item.OCPMJKIEPIG() == null || item.OCPMJKIEPIG().JJBEAOPDGCO() != null) && (IsMissileAnimation(item.OCPMJKIEPIG().NNMAFFCCMHC()) || LFLGCDNKNJI != MFHIONPNAGO.SimpleMissile) && (IsMagicAnimation(item.OCPMJKIEPIG().NNMAFFCCMHC()) || LFLGCDNKNJI != MFHIONPNAGO.MagicMissile))
			{
				float num2 = _Model.CLDMEJKGLBA().CJELIBMCCMA().ICLEOFDKDIF()
					.GILCBJJPKBK();
				float num3 = item.CLDMEJKGLBA().NAMKCLGOPDD()[0].ICLEOFDKDIF().GILCBJJPKBK();
				int num4 = ((num2 - num3 < 0f) ? 1 : (-1));
				int num5 = item.OCPMJKIEPIG().KFCNPADAMHA();
				if (num4 * num5 < 0)
				{
					result = true;
				}
				else if (Mathf.Abs(num3 - num2) < 100f)
				{
					result = true;
				}
			}
		}
		return result;
	}

	private FOOLLKCDGFP SelectDefenceMode(Model FNKFIMEDNLP)
	{
		float num = NekkiMath.randomFloat();
		TacticFactors fJCBLOKOBBD = SetFactors(FNKFIMEDNLP);
		CJBHLGHFEGC = GIBODDNLGJH.GetCounterAttackChance(fJCBLOKOBBD);
		DDLPBPJJJNC = GIBODDNLGJH.GetDodgeChance(fJCBLOKOBBD);
		FKPMMOGNMLK = GIBODDNLGJH.GetBlockChance(fJCBLOKOBBD);
		float cJBHLGHFEGC = CJBHLGHFEGC;
		if (num < CJBHLGHFEGC)
		{
			return FOOLLKCDGFP.DefenceUseCounterAttack;
		}
		cJBHLGHFEGC += DDLPBPJJJNC;
		if (num < cJBHLGHFEGC)
		{
			return FOOLLKCDGFP.DefenceUseDodge;
		}
		cJBHLGHFEGC += FKPMMOGNMLK;
		if (num < cJBHLGHFEGC)
		{
			return FOOLLKCDGFP.DefenceUseBlock;
		}
		return FOOLLKCDGFP.DefenceUseRandom;
	}

	private float GetNodeX(string IMGCANJHPND, Model ACENLMONNPA, Model FNKFIMEDNLP)
	{
		int aOJJBKLCHJO = GetModelDirection(ACENLMONNPA, FNKFIMEDNLP);
		ModelNode lCDGOCIAIDK = ACENLMONNPA.OCPMJKIEPIG().EGHIDHMENEF(IMGCANJHPND, aOJJBKLCHJO);
		if (lCDGOCIAIDK != null)
		{
			return lCDGOCIAIDK.ICLEOFDKDIF().GILCBJJPKBK();
		}
		return float.MaxValue;
	}

	private float GetNodeX(string IMGCANJHPND, List<global::Pair<string, float>> EGMLEFHEBLL)
	{
		foreach (global::Pair<string, float> item in EGMLEFHEBLL)
		{
			if (item.First == IMGCANJHPND)
			{
				return item.Second;
			}
		}
		return float.MaxValue;
	}

	private void SetRandomAnimation()
	{
		_Model.JAOKKOPAKKL(false);
		DOPHKPJFBJN = AiData.HDHPLDFCDOF.randomAnimation;
		BEEPJNOFDCK = true;
		NHIPFEIIPKG = int.MinValue;
	}

	private int GetPlayableAnimations(List<InfoAnimation> MAHEJFLCCHP, List<int> FIFFFOLGCND = null, bool AEGBKDJEABP = false)
	{
		List<InfoAnimation> list = _Model.MCFPDHOLNGB();
		int num = 0;
		int count = MAHEJFLCCHP.Count;
		ModelConditions dGJJDPIAEAO = get_Model().EBABHGHPLFK();
		if (dGJJDPIAEAO == null)
		{
			LLLOJBFMONN.Error("modelConditions is null");
			return 0;
		}
		dGJJDPIAEAO.IDCHHGHAENM = false;
		for (int i = 0; i < count; i++)
		{
			InfoAnimation pJAHIOELGGD = MAHEJFLCCHP[i];
			if (pJAHIOELGGD != null && ((!AEGBKDJEABP) ? IsPlayableAnimations(pJAHIOELGGD) : IsTacticPlayableAnimations(pJAHIOELGGD)))
			{
				MAHEJFLCCHP[num] = MAHEJFLCCHP[i];
				if (FIFFFOLGCND != null)
				{
					FIFFFOLGCND[num] = FIFFFOLGCND[i];
				}
				num++;
			}
		}
		MAHEJFLCCHP.CPCAJIKOIEE(num);
		if (FIFFFOLGCND != null)
		{
			FIFFFOLGCND.CPCAJIKOIEE(num);
		}
		return num;
	}

	private int GetPlayableAnimations(List<Decision> PJGOCFKJGJJ)
	{
		List<InfoAnimation> list = _Model.MCFPDHOLNGB();
		int num = 0;
		int count = PJGOCFKJGJJ.Count;
		ModelConditions dGJJDPIAEAO = get_Model().EBABHGHPLFK();
		if (dGJJDPIAEAO == null)
		{
			LLLOJBFMONN.Error("modelConditions is null");
			return 0;
		}
		dGJJDPIAEAO.IDCHHGHAENM = false;
		for (int i = 0; i < count; i++)
		{
			InfoAnimation fGICHADOEHF = PJGOCFKJGJJ[i].FGICHADOEHF;
			if (fGICHADOEHF == null || IsPlayableAnimations(fGICHADOEHF))
			{
				PJGOCFKJGJJ[num] = PJGOCFKJGJJ[i];
				num++;
			}
		}
		PJGOCFKJGJJ.CPCAJIKOIEE(num);
		return num;
	}

	private bool IsPlayableAnimations(InfoAnimation DBOLBEOCEME)
	{
		List<InfoAnimation> list = _Model.MCFPDHOLNGB();
		if (!list.Contains(DBOLBEOCEME))
		{
			return false;
		}
		ModelConditions dGJJDPIAEAO = get_Model().EBABHGHPLFK();
		dGJJDPIAEAO.IDCHHGHAENM = false;
		dGJJDPIAEAO.PDKPGKPBBIL = DBOLBEOCEME.FOLOOGCLPNE();
		dGJJDPIAEAO.PCAOCHAIBJC = DBOLBEOCEME.CEDEDCLGJDE(dGJJDPIAEAO, _ModelAnimation.KFCNPADAMHA());
		dGJJDPIAEAO.FOIHIKCEBJF = (int)DBOLBEOCEME.ODACDCDONJE.ILOEBFFAEAN.OLBDPMKCJIF;
		if (!DBOLBEOCEME.HPPGNJJCEGF(get_Model(), null, DBOLBEOCEME.OIGBIFNICBI(EventAnimation.EECEJKADLCK.EVENT_KEY_PRESSED)))
		{
			return false;
		}
		InfoAnimation.CapabilityTable iCANLHJKKNE = DBOLBEOCEME.ICANLHJKKNE;
		int count = iCANLHJKKNE.NINJLLDJLFI.Count;
		if (0 < count)
		{
			foreach (InfoAnimation item in iCANLHJKKNE.NINJLLDJLFI)
			{
				if (list.Contains(item))
				{
					dGJJDPIAEAO.PDKPGKPBBIL = item.FOLOOGCLPNE();
					dGJJDPIAEAO.PCAOCHAIBJC = item.CEDEDCLGJDE(dGJJDPIAEAO, _ModelAnimation.KFCNPADAMHA());
					dGJJDPIAEAO.FOIHIKCEBJF = (int)item.ODACDCDONJE.ILOEBFFAEAN.OLBDPMKCJIF;
					if (item.HPPGNJJCEGF(get_Model(), null, DBOLBEOCEME.OIGBIFNICBI(EventAnimation.EECEJKADLCK.EVENT_KEY_PRESSED)))
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	private bool IsTacticPlayableAnimations(InfoAnimation DBOLBEOCEME)
	{
		List<InfoAnimation> list = _Model.MCFPDHOLNGB();
		if (!list.Contains(DBOLBEOCEME))
		{
			return false;
		}
		ModelConditions dGJJDPIAEAO = get_Model().EBABHGHPLFK();
		dGJJDPIAEAO.IDCHHGHAENM = false;
		dGJJDPIAEAO.PDKPGKPBBIL = DBOLBEOCEME.FOLOOGCLPNE();
		dGJJDPIAEAO.PCAOCHAIBJC = DBOLBEOCEME.CEDEDCLGJDE(dGJJDPIAEAO, _ModelAnimation.KFCNPADAMHA());
		dGJJDPIAEAO.FOIHIKCEBJF = (int)DBOLBEOCEME.ODACDCDONJE.ILOEBFFAEAN.OLBDPMKCJIF;
		if (!DBOLBEOCEME.HPPGNJJCEGF(_Model.EBABHGHPLFK(), DBOLBEOCEME.ODACDCDONJE.NIDNJFOGBFO, DBOLBEOCEME.OIGBIFNICBI(EventAnimation.EECEJKADLCK.EVENT_KEY_PRESSED)))
		{
			return false;
		}
		return true;
	}

	private float GetCurrentDirectionToEnemy(ModelAnimation HFGPAELCNMF)
	{
		ModelNode lCDGOCIAIDK = HFGPAELCNMF.CJELIBMCCMA();
		ModelNode lCDGOCIAIDK2 = _ModelAnimation.CJELIBMCCMA();
		if (lCDGOCIAIDK == null || lCDGOCIAIDK2 == null)
		{
			return 0f;
		}
		float num = lCDGOCIAIDK.ICLEOFDKDIF().GILCBJJPKBK() - lCDGOCIAIDK2.ICLEOFDKDIF().GILCBJJPKBK();
		if (num >= 0f)
		{
			return 1f;
		}
		return -1f;
	}

	private bool IsFitCondition(InfoAnimation DBOLBEOCEME, bool EMALNKEEEEN)
	{
		ModelConditions dGJJDPIAEAO = get_Model().EBABHGHPLFK();
		if (dGJJDPIAEAO == null)
		{
			LLLOJBFMONN.Error("modelConditions is null");
			return false;
		}
		dGJJDPIAEAO.PDKPGKPBBIL = DBOLBEOCEME.FOLOOGCLPNE();
		dGJJDPIAEAO.PCAOCHAIBJC = DBOLBEOCEME.CEDEDCLGJDE(dGJJDPIAEAO, _ModelAnimation.KFCNPADAMHA());
		dGJJDPIAEAO.FOIHIKCEBJF = (int)DBOLBEOCEME.ODACDCDONJE.ILOEBFFAEAN.OLBDPMKCJIF;
		return DBOLBEOCEME.HPPGNJJCEGF(dGJJDPIAEAO, (!EMALNKEEEEN) ? null : DBOLBEOCEME.ODACDCDONJE.NIDNJFOGBFO);
	}

	private int SetDecisionList(Model FNKFIMEDNLP, int JLLPJLEDBPG)
	{
		NCBNEMAOHJE.Clear();
		ModelAnimation oJIEPADIEDE = FNKFIMEDNLP.OCPMJKIEPIG();
		if (GetCurrentDirectionToEnemy(oJIEPADIEDE) * (float)oJIEPADIEDE.KFCNPADAMHA() > 0f)
		{
			SetRandomAnimation();
			PLDABIGHHFG = CBGBLIPAMGA.SetWaitAnimationLength;
			return 0;
		}
		bool flag = false;
		if (GetUseChildrenDodge(FNKFIMEDNLP, MFHIONPNAGO.SimpleMissile) && PMOBMNECIBC)
		{
			int num = GetForDodgeMissiles(FNKFIMEDNLP, MFHIONPNAGO.SimpleMissile);
			DOPHKPJFBJN = AiData.HDHPLDFCDOF.dodgeTable;
			flag = true;
		}
		if (NHBNECINMPM && GetUseChildrenDodge(FNKFIMEDNLP, MFHIONPNAGO.MagicMissile))
		{
			int num2 = GetForDodgeMissiles(FNKFIMEDNLP, MFHIONPNAGO.MagicMissile);
			DOPHKPJFBJN = AiData.HDHPLDFCDOF.dodgeTable;
			flag = true;
		}
		if (flag)
		{
			return NCBNEMAOHJE.Count;
		}
		int num3 = oJIEPADIEDE.HILLKPNMCIP();
		if (EBEHPENMJLK < num3 && !IsUninteruptIntervalEnd(oJIEPADIEDE))
		{
			if (COKFBIJAFLH.CJAHEDOHHEG("Uninterrupt", MEHOEEIGCEP))
			{
				if (!IsAttackIntervalEnd(oJIEPADIEDE))
				{
					switch (MKMJONLDKEM)
					{
					case FOOLLKCDGFP.DefenceUseCounterAttack:
					{
						int num5 = GetFromTablesMove(FNKFIMEDNLP);
						if (0 < num5)
						{
							DOPHKPJFBJN = AiData.HDHPLDFCDOF.movementsTable;
						}
						if (num5 == 0)
						{
							num5 = GetFromTablesDodge(FNKFIMEDNLP);
							if (0 < num5)
							{
								DOPHKPJFBJN = AiData.HDHPLDFCDOF.dodgeTable;
							}
						}
						return num5;
					}
					case FOOLLKCDGFP.DefenceUseDodge:
					{
						int num4 = GetFromTablesDodge(FNKFIMEDNLP);
						if (0 < num4)
						{
							DOPHKPJFBJN = AiData.HDHPLDFCDOF.dodgeTable;
						}
						return num4;
					}
					case FOOLLKCDGFP.DefenceUseBlock:
						DOPHKPJFBJN = AiData.HDHPLDFCDOF.block;
						return 0;
					default:
						SetRandomAnimation();
						PLDABIGHHFG = CBGBLIPAMGA.SetWaitRandAttack;
						return 0;
					}
				}
				if (FHBHNNIIBDD)
				{
					int num6 = GetFromTablesMove(FNKFIMEDNLP);
					if (0 < num6)
					{
						DOPHKPJFBJN = AiData.HDHPLDFCDOF.movementsTable;
					}
					if (EHAPKBCGPIA || 0 < num6)
					{
						return NCBNEMAOHJE.Count;
					}
				}
				if (EKOFADCNGAF)
				{
					int num7 = GetFromTablesAttack(FNKFIMEDNLP);
					if (0 < num7)
					{
						DOPHKPJFBJN = AiData.HDHPLDFCDOF.outcometablesforattack;
					}
					if (EHAPKBCGPIA || 0 < num7)
					{
						return NCBNEMAOHJE.Count;
					}
				}
				if (JOOELCENMFC)
				{
					List<TemplateAnimation> list = AiData.get_CautiousMovements();
					InfoAnimation pJAHIOELGGD = FNKFIMEDNLP.OCPMJKIEPIG().NNMAFFCCMHC();
					int num8 = 0;
					if (pJAHIOELGGD != null && FNKFIMEDNLP.OCPMJKIEPIG().NMEEPBDJHMG())
					{
						num8 = pJAHIOELGGD.IKFCNCLKDGD(true) - MEHOEEIGCEP + 1;
					}
					NCBNEMAOHJE.Clear();
					foreach (TemplateAnimation item in list)
					{
						List<InfoAnimation> list2 = item.LDEBJOPLCKO();
						foreach (InfoAnimation item2 in list2)
						{
							int a = num8;
							a = Mathf.Min(a, item2.JMIDABBAKEP());
							int num9 = item2.HGMPJJACFHN();
							if (num9 > a)
							{
								a = num9;
							}
							NCBNEMAOHJE.Add(new Decision(item2, a));
						}
					}
					int count = NCBNEMAOHJE.Count;
					if (0 < count)
					{
						DOPHKPJFBJN = AiData.HDHPLDFCDOF.safeTable;
					}
					return NCBNEMAOHJE.Count;
				}
				SetRandomAnimation();
				PLDABIGHHFG = CBGBLIPAMGA.SetWaitRandUnint;
				return 0;
			}
			return 0;
		}
		bool flag2 = false;
		int count2 = OAPNPEFCBJB.Count;
		List<global::Pair<string, TacticValue>> list3 = GIBODDNLGJH.get_QuickAttacks();
		for (int i = 0; i < count2; i++)
		{
			BHDKGLJIOJD bHDKGLJIOJD = OAPNPEFCBJB[i];
			if (!bHDKGLJIOJD.Flag)
			{
				continue;
			}
			global::Pair<string, TacticValue> cCKLNOPEKHO = list3[i];
			List<InfoAnimation> list4 = new List<InfoAnimation>();
			AnimationData.NEBELEFIDMB(cCKLNOPEKHO.First, list4);
			foreach (InfoAnimation item3 in list4)
			{
				if (item3 != null && IsPlayableAnimations(item3))
				{
					int jOHDCPNACOC = item3.HGMPJJACFHN();
					NCBNEMAOHJE.Add(new Decision(item3, jOHDCPNACOC));
					flag2 = true;
				}
			}
		}
		if (flag2)
		{
			DOPHKPJFBJN = AiData.HDHPLDFCDOF.quickAttact;
		}
		BECCDKJJDAC = false;
		int count3 = JNOMCJLDJGE.Count;
		List<global::Pair<string, TacticValue>> list5 = GIBODDNLGJH.get_Evades();
		for (int j = 0; j < count3; j++)
		{
			if (BECCDKJJDAC)
			{
				break;
			}
			BHDKGLJIOJD bHDKGLJIOJD2 = JNOMCJLDJGE[j];
			if (!bHDKGLJIOJD2.Flag)
			{
				continue;
			}
			global::Pair<string, TacticValue> cCKLNOPEKHO2 = list5[j];
			List<InfoAnimation> list6 = new List<InfoAnimation>();
			AnimationData.NEBELEFIDMB(cCKLNOPEKHO2.First, list6);
			ModelAi pCFGKAFOCDO = FNKFIMEDNLP.EEIGOJBKFGE();
			if (pCFGKAFOCDO == null)
			{
				continue;
			}
			foreach (InfoAnimation item4 in list6)
			{
				if (item4 != null && pCFGKAFOCDO.IsPlayableAnimations(item4))
				{
					BECCDKJJDAC = true;
					break;
				}
			}
		}
		TacticFactors fJCBLOKOBBD = SetFactors(FNKFIMEDNLP);
		float num10 = GIBODDNLGJH.GetExpectedWait(CGPDPHJIDPA, fJCBLOKOBBD);
		if (num10 < 1f)
		{
			num10 = 1f;
		}
		float num11 = 1f - 1f / num10;
		float num12 = NekkiMath.randomFloat();
		if (num11 < num12)
		{
			EOGGOEGGONJ = true;
		}
		if (EOGGOEGGONJ || BECCDKJJDAC)
		{
			int num13 = 0;
			if (EKOFADCNGAF)
			{
				num13 = GetFromTablesAttack(FNKFIMEDNLP);
				if (0 < num13)
				{
					DOPHKPJFBJN = AiData.HDHPLDFCDOF.outcometablesforattack;
				}
				if (EHAPKBCGPIA && !BECCDKJJDAC)
				{
					return NCBNEMAOHJE.Count;
				}
				if (0 < num13)
				{
					return num13;
				}
			}
			if (BECCDKJJDAC)
			{
				List<TemplateAnimation> list7 = AiData.get_EvadeThrowDodges();
				NCBNEMAOHJE.Clear();
				foreach (TemplateAnimation item5 in list7)
				{
					List<InfoAnimation> list8 = item5.LDEBJOPLCKO();
					foreach (InfoAnimation item6 in list8)
					{
						int jOHDCPNACOC2 = item6.HGMPJJACFHN();
						NCBNEMAOHJE.Add(new Decision(item6, jOHDCPNACOC2));
					}
				}
				num13 = NCBNEMAOHJE.Count;
				if (0 < num13)
				{
					DOPHKPJFBJN = AiData.HDHPLDFCDOF.evadeList;
				}
			}
			else if (JOOELCENMFC)
			{
				List<TemplateAnimation> list9 = AiData.get_CautiousMovements();
				NCBNEMAOHJE.Clear();
				foreach (TemplateAnimation item7 in list9)
				{
					List<InfoAnimation> list10 = item7.LDEBJOPLCKO();
					foreach (InfoAnimation item8 in list10)
					{
						int jOHDCPNACOC3 = item8.HGMPJJACFHN();
						NCBNEMAOHJE.Add(new Decision(item8, jOHDCPNACOC3));
					}
				}
				num13 = NCBNEMAOHJE.Count;
				if (0 < num13)
				{
					DOPHKPJFBJN = AiData.HDHPLDFCDOF.safeTable;
				}
			}
			if (num13 == 0)
			{
				SetRandomAnimation();
				PLDABIGHHFG = CBGBLIPAMGA.SetWaitAnimationLength;
				if (BECCDKJJDAC)
				{
					RemoveEvadeUnsafeDodgesAnimations();
				}
				return 0;
			}
		}
		return NCBNEMAOHJE.Count;
	}

	private static bool IsUninteruptIntervalEnd(ModelAnimation MEKLGEGJPFP)
	{
		if (MEKLGEGJPFP.NMEEPBDJHMG())
		{
			InfoAnimation pJAHIOELGGD = MEKLGEGJPFP.NNMAFFCCMHC();
			if (pJAHIOELGGD != null)
			{
				int num = pJAHIOELGGD.IKFCNCLKDGD(false);
				int num2 = MEKLGEGJPFP.LPFPGDJALED();
				if (num2 <= num)
				{
					return false;
				}
			}
		}
		return true;
	}

	private static bool IsAttackIntervalEnd(ModelAnimation MEKLGEGJPFP)
	{
		if (MEKLGEGJPFP.NMEEPBDJHMG())
		{
			InfoAnimation pJAHIOELGGD = MEKLGEGJPFP.NNMAFFCCMHC();
			if (pJAHIOELGGD != null)
			{
				int num = pJAHIOELGGD.MLLLLMFLOBG(false);
				int num2 = MEKLGEGJPFP.LPFPGDJALED();
				if (num2 <= num)
				{
					return false;
				}
			}
		}
		return true;
	}

	private int RemoveEvadeUnsafeDodgesAnimations()
	{
		int num = 0;
		List<string> list = AiData.get_EvadeUnsafeDodgesAnimations();
		foreach (Decision item in NCBNEMAOHJE)
		{
			bool flag = true;
			foreach (string item2 in list)
			{
				if (item.FGICHADOEHF.CNPFHBMGDFP(item2))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				NCBNEMAOHJE[num] = item;
				num++;
			}
		}
		return num;
	}

	private bool IsThrowingState(InfoAnimation DBOLBEOCEME, int IHICCKAOPKG)
	{
		List<string> nIKHAICFGNM = AiData.get_ThrowableIntervals();
		bool fPMGBALCKPI = true;
		return DBOLBEOCEME.FMFFHKJBHNG(nIKHAICFGNM, IHICCKAOPKG, fPMGBALCKPI);
	}

	private bool IsModelCanThrow(Model ACENLMONNPA)
	{
		PIOGLIJBPLL.Clear();
		List<string> nIKHAICFGNM = AiData.get_Throws();
		AnimationData.OCMIKNOMINM(nIKHAICFGNM, PIOGLIJBPLL);
		int num = PIOGLIJBPLL.Count;
		if (0 < num)
		{
			num = ACENLMONNPA.EEIGOJBKFGE().GetPlayableAnimations(PIOGLIJBPLL);
		}
		if (0 < num)
		{
			return true;
		}
		return false;
	}

	private bool get_IsEnabled()
	{
		return get_AiOn() && (NHDAJBADMND.EEGMBGBLLIF || AiData.get_BothBotEnabled());
	}

	private int DecisionsToAimationsList()
	{
		PIOGLIJBPLL.Clear();
		_InterframesList.Clear();
		foreach (Decision item in NCBNEMAOHJE)
		{
			PIOGLIJBPLL.Add(item.FGICHADOEHF);
			_InterframesList.Add(item.Wait);
		}
		return 0;
	}

	private int GetSafetyAnimations(Model FNKFIMEDNLP)
	{
		List<TemplateAnimation> list = AiData.get_CautiousMovements();
		PIOGLIJBPLL.Clear();
		foreach (TemplateAnimation item in list)
		{
			List<InfoAnimation> list2 = item.LDEBJOPLCKO();
			foreach (InfoAnimation item2 in list2)
			{
				PIOGLIJBPLL.Add(item2);
			}
		}
		InfoAnimation pJAHIOELGGD = FNKFIMEDNLP.OCPMJKIEPIG().NNMAFFCCMHC();
		NCBNEMAOHJE.Clear();
		foreach (InfoAnimation item3 in PIOGLIJBPLL)
		{
			int num = 0;
			if (pJAHIOELGGD != null && FNKFIMEDNLP.OCPMJKIEPIG().NMEEPBDJHMG())
			{
				int num2 = item3.HGMPJJACFHN();
				num = pJAHIOELGGD.IKFCNCLKDGD(true) - MEHOEEIGCEP + 1;
				if (num2 < num)
				{
					num = num2;
				}
			}
			NCBNEMAOHJE.Add(new Decision(item3, num));
		}
		return NCBNEMAOHJE.Count;
	}

	private int GetFromTablesMove(Model FNKFIMEDNLP)
	{
		if (MEHOEEIGCEP % AiData.KINPOOFGAGD != 0)
		{
			EHAPKBCGPIA = true;
		}
		else
		{
			EHAPKBCGPIA = false;
		}
		if (CGPDPHJIDPA != null && COKFBIJAFLH != null)
		{
			int num = GetFromTablesMove(FNKFIMEDNLP, CGPDPHJIDPA, COKFBIJAFLH, _ModelAnimation.FHGNPPBLIIL(), FNKFIMEDNLP.OCPMJKIEPIG().FHGNPPBLIIL(), NJPDFMHHIDE, MEHOEEIGCEP, OCCHDKNLAON);
			if (0 < num)
			{
				num = GetPlayableAnimations(NCBNEMAOHJE);
			}
			if (0 < num)
			{
				int num2 = 0;
				for (int i = 0; i < num; i++)
				{
					InfoAnimation fGICHADOEHF = NCBNEMAOHJE[i].FGICHADOEHF;
					if (fGICHADOEHF == null || TestWall(fGICHADOEHF, _Model, FNKFIMEDNLP))
					{
						if (num2 < i)
						{
							NCBNEMAOHJE[num2] = NCBNEMAOHJE[i];
						}
						num2++;
					}
				}
				num = num2;
				NCBNEMAOHJE.CPCAJIKOIEE(num);
			}
			return num;
		}
		return 0;
	}

	private int GetFromTablesMove(Model FNKFIMEDNLP, InfoAnimation GJAGBKICAGF, InfoAnimation GPHCOBPINCK, float JPPHFIKBFDM, float KACDJBFHICO, int FIPCJOMEPCJ, int FHDLJPNEOLD, float GLFFEFOFCFE, List<global::Pair<string, float>> PJCNHALDPHO = null)
	{
		NCBNEMAOHJE.Clear();
		GroupTables kMLMHGLLOHM = null;
		List<global::Pair<List<GroupTables>, string>> list = GPHCOBPINCK.NLCLHLIPFFH()[1];
		foreach (global::Pair<List<GroupTables>, string> item in list)
		{
			if (!(EIMKBOMDAAE == item.Second))
			{
				continue;
			}
			List<GroupTables> lLHEDBIEHAA = item.First;
			foreach (GroupTables item2 in lLHEDBIEHAA)
			{
				if (EIMKBOMDAAE == item2.GroupLabel)
				{
					kMLMHGLLOHM = item2;
					break;
				}
			}
			break;
		}
		if (kMLMHGLLOHM != null)
		{
			ModelAnimation oJIEPADIEDE = FNKFIMEDNLP.OCPMJKIEPIG();
			int num = oJIEPADIEDE.KFCNPADAMHA();
			if (IsFitBotAnimation(GJAGBKICAGF))
			{
				GroupTables cIHPJCIFLHN = kMLMHGLLOHM;
				string mMJNDPGKNPM = GJAGBKICAGF.KPIMAMCOEAN();
				int num2 = FHDLJPNEOLD - FIPCJOMEPCJ;
				int num3 = GetNearestKeyFrameId(num2);
				int num4 = num3 - num2;
				float oIOMNNFMDOO = (float)num * (JPPHFIKBFDM - KACDJBFHICO) + GLFFEFOFCFE;
				NCBNEMAOHJE.Clear();
				int num5 = GetRow(cIHPJCIFLHN, mMJNDPGKNPM, num3, oIOMNNFMDOO, NCBNEMAOHJE);
				foreach (Decision item3 in NCBNEMAOHJE)
				{
					if (GJAGBKICAGF == item3.FGICHADOEHF)
					{
						int num6 = item3.Wait - FIPCJOMEPCJ + num4;
						NCBNEMAOHJE.Clear();
						if (0 < num6)
						{
							NCBNEMAOHJE.Add(new Decision(null, num6));
							return NCBNEMAOHJE.Count;
						}
						break;
					}
				}
			}
			int num7 = ((FHDLJPNEOLD % AiData.KINPOOFGAGD == 0) ? FHDLJPNEOLD : (FHDLJPNEOLD + AiData.KINPOOFGAGD - FHDLJPNEOLD % AiData.KINPOOFGAGD));
			NCBNEMAOHJE.Clear();
			int num8 = _ModelAnimation.KFCNPADAMHA();
			int num9 = 0;
			int count = kMLMHGLLOHM.DOCMMNLEAMH.Count;
			for (int i = 0; i < count; i++)
			{
				string hOGFLOLGGOL = kMLMHGLLOHM.DOCMMNLEAMH[i].Label;
				float num10 = ((PJCNHALDPHO != null) ? GetNodeX(hOGFLOLGGOL, PJCNHALDPHO) : GetNodeX(hOGFLOLGGOL, _Model, FNKFIMEDNLP));
				float num11 = GJAGBKICAGF.OBIBINIEJJE.GetDistance(FHDLJPNEOLD, hOGFLOLGGOL);
				float num12 = GJAGBKICAGF.OBIBINIEJJE.GetDistance(num7, hOGFLOLGGOL);
				float num13 = num12 - num11;
				float oIOMNNFMDOO2 = (float)num * (num10 + num13 * (float)num8 - KACDJBFHICO) + GLFFEFOFCFE;
				num9 += GetRow(kMLMHGLLOHM, hOGFLOLGGOL, num7, oIOMNNFMDOO2, NCBNEMAOHJE);
			}
			if (num7 == FHDLJPNEOLD || num9 == 0)
			{
				return num9;
			}
			NCBNEMAOHJE.Clear();
			NCBNEMAOHJE.Add(new Decision(null, num7 - FHDLJPNEOLD));
			return 1;
		}
		return NCBNEMAOHJE.Count;
	}

	private int GetFromTablesAttack(Model FNKFIMEDNLP)
	{
		NCBNEMAOHJE.Clear();
		if (MEHOEEIGCEP % AiData.KINPOOFGAGD != 0)
		{
			EHAPKBCGPIA = true;
			return 0;
		}
		EHAPKBCGPIA = false;
		TacticFactors fJCBLOKOBBD = SetFactors(FNKFIMEDNLP);
		int num = GetEnemyResponseDelay(fJCBLOKOBBD);
		GroupTables kMLMHGLLOHM = null;
		List<global::Pair<List<GroupTables>, string>> list = COKFBIJAFLH.NLCLHLIPFFH()[0];
		foreach (global::Pair<List<GroupTables>, string> item in list)
		{
			if (!(EIMKBOMDAAE == item.Second))
			{
				continue;
			}
			List<GroupTables> lLHEDBIEHAA = item.First;
			foreach (GroupTables item2 in lLHEDBIEHAA)
			{
				if (EIMKBOMDAAE == item2.GroupLabel)
				{
					kMLMHGLLOHM = item2;
					break;
				}
			}
			break;
		}
		if (kMLMHGLLOHM != null)
		{
			ModelAnimation oJIEPADIEDE = FNKFIMEDNLP.OCPMJKIEPIG();
			int mEHOEEIGCEP = MEHOEEIGCEP;
			int nJPDFMHHIDE = NJPDFMHHIDE;
			float num2 = oJIEPADIEDE.FHGNPPBLIIL();
			int num3 = oJIEPADIEDE.KFCNPADAMHA();
			int count = kMLMHGLLOHM.DOCMMNLEAMH.Count;
			PIOGLIJBPLL.Clear();
			_InterframesList.Clear();
			int num4 = MEHOEEIGCEP + num;
			for (int i = 0; i < count; i++)
			{
				TacticalTable iCLOAGENLJG = kMLMHGLLOHM.DOCMMNLEAMH[i];
				int num5 = iCLOAGENLJG.GetArrayIndexByFrameIndex(mEHOEEIGCEP);
				if (-1 >= num5)
				{
					continue;
				}
				float num6 = GetNodeX(iCLOAGENLJG.Label, _Model, FNKFIMEDNLP);
				float oIOMNNFMDOO = (float)num3 * (num6 - num2) + OCCHDKNLAON;
				List<IntervalNew> mFFPCMPGEBK = iCLOAGENLJG.OCFKLCDIEBF[num5].MFFPCMPGEBK;
				foreach (IntervalNew item3 in mFFPCMPGEBK)
				{
					int num7 = item3.GetInterframeByDistance(oIOMNNFMDOO);
					if (0 < num7 && num7 <= num4)
					{
						PIOGLIJBPLL.Add(item3.FGICHADOEHF);
						_InterframesList.Add(item3.FGICHADOEHF.HGMPJJACFHN());
					}
				}
			}
			int num8 = PIOGLIJBPLL.Count;
			if (0 < num8)
			{
				num8 = GetPlayableAnimations(PIOGLIJBPLL, _InterframesList);
			}
			if (0 < num8)
			{
				int num9 = 0;
				for (int j = 0; j < num8; j++)
				{
					InfoAnimation pJAHIOELGGD = PIOGLIJBPLL[j];
					if (TestWall(pJAHIOELGGD, _Model, FNKFIMEDNLP))
					{
						if (num9 < j)
						{
							PIOGLIJBPLL[num9] = pJAHIOELGGD;
							_InterframesList[num9] = _InterframesList[j];
						}
						num9++;
					}
				}
				num8 = num9;
				PIOGLIJBPLL.CPCAJIKOIEE(num8);
				_InterframesList.CPCAJIKOIEE(num8);
			}
			for (int k = 0; k < num8; k++)
			{
				NCBNEMAOHJE.Add(new Decision(PIOGLIJBPLL[k], _InterframesList[k]));
			}
		}
		return NCBNEMAOHJE.Count;
	}

	private int GetFromTablesDodge(Model FNKFIMEDNLP, BHIOPDNPEPA NPIOFGMJDKI = BHIOPDNPEPA.Standard)
	{
		NCBNEMAOHJE.Clear();
		int num = 0;
		switch (NPIOFGMJDKI)
		{
		case BHIOPDNPEPA.Standard:
			num = MEHOEEIGCEP;
			break;
		case BHIOPDNPEPA.Missile:
			num = FNKFIMEDNLP.OCPMJKIEPIG().JFGEHNHLDJM();
			break;
		}
		if (num % AiData.KINPOOFGAGD != 0)
		{
			switch (NPIOFGMJDKI)
			{
			case BHIOPDNPEPA.Standard:
				EHAPKBCGPIA = true;
				break;
			case BHIOPDNPEPA.Missile:
			{
				EHAPKBCGPIA = false;
				int num2 = (num / AiData.KINPOOFGAGD + 1) * AiData.KINPOOFGAGD;
				int num3 = num2 - num;
				break;
			}
			}
		}
		else
		{
			EHAPKBCGPIA = false;
		}
		InfoAnimation pJAHIOELGGD = null;
		switch (NPIOFGMJDKI)
		{
		case BHIOPDNPEPA.Standard:
			pJAHIOELGGD = FNKFIMEDNLP.OCPMJKIEPIG().NNMAFFCCMHC();
			break;
		case BHIOPDNPEPA.Missile:
			pJAHIOELGGD = FNKFIMEDNLP.OCPMJKIEPIG().JJBEAOPDGCO();
			break;
		}
		GroupTables kMLMHGLLOHM = null;
		List<global::Pair<List<GroupTables>, string>> list = pJAHIOELGGD.NLCLHLIPFFH()[2];
		if (list.Count == 1)
		{
			List<GroupTables> lLHEDBIEHAA = list[0].First;
			if (lLHEDBIEHAA.Count == 1)
			{
				kMLMHGLLOHM = lLHEDBIEHAA[0];
				if (kMLMHGLLOHM != null)
				{
					List<string> nIKHAICFGNM = null;
					List<string> list2 = null;
					switch (NPIOFGMJDKI)
					{
					case BHIOPDNPEPA.Standard:
						nIKHAICFGNM = AiData.get_MovesFirstIteration();
						list2 = AiData.get_MovesLastIteration();
						break;
					case BHIOPDNPEPA.Missile:
						nIKHAICFGNM = AiData.get_MissilesFirstIteration();
						list2 = AiData.get_MissilesLastIteration();
						break;
					}
					ModelAnimation oJIEPADIEDE = FNKFIMEDNLP.OCPMJKIEPIG();
					InfoAnimation pJAHIOELGGD2 = CGPDPHJIDPA;
					if (_ModelAnimation.NMEEPBDJHMG() && pJAHIOELGGD2 != null)
					{
						InfoAnimation pJAHIOELGGD3 = pJAHIOELGGD2.IMFGMAAEMIC();
						if (pJAHIOELGGD3 != null)
						{
							pJAHIOELGGD2 = pJAHIOELGGD3;
						}
						bool flag = false;
						foreach (string item in list2)
						{
							if (pJAHIOELGGD2.CNPFHBMGDFP(item))
							{
								flag = true;
								break;
							}
						}
						if (flag)
						{
							string iCBBNJMLDJH = pJAHIOELGGD2.KPIMAMCOEAN();
							TacticalTable iCLOAGENLJG = kMLMHGLLOHM.GetTacticalTableByLabel(iCBBNJMLDJH);
							if (iCLOAGENLJG != null)
							{
								int num4 = num - NJPDFMHHIDE;
								int num5 = GetNearestKeyFrameId(num4);
								int num6 = num5 - num4;
								int num7 = iCLOAGENLJG.GetArrayIndexByFrameIndex(num5);
								if (-1 < num7)
								{
									float num8 = oJIEPADIEDE.FHGNPPBLIIL();
									if (NPIOFGMJDKI == BHIOPDNPEPA.Missile)
									{
										num8 = oJIEPADIEDE.LPNPNBJGKJM();
									}
									int num9 = oJIEPADIEDE.KFCNPADAMHA();
									float num10 = _ModelAnimation.FHGNPPBLIIL();
									float oIOMNNFMDOO = (float)num9 * (num10 - num8) + OCCHDKNLAON;
									Intervals gOOGNIPMCEM = iCLOAGENLJG.OCFKLCDIEBF[num7];
									bool flag2 = false;
									foreach (IntervalNew item2 in gOOGNIPMCEM.MFFPCMPGEBK)
									{
										if (item2.FGICHADOEHF == pJAHIOELGGD2)
										{
											int num11 = item2.GetInterframeByDistance(oIOMNNFMDOO);
											if (0 < num11)
											{
												flag2 = true;
												break;
											}
										}
									}
									if (!flag2)
									{
										int num12 = pJAHIOELGGD.MLLLLMFLOBG(true);
										int jOHDCPNACOC = num12 - MEHOEEIGCEP + 1;
										NCBNEMAOHJE.Add(new Decision(null, jOHDCPNACOC));
										return NCBNEMAOHJE.Count;
									}
								}
							}
						}
					}
					if (num % AiData.KINPOOFGAGD != 0)
					{
						switch (NPIOFGMJDKI)
						{
						case BHIOPDNPEPA.Standard:
							EHAPKBCGPIA = true;
							NCBNEMAOHJE.Clear();
							return 0;
						case BHIOPDNPEPA.Missile:
						{
							EHAPKBCGPIA = false;
							int num13 = (num / AiData.KINPOOFGAGD + 1) * AiData.KINPOOFGAGD;
							int num3 = num13 - num;
							break;
						}
						}
					}
					else
					{
						EHAPKBCGPIA = false;
					}
					PIOGLIJBPLL.Clear();
					AnimationData.OCMIKNOMINM(nIKHAICFGNM, PIOGLIJBPLL);
					int count = PIOGLIJBPLL.Count;
					int num14 = 0;
					for (int i = 0; i < count; i++)
					{
						InfoAnimation pJAHIOELGGD4 = PIOGLIJBPLL[i];
						if (TestWall(pJAHIOELGGD4, _Model, FNKFIMEDNLP))
						{
							if (num14 < i)
							{
								PIOGLIJBPLL[num14] = pJAHIOELGGD4;
							}
							num14++;
						}
					}
					count = num14;
					PIOGLIJBPLL.CPCAJIKOIEE(count);
					float num15 = oJIEPADIEDE.FHGNPPBLIIL();
					if (NPIOFGMJDKI == BHIOPDNPEPA.Missile)
					{
						num15 = oJIEPADIEDE.LPNPNBJGKJM();
					}
					int num16 = oJIEPADIEDE.KFCNPADAMHA();
					foreach (TacticalTable item3 in kMLMHGLLOHM.DOCMMNLEAMH)
					{
						string hOGFLOLGGOL = item3.Label;
						int num17;
						if (NPIOFGMJDKI == BHIOPDNPEPA.Missile)
						{
							num17 = num;
						}
						else
						{
							int num18 = GetNearestKeyFrameId(num);
							int num19 = num18 - num;
							num17 = item3.GetArrayIndexByFrameIndex(num18);
						}
						if (-1 < num17)
						{
							float num20 = GetNodeX(hOGFLOLGGOL, _Model, FNKFIMEDNLP.BDJBNOPNCNB());
							float oIOMNNFMDOO2 = (float)num16 * (num20 - num15) + OCCHDKNLAON;
							Intervals gOOGNIPMCEM2 = null;
							gOOGNIPMCEM2 = ((num17 >= item3.OCFKLCDIEBF.Count) ? item3.OCFKLCDIEBF[0] : item3.OCFKLCDIEBF[num17]);
							foreach (IntervalNew item4 in gOOGNIPMCEM2.MFFPCMPGEBK)
							{
								int num21 = item4.GetInterframeByDistance(oIOMNNFMDOO2);
								if (0 >= num21)
								{
									continue;
								}
								for (int j = 0; j < count; j++)
								{
									if (PIOGLIJBPLL[j] == item4.FGICHADOEHF)
									{
										count--;
										PIOGLIJBPLL[j] = PIOGLIJBPLL[count];
										break;
									}
								}
							}
						}
						else
						{
							count = 0;
						}
					}
					PIOGLIJBPLL.CPCAJIKOIEE(count);
					IntervalAnimation mNOIEOBBCMI = oJIEPADIEDE.HDJBHPOGKNJ(IntervalAnimation.NGAJJDIEDGF.INTERVAL_UNINTERRUPT);
					InfoAnimation pJAHIOELGGD5 = pJAHIOELGGD;
					bool flag3 = oJIEPADIEDE.NMEEPBDJHMG();
					if (mNOIEOBBCMI != null && pJAHIOELGGD5 != null && flag3)
					{
						int num22 = pJAHIOELGGD5.IKFCNCLKDGD(true);
						int num23 = num22 - MEHOEEIGCEP;
						for (int k = 0; k < count; k++)
						{
							InfoAnimation pJAHIOELGGD6 = PIOGLIJBPLL[k];
							int num24 = pJAHIOELGGD6.IKFCNCLKDGD(true);
							if (num23 <= num24 && !IsSafeDodges(pJAHIOELGGD6))
							{
								count--;
								PIOGLIJBPLL[k] = PIOGLIJBPLL[count];
								k--;
							}
						}
						PIOGLIJBPLL.CPCAJIKOIEE(count);
					}
					int count2 = PIOGLIJBPLL.Count;
					if (0 < count2)
					{
						count2 = GetPlayableAnimations(PIOGLIJBPLL);
					}
					if (PIOGLIJBPLL.Count == 0 && NPIOFGMJDKI == BHIOPDNPEPA.Standard)
					{
						List<TemplateAnimation> list3 = AiData.get_EmergencyDodgesAnimations();
						foreach (TemplateAnimation item5 in list3)
						{
							List<InfoAnimation> list4 = item5.LDEBJOPLCKO();
							foreach (InfoAnimation item6 in list4)
							{
								PIOGLIJBPLL.Add(item6);
							}
						}
						count2 = GetPlayableAnimations(PIOGLIJBPLL, null, true);
					}
					int num25 = PIOGLIJBPLL.Count;
					if (0 < num25)
					{
						num25 = GetPlayableAnimations(PIOGLIJBPLL);
					}
					for (int l = 0; l < num25; l++)
					{
						int num26 = pJAHIOELGGD.MLLLLMFLOBG(true);
						int jOHDCPNACOC2 = num26 - MEHOEEIGCEP + 1;
						NCBNEMAOHJE.Add(new Decision(PIOGLIJBPLL[l], jOHDCPNACOC2));
					}
				}
				return NCBNEMAOHJE.Count;
			}
			LLLOJBFMONN.Write("null dodge table");
			return 0;
		}
		LLLOJBFMONN.Write("null dodge table");
		return 0;
	}

	private int GetForDodgeMissiles(Model FNKFIMEDNLP, MFHIONPNAGO OKIFFDGBGDA)
	{
		int num = 0;
		List<Decision> list = new List<Decision>();
		List<Decision> list2 = new List<Decision>(NCBNEMAOHJE);
		int i = 0;
		for (int count = FNKFIMEDNLP.KGGIDBLBMDJ().Count; i < count; i++)
		{
			WeaponModel gKIANLDJFCH = FNKFIMEDNLP.KGGIDBLBMDJ()[i];
			List<Decision> jOJBDADJOAP = new List<Decision>(list);
			if ((IsMissileAnimation(gKIANLDJFCH.OCPMJKIEPIG().NNMAFFCCMHC()) || OKIFFDGBGDA != MFHIONPNAGO.SimpleMissile) && (IsMagicAnimation(gKIANLDJFCH.OCPMJKIEPIG().NNMAFFCCMHC()) || OKIFFDGBGDA != MFHIONPNAGO.MagicMissile) && gKIANLDJFCH.OCPMJKIEPIG().NNMAFFCCMHC() != null)
			{
				num = GetFromTablesDodge(gKIANLDJFCH, BHIOPDNPEPA.Missile);
				list = ((list.Count <= 0) ? new List<Decision>(NCBNEMAOHJE) : Intersection(jOJBDADJOAP, NCBNEMAOHJE));
			}
		}
		num = list.Count;
		if (list2.Count != 0)
		{
			NCBNEMAOHJE = Intersection(list2, list);
		}
		else
		{
			NCBNEMAOHJE = list;
		}
		return num;
	}

	private bool IsSafetyAnimations(InfoAnimation DBOLBEOCEME, Model ACENLMONNPA, int IHICCKAOPKG)
	{
		return false;
	}

	private int GetRow(GroupTables CIHPJCIFLHN, string MMJNDPGKNPM, int FMNGLKIGFNA, float OIOMNNFMDOO, List<Decision> OEMALIFPGPO)
	{
		int count = OEMALIFPGPO.Count;
		TacticalTable iCLOAGENLJG = CIHPJCIFLHN.GetTacticalTableByLabel(MMJNDPGKNPM);
		if (iCLOAGENLJG != null)
		{
			Intervals gOOGNIPMCEM = iCLOAGENLJG.GetFrameByFrameIndex(FMNGLKIGFNA);
			if (gOOGNIPMCEM != null)
			{
				foreach (IntervalNew item in gOOGNIPMCEM.MFFPCMPGEBK)
				{
					int num = item.GetInterframeByDistance(OIOMNNFMDOO);
					if (0 < num)
					{
						OEMALIFPGPO.Add(new Decision(item.FGICHADOEHF, num));
					}
				}
			}
		}
		return OEMALIFPGPO.Count - count;
	}

	private int GetModelDirection(Model ACENLMONNPA, Model FNKFIMEDNLP)
	{
		int num = 0;
		if (ACENLMONNPA.CLDMEJKGLBA().CJELIBMCCMA() == null || FNKFIMEDNLP.CLDMEJKGLBA().CJELIBMCCMA() == null)
		{
			return 0;
		}
		return (ACENLMONNPA.CLDMEJKGLBA().CJELIBMCCMA().ICLEOFDKDIF()
			.GILCBJJPKBK() < FNKFIMEDNLP.CLDMEJKGLBA().CJELIBMCCMA().ICLEOFDKDIF()
			.GILCBJJPKBK()) ? 1 : (-1);
	}

	private List<Decision> Intersection(List<Decision> JOJBDADJOAP, List<Decision> DLADGODCJMD)
	{
		List<Decision> list = new List<Decision>();
		foreach (Decision item in JOJBDADJOAP)
		{
			foreach (Decision item2 in DLADGODCJMD)
			{
				if (item2.FGICHADOEHF == item.FGICHADOEHF)
				{
					list.Add(item);
				}
			}
		}
		return list;
	}

	private bool TestBack(InfoAnimation DBOLBEOCEME, Model ACENLMONNPA, Model FNKFIMEDNLP)
	{
		return true && TestBack(DBOLBEOCEME, _Model, FNKFIMEDNLP, "NPivot", "NPivot");
	}

	private bool TestWall(InfoAnimation DBOLBEOCEME, Model ACENLMONNPA, Model FNKFIMEDNLP)
	{
		bool flag = true;
		Model fNKFIMEDNLP = FNKFIMEDNLP.BDJBNOPNCNB();
		return flag && TestWall(DBOLBEOCEME, _Model, fNKFIMEDNLP, "NPivot");
	}

	private bool TestBack(InfoAnimation DBOLBEOCEME, Model ACENLMONNPA, Model FNKFIMEDNLP, string name, string ODEADGPBDEM)
	{
		float num = ACENLMONNPA.CLDMEJKGLBA().EGHIDHMENEF(name).ICLEOFDKDIF()
			.GILCBJJPKBK();
		float num2 = FNKFIMEDNLP.CLDMEJKGLBA().EGHIDHMENEF(ODEADGPBDEM).ICLEOFDKDIF()
			.GILCBJJPKBK();
		int num3 = DBOLBEOCEME.IKFCNCLKDGD(true);
		if (num3 < 0)
		{
			num3 = 0;
		}
		float num4 = DBOLBEOCEME.OBIBINIEJJE.GetDistance(num3, name);
		float num5 = num + (float)GetModelDirection(ACENLMONNPA, FNKFIMEDNLP) * num4;
		float num8;
		if (FNKFIMEDNLP.OCPMJKIEPIG().NMEEPBDJHMG())
		{
			InfoAnimation pJAHIOELGGD = FNKFIMEDNLP.OCPMJKIEPIG().NNMAFFCCMHC();
			int mEHOEEIGCEP = MEHOEEIGCEP;
			int jAPBDIJOKDJ = mEHOEEIGCEP + num3;
			float num6 = pJAHIOELGGD.OBIBINIEJJE.GetDistance(jAPBDIJOKDJ, ODEADGPBDEM);
			float num7 = FNKFIMEDNLP.OCPMJKIEPIG().FHGNPPBLIIL();
			num8 = num7 + (float)FNKFIMEDNLP.KFCNPADAMHA() * num6;
		}
		else
		{
			num8 = FNKFIMEDNLP.CLDMEJKGLBA().EGHIDHMENEF(ODEADGPBDEM).ICLEOFDKDIF()
				.GILCBJJPKBK();
		}
		if ((num - num2) * (num5 - num8) < 0f)
		{
			return false;
		}
		return true;
	}

	private bool TestWall(InfoAnimation DBOLBEOCEME, Model ACENLMONNPA, Model FNKFIMEDNLP, string name)
	{
		int num = DBOLBEOCEME.IKFCNCLKDGD(true);
		if (num < 0)
		{
			num = 0;
		}
		float num2 = ACENLMONNPA.CLDMEJKGLBA().EGHIDHMENEF(name).ICLEOFDKDIF()
			.GILCBJJPKBK();
		float num3 = DBOLBEOCEME.OBIBINIEJJE.GetDistance(num, name);
		float num4 = num2 + (float)GetModelDirection(ACENLMONNPA, FNKFIMEDNLP) * num3;
		float num5 = ACENLMONNPA.OCPMJKIEPIG().KJFIBMMOEPI();
		float num6 = ACENLMONNPA.OCPMJKIEPIG().PHHHEGOBAPB();
		float num7 = ACENLMONNPA.OCPMJKIEPIG().IHKKEEOCOOF();
		float num8 = ACENLMONNPA.OCPMJKIEPIG().INAOPLIFJEJ();
		float num9 = ((!(num8 < num7)) ? num8 : num7);
		if (num4 - num9 < num5 || num6 < num4 + num9)
		{
			return false;
		}
		return true;
	}

	private bool IsIncludeIntervalAttack(InfoAnimation DBOLBEOCEME)
	{
		List<IntervalAnimation> cAANBJEPGAA = DBOLBEOCEME.ODACDCDONJE.Intervals;
		foreach (IntervalAnimation item in cAANBJEPGAA)
		{
			if (item.Type == IntervalAnimation.NGAJJDIEDGF.INTERVAL_ATTACK)
			{
				return true;
			}
		}
		return false;
	}

	private void SetQuickAttackRnd()
	{
		List<global::Pair<string, TacticValue>> list = GIBODDNLGJH.get_QuickAttacks();
		int count = list.Count;
		OAPNPEFCBJB.CPCAJIKOIEE(count);
		foreach (BHDKGLJIOJD item in OAPNPEFCBJB)
		{
			item.NKFHNNFHLMM = NekkiMath.randomFloat();
		}
	}

	private void SetQuickAttackChances(TacticFactors FJCBLOKOBBD)
	{
		List<global::Pair<string, TacticValue>> list = GIBODDNLGJH.get_QuickAttacks();
		int count = list.Count;
		for (int i = 0; i < count; i++)
		{
			BHDKGLJIOJD bHDKGLJIOJD = OAPNPEFCBJB[i];
			bHDKGLJIOJD.PAIFCMNKFCP = list[i].Second.GetValue(FJCBLOKOBBD);
			bHDKGLJIOJD.Flag = bHDKGLJIOJD.NKFHNNFHLMM < bHDKGLJIOJD.PAIFCMNKFCP;
		}
	}

	private void SetEvadesRnd()
	{
		List<global::Pair<string, TacticValue>> list = GIBODDNLGJH.get_Evades();
		int count = list.Count;
		JNOMCJLDJGE.CPCAJIKOIEE(count);
		foreach (BHDKGLJIOJD item in JNOMCJLDJGE)
		{
			item.NKFHNNFHLMM = NekkiMath.randomFloat();
		}
	}

	private void SetEvadesChances(TacticFactors FJCBLOKOBBD)
	{
		List<global::Pair<string, TacticValue>> list = GIBODDNLGJH.get_Evades();
		int count = list.Count;
		for (int i = 0; i < count; i++)
		{
			BHDKGLJIOJD bHDKGLJIOJD = JNOMCJLDJGE[i];
			bHDKGLJIOJD.PAIFCMNKFCP = list[i].Second.GetValue(FJCBLOKOBBD);
			bHDKGLJIOJD.Flag = bHDKGLJIOJD.NKFHNNFHLMM < bHDKGLJIOJD.PAIFCMNKFCP;
		}
	}

	private float GetDistanceToEnemy(Model ACENLMONNPA)
	{
		int aOJJBKLCHJO = GetModelDirection(ACENLMONNPA, ACENLMONNPA.EGGEACCDAEK().BDJBNOPNCNB());
		int aOJJBKLCHJO2 = GetModelDirection(ACENLMONNPA.EGGEACCDAEK().BDJBNOPNCNB(), ACENLMONNPA);
		ModelNode lCDGOCIAIDK = ACENLMONNPA.OCPMJKIEPIG().EGHIDHMENEF(AiData.get_DistanceNode(), aOJJBKLCHJO);
		ModelNode lCDGOCIAIDK2 = ACENLMONNPA.EGGEACCDAEK().BDJBNOPNCNB().OCPMJKIEPIG()
			.EGHIDHMENEF(AiData.get_DistanceNode(), aOJJBKLCHJO2);
		if (lCDGOCIAIDK != null && lCDGOCIAIDK2 != null)
		{
			float f = lCDGOCIAIDK.ICLEOFDKDIF().GILCBJJPKBK() - lCDGOCIAIDK2.ICLEOFDKDIF().GILCBJJPKBK();
			return Mathf.Abs(f);
		}
		return 0f;
	}

	private TacticFactors SetFactors(Model FNKFIMEDNLP)
	{
		TacticFactors oHKCJDCMOKN = new TacticFactors(_Model.FGACEEPJBIF(), _Model.GLEKCPCMINJ(), _Model.LPOJKGLFMAL());
		_Model.FGACEEPJBIF().GetCountAndDamage(true, COKFBIJAFLH, ref oHKCJDCMOKN.EOGLBDCLMBM, ref oHKCJDCMOKN.KFMJMBANIGF, ref oHKCJDCMOKN.AAKOCIPFDNM);
		oHKCJDCMOKN.MGICNNKKCAN = (ObscuredFloat)(_Model.KMMJCHDKBDO.KKMCHCNOHMB());
		oHKCJDCMOKN.DDGNCMJGDAG = (ObscuredFloat)(FNKFIMEDNLP.KMMJCHDKBDO.KKMCHCNOHMB());
		oHKCJDCMOKN.OLCKGMBDGOG = FNKFIMEDNLP.OCPMJKIEPIG().HILLKPNMCIP();
		oHKCJDCMOKN.NGMLGDJGBCD = ChildMaxModelFrame(FNKFIMEDNLP);
		oHKCJDCMOKN.DDFBIOFIDIH = GetDistanceToEnemy(_Model);
		oHKCJDCMOKN.HDCPIAPMFNO = _Model.OCPMJKIEPIG().NNMAFFCCMHC();
		oHKCJDCMOKN.PBDLLNEOIDG = FNKFIMEDNLP.OCPMJKIEPIG().NNMAFFCCMHC();
		return oHKCJDCMOKN;
	}
}
