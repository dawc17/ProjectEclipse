using System;

namespace SF2DE.Underworld.UI
{
	public static class UnderworldMapBattlePresentation
	{
		public static bool IsBattleVisible(Battle battle, Zone zone, bool powerMode)
		{
			if (battle == null || !UnderworldZonePolicy.IsRaidZone(zone))
			{
				return true;
			}
			bool hardMode = battle.get_Name().EndsWith("_HARDMODE", StringComparison.OrdinalIgnoreCase);
			return hardMode == powerMode;
		}

		public static string ResolveZoneSpriteName(string spriteName)
		{
			if (spriteName == "Raid1.1" || spriteName == "Raid1.2" ||
				spriteName == "Raid1.3" || spriteName == "Raid2.1")
			{
				// The reference export suffixes the full-resolution raid crop with
				// _0; the unsuffixed duplicate points at the half-size low atlas.
				return spriteName + "_0";
			}
			return spriteName;
		}

		public static string ResolveBattleIconAtlas(Zone zone, string iconAtlas)
		{
			if (string.IsNullOrEmpty(iconAtlas) && UnderworldZonePolicy.IsRaidZone(zone))
			{
				return "BattleBtn_raid";
			}
			return iconAtlas;
		}
	}
}
