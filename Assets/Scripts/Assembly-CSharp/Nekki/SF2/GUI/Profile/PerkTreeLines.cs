using UnityEngine;

namespace Nekki.SF2.GUI.Profile
{
	public class PerkTreeLines : SFMonoBehaviour<object>
	{
		[SerializeField]
		private GameObject _linesForTwoPerks;

		[SerializeField]
		private GameObject _topLine;

		[SerializeField]
		private GameObject _bottomLine;

		public void Init(bool CAAHFHBHAIC, bool IKNHLPGLLKB, bool ABNOAAMEBFJ)
		{
			_linesForTwoPerks.gameObject.SetActive(CAAHFHBHAIC);
			_topLine.gameObject.SetActive(!IKNHLPGLLKB);
			_bottomLine.gameObject.SetActive(!ABNOAAMEBFJ);
		}

		private void Start()
		{
		}

		private void Update()
		{
		}
	}
}
