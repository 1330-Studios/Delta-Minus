using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using Delta_Minus.Util;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Archives.Zip;
using Steamworks;
using Terminal.Gui;

namespace Delta_Minus.Gui {
    public class App : Toplevel {
        public static bool MLinstalled = true;
        private Toplevel _top;
        
        public App() {
            Console.Title = "Delta Minus";
            Driver.SetAttribute(ColorScheme.Focus);
            Application.Init();
            Application.UseSystemConsole = true;
            _top = Application.Top;
            _top.ColorScheme.Normal = ColorAttributes.green_black;
            _top.ColorScheme.Focus = ColorAttributes.green_black;
            if (MLinstalled)
                resetMods();
            else {
                MessageBox.ErrorQuery("Can't find MelonLoader v0.3.0", "MelonLoader v0.3.0 can not be detected. Either install it or update it.", "Ok");
                "https://github.com/LavaGang/MelonLoader.Installer/releases".openLink();
                Environment.Exit(0);
            }

            Application.Run();
        }

        private void windowCreation() {
            var window = new Window("Delta Minus") {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            window.ColorScheme.Focus = window.ColorScheme.Normal;
            window.ColorScheme.HotFocus = window.ColorScheme.Normal;
            _top.Add(window);
            var version = new Label($"Version: {Assembly.GetCallingAssembly().GetName().Version.ToString(3)}");
            version.ColorScheme = new ColorScheme();
            version.ColorScheme.Normal = ColorAttributes.red_black;
            _top.LayoutComplete += delegate(LayoutEventArgs args) { version.X = Pos.Right(window) - 15; version.Y = Pos.Top(window); };
            _top.Add(version);
        }

        private void addMenuBar() {
            var menu = new MenuBar(new [] {
                new MenuBarItem("BTD6".makeMarked(), new MenuItem[] {
                    new MenuItem("Launch (Modded)".makeMarked(), "", () => new Process() {StartInfo = new(SteamApps.AppInstallDir(getGameId()) + @"\" + getGamesEXEName(getGameId()))}.Start(), shortcut: Key.CtrlMask | Key.L),
                    new MenuItem("Launch (Vanilla)".makeMarked(), "", () => new Process() {StartInfo = new(SteamApps.AppInstallDir(getGameId()) + @"\" + getGamesEXEName(getGameId()), "--no-mods")}.Start(), shortcut: Key.CtrlMask | Key.ShiftMask | Key.L),
                    null,
                    new MenuItem("Add".makeMarked(), ": Adds a mod", () => {
                        K_OpenDialog openDialog = new K_OpenDialog("Select Mod", "Select a DLL, ZIP, 7z, or RAR");
                        Application.Run(openDialog);

                        if (!openDialog.Canceled)
                            switch (Path.GetExtension(openDialog.FilePath.ToString()).Replace(".", "")) {
                                case "dll":
                                    File.Copy(openDialog.FilePath.ToString(),
                                        SteamApps.AppInstallDir(getGameId()) + @"\Mods\" + Path.GetFileName(openDialog.FilePath.ToString()), true);
                                    break;
                                case "zip":
                                    using (var archive = ZipArchive.Open(openDialog.FilePath.ToString()))
                                        foreach (var entry in archive.Entries)
                                            entry.WriteToFile(SteamApps.AppInstallDir(getGameId()) + @"\Mods\" + entry.Key);
                                    break;
                                case "rar":
                                    using (var archive = RarArchive.Open(openDialog.FilePath.ToString()))
                                        foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                                            entry.WriteToFile(SteamApps.AppInstallDir(getGameId()) + @"\Mods\" + entry.Key);
                                    break;
                                case "7z":
                                case "7zip":
                                    using (var archive = SevenZipArchive.Open(openDialog.FilePath.ToString()))
                                        foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                                            entry.WriteToFile(SteamApps.AppInstallDir(getGameId()) + @"\Mods\" + entry.Key);
                                    break;
                                default:
                                    MessageBox.ErrorQuery(50, 7, "SELECTED FILE IS NOT VALID!", "FILE DOES NOT HAVE VALID EXTENSION!", "Okay");
                                    break;
                            }

                        Application.Shutdown();
                        Application.Run<App>();
                    }, shortcut: Key.CtrlMask | Key.A),
                    new MenuItem("Open Folder".makeMarked(), ": Open BTD6 folder", () => (SteamApps.AppInstallDir(getGameId())+@"\Mods").openFolder(), shortcut: Key.CtrlMask | Key.O)
                }),
                new MenuBarItem("About", new [] {
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
            for (int i = 0; i < files.Length; i++) {
                var l = new Label(1, i+3, Path.GetFileName(files[i]));
                l.ColorScheme = new ColorScheme();
                l.ColorScheme.Normal = ColorAttributes.aqua_black;
                var filePath = files[i];
                l.Clicked += delegate {
                    l.ColorScheme.Normal = ColorAttributes.red_black;
                    var res = MessageBox.Query("Are you sure you want to delete this mod?", "", "Yes", "No");
                    if (res != 1) { 
                        File.Delete(filePath);
                        Thread.Sleep(250);
                    }
                    Application.Shutdown();
                    Application.Run<App>();
                };
                _top.Add(l);
            }
        }


        private uint getGameId() {
            //TODO other game support
            return 960090;
        }
        
        private string getGamesEXEName(uint game) {
            //TODO other game support
            return "BloonsTD6.exe";
        }
    }
}