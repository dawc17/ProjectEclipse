using System.Collections.Generic;
using UnityEngine;

namespace Nekki.SF2.GUI.Profile
{
	public class ProfileSliderItem : BaseScrollItem
	{
		public enum LLJHOHPLLME
		{
			NONE_ALIGNMENT = 0,
			LEFT_ALIGNMENT = 1
		}

		private List<SubItem> BNIBNKHLLIC = new List<SubItem>();

		private float NDKEJKELOHF = 95f;

		[SerializeField]
		private PerkTreeLines _perkLines;

		public void Init(float PEIAPNNLFFL = 95f)
		{
			NDKEJKELOHF = PEIAPNNLFFL;
			_perkLines.gameObject.SetActive(false);
		}

		public void AddIcons(SubItem ADONPNOBBDE, LLJHOHPLLME LJFADBBKKPH = LLJHOHPLLME.NONE_ALIGNMENT)
		{
			if (ADONPNOBBDE != null)
			{
				GAIHDBGNEFA(ADONPNOBBDE);
				switch (LJFADBBKKPH)
				{
				case LLJHOHPLLME.NONE_ALIGNMENT:
					OELPCLPNGGF();
					break;
				case LLJHOHPLLME.LEFT_ALIGNMENT:
					ADONPNOBBDE.transform.OKHPLHPBPKJ(60f - GetComponent<RectTransform>().rect.width / 2f);
					break;
				}
			}
		}

		public void AddIcons(List<SubItem> BAOPCLKCLAF)
		{
			int count = BAOPCLKCLAF.Count;
			for (int i = 0; i < count; i++)
			{
				GAIHDBGNEFA(BAOPCLKCLAF[i]);
			}
			OELPCLPNGGF();
		}

		public List<SubItem> GetIcons()
		{
			return BNIBNKHLLIC;
		}

		public bool IsUnlokedItem()
		{
			bool result = false;
			for (int i = 0; i < BNIBNKHLLIC.Count; i++)
			{
				if (!BNIBNKHLLIC[i].GetLock())
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public virtual void UpdateState()
		{
			foreach (SubItem item in BNIBNKHLLIC)
			{
				item.UpdateState();
			}
		}

		public void Clear()
		{
			foreach (SubItem item in BNIBNKHLLIC)
			{
				item.ParentCell = null;
				Object.Destroy(item.gameObject);
			}
			BNIBNKHLLIC.Clear();
		}

		private void GAIHDBGNEFA(SubItem ADONPNOBBDE)
		{
			ADONPNOBBDE.transform.BGNJGIACJBG(0f);
			BNIBNKHLLIC.Add(ADONPNOBBDE);
		}

		private void OELPCLPNGGF()
		{
			int count = BNIBNKHLLIC.Count;
			switch (count)
			{
			case 0:
				break;
			case 2:
			{
				SubItem subItem = BNIBNKHLLIC[0];
				float num2 = subItem.GetComponent<RectTransform>().rect.width / 2f + NDKEJKELOHF;
				subItem.transform.OKHPLHPBPKJ(0f - num2);
				BNIBNKHLLIC[1].transform.OKHPLHPBPKJ(num2);
				break;
			}
			default:
			{
				float num = (0f - GetComponent<RectTransform>().rect.width) / 2f + GetComponent<RectTransform>().rect.width / 2f / (float)count;
				for (int i = 0; i < count; i++)
				{
					BNIBNKHLLIC[i].transform.OKHPLHPBPKJ(num + GetComponent<RectTransform>().rect.width / (float)count * (float)i);
				}
				break;
			}
			}
		}

		public void AddPerkLines(bool NMBEADHHHFH, bool IBMGAPMHMOB)
		{
			_perkLines.gameObject.SetActive(true);
			int count = GetIcons().Count;
			_perkLines.Init(count >= 2, !NMBEADHHHFH, !IBMGAPMHMOB);
		}
	}
}
