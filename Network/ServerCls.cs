using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineFramework
{
    internal class ServerCls
    {
        public OperatingSystem os;
        public Version ver;

        /// <summary>
        /// OS Version을 조회합니다.
        /// </summary>
        /// <returns></returns>
        public string getOSVersion()
        {
            GeneralDictionary gd = new GeneralDictionary();
            gd["Major"] = ver.Major.ToString();
            gd["Minor"] = ver.Minor.ToString();
            gd["Build"] = ver.Build.ToString();
            gd["Revision"] = ver.Revision.ToString();

            return ver.ToString();
        }
    }
}
