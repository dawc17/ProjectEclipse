using System;

namespace Eclipse.Modding
{
    public readonly struct AssetId : IEquatable<AssetId>
    {
        public ModId Namespace { get; }
        public string Path { get; }

        private AssetId(ModId namespaceId, string path)
        {
            Namespace = namespaceId;
            Path = path;
        }

        public static AssetId Parse(string value)
        {
            AssetId id;
            string error;
            if (!TryParse(value, out id, out error))
                throw new FormatException(error);
            return id;
        }

        public static bool TryParse(string value, out AssetId id)
        {
            string error;
            return TryParse(value, out id, out error);
        }

        internal static bool TryParse(string value, out AssetId id, out string error)
        {
            id = default;
            error = null;
            if (string.IsNullOrEmpty(value))
            {
                error = "Asset ID must not be empty.";
                return false;
            }

            int separator = value.IndexOf(':');
            if (separator <= 0 || separator != value.LastIndexOf(':') || separator == value.Length - 1)
            {
                error = "Asset ID must use 'namespace:path' syntax: '" + value + "'.";
                return false;
            }

            ModId namespaceId;
            if (!ModId.TryParse(value.Substring(0, separator), out namespaceId))
            {
                error = "Invalid asset namespace in '" + value + "'.";
                return false;
            }

            string path;
            if (!ModIdentityRules.TryNormalizePath(value.Substring(separator + 1), out path, out error))
                return false;

            id = new AssetId(namespaceId, path);
            return true;
        }

        public bool Equals(AssetId other)
        {
            return Namespace.Equals(other.Namespace) && string.Equals(Path, other.Path, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is AssetId other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Namespace.GetHashCode() * 397) ^
                    (Path == null ? 0 : StringComparer.Ordinal.GetHashCode(Path));
            }
        }

        public override string ToString()
        {
            return Namespace + ":" + (Path ?? string.Empty);
        }

        public static bool operator ==(AssetId left, AssetId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AssetId left, AssetId right)
        {
            return !left.Equals(right);
        }
    }
}
