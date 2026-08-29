using System;
using System.Xml;
using UnityEngine;

namespace Nekki.SF2.GUI.Dialogs
{
	public class SettingsDialog : BaseDialog
	{
		public enum AHDEAELNGBD
		{
			BTN_MUSIC = 0,
			BTN_SOUND = 1,
			BTN_CREDITS = 2,
			BTN_FACEBOOK = 3,
			BTN_LANGUAGE = 4,
			BTN_GAMECENTER = 5,
			BTN_SUPPORT = 6,
			BTN_ITUNES = 7,
			BTN_RESOLUTION_HIGHER = 8,
			BTN_RESOLUTION_LOWER = 9,
				BTN_RESOLUTION_SET = 10,
				BTN_GRAPHICS = 11,
				BTN_LOCATION_RESOLUTION = 12,
				BTN_CONTROLLER = 13,
				BTN_MUSIC_ADV = 14,
				BTN_SOUND_ADV = 15,
				BTN_RENDER_INTERPOLATION = 16,
				BTN_MAX_FRAME_RATE = 17,
				BTN_MOTION_BLUR = 18
			}

		protected const int MNAMPADEPHJ = 1540;

		public const float USER_ID_Y = -516f;

		public const float USER_ID_Y_ANDROID = -356f;

		protected const float GLBKJGELJEG = 37f;

		public const float STRIPES_TOP_OFFSET_ANDROID = 500f;

		public const float STRIPES_BOTTOM_OFFSET_ANDROID = -540f;

		private const float KHCBFOBKIDF = 600f;

		private const float KENGHLNFHBD = -600f;

		private const float AFGCLAPADNK = 1078f;

		private const float MMOPDOACJDJ = 1000.99994f;

		private const int JIALLKOOGMF = 100;

		private const int IHMICEDLEPB = 110;

		private const int DPJCJMNHJPA = -70;

		private const int JFKACFHPFMB = 60;

		private const int JEDCCAHEIFM = 101;

		private const int NOKIFJDNJGA = -670;

		private const int FPPFMHHIDJN = -670;

		private const int EELEELFNIIK = -670;

		private const int DKOIOOJCOHM = -670;

		private const int LLLHICKGIKG = 0;

		private const int BGLFDFFOLLI = 0;

		private const int JFJMBBCLCNI = 0;

		private const int MNFEFBMKENF = 0;

		private const int GAEOEHAEMBL = 0;

		private const int LKGBADCONLK = 200;

		private const int EOFMGPCDJMI = 0;

		private const int FKEOINAIDBA = -200;

		private const int EJONAFGHFEK = -400;

		private const int GNILBLHKHAA = 200;

		private const int DJCCEAPNAPM = 0;

		private const int CBDOPCAHDFI = -196;

		private const int NOBMHPEHNDF = -400;

		private const int KHBDPMPNOMC = -620;

		private const int OLALCEIBOIA = 50;

		private const int IDMFLHKKOJO = 50;

		private const int IHCCJENKGDD = -200;

		private const int EPDLFHKCNIC = 200;

		private const int OBJOIFGACBE = 0;

		private const int GNLDHNMLEHE = -200;

		private const int LHOLJEOAHIG = -400;

		private const int OKPAHKAEGDA = 0;

		private const int BOHPHDEAMLH = -300;

		private const string DELBIFJECOK = "SettingsButtons.music";

		private const string NMGMEJPLCFC = "SettingsButtons.music_off";

		private const string KGKMLNJPDOO = "SettingsButtons.music_selected";

		private const string EOOAPLHPCEE = "SettingsButtons.music_off_selected";

		private const string AGHOMBNCFNG = "SettingsButtons.sound";

		private const string GFFCBAEJKGG = "SettingsButtons.sound_off";

		private const string IBLAMIJBCMI = "SettingsButtons.sound_selected";

		private const string HOIOIFJDFJL = "SettingsButtons.sound_off_selected";

		private const string ODIEKCBNCIN = "SettingsButtons.credits";

		private const string AHCLDBMPFAI = "SettingsButtons.credits_selected";

		private const string FPOPMMGBLDP = "SettingsButtons.facebook";

		private const string PINCADCEMMG = "SettingsButtons.facebook_off";

		private const string FPODLKIHKOG = "SettingsButtons.facebook_selected";

		private const string LIHGANIGBGH = "SettingsButtons.facebook_off_selected";

		private const string INLKAIDLAEA = "SettingsButtons.facebook_disable";

		private const string EKJCLAIAJHK = "SettingsButtons.gamecenter";

		private const string BJFGLANPDKE = "SettingsButtons.gamecenter_off";

		private const string GBGBCIEINHK = "SettingsButtons.gamecenter_disable";

		private const string HCLHADBIKLL = "SettingsButtons.googleplay_controller";

		private const string HPPLJGCCPDA = "SettingsButtons.googleplay_off_controller";

		private const string LBMIGCLAGCN = "SettingsButtons.googleplay_disable_controller";

		private const string INEKJKDPAMF = "SettingsButtons.windows";

		private const string HNLJFNHOLLD = "SettingsButtons.windows_off";

		private const string JMIPBDFDDAF = "SettingsButtons.support";

		private const string JAFAFABKPJF = "SettingsButtons.support_selected";

		private const string BOCGDODMONJ = "SettingsButtons.itunes";

		private const string LEDIAMNEJFN = "SettingsButtons.googleplay_music";

		private const string OCLOOGLKILP = "SettingsButtons.amazon_mp3";

		[SerializeField]
		private ResolutionButton btnMusic;

		[SerializeField]
		private ResolutionButton btnSound;

		[SerializeField]
		private ResolutionButton btnCredits;

		[SerializeField]
		private ResolutionButton btnSupport;

		[SerializeField]
		private ResolutionButton btnLanguage;

		[SerializeField]
		private ResolutionButton btnGameCenter;

		[SerializeField]
		private ResolutionButton btnItunes;

		[SerializeField]
		private Sprite resolutionChanger;

		[SerializeField]
		private LabelAlias lblMusic;

		[SerializeField]
		private LabelAlias lblSound;

		[SerializeField]
		private LabelAlias lblCredits;

		[SerializeField]
		private LabelAlias lblSupport;

		[SerializeField]
		private LabelAlias lblLanguage;

		[SerializeField]
		private LabelAlias lblGameCenter;

		[SerializeField]
		private LabelAlias lblItunes;

		[SerializeField]
		protected LabelAlias lblUserId;

		[SerializeField]
		private LabelAlias lblResolution;

		[SerializeField]
		private LabelAlias lblResolutionValue;

		protected bool KMIJDCPHHML;

		protected int EDHGAEOJKGO = -1;

		protected bool EIJHFKINNPF;

		protected int BIBDCOKMMKO;

		protected LocalizationManager.Language LFFLJJGJHIB;

		private BaseDialog EBBGCLGJBBC;

		public override void Init(object data)
		{
			KMIJDCPHHML = GameCenterController.OBDJPKOJADA();
			if (AssemblyController.JONCCPLEIBE().NPNOMBEEPJD() || AssemblyController.JONCCPLEIBE().BKGIFIPIHAL() || AssemblyController.JONCCPLEIBE().COPJOJAMBKA())
			{
				base.Init("Settings_Title", "Settings_Advanced", "Settings_Back", KBDHPMOMJLL.FOOTER_CANCEL);
			}
			else
			{
				base.Init("Settings_Title", "Settings_Advanced", "Settings_Back", KBDHPMOMJLL.FOOTER_BOTH);
			}
			GameCenterAbstract.OnAuthenticate = (Action<bool>)Delegate.Combine(GameCenterAbstract.OnAuthenticate, new Action<bool>(OnAuthenticate));
			// Credits/support were mobile storefront/service entries in this build.
			// Their labels are absent in the migrated desktop localization, so do
			// not expose two anonymous buttons.
			if (btnCredits != null) btnCredits.gameObject.SetActive(false);
			if (lblCredits != null) lblCredits.gameObject.SetActive(false);
			if (btnSupport != null) btnSupport.gameObject.SetActive(false);
			if (lblSupport != null) lblSupport.gameObject.SetActive(false);
			if (btnGameCenter != null) btnGameCenter.gameObject.SetActive(false);
			if (lblGameCenter != null) lblGameCenter.gameObject.SetActive(false);
			if (btnItunes != null) btnItunes.gameObject.SetActive(false);
			if (lblItunes != null) lblItunes.gameObject.SetActive(false);
		}

		private void OnDestroy()
		{
			GameCenterAbstract.OnAuthenticate = (Action<bool>)Delegate.Remove(GameCenterAbstract.OnAuthenticate, new Action<bool>(OnAuthenticate));
		}

		public virtual void UpdateFacebookBtnImg()
		{
			if (!AssemblyController.JONCCPLEIBE().BKGIFIPIHAL() && !AssemblyController.JONCCPLEIBE().NPNOMBEEPJD() && AssemblyController.JONCCPLEIBE().DBJOHGNPDDO())
			{
			}
		}

		public void UpdateLabels()
		{
			PGMBIJFAEHP(lblMusic, "Settings_Music");
			PGMBIJFAEHP(lblSound, "Settings_Sound");
			PGMBIJFAEHP(lblCredits, "Settings_Credits");
			PGMBIJFAEHP(lblLanguage, LocalizationManager.ILAJKOBCHFH.LOKLDPLAPOL);
			if (AssemblyController.JONCCPLEIBE().NPNOMBEEPJD())
			{
				KEGMJAHJODA();
			}
			else if (AssemblyController.JONCCPLEIBE().OPCBKOOFMAK())
			{
				KLAMABIPJDM();
			}
			else if (AssemblyController.JONCCPLEIBE().BKGIFIPIHAL())
			{
				PDIPGANMBOM();
			}
			else if (AssemblyController.JONCCPLEIBE().DBJOHGNPDDO())
			{
				DLIENPPKMCI();
			}
			else
			{
				EBNPPFKKPLD();
			}
		}

		public override void Close(object data)
		{
			OnClose(IPJEOLNMLEH.OnPopupClose);
		}

		public override void OnClose(object data)
		{
			if (EIJHFKINNPF)
			{
				ILLEDELEEPC();
			}
			if (EBBGCLGJBBC != null)
			{
				EBBGCLGJBBC.RemoveAllEventListener();
				EBBGCLGJBBC = null;
			}
			base.OnClose(data);
		}

		protected override void HLJBLAPMDCB()
		{
			bool flag = SoundController.ELHMADOKHHE();
			OHDFPIADEIG(btnMusic, (!flag) ? "SettingsButtons.music" : "SettingsButtons.music_off", (!flag) ? "SettingsButtons.music_selected" : "SettingsButtons.music_off_selected", -670f, 200f, AHDEAELNGBD.BTN_MUSIC);
			bool flag2 = SoundController.AAFLCDKJEPL();
			OHDFPIADEIG(btnSound, (!flag2) ? "SettingsButtons.sound" : "SettingsButtons.sound_off", (!flag2) ? "SettingsButtons.sound_selected" : "SettingsButtons.sound_off_selected", -670f, 0f, AHDEAELNGBD.BTN_SOUND);
			OHDFPIADEIG(btnCredits, "SettingsButtons.credits", "SettingsButtons.credits_selected", -670f, -200f, AHDEAELNGBD.BTN_CREDITS);
			string mMBELNEBNBM = LocalizationManager.ILAJKOBCHFH.MMBELNEBNBM;
			string oKGJAMBPDGO = LocalizationManager.ILAJKOBCHFH.OKGJAMBPDGO;
			oKGJAMBPDGO = ((!(oKGJAMBPDGO == string.Empty)) ? oKGJAMBPDGO : mMBELNEBNBM);
			OHDFPIADEIG(btnLanguage, mMBELNEBNBM, oKGJAMBPDGO, 0f, 200f, AHDEAELNGBD.BTN_LANGUAGE);
			PGMBIJFAEHP(lblLanguage, LocalizationManager.ILAJKOBCHFH.LOKLDPLAPOL);
			if (AssemblyController.JONCCPLEIBE().NPNOMBEEPJD())
			{
				btnMusic.transform.BGNJGIACJBG(0f);
				btnSound.transform.BGNJGIACJBG(-200f);
				if (btnCredits != null) btnCredits.transform.OKHPLHPBPKJ(0f);
				btnLanguage.transform.BGNJGIACJBG(0f);
			}
			else if (AssemblyController.JONCCPLEIBE().OPCBKOOFMAK())
			{
				float bAINMLLIKOL = ((!AssemblyController.JONCCPLEIBE().BKGIFIPIHAL()) ? 200 : 200);
				btnLanguage.transform.BGNJGIACJBG(bAINMLLIKOL);
				DJAMBJAJOJB();
			}
			else if (AssemblyController.JONCCPLEIBE().BKGIFIPIHAL())
			{
				float bAINMLLIKOL2 = ((!AssemblyController.JONCCPLEIBE().BKGIFIPIHAL()) ? 200 : 200);
				btnLanguage.transform.BGNJGIACJBG(bAINMLLIKOL2);
				ANAMMJCIDBD();
			}
			else if (AssemblyController.JONCCPLEIBE().DBJOHGNPDDO())
			{
				OCFFGHFCJBM();
			}
			else if (SystemProperties.DDIDANINPJE())
			{
				if (btnCredits != null) btnCredits.transform.OKHPLHPBPKJ(-620f);
				NHCFEKKGMAE();
			}
			else
			{
				float bAINMLLIKOL3 = ((!AssemblyController.JONCCPLEIBE().BKGIFIPIHAL()) ? 200 : 200);
				btnLanguage.transform.BGNJGIACJBG(bAINMLLIKOL3);
				BBDGOPHHDBJ();
			}
			UpdateLabels();
			LMAAIDIDNEF();
		}

		protected override void FLOHKIBCOKG()
		{
			base.FLOHKIBCOKG();
			_topStripe.rectTransform.sizeDelta = new Vector2(1078f, _topStripe.rectTransform.rect.height);
			_bottomStripe.rectTransform.sizeDelta = new Vector2(1000.99994f, _bottomStripe.rectTransform.rect.height);
			if (AssemblyController.JONCCPLEIBE().NPNOMBEEPJD())
			{
				LDIIKCNPLKB();
			}
			else if (AssemblyController.JONCCPLEIBE().OPCBKOOFMAK())
			{
				OALBAOJIKMC();
			}
			else if (AssemblyController.JONCCPLEIBE().BKGIFIPIHAL())
			{
				OPFAOIMPGAD();
			}
			else
			{
				LOFKNKHJEDJ();
			}
		}

		protected override void MAGOIKICKAH(KBDHPMOMJLL HJNAHNICGMH)
		{
			base.MAGOIKICKAH(HJNAHNICGMH);
			float num = _bottomStripe.transform.localPosition.y - -70f;
			if (_btnOK.gameObject.activeSelf)
			{
				_btnOK.transform.BGNJGIACJBG(num + _btnOK.get_rect().height / 2f);
				_btnOK.RemoveEvent(2);
				_btnOK.AddEventListener(2, MFIGCNECAAN);
			}
			if (_btnCancel.gameObject.activeSelf)
			{
				_btnCancel.transform.BGNJGIACJBG(num + _btnCancel.get_rect().height / 2f);
				_btnCancel.RemoveEventListener(2, OnClose);
				_btnCancel.AddEventListener(2, OnClose);
			}
		}

		protected override void SetupHeader(string HCPNFPMHFCM)
		{
			base.SetupHeader(HCPNFPMHFCM);
			float bAINMLLIKOL = _topStripe.transform.localPosition.y - 110f;
			_header.transform.BGNJGIACJBG(bAINMLLIKOL);
		}

		private void Update()
		{
			if (EIJHFKINNPF)
			{
				BIBDCOKMMKO++;
				if (BIBDCOKMMKO >= 60)
				{
					ILLEDELEEPC();
					EIJHFKINNPF = false;
					BIBDCOKMMKO = 0;
				}
			}
			bool flag = GameCenterController.OBDJPKOJADA();
			if (KMIJDCPHHML != flag)
			{
				GOCIHHMADOC();
				KMIJDCPHHML = flag;
			}
		}

		protected virtual void GPLBHPLJNAE()
		{
			lblUserId.set_Alias(string.Empty);
			lblUserId.set_text(string.Empty);
			lblUserId.set_LabelFontSize(101);
			float bAINMLLIKOL = ((!AssemblyController.JONCCPLEIBE().BKGIFIPIHAL()) ? (-516f) : (-356f));
			lblUserId.transform.BGNJGIACJBG(bAINMLLIKOL);
			lblUserId.color = Constants.PJJIMHMJPAL;
			string text = ListSF.CCDKHLAMKKO().KNGJJEOLFHF();
			lblUserId.set_text(LocalizationManager.GetString("Settings_UserID") + ": " + text);
			lblUserId.gameObject.SetActive(text != string.Empty);
		}

		protected virtual void LMAAIDIDNEF()
		{
			if (AssemblyController.JONCCPLEIBE().NPNOMBEEPJD())
			{
				_content.transform.BGNJGIACJBG(100f);
			}
			else if (AssemblyController.JONCCPLEIBE().OPCBKOOFMAK())
			{
				float num = 100f;
				num += ((!lblUserId.gameObject.activeSelf) ? 0f : 37f);
				_content.transform.BGNJGIACJBG(num);
			}
			else if (AssemblyController.JONCCPLEIBE().BKGIFIPIHAL())
			{
				float num2 = ((!AssemblyController.JONCCPLEIBE().BKGIFIPIHAL()) ? 100 : 0);
				num2 += ((!lblUserId.gameObject.activeSelf) ? 0f : 37f);
				_content.transform.BGNJGIACJBG(num2);
			}
			else if (AssemblyController.JONCCPLEIBE().DBJOHGNPDDO())
			{
				float num3 = 100f;
				num3 += ((!lblUserId.gameObject.activeSelf) ? 0f : 37f);
				_content.transform.BGNJGIACJBG(num3);
			}
			else
			{
				float num4 = ((!AssemblyController.JONCCPLEIBE().BKGIFIPIHAL()) ? 100 : 0);
				num4 += ((!lblUserId.gameObject.activeSelf) ? 0f : 37f);
				_content.transform.BGNJGIACJBG(num4);
			}
		}

		protected void OHDFPIADEIG(ResolutionButton GAMILDJHFDB, string CGNJEDIFEKJ, string AOFLEGLGGAC, float DHDMNHCIPEH, float BGEEALIPKCC, AHDEAELNGBD OKNNNLIPODI)
		{
			// Desktop prefabs can omit the legacy credits/support controls.
			if (GAMILDJHFDB == null)
			{
				return;
			}
			if (OKNNNLIPODI == AHDEAELNGBD.BTN_GAMECENTER || OKNNNLIPODI == AHDEAELNGBD.BTN_ITUNES)
			{
				if (GAMILDJHFDB != null) GAMILDJHFDB.gameObject.SetActive(false);
				return;
			}
			GAMILDJHFDB.SetNormalSprite("UI/Atlases/", CGNJEDIFEKJ);
			GAMILDJHFDB.SetPressedSprite("UI/Atlases/", (!PEPADDIALAO()) ? CGNJEDIFEKJ : AOFLEGLGGAC);
			GAMILDJHFDB.ButtonId = (int)OKNNNLIPODI;
			GAMILDJHFDB.RemoveEventListener(2, OnClickButton);
			GAMILDJHFDB.AddEventListener(2, OnClickButton);
			GAMILDJHFDB.transform.localPosition = new Vector2(DHDMNHCIPEH, BGEEALIPKCC);
			GAMILDJHFDB.gameObject.SetActive(true);
		}

		protected void PGMBIJFAEHP(LabelAlias NCJDCOLEFHG, string LOKLDPLAPOL)
		{
			if (NCJDCOLEFHG == null)
			{
				return;
			}
			if (NCJDCOLEFHG == lblGameCenter || NCJDCOLEFHG == lblItunes)
			{
				if (NCJDCOLEFHG != null) NCJDCOLEFHG.gameObject.SetActive(false);
				return;
			}
			NCJDCOLEFHG.gameObject.SetActive(true);
			NCJDCOLEFHG.set_Alias(LOKLDPLAPOL);
			NCJDCOLEFHG.alignment = TextAnchor.MiddleLeft;
			NCJDCOLEFHG.set_LabelFontSize(101);
			NCJDCOLEFHG.color = Constants.PJJIMHMJPAL;
		}

		protected void HJLOKEPOCHN()
		{
		}

		protected virtual void BBDGOPHHDBJ()
		{
			if (!SystemProperties.PPFPHAKMNLC() && !SystemProperties.CEJMCBKCPOH() && !SystemProperties.LHGPKEFEHDH())
			{
				string empty = string.Empty;
				empty = ((!SystemProperties.IPJFCBAGMJJ()) ? ((!SFSocial.GBPBIPFIOJH().CMOOANCABOG()) ? "SettingsButtons.gamecenter_off" : "SettingsButtons.gamecenter") : ((!SFSocial.GBPBIPFIOJH().CMOOANCABOG()) ? "SettingsButtons.googleplay_off_controller" : "SettingsButtons.googleplay_controller"));
				OHDFPIADEIG(btnGameCenter, empty, empty, 0f, 0f, AHDEAELNGBD.BTN_GAMECENTER);
				if (SystemProperties.IPJFCBAGMJJ())
				{
					btnGameCenter.SetDisabledSprite("UI/Atlases/", "SettingsButtons.googleplay_disable_controller");
				}
				else
				{
					btnGameCenter.SetDisabledSprite("UI/Atlases/", "SettingsButtons.gamecenter_disable");
				}
			}
			OHDFPIADEIG(btnSupport, "SettingsButtons.support", "SettingsButtons.support", -670f, -400f, AHDEAELNGBD.BTN_SUPPORT);
			if (SystemProperties.MEBGOGMJFLM())
			{
				OHDFPIADEIG(btnItunes, "SettingsButtons.itunes", "SettingsButtons.itunes", 0f, -196f, AHDEAELNGBD.BTN_ITUNES);
			}
			else if (SystemProperties.IPJFCBAGMJJ())
			{
				OHDFPIADEIG(btnItunes, "SettingsButtons.googleplay_music", "SettingsButtons.googleplay_music", 0f, -196f, AHDEAELNGBD.BTN_ITUNES);
			}
		}

		private void ANAMMJCIDBD()
		{
			float dHDMNHCIPEH = ((!AssemblyController.JONCCPLEIBE().BKGIFIPIHAL()) ? (-670) : 50);
			float bGEEALIPKCC = 0f;
			OHDFPIADEIG(btnSupport, "SettingsButtons.support", "SettingsButtons.support", dHDMNHCIPEH, bGEEALIPKCC, AHDEAELNGBD.BTN_SUPPORT);
		}

		private void DJAMBJAJOJB()
		{
			float dHDMNHCIPEH = 50f;
			float bGEEALIPKCC = 0f;
			OHDFPIADEIG(btnSupport, "SettingsButtons.support", "SettingsButtons.support", dHDMNHCIPEH, bGEEALIPKCC, AHDEAELNGBD.BTN_SUPPORT);
			OHDFPIADEIG(btnItunes, "SettingsButtons.amazon_mp3", "SettingsButtons.amazon_mp3", -670f, -196f, AHDEAELNGBD.BTN_ITUNES);
		}

		private void OCFFGHFCJBM()
		{
			OHDFPIADEIG(btnSupport, "SettingsButtons.support", "SettingsButtons.support", -670f, -400f, AHDEAELNGBD.BTN_SUPPORT);
			HJLOKEPOCHN();
		}

		private void NHCFEKKGMAE()
		{
			if (SystemProperties.DDIDANINPJE())
			{
				OHDFPIADEIG(btnGameCenter, (!GameCenterController.OBDJPKOJADA()) ? "SettingsButtons.windows_off" : "SettingsButtons.windows", (!GameCenterController.OBDJPKOJADA()) ? "SettingsButtons.windows_off" : "SettingsButtons.windows", 0f, 0f, AHDEAELNGBD.BTN_GAMECENTER);
			}
			float num = 0f;
			float num2 = 0f;
			if (SystemProperties.DDIDANINPJE())
			{
				num = -200f;
				num2 = -400f;
			}
			else
			{
				num = 50f;
				num2 = -200f;
			}
			OHDFPIADEIG(btnSupport, "SettingsButtons.support", "SettingsButtons.support_selected", num, num2, AHDEAELNGBD.BTN_SUPPORT);
		}

		protected virtual void EBNPPFKKPLD()
		{
			PGMBIJFAEHP(lblSupport, "Settings_Support");
			if (!AssemblyController.JONCCPLEIBE().BKGIFIPIHAL() && !AssemblyController.JONCCPLEIBE().DBJOHGNPDDO())
			{
				if (SystemProperties.IPJFCBAGMJJ())
				{
					PGMBIJFAEHP(lblGameCenter, "Settings_GooglePlus");
				}
				else if (SystemProperties.MEBGOGMJFLM())
				{
					PGMBIJFAEHP(lblGameCenter, "Settings_GameCenter");
				}
				else if (SystemProperties.AFKGHBJPLOK() && SystemProperties.DDIDANINPJE())
				{
					PGMBIJFAEHP(lblGameCenter, "Settings_live_id");
				}
			}
			if (btnItunes.gameObject.activeSelf)
			{
				PGMBIJFAEHP(lblItunes, "Settings_Soundtack");
			}
			GPLBHPLJNAE();
		}

		protected void PDIPGANMBOM()
		{
			PGMBIJFAEHP(lblSupport, "Settings_Support");
			if (!AssemblyController.JONCCPLEIBE().BKGIFIPIHAL())
			{
				if (SystemProperties.IPJFCBAGMJJ())
				{
					PGMBIJFAEHP(lblGameCenter, "Settings_GooglePlus");
				}
				else
				{
					PGMBIJFAEHP(lblGameCenter, "Settings_GameCenter");
				}
			}
			GPLBHPLJNAE();
		}

		protected void KEGMJAHJODA()
		{
			GPLBHPLJNAE();
			lblUserId.gameObject.SetActive(false);
		}

		protected void KLAMABIPJDM()
		{
			PGMBIJFAEHP(lblSupport, "Settings_Support");
			PGMBIJFAEHP(lblItunes, "Settings_Soundtack");
			GPLBHPLJNAE();
			if (lblUserId.gameObject.activeSelf)
			{
				lblUserId.transform.BGNJGIACJBG(-516f);
			}
		}

		protected void DLIENPPKMCI()
		{
			PGMBIJFAEHP(lblSupport, "Settings_Support");
			lblResolution.gameObject.SetActive(true);
			lblResolution.set_Alias("Settings_Resolution");
			lblResolution.alignment = TextAnchor.MiddleLeft;
			lblResolution.set_LabelFontSize(101);
			lblResolution.transform.OKHPLHPBPKJ(0f);
			lblResolution.transform.BGNJGIACJBG(150f);
			lblResolution.color = Constants.PJJIMHMJPAL;
			lblResolutionValue.set_Alias(string.Empty);
			lblResolutionValue.alignment = TextAnchor.MiddleLeft;
			lblResolutionValue.set_LabelFontSize(96);
			lblResolutionValue.color = Constants.PJJIMHMJPAL;
			lblResolutionValue.transform.OKHPLHPBPKJ(100f);
			lblResolutionValue.transform.BGNJGIACJBG(0f);
			GPLBHPLJNAE();
		}

		protected virtual void LOFKNKHJEDJ()
		{
			float num = ((!AssemblyController.JONCCPLEIBE().BKGIFIPIHAL()) ? 600f : 500f);
			float num2 = ((!lblUserId.gameObject.activeSelf) ? 0f : 37f);
			_topStripe.transform.BGNJGIACJBG(num + num2);
			num = ((!AssemblyController.JONCCPLEIBE().BKGIFIPIHAL()) ? (-600f) : (-540f));
			_bottomStripe.transform.BGNJGIACJBG(num - num2);
		}

		protected void OPFAOIMPGAD()
		{
			float num = ((!AssemblyController.JONCCPLEIBE().BKGIFIPIHAL()) ? 600f : 500f);
			float num2 = ((!lblUserId.gameObject.activeSelf) ? 0f : 37f);
			_topStripe.transform.BGNJGIACJBG(num + num2);
			num = ((!AssemblyController.JONCCPLEIBE().BKGIFIPIHAL()) ? (-600f) : (-540f));
			_bottomStripe.transform.BGNJGIACJBG(num - num2);
		}

		protected void LDIIKCNPLKB()
		{
			_topStripe.transform.BGNJGIACJBG(400f);
			_bottomStripe.transform.BGNJGIACJBG(-440f);
		}

		protected void OALBAOJIKMC()
		{
			float num = 600f;
			float num2 = ((!lblUserId.gameObject.activeSelf) ? 0f : 37f);
			_topStripe.transform.BGNJGIACJBG(num + num2);
			num = -600f;
			_bottomStripe.transform.BGNJGIACJBG(num - num2);
		}

		protected void GOCIHHMADOC()
		{
			string text = string.Empty;
			bool flag = GameCenterController.OBDJPKOJADA();
			if (SystemProperties.IPJFCBAGMJJ())
			{
				text = ((!flag) ? "SettingsButtons.googleplay_off_controller" : "SettingsButtons.googleplay_controller");
			}
			else if (SystemProperties.MEBGOGMJFLM())
			{
				text = ((!flag) ? "SettingsButtons.gamecenter_off" : "SettingsButtons.gamecenter");
			}
			else if (SystemProperties.AFKGHBJPLOK())
			{
				text = ((!flag) ? "SettingsButtons.windows_off" : "SettingsButtons.windows");
			}
			OHDFPIADEIG(btnGameCenter, text, text, 0f, 0f, AHDEAELNGBD.BTN_GAMECENTER);
			if (SystemProperties.IPJFCBAGMJJ())
			{
				btnGameCenter.SetDisabledSprite("UI/Atlases/", "SettingsButtons.googleplay_disable_controller");
			}
			else
			{
				btnGameCenter.SetDisabledSprite("UI/Atlases/", "SettingsButtons.gamecenter_disable");
			}
		}

		protected virtual void OnClickButton(object data)
		{
			switch ((AHDEAELNGBD)data)
			{
			case AHDEAELNGBD.BTN_RESOLUTION_HIGHER:
				if (EDHGAEOJKGO < SystemProperties.NPDPKLMFBHH() - 1)
				{
					EDHGAEOJKGO++;
				}
				else
				{
					EDHGAEOJKGO = 0;
				}
				UpdateLabels();
				break;
			case AHDEAELNGBD.BTN_RESOLUTION_LOWER:
				if (EDHGAEOJKGO > 0)
				{
					EDHGAEOJKGO--;
				}
				else
				{
					EDHGAEOJKGO = SystemProperties.NPDPKLMFBHH() - 1;
				}
				UpdateLabels();
				break;
			case AHDEAELNGBD.BTN_RESOLUTION_SET:
			{
				XmlDocument jFJPKEONJIJ = XmlUtils.OpenXMLDocument(SF2Paths.KKIDGPBOBNI(), "devices.xml");
				SystemProperties.GOCLBADJDGK(jFJPKEONJIJ);
				SystemProperties.CMEKDMFKDEO(EDHGAEOJKGO);
				GameUtils.StartFight();
				break;
			}
			case AHDEAELNGBD.BTN_MUSIC:
				SoundController.FMLHEDIPGAF(!SoundController.ELHMADOKHHE());
				OHDFPIADEIG(btnMusic, (!SoundController.ELHMADOKHHE()) ? "SettingsButtons.music" : "SettingsButtons.music_off", (!SoundController.ELHMADOKHHE()) ? "SettingsButtons.music_selected" : "SettingsButtons.music_off_selected", btnMusic.transform.localPosition.x, btnMusic.transform.localPosition.y, AHDEAELNGBD.BTN_MUSIC);
				break;
			case AHDEAELNGBD.BTN_SOUND:
				SoundController.FLOFHMBDHNM(!SoundController.AAFLCDKJEPL());
				OHDFPIADEIG(btnSound, (!SoundController.AAFLCDKJEPL()) ? "SettingsButtons.sound" : "SettingsButtons.sound_off", (!SoundController.AAFLCDKJEPL()) ? "SettingsButtons.sound_selected" : "SettingsButtons.sound_off_selected", btnSound.transform.localPosition.x, btnSound.transform.localPosition.y, AHDEAELNGBD.BTN_SOUND);
				break;
			case AHDEAELNGBD.BTN_CREDITS:
				CreditsScreen.Create();
				OnClose(0);
				break;
			case AHDEAELNGBD.BTN_FACEBOOK:
			{
				QuestParameters hHKLFIIBIFF = ListSF.ELEBLBJKDBI().BNMLDPNCMLB();
				FightIDS jLGLBLDPAAF = hHKLFIIBIFF.JLGLBLDPAAF;
				hHKLFIIBIFF.JLGLBLDPAAF = FightIDS.Empty();
				hHKLFIIBIFF.HEIADONEACH = string.Empty;
				hHKLFIIBIFF.AIEHNBBFNPF = string.Empty;
				hHKLFIIBIFF.DLKPBAJDHBO = null;
				if (ListSF.ELEBLBJKDBI().FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_LOGIN_FB))
				{
					ListSF.ELEBLBJKDBI().MHHNIPBJNAD();
				}
				hHKLFIIBIFF.JLGLBLDPAAF = jLGLBLDPAAF;
				break;
			}
			case AHDEAELNGBD.BTN_LANGUAGE:
			{
				LocalizationManager.Language pPNFBAFOOAH = LocalizationManager.KNEELNNCIBG(LFFLJJGJHIB);
				string mMBELNEBNBM = pPNFBAFOOAH.MMBELNEBNBM;
				string oKGJAMBPDGO = pPNFBAFOOAH.OKGJAMBPDGO;
				oKGJAMBPDGO = ((!(oKGJAMBPDGO == string.Empty)) ? oKGJAMBPDGO : mMBELNEBNBM);
				OHDFPIADEIG(btnLanguage, mMBELNEBNBM, oKGJAMBPDGO, btnLanguage.transform.localPosition.x, btnLanguage.transform.localPosition.y, AHDEAELNGBD.BTN_LANGUAGE);
				PGMBIJFAEHP(lblLanguage, pPNFBAFOOAH.LOKLDPLAPOL);
				EIJHFKINNPF = true;
				LFFLJJGJHIB = pPNFBAFOOAH;
				BIBDCOKMMKO = 0;
				break;
			}
			case AHDEAELNGBD.BTN_GAMECENTER:
				if (GameCenterController.OBDJPKOJADA())
				{
					if (SystemProperties.IPJFCBAGMJJ())
					{
						GameCenterController.CLPNGGPKAHO();
						GOCIHHMADOC();
						ListSF.CCDKHLAMKKO().BCMKHEKOMDB(false);
					}
					else if (SystemProperties.MEBGOGMJFLM())
					{
						GameCenterController.NPMGIFJKAEG();
					}
					else if (SystemProperties.DDIDANINPJE())
					{
						GameCenterController.CLPNGGPKAHO();
						GOCIHHMADOC();
					}
				}
				else
				{
					if (SystemProperties.DDIDANINPJE())
					{
						GameUtils.FMICOICLCNL();
						BOPCBJJIHNK(true);
					}
					GameCenterController.EFKOIIKEHDO();
					ListSF.CCDKHLAMKKO().BCMKHEKOMDB(true);
					ListSF.CCDKHLAMKKO().MFJLCDAEFFD();
				}
				break;
			case AHDEAELNGBD.BTN_SUPPORT:
			{
				string url2 = GameUtils.JOEMCCADMON.EBCODADFJLB(LocalizationManager.ILAJKOBCHFH.name, LocalizationManager.POIPGLLCCKC);
				OfflineServices.OpenExternalUrl(url2);
				break;
			}
			case AHDEAELNGBD.BTN_ITUNES:
			{
				string url = InternetController.MMIHGFKCMCC();
				OfflineServices.OpenExternalUrl(url);
				break;
			}
			}
		}

		protected void OnAuthenticate(bool CELFBNLILMA)
		{
			if (CELFBNLILMA)
			{
				ELGPMKOMMKL();
				ListSF.ELEBLBJKDBI().DKBINLMJIJG();
			}
			else
			{
				DMDOKDCLNLM();
			}
		}

		protected void ELGPMKOMMKL()
		{
			btnGameCenter.interactable = true;
			if (!ListSF.CCDKHLAMKKO().DFNEGEEHLFJ())
			{
				ListSF.CCDKHLAMKKO().OEAOPFDLMJJ(true);
			}
			if (SystemProperties.DDIDANINPJE())
			{
				BOPCBJJIHNK(false);
			}
		}

		protected void DMDOKDCLNLM()
		{
			if (SystemProperties.DDIDANINPJE())
			{
				GOCIHHMADOC();
			}
		}

		protected void MFIGCNECAAN(object data)
		{
			EBBGCLGJBBC = DialogsOpener.CLOCBDBIAEF();
			if (EBBGCLGJBBC != null)
			{
				EBBGCLGJBBC.AddEventListener(0, FDJDPMIKIHN);
				EBBGCLGJBBC.AddEventListener(2, OnClose);
				base.gameObject.SetActive(false);
			}
		}

		protected void FDJDPMIKIHN(object data)
		{
			BHLHODFNHHO();
			bool flag = SoundController.ELHMADOKHHE();
			OHDFPIADEIG(btnMusic, (!flag) ? "SettingsButtons.music" : "SettingsButtons.music_off", (!flag) ? "SettingsButtons.music_selected" : "SettingsButtons.music_off_selected", btnMusic.transform.localPosition.x, btnMusic.transform.localPosition.y, AHDEAELNGBD.BTN_MUSIC);
			bool flag2 = SoundController.AAFLCDKJEPL();
			OHDFPIADEIG(btnSound, (!flag2) ? "SettingsButtons.sound" : "SettingsButtons.sound_off", (!flag2) ? "SettingsButtons.sound_selected" : "SettingsButtons.sound_off_selected", btnSound.transform.localPosition.x, btnSound.transform.localPosition.y, AHDEAELNGBD.BTN_SOUND);
			base.gameObject.SetActive(true);
			EBBGCLGJBBC.RemoveAllEventListener();
			EBBGCLGJBBC = null;
		}

		protected override void BHLHODFNHHO()
		{
		}

		protected void ILLEDELEEPC()
		{
			QuestParameters hHKLFIIBIFF = ListSF.ELEBLBJKDBI().BNMLDPNCMLB();
			hHKLFIIBIFF.GMGMEEIKGLG = LFFLJJGJHIB;
			if (ListSF.ELEBLBJKDBI().FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_LANGUAGE_SWITCH))
			{
				ListSF.ELEBLBJKDBI().MHHNIPBJNAD();
				return;
			}
			LocalizationManager.BJPNKAGDKFL(LFFLJJGJHIB);
			UpdateLabels();
		}

		protected bool PEPADDIALAO()
		{
			return AssemblyController.KMEOEAGGPBI();
		}

		protected void BOPCBJJIHNK(bool JILGHDDEMPE)
		{
			btnSound.gameObject.SetActive(!JILGHDDEMPE);
			btnMusic.gameObject.SetActive(!JILGHDDEMPE);
			if (btnCredits != null) btnCredits.gameObject.SetActive(!JILGHDDEMPE);
			btnLanguage.gameObject.SetActive(!JILGHDDEMPE);
			if (SystemProperties.DDIDANINPJE())
			{
				btnGameCenter.interactable = !JILGHDDEMPE;
			}
			if (btnSupport != null) btnSupport.gameObject.SetActive(!JILGHDDEMPE);
		}
	}
}
