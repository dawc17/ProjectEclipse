using System;
using System.Collections.Generic;

namespace Eclipse.Modding
{
    public sealed class VersionRange
    {
        private readonly Comparator[] _comparators;
        private readonly string _expression;

        private VersionRange(Comparator[] comparators, string expression)
        {
            _comparators = comparators;
            _expression = expression;
        }

        public static VersionRange Parse(string expression)
        {
            VersionRange range;
            string error;
            if (!TryParse(expression, out range, out error))
                throw new FormatException(error);
            return range;
        }

        public static bool TryParse(string expression, out VersionRange range)
        {
            string error;
            return TryParse(expression, out range, out error);
        }

        internal static bool TryParse(string expression, out VersionRange range, out string error)
        {
            range = null;
            error = null;
            if (string.IsNullOrWhiteSpace(expression))
            {
                error = "Version range must not be empty.";
                return false;
            }

            string[] terms = expression.Trim().Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            var comparators = new List<Comparator>();
            foreach (string term in terms)
            {
                string op;
                string versionText;
                if (term.StartsWith(">=", StringComparison.Ordinal) || term.StartsWith("<=", StringComparison.Ordinal))
                {
                    op = term.Substring(0, 2);
                    versionText = term.Substring(2);
                }
                else if (term.StartsWith(">", StringComparison.Ordinal) ||
                    term.StartsWith("<", StringComparison.Ordinal) ||
                    term.StartsWith("=", StringComparison.Ordinal))
                {
                    op = term.Substring(0, 1);
                    versionText = term.Substring(1);
                }
                else
                {
                    op = "=";
                    versionText = term;
                }

                SemanticVersion version;
                string versionError;
                if (!SemanticVersion.TryParseRangeEndpoint(versionText, out version, out versionError))
                {
                    error = "Invalid version range term '" + term + "': " + versionError;
                    return false;
                }
                comparators.Add(new Comparator(op, version));
            }

            range = new VersionRange(comparators.ToArray(), expression.Trim());
            return true;
        }

        public bool Contains(SemanticVersion version)
        {
            foreach (Comparator comparator in _comparators)
            {
                int cmp = version.CompareTo(comparator.Version);
                if (comparator.Operator == ">=" && cmp < 0) return false;
                if (comparator.Operator == ">" && cmp <= 0) return false;
                if (comparator.Operator == "<=" && cmp > 0) return false;
                if (comparator.Operator == "<" && cmp >= 0) return false;
                if (comparator.Operator == "=" && cmp != 0) return false;
            }
            return true;
        }

        public override string ToString() => _expression;

        private readonly struct Comparator
        {
            public string Operator { get; }
            public SemanticVersion Version { get; }

            public Comparator(string op, SemanticVersion version)
            {
                Operator = op;
                Version = version;
            }
        }
    }
}
