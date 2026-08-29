using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SF2DE.Underworld.Content;
using UnityEngine;
using UnityEngine.Video;

public static class ResourceManager
{
	public static Texture2D GetTextureFromExternal(string DCOPLCIFCFL)
	{
		byte[] array;
		using (FileStream fileStream = new FileStream(DCOPLCIFCFL, FileMode.Open, FileAccess.Read))
		{
			array = new byte[fileStream.Length];
			fileStream.Read(array, 0, array.Length);
		}
		Texture2D texture2D = new Texture2D(2, 2, TextureFormat.ARGB32, false);
		texture2D.LoadImage(array);
		return texture2D;
	}

	public static byte[] GetBinary(string DCOPLCIFCFL)
	{
		if (SF2Paths.CGOHPKEBECD)
		{
			DCOPLCIFCFL = DCOPLCIFCFL.TrimStart('\\', '/');
			DCOPLCIFCFL = NNBCLAEKMIO(DCOPLCIFCFL);
			TextAsset textAsset = ResourcesAndBundles.Load<TextAsset>(DCOPLCIFCFL);
			return (!textAsset) ? null : textAsset.bytes;
		}
		FileStream fileStream = new FileStream(DCOPLCIFCFL, FileMode.Open, FileAccess.Read);
		byte[] array = new byte[fileStream.Length];
		fileStream.Read(array, 0, (int)fileStream.Length);
		fileStream.Close();
		return array;
	}

	public static AudioClip GetAudioClip(string DCOPLCIFCFL)
	{
		if (SF2Paths.CGOHPKEBECD)
		{
			DCOPLCIFCFL = DCOPLCIFCFL.TrimStart('\\', '/');
			return ResourcesAndBundles.Load<AudioClip>(DCOPLCIFCFL);
		}
		WWW wWW = new WWW(string.Format("file:///{0}", DCOPLCIFCFL));
		while (!wWW.isDone && string.IsNullOrEmpty(wWW.error))
		{
		}
		if (string.IsNullOrEmpty(wWW.error))
		{
			return wWW.GetAudioClipCompressed(true);
		}
		return null;
	}

	public static VideoClip DEKCGMCMGKK(string DCOPLCIFCFL)
	{
		DCOPLCIFCFL = SF2Paths.HAHDKJAPIJL() + "/" + DCOPLCIFCFL;
		DCOPLCIFCFL = DCOPLCIFCFL.TrimStart('\\', '/');
		return ResourcesAndBundles.Load<VideoClip>(DCOPLCIFCFL);
	}


		private static string _devXmlRoot;
		private static bool _devXmlRootInit;
		private static readonly HashSet<string> _devXmlLogged = new HashSet<string>();
		private static readonly Dictionary<string, string> _devModelFallbacks =
			new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		// Newer plaintext gamedata is adapted at the boundary before old parsers see it.
		private static bool _devXmlGameplayOverridesEnabled = true;

		private static bool IsDevModelRequest(string requestPath)
		{
			string normalizedPath = requestPath.Replace('\\', '/');
			return normalizedPath.IndexOf("/models/", StringComparison.OrdinalIgnoreCase) >= 0 ||
				normalizedPath.StartsWith("models/", StringComparison.OrdinalIgnoreCase) ||
				normalizedPath.StartsWith("assets/models/", StringComparison.OrdinalIgnoreCase);
		}

		private static bool IsCompatibleDevXml(string requestPath, string file, string text)
		{
			try
			{
				if (string.Equals(Path.GetExtension(file), ".json", StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(text);
				if (IsDevModelRequest(requestPath))
				{
					XmlElement scene = xmlDocument.DocumentElement;
					if (scene == null || !string.Equals(scene.Name, "Scene", StringComparison.Ordinal) ||
						scene["Figures"] == null)
					{
						LogIncompatibleDevXml(file, "model XML must have a <Scene> root and <Figures> section");
						return false;
					}
					return true;
				}
				string fileName = Path.GetFileName(requestPath);
				string expectedRoot = null;
				if (string.Equals(fileName, "quests.xml", StringComparison.OrdinalIgnoreCase))
				{
					expectedRoot = "Quests";
				}
				else if (string.Equals(fileName, "list.xml", StringComparison.OrdinalIgnoreCase))
				{
					expectedRoot = "List";
				}
				else if (string.Equals(fileName, "stages.xml", StringComparison.OrdinalIgnoreCase))
				{
					expectedRoot = "Stages";
				}
				else if (string.Equals(fileName, "moves.xml", StringComparison.OrdinalIgnoreCase))
				{
					expectedRoot = "Movesxml";
				}

				if (expectedRoot != null && (xmlDocument.DocumentElement == null ||
					!string.Equals(xmlDocument.DocumentElement.Name, expectedRoot, StringComparison.Ordinal)))
				{
					LogIncompatibleDevXml(file, "expected <" + expectedRoot + "> root, found <" +
						((xmlDocument.DocumentElement == null) ? "none" : xmlDocument.DocumentElement.Name) + ">");
					return false;
				}

				if (string.Equals(fileName, "moves.xml", StringComparison.OrdinalIgnoreCase))
				{
					XmlNode templatesNode = xmlDocument.SelectSingleNode("/Movesxml/Templates");
					XmlNode movesNode = xmlDocument.SelectSingleNode("/Movesxml/Moves");
					if (templatesNode == null || movesNode == null)
					{
						LogIncompatibleDevXml(file, "Movesxml must contain Templates and Moves sections");
						return false;
					}

					HashSet<string> templateNames = new HashSet<string>(StringComparer.Ordinal);
					foreach (XmlNode templateNode in templatesNode.ChildNodes)
					{
						XmlAttribute nameAttribute = templateNode.Attributes == null ? null : templateNode.Attributes["Name"];
						if (nameAttribute != null)
						{
							templateNames.Add(nameAttribute.Value);
						}
					}
					List<string> missingTemplates = new List<string>();
					XmlNodeList templatedNodes = xmlDocument.SelectNodes("/Movesxml/Templates/Template[@Template] | /Movesxml/Moves/Move[@Template]");
					foreach (XmlNode templatedNode in templatedNodes)
					{
						foreach (string templateName in templatedNode.Attributes["Template"].Value.Split('|'))
						{
							if (!templateNames.Contains(templateName) && !missingTemplates.Contains(templateName))
							{
								missingTemplates.Add(templateName);
							}
						}
					}
					if (missingTemplates.Count != 0)
					{
						LogIncompatibleDevXml(file, "undefined move templates: " + string.Join(", ", missingTemplates.ToArray()));
						return false;
					}
				}

				if (!string.Equals(fileName, "internalSettings.xml", StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}

				XmlNode settings = xmlDocument["Settings"];
				string[] requiredSections =
				{
					"AssemblySettings",
					"Internet",
					"Supports",
					"EULA",
					"Log",
					"ForcedLogConditions",
					"StarterPackTimer"
				};
				List<string> missingSections = new List<string>();
				foreach (string section in requiredSections)
				{
					if (settings == null || settings[section] == null)
					{
						missingSections.Add(section);
					}
				}
				if (missingSections.Count == 0)
				{
					return true;
				}
				LogIncompatibleDevXml(file, "missing sections: " + string.Join(", ", missingSections.ToArray()));
			}
			catch (Exception exception)
			{
				string logKey = "invalid:" + file;
				if (_devXmlLogged.Add(logKey))
				{
					Debug.LogWarning("[DevXml] ignoring invalid XML override " + file + ": " + exception.Message);
				}
			}
			return false;
		}

		private static void LogIncompatibleDevXml(string file, string reason)
		{
			string logKey = "incompatible:" + file;
			if (_devXmlLogged.Add(logKey))
			{
				Debug.LogWarning("[DevXml] ignoring incompatible override " + file + "; " + reason +
					". Using bundled gamedata instead.");
			}
		}

		private static string GetDevXmlRoot()
		{
			if (!_devXmlRootInit)
			{
				_devXmlRoot = GameplayContentArchive.GetXmlRoot();
				_devXmlRootInit = true;
			}
			return _devXmlRoot;
		}

		private static XmlDocument LoadPlainXml(string file)
		{
			XmlDocument document = new XmlDocument();
			document.XmlResolver = null;
			document.LoadXml(File.ReadAllText(file));
			return document;
		}

		private static void NormalizeFunctionSyntax(XmlDocument document)
		{
			// FunctionExtension.JCJHEBOMKIC parses the legacy expression form
			// ?Function[arguments].Property and directly searches for '[' and ']'.
			// The recovered 2.41.x XML already uses that exact syntax.  Do not
			// rewrite the brackets: doing so makes every perk function reach
			// Substring with a negative length and traps GameLoaderScene in a loop.
		}

		private static void NormalizeQuestFunctionSyntax(XmlDocument document)
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

		private static void NormalizeQuestActions(XmlDocument document)
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

		private static void NormalizeMoves(XmlDocument document)
		{
			// The 2.41.x table is an additive live-service table, not a complete
			// replacement for the Special Edition table embedded in this client.
			// In particular it no longer contains several legacy magic/ranged and
			// shop animations which are still referenced by this executable's AI
			// tables and item models.  Merge missing named entries from the bundled
			// table before resolving templates.  Custom entries always win.
			try
			{
				TextAsset bundledMoves = ResourcesAndBundles.Load<TextAsset>("gamedata/animations/moves");
				if (bundledMoves != null && !string.IsNullOrEmpty(bundledMoves.text))
				{
					XmlDocument baseline = new XmlDocument();
					baseline.XmlResolver = null;
					baseline.LoadXml(bundledMoves.text);
					int restored = 0;
					string[] sections = { "Templates", "Moves", "Triggers" };
					foreach (string sectionName in sections)
					{
						XmlNode targetSection = document.SelectSingleNode("/Movesxml/" + sectionName);
						XmlNode sourceSection = baseline.SelectSingleNode("/Movesxml/" + sectionName);
						if (targetSection == null || sourceSection == null)
							continue;
						HashSet<string> names = new HashSet<string>(StringComparer.Ordinal);
						foreach (XmlNode existing in targetSection.ChildNodes)
						{
							XmlAttribute name = existing.Attributes == null ? null : existing.Attributes["Name"];
							if (name != null)
								names.Add(name.Value);
						}
						foreach (XmlNode legacy in sourceSection.ChildNodes)
						{
							XmlAttribute name = legacy.Attributes == null ? null : legacy.Attributes["Name"];
							if (name != null && names.Add(name.Value))
							{
								XmlElement imported = (XmlElement)document.ImportNode(legacy, true);
								if (sectionName == "Moves")
									imported.SetAttribute("UseLegacyTemplates", "1");
								targetSection.AppendChild(imported);
								restored++;
							}
						}
					}
					if (restored != 0)
					{
						// The newer table has flattened its templates into each move.
						// Its empty template markers cannot supply the locks, conditions,
						// actions or alignment still inherited by bundled legacy moves.
						// Keep their original definitions separate so modern moves do not
						// inherit duplicate actions or obsolete restrictions.
						XmlElement legacyTemplates = document.CreateElement("LegacyTemplates");
						foreach (XmlNode template in baseline.SelectNodes("/Movesxml/Templates/Template"))
							legacyTemplates.AppendChild(document.ImportNode(template, true));
						document.DocumentElement.AppendChild(legacyTemplates);
					}
					if (restored != 0 && _devXmlLogged.Add("moves-legacy-merge"))
						Debug.Log("[DevXml] restored " + restored +
							" legacy magic/ranged animation definitions missing from the newer table");
				}
			}
			catch (Exception exception)
			{
				if (_devXmlLogged.Add("moves-legacy-merge-error"))
					Debug.LogWarning("[DevXml] could not merge bundled move compatibility data: " + exception.Message);
			}

			// Newer one-frame move ModFlag actions are supported by the compatibility
			// bridge in ActionsParser and must remain in the adapted document.

			XmlNode templatesNode = document.SelectSingleNode("/Movesxml/Templates");
			if (templatesNode == null)
			{
				return;
			}

			HashSet<string> templateNames = new HashSet<string>(StringComparer.Ordinal);
			foreach (XmlNode templateNode in templatesNode.ChildNodes)
			{
				XmlAttribute name = templateNode.Attributes == null ? null : templateNode.Attributes["Name"];
				if (name != null && !string.IsNullOrEmpty(name.Value))
				{
					templateNames.Add(name.Value);
				}
			}

			XmlNodeList templatedNodes = document.SelectNodes(
				"/Movesxml/Templates/Template[@Template] | /Movesxml/Moves/Move[@Template]");
			foreach (XmlNode templatedNode in templatedNodes)
			{
				XmlAttribute attribute = templatedNode.Attributes["Template"];
				List<string> compatible = new List<string>();
				List<string> missing = new List<string>();
				foreach (string templateName in attribute.Value.Split('|'))
				{
					if (templateNames.Contains(templateName))
					{
						compatible.Add(templateName);
					}
					else if (!string.IsNullOrEmpty(templateName))
					{
						missing.Add(templateName);
					}
				}
				if (missing.Count == 0)
				{
					continue;
				}
				if (compatible.Count == 0)
				{
					templatedNode.Attributes.Remove(attribute);
				}
				else
				{
					attribute.Value = string.Join("|", compatible.ToArray());
				}
				string moveName = templatedNode.Attributes["Name"].CIPOICEEIBK(templatedNode.Name);
				string logKey = "move-template:" + moveName + ":" + string.Join("|", missing.ToArray());
				if (_devXmlLogged.Add(logKey))
				{
					Debug.LogWarning("[DevXml] move '" + moveName + "' drops unavailable newer template(s): " +
						string.Join(", ", missing.ToArray()));
				}
			}
		}

		private static string GetModelFallback(XmlElement item)
		{
			string model = item.GetAttribute("Model").ToLowerInvariant();
			string type = item.GetAttribute("Type");
			string subType = item.GetAttribute("SubType");
			if (model.StartsWith("mdl_body") || model == "mdl_blackness_body")
				return "mdl_body";
			if (model.StartsWith("mdl_head"))
				return "mdl_head";
			if (model.StartsWith("mdl_helm"))
				return "mdl_helm_light";
			if (model.StartsWith("mdl_armor"))
				return "mdl_armor_leather";
			if (model.IndexOf("punch_bag") >= 0)
				return (type == "Skeleton") ? "mdl_skeleton_punching_bag" : "mdl_punching_bag";
			if (type == "Skeleton")
				return "mdl_skeleton";
			if (type == "Helm")
				return "mdl_helm_light";
			if (type == "Armor")
				return "mdl_armor_leather";
			if (type == "Ranged")
			{
				if (subType == "Chakram") return "mdl_ranged_chakram";
				if (subType == "Kunai") return "mdl_ranged_kunai";
				return "mdl_ranged_shurikens";
			}
			if (type == "Magic")
			{
				if (subType == "HitBox" || subType == "VerticalTrigger")
					return "mdl_magic_collision_box";
				return "mdl_magic_energy_ball";
			}
			if (type == "Weapon")
			{
				switch (subType)
				{
				case "Kusarigama": return "mdl_weapon_super_kusarigama";
				case "OneHandedSword": return "mdl_one_handed_sword";
				case "Staff": return "mdl_weapon_staff";
				case "TwoHandedBlunt": return "mdl_weapon_super_hammers";
				case "Katana": return "mdl_weapon_katana";
				case "SteelClaws":
				case "Claws": return "mdl_weapon_claws";
				case "HunterClaws": return "mdl_hunter_claw";
				case "TwoHanded": return "mdl_weapon_two_hand_sword";
				case "Sickles": return "mdl_weapon_sickles";
				case "Batons": return "mdl_weapon_batons";
				case "Scythe": return "mdl_weapon_composite_scythe";
				case "CompositeSword": return "mdl_weapon_super_composite_sword";
				case "Daggers": return "mdl_weapon_daggers";
				case "Glaive": return "mdl_weapon_glaive";
				case "Knuckles":
				case "PowerFistsPrometheus": return "mdl_weapon_knuckles";
				case "Nunchaku": return "mdl_weapon_nunchaku";
				default: return "mdl_weapon_ninja_sword";
				}
			}
			return "mdl_skeleton";
		}

		private static void NormalizeList(XmlDocument document)
		{
			// The 2.41.9 stages file still uses a handful of historical internal item
			// identifiers which were renamed/removed from the newer list. Keep the
			// user's source XML intact and materialize hidden aliases in memory so an
			// affected NPC template cannot fail while equipping its loadout.
			var stageItemAliases = new Dictionary<string, string>(StringComparer.Ordinal)
			{
				{ "ARMOR_IM_CEREMONIAL", "ARMOR_CEREMONIAL" },
				{ "HELM_IM_CEREMONIAL", "HELM_CEREMONIAL" },
				{ "MAGIC_DARK_WAVE", "MAGIC_C4_Z1_WARLOCK_DARK_WAVE" },
				{ "RANGED_NEEDLES", "RANGED_NEEDLE" },
				{ "WEAPON_SPEAR", "WEAPON_AE21_SPEAR" }
			};
			XmlElement items = document.SelectSingleNode("/List/Items") as XmlElement;
			int aliasesAdded = 0;
			if (items != null)
			{
				foreach (KeyValuePair<string, string> alias in stageItemAliases)
				{
					if (document.SelectSingleNode("/List/Items/Item[@Name='" + alias.Key + "']") != null)
						continue;
					XmlElement source = document.SelectSingleNode(
						"/List/Items/Item[@Name='" + alias.Value + "']") as XmlElement;
					if (source == null)
						continue;
					XmlElement itemAlias = (XmlElement)source.CloneNode(true);
					itemAlias.SetAttribute("Name", alias.Key);
					itemAlias.SetAttribute("ShopHide", "1");
					items.AppendChild(itemAlias);
					aliasesAdded++;
				}
			}
			if (aliasesAdded != 0 && _devXmlLogged.Add("stage-item-aliases"))
			{
				Debug.Log("[DevXml] restored " + aliasesAdded +
					" historical stage-only item aliases");
			}

			string modelsRoot = Path.Combine(GetDevXmlRoot(), "models");
			int hidden = 0;
			XmlNodeList itemNodes = document.SelectNodes("/List/Items/Item[@Model]");
			foreach (XmlNode node in itemNodes)
			{
				XmlElement item = node as XmlElement;
				if (item == null)
					continue;
				string model = item.GetAttribute("Model");
				if (string.IsNullOrEmpty(model) || File.Exists(Path.Combine(modelsRoot, model + ".xml")))
					continue;
				string fallback = GetModelFallback(item);
				if (File.Exists(Path.Combine(modelsRoot, fallback + ".xml")))
					_devModelFallbacks[model] = fallback;
				if (item.GetAttribute("ShopHide") != "1")
				{
					item.SetAttribute("ShopHide", "1");
					hidden++;
				}
			}
			if (hidden != 0 && _devXmlLogged.Add("unresolved-shop-models"))
			{
				Debug.LogWarning("[DevXml] hid " + hidden +
					" optional shop items whose newer model assets are unavailable; owned items use safe model fallbacks");
			}
		}

		private static void AddQuestWithConditions(XmlDocument output, XmlElement outputRoot,
			XmlNode quest, XmlNode inheritedConditions)
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

		private static void ExpandQuestContainer(XmlDocument output, XmlElement outputRoot,
			XmlNode container, XmlNode inheritedConditions, HashSet<string> includeStack)
		{
			foreach (XmlNode child in container.ChildNodes)
			{
				if (child.NodeType != XmlNodeType.Element)
				{
					continue;
				}
				if (child.Name == "Quest")
				{
					AddQuestWithConditions(output, outputRoot, child, inheritedConditions);
					continue;
				}
				if (child.Name != "Include" || child.Attributes == null || child.Attributes["File"] == null)
				{
					continue;
				}

				string root = Path.GetFullPath(GetDevXmlRoot()).TrimEnd(
					Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
				string includeFile = Path.GetFullPath(Path.Combine(root,
					child.Attributes["File"].Value.Replace('/', Path.DirectorySeparatorChar)));
				if (!includeFile.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ||
					!File.Exists(includeFile) || !includeStack.Add(includeFile))
				{
					Debug.LogWarning("[DevXml] missing or recursive quest include: " + includeFile);
					continue;
				}
				XmlDocument included = LoadPlainXml(includeFile);
				XmlNode includeConditions = child["Conditions"] ?? inheritedConditions;
				ExpandQuestContainer(output, outputRoot, included.DocumentElement, includeConditions, includeStack);
				includeStack.Remove(includeFile);
			}
		}

		private static string AdaptQuests(string file)
		{
			XmlDocument source = LoadPlainXml(file);
			XmlDocument output;
			if (source.DocumentElement != null && source.DocumentElement.Name == "Root")
			{
				output = new XmlDocument();
				XmlElement quests = output.CreateElement("Quests");
				output.AppendChild(quests);
				ExpandQuestContainer(output, quests, source.DocumentElement, null,
					new HashSet<string>(StringComparer.OrdinalIgnoreCase));
			}
			else
			{
				output = source;
			}
			NormalizeQuestFunctionSyntax(output);
			NormalizeQuestActions(output);
			return output.OuterXml;
		}

		private static XmlElement FindMatchingSettingsChild(XmlElement target, XmlElement fallbackChild)
		{
			string[] identityAttributes = { "Name", "Type", "ID", "Gems", "PlatformID" };
			foreach (XmlNode childNode in target.ChildNodes)
			{
				XmlElement child = childNode as XmlElement;
				if (child == null || child.Name != fallbackChild.Name)
					continue;
				bool keyed = false;
				bool matches = true;
				foreach (string identityAttribute in identityAttributes)
				{
					if (!fallbackChild.HasAttribute(identityAttribute))
						continue;
					keyed = true;
					if (child.GetAttribute(identityAttribute) != fallbackChild.GetAttribute(identityAttribute))
					{
						matches = false;
						break;
					}
				}
				if (!keyed || matches)
					return child;
			}
			return null;
		}

		private static int MergeMissingSettings(XmlDocument document, XmlElement target, XmlElement fallback)
		{
			int imported = 0;
			foreach (XmlAttribute attribute in fallback.Attributes)
			{
				if (!target.HasAttribute(attribute.Name))
				{
					target.SetAttribute(attribute.Name, attribute.Value);
					imported++;
				}
			}
			foreach (XmlNode fallbackNode in fallback.ChildNodes)
			{
				XmlElement fallbackChild = fallbackNode as XmlElement;
				if (fallbackChild == null)
					continue;
				XmlElement targetChild = FindMatchingSettingsChild(target, fallbackChild);
				if (targetChild == null)
				{
					target.AppendChild(document.ImportNode(fallbackChild, true));
					imported++;
					continue;
				}
				imported += MergeMissingSettings(document, targetChild, fallbackChild);
			}
			return imported;
		}

		private static int RemoveUnsupportedQualityOptions(XmlDocument document)
		{
			int removed = 0;
			XmlNodeList options = document.SelectNodes("/Settings/QualityOptions/Option[@Name]");
			var unsupported = new List<XmlNode>();
			foreach (XmlNode option in options)
			{
				string name = option.Attributes["Name"].Value;
				if (name != "ReduceFPS" && name != "ParticlesOff" && name != "SequencesOff")
					unsupported.Add(option);
			}
			foreach (XmlNode option in unsupported)
			{
				option.ParentNode.RemoveChild(option);
				removed++;
			}
			return removed;
		}

		private static string AdaptInternalSettings(string file)
		{
			XmlDocument custom = LoadPlainXml(file);
			string compatFile = Path.Combine(GetDevXmlRoot(), "compat", "internalSettings.xml");
			if (custom.DocumentElement != null && File.Exists(compatFile))
			{
				XmlDocument compat = LoadPlainXml(compatFile);
				if (compat.DocumentElement != null)
				{
					int imported = MergeMissingSettings(custom, custom.DocumentElement,
						compat.DocumentElement);
					if (imported != 0 && _devXmlLogged.Add("settings-schema-bridge"))
					{
						Debug.Log("[DevXml] restored " + imported +
							" missing legacy settings nodes/attributes");
					}
				}
			}
			int removedQualityOptions = RemoveUnsupportedQualityOptions(custom);
			if (removedQualityOptions != 0 && _devXmlLogged.Add("settings-quality-bridge"))
			{
				Debug.Log("[DevXml] ignored " + removedQualityOptions +
					" newer quality profile marker(s) unsupported by this runtime");
			}
			// The newer development settings ship with this cheat enabled. In a
			// Unity Editor build Debug.isDebugBuild is always true, so honoring it
			// makes magic conditions accept zero bullets and rewrites the cast's
			// -1 consumption action to zero. Keep migrated gameplay data active,
			// but never import this development-only unlimited-magic switch.
			XmlElement alwaysMagic = custom.SelectSingleNode("/Settings/AlwaysMagicMode") as XmlElement;
			if (alwaysMagic != null && alwaysMagic.GetAttribute("Value") != "0")
			{
				alwaysMagic.SetAttribute("Value", "0");
				if (_devXmlLogged.Add("settings-disable-always-magic"))
				{
					Debug.Log("[DevXml] disabled newer AlwaysMagicMode developer cheat");
				}
			}
			NormalizeFunctionSyntax(custom);
			return custom.OuterXml;
		}

		private static int EnsureSurvivalRewardRows(XmlDocument document)
		{
			int addedRows = 0;
			foreach (XmlElement battle in document.SelectNodes("//Battle[@Type='SURVIVAL']"))
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
						rewards.AppendChild(document.ImportNode(terminalReward, true));
						addedRows++;
					}
				}
			}
			return addedRows;
		}

		private static string AdaptStages(string file)
		{
			XmlDocument custom = LoadPlainXml(file);
			// Underworld shipped as a second stage document in newer gamedata, but
			// this legacy runtime only opens stages.xml. Merge the raid zones into
			// the document before applying the normal compatibility transforms so
			// their fights use the same parser and local FightScene as story fights.
				string raidsFile = Path.Combine(GetDevXmlRoot(), "raid_stages_default.xml");
				int importedRaidZones = 0;
				if (custom.DocumentElement != null && File.Exists(raidsFile))
				{
					importedRaidZones = UnderworldStageCompatibility.ImportRaidZones(custom, LoadPlainXml(raidsFile));
				}
			if (importedRaidZones != 0 && _devXmlLogged.Add("underworld-stage-zones"))
			{
				Debug.Log("[Underworld] loaded " + importedRaidZones +
					" raid map zone(s) from raid_stages_default.xml");
			}
			string compatFile = Path.Combine(GetDevXmlRoot(), "compat", "stages.xml");
			int imported = 0;
			if (custom.DocumentElement != null && File.Exists(compatFile))
			{
				XmlDocument compat = LoadPlainXml(compatFile);
				foreach (XmlElement compatZone in compat.SelectNodes("/Stages/Zones/Zone[@Name]"))
				{
					string zoneName = compatZone.GetAttribute("Name");
					XmlElement customZone = custom.SelectSingleNode(
						"/Stages/Zones/Zone[@Name='" + zoneName + "']") as XmlElement;
					if (customZone == null)
						continue;
					foreach (XmlElement compatBattle in compatZone.SelectNodes("./Battle[@Name]"))
					{
						string battleName = compatBattle.GetAttribute("Name");
						if (customZone.SelectSingleNode("./Battle[@Name='" + battleName + "']") != null)
							continue;
						customZone.AppendChild(custom.ImportNode(compatBattle, true));
						imported++;
					}
				}
			}
			if (imported != 0 && _devXmlLogged.Add("compat-stage-battles"))
			{
				Debug.Log("[DevXml] restored " + imported +
					" required battle definition(s) from recovered 2.41.9 data");
			}

			// Newer stage data can put a common enemy pool and common rules on the
			// Battle.  This legacy runtime only parses those nodes from each Fight.
			// Materialize the inheritance before handing the document to ListSF.
			int inheritedWarriors = 0;
			int inheritedRuleSets = 0;
			foreach (XmlElement battle in custom.SelectNodes("//Battle"))
			{
				XmlElement commonWarriors = battle["Warriors"];
				XmlElement commonRules = battle["Rules"];
				foreach (XmlElement fight in battle.SelectNodes("./Fight"))
				{
					if (fight["Warriors"] == null && commonWarriors != null)
					{
						fight.AppendChild(custom.ImportNode(commonWarriors, true));
						inheritedWarriors++;
					}
					if (commonRules != null)
					{
						XmlElement fightRules = fight["Rules"];
						if (fightRules == null)
						{
							fightRules = custom.CreateElement("Rules");
							fight.AppendChild(fightRules);
						}
						foreach (XmlNode commonRule in commonRules.ChildNodes)
						{
							fightRules.AppendChild(custom.ImportNode(commonRule, true));
						}
						inheritedRuleSets++;
					}
				}
			}
			if (inheritedWarriors != 0 && _devXmlLogged.Add("stages-battle-warriors"))
			{
				Debug.Log("[DevXml] inherited newer battle-level warrior pools into " +
					inheritedWarriors + " legacy fight definitions");
			}
			if (inheritedRuleSets != 0 && _devXmlLogged.Add("stages-battle-rules"))
			{
				Debug.Log("[DevXml] inherited newer battle-level rules into " +
					inheritedRuleSets + " legacy fight definitions");
			}
			int addedSurvivalRewardRows = EnsureSurvivalRewardRows(custom);
			if (addedSurvivalRewardRows != 0 && _devXmlLogged.Add("survival-reward-rows"))
			{
				Debug.Log("[Survival] supplied " + addedSurvivalRewardRows +
					" missing terminal reward row(s) for full-length survival battles");
			}

			// The old fight HUD has room for two timer digits.  New data commonly
			// requests 150 seconds, which appeared as 15 and then 14 as the clipped
			// third digit changed.  Preserve shorter challenge timers and clamp only
			// values the legacy presentation cannot display. Underworld raid
			// encounters keep their original long timers: their multi-bar fights
			// routinely outlast the HUD limit and a timeout would count as a loss.
				int clampedRoundTimes = 0;
				foreach (XmlElement timedNode in custom.SelectNodes("//*[@RoundTime]"))
				{
					if (UnderworldStageCompatibility.IsInsideRaidZone(timedNode))
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
			if (clampedRoundTimes != 0 && _devXmlLogged.Add("stages-round-time"))
			{
				Debug.Log("[DevXml] clamped " + clampedRoundTimes +
					" newer round timers to the legacy HUD maximum (99 seconds)");
			}
			// Server-driven raids use Rounds=0 because their shield/session ends
			// the encounter. The restored offline path has no raid server, so make
			// each boss a conventional one-round fight instead of an immediate
			// zero-round completion.
				int localizedRaidRounds = UnderworldStageCompatibility.AdaptOfflineRaidRounds(custom);
			if (localizedRaidRounds != 0 && _devXmlLogged.Add("underworld-local-rounds"))
			{
				Debug.Log("[Underworld] adapted " + localizedRaidRounds +
					" server raid encounter(s) into one-round local fights");
			}
			NormalizeFunctionSyntax(custom);
			return custom.OuterXml;
		}

		private static string AdaptDevXml(string requestPath, string file, string text)
		{
			if (string.Equals(Path.GetExtension(file), ".json", StringComparison.OrdinalIgnoreCase))
			{
				return text;
			}
			string fileName = Path.GetFileName(requestPath);
			if (string.Equals(fileName, "quests.xml", StringComparison.OrdinalIgnoreCase))
			{
				return AdaptQuests(file);
			}
			if (string.Equals(fileName, "internalSettings.xml", StringComparison.OrdinalIgnoreCase))
			{
				return AdaptInternalSettings(file);
			}
			if (string.Equals(fileName, "stages.xml", StringComparison.OrdinalIgnoreCase))
			{
				return AdaptStages(file);
			}
			XmlDocument document = LoadPlainXml(file);
			NormalizeFunctionSyntax(document);
			if (document.DocumentElement != null && document.DocumentElement.Name == "Quests")
			{
				NormalizeQuestFunctionSyntax(document);
				NormalizeQuestActions(document);
			}
			else if (document.DocumentElement != null && document.DocumentElement.Name == "Movesxml")
			{
				NormalizeMoves(document);
			}
			else if (document.DocumentElement != null && document.DocumentElement.Name == "List")
			{
				NormalizeList(document);
			}
			return document.OuterXml;
		}

		public static bool TryDevXml(string ONEIGMLOGDC, out string text)
		{
			text = null;
			string rel = ONEIGMLOGDC.Replace('\\', '/').TrimStart('/');
			int gamedataIndex = rel.IndexOf("gamedata/", StringComparison.OrdinalIgnoreCase);
			if (gamedataIndex < 0)
			{
				return false;
			}
			string gamedataRelativePath = rel.Substring(gamedataIndex + "gamedata/".Length);
			bool isModelRequest = IsDevModelRequest(gamedataRelativePath);
			if (!_devXmlGameplayOverridesEnabled && !isModelRequest)
			{
				if (_devXmlLogged.Add("gameplay-disabled"))
				{
					Debug.LogWarning("[DevXml] gameplay XML overrides disabled: schema adapters are not complete. " +
						"Compatible model overrides remain enabled.");
				}
				return false;
			}
			// The exported project already contains the recovered content. The legacy
			// external packs manifest otherwise treats later acts as absent and opens a
			// downloader that cannot install modern Unity bundles into this 2019 project.
			if (string.Equals(Path.GetFileName(gamedataRelativePath), "packs.xml",
				StringComparison.OrdinalIgnoreCase))
			{
				text = "<?xml version=\"1.0\" encoding=\"utf-8\"?><Packs />";
				if (_devXmlLogged.Add("local-packs-manifest"))
				{
					Debug.Log("[DevXml] local content mode: skipping obsolete pack downloads");
				}
				return true;
			}
			string root = GetDevXmlRoot();
			if (root == null || !Directory.Exists(root))
			{
				Debug.LogWarning("[DevXml] root missing: " + root);
				return false;
			}
			if (!_devXmlLogged.Contains("lookup:" + ONEIGMLOGDC))
			{
				_devXmlLogged.Add("lookup:" + ONEIGMLOGDC);
				Debug.Log("[DevXml] lookup: " + ONEIGMLOGDC);
			}
			var candidates = new List<string>();
			candidates.Add(gamedataRelativePath);
			string normalizedRelativePath = gamedataRelativePath.Replace('\\', '/');
			if (normalizedRelativePath.EndsWith("/params.xml", StringComparison.OrdinalIgnoreCase))
			{
				string directory = Path.GetDirectoryName(gamedataRelativePath);
				string locationName = Path.GetFileName(directory);
				if (!string.IsNullOrEmpty(directory) && !string.IsNullOrEmpty(locationName))
				{
					candidates.Add(Path.Combine(directory, locationName + "_params.xml"));
				}
			}
			if (gamedataRelativePath.StartsWith("assets/", StringComparison.OrdinalIgnoreCase))
			{
				candidates.Add(gamedataRelativePath.Substring("assets/".Length));
			}
			candidates.Add(Path.GetFileName(gamedataRelativePath));
			foreach (string cand in candidates)
			{
				if (string.IsNullOrEmpty(cand))
				{
					continue;
				}
				string file = Path.Combine(root, cand.Replace('/', Path.DirectorySeparatorChar));
				if (!File.Exists(file) && !file.EndsWith(".xml") && !file.EndsWith(".json"))
				{
					file = file + ".xml";
				}
				if (File.Exists(file))
				{
					try
					{
						text = File.ReadAllText(file);
						text = AdaptDevXml(ONEIGMLOGDC, file, text);
					}
					catch (Exception exception)
					{
						string logKey = "adapt-error:" + file;
						if (_devXmlLogged.Add(logKey))
						{
							Debug.LogError("[DevXml] failed to adapt " + file + ": " + exception);
						}
						return false;
					}
					if (!IsCompatibleDevXml(ONEIGMLOGDC, file, text))
					{
						text = null;
						continue;
					}
					if (_devXmlLogged.Add(file))
					{
						Debug.Log("[DevXml] override: " + ONEIGMLOGDC + " -> " + file);
					}
					return true;
				}
			}
			if (isModelRequest)
			{
				string requestedModel = Path.GetFileNameWithoutExtension(gamedataRelativePath);
				string fallbackModel;
				if (_devModelFallbacks.TryGetValue(requestedModel, out fallbackModel))
				{
					string fallbackFile = Path.Combine(root, "models", fallbackModel + ".xml");
					if (File.Exists(fallbackFile))
					{
						text = AdaptDevXml(ONEIGMLOGDC, fallbackFile, File.ReadAllText(fallbackFile));
						string logKey = "model-fallback:" + requestedModel;
						if (_devXmlLogged.Add(logKey))
						{
							Debug.LogWarning("[DevXml] unavailable model '" + requestedModel +
								"' uses compatibility fallback '" + fallbackModel + "'");
						}
						return true;
					}
				}
			}
			return false;
		}

	public static string GetText(string ONEIGMLOGDC, bool GIEAPLJHHDK = false)
	{
		if (TryDevXml(ONEIGMLOGDC, out var t0))
		{
			return t0;
		}

		if (SF2Paths.CGOHPKEBECD && !GIEAPLJHHDK)
		{
			return IJMMFCDCOAC(ONEIGMLOGDC);
		}
		return KIHHJGJKMIC(ONEIGMLOGDC);
	}

	// Loads the asset embedded in this Unity project without consulting the
	// plaintext override directory.  Visual layout XML must stay paired with
	// the installed textures/atlases even while gameplay XML is migrated.
	public static string GetBundledText(string ONEIGMLOGDC)
	{
		if (string.IsNullOrEmpty(ONEIGMLOGDC))
		{
			return string.Empty;
		}
		ONEIGMLOGDC = ONEIGMLOGDC.TrimStart('\\', '/');
		ONEIGMLOGDC = NNBCLAEKMIO(ONEIGMLOGDC);
		TextAsset textAsset = ResourcesAndBundles.Load<TextAsset>(ONEIGMLOGDC);
		return (!textAsset) ? string.Empty : textAsset.text;
	}

	public static string IJMMFCDCOAC(string ONEIGMLOGDC)
	{
		if (TryDevXml(ONEIGMLOGDC, out var t1))
		{
			return t1;
		}
		ONEIGMLOGDC = ONEIGMLOGDC.TrimStart('\\', '/');
		string text = SF2Paths.COGELDOPEJG(ONEIGMLOGDC);
		if (File.Exists(text))
		{
			return KIHHJGJKMIC(text);
		}
		ONEIGMLOGDC = NNBCLAEKMIO(ONEIGMLOGDC);
		TextAsset textAsset = ResourcesAndBundles.Load<TextAsset>(ONEIGMLOGDC);
		return (!textAsset) ? string.Empty : textAsset.text;
	}

	public static string KIHHJGJKMIC(string ONEIGMLOGDC)
	{
		if (TryDevXml(ONEIGMLOGDC, out var t2))
		{
			return t2;
		}
		if (ONEIGMLOGDC.StartsWith(SF2Paths.KKIDGPBOBNI()))
		{
			string path = ONEIGMLOGDC.Replace(SF2Paths.KKIDGPBOBNI(), SF2Paths.GBOFOFGDMBN());
			if (File.Exists(path))
			{
				return File.ReadAllText(path);
			}
			if (File.Exists(ONEIGMLOGDC))
			{
				return File.ReadAllText(ONEIGMLOGDC);
			}
			return null;
		}
		if (File.Exists(ONEIGMLOGDC))
		{
			return File.ReadAllText(ONEIGMLOGDC);
		}
		return null;
	}

	private static string NNBCLAEKMIO(string ONEIGMLOGDC)
	{
		if (Path.HasExtension(ONEIGMLOGDC))
		{
			return Path.ChangeExtension(ONEIGMLOGDC, null);
		}
		return ONEIGMLOGDC;
	}
}
