using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DMSAutoUpdater {
    static class Program {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            Application.ThreadException += Application_ThreadException;
            Process instance = RunningInstance(Process.GetCurrentProcess().ProcessName);
            if (instance == null) {
                CreateTempDirectory();
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
                Utils.WriteLog(UpgradeContext.LogFullName, string.Format("开始升级版本[{0}]...", UpgradeContext.NewVersion));
                //尝试删除DMS
                KillDms();
                Application.Run(new frmMainForm());
            }
        }

        private static Process RunningInstance(string processName) {
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(processName);
            //Loop through the running processes in with the same name 
            foreach (Process process in processes) {
                //Ignore the current process 
                if (process.Id != current.Id) {
                    //Make sure that the process is running from the exe file. 
                    if (Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == current.MainModule.FileName) {
                        //Return the other process instance. 
                        return process;
                    }
                }
            }
            //No other instance was found, return null. 
            return null;
        }

        private static void KillDms() {
            Process process = RunningInstance("DMS");
            if (process != null) {
                Utils.WriteLog(UpgradeContext.LogFullName, "停止DMS...");
                process.Kill();
            }
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e) {
            Utils.WriteLog(UpgradeContext.LogFullName, "程序运行错误", e.Exception);
        }


        private static bool CheckNeedUpgrade(string newVersion) {
            try {
                FileVersionInfo fv = FileVersionInfo.GetVersionInfo("DMS.exe");
                return newVersion.CompareTo(fv.FileVersion) > 0;
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
            UpgradeContext.LogFullName = Path.Combine(Environment.CurrentDirectory, "DMS.log");
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
