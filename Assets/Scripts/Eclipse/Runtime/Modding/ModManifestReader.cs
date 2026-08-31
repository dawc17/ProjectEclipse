using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Eclipse.Modding
{
    public static class ModManifestReader
    {
        public static ModManifest ReadExternalFile(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            return ParseExternal(File.ReadAllText(path), path);
        }

        public static ModManifest ParseExternal(string text, string sourceName = "mod.toml")
        {
            return Parse(text, sourceName, false);
        }

        internal static ModManifest Parse(string text, string sourceName, bool allowReservedId)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (string.IsNullOrEmpty(sourceName)) sourceName = "mod.toml";

            int? schema = null;
            string id = null;
            string name = null;
            string version = null;
            string api = null;
            string[] authors = null;
            string entrypoint = null;
            string[] capabilities = null;
            var dependencies = new List<DependencyBuilder>();
            DependencyBuilder currentDependency = null;
            var rootKeys = new HashSet<string>(StringComparer.Ordinal);

            string[] lines = text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                int lineNumber = i + 1;
                string line = StripComment(lines[i]).Trim();
                if (line.Length == 0) continue;

                if (line == "[[dependencies]]")
                {
                    currentDependency = new DependencyBuilder(lineNumber);
                    dependencies.Add(currentDependency);
                    continue;
                }
                if (line.StartsWith("[", StringComparison.Ordinal))
                    Fail(sourceName, lineNumber, "Unsupported TOML section '" + line + "'.");

                int equals = FindEquals(line);
                if (equals <= 0) Fail(sourceName, lineNumber, "Expected key = value.");
                string key = line.Substring(0, equals).Trim();
                string raw = line.Substring(equals + 1).Trim();
                if (key.Length == 0 || raw.Length == 0) Fail(sourceName, lineNumber, "Expected key = value.");

                if (currentDependency != null)
                {
                    if (key == "id") currentDependency.SetId(ParseString(raw, sourceName, lineNumber), sourceName, lineNumber);
                    else if (key == "version") currentDependency.SetVersion(ParseString(raw, sourceName, lineNumber), sourceName, lineNumber);
                    else Fail(sourceName, lineNumber,
                        "Unknown dependency field '" + key + "'. Root fields must appear before [[dependencies]].");
                    continue;
                }

                if (!rootKeys.Add(key)) Fail(sourceName, lineNumber, "Duplicate root field '" + key + "'.");
                switch (key)
                {
                    case "schema": schema = ParseInteger(raw, sourceName, lineNumber); break;
                    case "id": id = ParseString(raw, sourceName, lineNumber); break;
                    case "name": name = ParseString(raw, sourceName, lineNumber); break;
                    case "version": version = ParseString(raw, sourceName, lineNumber); break;
                    case "api": api = ParseString(raw, sourceName, lineNumber); break;
                    case "authors": authors = ParseStringArray(raw, sourceName, lineNumber); break;
                    case "entrypoint": entrypoint = ParseString(raw, sourceName, lineNumber); break;
                    case "capabilities": capabilities = ParseStringArray(raw, sourceName, lineNumber); break;
                    default: Fail(sourceName, lineNumber, "Unknown root field '" + key + "'."); break;
                }
            }

            if (schema != 1) Fail(sourceName, 0, "Manifest schema must be exactly 1.");
            if (id == null || name == null || version == null || api == null || authors == null ||
                entrypoint == null || capabilities == null)
                Fail(sourceName, 0,
                    "Manifest requires schema, id, name, version, api, authors, entrypoint and capabilities.");
            if (string.IsNullOrWhiteSpace(name)) Fail(sourceName, 0, "Manifest name must not be empty.");
            if (authors.Length == 0) Fail(sourceName, 0, "Manifest authors must contain at least one author.");

            ModId modId;
            try { modId = ModId.Parse(id); }
            catch (FormatException ex) { Fail(sourceName, 0, ex.Message); throw; }
            if (!allowReservedId && (modId.Value == "core" || modId.Value == "sf2de"))
                Fail(sourceName, 0, "External mods may not use reserved ID '" + modId + "'.");

            SemanticVersion semanticVersion;
            try { semanticVersion = SemanticVersion.Parse(version); }
            catch (FormatException ex) { Fail(sourceName, 0, ex.Message); throw; }
            VersionRange apiRange;
            try { apiRange = VersionRange.Parse(api); }
            catch (FormatException ex) { Fail(sourceName, 0, ex.Message); throw; }

            string normalizedEntrypoint;
            string pathError;
            if (!ModIdentityRules.TryNormalizePath(entrypoint, out normalizedEntrypoint, out pathError) ||
                !normalizedEntrypoint.StartsWith("scripts/", StringComparison.Ordinal) ||
                !normalizedEntrypoint.EndsWith(".lua", StringComparison.Ordinal))
                Fail(sourceName, 0, "Entrypoint must be a safe scripts/*.lua path: '" + entrypoint + "'.");

            NormalizeSimpleList(authors, "author", sourceName, false);
            NormalizeSimpleList(capabilities, "capability", sourceName, true);

            var parsedDependencies = new List<ModDependency>();
            var dependencyIds = new HashSet<ModId>();
            foreach (DependencyBuilder dependency in dependencies)
            {
                if (dependency.Id == null || dependency.Version == null)
                    Fail(sourceName, dependency.Line, "Each [[dependencies]] table requires id and version.");
                ModId dependencyId;
                try { dependencyId = ModId.Parse(dependency.Id); }
                catch (FormatException ex) { Fail(sourceName, dependency.Line, ex.Message); throw; }
                if (!dependencyIds.Add(dependencyId))
                    Fail(sourceName, dependency.Line, "Duplicate dependency '" + dependencyId + "'.");
                VersionRange dependencyRange;
                try { dependencyRange = VersionRange.Parse(dependency.Version); }
                catch (FormatException ex) { Fail(sourceName, dependency.Line, ex.Message); throw; }
                parsedDependencies.Add(new ModDependency(dependencyId, dependencyRange));
            }

            return new ModManifest(1, modId, name.Trim(), semanticVersion, apiRange,
                authors, normalizedEntrypoint, capabilities, parsedDependencies.ToArray());
        }

        private static void NormalizeSimpleList(string[] values, string kind, string source, bool identifier)
        {
            var seen = new HashSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < values.Length; i++)
            {
                string value = values[i].Trim();
                if (value.Length == 0) Fail(source, 0, "Manifest " + kind + " must not be empty.");
                if (identifier)
                {
                    ModId token;
                    if (!ModId.TryParse(value, out token)) Fail(source, 0, "Invalid capability '" + value + "'.");
                    value = token.Value;
                }
                if (!seen.Add(value)) Fail(source, 0, "Duplicate " + kind + " '" + value + "'.");
                values[i] = value;
            }
        }

        private static int FindEquals(string line)
        {
            bool quoted = false;
            bool escaped = false;
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (escaped) { escaped = false; continue; }
                if (quoted && c == '\\') { escaped = true; continue; }
                if (c == '"') quoted = !quoted;
                else if (!quoted && c == '=') return i;
            }
            return -1;
        }

        private static string StripComment(string line)
        {
            bool quoted = false;
            bool escaped = false;
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (escaped) { escaped = false; continue; }
                if (quoted && c == '\\') { escaped = true; continue; }
                if (c == '"') quoted = !quoted;
                else if (!quoted && c == '#') return line.Substring(0, i);
            }
            return line;
        }

        private static int ParseInteger(string raw, string source, int line)
        {
            int value;
            if (!int.TryParse(raw, out value))
                Fail(source, line, "Expected integer value, found '" + raw + "'.");
            return value;
        }

        private static string ParseString(string raw, string source, int line)
        {
            if (raw.Length < 2 || raw[0] != '"' || raw[raw.Length - 1] != '"')
                Fail(source, line, "Expected a double-quoted string.");
            var result = new StringBuilder(raw.Length - 2);
            bool escaped = false;
            for (int i = 1; i < raw.Length - 1; i++)
            {
                char c = raw[i];
                if (!escaped)
                {
                    if (c == '\\') escaped = true;
                    else if (c == '"') Fail(source, line, "Unescaped quote in string.");
                    else result.Append(c);
                    continue;
                }
                if (c == 'n') result.Append('\n');
                else if (c == 'r') result.Append('\r');
                else if (c == 't') result.Append('\t');
                else if (c == '"' || c == '\\') result.Append(c);
                else Fail(source, line, "Unsupported string escape '\\" + c + "'.");
                escaped = false;
            }
            if (escaped) Fail(source, line, "String ends with an incomplete escape.");
            return result.ToString();
        }

        private static string[] ParseStringArray(string raw, string source, int line)
        {
            if (raw.Length < 2 || raw[0] != '[' || raw[raw.Length - 1] != ']')
                Fail(source, line, "Expected an array of double-quoted strings.");
            string inner = raw.Substring(1, raw.Length - 2).Trim();
            if (inner.Length == 0) return Array.Empty<string>();
            var values = new List<string>();
            int start = 0;
            bool quoted = false;
            bool escaped = false;
            for (int i = 0; i <= inner.Length; i++)
            {
                bool end = i == inner.Length;
                char c = end ? '\0' : inner[i];
                if (!end)
                {
                    if (escaped) { escaped = false; continue; }
                    if (quoted && c == '\\') { escaped = true; continue; }
                    if (c == '"') quoted = !quoted;
                }
                if (end || (!quoted && c == ','))
                {
                    string token = inner.Substring(start, i - start).Trim();
                    if (token.Length == 0) Fail(source, line, "Empty array element.");
                    values.Add(ParseString(token, source, line));
                    start = i + 1;
                }
            }
            if (quoted) Fail(source, line, "Unterminated quoted string in array.");
            return values.ToArray();
        }

        private static void Fail(string source, int line, string message)
        {
            string location = line > 0 ? source + ":" + line : source;
            throw new FormatException(location + ": " + message);
        }

        private sealed class DependencyBuilder
        {
            public int Line { get; }
            public string Id { get; private set; }
            public string Version { get; private set; }

            public DependencyBuilder(int line) { Line = line; }

            public void SetId(string value, string source, int line)
            {
                if (Id != null) Fail(source, line, "Duplicate dependency id.");
                Id = value;
            }

            public void SetVersion(string value, string source, int line)
            {
                if (Version != null) Fail(source, line, "Duplicate dependency version.");
                Version = value;
            }
        }
    }
}
