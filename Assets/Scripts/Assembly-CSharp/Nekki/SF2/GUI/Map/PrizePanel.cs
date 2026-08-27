using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace Nekki.SF2.GUI.Map
{
	public class PrizePanel : SFMonoBehaviour<object>
	{
		[SerializeField]
		private BattlePrize _prize;

		public const int MONEY_LABEL_SIZE = 68;

		public const float PRIZE_CONTAINER_HEIGHT = 200f;

		public const float PRIZE_CONTAINER_HEIGHT_BOSSES = 300f;

		public void Init(int count, bool MMDLKOPCFLK, FightList KOMGFJOCEDN)
		{
			if (KOMGFJOCEDN != null)
			{
				long bAINMLLIKOL = 0L;
				long num = 0L;
				if (MMDLKOPCFLK)
				{
					bAINMLLIKOL = (ObscuredLong)(KOMGFJOCEDN.LDHOBIADNEC);
					num = (ObscuredLong)(KOMGFJOCEDN.JBNAJPPNGFB);
				}
				bAINMLLIKOL = GameUtils.GetDenominatedValue(bAINMLLIKOL);
				if (count > 0)
				{
					bAINMLLIKOL *= count;
					num *= count;
				}
				RewardStruct fDFKLPHBAHJ = null;
				if (KOMGFJOCEDN.APKPCGDBMEP().Count > 0)
				{
					fDFKLPHBAHJ = KOMGFJOCEDN.APKPCGDBMEP()[KOMGFJOCEDN.APKPCGDBMEP().Count - 1];
				}
				float num2 = 0f;
				BattleType pJMEMGHKKBM = KOMGFJOCEDN.get_Type();
				num2 = ((pJMEMGHKKBM != BattleType.FightBosses && pJMEMGHKKBM != BattleType.FightFinalTitan) ? 200f : 300f);
				int gNLOCMLBNHF = ListSF.CCDKHLAMKKO().PINDEKDNCNL();
				RewardPrize dPIIJICBGGA = fDFKLPHBAHJ.KOBOIFJNPMO(gNLOCMLBNHF);
				int cFMPJLLNCFF = 68;
				_prize.Init(bAINMLLIKOL, num, dPIIJICBGGA, 0f, num2, cFMPJLLNCFF);
				PDDFGIGHAEE(0f, num2);
			}
		}

		private void PDDFGIGHAEE(float JMLAKAKDBBL, float FEIHFIPFNKF)
		{
		}
	}
}
