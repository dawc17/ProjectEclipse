using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI
{
	public class HintBox : MonoBehaviour
	{
		[SerializeField]
		private ResolutionImage arrow;

		[SerializeField]
		private LabelAlias header;

		[SerializeField]
		private LabelAlias description;

		[SerializeField]
		private RectTransform backgroudTransform;

		[SerializeField]
		private VerticalLayoutGroup backgroudLayout;

		private bool JKJOAIKAMOH;

		private RectTransform IOAEAJAEOFK;

		public RectTransform DIBDBBCPEGN
		{
			get
			{
				return get_RectTransform();
			}
		}

		public RectTransform get_RectTransform()
		{
			if (IOAEAJAEOFK == null)
			{
				IOAEAJAEOFK = GetComponent<RectTransform>();
			}
			return IOAEAJAEOFK;
		}

		public void Init()
		{
			if (backgroudLayout != null)
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)backgroudLayout.gameObject.transform);
			}
		}

		public void SetText(string BHJILACALPJ, string LNGIMAAHIFE)
		{
			if (!(header == null) && !(description == null) && !(backgroudTransform == null) && !(backgroudLayout == null))
			{
				header.SetAlias(BHJILACALPJ);
				description.SetAlias(LNGIMAAHIFE);
				ResizeToContent();
			}
		}

		// Composed lists are already localized; preserve explicit inline icon sizes.
		public void SetListContent(string titleAlias, string content)
		{
			header.SetAlias(titleAlias);
			description.SetAlias(string.Empty);
			description.text = content;
			ResizeToContent();
		}

		private void ResizeToContent()
		{
			// Establish the available text width before measuring wrapped content.
			LayoutRebuilder.ForceRebuildLayoutImmediate(backgroudTransform);
			float height = header.preferredHeight + description.preferredHeight
				+ Mathf.Abs(backgroudTransform.offsetMax.y) + backgroudLayout.spacing
				+ backgroudLayout.padding.top + backgroudLayout.padding.bottom;
			RectTransform rect = get_RectTransform();
			rect.sizeDelta = new Vector2(rect.rect.width, height);
			LayoutRebuilder.ForceRebuildLayoutImmediate(backgroudTransform);
		}

		// Eclipse: keeps the arrow on the icon when the box is shifted to stay on screen.
		public void SetArrowOffset(float offset, float limit)
		{
			if (arrow != null)
			{
				RectTransform rectTransform = arrow.rectTransform;
				float x = (limit > 0f) ? Mathf.Clamp(offset, -limit, limit) : 0f;
				rectTransform.anchoredPosition = new Vector2(x, rectTransform.anchoredPosition.y);
			}
		}

		public void Flip()
		{
			if (!JKJOAIKAMOH)
			{
				JKJOAIKAMOH = true;
				Vector3 eulerAngles = new Vector3(0f, 0f, 180f);
				Vector3 eulerAngles2 = new Vector3(180f, 180f, 0f);
				base.transform.Rotate(eulerAngles);
				if (arrow != null)
				{
					arrow.transform.SetSiblingIndex(0);
				}
				if (header != null)
				{
					header.transform.Rotate(eulerAngles2);
					header.transform.SetSiblingIndex(2);
				}
				if (description != null)
				{
					description.transform.Rotate(eulerAngles2);
					description.transform.SetSiblingIndex(1);
				}
			}
		}

		public void ResetFlip()
		{
			if (JKJOAIKAMOH)
			{
				JKJOAIKAMOH = false;
				Vector3 eulerAngles = new Vector3(0f, 0f, -180f);
				Vector3 eulerAngles2 = new Vector3(-180f, -180f, 0f);
				base.transform.Rotate(eulerAngles);
				if (arrow != null)
				{
					arrow.transform.SetSiblingIndex(0);
				}
				if (header != null)
				{
					header.transform.Rotate(eulerAngles2);
					header.transform.SetSiblingIndex(1);
				}
				if (description != null)
				{
					description.transform.Rotate(eulerAngles2);
					description.transform.SetSiblingIndex(2);
				}
			}
		}
	}
}
