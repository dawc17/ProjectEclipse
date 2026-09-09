using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Eclipse.Modding
{
    // Disabled IDs are retained when a mod is temporarily absent. Newly installed mods default on.
    public sealed class ModSelection
    {
        private readonly HashSet<ModId> disabled = new HashSet<ModId>();
        public bool IsEnabled(ModId id) => id.Value == "core" || !disabled.Contains(id);
        public List<ModDescriptor> Filter(IReadOnlyList<ModDescriptor> mods)
        {
            var result = new List<ModDescriptor>();
            foreach (var mod in mods) if (IsEnabled(mod.Id)) result.Add(mod);
            return result;
        }

        public void SetEnabled(ModId id, bool enabled, IReadOnlyList<ModDescriptor> mods)
        {
            if (id.Value == "core") throw new InvalidOperationException("Core cannot be disabled.");
            var pending = new Queue<ModId>();
            var visited = new HashSet<ModId>();
            pending.Enqueue(id);
            while (pending.Count > 0)
            {
                var current = pending.Dequeue();
                if (current.Value == "core" || !visited.Add(current)) continue;
                if (enabled) disabled.Remove(current); else disabled.Add(current);
                foreach (var mod in mods)
                    foreach (var dependency in mod.Manifest.Dependencies)
                    {
                        if (enabled && mod.Id == current) pending.Enqueue(dependency.Id);
                        if (!enabled && dependency.Id == current) pending.Enqueue(mod.Id);
                    }
            }
        }

        public static ModSelection Load(string path)
        {
            var result = new ModSelection();
            if (!File.Exists(path)) return result;
            var doc = new XmlDocument { XmlResolver = null };
            using (var reader = XmlReader.Create(path, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null })) doc.Load(reader);
            if (doc.DocumentElement?.Name != "ModSelection" || doc.DocumentElement.GetAttribute("version") != "1")
                throw new InvalidDataException("Unsupported mod selection file.");
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                if (node.NodeType != XmlNodeType.Element) continue;
                if (node.Name != "Disabled" || !ModId.TryParse(node.Attributes?["id"]?.Value, out var id) || id.Value == "core")
                    throw new InvalidDataException("Invalid disabled mod entry.");
                result.disabled.Add(id);
            }
            return result;
        }

        public void Save(string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path)));
            var doc = new XmlDocument();
            var root = doc.CreateElement("ModSelection"); root.SetAttribute("version", "1"); doc.AppendChild(root);
            var ids = new List<ModId>(disabled); ids.Sort((a, b) => string.CompareOrdinal(a.Value, b.Value));
            foreach (var id in ids) { var node = doc.CreateElement("Disabled"); node.SetAttribute("id", id.Value); root.AppendChild(node); }
            string temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                doc.Save(temporary);
                if (File.Exists(path)) File.Replace(temporary, path, null); else File.Move(temporary, path);
            }
            finally { if (File.Exists(temporary)) File.Delete(temporary); }
        }
    }
}
