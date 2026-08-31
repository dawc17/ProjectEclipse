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
            Append(canonical, "fingerprint-v1");
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
            Append(canonical, item.Model.ToString());
            Append(canonical, item.LegacyName ?? string.Empty);
            Append(canonical, item.LegacyItemXml ?? string.Empty);

            if (item is WeaponDefinition weapon)
            {
                Append(canonical, weapon.Icon.ToString());
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
}
