using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Delta_Minus.Util {
    public static class Extensions {
        public static string makeMarked(this string original) {
            var sb = new StringBuilder();
            foreach (var carr in original.ToCharArray()) sb.Append("_" + carr);
            return sb.ToString();
        }

        public static void openLink(this string url) {
            try {
                Process.Start(url);
            }
            catch {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") {CreateNoWindow = true});
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                    Process.Start("open", url);
                }
                else {
                    throw;
                }
            }
        }

        public static void openFolder(this string uri) {
            try {
                Process.Start(uri);
            }
            catch {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                    uri = uri.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("explorer.exe", $"/select,{uri}") {CreateNoWindow = true});
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                    Process.Start("xdg-open", uri);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                    Process.Start("open", uri);
                }
                else {
                    throw;
                }
            }
        }
    }
}