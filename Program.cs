using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using Delta_Minus.Gui;
using Delta_Minus.Util;
using Steamworks;

namespace Delta_Minus {
    internal class Program {
        public static Preferences prefs = new() { Theme = 1, BTD6InstallLocation = "CHANGE" };
        public static IApp app;

        public static void Main() {
            if (!prefs.Exists())
                prefs.Save();
            prefs.Load();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                SteamAPI.Init();
                SteamClient.Init(960090);
                Checks();
                app = new App();
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                app = new AppUnix();
        }

        private static void Checks() {
            var alc = new AssemblyLoadContext("Temporary Context", true);
            if (!File.Exists(SteamApps.AppInstallDir() + @"\version.dll") || alc.LoadFromAssemblyPath(SteamApps.AppInstallDir() + @"\MelonLoader\MelonLoader.dll").GetName().Version.Minor < 4)
                App.MLinstalled = false;
            alc.Unload();
        }
    }
}