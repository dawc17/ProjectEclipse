using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Nekki.SF2.GUI
{
	[Serializable]
	public class BaseScrollContent : MonoBehaviour
	{
		[Serializable]
		public class SelectItemEvent : UnityEvent<BaseScrollItem>
		{
		}

		[Serializable]
		public class ClickItemEvent : UnityEvent<BaseScrollItem>
		{
		}

		[SerializeField]
		public SelectItemEvent onSelectItem = new SelectItemEvent();

		[SerializeField]
		public ClickItemEvent onClickItem = new ClickItemEvent();

		[SerializeField]
		private HorizontalOrVerticalLayoutGroup _layout;

		[SerializeField]
		public float Spacing;

		private Vector2 _additionalContentSize = new Vector2(0f, 0f);

		[SerializeField]
		private bool _centering;

		private List<BaseScrollItem> _Items = new List<BaseScrollItem>();

		[SerializeField]
		public int TopSpacing
		{
			get
			{
				return _layout.padding.top;
			}
			set
			{
				_layout.padding.top = value;
			}
		}

		[SerializeField]
		public int BottomSpacing
		{
			get
			{
				return _layout.padding.bottom;
			}
			set
			{
				_layout.padding.bottom = value;
			}
		}

		public Vector2 AdditionalContentSize
		{
			get
			{
				return _additionalContentSize;
			}
			set
			{
				_additionalContentSize = value;
			}
		}

		public bool Centering
		{
			get
			{
				return _centering;
			}
			set
			{
				_centering = value;
			}
		}

		public RectTransform Center { get; set; }

		public List<BaseScrollItem> Items
		{
			get
			{
				return _Items;
			}
		}

		public BaseScrollItem SelectedItem { get; protected set; }

		public RectTransform SelectedItemRect
		{
			get
			{
				return (!(SelectedItem != null)) ? null : (SelectedItem.transform as RectTransform);
			}
		}

		public BaseScrollItem FirstItem
		{
			get
			{
				return (_Items.Count <= 0) ? null : _Items[0];
			}
		}

		public BaseScrollItem LastItem
		{
			get
			{
				return (_Items.Count <= 0) ? null : _Items[_Items.Count - 1];
			}
		}

		private void GEEKMNDAEHK(BaseScrollItem item)
		{
			onClickItem.Invoke(item);
		}

		public void AddItem(BaseScrollItem item)
		{
			if (item == base.gameObject)
			{
				return;
			}
			item.gameObject.transform.SetParent(base.gameObject.transform, false);
			RectTransform rectTransform = (RectTransform)base.gameObject.transform;
			float num = rectTransform.rect.width;
			float num2 = rectTransform.rect.height;
			RectTransform rectTransform2 = (RectTransform)item.gameObject.transform;
			float width = rectTransform2.rect.width;
			float height = rectTransform2.rect.height;
			if (_Items.Count > 0)
			{
				num -= AdditionalContentSize.x;
				num2 -= AdditionalContentSize.y;
			}
			RectTransform rectTransform3 = base.gameObject.transform.parent as RectTransform;
			VerticalLayoutGroup verticalLayoutGroup = _layout as VerticalLayoutGroup;
			if (verticalLayoutGroup != null)
			{
				num2 = ((_Items.Count <= 0) ? ((float)(_layout.padding.top + _layout.padding.bottom)) : (num2 + Spacing));
				num2 += height;
				if (rectTransform3 != null && Centering)
				{
					float num3 = rectTransform3.rect.height / 2f - height / 2f;
				}
			}
			else
			{
				num = ((_Items.Count <= 0) ? ((float)(_layout.padding.left + _layout.padding.right)) : (num + Spacing));
				num += width;
				if (rectTransform3 != null && Centering)
				{
					float num4 = rectTransform3.rect.width / 2f - width / 2f;
				}
			}
			rectTransform.sizeDelta = new Vector2(num, num2) + AdditionalContentSize;
			_Items.Add(item);
			item.onClick.AddListener(() =>
			{
				GEEKMNDAEHK(item);
			});
			UpdateLayout();
		}

		public void RemoveItem(BaseScrollItem item)
		{
			_Items.Remove(item);
			item.onClick.RemoveAllListeners();
		}

		public void ClearItems()
		{
			foreach (BaseScrollItem item in _Items)
			{
				item.onClick.RemoveAllListeners();
			}
			_Items.Clear();
		}

		public void SetItems(List<BaseScrollItem> HELFDCAIJNE)
		{
			if (HELFDCAIJNE == null)
			{
				LLLOJBFMONN.Error("BaseScrollContent.SetItems items is null");
				return;
			}
			Clear();
			foreach (BaseScrollItem item in HELFDCAIJNE)
			{
				AddItem(item);
			}
		}

		public void UpdateLayout()
		{
			if (_layout != null)
			{
				_layout.spacing = Spacing;
				LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_layout.gameObject.transform);
				BMAPAEGMIFO();
			}
		}

		public void Clear()
		{
			if (base.gameObject == null)
			{
				LLLOJBFMONN.Error("BaseScrollContent.Clear gameObject is null");
				return;
			}
			List<GameObject> list = new List<GameObject>();
			foreach (Transform item in base.gameObject.transform)
			{
				item.gameObject.SetActive(false);
				list.Add(item.gameObject);
			}
			list.ForEach((GameObject BFEBLBKODLK) =>
			{
				UnityEngine.Object.Destroy(BFEBLBKODLK);
			});
			ClearItems();
			VerticalLayoutGroup verticalLayoutGroup = _layout as VerticalLayoutGroup;
			RectTransform rectTransform = (RectTransform)base.gameObject.transform;
			if (verticalLayoutGroup != null)
			{
				rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, 0f);
			}
			else
			{
				rectTransform.sizeDelta = new Vector2(0f, rectTransform.rect.height);
			}
		}

		public float GetDistanceToCenter(BaseScrollItem item)
		{
			if (item == null)
			{
				return 0f;
			}
			return Vector2.Distance(Center.position, item.transform.position);
		}

		public virtual void Update()
		{
			BMAPAEGMIFO();
		}

		private void BMAPAEGMIFO()
		{
			if (Center != null)
			{
				BaseScrollItem nearestItem = GetNearestItem();
				if (nearestItem != SelectedItem)
				{
					SelectedItem = nearestItem;
					onSelectItem.Invoke(SelectedItem);
				}
			}
		}

		public BaseScrollItem GetNearestItem(float GHGLPGGMDNP = 0f)
		{
			VerticalLayoutGroup verticalLayoutGroup = _layout as VerticalLayoutGroup;
			float num = Center.position.x;
			float num2 = Center.position.y;
			if (verticalLayoutGroup != null)
			{
				num2 += GHGLPGGMDNP;
			}
			else
			{
				num += GHGLPGGMDNP;
			}
			Vector2 a = new Vector2(num, num2);
			BaseScrollItem result = null;
			float num3 = float.MaxValue;
			foreach (BaseScrollItem item in _Items)
			{
				float num4 = Vector2.Distance(a, item.transform.position);
				if (num4 < num3)
				{
					num3 = num4;
					result = item;
				}
			}
			return result;
		}
	}
}
