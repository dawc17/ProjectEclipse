using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Menu
{
	public class MenuExpPanel : SFMonoBehaviour<object>
	{
		public enum MKGNLOJGOEA
		{
			onLevelHintBtnClicked = 0
		}

		[SerializeField]
		private Image _iconLevel;

		[SerializeField]
		private Text _labelLevel;

		[SerializeField]
		private ProgressBar _barExp;

		[SerializeField]
		private Image _iconMaxLevel;

		[SerializeField]
		private Button _levelHintButton;

		[SerializeField]
		private Text _hintText;

		[SerializeField]
		private GameObject _hintRootGO;

		private int IAGHIGDNCGO;

		public void ShowHint(string HCPNFPMHFCM)
		{
			_hintRootGO.SetActive(true);
			_hintText.text = HCPNFPMHFCM;
			IAGHIGDNCGO = 1;
		}

		public void HideHint()
		{
			IAGHIGDNCGO = 0;
			_hintRootGO.SetActive(false);
		}

		public void Init()
		{
			Font font = LocalizationManager.MBPJIKFOEBJ();
			if (_hintText != null && font != null)
			{
				_hintText.font = font;
			}
			_levelHintButton.onClick.AddListener(() =>
			{
				OAEKFDFEKCL();
			});
			_barExp.Init();
			_barExp.SetValueBorders(0f, 1f);
			_barExp.SetValue(0f);
			HideHint();
			UpdateLevel();
		}

		private void Update()
		{
			if (IAGHIGDNCGO <= 0 && Input.anyKeyDown)
			{
				HideHint();
			}
			else
			{
				IAGHIGDNCGO--;
			}
		}

		private void CHILAIJNEHG()
		{
			_levelHintButton.onClick.RemoveListener(() =>
			{
				OAEKFDFEKCL();
			});
		}

		public void UpdateLevel()
		{
			string text = ListSF.CCDKHLAMKKO().PINDEKDNCNL().ToString();
			_labelLevel.text = text;
		}

		public void UpdateBarExp(float OBLEMIHLFII, float KAEPJHHLLPK)
		{
			if (OBLEMIHLFII != 0f && _barExp.GetValue() == OBLEMIHLFII)
			{
				return;
			}
			_barExp.SetValueBorders(0f, KAEPJHHLLPK);
			_barExp.SetValue(OBLEMIHLFII);
			int num = ListSF.CCDKHLAMKKO().PINDEKDNCNL();
			int count = GameUtils.HHONBOCJBLB.PEDIMBMABIG.Count;
			if (count != 0)
			{
				global::Pair<int, uint> cCKLNOPEKHO = GameUtils.HHONBOCJBLB.PEDIMBMABIG[count - 1];
				int lLHEDBIEHAA = cCKLNOPEKHO.First;
				if (num >= lLHEDBIEHAA)
				{
					_iconMaxLevel.gameObject.SetActive(true);
					_barExp.gameObject.SetActive(false);
					_levelHintButton.OFPNNIBBNCE(NFOGOFFAPPP.HHGPKAJENGF.PressInactive);
				}
				else
				{
					_iconMaxLevel.gameObject.SetActive(false);
					_barExp.gameObject.SetActive(true);
					_levelHintButton.OFPNNIBBNCE(NFOGOFFAPPP.HHGPKAJENGF.PressNormal);
				}
			}
		}

		private void OAEKFDFEKCL()
		{
			CallEvent(0, 0);
		}

		public void SetHintBtnPressType(NFOGOFFAPPP.HHGPKAJENGF LFLGCDNKNJI, bool GHJGPAEDIHG)
		{
			if (_iconMaxLevel.gameObject.activeSelf)
			{
				_levelHintButton.OFPNNIBBNCE(NFOGOFFAPPP.HHGPKAJENGF.PressInactive);
			}
			else
			{
				_levelHintButton.OFPNNIBBNCE(LFLGCDNKNJI, GHJGPAEDIHG);
			}
		}

		public virtual void SetTouchEnabled(bool value)
		{
			_levelHintButton.gameObject.SetActive(value);
		}
	}
}
