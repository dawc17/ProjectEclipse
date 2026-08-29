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
				name.StartsWith(RaidZonePrefix, StringComparison.OrdinalIgnoreCase);
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
					battle.DCHJDPCEODD = true;
				}
			}
		}

		public static bool ShouldShowRoundPips(Battle battle)
		{
			return battle == null ||
				(battle.get_Type() != BattleType.FightRaid && !IsRaidZone(battle.OAEIILGHJMG));
		}
	}
}
