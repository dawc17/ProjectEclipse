using System;
using System.Collections.Generic;
using System.Xml;

namespace Eclipse.Content
{
	public static class QuestCompatibility
	{
		private static readonly Dictionary<string, string> UnsupportedHardmodeBosses =
			new Dictionary<string, string>(StringComparer.Ordinal)
			{
				{ "ZONE_1|BOSS_HARDMODE|", "ZONE_1|BOSS_LYNX|" },
				{ "ZONE_2|BOSS_HARDMODE|", "ZONE_2|BOSS_HERMIT|" },
				{ "ZONE_3|BOSS_HARDMODE|", "ZONE_3|BOSS_BUTCHER|" },
				{ "ZONE_4|BOSS_HARDMODE|", "ZONE_4|BOSS_WASP|" },
				{ "ZONE_5|BOSS_HARDMODE|", "ZONE_5|BOSS_HUNTRESS|" },
				{ "ZONE_6|BOSS_HARDMODE|", "ZONE_6|BOSS_SAMURAI|" }
			};

		private static readonly HashSet<string> DeferredQuestEvents = new HashSet<string>(StringComparer.Ordinal)
		{
			"BeforeQueue",
			"CheckUserUpdate",
			"RaidFloorChanged",
			"RaidMapEnter",
			"ReplayButtonPress",
			"ShowRaidLoot"
		};

		private static readonly HashSet<string> LoggedDeferredActions = new HashSet<string>(StringComparer.Ordinal);

		private static XmlElement RenameElement(XmlDocument document, XmlElement source, string newName)
		{
			XmlElement replacement = document.CreateElement(newName);
			foreach (XmlAttribute attribute in source.Attributes)
			{
				replacement.SetAttribute(attribute.Name, attribute.Value);
			}
			while (source.HasChildNodes)
			{
				replacement.AppendChild(source.FirstChild);
			}
			source.ParentNode.ReplaceChild(replacement, source);
			return replacement;
		}

		public static int RestoreUnsupportedHardmodeBosses(XmlNode user)
		{
			if (user == null || user.OwnerDocument == null)
			{
				return 0;
			}

			XmlNode battles = user["Battles"];
			XmlNode fights = user["Fights"];
			if (battles == null || fights == null)
			{
				return 0;
			}

			int changes = 0;
			List<XmlNode> invalid = new List<XmlNode>();
			foreach (XmlNode battle in battles.ChildNodes)
			{
				if (battle.Attributes != null && battle.Attributes["Name"] != null &&
					UnsupportedHardmodeBosses.ContainsKey(battle.Attributes["Name"].Value))
				{
					invalid.Add(battle);
				}
			}

			foreach (XmlNode battle in invalid)
			{
				string replacement = UnsupportedHardmodeBosses[battle.Attributes["Name"].Value];
				battles.RemoveChild(battle);
				changes++;

				if (!HasBattle(battles, replacement) && HasCompletedFight(fights, replacement + "6"))
				{
					XmlElement restored = user.OwnerDocument.CreateElement("Battle");
					restored.SetAttribute("Name", replacement);
					restored.SetAttribute("Locked", "0");
					restored.SetAttribute("Hidden", "0");
					restored.SetAttribute("ReplayCount", "0");
					battles.AppendChild(restored);
					changes++;
				}
			}
			return changes;
		}

		public static bool EnsureEligibleEclipseButton(XmlNode user)
		{
			if (user == null || user.OwnerDocument == null || user["Fights"] == null)
			{
				return false;
			}

			bool unlocked = HasQuestVariableAtLeast(user, "EclipseModeEnabled", 1) ||
				(HasQuestVariableAtLeast(user, "ForgeEnabled", 2) &&
				HasCompletedFight(user["Fights"], "ZONE_2|Tournament|4"));
			if (!unlocked)
			{
				return false;
			}

			XmlNode mapButtons = user["MapButtons"];
			if (mapButtons == null)
			{
				mapButtons = user.AppendChild(user.OwnerDocument.CreateElement("MapButtons"));
			}
			if (HasMapButton(mapButtons, "EclipseModeOn") || HasMapButton(mapButtons, "EclipseModeOff"))
			{
				return false;
			}

			bool eclipseMode = user.Attributes != null && user.Attributes["EclipseMode"] != null &&
				string.Equals(user.Attributes["EclipseMode"].Value, "On", StringComparison.OrdinalIgnoreCase);
			XmlElement button = user.OwnerDocument.CreateElement("Button");
			button.SetAttribute("Name", eclipseMode ? "EclipseModeOff" : "EclipseModeOn");
			button.SetAttribute("Image", eclipseMode ? "eclipse" : "sun_icon");
			button.SetAttribute("Type", "Image");
			button.SetAttribute("X", "-876");
			button.SetAttribute("Y", "432");
			button.SetAttribute("AnchorMinX", "1");
			button.SetAttribute("AnchorMaxX", "1");
			button.SetAttribute("ShowType", "Story");
			mapButtons.AppendChild(button);
			return true;
		}

		private static bool HasBattle(XmlNode battles, string name)
		{
			foreach (XmlNode battle in battles.ChildNodes)
			{
				if (battle.Attributes != null && battle.Attributes["Name"] != null &&
					battle.Attributes["Name"].Value == name)
				{
					return true;
				}
			}
			return false;
		}

		private static bool HasMapButton(XmlNode mapButtons, string name)
		{
			foreach (XmlNode button in mapButtons.ChildNodes)
			{
				if (button.Attributes != null && button.Attributes["Name"] != null &&
					button.Attributes["Name"].Value == name)
				{
					return true;
				}
			}
			return false;
		}

		private static bool HasQuestVariableAtLeast(XmlNode user, string name, int minimum)
		{
			XmlNode variables = user["Quests"] == null ? null : user["Quests"]["Variables"];
			if (variables == null)
			{
				return false;
			}
			foreach (XmlNode variable in variables.ChildNodes)
			{
				if (variable.Attributes == null || variable.Attributes["Name"] == null ||
					variable.Attributes["Name"].Value != name || variable.Attributes["Value"] == null)
				{
					continue;
				}
				int value;
				return int.TryParse(variable.Attributes["Value"].Value, out value) && value >= minimum;
			}
			return false;
		}

		private static bool HasCompletedFight(XmlNode fights, string ids)
		{
			foreach (XmlNode fight in fights.ChildNodes)
			{
				if (fight.Attributes == null || fight.Attributes["IDS"] == null ||
					fight.Attributes["IDS"].Value != ids)
				{
					continue;
				}
				int completed;
				return fight.Attributes["CompletedCount"] != null &&
					int.TryParse(fight.Attributes["CompletedCount"].Value, out completed) && completed > 0;
			}
			return false;
		}

		public static void NormalizeFunctionSyntax(XmlDocument document)
		{
			// Quest ConditionExtension is a different, older evaluator than the
			// combat FunctionExtension: it searches for '(' and ')' explicitly.
			foreach (XmlNode attribute in document.SelectNodes("//@*"))
			{
				string value = attribute.Value;
				if (!string.IsNullOrEmpty(value) && value.IndexOf('?') >= 0 &&
					(value.IndexOf('[') >= 0 || value.IndexOf(']') >= 0))
				{
					attribute.Value = value.Replace('[', '(').Replace(']', ')');
				}
			}
		}

		public static void NormalizeActions(XmlDocument document)
		{
			XmlNodeList visibilityNodes = document.SelectNodes("//SetBattleVisibility");
			List<XmlElement> visibility = new List<XmlElement>();
			foreach (XmlNode node in visibilityNodes)
			{
				if (node is XmlElement)
				{
					visibility.Add((XmlElement)node);
				}
			}
			foreach (XmlElement node in visibility)
			{
				bool isVisible = node.GetAttribute("IsVisible") == "1" ||
					string.Equals(node.GetAttribute("IsVisible"), "true", StringComparison.OrdinalIgnoreCase);
				XmlElement replacement = RenameElement(document, node, isVisible ? "ShowBattle" : "HideBattle");
				replacement.RemoveAttribute("IsVisible");
			}

			Dictionary<string, string> aliases = new Dictionary<string, string>(StringComparer.Ordinal)
			{
				{ "UpdateShopItems", "UpdateScene" },
				{ "ValidatePacks", "ShowForgeTutorial" },
				{ "ShowArrow", "ShowForgeTutorial" },
				{ "HideArrow", "ShowForgeTutorial" },
				{ "ShowHint", "ShowForgeTutorial" },
				{ "HideHint", "ShowForgeTutorial" },
				{ "BlockTouches", "ShowForgeTutorial" },
				{ "UnblockTouches", "ShowForgeTutorial" },
				{ "ClickButton", "ShowForgeTutorial" },
				{ "ForgeTutorialRevealPropertiesPanel", "ShowForgeTutorial" },
				{ "ForgeTutorialOpenForge", "ShowForgeTutorial" },
				{ "ForgeTutorialGiveRequiredMaterials", "ShowForgeTutorial" },
				{ "ForgeTutorialEnchantItem", "ShowForgeTutorial" }
			};
			foreach (KeyValuePair<string, string> alias in aliases)
			{
				XmlNodeList found = document.SelectNodes("//" + alias.Key);
				List<XmlElement> nodes = new List<XmlElement>();
				foreach (XmlNode node in found)
				{
					if (node is XmlElement)
					{
						nodes.Add((XmlElement)node);
					}
				}
				foreach (XmlElement node in nodes)
				{
					RenameElement(document, node, alias.Value);
				}
			}
		}

		public static int RemoveObsoleteClientUpdateQuests(XmlDocument document)
		{
			XmlNodeList found = document.SelectNodes("//Quest[@Name='SetBackVersionCheck']");
			List<XmlNode> quests = new List<XmlNode>();
			foreach (XmlNode node in found)
			{
				quests.Add(node);
			}
			foreach (XmlNode quest in quests)
			{
				if (quest.ParentNode != null)
				{
					quest.ParentNode.RemoveChild(quest);
				}
			}
			return quests.Count;
		}

		public static int EnableLocalStoryShopUnlockQuests(XmlDocument document)
		{
			if (document == null || document.DocumentElement == null)
			{
				return 0;
			}

			int enabled = 0;
			foreach (XmlNode node in document.SelectNodes("//Quest"))
			{
				XmlElement quest = node as XmlElement;
				if (quest == null || !quest.GetAttribute("Name").EndsWith(
					"_Toggle_For_Steam_and_Switch", StringComparison.Ordinal))
				{
					continue;
				}

				XmlElement actions = quest["Actions"];
				bool unlocksStoryItems = false;
				if (actions != null)
				{
					foreach (XmlNode actionNode in actions.ChildNodes)
					{
						XmlElement action = actionNode as XmlElement;
						if (action != null && action.Name == "ToggleItems" &&
							action.GetAttribute("Label").StartsWith("ZONE_", StringComparison.Ordinal) &&
							string.Equals(action.GetAttribute("Toggle"), "on", StringComparison.OrdinalIgnoreCase))
						{
							unlocksStoryItems = true;
							break;
						}
					}
				}
				if (!unlocksStoryItems)
				{
					continue;
				}

				XmlElement conditions = quest["Conditions"];
				if (conditions == null)
				{
					continue;
				}
				XmlElement platformGate = null;
				foreach (XmlNode conditionNode in conditions.ChildNodes)
				{
					XmlElement operation = conditionNode as XmlElement;
					if (operation == null || operation.Name != "Operator" ||
						!string.Equals(operation.GetAttribute("Type"), "Or", StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}

					bool foundPlatformCheck = false;
					bool onlyPlatformChecks = true;
					foreach (XmlNode termNode in operation.ChildNodes)
					{
						XmlElement term = termNode as XmlElement;
						if (term == null)
						{
							continue;
						}
						string value = term.GetAttribute("Value1");
						bool isPlatformCheck = term.Name == "Equal" &&
							(value == "?SysInfo[].Steam" || value == "?SysInfo[].Switch");
						foundPlatformCheck |= isPlatformCheck;
						onlyPlatformChecks &= isPlatformCheck;
					}
					if (foundPlatformCheck && onlyPlatformChecks)
					{
						platformGate = operation;
						break;
					}
				}

				if (platformGate != null)
				{
					conditions.RemoveChild(platformGate);
					enabled++;
				}
			}
			return enabled;
		}

		public static int PromoteLocalQuestExtension(XmlDocument document, XmlDocument extension)
		{
			if (document == null || document.DocumentElement == null || extension == null ||
				extension.DocumentElement == null)
			{
				return 0;
			}

			List<XmlElement> sourceQuests = new List<XmlElement>();
			HashSet<string> names = new HashSet<string>(StringComparer.Ordinal);
			foreach (XmlNode node in extension.DocumentElement.ChildNodes)
			{
				XmlElement quest = node as XmlElement;
				if (quest == null || quest.Name != "Quest")
				{
					continue;
				}
				string name = quest.GetAttribute("Name");
				if (string.IsNullOrEmpty(name))
				{
					continue;
				}
				sourceQuests.Add(quest);
				names.Add(name);
			}

			List<XmlNode> existing = new List<XmlNode>();
			foreach (XmlNode node in document.DocumentElement.ChildNodes)
			{
				XmlElement quest = node as XmlElement;
				if (quest != null && quest.Name == "Quest" && names.Contains(quest.GetAttribute("Name")))
				{
					existing.Add(quest);
				}
			}
			foreach (XmlNode quest in existing)
			{
				document.DocumentElement.RemoveChild(quest);
			}

			foreach (XmlElement quest in sourceQuests)
			{
				document.DocumentElement.AppendChild(document.ImportNode(quest, true));
			}
			return sourceQuests.Count;
		}

		public static global::QuestAction CreateRuntimeAction(string name)
		{
			switch (name)
			{
			case "OpenRateUrl":
				return new global::QuestActionOpenUrl();
			case "SwitchToRaidsMap":
				return new global::QuestActionSwitchToRaidsMap();
			case "ChangeButtonState":
			case "ClickHint":
			case "ConnectToRaids":
			case "GiveGift":
			case "OpenRaidZone":
			case "RaidIndicateRaidBtn":
			case "SceneMenuScroll":
			case "ShowRaidLoot":
			case "UnlockCharacter":
				return new DeferredQuestAction();
			default:
				return null;
			}
		}

		public static bool TryGetModernSysInfo(string name, out string stringValue, out double numberValue)
		{
			stringValue = string.Empty;
			numberValue = 0.0;
			switch (name)
			{
			// The recovered runtime is the paid/Special Edition codebase. Its
			// bundled internalSettings also points at config_SF2_paid.xml. Modern
			// quest data expresses that build channel through these newer flags.
			case "Paid":
				numberValue = 1.0;
				return true;
			case "AnyF2P":
			case "ChinaF2P":
			case "SamsungF2P":
			case "NBO":
				numberValue = 0.0;
				return true;

			// Eclipse is a local PC port, not a Switch/Steam/mobile service build.
			// Do not advertise online/platform features whose backing subsystem is
			// absent or intentionally deferred in this recovered runtime.
			case "Switch":
			case "AdvertisingSupport":
			case "FacebookLoginSupport":
			case "RaidsSupport":
			case "LowGraphicsSupport":
				numberValue = 0.0;
				return true;

			case "IsDebug":
				numberValue = global::SystemProperties.DBBOCENKMGD() ? 1.0 : 0.0;
				return true;
			case "IsSocialAuthorized":
				numberValue = global::GameCenterController.OBDJPKOJADA() ? 1.0 : 0.0;
				return true;
			case "FramesCount":
				numberValue = UnityEngine.Time.frameCount;
				return true;
			case "OsVersion":
				stringValue = Environment.OSVersion.Version.Major.ToString();
				return true;
			default:
				return false;
			}
		}

		public static bool IsDeferredQuestEvent(string name)
		{
			return DeferredQuestEvents.Contains(name);
		}

		internal static void LogDeferredAction(string name)
		{
			if (LoggedDeferredActions.Add(name))
			{
				UnityEngine.Debug.LogWarning("[DevXml] quest action '" + name +
					"' belongs to a newer runtime subsystem and is skipped when reached");
			}
		}

		public static void AddQuestWithConditions(
			XmlDocument output,
			XmlElement outputRoot,
			XmlNode quest,
			XmlNode inheritedConditions)
		{
			XmlElement importedQuest = (XmlElement)output.ImportNode(quest, true);
			if (inheritedConditions != null && inheritedConditions.HasChildNodes)
			{
				XmlElement questConditions = importedQuest["Conditions"];
				if (questConditions == null)
				{
					questConditions = output.CreateElement("Conditions");
					XmlNode actions = importedQuest["Actions"];
					if (actions == null)
					{
						importedQuest.AppendChild(questConditions);
					}
					else
					{
						importedQuest.InsertBefore(questConditions, actions);
					}
				}
				for (int index = inheritedConditions.ChildNodes.Count - 1; index >= 0; index--)
				{
					questConditions.PrependChild(output.ImportNode(inheritedConditions.ChildNodes[index], true));
				}
			}
			outputRoot.AppendChild(importedQuest);
		}
	}

	public sealed class DeferredQuestAction : global::QuestAction
	{
		public override void DEJMHFMLKIC(global::QuestParameters parameters)
		{
			base.DEJMHFMLKIC(parameters);
			QuestCompatibility.LogDeferredAction(EFJMDEMAGIM);
			OGIJONMKABB();
		}
	}
}
