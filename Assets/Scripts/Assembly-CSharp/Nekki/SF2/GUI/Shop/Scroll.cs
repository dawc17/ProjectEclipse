using System.Collections.Generic;
using UnityEngine;

namespace Nekki.SF2.GUI.Shop
{
	public class Scroll : MonoBehaviour
	{
		[SerializeField]
		private ItemsScroll _itemsScroll;

		private BaseScrollContent _baseScrollContent;

		public ItemsScroll LCHHABGJMND
		{
			get
			{
				return get_ItemsScroll();
			}
		}

		public BaseScrollContent FAIHAAIMHDD
		{
			get
			{
				return get_BaseScrollContent();
			}
		}

		public ItemsScroll get_ItemsScroll()
		{
			return _itemsScroll;
		}

		public BaseScrollContent get_BaseScrollContent()
		{
			return _baseScrollContent;
		}

		public void Init(BaseScrollContent KHDHFALCAEJ)
		{
			_baseScrollContent = KHDHFALCAEJ;
			if (_itemsScroll != null)
			{
				_baseScrollContent.transform.SetParent(_itemsScroll.transform, false);
				_itemsScroll.scrollContent = _baseScrollContent;
				_itemsScroll.set_content((RectTransform)_baseScrollContent.gameObject.transform);
				_itemsScroll.Init();
			}
		}

		public void AddItem(BaseScrollItem item)
		{
			if (_baseScrollContent != null)
			{
				_baseScrollContent.AddItem(item);
			}
		}

		public void SetItems(List<BaseScrollItem> HELFDCAIJNE)
		{
			if (_baseScrollContent != null)
			{
				_baseScrollContent.SetItems(HELFDCAIJNE);
			}
		}

		public virtual void ClearItems()
		{
			if (_baseScrollContent != null)
			{
				_baseScrollContent.ClearItems();
			}
		}

		public List<BaseScrollItem> GetItems()
		{
			if (_baseScrollContent != null)
			{
				return _baseScrollContent.Items;
			}
			return new List<BaseScrollItem>();
		}

		public void UpdateLayout()
		{
			if (_baseScrollContent != null)
			{
				_baseScrollContent.UpdateLayout();
			}
		}

		public void ScrollToItem(BaseScrollItem item, float _Duration = 0f)
		{
			if (_itemsScroll != null && item != null)
			{
				_itemsScroll.ScrollToItem(item, _Duration);
			}
		}

		public void ScrollToItem(int index, float _Duration = 0f)
		{
			List<BaseScrollItem> items = GetItems();
			if (items.Count > index)
			{
				ScrollToItem(items[index], _Duration);
			}
		}

		public void ScrollToBegin()
		{
			if (_itemsScroll != null && _baseScrollContent != null)
			{
				BaseScrollItem firstItem = _baseScrollContent.FirstItem;
				if (firstItem != null)
				{
					_itemsScroll.ScrollToItem(firstItem, 0f);
				}
			}
		}

		public void ScrollToEnd()
		{
			if (_itemsScroll != null && _baseScrollContent != null)
			{
				BaseScrollItem lastItem = _baseScrollContent.LastItem;
				if (lastItem != null)
				{
					_itemsScroll.ScrollToItem(lastItem, 0f);
				}
			}
		}

		public void StopMovement()
		{
			if (_itemsScroll != null)
			{
				_itemsScroll.StopMovement();
			}
		}

		public int GetCurrentItemIndex()
		{
			if (_baseScrollContent != null && _baseScrollContent.SelectedItem != null)
			{
				List<BaseScrollItem> items = GetItems();
				return items.IndexOf(_baseScrollContent.SelectedItem);
			}
			return 0;
		}

		public BaseScrollItem GetCurrentItem()
		{
			if (_baseScrollContent != null)
			{
				return _baseScrollContent.SelectedItem;
			}
			return null;
		}

		public int GetIndexOfItem(BaseScrollItem item)
		{
			List<BaseScrollItem> items = GetItems();
			return items.IndexOf(item);
		}

		public BaseScrollItem GetItem(int index)
		{
			List<BaseScrollItem> items = GetItems();
			if (index < items.Count)
			{
				return items[index];
			}
			return null;
		}
	}
}
