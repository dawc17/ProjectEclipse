using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI
{
	[AddComponentMenu("UI_Nekki/ResolutionImageLE")]
	[RequireComponent(typeof(LayoutElement))]
	public class ResolutionImageLE : ResolutionImage
	{
		[SerializeField]
		private LayoutElement layoutElement;

		public LayoutElement CMFIABIFDDD
		{
			get
			{
				return get_LayoutElement();
			}
			set
			{
				set_LayoutElement(value);
			}
		}

		public LayoutElement get_LayoutElement()
		{
			return layoutElement;
		}

		public void set_LayoutElement(LayoutElement value)
		{
			layoutElement = value;
		}

		public override void SetNativeSize()
		{
			base.SetNativeSize();
			if (layoutElement == null)
			{
				layoutElement = base.gameObject.GetComponent<LayoutElement>();
			}
			if (layoutElement != null)
			{
				layoutElement.minWidth = base.rectTransform.rect.width;
				layoutElement.minHeight = base.rectTransform.rect.height;
			}
		}
	}
}
