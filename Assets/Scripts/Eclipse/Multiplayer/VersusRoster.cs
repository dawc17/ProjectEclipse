using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Eclipse.Multiplayer.Online;
using UnityEngine;

namespace Eclipse.Multiplayer
{
    public enum LoadoutSlot { Weapon = 0, Armor = 1, Helm = 2, Ranged = 3, Magic = 4 }

    /// <summary>One piece of equipment a versus player may choose.</summary>
    public sealed class VersusItem
    {
        public string Id;
        public string Name;
        public LoadoutSlot Slot;
        /// <summary>The item's class, e.g. "Katana" or "TwoHandedBlunt"; selects its moves.</summary>
        public string SubType;
        public int Level;
        /// <summary>The item's icon name in UI/Items, or empty.</summary>
        public string Icon;
        /// <summary>The game's "nothing equipped" item for the slot (Fists, Body, Head, NoRanged, NoMagic).</summary>
        public bool IsNothing;
        private Sprite _sprite;
        private bool _spriteLoaded;

        public Sprite Sprite
        {
            get
            {
                if (_spriteLoaded) return _sprite;
                _spriteLoaded = true;
                if (!string.IsNullOrEmpty(Icon))
                {
                    try { _sprite = Nekki.SF2.GUI.ResolutionImage.GetSprite(SF2Paths.LFIIMPEAMFG(), Icon); }
                    catch (Exception exception) { Debug.LogWarning("[Versus] No icon for " + Id + ": " + exception.Message); }
                }
                return _sprite;
            }
        }
    }

    public sealed class VersusArena
    {
        public string Id;
        public string Name;
    }

    /// <summary>
    /// The equipment and arenas versus players choose from, read from
    /// Resources/EclipseVersus/roster.json and checked against the loaded game data.
    /// Online, a loadout travels as roster indices (<see cref="LoadoutCode"/>); the
    /// roster's <see cref="Fingerprint"/> is part of the content fingerprint, so only
    /// players with identical rosters meet.
    /// </summary>
    public static class VersusRoster
    {
        public const string RandomArena = Online.Rooms.RoomSettings.RandomArena;

        [Serializable] private sealed class Entry { public string id; public string name; }
        [Serializable]
        private sealed class File
        {
            public Entry[] weapons, ranged, magic, arenas;
            public Entry armorDefault, helmDefault;
            public string[] excludeItems, excludePrefixes, excludeContaining;
        }

        private static readonly List<VersusItem>[] _slots = new List<VersusItem>[5];
        private static readonly Dictionary<string, VersusItem>[] _byId = new Dictionary<string, VersusItem>[5];
        private static readonly List<VersusArena> _arenas = new List<VersusArena>();
        private static string _fingerprint;
        private static bool _loaded;

        public static IReadOnlyList<VersusItem> Items(LoadoutSlot slot) { EnsureLoaded(); return _slots[(int)slot]; }
        public static IReadOnlyList<VersusArena> Arenas { get { EnsureLoaded(); return _arenas; } }
        /// <summary>A short hash of every list, in order.</summary>
        public static string Fingerprint { get { EnsureLoaded(); return _fingerprint; } }

        public static VersusItem Find(LoadoutSlot slot, string id)
        {
            EnsureLoaded();
            return id != null && _byId[(int)slot].TryGetValue(id, out var item) ? item : null;
        }

        public static int IndexOf(LoadoutSlot slot, string id)
        {
            var item = Find(slot, id);
            return item == null ? -1 : _slots[(int)slot].IndexOf(item);
        }

        public static VersusArena FindArena(string id)
        {
            EnsureLoaded();
            return _arenas.Find(arena => arena.Id == id);
        }

        public static string ArenaName(string id) =>
            id == RandomArena ? "Random" : FindArena(id)?.Name ?? (string.IsNullOrEmpty(id) ? "-" : id);

        public static bool IsArena(string id) => FindArena(id) != null;

        /// <summary>A roster arena picked from <paramref name="seed"/>, the same on every peer.</summary>
        public static string ResolveArena(string id, int seed)
        {
            EnsureLoaded();
            if (IsArena(id) || _arenas.Count == 0) return IsArena(id) ? id : "dojo";
            return _arenas[(int)((uint)seed % (uint)_arenas.Count)].Id;
        }

        public static void EnsureLoaded()
        {
            if (_loaded) return;
            if (ListSF.GetItems() == null) throw new InvalidOperationException("The versus roster needs the game data loaded first.");
            _loaded = true;
            for (int i = 0; i < _slots.Length; i++) { _slots[i] = new List<VersusItem>(); _byId[i] = new Dictionary<string, VersusItem>(); }
            var asset = Resources.Load<TextAsset>("EclipseVersus/roster");
            var file = asset != null ? JsonUtility.FromJson<File>(asset.text) : null;
            if (file == null) { Debug.LogError("[Versus] Resources/EclipseVersus/roster.json is missing or unreadable."); file = new File(); }
            AddListed(LoadoutSlot.Weapon, file.weapons);
            AddListed(LoadoutSlot.Ranged, file.ranged);
            AddListed(LoadoutSlot.Magic, file.magic);
            var excluded = new HashSet<string>(file.excludeItems ?? Array.Empty<string>(), StringComparer.Ordinal);
            AddAll(LoadoutSlot.Armor, "Armor", file.armorDefault, excluded, file);
            AddAll(LoadoutSlot.Helm, "Helm", file.helmDefault, excluded, file);
            foreach (var entry in file.arenas ?? Array.Empty<Entry>())
            {
                if (entry == null || string.IsNullOrEmpty(entry.id)) continue;
                if (!LocationInstalled(entry.id)) { Debug.LogWarning("[Versus] Arena " + entry.id + " is not installed; left out of the roster."); continue; }
                _arenas.Add(new VersusArena { Id = entry.id, Name = string.IsNullOrEmpty(entry.name) ? entry.id : entry.name });
            }
            _fingerprint = ComputeFingerprint();
            Debug.Log("[Versus] Roster " + _fingerprint + ": " + _slots[0].Count + " weapons, " + _slots[1].Count + " armors, " + _slots[2].Count +
                " helms, " + _slots[3].Count + " ranged, " + _slots[4].Count + " magic, " + _arenas.Count + " arenas.");
        }

        private static void AddListed(LoadoutSlot slot, Entry[] entries)
        {
            foreach (var entry in entries ?? Array.Empty<Entry>())
                if (entry != null && !string.IsNullOrEmpty(entry.id)) TryAdd(slot, entry.id, entry.name, requireArt: !IsGameDefault(entry.id));
        }

        /// <summary>The game's "nothing equipped" items (Fists, Body, Head, NoRanged, NoMagic) have no icon or model of their own.</summary>
        private static bool IsGameDefault(string id)
        {
            foreach (string type in new[] { "Weapon", "Armor", "Helm", "Ranged", "Magic" })
                if (string.Equals(GameUtils.GetDefaultItem(type), id, StringComparison.Ordinal)) return true;
            return false;
        }

        /// <summary>The slot's default first, then every usable item of the type, by level and name.</summary>
        private static void AddAll(LoadoutSlot slot, string type, Entry defaultEntry, HashSet<string> excluded, File file)
        {
            if (defaultEntry != null && !string.IsNullOrEmpty(defaultEntry.id)) TryAdd(slot, defaultEntry.id, defaultEntry.name, requireArt: !IsGameDefault(defaultEntry.id));
            var candidates = new List<ItemInfo>(ListSF.GetItems().ONFMAJEAACM(type) ?? new List<ItemInfo>());
            candidates.Sort((a, b) => a.ItemLevel != b.ItemLevel ? a.ItemLevel.CompareTo(b.ItemLevel) : string.CompareOrdinal(a.Name, b.Name));
            foreach (var item in candidates)
            {
                if (item == null || string.IsNullOrEmpty(item.Name) || excluded.Contains(item.Name) || Excluded(item.Name, file)) continue;
                TryAdd(slot, item.Name, null, requireArt: true, log: false);
            }
        }

        private static bool Excluded(string name, File file)
        {
            foreach (string prefix in file.excludePrefixes ?? Array.Empty<string>())
                if (!string.IsNullOrEmpty(prefix) && name.StartsWith(prefix, StringComparison.Ordinal)) return true;
            foreach (string part in file.excludeContaining ?? Array.Empty<string>())
                if (!string.IsNullOrEmpty(part) && name.IndexOf(part, StringComparison.OrdinalIgnoreCase) >= 0) return true;
            return false;
        }

        private static void TryAdd(LoadoutSlot slot, string id, string name, bool requireArt, bool log = true)
        {
            if (_byId[(int)slot].ContainsKey(id)) return;
            var info = ListSF.GetItems().GetItemByName(id);
            string problem = null;
            if (info == null) problem = "not in the item list";
            else if (requireArt && string.IsNullOrEmpty(info.FileName)) problem = "has no icon";
            else if (!string.IsNullOrEmpty(info.ModelFileName) && !Eclipse.Content.PackagedArtCatalog.HasModel(info.ModelFileName)) problem = "has no model (" + info.ModelFileName + ")";
            else if (requireArt && string.IsNullOrEmpty(info.ModelFileName) && slot != LoadoutSlot.Ranged && slot != LoadoutSlot.Magic) problem = "has no model";
            if (problem != null)
            {
                if (log) Debug.LogWarning("[Versus] " + id + " " + problem + "; left out of the roster.");
                return;
            }
            var item = new VersusItem
            {
                Id = id,
                Name = !string.IsNullOrEmpty(name) ? name : LocalizationManager.GetStringOrDefault(id, Humanize(id)),
                Slot = slot,
                SubType = info.SubType ?? string.Empty,
                Level = info.ItemLevel,
                Icon = info.FileName ?? string.Empty,
                IsNothing = IsGameDefault(id),
            };
            _slots[(int)slot].Add(item);
            _byId[(int)slot][id] = item;
        }

        private static string Humanize(string id)
        {
            string text = id;
            foreach (string prefix in new[] { "WEAPON_", "ARMOR_", "BODY_", "HELM_", "HEAD_", "RANGED_", "MAGIC_" })
                if (text.StartsWith(prefix, StringComparison.Ordinal)) { text = text.Substring(prefix.Length); break; }
            var words = text.ToLowerInvariant().Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++) words[i] = char.ToUpperInvariant(words[i][0]) + words[i].Substring(1);
            return string.Join(" ", words);
        }

        private static bool LocationInstalled(string name)
        {
            try { return !string.IsNullOrEmpty(ResourceManager.GetBundledText(SF2Paths.OCAKEHJCNCC() + "/" + name + "/params.xml")); }
            catch (Exception) { return false; }
        }

        private static string ComputeFingerprint()
        {
            var text = new StringBuilder();
            for (int slot = 0; slot < _slots.Length; slot++)
            {
                text.Append('|').Append(slot).Append(':');
                foreach (var item in _slots[slot]) text.Append(item.Id).Append(',');
            }
            text.Append("|arenas:");
            foreach (var arena in _arenas) text.Append(arena.Id).Append(',');
            using (var sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(text.ToString()));
                return BitConverter.ToString(hash, 0, 4).Replace("-", "").ToLowerInvariant();
            }
        }
    }

    /// <summary>A fighter's equipment by item id, in <see cref="LoadoutSlot"/> order.</summary>
    public sealed class VersusLoadout : IEquatable<VersusLoadout>
    {
        private readonly string[] _ids = new string[5];

        public string this[LoadoutSlot slot] => _ids[(int)slot];
        public string Weapon => _ids[0];
        public string Armor => _ids[1];
        public string Helm => _ids[2];
        public string Ranged => _ids[3];
        public string Magic => _ids[4];

        public VersusLoadout(string weapon, string armor, string helm, string ranged, string magic)
        {
            _ids[0] = weapon; _ids[1] = armor; _ids[2] = helm; _ids[3] = ranged; _ids[4] = magic;
        }

        /// <summary>The first roster entry of every slot: unarmed, plain clothes, no ranged or magic.</summary>
        public static VersusLoadout Default
        {
            get
            {
                var ids = new string[5];
                for (int slot = 0; slot < 5; slot++)
                {
                    var items = VersusRoster.Items((LoadoutSlot)slot);
                    ids[slot] = items.Count > 0 ? items[0].Id : string.Empty;
                }
                return new VersusLoadout(ids[0], ids[1], ids[2], ids[3], ids[4]);
            }
        }

        /// <summary>This loadout with one slot changed.</summary>
        public VersusLoadout With(LoadoutSlot slot, string id)
        {
            var copy = (string[])_ids.Clone();
            copy[(int)slot] = id;
            return new VersusLoadout(copy[0], copy[1], copy[2], copy[3], copy[4]);
        }

        public static VersusLoadout Random(System.Random random, LoadoutSlot? only = null, VersusLoadout from = null)
        {
            var result = from ?? Default;
            for (int slot = 0; slot < 5; slot++)
            {
                if (only.HasValue && (int)only.Value != slot) continue;
                var items = VersusRoster.Items((LoadoutSlot)slot);
                if (items.Count > 0) result = result.With((LoadoutSlot)slot, items[random.Next(items.Count)].Id);
            }
            return result;
        }

        /// <summary>Every slot names a roster item.</summary>
        public bool IsValid
        {
            get
            {
                for (int slot = 0; slot < 5; slot++) if (VersusRoster.Find((LoadoutSlot)slot, _ids[slot]) == null) return false;
                return true;
            }
        }

        /// <summary>This loadout with any slot that is not on the roster replaced by that slot's default.</summary>
        public VersusLoadout Sanitized()
        {
            var fallback = Default;
            var result = this;
            for (int slot = 0; slot < 5; slot++)
                if (VersusRoster.Find((LoadoutSlot)slot, _ids[slot]) == null) result = result.With((LoadoutSlot)slot, fallback._ids[slot]);
            return result;
        }

        public LoadoutCode ToCode()
        {
            ushort Index(LoadoutSlot slot)
            {
                int index = VersusRoster.IndexOf(slot, _ids[(int)slot]);
                return index < 0 ? LoadoutCode.Unset : (ushort)index;
            }
            return new LoadoutCode { Weapon = Index(LoadoutSlot.Weapon), Armor = Index(LoadoutSlot.Armor), Helm = Index(LoadoutSlot.Helm),
                Ranged = Index(LoadoutSlot.Ranged), Magic = Index(LoadoutSlot.Magic) };
        }

        /// <summary>The loadout a code names; unknown or unset slots fall back to the slot's default.</summary>
        public static VersusLoadout FromCode(LoadoutCode code)
        {
            string Id(LoadoutSlot slot, ushort index)
            {
                var items = VersusRoster.Items(slot);
                return index < items.Count ? items[index].Id : items.Count > 0 ? items[0].Id : string.Empty;
            }
            return new VersusLoadout(Id(LoadoutSlot.Weapon, code.Weapon), Id(LoadoutSlot.Armor, code.Armor), Id(LoadoutSlot.Helm, code.Helm),
                Id(LoadoutSlot.Ranged, code.Ranged), Id(LoadoutSlot.Magic, code.Magic));
        }

        /// <summary>Item ids in slot order, for saving (replays, preferences).</summary>
        public string[] ToIds() => (string[])_ids.Clone();

        /// <summary>A saved loadout; empty or unknown slots become the slot's default.</summary>
        public static VersusLoadout FromIds(IList<string> ids)
        {
            var fallback = Default;
            string At(int slot) => ids != null && slot < ids.Count && !string.IsNullOrEmpty(ids[slot]) ? ids[slot] : fallback._ids[slot];
            return new VersusLoadout(At(0), At(1), At(2), At(3), At(4)).Sanitized();
        }

        public string Serialize() => string.Join(";", _ids);
        public static VersusLoadout Deserialize(string text) => FromIds(string.IsNullOrEmpty(text) ? null : text.Split(';'));

        public string WeaponName => VersusRoster.Find(LoadoutSlot.Weapon, Weapon)?.Name ?? Weapon;

        public bool Equals(VersusLoadout other)
        {
            if (ReferenceEquals(other, null)) return false;
            for (int i = 0; i < 5; i++) if (!string.Equals(_ids[i], other._ids[i], StringComparison.Ordinal)) return false;
            return true;
        }

        public override bool Equals(object obj) => Equals(obj as VersusLoadout);
        public override int GetHashCode() => Serialize().GetHashCode();
        public override string ToString() => Serialize();
    }
}
