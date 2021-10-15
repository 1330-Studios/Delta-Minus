using Delta_Minus.Assets;
using Delta_Minus.Gui;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Delta_Minus.Util {
    internal class SteamAPI {
        static bool alreadyLoaded = false;
		static readonly IntPtr Memory = Marshal.AllocHGlobal(1024 * 32);

        internal static Thread SteamCallbackThread = new(() => {
            var pipe = (HSteamPipe)Native.SteamAPI_GetHSteamPipe();
            Native.SteamAPI_ManualDispatch_RunFrame(pipe);
            CallbackMsg_t callback = default;
			while (Native.SteamAPI_ManualDispatch_GetNextCallback(pipe, ref callback)) ;
			Thread.Sleep(16);
        });

        internal static bool lookForSteam;
		internal static uint appId = 960090;
        internal static readonly string Cache_FOLDER = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DeltaMinus\\Cache");
        internal static readonly string SteamAPI_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DeltaMinus\\Cache\\steam_api64.dll");
        
        public static void Init(bool lookForSteam, uint appId = 960090) {
            SteamAPI.lookForSteam = lookForSteam;
			if (SteamAPI.appId != appId) SteamAPI.appId = appId;
            Directory.CreateDirectory(Cache_FOLDER);
            if (!File.Exists(SteamAPI_PATH))
                File.WriteAllBytes(SteamAPI_PATH, NativeResources.steam_api64);
            if (!IsSteamOpen() || Native.LoadLibrary(SteamAPI_PATH) == IntPtr.Zero) _ = new SteamErrorMessage(IsSteamOpen());
            Init(appId);
#if _WINDOWS
            Program.handler += new EventHandler(Shutdown);
#endif
        }
        
        internal static string GetAppInstallDir(uint appId) {
            unsafe { ((byte*)Memory)[0] = 0; }
            _ = Native.SteamAPI_ISteamApps_GetAppInstallDir(Native.SteamAPI_SteamApps_v008(), appId, Memory, (1024 * 32));

            int index = 0;
            while (Marshal.ReadByte(Memory, index) != 0) index++;
            if (index == 0) return string.Empty;

            byte[] arr = new byte[index];
            Marshal.Copy(Memory, arr, 0, arr.Length);
            return Encoding.UTF8.GetString(arr);
        }

        private static bool Shutdown(CtrlType sig) {
            try {
                Native.SteamAPI_Shutdown();
                return true;
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        private static bool IsSteamOpen() => !lookForSteam || Process.GetProcesses().Any(p => p.ProcessName.ToLower().Contains("steam"));

        private static void Init(uint appId) {
            if (alreadyLoaded)
                throw new Exception("Steam already initialized!");

            Environment.SetEnvironmentVariable("SteamAppId", appId.ToString());
            Environment.SetEnvironmentVariable("SteamGameId", appId.ToString());

            var initialized = Native.SteamAPI_Init();

            if (!initialized)
                _ = new SteamErrorMessage(IsSteamOpen());

			SteamCallbackThread.Start();
			alreadyLoaded = true;
		}

        internal struct HSteamPipe : IEquatable<HSteamPipe>, IComparable<HSteamPipe> {
            public HSteamPipe(int value) => m_HSteamPipe = value;
            public override string ToString() => m_HSteamPipe.ToString();
            public override bool Equals(object other) => other is HSteamPipe pipe && this == pipe;
            public override int GetHashCode() => m_HSteamPipe.GetHashCode();
            public static bool operator ==(HSteamPipe x, HSteamPipe y) => x.m_HSteamPipe == y.m_HSteamPipe;
            public static bool operator !=(HSteamPipe x, HSteamPipe y) => !(x == y);
            public static explicit operator HSteamPipe(int value) => new HSteamPipe(value);
            public static explicit operator int(HSteamPipe that) => that.m_HSteamPipe;
            public bool Equals(HSteamPipe other) => m_HSteamPipe == other.m_HSteamPipe;
            public int CompareTo(HSteamPipe other) => m_HSteamPipe.CompareTo(other.m_HSteamPipe);

            public int m_HSteamPipe;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal struct CallbackMsg_t : IEquatable<CallbackMsg_t> {
            public int m_hSteamUser;
            public int m_iCallback;
            public IntPtr m_pubParam;
            public int m_cubParam;

            bool IEquatable<CallbackMsg_t>.Equals(CallbackMsg_t other) =>
                    m_hSteamUser == other.m_hSteamUser &&
                    m_iCallback == other.m_iCallback &&
                    m_pubParam.ToInt64() == other.m_pubParam.ToInt64() &&
                    m_cubParam == other.m_cubParam;
            public override bool Equals(object obj) =>
                 obj is CallbackMsg_t t && ((IEquatable<CallbackMsg_t>)this).Equals(t);
            public override int GetHashCode() =>
                m_iCallback.GetHashCode();
        }
	}
}
