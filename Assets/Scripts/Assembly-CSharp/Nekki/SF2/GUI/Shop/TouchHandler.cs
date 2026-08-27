using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Shop
{
	public class TouchHandler : Selectable
	{
		private UnityEvent FPANPKJAHCA = new UnityEvent();

		public UnityEvent PFPJJNKLCKK
		{
			get
			{
				return get_OnTouch();
			}
		}

		public UnityEvent get_OnTouch()
		{
			return FPANPKJAHCA;
		}

		public override void OnPointerDown(PointerEventData BHOLFGOGPCP)
		{
			base.OnPointerDown(BHOLFGOGPCP);
			FPANPKJAHCA.Invoke();
		}

		private new void OnDestroy()
		{
			get_OnTouch().RemoveAllListeners();
			base.OnDestroy();
		}
	}
}
