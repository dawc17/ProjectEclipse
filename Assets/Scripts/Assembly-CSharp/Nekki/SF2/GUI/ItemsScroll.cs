using System;
using DG.Tweening;
using DG.Tweening.Core.Surrogates;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Nekki.SF2.GUI
{
	public class ItemsScroll : SFScrollRect
	{
		[Serializable]
		public class ScrollEndEvent : UnityEvent
		{
		}

		[SerializeField]
		public ScrollEndEvent onScrollEnd = new ScrollEndEvent();

		[SerializeField]
		public UnityEvent onDragBegin = new UnityEvent();

		[SerializeField]
		private float _MinScrollVelocity;

		[SerializeField]
		private float _AutoscrollDuration = 1f;

		[SerializeField]
		public bool AutoscrollIsOn = true;

		[SerializeField]
		private bool _Scrolling;

		[SerializeField]
		public BaseScrollContent scrollContent;

		private bool CGAPKDMAENM;

		private Tween _tween;

		public float CNBIHHDLDGE
		{
			get
			{
				return get_MinScrollVelocity();
			}
			set
			{
				set_MinScrollVelocity(value);
			}
		}

		public float IHIPAJABAKO
		{
			get
			{
				return get_AutoscrollDuration();
			}
			set
			{
				set_AutoscrollDuration(value);
			}
		}

		public bool BOFPNLPDEPF
		{
			get
			{
				return get_Scrolling();
			}
			set
			{
				set_Scrolling(value);
			}
		}

		public float get_MinScrollVelocity()
		{
			return _MinScrollVelocity;
		}

		public void set_MinScrollVelocity(float value)
		{
			_MinScrollVelocity = value;
		}

		public float get_AutoscrollDuration()
		{
			return _AutoscrollDuration;
		}

		public void set_AutoscrollDuration(float value)
		{
			_AutoscrollDuration = value;
		}

		public bool get_Scrolling()
		{
			return _Scrolling;
		}

		public void set_Scrolling(bool value)
		{
			_Scrolling = value;
		}

		private void KillTween()
		{
			if (_tween != null)
			{
				_tween.Kill();
				_tween = null;
			}
		}

		private void MoveTo(Vector2 LCCLEFMKLPB, float _Duration)
		{
			KillTween();
			_tween = DOTween.To(() => NAEMFBHBLJO(), (Vector2Wrapper HBLGAEMOHAL) =>
			{
				SetContentPosition(HBLGAEMOHAL);
			}, LCCLEFMKLPB, _Duration);
			_tween.OnComplete(FLGCOBPCHMB);
		}

		private void FLGCOBPCHMB()
		{
			StopMovement();
			onScrollEnd.Invoke();
		}

		private Vector2 NAEMFBHBLJO()
		{
			if (scrollContent != null)
			{
				return ((RectTransform)scrollContent.transform).anchoredPosition;
			}
			return new Vector2(0f, 0f);
		}

		private void SetContentPosition(Vector2 LCCLEFMKLPB)
		{
			if (scrollContent != null)
			{
				((RectTransform)scrollContent.transform).anchoredPosition = LCCLEFMKLPB;
			}
		}

		private Vector2 KMCOHAGCFBN(BaseScrollItem item)
		{
			RectTransform rectTransform = (RectTransform)scrollContent.transform;
			Transform parent = rectTransform.parent;
			if (parent == null)
			{
				return rectTransform.anchoredPosition;
			}
			Vector3 vector = parent.InverseTransformPoint(base.transform.position);
			Vector3 vector2 = parent.InverseTransformPoint(item.get_CenterPosition());
			return rectTransform.anchoredPosition + (Vector2)(vector - vector2);
		}

		public void Init()
		{
			if (scrollContent != null)
			{
				scrollContent.onSelectItem.AddListener(KPMMGHLPOBL);
				scrollContent.Center = (RectTransform)base.transform;
				scrollContent.onClickItem.AddListener(JPJDBLDGCCK);
			}
		}

		protected void KPMMGHLPOBL(BaseScrollItem item)
		{
		}

		protected void JPJDBLDGCCK(BaseScrollItem item)
		{
			ScrollToItem(item, 1f);
		}

		public override void OnBeginDrag(PointerEventData BHOLFGOGPCP)
		{
			base.OnBeginDrag(BHOLFGOGPCP);
			KillTween();
			CGAPKDMAENM = true;
			onDragBegin.Invoke();
		}

		public override void OnEndDrag(PointerEventData BHOLFGOGPCP)
		{
			base.OnEndDrag(BHOLFGOGPCP);
			CGAPKDMAENM = false;
			if (AutoscrollIsOn && Math.Abs(get_velocity().magnitude) != 0f)
			{
				float num = 0f;
				if (get_horizontal())
				{
					num = get_velocity().x * 0.5f;
				}
				else if (get_vertical())
				{
					num = get_velocity().y * 0.5f;
				}
				BaseScrollItem selectedItem = scrollContent.SelectedItem;
				BaseScrollItem mBIJKDIEFIF = ((!(Math.Abs(get_velocity().magnitude) > Math.Abs(get_MinScrollVelocity()))) ? selectedItem : scrollContent.GetNearestItem(0f - num));
				num = scrollContent.GetDistanceToCenter(mBIJKDIEFIF);
				float dFNBHOEGAHO = Mathf.Min(0.5f, Mathf.Abs(Mathf.Ceil(num / get_velocity().magnitude)));
				ScrollToItem(mBIJKDIEFIF, dFNBHOEGAHO);
			}
		}

		public void ScrollToItem(BaseScrollItem item, float _Duration)
		{
			if (!CGAPKDMAENM)
			{
				Vector2 vector = KMCOHAGCFBN(item);
				if (_Duration == 0f)
				{
					SetContentPosition(vector);
					onScrollEnd.Invoke();
					StopMovement();
				}
				else
				{
					StopMovement();
					MoveTo(vector, _Duration);
				}
			}
		}

		public override void StopMovement()
		{
			base.StopMovement();
			set_Scrolling(false);
		}

		private bool PBNHJPMBJDA()
		{
			return get_velocity().magnitude > get_MinScrollVelocity();
		}
	}
}
