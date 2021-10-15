using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;

using Delta_Minus.Util;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Archives.Zip;
using Terminal.Gui;

namespace Delta_Minus.Gui {
    public sealed class App : Toplevel, IApp {
        public static bool MLinstalled = true;
        private readonly Toplevel _top;

        public App() {
            Application.Init();
            Application.UseSystemConsole = true;
            Console.Title = "Delta Minus";
            Driver.SetAttribute(ColorScheme.Focus);
            ColorAttributes.SetColor(Program.prefs.Theme);
            AutoSize = true;
            _top = Application.Top;
            _top.ColorScheme.Normal = ColorAttributes.Current.baseColor;
            _top.ColorScheme.Focus = ColorAttributes.Current.baseColor;
            if (MLinstalled) {
                ResetMods();
            }
            else {
                _ = MessageBox.ErrorQuery("Can't find MelonLoader v0.4.3", "MelonLoader v0.4.3 can not be detected. Either install it or update it.", "Ok");
                "https://github.com/LavaGang/MelonLoader.Installer/releases".openLink();
                Environment.Exit(0);
            }
            Application.Run();
        }

        private void WindowCreation() {
            var window = new Window($"Delta Minus ─── {PlatformHelper.Current().PlatformName} ──────────── Preview 1") {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            window.ColorScheme.Focus = window.ColorScheme.Normal;
            window.ColorScheme.HotFocus = window.ColorScheme.Normal;
            _top.Add(window);
            var version = new Label($"Version: {Assembly.GetCallingAssembly().GetName().Version.ToString(3)}") {
                ColorScheme = new() {
                    Normal = ColorAttributes.Current.versionColor
                }
            };
            Application.Resized += delegate {
                version.X = Pos.Right(window) - 15;
                version.Y = Pos.Top(window);
            };
            _top.Add(version);
        }

        private void AddMenuBar() {
            var menu = new MenuBar(new[] {
                new MenuBarItem("BTD6".makeMarked(), new[] {
                    new("Launch (Modded)".makeMarked(), "", () => new Process {
                            StartInfo = new(SteamAPI.GetAppInstallDir(GetGameId()) + @"\" + GetGamesEXEName(GetGameId()))
                        }
                        .Start(), shortcut: Key.CtrlMask | Key.L),
                    new MenuItem("Launch (Vanilla)".makeMarked(), "",
                        () => new Process {
                            StartInfo =
                                new(SteamAPI.GetAppInstallDir(GetGameId()) + @"\" +
                                    GetGamesEXEName(GetGameId()), "--no-mods")
                        }.Start(), shortcut: Key.CtrlMask | Key.ShiftMask | Key.L),
                    null,
                    new MenuItem("Add Mod".makeMarked(), ": Adds a mod", () => {
                        var openDialog = new OpenDialog("Select Mod", "Select a DLL, ZIP, 7z, or RAR");
                        
                        openDialog.ColorScheme.Normal = ColorAttributes.Current.addModColor;
                        openDialog.ColorScheme.Focus = ColorAttributes.Current.addModColor2;
                        Application.Run(openDialog);

                        if (!openDialog.Canceled)
                            switch (Path.GetExtension(openDialog.FilePath.ToString()).Replace(".", "")) {
                                case "dll":
                                    File.Copy(openDialog.FilePath.ToString(),
                                        SteamAPI.GetAppInstallDir(GetGameId()) + @"\Mods\" + Path.GetFileName(openDialog.FilePath.ToString()), true);
                                    break;
                                case "zip":
                                    using (var archive = ZipArchive.Open(openDialog.FilePath.ToString())) {
                                        foreach (var entry in archive.Entries)
                                            entry.WriteToFile(SteamAPI.GetAppInstallDir(GetGameId()) + @"\Mods\" + entry.Key);
                                    }

                                    break;
                                case "rar":
                                    using (var archive = RarArchive.Open(openDialog.FilePath.ToString())) {
                                        foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                                            entry.WriteToFile(SteamAPI.GetAppInstallDir(GetGameId()) + @"\Mods\" + entry.Key);
                                    }

                                    break;
                                case "7z":
                                case "7zip":
                                    using (var archive = SevenZipArchive.Open(openDialog.FilePath.ToString())) {
                                        foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                                            entry.WriteToFile(SteamAPI.GetAppInstallDir(GetGameId()) + @"\Mods\" + entry.Key);
                                    }

                                    break;
                                default:
                                    MessageBox.ErrorQuery(50, 7, "SELECTED FILE IS NOT VALID!",
                                        "FILE DOES NOT HAVE VALID EXTENSION!", "Okay");
                                    break;
                            }

                        Reset();
                    }, shortcut: Key.CtrlMask | Key.A),
                    new MenuItem("Open Folder".makeMarked(), ": Open BTD6 folder",
                        () => (SteamAPI.GetAppInstallDir(GetGameId()) + @"\Mods").openFolder(),
                        shortcut: Key.CtrlMask | Key.O)
                }),
                new MenuBarItem("Options", new [] {
                    new MenuBarItem("Themes", new [] {
                        new MenuItem("Auto", "", () => SetColor(0)),
                        new MenuItem("Dark", "", () => SetColor(1)),
                        new MenuItem("Windows 98", "", () => SetColor(2)),
                        new MenuItem("Pure White", "", () => SetColor(3)),
                        new MenuItem("Monochrome", "", () => SetColor(4)),
                        new MenuItem("Light", "", () => SetColor(5)),
                        new MenuItem("Caramel Apple", "", () => SetColor(6)),
                        new MenuItem("Orange", "", () => SetColor(7)),
                        new MenuItem("Silentstorm™", "", () => SetColor(8)),
                        new MenuItem("Water and Lightning", "", () => SetColor(9)),
                        new MenuItem("No More Eyes", "", () => SetColor(10)),
                    }),
                    new MenuItem("Toggle Transparency", "", () => SetTransparency())
                }),
                new MenuBarItem("About", new[] {
                    new MenuItem("Kosmic", "", () => "https://github.com/KosmicShovel".openLink()),
                    null,
                    new MenuItem("1330 Studios", "", () => "https://github.com/1330-Studios".openLink()),
                    null,
                    new MenuItem("Discord Server", "", () => "http://discord.1330studios.com/".openLink())
                })
            });
            _top.Add(menu);
        }

        private void ResetMods() {
            _top.Clear();
            WindowCreation();
            AddMenuBar();
            var files = Directory.GetFiles(SteamAPI.GetAppInstallDir(GetGameId()) + @"\Mods");
            var alc = new AssemblyLoadContext("Temporary Context MelonLoader dll", true);
            var ML = alc.LoadFromAssemblyPath(SteamAPI.GetAppInstallDir(GetGameId()) + @"\MelonLoader\MelonLoader.dll");
            alc.Unload();
            alc = null;
            for (var i = 0; i < files.Length; i++) {
                var l = new Label();
                try {
                    var alcMod = new AssemblyLoadContext("Temporary Context Mod", true);
                    alcMod.LoadFromAssemblyPath(SteamAPI.GetAppInstallDir(GetGameId()) + @"\MelonLoader\MelonLoader.dll");
                    var asm = alcMod.LoadFromAssemblyPath(files[i]);
                    var mia = asm.CustomAttributes.First(a => a.AttributeType.FullName.Equals("MelonLoader.MelonInfoAttribute"));
                    var name = mia.ConstructorArguments[1].Value.ToString().Trim();
                    var version = mia.ConstructorArguments[2].Value.ToString().Replace("v", "");
                    l = new Label(2, i + 3, $"{name}, v{version}");
                    alcMod.Unload();
                    alcMod = null;
                } catch (Exception) {
                    //Usually when mod is built for previous MelonLoader versions than the one you have
                    l = new Label(2, i + 3, $"{Path.GetFileNameWithoutExtension(files[i])}, v?.?");
                }

                l.ColorScheme = new();
                l.ColorScheme.Normal = ColorAttributes.Current.modColor;
                var filePath = files[i];
                l.Clicked += delegate {
                    l.ColorScheme.Normal = ColorAttributes.Current.versionColor;
                    var res = MessageBox.Query("Are you sure you want to delete this mod?", "", "Yes", "No");
                    if (res != 1) {
                        File.Delete(filePath);
                        Thread.Sleep(250);
                    }

                    Reset();
                };
                _top.Add(l);
            }
        }

        private static void SetColor(byte col) {
            Program.prefs.Theme = col;
            ColorAttributes.SetColor(col);
            Reset();
        }

        private static void SetTransparency() {
            var b = Program.prefs.Transparency;
            if (b != 0)
                Program.prefs.Transparency = 0;
            else
                Program.prefs.Transparency = 1;
            Reset();
        }

        private static void Reset() {
            Program.prefs.Save();
            Application.Shutdown();
            Program.app = null;
            Console.ResetColor();
            Console.Clear();
            var app = AppDomain.CurrentDomain.FriendlyName;
            Process cmd = new() { StartInfo = new(app, "PassedCheck") };
            cmd.Start();
            Environment.Exit(0);
        }


        private static uint GetGameId() =>
            960090;

        private static string GetGamesEXEName(uint game) =>
            "BloonsTD6.exe";
    }
}
