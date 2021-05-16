using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;
using System.Threading;
using Delta_Minus.Util;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Archives.Zip;
using Steamworks;
using Terminal.Gui;
using Terminal.Gui.Graphs;

namespace Delta_Minus.Gui {
    public class App : Toplevel {
        public static bool MLinstalled = true;
        private readonly Toplevel _top;

        public App() {
            Console.Title = "Delta Minus";
            Driver.SetAttribute(ColorScheme.Focus);
            ColorAttributes.SetColor(Program.prefs.theme);
            Application.Init();
            Application.UseSystemConsole = true;
            AutoSize = true;
            _top = Application.Top;
            _top.ColorScheme.Normal = ColorAttributes.Current.baseColor;
            _top.ColorScheme.Focus = ColorAttributes.Current.baseColor;
            if (MLinstalled) {
                resetMods();
            }
            else {
                MessageBox.ErrorQuery("Can't find MelonLoader v0.3.0", "MelonLoader v0.3.0 can not be detected. Either install it or update it.", "Ok");
                "https://github.com/LavaGang/MelonLoader.Installer/releases".openLink();
                Environment.Exit(0);
            }

            Application.Run();
        }

        private void windowCreation() {
            var window = new Window($"Delta Minus ─── {PlatformHelper.current().PlatformName}") {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            window.ColorScheme.Focus = window.ColorScheme.Normal;
            window.ColorScheme.HotFocus = window.ColorScheme.Normal;
            _top.Add(window);
            var version = new Label($"Version: {Assembly.GetCallingAssembly().GetName().Version.ToString(3)}");
            version.ColorScheme = new();
            version.ColorScheme.Normal = ColorAttributes.Current.versionColor;
            Application.Resized += delegate {
                version.X = Pos.Right(window) - 15;
                version.Y = Pos.Top(window);
            };
            _top.Add(version);
        }

        private void addMenuBar() {
            var menu = new MenuBar(new[] {
                new MenuBarItem("BTD6".makeMarked(), new[] {
                    new("Launch (Modded)".makeMarked(), "", () => new Process {
                            StartInfo = new(SteamApps.AppInstallDir(getGameId()) + @"\" + getGamesEXEName(getGameId()))
                        }
                        .Start(), shortcut: Key.CtrlMask | Key.L),
                    new MenuItem("Launch (Vanilla)".makeMarked(), "",
                        () => new Process {
                            StartInfo =
                                new(SteamApps.AppInstallDir(getGameId()) + @"\" +
                                    getGamesEXEName(getGameId()), "--no-mods")
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
                                        SteamApps.AppInstallDir(getGameId()) + @"\Mods\" + Path.GetFileName(openDialog.FilePath.ToString()), true);
                                    break;
                                case "zip":
                                    using (var archive = ZipArchive.Open(openDialog.FilePath.ToString())) {
                                        foreach (var entry in archive.Entries)
                                            entry.WriteToFile(SteamApps.AppInstallDir(getGameId()) + @"\Mods\" + entry.Key);
                                    }

                                    break;
                                case "rar":
                                    using (var archive = RarArchive.Open(openDialog.FilePath.ToString())) {
                                        foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                                            entry.WriteToFile(SteamApps.AppInstallDir(getGameId()) + @"\Mods\" + entry.Key);
                                    }

                                    break;
                                case "7z":
                                case "7zip":
                                    using (var archive = SevenZipArchive.Open(openDialog.FilePath.ToString())) {
                                        foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                                            entry.WriteToFile(SteamApps.AppInstallDir(getGameId()) + @"\Mods\" + entry.Key);
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
                        () => (SteamApps.AppInstallDir(getGameId()) + @"\Mods").openFolder(),
                        shortcut: Key.CtrlMask | Key.O)
                }),
                new MenuBarItem("Options", new [] {
                    new MenuBarItem("Themes", new [] {
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
                    })
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

        private void resetMods() {
            _top.Clear();
            windowCreation();
            addMenuBar();
            var files = Directory.GetFiles(SteamApps.AppInstallDir(getGameId()) + @"\Mods");
            var alc = new AssemblyLoadContext("Temporary Context resetMods", true);
            var ML = alc.LoadFromAssemblyPath(SteamApps.AppInstallDir() + @"\MelonLoader\MelonLoader.dll");
            for (var i = 0; i < files.Length; i++) {
                var l = new Label();
                try {
                    var asm = alc.LoadFromAssemblyPath(files[i]);
                    var mia = asm.CustomAttributes.First(a => a.AttributeType.Assembly.Equals(ML) && a.AttributeType.FullName.Equals("MelonLoader.MelonInfoAttribute"));
                    var name = mia.ConstructorArguments[1].Value.ToString().Trim();
                    var version = mia.ConstructorArguments[2].Value.ToString().Replace("v", "");
                    l = new Label(2, i + 3, $"{name}, v{version}");
                } catch {
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
            alc.Unload();
        }

        private void SetColor(byte col) {
            Program.prefs.theme = col;
            ColorAttributes.SetColor(col);
            Reset();
        }

        private void Reset() {
            Program.prefs.Save();
            Application.Shutdown();
            Application.Run<App>();
        }


        private uint getGameId() =>
            //TODO other game support
            960090;

        private string getGamesEXEName(uint game) =>
            //TODO other game support
            "BloonsTD6.exe";
    }
}