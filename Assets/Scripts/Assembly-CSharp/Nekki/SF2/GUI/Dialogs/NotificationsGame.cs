using System;
using System.Collections;
using System.Collections.Generic;
using Nekki.SF2.GUI.Fight;
using Nekki.SF2.GUI.Menu;
using UnityEngine;

namespace Nekki.SF2.GUI.Dialogs
{
	public class NotificationsGame : SFMonoBehaviour<object>, BackKeyController
	{
		public enum HMFHPEHAOFK
		{
			ON_CLOSE = 0
		}

		public const float SCROLL_SECONDS = 0.5f;

		private static bool GLABMAEJPPN;

		private bool DAJKLNPEONE;

		private bool KKGOCBCNLGF;

		private bool KNHMOOPFLJJ;

		[SerializeField]
		private MenuScroll _scroll;

		[SerializeField]
		private ResolutionImageAvatar _image;

		[SerializeField]
		private LabelAlias _label;

		[SerializeField]
		private LabelButton _button;

		private string JGPEECABCHF = string.Empty;

		private string HCPNFPMHFCM = string.Empty;

		private string CAHABJBMIKJ = string.Empty;

		private LabelButton.FBMGEHJPPIK CJCHGLEFGED = LabelButton.FBMGEHJPPIK.BUTTON_WHITE;

		private float FFLLNCBOGJJ;

		private Action<object> callback;

		private IEnumerator AHMGGJBPHHO;

		private static NotificationsGame _instance;

		public static bool PLCIGHLBOPP
		{
			get
			{
				return get_IsOpen();
			}
		}

		public static NotificationsGame BPCBBHAKFDM
		{
			get
			{
				return get_Instance();
			}
		}

		public static bool get_IsOpen()
		{
			return GLABMAEJPPN;
		}

		public static NotificationsGame get_Instance()
		{
			if (_instance == null)
			{
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Dialogs/NotificationGame"));
				gameObject.name = "[NotificationsGame]";
				_instance = gameObject.GetComponent<NotificationsGame>();
				_instance.Init();
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
			}
			return _instance;
		}

		public void Init()
		{
			KFIMLFDHHEL();
		}

		public static void CloseNotifications()
		{
			if (_instance != null)
			{
				_instance.Close();
			}
		}

		private void OnDestroy()
		{
			_instance = null;
		}

		public void OpenNotification(string KCDCAGJFHJF, List<StoryDialogContent> IHMEPGICLGF, Action<object> CGGAFBLKFBP, string JEDLCDIIFDN, LabelButton.FBMGEHJPPIK JNBBEBBCEJK, float HEPAGADOKGI)
		{
			if (HDLDBIJDEIL())
			{
				JGPEECABCHF = KCDCAGJFHJF;
				NBGHLFJPOGM(IHMEPGICLGF);
				CAHABJBMIKJ = JEDLCDIIFDN;
				CJCHGLEFGED = JNBBEBBCEJK;
				callback = CGGAFBLKFBP;
				FFLLNCBOGJJ = HEPAGADOKGI;
				EGHJDCCGLOD();
				if (AHMGGJBPHHO != null)
				{
					CoroutineManager.get_Current().StopRoutine(AHMGGJBPHHO);
				}
				AHMGGJBPHHO = ENMNDLBFNMN();
				CoroutineManager.get_Current().StartRoutine(AHMGGJBPHHO);
			}
		}

		private IEnumerator ENMNDLBFNMN()
		{
			DAJKLNPEONE = false;
			yield return new WaitForSeconds(FFLLNCBOGJJ);
			DAJKLNPEONE = true;
		}

		private void Update()
		{
			if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && DAJKLNPEONE)
			{
				_scroll.OnBackgroundClick();
			}
		}

		public void OnBackKeyClicked(object data)
		{
			if (DAJKLNPEONE)
			{
				Close();
			}
		}

		private void NBGHLFJPOGM(List<StoryDialogContent> IHMEPGICLGF)
		{
			HCPNFPMHFCM = string.Empty;
			for (int i = 0; i < IHMEPGICLGF.Count; i++)
			{
				HCPNFPMHFCM += LocalizationManager.GetString(IHMEPGICLGF[i].GGDJIPKMKFC);
				if (i + 1 < IHMEPGICLGF.Count)
				{
					HCPNFPMHFCM += "\n";
				}
			}
		}

		private void EGHJDCCGLOD()
		{
			if (GLABMAEJPPN)
			{
				GMOKBKIGGFN();
				return;
			}
			_scroll.SetOutsideTouchProperties(false);
			string[] array = JGPEECABCHF.Split('|');
			string[] array2 = JGPEECABCHF.Split('/');
			string[] array3 = array2[array2.Length - 1].Split('.');
			_image.set_TexturePath(SF2Paths.BHCPOOOJAAK());
			_image.set_SpriteName(array3[0]);
			_label.set_text(HCPNFPMHFCM);
			_button.gameObject.SetActive(CAHABJBMIKJ == string.Empty);
			_button.interactable = CAHABJBMIKJ == string.Empty;
			if (CAHABJBMIKJ == string.Empty)
			{
				_button.SetColor(CJCHGLEFGED);
				_button.SetAlias(CAHABJBMIKJ);
				_button.AddEventListener(2, OnButtonClick);
			}
			LAJCMNNNIIM();
		}

		private void IDFCHLJMFJC(object data)
		{
			GLABMAEJPPN = (bool)data;
			if (!GLABMAEJPPN)
			{
				_scroll.gameObject.SetActive(false);
				IMHFKNLNEEJ(KNHMOOPFLJJ ? 1 : 0);
				KNHMOOPFLJJ = false;
				_scroll.SetOutsideTouchProperties(false);
			}
			else
			{
				_scroll.SetOutsideTouchProperties(true);
			}
		}

		private void AHHJGONNGHF(object data)
		{
			CallEvent(0, 0);
			if (KKGOCBCNLGF)
			{
				KKGOCBCNLGF = false;
				EGHJDCCGLOD();
			}
		}

		private void OnButtonClick(object data)
		{
			if (GLABMAEJPPN)
			{
				KNHMOOPFLJJ = true;
				Close();
			}
		}

		private void KFIMLFDHHEL()
		{
			_scroll.Init(MenuScroll.GLLGENPACJB.Horizontal);
			_scroll.SetOutsideTouchProperties(false);
			_scroll.Collapse(0f);
			_scroll.AddEventListener(2, IDFCHLJMFJC);
			_scroll.AddEventListener(1, AHHJGONNGHF);
			_scroll.GetButton().interactable = false;
			_scroll.gameObject.SetActive(false);
		}

		private void LAJCMNNNIIM()
		{
			BackKeyManager.get_Instance().AddBackKeyController(this);
			_scroll.gameObject.SetActive(true);
			_scroll.Expand(0.5f);
		}

		private void Close()
		{
			BackKeyManager.get_Instance().RemoveBackKeyController(this);
			if (_button != null)
			{
				_button.RemoveEventListener(2, OnButtonClick);
			}
			_scroll.Collapse(0.5f);
		}

		private void GMOKBKIGGFN()
		{
			KKGOCBCNLGF = true;
			Close();
		}

		private void IMHFKNLNEEJ(int value)
		{
			callback(value);
		}

		private void IAIDJOCDLJA(object data)
		{
			Close();
		}

		private bool HDLDBIJDEIL()
		{
			switch (Module.ELEBLBJKDBI().NMCNDOPKFJD())
			{
			case ScreenType.ModulePreloader:
			case ScreenType.ModuleCreditsScreen:
				return false;
			case ScreenType.ModuleFight:
			{
				FightScene current = Scene<FightScene>.get_Current();
				if (current != null && current.Fight != null && current.Fight.OGNINOBBHIG() != null && current.Fight.OGNINOBBHIG().get_Type() != BattleType.FightNone)
				{
					return false;
				}
				break;
			}
			}
			return true;
		}
	}
}
