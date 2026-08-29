using System;
using System.Collections.Generic;
using System.Xml;

namespace SF2DE.Content
{
	public static class QuestCompatibility
	{
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
}
