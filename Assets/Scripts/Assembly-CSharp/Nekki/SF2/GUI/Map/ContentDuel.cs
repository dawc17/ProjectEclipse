using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Map
{
	public class ContentDuel : ContentBase
	{
		[SerializeField]
		private LabelAlias _lblDescription;

		[SerializeField]
		private DifficultyPanel _difficultyPanel;

		[SerializeField]
		private PrizePanel _prizePanel;

		[SerializeField]
		protected Button _btnPeriodicReset;

		private Battle FODLHLABAMI;

		public void Init(Battle DPOOIONCEOA, FightList KOMGFJOCEDN)
		{
			FODLHLABAMI = DPOOIONCEOA;
			_lblDescription.SetAlias(KOMGFJOCEDN.GJOAJAIJHOE());
			bool mMDLKOPCFLK = (DPOOIONCEOA.get_Type() != BattleType.FightBosses && DPOOIONCEOA.get_Type() != BattleType.FightBossesReplayable && DPOOIONCEOA.get_Type() != BattleType.FightFinalTitan) || !KJHIOOFNKEG(KOMGFJOCEDN);
			_prizePanel.Init(-1, mMDLKOPCFLK, KOMGFJOCEDN);
			_difficultyPanel.gameObject.SetActive(DPOOIONCEOA.KCIKELGFHOA() != 0);
			_difficultyPanel.Init(GameUtils.JEILJMPPEGL(KOMGFJOCEDN));
			_btnPeriodicReset.gameObject.SetActive(false);
			if (DPOOIONCEOA.get_Type() == BattleType.FightPeriodic && SystemProperties.DBBOCENKMGD())
			{
				_btnPeriodicReset.gameObject.SetActive(true);
				_btnPeriodicReset.onClick.AddListener(PDFOEKDFNHI);
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

		private void PDFOEKDFNHI()
		{
			if (FODLHLABAMI != null)
			{
				if (FODLHLABAMI.get_Type() == BattleType.FightPeriodic)
				{
					BattlePeriodic.Reset(false);
				}
				CallEvent(0, FODLHLABAMI);
			}
		}
	}
}
