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

        public static int ImportWeapons(ModContentCatalog catalog, IEnumerable<XmlNode> source,
            IReadOnlyDictionary<string, XmlDocument> languages)
        {
            if (catalog == null) throw new ArgumentNullException(nameof(catalog));
            if (source == null) throw new ArgumentNullException(nameof(source));
            var translations = new Dictionary<string, Dictionary<string, string>>(StringComparer.Ordinal);
            if (languages != null)
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

            var weapons = new List<WeaponDefinition>();
            var localizations = new List<LocalizationDefinition>();
            foreach (XmlNode node in source)
            {
                if (node?.Attributes?["Type"]?.Value != "Weapon") continue;
                string name = node.Attributes["Name"]?.Value;
                if (string.IsNullOrEmpty(name) || name.IndexOf(':') >= 0)
                    throw new ModContentException("Core weapon requires an unqualified legacy Name.");
                DefinitionId id = WeaponId(name);
                DefinitionId displayName = DefinitionId.Parse("core:localization/" + name);
                var values = new Dictionary<string, string>(StringComparer.Ordinal);
                foreach (KeyValuePair<string, Dictionary<string, string>> language in translations)
                {
                    string value;
                    if (language.Value.TryGetValue(name, out value) && !string.IsNullOrEmpty(value))
                        values.Add(language.Key, value);
                }
                // Hidden/NPC-only definitions can legitimately have no localized label.
                if (!values.ContainsKey("eng")) values.Add("eng", name);
                localizations.Add(new LocalizationDefinition(displayName, values));

                string damageText = node.Attributes["WeaponDamage"]?.Value;
                int damage = 0;
                if (damageText != null && !int.TryParse(damageText, NumberStyles.Integer,
                    CultureInfo.InvariantCulture, out damage))
                    throw new ModContentException("Invalid vanilla WeaponDamage for '" + name + "'.");
                string model = node.Attributes["Model"]?.Value;
                AssetId modelId = string.IsNullOrEmpty(model) ? default :
                    AssetId.Parse("core:gamedata/models/" + StripExtension(model, ".xml"));
                // Legacy Image values can be atlas members or fallback resource names. Keep
                // those verbatim in LegacyItemXml instead of inventing exact typed asset IDs.
                weapons.Add(new WeaponDefinition(id, displayName, default, modelId,
                    node.Attributes["SubType"]?.Value ?? string.Empty, damage, name, node.OuterXml));
            }
            catalog.ImportCore(localizations.ToArray(), weapons.ToArray());
            return weapons.Count;
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

        private static string StripExtension(string value, string extension)
        {
            return value.EndsWith(extension, StringComparison.OrdinalIgnoreCase)
                ? value.Substring(0, value.Length - extension.Length) : value;
        }
    }
}
