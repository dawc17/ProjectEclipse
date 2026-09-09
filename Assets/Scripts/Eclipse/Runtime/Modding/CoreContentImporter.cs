using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Eclipse.Modding
{
    // A read-only projection of vanilla definitions. LegacyItemXml retains fields not yet
    // modeled by the public API; this importer must not reconstruct or replace ItemInfo.
    public static partial class CoreContentImporter
    {
        public static DefinitionId WeaponId(string legacyName)
        {
            return DefinitionId.Parse("core:items/weapon/" + legacyName);
        }

        public static DefinitionId ArmorId(string legacyName)
        {
            return DefinitionId.Parse("core:items/armor/" + legacyName);
        }

        public static DefinitionId HelmId(string legacyName)
        {
            return DefinitionId.Parse("core:items/helm/" + legacyName);
        }

        public static DefinitionId RangedId(string legacyName)
        {
            return DefinitionId.Parse("core:items/ranged/" + legacyName);
        }

        public static DefinitionId MagicId(string legacyName)
        {
            return DefinitionId.Parse("core:items/magic/" + legacyName);
        }

        public static DefinitionId PerkId(string legacyName)
        {
            return DefinitionId.Parse("core:perks/" + legacyName);
        }

        public static DefinitionId ZoneId(string legacyName)
        {
            return DefinitionId.Parse("core:zones/" + StageSegment(legacyName));
        }

        public static DefinitionId BattleId(string zoneLegacyName, string battleLegacyName)
        {
            return DefinitionId.Parse("core:battles/" + StageSegment(zoneLegacyName) + "/" + StageSegment(battleLegacyName));
        }

        public static DefinitionId FightId(string zoneLegacyName, string battleLegacyName, string fightLegacyName)
        {
            return DefinitionId.Parse("core:fights/" + StageSegment(zoneLegacyName) + "/" +
                StageSegment(battleLegacyName) + "/" + StageSegment(fightLegacyName));
        }

        public static int ImportStages(ModContentCatalog catalog, XmlNode zonesRoot)
        {
            if (catalog == null) throw new ArgumentNullException(nameof(catalog));
            if (zonesRoot == null) throw new ArgumentNullException(nameof(zonesRoot));
            var zones = new List<ZoneDefinition>();
            var battles = new List<BattleDefinition>();
            var fights = new List<FightDefinition>();
            var seen = new HashSet<DefinitionId>();

            foreach (XmlNode zoneNode in zonesRoot.ChildNodes)
            {
                if (zoneNode.NodeType != XmlNodeType.Element || zoneNode.Name != "Zone") continue;
                string zoneName = RequiredAttribute(zoneNode, "Name", "core zone");
                DefinitionId zoneId = ZoneId(zoneName);
                if (!seen.Add(zoneId)) throw new ModContentException("Duplicate projected core zone ID: '" + zoneId + "'.");
                var battleIds = new List<DefinitionId>();
                foreach (XmlNode battleNode in zoneNode.ChildNodes)
                {
                    if (battleNode.NodeType != XmlNodeType.Element || battleNode.Name != "Battle") continue;
                    string battleName = RequiredAttribute(battleNode, "Name", "core battle");
                    DefinitionId battleId = BattleId(zoneName, battleName);
                    if (!seen.Add(battleId)) throw new ModContentException("Duplicate projected core battle ID: '" + battleId + "'.");
                    battleIds.Add(battleId);
                    var fightIds = new List<DefinitionId>();
                    foreach (XmlNode fightNode in battleNode.SelectNodes("Fight"))
                    {
                        string fightName = RequiredAttribute(fightNode, "Name", "core fight");
                        DefinitionId fightId = FightId(zoneName, battleName, fightName);
                        if (!seen.Add(fightId)) throw new ModContentException("Duplicate projected core fight ID: '" + fightId + "'.");
                        fightIds.Add(fightId);
                        fights.Add(new FightDefinition(fightId, battleId, fightName,
                            IntAttribute(fightNode, "Replays", 0),
                            IntAttribute(fightNode, "ReplayInterval", 0),
                            IntAttribute(fightNode, "Power", 1),
                            IntAttribute(fightNode, "Rounds", 2),
                            IntAttribute(fightNode, "RoundTime", 60),
                            StringAttribute(fightNode, "Location", string.Empty),
                            StringAttribute(fightNode, "Music", string.Empty),
                            FloatAttribute(fightNode, "EvaluatedRating", -1f),
                            FloatAttribute(fightNode, "HealthRecovery", 1f),
                            StringAttribute(fightNode, "Description", string.Empty),
                            BoolAttribute(fightNode, "Locked", false),
                            StringAttribute(fightNode, "RewardImage", string.Empty),
                            Array.Empty<DefinitionId>(), Array.Empty<DefinitionId>(), Array.Empty<DefinitionId>(),
                            fightNode.OuterXml));
                    }
                    battles.Add(new BattleDefinition(battleId, zoneId, battleName,
                        ParseBattleKind(StringAttribute(battleNode, "Type", "DUMMY")),
                        IntAttribute(battleNode, "X", 0), IntAttribute(battleNode, "Y", 0),
                        StringAttribute(battleNode, "Alias", string.Empty),
                        StringAttribute(battleNode, "Title", string.Empty),
                        StringAttribute(battleNode, "Icon", "training"),
                        StringAttribute(battleNode, "Preview", string.Empty),
                        StringAttribute(battleNode, "Description", string.Empty),
                        StringAttribute(battleNode, "Location", string.Empty),
                        StringAttribute(battleNode, "Music", string.Empty),
                        StringAttribute(battleNode, "RewardImage", string.Empty),
                        BoolAttribute(battleNode, "ShowResistance", false), fightIds.ToArray(),
                        StringAttribute(battleNode, "IconAtlas", string.Empty),
                        StringAttribute(battleNode, "EclipseToggleName", string.Empty), battleNode.OuterXml));
                }
                zones.Add(new ZoneDefinition(zoneId, zoneName,
                    StringAttribute(zoneNode, "FileName", string.Empty),
                    IntAttribute(zoneNode, "Start", 0) > 0, battleIds.ToArray()));
            }
            catalog.ImportCoreStages(zones.ToArray(), battles.ToArray(), fights.ToArray());
            return fights.Count;
        }

        private static ModBattleKind ParseBattleKind(string value)
        {
            switch (value)
            {
                case "TUTORIAL": return ModBattleKind.Tutorial;
                case "CHALLENGE": return ModBattleKind.Challenge;
                case "BOSSES": return ModBattleKind.Bosses;
                case "TOURNAMENT": return ModBattleKind.Tournament;
                case "STORY": return ModBattleKind.Story;
                case "SURVIVAL": return ModBattleKind.Survival;
                case "TACTICS": return ModBattleKind.Friendly;
                case "AUTO": return ModBattleKind.Auto;
                case "AI": return ModBattleKind.Ai;
                case "HIDDEN": return ModBattleKind.Hidden;
                case "FAKE": return ModBattleKind.Fake;
                case "PVP": return ModBattleKind.Pvp;
                case "PERIODIC": return ModBattleKind.Periodic;
                case "FINAL_BATTLE": return ModBattleKind.Final;
                case "FINAL_BATTLE_REPLAYABLE": return ModBattleKind.FinalReplayable;
                case "BOSSES_INTERMISSION": return ModBattleKind.BossesIntermission;
                case "REPLAYABLE": return ModBattleKind.Replayable;
                case "BOSSES_REPLAYABLE": return ModBattleKind.BossesReplayable;
                case "FINAL_BATTLE_TITAN": return ModBattleKind.FinalTitan;
                case "ASCENSION": return ModBattleKind.Ascension;
                case "RAID": return ModBattleKind.Raid;
                case "DUMMY": return ModBattleKind.Dummy;
                default: throw new ModContentException("Unsupported core battle type '" + value + "'.");
            }
        }

        private static string StageSegment(string value)
        {
            if (string.IsNullOrEmpty(value)) throw new ModContentException("Stage identity must not be empty.");
            var builder = new System.Text.StringBuilder(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                char c = char.ToLowerInvariant(value[i]);
                bool safe = (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c == '_' || c == '-' || c == '.';
                builder.Append(safe ? c : '_');
            }
            return builder.ToString();
        }

        private static string RequiredAttribute(XmlNode node, string name, string kind)
        {
            string value = StringAttribute(node, name, string.Empty);
            if (string.IsNullOrEmpty(value)) throw new ModContentException(kind + " requires attribute '" + name + "'.");
            return value;
        }

        private static string StringAttribute(XmlNode node, string name, string fallback)
        {
            XmlAttribute attribute = node?.Attributes?[name];
            return attribute == null ? fallback : attribute.Value;
        }

        private static int IntAttribute(XmlNode node, string name, int fallback)
        {
            int value;
            return int.TryParse(StringAttribute(node, name, string.Empty), NumberStyles.Integer,
                CultureInfo.InvariantCulture, out value) ? value : fallback;
        }

        private static float FloatAttribute(XmlNode node, string name, float fallback)
        {
            float value;
            return float.TryParse(StringAttribute(node, name, string.Empty), NumberStyles.Float,
                CultureInfo.InvariantCulture, out value) ? value : fallback;
        }

        private static bool BoolAttribute(XmlNode node, string name, bool fallback)
        {
            string value = StringAttribute(node, name, string.Empty);
            if (value == "1" || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase)) return true;
            if (value == "0" || string.Equals(value, "false", StringComparison.OrdinalIgnoreCase)) return false;
            return fallback;
        }

        public static int ImportWeapons(ModContentCatalog catalog, IEnumerable<XmlNode> source,
            IReadOnlyDictionary<string, XmlDocument> languages)
        {
            if (catalog == null) throw new ArgumentNullException(nameof(catalog));
            if (source == null) throw new ArgumentNullException(nameof(source));
            var translations = BuildTranslations(languages);

            var weapons = new List<WeaponDefinition>();
            var localizations = new List<LocalizationDefinition>();
            foreach (XmlNode node in source)
            {
                if (node?.Attributes?["Type"]?.Value != "Weapon") continue;
                string name = node.Attributes["Name"]?.Value;
                if (string.IsNullOrEmpty(name) || name.IndexOf(':') >= 0)
                    throw new ModContentException("Core weapon requires an unqualified legacy Name.");
                DefinitionId id = WeaponId(name);
                DefinitionId displayName = ResolveLocalization(catalog, localizations, name, translations);

                int damage = ParseIntAttribute(node, "WeaponDamage", name);
                AssetId modelId = ParseModel(node);
                // Legacy Image values can be atlas members or fallback resource names. Keep
                // those verbatim in LegacyItemXml instead of inventing exact typed asset IDs.
                weapons.Add(new WeaponDefinition(id, displayName, default, modelId,
                    node.Attributes["SubType"]?.Value ?? string.Empty, damage, name, node.OuterXml));
            }
            catalog.ImportCore(localizations.ToArray(), weapons.ToArray());
            return weapons.Count;
        }

        public static int ImportArmors(ModContentCatalog catalog, IEnumerable<XmlNode> source,
            IReadOnlyDictionary<string, XmlDocument> languages)
        {
            if (catalog == null) throw new ArgumentNullException(nameof(catalog));
            if (source == null) throw new ArgumentNullException(nameof(source));
            var translations = BuildTranslations(languages);
            var armors = new List<ArmorDefinition>();
            var localizations = new List<LocalizationDefinition>();
            foreach (XmlNode node in source)
            {
                if (node?.Attributes?["Type"]?.Value != "Armor") continue;
                string name = node.Attributes["Name"]?.Value;
                if (string.IsNullOrEmpty(name) || name.IndexOf(':') >= 0)
                    throw new ModContentException("Core armor requires an unqualified legacy Name.");
                DefinitionId displayName = ResolveLocalization(catalog, localizations, name, translations);
                armors.Add(new ArmorDefinition(ArmorId(name), displayName, default, ParseModel(node),
                    ParseIntAttribute(node, "BodyDefense", name),
                    ParseIntAttribute(node, "HeadDefense", name),
                    ParseIntAttribute(node, "UnarmedDamage", name), name, node.OuterXml));
            }
            catalog.ImportCore(localizations.ToArray(), Array.Empty<WeaponDefinition>(), armors.ToArray());
            return armors.Count;
        }

        public static int ImportHelms(ModContentCatalog catalog, IEnumerable<XmlNode> source,
            IReadOnlyDictionary<string, XmlDocument> languages)
        {
            if (catalog == null) throw new ArgumentNullException(nameof(catalog));
            if (source == null) throw new ArgumentNullException(nameof(source));
            var translations = BuildTranslations(languages);
            var helms = new List<HelmDefinition>();
            var localizations = new List<LocalizationDefinition>();
            foreach (XmlNode node in source)
            {
                if (node?.Attributes?["Type"]?.Value != "Helm") continue;
                string name = RequireLegacyName(node, "helm");
                DefinitionId displayName = ResolveLocalization(catalog, localizations, name, translations);
                helms.Add(new HelmDefinition(HelmId(name), displayName, default, ParseModel(node),
                    ParseIntAttribute(node, "HeadDefense", name), name, node.OuterXml));
            }
            catalog.ImportCore(localizations.ToArray(), Array.Empty<WeaponDefinition>(),
                Array.Empty<ArmorDefinition>(), helms.ToArray());
            return helms.Count;
        }

        public static int ImportRanged(ModContentCatalog catalog, IEnumerable<XmlNode> source,
            IReadOnlyDictionary<string, XmlDocument> languages)
        {
            if (catalog == null) throw new ArgumentNullException(nameof(catalog));
            if (source == null) throw new ArgumentNullException(nameof(source));
            var translations = BuildTranslations(languages);
            var ranged = new List<RangedDefinition>();
            var localizations = new List<LocalizationDefinition>();
            var nameOrdinals = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (XmlNode node in source)
            {
                if (node?.Attributes?["Type"]?.Value != "Ranged") continue;
                string name = RequireLegacyName(node, "ranged item");
                int ordinal;
                if (!nameOrdinals.TryGetValue(name, out ordinal)) ordinal = 0;
                nameOrdinals[name] = ordinal + 1;
                DefinitionId displayName = ResolveLocalization(catalog, localizations, name, translations);
                string subType = node.Attributes["SubType"]?.Value ?? string.Empty;
                DefinitionId id = ordinal == 0 ? RangedId(name) : DuplicateRangedId(name, subType, ordinal + 1);
                ranged.Add(new RangedDefinition(id, displayName, default, ParseModel(node), subType,
                    ParseIntAttribute(node, "RangedDamage", name),
                    ParseIntAttribute(node, "WeaponDamage", name), name, node.OuterXml));
            }
            catalog.ImportCore(localizations.ToArray(), Array.Empty<WeaponDefinition>(),
                Array.Empty<ArmorDefinition>(), Array.Empty<HelmDefinition>(), ranged.ToArray());
            return ranged.Count;
        }

        public static int ImportMagic(ModContentCatalog catalog, IEnumerable<XmlNode> source,
            IReadOnlyDictionary<string, XmlDocument> languages)
        {
            if (catalog == null) throw new ArgumentNullException(nameof(catalog));
            if (source == null) throw new ArgumentNullException(nameof(source));
            var translations = BuildTranslations(languages);
            var magic = new List<MagicDefinition>();
            var localizations = new List<LocalizationDefinition>();
            foreach (XmlNode node in source)
            {
                if (node?.Attributes?["Type"]?.Value != "Magic") continue;
                string name = RequireLegacyName(node, "magic item");
                DefinitionId displayName = ResolveLocalization(catalog, localizations, name, translations);
                magic.Add(new MagicDefinition(MagicId(name), displayName, default, ParseModel(node),
                    node.Attributes["SubType"]?.Value ?? string.Empty,
                    ParseIntAttribute(node, "MagicDamage", name), name, node.OuterXml));
            }
            catalog.ImportCore(localizations.ToArray(), Array.Empty<WeaponDefinition>(),
                Array.Empty<ArmorDefinition>(), Array.Empty<HelmDefinition>(),
                Array.Empty<RangedDefinition>(), magic.ToArray());
            return magic.Count;
        }

        public static int ImportPerks(ModContentCatalog catalog, IEnumerable<XmlNode> source)
        {
            if (catalog == null) throw new ArgumentNullException(nameof(catalog));
            if (source == null) throw new ArgumentNullException(nameof(source));
            var perks = new List<PerkDefinition>();
            foreach (XmlNode node in source)
            {
                if (node?.NodeType != XmlNodeType.Element || node.Name != "Perk") continue;
                string name = RequireLegacyName(node, "perk");
                ModPerkKind kind = string.Equals(node.Attributes?["PerkType"]?.Value, "Combo",
                    StringComparison.OrdinalIgnoreCase) ? ModPerkKind.Combo : ModPerkKind.Single;
                perks.Add(new PerkDefinition(PerkId(name), default(DefinitionId), false,
                    default(DefinitionId), default(DefinitionId), default(AssetId), kind,
                    legacyName: name, legacyPerkXml: node.OuterXml));
            }
            catalog.ImportCorePerks(perks.ToArray());
            return perks.Count;
        }

        public static Dictionary<string, XmlDocument> ReadLocalizations(string directory)
        {
            var result = new Dictionary<string, XmlDocument>(StringComparer.Ordinal);
            foreach (string file in Directory.GetFiles(directory, "*.xml"))
            {
                var document = new XmlDocument { XmlResolver = null };
                using (XmlReader reader = XmlReader.Create(file, new XmlReaderSettings
                    { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null })) document.Load(reader);
                result.Add(Path.GetFileNameWithoutExtension(file).ToLowerInvariant(), document);
            }
            return result;
        }

        private static Dictionary<string, Dictionary<string, string>> BuildTranslations(
            IReadOnlyDictionary<string, XmlDocument> languages)
        {
            var translations = new Dictionary<string, Dictionary<string, string>>(StringComparer.Ordinal);
            if (languages == null) return translations;
            foreach (KeyValuePair<string, XmlDocument> language in languages)
            {
                var words = new Dictionary<string, string>(StringComparer.Ordinal);
                foreach (XmlNode word in language.Value.SelectNodes("/Localization/Words/Word"))
                {
                    string title = word.Attributes?["Title"]?.Value;
                    if (!string.IsNullOrEmpty(title)) words[title] = word.InnerText;
                }
                translations.Add(language.Key.ToLowerInvariant(), words);
            }
            return translations;
        }

        private static LocalizationDefinition BuildLocalization(string name,
            IReadOnlyDictionary<string, Dictionary<string, string>> translations)
        {
            DefinitionId id = DefinitionId.Parse("core:localization/" + name);
            var values = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (KeyValuePair<string, Dictionary<string, string>> language in translations)
            {
                string value;
                if (language.Value.TryGetValue(name, out value) && !string.IsNullOrEmpty(value))
                    values.Add(language.Key, value);
            }
            // Hidden/NPC-only definitions can legitimately have no localized label.
            if (!values.ContainsKey("eng")) values.Add("eng", name);
            return new LocalizationDefinition(id, values, name);
        }

        private static DefinitionId ResolveLocalization(ModContentCatalog catalog,
            List<LocalizationDefinition> pending, string name,
            IReadOnlyDictionary<string, Dictionary<string, string>> translations)
        {
            LocalizationDefinition candidate = BuildLocalization(name, translations);
            LocalizationDefinition existing;
            if (catalog.TryGetLocalization(candidate.Id, out existing))
            {
                if (!SameLocalization(existing, candidate))
                    throw new ModContentException("Conflicting core localization projection: " + candidate.Id);
                return existing.Id;
            }
            for (int i = 0; i < pending.Count; i++)
            {
                if (pending[i].Id != candidate.Id) continue;
                if (!SameLocalization(pending[i], candidate))
                    throw new ModContentException("Conflicting pending core localization projection: " + candidate.Id);
                return pending[i].Id;
            }
            pending.Add(candidate);
            return candidate.Id;
        }

        private static bool SameLocalization(LocalizationDefinition left, LocalizationDefinition right)
        {
            if (!string.Equals(left.LegacyKey, right.LegacyKey, StringComparison.Ordinal)) return false;
            if (left.Values.Count != right.Values.Count) return false;
            foreach (KeyValuePair<string, string> pair in left.Values)
            {
                string value;
                if (!right.Values.TryGetValue(pair.Key, out value) ||
                    !string.Equals(pair.Value, value, StringComparison.Ordinal)) return false;
            }
            return true;
        }

        private static string RequireLegacyName(XmlNode node, string type)
        {
            string name = node.Attributes?["Name"]?.Value;
            if (string.IsNullOrEmpty(name) || name.IndexOf(':') >= 0)
                throw new ModContentException("Core " + type + " requires an unqualified legacy Name.");
            return name;
        }

        private static DefinitionId DuplicateRangedId(string legacyName, string subType, int ordinal)
        {
            string suffix = string.IsNullOrWhiteSpace(subType) ? "duplicate-" + ordinal : subType;
            DefinitionId id;
            if (DefinitionId.TryParse("core:items/ranged/" + legacyName + "/" + suffix, out id)) return id;
            return DefinitionId.Parse("core:items/ranged/" + legacyName + "/duplicate-" + ordinal);
        }

        private static int ParseIntAttribute(XmlNode node, string attribute, string itemName)
        {
            string text = node.Attributes[attribute]?.Value;
            if (string.IsNullOrEmpty(text)) return 0;
            int value;
            if (!int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
                throw new ModContentException("Invalid vanilla " + attribute + " for '" + itemName + "'.");
            return value;
        }

        private static AssetId ParseModel(XmlNode node)
        {
            string model = node.Attributes["Model"]?.Value;
            return string.IsNullOrEmpty(model) ? default :
                AssetId.Parse("core:gamedata/models/" + StripExtension(model, ".xml"));
        }

        private static string StripExtension(string value, string extension)
        {
            return value.EndsWith(extension, StringComparison.OrdinalIgnoreCase)
                ? value.Substring(0, value.Length - extension.Length) : value;
        }
    }
}
