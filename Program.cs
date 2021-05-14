using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;
using Delta_Minus.Assets;
using Delta_Minus.Gui;
using Delta_Minus.Util;
using Steamworks;
using Terminal.Gui;

namespace Delta_Minus {
    internal class Program {
        public static Preferences prefs = new(){theme = 1};
        public static void Main() {
            if (!File.Exists("steam_api64.dll"))
                File.WriteAllBytes("steam_api64.dll", NativeResources.steam_api64);
            if (!prefs.Exists())
                prefs.Save();
            prefs.Load();
            SteamClient.Init(960090);
            Checks();
            Application.Run<App>();
            prefs.Save();
        }

        private static void Checks() {
            var alc = new AssemblyLoadContext("Temporary Context", true);
            if (!File.Exists(SteamApps.AppInstallDir() + @"\version.dll") || alc.LoadFromAssemblyPath(SteamApps.AppInstallDir() + @"\MelonLoader\MelonLoader.dll").GetName().Version.Minor < 3)
                App.MLinstalled = false;
            alc.Unload();
        }
    }
}