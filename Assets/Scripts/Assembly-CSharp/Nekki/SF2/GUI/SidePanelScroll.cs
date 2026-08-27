using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Nekki.SF2.GUI
{
	public class SidePanelScroll : ScrollRect
	{
		[Serializable]
		public class ScrollDragEvent : UnityEvent<PointerEventData>
		{
		}

		[SerializeField]
		public ScrollDragEvent onScrollDragEnd = new ScrollDragEvent();

		[SerializeField]
		public ScrollDragEvent onScrollDragBegin = new ScrollDragEvent();

		public override void OnBeginDrag(PointerEventData data)
		{
			base.OnBeginDrag(data);
			onScrollDragBegin.Invoke(data);
		}

		public override void OnEndDrag(PointerEventData data)
		{
			base.OnEndDrag(data);
			onScrollDragEnd.Invoke(data);
		}
	}
}
