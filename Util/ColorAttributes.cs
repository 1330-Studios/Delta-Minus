﻿using Terminal.Gui;

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
        public static ColorAttributes Light = new ColorAttributes() {
            baseColor = Attribute.Make(Color.Green, Color.White),
            versionColor = Attribute.Make(Color.Red, Color.White),
            modColor = Attribute.Make(Color.Cyan, Color.White),
            addModColor = Attribute.Make(Color.DarkGray, Color.White),
            addModColor2 = Attribute.Make(Color.Red, Color.Gray)
        };
        public static ColorAttributes Caramel = new ColorAttributes() {
            baseColor = Attribute.Make(Color.Brown, Color.Red),
            versionColor = Attribute.Make(Color.Brown, Color.Red),
            modColor = Attribute.Make(Color.Brown, Color.Red),
            addModColor = Attribute.Make(Color.Brown, Color.Red),
            addModColor2 = Attribute.Make(Color.Brown, Color.BrightRed)
        };
        public static ColorAttributes Orange = new ColorAttributes() {
            baseColor = Attribute.Make(Color.Red, Color.Brown),
            versionColor = Attribute.Make(Color.Red, Color.Brown),
            modColor = Attribute.Make(Color.Red, Color.Brown),
            addModColor = Attribute.Make(Color.Red, Color.Brown),
            addModColor2 = Attribute.Make(Color.Red, Color.Brown)
        };
        public static ColorAttributes SilentStorm = new ColorAttributes() {
            baseColor = Attribute.Make(Color.BrightRed, Color.Magenta),
            versionColor = Attribute.Make(Color.BrightRed, Color.Magenta),
            modColor = Attribute.Make(Color.BrightRed, Color.Magenta),
            addModColor = Attribute.Make(Color.BrightRed, Color.Magenta),
            addModColor2 = Attribute.Make(Color.BrightRed, Color.Magenta)
        };
        public static ColorAttributes WaterAndLightning = new ColorAttributes() {
            baseColor = Attribute.Make(Color.Blue, Color.BrightYellow),
            versionColor = Attribute.Make(Color.Blue, Color.BrightYellow),
            modColor = Attribute.Make(Color.Blue, Color.BrightYellow),
            addModColor = Attribute.Make(Color.Blue, Color.BrightYellow),
            addModColor2 = Attribute.Make(Color.Blue, Color.BrightYellow)
        };
        public static ColorAttributes NoMoreEyes = new ColorAttributes() {
            baseColor = Attribute.Make(Color.BrightRed, Color.Brown),
            versionColor = Attribute.Make(Color.BrightMagenta, Color.BrightGreen),
            modColor = Attribute.Make(Color.BrightYellow, Color.BrightRed),
            addModColor = Attribute.Make(Color.BrightGreen, Color.BrightMagenta),
            addModColor2 = Attribute.Make(Color.Brown, Color.BrightYellow)
        };
        
        public static ColorAttributes Current = Dark;

        public static void SetColor(byte color) {
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
                    Current = Light;
                    break;
                case 6:
                    Current = Caramel;
                    break;
                case 7:
                    Current = Orange;
                    break;
                case 8:
                    Current = SilentStorm;
                    break;
                case 9:
                    Current = WaterAndLightning;
                    break;
                case 10:
                    Current = NoMoreEyes;
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