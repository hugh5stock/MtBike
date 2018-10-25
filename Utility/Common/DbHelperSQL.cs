﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Management;

namespace HuRongClub.DBUtility
{
    /// <summary>
    /// 数据访问基础类(基于SQLServer)
    /// Copyright (C) 2004-2012 By Maticsoft
    /// </summary>
    public class DbHelperSQL
    {
        //数据库连接字符串(web.config来配置)
        //public static string connectionString = PubConstant.ConnectionString;
        public static string connectionString = ConfigurationManager.ConnectionStrings["MintBicycleConnectionString"].ToString();


        public DbHelperSQL()
        {
        }

        public DbHelperSQL(string ConnectionString)
        {
            connectionString = ConnectionString;
        }

        #region 公用方法

        public static SqlConnection GetConnection
        {
            get { return new SqlConnection(connectionString); }
        }

        /// <summary>
        /// 判断是否存在某表的某个字段
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="columnName">列名称</param>
        /// <returns>是否存在</returns>
        public static bool ColumnExists(string tableName, string columnName)
        {
            string sql = "select count(1) from syscolumns where [id]=object_id('" + tableName + "') and [name]='" + columnName + "'";
            object res = GetSingle(sql);
            if (res == null)
            {
                return false;
            }
            return Convert.ToInt32(res) > 0;
        }

        public static int GetMinID(string FieldName, string TableName)
        {
            string strsql = "select min(" + FieldName + ") from " + TableName;
            object obj = DbHelperSQL.GetSingle(strsql);
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }

        public static int GetMaxID(string FieldName, string TableName)
        {
            string strsql = "select max(" + FieldName + ")+1 from " + TableName;
            object obj = DbHelperSQL.GetSingle(strsql);
            if (obj == null)
            {
                return 1;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }

        public static bool Exists(string strSql)
        {
            object obj = GetSingle(strSql);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 表是否存在
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public static bool TabExists(string TableName)
        {
            string strsql = "select count(*) from sysobjects where id = object_id(N'[" + TableName + "]') and OBJECTPROPERTY(id, N'IsUserTable') = 1";
            //string strsql = "SELECT count(*) FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + TableName + "]') AND type in (N'U')";
            object obj = GetSingle(strsql);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool Exists(string strSql, params SqlParameter[] cmdParms)
        {
            object obj = GetSingle(strSql, cmdParms);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion

        #region  执行简单SQL语句
        /// 执行查询语句，返回DataTable
        public static DataTable dt_table(string SQLString)
        {
            DataSet myDs;
            myDs = Query(SQLString);
            return myDs == null ? new DataTable() : myDs.Tables[0];
        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string SQLString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// 2012-2-21新增重载，执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="connection">SqlConnection对象</param>
        /// <param name="trans">SqlTransaction事件</param>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(SqlConnection connection, SqlTransaction trans, string SQLString)
        {
            using (SqlCommand cmd = new SqlCommand(SQLString, connection))
            {
                try
                {
                    cmd.Connection = connection;
                    cmd.Transaction = trans;
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    //trans.Rollback();
                    throw e;
                }
            }
        }

        public static int ExecuteSqlByTime(string SQLString, int Times)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        cmd.CommandTimeout = Times;
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// 执行Sql和Oracle滴混合事务
        /// </summary>
        /// <param name="list">SQL命令行列表</param>
        /// <param name="oracleCmdSqlList">Oracle命令行列表</param>
        /// <returns>执行结果 0-由于SQL造成事务失败 -1 由于Oracle造成事务失败 1-整体事务执行成功</returns>
        public static int ExecuteSqlTran(List<CommandInfo> list, List<CommandInfo> oracleCmdSqlList)
        {
            //using (SqlConnection conn = new SqlConnection(connectionString))
            //{
            //    conn.Open();
            //    SqlCommand cmd = new SqlCommand();
            //    cmd.Connection = conn;
            //    SqlTransaction tx = conn.BeginTransaction();
            //    cmd.Transaction = tx;
            //    try
            //    {
            //        foreach (CommandInfo myDE in list)
            //        {
            //            string cmdText = myDE.CommandText;
            //            SqlParameter[] cmdParms = (SqlParameter[])myDE.Parameters;
            //            PrepareCommand(cmd, conn, tx, cmdText, cmdParms);
            //            if (myDE.EffentNextType == EffentNextType.SolicitationEvent)
            //            {
            //                if (myDE.CommandText.ToLower().IndexOf("count(") == -1)
            //                {
            //                    tx.Rollback();
            //                    throw new Exception("违背要求"+myDE.CommandText+"必须符合select count(..的格式");
            //                    //return 0;
            //                }

            //                object obj = cmd.ExecuteScalar();
            //                bool isHave = false;
            //                if (obj == null && obj == DBNull.Value)
            //                {
            //                    isHave = false;
            //                }
            //                isHave = Convert.ToInt32(obj) > 0;
            //                if (isHave)
            //                {
            //                    //引发事件
            //                    myDE.OnSolicitationEvent();
            //                }
            //            }
            //            if (myDE.EffentNextType == EffentNextType.WhenHaveContine || myDE.EffentNextType == EffentNextType.WhenNoHaveContine)
            //            {
            //                if (myDE.CommandText.ToLower().IndexOf("count(") == -1)
            //                {
            //                    tx.Rollback();
            //                    throw new Exception("SQL:违背要求" + myDE.CommandText + "必须符合select count(..的格式");
            //                    //return 0;
            //                }

            //                object obj = cmd.ExecuteScalar();
            //                bool isHave = false;
            //                if (obj == null && obj == DBNull.Value)
            //                {
            //                    isHave = false;
            //                }
            //                isHave = Convert.ToInt32(obj) > 0;

            //                if (myDE.EffentNextType == EffentNextType.WhenHaveContine && !isHave)
            //                {
            //                    tx.Rollback();
            //                    throw new Exception("SQL:违背要求" + myDE.CommandText + "返回值必须大于0");
            //                    //return 0;
            //                }
            //                if (myDE.EffentNextType == EffentNextType.WhenNoHaveContine && isHave)
            //                {
            //                    tx.Rollback();
            //                    throw new Exception("SQL:违背要求" + myDE.CommandText + "返回值必须等于0");
            //                    //return 0;
            //                }
            //                continue;
            //            }
            //            int val = cmd.ExecuteNonQuery();
            //            if (myDE.EffentNextType == EffentNextType.ExcuteEffectRows && val == 0)
            //            {
            //                tx.Rollback();
            //                throw new Exception("SQL:违背要求" + myDE.CommandText + "必须有影响行");
            //                //return 0;
            //            }
            //            cmd.Parameters.Clear();
            //        }
            //        string oraConnectionString = PubConstant.GetConnectionString("ConnectionStringPPC");
            //        bool res = OracleHelper.ExecuteSqlTran(oraConnectionString, oracleCmdSqlList);
            //        if (!res)
            //        {
            //            tx.Rollback();
            //            throw new Exception("Oracle执行失败");
            //            // return -1;
            //        }
            //        tx.Commit();
            //        return 1;
            //    }
            //    catch (System.Data.SqlClient.SqlException e)
            //    {
            //        tx.Rollback();
            //        throw e;
            //    }
            //    catch (Exception e)
            //    {
            //        tx.Rollback();
            //        throw e;
            //    }
            //}
            return 0;
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">多条SQL语句</param>
        public static int ExecuteSqlTran(List<String> SQLStringList)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                SqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    int count = 0;
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        string strsql = SQLStringList[n];
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            count += cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                    return count;
                }
                catch
                {
                    tx.Rollback();
                    return 0;
                }
            }
        }

        /// <summary>
        /// 执行带一个存储过程参数的的SQL语句。
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string SQLString, string content)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(SQLString, connection);
                System.Data.SqlClient.SqlParameter myParameter = new System.Data.SqlClient.SqlParameter("@content", SqlDbType.NText);
                myParameter.Value = content;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    throw e;
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// 执行带一个存储过程参数的的SQL语句。
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
        /// <returns>影响的记录数</returns>
        public static object ExecuteSqlGet(string SQLString, string content)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(SQLString, connection);
                System.Data.SqlClient.SqlParameter myParameter = new System.Data.SqlClient.SqlParameter("@content", SqlDbType.NText);
                myParameter.Value = content;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    object obj = cmd.ExecuteScalar();
                    if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                    {
                        return null;
                    }
                    else
                    {
                        return obj;
                    }
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    throw e;
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// 向数据库里插入图像格式的字段(和上面情况类似的另一种实例)
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <param name="fs">图像字节,数据库的字段类型为image的情况</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSqlInsertImg(string strSQL, byte[] fs)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(strSQL, connection);
                System.Data.SqlClient.SqlParameter myParameter = new System.Data.SqlClient.SqlParameter("@fs", SqlDbType.Image);
                myParameter.Value = fs;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    throw e;
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public static object GetSingle(string SQLString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }

        public static object GetSingle(string SQLString, int Times)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        cmd.CommandTimeout = Times;
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>SqlDataReader</returns>
        public static SqlDataReader ExecuteReader(string strSQL)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(strSQL, connection);
            try
            {
                connection.Open();
                SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return myReader;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    SqlDataAdapter command = new SqlDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }

        public static DataSet Query(string SQLString, int Times)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    SqlDataAdapter command = new SqlDataAdapter(SQLString, connection);
                    command.SelectCommand.CommandTimeout = Times;
                    command.Fill(ds, "ds");
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }

        /// <summary>
        /// 2012-2-21新增重载，执行查询语句，返回DataSet
        /// </summary>
        /// <param name="connection">SqlConnection对象</param>
        /// <param name="trans">SqlTransaction事务</param>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(SqlConnection connection, SqlTransaction trans, string SQLString)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlDataAdapter command = new SqlDataAdapter(SQLString, connection);
                command.SelectCommand.Transaction = trans;
                command.Fill(ds, "ds");
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return ds;
        }

        #endregion

        #region 执行带参数的SQL语句

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string SQLString, params SqlParameter[] cmdParms)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// 2012-2-29新增重载，执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="connection">SqlConnection对象</param>
        /// <param name="trans">SqlTransaction对象</param>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(SqlConnection connection, SqlTransaction trans, string SQLString, params SqlParameter[] cmdParms)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                try
                {
                    PrepareCommand(cmd, connection, trans, SQLString, cmdParms);
                    int rows = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    return rows;
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    //trans.Rollback();
                    throw e;
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
        public static void ExecuteSqlTran(Hashtable SQLStringList)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    SqlCommand cmd = new SqlCommand();
                    try
                    {
                        //循环
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            SqlParameter[] cmdParms = (SqlParameter[])myDE.Value;
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
        public static int ExecuteSqlTran(System.Collections.Generic.List<CommandInfo> cmdList)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    SqlCommand cmd = new SqlCommand();
                    try
                    {
                        int count = 0;
                        //循环
                        foreach (CommandInfo myDE in cmdList)
                        {
                            string cmdText = myDE.CommandText;
                            SqlParameter[] cmdParms = (SqlParameter[])myDE.Parameters;
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);

                            if (myDE.EffentNextType == EffentNextType.WhenHaveContine || myDE.EffentNextType == EffentNextType.WhenNoHaveContine)
                            {
                                if (myDE.CommandText.ToLower().IndexOf("count(") == -1)
                                {
                                    trans.Rollback();
                                    return 0;
                                }

                                object obj = cmd.ExecuteScalar();
                                bool isHave = false;
                                if (obj == null && obj == DBNull.Value)
                                {
                                    isHave = false;
                                }
                                isHave = Convert.ToInt32(obj) > 0;

                                if (myDE.EffentNextType == EffentNextType.WhenHaveContine && !isHave)
                                {
                                    trans.Rollback();
                                    return 0;
                                }
                                if (myDE.EffentNextType == EffentNextType.WhenNoHaveContine && isHave)
                                {
                                    trans.Rollback();
                                    return 0;
                                }
                                continue;
                            }
                            int val = cmd.ExecuteNonQuery();
                            count += val;
                            if (myDE.EffentNextType == EffentNextType.ExcuteEffectRows && val == 0)
                            {
                                trans.Rollback();
                                return 0;
                            }
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                        return count;
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="cmdList">SQL语句的CommandInfo</param>
        /// <remarks>只使用了CommandInfo-EffentNextType.ExcuteEffectRows, 其它项暂不支持</remarks>
        public static int ExecuteSqlTran4Indentity(System.Collections.Generic.List<CommandInfo> cmdList)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    SqlCommand cmd = new SqlCommand();
                    try
                    {
                        int count = 0;
                        int indentity = 0;
                        //循环
                        foreach (CommandInfo commandInfo in cmdList)
                        {
                            string cmdText = commandInfo.CommandText;
                            SqlParameter[] cmdParms = (SqlParameter[])commandInfo.Parameters;
                            foreach (SqlParameter parameter in cmdParms)
                            {
                                if (parameter.Direction == ParameterDirection.InputOutput)
                                {
                                    parameter.Value = indentity;
                                }
                            }
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            //执行 并计数
                            int val = cmd.ExecuteNonQuery();
                            count += val;
                            if (commandInfo.EffentNextType == EffentNextType.ExcuteEffectRows && val == 0)
                            {
                                //未执行成功进行回滚
                                trans.Rollback();
                                return 0;
                            }
                            foreach (SqlParameter parameter in cmdParms)
                            {
                                if (parameter.Direction == ParameterDirection.Output)
                                {
                                    indentity = Convert.ToInt32(parameter.Value);
                                }
                            }
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                        return count;
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="cmdList">SQL语句的CommandInfo</param>
        /// <param name="trans">外部事务对象</param>
        /// <remarks>警告:内部不触发事务的提交和回滚</remarks>
        /// <remarks>只使用了CommandInfo-EffentNextType.ExcuteEffectRows, 其它项暂不支持</remarks>
        public static int ExecuteSqlTran4Indentity(System.Collections.Generic.List<CommandInfo> cmdList, SqlTransaction trans)
        {
            SqlCommand cmd = new SqlCommand();
            int count = 0;
            int indentity = 0;
            //循环
            foreach (CommandInfo commandInfo in cmdList)
            {
                string cmdText = commandInfo.CommandText;
                SqlParameter[] cmdParms = (SqlParameter[])commandInfo.Parameters;
                foreach (SqlParameter parameter in cmdParms)
                {
                    if (parameter.Direction == ParameterDirection.InputOutput)
                    {
                        parameter.Value = indentity;
                    }
                }
                PrepareCommand(cmd, trans.Connection, trans, cmdText, cmdParms);
                //执行 并计数
                int val = cmd.ExecuteNonQuery();
                count += val;
                if (commandInfo.EffentNextType == EffentNextType.ExcuteEffectRows && val == 0)
                {
                    //未执行成功抛出异常, 由外部进行事务回滚
                    throw new SqlExecutionException("DbHelperSQL.ExecuteSqlTran4Indentity - [" + cmd.CommandText + "] 未执行成功!");
                }
                foreach (SqlParameter parameter in cmdParms)
                {
                    if (parameter.Direction == ParameterDirection.Output)
                    {
                        indentity = Convert.ToInt32(parameter.Value);
                    }
                }
                cmd.Parameters.Clear();
            }
            return count;
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。[已过时, 请使用ExecuteSqlTran4Indentity]
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
        [Obsolete]
        public static void ExecuteSqlTranWithIndentity(System.Collections.Generic.List<CommandInfo> SQLStringList)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    SqlCommand cmd = new SqlCommand();
                    try
                    {
                        int indentity = 0;
                        //循环
                        foreach (CommandInfo myDE in SQLStringList)
                        {
                            string cmdText = myDE.CommandText;
                            SqlParameter[] cmdParms = (SqlParameter[])myDE.Parameters;
                            foreach (SqlParameter q in cmdParms)
                            {
                                if (q.Direction == ParameterDirection.InputOutput)
                                {
                                    q.Value = indentity;
                                }
                            }
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            foreach (SqlParameter q in cmdParms)
                            {
                                if (q.Direction == ParameterDirection.Output)
                                {
                                    indentity = Convert.ToInt32(q.Value);
                                }
                            }
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
        public static void ExecuteSqlTranWithIndentity(Hashtable SQLStringList)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    SqlCommand cmd = new SqlCommand();
                    try
                    {
                        int indentity = 0;
                        //循环
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            SqlParameter[] cmdParms = (SqlParameter[])myDE.Value;
                            foreach (SqlParameter q in cmdParms)
                            {
                                if (q.Direction == ParameterDirection.InputOutput)
                                {
                                    q.Value = indentity;
                                }
                            }
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            foreach (SqlParameter q in cmdParms)
                            {
                                if (q.Direction == ParameterDirection.Output)
                                {
                                    indentity = Convert.ToInt32(q.Value);
                                }
                            }
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public static object GetSingle(string SQLString, params SqlParameter[] cmdParms)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="commandInfo">SQL语句的CommandInfo</param>
        /// <param name="trans">外部事务对象</param>
        /// <remarks>警告: 内部无异常处理和using语句, 请务必在外部实现</remarks>
        /// <returns>查询结果（object）</returns>
        public static object GetSingle4Trans(CommandInfo commandInfo, SqlTransaction trans)
        {
            SqlCommand cmd = new SqlCommand();
            string cmdText = commandInfo.CommandText;
            SqlParameter[] cmdParms = (SqlParameter[])commandInfo.Parameters;
            PrepareCommand(cmd, trans.Connection, trans, cmdText, cmdParms);
            object obj = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                return null;
            }
            else
            {
                return obj;
            }
        }

        /// <summary>
        /// 执行查询语句，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>SqlDataReader</returns>
        public static SqlDataReader ExecuteReader(string SQLString, params SqlParameter[] cmdParms)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            try
            {
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return myReader;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                throw e;
            }
            //			finally
            //			{
            //				cmd.Dispose();
            //				connection.Close();
            //			}
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString, params SqlParameter[] cmdParms)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    return ds;
                }
            }
        }

        /// <summary>
        /// 2012-2-21新增重载，执行查询语句，返回DataSet
        /// </summary>
        /// <param name="connection">SqlConnection对象</param>
        /// <param name="trans">SqlTransaction事务</param>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(SqlConnection connection, SqlTransaction trans, string SQLString, params SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, trans, SQLString, cmdParms);
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                DataSet ds = new DataSet();
                try
                {
                    da.Fill(ds, "ds");
                    cmd.Parameters.Clear();
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    //trans.Rollback();
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }

        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, string cmdText, SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {
                foreach (SqlParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }

        #endregion

        #region 存储过程操作

        /// <summary>
        /// 执行存储过程，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlDataReader</returns>
        public static SqlDataReader RunProcedure(string storedProcName, IDataParameter[] parameters)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlDataReader returnReader;
            connection.Open();
            SqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.CommandType = CommandType.StoredProcedure;
            returnReader = command.ExecuteReader(CommandBehavior.CloseConnection);
            return returnReader;
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="tableName">DataSet结果中的表名</param>
        /// <returns>DataSet</returns>
        public static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                SqlDataAdapter sqlDA = new SqlDataAdapter();
                sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                sqlDA.Fill(dataSet, tableName);
                connection.Close();
                return dataSet;
            }
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="tableName">DataSet结果中的表名</param>
        /// <returns>DataSet</returns>
        public static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName, out int returnValue)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                SqlDataAdapter sqlDA = new SqlDataAdapter();
                SqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
                sqlDA.SelectCommand = command;
                sqlDA.Fill(dataSet, tableName);
                returnValue = (int)command.Parameters["ReturnValue"].Value;
                connection.Close();
                return dataSet;
            }
        }

        public static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName, int Times)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                SqlDataAdapter sqlDA = new SqlDataAdapter();
                sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                sqlDA.SelectCommand.CommandTimeout = Times;
                sqlDA.Fill(dataSet, tableName);
                connection.Close();
                return dataSet;
            }
        }

        /// <summary>
        /// 构建 SqlCommand 对象(用来返回一个结果集，而不是一个整数值)
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlCommand</returns>
        private static SqlCommand BuildQueryCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = new SqlCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            foreach (SqlParameter parameter in parameters)
            {
                if (parameter != null)
                {
                    // 检查未分配值的输出参数,将其分配以DBNull.Value.
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    command.Parameters.Add(parameter);
                }
            }

            return command;
        }

        /// <summary>
        /// 执行存储过程，返回影响的行数
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="rowsAffected">影响的行数</param>
        /// <returns></returns>
        public static int RunProcedure(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                int result;
                connection.Open();
                SqlCommand command = BuildIntCommand(connection, storedProcName, parameters);
                rowsAffected = command.ExecuteNonQuery();
                result = (int)command.Parameters["ReturnValue"].Value;
                //Connection.Close();
                return result;
            }
        }

        /// <summary>
        /// 创建 SqlCommand 对象实例(用来返回一个整数值)
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlCommand 对象实例</returns>
        private static SqlCommand BuildIntCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.Parameters.Add(new SqlParameter("ReturnValue",
                SqlDbType.Int, 4, ParameterDirection.ReturnValue,
                false, 0, 0, string.Empty, DataRowVersion.Default, null));
            return command;
        }

        #endregion

        #region 添加一个传连接字符串的简单执行sql语句 2009-2-4

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="strConnectionString">连接字符串</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string SQLString, string strConnectionString, params SqlParameter[] cmdParms)
        {
            using (SqlConnection connection = new SqlConnection(strConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <param name="strConnectionString">连接字符串</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString, string strConnectionString)
        {
            using (SqlConnection connection = new SqlConnection(strConnectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    SqlDataAdapter command = new SqlDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }

        #endregion

        #region 创建参数 SqlParameter

        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="ParamName">参数名称</param>
        /// <param name="DbType">参数类型</param>
        /// <param name="Size">参数大小</param>
        /// <param name="Direction">参数方向</param>
        /// <param name="Value">参数值</param>
        /// <returns>新的 parameter 对象</returns>
        private static SqlParameter CreateParam(string ParamName, SqlDbType DbType,
            Int32 Size, ParameterDirection Direction, object Value)
        {
            SqlParameter param;
            if (Size > 0)
            {   ///使用size创建大小不为0的参数
                param = new SqlParameter(ParamName, DbType, Size);
            }
            else
            {
                param = new SqlParameter(ParamName, DbType);
            }
            ///创建输出类型参数
            param.Direction = Direction;
            if (!(Direction == ParameterDirection.Output && Value == null))
            {
                param.Value = Value;
            }
            ///返回参数
            return param;
        }

        /// <summary>
        /// 创建输入类型参数
        /// </summary>
        /// <param name="ParamName">参数名称</param>
        /// <param name="DbType">参数类型</param></param>
        /// <param name="Size">参数大小</param>
        /// <param name="Value">参数值</param>
        /// <returns>新的parameter 对象</returns>
        public static SqlParameter CreateInParam(string ParamName,
            SqlDbType DbType, int Size, object Value)
        {
            return CreateParam(ParamName, DbType, Size, ParameterDirection.Input, Value);
        }

        /// <summary>
        /// 创建输出类型参数
        /// </summary>
        /// <param name="ParamName">参数名称</param>
        /// <param name="DbType">参数类型</param>
        /// <param name="Size">参数大小</param>
        /// <returns>新的 parameter 对象</returns>
        public static SqlParameter CreateOutParam(string ParamName,
            SqlDbType DbType, int Size)
        {
            return CreateParam(ParamName, DbType, Size, ParameterDirection.Output, null);
        }

        /// <summary>
        /// 创建输出类型参数
        /// </summary>
        /// <param name="ParamName">参数名称</param>
        /// <param name="DbType">参数类型</param>
        /// <param name="Size">参数大小</param>
        /// <returns>新的 parameter 对象</returns>
        public static SqlParameter CreateInputOutParam(string ParamName,
            SqlDbType DbType, int Size, object Value)
        {
            return CreateParam(ParamName, DbType, Size, ParameterDirection.InputOutput, Value);
        }

        /// <summary>
        /// 创建返回类型参数
        /// </summary>
        /// <param name="ParamName">参数名称</param>
        /// <param name="DbType">参数类型</param>
        /// <param name="Size">参数大小</param>
        /// <returns>新的 parameter 对象</returns>
        public static SqlParameter CreateReturnParam(string ParamName,
            SqlDbType DbType, int Size)
        {
            return CreateParam(ParamName, DbType, Size, ParameterDirection.ReturnValue, null);
        }

        #endregion

        /// <summary>
        /// 执行Sql 命令
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">sql语句/参数化sql语句/存储过程名</param>
        /// <param name="commandParameters">参数</param>
        /// <returns>执行结果对象</returns>
        public static object ExecuteScalar(CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, commandType, conn, commandText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 设置一个等待执行的SqlCommand对象
        /// </summary>
        /// <param name="cmd">SqlCommand 对象，不允许空对象</param>
        /// <param name="conn">SqlConnection 对象，不允许空对象</param>
        /// <param name="commandText">Sql 语句</param>
        /// <param name="cmdParms">SqlParameters  对象,允许为空对象</param>
        private static void PrepareCommand(SqlCommand cmd, CommandType commandType, SqlConnection conn, string commandText, SqlParameter[] cmdParms)
        {
            //打开连接
            if (conn.State != ConnectionState.Open)
                conn.Open();

            //设置SqlCommand对象
            cmd.Connection = conn;
            cmd.CommandText = commandText;
            cmd.CommandType = commandType;

            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }

        
        /// <summary>
        /// 【已过期，请使用QuerySqlByRowNumDontToLower】
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="selectFields"></param>
        /// <param name="sortString"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        [Obsolete]
        public static DataSet QuerySqlByRowNum(string sqlString, string selectFields, string sortString, int pageSize, int pageIndex, ref int recordCount)
        {
            sqlString = sqlString.Trim().ToLower();
            if (!sqlString.ToLower().StartsWith("select"))
            {
                throw new Exception("查询语句必须以Select开始");
            }
            if (recordCount < 0)
            {
                string str = "select count(*) from  ( {0} )  as  a ";
                recordCount = Convert.ToInt32(DbHelperSQL.ExecuteScalar(CommandType.Text, string.Format(str, sqlString), null));
            }
            string str2 = " order by (select 1)";
            if ((sortString != "") && (sortString != null))
            {
                str2 = string.Format("ORDER BY {0}", sortString);
            }
            string str3 = "";
            str3 = string.Format("select * from ( select ROW_NUMBER() OVER(" + str2 + ") AS rownum,* from ( select {0} from ({1}) as a ) as b) as c where rownum between {2} and {3}", new object[] { selectFields, sqlString, (pageIndex * pageSize) + 1, (pageIndex + 1) * pageSize });
            return DbHelperSQL.Query(str3);
        }

       /// <summary>
        /// 分页方法，不带parameters
       /// </summary>
       /// <param name="sqlString"></param>
       /// <param name="selectFields"></param>
       /// <param name="sortString"></param>
       /// <param name="pageSize"></param>
       /// <param name="pageIndex"></param>
       /// <param name="recordCount"></param>
       /// <returns></returns>
        public static DataSet QuerySqlByRowNumDontToLower(string sqlString, string selectFields, string sortString, int pageSize, int pageIndex, ref int recordCount)
        {
            sqlString = sqlString.Trim().ToLower();
            if (!sqlString.ToLower().StartsWith("select"))
            {
                throw new Exception("查询语句必须以Select开始");
            }
            if (recordCount < 0)
            {
                string str = "select count(*) from  ( {0} )  as  a ";
                recordCount = Convert.ToInt32(DbHelperSQL.ExecuteScalar(CommandType.Text, string.Format(str, sqlString), null));
            }
            string str2 = " order by (select 1)";
            if ((sortString != "") && (sortString != null))
            {
                str2 = string.Format("ORDER BY {0}", sortString);
            }
            string str3 = "";
            str3 = string.Format("select * from ( select ROW_NUMBER() OVER(" + str2 + ") AS rownum,* from ( select {0} from ({1}) as a ) as b) as c where rownum between {2} and {3}", new object[] { selectFields, sqlString, (pageIndex * pageSize) + 1, (pageIndex + 1) * pageSize });
            return DbHelperSQL.Query(str3);
        }

        /// <summary>
        /// 【过期，请使用 QuerySqlByRowNumDontToLower】分页查询 
        /// 
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="selectFields"></param>
        /// <param name="sortString"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
       
        [Obsolete]
        public static DataSet QuerySqlByRowNum(string sqlString, string selectFields, string sortString, int pageSize, int pageIndex, ref int recordCount, SqlParameter[] parameters)
        {
            sqlString = sqlString.Trim().ToLower();
            if (!sqlString.ToLower().StartsWith("select"))
            {
                throw new Exception("查询语句必须以Select开始");
            }
            if (recordCount < 0)
            {
                string str = "select count(*) from  ( {0} )  as  a ";
                recordCount = Convert.ToInt32(DbHelperSQL.ExecuteScalar(CommandType.Text, string.Format(str, sqlString), parameters));
            }
            string str2 = " order by (select 1)";
            if ((sortString != "") && (sortString != null))
            {
                str2 = string.Format("ORDER BY {0}", sortString);
            }
            string str3 = "";
            str3 = string.Format("select * from ( select ROW_NUMBER() OVER(" + str2 + ") AS rownum,* from ( select {0} from ({1}) as a ) as b) as c where rownum between {2} and {3}", new object[] { selectFields, sqlString, (pageIndex * pageSize) + 1, (pageIndex + 1) * pageSize });
            return DbHelperSQL.Query(str3, parameters);
        }

        /// <summary>
        /// 分页查询，不验证 是否存在 select
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="selectFields"></param>
        /// <param name="sortString"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DataSet QuerySqlByRowNumDontToLower(string sqlString, string selectFields, string sortString, int pageSize, int pageIndex, ref int recordCount, SqlParameter[] parameters)
        {
            if (recordCount < 0)
            {
                string str = "select count(*) from  ( {0} )  as  a ";
                recordCount = Convert.ToInt32(DbHelperSQL.ExecuteScalar(CommandType.Text, string.Format(str, sqlString), parameters));
            }
            string str2 = " order by (select 1)";
            if ((sortString != "") && (sortString != null))
            {
                str2 = string.Format("ORDER BY {0}", sortString);
            }
            string str3 = "";
            str3 = string.Format("select * from ( select ROW_NUMBER() OVER(" + str2 + ") AS rownum,* from ( select {0} from ({1}) as a ) as b) as c where rownum between {2} and {3}", new object[] { selectFields, sqlString, (pageIndex * pageSize) + 1, (pageIndex + 1) * pageSize });
            return DbHelperSQL.Query(str3, parameters);
        }


        public static object GetSingle(SqlConnection connection, SqlTransaction trans, string SQLString, params SqlParameter[] cmdParms)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                try
                {
                    PrepareCommand(cmd, connection, trans, SQLString, cmdParms);
                    object obj = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                    if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                    {
                        return null;
                    }
                    else
                    {
                        return obj;
                    }
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    //trans.Rollback();
                    throw e;
                }
            }
        }

        /// <summary>
        /// DataTable 批量插入到数据库
        /// 注意：tableName 必须与插入的表名一致
        /// Author:Jerry.Li Time:2016/5/9
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="dt"></param>
        public static void BulkToDB(string tableName, DataTable dt)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    try
                    {
                        bulkCopy.DestinationTableName = tableName;
                        bulkCopy.BatchSize = dt.Rows.Count;

                        connection.Open();

                        if (dt != null && dt.Rows.Count != 0)
                            bulkCopy.WriteToServer(dt);
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// 批量插入其他表数据到新表
        /// Sql 示例：insert into BulkTestTable (Id,UserName,Pwd) SELECT nc.Id, nc.UserName,nc.Pwd FROM @NewBulkTestTvp AS nc
        /// Author:Jerry.Li Time:2016/5/9
        /// </summary>
        /// <param name="Sql">Sql 脚本</param>
        /// <param name="dt"></param>
        public static void TableValuedToDB(string Sql, DataTable dt)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(Sql, connection))
                {
                    try
                    {
                        SqlParameter catParam = cmd.Parameters.AddWithValue("@NewBulkTestTvp", dt);
                        catParam.SqlDbType = SqlDbType.Structured;
                        //表值参数的名字叫BulkUdt，在上面的建立测试环境的SQL中有。
                        catParam.TypeName = "dbo.BulkUdt";

                        connection.Open();
                        if (dt != null && dt.Rows.Count != 0)
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }
    }
}