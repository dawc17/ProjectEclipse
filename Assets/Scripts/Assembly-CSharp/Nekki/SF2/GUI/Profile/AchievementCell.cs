using UnityEngine;

namespace Nekki.SF2.GUI.Profile
{
	public class AchievementCell : ProfileCell
	{
		private const int HLPKKEEPNCJ = 3;

		[SerializeField]
		private AchievementSubItem achievSubItem;

		private void BHNDDBGCBNP()
		{
			achievSubItem.ParentCell = this;
			achievSubItem.transform.BGNJGIACJBG(0f);
			achievSubItem.transform.OKHPLHPBPKJ(-240f);
			achievSubItem.RemoveAllEventListener();
			achievSubItem.AddEventListener(2, OnSubItemClick);
			achievSubItem.AddEventListener(10, Scene<ProfileScene>.get_Current().OnSubItemClick);
			achievSubItem.AddEventListener(12, Scene<ProfileScene>.get_Current().OnAchievementRewardTake);
		}

		public void Init(Achievement PGAGNLJABIE, int EPJGLECOIBG, int IBAKGENOEPH)
		{
			Clear();
			BHNDDBGCBNP();
			string kHPKDMGDMAB = PGAGNLJABIE.CIOKDNDHFBE();
			int oKNNNLIPODI = 30000 + IBAKGENOEPH * 10;
			achievSubItem.Init(kHPKDMGDMAB, PGAGNLJABIE.Name, PGAGNLJABIE.MGNNJPBCOGD, PGAGNLJABIE.EOGLBDCLMBM, EPJGLECOIBG, oKNNNLIPODI, PGAGNLJABIE);
			Scene<ProfileScene>.get_Current().SubItems.Add(achievSubItem);
		}

		public override SubItem GetFirstIcon()
		{
			return achievSubItem;
		}

		public override void UpdateState()
		{
			achievSubItem.UpdateState();
		}

		public override void Clear()
		{
			Scene<ProfileScene>.get_Current().SubItems.Remove(achievSubItem);
		}

		public void OnSubItemClick(object data)
		{
			if (DidSelectEvent != null)
			{
				DidSelectEvent.Invoke(get_RowNumber());
			}
		}

		private void OnDestroy()
		{
			RemoveAllEventListener();
			achievSubItem.RemoveAllEventListener();
		}
	}
}
