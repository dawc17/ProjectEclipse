using DG.Tweening;
using DG.Tweening.Core.Surrogates;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Shop
{
	public class SidePanel : SFMonoBehaviour<object>
	{
		[SerializeField]
		private Vector3 _openBtnPos = new Vector2(0f, 0f);

		[SerializeField]
		private Vector3 _closeBtnPos = new Vector2(0f, 0f);

		[SerializeField]
		private Button _moveButton;

		[SerializeField]
		private ScrollRect _scrollRect;

		[SerializeField]
		private RectTransform _contentParent;

		[SerializeField]
		private CanvasGroup _canvasGroup;

		private SidePanelContent GCGGIJDKKKO;

		private const float JMNKFPNCFFG = 1f;

		private bool EABBPDKDKJE = true;

		private bool EOPFNNGEJOL = true;

		private bool MMEDJKEFDJC = true;

		private string NKPHGKJGJFK;

		private string IBFJLPCDHBH;

		private Vector2 EGKGKFBGFNN;

		private Vector3 NCEDIDPKGPF = new Vector3(0f, 0f);

		private Vector3 DCCOOOKAGIF = new Vector3(0f, 0f);

		private Tween _tween;

		public string CMJPCPLOJCA
		{
			get
			{
				return get_OpenImage();
			}
			set
			{
				set_OpenImage(value);
			}
		}

		public string KHCPHAPBIOF
		{
			get
			{
				return get_CloseImage();
			}
			set
			{
				set_CloseImage(value);
			}
		}

		public string get_OpenImage()
		{
			return NKPHGKJGJFK;
		}

		public void set_OpenImage(string value)
		{
			NKPHGKJGJFK = value;
			NLCJBAIMKLM();
		}

		public string get_CloseImage()
		{
			return IBFJLPCDHBH;
		}

		public void set_CloseImage(string value)
		{
			IBFJLPCDHBH = value;
			NLCJBAIMKLM();
		}

		public void Init(SidePanelContent DMNBDBJNKME, bool JOJGKNGGAHB, float MDPGKEDBHNO = 0f, bool NKGDKKNNJOF = true, string NEFNMHJLBPC = null, string AENEHAMGPBC = null)
		{
			MMEDJKEFDJC = JOJGKNGGAHB;
			NCEDIDPKGPF.y = MDPGKEDBHNO;
			DCCOOOKAGIF.y = MDPGKEDBHNO;
			EABBPDKDKJE = NKGDKKNNJOF;
			NKPHGKJGJFK = NEFNMHJLBPC;
			IBFJLPCDHBH = AENEHAMGPBC;
			GCGGIJDKKKO = DMNBDBJNKME;
			if (_scrollRect != null)
			{
				EGKGKFBGFNN = _scrollRect.normalizedPosition;
			}
			if (_moveButton != null)
			{
				_moveButton.gameObject.SetActive(MMEDJKEFDJC);
			}
			if (_scrollRect != null)
			{
				_scrollRect.horizontal = MMEDJKEFDJC;
			}
			if (GCGGIJDKKKO != null && _contentParent != null)
			{
				GCGGIJDKKKO.transform.SetParent(_contentParent, false);
			}
			SetOpen(EABBPDKDKJE, 0f);
		}

		public void OnClick()
		{
			if (!MMEDJKEFDJC) return;
			SetOpen(!EABBPDKDKJE);
		}

		public void OnValueChanged(Vector2 LCCLEFMKLPB)
		{
			if (!MMEDJKEFDJC) return;
			if (EGKGKFBGFNN.x > LCCLEFMKLPB.x || LCCLEFMKLPB.x == 0f)
			{
				EOPFNNGEJOL = true;
			}
			if (EGKGKFBGFNN.x < LCCLEFMKLPB.x || LCCLEFMKLPB.x == 1f)
			{
				EOPFNNGEJOL = false;
			}
			EGKGKFBGFNN = LCCLEFMKLPB;
		}

		public void OnScrollDragBegin(PointerEventData data)
		{
			if (!MMEDJKEFDJC) return;
			KillTween();
			base.gameObject.transform.SetSiblingIndex(1);
		}

		public void OnScrollDragEnd(PointerEventData data)
		{
			if (!MMEDJKEFDJC) return;
			SetOpen(EOPFNNGEJOL);
		}

		private void SetPosition(Vector2 LCCLEFMKLPB)
		{
			EGKGKFBGFNN = LCCLEFMKLPB;
			if (_scrollRect != null)
			{
				_scrollRect.normalizedPosition = EGKGKFBGFNN;
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

		private void MGDHEBAMBFN(Vector2 LCCLEFMKLPB, float _Duration)
		{
			KillTween();
			_tween = DOTween.To(() => EGKGKFBGFNN, (Vector2Wrapper HBLGAEMOHAL) =>
			{
				SetPosition(HBLGAEMOHAL);
			}, LCCLEFMKLPB, _Duration);
			_tween.OnComplete(CMLDLHPOPFJ);
		}

		private void CMLDLHPOPFJ()
		{
			if (!EABBPDKDKJE)
			{
				base.gameObject.transform.SetSiblingIndex(0);
				if (_canvasGroup != null)
				{
					_canvasGroup.blocksRaycasts = false;
				}
			}
			else if (_canvasGroup != null)
			{
				_canvasGroup.blocksRaycasts = true;
			}
		}

		private void MJNKCCHGOLD(string KHPKDMGDMAB)
		{
			ResolutionImage resolutionImage = _moveButton.image as ResolutionImage;
			if (resolutionImage != null)
			{
				resolutionImage.set_SpriteName(KHPKDMGDMAB);
			}
		}

		private void NLCJBAIMKLM()
		{
			if (_moveButton != null)
			{
				if (EABBPDKDKJE && NKPHGKJGJFK != null)
				{
					MJNKCCHGOLD(NKPHGKJGJFK);
					_moveButton.transform.localPosition = _openBtnPos + DCCOOOKAGIF;
					_moveButton.transform.SetSiblingIndex(1);
				}
				else if (!EABBPDKDKJE && IBFJLPCDHBH != null)
				{
					MJNKCCHGOLD(IBFJLPCDHBH);
					_moveButton.transform.localPosition = _closeBtnPos + NCEDIDPKGPF;
					_moveButton.transform.SetSiblingIndex(0);
				}
			}
		}

		public void SetOpen(bool FPCBALMEPEN, float _Duration = 1f)
		{
			EABBPDKDKJE = FPCBALMEPEN;
			if (FPCBALMEPEN)
			{
				MGDHEBAMBFN(new Vector2(0f, 0f), _Duration);
				base.gameObject.transform.SetSiblingIndex(1);
				NLCJBAIMKLM();
			}
			else if (!FPCBALMEPEN)
			{
				MGDHEBAMBFN(new Vector2(1f, 0f), _Duration);
				NLCJBAIMKLM();
				if (_canvasGroup != null)
				{
					_canvasGroup.blocksRaycasts = false;
				}
			}
		}
	}
}
