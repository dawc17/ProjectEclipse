using System;
using Nekki.SF2.GUI.Scenes;
using Eclipse.UI.Settings;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Dialogs
{
	public class SettingsAdvancedDialog : SettingsDialog
	{
		private const int KNNGJJLIFHK = -620;

		private const int FCNBBIKDIFH = -620;

		private const int LMNCFKPNJPN = -620;

		private const int FPPFMHHIDJN = -620;

		private const int NOKIFJDNJGA = -620;

		private const int JIALLKOOGMF = 0;

		private const int EBHMMNCHFDP = 300;

		private const int DFDOFGAGIPE = 100;

		private const int HKLGPCMPAGK = 100;

		private const int LKGBADCONLK = -100;

		private const int EOFMGPCDJMI = -300;

		private const int KHCBFOBKIDF = 650;

		private const int KENGHLNFHBD = -680;

		private const int AFGCLAPADNK = 1077;

		private const int MMOPDOACJDJ = 1000;

		private const int JOCDMOJCLKD = -100;

		private const string AGHOMBNCFNG = "SettingsButtons.sound";

		private const string GFFCBAEJKGG = "SettingsButtons.sound_off";

		private const string IBLAMIJBCMI = "SettingsButtons.sound_selected";

		private const string HOIOIFJDFJL = "SettingsButtons.sound_off_selected";

		private const string DELBIFJECOK = "SettingsButtons.music";

		private const string NMGMEJPLCFC = "SettingsButtons.music_off";

		private const string KGKMLNJPDOO = "SettingsButtons.music_selected";

		private const string EOOAPLHPCEE = "SettingsButtons.music_off_selected";

		private const string BFELEFMPJEC = "SettingsButtons.graphics";

		private const string AGKCMELNNGG = "SettingsButtons.graphics_selected";

		private const string CLNPNHKMPJA = "SettingsButtons.location";

		private const string CHALFKPGEDG = "SettingsButtons.location_selected";

		private const string ECMLLFANLKJ = "SettingsButtons.controller";

		private const string CHMDLBIBIMD = "SettingsButtons.controller_selected";

		protected string BNEHHACJNGB;

		[SerializeField]
		private ResolutionButton btnGraphics;

		[SerializeField]
		private ResolutionButton btnController;

		[SerializeField]
		private ResolutionButton btnSoundAdv;

		[SerializeField]
		private ResolutionButton btnMusicAdv;

		[SerializeField]
		private Slider soundTrackBar;

		[SerializeField]
		private Slider musicTrackBar;

		[SerializeField]
		private LabelAlias lblGraphics;

		[SerializeField]
		private LabelAlias lblController;

		[SerializeField]
		private LabelAlias lblSoundAdv;

		[SerializeField]
		private LabelAlias lblMusicAdv;

		private DesktopRenderSettingsControls _desktopRenderSettings;

		public override void Init(object data)
		{
			BNEHHACJNGB = GraphicsController.PMAODLMLDLK();
			IsPausing = false;
			Init("Settings_Advanced_Title", "Settings_Advanced", "Settings_Back", KBDHPMOMJLL.FOOTER_CANCEL);
		}

		protected override void HLJBLAPMDCB()
		{
			BBDGOPHHDBJ();
			EBNPPFKKPLD();
			GetDesktopRenderSettings().Setup();
			AABKDFHHFOF();
			GPLBHPLJNAE();
			LMAAIDIDNEF();
			ALBPEOFFDKK();
		}

		protected override void FLOHKIBCOKG()
		{
			base.FLOHKIBCOKG();
			LOFKNKHJEDJ();
		}

		protected override void OnClickButton(object data)
		{
			AHDEAELNGBD buttonId = (AHDEAELNGBD)data;
			if (GetDesktopRenderSettings().HandleClick(buttonId))
			{
				return;
			}
			switch (buttonId)
			{
			case AHDEAELNGBD.BTN_SOUND_ADV:
				SoundController.FLOFHMBDHNM(!SoundController.AAFLCDKJEPL());
				if (SoundController.AAFLCDKJEPL())
				{
					soundTrackBar.value = 0f;
				}
				else
				{
					soundTrackBar.value = SoundController.LOLBPMLPBGL();
				}
				AOFFEDGGNMN();
				break;
			case AHDEAELNGBD.BTN_MUSIC_ADV:
				SoundController.FMLHEDIPGAF(!SoundController.ELHMADOKHHE());
				if (SoundController.ELHMADOKHHE())
				{
					musicTrackBar.value = 0f;
				}
				else
				{
					musicTrackBar.value = SoundController.FGFHCAAFODL();
				}
				IHPJIBKOPDL();
				break;
			case AHDEAELNGBD.BTN_CONTROLLER:
				GraphicsController.JLDMJOEGJLF();
				AABJCHNIDJP();
				DEMCCCLKNEM();
				break;
			case AHDEAELNGBD.BTN_GRAPHICS:
				if (GraphicsController.AFLFDJKLIEE())
				{
					NPMFPLFMFMI();
				}
				break;
			case AHDEAELNGBD.BTN_LOCATION_RESOLUTION:
				GraphicsController.FELIOKHNIKI();
				GDPCOKJGAJO();
				break;
			}
		}

		public override void OnClose(object data)
		{
			if (BNEHHACJNGB != GraphicsController.PMAODLMLDLK())
			{
				DFLOLCIKPEM();
			}
			else
			{
				base.OnClose(data);
			}
		}

		private void BGPPLMPDNKF()
		{
			CallEvent(2, null);
			base.OnClose((object)IPJEOLNMLEH.OnPopupCloseCascade);
		}

		private void Update()
		{
		}

		protected override void BBDGOPHHDBJ()
		{
			OHDFPIADEIG(btnGraphics, "SettingsButtons.graphics", "SettingsButtons.graphics_selected", -620f, 300f, AHDEAELNGBD.BTN_GRAPHICS);
			OHDFPIADEIG(btnController, "SettingsButtons.controller", "SettingsButtons.controller_selected", -620f, 100f, AHDEAELNGBD.BTN_CONTROLLER);
			IHPJIBKOPDL();
			AOFFEDGGNMN();
		}

		protected override void EBNPPFKKPLD()
		{
			NPMFPLFMFMI();
			GDPCOKJGAJO();
			AABJCHNIDJP();
			PGMBIJFAEHP(lblMusicAdv, "Settings_Music");
			PGMBIJFAEHP(lblSoundAdv, "Settings_Sound");
		}

		protected void AABKDFHHFOF()
		{
			PGOGAGFHNFK(soundTrackBar, SoundController.LOLBPMLPBGL(), new Vector2(50f, btnSoundAdv.transform.localPosition.y), OEBEMLDGNDB);
			PGOGAGFHNFK(musicTrackBar, SoundController.FGFHCAAFODL(), new Vector2(50f, btnMusicAdv.transform.localPosition.y), IMJLOABEMOL);
		}

		protected override void LOFKNKHJEDJ()
		{
			float num = 650f;
			float num2 = ((!lblUserId.gameObject.activeSelf) ? 0f : 37f);
			_topStripe.transform.BGNJGIACJBG(num + num2);
			_topStripe.rectTransform.sizeDelta = new Vector2(1077f, _topStripe.rectTransform.rect.height);
			num = -680f;
			_bottomStripe.transform.BGNJGIACJBG(num - num2);
			_bottomStripe.rectTransform.sizeDelta = new Vector2(1000f, _bottomStripe.rectTransform.rect.height);
		}

		protected override void GPLBHPLJNAE()
		{
			base.GPLBHPLJNAE();
			float y = lblUserId.transform.localPosition.y;
			y += -100f;
			lblUserId.transform.BGNJGIACJBG(y);
			lblUserId.gameObject.SetActive(false);
		}

		protected override void LMAAIDIDNEF()
		{
			float num = 0f;
			num += ((!lblUserId.gameObject.activeSelf) ? 0f : 37f);
			_content.transform.BGNJGIACJBG(num);
		}

		protected void OEBEMLDGNDB(float JIJAJFEJJHK)
		{
			bool flag = SoundController.AAFLCDKJEPL();
			SoundController.EDPABAPLCGN(JIJAJFEJJHK);
			if (flag != SoundController.AAFLCDKJEPL())
			{
				AOFFEDGGNMN();
			}
		}

		protected void IMJLOABEMOL(float JIJAJFEJJHK)
		{
			bool flag = SoundController.ELHMADOKHHE();
			SoundController.IDLBNOCKEBK(JIJAJFEJJHK);
			if (flag != SoundController.ELHMADOKHHE())
			{
				IHPJIBKOPDL();
			}
		}

		protected void BOEJIOIJLJO(object data)
		{
			if (data != null && ((IPJEOLNMLEH)Enum.Parse(typeof(IPJEOLNMLEH), Convert.ToString(data))/*cast due to constrained. prefix*/).Equals(IPJEOLNMLEH.OnPopupCloseOK))
			{
				BGPPLMPDNKF();
				GameUtils.BKFMHANNIEF();
			}
		}

			protected void AOFFEDGGNMN()
			{
				bool flag = SoundController.AAFLCDKJEPL();
				OHDFPIADEIG(btnSoundAdv, (!flag) ? "SettingsButtons.sound" : "SettingsButtons.sound_off", (!flag) ? "SettingsButtons.sound_selected" : "SettingsButtons.sound_off_selected", -620f, -375f, AHDEAELNGBD.BTN_SOUND_ADV);
			}

			protected void IHPJIBKOPDL()
			{
				bool flag = SoundController.ELHMADOKHHE();
				OHDFPIADEIG(btnMusicAdv, (!flag) ? "SettingsButtons.music" : "SettingsButtons.music_off", (!flag) ? "SettingsButtons.music_selected" : "SettingsButtons.music_off_selected", -620f, -235f, AHDEAELNGBD.BTN_MUSIC_ADV);
			}

		protected void AABJCHNIDJP()
		{
			PGMBIJFAEHP(lblController, string.Empty);
			string text = LocalizationManager.GetString("Settings_Controller_Scale") + LocalizationManager.GetString(HHBPNDNGFAM());
			lblController.set_text(text);
		}

		protected void NPMFPLFMFMI()
		{
			PGMBIJFAEHP(lblGraphics, string.Empty);
			lblGraphics.set_text(LocalizationManager.GetString("Settings_Graphics_Quality") + LocalizationManager.GetString(BPBKNHEDMKI()));
		}

		protected void GDPCOKJGAJO()
		{
		}

		protected void ALBPEOFFDKK()
		{
			float jMLAKAKDBBL = 1500f;
			ChangeButtonTouchZone(btnGraphics, jMLAKAKDBBL);
			ChangeButtonTouchZone(GetDesktopRenderSettings().FrameRateButton, jMLAKAKDBBL);
			ChangeButtonTouchZone(GetDesktopRenderSettings().MotionBlurButton, jMLAKAKDBBL);
			ChangeButtonTouchZone(btnController, jMLAKAKDBBL);
		}

		private DesktopRenderSettingsControls GetDesktopRenderSettings()
		{
			if (_desktopRenderSettings == null)
			{
				_desktopRenderSettings = new DesktopRenderSettingsControls(
					(_content == null) ? null : _content.transform,
					btnGraphics, lblGraphics, btnController, btnMusicAdv, btnSoundAdv,
					OHDFPIADEIG, IHPJIBKOPDL, AOFFEDGGNMN);
			}
			return _desktopRenderSettings;
		}

		protected void DFLOLCIKPEM()
		{
			DialogsOpener.PEDJMOMBJJI("dlgAlertTitle", "dlgSettingsRestart", "dlgServiceRestart", "dlgServiceBtnLater", BOEJIOIJLJO, LabelButton.FBMGEHJPPIK.BUTTON_WHITE, LabelButton.FBMGEHJPPIK.BUTTON_DARK, false, false, string.Empty);
		}

		protected string HHBPNDNGFAM()
		{
			return (!GraphicsController.OPEHHMBJABL()) ? "Settings_Controller_Small" : "Settings_Controller_Large";
		}

		protected string BPBKNHEDMKI()
		{
			string gOHIIMFFFJI = GraphicsController.PMAODLMLDLK();
			QualityOption.HPNJCDGIHLI hPNJCDGIHLI = QualityOption.ONPFEBDGLFO(gOHIIMFFFJI);
			string empty = string.Empty;
			switch (hPNJCDGIHLI)
			{
			case QualityOption.HPNJCDGIHLI.QUALITY_LOW:
				return "Settings_Graphics_Low";
			case QualityOption.HPNJCDGIHLI.QUALITY_MEDIUM:
				return "Settings_Graphics_Medium";
			case QualityOption.HPNJCDGIHLI.QUALITY_HIGH:
				return "Settings_Graphics_High";
			default:
				return string.Empty;
			}
		}

		protected string MOIIMMLICLB()
		{
			string empty = string.Empty;
			switch (GraphicsController.GHLDNALLEKN())
			{
			case SystemProperties.LOHALAKNGFB.PATH_SMALL:
				return "Settings_Graphics_Low";
			case SystemProperties.LOHALAKNGFB.PATH_BIG:
				return "Settings_Graphics_High";
			default:
				return string.Empty;
			}
		}

		protected void PGOGAGFHNFK(Slider KFKCPEALPDL, float value, Vector2 MGMMDGFPBLP, UnityAction<float> ODDEOFKLIAG)
		{
			KFKCPEALPDL.gameObject.SetActive(true);
			KFKCPEALPDL.onValueChanged.AddListener(ODDEOFKLIAG);
			KFKCPEALPDL.minValue = 0f;
			KFKCPEALPDL.maxValue = 1f;
			KFKCPEALPDL.value = value;
			KFKCPEALPDL.transform.OKHPLHPBPKJ(MGMMDGFPBLP.x);
			KFKCPEALPDL.transform.BGNJGIACJBG(MGMMDGFPBLP.y);
		}

		protected void ChangeButtonTouchZone(Button GAMILDJHFDB, LabelAlias NCJDCOLEFHG)
		{
		}

		protected void ChangeButtonTouchZone(Button GAMILDJHFDB, float JMLAKAKDBBL)
		{
		}

		protected void DEMCCCLKNEM()
		{
			DojoScene current = Scene<DojoScene>.get_Current();
			if (current != null)
			{
				current.fight.BPFBPOCPPCB();
			}
		}
	}
}
