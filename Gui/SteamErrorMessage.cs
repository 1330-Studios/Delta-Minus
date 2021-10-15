using Delta_Minus.Util;
using System;
using Terminal.Gui;

namespace Delta_Minus.Gui {
    class SteamErrorMessage : Toplevel {
        private readonly Toplevel _top;

        public SteamErrorMessage(bool type) {
            Application.Init();
            Application.UseSystemConsole = true;
            Console.Title = "Delta Minus";
            Driver.SetAttribute(ColorScheme.Focus);
            ColorAttributes.SetColor(Program.prefs.Theme);
            AutoSize = true;
            _top = Application.Top;
            _top.ColorScheme.Normal = ColorAttributes.Current.baseColor;
            _top.ColorScheme.Focus = ColorAttributes.Current.baseColor;
            if (type) {
                _ = MessageBox.ErrorQuery($"Can't load the Steam API. Please create a support ticket in our discord for further assistance.", "");
                Environment.Exit(0);
            } else if (!type) {
                _ = MessageBox.ErrorQuery($"Please restart Delta Minus with steam open. Please create a support ticket in our discord for further assistance.", "");
                Environment.Exit(0);
            }
            Application.Run();
        }
    }
}
