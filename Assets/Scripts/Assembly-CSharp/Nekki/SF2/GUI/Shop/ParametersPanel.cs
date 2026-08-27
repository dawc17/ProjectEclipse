using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Shop
{
	public class ParametersPanel : MonoBehaviour
	{
		[SerializeField]
		private GameObject _itemPrefab;

		[SerializeField]
		private VerticalLayoutGroup _verticalLayoutGroup;

		[SerializeField]
		private LayoutElement _layoutElement;

		private List<ParameterScrollItem> _items = new List<ParameterScrollItem>();

		public void Clear()
		{
			foreach (ParameterScrollItem item in _items)
			{
				item.gameObject.SetActive(false);
				Object.Destroy(item.gameObject);
			}
			_items.Clear();
		}

		public void SetParameters(ItemInfo item, ItemInfo FJIENDKAIDO, bool OGMLCLNEAIJ)
		{
			Clear();
			List<WarriorAttribute> iBLHIAHECLK = GameUtils.BGENALLCKII.IBLHIAHECLK;
			float num = 0f;
			foreach (WarriorAttribute warriorItem in iBLHIAHECLK)
			{
				int OEMALIFPGPO = 0;
				if (item == null || !item.IBLHIAHECLK.Get(warriorItem.get_Name(), ref OEMALIFPGPO) || warriorItem.GDCBBAHKCIE || warriorItem.GDECIAJAFHH)
				{
					continue;
				}
				int OEMALIFPGPO2 = OEMALIFPGPO;
				if (FJIENDKAIDO != null && OGMLCLNEAIJ)
				{
					FJIENDKAIDO.IBLHIAHECLK.Get(warriorItem.get_Name(), ref OEMALIFPGPO2);
				}
				if (_itemPrefab != null)
				{
					GameObject gameObject = Object.Instantiate(_itemPrefab);
					gameObject.transform.SetParent(base.gameObject.transform, false);
					ParameterScrollItem component = gameObject.GetComponent<ParameterScrollItem>();
					if (component != null)
					{
						bool eIAKNKDEEKA = true;
						component.Init(warriorItem.get_Name(), warriorItem.MJBPMLCLMFN, OEMALIFPGPO, OEMALIFPGPO2, eIAKNKDEEKA);
						num += component.get_MinHeight();
						_items.Add(component);
					}
				}
			}
			if (_verticalLayoutGroup != null && _items.Count > 0)
			{
				num += (float)(_items.Count - 1) * _verticalLayoutGroup.spacing;
			}
			if (_layoutElement != null)
			{
				_layoutElement.minHeight = num;
			}
		}

		public void SetParameters(ItemInfo item, bool OGMLCLNEAIJ)
		{
			SetParameters(item, null, OGMLCLNEAIJ);
		}
	}
}
