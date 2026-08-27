using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Map
{
	public class MapContainer : SFMonoBehaviour<object>
	{
		public enum KEIEKJLIOFN
		{
			onBattleClick = 0,
			onZoneSelect = 1,
			onOpened = 2,
			onClosed = 3
		}

		public static float LAMP_FRAMES_PER_ZONE = 20f;

		public static float LAMP_MAX_FRAMES = 50f;

		[SerializeField]
		private MoveableMapPanel _moveableMapPanel;

		[SerializeField]
		private MapPanel _mapPanel;

		[SerializeField]
		private LampsPanel _lampsPanel;

		public void Init()
		{
			_mapPanel.Init();
			_mapPanel.AddEventListener(0, AOGHKADFFAK);
			_mapPanel.AddEventListener(1, GBFNCCGLEGL);
			_lampsPanel.Init();
			_lampsPanel.AddEventListener(0, HLLNHKEKCBP);
		}

		private void OnDestroy()
		{
			_mapPanel.RemoveEventListener(0, AOGHKADFFAK);
			_mapPanel.RemoveEventListener(1, GBFNCCGLEGL);
			_mapPanel = null;
			_lampsPanel.RemoveEventListener(0, HLLNHKEKCBP);
		}

		public void Open()
		{
			_moveableMapPanel.Open();
		}

		public void Close()
		{
			_moveableMapPanel.Close();
		}

		public void OpenRightNow()
		{
			_moveableMapPanel.OpenRightNow();
		}

		public void CloseRightNow()
		{
			_moveableMapPanel.CloseRightNow();
		}

		public int GetZoneIndex(ZoneScrollItem JEOIJBLAMIO)
		{
			return _mapPanel.GetZoneIndex(JEOIJBLAMIO);
		}

		public int GetZoneIndexByName(string name)
		{
			return _mapPanel.GetZoneIndexByName(name);
		}

		public void SelectBattle(Battle DPOOIONCEOA, float _Duration)
		{
			_mapPanel.SelectBattle(DPOOIONCEOA, _Duration);
		}

		public void AddStoryZones()
		{
			_mapPanel.AddStoryZones();
			RefreshLamps();
		}

		public void AddRaidZones()
		{
			_mapPanel.AddRaidZones();
			RefreshLamps(false);
		}

		private void RefreshLamps(bool checkOpenZones = true)
		{
			List<ZoneScrollItem> zones = _mapPanel.GetZones();
			_lampsPanel.ClearLamps();
			_lampsPanel.AddLamps(zones.Count);
			if (checkOpenZones)
			{
				_lampsPanel.CheckOpenZones(zones);
			}
		}

		public void SetRaidPowerMode(bool enabled)
		{
			_mapPanel.SetRaidPowerMode(enabled);
		}

		public void Clear()
		{
			_mapPanel.Clear();
		}

		public List<Button> GetLamps()
		{
			return _lampsPanel.GetLamps();
		}

		public LampsPanel GetLampsPanel()
		{
			return _lampsPanel;
		}

		public ZoneScrollItem GetZone(int index)
		{
			return _mapPanel.GetZone(index);
		}

		public void SetCurrentZone(int index, string ABJMDKJHJCP)
		{
			_lampsPanel.SetCurrentZone(index, ABJMDKJHJCP);
		}

		public void ScrollToZone(int index, float _Duration)
		{
			_mapPanel.ScrollToZone(index, _Duration);
		}

		public void Flashing()
		{
			_lampsPanel.Flashing();
		}

		public int GetCurrentItemIndex()
		{
			return _mapPanel.GetCurrentItemIndex();
		}

		public ZoneScrollItem GetCurrentZone()
		{
			return _mapPanel.GetCurrentZone();
		}

		public int GetZonesCount()
		{
			return _mapPanel.GetZones().Count;
		}

		public void SetLampsEnable(bool IJHFJPBBNEJ)
		{
			_lampsPanel.gameObject.SetActive(IJHFJPBBNEJ);
		}

		public bool HasBattle(Battle DPOOIONCEOA)
		{
			return _mapPanel.HasBattle(DPOOIONCEOA);
		}

		public bool HasZone(ZoneScrollItem HLJKOKMKMLM)
		{
			return _mapPanel.HasZone(HLJKOKMKMLM);
		}

		public void SetZonesBackgroundMask(Color color)
		{
			List<ZoneScrollItem> zones = _mapPanel.GetZones();
			foreach (ZoneScrollItem item in zones)
			{
				item.SetBackgroundColor(color);
			}
		}

		public virtual void SetTouchEnabled(bool MINKNLEJMKF)
		{
			_mapPanel.SetTouchEnabled(MINKNLEJMKF);
		}

		public bool IsMoveNow()
		{
			return false;
		}

		private void AOGHKADFFAK(object data)
		{
			CallEvent(0, data);
		}

		private void GBFNCCGLEGL(object data)
		{
			CallEvent(1, data);
		}

		private void CBAFDCAEGNH(object data)
		{
			CallEvent(2, data);
		}

		private void BMALNCHKHME(object data)
		{
			CallEvent(3, data);
		}

		private void HLLNHKEKCBP(object data)
		{
			int num = (int)data;
			int num2 = (int)Mathf.Min(LAMP_MAX_FRAMES, (float)Mathf.Abs(_lampsPanel.GetCurrentLamp() - num) * LAMP_FRAMES_PER_ZONE);
			float dFNBHOEGAHO = (float)num2 / 60f;
			int count = _mapPanel.GetZones().Count;
			if (num >= 0 && num < count)
			{
				ZoneScrollItem zone = GetZone(num);
				ScrollToZone(num, dFNBHOEGAHO);
				Zone zone2 = zone.get_Zone();
				if (zone2 != null && MapScene.IsZoneOpen(zone2) && MapScene.IsZoneHaveDontCompleteBattle(zone2))
				{
					MOPMPLKDEBD(zone2, zone);
				}
			}
		}

		private void MOPMPLKDEBD(Zone HLJKOKMKMLM, ZoneScrollItem ELOKNHJDCCD)
		{
			List<string> gBDHOPBMLHK = MapGUI.JHLMDGBGGEP.GBDHOPBMLHK;
			List<Battle> lGIIBNJFADA = HLJKOKMKMLM.LGIIBNJFADA;
			foreach (string item in gBDHOPBMLHK)
			{
				foreach (Battle item2 in lGIIBNJFADA)
				{
					if (DCDNICEMCCO(item, item2))
					{
						ELOKNHJDCCD.SetLastBattle(item);
						return;
					}
				}
			}
		}

		private static bool DCDNICEMCCO(string name, Battle DPOOIONCEOA)
		{
			if (DPOOIONCEOA == null)
			{
				return false;
			}
			bool flag = name == DPOOIONCEOA.get_Name();
			bool flag2 = DPOOIONCEOA.MNHLGELMOEJ() == ConditionStatus.StatusOpen && !DPOOIONCEOA.BACJPLBBCKL();
			return flag && flag2;
		}
	}
}
