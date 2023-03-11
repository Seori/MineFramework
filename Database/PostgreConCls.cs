using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MineFramework
{
    public class PostgreConCls
    {
        public static string constring = string.Empty;
        public static string dbipaddr = string.Empty;
        public static string dbport = string.Empty;
        public static string dbname = string.Empty;
        public static string dbid = string.Empty;
        public static string dbpw = string.Empty;

        public static NpgsqlConnection npgconn;

        /// <summary>
        /// Database 접속정보를 셋팅합니다.
        /// </summary>
        /// <param name="_dbip">Database IP Address</param>
        /// <param name="_dbport">Database Port</param>
        /// <param name="_dbname">Database Name</param>
        /// <param name="_dbid">Database connect id</param>
        /// <param name="_dbpw">Database connect pw</param>
        public static void constrSet(string _dbip, string _dbport, string _dbname, string _dbid, string _dbpw)
        {
            dbipaddr = _dbip;
            dbport = _dbport;
            dbname = _dbname;
            dbid = _dbid;
            dbpw = _dbpw;

            // Connection String Setting
            connectionString = "host=" + dbipaddr + ";port=" + dbport + ";database=" + dbname + ";username=" + dbid + ";password=" + dbpw + ";";
        }

        public static string connectionString
        {
            get { return constring; }
            set { constring = value; }
        }

        /// <summary>
        /// PostgresSQL에 접속합니다.
        /// </summary>
        /// <returns>
        /// 0 : 성공
        /// -99 : 접속실패(C:\Mines\log 내에 접속로그 출력됨)
        /// </returns>
        public static int connect()
        {
            
            npgconn = new NpgsqlConnection(connectionString);            

            try
            {
                npgconn.Open();

                LogCls.writeLog("", "ConnLog.log", "DatabaseType : postgresql | DatabaseIp : " + dbipaddr + " | Connection Complete(" + DateTime.Now.ToString() + ")");

                return 0;
            }
            catch(NpgsqlException npg)
            {
                LogCls.writeLog("", "ConnLog.log", "DatabaseType : postgresql | DatabaseIp : " + dbipaddr + " | Connection Fail(" + DateTime.Now.ToString() + ") " + npg.Message);

                return -99;
            }
        }

        public static DataTable queryExec(string _query)
        {
            try
            {
                NpgsqlCommand npcmd = new NpgsqlCommand();
                npcmd.CommandType = System.Data.CommandType.Text;
                npcmd.Connection = npgconn;
                npcmd.CommandText = _query;

                NpgsqlDataReader reader = npcmd.ExecuteReader();
                DataTable queryDt = new DataTable();

                queryDt.Load(reader);

                return queryDt;
            }
            catch(Exception ex)
            {
                LogCls.writeLog("", "ConnLog.log", "queryExec 오류발생 : " + _query);

                return null;
            }            
        }

        public static int executeNonQuery(string _query)
        {
            try
            {
                NpgsqlCommand npcmd = new NpgsqlCommand();
                npcmd.CommandType = System.Data.CommandType.Text;
                npcmd.Connection = npgconn;
                npcmd.CommandText = _query;

                npcmd.ExecuteNonQuery();

                return 0;
            }
            catch(Exception e)
            {
                LogCls.writeLog("", "ConnLog.log", "executeNonQuery 오류발생 : " + _query);
                return -99;
            }
        }
    }
}
