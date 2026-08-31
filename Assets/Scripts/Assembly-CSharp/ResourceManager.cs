using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Eclipse.Content;
using Eclipse.Underworld.Content;
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
#if UNITY_EDITOR
		byte[] previewAnimation;
		if (Eclipse.Content.LocalAnimationPreview.TryGetBinary(DCOPLCIFCFL, out previewAnimation))
			return previewAnimation;
#endif
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
				string reason;
				if (ContentOverrideCompatibility.Validate(
					xmlDocument,
					Path.GetFileName(requestPath),
					ContentOverridePaths.IsModelRequest(requestPath),
					out reason))
				{
					return true;
				}
				LogIncompatibleDevXml(file, reason);
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

		private static void NormalizeMoves(XmlDocument document)
		{
			// The newer table is additive and still omits definitions required by
			// this recovered runtime. ResourceManager owns access to the
			// bundled baseline; the schema transformations live in Eclipse.Content.
			try
			{
				TextAsset bundledMoves = ResourcesAndBundles.Load<TextAsset>("gamedata/animations/moves");
				if (bundledMoves != null && !string.IsNullOrEmpty(bundledMoves.text))
				{
					XmlDocument baseline = new XmlDocument();
					baseline.XmlResolver = null;
					baseline.LoadXml(bundledMoves.text);
					int restored = MoveCompatibility.MergeMissingLegacyDefinitions(document, baseline);
					if (restored != 0 && _devXmlLogged.Add("moves-legacy-merge"))
					{
						Debug.Log("[DevXml] restored " + restored +
							" legacy magic/ranged animation definitions missing from the newer table");
					}
				}
			}
			catch (Exception exception)
			{
				if (_devXmlLogged.Add("moves-legacy-merge-error"))
				{
					Debug.LogWarning("[DevXml] could not merge bundled move compatibility data: " + exception.Message);
				}
			}

			// Newer one-frame move ModFlag actions are supported by the compatibility
			// bridge in ActionsParser and must remain in the adapted document.
			foreach (MoveTemplateCompatibilityIssue issue in MoveCompatibility.RemoveUnavailableTemplates(document))
			{
				string missing = string.Join(", ", issue.MissingTemplates);
				string logKey = "move-template:" + issue.MoveName + ":" +
					string.Join("|", issue.MissingTemplates);
				if (_devXmlLogged.Add(logKey))
				{
					Debug.LogWarning("[DevXml] move '" + issue.MoveName +
						"' drops unavailable newer template(s): " + missing);
				}
			}
		}

		private static void NormalizeList(XmlDocument document)
		{
			int aliasesAdded = ItemListCompatibility.AddHistoricalStageAliases(document);
			if (aliasesAdded != 0 && _devXmlLogged.Add("stage-item-aliases"))
			{
				Debug.Log("[DevXml] restored " + aliasesAdded +
					" historical stage-only item aliases");
			}

			string modelsRoot = Path.Combine(GetDevXmlRoot(), "models");
			bool hasExternalModels = Directory.Exists(modelsRoot);
			int hidden;
			List<ModelFallbackMapping> fallbacks = ItemListCompatibility.HideUnavailableModelItems(
				document,
				model =>
				{
					if (hasExternalModels && File.Exists(Path.Combine(modelsRoot, model + ".xml")))
					{
						return true;
					}
					return Eclipse.Content.PackagedArtCatalog.HasModel(model) ||
						ResourcesAndBundles.Load<TextAsset>("gamedata/models/" + model) != null;
				},
				out hidden);
			foreach (ModelFallbackMapping fallback in fallbacks)
			{
				_devModelFallbacks[fallback.RequestedModel] = fallback.FallbackModel;
			}
			if (hidden != 0 && _devXmlLogged.Add("unresolved-shop-models"))
			{
				Debug.LogWarning("[DevXml] hid " + hidden +
					" optional shop items whose newer model assets are unavailable; owned items use safe model fallbacks");
			}
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
					QuestCompatibility.AddQuestWithConditions(output, outputRoot, child, inheritedConditions);
					continue;
				}
				if (child.Name != "Include" || child.Attributes == null || child.Attributes["File"] == null)
				{
					continue;
				}

				XmlNode includeConditions = child["Conditions"] ?? inheritedConditions;
				string root = Path.GetFullPath(GetDevXmlRoot()).TrimEnd(
					Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
				string includeFile = null;
				foreach (string includePath in child.Attributes["File"].Value.Split('|'))
				{
					string relativePath = includePath.Trim();
					if (relativePath.Length == 0)
					{
						continue;
					}
					string candidate = Path.GetFullPath(Path.Combine(root,
						relativePath.Replace('/', Path.DirectorySeparatorChar)));
					if (candidate.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) &&
						File.Exists(candidate))
					{
						includeFile = candidate;
						break;
					}
				}
				if (includeFile == null)
				{
					Debug.LogWarning("[DevXml] missing quest include: " + child.Attributes["File"].Value);
					continue;
				}
				if (!includeStack.Add(includeFile))
				{
					Debug.LogWarning("[DevXml] recursive quest include: " + includeFile);
					continue;
				}
				try
				{
					XmlDocument included = LoadPlainXml(includeFile);
					ExpandQuestContainer(output, outputRoot, included.DocumentElement, includeConditions, includeStack);
				}
				finally
				{
					includeStack.Remove(includeFile);
				}
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
			// Modern vanilla gates Eclipse Mode behind its mobile/Steam/Switch build
			// channels. Eclipse local PC mode intentionally exposes that gameplay
			// system without pretending to be one of those service platforms.
			string eclipseFile = Path.Combine(GetDevXmlRoot(), "quest_extensions", "eclipse.xml");
			if (output.DocumentElement != null && File.Exists(eclipseFile))
			{
				int promotedEclipseQuests = QuestCompatibility.PromoteLocalQuestExtension(
					output, LoadPlainXml(eclipseFile));
				if (promotedEclipseQuests != 0 && _devXmlLogged.Add("local-eclipse-quests"))
				{
					Debug.Log("[DevXml] enabled " + promotedEclipseQuests +
						" vanilla Eclipse Mode quest(s) for local PC content mode");
				}
			}
			int removedUpdateQuests = QuestCompatibility.RemoveObsoleteClientUpdateQuests(output);
			if (removedUpdateQuests != 0 && _devXmlLogged.Add("obsolete-client-update-quests"))
			{
				Debug.Log("[DevXml] disabled " + removedUpdateQuests +
					" obsolete mobile client update quest(s) in local content mode");
			}
			QuestCompatibility.NormalizeFunctionSyntax(output);
			QuestCompatibility.NormalizeActions(output);
			return output.OuterXml;
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
					int imported = InternalSettingsCompatibility.MergeMissingSettings(
						custom,
						custom.DocumentElement,
						compat.DocumentElement);
					if (imported != 0 && _devXmlLogged.Add("settings-schema-bridge"))
					{
						Debug.Log("[DevXml] restored " + imported +
							" missing legacy settings nodes/attributes");
					}
				}
			}

			// The 2.41.9 data no longer carries several top-level sections that this
			// older recovered runtime still reads directly. Import only those missing
			// runtime contract sections from the bundled baseline. Do not merge the
			// rest of the old settings over the vanilla document.
			TextAsset bundledSettings = ResourcesAndBundles.Load<TextAsset>("gamedata/internalSettings");
			if (bundledSettings != null && !string.IsNullOrEmpty(bundledSettings.text))
			{
				XmlDocument runtimeBaseline = new XmlDocument();
				runtimeBaseline.XmlResolver = null;
				runtimeBaseline.LoadXml(bundledSettings.text);
				int importedRuntimeSections = InternalSettingsCompatibility.ImportMissingTopLevelSections(
					custom,
					runtimeBaseline,
					new[] { "AssemblySettings", "Internet", "Supports", "EULA", "Log", "ForcedLogConditions", "StarterPackTimer" });
				int importedMapGuiSettings = InternalSettingsCompatibility.ImportMissingSubtree(
					custom,
					runtimeBaseline,
					"/Settings/GUI/Map/Challenge");
				if ((importedRuntimeSections != 0 || importedMapGuiSettings != 0) && _devXmlLogged.Add("settings-runtime-sections"))
				{
					Debug.Log("[DevXml] restored " + importedRuntimeSections +
						" legacy runtime settings section(s) and " + importedMapGuiSettings +
						" nested runtime setting(s) without replacing vanilla settings");
				}
			}

			int removedQualityOptions = InternalSettingsCompatibility.RemoveUnsupportedQualityOptions(custom);
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
			if (InternalSettingsCompatibility.DisableAlwaysMagicMode(custom) &&
				_devXmlLogged.Add("settings-disable-always-magic"))
			{
				Debug.Log("[DevXml] disabled newer AlwaysMagicMode developer cheat");
			}
			NormalizeFunctionSyntax(custom);
			return custom.OuterXml;
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
				imported = StageCompatibility.MergeMissingBattles(custom, LoadPlainXml(compatFile));
			}
			if (imported != 0 && _devXmlLogged.Add("compat-stage-battles"))
			{
				Debug.Log("[DevXml] restored " + imported +
					" required battle definition(s) from recovered 2.41.9 data");
			}

			// Newer stage data can put a common enemy pool and common rules on the
			// Battle. This legacy runtime only parses those nodes from each Fight.
			// Materialize the inheritance before handing the document to ListSF.
			int inheritedWarriors;
			int inheritedRuleSets;
			StageCompatibility.MaterializeBattleInheritance(custom, out inheritedWarriors, out inheritedRuleSets);
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

			int addedSurvivalRewardRows = StageCompatibility.EnsureSurvivalRewardRows(custom);
			if (addedSurvivalRewardRows != 0 && _devXmlLogged.Add("survival-reward-rows"))
			{
				Debug.Log("[Survival] supplied " + addedSurvivalRewardRows +
					" missing terminal reward row(s) for full-length survival battles");
			}

			// The old fight HUD has room for two timer digits. New data commonly
			// requests 150 seconds, which appeared as 15 and then 14 as the clipped
			// third digit changed. Preserve shorter challenge timers and clamp only
			// values the legacy presentation cannot display. Underworld raid
			// encounters keep their original long timers: their multi-bar fights
			// routinely outlast the HUD limit and a timeout would count as a loss.
			int clampedRoundTimes = StageCompatibility.ClampLegacyRoundTimes(
				custom,
				UnderworldStageCompatibility.IsInsideRaidZone);
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
				QuestCompatibility.NormalizeFunctionSyntax(document);
				QuestCompatibility.NormalizeActions(document);
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
			string gamedataRelativePath;
			if (!ContentOverridePaths.TryGetGamedataRelativePath(ONEIGMLOGDC, out gamedataRelativePath))
			{
				return false;
			}
			bool isModelRequest = ContentOverridePaths.IsModelRequest(gamedataRelativePath);
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
			if (ContentOverridePaths.IsPacksManifest(gamedataRelativePath))
			{
				text = ContentOverridePaths.EmptyPacksManifest;
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
			foreach (string cand in ContentOverridePaths.BuildCandidates(gamedataRelativePath))
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
					}
					else
					{
						text = Eclipse.Content.PackagedArtCatalog.LoadModelText("gamedata/models/" + fallbackModel);
						if (string.IsNullOrEmpty(text))
						{
							TextAsset bundledFallback = ResourcesAndBundles.Load<TextAsset>("gamedata/models/" + fallbackModel);
							if (bundledFallback == null)
							{
								return false;
							}
							text = bundledFallback.text;
						}
					}
					string logKey = "model-fallback:" + requestedModel;
					if (_devXmlLogged.Add(logKey))
					{
						Debug.LogWarning("[DevXml] unavailable model '" + requestedModel +
							"' uses compatibility fallback '" + fallbackModel + "'");
					}
					return true;
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
		string modModel = Eclipse.Modding.ModRuntime.LoadQualifiedModelText(ONEIGMLOGDC);
		if (!string.IsNullOrEmpty(modModel))
		{
			return modModel;
		}
		ONEIGMLOGDC = ONEIGMLOGDC.TrimStart('\\', '/');
		string text = SF2Paths.COGELDOPEJG(ONEIGMLOGDC);
		if (File.Exists(text))
		{
			return KIHHJGJKMIC(text);
		}
		string packagedModel = Eclipse.Content.PackagedArtCatalog.LoadModelText(ONEIGMLOGDC);
		if (!string.IsNullOrEmpty(packagedModel))
		{
			return packagedModel;
		}
		string packagedLocationData = Eclipse.Content.PackagedArtCatalog.LoadLocationDataText(ONEIGMLOGDC);
		if (!string.IsNullOrEmpty(packagedLocationData))
		{
			return packagedLocationData;
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
