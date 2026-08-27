using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Fight
{
	public class PauseScreen : MonoBehaviour
	{
		public class OLFKCBNNBFI : UnityEvent
		{
		}

		[SerializeField]
		private PressButton musicOn;

		[SerializeField]
		private PressButton musicOff;

		[SerializeField]
		private PressButton soundOn;

		[SerializeField]
		private PressButton soundOff;

		public LabelAlias RulesLabel;

		public OLFKCBNNBFI OnPlay = new OLFKCBNNBFI();

		public OLFKCBNNBFI OnSurrender = new OLFKCBNNBFI();

		public void Init()
		{
			if (musicOn != null)
			{
				musicOn.gameObject.SetActive(!SoundController.ELHMADOKHHE());
			}
			if (musicOff != null)
			{
				musicOff.gameObject.SetActive(SoundController.ELHMADOKHHE());
			}
			if (soundOn != null)
			{
				soundOn.gameObject.SetActive(!SoundController.AAFLCDKJEPL());
			}
			if (soundOff != null)
			{
				soundOff.gameObject.SetActive(SoundController.AAFLCDKJEPL());
			}
			Sound.PMOECBEJGBL();
			PBFOLPNNGOJ();
			RebuildButtonsLayout();
		}

		private void RebuildButtonsLayout()
		{
			HorizontalLayoutGroup horizontalLayoutGroup = GetComponentInChildren<HorizontalLayoutGroup>(true);
			if (horizontalLayoutGroup != null)
			{
				Canvas.ForceUpdateCanvases();
				LayoutRebuilder.ForceRebuildLayoutImmediate(horizontalLayoutGroup.transform as RectTransform);
			}
		}

		private void PBFOLPNNGOJ()
		{
			string text = global::Fight.OHNKFOHIAKG().OGNINOBBHIG().GJOAJAIJHOE();
			bool flag = !string.IsNullOrEmpty(text);
			RulesLabel.gameObject.SetActive(flag);
			if (flag)
			{
				RulesLabel.SetAlias(text);
			}
		}

		public void OnHomeClick()
		{
			Sound.BPPCHJFPEHB();
			OnSurrender.Invoke();
		}

		public void OnPlayClick()
		{
			Sound.BPPCHJFPEHB();
			OnPlay.Invoke();
		}

		public void OnMusicOnClick()
		{
			if (musicOn != null)
			{
				musicOn.gameObject.SetActive(false);
			}
			if (musicOff != null)
			{
				musicOff.gameObject.SetActive(true);
			}
			SoundController.FMLHEDIPGAF(true);
			RebuildButtonsLayout();
		}

		public void OnMusicOffClick()
		{
			if (musicOn != null)
			{
				musicOn.gameObject.SetActive(true);
			}
			if (musicOff != null)
			{
				musicOff.gameObject.SetActive(false);
			}
			SoundController.FMLHEDIPGAF(false);
			RebuildButtonsLayout();
		}

		public void OnSoundOnClick()
		{
			if (soundOn != null)
			{
				soundOn.gameObject.SetActive(false);
			}
			if (soundOff != null)
			{
				soundOff.gameObject.SetActive(true);
			}
			SoundController.FLOFHMBDHNM(true);
			RebuildButtonsLayout();
		}

		public void OnSoundOffClick()
		{
			if (soundOn != null)
			{
				soundOn.gameObject.SetActive(true);
			}
			if (soundOff != null)
			{
				soundOff.gameObject.SetActive(false);
			}
			SoundController.FLOFHMBDHNM(false);
			RebuildButtonsLayout();
		}
	}
}
