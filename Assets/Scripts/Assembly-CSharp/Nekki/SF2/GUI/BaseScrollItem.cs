using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Nekki.SF2.GUI
{
	public class BaseScrollItem : Button
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string HKGHEJDKCPI;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ButtonClickedEvent EIAODLFEAPA;

		protected float BFOEJPPDBAA = 1f;

		protected float PNOCLNNCEBB;

		protected float MGPPBIADMJM = 1f;

		public ButtonClickedEvent AGPIEADCCOC
		{
			get
			{
				return get_onDoubleClick();
			}
			set
			{
				set_onDoubleClick(value);
			}
		}

		public virtual Vector3 GEHBDCJNJMJ
		{
			get
			{
				return get_CenterPosition();
			}
		}

		public float JCMBJGNEBPM
		{
			get
			{
				return get_MaxOpacity();
			}
			set
			{
				set_MaxOpacity(value);
			}
		}

		public float GDMHGFKKIDL
		{
			get
			{
				return get_MinOpacity();
			}
			set
			{
				set_MinOpacity(value);
			}
		}

		public virtual float NLLBLGNNFBA
		{
			get
			{
				return get_Opacity();
			}
			set
			{
				set_Opacity(value);
			}
		}

		public BaseScrollItem()
		{
			set_Name(string.Empty);
			set_onDoubleClick(new ButtonClickedEvent());
		}

		public string get_Name()
		{
			return HKGHEJDKCPI;
		}

		public void set_Name(string value)
		{
			HKGHEJDKCPI = value;
		}

		public ButtonClickedEvent get_onDoubleClick()
		{
			return EIAODLFEAPA;
		}

		public void set_onDoubleClick(ButtonClickedEvent value)
		{
			EIAODLFEAPA = value;
		}

		public virtual Vector2 get_Size()
		{
			RectTransform rectTransform = (RectTransform)base.transform;
			return rectTransform.sizeDelta;
		}

		public virtual void set_Size(Vector2 value)
		{
			RectTransform rectTransform = (RectTransform)base.transform;
			rectTransform.sizeDelta = value;
		}

		public virtual Vector3 get_CenterPosition()
		{
			return base.transform.position;
		}

		public float get_MaxOpacity()
		{
			return BFOEJPPDBAA;
		}

		public void set_MaxOpacity(float value)
		{
			BFOEJPPDBAA = value;
		}

		public float get_MinOpacity()
		{
			return PNOCLNNCEBB;
		}

		public void set_MinOpacity(float value)
		{
			PNOCLNNCEBB = value;
		}

		public virtual float get_Opacity()
		{
			return MGPPBIADMJM;
		}

		public virtual void set_Opacity(float value)
		{
			MGPPBIADMJM = value;
		}

		public override void OnPointerClick(PointerEventData BHOLFGOGPCP)
		{
			base.OnPointerClick(BHOLFGOGPCP);
			if (BHOLFGOGPCP.clickCount > 1)
			{
				get_onDoubleClick().Invoke();
			}
		}
	}
}
