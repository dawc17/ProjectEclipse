using System;
using System.Collections.Generic;
using Eclipse.Multiplayer.Online;
using UnityEngine;

namespace Eclipse.Multiplayer
{
    /// <summary>
    /// Remembered loadouts (one per use: each local player, online, the training dummy)
    /// and the player's named presets, kept in PlayerPrefs as item ids so they survive
    /// roster changes; anything no longer on the roster falls back to that slot's default.
    /// </summary>
    public static class VersusLoadouts
    {
        public const string PlayerOne = "P1";
        public const string PlayerTwo = "P2";
        public const string Online = "Online";
        public const string Dummy = "Dummy";
        public const int MaxPresets = 6;
        private const string Prefix = "Eclipse.Versus.Loadout.";
        private const string PresetPrefix = "Eclipse.Versus.Preset.";

        public static VersusLoadout Load(string use)
        {
            string saved = PlayerPrefs.GetString(Prefix + use, string.Empty);
            // Older builds remembered only a room weapon.
            if (saved.Length == 0 && use == Online) saved = PlayerPrefs.GetString("Eclipse.Online.RoomWeapon", string.Empty);
            return VersusLoadout.Deserialize(saved);
        }

        public static void Save(string use, VersusLoadout loadout)
        {
            if (loadout == null) return;
            PlayerPrefs.SetString(Prefix + use, loadout.Serialize());
            PlayerPrefs.Save();
        }

        /// <summary>A loadout from the wire; false when any index is outside this roster.</summary>
        public static bool TryFromCode(LoadoutCode code, out VersusLoadout loadout)
        {
            loadout = null;
            if (!code.IsSet) return false;
            ushort[] indices = { code.Weapon, code.Armor, code.Helm, code.Ranged, code.Magic };
            for (int slot = 0; slot < indices.Length; slot++)
                if (indices[slot] >= VersusRoster.Items((LoadoutSlot)slot).Count) return false;
            loadout = VersusLoadout.FromCode(code);
            return true;
        }

        public sealed class Preset
        {
            public int Index;
            public string Name;
            public VersusLoadout Loadout;
        }

        /// <summary>Saved presets in their slots; empty slots are skipped.</summary>
        public static List<Preset> Presets()
        {
            var presets = new List<Preset>();
            for (int i = 0; i < MaxPresets; i++)
            {
                string data = PlayerPrefs.GetString(PresetPrefix + i, string.Empty);
                if (data.Length == 0) continue;
                int split = data.IndexOf('|');
                if (split <= 0) continue;
                presets.Add(new Preset { Index = i, Name = data.Substring(0, split), Loadout = VersusLoadout.Deserialize(data.Substring(split + 1)) });
            }
            return presets;
        }

        /// <returns>The slot used, or -1 when every slot is taken.</returns>
        public static int SavePreset(string name, VersusLoadout loadout, int index = -1)
        {
            if (loadout == null) return -1;
            if (index < 0)
                for (int i = 0; i < MaxPresets && index < 0; i++)
                    if (PlayerPrefs.GetString(PresetPrefix + i, string.Empty).Length == 0) index = i;
            if (index < 0 || index >= MaxPresets) return -1;
            name = string.IsNullOrWhiteSpace(name) ? "Preset " + (index + 1) : name.Trim().Replace("|", "/");
            if (name.Length > 24) name = name.Substring(0, 24);
            PlayerPrefs.SetString(PresetPrefix + index, name + "|" + loadout.Serialize());
            PlayerPrefs.Save();
            return index;
        }

        public static void DeletePreset(int index)
        {
            if (index < 0 || index >= MaxPresets) return;
            PlayerPrefs.DeleteKey(PresetPrefix + index);
            PlayerPrefs.Save();
        }
    }
}
