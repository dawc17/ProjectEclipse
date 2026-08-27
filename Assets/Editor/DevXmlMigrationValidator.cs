using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;

public static class DevXmlMigrationValidator
{
	private sealed class XmlCase
	{
		public readonly string Path;
		public readonly string Root;

		public XmlCase(string path, string root)
		{
			Path = path;
			Root = root;
		}
	}

	[MenuItem("Tools/SF2/Validate Custom XML Migration")]
	public static void ValidateFromMenu()
	{
		RunValidation(false);
	}

	// Command-line entry point used by the migration smoke test.
	public static void ValidateBatch()
	{
		RunValidation(true);
	}

	private static void RunValidation(bool exitWhenFinished)
	{
		var failures = new List<string>();
		var cases = new[]
		{
			new XmlCase("gamedata/list.xml", "List"),
			new XmlCase("gamedata/stages.xml", "Stages"),
			new XmlCase("gamedata/animations/moves.xml", "Movesxml"),
			new XmlCase("gamedata/perks.xml", "Perks"),
			new XmlCase("gamedata/forge.xml", "Forge"),
			new XmlCase("gamedata/internalSettings.xml", "Settings"),
			new XmlCase("gamedata/quests.xml", "Quests"),
			new XmlCase("gamedata/quest_extensions/zone_1/story.xml", "Quests"),
			new XmlCase("gamedata/locations/arena/params.xml", "Root"),
			new XmlCase("gamedata/models/mdl_magic_bomb.xml", "Scene"),
			new XmlCase("gamedata/packs.xml", "Packs")
		};

		var loaded = new Dictionary<string, XmlDocument>(StringComparer.OrdinalIgnoreCase);
		foreach (XmlCase testCase in cases)
		{
			string text;
			if (!ResourceManager.TryDevXml(testCase.Path, out text))
			{
				failures.Add(testCase.Path + ": loader returned false");
				continue;
			}
			try
			{
				var document = new XmlDocument();
				document.LoadXml(text);
				loaded[testCase.Path] = document;
				string actualRoot = document.DocumentElement == null ? "<null>" : document.DocumentElement.Name;
				if (!string.Equals(actualRoot, testCase.Root, StringComparison.Ordinal))
					failures.Add(testCase.Path + ": expected <" + testCase.Root + ">, got <" + actualRoot + ">");
			}
			catch (Exception exception)
			{
				failures.Add(testCase.Path + ": " + exception.Message);
			}
		}

		XmlDocument quests;
		if (loaded.TryGetValue("gamedata/quests.xml", out quests))
		{
			int questCount = quests.SelectNodes("/Quests/Quest").Count;
			// The root Include is expanded immediately. AttachQuestFile actions are
			// intentionally retained because the quest manager loads those zone files
			// as progression advances; flattening all 712 quests here would change
			// activation order and create duplicate registrations.
			if (questCount < 300)
				failures.Add("quests.xml: expected the expanded root quest graph, got " + questCount + " quests");
			if (quests.SelectNodes("//AttachQuestFile").Count < 20)
				failures.Add("quests.xml: dynamic zone quest attachments disappeared during adaptation");
			// Quest actions are element names (<If>, <ChangeDojoLocation>, ...), not
			// <Action Type='...'> records.
			if (quests.SelectSingleNode("//If") == null ||
				quests.SelectSingleNode("//ChangeDojoLocation") == null)
				failures.Add("quests.xml: required newer quest actions disappeared during adaptation");
			foreach (XmlAttribute attribute in quests.SelectNodes("//@*"))
			{
				if (attribute.Value.IndexOf('?') >= 0 && attribute.Value.IndexOf('[') >= 0)
				{
					failures.Add("quests.xml: bracketed function reached the parenthesis-based quest evaluator: " + attribute.Value);
					break;
				}
			}
		}

		XmlDocument moves;
		if (loaded.TryGetValue("gamedata/animations/moves.xml", out moves))
		{
			if (moves.SelectNodes("//Actions/ModFlag").Count != 6)
				failures.Add("moves.xml: newer projectile ModFlag actions disappeared during adaptation");
			if (moves.SelectSingleNode("//Actions/SetEndStage") == null ||
				moves.SelectSingleNode("//Actions/PlayAnimation") == null)
				failures.Add("moves.xml: ported newer animation actions disappeared during adaptation");

			foreach (string requiredMove in new[]
			{
				"EnergyballStart", "EnergyballMiddle", "EnergyballWall", "EnergyballPlayer",
				"IceballStart", "IceballMiddle", "IceballWall", "IceballPlayer",
				"EarthStrikeStart", "EarthStrikeMiddle", "EarthStrikePlayer", "FireballWall",
				"HermitStormStart", "ShopMagicEnergyball", "ShopMagicEnergyballPlayer"
			})
			{
				if (moves.SelectSingleNode("//Move[@Name='" + requiredMove + "']") == null)
					failures.Add("moves.xml: required legacy magic/ranged move missing: " + requiredMove);
			}

			var effectAliases = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
			{
				"mgc_effect_prediction_start", "mgc_effect_prediction_loop",
				"mgc_effect_prediction_end", "mgc_surge_time_effec"
			};
			string effectRoot = Path.Combine(Application.dataPath, "Resources/textures/effects/magic");
			foreach (XmlElement effect in moves.SelectNodes("//Actions/Effect[@Sequence]"))
			{
				string sequence = effect.GetAttribute("Sequence");
				if (effectAliases.Contains(sequence))
					continue;
				string metadata = Path.Combine(effectRoot, sequence.Replace('/', Path.DirectorySeparatorChar) + "_xml.txt");
				if (!File.Exists(metadata))
					failures.Add("moves.xml: effect sequence has no recovered metadata: " + sequence);
			}

			string soundRoot = Path.Combine(Application.dataPath, "Resources/gamedata/sounds");
			var checkedSounds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (XmlElement sound in moves.SelectNodes("//Actions//Sound[@Name] | //Actions/Sound[@Name] | //Actions/StopSound[@Name]"))
			{
				string soundName = sound.GetAttribute("Name");
				if (!checkedSounds.Add(soundName))
					continue;
				string relative = soundName.Replace('/', Path.DirectorySeparatorChar);
				if (!File.Exists(Path.Combine(soundRoot, relative + ".wav")) &&
					!File.Exists(Path.Combine(soundRoot, relative + ".ogg")) &&
					!File.Exists(Path.Combine(soundRoot, relative + ".mp3")))
					failures.Add("moves.xml: sound has no recovered clip: " + soundName);
			}
		}

		XmlDocument stages;
		if (loaded.TryGetValue("gamedata/stages.xml", out stages) &&
			stages.SelectSingleNode("/Stages/Zones/Zone[@Name='ZONE_7']/Battle[@Name='BOSS_TITAN']") == null)
		{
			failures.Add("stages.xml: recovered ZONE_7/BOSS_TITAN battle is missing");
		}

		XmlDocument perks;
		if (loaded.TryGetValue("gamedata/perks.xml", out perks))
		{
			foreach (string action in new[]
			{
				"ChangeModelColor", "SlowModel", "TurnOffCollision", "Switch",
				"MarkPerkAsUsed", "PerkArea", "MoveModel", "SetMovesVariable", "StealMagicMod"
			})
			{
				if (perks.SelectSingleNode("//Trigger/Actions/" + action) == null)
					failures.Add("perks.xml: migrated action disappeared: " + action);
			}
			if (perks.SelectNodes("//Trigger/Events/IntervalEnd").Count != 5)
				failures.Add("perks.xml: IntervalEnd enchantment triggers disappeared");
			int functionCount = 0;
			foreach (XmlAttribute attribute in perks.SelectNodes("//@*"))
			{
				string value = attribute.Value;
				if (!string.IsNullOrEmpty(value) && value.IndexOf('?') >= 0)
				{
					functionCount++;
					int functionStart = value.IndexOf('?');
					int bracket = value.IndexOf('[', functionStart);
					int parenthesis = value.IndexOf('(', functionStart);
					if (parenthesis >= 0 && (bracket < 0 || parenthesis < bracket))
					{
						failures.Add("perks.xml: function brackets were rewritten: " + value);
						break;
					}
				}
			}
			if (functionCount == 0)
				failures.Add("perks.xml: expected function expressions were not loaded");
		}

		XmlDocument settings;
		if (loaded.TryGetValue("gamedata/internalSettings.xml", out settings))
		{
			XmlElement alwaysMagic = settings.SelectSingleNode("/Settings/AlwaysMagicMode") as XmlElement;
			if (alwaysMagic == null || alwaysMagic.GetAttribute("Value") != "0")
				failures.Add("internalSettings.xml: AlwaysMagicMode developer cheat is enabled");
			foreach (string section in new[] { "AssemblySettings", "EULA", "Internet", "Log", "Supports" })
			{
				if (settings.DocumentElement[section] == null)
					failures.Add("internalSettings.xml: compatibility section missing: " + section);
			}
			XmlElement assemblySettings = settings.SelectSingleNode("/Settings/AssemblySettings") as XmlElement;
			foreach (string setting in new[]
			{
				"CacheTexturesLog", "ShowIntro", "ShowController", "ShowPVP", "AiEnabled",
				"SkipContentDownload", "SkipPayment", "ShowSensitiveArea",
				"ControllerPrimaryAngle", "ControllerGripRelativeRadius", "ETCEnabled",
				"EnableNotifications", "Market", "ShowCrashButtons", "Gamepad", "ShowTimeResults"
			})
			{
				if (assemblySettings == null || assemblySettings[setting] == null)
					failures.Add("internalSettings.xml: AssemblyController setting missing: " + setting);
			}
			foreach (XmlElement option in settings.SelectNodes("/Settings/QualityOptions/Option[@Name]"))
			{
				string name = option.GetAttribute("Name");
				if (name != "ReduceFPS" && name != "ParticlesOff" && name != "SequencesOff")
					failures.Add("internalSettings.xml: unsupported legacy quality option: " + name);
			}
		}

		XmlDocument list;
		if (loaded.TryGetValue("gamedata/list.xml", out list))
		{
			XmlElement claw = list.SelectSingleNode("/List/Items/Item[@Name='WEAPON_KUSARIGAMA_CLAW_OF_WISDOM_26']") as XmlElement;
			if (claw == null || claw.GetAttribute("ShopHide") != "1")
				failures.Add("list.xml: unavailable Weekly Offer 42 model was not quarantined");
			foreach (string alias in new[]
			{
				"ARMOR_IM_CEREMONIAL", "HELM_IM_CEREMONIAL", "MAGIC_DARK_WAVE",
				"RANGED_NEEDLES", "WEAPON_SPEAR"
			})
			{
				if (list.SelectSingleNode("/List/Items/Item[@Name='" + alias + "']") == null)
					failures.Add("list.xml: historical stage item alias missing: " + alias);
			}
		}

		// Loading list.xml registers model fallbacks. Verify a hidden newer boss model
		// resolves to valid legacy-compatible scene XML rather than aborting a fight.
		string fallbackText;
		if (!ResourceManager.TryDevXml("gamedata/models/mdl_body_pristess.xml", out fallbackText))
		{
			failures.Add("model fallback: mdl_body_pristess did not resolve");
		}
		else
		{
			try
			{
				var fallback = new XmlDocument();
				fallback.LoadXml(fallbackText);
				if (fallback.DocumentElement == null || fallback.DocumentElement.Name != "Scene" ||
					fallback.SelectSingleNode("/Scene/Figures") == null)
					failures.Add("model fallback: returned XML is not a model scene");
			}
			catch (Exception exception)
			{
				failures.Add("model fallback: " + exception.Message);
			}
		}

		if (failures.Count == 0)
		{
			Debug.Log("[DevXmlValidation] PASS: custom XML loader, adapters, locations, models, quests and local-content mode are coherent.");
			if (exitWhenFinished)
				EditorApplication.Exit(0);
			return;
		}

		foreach (string failure in failures)
			Debug.LogError("[DevXmlValidation] " + failure);
		if (exitWhenFinished)
			EditorApplication.Exit(1);
		else
			throw new InvalidOperationException("Custom XML migration validation failed; see Console for details.");
	}
}
