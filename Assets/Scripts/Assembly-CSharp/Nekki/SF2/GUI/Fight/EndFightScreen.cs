using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Fight
{
	public class EndFightScreen : MonoBehaviour, BackKeyController
	{
		[SerializeField]
		private GameObject _endFightContentPrefab;

		[SerializeField]
		private Transform _content;

		[SerializeField]
		private ResolutionImage _resultHeader;

		[SerializeField]
		private Button _animationFinishButton;

		[SerializeField]
		private GameObject _contentLayout;

		private EndFightContent _endFightContent;

		private FightResult MPFLHOFEOGI;

		private string GLNMNCPDNFJ = "FightUI.Label_Win";

		private string ADGALAFHMHH = "FightUI.Label_Lose";

		private string IBAIBAAPGDG = "FightUI.Label_Timesup";

		public void Init(FightResult HEIADONEACH)
		{
			EnableFinishButton(false);
			MPFLHOFEOGI = HEIADONEACH;
			if (_resultHeader != null)
			{
				bool flag = MPFLHOFEOGI.IsWinner();
				bool flag2 = MPFLHOFEOGI.EKBAHCGBNEM();
				_resultHeader.set_SpriteName(flag ? GLNMNCPDNFJ : ((!flag2) ? ADGALAFHMHH : IBAIBAAPGDG));
				_resultHeader.SetNativeSize();
			}
			if (_endFightContentPrefab != null)
			{
				_endFightContent = Object.Instantiate(_endFightContentPrefab).GetComponent<EndFightContent>();
				_endFightContent.gameObject.SetActive(true);
				Transform parent = ((!(_content != null)) ? base.transform : _content);
				_endFightContent.transform.SetParent(parent, false);
				_endFightContent.Init(MPFLHOFEOGI, _contentLayout.GetComponent<VerticalLayoutGroup>(), _animationFinishButton);
				_endFightContent.CloseEvent.AddListener(() =>
				{
					GameUtils.FKMEIHGOFDD(MPFLHOFEOGI);
				});
				_endFightContent.AnimationEndEvent.AddListener(MLPIMHGLKDI);
			}
		}

		private void EnableFinishButton(bool IJHFJPBBNEJ)
		{
			if (_animationFinishButton != null)
			{
				_animationFinishButton.gameObject.SetActive(IJHFJPBBNEJ);
			}
		}

		private void MLPIMHGLKDI()
		{
			EnableFinishButton(false);
		}

		public void OnAnimationFinishButton()
		{
			if (_endFightContent != null)
			{
				_endFightContent.FinishAnimation();
			}
			EnableFinishButton(false);
		}

		private void Awake()
		{
			BackKeyManager.get_Instance().AddBackKeyController(this);
		}

		private void OnDestroy()
		{
			BackKeyManager.get_Instance().RemoveBackKeyController(this);
		}

		public void OnBackKeyClicked(object GHDPPHAAPCA)
		{
			GameUtils.FKMEIHGOFDD(MPFLHOFEOGI);
		}
	}
}
