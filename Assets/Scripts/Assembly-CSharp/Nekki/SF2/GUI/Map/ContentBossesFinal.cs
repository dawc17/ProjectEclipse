using UnityEngine;

namespace Nekki.SF2.GUI.Map
{
	public class ContentBossesFinal : ContentBase
	{
		[SerializeField]
		private LabelAlias _replaysLabel;

		[SerializeField]
		private LabelAlias _lblDescription;

		[SerializeField]
		private PrizePanel _prizePanel;

		public void Init(Battle DPOOIONCEOA, FightList KOMGFJOCEDN)
		{
			string empty = string.Empty;
			empty = ((DPOOIONCEOA.get_Type() == BattleType.FightBosses || DPOOIONCEOA.get_Type() == BattleType.FightBossesReplayable || DPOOIONCEOA.get_Type() == BattleType.FightFinalTitan) ? (LocalizationManager.GetString(DPOOIONCEOA.IGPOHDHPIIL()) + " " + LocalizationManager.GetString("challengeBoss")) : LocalizationManager.GetString(DPOOIONCEOA.GJOAJAIJHOE()));
			_lblDescription.set_text(empty);
			MKHMHMAOKOA(DPOOIONCEOA);
			bool mMDLKOPCFLK = (DPOOIONCEOA.get_Type() != BattleType.FightBosses && DPOOIONCEOA.get_Type() != BattleType.FightBossesReplayable && DPOOIONCEOA.get_Type() != BattleType.FightFinalTitan) || !KJHIOOFNKEG(KOMGFJOCEDN);
			_prizePanel.Init(-1, mMDLKOPCFLK, KOMGFJOCEDN);
		}

		private void MKHMHMAOKOA(Battle DPOOIONCEOA)
		{
			if (DPOOIONCEOA.get_Type() == BattleType.FightFinalReplayable)
			{
				_replaysLabel.gameObject.SetActive(true);
				BattleReplayable bKKPCBGAEHC = (BattleReplayable)DPOOIONCEOA;
				string alias = "replays {1}{" + bKKPCBGAEHC.HLBOMMKJAAO() + "}";
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
	}
}
