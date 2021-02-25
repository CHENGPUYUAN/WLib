﻿/*---------------------------------------------------------------- 
// auth： Windragon
// date： 2018
// desc： None
// mdfy:  None
// sorc:  https://gitee.com/windr07/WLib
//        https://github.com/Windr07/WLib
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using WLib.ArcGis.GeoDatabase.WorkSpace;

namespace WLib.ArcGis.GeoDatabase.Table
{
    /// <summary>
    /// 提供对表格【<see cref="ITable"/>】数据的获取、增、删、改、查、复制、检查、重命名等方法
    /// </summary>
    public static partial class TableEx
    {
        #region 新增记录
        /// <summary>
        /// 在表中创建多条新记录，新记录是空的需要对其内容执行赋值操作
        /// </summary>
        /// <param name="table">操作的表</param>
        /// <param name="insertCount">创建新记录的数量</param>
        /// <param name="doActionByRows">在保存记录前，对记录执行的操作，整型参数是新增记录的索引</param>
        public static void InsertRows(this ITable table, int insertCount, Action<IRowBuffer, int> doActionByRows)
        {
            var cursor = table.Insert(true);
            var tarRowBuffer = table.CreateRowBuffer();
            for (var i = 0; i < insertCount; i++)
            {
                doActionByRows(tarRowBuffer, i);
                cursor.InsertRow(tarRowBuffer);
            }
            cursor.Flush();
            Marshal.ReleaseComObject(tarRowBuffer);
            Marshal.ReleaseComObject(cursor);
        }
        /// <summary>
        ///  在表类中创建一条新记录，新记录是空的需要对其内容执行赋值操作
        /// </summary>
        /// <param name="table">操作的表</param>
        /// <param name="doActionByRow">在保存记录前，对记录执行的操作</param>
        public static void InsertOneRow(this ITable table, Action<IRowBuffer> doActionByRow)
        {
            var cursor = table.Insert(true);
            var tarRowBuffer = table.CreateRowBuffer();
            doActionByRow(tarRowBuffer);

            cursor.InsertRow(tarRowBuffer);
            cursor.Flush();
            Marshal.ReleaseComObject(tarRowBuffer);
        }
        #endregion


        #region 删除记录
        /// <summary>
        /// 删除所有符合查询条件的记录（使用Update游标方式删除）
        /// </summary>
        /// <param name="table">操作的表</param>
        /// <param name="whereClause">查询条件，注意如果值为空则删除所有记录</param>
        public static void DeleteRows(this ITable table, string whereClause)
        {
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = whereClause;
            var cursor = table.Update(queryFilter, false);
            var row = cursor.NextRow();
            while (row != null)
            {
                cursor.DeleteRow();
                row = cursor.NextRow();
            }
            Marshal.ReleaseComObject(queryFilter);
            Marshal.ReleaseComObject(cursor);
        }
        /// <summary>
        /// 根据查询条件查询要素，按判断条件执行删除操作（使用Update游标方式删除，此方法执行速度较Search方法快）
        /// </summary>
        /// <param name="table">操作的表</param>
        /// <param name="whereClause">查询条件，注意如果值为空则删除所有要素</param>
        /// <param name="isDeleteFunc">判断函数，根据返回值确定是否删除要素（返回值为True则删除）</param>
        public static void DeleteRows(this ITable table, string whereClause, Func<IRow, bool> isDeleteFunc)
        {
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = whereClause;
            var cursor = table.Update(queryFilter, false);
            var row = cursor.NextRow();
            while (row != null)
            {
                if (isDeleteFunc(row))
                    cursor.DeleteRow();
                row = cursor.NextRow();
            }
            Marshal.ReleaseComObject(queryFilter);
            Marshal.ReleaseComObject(cursor);
        }
        ///<summary>  
        ///删除所有符合查询条件的记录（使用ITable.DeleteSearchedRows）
        ///</summary>  
        ///<param name="table">操作的表</param>  
        ///<param name="whereClause">查询条件，注意如果值为空则删除所有记录</param>  
        public static void DeleteRows2(this ITable table, string whereClause)
        {
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = whereClause;
            table.DeleteSearchedRows(queryFilter);
        }
        /// <summary>
        /// 删除所有符合查询条件的记录（执行sql）
        /// </summary>
        /// <param name="table">操作的表</param>
        /// <param name="whereClause">查询条件，注意如果值为空则删除所有记录</param>
        public static void DeleteRows3(this ITable table, string whereClause)
        {
            var dataset = (IDataset)table;
            whereClause = string.IsNullOrEmpty(whereClause) ? "1=1" : whereClause;
            var sql = $"delete from {dataset.Name} where {whereClause}";
            dataset.Workspace.ExecuteSQL(sql);
        }
        #endregion


        #region 更新记录
        /// <summary>
        /// 根据查询条件查询记录，对查询获取的记录执行更新操作
        /// </summary>
        /// <param name="table">查询的表</param>
        /// <param name="whereClause">查询条件，此值为null时查询所有记录</param>
        /// <param name="doFuncByRows">针对记录执行的操作，返回值False表示继续执行，true表示中断操作</param>
        /// <param name="nullRecordException">在查询不到记录时是否抛出异常，默认false</param>
        public static void UpdateRows(this ITable table, string whereClause, Func<IRow, bool> doFuncByRows, bool nullRecordException = false)
        {
            IQueryFilter queryFilter = new QueryFilterClass { WhereClause = whereClause };
            var cursor = table.Update(queryFilter, false);
            IRow row = null;

            if (nullRecordException && table.RowCount(queryFilter) == 0)
                CheckNullToThrowException(table, null, whereClause);

            try
            {
                while ((row = cursor.NextRow()) != null)
                {
                    var @break = doFuncByRows(row);
                    cursor.UpdateRow(row);
                    if (@break) break;
                }
            }
            catch (Exception ex)//抛出更具体的异常信息
            {
                var msgOID = row == null ? null : $"“OID = {row.OID}”的";
                var msgWhereClause = string.IsNullOrEmpty(whereClause) ? null : $"根据条件“{whereClause}”";

                throw new Exception($"在{(table as IDataset)?.Name}表格中，{msgWhereClause}更新{msgOID}记录时出错：{ex.Message}");
            }
            finally
            {
                Marshal.ReleaseComObject(cursor);
            }
        }
        #endregion


        #region 查询记录
        /// <summary>
        /// 查询符合条件的行数
        /// </summary>
        /// <param name="table">查询的表</param>
        /// <param name="whereClause">查询条件</param>
        /// <returns></returns>
        public static int QueryCount(this ITable table, string whereClause = null)
        {
            return table.RowCount(new QueryFilterClass { WhereClause = whereClause });
        }
        /// <summary>
        /// 查询获取表格行
        /// </summary>
        /// <param name="table">查询的表</param>
        /// <param name="whereClause">查询条件</param>
        /// <param name="nullRecordException">在查询不到记录时是否抛出异常，默认false</param>
        /// <returns></returns>
        public static List<IRow> QueryRows(this ITable table, string whereClause = null, bool nullRecordException = false)
        {
            var rows = new List<IRow>();
            var cursor = table.Search(new QueryFilterClass { WhereClause = whereClause }, false);
            IRow row;
            while ((row = cursor.NextRow()) != null)
            {
                rows.Add(row);
            }
            Marshal.ReleaseComObject(cursor);

            if (nullRecordException && rows.Count == 0)
                CheckNullToThrowException(table, null, whereClause);

            return rows;
        }
        /// <summary>
        /// 查找符合条件的第一条记录
        /// </summary>
        /// <param name="table">查询表</param>
        /// <param name="whereClause">查询条件</param>
        /// <param name="nullRecordException">在查询不到记录时是否抛出异常，默认false</param>
        /// <returns></returns>
        public static IRow QueryFirstRow(this ITable table, string whereClause = null, bool nullRecordException = false)
        {
            if (table == null) return null;
            var cursor = table.Search(new QueryFilterClass { WhereClause = whereClause }, false);
            var row = cursor.NextRow();
            Marshal.ReleaseComObject(cursor);

            if (nullRecordException)
                CheckNullToThrowException(table, row, whereClause);

            return row;
        }
        /// <summary>
        ///  根据查询条件查询记录，对查询获取的记录行执行指定操作
        /// </summary>
        /// <param name="table">查询的表</param>
        /// <param name="whereClause">查询条件，此值为null时查询所有记录</param>
        /// <param name="doActionByRows">针对记录执行的操作</param>
        /// <param name="nullRecordException">在查询不到记录时是否抛出异常，默认false</param>
        public static void QueryRows(this ITable table, string whereClause, Action<IRow> doActionByRows, bool nullRecordException = false)
        {
            IQueryFilter queryFilter = new QueryFilterClass { WhereClause = whereClause };
            if (nullRecordException && table.RowCount(queryFilter) == 0)
                CheckNullToThrowException(table, null, whereClause);

            var cursor = table.Search(queryFilter, false);
            IRow row;
            while ((row = cursor.NextRow()) != null)
            {
                doActionByRows(row);
            }
            Marshal.ReleaseComObject(cursor);
        }
        #endregion


        #region 复制记录
        /// <summary>
        /// 从源表格中获取数据添加到目标表格中
        /// </summary>
        /// <param name="sourceTable">源表格</param>
        /// <param name="targetTable">目标表格</param>
        /// <param name="aferInsertEach">每复制一条要素之后执行的操作</param>
        public static void CopyDataToTable(this ITable sourceTable, ITable targetTable, Action<IRowBuffer> aferInsertEach = null)
        {
            var cursor = sourceTable.Search(null, true);
            var row = cursor.NextRow();
            var tarRowCursor = targetTable.Insert(true);
            IRowBuffer tarRowBuffer;
            while (row != null)
            {
                tarRowBuffer = targetTable.CreateRowBuffer();
                var fields = row.Fields;
                for (var i = 0; i < fields.FieldCount; i++)
                {
                    var field = fields.get_Field(i);
                    var index = tarRowBuffer.Fields.FindField(field.Name);
                    if (index != -1 && tarRowBuffer.Fields.get_Field(index).Editable)
                    {
                        tarRowBuffer.set_Value(index, row.get_Value(i));
                    }
                }
                tarRowCursor.InsertRow(tarRowBuffer);
                aferInsertEach?.Invoke(tarRowBuffer);
                row = cursor.NextRow();
            }
            tarRowCursor.Flush();
            Marshal.ReleaseComObject(cursor);
            Marshal.ReleaseComObject(tarRowCursor);
        }
        #endregion


        #region 查询值
        /// <summary>
        /// 获取表格指定字段的唯一值（全部不重复的值）
        /// </summary>
        /// <param name="table">被查询的表格</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="whereClause">条件语句</param>
        /// <returns></returns>
        public static List<object> GetUniqueValues(this ITable table, string fieldName, string whereClause = null)
        {
            var cursor = table.GetSearchCursor(whereClause, fieldName, true);

            IDataStatistics dataStatistics = new DataStatisticsClass();
            dataStatistics.Field = fieldName;
            dataStatistics.Cursor = cursor;
            var enumerator = dataStatistics.UniqueValues;
            enumerator.Reset();

            var uniqueValues = new List<object>();
            while (enumerator.MoveNext())
            {
                uniqueValues.Add(enumerator.Current);
            }
            uniqueValues.Sort();
            return uniqueValues;
        }
        /// <summary>
        /// 获取表格指定字段的唯一字符串值（全部不重复的值）
        /// </summary>
        /// <param name="table">被查询的表格</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="whereClause">条件语句</param>
        /// <returns></returns>
        public static List<string> GetUniqueStrValues(this ITable table, string fieldName, string whereClause = null)
        {
            return GetUniqueValues(table, fieldName, whereClause).Select(v => v.ToString()).ToList(); ;
        }
        /// <summary>
        ///  查找符合条件的第一条记录，并返回记录中指定字段的值
        /// </summary>
        /// <param name="table">查询的表</param>
        /// <param name="queryFiledName">查询的字段（该字段必须存在否则抛出异常）</param>
        /// <param name="whereClause">查询条件</param>
        /// <returns>查询结果的第一条记录的指定字段值，找不到则返回null</returns>
        public static object QueryFirstValue(this ITable table, string queryFiledName, string whereClause = null)
        {
            var fieldIndex = table.FindField(queryFiledName);
            if (fieldIndex < 0)
                throw new Exception("找不到字段：" + queryFiledName);

            object value = null;
            var cursor = table.Search(new QueryFilterClass { WhereClause = whereClause }, true);
            var row = cursor.NextRow();
            if (row != null)
                value = row.get_Value(fieldIndex);

            Marshal.ReleaseComObject(cursor);
            return value;
        }
        /// <summary>
        /// 查找符合条件的第一条记录，并返回记录中指定字段的值，此值为不包含前后空白的字符串或null
        /// </summary>
        /// <param name="table">查询的表</param>
        /// <param name="queryFiledName">查询的字段（该字段必须存在否则抛出异常）</param>
        /// <param name="whereClause">查询条件</param>
        /// <returns>查询结果的第一条记录的指定字段的转成字符串的值，找不到则返回null</returns>
        public static string QueryFirstStringValue(this ITable table, string queryFiledName, string whereClause = null)
        {
            var value = QueryFirstValue(table, queryFiledName, whereClause);
            return (value == null || value == DBNull.Value) ? null : value.ToString().Trim();
        }
        /// <summary>
        /// 查找符合条件的第一条记录，并返回记录中指定若干个字段的值
        /// </summary>
        /// <param name="table">查询的表</param>
        /// <param name="queryFiledNames">查询的字段集合（当此数组为null时返回记录的全部字段值，否则字段必须存在，不存在则会出现异常）</param>
        /// <param name="whereClause">查询条件</param>
        /// <returns></returns>
        public static List<object> QueryFirstRowValues(this ITable table, string[] queryFiledNames = null, string whereClause = null)
        {
            var values = new List<object>();
            var row = QueryFirstRow(table, whereClause);

            if (queryFiledNames == null)
            {
                for (var i = 0; i < row.Fields.FieldCount; i++)
                {
                    values.Add(row.get_Value(i));
                }
            }
            else
            {
                foreach (var fieldName in queryFiledNames)
                {
                    values.Add(row.get_Value(row.Fields.FindField(fieldName)));
                }
            }
            Marshal.ReleaseComObject(row);
            return values;
        }
        /// <summary>
        /// 查询符合条件的指定字段值
        /// </summary>
        /// <param name="table">查询的表</param>
        /// <param name="queryFiledName">查询的字段（该字段必须存在否则抛出异常）</param>
        /// <param name="whereClause">查询条件</param>
        /// <returns></returns>
        public static List<object> QueryValues(this ITable table, string queryFiledName, string whereClause = null)
        {
            var fieldIndex = table.FindField(queryFiledName);
            if (fieldIndex < 0)
                throw new Exception("找不到字段：" + queryFiledName);

            var values = new List<object>();
            var cursor = table.Search(new QueryFilterClass { WhereClause = whereClause }, true);//Search方法Recycling参数此处可以为true
            IRow row;
            while ((row = cursor.NextRow()) != null)
            {
                values.Add(row.get_Value(fieldIndex));
            }
            Marshal.ReleaseComObject(cursor);
            return values;
        }
        /// <summary>
        /// 查询符合条件的字段值组，组合成键值对（注意key字段必须符合唯一值规范）
        /// </summary>
        /// <param name="table">查询的表</param>
        /// <param name="keyFiledName">查询的字段，此作为键值对key值，查询前必须确定此字段值符合唯一值规范</param>
        /// <param name="valueFiledName">查询的字段，此作为键值对value值</param>
        /// <param name="whereClause">查询条件</param>
        /// <returns></returns>
        public static Dictionary<object, object> QueryValueDict(this ITable table, string keyFiledName, string valueFiledName, string whereClause = null)
        {
            var keyFieldIndex = table.FindField(keyFiledName);
            if (keyFieldIndex < 0) throw new Exception((table as IDataset)?.Name + "表找不到字段：" + keyFiledName);
            var valueFieldIndex = table.FindField(valueFiledName);
            if (valueFieldIndex < 0) throw new Exception((table as IDataset)?.Name + "表找不到字段：" + valueFiledName);

            var values = new Dictionary<object, object>();
            var cursor = table.Search(new QueryFilterClass { WhereClause = whereClause }, true);//Search方法Recycling参数此处可以为true
            IRow row;
            while ((row = cursor.NextRow()) != null)
            {
                var key = row.get_Value(keyFieldIndex);
                if (values.ContainsKey(key))
                {
                    if (key.ToString().Trim() == string.Empty)
                        throw new Exception($"{(table as IDataset)?.Name}表中，字段“{keyFiledName}”存在空值或空格，该字段的值要求不能重复，也不应为空！");
                    else
                        throw new Exception($"{(table as IDataset)?.Name}表中，要求值不能重复的字段“{keyFiledName}”，出现了重复的值“{key}”！");
                }
                values.Add(key, row.get_Value(valueFieldIndex));
            }
            Marshal.ReleaseComObject(cursor);
            return values;
        }
        /// <summary>
        /// 查询符合条件的字段值组，组合成键值对（注意key字段必须符合唯一值规范）
        /// </summary>
        /// <param name="table">查询的表</param>
        /// <param name="keyFiledName">查询的字段，此作为键值对key值，查询前必须确定此字段值符合唯一值规范</param>
        /// <param name="valueFiledName">查询的字段，此作为键值对value值</param>
        /// <param name="whereClause">查询条件</param>
        /// <returns></returns>
        public static Dictionary<string, string> QueryValueStringDict(this ITable table, string keyFiledName, string valueFiledName, string whereClause = null)
        {
            var keyFieldIndex = table.FindField(keyFiledName);
            if (keyFieldIndex < 0) throw new Exception("找不到字段：" + keyFiledName);
            var valueFieldIndex = table.FindField(valueFiledName);
            if (valueFieldIndex < 0) throw new Exception("找不到字段：" + valueFiledName);

            var values = new Dictionary<string, string>();
            var cursor = table.Search(new QueryFilterClass { WhereClause = whereClause }, true);//Search方法Recycling参数此处可以为true
            IRow row;
            while ((row = cursor.NextRow()) != null)
            {
                var key = row.get_Value(keyFieldIndex).ToString();
                if (values.ContainsKey(key))
                {
                    if (key.Trim() == string.Empty)
                        throw new Exception($"{(table as IDataset).Name}表中，字段“{keyFiledName}”存在空值或空格，该字段的值要求不能重复，也不应为空！");
                    else
                        throw new Exception($"{(table as IDataset).Name}表中，要求值不能重复的字段“{keyFiledName}”，出现了重复的值“{key}”！");
                }
                values.Add(key, row.get_Value(valueFieldIndex).ToString());
            }
            Marshal.ReleaseComObject(cursor);
            return values;
        }
        /// <summary>
        /// 查询符合条件的字段值组，组合成键值对（注意key字段必须符合唯一值规范）允许字段位空的且获取唯一值，
        /// </summary>
        /// <param name="table">查询的表</param>
        /// <param name="keyFiledName">查询的字段，此作为键值对key值，查询前必须确定此字段值符合唯一值规范</param>
        /// <param name="valueFiledName">查询的字段，此作为键值对value值</param>
        /// <param name="whereClause">查询条件</param>
        /// <returns></returns>
        public static Dictionary<string, string> QueryValueStringDict2(this ITable table, string keyFiledName, string valueFiledName, string whereClause = null)
        {
            var keyFieldIndex = table.FindField(keyFiledName);
            if (keyFieldIndex < 0) throw new Exception("找不到字段：" + keyFiledName);
            var valueFieldIndex = table.FindField(valueFiledName);
            if (valueFieldIndex < 0) throw new Exception("找不到字段：" + valueFiledName);

            var values = new Dictionary<string, string>();
            var cursor = table.Search(new QueryFilterClass { WhereClause = whereClause }, true);//Search方法Recycling参数此处可以为true
            IRow row;
            while ((row = cursor.NextRow()) != null)
            {
                var key = row.get_Value(keyFieldIndex).ToString().Trim();
                if (values.ContainsKey(key) || key == string.Empty)
                    continue;
                values.Add(key, row.get_Value(valueFieldIndex).ToString());
            }
            Marshal.ReleaseComObject(cursor);
            return values;
        }
        #endregion


        #region 数据源
        //当表格是从地图文档中获取（var table = (map as ITableCollection).Table[0]）
        //且表格没有正确关联数据源时，下列三个方法获取的数据源(路径)为null，暂未找到合适的接口处理此类情况，目前解决方法是：
        //表格没有正确关联数据源时应直接从地图中移除这些表格(ITableCollection.RemoveAllTables或RemoveTable)，然后再添加

        /// <summary>
        /// 获取表格所属的工作空间（IWorkspace）
        /// </summary>
        /// <param name="table">表格，此处不能把要素类(IFeatureClass)等当成表格处理</param>
        /// <returns></returns>
        public static IWorkspace GetWorkspace(this ITable table)
        {
            return (table as IDataset)?.Workspace;
        }
        /// <summary>
        /// 获取表格所属的工作空间的路径（IWorkspace）
        /// </summary>
        /// <param name="table">表格，此处不能把要素类(IFeatureClass)等当成表格处理</param>
        /// <returns></returns>
        public static string GetWorkspacePathName(this ITable table)
        {
            return (table as IDataset)?.Workspace.PathName;
        }
        /// <summary>
        /// 获取表格的完整路径（eg: C:\xxx.mdb\xx表）
        /// </summary>
        /// <param name="table">表格，此处不能把要素类(IFeatureClass)等当成表格处理</param>
        /// <returns></returns>
        public static string GetSourcePath(this ITable table)
        {
            var dataset = table as IDataset;
            return System.IO.Path.Combine(dataset?.Workspace.PathName, dataset?.Name);
        }
        #endregion


        #region 修改表格名称、别名
        /// <summary>
        /// 修改表格别名
        /// </summary>
        /// <param name="table">表格</param>
        /// <param name="newAliasName">新表格别名</param>
        public static void RenameTableAliasName(this ITable table, string newAliasName)
        {
            var classSchemaEdit2 = (IClassSchemaEdit2)table;
            classSchemaEdit2.AlterAliasName(newAliasName);
        }
        /// <summary>
        /// 修改表格名称以及别名
        /// （修改成功的条件：①需要Advanced级别的License权限，②表格不能被其他程序锁定）
        /// </summary> 
        /// <param name="table">表格</param>
        ///<param name="newName">新表格名</param>
        ///<param name="newAliasName">新表格别名</param>
        ///<returns>修改成功返回True,否则False</returns>
        public static bool RenameTableName(this ITable table, string newName, string newAliasName = null)
        {
            var ds = table as IDataset;
            var isRename = false;
            string oldAliasName = ((IObjectClass)table).AliasName, oldName = ds.Name;
            try
            {
                if (!string.IsNullOrEmpty(newAliasName))
                    RenameTableAliasName(table, newAliasName);

                if (ds.CanRename())
                {
                    ds.Rename(newName);
                    isRename = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"修改表格名称失败（{oldName}，别名：{oldAliasName}，改为：{newName}，别名：{newAliasName}）：\r\n{ex.Message}");
            }
            return isRename;
        }
        #endregion


        #region 校验WhereClause
        /// <summary>
        /// 校验并返回与数据源一致的条件查询语句
        /// （例如table的数据源是mdb，条件查询语句为 BH like '440101%'，则返回结果为 BH like '440101*'）
        /// </summary>
        /// <param name="table">执行条件查询的表</param>
        /// <param name="whereClause">需要校验的条件查询语句</param>
        public static string ValidateWhereClause(this ITable table, string whereClause)
        {
            var workspacePath = GetWorkspacePathName(table);
            if (System.IO.Path.GetExtension(workspacePath) == ".mdb" && whereClause.Contains("like"))
                return whereClause.Replace('_', '?').Replace('%', '*');
            return whereClause;
        }
        /// <summary>
        /// 校验并返回与数据源一致的条件查询语句
        /// <para>例如eType == <see cref="EWorkspaceType.Access"/>，条件查询语句为 BH like '440101%'，则返回结果为 BH like '440101*'</para>
        /// </summary>
        /// <param name="eType">数据源类型</param>
        /// <param name="whereClause">需要校验的条件查询语句</param>
        public static string ValidateWhereClause(EWorkspaceType eType, string whereClause)
        {
            if (eType == EWorkspaceType.Access && whereClause.Contains("like"))
                return whereClause.Replace('_', '?').Replace('%', '*');
            return whereClause;
        }
        #endregion



        #region 获取字段
        /// <summary>
        /// 获取字段
        /// </summary>
        /// <param name="table"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static IField GetField(this ITable table, string fieldName) => table.Fields.get_Field(table.FindField(fieldName));
        /// <summary>
        /// 获取字段
        /// </summary>
        /// <param name="table"></param>
        /// <param name="fieldIndex"></param>
        /// <returns></returns>
        public static IField GetField(this ITable table, int fieldIndex) => table.Fields.get_Field(fieldIndex);
        #endregion

        #region 创建、删除索引
        /// <summary>
        /// 创建表格的字段索引
        /// </summary>
        /// <param name="table">要创建索引的表格</param>
        /// <param name="fieldName">要创建索引的字段</param>
        /// <param name="indexName">要创建的索引的名称，值为null则索引名称为字段名后加上“_Index”：即“{<paramref name="fieldName"/>}_Index”</param>
        public static void CreateIndex(this ITable table, string fieldName, string indexName = null)
        {
            int fieldIndex = table.FindField(fieldName);
            if (fieldIndex < 0)
                throw new Exception($"找不到字段{fieldName}");

            IField field = table.Fields.Field[fieldIndex];
            IIndex index = CreateIndex(field, indexName);
            table.AddIndex(index);
        }
        /// <summary>
        /// 创建字段索引
        /// </summary>
        /// <param name="field">要创建索引的字段</param>
        /// <param name="indexName">要创建的索引的名称，值为null则索引名称为字段名后加上“_Index”</param>
        /// <returns></returns>
        internal static IIndex CreateIndex(IField field, string indexName = null)
        {
            IFields fields = new FieldsClass();
            IFieldsEdit fieldsEdit = fields as IFieldsEdit;
            fieldsEdit.FieldCount_2 = 1;
            fieldsEdit.set_Field(0, field);

            IIndex index = new IndexClass();
            IIndexEdit indexEdit = index as IIndexEdit;
            indexEdit.Fields_2 = fields;
            indexEdit.Name_2 = indexName ?? field.Name + "_Index";//索引名称
            indexEdit.IsAscending_2 = true;

            return index;
        }
        /// <summary>
        /// 删除表格的索引
        /// </summary>
        /// <param name="table"></param>
        /// <param name="indexName">要删除的索引的名称</param>
        public static void DeleteIndexByName(this ITable table, String indexName)
        {
            IIndexes indexes = table.Indexes;
            indexes.FindIndex(indexName, out var indexPos);
            if (indexPos < 0)
                throw new ArgumentException($"找不到名称为“{indexName}”的索引");

            IIndex index = indexes.get_Index(indexPos);
            table.DeleteIndex(index);
        }
        #endregion


        /// <summary>
        /// 获取表格名称
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string GetName(this ITable table) => ((IDataset)table).Name;
        /// <summary>
        /// 获取表格别名
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string GetAliasName(this ITable table) => ((IObjectClass)table).AliasName;

        /// <summary>
        /// 创建查询要素的游标
        /// </summary>
        /// <param name="table">查询的要素类</param>
        /// <param name="whereClause">查询条件</param>
        /// <param name="subFields">查询所返回的字段，多个字段用逗号隔开：e.g. "OBJECTID，NAME"</param>
        /// <param name="recyling"></param>
        /// <returns></returns>
        public static ICursor GetSearchCursor(this ITable table, string whereClause = null, string subFields = null, bool recyling = false)
        {
            IQueryFilter filter = new QueryFilterClass();
            filter.WhereClause = whereClause;
            if (!string.IsNullOrEmpty(subFields))
                filter.SubFields = subFields;

            return table.Search(filter, recyling);
        }
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="table"></param>
        /// <param name="sql"></param>
        public static void ExecuteSql(this ITable table, string sql)
        {
            var workspace = ((IDataset)table).Workspace;
            var workspaceProperties = (IWorkspaceProperties)workspace;
            var workspaceProperty = workspaceProperties.get_Property(esriWorkspacePropertyGroupType.esriWorkspacePropertyGroup,
                (int)esriWorkspacePropertyType.esriWorkspacePropCanExecuteSQL);
            if (!workspaceProperty.IsSupported)
                throw new Exception("当前数据源不支持执行Workspace.ExecuteSQL的方式处理数据");
            workspace.ExecuteSQL(sql);
        }
        /// <summary>
        /// 记录(row)为空时，抛出包含具体提示信息的异常
        /// </summary>
        /// <param name="row"></param>
        /// <param name="table"></param>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        public static void CheckNullToThrowException(this ITable table, IRow row, string whereClause)
        {
            if (row != null) return;

            if (string.IsNullOrEmpty(whereClause))
                throw new Exception($"在{(table as IDataset)?.Name}表格中，找不到记录！");
            else
                throw new Exception($"在{(table as IDataset)?.Name}表格中，找不到“{whereClause}”的记录！");
        }

        /// <summary>
        /// 判断表格是否被占用
        /// </summary>
        /// <param name="table"></param>
        /// <param name="message">被占用情况信息，未被占用则值为null</param>
        /// <returns>对象被占用返回True，未被占用返回False</returns>
        public static bool IsLock(this ITable table, out string message)
        {
            return IsLock(table as IObjectClass, out message);
        }
        /// <summary>
        /// 判断对象是否被占用
        /// </summary>
        /// <param name="objectClass"></param>
        /// <param name="message">被占用情况信息，未被占用则值为null</param>
        /// <returns>对象被占用返回True，未被占用返回False</returns>
        public static bool IsLock(this IObjectClass objectClass, out string message)
        {
            var sb = new StringBuilder();
            ISchemaLock schemaLock = (ISchemaLock)objectClass;
            schemaLock.GetCurrentSchemaLocks(out var enumSchemaLockInfo);
            ISchemaLockInfo schemaLockInfo;
            while ((schemaLockInfo = enumSchemaLockInfo.Next()) != null)
            {
                sb.AppendFormat("{0} : {1} : {2}\r\n", schemaLockInfo.TableName, schemaLockInfo.UserName, schemaLockInfo.SchemaLockType);
            }

            message = sb.Length > 0 ? sb.ToString() : null;
            return sb.Length > 0;
        }
    }
}
