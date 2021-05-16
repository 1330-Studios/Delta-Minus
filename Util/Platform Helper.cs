using System.Runtime.InteropServices;

namespace Delta_Minus.Util {
    public static class PlatformHelper {
        public static readonly Platform Linux = new() {id = 1, PlatformName = "Linux"};
        public static readonly Platform OSX = new() {id = 2, PlatformName = "OSX"};
        public static readonly Platform Windows = new() {id = 3, PlatformName = "Windows"};
        public static readonly Platform None = new() {id = -1, PlatformName = "NONE"};

        public static OSPlatform convert(Platform platform) =>
            platform.id switch {
                1 => OSPlatform.Linux,
                2 => OSPlatform.OSX,
                _ => OSPlatform.Windows
            };

        public static Platform convert(OSPlatform platform) =>
            platform.ToString() switch {
                "LINUX" => PlatformHelper.Linux, 
                "OSX" => PlatformHelper.OSX,
                "WINDOWS" => PlatformHelper.Windows,
                _ => PlatformHelper.None
            };

        public static Platform current() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return Windows;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return Linux;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return OSX;
            return None;
        }

        public struct Platform {
            public int id;
            public string PlatformName;
        }
    }
}