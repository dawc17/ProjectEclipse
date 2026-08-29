using System.Xml;

namespace Eclipse.Underworld.Content
{
	public static class UnderworldStageCompatibility
	{
		public static int ImportRaidZones(XmlDocument stages, XmlDocument raidStages)
		{
			if (stages == null || raidStages == null || stages.DocumentElement == null)
			{
				return 0;
			}
			XmlElement targetZones = stages.SelectSingleNode("/Stages/Zones") as XmlElement;
			if (targetZones == null)
			{
				return 0;
			}

			int imported = 0;
			foreach (XmlElement raidZone in raidStages.SelectNodes("/Stages/Zones/Zone[@Name]"))
			{
				string zoneName = raidZone.GetAttribute("Name");
				if (stages.SelectSingleNode("/Stages/Zones/Zone[@Name='" + zoneName + "']") != null)
				{
					continue;
				}
				targetZones.AppendChild(stages.ImportNode(raidZone, true));
				imported++;
			}
			return imported;
		}

		public static bool IsInsideRaidZone(XmlElement element)
		{
			for (XmlNode node = element == null ? null : element.ParentNode; node != null; node = node.ParentNode)
			{
				XmlElement zone = node as XmlElement;
				if (zone != null && zone.Name == "Zone" &&
					UnderworldZonePolicy.IsRaidZoneName(zone.GetAttribute("Name")))
				{
					return true;
				}
			}
			return false;
		}

		public static int AdaptOfflineRaidRounds(XmlDocument stages)
		{
			if (stages == null)
			{
				return 0;
			}
			int adapted = 0;
			string xpath = "/Stages/Zones/Zone[starts-with(@Name,'" +
				UnderworldZonePolicy.RaidZonePrefix + "')]/Battle/Fight[@Rounds='0']";
			foreach (XmlElement raidFight in stages.SelectNodes(xpath))
			{
				raidFight.SetAttribute("Rounds", "1");
				adapted++;
			}
			return adapted;
		}
	}
}
