using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace Nekki.SF2.GUI.Map
{
	public class ContentSurvival : ContentBase
	{
		[SerializeField]
		private LabelAlias _minLabel;

		[SerializeField]
		private LabelAlias _maxLabel;

		[SerializeField]
		private BattlePrize _survivalPrizeMin;

		[SerializeField]
		private BattlePrize _survivalPrizeMax;

		[SerializeField]
		private LabelAlias _lblDescription;

		public const float PRIZE_CONTAINER_HEIGHT = 100f;

		public const int MONEY_LABEL_SIZE = 68;

		public const int SURVIVAL_LABEL_INTERMISSION_SIZE = 50;

		public void Init(Battle DPOOIONCEOA)
		{
			List<FightList> list = DPOOIONCEOA.ANNHMNIHKCC();
			_minLabel.gameObject.SetActive(list.Count > 0);
			_maxLabel.gameObject.SetActive(list.Count > 0);
			_survivalPrizeMin.gameObject.SetActive(list.Count > 0);
			_survivalPrizeMax.gameObject.SetActive(list.Count > 0);
			if (list.Count == 0)
			{
				return;
			}
			List<RewardStruct> list2 = list[0].APKPCGDBMEP();
			long num = 0L;
			long num2 = 0L;
			long num3 = 0L;
			long num4 = 0L;
			int count = list2.Count;
			if (count < 2)
			{
				LLLOJBFMONN.Error("Survival has no rewards");
				return;
			}
			int gNLOCMLBNHF = ListSF.CCDKHLAMKKO().PINDEKDNCNL();
			RewardPrize cMHHEHILIIH = list2[1].KOBOIFJNPMO(gNLOCMLBNHF);
			RewardPrize cMHHEHILIIH2 = list2[list2.Count - 1].KOBOIFJNPMO(gNLOCMLBNHF);
			num = (ObscuredLong)(cMHHEHILIIH.PNDAIFALIKF);
			num2 = (ObscuredLong)(cMHHEHILIIH2.PNDAIFALIKF);
			num3 = (ObscuredLong)(cMHHEHILIIH.GBGNFPNCGED);
			num4 = (ObscuredLong)(cMHHEHILIIH2.GBGNFPNCGED);
			num3 = GameUtils.GetDenominatedValue(num3);
			num4 = GameUtils.GetDenominatedValue(num4);
			int cFMPJLLNCFF = 68;
			if (DPOOIONCEOA.get_Type() == BattleType.FightBossesIntermission)
			{
				cFMPJLLNCFF = 50;
			}
			_survivalPrizeMin.Init(num3, num, cMHHEHILIIH, 0f, 100f, cFMPJLLNCFF);
			_survivalPrizeMax.Init(num4, num2, cMHHEHILIIH2, 0f, 100f, cFMPJLLNCFF);
			if (DPOOIONCEOA.get_Type() == BattleType.FightBossesIntermission)
			{
				_lblDescription.SetAlias(DPOOIONCEOA.GJOAJAIJHOE());
			}
		}
	}
}
