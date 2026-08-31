using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Eclipse.Modding
{
    public static class ModLocalizationLoader
    {
        private static readonly UTF8Encoding StrictUtf8 = new UTF8Encoding(false, true);
        private const int MaxLocalizationBytes = 4 * 1024 * 1024;

        public static int Load(ModDescriptor mod, AssetResolver assets, ModRegistrationTransaction registration)
        {
            if (mod == null) throw new ArgumentNullException(nameof(mod));
            if (assets == null) throw new ArgumentNullException(nameof(assets));
            if (registration == null) throw new ArgumentNullException(nameof(registration));
            if (registration.Mod.Id != mod.Id)
                throw new ArgumentException("Localization transaction belongs to another mod.", nameof(registration));

            IAssetProvider provider;
            if (!assets.TryGetProvider(mod.Id, out provider))
                throw new ModContentException("No asset provider is mounted for mod '" + mod.Id + "'.");
            IAssetEnumerableProvider enumerable = provider as IAssetEnumerableProvider;
            if (enumerable == null) return 0;

            var files = new List<AssetMetadata>();
            foreach (AssetMetadata metadata in enumerable.Assets)
            {
                if (metadata.Kind == AssetKind.Text && metadata.Format == ".toml" &&
                    metadata.Id.Path.StartsWith("localizations/", StringComparison.Ordinal))
                    files.Add(metadata);
            }
            files.Sort((a, b) => string.CompareOrdinal(a.Id.Path, b.Id.Path));

            int count = 0;
            foreach (AssetMetadata metadata in files)
            {
                string language = metadata.Id.Path.Substring("localizations/".Length);
                if (language.Length == 0 || language.IndexOf('/') >= 0)
                    throw new ModContentException("Localization files must be directly below localizations/: '" +
                        metadata.DiagnosticSource + "'.");

                AssetBytes bytes;
                if (!assets.TryRead(metadata.Id, out bytes))
                    throw new ModContentException("Localization file disappeared after indexing: '" + metadata.Id + "'.");
                if (bytes.Data.Length > MaxLocalizationBytes)
                    throw new ModContentException("Localization file exceeds " + MaxLocalizationBytes +
                        " bytes: '" + metadata.DiagnosticSource + "'.");

                string text;
                try { text = StrictUtf8.GetString(bytes.Data); }
                catch (DecoderFallbackException exception)
                {
                    throw new ModContentException("Localization file is not valid UTF-8: '" +
                        metadata.DiagnosticSource + "'.", exception);
                }
                count += Parse(text, metadata.DiagnosticSource, language, registration);
            }
            return count;
        }

        internal static int Parse(string text, string source, string language, ModRegistrationTransaction registration)
        {
            int count = 0;
            string[] lines = (text ?? string.Empty).Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                string line = StripComment(lines[i]).Trim();
                if (line.Length == 0) continue;
                int equals = FindEquals(line);
                if (equals <= 0) throw Error(source, i + 1, "Expected key = \"value\".");
                string key = line.Substring(0, equals).Trim();
                string rawValue = line.Substring(equals + 1).Trim();
                if (key.Length == 0) throw Error(source, i + 1, "Localization key must not be empty.");
                registration.AddLocalization(key, language, ParseString(rawValue, source, i + 1));
                count++;
            }
            return count;
        }

        private static int FindEquals(string line)
        {
            bool quoted = false;
            bool escaped = false;
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (quoted && escaped) { escaped = false; continue; }
                if (quoted && c == '\\') { escaped = true; continue; }
                if (c == '"') { quoted = !quoted; continue; }
                if (!quoted && c == '=') return i;
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
                if (quoted && escaped) { escaped = false; continue; }
                if (quoted && c == '\\') { escaped = true; continue; }
                if (c == '"') { quoted = !quoted; continue; }
                if (!quoted && c == '#') return line.Substring(0, i);
            }
            return line;
        }

        private static string ParseString(string value, string source, int line)
        {
            if (value.Length < 2 || value[0] != '"' || value[value.Length - 1] != '"')
                throw Error(source, line, "Expected quoted localization value.");

            var result = new StringBuilder(value.Length - 2);
            for (int i = 1; i < value.Length - 1; i++)
            {
                char c = value[i];
                if (c != '\\') { result.Append(c); continue; }
                if (++i >= value.Length - 1) throw Error(source, line, "Incomplete string escape.");
                switch (value[i])
                {
                    case '\\': result.Append('\\'); break;
                    case '"': result.Append('"'); break;
                    case 'n': result.Append('\n'); break;
                    case 'r': result.Append('\r'); break;
                    case 't': result.Append('\t'); break;
                    default: throw Error(source, line, "Unsupported string escape '\\" + value[i] + "'.");
                }
            }
            return result.ToString();
        }

        private static ModContentException Error(string source, int line, string message)
        {
            return new ModContentException((source ?? "localization") + ":" + line + ": " + message);
        }
    }
}
