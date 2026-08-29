using System;
using System.Xml;

namespace SF2DE.Content
{
	public static class StageCompatibility
	{
		public static int MergeMissingBattles(XmlDocument stages, XmlDocument compatibilityStages)
		{
			int imported = 0;
			foreach (XmlElement compatZone in compatibilityStages.SelectNodes("/Stages/Zones/Zone[@Name]"))
			{
				string zoneName = compatZone.GetAttribute("Name");
				XmlElement customZone = stages.SelectSingleNode(
					"/Stages/Zones/Zone[@Name='" + zoneName + "']") as XmlElement;
				if (customZone == null)
				{
					continue;
				}
				foreach (XmlElement compatBattle in compatZone.SelectNodes("./Battle[@Name]"))
				{
					string battleName = compatBattle.GetAttribute("Name");
					if (customZone.SelectSingleNode("./Battle[@Name='" + battleName + "']") != null)
					{
						continue;
					}
					customZone.AppendChild(stages.ImportNode(compatBattle, true));
					imported++;
				}
			}
			return imported;
		}

		public static void MaterializeBattleInheritance(
			XmlDocument stages,
			out int inheritedWarriors,
			out int inheritedRuleSets)
		{
			inheritedWarriors = 0;
			inheritedRuleSets = 0;
			foreach (XmlElement battle in stages.SelectNodes("//Battle"))
			{
				XmlElement commonWarriors = battle["Warriors"];
				XmlElement commonRules = battle["Rules"];
				foreach (XmlElement fight in battle.SelectNodes("./Fight"))
				{
					if (fight["Warriors"] == null && commonWarriors != null)
					{
						fight.AppendChild(stages.ImportNode(commonWarriors, true));
						inheritedWarriors++;
					}
					if (commonRules != null)
					{
						XmlElement fightRules = fight["Rules"];
						if (fightRules == null)
						{
							fightRules = stages.CreateElement("Rules");
							fight.AppendChild(fightRules);
						}
						foreach (XmlNode commonRule in commonRules.ChildNodes)
						{
							fightRules.AppendChild(stages.ImportNode(commonRule, true));
						}
						inheritedRuleSets++;
					}
				}
			}
		}

		public static int EnsureSurvivalRewardRows(XmlDocument stages)
		{
			int addedRows = 0;
			foreach (XmlElement battle in stages.SelectNodes("//Battle[@Type='SURVIVAL']"))
			{
				foreach (XmlElement fight in battle.SelectNodes("./Fight"))
				{
					XmlElement warrior = fight.SelectSingleNode("./Warriors/Warrior[@Number]") as XmlElement;
					XmlElement rewards = fight["Rewards"];
					if (warrior == null || rewards == null)
					{
						continue;
					}
					int waves;
					if (!int.TryParse(warrior.GetAttribute("Number"), out waves) || waves <= 0)
					{
						continue;
					}
					XmlNodeList rewardNodes = rewards.SelectNodes("./Reward");
					XmlElement terminalReward = rewards.SelectSingleNode("./Reward[last()]") as XmlElement;
					if (terminalReward == null)
					{
						continue;
					}
					// Survival reward index 0 is the initial state; the legacy result
					// flow selects the reward at the completed-wave index. Recovered
					// late-game tables have no authored payouts beyond wave six, so keep
					// their terminal payout rather than producing a null reward.
					for (int rewardCount = rewardNodes.Count; rewardCount <= waves; rewardCount++)
					{
						rewards.AppendChild(stages.ImportNode(terminalReward, true));
						addedRows++;
					}
				}
			}
			return addedRows;
		}

		public static int ClampLegacyRoundTimes(
			XmlDocument stages,
			Predicate<XmlElement> preserveLongTimer)
		{
			int clampedRoundTimes = 0;
			foreach (XmlElement timedNode in stages.SelectNodes("//*[@RoundTime]"))
			{
				if (preserveLongTimer != null && preserveLongTimer(timedNode))
				{
					continue;
				}
				int roundTime;
				if (int.TryParse(timedNode.GetAttribute("RoundTime"), out roundTime) && roundTime > 99)
				{
					timedNode.SetAttribute("RoundTime", "99");
					clampedRoundTimes++;
				}
			}
			return clampedRoundTimes;
		}
	}
}
