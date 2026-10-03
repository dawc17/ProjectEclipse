using System;
using System.IO;
using UnityEngine;

namespace Eclipse.Runtime
{
    /// <summary>Separate writable data for explicitly tagged multiplayer editor clients.</summary>
    public static class EditorPlayModeContext
    {
        public const string Host = "EclipseHost", Guest = "EclipseGuest";
        public const string RoomHost = "EclipseRoomHost", RoomGuest = "EclipseRoomGuest", Observer = "EclipseObserver";
        public static string Role
        {
            get
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                    foreach (var tag in Unity.Multiplayer.PlayMode.CurrentPlayer.Tags)
                        if (tag == Host || tag == Guest || tag == RoomHost || tag == RoomGuest || tag == Observer) return tag;
#endif
                return null;
            }
        }
        public static bool IsRoomScenario => Role == RoomHost || Role == RoomGuest || Role == Observer;
        public static string PersistentDataPath => Role == null ? Application.persistentDataPath :
            TaggedDataPath(Application.persistentDataPath, Role);

        // Recovered XmlUtils identifies disk files with a string prefix check
        // against SF2Paths' root. Unity and SF2Paths use forward slashes, so a
        // Windows Path.Combine result must use that same spelling.
        private static string TaggedDataPath(string root, string role) =>
            Path.Combine(root, "MultiplayerPlayMode", role).Replace('\\', '/');

    }
}
