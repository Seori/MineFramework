using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Threading;
using Oracle.ManagedDataAccess.Client;
using Npgsql;
using System.ComponentModel;

namespace MineFramework
{
    public class DbConCls
    {
        public static string constring = string.Empty;
        public static string dbipaddr = string.Empty;
        public static string dbport = string.Empty;
        public static string dbname = string.Empty;
        public static string dbid = string.Empty;
        public static string dbpw = string.Empty;
        public string sqlitepath = string.Empty;

        // 일반 SQLConnection
        protected static SqlConnection _conn;

        // Oracle용 Connection
        protected static OracleConnection _oraconn;

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

        /// <summary>
        /// Database Type
        /// Default : MS-SQL Server
        /// </summary>
        protected static DatabaseType _dbType = DatabaseType.MSSQL;

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

        public DatabaseType getDbType
        {
            get { return _dbType; }
            set { _dbType = value; }
        }

        public static string connectionString
        {
            get { return constring; }
            set { constring = value; }
        }

        /// <summary>
        /// ConnectionString을 생성합니다.
        /// </summary>
        public static void makeConString()
        {
            constring = "Data Source=" + dbipaddr + "," + dbport + ";Database=" + dbname + ";UID=" + dbid + ";PWD=" + dbpw + ";Connection Timeout=3600"; 
        }
        
        public static void makePgConString()
        {
            constring = "User ID=" + dbid + ";Password=" + dbpw + ";Host=" + dbipaddr + ";Port=" + dbport + ";Database=" + dbname + ";";
        }

        public static SqlConnection connObject
        {
            get { return _conn; }
        }

        #region Connect 접속함수(newConnect로이동 - 사용안함)
        /// <summary>
        /// Database에 접속합니다. (Timeout 10초)
        /// </summary>
        /// <param name="constr">DB연결 String</param>
        /// <returns></returns>
        public static int Connect(string constr)
        {
            try
            {
                if (_dbType == DatabaseType.MSSQL)
                {
                    _conn = (SqlConnection)new SqlConnection();
                    if (string.IsNullOrEmpty(constr))
                    {
                        _conn.ConnectionString = "Data Source=" + dbipaddr + "," + dbport + ";Database=" + dbname + ";UID=" + dbid + ";PWD=" + dbpw + ";Connection Timeout=10";
                    }
                    else
                    {
                        _conn.ConnectionString = constr;
                    }                    
                    _conn.StateChange += _conn_StateChange;
                    _conn.Open();
                }
                else if (_dbType == DatabaseType.SQLITE)
                {
                    //_connlite = new SQLiteConnection("Data Source=" + sqlitepath + ";Version=3;");
                    //_connlite.StateChange += _connlite_StateChange;
                    //_connlite.Open();
                }
                else if (_dbType == DatabaseType.ORACLE)
                {
                    //_oraconn = new OracleConnection($"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={dbipaddr})(PORT={dbport})))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME={dbname})));User ID={dbid};Password={dbpw};Connection Timeout=30;");
                    //_oraconn.Open();
                }
                else if (_dbType == DatabaseType.POSTGRES)
                {
                    _pgconn = new NpgsqlConnection();
                    ((DbConnection)_pgconn).ConnectionString = constr;
                    ((DbConnection)_pgconn).StateChange += new StateChangeEventHandler(_pgconn_StateChange);
                    ((DbConnection)_pgconn).Open();
                }

                LogCls.writeLog("", "ConnLog.log", "DatabaseType : " + _dbType + " | DatabaseIp : " + dbipaddr + " | Connection Complete(" + DateTime.Now.ToString() + ")");

                return 0;
            }
            catch(Exception ex)
            {
                LogCls.writeLog("", "ConnLog.log", "DatabaseType : " + _dbType + " | DatabaseIp : " + dbipaddr + " | Connection Fail(" + DateTime.Now.ToString() + ") | Reason : " + ex.Message);

                if (_dbType == DatabaseType.MSSQL)
                {
                    if (_conn.State == System.Data.ConnectionState.Open)
                    {
                        // Connection 객체연결 닫기
                        _conn.Close();

                        // Connection 객체연결 삭제
                        _conn.Dispose();
                    }
                }
                else if (_dbType == DatabaseType.POSTGRES)
                {
                    if (_pgconn.State == ConnectionState.Open)
                    {
                        _pgconn.Close();
                        _pgconn.Dispose();
                    }
                }

                return 99;
            }
            finally
            {
                
            }
        }
        #endregion

        /// <summary>
        /// DB Connection 함수
        /// </summary>
        /// <returns></returns>
        public static int newConnect()
        {
            try
            {
                // 연결되어있다면 종료한다.
                disConnect();

                if (string.IsNullOrEmpty(connectionString))
                {
                    // Connection String이 없다면 연결종료처리한다.
                    return -9;
                }

                if (_dbType == DatabaseType.MSSQL)
                {
                    if (_conn != null && _conn.State == System.Data.ConnectionState.Closed)
                    {
                        _conn = (SqlConnection)new SqlConnection();
                        _conn.ConnectionString = connectionString;
                        _conn.StateChange += _conn_StateChange;
                        _conn.Open();
                    }
                    else
                    {
                        _conn = (SqlConnection)new SqlConnection();
                        _conn.ConnectionString = connectionString;
                        _conn.StateChange += _conn_StateChange;
                        _conn.Open();
                    }                        
                }
                else if (_dbType == DatabaseType.POSTGRES)
                {
                    //_pgconn = new NpgsqlConnection();
                    //((DbConnection)_pgconn).ConnectionString = constr;
                    //((DbConnection)_pgconn).StateChange += new StateChangeEventHandler(_pgconn_StateChange);
                    //((DbConnection)_pgconn).Open();
                }
            }
            catch(Exception ex)
            {
                LogCls.writeLog("", "ConnLog.log", "DatabaseType : " + _dbType + " | DatabaseIp : " + dbipaddr + " | Connection Fail(" + DateTime.Now.ToString() + ") | Reason : " + ex.Message);

                if (_conn.State == System.Data.ConnectionState.Open)
                {
                    // Connection 객체연결 닫기
                    _conn.Close();

                    // Connection 객체연결 삭제
                    _conn.Dispose();
                }

                return -8;
            }

            return 0;
        }

        #region Multi Connection 함수 (MultiConnect)
        public static SqlConnection MultiConnect(string constr)
        {
            if (string.IsNullOrEmpty(constr))
            {
                return null;
            }

            SqlConnection mConn = (SqlConnection)new SqlConnection();

            try
            {
                if (_dbType == DatabaseType.MSSQL)
                {                    
                    mConn.ConnectionString = constr;
                    mConn.StateChange += mConn_StateChange;
                    mConn.Open();

                    return mConn;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                LogCls.writeLog("", "ConnLog.log", "DatabaseType : " + _dbType + " | DatabaseIp : " + dbipaddr + " | Connection Fail(" + DateTime.Now.ToString() + ") | Reason : " + ex.Message);

                if (mConn.State == System.Data.ConnectionState.Open)
                {
                    // Connection 객체연결 닫기
                    mConn.Close();

                    // Connection 객체연결 삭제
                    mConn.Dispose();
                }

                return null;
            }
        }
        #endregion

        #region DB접속을 종료합니다. (disConnect)
        public static void disConnect()
        {
            if (_dbType == DatabaseType.MSSQL)
            {
                if (_conn != null)
                {
                    if (_conn.State == System.Data.ConnectionState.Open)
                    {
                        // Connection 객체연결 닫기
                        _conn.Close();

                        // Connection 객체연결 삭제
                        _conn.Dispose();
                    }
                }
            }
            else if (_dbType == DatabaseType.POSTGRES)
            {
                disConnect_P();
            }
                       
        }
        #endregion

        private static void mConn_StateChange(object sender, StateChangeEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private static void _connlite_StateChange(object sender, StateChangeEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private static void _conn_StateChange(object sender, System.Data.StateChangeEventArgs e)
        {
            //addLog("Connection 상태 : " + e.CurrentState);
        }

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

        #region MS-SQL Section ==============================================

        #region 조회쿼리를 실행합니다. (MSsqlQueryExec)
        /// <summary>
        /// MS-SQL Query를 실행합니다.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static DataSet MSsqlQueryExec(string query)
        {
            DataSet sData = new DataSet();
            SqlDataAdapter myCmdUser = new SqlDataAdapter();
            SqlCommand cmd = _conn.CreateCommand();

            try
            {
                if (newConnect() != 0)
                {
                    LogCls.writeLog("", "ConnLog.log", "Database 접속이 실패하였습니다.");
                    return null;
                }

                cmd.Connection = _conn;
                cmd.CommandText = query;
                cmd.CommandTimeout = 10000;
                myCmdUser.SelectCommand = cmd;
                myCmdUser.Fill(sData);
            }
            catch (SqlException ex)
            {
                LogCls.writeLog("", "ConnLog.log", "MSsqlQueryExec : " + ex.Message);
            }
            finally
            {
                myCmdUser.Dispose();
                cmd.Dispose();

                if (_conn.State == System.Data.ConnectionState.Open)
                {
                    // Connection 객체연결 닫기
                    _conn.Close();

                    // Connection 객체연결 삭제
                    _conn.Dispose();
                }
            }

            return sData;
        }
        #endregion

        #region 조회쿼리를 실행합니다. (MSsqlQueryExec) - Connection받는버전
        /// <summary>
        /// MS-SQL Query를 실행합니다.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static DataSet MSsqlQueryExec(SqlConnection _connlocal, string query)
        {
            DataSet sData = new DataSet();
            SqlDataAdapter myCmdUser = new SqlDataAdapter();
            SqlCommand cmd = _connlocal.CreateCommand();

            try
            {
                if (newConnect() != 0)
                {
                    LogCls.writeLog("", "ConnLog.log", "Database 접속이 실패하였습니다.");
                    return null;
                }

                // 초기화
                cmd.Connection = _conn;
                cmd.CommandText = query;
                cmd.CommandTimeout = 10000;
                myCmdUser.SelectCommand = cmd;
                myCmdUser.Fill(sData);

            }
            catch(SqlException ex)
            {
                LogCls.writeLog("", "ConnLog.log", "MSsqlQueryExec(conn) : " + ex.Message);
                return null;
            }
            finally
            {
                if (cmd != null)
                {                    
                    myCmdUser.Dispose();
                    cmd.Dispose();
                }

                if (_conn.State == System.Data.ConnectionState.Open)
                {
                    // Connection 객체연결 닫기
                    _conn.Close();

                    // Connection 객체연결 삭제
                    _conn.Dispose();
                }
            }            

            return sData;
        }
        #endregion

        #region 실행쿼리를 실행합니다. (ExecuteNonQuery) - 파라미터1개
        /// <summary>
        /// 실행(저장, 삭제, 수정)용 쿼리를 실행합니다.
        /// </summary>
        /// <param name="szQuery">쿼리를 받습니다.</param>
        /// <returns>
        /// 쿼리없는경우 : 7
        /// 데이터베이스연결실패 : 1
        /// 성공 : 0
        /// 예외오류 : 9
        /// </returns>
        public static int ExecuteNonQuery(string szQuery)
        {
            DbCommand dcd = null;

            try
            {
                if (newConnect() != 0)
                {
                    LogCls.writeLog("", "ConnLog.log", "Database 접속이 실패하였습니다.");
                    return 9;
                }

                dcd = _conn.CreateCommand();

                // 트랜잭션을 시작합니다.
                dcd.Transaction = _conn.BeginTransaction();

                // DB TimeOut은 3분으로 셋팅
                dcd.Connection = _conn;
                dcd.CommandTimeout = 180;
                dcd.CommandText = szQuery;
                int sRow = dcd.ExecuteNonQuery();

                // 트랜잭션을 커밋합니다.
                dcd.Transaction.Commit();

                return 0;

            }
            catch (Exception ex)
            {
                // 트랜잭션을 RollBack 합니다.
                //dcd.Transaction.Rollback();
                LogCls.writeLog("", "ConnLog.log", "ExecuteNonQuery : " + ex.Message);

                return 9;
            }
            finally
            {
                // DB커넥션을 종료합니다.
                if (dcd != null)
                {
                    dcd.Dispose();
                }

                if (_conn.State == System.Data.ConnectionState.Open)
                {
                    // Connection 객체연결 닫기
                    _conn.Close();

                    // Connection 객체연결 삭제
                    _conn.Dispose();
                }
            }
        }
        #endregion 

        #region 실행쿼리를 실행합니다. (ExecuteNonQuery) - 파라미터1개
        /// <summary>
        /// 실행(저장, 삭제, 수정)용 쿼리를 실행합니다.
        /// </summary>
        /// <param name="szQuery">쿼리를 받습니다.</param>
        /// <returns>
        /// 쿼리없는경우 : 7
        /// 데이터베이스연결실패 : 1
        /// 성공 : 0
        /// 예외오류 : 9
        /// </returns>
        public static int ExecuteNonQuery(string szQuery, out string rmsg)
        {
            DbCommand dcd = null;
            rmsg = string.Empty;

            try
            {
                if (newConnect() != 0)
                {
                    LogCls.writeLog("", "ConnLog.log", "Database 접속이 실패하였습니다.");
                    return 9;
                }

                dcd = _conn.CreateCommand();

                // 트랜잭션을 시작합니다.
                dcd.Transaction = _conn.BeginTransaction();

                // DB TimeOut은 3분으로 셋팅
                dcd.Connection = _conn;
                dcd.CommandTimeout = 180;
                dcd.CommandText = szQuery;
                int sRow = dcd.ExecuteNonQuery();

                // 트랜잭션을 커밋합니다.
                dcd.Transaction.Commit();

                rmsg = "";
                return 0;

            }
            catch (Exception ex)
            {
                // 트랜잭션을 RollBack 합니다.
                //dcd.Transaction.Rollback();

                rmsg = ex.Message;
                return 9;
            }
            finally
            {
                // DB커넥션을 종료합니다.
                if (dcd != null)
                {
                    dcd.Dispose();
                }

                if (_conn.State == System.Data.ConnectionState.Open)
                {
                    // Connection 객체연결 닫기
                    _conn.Close();

                    // Connection 객체연결 삭제
                    _conn.Dispose();
                }
            }
        }
        #endregion 

        #region 실행쿼리를 실행합니다. (ExecuteNonQuery) - 파라미터 2개
        /// <summary>
        /// 실행(저장, 삭제, 수정)용 쿼리를 실행합니다.
        /// </summary>
        /// <param name="szQuery">쿼리를 받습니다.</param>
        /// <returns>
        /// 쿼리없는경우 : 7
        /// 데이터베이스연결실패 : 1
        /// 성공 : 0
        /// 예외오류 : 9
        /// </returns>
        public static int ExecuteNonQuery(SqlConnection _paraconn, string szQuery, out string rmsg)
        {
            DbCommand dcd = null;
            rmsg = string.Empty;

            try
            {
                if (newConnect() != 0)
                {
                    LogCls.writeLog("", "ConnLog.log", "Database 접속이 실패하였습니다.");
                    return 9;
                }

                dcd = _paraconn.CreateCommand();

                // 트랜잭션을 시작합니다.
                dcd.Transaction = _paraconn.BeginTransaction();

                // DB TimeOut은 3분으로 셋팅
                dcd.Connection = _paraconn;
                dcd.CommandTimeout = 180;
                dcd.CommandText = szQuery;
                int sRow = dcd.ExecuteNonQuery();

                // 트랜잭션을 커밋합니다.
                dcd.Transaction.Commit();

                rmsg = "";
                return 0;

            }
            catch (Exception ex)
            {
                // 트랜잭션을 RollBack 합니다.
                //dcd.Transaction.Rollback();

                rmsg = ex.Message;
                return 9;
            }
            finally
            {
                // DB커넥션을 종료합니다.
                if (dcd != null)
                {
                    dcd.Dispose();
                }

                if (_conn.State == System.Data.ConnectionState.Open)
                {
                    // Connection 객체연결 닫기
                    _conn.Close();

                    // Connection 객체연결 삭제
                    _conn.Dispose();
                }
            }
        }
        #endregion

        #endregion ===========================================================

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

        #endregion ===========================================================
    }
}
