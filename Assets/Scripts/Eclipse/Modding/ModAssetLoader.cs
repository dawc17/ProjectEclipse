using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Eclipse.Content.TarAssets;
using UnityEngine;

namespace Eclipse.Modding
{
    public interface IRuntimeAssetProvider
    {
        bool TryLoadUnityAsset<T>(AssetId id, out T asset) where T : UnityEngine.Object;
        bool TryLoadUnityAssets<T>(AssetId id, out T[] assets) where T : UnityEngine.Object;
        bool TryLoadModelText(AssetId id, out string text);
    }

    public sealed class ModAssetLoader : IDisposable
    {
        private static readonly UTF8Encoding StrictUtf8 = new UTF8Encoding(false, true);
        private const int MaxTextBytes = 64 * 1024 * 1024;

        private readonly AssetResolver _resolver;
        private readonly Dictionary<AssetId, Sprite> _sprites = new Dictionary<AssetId, Sprite>();
        // Import settings belong to the texture instance. Cache variants so descriptors with
        // different settings cannot mutate each other's textures or depend on load order.
        private readonly Dictionary<(AssetId, FilterMode, TextureWrapMode, bool), Texture2D> _textures =
            new Dictionary<(AssetId, FilterMode, TextureWrapMode, bool), Texture2D>();
        private readonly Dictionary<AssetId, AudioClip> _audio = new Dictionary<AssetId, AudioClip>();

        public ModAssetLoader(AssetResolver resolver)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        public T LoadUnityAsset<T>(AssetId id) where T : UnityEngine.Object
        {
            AssetMetadata metadata;
            if (!_resolver.TryDescribe(id, out metadata)) return null;
            IRuntimeAssetProvider runtimeProvider;
            if (TryGetRuntimeProvider(id, out runtimeProvider))
            {
                T runtimeAsset;
                return runtimeProvider.TryLoadUnityAsset(id, out runtimeAsset) ? runtimeAsset : null;
            }
            if (typeof(T) == typeof(Sprite)) return LoadSprite(id) as T;
            if (typeof(T) == typeof(Texture2D))
                return LoadTexture(id) as T;
            if (typeof(T) == typeof(AudioClip)) return LoadAudio(id) as T;
            return null;
        }

        public T[] LoadUnityAssets<T>(AssetId id) where T : UnityEngine.Object
        {
            AssetMetadata metadata;
            if (!_resolver.TryDescribe(id, out metadata)) return null;
            IRuntimeAssetProvider runtimeProvider;
            if (TryGetRuntimeProvider(id, out runtimeProvider))
            {
                T[] runtimeAssets;
                return runtimeProvider.TryLoadUnityAssets(id, out runtimeAssets) ? runtimeAssets : null;
            }
            T asset = LoadUnityAsset<T>(id);
            return asset == null ? null : new[] { asset };
        }

        public Sprite LoadSprite(AssetId id)
        {
            AssetMetadata metadata;
            if (!_resolver.TryDescribe(id, out metadata)) return null;
            IRuntimeAssetProvider runtimeProvider;
            if (TryGetRuntimeProvider(id, out runtimeProvider))
            {
                Sprite runtimeAsset;
                return runtimeProvider.TryLoadUnityAsset(id, out runtimeAsset) ? runtimeAsset : null;
            }

            Sprite cached;
            if (_sprites.TryGetValue(id, out cached) && cached != null) return cached;

            AssetBytes bytes;
            if (!_resolver.TryRead(id, out bytes)) return null;
            if (bytes.Metadata.Kind != AssetKind.Sprite)
                throw new InvalidDataException("Asset is not a sprite: " + id);

            SpriteAssetDescriptor descriptor;
            Texture2D texture;
            if (bytes.Metadata.Format == ".asset")
            {
                descriptor = SpriteAssetDescriptor.Parse(DecodeText(bytes, id), id.ToString(), standalone: true);
                AssetId textureId = descriptor.GetTextureId(id);
                AssetMetadata textureMetadata;
                if (!_resolver.TryDescribe(textureId, out textureMetadata) ||
                    textureMetadata.Kind != AssetKind.Texture || textureMetadata.Format != ".png")
                    throw new InvalidDataException("Sprite '" + id + "' requires a PNG texture: " + textureId);
                texture = LoadTextureData(textureId, descriptor);
                if (texture == null) throw new InvalidDataException("Sprite texture is missing: " + textureId);
            }
            else if (bytes.Metadata.Format == ".png")
            {
                descriptor = LoadSpriteDescriptor(id);
                texture = LoadTextureData(id, descriptor, bytes);
            }
            else throw new InvalidDataException("Unsupported sprite format: " + id);

            Rect rect = descriptor.HasRect ? descriptor.Rect : new Rect(0f, 0f, texture.width, texture.height);
            ValidateSpriteRect(rect, texture, id);
            ValidateBorder(descriptor.Border, rect, id);
            Sprite sprite = Sprite.Create(texture, rect, descriptor.Pivot, descriptor.PixelsPerUnit, 0,
                SpriteMeshType.FullRect, descriptor.Border);
            if (sprite == null) throw new InvalidDataException("Unity cannot create loose mod sprite: " + id);
            sprite.name = GetLastSegment(id.Path);
            _sprites[id] = sprite;
            return sprite;
        }

        public Texture2D LoadTexture(AssetId id)
        {
            AssetMetadata metadata;
            if (!_resolver.TryDescribe(id, out metadata)) return null;
            IRuntimeAssetProvider runtimeProvider;
            if (TryGetRuntimeProvider(id, out runtimeProvider))
            {
                Texture2D runtimeAsset;
                return runtimeProvider.TryLoadUnityAsset(id, out runtimeAsset) ? runtimeAsset : null;
            }
            // Preserve early mods and legacy callers that request a sprite's backing texture.
            if (metadata.Kind == AssetKind.Sprite)
            {
                Sprite sprite = LoadSprite(id);
                return sprite == null ? null : sprite.texture;
            }
            if (metadata.Kind != AssetKind.Texture || metadata.Format != ".png")
                throw new InvalidDataException("Asset is not a PNG texture: " + id);
            return LoadTextureData(id, SpriteAssetDescriptor.Parse(string.Empty, id.ToString()));
        }

        private Texture2D LoadTextureData(AssetId id, SpriteAssetDescriptor descriptor, AssetBytes bytes = null)
        {
            var key = (id, descriptor.Filter, descriptor.Wrap, descriptor.Mipmaps);
            Texture2D cached;
            if (_textures.TryGetValue(key, out cached) && cached != null) return cached;
            if (bytes == null && !_resolver.TryRead(id, out bytes)) return null;
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, descriptor.Mipmaps);
            try
            {
                if (!ImageConversion.LoadImage(texture, bytes.Data, false))
                    throw new InvalidDataException("Unity cannot decode loose mod PNG: " + id);
                texture.name = GetLastSegment(id.Path);
                texture.filterMode = descriptor.Filter;
                texture.wrapMode = descriptor.Wrap;
                _textures[key] = texture;
                return texture;
            }
            catch { Destroy(texture); throw; }
        }

        public string LoadModelText(AssetId id)
        {
            AssetMetadata metadata;
            if (!_resolver.TryDescribe(id, out metadata)) return null;
            IRuntimeAssetProvider runtimeProvider;
            if (TryGetRuntimeProvider(id, out runtimeProvider))
            {
                string text;
                return runtimeProvider.TryLoadModelText(id, out text) ? text : null;
            }
            AssetBytes bytes;
            if (!_resolver.TryRead(id, out bytes)) return null;
            if (bytes.Metadata.Kind != AssetKind.Model)
                throw new InvalidDataException("Asset is not model XML: " + id);
            return DecodeText(bytes, id);
        }

        public string LoadText(AssetId id)
        {
            AssetBytes bytes;
            if (!_resolver.TryRead(id, out bytes)) return null;
            if (bytes.Metadata.Kind != AssetKind.Text && bytes.Metadata.Kind != AssetKind.Model)
                throw new InvalidDataException("Asset is not text: " + id);
            return DecodeText(bytes, id);
        }

        public byte[] LoadBinary(AssetId id)
        {
            AssetBytes bytes;
            if (!_resolver.TryRead(id, out bytes)) return null;
            if (bytes.Metadata.Kind != AssetKind.Binary)
                throw new InvalidDataException("Asset is not binary data: " + id);
            return bytes.Data;
        }

        public AudioClip LoadAudio(AssetId id)
        {
            AssetMetadata metadata;
            if (!_resolver.TryDescribe(id, out metadata)) return null;
            IRuntimeAssetProvider runtimeProvider;
            if (TryGetRuntimeProvider(id, out runtimeProvider))
            {
                AudioClip runtimeAsset;
                return runtimeProvider.TryLoadUnityAsset(id, out runtimeAsset) ? runtimeAsset : null;
            }
            AudioClip cached;
            if (_audio.TryGetValue(id, out cached) && cached != null) return cached;

            AssetBytes bytes;
            if (!_resolver.TryRead(id, out bytes)) return null;
            if (bytes.Metadata.Kind != AssetKind.Audio)
                throw new InvalidDataException("Asset is not audio: " + id);
            if (bytes.Metadata.Format != ".wav")
                throw new NotSupportedException("Loose mod audio currently supports PCM16 WAV only: " + id);

            AudioClip clip = WaveDecoder.Decode(bytes.Data, GetLastSegment(id.Path));
            _audio.Add(id, clip);
            return clip;
        }

        public void Dispose()
        {
            foreach (Sprite sprite in _sprites.Values)
                if (sprite != null) Destroy(sprite);
            foreach (Texture2D texture in _textures.Values)
                if (texture != null) Destroy(texture);
            foreach (AudioClip clip in _audio.Values)
                if (clip != null) Destroy(clip);
            _sprites.Clear();
            _textures.Clear();
            _audio.Clear();
        }

        private SpriteAssetDescriptor LoadSpriteDescriptor(AssetId id)
        {
            AssetId descriptorId = AssetId.Parse(id.Namespace.Value + ":" + id.Path + ".sprite");
            AssetBytes bytes;
            if (!_resolver.TryRead(descriptorId, out bytes)) return SpriteAssetDescriptor.Parse(string.Empty, descriptorId.ToString());
            if (bytes.Metadata.Kind != AssetKind.Text || bytes.Metadata.Format != ".toml")
                throw new InvalidDataException("Sprite sidecar must be TOML text: " + descriptorId);
            return SpriteAssetDescriptor.Parse(DecodeText(bytes, descriptorId), descriptorId.ToString());
        }

        private static string DecodeText(AssetBytes bytes, AssetId id)
        {
            if (bytes.Data.Length > MaxTextBytes) throw new InvalidDataException("Text asset exceeds size limit: " + id);
            try { return StrictUtf8.GetString(bytes.Data); }
            catch (DecoderFallbackException exception)
            {
                throw new InvalidDataException("Text asset is not valid UTF-8: " + id, exception);
            }
        }

        private static void ValidateSpriteRect(Rect rect, Texture2D texture, AssetId id)
        {
            if (rect.width <= 0f || rect.height <= 0f || rect.x < 0f || rect.y < 0f ||
                rect.xMax > texture.width || rect.yMax > texture.height)
                throw new InvalidDataException("Sprite rect is outside PNG bounds: " + id + "; rect=" + rect +
                    "; texture=" + texture.width + "x" + texture.height);
        }

        private static void ValidateBorder(Vector4 border, Rect rect, AssetId id)
        {
            if (border.x < 0f || border.y < 0f || border.z < 0f || border.w < 0f ||
                border.x + border.z > rect.width || border.y + border.w > rect.height)
                throw new InvalidDataException("Sprite border is outside rect: " + id);
        }

        private static string GetLastSegment(string path)
        {
            int slash = path.LastIndexOf('/');
            return slash < 0 ? path : path.Substring(slash + 1);
        }

        private bool TryGetRuntimeProvider(AssetId id, out IRuntimeAssetProvider provider)
        {
            provider = null;
            IAssetProvider raw;
            if (!_resolver.TryGetProvider(id.Namespace, out raw)) return false;
            provider = raw as IRuntimeAssetProvider;
            return provider != null;
        }

        private static void Destroy(UnityEngine.Object value)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) UnityEngine.Object.DestroyImmediate(value);
            else UnityEngine.Object.Destroy(value);
#else
            UnityEngine.Object.Destroy(value);
#endif
        }
    }

    // Runtime-only bridge used by recovered host systems. Public scripting receives typed
    // handles instead of this loader or raw Unity objects.
    internal static class ModAssetBinding
    {
        internal static bool TryLoadAudio(string reference, out AudioClip clip)
        {
            clip = null;
            AssetId id;
            if (!TryParseExternal(reference, out id) || !ModRuntime.IsInitialized) return false;
            clip = ModRuntime.Host.TypedAssets.LoadAudio(id);
            return clip != null;
        }

        internal static bool TryLoadBinary(string reference, out byte[] data)
        {
            data = null;
            AssetId id;
            if (!TryParseExternal(reference, out id) || !ModRuntime.IsInitialized) return false;
            data = ModRuntime.Host.TypedAssets.LoadBinary(id);
            return data != null;
        }

        internal static bool TryLoadSprite(string reference, out Sprite sprite)
        {
            sprite = null;
            AssetId id;
            if (!TryParseExternal(reference, out id) || !ModRuntime.IsInitialized) return false;
            sprite = ModRuntime.Host.TypedAssets.LoadSprite(id);
            return sprite != null;
        }

        internal static bool IsQualified(string reference)
        {
            AssetId id;
            return TryParseExternal(reference, out id);
        }

        private static bool TryParseExternal(string reference, out AssetId id)
        {
            if (!AssetId.TryParse(reference, out id)) return false;
            return id.Namespace.Value != "core";
        }
    }

    internal static class ExternalLocationRuntime
    {
        internal sealed class Entry
        {
            internal XmlDocument Params;
            internal string MusicAsset;
        }

        private static readonly Dictionary<string, Entry> Entries =
            new Dictionary<string, Entry>(StringComparer.Ordinal);

        internal static void Set(string runtimeName, XmlDocument parameters, string musicAsset)
        {
            if (string.IsNullOrWhiteSpace(runtimeName))
                throw new ArgumentException("External location runtime name must not be empty.", "runtimeName");
            if (parameters == null || parameters["Root"] == null)
                throw new ArgumentException("External location parameters require a Root element.", "parameters");
            if (!string.IsNullOrEmpty(musicAsset) && !ModAssetBinding.IsQualified(musicAsset))
                throw new ArgumentException("External location music must be a mod-owned qualified asset.", "musicAsset");
            Entries[runtimeName] = new Entry
            {
                Params = (XmlDocument)parameters.CloneNode(true),
                MusicAsset = musicAsset ?? string.Empty
            };
        }

        internal static bool TryGet(string runtimeName, out Entry entry)
        {
            return Entries.TryGetValue(runtimeName ?? string.Empty, out entry);
        }

        internal static void Remove(string runtimeName)
        {
            if (!string.IsNullOrEmpty(runtimeName)) Entries.Remove(runtimeName);
        }

        internal static void Clear()
        {
            Entries.Clear();
        }
    }

    internal sealed class ExternalLocaleMetadata
    {
        internal string Name;
        internal string Locale;
        internal string Alias;
        internal string FileIcon;
        internal string FileIconSelected;
        internal string LoaderImage;
        internal string PreloaderImage;
        internal bool IsAsian;
        internal string ContentFont;
        internal string TitleFont;
        internal string ButtonFont;
        internal float FontSizeScale = 1f;
        internal float LineSpacing = 1f;
        internal float CustomLineSpacingScale = 1f;
    }

    internal static class ExternalLocaleRuntime
    {
        private static readonly List<ExternalLocaleMetadata> Entries = new List<ExternalLocaleMetadata>();
        private static readonly List<LocalizationManager.Language> Applied = new List<LocalizationManager.Language>();
        private static XmlNode BaseLanguage;

        internal static void ValidateBaseLanguages(XmlNode languages)
        {
            if (Entries.Count == 0) return;
            if (languages == null) throw new InvalidOperationException("Recovered locale metadata is unavailable.");
            string defaultName = languages.Attributes?["Default"]?.Value;
            XmlNode fallback = null;
            foreach (XmlNode language in languages.ChildNodes)
            {
                if (language.NodeType != XmlNodeType.Element) continue;
                string name = language.Attributes?["Name"]?.Value;
                string locale = language.Attributes?["Locale"]?.Value;
                if (name == defaultName) fallback = language;
                foreach (ExternalLocaleMetadata metadata in Entries)
                    ValidateNoCollision(metadata, name, locale);
            }
            if (fallback == null) throw new InvalidOperationException("Recovered default locale metadata is unavailable.");
            BaseLanguage = fallback.CloneNode(true);
        }

        private static void ValidateNoCollision(ExternalLocaleMetadata metadata, string name, string locale)
        {
            if (string.Equals(metadata.Name, name, StringComparison.Ordinal) ||
                string.Equals(metadata.Locale, locale, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("External locale collides with recovered locale metadata: " + metadata.Name);
        }

        internal static void Add(ExternalLocaleMetadata metadata)
        {
            if (metadata == null) throw new ArgumentNullException("metadata");
            if (string.IsNullOrWhiteSpace(metadata.Name) || string.IsNullOrWhiteSpace(metadata.Locale))
                throw new ArgumentException("External locale requires both Name and Locale.", "metadata");
            bool anyFont = !string.IsNullOrEmpty(metadata.ContentFont) || !string.IsNullOrEmpty(metadata.TitleFont) ||
                !string.IsNullOrEmpty(metadata.ButtonFont);
            if (anyFont && (string.IsNullOrEmpty(metadata.ContentFont) || string.IsNullOrEmpty(metadata.TitleFont) ||
                string.IsNullOrEmpty(metadata.ButtonFont)))
                throw new ArgumentException("External locale font metadata must provide content, title, and button fonts together.", "metadata");
            for (int i = 0; i < Entries.Count; i++)
            {
                if (string.Equals(Entries[i].Name, metadata.Name, StringComparison.Ordinal) ||
                    string.Equals(Entries[i].Locale, metadata.Locale, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("External locale metadata collides with an already registered locale: " + metadata.Name);
            }
            Entries.Add(metadata);
        }

        internal static void Apply()
        {
            if (LocalizationManager.MCLNNPPCFFL == null || Entries.Count == 0) return;
            // Validate the complete overlay before adding anything. A collision must not
            // escape halfway through ParseModule and cause all game data to be parsed again.
            foreach (ExternalLocaleMetadata metadata in Entries)
                foreach (LocalizationManager.Language language in LocalizationManager.MCLNNPPCFFL)
                    if (!Applied.Contains(language)) ValidateNoCollision(metadata, language.name, language.EOMNCDDELLB);

            var pending = new List<LocalizationManager.Language>();
            for (int i = 0; i < Entries.Count; i++)
            {
                ExternalLocaleMetadata metadata = Entries[i];
                if (Applied.Exists(language => language.name == metadata.Name &&
                    LocalizationManager.MCLNNPPCFFL.Contains(language))) continue;

                XmlDocument document = new XmlDocument();
                XmlElement node = BaseLanguage == null ? document.CreateElement("Language") :
                    (XmlElement)document.ImportNode(BaseLanguage, true);
                document.AppendChild(node);
                Set(node, "Name", metadata.Name);
                Set(node, "Locale", metadata.Locale);
                Set(node, "Alias", metadata.Alias);
                Set(node, "FileIcon", metadata.FileIcon);
                Set(node, "FileIconSelected", metadata.FileIconSelected);
                Set(node, "LoaderImage", metadata.LoaderImage);
                Set(node, "PreloaderImage", metadata.PreloaderImage);
                Set(node, "IsAsian", metadata.IsAsian ? "1" : "0");
                if (!string.IsNullOrEmpty(metadata.ContentFont))
                {
                    if (node["Fonts"] != null) node.RemoveChild(node["Fonts"]);
                    XmlElement fonts = document.CreateElement("Fonts");
                    Set(fonts, "ContentFont", metadata.ContentFont);
                    Set(fonts, "TitleFont", metadata.TitleFont);
                    Set(fonts, "ButtonFont", metadata.ButtonFont);
                    Set(fonts, "FontSizeScale", metadata.FontSizeScale.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    Set(fonts, "LineSpacing", metadata.LineSpacing.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    Set(fonts, "CustomLineSpacingScale", metadata.CustomLineSpacingScale.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    node.AppendChild(fonts);
                }
                var language = new LocalizationManager.Language(node, LocalizationManager.MCLNNPPCFFL.Count + pending.Count);
                // Loose locales supply mod strings through TOML, not a replacement base
                // localization XML. Keep ordinary game text available through the base locale.
                LocalizationManager.Language fallback = LocalizationManager.NLFKNPBICED(LocalizationManager.POIPGLLCCKC);
                if (fallback != null) language.PMFEIPCHENB = fallback.PMFEIPCHENB;
                pending.Add(language);
            }
            LocalizationManager.MCLNNPPCFFL.AddRange(pending);
            Applied.AddRange(pending);
        }

        internal static void Clear()
        {
            if (LocalizationManager.MCLNNPPCFFL != null)
            {
                bool selected = Applied.Contains(LocalizationManager.ILAJKOBCHFH);
                foreach (LocalizationManager.Language language in Applied)
                    LocalizationManager.MCLNNPPCFFL.Remove(language);
                for (int i = 0; i < LocalizationManager.MCLNNPPCFFL.Count; i++)
                    LocalizationManager.MCLNNPPCFFL[i].index = i;
                if (selected) LocalizationManager.ILAJKOBCHFH = LocalizationManager.NLFKNPBICED(LocalizationManager.POIPGLLCCKC);
            }
            Applied.Clear();
            Entries.Clear();
            BaseLanguage = null;
        }

        private static void Set(XmlElement node, string name, string value)
        {
            node.SetAttribute(name, value ?? string.Empty);
        }
    }

    internal static class ExternalCombatContentRuntime
    {
        internal static int ApplyMoves(XmlDocument document)
        {
            if (document == null || document["Movesxml"] == null)
                throw new InvalidOperationException("External moves overlay requires a Movesxml root.");
            return AnimationData.AddExternalMoves(document);
        }

        internal static int ApplyTactics(XmlDocument overlay)
        {
            if (overlay == null)
                throw new ArgumentNullException("overlay");
            XmlNode overlayRoot = overlay["TacticsSettings"];
            if (overlayRoot == null)
                throw new InvalidOperationException("External tactics overlay requires a TacticsSettings root.");
            if (overlayRoot["ConditionalDecisions"] != null)
                throw new NotSupportedException("ConditionalDecisions have no recovered parser/evaluator and cannot be registered by mods.");
            XmlNode overlayTactics = overlayRoot["Tactics"];
            if (overlayTactics == null) return 0;

            XmlDocument combined = XmlUtils.OpenXMLDocument(SF2Paths.KKIDGPBOBNI(), "tacticSettings.xml");
            if (combined == null || combined["TacticsSettings"] == null || combined["TacticsSettings"]["Tactics"] == null)
                throw new InvalidOperationException("Recovered tacticSettings.xml is unavailable.");
            XmlNode combinedTactics = combined["TacticsSettings"]["Tactics"];
            List<string> names = new List<string>();
            HashSet<string> pendingNames = new HashSet<string>(StringComparer.Ordinal);
            foreach (XmlNode node in overlayTactics.ChildNodes)
            {
                if (node.Name != "Tactic") continue;
                string name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
                if (string.IsNullOrEmpty(name))
                    throw new InvalidOperationException("External tactic requires a Name.");
                if (!pendingNames.Add(name))
                    throw new InvalidOperationException("External tactics asset contains duplicate tactic '" + name + "'.");
                Tactic existing = AiData.GetTacticByName(name);
                if (existing != null && existing.get_Name() == name)
                    throw new InvalidOperationException("External tactic collides with existing tactic '" + name + "'.");
                names.Add(name);
                combinedTactics.AppendChild(combined.ImportNode(node, true));
            }

            TacticsCompiler.CompileTacticsSettings(combined);
            List<XmlNode> compiledNodes = new List<XmlNode>();
            for (int i = 0; i < names.Count; i++)
            {
                XmlNode node = FindTactic(combinedTactics, names[i]);
                if (node == null)
                    throw new InvalidOperationException("Compiled external tactic disappeared: " + names[i]);
                // Construct before mutating AiData so malformed tactic content cannot leave
                // a partially applied overlay.
                new Tactic(node);
                compiledNodes.Add(node);
            }
            for (int i = 0; i < compiledNodes.Count; i++) AiData.AddExternalTactic(compiledNodes[i]);
            return compiledNodes.Count;
        }

        internal static bool RemoveTactic(string runtimeName)
        {
            return AiData.RemoveExternalTactic(runtimeName);
        }

        private static XmlNode FindTactic(XmlNode tactics, string name)
        {
            foreach (XmlNode node in tactics.ChildNodes)
                if (node.Name == "Tactic" && node.Attributes["Name"].CIPOICEEIBK(string.Empty) == name)
                    return node;
            return null;
        }

    }
}
