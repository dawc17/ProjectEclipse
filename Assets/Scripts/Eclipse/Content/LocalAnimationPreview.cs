#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

namespace Eclipse.Content
{
    // Temporary editor preview seam, not the public mod/content registration API.
    // Local samples live outside Assets so they cannot enter player builds.
    public static class LocalAnimationPreview
    {
        public const string FilePrefix = "_eclipse_preview/";
        private static readonly Dictionary<string, byte[]> Animations = new Dictionary<string, byte[]>(StringComparer.Ordinal);

        public static string DirectoryPath
        {
            get { return Path.GetFullPath(Path.Combine(Application.dataPath, "../Library/EclipseAnimationPreview")); }
        }

        public static bool Apply(XmlDocument moves, string directory = null)
        {
            Animations.Clear();
            directory = directory ?? DirectoryPath;
            if (!File.Exists(Path.Combine(directory, "enabled"))) return false;
            try
            {
                var preview = new XmlDocument { XmlResolver = null };
                preview.Load(Path.Combine(directory, "Move.xml"));
                XmlElement replacement = preview.DocumentElement;
                if (replacement == null || replacement.Name != "Move")
                    throw new InvalidDataException("Preview must contain one Move element.");
                string name = replacement.GetAttribute("Name");
                XmlElement original = null;
                foreach (XmlElement move in moves.SelectNodes("/Movesxml/Moves/Move"))
                    if (move.GetAttribute("Name") == name) original = move;
                if (original == null) throw new InvalidDataException("Preview target move does not exist: " + name);

                string file = replacement.GetAttribute("FileName");
                if (!file.StartsWith(FilePrefix, StringComparison.Ordinal))
                    throw new InvalidDataException("Preview FileName must start with " + FilePrefix);
                string leaf = file.Substring(FilePrefix.Length);
                if (string.IsNullOrEmpty(leaf) || leaf.IndexOfAny(new[] { '/', '\\', ':' }) >= 0 ||
                    leaf != Path.GetFileName(leaf) || !leaf.EndsWith(".bytes", StringComparison.Ordinal))
                    throw new InvalidDataException("Preview animation must be a local .bytes filename.");
                byte[] bytes = File.ReadAllBytes(Path.Combine(directory, leaf));
                int frames = ValidateBinary(bytes);
                int first = int.Parse(replacement.GetAttribute("FirstFrame"));
                int last = int.Parse(replacement.GetAttribute("EndFrame"));
                int mid = int.Parse(replacement.GetAttribute("MidFrames"));
                if (first < 0 || last >= frames || last - first < 2 || mid < 0 || mid > 10)
                    throw new InvalidDataException("Preview frame settings are out of range.");

                original.ParentNode.ReplaceChild(moves.ImportNode(replacement, true), original);
                Animations.Add(file, bytes);
                Debug.Log("[AnimationPreview] " + name + " uses " + leaf + " (" + frames +
                    " frames). Local editor experiment; disable under SF2/Animation Preview.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[AnimationPreview] Keeping vanilla move: " + ex.Message);
                return false;
            }
        }

        public static bool TryGetBinary(string request, out byte[] bytes)
        {
            bytes = null;
            if (string.IsNullOrEmpty(request)) return false;
            string normalized = request.Replace('\\', '/');
            foreach (var pair in Animations)
                if (normalized.EndsWith("/animations/binary/" + pair.Key, StringComparison.Ordinal))
                {
                    bytes = pair.Value;
                    return true;
                }
            return false;
        }

        private static int ValidateBinary(byte[] data)
        {
            using (var reader = new BinaryReader(new MemoryStream(data, false)))
            {
                int frames = reader.ReadInt32();
                if (frames < 3 || frames > 10000) throw new InvalidDataException("Invalid frame count.");
                for (int i = 0; i < frames; i++)
                {
                    reader.ReadByte();
                    int nodes = reader.ReadInt32();
                    if (nodes != 67) throw new InvalidDataException("Preview requires the standard 67-node skeleton.");
                    for (int j = 0; j < nodes * 3; j++)
                    {
                        float v = reader.ReadSingle();
                        if (float.IsNaN(v) || float.IsInfinity(v)) throw new InvalidDataException("Nonfinite node.");
                    }
                }
                if (reader.BaseStream.Position != data.Length) throw new InvalidDataException("Trailing animation bytes.");
                return frames;
            }
        }
    }
}
#endif
