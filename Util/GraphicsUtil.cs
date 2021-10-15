using Delta_Minus.Assets;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

namespace Delta_Minus.Util {
    class GraphicsUtil {
        internal static long timeOfLastUpdate = 0;

        internal static Thread graphicsUpdateThread = new(() => {
            var icon = NativeResources.DeltaMinus;
            graphics = Graphics.FromHwnd(Program.handle);
            while (true) {
                Native.GetWindowRect(Program.handle, out RECT lpRect);
                var width = lpRect.right - lpRect.left;
                var imageX = width < 976 ? width : 976;
                graphics.DrawImage(icon, new Rectangle(imageX - 75, 30, 50, 50));
            }
        }) { IsBackground = true };

        internal static Graphics graphics;

        public static void Init() {
            graphicsUpdateThread.Start();

            DWM_BLURBEHIND bb = new() {
                dwFlags = DWM_BB.Enable,
                fEnable = true
            };
            graphics = Graphics.FromHwnd(Program.handle);

            bb.SetRegion(graphics, new Region(new Rectangle((int)-graphics.DpiX, (int)-graphics.DpiY, (int)graphics.DpiX, (int)graphics.DpiY)));
            if (Program.prefs.Transparency == 0)
                bb = new() {
                    fEnable = false,
                    dwFlags = DWM_BB.Disable
                };

            unsafe {
                Native.DwmEnableBlurBehindWindow(Program.handle, &bb);
            }
        }


        [StructLayout(LayoutKind.Sequential)]
        internal struct DWM_BLURBEHIND {
            public DWM_BB dwFlags;
            public bool fEnable;
            public IntPtr hRgnBlur;
            public bool fTransitionOnMaximized;

            public DWM_BLURBEHIND(bool enabled) {
                fEnable = enabled;
                hRgnBlur = IntPtr.Zero;
                fTransitionOnMaximized = false;
                dwFlags = DWM_BB.Enable;
            }

            public Region Region {
                get { return Region.FromHrgn(hRgnBlur); }
            }

            public bool TransitionOnMaximized {
                get { return fTransitionOnMaximized; }
                set {
                    fTransitionOnMaximized = value;
                    dwFlags |= DWM_BB.TransitionMaximized;
                }
            }

            public void SetRegion(Graphics graphics, Region region) {
                hRgnBlur = region.GetHrgn(graphics);
                dwFlags |= DWM_BB.BlurRegion;
            }
        }

        [Flags]
        internal enum DWM_BB {
            Enable = 1,
            BlurRegion = 2,
            Disable = 3,
            TransitionMaximized = 4
        }

        public struct RECT {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
    }
}
