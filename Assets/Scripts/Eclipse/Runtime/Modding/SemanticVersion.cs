using System;

namespace Eclipse.Modding
{
    public readonly struct SemanticVersion : IComparable<SemanticVersion>, IEquatable<SemanticVersion>
    {
        public int Major { get; }
        public int Minor { get; }
        public int Patch { get; }
        public string PreRelease { get; }
        public string BuildMetadata { get; }

        private SemanticVersion(int major, int minor, int patch, string preRelease, string buildMetadata)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            PreRelease = preRelease ?? string.Empty;
            BuildMetadata = buildMetadata ?? string.Empty;
        }

        public static SemanticVersion Parse(string value)
        {
            SemanticVersion version;
            string error;
            if (!TryParse(value, false, out version, out error))
                throw new FormatException(error);
            return version;
        }

        public static bool TryParse(string value, out SemanticVersion version)
        {
            string error;
            return TryParse(value, false, out version, out error);
        }

        internal static bool TryParseRangeEndpoint(string value, out SemanticVersion version, out string error)
        {
            return TryParse(value, true, out version, out error);
        }

        private static bool TryParse(string value, bool allowPartial, out SemanticVersion version, out string error)
        {
            version = default;
            error = null;
            if (string.IsNullOrEmpty(value) || !string.Equals(value, value.Trim(), StringComparison.Ordinal))
            {
                error = "Semantic version must be a non-empty trimmed value.";
                return false;
            }

            string core = value;
            string build = string.Empty;
            int plus = core.IndexOf('+');
            if (plus >= 0)
            {
                if (plus != core.LastIndexOf('+') || plus == core.Length - 1)
                {
                    error = "Invalid semantic version build metadata: '" + value + "'.";
                    return false;
                }
                build = core.Substring(plus + 1);
                core = core.Substring(0, plus);
                if (!ValidIdentifiers(build, false))
                {
                    error = "Invalid semantic version build metadata: '" + value + "'.";
                    return false;
                }
            }

            string pre = string.Empty;
            int dash = core.IndexOf('-');
            if (dash >= 0)
            {
                if (dash == core.Length - 1)
                {
                    error = "Invalid semantic version prerelease: '" + value + "'.";
                    return false;
                }
                pre = core.Substring(dash + 1);
                core = core.Substring(0, dash);
                if (!ValidIdentifiers(pre, true))
                {
                    error = "Invalid semantic version prerelease: '" + value + "'.";
                    return false;
                }
            }

            string[] parts = core.Split('.');
            if ((!allowPartial && parts.Length != 3) || (allowPartial && (parts.Length < 1 || parts.Length > 3)))
            {
                error = "Semantic version must use MAJOR.MINOR.PATCH syntax: '" + value + "'.";
                return false;
            }

            int major = 0;
            int minor = 0;
            int patch = 0;
            if (!ParseNumber(parts[0], out major) ||
                (parts.Length > 1 && !ParseNumber(parts[1], out minor)) ||
                (parts.Length > 2 && !ParseNumber(parts[2], out patch)))
            {
                error = "Invalid semantic version number: '" + value + "'.";
                return false;
            }

            version = new SemanticVersion(major, minor, patch, pre, build);
            return true;
        }

        private static bool ParseNumber(string value, out int result)
        {
            result = 0;
            if (string.IsNullOrEmpty(value) || (value.Length > 1 && value[0] == '0')) return false;
            for (int i = 0; i < value.Length; i++)
                if (value[i] < '0' || value[i] > '9') return false;
            return int.TryParse(value, out result) && result >= 0;
        }

        private static bool ValidIdentifiers(string value, bool rejectNumericLeadingZeroes)
        {
            string[] identifiers = value.Split('.');
            foreach (string identifier in identifiers)
            {
                if (identifier.Length == 0) return false;
                bool numeric = true;
                foreach (char c in identifier)
                {
                    bool valid = (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') ||
                        (c >= '0' && c <= '9') || c == '-';
                    if (!valid) return false;
                    if (c < '0' || c > '9') numeric = false;
                }
                if (rejectNumericLeadingZeroes && numeric && identifier.Length > 1 && identifier[0] == '0') return false;
            }
            return true;
        }

        public int CompareTo(SemanticVersion other)
        {
            int result = Major.CompareTo(other.Major);
            if (result != 0) return result;
            result = Minor.CompareTo(other.Minor);
            if (result != 0) return result;
            result = Patch.CompareTo(other.Patch);
            if (result != 0) return result;
            return ComparePreRelease(PreRelease, other.PreRelease);
        }

        private static int ComparePreRelease(string left, string right)
        {
            bool leftEmpty = string.IsNullOrEmpty(left);
            bool rightEmpty = string.IsNullOrEmpty(right);
            if (leftEmpty || rightEmpty)
                return leftEmpty == rightEmpty ? 0 : (leftEmpty ? 1 : -1);

            string[] a = left.Split('.');
            string[] b = right.Split('.');
            int count = Math.Min(a.Length, b.Length);
            for (int i = 0; i < count; i++)
            {
                int ai;
                int bi;
                bool an = int.TryParse(a[i], out ai);
                bool bn = int.TryParse(b[i], out bi);
                int result;
                if (an && bn) result = ai.CompareTo(bi);
                else if (an != bn) result = an ? -1 : 1;
                else result = string.CompareOrdinal(a[i], b[i]);
                if (result != 0) return result;
            }
            return a.Length.CompareTo(b.Length);
        }

        public bool Equals(SemanticVersion other) => CompareTo(other) == 0;
        public override bool Equals(object obj) => obj is SemanticVersion other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = Major;
                hash = (hash * 397) ^ Minor;
                hash = (hash * 397) ^ Patch;
                hash = (hash * 397) ^ StringComparer.Ordinal.GetHashCode(PreRelease ?? string.Empty);
                return hash;
            }
        }

        public override string ToString()
        {
            string value = Major + "." + Minor + "." + Patch;
            if (!string.IsNullOrEmpty(PreRelease)) value += "-" + PreRelease;
            if (!string.IsNullOrEmpty(BuildMetadata)) value += "+" + BuildMetadata;
            return value;
        }

        public static bool operator <(SemanticVersion left, SemanticVersion right) => left.CompareTo(right) < 0;
        public static bool operator >(SemanticVersion left, SemanticVersion right) => left.CompareTo(right) > 0;
        public static bool operator <=(SemanticVersion left, SemanticVersion right) => left.CompareTo(right) <= 0;
        public static bool operator >=(SemanticVersion left, SemanticVersion right) => left.CompareTo(right) >= 0;
        public static bool operator ==(SemanticVersion left, SemanticVersion right) => left.Equals(right);
        public static bool operator !=(SemanticVersion left, SemanticVersion right) => !left.Equals(right);
    }
}
