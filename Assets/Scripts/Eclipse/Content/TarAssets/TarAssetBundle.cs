using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Eclipse.Content.TarAssets
{
    public sealed class TarAssetBundle : IDisposable
    {
        private readonly PackagedArtCatalog.BundleRecord _record;
        private readonly TarArchive _archive;
        private readonly Dictionary<string, List<TarAssetMeta>> _assets =
            new Dictionary<string, List<TarAssetMeta>>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Texture2D> _textures =
            new Dictionary<string, Texture2D>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Sprite> _sprites =
            new Dictionary<string, Sprite>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, AudioClip> _audio =
            new Dictionary<string, AudioClip>(StringComparer.OrdinalIgnoreCase);

        public TarAssetBundle(PackagedArtCatalog.BundleRecord record)
        {
            _record = record ?? throw new ArgumentNullException("record");
            _archive = TarArchive.Open(Lz4BundleCache.OpenTar(record));
            IndexMetadata();
        }

        // Primarily used by the migration/validation tooling before a compressed bundle is committed
        // into StreamingAssets. It also makes the storage backend independently testable.
        public TarAssetBundle(PackagedArtCatalog.BundleRecord record, string uncompressedTarPath)
        {
            _record = record ?? throw new ArgumentNullException("record");
            _archive = TarArchive.Open(uncompressedTarPath);
            IndexMetadata();
        }

        private void IndexMetadata()
        {
            foreach (TarArchive.Entry entry in _archive.Entries.OrderBy(x => x.Name, StringComparer.Ordinal))
            {
                if (!entry.Name.EndsWith(".meta", StringComparison.OrdinalIgnoreCase))
                    continue;
                TarAssetMeta meta = TarAssetMeta.Parse(entry.Name, _archive.ReadText(entry.Name));
                if (!string.Equals(meta.Namespace, string.IsNullOrEmpty(_record.namespaceId) ? "core" : _record.namespaceId,
                        StringComparison.OrdinalIgnoreCase))
                    throw new InvalidDataException("Asset namespace differs from bundle namespace: " + entry.Name);
                ValidateReferences(meta);
                List<TarAssetMeta> list;
                if (!_assets.TryGetValue(meta.Address, out list))
                {
                    list = new List<TarAssetMeta>();
                    _assets.Add(meta.Address, list);
                }
                list.Add(meta);
            }
        }

        public void Dispose()
        {
            foreach (Sprite sprite in _sprites.Values.Where(x => x != null)) Destroy(sprite);
            foreach (AudioClip clip in _audio.Values.Where(x => x != null)) Destroy(clip);
            foreach (Texture2D texture in _textures.Values.Where(x => x != null)) Destroy(texture);
            _sprites.Clear();
            _audio.Clear();
            _textures.Clear();
        }

        public T LoadAsset<T>(string address) where T : UnityEngine.Object
        {
            T[] values = LoadAssetWithSubAssets<T>(address);
            return values == null || values.Length == 0 ? null : values[0];
        }

        public T[] LoadAssetWithSubAssets<T>(string address) where T : UnityEngine.Object
        {
            List<TarAssetMeta> metas;
            if (!_assets.TryGetValue(TarAssetMeta.NormalizeAddress(address), out metas))
                return null;

            if (typeof(T) == typeof(Sprite))
                return Cast<T>(metas.Where(x => x.Type == "sprite").Select(LoadSprite).ToArray());
            if (typeof(T) == typeof(Texture2D))
            {
                TarAssetMeta sprite = metas.FirstOrDefault(x => x.Type == "sprite");
                return sprite == null ? null : Cast<T>(new UnityEngine.Object[] { LoadTexture(sprite) });
            }
            if (typeof(T) == typeof(AudioClip))
                return Cast<T>(metas.Where(x => x.Type == "audio").Select(LoadAudio).ToArray());
            return null;
        }

        public string LoadText(string address, string type)
        {
            List<TarAssetMeta> metas;
            if (!_assets.TryGetValue(TarAssetMeta.NormalizeAddress(address), out metas))
                return null;
            TarAssetMeta meta = metas.FirstOrDefault(x => string.Equals(x.Type, type, StringComparison.OrdinalIgnoreCase));
            return meta == null ? null : _archive.ReadText(meta.File);
        }

        private Sprite LoadSprite(TarAssetMeta meta)
        {
            Sprite cached;
            if (_sprites.TryGetValue(meta.MetaPath, out cached) && cached != null)
                return cached;

            Texture2D texture = LoadTexture(meta);
            Sprite sprite = Sprite.Create(texture, meta.Rect, meta.Pivot, meta.PixelsPerUnit, 0,
                SpriteMeshType.FullRect, meta.Border);
            if (sprite == null)
                throw new InvalidDataException("Unity cannot create TAR sprite: " + meta.MetaPath);
            sprite.name = meta.Name;
            if (NeedsGeometryOverride(meta))
            {
                sprite.OverrideGeometry(ToRectSpace(meta), meta.Triangles);
                ValidateOverriddenGeometry(sprite, meta);
            }
            _sprites.Add(meta.MetaPath, sprite);
            return sprite;
        }

        private static void ValidateOverriddenGeometry(Sprite sprite, TarAssetMeta meta)
        {
            Vector2[] vertices = sprite.vertices;
            ushort[] triangles = sprite.triangles;
            Vector2[] uv = sprite.uv;
            if (vertices.Length != meta.Vertices.Length || triangles.Length != meta.Triangles.Length ||
                uv.Length != meta.Uv.Length)
                throw new InvalidDataException("Unity changed TAR sprite geometry counts: " + meta.MetaPath);

            int[] actualToExpected = MatchVertices(vertices, uv, meta.Vertices, meta.Uv, meta.MetaPath);
            ushort[] remappedTriangles = RemapTriangles(triangles, actualToExpected, meta.MetaPath);
            ValidateTriangleShape(meta.Vertices, remappedTriangles, meta.Triangles, meta.MetaPath);
        }

        private static int[] MatchVertices(Vector2[] actualVertices, Vector2[] actualUv,
            Vector2[] expectedVertices, Vector2[] expectedUv, string path)
        {
            const float vertexTolerance = 0.00002f;
            const float uvTolerance = 0.00001f;
            float vertexToleranceSquared = vertexTolerance * vertexTolerance;
            float uvToleranceSquared = uvTolerance * uvTolerance;
            bool[] used = new bool[expectedVertices.Length];
            int[] map = new int[actualVertices.Length];

            for (int actual = 0; actual < actualVertices.Length; actual++)
            {
                int best = -1;
                float bestScore = float.MaxValue;
                for (int expected = 0; expected < expectedVertices.Length; expected++)
                {
                    if (used[expected]) continue;
                    float vertexError = (actualVertices[actual] - expectedVertices[expected]).sqrMagnitude;
                    if (vertexError > vertexToleranceSquared) continue;
                    float uvError = (actualUv[actual] - expectedUv[expected]).sqrMagnitude;
                    if (uvError > uvToleranceSquared) continue;
                    float score = vertexError + uvError;
                    if (score < bestScore)
                    {
                        best = expected;
                        bestScore = score;
                    }
                }
                if (best < 0)
                    throw new InvalidDataException("Unity changed TAR sprite vertex/UV data: " + path +
                        "; unmatched actual vertex " + actual + "=" + actualVertices[actual] +
                        "; uv=" + actualUv[actual]);
                used[best] = true;
                map[actual] = best;
            }
            return map;
        }

        private static ushort[] RemapTriangles(ushort[] triangles, int[] actualToExpected, string path)
        {
            ushort[] result = new ushort[triangles.Length];
            for (int i = 0; i < triangles.Length; i++)
            {
                int actual = triangles[i];
                if (actual < 0 || actual >= actualToExpected.Length)
                    throw new InvalidDataException("Unity returned an invalid TAR sprite triangle index: " + path);
                int expected = actualToExpected[actual];
                if (expected < 0 || expected > ushort.MaxValue)
                    throw new InvalidDataException("TAR sprite vertex remap overflow: " + path);
                result[i] = (ushort)expected;
            }
            return result;
        }

        private static void ValidateTriangleShape(Vector2[] vertices, ushort[] actual, ushort[] expected, string path)
        {
            if (actual.Length % 3 != 0 || expected.Length % 3 != 0)
                throw new InvalidDataException("Invalid TAR sprite triangle list: " + path);

            Dictionary<string, int> expectedEdges = BuildEdgeCounts(vertices.Length, expected, path);
            Dictionary<string, int> actualEdges = BuildEdgeCounts(vertices.Length, actual, path);
            HashSet<string> expectedBoundary = BoundaryEdges(expectedEdges);
            HashSet<string> actualBoundary = BoundaryEdges(actualEdges);
            if (!expectedBoundary.SetEquals(actualBoundary))
                throw new InvalidDataException("Unity changed TAR sprite mesh boundary: " + path);

            float expectedArea = TriangleArea(vertices, expected);
            float actualArea = TriangleArea(vertices, actual);
            float tolerance = Mathf.Max(0.00001f, expectedArea * 0.00001f);
            if (Mathf.Abs(expectedArea - actualArea) > tolerance)
                throw new InvalidDataException("Unity changed TAR sprite mesh area: " + path +
                    "; expected=" + expectedArea + "; actual=" + actualArea);
        }

        private static Dictionary<string, int> BuildEdgeCounts(int vertexCount, ushort[] triangles, string path)
        {
            var result = new Dictionary<string, int>(StringComparer.Ordinal);
            for (int i = 0; i < triangles.Length; i += 3)
            {
                ushort a = triangles[i];
                ushort b = triangles[i + 1];
                ushort c = triangles[i + 2];
                if (a >= vertexCount || b >= vertexCount || c >= vertexCount)
                    throw new InvalidDataException("TAR sprite triangle references an invalid vertex: " + path);
                AddEdge(result, a, b);
                AddEdge(result, b, c);
                AddEdge(result, c, a);
            }
            return result;
        }

        private static void AddEdge(Dictionary<string, int> edges, ushort a, ushort b)
        {
            if (a > b)
            {
                ushort swap = a;
                a = b;
                b = swap;
            }
            string key = a + "," + b;
            int count;
            edges.TryGetValue(key, out count);
            edges[key] = count + 1;
        }

        private static HashSet<string> BoundaryEdges(Dictionary<string, int> edges)
        {
            var result = new HashSet<string>(StringComparer.Ordinal);
            foreach (KeyValuePair<string, int> edge in edges)
                if ((edge.Value & 1) != 0)
                    result.Add(edge.Key);
            return result;
        }

        private static float TriangleArea(Vector2[] vertices, ushort[] triangles)
        {
            float result = 0f;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector2 a = vertices[triangles[i]];
                Vector2 b = vertices[triangles[i + 1]];
                Vector2 c = vertices[triangles[i + 2]];
                result += Mathf.Abs((b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x)) * 0.5f;
            }
            return result;
        }

        private static bool NeedsGeometryOverride(TarAssetMeta meta)
        {
            if (meta.Vertices == null || meta.Vertices.Length == 0) return false;
            if (meta.Vertices.Length != 4 || meta.Triangles == null || meta.Triangles.Length != 6) return true;
            ushort[] fullRectTriangles = { 0, 1, 2, 2, 1, 3 };
            for (int i = 0; i < fullRectTriangles.Length; i++)
                if (meta.Triangles[i] != fullRectTriangles[i]) return true;

            float left = -meta.Pivot.x * meta.Rect.width / meta.PixelsPerUnit;
            float right = (1f - meta.Pivot.x) * meta.Rect.width / meta.PixelsPerUnit;
            float bottom = -meta.Pivot.y * meta.Rect.height / meta.PixelsPerUnit;
            float top = (1f - meta.Pivot.y) * meta.Rect.height / meta.PixelsPerUnit;
            Vector2[] expected =
            {
                new Vector2(left, top), new Vector2(right, top),
                new Vector2(left, bottom), new Vector2(right, bottom)
            };
            for (int i = 0; i < expected.Length; i++)
                if ((meta.Vertices[i] - expected[i]).sqrMagnitude > 0.000001f) return true;
            return false;
        }

        private static Vector2[] ToRectSpace(TarAssetMeta meta)
        {
            Vector2[] result = new Vector2[meta.Vertices.Length];
            float pivotX = meta.Pivot.x * meta.Rect.width;
            float pivotY = meta.Pivot.y * meta.Rect.height;
            for (int i = 0; i < result.Length; i++)
            {
                float x = meta.Vertices[i].x * meta.PixelsPerUnit + pivotX;
                float y = meta.Vertices[i].y * meta.PixelsPerUnit + pivotY;
                x = ClampBoundaryNoise(x, meta.Rect.width, meta.MetaPath, "x");
                y = ClampBoundaryNoise(y, meta.Rect.height, meta.MetaPath, "y");
                result[i] = new Vector2(x, y);
            }
            return result;
        }

        private static float ClampBoundaryNoise(float value, float maximum, string path, string axis)
        {
            const float tolerance = 0.001f;
            if (value < 0f)
            {
                if (value >= -tolerance) return 0f;
                throw new InvalidDataException("Sprite geometry is outside its rect on " + axis + ": " + path);
            }
            if (value > maximum)
            {
                if (value <= maximum + tolerance) return maximum;
                throw new InvalidDataException("Sprite geometry is outside its rect on " + axis + ": " + path);
            }
            return value;
        }

        private Texture2D LoadTexture(TarAssetMeta meta)
        {
            Texture2D cached;
            if (_textures.TryGetValue(meta.Texture, out cached) && cached != null)
                return cached;

            byte[] bytes = _archive.ReadBytes(meta.Texture);
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, meta.Mipmaps);
            texture.name = Path.GetFileNameWithoutExtension(meta.Texture);
            if (!ImageConversion.LoadImage(texture, bytes, false))
            {
                UnityEngine.Object.Destroy(texture);
                throw new InvalidDataException("Unity cannot decode TAR texture: " + meta.Texture);
            }
            texture.filterMode = meta.Filter;
            texture.anisoLevel = meta.Aniso;
            texture.wrapModeU = meta.WrapU;
            texture.wrapModeV = meta.WrapV;
            _textures.Add(meta.Texture, texture);
            return texture;
        }

        private AudioClip LoadAudio(TarAssetMeta meta)
        {
            AudioClip cached;
            if (_audio.TryGetValue(meta.MetaPath, out cached) && cached != null)
                return cached;
            AudioClip clip = WaveDecoder.Decode(_archive.ReadBytes(meta.File), meta.Name);
            _audio.Add(meta.MetaPath, clip);
            return clip;
        }

        private void ValidateReferences(TarAssetMeta meta)
        {
            if (meta.Type == "sprite" && !_archive.Contains(meta.Texture))
                throw new InvalidDataException("Sprite texture is outside/missing from bundle '" + _record.name + "': " + meta.Texture);
            if (meta.Type == "audio" && !_archive.Contains(meta.File))
                throw new InvalidDataException("Audio file is outside/missing from bundle '" + _record.name + "': " + meta.File);
            if (meta.Type == "model" && !_archive.Contains(meta.File))
                throw new InvalidDataException("Model file is outside/missing from bundle '" + _record.name + "': " + meta.File);
            if (meta.Type == "atlas" && !_archive.Contains(meta.File))
                throw new InvalidDataException("Atlas data is outside/missing from bundle '" + _record.name + "': " + meta.File);
        }

        private static T[] Cast<T>(UnityEngine.Object[] values) where T : UnityEngine.Object
        {
            if (values == null || values.Length == 0) return null;
            T[] result = new T[values.Length];
            for (int i = 0; i < values.Length; i++) result[i] = values[i] as T;
            return result;
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
