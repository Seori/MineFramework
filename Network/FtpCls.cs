using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MineFramework
{
    public class FtpCls
    {
        private static string lastErrorMessage = string.Empty;

        public static string lastErrMsg
        {
            get { return lastErrorMessage; }
            set { lastErrorMessage = value; }
        }

        // 생성자함수
        public FtpCls()
        {

        }

        #region 파일을 업로드합니다. 
        /// <summary>
        /// FTP서버에 파일을 업로드합니다.
        /// </summary>
        /// <param name="localPath">로컬경로</param>
        /// <param name="serverIp">FTP서버IP</param>
        /// <param name="ftpPath">FTP업로드경로</param>
        /// <param name="filename">파일명</param>
        /// <param name="ftpid">FTP ID</param>
        /// <param name="ftppw">FTP PW</param>
        /// <param name="usePassive">PassiveMode 사용여부</param>
        /// <returns></returns>
        public static int fnFileUpload(string localPath, string serverIp, string ftpPath, string filename, string ftpid, string ftppw, bool usePassive)
        {
            if (string.IsNullOrEmpty(localPath))
            {
                return -10;
            }

            if (string.IsNullOrEmpty(ftpPath))
            {
                return -20;
            }

            if (string.IsNullOrEmpty(filename))
            {
                return -30;
            }

            // 경로 정리
            ftpPath = ftpPath.Replace("\\", "/");

            string strFTPFullPath = string.Format("ftp://{0}{1}/{2}", serverIp, ftpPath, filename);
            FtpWebRequest ftpWeb = (FtpWebRequest)WebRequest.Create(strFTPFullPath);
            ftpWeb.Credentials = new NetworkCredential(ftpid, ftppw);
            ftpWeb.UseBinary = true;
            ftpWeb.Timeout = 60000;
            ftpWeb.Method = WebRequestMethods.Ftp.UploadFile;

            Stream stream = null;
            FileInfo uploadfile = new FileInfo(localPath);
            FileStream fs = uploadfile.OpenRead();

            try
            {
                byte[] buf = new byte[2048];
                int currentOffSet = 0;

                stream = ftpWeb.GetRequestStream();
                currentOffSet = fs.Read(buf, 0, 2048);
                while (currentOffSet != 0)
                {
                    stream.Write(buf, 0, currentOffSet);
                    currentOffSet = fs.Read(buf, 0, 2048);
                }
            } 
            catch(Exception e)
            {
                LogCls.writeLog("", "MineError.log", "FTP업로드가 실패하였습니다. " + filename + " " + e.Message);
                lastErrMsg = "FTP업로드가 실패하였습니다. " + filename + " " + e.Message;
                return -40;
            }
            finally
            {
                fs.Dispose();
                if (stream != null)
                {
                    stream.Dispose();
                }
            }

            return 0;
        }
        #endregion

        /// <summary>
        /// SFTP로 업로드 처리합니다.
        /// </summary>
        /// <param name="localPath">로컬경로</param>
        /// <param name="serverIp">FTP서버IP</param>
        /// <param name="ftpBasicPath">FTP기본경로</param>
        /// <param name="ftpFilePath">FTP경로</param>
        /// <param name="ftpFilename">FTP파일명</param>
        /// <param name="ftpid">FTP ID</param>
        /// <param name="ftppw">FTP PW</param>
        /// <returns></returns>
        public int fnSFTPUpload(string localPath, string serverIp, string ftpBasicPath, string ftpFilePath, string ftpFilename, string ftpid, string ftppw)
        {
            if (string.IsNullOrEmpty(localPath))
            {
                return -10;
            }

            if (string.IsNullOrEmpty(ftpFilename))
            {
                return -30;
            }

            // 경로 정리
            ftpFilePath = ftpFilePath.Replace("\\", "/");

            /*
            using (SftpClient client = new SftpClient(serverIp, 22, ftpid, ftppw))
            {
                client.KeepAliveInterval = TimeSpan.FromSeconds(60);
                client.ConnectionInfo.Timeout = TimeSpan.FromMinutes(180);
                client.OperationTimeout = TimeSpan.FromMinutes(180);
                client.Connect();
                bool connected = client.IsConnected;

                string remoteDir = ftpBasicPath + ftpFilePath;
                addLog("remoteDir" + remoteDir);

                client.ChangeDirectory(remoteDir);
                addLog("Working Directory : " + client.WorkingDirectory);

                UploadDirectory(client, localPath, remoteDir);

                client.Disconnect();
            }
            */

            return 0;
        }

        #region FTP파일을 다운로드 합니다.
        /// <summary>
        /// FTP파일을 다운로드 합니다.
        /// </summary>
        /// <param name="serverIp">FTP서버IP</param>
        /// <param name="ftpBasicPath">FTP기본경로</param>
        /// <param name="ftpFilePath">FTP경로</param>
        /// <param name="ftpFilename">FTP다운로드파일명</param>
        /// <param name="ftpid">FTP ID</param>
        /// <param name="ftppw">FTP PW</param>
        /// <param name="backuppath">백업할 경로</param>
        /// <returns>
        /// 0 : 성공, -11 : 파일없음
        /// </returns>
        public int fnBackupDownload(string serverIp, string ftpBasicPath, string ftpFilePath, string ftpFilename, string ftpid, string ftppw, string backuppath)
        {
            try
            {
                string strFTPFullPath = string.Format("ftp://{0}{1}{2}/{3}", serverIp, ftpBasicPath, ftpFilePath, ftpFilename);
                FtpWebRequest ftpWeb = (FtpWebRequest)WebRequest.Create(strFTPFullPath);
                ftpWeb.Credentials = new NetworkCredential(ftpid, ftppw);
                
                /*
                if (usePassive)
                {
                    ftpWeb.UsePassive = true;
                }
                */

                ftpWeb.UseBinary = true;
                ftpWeb.Timeout = 60000;
                ftpWeb.Method = WebRequestMethods.Ftp.DownloadFile;

                // RollBack 파일
                string rolldata = backuppath + ftpFilePath + "\\" + ftpFilename;

                using (FtpWebResponse resp = (FtpWebResponse)ftpWeb.GetResponse())
                using (Stream respStream = resp.GetResponseStream())
                using (FileStream fileStream = new FileStream(rolldata, FileMode.Create))
                {
                    respStream.CopyTo(fileStream);
                }
            }
            catch (Exception ex)
            {
                // 파일없는경우
                return -11;
            }

            return 0;
        }
        #endregion
    }
}