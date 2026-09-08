using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace Eclipse.Modding
{
    // Operates on the existing save DOM. Missing content must never require decoding,
    // normalizing, moving, or rebuilding its ownership XML.
    public static class ModSaveData
    {
        public static bool IsExternalItem(string name)
        {
            DefinitionId id;
            return DefinitionId.TryParse(name, out id) && id.Namespace.Value != "core" && id.Category == "items";
        }

        public static bool IsMissingItem(XmlNode node, Func<string, bool> itemExists)
        {
            string name = node?.Attributes?["Name"]?.Value;
            return IsExternalItem(name) && !itemExists(name);
        }

        public static XmlNode CreateEquipmentView(XmlNode warrior, Func<string, bool> itemExists,
            Func<string, string> defaultItem)
        {
            if (warrior == null) return null;
            XmlNode view = warrior;
            foreach (string slot in new[] { "Weapon", "Armor", "Helm", "Ranged", "Magic" })
            {
                string name = warrior.Attributes?[slot]?.Value;
                if (!IsExternalItem(name) || itemExists(name)) continue;
                if (ReferenceEquals(view, warrior)) view = warrior.CloneNode(true);
                // Only the temporary model input changes. The original equipped reference
                // stays in the save until the player explicitly equips something else.
                view.Attributes[slot].Value = defaultItem(slot) ?? string.Empty;
            }
            return view;
        }

        public static bool RecordContext(XmlNode warrior, IReadOnlyList<ModDescriptor> activeMods,
            ModContentCatalog content = null)
        {
            if (warrior == null || activeMods == null) return false;
            XmlElement state = warrior["EclipseMods"];
            if (state != null && state.GetAttribute("schema") != "1") return false;
            if (state == null)
            {
                state = warrior.OwnerDocument.CreateElement("EclipseMods");
                state.SetAttribute("schema", "1");
                warrior.AppendChild(state);
            }
            state.SetAttribute("api", ModPlatformVersions.Api.ToString());
            state.SetAttribute("core", ModPlatformVersions.Core.ToString());
            if (content != null)
                state.SetAttribute("contentHash", ComputeContentSetFingerprint(activeMods, content));
            // Keep last-seen records for absent mods and all unrecognized attributes/children.
            foreach (XmlNode child in state.ChildNodes)
                if (child is XmlElement entry && entry.Name == "Mod") entry.SetAttribute("active", "false");
            foreach (ModDescriptor mod in activeMods)
            {
                XmlElement entry = null;
                foreach (XmlNode child in state.ChildNodes)
                    if (child is XmlElement candidate && candidate.Name == "Mod" && candidate.GetAttribute("id") == mod.Id.Value)
                    { entry = candidate; break; }
                if (entry == null)
                {
                    entry = warrior.OwnerDocument.CreateElement("Mod");
                    entry.SetAttribute("id", mod.Id.Value);
                    state.AppendChild(entry);
                }
                entry.SetAttribute("version", mod.Version.ToString());
                entry.SetAttribute("active", "true");
            }
            return true;
        }

        public static string ComputeContentSetFingerprint(IReadOnlyList<ModDescriptor> activeMods,
            ModContentCatalog content)
        {
            if (activeMods == null) throw new ArgumentNullException(nameof(activeMods));
            if (content == null) throw new ArgumentNullException(nameof(content));

            var canonical = new StringBuilder();
            Append(canonical, "fingerprint-v3");
            Append(canonical, ModPlatformVersions.Api.ToString());
            Append(canonical, ModPlatformVersions.Core.ToString());

            var mods = new List<ModDescriptor>(activeMods.Count);
            for (int i = 0; i < activeMods.Count; i++)
                if (activeMods[i] != null) mods.Add(activeMods[i]);
            mods.Sort((left, right) => string.CompareOrdinal(left.Id.Value, right.Id.Value));
            Append(canonical, "mods");
            Append(canonical, mods.Count);
            foreach (ModDescriptor mod in mods)
            {
                Append(canonical, mod.Id.Value);
                Append(canonical, mod.Version.ToString());
            }

            var localizations = new List<LocalizationDefinition>(content.Localizations);
            localizations.Sort((left, right) => CompareIds(left.Id, right.Id));
            Append(canonical, "localizations");
            Append(canonical, localizations.Count);
            foreach (LocalizationDefinition localization in localizations)
            {
                Append(canonical, localization.Id.ToString());
                var languages = new List<string>(localization.Values.Keys);
                languages.Sort(StringComparer.Ordinal);
                Append(canonical, languages.Count);
                foreach (string language in languages)
                {
                    Append(canonical, language);
                    Append(canonical, localization.Values[language]);
                }
            }

            var items = new List<ItemDefinition>(content.Weapons.Count + content.Armors.Count +
                content.Helms.Count + content.Ranged.Count + content.Magic.Count);
            AddItems(items, content.Weapons);
            AddItems(items, content.Armors);
            AddItems(items, content.Helms);
            AddItems(items, content.Ranged);
            AddItems(items, content.Magic);
            items.Sort((left, right) => CompareIds(left.Id, right.Id));
            Append(canonical, "items");
            Append(canonical, items.Count);
            foreach (ItemDefinition item in items) AppendItem(canonical, item);

            var redirects = new List<ItemRedirectDefinition>(content.ItemRedirects);
            redirects.Sort((left, right) => CompareIds(left.Id, right.Id));
            Append(canonical, "item-redirects");
            Append(canonical, redirects.Count);
            foreach (ItemRedirectDefinition redirect in redirects)
            {
                Append(canonical, redirect.Id.ToString());
                Append(canonical, redirect.IsTombstone ? "tombstone" : "alias");
                Append(canonical, redirect.IsTombstone ? string.Empty : redirect.Target.ToString());
            }

            var listings = new List<ShopListingDefinition>(content.ShopListings);
            listings.Sort((left, right) => CompareIds(left.Id, right.Id));
            Append(canonical, "shop");
            Append(canonical, listings.Count);
            foreach (ShopListingDefinition listing in listings)
            {
                Append(canonical, listing.Id.ToString());
                Append(canonical, listing.Item.ToString());
                Append(canonical, ((int)listing.Section).ToString(CultureInfo.InvariantCulture));
                Append(canonical, listing.Level);
                Append(canonical, ((int)listing.Price.Currency).ToString(CultureInfo.InvariantCulture));
                Append(canonical, listing.Price.Amount.ToString(CultureInfo.InvariantCulture));
            }

            var perks = new List<PerkDefinition>(content.Perks);
            perks.Sort((left, right) => CompareIds(left.Id, right.Id));
            Append(canonical, "perks");
            Append(canonical, perks.Count);
            foreach (PerkDefinition perk in perks)
            {
                Append(canonical, perk.Id.ToString());
                Append(canonical, perk.HasTemplate ? perk.Template.ToString() : string.Empty);
                Append(canonical, perk.HasBehavior ? perk.Behavior.ToString() : string.Empty);
                Append(canonical, perk.DisplayName.ToString());
                Append(canonical, perk.Description.ToString());
                Append(canonical, perk.Icon.ToString());
                Append(canonical, ((int)perk.Kind).ToString(CultureInfo.InvariantCulture));
                Append(canonical, perk.LegacyName ?? string.Empty);
                Append(canonical, perk.LegacyPerkXml ?? string.Empty);
                var parameterNames = new List<string>(perk.Parameters.Keys);
                parameterNames.Sort(StringComparer.Ordinal);
                Append(canonical, parameterNames.Count);
                foreach (string parameter in parameterNames)
                {
                    Append(canonical, parameter);
                    Append(canonical, perk.Parameters[parameter]);
                }
                AppendParameterValues(canonical, perk.InitialParameters);
            }

            var enchantments = new List<EnchantmentDefinition>(content.Enchantments);
            enchantments.Sort((left, right) => CompareIds(left.Id, right.Id));
            Append(canonical, "enchantments");
            Append(canonical, enchantments.Count);
            foreach (EnchantmentDefinition enchantment in enchantments)
            {
                Append(canonical, enchantment.Id.ToString());
                Append(canonical, enchantment.HasPerk ? enchantment.Perk.ToString() : string.Empty);
                Append(canonical, enchantment.HasBehavior ? enchantment.Behavior.ToString() : string.Empty);
                Append(canonical, enchantment.DisplayName.ToString());
                Append(canonical, enchantment.Description.ToString());
                Append(canonical, enchantment.Icon.ToString());
                Append(canonical, ((int)enchantment.Recipe).ToString(CultureInfo.InvariantCulture));
                Append(canonical, enchantment.Equipment.Count);
                for (int i = 0; i < enchantment.Equipment.Count; i++)
                    Append(canonical, ((int)enchantment.Equipment[i]).ToString(CultureInfo.InvariantCulture));
                AppendParameterValues(canonical, enchantment.InitialParameters);
            }

            var behaviors = new List<ModBehaviorDefinition>(content.Behaviors);
            behaviors.Sort((left, right) => CompareIds(left.Id, right.Id));
            Append(canonical, "behaviors");
            Append(canonical, behaviors.Count);
            foreach (ModBehaviorDefinition behavior in behaviors)
            {
                Append(canonical, behavior.Id.ToString());
                var parameters = new List<ModParameterDefinition>(behavior.Parameters.Parameters);
                parameters.Sort((left, right) => string.CompareOrdinal(left.Name, right.Name));
                Append(canonical, parameters.Count);
                for (int i = 0; i < parameters.Count; i++)
                {
                    ModParameterDefinition parameter = parameters[i];
                    Append(canonical, parameter.Name);
                    Append(canonical, ((int)parameter.Type).ToString(CultureInfo.InvariantCulture));
                    Append(canonical, parameter.Required ? "required" : "optional");
                    Append(canonical, parameter.HasDefault ? parameter.DefaultValue.ToWireString() : string.Empty);
                }
            }

            byte[] data = Encoding.UTF8.GetBytes(canonical.ToString());
            byte[] hash;
            using (SHA256 sha = SHA256.Create()) hash = sha.ComputeHash(data);
            var hex = new StringBuilder(hash.Length * 2);
            for (int i = 0; i < hash.Length; i++) hex.Append(hash[i].ToString("x2", CultureInfo.InvariantCulture));
            return "sha256:" + hex;
        }

        private static void AddItems<T>(List<ItemDefinition> target, IReadOnlyList<T> source)
            where T : ItemDefinition
        {
            for (int i = 0; i < source.Count; i++) target.Add(source[i]);
        }

        private static void AppendItem(StringBuilder canonical, ItemDefinition item)
        {
            Append(canonical, item.GetType().Name);
            Append(canonical, item.Id.ToString());
            Append(canonical, item.DisplayName.ToString());
            Append(canonical, item.Icon.ToString());
            Append(canonical, item.Model.ToString());
            Append(canonical, item.LegacyName ?? string.Empty);
            Append(canonical, item.LegacyItemXml ?? string.Empty);
            Append(canonical, ((int)item.Progression).ToString(CultureInfo.InvariantCulture));

            if (item is WeaponDefinition weapon)
            {
                Append(canonical, weapon.SubType);
                Append(canonical, weapon.Damage);
            }
            else if (item is ArmorDefinition armor)
            {
                Append(canonical, armor.BodyDefense);
                Append(canonical, armor.HeadDefense);
                Append(canonical, armor.UnarmedDamage);
            }
            else if (item is HelmDefinition helm)
            {
                Append(canonical, helm.HeadDefense);
            }
            else if (item is RangedDefinition ranged)
            {
                Append(canonical, ranged.SubType);
                Append(canonical, ranged.RangedDamage);
                Append(canonical, ranged.WeaponDamage);
            }
            else if (item is MagicDefinition magic)
            {
                Append(canonical, magic.SubType);
                Append(canonical, magic.MagicDamage);
            }
        }

        private static void AppendParameterValues(StringBuilder canonical,
            IReadOnlyDictionary<string, ModParameterValue> values)
        {
            var names = new List<string>(values.Keys);
            names.Sort(StringComparer.Ordinal);
            Append(canonical, names.Count);
            for (int i = 0; i < names.Count; i++)
            {
                string name = names[i];
                Append(canonical, name);
                Append(canonical, ((int)values[name].Type).ToString(CultureInfo.InvariantCulture));
                Append(canonical, values[name].ToWireString());
            }
        }

        private static int CompareIds(DefinitionId left, DefinitionId right)
        {
            return string.CompareOrdinal(left.ToString(), right.ToString());
        }

        private static void Append(StringBuilder builder, int value)
        {
            Append(builder, value.ToString(CultureInfo.InvariantCulture));
        }

        private static void Append(StringBuilder builder, string value)
        {
            value = value ?? string.Empty;
            builder.Append(value.Length.ToString(CultureInfo.InvariantCulture)).Append(':').Append(value).Append(';');
        }
    }

    public sealed class ModEffectInstance
    {
        private readonly IReadOnlyDictionary<string, ModParameterValue> _values;

        public DefinitionId Owner { get; }
        public IReadOnlyDictionary<string, ModParameterValue> Values => _values;

        public ModEffectInstance(DefinitionId owner, IDictionary<string, ModParameterValue> values)
        {
            Owner = owner;
            _values = new System.Collections.ObjectModel.ReadOnlyDictionary<string, ModParameterValue>(
                new Dictionary<string, ModParameterValue>(values, StringComparer.Ordinal));
        }
    }

    // Eclipse-owned typed state lives beside the recovered <Set> node, never inside it.
    // This keeps arbitrary Lua parameter names out of PerkInfoItem/PerkSetAttributes while
    // allowing missing mods and future fields to round-trip as opaque XML.
    public static class ModEffectSaveData
    {
        public const string NodeName = "EclipseParams";
        public const string ParameterNodeName = "Param";
        public const string Format = "1";

        public static bool TryRead(XmlNode effectNode, DefinitionId owner, ModParameterSchema schema,
            out ModEffectInstance instance, out string error)
        {
            instance = null;
            error = string.Empty;
            if (effectNode == null) { error = "Effect node is missing."; return false; }
            if (schema == null) { error = "Parameter schema is missing."; return false; }

            XmlNode paramsNode = effectNode[NodeName];
            if (paramsNode != null)
            {
                string format = paramsNode.Attributes?["Format"]?.Value;
                if (!string.Equals(format, Format, StringComparison.Ordinal))
                { error = "Unsupported Eclipse parameter format '" + (format ?? string.Empty) + "'."; return false; }
            }

            var raw = new Dictionary<string, string>(StringComparer.Ordinal);
            if (paramsNode != null)
            {
                foreach (XmlNode child in paramsNode.ChildNodes)
                {
                    if (child.NodeType != XmlNodeType.Element || child.Name != ParameterNodeName) continue;
                    string name = child.Attributes?["Name"]?.Value;
                    if (string.IsNullOrEmpty(name)) continue;
                    ModParameterDefinition known;
                    if (!schema.TryGet(name, out known)) continue;
                    if (raw.ContainsKey(name))
                    { error = "Parameter '" + name + "' is saved more than once."; return false; }
                    XmlAttribute value = child.Attributes?["Value"];
                    if (value == null)
                    { error = "Parameter '" + name + "' has no Value attribute."; return false; }
                    raw.Add(name, value.Value);
                }
            }

            var supplied = new Dictionary<string, ModParameterValue>(StringComparer.Ordinal);
            foreach (KeyValuePair<string, string> pair in raw)
            {
                ModParameterDefinition definition;
                schema.TryGet(pair.Key, out definition);
                ModParameterValue value;
                if (!ModParameterValue.TryParse(definition.Type, pair.Value, out value))
                { error = "Parameter '" + pair.Key + "' is not a valid " + definition.Type + "."; return false; }
                supplied.Add(pair.Key, value);
            }

            Dictionary<string, ModParameterValue> resolved;
            try { resolved = schema.ResolveValues(supplied); }
            catch (ModContentException exception) { error = exception.Message; return false; }
            instance = new ModEffectInstance(owner, resolved);
            return true;
        }

        public static void Write(XmlElement effectNode, DefinitionId owner, ModParameterSchema schema,
            IReadOnlyDictionary<string, ModParameterValue> values)
        {
            if (effectNode == null) throw new ArgumentNullException(nameof(effectNode));
            if (schema == null) throw new ArgumentNullException(nameof(schema));
            Dictionary<string, ModParameterValue> resolved = schema.ResolveValues(values);

            XmlElement paramsNode = effectNode[NodeName] as XmlElement;
            if (paramsNode != null)
            {
                string format = paramsNode.GetAttribute("Format");
                if (!string.Equals(format, Format, StringComparison.Ordinal))
                    throw new ModContentException("Cannot rewrite unsupported Eclipse parameter format '" + format + "'.");
            }
            else
            {
                paramsNode = effectNode.OwnerDocument.CreateElement(NodeName);
                paramsNode.SetAttribute("Format", Format);
                effectNode.AppendChild(paramsNode);
            }

            var knownNodes = new Dictionary<string, List<XmlElement>>(StringComparer.Ordinal);
            foreach (XmlNode child in paramsNode.ChildNodes)
            {
                XmlElement element = child as XmlElement;
                if (element == null || element.Name != ParameterNodeName) continue;
                string name = element.GetAttribute("Name");
                ModParameterDefinition ignored;
                if (!schema.TryGet(name, out ignored)) continue;
                List<XmlElement> nodes;
                if (!knownNodes.TryGetValue(name, out nodes))
                {
                    nodes = new List<XmlElement>();
                    knownNodes.Add(name, nodes);
                }
                nodes.Add(element);
            }

            for (int i = 0; i < schema.Parameters.Count; i++)
            {
                ModParameterDefinition definition = schema.Parameters[i];
                ModParameterValue value;
                bool hasValue = resolved.TryGetValue(definition.Name, out value);
                List<XmlElement> nodes;
                knownNodes.TryGetValue(definition.Name, out nodes);
                if (!hasValue)
                {
                    if (nodes != null) for (int j = 0; j < nodes.Count; j++) paramsNode.RemoveChild(nodes[j]);
                    continue;
                }

                XmlElement parameter;
                if (nodes == null || nodes.Count == 0)
                {
                    parameter = effectNode.OwnerDocument.CreateElement(ParameterNodeName);
                    paramsNode.AppendChild(parameter);
                }
                else
                {
                    parameter = nodes[0];
                    for (int j = 1; j < nodes.Count; j++) paramsNode.RemoveChild(nodes[j]);
                }
                parameter.SetAttribute("Name", definition.Name);
                parameter.SetAttribute("Value", value.ToWireString());
            }
        }
    }
}
