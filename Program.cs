using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DMSAutoUpdater {
    static class Program {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            CreateTempDirectory();
            Application.ThreadException += Application_ThreadException;
            if (args.Length > 0) {
                UpgradeContext.NewVersion = args[0];
            } else {
                UpgradeContext.NewVersion = NewVersion();
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (string.IsNullOrEmpty(UpgradeContext.NewVersion)) {
                MessageBox.Show("未检测到更新包，即将退出...");
                return;
            }
            if (!CheckNeedUpgrade(UpgradeContext.NewVersion)) {
                MessageBox.Show("当前程序已是最新版本，无需更新...");
                return;
            }
            Utils.WriteLog(UpgradeContext.LogFullName, "开始升级...");
            Application.Run(new frmMainForm());
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e) {
            Utils.WriteLog(UpgradeContext.LogFullName, "程序运行错误", e.Exception);
        }


        private static bool CheckNeedUpgrade(string newVersion) {
            try {
                System.Diagnostics.FileVersionInfo fv = System.Diagnostics.FileVersionInfo.GetVersionInfo("DMS.exe");
                if (fv.FileVersion.Equals(newVersion)) {
                    return false;
                }
            } catch (Exception ex) {
            }
            return true;
        }
        public static void CreateTempDirectory() {
            UpgradeContext.TempDirectory = Path.Combine(new DirectoryInfo(Environment.CurrentDirectory).Parent.FullName,
                UpgradeContext.UpgradeDirectoryName);
            if (!Directory.Exists(UpgradeContext.TempDirectory)) {
                Directory.CreateDirectory(UpgradeContext.TempDirectory);
            }
            UpgradeContext.LogFullName = Path.Combine(UpgradeContext.TempDirectory, "DMS_1.0.1.50.log");
        }

        public static string NewVersion() {
            try {
                List<string> fileList = Utils.GetFileListFromFtp(UpgradeContext.DmsFtpServer, UpgradeContext.DmsFtpUser, UpgradeContext.DmsFtpPwd, "/");
                string maxFileName = fileList.Max();
                return maxFileName.Substring(maxFileName.IndexOf("_") + 1).TrimEnd(".zip".ToCharArray());
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
                Utils.WriteLog(UpgradeContext.LogFullName, "获取更新包版本出错===>" + ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return "";
        }
    }
}
