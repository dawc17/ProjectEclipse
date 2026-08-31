using System;

namespace Eclipse.Modding
{
    public readonly struct DefinitionId : IEquatable<DefinitionId>
    {
        public ModId Namespace { get; }
        public string Category { get; }
        public string LocalId { get; }

        private DefinitionId(ModId namespaceId, string category, string localId)
        {
            Namespace = namespaceId;
            Category = category;
            LocalId = localId;
        }

        public static DefinitionId Parse(string value)
        {
            DefinitionId id;
            string error;
            if (!TryParse(value, out id, out error))
                throw new FormatException(error);
            return id;
        }

        public static bool TryParse(string value, out DefinitionId id)
        {
            string error;
            return TryParse(value, out id, out error);
        }

        internal static bool TryParse(string value, out DefinitionId id, out string error)
        {
            id = default;
            error = null;
            if (string.IsNullOrEmpty(value))
            {
                error = "Definition ID must not be empty.";
                return false;
            }

            int separator = value.IndexOf(':');
            if (separator <= 0 || separator != value.LastIndexOf(':') || separator == value.Length - 1)
            {
                error = "Definition ID must use 'namespace:category/id' syntax: '" + value + "'.";
                return false;
            }

            ModId namespaceId;
            if (!ModId.TryParse(value.Substring(0, separator), out namespaceId))
            {
                error = "Invalid definition namespace in '" + value + "'.";
                return false;
            }

            string path;
            if (!ModIdentityRules.TryNormalizePath(value.Substring(separator + 1), out path, out error))
                return false;

            int slash = path.IndexOf('/');
            if (slash <= 0 || slash == path.Length - 1)
            {
                error = "Definition ID must contain both a category and local ID: '" + value + "'.";
                return false;
            }

            id = new DefinitionId(namespaceId, path.Substring(0, slash), path.Substring(slash + 1));
            return true;
        }

        public bool Equals(DefinitionId other)
        {
            return Namespace.Equals(other.Namespace) &&
                string.Equals(Category, other.Category, StringComparison.Ordinal) &&
                string.Equals(LocalId, other.LocalId, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is DefinitionId other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = Namespace.GetHashCode();
                hash = (hash * 397) ^ (Category == null ? 0 : StringComparer.Ordinal.GetHashCode(Category));
                hash = (hash * 397) ^ (LocalId == null ? 0 : StringComparer.Ordinal.GetHashCode(LocalId));
                return hash;
            }
        }

        public override string ToString()
        {
            return Namespace + ":" + (Category ?? string.Empty) + "/" + (LocalId ?? string.Empty);
        }

        public static bool operator ==(DefinitionId left, DefinitionId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DefinitionId left, DefinitionId right)
        {
            return !left.Equals(right);
        }
    }
}
