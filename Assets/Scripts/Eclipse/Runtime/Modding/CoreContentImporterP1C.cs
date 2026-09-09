using System;
using System.Collections.Generic;
using System.Xml;

namespace Eclipse.Modding
{
    public static partial class CoreContentImporter
    {
        public static int ImportNonEquipment(ModContentCatalog catalog, IEnumerable<XmlNode> source)
        {
            if (catalog == null) throw new ArgumentNullException(nameof(catalog));
            if (source == null) throw new ArgumentNullException(nameof(source));
            var result = new List<NonEquipmentItemDefinition>();
            foreach (XmlNode node in source)
            {
                if (node?.Attributes == null) continue;
                string type = P1CAttr(node, "Type");
                ModNonEquipmentItemKind kind;
                string path;
                if (type == "Consumable") { kind = ModNonEquipmentItemKind.Consumable; path = "consumable/"; }
                else if (type == "Free") { kind = ModNonEquipmentItemKind.Free; path = "free/"; }
                else if (type == "Seal") { kind = ModNonEquipmentItemKind.Seal; path = "seal/"; }
                else continue;
                string name = P1CAttr(node, "Name");
                if (string.IsNullOrEmpty(name)) continue;
                DefinitionId id = DefinitionId.Parse("core:items/" + path + name);
                DefinitionId display = DefinitionId.Parse("core:localization/" + name);
                result.Add(new NonEquipmentItemDefinition(id, display, default(AssetId), default(AssetId), kind,
                    P1CAttr(node, "SubType"), P1CAttr(node, "PackLabel"), BoolAttr(node, "SilentRecieve"),
                    BoolAttr(node, "SpendAfterUse"), name, node.OuterXml));
            }
            catalog.ImportCoreNonEquipment(result.ToArray());
            return result.Count;
        }

        public static int ImportForgeEconomicProfiles(ModContentCatalog catalog, IEnumerable<string> recipeNames)
        {
            if (catalog == null) throw new ArgumentNullException(nameof(catalog));
            if (recipeNames == null) throw new ArgumentNullException(nameof(recipeNames));
            var profiles = new List<ForgeEconomicProfileDefinition>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string recipeName in recipeNames)
            {
                if (string.IsNullOrWhiteSpace(recipeName) || !seen.Add(recipeName)) continue;
                DefinitionId id = DefinitionId.Parse("core:forge-profiles/" + recipeName);
                profiles.Add(new ForgeEconomicProfileDefinition(id, recipeName));
            }
            catalog.ImportCoreForgeProfiles(profiles.ToArray());
            return profiles.Count;
        }

        private static bool BoolAttr(XmlNode node, string name)
        {
            string value = P1CAttr(node, name);
            return value == "1" || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
        }

        private static string P1CAttr(XmlNode node, string name)
        {
            XmlAttribute attribute = node?.Attributes?[name];
            return attribute == null ? string.Empty : attribute.Value;
        }
    }
}
