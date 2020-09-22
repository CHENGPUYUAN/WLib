﻿/*---------------------------------------------------------------- 
// auth： Windragon
// date： 2018
// desc： None
// mdfy:  None
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using WLib.Database.DbBase;

namespace WLib.Database
{
    /// <summary>
    /// 实现<see cref="IDbConnection"/>，并且提供连接、使用SQL操作不同类型的数据库(数据源)的方法；
    /// <para>同时提供构建各类连接字符串的方法；</para>
    /// </summary>
    public partial class DbHelper : IDbConnection
    {
        #region 私有成员
        /// <summary>
        /// 数据库连接
        /// </summary>
        private DbConnection _connection;
        /// <summary>
        /// 表示一组方法，这些方法用于创建提供程序对数据源类的实现的实例
        /// </summary>
        private readonly DbProviderFactory _providerFactory;
        #endregion


        #region 属性事件
        /// <summary>
        /// 执行一条命令的超时时间（以秒为单位）
        /// </summary>
        public int CommandTimeOut { get; set; }
        /// <summary>
        /// 连接数据库的超时时间（以秒为单位）
        /// </summary>
        public int ConnectionTimeOut => Connection.ConnectionTimeout;
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// 数据库连接
        /// </summary>
        public DbConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = _providerFactory.CreateConnection();
                    if (_connection == null)
                        throw new Exception("通过ProviderFactory创建数据库连接失败！");

                    _connection.ConnectionString = ConnectionString;
                    _connection.Open();
                }
                else if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                else if (_connection.State == ConnectionState.Broken)
                {
                    _connection.Close();
                    _connection.Open();
                }

                return _connection;
            }
        }
        /// <summary>
        /// 执行SQL语句之前的事件
        /// </summary>
        public EventHandler<PreExcuteEventArgs> PreExcute;
        /// <summary>
        /// 触发执行SQL语句之前的事件处理
        /// </summary>
        /// <param name="description">执行的sql操作的描述</param>
        /// <param name="sql"></param>
        protected void OnPreExcute(string description, string sql) => PreExcute?.Invoke(this, new PreExcuteEventArgs(description, sql));
        #endregion


        #region 构造函数
        /// <summary>
        /// 提供连接和使用SQL操作不同类型的数据库(数据源)的方法
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="providerType">数据库类型</param>
        /// <param name="commandTimeOut">执行一条命令的超时时间（以秒为单位）</param>
        protected DbHelper(string connectionString, EDbProviderType providerType, int commandTimeOut = 30)
        {
            ConnectionString = connectionString;
            CommandTimeOut = commandTimeOut;
            _providerFactory = ProviderFactory.GetDbProviderFactory(providerType);
            if (_providerFactory == null)
                throw new ArgumentException($"无法加载数据源类型为“{providerType.ToString()}”的数据库操作对象(ProviderFactory)");
        }
        #endregion


        #region 查询方法
        /// <summary>
        /// 连接数据源，执行SQL语句并返回状态值
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExcNonQuery(string sql)
        {
            return ExcNonQuery(sql, null);
        }
        /// <summary>
        /// 连接数据源，执行多条SQL语句
        /// </summary>
        /// <param name="sqls"></param>
        /// <returns></returns>
        public void ExcNonQuery(IEnumerable<string> sqls)
        {
            OnPreExcute("Excute None Query", sqls.Aggregate((a, b) => a + ";" + b));
            DbCommand dbCommand = _providerFactory.CreateCommand();
            dbCommand.Connection = Connection;
            dbCommand.CommandTimeout = CommandTimeOut;
            foreach (var sql in sqls)
            {
                dbCommand.CommandText = sql;
                dbCommand.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 连接数据源，执行SQL语句并返回状态值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="dbParameters"></param>
        /// <returns></returns>
        public int ExcNonQuery(string sql, params DbParameter[] dbParameters)
        {
            OnPreExcute("Excute None Query", sql);
            DbCommand dbCommand = _providerFactory.CreateCommand();
            dbCommand.Connection = Connection;
            dbCommand.CommandTimeout = CommandTimeOut;
            dbCommand.CommandText = sql;
            if (dbParameters != null)
                dbCommand.Parameters.AddRange(dbParameters);
            return dbCommand.ExecuteNonQuery();
        }

        /// <summary> 
        /// 连接数据源，执行SQL语句并返回Dataset
        /// </summary> 
        /// <param name="sql">SQL语句</param> 
        /// <returns>返回数据集</returns> 
        public DataSet GetDataset(string sql)
        {
            return GetDataset(sql, null);
        }
        /// <summary> 
        /// 连接数据源，执行SQL语句并返回Dataset
        /// </summary> 
        /// <param name="sql">SQL语句</param> 
        /// <param name="dbParameters">参数集合</param> 
        /// <returns>返回数据集</returns> 
        public DataSet GetDataset(string sql, params DbParameter[] dbParameters)
        {
            OnPreExcute("Get Data Set", sql);
            DbDataAdapter adapter = _providerFactory.CreateDataAdapter();
            DbCommand dbCommand = _providerFactory.CreateCommand();
            dbCommand.Connection = Connection;
            dbCommand.CommandTimeout = CommandTimeOut;
            dbCommand.CommandText = sql;
            if (dbParameters != null)
                dbCommand.Parameters.AddRange(dbParameters);
            adapter.SelectCommand = dbCommand;

            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            return dataSet;
        }

        /// <summary>
        /// 连接数据源，执行SQL语句并返回DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql)
        {
            return GetDataset(sql).Tables[0];
        }
        /// <summary>
        /// 连接数据源，执行SQL语句并返回DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="tableName">表格名称</param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql, string tableName)
        {
            var dataTable = GetDataset(sql).Tables[0];
            dataTable.TableName = tableName;
            return dataTable;
        }
        /// <summary>
        /// 连接数据源，执行SQL语句并返回DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="dbParameters"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql, params DbParameter[] dbParameters)
        {
            return GetDataset(sql, dbParameters).Tables[0];
        }

        /// <summary>
        /// 连接数据源，执行查询得到第一行第一列的值
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public object ExcScalar(string sql)
        {
            return ExcScalar(sql, null);
        }
        /// <summary>
        /// 连接数据源，执行查询得到第一行第一列的值，强制转换成指定类型的值
        /// （查询结果为空时返回default(T)，一般是null或0）
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public T ExcScalar<T>(string sql)
        {
            var obj = ExcScalar(sql);
            if (obj == null || obj == DBNull.Value)
                return default(T);
            try
            {
                return (T)obj;
            }
            catch (Exception ex) { throw new Exception($"将ExcuteScalar方法执行SQL查询的结果“{obj}”({obj.GetType()})强制转换为类型“{typeof(T)}”的数据失败：{ex.Message}"); }
        }
        /// <summary>
        /// 连接数据源，执行查询得到第一行第一列的值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="dbParameters"></param>
        /// <returns></returns>
        public object ExcScalar(string sql, params DbParameter[] dbParameters)
        {
            OnPreExcute("Excute Scalar", sql);
            DbCommand command = _providerFactory.CreateCommand();
            command.Connection = Connection;
            command.CommandTimeout = CommandTimeOut;
            command.CommandText = sql;
            if (dbParameters != null)
                command.Parameters.AddRange(dbParameters);
            return command.ExecuteScalar();
        }
        #endregion


        #region 实现IDbConnection接口
        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public void Close() => _connection?.Close();

        public int ConnectionTimeout => Connection.ConnectionTimeout;

        public string Database => Connection.Database;

        public ConnectionState State => Connection.State;

        public IDbTransaction BeginTransaction() => Connection.BeginTransaction();

        public IDbTransaction BeginTransaction(IsolationLevel il) => Connection.BeginTransaction(il);

        public IDbCommand CreateCommand() => Connection.CreateCommand();

        public void Dispose() => _connection?.Dispose();

        public void ChangeDatabase(string databaseName) => Connection.ChangeDatabase(databaseName);

        public void Open() => _connection?.Open();
        #endregion
    }
}
