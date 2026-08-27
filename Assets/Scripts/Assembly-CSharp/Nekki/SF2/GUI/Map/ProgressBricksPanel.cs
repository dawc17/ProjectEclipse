using System.Collections.Generic;
using UnityEngine;

namespace Nekki.SF2.GUI.Map
{
	public class ProgressBricksPanel : SFMonoBehaviour<object>
	{
		public const float INDICATOR_SMALL_SCALE = 0.71f;

		public const float INDICATOR_NORMAL_SCALE = 1f;

		public const float INDICATOR_IMAGE_SIZE = 96f;

		public const float INDICATOR_PADDING_X = -5f;

		public const int MAX_BIG_ICON_FIGHTS = 5;

		private const ushort BPOBIJJOOIH = 6;

		private const ushort ONOMMNNNJNL = 8;

		[SerializeField]
		private LabelAlias _lblTour;

		[SerializeField]
		private GameObject IndicatorFightPrefab;

		private List<IndicatorFight> _indicators = new List<IndicatorFight>();

		public void Init(Battle DPOOIONCEOA, FightList KOMGFJOCEDN)
		{
			foreach (IndicatorFight item in _indicators)
			{
				Object.Destroy(item.gameObject);
			}
			_indicators.Clear();
			_lblTour.set_Alias(string.Empty);
			_lblTour.set_text(string.Empty);
			List<FightList> list = DPOOIONCEOA.ANNHMNIHKCC();
			int num = list.Count;
			if (DPOOIONCEOA.get_Type() == BattleType.FightBosses || DPOOIONCEOA.get_Type() == BattleType.FightBossesReplayable)
			{
				num--;
			}
			bool flag = (DPOOIONCEOA.get_Type() != BattleType.FightBosses && DPOOIONCEOA.get_Type() != BattleType.FightBossesReplayable) || KOMGFJOCEDN.Index != num;
			if (!flag)
			{
				return;
			}
			float num2 = ((num <= 5) ? 1f : 0.71f);
			float num3 = 96f * num2;
			int num4 = ((num <= 12) ? 6 : 8);
			int num5 = 0;
			float num6 = Mathf.Ceil((float)num / (float)num4);
			RectTransform component = GetComponent<RectTransform>();
			component.sizeDelta = new Vector2(component.sizeDelta.x, num6 * num3);
			float num7 = -5f * num2;
			float num8 = (float)((double)(num6 / 2f) - 0.5) * num3;
			for (int i = 0; (float)i < num6; i++)
			{
				int num9 = (((float)i == num6 - 1f) ? (num % num4) : num4);
				if (num9 == 0)
				{
					num9 = num4;
				}
				float num10 = (float)num9 * num3 + (float)(num9 - 1) * num7;
				float num11 = (0f - num10) / 2f + num3 / 2f;
				for (int j = 0; j < num9; j++)
				{
					int index = i * num4 + j;
					bool flag2 = list[index].PGBKNLAEANJ == ConditionStatus.StatusComplete;
					bool cNNCIENODGE = list[index].CNNCIENODGE;
					GameObject gameObject = Object.Instantiate(IndicatorFightPrefab);
					IndicatorFight component2 = gameObject.GetComponent<IndicatorFight>();
					component2.gameObject.transform.SetParent(base.gameObject.transform, false);
					_indicators.Add(component2);
					if (cNNCIENODGE)
					{
						component2.set_CurrentState(IndicatorFight.ILPCJIPBONE.IsLocked);
					}
					else
					{
						component2.set_CurrentState((!flag2) ? IndicatorFight.ILPCJIPBONE.IsOff : IndicatorFight.ILPCJIPBONE.IsOn);
					}
					component2.set_Scale(num2);
					component2.transform.localPosition = new Vector3(num11, num8, component2.transform.localPosition.z);
					num11 += num3 + num7;
					if (flag2)
					{
						num5++;
					}
				}
				num8 -= num3;
			}
			switch (DPOOIONCEOA.get_Type())
			{
			case BattleType.FightChallenge:
			case BattleType.FightTournament:
			case BattleType.FightStory:
			case BattleType.FightAscension:
			case BattleType.FightReplayable:
			case BattleType.FightPVP:
			case BattleType.FightRaid:
			{
				string alias = "stage{" + (num5 + 1) + "}{" + num + "}";
				_lblTour.set_Alias(alias);
				break;
			}
			case BattleType.FightBosses:
			case BattleType.FightBossesReplayable:
				if (flag)
				{
					_lblTour.set_Alias("bodyguards");
				}
				else
				{
					_lblTour.set_text(LocalizationManager.GetString(DPOOIONCEOA.IGPOHDHPIIL()) + " " + LocalizationManager.GetString("challengeBoss"));
				}
				break;
			}
		}
	}
}
