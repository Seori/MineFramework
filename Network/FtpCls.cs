using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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

        public static int fnFileUpload(string localPath, string serverIp, string ftpPath, string filename, string ftpid, string ftppw)
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

            string strFTPFullPath = string.Format("ftp://{0}{1}/{2}", serverIp, ftpPath, filename);
            FtpWebRequest ftpWeb = (FtpWebRequest)WebRequest.Create(strFTPFullPath);
            ftpWeb.Credentials = new NetworkCredential(ftpid, ftppw);
            //ftpWeb.KeepAlive = false;
            //ftpWeb.Method = WebRequestMethods.Ftp.ListDirectory;
            ftpWeb.UsePassive = false;
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
    }
}
