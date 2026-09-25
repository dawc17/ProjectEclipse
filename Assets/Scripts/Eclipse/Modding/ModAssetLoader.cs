using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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

        private readonly Dictionary<string, Sprite> _replacementMembers = new Dictionary<string, Sprite>();
        public Sprite LoadReplacementMember(AssetId id, string memberName)
        {
            string key = id + "|" + memberName;
            if (_replacementMembers.TryGetValue(key, out var cached) && cached != null) return cached;
            var source = LoadSprite(id);
            if (source == null) throw new InvalidDataException("Replacement sprite is missing: " + id);
            var member = UnityEngine.Object.Instantiate(source);
            member.name = memberName;
            _replacementMembers.Add(key, member);
            return member;
        }

        public T LoadUnityAsset<T>(AssetId id) where T : UnityEngine.Object
        {
            id = _resolver.Resolve(id);
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
            id = _resolver.Resolve(id);
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
            id = _resolver.Resolve(id);
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
            id = _resolver.Resolve(id);
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
            id = _resolver.Resolve(id);
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
                throw new InvalidDataException("Asset is not model geometry: " + id);
            return DecodeModelText(bytes, id);
        }

        public string LoadText(AssetId id)
        {
            AssetBytes bytes;
            if (!_resolver.TryRead(id, out bytes)) return null;
            if (bytes.Metadata.Kind != AssetKind.Text && bytes.Metadata.Kind != AssetKind.Model)
                throw new InvalidDataException("Asset is not text: " + id);
            return bytes.Metadata.Kind == AssetKind.Model ? DecodeModelText(bytes, id) : DecodeText(bytes, id);
        }

        public byte[] LoadBinary(AssetId id)
        {
            id = _resolver.Resolve(id);
            AssetBytes bytes;
            if (!_resolver.TryRead(id, out bytes)) return null;
            if (bytes.Metadata.Kind != AssetKind.Binary)
                throw new InvalidDataException("Asset is not binary data: " + id);
            return bytes.Data;
        }

        public AudioClip LoadAudio(AssetId id)
        {
            id = _resolver.Resolve(id);
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
            foreach (var member in _replacementMembers.Values) if (member != null) Destroy(member);
            _replacementMembers.Clear();
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

        private static string DecodeModelText(AssetBytes bytes, AssetId id)
        {
            if (bytes.Metadata.Format != ".modelz") return DecodeText(bytes, id);
            try
            {
                using (var compressed = new MemoryStream(bytes.Data, false))
                using (var gzip = new GZipStream(compressed, CompressionMode.Decompress))
                using (var decoded = new MemoryStream())
                {
                    byte[] buffer = new byte[8192];
                    int count;
                    while ((count = gzip.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        if (decoded.Length + count > MaxTextBytes)
                            throw new InvalidDataException("Model asset exceeds decoded size limit: " + id);
                        decoded.Write(buffer, 0, count);
                    }
                    return StrictUtf8.GetString(decoded.ToArray());
                }
            }
            catch (DecoderFallbackException error)
            {
                throw new InvalidDataException("Compressed model is not valid UTF-8: " + id, error);
            }
            catch (InvalidDataException) { throw; }
            catch (IOException error)
            {
                throw new InvalidDataException("Compressed model could not be decoded: " + id, error);
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
        internal sealed class MovePerkLockRollback
        {
            internal sealed class Snapshot
            {
                internal readonly InfoAnimation Move;
                internal readonly ConditionAnimation[] Locks;
                internal Snapshot(InfoAnimation move, ConditionAnimation[] locks) { Move = move; Locks = locks; }
            }

            internal readonly List<Snapshot> Snapshots = new List<Snapshot>();
        }

        internal sealed class MoveItemLockRollback : IDisposable
        {
            internal sealed class Replacement
            {
                internal List<ConditionAnimation> Locks;
                internal int Index;
                internal ConditionAnimation Original;
                internal ConditionAnimation Applied;
            }
            internal readonly List<Replacement> Replacements = new List<Replacement>();
            public void Dispose()
            {
                for (int i=Replacements.Count-1; i>=0; i--)
                {
                    var entry=Replacements[i];
                    int index=entry.Locks.IndexOf(entry.Applied);
                    if (index>=0) entry.Locks[index]=entry.Original;
                }
                Replacements.Clear();
            }
        }

        internal static MoveItemLockRollback ApplyItemLockExtensions(IReadOnlyList<InfoAnimation> moves,
            IReadOnlyList<MoveItemLockExtension> extensions)
        {
            if (extensions == null || extensions.Count == 0) return null;
            var byName=new Dictionary<string,InfoAnimation>(StringComparer.Ordinal);
            foreach (var move in moves)
                if (move != null && !string.IsNullOrEmpty(move.Name))
                {
                    if (byName.ContainsKey(move.Name)) throw new InvalidOperationException("Ambiguous live move name: " + move.Name);
                    byName.Add(move.Name,move);
                }
            var rollback=new MoveItemLockRollback();
            var pending=new Dictionary<List<ConditionAnimation>,List<ConditionAnimation>>();
            var document=new XmlDocument { XmlResolver=null };
            foreach (var extension in extensions)
            {
                if (!byName.TryGetValue(extension.MoveName,out var move) || move.MoveData == null)
                    throw new InvalidOperationException("Item lock extension references unavailable move '"+extension.MoveName+"'.");
                var live=move.MoveData.Locks;
                if (!pending.TryGetValue(live,out var locks)) pending.Add(live,locks=new List<ConditionAnimation>(live));
                int match=-1;
                for(int i=0;i<live.Count;i++)
                {
                    // Select against original clauses, never another pending addition.
                    // This keeps independent mod registration order from changing targets.
                    var candidate=live[i];
                    bool found=MatchesItemLock(candidate,extension.ItemType,extension.SourceSubtype);
                    if (candidate is ConditionList group && !group.IsNot && group.get_Type()==ConditionList.OperatorType.OR)
                        foreach (var child in group.GetConditions()) found |= MatchesItemLock(child,extension.ItemType,extension.SourceSubtype);
                    if (!found) continue;
                    if (match>=0) throw new InvalidOperationException("Ambiguous item lock clause for '"+extension.MoveName+"'.");
                    match=i;
                }
                if (match<0) throw new InvalidOperationException("No matching positive item lock clause for '"+extension.MoveName+"'.");
                var original=locks[match];
                var children=original is ConditionList existing ? new List<ConditionAnimation>(existing.GetConditions()) : new List<ConditionAnimation>{original};
                foreach (var child in children)
                    if (MatchesItemLock(child,extension.ItemType,extension.Subtype))
                        throw new InvalidOperationException("Item lock subtype already exists for '"+extension.MoveName+"'.");
                var item=document.CreateElement("Item"); item.SetAttribute("Type",extension.ItemType); item.SetAttribute("SubType",extension.Subtype);
                var added=new ConditionItemInfo(item); added.Parse(item); children.Add(added);
                var op=document.CreateElement("Operator"); op.SetAttribute("Type","Or");
                var replacement=new ConditionList(op,children); replacement.Parse(op);
                locks[match]=replacement;
            }
            // Validate the entire batch before changing any live condition list.
            foreach (var pair in pending)
                for(int i=0;i<pair.Key.Count;i++)
                    if (!ReferenceEquals(pair.Key[i],pair.Value[i]))
                        rollback.Replacements.Add(new MoveItemLockRollback.Replacement {
                            Locks=pair.Key,Index=i,Original=pair.Key[i],Applied=pair.Value[i] });
            foreach(var entry in rollback.Replacements) entry.Locks[entry.Index]=entry.Applied;
            return rollback;
        }

        private static bool MatchesItemLock(ConditionAnimation condition,string itemType,string subtype)
            => condition is ConditionItemInfo item && !item.IsNot && item.get_Type()==itemType &&
                item.GetSubType()==subtype && string.IsNullOrEmpty(item.get_Name());

        internal static int ApplyMoves(XmlDocument document)
        {
            if (document == null || document["Movesxml"] == null)
                throw new InvalidOperationException("External moves overlay requires a Movesxml root.");
            return AnimationData.AddExternalMoves(document);
        }

        internal static MovePerkLockRollback ApplyMovePerkLocks(IReadOnlyList<MovePerkLockRemoval> removals)
        {
            if (removals == null || removals.Count == 0) return null;
            XmlDocument source = XmlUtils.OpenXMLDocument(SF2Paths.MCFPDHOLNGB() + "/moves.xml", string.Empty);
            XmlNode sourceMoves = source?["Movesxml"]?["Moves"];
            if (sourceMoves == null) throw new InvalidOperationException("Recovered animations/moves.xml is unavailable.");

            var liveByName = new Dictionary<string, InfoAnimation>(StringComparer.Ordinal);
            foreach (InfoAnimation move in AnimationData.Animations)
                if (move != null && !string.IsNullOrEmpty(move.Name)) liveByName[move.Name] = move;

            var sourceByName = new Dictionary<string, XmlNode>(StringComparer.Ordinal);
            foreach (XmlNode move in sourceMoves.ChildNodes)
            {
                if (move.NodeType != XmlNodeType.Element || move.Name != "Move") continue;
                string name = move.Attributes?["Name"]?.Value;
                if (!string.IsNullOrEmpty(name)) sourceByName[name] = move;
            }

            var removalsByMove = new Dictionary<InfoAnimation, HashSet<int>>();
            var rollback = new MovePerkLockRollback();
            foreach (MovePerkLockRemoval removal in removals)
            {
                if (!sourceByName.TryGetValue(removal.MoveName, out XmlNode sourceMove))
                    throw new InvalidOperationException("Move perk-lock removal references missing base move '" + removal.MoveName + "'.");
                if (!liveByName.TryGetValue(removal.MoveName, out InfoAnimation liveMove) || liveMove.MoveData == null)
                    throw new InvalidOperationException("Move perk-lock removal references unavailable live move '" + removal.MoveName + "'.");

                XmlNode locksNode = sourceMove["Locks"];
                if (locksNode == null)
                    throw new InvalidOperationException("Move '" + removal.MoveName + "' has no direct Locks block.");
                string perkName = removal.RuntimePerkName;
                int parsedIndex = 0, targetIndex = -1;
                foreach (XmlNode lockNode in locksNode.ChildNodes)
                {
                    if (lockNode.NodeType != XmlNodeType.Element) continue;
                    ConditionAnimation parsed = ConditionsParser.Create(lockNode);
                    if (parsed == null) continue;
                    if (lockNode.Name == "Perk" && string.Equals(lockNode.Attributes?["Name"]?.Value, perkName, StringComparison.Ordinal))
                    {
                        if (targetIndex >= 0)
                            throw new InvalidOperationException("Move '" + removal.MoveName + "' contains duplicate direct perk lock '" + perkName + "'.");
                        targetIndex = parsedIndex;
                    }
                    parsedIndex++;
                }
                if (targetIndex < 0)
                    throw new InvalidOperationException("Move '" + removal.MoveName + "' has no direct perk lock '" + perkName + "'.");

                List<ConditionAnimation> liveLocks = liveMove.MoveData.Locks;
                if (targetIndex >= liveLocks.Count || !(liveLocks[targetIndex] is ConditionPerk livePerk) ||
                    !string.Equals(livePerk.get_Name(), perkName, StringComparison.Ordinal))
                    throw new InvalidOperationException("Live move lock layout does not match recovered XML for '" + removal.MoveName + "'.");
                if (!removalsByMove.TryGetValue(liveMove, out HashSet<int> indices))
                {
                    indices = new HashSet<int>();
                    removalsByMove.Add(liveMove, indices);
                    rollback.Snapshots.Add(new MovePerkLockRollback.Snapshot(liveMove, liveLocks.ToArray()));
                }
                if (!indices.Add(targetIndex))
                    throw new InvalidOperationException("Duplicate live move perk-lock removal for '" + removal.MoveName + "' and '" + perkName + "'.");
            }

            foreach (MovePerkLockRollback.Snapshot snapshot in rollback.Snapshots)
            {
                HashSet<int> indices = removalsByMove[snapshot.Move];
                List<ConditionAnimation> liveLocks = snapshot.Move.MoveData.Locks;
                liveLocks.Clear();
                for (int i = 0; i < snapshot.Locks.Length; i++) if (!indices.Contains(i)) liveLocks.Add(snapshot.Locks[i]);
            }
            return rollback;
        }

        internal static void RemoveMovePerkLocks(MovePerkLockRollback rollback)
        {
            if (rollback == null) return;
            for (int i = rollback.Snapshots.Count - 1; i >= 0; i--)
            {
                MovePerkLockRollback.Snapshot snapshot = rollback.Snapshots[i];
                if (snapshot.Move?.MoveData == null) continue;
                List<ConditionAnimation> locks = snapshot.Move.MoveData.Locks;
                locks.Clear();
                locks.AddRange(snapshot.Locks);
            }
            rollback.Snapshots.Clear();
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

namespace Eclipse.Modding
{
    // Validate the complete batch first. Animation loading can consume NodeInterval
    // between application and removal, so rollback handles both representations.
    internal static class MoveCombatPatchRuntime
    {
        private sealed class DisabledMoveCondition : ConditionAnimation
        {
            internal DisabledMoveCondition() : base(ConditionType.NONE) { }
            public override bool IsEqual(ModelConditions conditions) => false;
            public override bool IsEqual(Model model, InfoAnimation animation) => false;
        }

        internal sealed class Lifetime : IDisposable
        {
            internal readonly List<Action> Apply = new List<Action>();
            internal readonly List<Action> Undo = new List<Action>();
            private bool _disposed;
            public void Dispose()
            {
                if (_disposed) return;
                _disposed = true;
                for (int i = Undo.Count - 1; i >= 0; i--) Undo[i]();
            }
        }

        internal static Lifetime Apply(IReadOnlyList<InfoAnimation> moves, IReadOnlyList<MoveCombatPatch> patches,
            Func<ModMoveCondition, ConditionAnimation> parse)
        {
            var lifetime = new Lifetime();
            var names = new HashSet<string>(StringComparer.Ordinal);
            foreach (var patch in patches)
            {
                if (!names.Add(patch.MoveName)) throw new InvalidOperationException("Duplicate runtime move patch: " + patch.MoveName);
                InfoAnimation move = null;
                foreach (var candidate in moves)
                    if (candidate != null && candidate.Name == patch.MoveName)
                    {
                        if (move != null) throw new InvalidOperationException("Ambiguous native move: " + patch.MoveName);
                        move = candidate;
                    }
                if (move == null || move.MoveData == null) throw new InvalidOperationException("Missing native move: " + patch.MoveName);
                var target = move;
                if (patch.Disable)
                {
                    var condition = new DisabledMoveCondition();
                    var conditions = target.SelectionConditions;
                    lifetime.Apply.Add(() => conditions.Add(condition));
                    lifetime.Undo.Add(() => conditions.Remove(condition));
                }
                foreach (var condition in patch.Conditions)
                {
                    var parsed = parse(condition);
                    if (parsed == null) throw new InvalidOperationException("Unsupported parsed patch condition: " + patch.MoveName);
                    var conditions = target.SelectionConditions;
                    lifetime.Apply.Add(() => conditions.Add(parsed));
                    lifetime.Undo.Add(() => conditions.Remove(parsed));
                }
                if (patch.IntervalEnd != null && patch.IntervalStart != null &&
                    patch.IntervalEnd.Name != patch.IntervalStart.Name)
                {
                    PrepareBounds(target, null, patch.IntervalEnd, lifetime);
                    PrepareBounds(target, patch.IntervalStart, null, lifetime);
                }
                else if (patch.IntervalEnd != null || patch.IntervalStart != null)
                    PrepareBounds(target, patch.IntervalStart, patch.IntervalEnd, lifetime);
                if (patch.Hit != null) PrepareHit(target, patch.Hit, lifetime);
                if (patch.SoundFrame != null) PrepareSound(target, patch.SoundFrame, lifetime);
                if (patch.Input != null) PrepareInput(target, patch.Input, parse, lifetime);
                if (patch.Priority != null) PreparePriority(target, patch.Priority, lifetime);
            }
            try { foreach (var apply in lifetime.Apply) apply(); }
            catch { lifetime.Dispose(); throw; }
            return lifetime;
        }

        private static string Attribute(XmlNode node, string name) => node?.Attributes?[name]?.Value;
        private static int Integer(XmlNode node, string name, int fallback)
        {
            string raw = Attribute(node, name);
            if (raw == null) return fallback;
            if (!int.TryParse(raw, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out int value))
                throw new InvalidOperationException("Native move field is not an integer: " + name);
            return value;
        }
        private static void PrepareBounds(InfoAnimation move, ModMoveFramePatch startPatch,
            ModMoveFramePatch endPatch, Lifetime lifetime)
        {
            string name = (startPatch ?? endPatch).Name;
            IntervalAnimation interval = null;
            foreach (var candidate in move.MoveData.Intervals)
                if ((candidate.NodeInterval != null ? Attribute(candidate.NodeInterval,"Name") : candidate.Name) == name)
                {
                    if (interval != null) throw new InvalidOperationException("Ambiguous interval: " + move.Name + "/" + name);
                    interval = candidate;
                }
            if (interval == null) throw new InvalidOperationException("Missing interval: " + move.Name + "/" + name);
            var target = interval; var originalNode = target.NodeInterval;
            int start = originalNode == null ? target.Start : Integer(originalNode,"Start",0);
            int end = originalNode == null ? target.EndFrame : Integer(originalNode,"End",-1);
            if ((startPatch != null && start != startPatch.Expected) ||
                (endPatch != null && end != endPatch.Expected) ||
                (startPatch?.Value ?? start) > (endPatch?.Value ?? end))
                throw new InvalidOperationException("Interval expected bounds mismatch: " + move.Name);
            XmlNode replacement = originalNode?.CloneNode(true);
            if (replacement != null)
            {
                if (startPatch != null) ((XmlElement)replacement).SetAttribute("Start",startPatch.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
                if (endPatch != null) ((XmlElement)replacement).SetAttribute("End",endPatch.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }
            lifetime.Apply.Add(() =>
            {
                if (replacement != null) target.NodeInterval = replacement;
                else
                {
                    if (startPatch != null) target.Start = startPatch.Value;
                    if (endPatch != null) target.EndFrame = endPatch.Value;
                }
            });
            lifetime.Undo.Add(() =>
            {
                if (replacement != null && ReferenceEquals(target.NodeInterval,replacement)) target.NodeInterval = originalNode;
                else if (target.NodeInterval == null)
                {
                    if (startPatch != null && target.Start == startPatch.Value) target.Start = startPatch.Expected;
                    if (endPatch != null && target.EndFrame == endPatch.Value) target.EndFrame = endPatch.Expected;
                }
            });
        }
        private static void PrepareHit(InfoAnimation move, ModMoveHitPatch patch, Lifetime lifetime)
        {
            IntervalAttack attack = null;
            foreach (var candidate in move.MoveData.Intervals)
                if (candidate is IntervalAttack found)
                {
                    if (attack != null) throw new InvalidOperationException("Hit patch requires one attack interval: " + move.Name);
                    attack = found;
                }
            if (attack == null) throw new InvalidOperationException("Hit patch has no attack interval: " + move.Name);
            var target = attack; var originalNode = target.NodeInterval;
            XmlNode replacement = null;
            if (originalNode != null)
            {
                var hits = originalNode.SelectNodes("Hit");
                if (hits.Count != 1 || Attribute(hits[0],"Name") != patch.Expected ||
                    Attribute(hits[0],"Start") != null || Attribute(hits[0],"End") != null)
                    throw new InvalidOperationException("Hit patch requires one matching full-interval hit: " + move.Name);
                replacement = originalNode.CloneNode(true);
                ((XmlElement)replacement["Hit"]).SetAttribute("Name",patch.Value);
            }
            else if (target.HitReactions.Count != 1 || target.HitReactions[0].Name != patch.Expected ||
                target.HitReactions[0].Start != target.Start || target.HitReactions[0].EndFrame != target.EndFrame)
                throw new InvalidOperationException("Hit patch requires one matching full-interval reaction: " + move.Name);
            lifetime.Apply.Add(() => { if (replacement != null) target.NodeInterval = replacement; else target.HitReactions[0].Name = patch.Value; });
            lifetime.Undo.Add(() =>
            {
                if (replacement != null && ReferenceEquals(target.NodeInterval,replacement)) target.NodeInterval = originalNode;
                else if (target.NodeInterval == null && target.HitReactions.Count == 1 && target.HitReactions[0].Name == patch.Value)
                    target.HitReactions[0].Name = patch.Expected;
            });
        }
        private static void PrepareSound(InfoAnimation move, ModMoveFramePatch patch, Lifetime lifetime)
        {
            ActionSound sound = null;
            foreach (var candidate in move.ScheduledActions)
                if (candidate is ActionSound found && found.get_Name() == patch.Name)
                {
                    if (sound != null) throw new InvalidOperationException("Ambiguous sound action: " + move.Name + "/" + patch.Name);
                    sound = found;
                }
            if (sound == null || sound.ScheduledFrame != patch.Expected)
                throw new InvalidOperationException("Sound expected frame mismatch: " + move.Name + "/" + patch.Name);
            var target = sound;
            lifetime.Apply.Add(() => target.SetScheduledFrame(patch.Value));
            lifetime.Undo.Add(() => { if (target.ScheduledFrame == patch.Value) target.SetScheduledFrame(patch.Expected); });
        }
        private static void PrepareInput(InfoAnimation move, ModMoveInputPatch patch,
            Func<ModMoveCondition, ConditionAnimation> parse, Lifetime lifetime)
        {
            var expected = parse(new ModMoveCondition(ModMoveConditionKind.Keys,
                keys: new[] { patch.Expected })) as ConditionKeys;
            var replacement = parse(new ModMoveCondition(ModMoveConditionKind.Keys,
                keys: new[] { patch.Value })) as ConditionKeys;
            if (expected == null || replacement == null)
                throw new InvalidOperationException("Move input patch could not parse native keys: " + move.Name);
            var conditions = move.SelectionConditions;
            int index = -1;
            for (int i = 0; i < conditions.Count; i++)
                if (conditions[i] is ConditionKeys)
                {
                    if (index >= 0) throw new InvalidOperationException("Ambiguous native key condition: " + move.Name);
                    index = i;
                }
            var original = index >= 0 ? conditions[index] as ConditionKeys : null;
            if (original == null || !original.HasSameKeyRequirementAs(expected))
                throw new InvalidOperationException("Move input expected key mismatch: " + move.Name);
            lifetime.Apply.Add(() => conditions[index] = replacement);
            lifetime.Undo.Add(() =>
            {
                int currentIndex = conditions.IndexOf(replacement);
                if (currentIndex >= 0) conditions[currentIndex] = original;
            });
        }

        private static void PreparePriority(InfoAnimation move, ModMovePriorityPatch patch, Lifetime lifetime)
        {
            if (move.Priority != patch.Expected)
                throw new InvalidOperationException("Move priority expected value mismatch: " + move.Name);
            lifetime.Apply.Add(() => move.Priority = patch.Value);
            lifetime.Undo.Add(() => { if (move.Priority == patch.Value) move.Priority = patch.Expected; });
        }
    }
}
