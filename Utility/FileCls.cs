using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Windows.Forms;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualBasic.FileIO;

namespace MineFramework
{
    public static class FileCls
    {
        /// <summary>
        /// 폴더를 컴퓨터내에 생성합니다.
        /// </summary>
        public static void makeFd(string _path)
        {
            DirectoryInfo di = new DirectoryInfo(_path);

            try
            {                
                if (!di.Exists)
                {
                    di.Create();
                }
            }
            catch(Exception ex)
            {
                LogCls.writeLog("", "MineFrameworkLog.log", "폴더생성중 오류발생 : " + ex.Message);
            }
            finally
            {
                
            }            
        }

        /// <summary>
        /// 폴더를 컴퓨터에서 삭제합니다. (폴더내에 파일이 있더라도 강제삭제합니다)
        /// </summary>
        /// <param name="_path"></param>
        public static void removeFd(string _path)
        {
            DirectoryInfo di = new DirectoryInfo(_path);
            if (di.Exists)
            {
                di.Delete();
            }
        }

        /// <summary>
        /// 폴더자체를 복사합니다.
        /// </summary>
        /// <param name="_oripath">오리지널 경로</param>
        /// <param name="_targetpath">타겟 경로</param>
        /// <returns>
        /// 0 : 정상복사 완료
        /// -71 : 복사될경로가 없는경우
        /// -72 : 복사할경로가 없는경우
        /// -79 : Exception 발생시
        /// </returns>
        public static int copyFd(string _oripath, string _targetpath)
        {
            // 오리지널 경로가 있는지부터 판단
            DirectoryInfo di = new DirectoryInfo(_oripath);
            if (!di.Exists)
            {
                return -71;
            }

            // 타겟 경로가 있는지부터 판단
            di = new DirectoryInfo(_targetpath);
            if (!di.Exists)
            {
                return -72;
            }

            try
            {
                FileSystem.CopyDirectory(_oripath, _targetpath, UIOption.AllDialogs);

                return 0;
            }
            catch(Exception ex)
            {
                LogCls.writeLog("", "MineFrameworkLog.log", "Copy Folder Error (" + ex.Message + ")");
                return -79;
            }            

            //string[] files = Directory.GetFiles(_oripath);
            //string[] folders = Directory.GetDirectories(_oripath);

            //string filename = string.Empty;
            //string destname = string.Empty;

            //// 파일부터 복사
            //foreach (string file in files)
            //{
            //    filename = Path.GetFileName(file);
            //    destname = Path.Combine(_targetpath, filename);
            //    File.Copy(filename, destname);
            //}

            //string orifolder = string.Empty;
            //string tarfolder = string.Empty;

            //// 폴더 복사
            //foreach (string folder in folders)
            //{
            //    orifolder = Path.GetFileName(folder);
            //    tarfolder = Path.Combine(_targetpath, orifolder);
                
            //}
        }

        /// <summary>
        /// 폴더자체를 이동합니다.
        /// </summary>
        /// <param name="_oripath">오리지널 경로</param>
        /// <param name="_targetpath">타겟 경로</param>
        /// <returns>
        /// 0 : 정상복사 완료
        /// -71 : 복사될경로가 없는경우
        /// -72 : 복사할경로가 없는경우
        /// -79 : Exception 발생시
        /// </returns>
        public static int moveFd(string _oripath, string _targetpath)
        {
            // 오리지널 경로가 있는지부터 판단
            DirectoryInfo di = new DirectoryInfo(_oripath);
            if (!di.Exists)
            {
                return -71;
            }

            // 타겟 경로가 있는지부터 판단
            di = new DirectoryInfo(_targetpath);
            if (!di.Exists)
            {
                return -72;
            }

            try
            {
                FileSystem.MoveDirectory(_oripath, _targetpath, UIOption.AllDialogs);

                return 0;
            }
            catch (Exception ex)
            {
                LogCls.writeLog("", "MineFrameworkLog.log", "Move Folder Error (" + ex.Message + ")");
                return -79;
            }
        }

        /// <summary>
        /// 파일이 존재하는지 체크합니다.
        /// </summary>
        /// <param name="_path">파일을 포함한 풀경로</param>
        /// <returns>파일이 존재하면 true, 존재하지 않으면 false</returns>
        public static bool chkFile(string _path)
        {
            FileInfo fi = new FileInfo(_path);
            if (!fi.Exists)
            {
                return false;
            }
            else
            {
                return true;
            }
        }        

        /// <summary>
        /// 파일을 생성합니다.
        /// </summary>
        /// <param name="_path">생성할 경로정보</param>
        /// <param name="_filename">생성할 파일명</param>
        /// <returns>-99 : 폴더경로없음</returns>
        /// <returns>-98 : 파일생성오류</returns>
        /// <returns>0 : 파일생성완료</returns>
        public static int makeFile(string _path, string _filename)
        {
            string filestr = _path + "/" + _filename;

            // 경로가 있는지부터 판단
            DirectoryInfo di = new DirectoryInfo(_path);
            if (!di.Exists)
            {
                return -99;
            }
            
            FileInfo fi = new FileInfo(filestr);
            try
            {
                fi.Create();

                return 0;
            }
            catch (Exception fiex)
            {
                LogCls.writeLog("", "MineFrameworkLog.log", "File Create Error (" + fiex.Message + ")");
                return -98;
            }
            finally
            {
                
            }
        }

        /// <summary>
        /// string 형태의 데이터를 전달받아 파일을 생성합니다.
        /// </summary>
        /// <param name="_savestr">저장할 string</param>
        /// <param name="_savefilepath">생성할 경로정보</param>
        /// <param name="_savefilename">생성할 파일명</param>
        /// <returns>-99 : 폴더경로없음</returns>
        /// <returns>-98 : 파일생성오류</returns>
        /// <returns>0 : 파일생성완료</returns>
        public static int makeFileText(string _savestr, string _savefilepath, string _savefilename)
        {
            string filestr = _savefilepath + "/" + _savefilename;

            // 경로가 있는지부터 판단
            DirectoryInfo di = new DirectoryInfo(_savefilepath);
            if (!di.Exists)
            {
                return -99;
            }

            try
            {
                // string을 파일로 씁니다.
                File.WriteAllText(filestr, _savestr);

                return 0;
            }
            catch(Exception ex)
            {
                LogCls.writeLog("", "MineFrameworkLog.log", "TextFile Create Error (" + ex.Message + ")");
                return -98;
            }
        }

        /// <summary>
        /// 파일을 삭제합니다.
        /// </summary>
        /// <param name="_path">삭제할 경로정보</param>
        /// <param name="_filename">삭제할 파일명</param>
        /// <returns>-99 : 폴더경로없음</returns>
        /// <returns>-98 : 파일생성오류</returns>
        /// <returns>0 : 파일생성완료</returns>
        public static int removeFile(string _path, string _filename)
        {
            string filestr = _path + "/" + _filename;

            // 경로가 있는지부터 판단
            DirectoryInfo di = new DirectoryInfo(_path);
            if (!di.Exists)
            {
                return -99;
            }

            try
            {
                FileInfo fi = new FileInfo(filestr);
                fi.Delete();

                return 0;
            }
            catch (Exception fiex)
            {
                LogCls.writeLog("", "MineFrameworkLog.log", "File Remove Error (" + fiex.Message + ")");
                return -98;
            }
        }

        /// <summary>
        /// 파일을 대상경로로 복사합니다.
        /// </summary>
        /// <param name="_oripath">원래경로(ex:@"C:\Users\Public\TestFolder")</param>
        /// <param name="_targetpath">대상경로</param>
        /// <param name="_filename">파일명</param>
        /// <param name="_overwrite">덮어쓰기여부</param>
        /// <returns></returns>
        public static int copyFile(string _oripath, string _targetpath, string _filename, bool _overwrite)
        {
            try
            {
                // 원래경로 셋팅
                string oripath = Path.Combine(_oripath + _filename);

                // 대상경로 셋팅
                string tarpath = Path.Combine(_targetpath + _filename);

                // 파일복사
                File.Copy(oripath, tarpath, _overwrite);

                return 0;
            }
            catch(Exception ex)
            {
                LogCls.writeLog("", "MineFrameworkLog.log", "File Copy Error (" + ex.Message + ")");
                return -98;
            }            
        }

        /// <summary>
        /// 파일을 대상경로로 이동합니다.
        /// </summary>
        /// <param name="_oripath"></param>
        /// <param name="_targetpath"></param>
        /// <param name="_filename"></param>
        /// <param name="_overwrite"></param>
        /// <returns></returns>
        public static int moveFile(string _oripath, string _targetpath, string _filename)
        {
            try
            {
                // 원래경로 셋팅
                string oripath = Path.Combine(_oripath + _filename);

                // 대상경로 셋팅
                string tarpath = Path.Combine(_targetpath + _filename);

                // 파일이동
                File.Move(oripath, tarpath);

                return 0;
            }
            catch (Exception ex)
            {
                LogCls.writeLog("", "MineFrameworkLog.log", "File Move Error (" + ex.Message + ")");
                return -98;
            }
        }

        /// <summary>
        /// 파일을 삭제합니다.
        /// </summary>
        /// <param name="_targetpath">삭제할 파일경로</param>
        /// <param name="_filename">삭제할파일명</param>
        /// <returns></returns>
        public static int delFile(string _targetpath, string _filename)
        {
            try
            {
                // 삭제할경로 및 파일명셋팅
                string delpath = _targetpath + _filename;

                // 파일삭제
                File.Delete(delpath);

                return 0;
            }
            catch(Exception ex)
            {
                LogCls.writeLog("", "MineFrameworkLog.log", "File Delete Error (" + ex.Message + ")");
                return -98;
            }
        }

        /// <summary>
        /// 파일을 삭제합니다.
        /// </summary>
        /// <param name="_targetpath">삭제할 파일경로</param>
        /// <param name="_filename">삭제할파일명</param>
        /// <returns></returns>
        public static int delFile(string _targetfullpath)
        {
            try
            {
                // 파일삭제
                File.Delete(_targetfullpath);

                return 0;
            }
            catch (Exception ex)
            {
                LogCls.writeLog("", "MineFrameworkLog.log", "File Delete Error (" + ex.Message + ")");
                return -98;
            }
        }

        /// <summary>
        /// 인증서 경로를 조회하여 데이터를 가져옵니다.
        /// </summary>
        /// <returns></returns>
        public static DataTable getCertInfo()
        {
            DataTable dtCertList = new DataTable();
            dtCertList.Columns.Add("Directory1");
            dtCertList.Columns.Add("Directory2");
            dtCertList.Columns.Add("Directory3");
            dtCertList.Columns.Add("FileName");
            dtCertList.Columns.Add("FileExt");
            dtCertList.Columns.Add("CertSerialKey");
            dtCertList.Columns.Add("CertSerialKey2");

            // 계정 Directory
            string strspecial = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString();
            
            // 인증서 경로 만들기
            string strpath = Directory.GetParent(strspecial).ToString();

            // 인증서 경로 (C:\Program Files\NPKI\, C:\Program Files (x86)\NPKI\)
            string strsub = "\\LocalLow\\NPKI\\";

            string fullpath = strpath + strsub;

            DirectoryInfo dirInfo = new DirectoryInfo(fullpath);
            foreach (var item in dirInfo.GetDirectories())
            {
                DirectoryInfo dirInfo2 = new DirectoryInfo(fullpath + "\\" + item.Name);
                foreach (var item2 in dirInfo2.GetDirectories())
                {
                    DirectoryInfo dirInfo3 = new DirectoryInfo(fullpath + "\\" + item.Name + "\\" + item2.Name);
                    foreach (var item3 in dirInfo3.GetDirectories())
                    {                        
                        DirectoryInfo dirInfo4 = new DirectoryInfo(fullpath + "\\" + item.Name + "\\" + item2.Name + "\\" + item3.Name);
                        foreach (FileInfo fi in dirInfo4.GetFiles())
                        {
                            if (fi.Extension == ".der")
                            {
                                DataRow dr = dtCertList.Rows.Add();
                                dr["Directory1"] = item.Name.ToString();
                                dr["Directory2"] = item2.Name.ToString();
                                dr["Directory3"] = item3.Name.ToString();
                                dr["FileName"] = fi.FullName;
                                dr["FileExt"] = fi.Extension;                               

                                byte[] bytes = File.ReadAllBytes(fi.FullName);
                                X509Certificate2 certinfo = new X509Certificate2(bytes);

                                // 일련번호
                                dr["CertSerialKey"] = certinfo.GetSerialNumberString();
                                dr["CertSerialKey2"] = certinfo.Subject;

                                // 인증서명 certinfo.Subject (CN=(주)핑거(FINGER)0020025201112072703630, OU=FINGER, OU=WOORI, OU=corporation, O=yessign, C=kr)
                                // 인증서발행 certinfo.Issuer (CN=yessignCA Class 2, OU=AccreditedCA, O=yessign, C=kr)
                                // 시작일자 certinfo.NotBefore (2018-12-11 오전 12:00:00)
                                // 종료일자 certinfo.NotAfter (2019-12-14 오전 11:59:59)

                            }
                        }
                    }
                }
            }

            foreach (DriveInfo drvInfo in DriveInfo.GetDrives())
            {
                if (drvInfo.DriveType == DriveType.Removable)
                {
                    // 이동식 디스크이면..

                    // 드라이브 경로값을 반환하고 있음.
                    string diskname = drvInfo.Name;
                }

            }

            return dtCertList;
        }

        /// <summary>
        /// 파일의 경로 및 파일명을 Return한다.
        /// Return string[] 0 : 파일 Full경로 + 파일명, 1 : 파일명, 2 : 파일 Full경로
        /// </summary>
        /// <param name="fileFilter">필터값을 셋팅한다.</param>
        /// 필터예제 : 모든파일 (*.*) | *.*
        /// <returns>        /// 
        /// string[] 배열형태로 파일정보를 Return한다.
        /// 0 : 파일 Full경로 + 파일명, 1 : 파일명, 2 : 파일 Full경로
        /// </returns>
        public static string[] openSelectFiles(string fileFilter)
        {
            string[] _fileinfo = null;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = fileFilter;
            DialogResult dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                _fileinfo = new string[3];

                // 파일 Full경로 + 파일명
                _fileinfo[0] = ofd.FileName;
                // 파일명
                _fileinfo[1] = ofd.SafeFileName;
                // 파일 Full경로
                _fileinfo[2] = ofd.FileName.Replace(ofd.SafeFileName, "");

                return _fileinfo;
            }
            else
            {
                return _fileinfo;
            }            
        }

        /// <summary>
        /// 경로선택 팝업을 띄어서 경로를 선택합니다.
        /// </summary>
        /// <returns></returns>
        public static string openFolderPath(string _defaultPath)
        {
            string _filepath = string.Empty;

            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if (!string.IsNullOrEmpty(_defaultPath))
            {
                // 넘어온경로가 있으면 미리 설정한다.
                fbd.SelectedPath = _defaultPath;
            }            

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                // 폴더경로
                _filepath = fbd.SelectedPath;

                return _filepath;
            }
            else
            {
                return _filepath;
            }
        }

        /// <summary>
        /// 해당폴더에서 해당하는 확장자의 파일리스트(경로포함)를 가져옵니다.
        /// </summary>
        /// <param name="_ext">확장자</param>
        /// <param name="_folderName"></param>
        /// <param name="fileList"></param>
        /// <returns></returns>
        public static ArrayList getFileList(string _dirPath, ArrayList fileList, string _mode)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(_dirPath);

                string strFile = string.Empty;
                string strFileName = string.Empty;

                string[] strFileFull;
                foreach (FileInfo f in di.GetFiles())
                {
                    if (_mode == "ALL")
                    {
                        // 전체경로 및 파일명 포함
                        strFile = di.FullName + "\\" + f.Name;
                        strFileName = f.Name;

                        strFileFull = new string[] { strFile, strFileName };
                        fileList.Add(strFileFull);
                    }
                    else if (_mode == "PATH")
                    {
                        // 전체경로포함
                        strFile = di.FullName + "\\" + f.Name;
                        fileList.Add(strFile);
                    }
                    else
                    {
                        // 파일명만
                        strFile = f.Name;
                        fileList.Add(strFile);
                    }
                }

                foreach (DirectoryInfo sd in di.GetDirectories())
                {
                    getFileList(sd.FullName, fileList, _mode);
                }

            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {

            }

            return fileList;
        }

        /// <summary>
        /// 해당폴더에서 해당하는 확장자의 파일리스트를 가져옵니다.
        /// </summary>
        /// <param name="_ext">확장자</param>
        /// <param name="_folderName"></param>
        /// <param name="fileList"></param>
        /// <returns></returns>
        public static ArrayList getFileList(string _ext, string _dirPath, ArrayList fileList)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(_dirPath);

                if (!_ext.StartsWith("."))
                {
                    _ext = "." + _ext;
                }

                foreach (FileInfo f in di.GetFiles())
                {
                    if (f.Extension.ToLower().CompareTo(_ext) == 0)
                    {
                        string strFile = di.FullName + "\\" + f.Name;
                        fileList.Add(strFile);
                    }
                }

                foreach (DirectoryInfo sd in di.GetDirectories())
                {
                    getFileList(_ext, sd.FullName, fileList);                
                }
                
            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
                
            }

            return fileList;
        }

        /// <summary>
        /// 파라미터로 넘어온경로에 파일이 있는지 확인하고 DataTable을 Json으로 전환하여
        /// 해당경로에 파일을 생성하거나 기존파일의 내용을 교체합니다.
        /// </summary>
        /// <param name="jsonDt"></param>
        /// <param name="filepath"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static int fnJsonFileMakeOrUpdate(DataTable jsonDt, string filepath, string filename)
        {
            // 경로 설정
            var TextFilePath = filepath + filename;

            // 경로가 있는지부터 판단
            DirectoryInfo di = new DirectoryInfo(filepath);
            if (!di.Exists)
            {
                return -99;
            }

            try
            {
                // 기존파일이 있는지부터 체크함.
                if (chkFile(TextFilePath))
                {
                    // 기존파일이 있다면 Update모드
                    using (var writer = new StreamWriter(TextFilePath))
                    {
                        // 기존내용은 읽지않고 덮어쓰기 모드로 작성한다.
                        string jsonstr = StringCls.fnDataTableToJson2(jsonDt);

                        // 내용을 다시씁니다.
                        writer.Write(jsonstr);

                        // 내용을 다시 썼으면 StreamWriter를 정리합니다.
                        writer.Close();
                        writer.Dispose();

                        return 0;
                    }
                }
                else
                {
                    // 기존파일이 없다면 Make모드
                    StringCls.fnMakeJsonFile(jsonDt, filepath, filename);

                    return 0;
                }
            }
            catch(Exception e)
            {
                LogCls.writeLog("", "MineFrameworkLog.log", "fnJsonFileMakeOrUpdate Error (" + e.Message + ")");
                return -98;
            }
                      
        }
    }
}
