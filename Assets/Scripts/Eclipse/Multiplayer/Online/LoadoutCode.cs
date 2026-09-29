using System;

namespace Eclipse.Multiplayer.Online
{
    /// <summary>
    /// A fighter's equipment on the wire: one roster index per slot. Only the game
    /// knows what the indices mean (its versus roster); servers store and relay the ten
    /// bytes as they are. Peers only ever pair with the same roster (it is part of the
    /// content fingerprint), so an index names the same item on both ends.
    /// </summary>
    public struct LoadoutCode : IEquatable<LoadoutCode>
    {
        public const int Size = 10;
        /// <summary>A slot with no choice made yet.</summary>
        public const ushort Unset = 0xFFFF;

        public ushort Weapon, Armor, Helm, Ranged, Magic;

        public static readonly LoadoutCode None = new LoadoutCode { Weapon = Unset, Armor = Unset, Helm = Unset, Ranged = Unset, Magic = Unset };

        public bool IsSet => Weapon != Unset;

        public void Write(NetWriter writer)
        {
            writer.U16(Weapon);
            writer.U16(Armor);
            writer.U16(Helm);
            writer.U16(Ranged);
            writer.U16(Magic);
        }

        public static LoadoutCode Read(NetReader reader) => new LoadoutCode
        {
            Weapon = reader.U16(),
            Armor = reader.U16(),
            Helm = reader.U16(),
            Ranged = reader.U16(),
            Magic = reader.U16(),
        };

        public bool Equals(LoadoutCode other) =>
            Weapon == other.Weapon && Armor == other.Armor && Helm == other.Helm && Ranged == other.Ranged && Magic == other.Magic;

        public override bool Equals(object obj) => obj is LoadoutCode other && Equals(other);
        public override int GetHashCode() => ((Weapon * 31 + Armor) * 31 + Helm) * 31 + Ranged * 7 + Magic;
        public static bool operator ==(LoadoutCode a, LoadoutCode b) => a.Equals(b);
        public static bool operator !=(LoadoutCode a, LoadoutCode b) => !a.Equals(b);
        public override string ToString() => Weapon + "/" + Armor + "/" + Helm + "/" + Ranged + "/" + Magic;
    }
}
