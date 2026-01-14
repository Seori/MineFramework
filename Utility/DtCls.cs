using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MineFramework
{
    public class DtCls
    {
        /// <summary>
        /// Source DataTable과 대상컬럼, 대상값을 받아서 해당값을 삭제후 Return합니다.
        /// </summary>
        /// <param name="sourceDt">소스 DataTable</param>
        /// <param name="targetColumn">대상컬럼</param>
        /// <param name="targetVal">대상값</param>
        /// <returns>returnDt</returns>
        /// <returns>error case return null</returns>
        public static DataTable RmDtRow(DataTable sourceDt, string targetColumn, string targetVal)
        {
            if (sourceDt.Rows.Count < 1)
            {
                return null;
            }

            DataTable returnDt;
            returnDt = sourceDt.Copy();

            for (int i = 0; i < returnDt.Rows.Count; i++)
            {
                DataRow deldr = returnDt.Rows[i];
                if (deldr[targetColumn].ToString() == targetVal)
                {
                    deldr.Delete();
                }
            }

            returnDt.AcceptChanges();

            return returnDt;
        }

        /// <summary>
        /// Source DataTable과 대상컬럼, 대상값의 배열을 받아서 해당값을 삭제후 Return합니다.
        /// </summary>
        /// <param name="sourceDt">소스 DataTable</param>
        /// <param name="targetColumn">삭제할 컬럼</param>
        /// <param name="targetVals">삭제할 값의 배열</param>
        /// <returns>returnDt</returns>
        /// <returns>error case return null</returns>
        public static DataTable RmDtRow(DataTable sourceDt, string targetColumn, string[] targetVals)
        {
            if (sourceDt.Rows.Count < 1)
            {
                return null;
            }

            DataTable returnDt;
            returnDt = sourceDt.Copy();

            for (int i = 0; i < returnDt.Rows.Count; i++)
            {
                DataRow deldr = returnDt.Rows[i];

                for (int j = 0; j < targetVals.Length; j++)
                {
                    if (deldr[targetColumn].ToString() == targetVals[j].ToString())
                    {
                        deldr.Delete();
                    }
                }                
            }

            returnDt.AcceptChanges();

            return returnDt;
        }

        /// <summary>
        /// 2차원배열을 받아서 DataTable형태로 만들어 반환합니다.
        /// </summary>
        /// <param name="oSource">2차원 배열</param>
        /// <sample>string[][] arrsource = { new string[] {"AA", "BB"}, new string[] {"CC", "DD"} };</sample>
        /// <returns>DataTable (key, value)</returns>
        public static DataTable ArrayToDataTable(string[][] oSource)
        {
            DataTable tempDt = new DataTable();

            tempDt.Columns.Add("key");
            tempDt.Columns.Add("value");
            tempDt.BeginLoadData();

            for (int i = 0; i < oSource.GetLength(0); i++)
            {
                tempDt.LoadDataRow(oSource[i], true);
            }

            tempDt.EndLoadData();

            return tempDt;
        }

        /// <summary>
        /// 2차원배열을 넘겨받아 DataTable형태로 Return합니다.
        /// </summary>
        /// <param name="dsource">데이터관련 배열</param>
        /// <returns></returns>
        public static DataTable TwoArrayToDataTable(string[][] dsource)
        {
            DataTable tempDt = new DataTable();

            string[][] headersource =
            {
                new string[] { "header_name", "resizable_yn", "text_align", "data_name", "width_size", "sort_mode", "col_type" }
            };

            for (int a = 0; a < headersource.Length; a++)
            {
                for (int j = 0; j < headersource[a].Length; j++)
                {
                    tempDt.Columns.Add(headersource[a][j].ToString());
                }                
            }

            for (int i = 0; i < dsource.Length; i++)
            {
                DataRow dr = tempDt.NewRow();
                for (int j = 0; j < dsource[i].Length; j++)
                {
                    dr[j] = dsource[i][j].ToString();
                }
                tempDt.Rows.Add(dr);
            }

            return tempDt;
        }

        /// <summary>
        /// 1차원 배열을 받아서 DataTable형태로 만들어 반환합니다.(같은값을 셋팅합니다.)
        /// </summary>
        /// <param name="oSource"></param>
        /// <returns></returns>
        public static DataTable ArrayToDataTable(string[] oSource)
        {
            DataTable tempDt = new DataTable();

            tempDt.Columns.Add("key");
            tempDt.Columns.Add("value");

            for (int i = 0; i < oSource.GetLength(0); i++)
            {
                DataRow dr = tempDt.NewRow();
                dr["key"] = oSource.GetValue(i);
                dr["value"] = oSource.GetValue(i);
                tempDt.Rows.Add(dr);
            }

            return tempDt;
        }

        /// <summary>
        /// DataTable에 컬럼을 추가합니다.
        /// 디폴트 값으로 생성되며, 컬럼이 존재하면 디폴트 값으로 해당 컬럼
        /// 값을 변경합니다.
        /// </summary>
        /// <param name="dt">DataTable 객체</param>
        /// <param name="szCol">생성할 컬럼명</param>
        /// <param name="szDefaultValue">컬럼의 디폴트 값</param>
        /// <returns>0 : 성공, !0 : 실패</returns>
        public static int DataTable_AddColumn(DataTable dt, string szCol, string szDefaultValue)
        {
            try
            {
                DataColumn dc = null;
                Type typeColumn;

                //--------------------------------------------------------------

                // 기본 검증
                if (dt == null)
                    return -1;

                // 기본 형식 : 문자열
                typeColumn = Type.GetType("System.String");

                // 컬럼유무 확인
                if (dt.Columns.Contains(szCol))
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        // 기본 값으로 변경
                        row[szCol] = szDefaultValue;
                    }
                    return 0;
                }

                // 컬럼 생성
                dc = new DataColumn();

                dc.DataType = typeColumn;
                dc.ColumnName = szCol;
                dc.DefaultValue = szDefaultValue;
                dt.Columns.Add(dc);

                return 0;
            }
            catch (Exception ex)
            {
                // 예외 오류
                Trace.WriteLine("DataTable_AddColumn Error : " + ex.ToString());

                return -91;
            }
        }

        /// <summary>
        /// DataTable을 구성합니다.
        /// 기본적으로 문자열 형식으로 주어진 컬럼들을 생성합니다.
        /// </summary>
        /// <param name="dt">DataTable 객체</param>
        /// <param name="arCols">컬럼명을 담고있는 배열</param>
        /// <returns>0 : 성공, !0 : 실패</returns>
        public static int DataTable_AddColumns(DataTable dt, string[] arCols)
        {
            try
            {
                DataColumn dc = null;
                Type typeColumn;

                //--------------------------------------------------------------

                // 기본 검증
                if (dt == null)
                    return -1;

                // 기본 형식 : 문자열
                typeColumn = Type.GetType("System.String");

                foreach (string szCol in arCols)
                {
                    // 컬럼유무 확인
                    if (dt.Columns.Contains(szCol))
                        continue;

                    // 컬럼 생성
                    dc = new DataColumn();

                    dc.DataType = typeColumn;
                    dc.ColumnName = szCol;
                    dt.Columns.Add(dc);
                }

                return 0;
            }
            catch (Exception ex)
            {
                // 예외 오류
                Trace.WriteLine("DataTable_AddColumns Error : " + ex.ToString());

                return -91;
            }
        }

        /// <summary>
        /// DataTable의 컬럼명을 변경합니다.
        /// </summary>
        /// <param name="dt">DataTable 객체</param>
        /// <param name="szCol">DataTable에 존재하는 컬럼명</param>
        /// <param name="szNewCol">새로운 컬럼명</param>
        /// <returns>0 : 성공, !0 : 실패</returns>
        public static int DataTable_RenameColumn(DataTable dt, string szCol, string szNewCol)
        {
            try
            {
                //--------------------------------------------------------------

                // 기본 검증
                if (dt == null)
                    return -1;

                // 컬럼유무 확인
                if (!dt.Columns.Contains(szCol))
                    return -1;

                // 새로운 컬럼유무 확인
                if (dt.Columns.Contains(szNewCol))
                    return -2;

                // 컬럼명 변경
                dt.Columns[szCol].ColumnName = szNewCol;

                return 0;
            }
            catch (Exception ex)
            {
                // 예외 오류
                Trace.WriteLine("DataTable_RenameColumn Error : " + ex.ToString());

                return -91;
            }
        }

        /// <summary>
        /// DataTable의 컬럼을 삭제합니다.
        /// </summary>
        /// <param name="dt">DataTable 객체</param>
        /// <param name="szCol">DataTable에서 삭제할 컬럼명</param>
        /// <returns>0 : 성공, !0 : 실패</returns>
        public static int DataTable_DeleteColumn(DataTable dt, string szCol)
        {
            try
            {
                //--------------------------------------------------------------

                // 기본 검증
                if (dt == null)
                    return -1;

                // 컬럼유무 확인
                if (!dt.Columns.Contains(szCol))
                    return -1;

                // 컬럼 삭제
                dt.Columns.Remove(szCol);

                return 0;
            }
            catch (Exception ex)
            {
                // 예외 오류
                Trace.WriteLine("DataTable_DeleteColumn Error : " + ex.ToString());

                return -91;
            }
        }

        /// <summary>
        /// Json String을 DataTable 형태로 변환합니다.
        /// </summary>
        /// <param name="jsonstr">json 형태의 string</param>
        /// <returns>DataTable Data</returns>
        public static DataTable ConvertJsonToDataTable(string jsonstr)
        {
            DataTable _makeDt = new DataTable();
            string[] jsonstrArray = Regex.Split(jsonstr.Replace("[", "").Replace("]", ""), "},{");
            List<string> colName = new List<string>();

            foreach (string js in jsonstrArray)
            {
                string[] jsonstrData = Regex.Split(js.Replace("{", "").Replace("}", ""), ",");
                foreach(string columsNameData in jsonstrData)
                {
                    try
                    {
                        int idx = columsNameData.IndexOf(":");
                        string colstr = columsNameData.Substring(0, idx - 1).Replace("\"", "");
                        if (!colName.Contains(colstr))
                        {
                            colName.Add(colstr);
                        }

                    }catch(Exception ex)
                    {
                        LogCls.writeLog("", "MineError.log", "ConvertJsonToDataTable Error Column : " + columsNameData + " (" + ex.Message + ")");
                    }
                }
                break;
            }

            // DataTable Column 정보 추가
            foreach (string addColName in colName)
            {
                _makeDt.Columns.Add(addColName);
            }

            // DataTable Data 입력
            foreach (string js in jsonstrArray)
            {
                string[] rowData = Regex.Split(js.Replace("{", "").Replace("}", ""), ",");
                DataRow dr = _makeDt.NewRow();
                foreach (string rd in rowData)
                {
                    try
                    {
                        int idx = rd.IndexOf(":");
                        string rowcolums = rd.Substring(0, idx - 1).Replace("\"", "");
                        string rowdatastring = rd.Substring(idx + 1).Replace("\"", "");
                        dr[rowcolums] = rowdatastring;
                    }
                    catch(Exception ex)
                    {
                        LogCls.writeLog("", "MineError.log", "ConvertJsonToDataTable Error Data (" + ex.Message + ")");
                        continue;
                    }

                }
                _makeDt.Rows.Add(dr);
            }

            return _makeDt;
        }

        /// <summary>
        /// DataTable의 Default를 셋팅합니다.
        /// </summary>
        /// <param name="_dt">셋팅할 DataTable</param>
        /// <param name="dispName">Display명</param>
        /// <param name="valName">Value명</param>
        /// <param name="_defaultCodeName">Default값(선택 또는 전체 또는 XX)</param>
        /// <returns></returns>
        public static DataTable fnInitDataTable(DataTable _dt, string dispName, string valName, string _defaultCodeName)
        {
            DtCls.DataTable_AddColumn(_dt, valName, "");
            DtCls.DataTable_AddColumn(_dt, dispName, "");

            DataRow dr = _dt.NewRow();
            dr[valName] = "";
            dr[dispName] = _defaultCodeName;
            _dt.Rows.InsertAt(dr, 0);

            return _dt;
        }
    }
}
