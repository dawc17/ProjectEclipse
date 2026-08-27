using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Map
{
	public class ContentTourChallBoss : ContentBase
	{
		public const float MAX_OPACITY = 255f;

		[SerializeField]
		private LabelAlias _replaysLabel;

		[SerializeField]
		private ProgressBricksPanel _bricksPanel;

		[SerializeField]
		private DifficultyPanel _difficultyPanel;

		[SerializeField]
		private PrizePanel _prizePanel;

		[SerializeField]
		private LabelAlias _lblDescription;

		[SerializeField]
		private Button _challangeTouchZone;

		private float JEJJAFNDNJP = 255f;

		private float BIEHOAACFGA = 255f;

		private bool JFJKICJIAGD;

		private bool LDKGAEDCFGA;

		private bool NFOIAMOILCA;

		private bool JOGOAMIDLLG;

		private bool LFOMLGFOIDI = true;

		private int ALHECCDENMO;

		private void Update()
		{
			if (LFOMLGFOIDI)
			{
				CFFCPCEKCIE();
			}
		}

		public void Init(Battle DPOOIONCEOA, FightList KOMGFJOCEDN)
		{
			_bricksPanel.Init(DPOOIONCEOA, KOMGFJOCEDN);
			NAMOMLIKKIA(DPOOIONCEOA, KOMGFJOCEDN);
			MKHMHMAOKOA(DPOOIONCEOA);
			bool mMDLKOPCFLK = (DPOOIONCEOA.get_Type() != BattleType.FightBosses && DPOOIONCEOA.get_Type() != BattleType.FightBossesReplayable && DPOOIONCEOA.get_Type() != BattleType.FightFinalTitan) || !KJHIOOFNKEG(KOMGFJOCEDN);
			_prizePanel.Init(-1, mMDLKOPCFLK, KOMGFJOCEDN);
			_difficultyPanel.gameObject.SetActive(DPOOIONCEOA.KCIKELGFHOA() != 0);
			_difficultyPanel.GetComponent<CanvasGroup>().alpha = 1f;
			_difficultyPanel.Init(GameUtils.JEILJMPPEGL(KOMGFJOCEDN));
			FBOAIDMKGEB();
		}

		private void NAMOMLIKKIA(Battle DPOOIONCEOA, FightList KOMGFJOCEDN)
		{
			if (DPOOIONCEOA.get_Type() == BattleType.FightChallenge || DPOOIONCEOA.get_Type() == BattleType.FightReplayable)
			{
				string text = KOMGFJOCEDN.GJOAJAIJHOE();
				if (text != string.Empty)
				{
					_lblDescription.SetAlias(text);
				}
				bool flag = KOMGFJOCEDN != null && !KOMGFJOCEDN.ECEFCOJPBPG();
				_lblDescription.gameObject.SetActive(!flag);
				LFOMLGFOIDI = text != string.Empty;
				if (LFOMLGFOIDI)
				{
					DFMHJKAIJJK(OIMAMAGKGIA());
				}
				else
				{
					BIEHOAACFGA = 255f;
					JEJJAFNDNJP = 255f;
				}
				_lblDescription.HNIHBGAOAIH(JEJJAFNDNJP);
			}
			else
			{
				LFOMLGFOIDI = false;
				_lblDescription.gameObject.SetActive(false);
				_difficultyPanel.GetComponent<CanvasGroup>().alpha = 1f;
			}
		}

		private void MKHMHMAOKOA(Battle DPOOIONCEOA)
		{
			if (DPOOIONCEOA.get_Type() == BattleType.FightReplayable || DPOOIONCEOA.get_Type() == BattleType.FightBossesReplayable)
			{
				_replaysLabel.gameObject.SetActive(true);
				BattleReplayable bKKPCBGAEHC = (BattleReplayable)DPOOIONCEOA;
				string alias = "replays {" + bKKPCBGAEHC.HLBOMMKJAAO() + "}";
				_replaysLabel.SetAlias(alias);
			}
			else
			{
				_replaysLabel.gameObject.SetActive(false);
			}
		}

		private bool KJHIOOFNKEG(FightList KOMGFJOCEDN)
		{
			if (KOMGFJOCEDN == null)
			{
				return false;
			}
			RewardStruct fDFKLPHBAHJ = KOMGFJOCEDN.APKPCGDBMEP()[KOMGFJOCEDN.APKPCGDBMEP().Count - 1];
			int gNLOCMLBNHF = ListSF.CCDKHLAMKKO().PINDEKDNCNL();
			RewardPrize cMHHEHILIIH = fDFKLPHBAHJ.KOBOIFJNPMO(gNLOCMLBNHF);
			if (cMHHEHILIIH.HELFDCAIJNE.Count == 0)
			{
				return false;
			}
			RewardItem cACJANFAJEC = cMHHEHILIIH.HELFDCAIJNE[0];
			if (!cACJANFAJEC.GOOBKHECJIF)
			{
				return false;
			}
			ItemInfo dJKEECEOCJB = ListSF.DJBOFEEKJMP().KCCDBEEKBCG(cACJANFAJEC.Name);
			return dJKEECEOCJB != null;
		}

		private void DFMHJKAIJJK(bool LFGNIIJJMIG)
		{
			JEJJAFNDNJP = (LFGNIIJJMIG ? 255 : 0);
			BIEHOAACFGA = ((!LFGNIIJJMIG) ? 255 : 0);
			ALHECCDENMO = MapGUI.HPDGECMMHBJ.CPLJCIFJAGN;
			JFJKICJIAGD = LFGNIIJJMIG;
			LDKGAEDCFGA = !LFGNIIJJMIG;
			NFOIAMOILCA = !LFGNIIJJMIG;
			JOGOAMIDLLG = LFGNIIJJMIG;
		}

		private bool OIMAMAGKGIA()
		{
			return MapGUI.HPDGECMMHBJ.BGDLPJKHBHP == 0;
		}

		private void CFFCPCEKCIE()
		{
			if (ALHECCDENMO > 0)
			{
				ALHECCDENMO--;
				return;
			}
			if (JFJKICJIAGD)
			{
				EJIAKIMBKJK();
				if (JEJJAFNDNJP <= (float)MapGUI.HPDGECMMHBJ.IEKAFNFKBNE && !NFOIAMOILCA)
				{
					NFOIAMOILCA = true;
				}
				if (JEJJAFNDNJP == 0f)
				{
					JFJKICJIAGD = false;
				}
			}
			if (NFOIAMOILCA)
			{
				DNOIDOGIFNL();
				if (BIEHOAACFGA <= (float)MapGUI.HPDGECMMHBJ.IEKAFNFKBNE && !JFJKICJIAGD)
				{
					JFJKICJIAGD = true;
				}
				if (BIEHOAACFGA == 0f)
				{
					NFOIAMOILCA = false;
				}
			}
		}

		private void EJIAKIMBKJK()
		{
			int num = ((MapGUI.HPDGECMMHBJ.HPJHAIALGHN <= 0) ? 1 : MapGUI.HPDGECMMHBJ.HPJHAIALGHN);
			if (LDKGAEDCFGA)
			{
				JEJJAFNDNJP += 255f / (float)num;
				if (JEJJAFNDNJP >= 255f)
				{
					JEJJAFNDNJP = 255f;
					LDKGAEDCFGA = false;
					ALHECCDENMO = MapGUI.HPDGECMMHBJ.CPLJCIFJAGN;
				}
			}
			else
			{
				JEJJAFNDNJP -= 255f / (float)num;
				if (JEJJAFNDNJP <= 0f)
				{
					JEJJAFNDNJP = 0f;
					LDKGAEDCFGA = true;
				}
			}
			_lblDescription.HNIHBGAOAIH(JEJJAFNDNJP / 255f);
		}

		private void DNOIDOGIFNL()
		{
			int num = ((MapGUI.HPDGECMMHBJ.HPJHAIALGHN <= 0) ? 1 : MapGUI.HPDGECMMHBJ.HPJHAIALGHN);
			if (JOGOAMIDLLG)
			{
				BIEHOAACFGA += 255f / (float)num;
				if (BIEHOAACFGA >= 255f)
				{
					BIEHOAACFGA = 255f;
					JOGOAMIDLLG = false;
					ALHECCDENMO = MapGUI.HPDGECMMHBJ.CPLJCIFJAGN;
				}
			}
			else
			{
				BIEHOAACFGA -= 255f / (float)num;
				if (BIEHOAACFGA <= 0f)
				{
					BIEHOAACFGA = 0f;
					JOGOAMIDLLG = true;
				}
			}
			_difficultyPanel.GetComponent<CanvasGroup>().alpha = BIEHOAACFGA / 255f;
		}

		private void FBOAIDMKGEB()
		{
			_challangeTouchZone.onClick.RemoveListener(HNLFHIMJMFJ);
			_challangeTouchZone.onClick.AddListener(HNLFHIMJMFJ);
		}

		private void HNLFHIMJMFJ()
		{
			if (LFOMLGFOIDI)
			{
				DFMHJKAIJJK(JEJJAFNDNJP < BIEHOAACFGA);
				if (_lblDescription != null)
				{
					_lblDescription.HNIHBGAOAIH(JEJJAFNDNJP / 255f);
				}
				if (_difficultyPanel != null)
				{
					_difficultyPanel.GetComponent<CanvasGroup>().alpha = BIEHOAACFGA;
				}
			}
		}
	}
}
