using System;
using System.Runtime.InteropServices;
using static Delta_Minus.Util.SteamAPI;

namespace Delta_Minus.Util {
    internal partial class Native {
        #region kernel32
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr LoadLibrary(string lpFileName);
        [DllImport("kernel32")]
        internal static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);
        [DllImport("kernel32")]
        internal static extern IntPtr GetConsoleWindow();
        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        #endregion
        #region dmdb
        [DllImport("dmdb", SetLastError = true)]
        internal static extern void shutdown();
        #endregion
        #region dwmapi
        [DllImport("dwmapi")]
        internal static extern unsafe IntPtr DwmEnableBlurBehindWindow(IntPtr hWnd, GraphicsUtil.DWM_BLURBEHIND* pBlurBehind);
        #endregion
        #region gdi32
        [DllImport("gdi32")]
        internal static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);
        #endregion
        #region user32
        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out GraphicsUtil.RECT lpRect);
        [DllImport("user32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool MessageBox(IntPtr hWnd, string lpText, string lpCaption, uint uType);
        #endregion
        #region steamapi
        [DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool SteamAPI_Init();
        [DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SteamAPI_Shutdown();
        [DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SteamAPI_GetHSteamPipe();
        [DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SteamAPI_ManualDispatch_RunFrame(HSteamPipe hSteamPipe);
        [DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool SteamAPI_ManualDispatch_GetNextCallback(HSteamPipe pipe, [In, Out] ref CallbackMsg_t msg);
        [DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint SteamAPI_ISteamApps_GetAppInstallDir(IntPtr self, uint appId, IntPtr pchFolder, uint cchFolderBufferSize);
        [DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr SteamAPI_SteamApps_v008();
        #endregion
    }
}
