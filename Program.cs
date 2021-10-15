using Delta_Minus.Assets;
using Delta_Minus.Gui;
using Delta_Minus.Util;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

namespace Delta_Minus {
    internal class Program {
        public static Preferences prefs = new() { Theme = 0, Version = 2, BTD6InstallLocation = "CHANGE", Transparency = 1 };
        public static IApp app;
        public static IntPtr handle;
        public static Util.EventHandler handler;

        internal static void AssignHandler() =>
            Native.SetConsoleCtrlHandler(handler, true);

        public static void Main(string[] args) {
            var lookForSteam = true;
            if (args.Length == 1) lookForSteam = !args[0].ToLower().Equals("passedcheck");

            if (!prefs.Exists())
                prefs.Save();
            prefs.Load();
            if (prefs.badPrefs)
                prefs = Preferences.defaultPrefs;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                handle = Native.GetConsoleWindow();
                _1330API.Init();
                SteamAPI.Init(lookForSteam);
                DiscordAPI.Init();
                GraphicsUtil.Init();
                AssignHandler();
                Checks();
                app = new App();
            }
        }

        private static void Checks() {
            var alc = new AssemblyLoadContext("Temporary Context", true);
            if (!File.Exists(SteamAPI.GetAppInstallDir(SteamAPI.appId) + @"\version.dll") || alc.LoadFromAssemblyPath(SteamAPI.GetAppInstallDir(SteamAPI.appId) + @"\MelonLoader\MelonLoader.dll").GetName().Version.Minor < 4)
                App.MLinstalled = false;
            alc.Unload();
        }
    }
}