using System.Collections.Generic;
using Nekki.SF2.GUI.Menu;
using SF2DE.Underworld;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Map
{
	public class MapScene : Scene<MapScene>
	{
		public enum NMFLNANKNOJ
		{
			StoryMode = 0,
			RaidMode = 1
		}

		public enum HGANCAGOEDN
		{
			ON_STORY_MAP_OPENED = 0,
			ON_STORY_MAP_CLOSED = 1,
			ON_RAID_MAP_RELOADED = 2
		}

		public class LastFight
		{
			private FightIDS FMOAFHBHOJD;

			public LastFight(FightIDS JFIIJBAOOIK)
			{
				FMOAFHBHOJD = JFIIJBAOOIK;
			}

			public FightIDS GPFKAGCNOMB()
			{
				return FMOAFHBHOJD;
			}

			public FightList DPNBOEMNCMJ()
			{
				return (AODOOCLOLMH() == null) ? null : AODOOCLOLMH().LPHHPIJLJBM(FMOAFHBHOJD.EJPNIFANKDG());
			}

			public Battle AODOOCLOLMH()
			{
				return (FLKKAJBLHIL() == null) ? null : FLKKAJBLHIL().MJINKOFNIAE(FMOAFHBHOJD.CPHDPCAECJN());
			}

			public Zone FLKKAJBLHIL()
			{
				return ListSF.CFEDCFACBLE(FMOAFHBHOJD.PELHCAEAOFE());
			}
		}

		private enum KPCEHBECAEN
		{
			ZUnderRaidMap = 0,
			ZRaidMap = 1,
			ZOverRaidMap = 2,
			ZStoryMap = 3,
			ZLampsPanel = 4,
			ZRaidToggleBtn = 5,
			ZInfoBattle = 6,
			ZButtons = 7,
			ZKeys = 8
		}

		private enum IOBFCNOBFHO
		{
			TOUCH_BATTLE_INFO = -128,
			TOUCH_MAP_PANEL = -124
		}

		private enum JFKHCOJJKGO
		{
			BUTTON_SKIP_ZONE = 100,
			BUTTON_RAID_CHEAT = 101
		}

		[SerializeField]
		private MainMenu _mainMenu;

		public const float STARTER_PACK_X = 70f;

		public const float STARTER_PACK_Y = 150f;

		public const float STARTER_PACK_TIMER_FONT_SIZE = 62f;

		public const float STARTER_PACK_TIMER_OFFSET_X = 21f;

		public const float STARTER_PACK_TIMER_OFFSET_Y = 8f;

		public const string STARTER_PACK_IMAGE = "textures/buttons/map/timer.png";

		[SerializeField]
		private MapContainer _storyContainer;

		[SerializeField]
		private InfoBattle _infoBattle;

		[SerializeField]
		private MapButtonsPanel _mapButtonsPanel;

		private ZoneScrollItem OFKGMKADHBD;

		private LastFight JOBMMLPAKBF;

		private LastFight JBAPLBALJII;

		private Sprite HANGGGFOGEJ;

		private NMFLNANKNOJ LDOJANLOFHI;

		private bool _raidPowerMode;

		private UnderworldMapControls _underworldControls;

		private UnderworldMapControls GetUnderworldControls()
		{
			if (_underworldControls == null)
			{
				_underworldControls = new UnderworldMapControls(_storyContainer.transform.parent, _infoBattle,
					ToggleRaidMap, ScrollRaidMapDown, ToggleRaidPowerMode);
			}
			return _underworldControls;
		}

		public override ScreenType PNAJHDBDDLP
		{
			get
			{
				return get_SceneId();
			}
		}

		public override ScreenType get_SceneId()
		{
			return ScreenType.ModuleMap;
		}

		protected override void Init(object data)
		{
			base.Init(data);
			if (!SoundController.IsBackgroundMusicIntro)
			{
				SoundController.KHPHDKFDCLL();
			}
			_mainMenu.Init();
			if (_mapButtonsPanel != null)
			{
				_mapButtonsPanel.Init();
			}
			LEEBPAIKMDP();
			LJGJEKBOPEN();
			SetStoryZonesBackgroundMask(ListSF.CCDKHLAMKKO().EPEDEDLCAJF());
			ListSF.CCDKHLAMKKO().AddEventListener(4, NKEBPJIHFHM);
			LPMGMNCGLOJ();
			if (0 == 0)
			{
				LDOJANLOFHI = NMFLNANKNOJ.StoryMode;
				_storyContainer.OpenRightNow();
				KBGJMMPBDGG(JOBMMLPAKBF);
			}
			UpdateCurrentZone();
			IOHMLGLJELB();
			GetUnderworldControls().Initialize(LDOJANLOFHI == NMFLNANKNOJ.RaidMode);
			UpdateRaidControls();
			PLJBFIGOFPJ();
		}

		protected override void OnDestroy()
		{
			if (ListSF.CCDKHLAMKKO() != null)
			{
				ListSF.CCDKHLAMKKO().GGGEHAGCLGC(true);
			}
			base.OnDestroy();
			ListSF.CCDKHLAMKKO().RemoveEventListener(4, NKEBPJIHFHM);
			_storyContainer.RemoveEventListener(0, AOGHKADFFAK);
			_storyContainer.RemoveEventListener(1, JIINGLBGKOA);
		}

		public void UpdateInfoBattle()
		{
			_infoBattle.Refresh();
		}

		public void UpdateBattleButtonHidden(Battle DPOOIONCEOA)
		{
			RosterBattle dDNLCGOPAGC = DPOOIONCEOA.NNPNEABKHPP();
			if (dDNLCGOPAGC == null)
			{
				return;
			}
			Zone pKCPOJKLMOK = DPOOIONCEOA.LKDFFCADHNO();
			if (pKCPOJKLMOK != null)
			{
				ZoneScrollItem zoneScrollItem = JCBDLFBBLFO(pKCPOJKLMOK);
				if (null != zoneScrollItem)
				{
					zoneScrollItem.UpdateBattleHidden(DPOOIONCEOA);
				}
			}
		}

		public void UpdateCurrentZone()
		{
			if ((bool)OFKGMKADHBD)
			{
				OFKGMKADHBD.Enabled(false);
				OFKGMKADHBD = null;
			}
			MapContainer mapContainer = _storyContainer;
			if (!(mapContainer == null))
			{
				ZoneScrollItem currentZone = mapContainer.GetCurrentZone();
				if (!(currentZone == null))
				{
					OFKGMKADHBD = currentZone;
					OFKGMKADHBD.Enabled(true);
				}
			}
		}

		public void ReloadZones()
		{
			_storyContainer.Clear();
			if (LDOJANLOFHI == NMFLNANKNOJ.RaidMode)
			{
				_storyContainer.AddRaidZones();
				_storyContainer.SetRaidPowerMode(_raidPowerMode);
			}
			else
			{
				_storyContainer.AddStoryZones();
			}
			OFKGMKADHBD = null;
			KJKKKGCLFBB();
			LPMGMNCGLOJ();
			KBGJMMPBDGG((LDOJANLOFHI == NMFLNANKNOJ.RaidMode) ? JBAPLBALJII : JOBMMLPAKBF);
			UpdateCurrentZone();
			IOHMLGLJELB();
			GetUnderworldControls().UpdateToggleSprite(LDOJANLOFHI == NMFLNANKNOJ.RaidMode);
		}

		public InfoBattle GetInfoBattle()
		{
			return _infoBattle;
		}

		public LabelButton GetBtnFight()
		{
			return _infoBattle.GetBtnFight();
		}

		public Button GetMapButtonsByIndex(int index)
		{
			return null;
		}

		public ZoneScrollItem GetCurrentZone()
		{
			return OFKGMKADHBD;
		}

		public NMFLNANKNOJ GetCurrentState()
		{
			return LDOJANLOFHI;
		}

		public void SetStoryZonesBackgroundMask(Color color)
		{
			_storyContainer.SetZonesBackgroundMask(color);
		}

		public void SelectFight(string IGGFGLLIGCG, int frames = 0)
		{
			FightIDS mOCEDDJOAEB = new FightIDS();
			mOCEDDJOAEB.SetFightIDSByString(IGGFGLLIGCG);
			FightList cPAOKGPGHEH = ListSF.CHMCKGCDGCM(mOCEDDJOAEB);
			SelectFight(cPAOKGPGHEH, frames);
		}

		public void SelectFight(FightList fight, float _Duration = 0f)
		{
			if (fight != null && fight.CNAOMDMIGLJ != null)
			{
				SelectBattle(fight.CNAOMDMIGLJ, _Duration);
			}
		}

		public void SelectBattle(Battle DPOOIONCEOA, float _Duration)
		{
			if (DPOOIONCEOA == null)
			{
				return;
			}
			MapContainer mapContainer = null;
			if (_storyContainer.HasBattle(DPOOIONCEOA))
			{
				mapContainer = _storyContainer;
			}
			if ((bool)mapContainer)
			{
				mapContainer.SelectBattle(DPOOIONCEOA, _Duration);
				UpdateCurrentZone();
				IOHMLGLJELB();
			}
		}

		public void SelectZone(MapContainer FJANDPPJDIH, int IGNBCKPAENN, float _Duration)
		{
			if (!(FJANDPPJDIH == null))
			{
				ZoneScrollItem zone = FJANDPPJDIH.GetZone(IGNBCKPAENN);
				Battle lastBattle = zone.get_LastBattle();
				SelectBattle(lastBattle, _Duration);
			}
		}

		public void GotoZoneByName(string name, int frames = 0)
		{
			int zoneIndexByName = _storyContainer.GetZoneIndexByName(name);
			if (zoneIndexByName >= 0)
			{
				SelectZone(_storyContainer, zoneIndexByName, frames);
			}
		}

		public void ActiveBattleByFightIDS(FightIDS DIAIIPCBMFL, bool PEJELKNFEKJ, bool HCNBLJBAOHK = true, bool DPFMIACNGLL = false)
		{
			ZoneScrollItem zoneScrollItem = GIHCEFGHAEO(DIAIIPCBMFL.PELHCAEAOFE());
			if ((bool)zoneScrollItem)
			{
				zoneScrollItem.ActiveBattle(DIAIIPCBMFL.CPHDPCAECJN(), PEJELKNFEKJ, HCNBLJBAOHK, DPFMIACNGLL);
			}
		}

		public static bool IsZoneHaveDontCompleteBattle(Zone HLJKOKMKMLM)
		{
			List<string> gBDHOPBMLHK = MapGUI.JHLMDGBGGEP.GBDHOPBMLHK;
			for (int i = 0; i < gBDHOPBMLHK.Count; i++)
			{
				Battle cGJCGEBPCAF = HLJKOKMKMLM.LGIIBNJFADA.Find(battle => battle.get_Name() == gBDHOPBMLHK[i]);
				if (cGJCGEBPCAF != null)
				{
					bool flag = cGJCGEBPCAF.MNHLGELMOEJ() == ConditionStatus.StatusOpen;
					bool flag2 = !cGJCGEBPCAF.BACJPLBBCKL();
					bool dCHJDPCEODD = cGJCGEBPCAF.DCHJDPCEODD;
					if (flag && flag2 && dCHJDPCEODD)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool IsZoneOpen(Zone HLJKOKMKMLM)
		{
			if (HLJKOKMKMLM == null)
			{
				return false;
			}
			if (HLJKOKMKMLM.get_Name().StartsWith("ZONE_RAID", System.StringComparison.OrdinalIgnoreCase))
				return HLJKOKMKMLM.LGIIBNJFADA.Exists(battle => battle.DCHJDPCEODD);
			List<Battle> list = HLJKOKMKMLM.LGIIBNJFADA.FindAll(battle =>
				battle.get_Type() == BattleType.FightBosses || battle.get_Type() == BattleType.FightFinalTitan ||
				battle.get_Type() == BattleType.FightBossesIntermission);
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].DCHJDPCEODD)
				{
					return true;
				}
			}
			return false;
		}

		public void EnableMapButtons(bool IJHFJPBBNEJ)
		{
			if (!IJHFJPBBNEJ)
			{
				CGMPKDGFIAG(false);
				AFFOJJHOALG(false);
			}
			else if (LDOJANLOFHI == NMFLNANKNOJ.StoryMode)
			{
				CGMPKDGFIAG(true);
				AFFOJJHOALG(false);
			}
			else
			{
				CGMPKDGFIAG(false);
				AFFOJJHOALG(true);
			}
		}

		public void ScrollToItemByName(SliderType _sliderType)
		{
			if (_sliderType != SliderType.SliderRaidMap && _sliderType == SliderType.SliderStoryMap)
			{
				KJKKKGCLFBB();
			}
		}

		private void LJGJEKBOPEN()
		{
			_storyContainer.Init();
			_storyContainer.AddStoryZones();
			_storyContainer.AddEventListener(0, AOGHKADFFAK);
			_storyContainer.AddEventListener(1, JIINGLBGKOA);
		}

		private void LEEBPAIKMDP()
		{
			_infoBattle.Init();
		}

		private void GFLGNFFGOED(Battle DPOOIONCEOA)
		{
			if (!(_infoBattle != null))
			{
				return;
			}
			_infoBattle.UpdateBattleInfo(DPOOIONCEOA);
			FightList currentFight = _infoBattle.GetCurrentFight();
			if (currentFight != null)
			{
				string jFIIJBAOOIK = currentFight.BCKFACGMOKC.ToString();
				if (LDOJANLOFHI != NMFLNANKNOJ.RaidMode)
				{
					ListSF.CCDKHLAMKKO().NDFLHPGHKMP(jFIIJBAOOIK);
				}
				else
				{
					ListSF.CCDKHLAMKKO().EOPPBJKPKGD(jFIIJBAOOIK);
				}
				return;
			}
			FightList jDIPBIHBGPF = GameUtils.JGDLLEAGBBD(DPOOIONCEOA);
			if (jDIPBIHBGPF != null)
			{
				string jFIIJBAOOIK2 = jDIPBIHBGPF.BCKFACGMOKC.ToString();
				if (LDOJANLOFHI != NMFLNANKNOJ.RaidMode)
				{
					ListSF.CCDKHLAMKKO().NDFLHPGHKMP(jFIIJBAOOIK2);
				}
				else
				{
					ListSF.CCDKHLAMKKO().EOPPBJKPKGD(jFIIJBAOOIK2);
				}
			}
		}

		private void IOHMLGLJELB()
		{
			if (!(OFKGMKADHBD == null))
			{
				MapContainer mapContainer = _storyContainer;
				bool flag = LDOJANLOFHI == NMFLNANKNOJ.StoryMode;
				if (!(mapContainer == null))
				{
					OFKGMKADHBD.SelectBattle();
					string aBJMDKJHJCP = OFKGMKADHBD.get_Zone().get_Name();
					int currentItemIndex = mapContainer.GetCurrentItemIndex();
					mapContainer.SetCurrentZone(currentItemIndex, aBJMDKJHJCP);
					mapContainer.GetLampsPanel().SetLampsVisible(true);
				}
			}
		}

		private void LPMGMNCGLOJ()
		{
			JOBMMLPAKBF = null;
			JBAPLBALJII = null;
			JOBMMLPAKBF = new LastFight(ListSF.CCDKHLAMKKO().KNJNHKDCINB());
			JBAPLBALJII = new LastFight(ListSF.CCDKHLAMKKO().MGICKOOCNAJ());
		}

		private void PLJBFIGOFPJ()
		{
		}

		private void ScrollRaidMapDown()
		{
			int count = _storyContainer.GetZonesCount();
			if (count <= 0)
			{
				return;
			}
			int next = (_storyContainer.GetCurrentItemIndex() + 1) % count;
			_storyContainer.ScrollToZone(next, 0.25f);
		}

		private void ToggleRaidPowerMode()
		{
			_raidPowerMode = !_raidPowerMode;
			_storyContainer.SetRaidPowerMode(_raidPowerMode);
			UpdateCurrentZone();
			IOHMLGLJELB();
			UpdateRaidControls();
			Debug.Log("[Underworld] Power Mode " + (_raidPowerMode ? "enabled" : "disabled"));
		}

		private void UpdateRaidControls()
		{
			bool raid = LDOJANLOFHI == NMFLNANKNOJ.RaidMode;
			GetUnderworldControls().UpdateState(raid, _raidPowerMode);
			if (_mapButtonsPanel != null)
			{
				_mapButtonsPanel.SetStoryButtonsVisible(!raid);
			}
		}

		public void SetRaidToggleVisible(bool visible)
		{
			GetUnderworldControls().SetToggleVisible(visible, LDOJANLOFHI == NMFLNANKNOJ.RaidMode);
		}

		public void ToggleRaidMap()
		{
			if (LDOJANLOFHI == NMFLNANKNOJ.RaidMode)
			{
				SwitchToStoryMap();
			}
			else
			{
				SwitchToRaidMap();
			}
		}

		public void SwitchToRaidMap()
		{
			SwitchMapMode(NMFLNANKNOJ.RaidMode);
		}

		public void SwitchToStoryMap()
		{
			SwitchMapMode(NMFLNANKNOJ.StoryMode);
		}

		private void SwitchMapMode(NMFLNANKNOJ mode)
		{
			if (LDOJANLOFHI == mode)
			{
				return;
			}
			_storyContainer.Clear();
			OFKGMKADHBD = null;
			LDOJANLOFHI = mode;
			if (mode == NMFLNANKNOJ.RaidMode)
			{
				_raidPowerMode = false;
				_storyContainer.AddRaidZones();
				_storyContainer.SetRaidPowerMode(false);
			}
			else
			{
				_storyContainer.AddStoryZones();
			}
			_storyContainer.OpenRightNow();
			LPMGMNCGLOJ();
			LastFight focus = (mode == NMFLNANKNOJ.RaidMode) ? JBAPLBALJII : JOBMMLPAKBF;
			Battle battle = (focus == null) ? null : focus.AODOOCLOLMH();
			if (battle != null && _storyContainer.HasBattle(battle))
			{
				SelectBattle(battle, 0f);
			}
			else if (_storyContainer.GetZonesCount() > 0)
			{
				_storyContainer.ScrollToZone(0, 0f);
				ZoneScrollItem firstZone = _storyContainer.GetZone(0);
				if (firstZone != null)
				{
					firstZone.SelectBattle();
				}
			}
			UpdateCurrentZone();
			IOHMLGLJELB();
			UpdateRaidToggleSprite();
			UpdateRaidControls();
			Debug.Log("[Underworld] switched to " + mode + " with " +
				_storyContainer.GetZonesCount() + " map page(s)");
		}

		private void UpdateRaidToggleSprite()
		{
			GetUnderworldControls().UpdateToggleSprite(LDOJANLOFHI == NMFLNANKNOJ.RaidMode);
		}

		private void AOGHKADFFAK(object data)
		{
			Battle cGJCGEBPCAF = (Battle)data;
			if (cGJCGEBPCAF != _infoBattle.GetCurrentBattle())
			{
				GFLGNFFGOED(cGJCGEBPCAF);
				LPMGMNCGLOJ();
			}
		}

		private void JIINGLBGKOA(object data)
		{
			ZoneScrollItem jEOIJBLAMIO = (ZoneScrollItem)data;
			int zoneIndex = _storyContainer.GetZoneIndex(jEOIJBLAMIO);
			SelectZone(_storyContainer, zoneIndex, 0f);
		}

		private ZoneScrollItem GIHCEFGHAEO(string BCKMHHFHGNH)
		{
			int zoneIndexByName = _storyContainer.GetZoneIndexByName(BCKMHHFHGNH);
			if (zoneIndexByName >= 0)
			{
				return _storyContainer.GetZone(zoneIndexByName);
			}
			return null;
		}

		private void NKEBPJIHFHM(object data)
		{
			if (OFKGMKADHBD != null)
			{
				OFKGMKADHBD.UpdateBattleFocus();
			}
		}

		private ZoneScrollItem JCBDLFBBLFO(Zone HLJKOKMKMLM)
		{
			return GIHCEFGHAEO(HLJKOKMKMLM.get_Name());
		}

		private void CGMPKDGFIAG(bool IJHFJPBBNEJ)
		{
		}

		private void AFFOJJHOALG(bool IJHFJPBBNEJ)
		{
		}

		private void KBGJMMPBDGG(LastFight BEFANIBLCPI)
		{
			if (BEFANIBLCPI != null)
			{
				SelectBattle(BEFANIBLCPI.AODOOCLOLMH(), 0f);
			}
		}

		private void KJKKKGCLFBB()
		{
			SwitchToStoryMap();
		}

		private void JKGCIEAGDAI()
		{
			if (LDOJANLOFHI != NMFLNANKNOJ.StoryMode)
			{
				KJKKKGCLFBB();
			}
		}

		public override void UpdateScene(object data)
		{
		}
	}
}
