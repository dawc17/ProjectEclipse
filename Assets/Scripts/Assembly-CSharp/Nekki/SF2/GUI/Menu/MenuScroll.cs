using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Menu
{
	public class MenuScroll : SFMonoBehaviour<object>, IEventSystemHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		public enum HOIFAHAFGAN
		{
			OnOpen = 0,
			OnClose = 1,
			OnChanging = 2,
			OnRolling = 3,
			OnTouch = 4
		}

		public enum GLLGENPACJB
		{
			Horizontal = 0,
			Vertical = 1
		}

		public enum ANJKEGGALAG
		{
			ScrollNone = 0,
			ScrollOpen = 1,
			ScrollClose = 2,
			ScrollOpeninig = 3,
			ScrollClosinig = 4
		}

		private enum BMGEOPAACDB
		{
			ZBackground = 0,
			ZPaper = 1,
			ZContent = 2,
			ZWheel = 3
		}

		public Color SCROLL_BORDER_COLOR = new Color(255f, 100f, 100f);

		public const float SCROLL_EXPAND_TIME = 0.3f;

		public const int SCROLL_SAFE_DISTANCE = 15;

		public bool IsOpen;

		public ANJKEGGALAG CurScrollState;

		private float LNINNFMPDBN;

		private float AFBEKDGHAJB;

		private float DBPDGHMCCIH = 10f;

		private GLLGENPACJB _type;

		[SerializeField]
		private Button _wheel;

		[SerializeField]
		private Text _label;

		[SerializeField]
		private Image _background;

		private float KHDCPBMEMIE = 1f;

		private Vector2 _touchPoint;

		private bool EDMOOGBMKNA;

		private bool IHMDFGALDOA;

		private bool MNKJBHOHCFL;

		private bool BMGKBJEFDJG;

		private Tween _tween;

		public void Init(GLLGENPACJB LFLGCDNKNJI = GLLGENPACJB.Vertical)
		{
			_type = LFLGCDNKNJI;
			AFBEKDGHAJB = GetCurrentLength();
			MNKJBHOHCFL = true;
			IsOpen = false;
			BMGKBJEFDJG = true;
			IHMDFGALDOA = false;
			CurScrollState = ANJKEGGALAG.ScrollNone;
			float a = _background.color.a;
			KHDCPBMEMIE = 255f / a;
			Collapse(0f);
		}

		public void Expand(float _Duration)
		{
			EDMOOGBMKNA = true;
			NKIIGBBMNNL(AFBEKDGHAJB, _Duration);
		}

		public void Collapse(float _Duration)
		{
			EDMOOGBMKNA = false;
			NKIIGBBMNNL(0f, _Duration);
		}

		private void NKIIGBBMNNL(float GGAIEIDOEAD, float _Duration)
		{
			KillTween();
			if (_Duration <= 0f)
			{
				HNNCMDONLMO(GGAIEIDOEAD);
				return;
			}
			_tween = DOTween.To(() => GetCurrentLength(), (float ECHIHNECKFK) =>
			{
				HNNCMDONLMO(ECHIHNECKFK);
			}, GGAIEIDOEAD, _Duration);
		}

		public bool IsExpanded()
		{
			return EDMOOGBMKNA;
		}

		public void SetOutsideTouchProperties(bool NEHLEJGGCIE)
		{
			BMGKBJEFDJG = NEHLEJGGCIE;
		}

		public float GetCurrentLength()
		{
			if (_type == GLLGENPACJB.Vertical)
			{
				return base.gameObject.GetComponent<RectTransform>().rect.height;
			}
			return base.gameObject.GetComponent<RectTransform>().rect.width;
		}

		public void SetAllowRolling(bool value)
		{
			MNKJBHOHCFL = value;
		}

		public bool GetAllowRolling()
		{
			return MNKJBHOHCFL;
		}

		public Button GetButton()
		{
			return _wheel;
		}

		private void Update()
		{
		}

		private void HNNCMDONLMO(float BDBOAEGELMC)
		{
			if (BDBOAEGELMC < 0f)
			{
				BDBOAEGELMC = 0f;
			}
			bool flag = BDBOAEGELMC == 0f;
			bool flag2 = Mathf.Abs(BDBOAEGELMC) == Mathf.Abs(AFBEKDGHAJB);
			if (_type == GLLGENPACJB.Vertical)
			{
				base.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(base.gameObject.GetComponent<RectTransform>().rect.width, BDBOAEGELMC);
			}
			else
			{
				base.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(BDBOAEGELMC, base.gameObject.GetComponent<RectTransform>().rect.height);
			}
			EAGCKGDFILH();
			if (flag && CurScrollState != ANJKEGGALAG.ScrollClose)
			{
				CurScrollState = ANJKEGGALAG.ScrollClose;
				CallEvent(2, false);
				CallEvent(1, 0);
			}
			else if (flag2 && CurScrollState != ANJKEGGALAG.ScrollOpen)
			{
				CurScrollState = ANJKEGGALAG.ScrollOpen;
				CallEvent(2, true);
				CallEvent(0, 0);
			}
			else if (!flag && !flag2)
			{
				CurScrollState = ANJKEGGALAG.ScrollNone;
			}
			CallEvent(3, CurScrollState);
		}

		public void OnMenuBtnClick()
		{
			if (EDMOOGBMKNA)
			{
				Collapse(0.3f);
			}
			else
			{
				Expand(0.3f);
			}
		}

		public void OnBackgroundClick()
		{
			if (BMGKBJEFDJG)
			{
				Collapse(0.3f);
			}
		}

		private void EAGCKGDFILH()
		{
			float num = Mathf.Abs(GetCurrentLength() / AFBEKDGHAJB) * 255f;
			float num2 = num / KHDCPBMEMIE;
			_background.color = new Color(_background.color.r, _background.color.g, _background.color.b, num2);
			if (num2 == 0f && _background.raycastTarget)
			{
				_background.raycastTarget = false;
			}
			else if (num2 > 0f && !_background.raycastTarget)
			{
				_background.raycastTarget = true;
			}
		}

		private bool IsTouchOnWheel(Vector2 DGEJJGMMODA)
		{
			return true;
		}

		public void OnBeginDrag(PointerEventData BHOLFGOGPCP)
		{
			Vector2 localPoint;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), BHOLFGOGPCP.position, BHOLFGOGPCP.pressEventCamera, out localPoint);
			if (MNKJBHOHCFL || IsTouchOnWheel(localPoint))
			{
				_touchPoint = localPoint;
				IHMDFGALDOA = true;
				KillTween();
				CallEvent(4, 0);
			}
		}

		public void OnDrag(PointerEventData BHOLFGOGPCP)
		{
			Vector2 localPoint;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), BHOLFGOGPCP.position, BHOLFGOGPCP.pressEventCamera, out localPoint);
			if (IHMDFGALDOA)
			{
				float num = _touchPoint.x - localPoint.x;
				float num2 = _touchPoint.y - localPoint.y;
				float a = GetCurrentLength() + ((_type != GLLGENPACJB.Vertical) ? num : num2);
				HNNCMDONLMO(Mathf.Min(a, AFBEKDGHAJB));
				_touchPoint = localPoint;
			}
		}

		public void OnEndDrag(PointerEventData BHOLFGOGPCP)
		{
			IHMDFGALDOA = false;
			float num = ((!EDMOOGBMKNA) ? GetCurrentLength() : (AFBEKDGHAJB - GetCurrentLength()));
			bool flag = num > AFBEKDGHAJB * DBPDGHMCCIH / 100f;
			bool flag2 = EDMOOGBMKNA != flag;
			float dFNBHOEGAHO = ((!flag2) ? GetCurrentLength() : (AFBEKDGHAJB - GetCurrentLength())) / AFBEKDGHAJB * 0.3f;
			if (flag2)
			{
				Expand(dFNBHOEGAHO);
			}
			else
			{
				Collapse(dFNBHOEGAHO);
			}
		}

		private void KillTween()
		{
			if (_tween != null)
			{
				_tween.Kill();
				_tween = null;
			}
		}
	}
}
