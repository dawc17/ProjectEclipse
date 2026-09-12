using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Eclipse.Modding
{
    // Host-only recoverable profile snapshot/hash writes; no Lua filesystem access.
    // The checksum detects corruption; validate must enforce the host's authenticity policy.
    public static class ModProfileWriteJournal
    {
        private const int MaximumSnapshotBytes = 64 * 1024 * 1024;
        private const int MaximumHashBytes = 1024;
        private const int Magic = 0x45505731;

        public static void Write(string path, byte[] snapshot, byte[] hash,
            Action<byte[], byte[]> validate)
        {
            if (snapshot == null) throw new ArgumentNullException(nameof(snapshot));
            if (validate == null) throw new ArgumentNullException(nameof(validate));
            path = Path.GetFullPath(path);
            var ownedSnapshot = (byte[])snapshot.Clone();
            var ownedHash = hash == null ? null : (byte[])hash.Clone();
            var journal = Encode(ownedSnapshot, ownedHash);
            // Validate before taking ownership of any existing pending write.
            validate((byte[])ownedSnapshot.Clone(), ownedHash == null ? null : (byte[])ownedHash.Clone());
            using (Acquire(path))
            {
                RecoverLocked(path, validate);
                Replace(path + ".eclipse-write", journal);
                Install(path, ownedSnapshot, ownedHash);
                File.Delete(path + ".eclipse-write");
            }
        }

        public static bool Recover(string path, Action<byte[], byte[]> validate)
        {
            if (validate == null) throw new ArgumentNullException(nameof(validate));
            path = Path.GetFullPath(path);
            using (Acquire(path)) return RecoverLocked(path, validate);
        }

        public static void Discard(string path)
        {
            path = Path.GetFullPath(path);
            if (!Directory.Exists(Path.GetDirectoryName(path))) return;
            using (Acquire(path)) File.Delete(path + ".eclipse-write");
        }

        private static FileStream Acquire(string path)
        {
            // The sidecar persists to avoid delete/recreate races between processes.
            return new FileStream(path + ".eclipse-write.lock", FileMode.OpenOrCreate,
                FileAccess.ReadWrite, FileShare.None);
        }

        private static bool RecoverLocked(string path, Action<byte[], byte[]> validate)
        {
            var journalPath = path + ".eclipse-write";
            if (!File.Exists(journalPath)) return false;
            byte[] snapshot, hash;
            using (var stream = new FileStream(journalPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (stream.Length > MaximumSnapshotBytes + MaximumHashBytes + 44L)
                    throw new InvalidDataException("Profile write journal exceeds its size limit.");
                var bytes = new byte[checked((int)stream.Length)];
                int offset = 0;
                while (offset < bytes.Length)
                {
                    int count = stream.Read(bytes, offset, bytes.Length - offset);
                    if (count == 0) throw new EndOfStreamException("Incomplete profile write journal.");
                    offset += count;
                }
                Decode(bytes, out snapshot, out hash);
            }
            validate((byte[])snapshot.Clone(), hash == null ? null : (byte[])hash.Clone());
            Install(path, snapshot, hash);
            File.Delete(journalPath);
            return true;
        }

        private static void Install(string path, byte[] snapshot, byte[] hash)
        {
            Replace(path, snapshot);
            // A null hash preserves the existing sidecar, matching disabled-hash writes.
            if (hash != null) Replace(path + ".hash", hash);
        }

        private static void Replace(string path, byte[] bytes)
        {
            string temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush(true);
                }
                if (File.Exists(path)) File.Replace(temporary, path, null);
                else File.Move(temporary, path);
            }
            finally
            {
                if (File.Exists(temporary)) File.Delete(temporary);
            }
        }

        private static byte[] Encode(byte[] snapshot, byte[] hash)
        {
            if (snapshot.Length == 0 || snapshot.Length > MaximumSnapshotBytes ||
                (hash != null && (hash.Length == 0 || hash.Length > MaximumHashBytes)))
                throw new InvalidDataException("Invalid profile snapshot or hash length.");
            using (var buffer = new MemoryStream())
            using (var writer = new BinaryWriter(buffer, Encoding.UTF8, true))
            {
                writer.Write(Magic);
                writer.Write(snapshot.Length);
                writer.Write(hash == null ? -1 : hash.Length);
                writer.Write(snapshot);
                if (hash != null) writer.Write(hash);
                writer.Flush();
                byte[] payload = buffer.ToArray();
                using (var sha = SHA256.Create()) writer.Write(sha.ComputeHash(payload));
                writer.Flush();
                return buffer.ToArray();
            }
        }

        private static void Decode(byte[] bytes, out byte[] snapshot, out byte[] hash)
        {
            if (bytes.Length < 45) throw new InvalidDataException("Incomplete profile write journal.");
            using (var stream = new MemoryStream(bytes, false))
            using (var reader = new BinaryReader(stream))
            {
                if (reader.ReadInt32() != Magic) throw new InvalidDataException("Unknown profile write journal version.");
                int size = reader.ReadInt32(), hashSize = reader.ReadInt32();
                if (size < 1 || size > MaximumSnapshotBytes || hashSize < -1 || hashSize == 0 ||
                    hashSize > MaximumHashBytes || bytes.Length != 44L + size + Math.Max(0, hashSize))
                    throw new InvalidDataException("Invalid profile write journal lengths.");
                using (var sha = SHA256.Create())
                {
                    byte[] digest = sha.ComputeHash(bytes, 0, bytes.Length - 32);
                    for (int i = 0; i < digest.Length; i++)
                        if (digest[i] != bytes[bytes.Length - 32 + i])
                            throw new InvalidDataException("Corrupt profile write journal.");
                }
                snapshot = reader.ReadBytes(size);
                hash = hashSize < 0 ? null : reader.ReadBytes(hashSize);
            }
        }
    }
}
