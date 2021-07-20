using Delta_Minus.Assets;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Delta_Minus.Util {
    class SteamAPI {
        public static bool Loaded { get; set; }
        internal static readonly string Cache_FOLDER = Environment.ExpandEnvironmentVariables("%AppData%\\DeltaMinus\\Cache");
        internal static readonly string SteamAPI_PATH = Environment.ExpandEnvironmentVariables("%AppData%\\DeltaMinus\\Cache\\steam_api64.dll");

        public static void Init() {
            Directory.CreateDirectory(Cache_FOLDER);
            if (!File.Exists(SteamAPI_PATH))
                File.WriteAllBytes(SteamAPI_PATH, NativeResources.steam_api64);
            Loaded = LoadLibrary(SteamAPI_PATH) != IntPtr.Zero;
        }

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr LoadLibrary(string lpFileName);
    }
}
