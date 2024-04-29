using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace MineFramework
{
    public class LogCls
    {
        // MineFramework 기본경로 설정
        public static string LogStr = @"C:\Mines\Log";

        public LogCls()
        {

        }

        /// <summary>
        /// 로그를 파일에 씁니다.
        /// </summary>
        /// <param name="_path">파일경로를 셋팅합니다.(경로가 없을시 기본경로 셋팅)</param>
        /// <param name="_filename">파일명을 셋팅합니다.</param>
        /// <param name="_msg">적을 메세지</param>
        public static void writeLog(string _path, string _filename, string _msg)
        {
            string logpath = string.Empty;

            if (string.IsNullOrEmpty(_filename))
            {
                return;
            }
            
            if (string.IsNullOrEmpty(_path))
            {
                logpath = LogStr;
            }
            else
            {
                logpath = _path;
            }

            // 실제 로그 저장위치
            string writepath = logpath + @"\" + _filename;

            try
            {
                // 실제경로와 파일이 생성되었는지 체크합니다.
                FileCls.makeFd(logpath);
                //FileCls.makeFile(logpath, _filename);

                // 로그를 저장합니다.
                using (StreamWriter sw = new StreamWriter(writepath, true))
                {
                    string _fullmsg = DateTime.Now + " | " + _msg;
                    sw.WriteLine(_fullmsg);

                    //쓰기를 완료했으면 닫아준다.
                    sw.Close();
                    sw.Dispose();
                }
            }
            catch(Exception ex)
            {
                Trace.WriteLine("LogCls.writeLog Error : " + ex.Message);
            }
        }
    }
}
