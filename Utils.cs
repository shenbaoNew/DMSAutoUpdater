using SevenZip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DMSAutoUpdater {
    public class Utils {
        public static void UnZip(string fileName, string desPath, EventHandler<EventArgs> finishedEvent) {
            SevenZipExtractor se = new SevenZipExtractor(fileName);
            if (finishedEvent != null) {
                se.ExtractionFinished += finishedEvent;
            }
            se.BeginExtractArchive(desPath);
        }

        /// <summary>
        /// 复制子目录
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="targetPath"></param>
        public static void CopyDirectory(string sourcePath, string targetPath,bool isOverride) {
            DirectoryInfo sourceDir = new DirectoryInfo(sourcePath);
            DirectoryInfo targetDir = new DirectoryInfo(targetPath);
            if (!targetDir.Exists) {
                targetDir.Create();
            }
            FileInfo[] files = sourceDir.GetFiles();
            foreach (FileInfo file in files) {
                try {
                    file.CopyTo(Path.Combine(targetDir.FullName, file.Name), isOverride); //复制目录中所有文件
                } catch (Exception ex) {
                    Console.WriteLine("复制文件出错：" + ex.Message);
                }
            }
            DirectoryInfo[] dirs = sourceDir.GetDirectories();
            foreach (DirectoryInfo dir in dirs) {
                string destinationDir = Path.Combine(targetDir.FullName, dir.Name);
                CopyDirectory(dir.FullName, destinationDir, isOverride); //复制子目录
            }
        }

        public static List<string> GetFileListFromFtp(string ftpServer, string ftpUser, string ftpPwd
            , string ftpPath) {
            FtpWebRequest reqFTP = null;
            FileStream outputStream = null;
            StreamReader reader = null;
            FtpWebResponse response = null;
            List<string> list = new List<string>();
            try {
                //FTP文件路径
                ftpPath = "ftp://" + ftpServer + ftpPath;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpPath));
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                reqFTP.Credentials = new NetworkCredential(ftpUser, ftpPwd);
                reqFTP.UsePassive = false;
                response = (FtpWebResponse)reqFTP.GetResponse();
                reader = new StreamReader(response.GetResponseStream());
                string line = reader.ReadLine();
                while (line != null) {
                    if (!line.Contains("<DIR>")
                        && line.StartsWith("DMS_")) {
                        list.Add(line);
                    }
                    line = reader.ReadLine();
                }
                return list;
            } finally {
                if (reader != null) {
                    reader.Close();
                }
                if (outputStream != null) {
                    outputStream.Close();
                }
                if (response != null) {
                    response.Close();
                }
            }
        }

        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="file">ip服务器下的相对路径</param>
        /// <returns>文件大小</returns>
        public static int GetFtpFileSize(string ftpUser, string ftpPwd, string fileFullName) {
            StringBuilder result = new StringBuilder();
            FtpWebRequest request;
            try {
                request = (FtpWebRequest)FtpWebRequest.Create(new Uri(fileFullName));
                request.UseBinary = true;
                request.Credentials = new NetworkCredential(ftpUser, ftpPwd);//设置用户名和密码
                request.Method = WebRequestMethods.Ftp.GetFileSize;

                int dataLength = (int)request.GetResponse().ContentLength;

                return dataLength;
            } catch (Exception ex) {
                return -1;
            }
        }

        public static void WriteLog(string logFullName, string message) {
            WriteLog(logFullName, message, true, true);
        }
        public static void WriteLog(string logFullName, string tip, Exception ex) {
            WriteLog(logFullName, tip + "===>" + ex.Message + Environment.NewLine + ex.StackTrace, true, true);
        }
        public static void WriteLog(string logFullName, string message, bool append, bool withTime) {
            try {
                if (withTime) {
                    message = string.Format("{0}[{1}]    {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), Thread.CurrentThread.ManagedThreadId.ToString(), message);
                }
                if (File.Exists(logFullName)) {
                    using (StreamWriter sw = new StreamWriter(logFullName, append)) {
                        sw.WriteLine(message);
                    }
                } else {
                    string path = logFullName.Substring(0, logFullName.LastIndexOf('\\'));
                    DirectoryInfo info = new DirectoryInfo(path);
                    if (!info.Exists)
                        info = Directory.CreateDirectory(path);
                    using (StreamWriter sw = File.CreateText(logFullName)) {
                        sw.WriteLine(message);
                    }
                }
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
