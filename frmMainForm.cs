using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace DMSAutoUpdater {
    public delegate void DownLoadFile(int size, bool complete);

    public partial class frmMainForm : Form {
        public frmMainForm() {
            InitializeComponent();
        }

        public void StartDownLoad() {
            Thread thread = new Thread(this.DownLoadFileFromFtp);
            thread.Start();
        }

        public void DownLoadFileFromFtp() {
            FtpWebRequest reqFTP = null;
            FileStream outputStream = null;
            Stream ftpStream = null;
            FtpWebResponse response = null;
            try {
                //下载到本地的文件路径
                string localFileName = GetLocalFileName();
                Utils.WriteLog(UpgradeContext.LogFullName, "开始下载更新包...");
                outputStream = new FileStream(localFileName, FileMode.Create);
                //FTP文件路径
                string ftpFileName = string.Format("ftp://{0}/", UpgradeContext.DmsFtpServer) + UpgradeContext.FileName;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpFileName));
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.UsePassive = false;
                reqFTP.Credentials = new NetworkCredential(UpgradeContext.DmsFtpUser, UpgradeContext.DmsFtpPwd);
                response = (FtpWebResponse)reqFTP.GetResponse();
                ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 2048;
                int readCount = 0;
                byte[] buffer = new byte[bufferSize];

                while ((readCount = ftpStream.Read(buffer, 0, bufferSize)) > 0) {
                    outputStream.Write(buffer, 0, readCount);
                    this.Invoke(new DownLoadFile(this.DowloadDms), readCount, false);
                }
                Utils.WriteLog(UpgradeContext.LogFullName, "更新包下载完毕...");
                this.Invoke(new DownLoadFile(this.DowloadDms), 0, true);
            } catch (Exception ex) {
                Utils.WriteLog(UpgradeContext.LogFullName, "下载更新包出错" + ex.Message + Environment.NewLine + ex.StackTrace);
            } finally {
                if (ftpStream != null) {
                    ftpStream.Close();
                }
                if (outputStream != null) {
                    outputStream.Close();
                }
                if (response != null) {
                    response.Close();
                }
            }
        }

        private void InitFileSize() {
            string ftpFileName = string.Format("ftp://{0}/", UpgradeContext.DmsFtpServer) + UpgradeContext.FileName;
            fileTotalSize = Utils.GetFtpFileSize(UpgradeContext.DmsFtpUser, UpgradeContext.DmsFtpPwd, ftpFileName) / 1024;
        }

        public string GetLocalFileName() {
            string path = UpgradeContext.TempDirectory;
            string fileName = Path.Combine(path, UpgradeContext.FileName);
            return fileName;
        }

        private int size = 0;
        private int fileTotalSize = 0;
        public void DowloadDms(int size, bool complete) {
            this.size = this.size + size / 1024;
            string msg = string.Format("正在下载更新包，共 {0} KB，已完成 {1} KB......", fileTotalSize.ToString("#,##0")
                , this.size.ToString("#,##0"));
            if (complete) {
                msg = string.Format("更新包下载完毕，共 {0} KB......", fileTotalSize.ToString("#,##0"));
            }
            this.lblInfo.Text = msg;
            if (complete) {
                this.lblUpgrade.Visible = true;
                this.StartUpgrade();
            }
        }

        public void StartUpgrade() {
            // 获取更新包名称
            string localFileName = GetLocalFileName();
            string filePath = Environment.CurrentDirectory;
            string desPath = UpgradeContext.TempDirectory;
            //备份配置文件
            this.BulkConfigFile();
            //开始解压缩
            Utils.WriteLog(UpgradeContext.LogFullName, "解压缩更新包...");
            Utils.UnZip(localFileName, desPath, UnZipDapPackageCallBack);
        }

        public void UnZipDapPackageCallBack(object sender, EventArgs e) {
            try {
                //更新程序
                this.UpdateNewProgram();
                //还原配置文件(暂时不还原了)
                //this.RestoreConfigFile();
                //删除解压缩临时文件
                this.DeleteUpgradePackage();
                //启动启动DMS
                this.StartDms();
                Utils.WriteLog(UpgradeContext.LogFullName, "升级完毕..." + Environment.NewLine + Environment.NewLine);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "升级出错：" + ex.Message);
                Utils.WriteLog(UpgradeContext.LogFullName, "升级出错", ex);
            }
        }

        private void UpdateNewProgram() {
            string sourcePath = Path.Combine(UpgradeContext.TempDirectory, "DMS");
            string desPath = Environment.CurrentDirectory;

            Utils.WriteLog(UpgradeContext.LogFullName, "正在更新程序...");
            Utils.CopyDirectory(sourcePath, desPath, true);
        }

        private void BulkConfigFile() {
            try {
                Utils.WriteLog(UpgradeContext.LogFullName, "备份配置文件(menu.xml)...");
                string sourceFile = Path.Combine(Environment.CurrentDirectory, "menu.xml");
                string desFile = Path.Combine(UpgradeContext.TempDirectory, "menu.xml");
                File.Copy(sourceFile, desFile, true);
            } catch (Exception ex) {
                Utils.WriteLog(UpgradeContext.LogFullName, "备份配置文件出错", ex);
            }
        }

        private void RestoreConfigFile() {
            try {
                Utils.WriteLog(UpgradeContext.LogFullName, "还原配置文件...");
                string sourceFile = Path.Combine(UpgradeContext.TempDirectory, "menu.xml");
                string desFile = Path.Combine(Environment.CurrentDirectory, "menu.xml");
                File.Copy(sourceFile, desFile, true);
            } catch (Exception ex) {
                Utils.WriteLog(UpgradeContext.LogFullName, "还原配置文件出错", ex);
            }
        }

        private void DeleteUpgradePackage() {
            try {
                Utils.WriteLog(UpgradeContext.LogFullName, "删除临时文件...");
                string path = Path.Combine(new DirectoryInfo(Environment.CurrentDirectory).Parent.FullName,
                     UpgradeContext.UpgradeDirectoryName, "DMS");
                Directory.Delete(path, true);
            } catch (Exception ex) {
                Utils.WriteLog(UpgradeContext.LogFullName, "删除临时文件出错", ex);
            }
        }

        private void StartDms() {
            Utils.WriteLog(UpgradeContext.LogFullName, "启动DMS...");
            timer.Enabled = true;
        }

        private void StartDmsProgram() {
            try {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = UpgradeContext.DmsProgramName;
                startInfo.WindowStyle = ProcessWindowStyle.Normal;
                Process.Start(startInfo);
                Application.Exit();
            } catch (Exception ex) {
                MessageBox.Show("启动DMS出错：" + ex.Message);
                Utils.WriteLog(UpgradeContext.LogFullName, "启动DMS出错", ex);
            }
        }

        private void frmMainForm_Load(object sender, EventArgs e) {
            this.InitFileSize();
            this.StartDownLoad();
        }

        private int second = 5;
        private void timer_Tick(object sender, EventArgs e) {
            lblUpgrade.Text = string.Format("更新完毕，正在重新启动({0})...", second);
            second--;
            if (second <= 0) {
                this.Close();
                this.StartDmsProgram();
            }
        }

        private void frmMainForm_FormClosed(object sender, FormClosedEventArgs e) {
            timer.Enabled = false;
        }
    }
}
