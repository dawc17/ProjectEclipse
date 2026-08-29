using System;
using System.Collections.Generic;
using System.Xml;

namespace Eclipse.Content
{
	public static class QuestCompatibility
	{
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
