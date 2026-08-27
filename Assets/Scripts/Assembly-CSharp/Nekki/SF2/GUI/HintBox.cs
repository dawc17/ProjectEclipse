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
				RectTransform rectTransform = (RectTransform)base.transform;
				RectTransform rectTransform2 = (RectTransform)header.transform;
				RectTransform rectTransform3 = (RectTransform)description.transform;
				float preferredHeight = header.preferredHeight;
				float preferredHeight2 = description.preferredHeight;
				float num = Mathf.Abs(backgroudTransform.offsetMax.y);
				float num2 = preferredHeight + preferredHeight2;
				num2 += num;
				num2 += backgroudLayout.spacing;
				num2 += (float)backgroudLayout.padding.top;
				num2 += (float)backgroudLayout.padding.bottom;
				rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, num2);
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
