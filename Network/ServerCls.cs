using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineFramework
{
    public class ServerCls
    {
        /// <summary>
        /// OS Version을 조회합니다.
        /// </summary>
        /// <returns></returns>
        public static string getOSVersion()
        {
            OperatingSystem os = Environment.OSVersion;
            Version ver = os.Version;

            GeneralDictionary gd = new GeneralDictionary();
            gd["Major"] = ver.Major.ToString();
            gd["Minor"] = ver.Minor.ToString();
            gd["Build"] = ver.Build.ToString();
            gd["Revision"] = ver.Revision.ToString();

            string verJson = StringCls.fnGdToJson(gd);

            return verJson;
        }
    }
}
