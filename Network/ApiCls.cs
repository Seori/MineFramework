using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net.Cache;

namespace MineFramework
{
    public class ApiCls
    {
        public static string _apiToken = string.Empty;
        public static string _chatNo = string.Empty;
        public static string _telegramUrl = "https://api.telegram.org/bot";

        public ApiCls() { }

        public static string apiToken
        {
            get { return _apiToken; }
            set { _apiToken = value; }
        }

        // Telegram Chatting Room No
        public static string chatNo
        {
            get { return _chatNo; }
            set { _chatNo = value; }
        }

        public static int sendTelegramMessage(string _msg, out string _errorMsg)
        {
            if (string.IsNullOrEmpty(_apiToken))
            {
                // api token 없음
                _errorMsg = "API Token 정보가 없음";
                return -89;
            }

            if (string.IsNullOrEmpty(_chatNo))
            {
                // chat no 없음
                _errorMsg = "Chatting No 정보가 없음";
                return -90;
            }

            string msgUrl = string.Format("{0}{1}/sendMessage", _telegramUrl, _apiToken);

            HttpWebRequest webRq = WebRequest.Create(new Uri(msgUrl)) as HttpWebRequest;
            webRq.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            webRq.Timeout = 1000 * 30;      // 30초
            webRq.Method = "POST";
            webRq.ContentType = "application/json";

            string _json = string.Format("{{\"chat_id\":\"{0}\", \"text\":\"{1}\"}}", _chatNo, EncodeJsonChar(_msg));
            byte[] data = UTF8Encoding.UTF8.GetBytes(_json);
            webRq.ContentLength = data.Length;

            using (Stream stream = webRq.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
                stream.Flush();
            }

            HttpWebResponse webRp = null;

            try
            {
                webRp = webRq.GetResponse() as HttpWebResponse;

                if (webRp.StatusCode == HttpStatusCode.OK)
                {
                    string responseData = string.Empty;
                    using (Stream responseStream = webRp.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(responseStream, UTF8Encoding.UTF8))
                        {
                            responseData += reader.ReadToEnd();
                        }
                    }

                    if (responseData.IndexOf("\"ok\":true") > 0)
                    {
                        // 성공메세지가 있으면
                        _errorMsg = "";
                        return 0;
                    }
                    else
                    {
                        // Json Parsing 오류
                        _errorMsg = "Json Parsing 오류 : " + responseData;
                        return -91;
                    }
                }
                else
                {
                    // Http 오류
                    _errorMsg = "Http 접속오류 : " + webRp.StatusCode;
                    return -95;
                }
            }
            catch (Exception ex)
            {
                _errorMsg = "Exception 오류 : " + ex.Message;
                return -98;
            }
            finally
            {
                if (webRp != null)
                {
                    webRp.Close();
                }
            }
        }

        public static string EncodeJsonChar(string _msg)
        {
            return _msg.Replace("\b", "\\\b")
                .Replace("\f", "\\\f")
                .Replace("\n", "\\\n")
                .Replace("\r", "\\\r")
                .Replace("\t", "\\\t")
                .Replace("\"", "\\\"")
                .Replace("\\", "\\\\");
        }
    }
}
