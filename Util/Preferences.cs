using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Delta_Minus.Util {
    
    public class Preferences {
        public byte theme { get; set; }
        public string BTD6InstallLocation { get; set; }
        private static readonly string fileLocation = "options.dat";

        #region key

        private static readonly byte[] key = new byte[] { 0x44, 0x65, 0x6c, 0x74, 0x61, 0x4d, 0x69, 0x6e, 0x75, 0x73, 0x5f, 0x32, 0x30, 0x32, 0x31, 0x5f, 0x31, 0x33, 0x33, 0x30, 0x20, 0x53, 0x74, 0x75, 0x64, 0x69, 0x6f, 0x73};

        #endregion
        
        public void Save() {
            using var fs = File.Create(fileLocation);
            var rfc = new Rfc2898DeriveBytes(UnicodeEncoding.Default.GetString(key), new byte[] {0x48, 0x89, 0x23, 0x77, 0x45, 0x57, 0x58, 0x14, 0x22, 0x64});
            Aes aes = new AesManaged();
            aes.Key = rfc.GetBytes(aes.KeySize / 8);
            aes.IV = rfc.GetBytes(aes.BlockSize / 8);
            using var cs = new CryptoStream(fs, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.WriteByte(theme);
            using var sw = new StreamWriter(cs, Encoding.UTF8);
            sw.Write(BTD6InstallLocation);
        }

        public void Load() {
            var bytes = new byte[1];
            using var fs = File.Open(fileLocation, FileMode.Open);
            var rfc = new Rfc2898DeriveBytes(UnicodeEncoding.Default.GetString(key), new byte[] {0x48, 0x89, 0x23, 0x77, 0x45, 0x57, 0x58, 0x14, 0x22, 0x64});
            Aes aes = new AesManaged();
            aes.Key = rfc.GetBytes(aes.KeySize / 8);
            aes.IV = rfc.GetBytes(aes.BlockSize / 8);
            using var cs = new CryptoStream(fs, aes.CreateDecryptor(), CryptoStreamMode.Read);
            cs.Read(bytes, 0, bytes.Length);
            Program.prefs.theme = bytes[0];
            using var sr = new StreamReader(cs, Encoding.UTF8);
            var sb = new StringBuilder();
            while (sr.Peek() >= 0) sb.Append((char)((byte)sr.Read()));
            BTD6InstallLocation = sb.ToString();
            //File.WriteAllText("temp.txt", sb.ToString());
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