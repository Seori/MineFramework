using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace MineFramework
{
    public class StringCls
    {
        #region strcut : string 문자열은 전달받아 시작포인트부터 원하는 지점까지 자릅니다.
        /// <summary>
        /// string 문자열은 전달받아 시작포인트부터 원하는 지점까지 자릅니다.
        /// </summary>
        /// <param name="_strmsg">메세지</param>
        /// <param name="_spoint">시작지점</param>
        /// <param name="_csize">자를길이</param>
        /// <returns>
        /// 9Z9X : 각종에러시 반환하는 코드
        /// </returns>
        public string strcut(string _strmsg, int _spoint, int _csize)
        {
            if (_strmsg.Length < _spoint)
            {
                // 글자의 길이보다 시작지점이 높으면 에러
                return "9Z9X";
            }

            if (_strmsg.Length < _csize || _csize == 0)
            {
                // 글자의 길이가 자를길이보다 높아도 에러
                return "9Z9X";
            }

            string _tempstr = _strmsg.Substring(_spoint, _csize);

            return _tempstr;
        }
        #endregion

        #region onlyNumber : 숫자만 입력되는지 체크합니다.
        /// <summary>
        /// 숫자만 입력되는지 체크합니다.
        /// </summary>
        /// <param name="_nbStr">체크할 문자열</param>
        /// <returns>숫자열의 문자열</returns>
        public static string onlyNumber(string _nbStr)
        {
            string rStr = string.Empty;

            foreach (Char c in _nbStr.ToCharArray())
            {
                if (Char.IsDigit(c) || Char.IsControl(c))
                    rStr += c;
            }

            return rStr;
        }
        #endregion

        // 한국형 숫자포맷 설정
        private static NumberFormatInfo _niCurrency = new CultureInfo("ko-KR", false).NumberFormat;

        #region formatCurrency : 숫자포맷을 설정합니다. (3 Params)
        /// <summary>
        /// 숫자포맷을 설정합니다.
        /// </summary>
        /// <param name="_amount">숫자금액</param>
        /// <param name="_nDecimal">소수점 자리수</param>
        /// <param name="_szCurrencySymbol">대체할 문자열</param>
        /// <returns></returns>
        public string formatCurrency(string _amount, decimal _nDecimal, string _szCurrencySymbol)
        {
            string szDecimal;
            double nValue;

            if (!double.TryParse(_amount, out nValue))
                return _amount;

            // 소수 자리수 계산
            if (_nDecimal < 0)
                _nDecimal = 0;

            szDecimal = _nDecimal.ToString();

            // CurrencySymbol을 설정한다.
            _niCurrency.CurrencySymbol = _szCurrencySymbol;

            // 통화 포맷으로 변경
            return string.Format(_niCurrency, "{0:C" + szDecimal + "}", nValue);
        }
        #endregion

        #region formatCurrency : 숫자포맷을 설정합니다. (2 Params)
        /// <summary>
        /// 숫자 문자열을 통화 문자열로 변경합니다.
        /// 숫자 문자열이 아니면 입력된 문자열을 변경없이 반환합니다.
        /// </summary>
        /// <param name="szIn">입력 숫자문자열</param>
        /// <param name="nDecimal">소수 자리수</param>
        /// <returns>변경된 통화문자열</returns>
        public string formatCurrency(string szIn, int nDecimal)
        {
            return formatCurrency(szIn, nDecimal, string.Empty);
        }
        #endregion

        #region formatCurrency : 숫자포맷을 설정합니다. (1 Param)
        /// <summary>
        /// 숫자 문자열을 통화 문자열로 변경합니다.
        /// 숫자 문자열이 아니면 입력된 문자열을 변경없이 반환합니다.
        /// 소수 자리수는 0으로 설정됩니다.
        /// </summary>
        /// <param name="szIn">입력 숫자문자열</param>
        /// <returns>변경된 통화문자열</returns>
        public string formatCurrency(string szIn)
        {
            return formatCurrency(szIn, 0, string.Empty);
        }
        #endregion

        #region CardNumber : 카드번호를 '-'를 포함하여 변환합니다. 
        /// <summary>
        /// 카드번호를 '-'를 포함하여 변환합니다.
        /// </summary>
        /// <remarks>         
        /// 일반카드(16자리)         : XXXX-XXXX-XXXX-XXXX
        /// 아멕스카드(15자리)       : XXXX-XXXXXX-XXXXX
        /// 현대다이너스카드(14자리) : XXXX-XXXXXX-XXXX
        /// </remarks>
        /// <param name="szIn">카드번호입력값</param>
        /// <returns>카드번호출력값</returns>
        public static string CardNumber(string szIn)
        {
            string szResult = string.Empty;
            string szDelimeter = "-";

            if (szIn.Length == 16) // 일반카드
            {
                szResult += szIn.Substring(0, 4) +
                            szDelimeter +
                            szIn.Substring(4, 4) +
                            szDelimeter +
                            szIn.Substring(8, 4) +
                            szDelimeter +
                            szIn.Substring(12, 4);
            }
            else if (szIn.Length == 15) // 아멕스카드
            {
                szResult += szIn.Substring(0, 4) +
                            szDelimeter +
                            szIn.Substring(4, 6) +
                            szDelimeter +
                            szIn.Substring(10, 5);
            }
            else if (szIn.Length == 14) // 현대-다이너스카드
            {
                szResult += szIn.Substring(0, 4) +
                            szDelimeter +
                            szIn.Substring(4, 6) +
                            szDelimeter +
                            szIn.Substring(10, 4);
            }
            else // 기타(미분류)
            {
                szResult = szIn;
            }

            return szResult;
        }
        #endregion

        #region TelephoneNumber : 전화번호를 - 를 입력하여 변환합니다.
        /// <summary>
        /// 전화번호를 - 를 입력하여 변환합니다.
        /// 01012345678 -> 010-1234-5678
        /// 지역번호나 모바일번호를 포함해야 합니다.
        /// 최소 9자리 이상이 입력되야합니다.
        /// 9자리 이하로 입력되면 앞자리가 '0'으로 채워져서 만들어집니다.
        /// </summary>
        /// <param name="szIn">입력 문자열</param>
        /// <returns>
        /// 전화번호 형식 문자열
        /// </returns>
        public static string TelephoneNumber(string szIn)
        {
            try
            {
                string szTel0, szTel1, szTel2;

                // 필요없는 문자를 지운다.
                szIn = szIn.Replace(" ", "");
                szIn = szIn.Replace("-", "");
                szIn = szIn.Replace("(", "");
                szIn = szIn.Replace(")", "");
                szIn = szIn.Replace("（", "");
                szIn = szIn.Replace("）", "");

                // 길이가 모자르면 앞을 0으로 채운 후 구성한다.
                if (szIn.Length < 9)
                {
                    if (szIn.Length == 7)
                    {
                        // 서울 지역번호를 붙여준다.
                        szIn = "02" + szIn;
                    }
                    else
                    {
                        // 앞을 0으로 채운다.
                        szIn = szIn.PadLeft(9, '0');

                        // 앞이 02가 아니면 10자리로 만들어야 한다.
                        if (szIn.Substring(0, 2) != "02")
                            szIn = szIn.PadLeft(10, '0');
                    }
                }

                //--------------------------------------------------------------

                // 앞 2자리 비교
                szTel0 = szIn.Substring(0, 2);

                if (szTel0 == "02")
                {
                    if (szIn.Length > 9)
                    {
                        // 2-4-4
                        szTel1 = szIn.Substring(2, 4);
                        szTel2 = szIn.Substring(6, 4);
                    }
                    else
                    {
                        // 2-3-4
                        szTel1 = szIn.Substring(2, 3);
                        szTel2 = szIn.Substring(5, 4);
                    }
                }
                else
                {
                    // 서울번호가 아니면 앞 3자리로 인식
                    szTel0 = szIn.Substring(0, 3);

                    if (szIn.Length > 10)
                    {
                        // 3-4-4
                        szTel1 = szIn.Substring(3, 4);
                        szTel2 = szIn.Substring(7, 4);
                    }
                    else
                    {
                        // 3-3-4
                        szTel1 = szIn.Substring(3, 3);
                        szTel2 = szIn.Substring(6, 4);
                    }
                }

                return szTel0 + "-" + szTel1 + "-" + szTel2;
            }
            catch (Exception ex)
            {
                // 예외 오류
                return "000-0000-0000";
            }
        }
        #endregion

        #region CompanyRegNumber : 10자리 사업자번호를 - 를 입력하여 변환합니다.
        /// <summary>
        /// 10자리 사업자번호를 - 를 입력하여 변환합니다.
        /// 1112233333 -> 111-22-33333
        /// </summary>
        /// <param name="szIn">입력 문자열</param>
        /// <returns>
        /// 입력이 올바르면 포맷 문자열을 아니면 원본 문자열을 반환합니다.
        /// </returns>
        public static string CompanyRegNumber(string szIn)
        {
            // 유효성은 길이만 체크한다. 문자, 숫자 구분은 체크하지 않는다.
            if (szIn.Length != 10)
                return szIn;

            // 사업자번호 XXX-XX-XXXXX
            return szIn.Substring(0, 3) + "-" +
                   szIn.Substring(3, 2) + "-" +
                   szIn.Substring(5, 5);
        }
        #endregion

        #region JuminRegNumber : 13자리 주민등록번호를 - 를 입력하여 변환합니다.
        /// <summary>
        /// 13자리 주민등록번호를 - 를 입력하여 변환합니다.
        /// 123456-1234567 -> 123456-1234567
        /// </summary>
        /// <param name="szIn">입력 문자열</param>
        /// <returns>
        /// 입력이 올바르면 포맷 문자열을 아니면 원본 문자열을 반환합니다.
        /// </returns>
        public static string JuminRegNumber(string szIn)
        {
            // 유효성은 길이만 체크한다. 문자, 숫자 구분은 체크하지 않는다.
            if (szIn.Length != 13)
                return szIn;

            // 사업자번호 XXX-XX-XXXXX
            return szIn.Substring(0, 6) + "-" +
                   szIn.Substring(6, 7);
        }
        #endregion

        #region JuminRegNumberMask : 13자리 주민등록번호를 - 를 입력하여 마스킹 변환합니다.
        /// <summary>
        /// 13자리 주민등록번호를 - 를 입력하여 마스킹 변환합니다.
        /// 1234561234567 -> 123456-●●●●●●●
        /// </summary>
        /// <param name="szIn">입력 문자열</param>
        /// <returns>
        /// 입력이 올바르면 포맷 문자열을 아니면 원본 문자열을 반환합니다.
        /// </returns>
        public static string JuminRegNumberMask(string szIn)
        {
            // 유효성은 길이만 체크한다. 문자, 숫자 구분은 체크하지 않는다.
            if (szIn.Length != 13)
                return szIn;

            // 주민등록번호 XXXXXX-●●●●●●●
            return szIn.Substring(0, 6) + "-" +
                   "●●●●●●●";
        }
        #endregion

        #region Date : YYYYMMDD 형식의 날짜 문자열 사이에 주어진 구분자를 추가합니다. (2 Params)
        /// <summary>
        /// YYYYMMDD 형식의 날짜 문자열 사이에 주어진 구분자를 추가합니다.
        /// </summary>
        /// <param name="szIn">입력 문자열</param>
        /// <param name="szDelimiter">구분자</param>
        /// <returns>
        /// 입력이 올바르면 구분자가 삽입된 날짜 문자열을 
        /// 아니면 원본 문자열을 반환합니다.
        /// </returns>
        public static string Date(string szIn, string szDelimiter)
        {
            // 유효성은 길이만 체크한다. 문자, 숫자 구분은 체크하지 않는다.
            if (szIn.Length != 8)
                return szIn;

            return szIn.Substring(0, 4) + szDelimiter +
                   szIn.Substring(4, 2) + szDelimiter +
                   szIn.Substring(6, 2);
        }
        #endregion

        #region Date : YYYYMMDD 형식의 날짜 문자열 사이에 주어진 디폴트 구분자를 추가합니다. (1 Param)
        /// <summary>
        /// YYYYMMDD 형식의 날짜 문자열 사이에 주어진 디폴트 구분자를 추가합니다.
        /// YYYYMMDD -> YYYY.MM.DD
        /// </summary>
        /// <param name="szIn">입력 문자열</param>
        /// <returns>
        /// 입력이 올바르면 "-" 구분자가 삽입된 날짜 문자열을 
        /// 아니면 원본 문자열을 반환합니다.
        /// </returns>
        public static string Date(string szIn)
        {
            return Date(szIn, "-");
        }
        #endregion

        #region AmountComma : 입력된 String을 콤마를 체크하여 Return한다.
        /// <summary>
        /// 콤마를 체크하여 Return한다. (Max 20자리)
        /// 해당함수 사용시 데이터를 직접 넣지 말고, 별도의 함수를 호출하여 Replace된 값을 보낼것
        /// </summary>
        /// <param name="_str">체크할 문자열</param>
        /// <returns></returns>
        public static string AmountComma(string _str)
        {
            char[] c = new char[20];
            int j = 20;
            int readcount = 0;
            string s1 = "";

            // Trim 처리
            _str = _str.Trim();

            bool minus = _str.Contains("-");
            if (minus)
            {
                _str = (Int32.Parse(_str) * -1).ToString();
            }

            if (_str == "0")
            {
                s1 = "0";
            }
            else
            {
                if (_str.Length > 3)
                {
                    for (int i = _str.Length - 1; i >= 0; i--)
                    {
                        j--;
                        c[j] = _str[i];
                        readcount++;

                        if (readcount == 3 && i != 0)
                        {
                            readcount = 0;
                            j--;
                            c[j] = ',';
                        }
                    }

                    if (minus)
                    {
                        j--;
                        c[j] = '-';
                    }

                    s1 = new String(c, j, 20 - j);
                }
                else
                {
                    if (minus)
                    {
                        _str = (Int32.Parse(_str) * -1).ToString();
                    }

                    s1 = _str;
                }
            }

            return s1;
        }
        #endregion

        #region delsosujum : 소수점이하는 절삭합니다.
        /// <summary>
        /// 소수점이하는 절삭합니다.
        /// </summary>
        /// <param name="sval"></param>
        /// <returns></returns>
        public static string delsosujum(string _sval)
        {
            int c = _sval.IndexOf(".");

            // .위치까지 잘라서 Return value에 넣는다.
            string rval = "";
            if (c > 0)
            {
                rval = _sval.Substring(0, c);
            }
            else
            {
                rval = _sval;
            }

            return rval;
        }
        #endregion

        #region getToDate : 현재날짜를 반환합니다.
        /// <summary>
        /// 현재날짜를 반환합니다.
        /// </summary>
        /// <param name="_gu">구분자</param>
        /// <returns>
        /// 구분자가 있는경우 구분자로 년,월,일을 구분하여 Return합니다.
        /// </returns>
        public static string getToDate(string _gu)
        {
            DateTime todaydate = DateTime.Now;

            string strdate = todaydate.ToString("yyyyMMdd");

            if (!string.IsNullOrEmpty(_gu))
            {
                strdate = strdate.Substring(0, 4) + _gu + strdate.Substring(5, 2) + _gu + strdate.Substring(7, 2);
            }

            return strdate;
        }
        #endregion

        #region fnDataTableToJsonString : DataTable을 Json형태로 변환하여 Return합니다. (JSonConvert사용)
        /// <summary>
        /// DataTable을 전달받아 Json String 형태로 변환하여 반환합니다.
        /// </summary>
        /// <param name="_sourceDt">대상 DataTable</param>
        /// <returns>Json String</returns>
        public static string fnDataTableToJsonString(DataTable _sourceDt)
        {
            string _JSONString = string.Empty;
            _JSONString = JsonConvert.SerializeObject(_sourceDt);
            return _JSONString;
        }
        #endregion

        #region fnDataTableToJson2 : DataTable을 Json형태로 변환하여 Return합니다.(JSonConvert미사용)
        /// <summary>
        /// DataTable을 Json형태로 변환하여 Return합니다. (JSonConvert미사용)
        /// </summary>
        /// <param name="_sourtDt">소스 DataTable</param>
        /// <returns>String Json 형태로 Return</returns>
        public static string fnDataTableToJson2(DataTable _sourtDt)
        {
            var JSONString = new StringBuilder();
            if (_sourtDt.Rows.Count > 0)
            {
                JSONString.Append("[");
                for (int i = 0; i < _sourtDt.Rows.Count; i++)
                {
                    JSONString.Append("{");
                    for (int j = 0; j < _sourtDt.Columns.Count; j++)
                    {
                        if (j < _sourtDt.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + _sourtDt.Columns[j].ColumnName.ToString() + "\":" + "\"" + _sourtDt.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == _sourtDt.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + _sourtDt.Columns[j].ColumnName.ToString() + "\":" + "\"" + _sourtDt.Rows[i][j].ToString() + "\"");
                        }
                    }
                    if (i == _sourtDt.Rows.Count - 1)
                    {
                        JSONString.Append("}");
                    }
                    else
                    {
                        JSONString.Append("},");
                    }
                }
                JSONString.Append("]");
            }
            return JSONString.ToString();
        }
        #endregion

        #region fnDataTableToCsv : DataTable을 csv형태로 변환하여 Return합니다.
        public static void fnDataTableToCsv(DataTable _sourceDt)
        {
            // csv경로 셋팅
            FileCls.makeFd(@"C:\CUBESTOCK");

            // 파일명 셋팅
            string csvfilepath = @"C:\CUBESTOCK\CUBE_" + StringCls.getToDate("") + ".csv";

            // 파일이 있으면 덮어씌운다.
            using (StreamWriter sw = new StreamWriter(csvfilepath, true, Encoding.UTF8))
            {
                // header 영역
                for (int i = 0; i < _sourceDt.Columns.Count; i++)
                {
                    sw.Write(_sourceDt.Columns[i]);
                    if (i < _sourceDt.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }

                // 개행
                sw.Write(sw.NewLine);

                foreach (DataRow dr in _sourceDt.Rows)
                {
                    for (int i = 0; i < _sourceDt.Columns.Count; i++)
                    {
                        if (!Convert.IsDBNull(dr[i]))
                        {
                            string valstr = dr[i].ToString();
                            if (valstr.Contains(","))
                            {
                                valstr = String.Format("\"{0}\"", valstr);
                                sw.Write(valstr);
                            }
                            else
                            {
                                sw.Write(dr[i].ToString());
                            }
                        }

                        if (i < _sourceDt.Columns.Count - 1)
                        {
                            sw.Write(",");
                        }
                    }

                    sw.Write(sw.NewLine);
                }

                sw.Close();
            }            
        }

        #endregion

        #region fnJsonToDataTable : Json 파일경로를 받아서 Json파일을 읽어 DataTable형태로 변환하여 Return합니다.
        /// <summary>
        /// Json 파일경로를 받아서 DataTable로 Return합니다.
        /// </summary>
        /// <param name="jsonpath">json파일 Full경로</param>
        /// <returns></returns>
        public static DataTable fnJsonToDataTable(string jsonpath)
        {
            DataTable returnDt = new DataTable();

            // json파일을 읽어온다.
            StreamReader sr = new StreamReader(jsonpath);
            string jsonstr = sr.ReadToEnd();

            returnDt = (DataTable)JsonConvert.DeserializeObject(jsonstr, typeof(DataTable));

            // 다 읽었다면 StreamReader를 정리한다.
            sr.Close();
            sr.Dispose();

            // json파일을 Reader로 읽는다.
            //using (JsonTextReader reader = new JsonTextReader(sr))
            //{
            //    JObject json = (JObject)JToken.ReadFrom(reader);
            //}

            return returnDt;
        }
        #endregion

        public static string[] fnJsonStrToDataRow(string jsonstring)
        {
            // JArray로 json 해석
            var jArray = JArray.Parse(jsonstring);

            // 넣을 공간 추가
            string[] tmpStr = new string[jArray.Count];
            int tmpcount = 0;

            foreach (var item in jArray)
            {
                tmpStr[tmpcount] = item.Value<string>().ToString();
                tmpcount++;
            }

            return tmpStr;
        }

        #region fnMakeJsonFile : DataTable을 받아 Json파일 넘어온경로에 생성합니다.
        /// <summary>
        /// DataTable을 받아 Json파일을 생성합니다.
        /// </summary>
        /// <param name="_dt">변환할 DataTable</param>
        /// <param name="_savepath">저장할 경로</param>
        /// <param name="_savefile">저장할 파일명</param>
        public static void fnMakeJsonFile(DataTable _dt, string _savepath, string _savefile)
        {
            // 해당경로가 있는지 체크하고 없으면 생성한다.
            FileCls.makeFd(_savepath);

            string jsonstr = string.Empty;

            if (_dt.Rows.Count > 0)
            {
                // DataTable -> Json
                jsonstr = fnDataTableToJson2(_dt);
                
                // 파일을 생성합니다.
                int filesc = FileCls.makeFileText(jsonstr, _savepath, _savefile);

                if (filesc == -99)
                {
                    // 폴더경로 없음
                    LogCls.writeLog("", "MineFrameworkLog.log", "JsonFile Create Error ("+ _savepath + " 해당경로가 존재하지 않음.)");
                }
                else if (filesc == -98)
                {
                    // 파일생성오류
                    LogCls.writeLog("", "MineFrameworkLog.log", "TextFile Create Error (파일생성오류)");
                }
            }
        }
        #endregion
    }
}
