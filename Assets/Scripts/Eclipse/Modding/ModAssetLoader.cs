using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Eclipse.Content;
using Eclipse.Content.TarAssets;
using UnityEngine;

namespace Eclipse.Modding
{
    public sealed class ModAssetLoader : IDisposable
    {
        private static readonly ModId Core = ModId.Parse("core");
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
            if (id.Namespace == Core) return PackagedArtCatalog.Load<T>(id.Path);
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
            if (id.Namespace == Core) return PackagedArtCatalog.LoadWithSubAssets<T>(id.Path);
            T asset = LoadUnityAsset<T>(id);
            return asset == null ? null : new[] { asset };
        }

        public Sprite LoadSprite(AssetId id)
        {
            AssetMetadata metadata;
            if (!_resolver.TryDescribe(id, out metadata)) return null;
            if (id.Namespace == Core) return PackagedArtCatalog.Load<Sprite>(id.Path);

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
            if (id.Namespace == Core) return PackagedArtCatalog.Load<Texture2D>(id.Path);
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
            if (id.Namespace == Core) return PackagedArtCatalog.LoadModelText(id.Path);
            AssetBytes bytes;
            if (!_resolver.TryRead(id, out bytes)) return null;
            if (bytes.Metadata.Kind != AssetKind.Model)
                throw new InvalidDataException("Asset is not model XML: " + id);
            return DecodeText(bytes, id);
        }

        public string LoadText(AssetId id)
        {
            if (id.Namespace == Core) return null;
            AssetBytes bytes;
            if (!_resolver.TryRead(id, out bytes)) return null;
            if (bytes.Metadata.Kind != AssetKind.Text && bytes.Metadata.Kind != AssetKind.Model)
                throw new InvalidDataException("Asset is not text: " + id);
            return DecodeText(bytes, id);
        }

        public AudioClip LoadAudio(AssetId id)
        {
            AssetMetadata metadata;
            if (!_resolver.TryDescribe(id, out metadata)) return null;
            if (id.Namespace == Core) return PackagedArtCatalog.Load<AudioClip>(id.Path);
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
}
