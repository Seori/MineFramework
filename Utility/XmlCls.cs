using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace MineFramework
{
    public class XmlCls
    {
        /// <summary>
        /// DataTable을 받아서 기본적인 형태의 XML String을 반환한다.
        /// </summary>
        /// <param name="rootName">루트명</param>
        /// <param name="_oneDt">데이터테이블</param>
        /// <param name="isSchemea">스키마출력여부(True or False)</param>
        /// <returns>XML String</returns>
        public static string fnDtToXML(string rootName, DataTable _oneDt, bool isSchemea)
        {
            DataSet ds = new DataSet(rootName);
            DataTable _temp = _oneDt.Copy();
            _temp.TableName = rootName;

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            ds.Tables.Add(_temp);

            if (isSchemea)
            {
                ds.WriteXml(sw, XmlWriteMode.WriteSchema);
            }
            else
            {
                ds.WriteXml(sw);
            }

            sw.Flush();

            return sb.ToString();
        }

        /// <summary>
        /// DataTable을 받아서 XML String으로 변환하여 반환합니다.
        /// </summary>
        /// <param name="_oneDt">데이터테이블</param>
        /// <param name="_rootName">Root명</param>
        /// <returns>XML String</returns>
        public static string fnDtToXMLReal(DataTable _oneDt, string _rootName)
        {
            // string으로 변환하기 위하여 선언
            StringWriter sw = new StringWriter();
            XmlTextWriter xmlTw = new XmlTextWriter(sw);

            XmlDocument xd = new XmlDocument();

            // Version, Encoding정보 셋팅
            XmlDeclaration xmldec;
            xmldec = xd.CreateXmlDeclaration("1.0", "utf-8", null);

            // Root 엘리먼트 생성
            XmlElement root = xd.DocumentElement;
            xd.InsertBefore(xmldec, root);

            // 1st Node
            XmlElement xe = xd.CreateElement(_rootName);

            foreach (DataRow dr in _oneDt.Rows)
            {
                // 2nd Node (Row단위 생성)
                XmlElement xe_t = xd.CreateElement(_oneDt.TableName);
                foreach (DataColumn dc in _oneDt.Columns)
                {
                    // 3rd Node (Column단위 생성)
                    XmlElement xe3 = xd.CreateElement(dc.ColumnName);
                    xe3.InnerText = dr[dc].ToString();
                    xe_t.AppendChild(xe3);
                }

                xe.AppendChild(xe_t);
            }

            xd.AppendChild(xe);
            xd.WriteTo(xmlTw);

            return sw.ToString();            
        }

        /// <summary>
        /// DataTable을 전달받아 XML 형태로 파일로 저장합니다.
        /// </summary>
        /// <param name="_oneDt">데이터테이블</param>
        /// <param name="_rootName">Root명</param>
        /// <param name="_savepath">저장경로</param>
        /// <param name="_savefile">저장파일명</param>
        public static void fnDtToXMLToFile(DataTable _oneDt, string _rootName, string _savepath, string _savefile)
        {
            // 저장/불러오기 경로 지정
            string savepath = _savepath + _savefile;

            try
            {
                // 문서를 읽는다.
                if (FileCls.chkFile(savepath))
                {
                    // 수정처리
                    fnUpdateXMLFile(_oneDt, savepath, savepath);
                }
                else
                {
                    // 신규이므로 
                    fnMakeXMLFile(_oneDt, _rootName, _savepath, _savefile);
                }
            }
            catch(Exception e)
            {
                // 문서가 위치에 없다.
                LogCls.writeLog("", "MineError.log", "fnDtToXMLToFile Error" + e.Message);
            }
        }

        /// <summary>
        /// Json String을 전달받아 XML파일로 저장합니다.
        /// </summary>
        /// <param name="_jsonstr">Json String</param>
        /// <param name="_rootName">Root명</param>
        /// <param name="_savepath">저장경로</param>
        /// <param name="_savefile">저장파일명</param>
        public static void fnJsonToXMLToFile(string _jsonstr, string _rootName, string _savepath, string _savefile)
        {
            // 저장/불러오기 경로 지정
            string savepath = _savepath + _savefile;

            // Json을 DataTable형태로 변환
            DataTable jsonDt = JsonConvert.DeserializeObject<DataTable>(_jsonstr);

            try
            {
                // 문서를 읽는다.
                if (FileCls.chkFile(savepath))
                {
                    // 기존파일 추가처리
                    fnUpdateXMLFile(jsonDt, savepath, savepath);
                }
                else
                {
                    // 신규이므로 
                    fnMakeXMLFile(jsonDt, _rootName, savepath, _savefile);
                }
            }
            catch (Exception e)
            {
                // 문서가 위치에 없다.
                LogCls.writeLog("", "MineError.log", "fnJsonToXMLToFile Error" + e.Message);
            }
        }

        /// <summary>
        /// 기존XML에 하위 경로로 데이터를 추가합니다.
        /// </summary>
        /// <param name="_addDt">추가할 DataTable</param>
        /// <param name="_xmlPath">기존XML경로</param>
        /// <param name="_savePath">저장할XML경로</param>
        public static void fnUpdateXMLFile(DataTable _addDt, string _xmlPath, string _savePath)
        {
            XmlDocument xd = new XmlDocument();
            xd.Load(_xmlPath);

            // 첫노드를 잡아준다.
            XmlNode firstNode = xd.DocumentElement;

            foreach (DataRow dr in _addDt.Rows)
            {
                // 2nd Node (Row단위 생성)
                if (string.IsNullOrEmpty(_addDt.TableName)) { _addDt.TableName = "COMMON"; }
                XmlElement xe_t = xd.CreateElement(_addDt.TableName);
                foreach (DataColumn dc in _addDt.Columns)
                {
                    // 3rd Node (Column단위 생성)
                    XmlElement xe3 = xd.CreateElement(dc.ColumnName);
                    xe3.InnerText = dr[dc].ToString();
                    xe_t.AppendChild(xe3);
                }

                firstNode.AppendChild(xe_t);
            }

            xd.AppendChild(firstNode);
            xd.Save(_savePath);
        }

        /// <summary>
        /// 신규 XML파일을 생성하여 저장한다.
        /// </summary>
        /// <param name="_oneDt">데이터</param>
        /// <param name="_rootName">루트명</param>
        /// <param name="_savePath">저장경로</param>
        public static void fnMakeXMLFile(DataTable _oneDt, string _rootName, string _savePath, string _saveFile)
        {
            // 해당경로가 있는지 체크하고 없으면 생성한다.
            FileCls.makeFd(_savePath);

            XmlDocument xd = new XmlDocument();

            // Version, Encoding정보 셋팅
            XmlDeclaration xmldec;
            xmldec = xd.CreateXmlDeclaration("1.0", "utf-8", null);

            // Root 엘리먼트 생성
            XmlElement root = xd.DocumentElement;
            xd.InsertBefore(xmldec, root);

            // 1st Node
            XmlElement xe = xd.CreateElement(_rootName);

            foreach (DataRow dr in _oneDt.Rows)
            {
                // 2nd Node (Row단위 생성)
                if (string.IsNullOrEmpty(_oneDt.TableName)) { _oneDt.TableName = "COMMON"; }
                XmlElement xe_t = xd.CreateElement(_oneDt.TableName);
                foreach (DataColumn dc in _oneDt.Columns)
                {
                    // 3rd Node (Column단위 생성)
                    XmlElement xe3 = xd.CreateElement(dc.ColumnName);
                    xe3.InnerText = dr[dc].ToString();
                    xe_t.AppendChild(xe3);
                }

                xe.AppendChild(xe_t);
            }

            xd.AppendChild(xe);
            xd.Save(_savePath + _saveFile);
        }

        /// <summary>
        /// XML경로에 있는 XML파일을 읽어 DataTable형태로 반환합니다.
        /// </summary>
        /// <param name="xmlpath">XML위치경로</param>
        /// <returns></returns>
        public static DataTable fnXMLFileToDt(string xmlpath)
        {
            DataTable _tmpDt = new DataTable();

            // 문서를 읽는다.
            if (FileCls.chkFile(xmlpath))
            {
                // XML을 읽는다.
                DataSet ds = new DataSet();
                ds.ReadXml(xmlpath);

                foreach (DataTable sdt in ds.Tables)
                {
                    if (_tmpDt.Rows.Count > 0)
                    {
                        _tmpDt.Merge(sdt);
                    }
                    else
                    {
                        _tmpDt = sdt;
                    }                    
                }
            }

            return _tmpDt;
        }
    }
}
