using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Eclipse.UI
{
    internal static class ModZipPicker
    {
        public static string PickDesktop()
        {
#if UNITY_EDITOR
            return UnityEditor.EditorUtility.OpenFilePanel("Install Eclipse mod ZIP", "", "zip");
#elif UNITY_STANDALONE_WIN
            IntPtr fileBuffer = Marshal.AllocHGlobal(4096 * sizeof(char));
            try
            {
                Marshal.WriteInt16(fileBuffer, 0);
                var dialog = new OpenFileName
                {
                    size = Marshal.SizeOf(typeof(OpenFileName)),
                    owner = GetActiveWindow(),
                    filter = "ZIP archives\0*.zip\0\0",
                    file = fileBuffer,
                    maxFile = 4096,
                    title = "Install Eclipse mod ZIP",
                    flags = 0x00000008 | 0x00000800 | 0x00001000 | 0x00080000
                };
                if (GetOpenFileName(ref dialog)) return Marshal.PtrToStringUni(fileBuffer);
                int error = CommDlgExtendedError();
                if (error != 0) throw new IOException("Windows file picker failed (" + error + ").");
                return null;
            }
            finally { Marshal.FreeHGlobal(fileBuffer); }
#else
            throw new PlatformNotSupportedException("ZIP selection is available in the editor, Windows and Android builds.");
#endif
        }

        public static void PickAndroid(string receiver)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            using (var unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var activity = unity.GetStatic<AndroidJavaObject>("currentActivity"))
            using (var picker = new AndroidJavaClass("com.project.eclipse.modding.ModZipPicker"))
                picker.CallStatic("pick", activity, receiver);
#else
            throw new PlatformNotSupportedException("Android's document picker is unavailable on this platform.");
#endif
        }

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct OpenFileName
        {
            public int size;
            public IntPtr owner;
            public IntPtr instance;
            [MarshalAs(UnmanagedType.LPWStr)] public string filter;
            public IntPtr customFilter;
            public int maxCustomFilter;
            public int filterIndex;
            public IntPtr file;
            public int maxFile;
            public IntPtr fileTitle;
            public int maxFileTitle;
            [MarshalAs(UnmanagedType.LPWStr)] public string initialDirectory;
            [MarshalAs(UnmanagedType.LPWStr)] public string title;
            public int flags;
            public short fileOffset;
            public short fileExtension;
            [MarshalAs(UnmanagedType.LPWStr)] public string defaultExtension;
            public IntPtr customData;
            public IntPtr hook;
            public IntPtr templateName;
            public IntPtr reserved;
            public int reservedFlags;
            public int flagsEx;
        }

        [DllImport("comdlg32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetOpenFileNameW")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetOpenFileName(ref OpenFileName dialog);

        [DllImport("comdlg32.dll")]
        private static extern int CommDlgExtendedError();

        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();
#endif
    }
}
