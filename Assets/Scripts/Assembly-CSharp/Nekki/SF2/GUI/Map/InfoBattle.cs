using System.Collections.Generic;
using Nekki.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Map
{
	public class InfoBattle : SFMonoBehaviour<object>
	{
		private enum HKMFFHDLDBG
		{
			zIcon = 0,
			zContainer = 1,
			zContent = 2
		}

		private enum JLEIGFAOBJL
		{
			zContainerSprite = 0,
			zSlider = 1,
			zComplete = 2
		}

		private enum DGDGGMBNEIN
		{
			zIndicators = 0,
			zLabel = 1
		}

		private enum DGDCMJDCIIK
		{
			SkipFight = 0,
			RemoveFight = 1,
			PeriodicReset = 2
		}

		public const string FOLDER_BATTLES = "UI/battles/";

		public const int MAX_DESCRIPTION_LINES = 3;

		public const int MAX_TOUR_LINES = 1;

		public const int MAX_PERIODIC_DESCRIPTION_LINES = 3;

		public const float DESCRIPTION_LABEL_LINE_HEIGHT = 80f;

		public const float DESCRIPTION_LABEL_INTERMISSION_LINE_HEIGHT = 80f;

		public const int MAX_BIG_ICON_FIGHTS = 5;

		public const int SURVIVAL_LABEL_INTERMISSION_SIZE = 50;

		public const int SURVIVAL_MONEY_INTERMISSION_SIZE = 52;

		public const float INTERMISSION_MIN_OFFSET_Y = 40f;

		public const float INTERMISSION_MAX_OFFSET_Y = 40f;

		public const float INTERMISSION_MONEY_OFFSET_Y = 40f;

		public const float INTERMISSION_DESCRIPTION_OFFSET_Y = 16f;

		private FightIDS DICGPFLPAIH = new FightIDS();

		[SerializeField]
		private LabelAlias _lblBattleName;

		[SerializeField]
		private ResolutionImage _icon;

		[SerializeField]
		private ResolutionImage _altImage;

		[SerializeField]
		private LabelButton _btnFight;

		[SerializeField]
		private Button _btnSkipFight;

		[SerializeField]
		private Button _btnRemoveFight;

		[SerializeField]
		private Button _btnPeriodicReset;

		private int KODLOCOEFON;

		[SerializeField]
		private ProgressBar _immunityBar;

		[SerializeField]
		private Text _immunityLabel;

		private float CAMIKINNJLC;

		private float EMAIKDJGMGL;

		private int ALHECCDENMO;

		private ContentBase MHPKMPKPBPG;

		[SerializeField]
		private ContentClosed _contentClosed;

		[SerializeField]
		private ContentCompleteOrLocked _contentCompleteOrLocked;

		[SerializeField]
		private ContentTourChallBoss _contentTourChall;

		[SerializeField]
		private ContentTourChallBoss _contentBosses;

		[SerializeField]
		private ContentBossesFinal _contentBossesFinal;

		[SerializeField]
		private ContentDuel _contentDuel;

		[SerializeField]
		private ContentSurvival _contentSurvival;

		[SerializeField]
		private ContentSurvival _contentBossIntermission;

		public void Init()
		{
			_contentDuel.AddEventListener(0, OnCallUpdate);
			IDJDCJNEMDH();
			KGHCKIFPONO();
			DIBHHDHBKJA();
			KOAAKKGJOKO();
			ClearInfo();
			Module.ELEBLBJKDBI().AddEventListener(4, EFPMIKBJMLD);
		}

		~InfoBattle()
		{
		}

		public void OnCallUpdate(object DPOOIONCEOA)
		{
			UpdateBattleInfo(DPOOIONCEOA as Battle);
		}

		public void UpdateBattleInfo(Battle DPOOIONCEOA)
		{
			ClearInfo();
			FightList jDIPBIHBGPF = null;
			DICGPFLPAIH.SetFightIDSByString(string.Empty);
			if (DPOOIONCEOA == null)
			{
				return;
			}
			RosterBattle dDNLCGOPAGC = DPOOIONCEOA.NNPNEABKHPP();
			jDIPBIHBGPF = GameUtils.GKBHKJNGNPO(DPOOIONCEOA);
			if (jDIPBIHBGPF != null)
			{
				DICGPFLPAIH = new FightIDS(jDIPBIHBGPF.BCKFACGMOKC);
			}
			else
			{
				DICGPFLPAIH.SetFightIDSByZBF(string.Copy((DPOOIONCEOA.LKDFFCADHNO() == null) ? string.Empty : DPOOIONCEOA.LKDFFCADHNO().get_Name()), string.Copy(DPOOIONCEOA.get_Name()), string.Empty);
			}
			if (jDIPBIHBGPF != null && jDIPBIHBGPF.PPCNJPCPGGP != string.Empty)
			{
				string kHPKDMGDMAB = string.Empty;
				if (jDIPBIHBGPF != null && jDIPBIHBGPF.PPCNJPCPGGP != string.Empty)
				{
					kHPKDMGDMAB = jDIPBIHBGPF.PPCNJPCPGGP;
				}
				UpdateAltImage(SF2Paths.BHCPOOOJAAK(), kHPKDMGDMAB);
			}
			_lblBattleName.SetAlias(DPOOIONCEOA.IGPOHDHPIIL());
			UpdateIcon(DPOOIONCEOA.FGPAPMGHBDE());
			if (jDIPBIHBGPF != null && GameUtils.HHKHINLNCJB && GameUtils.NFBKHONMMDL != jDIPBIHBGPF.JKMJHIIMHPG + jDIPBIHBGPF.CNAOMDMIGLJ.get_Name() + jDIPBIHBGPF.Name)
			{
				GameUtils.HHKHINLNCJB = false;
			}
			if (_btnRemoveFight != null)
			{
				if (DPOOIONCEOA.CHLIJGLJAOA() && SystemProperties.DBBOCENKMGD())
				{
					_btnRemoveFight.gameObject.SetActive(true);
				}
				else
				{
					_btnRemoveFight.gameObject.SetActive(false);
				}
			}
			if ((dDNLCGOPAGC != null && dDNLCGOPAGC.NLIJBCHAEBK()) || DPOOIONCEOA.get_Type() == BattleType.FightFake)
			{
				_contentClosed.Init(DPOOIONCEOA.GJOAJAIJHOE());
				MHPKMPKPBPG = _contentClosed;
				MHPKMPKPBPG.gameObject.SetActive(true);
			}
			else if (jDIPBIHBGPF != null && jDIPBIHBGPF.CNNCIENODGE)
			{
				_contentClosed.Init(jDIPBIHBGPF.GJOAJAIJHOE());
				MHPKMPKPBPG = _contentClosed;
				MHPKMPKPBPG.gameObject.SetActive(true);
			}
			else if ((jDIPBIHBGPF != null && !jDIPBIHBGPF.ECEFCOJPBPG()) || DPOOIONCEOA.MNHLGELMOEJ() == ConditionStatus.StatusComplete || DPOOIONCEOA.MNHLGELMOEJ() == ConditionStatus.StatusIncomplete)
			{
				_contentCompleteOrLocked.Init(DPOOIONCEOA, DICGPFLPAIH);
				MHPKMPKPBPG = _contentCompleteOrLocked;
				MHPKMPKPBPG.gameObject.SetActive(true);
			}
			else
			{
				MMPGJGCCHHI(DPOOIONCEOA, jDIPBIHBGPF);
			}
		}

		public void ClearInfo()
		{
			if (MHPKMPKPBPG != null)
			{
				MHPKMPKPBPG.gameObject.SetActive(false);
				MHPKMPKPBPG = null;
			}
			DICGPFLPAIH.SetFightIDSByString(string.Empty);
			_lblBattleName.set_text(string.Empty);
			_btnFight.gameObject.SetActive(false);
			if (_btnRemoveFight != null)
			{
				_btnRemoveFight.gameObject.SetActive(false);
			}
			if (_btnRemoveFight != null)
			{
				_btnRemoveFight.gameObject.SetActive(false);
			}
			if (_altImage != null)
			{
				_altImage.gameObject.SetActive(false);
			}
			if (_immunityBar != null)
			{
				_immunityBar.gameObject.SetActive(false);
			}
			if (_immunityLabel != null)
			{
				_immunityLabel.gameObject.SetActive(false);
			}
		}

		public Battle GetCurrentBattle()
		{
			if (GetCurrentFight() != null)
			{
				return GetCurrentFight().CNAOMDMIGLJ;
			}
			return ListSF.MKHAAGMJOPG(DICGPFLPAIH);
		}

		public FightList GetCurrentFight()
		{
			return ListSF.CHMCKGCDGCM(DICGPFLPAIH);
		}

		public LabelButton GetBtnFight()
		{
			return _btnFight;
		}

		public Button GetBtnPlayVideo()
		{
			return _contentCompleteOrLocked.GetBtnPlayVideo();
		}

		public void StartCurrentFight()
		{
			FightList jDIPBIHBGPF = ListSF.CHMCKGCDGCM(DICGPFLPAIH);
			if (jDIPBIHBGPF != null)
			{
				StartFight(jDIPBIHBGPF);
			}
		}

		public void Refresh()
		{
			UpdateBattleInfo(GetCurrentBattle());
		}

		public bool GetIsHavePrize()
		{
			return false;
		}

		public float GetPrizePositionY()
		{
			return 0f;
		}

		private void IDJDCJNEMDH()
		{
			_btnFight.SetAlias("startFight");
			_btnFight.onClick.AddListener(FONNLNNBFHM);
			_btnFight.gameObject.SetActive(false);
		}

		private void KGHCKIFPONO()
		{
			_btnSkipFight.gameObject.SetActive(SystemProperties.DBBOCENKMGD());
			if (SystemProperties.DBBOCENKMGD())
			{
				_btnSkipFight.onClick.AddListener(() =>
				{
					GPAPKBPIPJP(DGDCMJDCIIK.SkipFight);
				});
			}
		}

		private void KOAAKKGJOKO()
		{
			_btnPeriodicReset.gameObject.SetActive(SystemProperties.DBBOCENKMGD());
			if (SystemProperties.DBBOCENKMGD())
			{
				_btnPeriodicReset.onClick.AddListener(() =>
				{
					GPAPKBPIPJP(DGDCMJDCIIK.PeriodicReset);
				});
			}
		}

		private void DIBHHDHBKJA()
		{
			_btnRemoveFight.gameObject.SetActive(SystemProperties.DBBOCENKMGD());
			if (SystemProperties.DBBOCENKMGD())
			{
				_btnRemoveFight.onClick.AddListener(() =>
				{
					GPAPKBPIPJP(DGDCMJDCIIK.RemoveFight);
				});
			}
		}

		private void UpdateIcon(string DAAIHHNLONA)
		{
			_icon.set_TexturePath("UI/battles/");
			_icon.set_SpriteName(DAAIHHNLONA);
		}

		private void UpdateAltImage(string KBIHPPDNFJD, string KHPKDMGDMAB)
		{
			if (!(KHPKDMGDMAB == string.Empty))
			{
				_altImage.set_SpriteName(KHPKDMGDMAB);
			}
		}

		private void OHNNACAAOKH(Battle DPOOIONCEOA, FightList KOMGFJOCEDN)
		{
			_btnFight.gameObject.SetActive(true);
			string alias = string.Empty;
			if (KOMGFJOCEDN != null && KOMGFJOCEDN.get_Type() == BattleType.FightRaid)
			{
				BattleRaid pAHLFJIMKCL = DPOOIONCEOA as BattleRaid;
				if (pAHLFJIMKCL != null)
				{
					List<CurrencyCostRule> list = KOMGFJOCEDN.LBGNOMEFLBA();
					if (list.Count == 0)
					{
						alias = "enterRaid";
					}
					else
					{
						string text = list[0].JFDCHNBPPNH();
						int num = list[0].LHNHLANLHMN();
					}
				}
			}
			else if (KOMGFJOCEDN != null && KOMGFJOCEDN.PCEPDPMOPKC())
			{
				List<CurrencyCostRule> list2 = KOMGFJOCEDN.LBGNOMEFLBA();
				string gOHIIMFFFJI = list2[0].JFDCHNBPPNH();
				int num2 = list2[0].LHNHLANLHMN();
				GameCurrency cJJOFMHLFFM = GameUtils.AJDKHINLIDI.ICFINJLNCPM(gOHIIMFFFJI);
				string mJBPMLCLMFN = cJJOFMHLFFM.MJBPMLCLMFN;
				alias = "startFight |<" + mJBPMLCLMFN + "><offsetX=10>" + num2 + "</>";
			}
			else
			{
				alias = "startFight";
			}
			_btnFight.SetAlias(alias);
		}

		private void FONNLNNBFHM()
		{
			GameUtils.HHKHINLNCJB = false;
			FightList jDIPBIHBGPF = ListSF.CHMCKGCDGCM(DICGPFLPAIH);
			if (jDIPBIHBGPF == null)
			{
				return;
			}
			if (jDIPBIHBGPF.get_Type() == BattleType.FightPeriodic)
			{
				if (!SystemProperties.DCKPKCIFOAG())
				{
					DialogsOpener.DNFMECAEDLJ();
				}
				else
				{
					GlobalTimer.ServerTimeSync(BDHAOMGEKBA, BDHAOMGEKBA);
				}
			}
			else if (jDIPBIHBGPF.get_Type() == BattleType.FightRaid)
			{
				Battle cNAOMDMIGLJ = jDIPBIHBGPF.CNAOMDMIGLJ;
				BattleRaid pAHLFJIMKCL = (BattleRaid)cNAOMDMIGLJ;
				if (pAHLFJIMKCL.DJCDFEAMPDA(jDIPBIHBGPF))
				{
					bool flag = ListSF.CCDKHLAMKKO().LDHANGLFDPJ();
					if (jDIPBIHBGPF.CENNLFIPNLH().Count != 0 && flag)
					{
					}
				}
				else
				{
					List<CurrencyCostRule> list = jDIPBIHBGPF.LBGNOMEFLBA();
					if (list.Count != 0)
					{
						JEKLMNFLDGK(list[0] as RaidCurrencyCostRule);
					}
				}
			}
			else
			{
				StartFight(jDIPBIHBGPF);
			}
		}

		private void MMPGJGCCHHI(Battle DPOOIONCEOA, FightList KOMGFJOCEDN)
		{
			switch (DPOOIONCEOA.get_Type())
			{
			case BattleType.FightChallenge:
			case BattleType.FightTournament:
			case BattleType.FightStory:
			case BattleType.FightReplayable:
				_contentTourChall.Init(DPOOIONCEOA, KOMGFJOCEDN);
				MHPKMPKPBPG = _contentTourChall;
				MHPKMPKPBPG.gameObject.SetActive(true);
				break;
			case BattleType.FightBosses:
			case BattleType.FightBossesReplayable:
			case BattleType.FightFinalTitan:
			{
				int num = DPOOIONCEOA.KCIKELGFHOA();
				if (KOMGFJOCEDN.Index != num - 1)
				{
					_contentBosses.Init(DPOOIONCEOA, KOMGFJOCEDN);
					MHPKMPKPBPG = _contentBosses;
				}
				else
				{
					_contentBossesFinal.Init(DPOOIONCEOA, KOMGFJOCEDN);
					MHPKMPKPBPG = _contentBossesFinal;
				}
				MHPKMPKPBPG.gameObject.SetActive(true);
				break;
			}
			case BattleType.FightFinal:
			case BattleType.FightFinalReplayable:
				_contentBossesFinal.Init(DPOOIONCEOA, KOMGFJOCEDN);
				MHPKMPKPBPG = _contentBossesFinal;
				MHPKMPKPBPG.gameObject.SetActive(true);
				break;
			case BattleType.FightPeriodic:
				_contentDuel.Init(DPOOIONCEOA, KOMGFJOCEDN);
				MHPKMPKPBPG = _contentDuel;
				MHPKMPKPBPG.gameObject.SetActive(true);
				break;
			case BattleType.FightSurvival:
				_contentSurvival.Init(DPOOIONCEOA);
				MHPKMPKPBPG = _contentSurvival;
				MHPKMPKPBPG.gameObject.SetActive(true);
				break;
			case BattleType.FightBossesIntermission:
				_contentBossIntermission.Init(DPOOIONCEOA);
				MHPKMPKPBPG = _contentBossIntermission;
				MHPKMPKPBPG.gameObject.SetActive(true);
				break;
			default:
				LLLOJBFMONN.Write("ERROR: openStatus() - unknown fight type: " + DPOOIONCEOA.get_Type());
				break;
			}
			OHNNACAAOKH(DPOOIONCEOA, KOMGFJOCEDN);
		}

		private void StartFight(FightList KGKDKENMAOA)
		{
			Battle cNAOMDMIGLJ = KGKDKENMAOA.CNAOMDMIGLJ;
			if (cNAOMDMIGLJ.get_Type() == BattleType.FightSurvival)
			{
				ListSF.CCDKHLAMKKO().set_IndexSlider((uint)KODLOCOEFON);
			}
			Battle dPOOIONCEOA = ((cNAOMDMIGLJ.get_Type() != BattleType.FightBosses && cNAOMDMIGLJ.get_Type() != BattleType.FightBossesReplayable && cNAOMDMIGLJ.get_Type() != BattleType.FightFinalTitan) ? null : cNAOMDMIGLJ);
			GameUtils.StartFight(KGKDKENMAOA, false, dPOOIONCEOA);
		}

		private void PDFOEKDFNHI()
		{
			Battle currentBattle = GetCurrentBattle();
			if (currentBattle != null)
			{
				if (currentBattle.get_Type() == BattleType.FightPeriodic)
				{
					BattlePeriodic.Reset(false);
				}
				UpdateBattleInfo(currentBattle);
			}
		}

		private void GPAPKBPIPJP(DGDCMJDCIIK PNBIFIIMEDL)
		{
			Battle currentBattle = GetCurrentBattle();
			if (currentBattle == null)
			{
				return;
			}
			switch (PNBIFIIMEDL)
			{
			case DGDCMJDCIIK.PeriodicReset:
				PDFOEKDFNHI();
				return;
			case DGDCMJDCIIK.RemoveFight:
				DFMGJEIJLCJ();
				currentBattle.BKGJCODJHKF();
				ListSF.CGJCKGAFPED();
				UpdateBattleInfo(currentBattle);
				return;
			}
			GameUtils.HHKHINLNCJB = true;
			FightList currentFight = GetCurrentFight();
			GameUtils.NFBKHONMMDL = ((currentFight != null) ? (currentFight.JKMJHIIMHPG + currentFight.CNAOMDMIGLJ.get_Name() + currentFight.Name) : string.Empty);
			if (currentFight == null)
			{
				return;
			}
			if (currentFight.get_Type() == BattleType.FightPeriodic)
			{
				if (!SystemProperties.DCKPKCIFOAG())
				{
					DialogsOpener.DNFMECAEDLJ();
				}
				else
				{
					GlobalTimer.ServerTimeSync(BDHAOMGEKBA, BDHAOMGEKBA);
				}
				return;
			}
			FightList jDIPBIHBGPF = currentFight;
			StartFight(currentFight);
			if (jDIPBIHBGPF != null)
			{
				int num = jDIPBIHBGPF.APKPCGDBMEP().Count - 2;
				if (num < 0)
				{
					num = 0;
				}
				jDIPBIHBGPF.JABJLCEJDDM = num;
			}
			UpdateBattleInfo(GetCurrentBattle());
		}

		private void BDHAOMGEKBA()
		{
			if (!GlobalTimer.get_IsSynchronized())
			{
				DialogsOpener.DNFMECAEDLJ();
				UpdateBattleInfo(GetCurrentBattle());
				return;
			}
			FightList currentFight = GetCurrentFight();
			if (currentFight != null && !currentFight.ECEFCOJPBPG())
			{
				UpdateBattleInfo(GetCurrentBattle());
			}
			else if (currentFight != null)
			{
				StartFight(currentFight);
			}
		}

		private void EFPMIKBJMLD(object data)
		{
			GameUtils.HHKHINLNCJB = false;
		}

		private void DFMGJEIJLCJ()
		{
			FightList currentFight = GetCurrentFight();
			if (currentFight != null && currentFight.BCKFACGMOKC.ToString() == "ZONE_6|BOSS_SAMURAI|6")
			{
				FightIDS dIAIIPCBMFL = new FightIDS("ZONE_6", "QuestBattle", string.Empty);
				Battle cGJCGEBPCAF = ListSF.MKHAAGMJOPG(dIAIIPCBMFL);
				if (cGJCGEBPCAF != null)
				{
					cGJCGEBPCAF.BKGJCODJHKF();
				}
			}
		}

		private void JEKLMNFLDGK(RaidCurrencyCostRule HNBFMAKFJAM)
		{
			if (HNBFMAKFJAM == null)
			{
				LLLOJBFMONN.Error("showRaidsNotEnoughKeys rule is NULL");
			}
			else
			{
				string text = HNBFMAKFJAM.JFDCHNBPPNH();
			}
		}

		private void GLGFKFICAOC(object data)
		{
			Battle currentBattle = GetCurrentBattle();
			if (currentBattle != null && currentBattle.get_Type() == BattleType.FightRaid)
			{
				Refresh();
			}
		}
	}
}
