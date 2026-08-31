using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Eclipse.Modding
{
    // A read-only projection of vanilla definitions. LegacyItemXml retains fields not yet
    // modeled by the public API; this importer must not reconstruct or replace ItemInfo.
    public static class CoreContentImporter
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
                armors.Add(new ArmorDefinition(ArmorId(name), displayName, ParseModel(node),
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
                helms.Add(new HelmDefinition(HelmId(name), displayName, ParseModel(node),
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
                ranged.Add(new RangedDefinition(id, displayName, ParseModel(node), subType,
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
                magic.Add(new MagicDefinition(MagicId(name), displayName, ParseModel(node),
                    node.Attributes["SubType"]?.Value ?? string.Empty,
                    ParseIntAttribute(node, "MagicDamage", name), name, node.OuterXml));
            }
            catalog.ImportCore(localizations.ToArray(), Array.Empty<WeaponDefinition>(),
                Array.Empty<ArmorDefinition>(), Array.Empty<HelmDefinition>(),
                Array.Empty<RangedDefinition>(), magic.ToArray());
            return magic.Count;
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
            return new LocalizationDefinition(id, values);
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
