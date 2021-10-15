using Delta_Minus.Assets;
using System;
using System.IO;

namespace Delta_Minus.Util {
    class DiscordAPI {
        internal static readonly string Cache_FOLDER = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DeltaMinus\\Cache");
        internal static readonly string DiscordAPI_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DeltaMinus\\Cache\\dmdb.dll");

        public static void Init() {
            Native.LoadLibrary(DiscordAPI_PATH);
#if _WINDOWS
            Program.handler += new EventHandler(Shutdown);
#endif
            Directory.CreateDirectory(Cache_FOLDER);
            try {
                File.WriteAllBytes(DiscordAPI_PATH, NativeResources.dmdb);
            } catch (Exception) {/*Process being used elsewhere or some other related error*/}
        }

        private static bool Shutdown(CtrlType sig) {
            try {
                Native.shutdown();
                return true;
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            return false;
        }
    }
}