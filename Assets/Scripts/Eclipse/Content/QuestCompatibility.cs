using System;
using System.Collections.Generic;
using System.Xml;

namespace Eclipse.Content
{
	public static class QuestCompatibility
	{
		private static readonly string[,] StoryShopUnlocks = new string[,]
		{
			{ "ZONE_2", "ZONE_1|BOSS_LYNX|6" },
			{ "ZONE_3", "ZONE_2|BOSS_HERMIT|6" },
			{ "ZONE_4", "ZONE_3|BOSS_BUTCHER|6" },
			{ "ZONE_5", "ZONE_4|BOSS_WASP|6" },
			{ "ZONE_6", "ZONE_5|BOSS_HUNTRESS|6" },
			{ "ZONE_IM", "ZONE_6|FINAL_BATTLE|1" },
			{ "ZONE_7", "ZONE_6|BOSS_SAMURAI_INTERMISSION|1" },
			{ "ZONE_7_2", "ZONE_6|BOSS_SAMURAI_INTERMISSION|1" },
			{ "ZONE_7_3", "ZONE_6|BOSS_SAMURAI_INTERMISSION|1" }
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

		public static int ReconcileCompletedStoryShopUnlocks(global::Roster roster)
		{
			if (roster == null)
			{
				return 0;
			}

			int restored = 0;
			for (int index = 0; index < StoryShopUnlocks.GetLength(0); index++)
			{
				string label = StoryShopUnlocks[index, 0];
				if (roster.FLFKOIPCEPI(label))
				{
					continue;
				}
				global::RosterFight fight = roster.DBMHOBPNIIA(
					new global::FightIDS(StoryShopUnlocks[index, 1]));
				if (fight != null && fight.JAJNIKDMPPO() > 0 && roster.AddShopLock(label, true))
				{
					restored++;
				}
			}
			return restored;
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
