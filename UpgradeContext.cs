using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DMSAutoUpdater {
    public class UpgradeContext {
        public static string NewVersion { get; set; }
        public static string FileNamePrex = "DMS_";
        public static string FileExtend = ".zip";
        public static string UpgradeDirectoryName = "DMSBak";
        public static string LogFullName = "";
        public static string TempDirectory = "";
        public static string FileName {
            get { return FileNamePrex + NewVersion + FileExtend; }
        }
        public static string DmsFtpUser = "dms";
        public static string DmsFtpPwd = "123!@#shen";
        public static string DmsFtpServer = "114.55.34.43";

        public static string DmsProgramName = "DMS.exe";
    }
}
