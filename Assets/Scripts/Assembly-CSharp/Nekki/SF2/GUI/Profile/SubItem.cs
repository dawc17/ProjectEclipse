using UnityEngine;
using UnityEngine.EventSystems;

namespace Nekki.SF2.GUI.Profile
{
	public class SubItem : SFButton
	{
		public enum DNJPAMBANOM
		{
			onChoose = 10
		}

		[SerializeField]
		protected ResolutionImage _icon;

		[SerializeField]
		protected ResolutionImage _lockPicture;

		[SerializeField]
		protected ResolutionImage _inactivePicture;

		[SerializeField]
		protected ResolutionImage _selectedPicture;

		[SerializeField]
		protected ResolutionImage _selectWhiteSquare;

		[SerializeField]
		protected ResolutionImage _backPicture;

		public ProfileCell ParentCell;

		protected bool IIPJNGBMJJP;

		protected bool _active;

		protected bool OAHJFBHINFG;

		public object Data;

		protected int BIBDCOKMMKO;

		protected int AGOKAGIEIJM;

		protected bool BLNIFOCLLPD = true;

		protected float BHKAAODJMJF = 1f;

		protected float CDNOKAKOLMP = 1f;

		protected string _texturePath = SF2Paths.KLIDILIHOFF();

		protected string GJPJJHACOJJ = string.Empty;

		private float HBEKOCIKIAJ;

		private float MPHAGFFDINJ;

		private static bool GIIHHBFGAFM = true;

		private bool APMOONFFHEC;

		private float MJBPKKGLGIH;

		private float DOOOGJGOJFB = 1f;

		public void Init(int OKNNNLIPODI)
		{
			HBEKOCIKIAJ = ProfileGUI.OMILCNNEBIL.EBDBPJNBHGI / 255f;
			MPHAGFFDINJ = ProfileGUI.OMILCNNEBIL.DPGMCKCDMBC / 255f;
			AGOKAGIEIJM = ProfileGUI.AnimationSpeed;
			ButtonId = OKNNNLIPODI;
			_backPicture.gameObject.SetActive(false);
		}

		public virtual void SetLock(bool AJPDLMOHKEN)
		{
			IIPJNGBMJJP = AJPDLMOHKEN;
			GAGLGNPDCCL();
		}

		public virtual bool GetLock()
		{
			return IIPJNGBMJJP;
		}

		public virtual void SetActive(bool HNJDHGDLLPD)
		{
			_active = HNJDHGDLLPD;
			JDCOEPMIBCI();
		}

		public virtual bool GetActive()
		{
			return _active;
		}

		public virtual void SetSelected(bool CMEFIGAKNFG)
		{
			OAHJFBHINFG = CMEFIGAKNFG;
			if ((bool)_selectedPicture)
			{
				_selectedPicture.gameObject.SetActive(OAHJFBHINFG);
			}
			if ((bool)_selectWhiteSquare && AssemblyController.KMEOEAGGPBI())
			{
				_selectWhiteSquare.gameObject.SetActive(OAHJFBHINFG);
			}
		}

		public virtual bool GetSelected()
		{
			return OAHJFBHINFG;
		}

		public virtual void Choose()
		{
			int buttonId = ButtonId;
			CallEvent(10, buttonId);
		}

		public virtual void UpdateState()
		{
		}

		public static void EnableAnimation(bool value)
		{
			GIIHHBFGAFM = value;
		}

		public void SetSelectFlashing(bool LHGLOOMODPK)
		{
			if (_selectWhiteSquare != null)
			{
			}
			APMOONFFHEC = LHGLOOMODPK;
		}

		public void SetSelectFlashingMinOpacity(float IEKAFNFKBNE)
		{
			if (_selectWhiteSquare != null)
			{
			}
			MJBPKKGLGIH = IEKAFNFKBNE;
			if (MJBPKKGLGIH < 0f)
			{
				MJBPKKGLGIH = 0f;
			}
			if (MJBPKKGLGIH > 1f)
			{
				MJBPKKGLGIH = 1f;
			}
		}

		public void SetSelectFlashingMaxOpacity(int BIPFOECJBNE)
		{
			if (_selectWhiteSquare != null)
			{
			}
			DOOOGJGOJFB = BIPFOECJBNE;
			if (DOOOGJGOJFB < 0f)
			{
				DOOOGJGOJFB = 0f;
			}
			if (DOOOGJGOJFB > 1f)
			{
				DOOOGJGOJFB = 1f;
			}
		}

		protected virtual void FGICHADOEHF()
		{
			if (OAHJFBHINFG)
			{
				float num = HBEKOCIKIAJ - MPHAGFFDINJ;
				if (num > 0f && AGOKAGIEIJM > 0)
				{
					float num2 = num / (float)AGOKAGIEIJM * (float)BIBDCOKMMKO;
					float kGJALFLDIBG = ((!BLNIFOCLLPD) ? (HBEKOCIKIAJ - num2) : (MPHAGFFDINJ + num2));
					UIExtensions.HNIHBGAOAIH(_selectedPicture, kGJALFLDIBG);
				}
			}
		}

		protected virtual void AJGODMIMDDP()
		{
			float num = BHKAAODJMJF - CDNOKAKOLMP;
			if (num > 0f && AGOKAGIEIJM > 0)
			{
				float num2 = num / (float)AGOKAGIEIJM * (float)BIBDCOKMMKO;
				float kGJALFLDIBG = ((!BLNIFOCLLPD) ? (BHKAAODJMJF - num2) : (CDNOKAKOLMP + num2));
				if (_icon != null)
				{
					UIExtensions.HNIHBGAOAIH(_icon, kGJALFLDIBG);
				}
			}
		}

		public virtual void UpdateIcon()
		{
			_icon.set_TexturePath(_texturePath);
			_icon.set_SpriteName(GJPJJHACOJJ);
			_selectedPicture.gameObject.SetActive(false);
			_lockPicture.gameObject.SetActive(false);
			_inactivePicture.gameObject.SetActive(false);
			JDCOEPMIBCI();
		}

		protected virtual void JDCOEPMIBCI()
		{
			if (!_inactivePicture.IsDestroyed())
			{
				_inactivePicture.gameObject.SetActive(!_active);
			}
		}

		protected virtual void GAGLGNPDCCL()
		{
			if (_lockPicture != null)
			{
				_lockPicture.gameObject.SetActive(IIPJNGBMJJP);
			}
			if (_icon != null)
			{
				_icon.gameObject.SetActive(!IIPJNGBMJJP);
			}
		}

		protected virtual void FHCOMGJEKHE(NFOGOFFAPPP.HHGPKAJENGF LFLGCDNKNJI)
		{
			JDCOEPMIBCI();
		}

		private void Update()
		{
			if (GIIHHBFGAFM)
			{
				FGICHADOEHF();
			}
			BIBDCOKMMKO++;
			if (BIBDCOKMMKO > AGOKAGIEIJM)
			{
				BIBDCOKMMKO = 0;
				BLNIFOCLLPD = !BLNIFOCLLPD;
			}
		}

		public override void OnPointerClick(PointerEventData BHOLFGOGPCP)
		{
			base.OnPointerClick(BHOLFGOGPCP);
			if (!GetLock())
			{
				Choose();
			}
		}

		protected virtual void FOPPGHBAKHJ(bool LPPNCLBEAFA)
		{
			_backPicture.gameObject.SetActive(LPPNCLBEAFA);
		}
	}
}
