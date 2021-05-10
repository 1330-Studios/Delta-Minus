using System.IO;
using System.Reflection;
using Delta_Minus.Gui;
using Steamworks;
using Terminal.Gui;

namespace Delta_Minus {
    class Program {
        static void Main(string[] args) {
            SteamClient.Init(960090);
            Checks();
            Application.Run<App>();
        }

        static void Checks() {
            if (!File.Exists(SteamApps.AppInstallDir() + @"\version.dll") || Assembly.Load(File.ReadAllBytes(SteamApps.AppInstallDir() + @"\MelonLoader\MelonLoader.dll")).GetName().Version.Minor < 3)
                App.MLinstalled = false;
        }
    }
}