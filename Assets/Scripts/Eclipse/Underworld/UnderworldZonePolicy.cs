using System;
using System.Collections.Generic;

namespace Eclipse.Underworld
{
	public static class UnderworldZonePolicy
	{
		public const string RaidZonePrefix = "ZONE_RAID";

		public static bool IsRaidZone(Zone zone)
		{
			return zone != null && IsRaidZoneName(zone.get_Name());
		}

		public static bool IsRaidZoneName(string name)
		{
			return !string.IsNullOrEmpty(name) &&
				(name.StartsWith(RaidZonePrefix, StringComparison.OrdinalIgnoreCase) || Eclipse.Modding.ModPolicies.IsRaidZone(name));
		}

		public static void MarkLocallyPlayable(IEnumerable<Zone> zones)
		{
			if (zones == null)
			{
				return;
			}
			foreach (Zone zone in zones)
			{
				if (zone == null)
				{
					continue;
				}
				foreach (Battle battle in zone.LGIIBNJFADA)
				{
					battle.IsMapVisible = true;
				}
			}
		}

		// MapScene.Init lost the recovered "return to the raid map" branch (it always opens the
		// story map). Remember an Underworld fight, and whether its entry is a Power Mode one,
		// so the next map opens the Underworld again at its saved raid focus.
		private static bool _returnToRaidMap;
		private static bool _returnPowerMode;

		public static void NoteFightStarted(Battle battle)
		{
			Zone zone = battle == null ? null : battle.OAEIILGHJMG;
			_returnToRaidMap = IsRaidZone(zone);
			_returnPowerMode = _returnToRaidMap &&
				UI.UnderworldMapBattlePresentation.IsBattleVisible(battle, zone, true) &&
				!UI.UnderworldMapBattlePresentation.IsBattleVisible(battle, zone, false);
		}

		public static bool ConsumeMapReturn(out bool powerMode)
		{
			bool result = _returnToRaidMap;
			powerMode = result && _returnPowerMode;
			_returnToRaidMap = _returnPowerMode = false;
			return result;
		}

		public static bool ShouldShowRoundPips(Battle battle)
		{
			return battle == null ||
				(battle.get_Type() != BattleType.FightRaid && !IsRaidZone(battle.OAEIILGHJMG));
		}
	}
}
