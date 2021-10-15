using System;

namespace Delta_Minus.Util {
    public class BTD6Mod {
        public string Name { get; set; } = "";
        public string Author { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsMLMod { get; set; } = true;
        public bool IsAPIMod { get; set; } = false;
        public VersionInfo[] Versions { get; set; } = Array.Empty<VersionInfo>();
        public uint InternalID { get; set; } = 0u;

        public class VersionInfo {
            public int VersionFromInitial { get; set; }
            public string ReadableVersion { get; set; } = "";
            public string Btd6Version { get; set; } = "";
            public string LoaderVersion { get; set; } = "";
            public string Changelog { get; set; } = "";
            public string DownloadLink { get; set; } = "";
            public DateOnlyK DateUploaded { get; set; } = new(1984, 1, 1);

            public class DateOnlyK {
                public int Day { get; set; }
                public int Month { get; set; }
                public int Year { get; set; }

                public DateOnlyK(int year, int month, int day) {
                    Day = day;
                    Month = month;
                    Year = year;
                }
            }
        }
    }
}
