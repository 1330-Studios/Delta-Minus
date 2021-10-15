using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Delta_Minus.Util {
    
    public class Preferences {
        internal bool badPrefs;
        internal static Preferences defaultPrefs = new() { Theme = 0, Version = 2, Transparency = 1, BTD6InstallLocation = "CHANGE" };

        public byte Theme { get; set; }
        public byte Version { get; set; }
        public byte Transparency { get; set; }
        public string BTD6InstallLocation { get; set; }
        public static readonly string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DeltaMinus");
        private static readonly string fileLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DeltaMinus\\options.dat");

        #region key

        private static readonly byte[] key = new byte[] { 0x44, 0x65, 0x6c, 0x74, 0x61, 0x4d, 0x69, 0x6e, 0x75, 0x73, 0x5f, 0x32, 0x30, 0x32, 0x31, 0x5f, 0x31, 0x33, 0x33, 0x30, 0x20, 0x53, 0x74, 0x75, 0x64, 0x69, 0x6f, 0x73};

        #endregion
        
        public void Save() {
            _ = Directory.CreateDirectory(dir);
            using var fs = File.Create(fileLocation);
            var rfc = new Rfc2898DeriveBytes(Encoding.Default.GetString(key), new byte[] {0x48, 0x89, 0x23, 0x77, 0x45, 0x57, 0x58, 0x14, 0x22, 0x64});
            using Aes aes = Aes.Create();
            aes.Key = rfc.GetBytes(aes.KeySize / 8);
            aes.IV = rfc.GetBytes(aes.BlockSize / 8);
            using var cs = new CryptoStream(fs, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.WriteByte(Theme);
            cs.WriteByte(Version);
            cs.WriteByte(Transparency);
            using var sw = new StreamWriter(cs, Encoding.UTF8);
            sw.Write(BTD6InstallLocation);
        }

        public void Load() {
            var bytes = new byte[1];
            using var fs = File.Open(fileLocation, FileMode.Open);
            var rfc = new Rfc2898DeriveBytes(Encoding.Default.GetString(key), new byte[] {0x48, 0x89, 0x23, 0x77, 0x45, 0x57, 0x58, 0x14, 0x22, 0x64});
            using Aes aes = Aes.Create();
            aes.Key = rfc.GetBytes(aes.KeySize / 8);
            aes.IV = rfc.GetBytes(aes.BlockSize / 8);
            using var cs = new CryptoStream(fs, aes.CreateDecryptor(), CryptoStreamMode.Read);
            cs.Read(bytes, 0, bytes.Length);
            Program.prefs.Theme = bytes[0];
            cs.Read(bytes, 0, bytes.Length);
            if (bytes[0] == 0x43 || bytes[0] != defaultPrefs.Version)
                badPrefs = true;
            Program.prefs.Version = bytes[0];
            cs.Read(bytes, 0, bytes.Length);
            Program.prefs.Transparency = bytes[0];
            using var sr = new StreamReader(cs, Encoding.UTF8);
            var sb = new StringBuilder();
            while (sr.Peek() >= 0) sb.Append((char)((byte)sr.Read()));
            BTD6InstallLocation = sb.ToString();
        }

        public bool Exists() => File.Exists(fileLocation);

        private byte ReadByte(ref byte[] bytes) {
            var ret = bytes[0];

            var newBytes = new List<byte>();
            for (int i = 1; i < bytes.Length; i++)
                newBytes.Add(bytes[i]);
            bytes = newBytes.ToArray();
            return ret;
        }
    }
}