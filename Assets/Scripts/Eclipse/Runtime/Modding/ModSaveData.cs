using System;
using System.Collections.Generic;
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

        public static bool RecordContext(XmlNode warrior, IReadOnlyList<ModDescriptor> activeMods)
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
    }
}
