using System.Collections.Generic;
using Nekki.SF2.GUI.Shop;
using SF2DE.Underworld;
using UnityEngine;

namespace Nekki.SF2.GUI.Map
{
	public class MapPanel : SFMonoBehaviour<object>
	{
		public enum HLEIAGFDMKK
		{
			ZShadow = 0,
			ZMap = 1
		}

		public enum OAHPENLBMKD
		{
			onClickBattle = 0,
			onSelectZone = 1
		}

		private Scroll LJOBLDELNGD;

		[SerializeField]
		private GameObject _baseScrollContentPrefab;

		private BaseScrollContent _baseScrollContent;

		[SerializeField]
		private GameObject _scrollItemPrefab;

		public void Init()
		{
			LJOBLDELNGD = GetComponent<Scroll>();
			if (_baseScrollContentPrefab != null)
			{
				GameObject gameObject = Object.Instantiate(_baseScrollContentPrefab);
				_baseScrollContent = gameObject.GetComponent<BaseScrollContent>();
			}
			IDCFACEODIF();
		}

		private void OnDestroy()
		{
			LJOBLDELNGD.ClearItems();
			_baseScrollContent.onClickItem = null;
			_baseScrollContent.onSelectItem = null;
		}

		~MapPanel()
		{
		}

		public int GetZoneIndex(ZoneScrollItem JEOIJBLAMIO)
		{
			List<ZoneScrollItem> zones = GetZones();
			return zones.IndexOf(JEOIJBLAMIO);
		}

		public int GetZoneIndexByName(string name)
		{
			int num = 0;
			List<ZoneScrollItem> zones = GetZones();
			foreach (ZoneScrollItem item in zones)
			{
				Zone zone = item.get_Zone();
				if (name == zone.get_Name())
				{
					break;
				}
				num++;
			}
			if (num >= zones.Count)
			{
				num = -1;
			}
			return num;
		}

		public void AddStoryZones()
		{
			List<Zone> hFPCBJLOJEM = ListSF.FHAIJEAPFEA().FindAll(
				zone => !UnderworldZonePolicy.IsRaidZone(zone));
			GHCJGLHOFHO(hFPCBJLOJEM);
		}

		public void AddRaidZones()
		{
			List<Zone> raidZones = ListSF.FHAIJEAPFEA().FindAll(UnderworldZonePolicy.IsRaidZone);
			// The legacy roster format never created availability records for the
			// newer Underworld bosses. Treat entries in the dedicated raid document
			// as locally playable; otherwise MapPanel discards every raid page.
			UnderworldZonePolicy.MarkLocallyPlayable(raidZones);
			GHCJGLHOFHO(raidZones);
		}

		public void SetRaidPowerMode(bool enabled)
		{
			foreach (ZoneScrollItem zone in GetZones())
			{
				zone.SetRaidPowerMode(enabled);
			}
		}

		public bool HasBattle(Battle DPOOIONCEOA)
		{
			List<ZoneScrollItem> zones = GetZones();
			foreach (ZoneScrollItem item in zones)
			{
				if ((bool)item.GetButtonByBattle(DPOOIONCEOA))
				{
					return true;
				}
			}
			return false;
		}

		public bool HasZone(ZoneScrollItem HLJKOKMKMLM)
		{
			List<ZoneScrollItem> zones = GetZones();
			foreach (ZoneScrollItem item in zones)
			{
				if (item == HLJKOKMKMLM)
				{
					return true;
				}
			}
			return false;
		}

		public void SelectBattle(Battle DPOOIONCEOA, float _Duration)
		{
			if (DPOOIONCEOA == null)
			{
				return;
			}
			List<ZoneScrollItem> zones = GetZones();
			foreach (ZoneScrollItem item in zones)
			{
				if ((bool)item.GetButtonByBattle(DPOOIONCEOA))
				{
					item.SetLastBattle(DPOOIONCEOA);
					if (LJOBLDELNGD.GetCurrentItem() != item)
					{
						LJOBLDELNGD.ScrollToItem(item, _Duration);
					}
					break;
				}
			}
		}

		public List<ZoneScrollItem> GetZones()
		{
			List<ZoneScrollItem> list = new List<ZoneScrollItem>();
			foreach (BaseScrollItem item2 in LJOBLDELNGD.GetItems())
			{
				ZoneScrollItem item = item2 as ZoneScrollItem;
				list.Add(item);
			}
			return list;
		}

		public ZoneScrollItem GetZone(int index)
		{
			List<ZoneScrollItem> zones = GetZones();
			if (zones != null && zones.Count > index)
			{
				return zones[index];
			}
			return null;
		}

		public int GetCurrentItemIndex()
		{
			return LJOBLDELNGD.GetCurrentItemIndex();
		}

		public ZoneScrollItem GetCurrentZone()
		{
			return LJOBLDELNGD.GetCurrentItem() as ZoneScrollItem;
		}

		public void ScrollToZone(int index, float _Duration)
		{
			LJOBLDELNGD.ScrollToItem(index, _Duration);
		}

		public virtual void SetTouchEnabled(bool value)
		{
			LJOBLDELNGD.enabled = value;
			List<ZoneScrollItem> zones = GetZones();
			foreach (ZoneScrollItem item in zones)
			{
				item.Enabled(value);
			}
		}

		public void Clear()
		{
			LJOBLDELNGD.ClearItems();
		}

		private void IDCFACEODIF()
		{
			if (!(LJOBLDELNGD == null) && !(_baseScrollContent == null))
			{
				LJOBLDELNGD.Init(_baseScrollContent);
				_baseScrollContent.onSelectItem.AddListener(GBFNCCGLEGL);
				LJOBLDELNGD.get_ItemsScroll().set_MinScrollVelocity(500f);
				LJOBLDELNGD.get_ItemsScroll().set_AutoscrollDuration(0.25f);
				LJOBLDELNGD.get_BaseScrollContent().Spacing = 0f;
				LJOBLDELNGD.ScrollToBegin();
			}
		}

		private void DMPHCAKENKH(object data)
		{
			CallEvent(0, data);
		}

		private void GBFNCCGLEGL(object data)
		{
			CallEvent(1, data);
		}

		private void GHCJGLHOFHO(List<Zone> HFPCBJLOJEM)
		{
			foreach (Zone item in HFPCBJLOJEM)
			{
				FMIEDAKDMOH(item);
			}
		}

		private void FMIEDAKDMOH(Zone HLJKOKMKMLM)
		{
			if (LJOBLDELNGD == null || _scrollItemPrefab == null)
			{
				LLLOJBFMONN.Error("MapPanel.AddZone some field is null");
			}
			else
			{
				if (HLJKOKMKMLM.AMBLIADMEOC())
				{
					return;
				}
				bool flag = true;
				List<Battle> lGIIBNJFADA = HLJKOKMKMLM.LGIIBNJFADA;
				for (int i = 0; i < lGIIBNJFADA.Count; i++)
				{
					Battle cGJCGEBPCAF = lGIIBNJFADA[i];
					bool dCHJDPCEODD = cGJCGEBPCAF.DCHJDPCEODD;
					flag &= !dCHJDPCEODD;
				}
				if (!flag)
				{
					GameObject gameObject = Object.Instantiate(_scrollItemPrefab);
					ZoneScrollItem component = gameObject.GetComponent<ZoneScrollItem>();
					if (component != null)
					{
						component.Init(HLJKOKMKMLM);
						component.AddEventListener(0, DMPHCAKENKH);
						component.SelectFirstBattle();
						LJOBLDELNGD.AddItem(component);
					}
					else
					{
						LLLOJBFMONN.Error("ShopScrollContent.SetItems scrollItem is null");
					}
					LJOBLDELNGD.ScrollToBegin();
				}
			}
		}
	}
}
