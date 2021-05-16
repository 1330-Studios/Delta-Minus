using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Delta_Minus.Util;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Archives.Zip;
using Terminal.Gui;

namespace Delta_Minus.Gui {
    public class AppUnix : Toplevel {
        private readonly Toplevel _top;

        public AppUnix() {
            Console.Title = "Delta Minus";
            Driver.SetAttribute(ColorScheme.Focus);
            ColorAttributes.SetColor(Program.prefs.theme);
            Application.Init();
            Application.UseSystemConsole = true;
            AutoSize = true;
            _top = Application.Top;
            _top.ColorScheme.Normal = ColorAttributes.Current.baseColor;
            _top.ColorScheme.Focus = ColorAttributes.Current.baseColor;
            run();
            Application.Run();
        }

        private void run() {
            _top.Clear();
            createWindow();
            addMenu();
        }

        private void createWindow() {
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

        private void addMenu() {
            _top.Add(new MenuBar(new MenuBarItem[] {
                new MenuBarItem("BTD6".makeMarked(), getBTD6Content()),
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
                    }),
                    null,
                    new MenuItem("Exit", "", () => Environment.Exit(0), shortcut: Key.CtrlMask | Key.C)
                }),
                new MenuBarItem("About", new[] {
                    new MenuItem("Kosmic", "", () => "https://github.com/KosmicShovel".openLink()),
                    null,
                    new MenuItem("1330 Studios", "", () => "https://github.com/1330-Studios".openLink()),
                    null,
                    new MenuItem("Discord Server", "", () => "http://discord.1330studios.com/".openLink())
                })
            }));
        }

        private MenuItem[] getBTD6Content() {
            var btd6InstallDirUnset = Program.prefs.BTD6InstallLocation.Equals("CHANGE");

            if (!btd6InstallDirUnset)
                return new MenuItem[] {
                    new("Launch (Modded)".makeMarked(), "", () => new Process() {
                            StartInfo = new(Program.prefs.BTD6InstallLocation)
                        }
                        .Start(), shortcut: Key.CtrlMask | Key.L),
                    new MenuItem("Launch (Vanilla)".makeMarked(), "",
                        () => new Process {
                            StartInfo = new(Program.prefs.BTD6InstallLocation, "--no-mods")
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
                                    File.Copy(openDialog.FilePath.ToString(), Path.Combine(Path.Combine(Path.GetDirectoryName(Program.prefs.BTD6InstallLocation), "Mods"), Path.GetFileName(openDialog.FilePath.ToString())), true);
                                    break;
                                case "zip":
                                    using (var archive = ZipArchive.Open(openDialog.FilePath.ToString())) {
                                        foreach (var entry in archive.Entries)
                                            entry.WriteToFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Program.prefs.BTD6InstallLocation), "Mods"), entry.Key));
                                    }
                                    break;
                                case "rar":
                                    using (var archive = RarArchive.Open(openDialog.FilePath.ToString())) {
                                        foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                                            entry.WriteToFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Program.prefs.BTD6InstallLocation), "Mods"), entry.Key));
                                    }
                                    break;
                                case "7z":
                                case "7zip":
                                    using (var archive = SevenZipArchive.Open(openDialog.FilePath.ToString())) {
                                        foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                                            entry.WriteToFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Program.prefs.BTD6InstallLocation), "Mods"), entry.Key));
                                    }
                                    break;
                                default:
                                    MessageBox.ErrorQuery(50, 7, "SELECTED FILE IS NOT VALID!", "FILE DOES NOT HAVE VALID EXTENSION!", "Okay");
                                    break;
                            }

                        Reset();
                    }, shortcut: Key.CtrlMask | Key.A),
                    new MenuItem("Open Folder".makeMarked(), ": Open BTD6 folder",
                        () => (Path.GetDirectoryName(Program.prefs.BTD6InstallLocation) + @"\Mods").openFolder(),
                        shortcut: Key.CtrlMask | Key.O)
                };

            return new MenuItem[] {
                new MenuItem("Set Location".makeMarked(), " : Set the install location for BTD6", () => {
                    var openDialog = new OpenDialog("Select your BloonsTD6.exe", "Select a EXE");

                    openDialog.ColorScheme.Normal = ColorAttributes.Current.addModColor;
                    openDialog.ColorScheme.Focus = ColorAttributes.Current.addModColor2;
                    Application.Run(openDialog);
                    if (!openDialog.Canceled)
                    if (Path.GetExtension(openDialog.FilePath.ToString()).ToUpper().Equals(".EXE")) {Program.prefs.BTD6InstallLocation = openDialog.FilePath.ToString();Reset();}
                    else MessageBox.ErrorQuery(50, 7, "SELECTED FILE IS NOT VALID!", "FILE DOES NOT HAVE VALID EXTENSION!", "Okay");

                    Reset();
                })
            };
        }

        private void SetColor(byte col) {
            Program.prefs.theme = col;
            ColorAttributes.SetColor(col);
            Reset();
        }

        private void Reset() {
            Program.prefs.Save();
            Application.Shutdown();
            Application.Run<AppUnix>();
        }
    }
}