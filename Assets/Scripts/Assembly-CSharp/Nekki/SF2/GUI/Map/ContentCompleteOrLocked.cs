using System;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Map
{
	public class ContentCompleteOrLocked : ContentBase
	{
		[SerializeField]
		protected LabelAlias _lblLocked;

		[SerializeField]
		protected Text _lblTime;

		[SerializeField]
		protected Button _btnPlayVideo;

		private Battle FODLHLABAMI;

		private FightIDS DICGPFLPAIH = new FightIDS();

		private void Update()
		{
			FightList jDIPBIHBGPF = ListSF.CHMCKGCDGCM(DICGPFLPAIH);
			if (jDIPBIHBGPF != null)
			{
				if (!jDIPBIHBGPF.ECEFCOJPBPG() && _lblTime != null)
				{
					long num = jDIPBIHBGPF.RepeatTime - jDIPBIHBGPF.FLKFFDLLBKA().CCCIFDLEMPI();
					TimeSpan timeSpan = TimeSpan.FromSeconds(num);
					string empty = string.Empty;
					empty = ((timeSpan.Hours <= 0) ? string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds) : string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds));
					_lblTime.text = empty;
				}
				else
				{
					CallEvent(0, FODLHLABAMI);
				}
			}
		}

		public void Init(Battle DPOOIONCEOA, FightIDS MMEJHKCKFDD)
		{
			FODLHLABAMI = DPOOIONCEOA;
			DICGPFLPAIH = new FightIDS(MMEJHKCKFDD);
			bool flag = DPOOIONCEOA.MNHLGELMOEJ() == ConditionStatus.StatusComplete;
			string empty = string.Empty;
			empty = ((DPOOIONCEOA.KCIKELGFHOA() != 0) ? ((!flag) ? "battleLocked" : "battleCompleted") : "battleComingSoon");
			_lblLocked.SetAlias(empty);
			if (_lblTime != null)
			{
				_lblTime.text = string.Empty;
			}
			_btnPlayVideo.onClick.AddListener(INDMIAIIHDD);
			if (DPOOIONCEOA.get_Type() == BattleType.FightFinal || DPOOIONCEOA.get_Type() == BattleType.FightFinalTitan)
			{
				_btnPlayVideo.gameObject.SetActive(true);
			}
			else
			{
				_btnPlayVideo.gameObject.SetActive(false);
			}
		}

		private void INDMIAIIHDD()
		{
			if (ListSF.ELEBLBJKDBI().FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_VIDEO_BUTTON_PRESS))
			{
				ListSF.ELEBLBJKDBI().MHHNIPBJNAD();
			}
		}

		public Button GetBtnPlayVideo()
		{
			return _btnPlayVideo;
		}
	}
}
