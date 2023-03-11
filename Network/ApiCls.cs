using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace MineFramework
{
    internal class ApiCls
    {
        public static void fnCallApi(string requestUri, GeneralDictionary gd, string _key, string _type, out DataTable _returnDt)
        {
            HttpWebRequest httpWeb = (HttpWebRequest)WebRequest.Create(requestUri);
            httpWeb.Method = "GET";
            httpWeb.ContentType = "application/json";
            httpWeb.Headers.Add("key", _key);

            if (string.IsNullOrEmpty(_type))
            {
                _type = "json";
            }
            httpWeb.Headers.Add("type", _type);

            if (gd.Count > 0)
            {
                for (int i = 0; i < gd.Count; i++)
                {
                    // GeneralDictionary 만큼 데이터부 생성
                }
            }

            _returnDt = null;
        }
    }
}
