using UnityEngine;

namespace Nekki.SF2.GUI.Profile
{
	public class TrickCell : ProfileCell
	{
		private const int DADLDEKKDKP = 2;

		[SerializeField]
		private TrickSubItem _trickSubItem;

		private void BHNDDBGCBNP()
		{
			_trickSubItem.ParentCell = this;
			_trickSubItem.transform.BGNJGIACJBG(0f);
			_trickSubItem.transform.OKHPLHPBPKJ(-240f);
			_trickSubItem.RemoveAllEventListener();
			_trickSubItem.AddEventListener(2, OnSubItemClick);
			_trickSubItem.AddEventListener(10, Scene<ProfileScene>.get_Current().OnSubItemClick);
			_trickSubItem.AddEventListener(12, Scene<ProfileScene>.get_Current().OnTrickShow);
		}

		public void Init(Trick KPKPFFGEFGI, int BIPGPCAHKIG)
		{
			Clear();
			BHNDDBGCBNP();
			string nHKMCLPOMFK = KPKPFFGEFGI.NHKMCLPOMFK;
			int oKNNNLIPODI = 20000 + BIPGPCAHKIG * 10;
			_trickSubItem.Init(nHKMCLPOMFK, KPKPFFGEFGI, oKNNNLIPODI);
			Scene<ProfileScene>.get_Current().SubItems.Add(_trickSubItem);
		}

		public override SubItem GetFirstIcon()
		{
			return _trickSubItem;
		}

		public override void UpdateState()
		{
			_trickSubItem.UpdateState();
		}

		public override void Clear()
		{
			Scene<ProfileScene>.get_Current().SubItems.Remove(_trickSubItem);
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
			_trickSubItem.RemoveAllEventListener();
		}
	}
}
