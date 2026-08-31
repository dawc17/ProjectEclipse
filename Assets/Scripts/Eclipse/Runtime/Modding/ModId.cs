using System;

namespace Eclipse.Modding
{
    public readonly struct ModId : IEquatable<ModId>
    {
        private readonly string _value;

        private ModId(string value)
        {
            _value = value;
        }

        public string Value => _value ?? string.Empty;

        public static ModId Parse(string value)
        {
            string canonical;
            string error;
            if (!ModIdentityRules.TryNormalizeModId(value, out canonical, out error))
                throw new FormatException(error);
            return new ModId(canonical);
        }

        public static bool TryParse(string value, out ModId id)
        {
            string canonical;
            string error;
            if (!ModIdentityRules.TryNormalizeModId(value, out canonical, out error))
            {
                id = default;
                return false;
            }
            id = new ModId(canonical);
            return true;
        }

        public bool Equals(ModId other)
        {
            return string.Equals(_value, other._value, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is ModId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _value == null ? 0 : StringComparer.Ordinal.GetHashCode(_value);
        }

        public override string ToString()
        {
            return Value;
        }

        public static bool operator ==(ModId left, ModId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ModId left, ModId right)
        {
            return !left.Equals(right);
        }
    }

    internal static class ModIdentityRules
    {
        public static bool TryNormalizeModId(string value, out string canonical, out string error)
        {
            canonical = null;
            error = null;
            if (string.IsNullOrEmpty(value))
            {
                error = "Mod ID must not be empty.";
                return false;
            }
            if (!string.Equals(value, value.Trim(), StringComparison.Ordinal))
            {
                error = "Mod ID must not contain leading or trailing whitespace: '" + value + "'.";
                return false;
            }

            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                bool valid = (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') ||
                    c == '.' || c == '_' || c == '-';
                if (!valid)
                {
                    error = "Invalid mod ID '" + value +
                        "'. IDs must use lowercase ASCII letters, digits, '.', '_' or '-'.";
                    return false;
                }
            }

            canonical = value;
            return true;
        }

        public static bool TryNormalizePath(string value, out string canonical, out string error)
        {
            canonical = null;
            error = null;
            if (string.IsNullOrEmpty(value))
            {
                error = "Namespaced path must not be empty.";
                return false;
            }
            if (!string.Equals(value, value.Trim(), StringComparison.Ordinal))
            {
                error = "Namespaced path must not contain leading or trailing whitespace: '" + value + "'.";
                return false;
            }

            string normalized = value.Replace('\\', '/');
            if (normalized[0] == '/' || normalized[normalized.Length - 1] == '/' || normalized.IndexOf(':') >= 0)
            {
                error = "Unsafe namespaced path: '" + value + "'.";
                return false;
            }

            string[] parts = normalized.Split('/');
            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i];
                if (part.Length == 0 || part == "." || part == "..")
                {
                    error = "Unsafe namespaced path: '" + value + "'.";
                    return false;
                }
                for (int j = 0; j < part.Length; j++)
                {
                    if (char.IsControl(part[j]))
                    {
                        error = "Namespaced path contains a control character: '" + value + "'.";
                        return false;
                    }
                }
            }

            canonical = normalized.ToLowerInvariant();
            return true;
        }
    }
}
