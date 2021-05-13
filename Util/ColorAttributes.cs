using Terminal.Gui;

namespace Delta_Minus.Util {
    public class ColorAttributes {
        public static ColorAttributes Dark = new ColorAttributes() {
            baseColor = Attribute.Make(Color.Green, Color.Black),
            versionColor = Attribute.Make(Color.Red, Color.Black),
            modColor = Attribute.Make(Color.Cyan, Color.Black),
            addModColor = Attribute.Make(Color.DarkGray, Color.Black),
            addModColor2 = Attribute.Make(Color.Red, Color.DarkGray)
        };
        public static ColorAttributes Win98 = new ColorAttributes() {
            baseColor = Attribute.Make(Color.White, Color.Blue),
            versionColor = Attribute.Make(Color.Gray, Color.Blue),
            modColor = Attribute.Make(Color.Gray, Color.Blue),
            addModColor = Attribute.Make(Color.Black, Color.Cyan),
            addModColor2 = Attribute.Make(Color.Gray, Color.Blue)
        };
        public static ColorAttributes PureWhite = new ColorAttributes() {
            baseColor = Attribute.Make(Color.BrightYellow, Color.White),
            versionColor = Attribute.Make(Color.BrightYellow, Color.White),
            modColor = Attribute.Make(Color.BrightYellow, Color.White),
            addModColor = Attribute.Make(Color.BrightYellow, Color.White),
            addModColor2 = Attribute.Make(Color.BrightYellow, Color.White)
        };
        public static ColorAttributes Mono = new ColorAttributes() {
            baseColor = Attribute.Make(Color.White, Color.Black),
            versionColor = Attribute.Make(Color.White, Color.Black),
            modColor = Attribute.Make(Color.White, Color.Black),
            addModColor = Attribute.Make(Color.White, Color.Black),
            addModColor2 = Attribute.Make(Color.White, Color.Black)
        };
        public static ColorAttributes Caramel = new ColorAttributes() {
            baseColor = Attribute.Make(Color.Brown, Color.Red),
            versionColor = Attribute.Make(Color.Brown, Color.Red),
            modColor = Attribute.Make(Color.Brown, Color.Red),
            addModColor = Attribute.Make(Color.Brown, Color.Red),
            addModColor2 = Attribute.Make(Color.Brown, Color.BrightRed)
        };
        
        public static ColorAttributes Current = Dark;

        public static void SetColor(int color) {
            switch (color) {
                case 1:
                    Current = Dark;
                    break;
                case 2:
                    Current = Win98;
                    break;
                case 3:
                    Current = PureWhite;
                    break;
                case 4:
                    Current = Mono;
                    break;
                case 5:
                    Current = Caramel;
                    break;
            }
        }
        
        public Attribute baseColor;
        public Attribute addModColor2;
        public Attribute versionColor;
        public Attribute modColor;
        public Attribute addModColor;
    }
}