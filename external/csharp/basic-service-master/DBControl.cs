using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;
namespace ServiceMaster
{
    /// <summary>
    /// 连接与操作MS Access 数据库
    /// </summary>
    /// <example>
    /// DBControl.getDataTable("User","UserName,UserAge","UserName Like %DK%");
    /// </example>
    public static class DBControl
    {
        /// <summary>
        /// 数据集
        /// </summary>
        private static DataSet ds;
        /// <summary>
        /// 获得数据库的连接
        /// </summary>
        /// <returns>数据库连接</returns>
        public static OleDbConnection getConnection()
        {
            //string conString =;
            OleDbConnection con = new OleDbConnection(@"provider=Microsoft.Jet.OLEDB.4.0;Data Source="+ Properties.Settings.Default.DBfile+";User ID=admin;Password=;Jet OLEDB:Database Password=19860922");
            return con;
        }
        /// <summary>
        /// 执行Update/Delete/Insert操作
        /// </summary>
        /// <param name="SQLcmd">SQL命令</param>
        public static void ExecuteSQL(string SQLcmd)  
        {
            OleDbConnection con = getConnection();
            OleDbCommand cmd = new OleDbCommand(SQLcmd, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
        /// <summary>
        /// 使用非连接模式加载数据到DataSet ds里面
        /// </summary>
        /// <param name="SQLcommand">SQL命令</param>
        public static void loadDataSet(string SQLcommand)
        {
            OleDbConnection con = getConnection();
            con.Open();
            ds = new DataSet();
            try {
                OleDbDataAdapter adp = new OleDbDataAdapter(SQLcommand, con);
                adp.Fill(ds);
            }
            catch (OleDbException OleDbEx)
            {
                throw OleDbEx;
            }
            finally
            {
                con.Close();
            }
        }
        /// <summary>
        /// 获取加载数据后的DataSet，如果没有加载，则用SQLcommand加载数据
        /// </summary>
        /// <param name="SQLcommand">SQL命令</param>
        /// <returns>加载后的数据集</returns>
        public static DataSet getDataSet(string SQLcommand)
        {
            if (ds==null)
                loadDataSet(SQLcommand);
            return ds;
        }
        /// <summary>
        /// 读取指定数据库中的指定数据表中指定列的数据
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <param name="Columns">列</param>
        /// <returns>对应的查询结果</returns>
        public static DataTable getDataTable(string TableName, string Columns)
        {
            return getDataSet("Select " + Columns + " From " + TableName).Tables[0];            
        }
        /// <summary>
        /// 读取指定数据库中的指定数据表中指定列中满足指定查询条件的数据
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <param name="Columns">列</param>
        /// /// <param name="Conditions">查询条件</param>
        /// <returns>对应的查询结果</returns>
        public static DataTable getDataTable(string TableName, string Columns, string Conditions)
        {
            return getDataSet("Select "+Columns+" From "+TableName+" Where "+Conditions).Tables[0];
        }
        /// <summary>
        /// 读取指定数据库中的指定数据表的数据
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <returns>对应的查询结果</returns>
        public static DataTable getDataTable(string TableName)
        {
            return getDataTable(TableName, "*");
        }
    }
}
