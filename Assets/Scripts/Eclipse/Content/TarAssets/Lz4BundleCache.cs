using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Networking;

namespace Eclipse.Content.TarAssets
{
    internal static class Lz4BundleCache
    {
        private const string StreamingRoot = "SF2Content/ArtBundles";
        private const string CacheDirectoryName = "SF2DE/TarAssetCache";

        public static string OpenTar(PackagedArtCatalog.BundleRecord bundle)
        {
            if (bundle == null || string.IsNullOrEmpty(bundle.file))
                throw new InvalidDataException("TAR asset bundle record has no file.");

            string cacheRoot = Path.Combine(Application.persistentDataPath, CacheDirectoryName);
            Directory.CreateDirectory(cacheRoot);
            string key = !string.IsNullOrEmpty(bundle.sha256) ? bundle.sha256 : SafeFileName(bundle.file);
            string tarPath = Path.Combine(cacheRoot, key + ".tar");
            if (ValidCachedTar(tarPath, bundle.unpackedSize))
                return tarPath;

            string compressedPath = MaterializeCompressed(bundle, cacheRoot);
            string temporaryTar = tarPath + ".tmp";
            TryDelete(temporaryTar);
            try
            {
                using (FileStream source = File.Open(compressedPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (FileStream target = File.Open(temporaryTar, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                    Lz4FrameDecoder.Decode(source, target, bundle.unpackedSize);

                if (bundle.unpackedSize > 0 && new FileInfo(temporaryTar).Length != bundle.unpackedSize)
                    throw new InvalidDataException("Decompressed TAR size mismatch: " + bundle.file);

                // Parsing here proves the stream produced a structurally valid archive before caching it.
                TarArchive.Open(temporaryTar);
                ReplaceFile(temporaryTar, tarPath);
                return tarPath;
            }
            catch
            {
                TryDelete(temporaryTar);
                throw;
            }
        }

        private static bool ValidCachedTar(string path, long expectedSize)
        {
            return File.Exists(path) && (expectedSize <= 0 || new FileInfo(path).Length == expectedSize);
        }

        private static string MaterializeCompressed(PackagedArtCatalog.BundleRecord bundle, string cacheRoot)
        {
            string source = CombineStreamingPath(StreamingRoot, bundle.file);
            if (IsNormalFilePath(source) && File.Exists(source))
            {
                ValidateCompressed(source, bundle);
                return source;
            }

            string extension = Path.GetExtension(bundle.file);
            string local = Path.Combine(cacheRoot,
                (!string.IsNullOrEmpty(bundle.sha256) ? bundle.sha256 : SafeFileName(bundle.file)) + extension + ".compressed");
            if (File.Exists(local))
            {
                try
                {
                    ValidateCompressed(local, bundle);
                    return local;
                }
                catch { TryDelete(local); }
            }

            string temporary = local + ".tmp";
            TryDelete(temporary);
#if UNITY_ANDROID && !UNITY_EDITOR
            MaterializeAndroidStreamingAsset(bundle.file, temporary);
#else
            using (UnityWebRequest request = UnityWebRequest.Get(source))
            {
                request.downloadHandler = new DownloadHandlerFile(temporary);
                UnityWebRequestAsyncOperation operation = request.SendWebRequest();
                while (!operation.isDone)
                    System.Threading.Thread.Sleep(1);
#if UNITY_2020_2_OR_NEWER
                bool failed = request.result != UnityWebRequest.Result.Success;
#else
                bool failed = request.isHttpError || request.isNetworkError;
#endif
                if (failed)
                {
                    TryDelete(temporary);
                    throw new IOException("Cannot materialize TAR asset bundle '" + bundle.file + "': " + request.error);
                }
            }
#endif
            ValidateCompressed(temporary, bundle);
            ReplaceFile(temporary, local);
            return local;
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        private static void MaterializeAndroidStreamingAsset(string file, string destination)
        {
            string apk = Application.dataPath;
            if (string.IsNullOrEmpty(apk) || !File.Exists(apk))
                throw new FileNotFoundException("Cannot open Android APK for TAR asset materialization.", apk);

            string entryName = "assets/" + StreamingRoot + "/" + file.Replace('\\', '/').TrimStart('/');
            using (FileStream input = File.Open(apk, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var zip = new ZipArchive(input, ZipArchiveMode.Read, false))
            {
                ZipArchiveEntry entry = zip.GetEntry(entryName);
                if (entry == null)
                    throw new FileNotFoundException("TAR asset is missing from Android APK: " + entryName, apk);
                using (Stream source = entry.Open())
                using (FileStream target = File.Open(destination, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                    source.CopyTo(target);
            }
        }
#endif

        private static void ValidateCompressed(string path, PackagedArtCatalog.BundleRecord bundle)
        {
            FileInfo file = new FileInfo(path);
            if (bundle.size > 0 && file.Length != bundle.size)
                throw new InvalidDataException("Compressed TAR asset size mismatch: " + bundle.file);
            if (string.IsNullOrEmpty(bundle.sha256))
                return;

            using (FileStream input = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (SHA256 hash = SHA256.Create())
            {
                string actual = BitConverter.ToString(hash.ComputeHash(input)).Replace("-", string.Empty).ToLowerInvariant();
                if (!string.Equals(actual, bundle.sha256, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidDataException("Compressed TAR asset checksum mismatch: " + bundle.file);
            }
        }

        private static string CombineStreamingPath(string root, string file)
        {
            string basePath = Application.streamingAssetsPath.Replace('\\', '/').TrimEnd('/');
            return basePath + "/" + root + "/" + file.Replace('\\', '/').TrimStart('/');
        }

        private static bool IsNormalFilePath(string path)
        {
            return path.IndexOf("://", StringComparison.Ordinal) < 0 &&
                !path.StartsWith("jar:", StringComparison.OrdinalIgnoreCase);
        }

        private static string SafeFileName(string value)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
                value = value.Replace(c, '_');
            return value;
        }

        private static void ReplaceFile(string source, string destination)
        {
            TryDelete(destination);
            File.Move(source, destination);
        }

        private static void TryDelete(string path)
        {
            try { if (File.Exists(path)) File.Delete(path); }
            catch { }
        }
    }
}
