using Delta_Minus.Util;
using NStack;
using Terminal.Gui;

namespace Delta_Minus.Gui {
    public class K_OpenDialog : OpenDialog {
        public K_OpenDialog(ustring title, ustring message) : base(title, message) {
            ColorScheme.Normal = ColorAttributes.gray_black;
            ColorScheme.Focus = ColorAttributes.red_gray;
        }
    }
}