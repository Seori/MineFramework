using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Xml.Linq;
using System.Drawing;
using System.ComponentModel;

namespace MineFramework
{
    public class DbConCls_Postgre
    {
        public static string constring = string.Empty;
        public static string dbipaddr = string.Empty;
        public static string dbport = string.Empty;
        public static string dbname = string.Empty;
        public static string dbid = string.Empty;
        public static string dbpw = string.Empty;
        public string sqlitepath = string.Empty;

        // PostgreSQL용 Connection
        protected static NpgsqlConnection _pgconn;

        public enum DatabaseType
        {
            MSSQL = 0,
            SQLITE = 1,
            ORACLE = 2,
            MYSQL = 3,
            POSTGRES = 4,
        }

        public static string DatabseIp
        {
            get { return dbipaddr; }
            set { dbipaddr = value; }
        }

        public static string DatabasePort
        {
            get { return dbport; }
            set { dbport = value; }
        }

        public static string DatabaseName
        {
            get { return dbname; }
            set { dbname = value; }
        }

        public static string DatabaseId
        {
            get { return dbid; }
            set { dbid = value; }
        }

        public static string DatabasePw
        {
            get { return dbpw; }
            set { dbpw = value; }
        }

        public static string connectionString
        {
            get { return constring; }
            set { constring = value; }
        }

        public static void makePgConString()
        {
            constring = "User ID=" + dbid + ";Password=" + dbpw + ";Host=" + dbipaddr + ";Port=" + dbport + ";Database=" + dbname + ";";
        }

        /// <summary>
        /// DB Connection 함수
        /// </summary>
        /// <returns></returns>
        public static int newConnect()
        {
            try
            {
                // 연결되어있다면 종료한다.
                disConnect_P();

                if (string.IsNullOrEmpty(connectionString))
                {
                    // Connection String이 없다면 연결종료처리한다.
                    return -9;
                }

                _pgconn = new NpgsqlConnection();
                ((DbConnection)_pgconn).ConnectionString = connectionString;
                ((DbConnection)_pgconn).StateChange += new StateChangeEventHandler(_pgconn_StateChange);
                ((DbConnection)_pgconn).Open();
            
            }
            catch (Exception ex)
            {
                LogCls.writeLog("", "ConnLog.log", "DatabaseType : POSTGRESQL | DatabaseIp : " + dbipaddr + " | Connection Fail(" + DateTime.Now.ToString() + ") | Reason : " + ex.Message);

                if (_pgconn.State == System.Data.ConnectionState.Open)
                {
                    // Connection 객체연결 닫기
                    _pgconn.Close();

                    // Connection 객체연결 삭제
                    _pgconn.Dispose();
                }

                return -8;
            }

            return 0;
        }

        #region Connection을 종료합니다. (disConnect_P)
        /// <summary>
        /// DB 연결 해제
        /// </summary>
        public static void disConnect_P()
        {
            if (_pgconn == null || ((DbConnection)_pgconn).State != ConnectionState.Open)
                return;
            ((DbConnection)_pgconn).Close();
            ((Component)_pgconn).Dispose();
        }
        #endregion

        #region PostgreSQL Section ===========================================

        #region 실행쿼리를 실행합니다. (pgSQLExecuteNonQuery) - 파라미터 1개

        /// <summary>
        /// 실행(입력 , 수정 ,삭제) 쿼리
        /// </summary>
        /// <param name="_query"></param>
        /// <returns></returns>
        public static int pgSQLExecuteNonQuery(string _query)
        {
            try
            {
                if (((DbConnection)_pgconn).State == ConnectionState.Open)
                {
                    using (NpgsqlCommand npgsqlCommand = new NpgsqlCommand())
                    {
                        npgsqlCommand.Connection = _pgconn;
                        ((DbCommand)npgsqlCommand).CommandText = _query;
                        ((DbCommand)npgsqlCommand).ExecuteNonQuery();
                        return 0;
                    }
                }
                else
                {
                    LogCls.writeLog("", "MineFrameworkLog.log", "PostgreSQL Database 접속안됨.");
                    return 9;
                }
            }
            catch (NpgsqlException ex)
            {
                LogCls.writeLog("", "MineFrameworkLog.log", "PostgreSQL 실행Error  : " + _query);

                return 9;
            }
        }

        #endregion

        #region 실행쿼리를 실행합니다. (pgSQLExecuteQuery) - 파라미터 2개
        public static int pgSQLExecuteQuery(NpgsqlConnection _pgcon, string _query)
        {
            DataSet dataSet = (DataSet)null;
            try
            {
                if (_pgcon.State == ConnectionState.Open)
                {
                    using (NpgsqlCommand npgsqlCommand = new NpgsqlCommand())
                    {
                        npgsqlCommand.Connection = _pgcon;
                        ((DbCommand)npgsqlCommand).CommandText = _query;
                        ((DataAdapter)new NpgsqlDataAdapter(npgsqlCommand)).Fill(dataSet);
                    }

                    return 0;
                }
                else
                {
                    return 9;
                    LogCls.writeLog("", "MineFrameworkLog.log", "PostgreSQL Database 접속안됨.");
                }
            }
            catch (NpgsqlException ex)
            {
                return 9;
                LogCls.writeLog("", "MineFrameworkLog.log", "PostgreSQL Database 조회실패 - Query" + _query + ((Exception)ex).Message);
            }
            //return dataSet;
        }
        #endregion

        #region 조회쿼리를 실행합니다. (pgSQLExecuteQuery)
        /// <summary>
        /// 조회 쿼리
        /// </summary>
        /// <param name="_query"></param>
        /// <returns></returns>
        public static DataSet pgSQLExecuteQuery(string _query)
        {
            DataSet dataSet = (DataSet)null;
            try
            {
                if (((DbConnection)_pgconn).State == ConnectionState.Open)
                {
                    using (NpgsqlCommand npgsqlCommand = new NpgsqlCommand())
                    {
                        npgsqlCommand.Connection = _pgconn;
                        ((DbCommand)npgsqlCommand).CommandText = _query;
                        ((DataAdapter)new NpgsqlDataAdapter(npgsqlCommand)).Fill(dataSet);
                    }
                }
                else
                {
                    LogCls.writeLog("", "MineFrameworkLog.log", "PostgreSQL Database 접속안됨.");
                }
            }
            catch (NpgsqlException ex)
            {
                LogCls.writeLog("", "MineFrameworkLog.log", "PostgreSQL Database 조회실패 - Query" + _query + ((Exception)ex).Message);
            }
            return dataSet;
        }
        #endregion

        #endregion ===========================================================

        /// <summary>
        /// PostgreSQL Connection 상태
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void _pgconn_StateChange(object sender, StateChangeEventArgs e)
        {
            LogCls.writeLog("", "MineFrameworkLog.log", "PostgreSQL Connection 상태 : " + e.CurrentState);

            if (e.CurrentState == ConnectionState.Open)
            {
                LogCls.writeLog("", "MineFrameworkLog.log", "PostgreSQL 접속 성공");
            }
            else
            {
                if (e.CurrentState != ConnectionState.Closed)
                    return;

                LogCls.writeLog("", "MineFrameworkLog.log", "PostgreSQL 접속 해제");
            }
        }
    }
}
