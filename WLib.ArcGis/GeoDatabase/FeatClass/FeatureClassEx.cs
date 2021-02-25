﻿/*---------------------------------------------------------------- 
// auth： Windragon
// date： 2018
// desc： None
// mdfy:  None
// sorc:  https://gitee.com/windr07/WLib
//        https://github.com/Windr07/WLib
//----------------------------------------------------------------*/

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using WLib.ArcGis.Analysis.OnShape;
using WLib.ArcGis.GeoDatabase.Fields;
using WLib.ArcGis.GeoDatabase.Table;
using WLib.ArcGis.GeoDatabase.WorkSpace;

namespace WLib.ArcGis.GeoDatabase.FeatClass
{
    /* *
     * 查询说明：
     * 1、关于数据查询操作：
     *    EN:http://help.arcgis.com/en/sdk/10.0/arcobjects_net/conceptualhelp/index.html#/d/000100000146000000.htm
     *    CN:https://blog.csdn.net/yh0503/article/details/53493583
     *    查询地理数据库的三个接口：IQueryFilter、ISpatialFilter、IQueryDef
     *
     * 2、关于IQueryFilter的WhereClause详解：http://www.cnblogs.com/GISRSMAN/articles/4618188.html
     *    （1）在mdb中，where field = 'fieldValue'中，fieldValue不区分大小写，其他数据库区分大小写，应使用UPPER("Field") = 'FIELDVALUE' 
     *    （2）关于通配符：
     *          在coverage, shp, INFO table, dBASE table, or shared geodatabase查询，'_' 表示任何一个字符， '%' 表示0到任意个字符.
     *          在mdb查询，'?' 表示任何一个字符，'*' 表示0到任意个字符.
     */

    /// <summary>
    /// 提供对要素类数据的获取、输出、复制、创建、增、删、改、查、筛选、检查、重命名等方法
    /// </summary>
    public static partial class FeatureClassEx
    {
        #region 新增要素
        /// <summary>
        /// 在要素类中创建若干条新要素，遍历新要素并在委托中对其内容执行赋值操作，最后保存全部新要素并释放资源
        /// 
        /// <para>* 如果报错：无法在编辑会话之外更新此类的对象（Objects in this class cannot be updated outside an edit session）</para>
        /// <para>* 可能原因：数据库与该图层存在关联的拓扑/注记层/几何网络等；License权限不足；是否注册版本；空间索引是否缺失；</para>
        /// </summary>
        /// <param name="featureClass">操作的要素类</param>
        /// <param name="insertCount">创建新要素的数量</param>
        /// <param name="doActionByFeatures">在保存要素前，对要素执行的操作（通常是<see cref="IFeatureBuffer.set_Value"/>赋值操作），整型参数是新增要素的索引</param>
        public static void InsertFeatures(this IFeatureClass featureClass, int insertCount, Action<IFeatureBuffer, int> doActionByFeatures)
        {
            var featureCursor = featureClass.Insert(true);
            var featureBuffer = featureClass.CreateFeatureBuffer();
            for (var i = 0; i < insertCount; i++)
            {
                doActionByFeatures(featureBuffer, i);
                featureCursor.InsertFeature(featureBuffer);
            }
            featureCursor.Flush();
            Marshal.ReleaseComObject(featureBuffer);
            Marshal.ReleaseComObject(featureCursor);
        }
        /// <summary>
        /// 在要素类中创建若干条新要素，遍历新要素并在委托中对其内容执行赋值操作，最后保存全部新要素并释放资源
        /// 
        /// <para>* 如果报错：无法在编辑会话之外更新此类的对象（Objects in this class cannot be updated outside an edit session）</para>
        /// <para>* 可能原因：数据库与该图层存在关联的拓扑/注记层/几何网络等；License权限不足；是否注册版本；空间索引是否缺失；</para>
        /// </summary>
        /// <param name="featureClass">操作的要素类</param>
        /// <param name="insertCount">创建新要素的数量</param>
        /// <param name="doActionByFeatures">在保存要素前，对要素执行的操作（通常是<see cref="IFeatureBuffer.set_Value"/>赋值操作），整型参数是新增要素的索引， bool型返回值表示是否立即跳出新增要素操作</param>
        public static void InsertFeatures(this IFeatureClass featureClass, int insertCount, Func<IFeatureBuffer, int, bool> doActionByFeatures)
        {
            var featureCursor = featureClass.Insert(true);
            var featureBuffer = featureClass.CreateFeatureBuffer();
            for (var i = 0; i < insertCount; i++)
            {
                var isStopped = doActionByFeatures(featureBuffer, i);
                featureCursor.InsertFeature(featureBuffer);
                if (isStopped)
                    break;
            }
            featureCursor.Flush();
            Marshal.ReleaseComObject(featureBuffer);
            Marshal.ReleaseComObject(featureCursor);
        }
        /// <summary>
        /// 在要素类中创建若干条新要素，将游标提供给委托以在委托中指定具体操作，最后保存全部新要素并释放资源
        /// 
        /// <para>* 如果报错：无法在编辑会话之外更新此类的对象（Objects in this class cannot be updated outside an edit session）</para>
        /// <para>* 可能原因：数据库与该图层存在关联的拓扑/注记层/几何网络等；License权限不足；是否注册版本；空间索引是否缺失；</para>
        /// </summary>
        /// <param name="featureClass"></param>
        /// <param name="doActionByFeatures">在保存要素前，对要素执行的操作（通常是<see cref="IFeatureBuffer.set_Value"/>赋值和<see cref="IFeatureCursor.InsertFeature(IFeatureBuffer)"/>操作）</param>
        public static void InsertFeatures(this IFeatureClass featureClass, Action<IFeatureCursor, IFeatureBuffer> doActionByFeatures)
        {
            var featureCursor = featureClass.Insert(true);
            var featureBuffer = featureClass.CreateFeatureBuffer();

            doActionByFeatures(featureCursor, featureBuffer);

            featureCursor.Flush();
            Marshal.ReleaseComObject(featureBuffer);
            Marshal.ReleaseComObject(featureCursor);
        }
        /// <summary>
        ///  在要素类中创建一条新要素，在委托中对其内容执行赋值操作，最后保存新要素并释放资源
        /// 
        /// <para>* 如果报错：无法在编辑会话之外更新此类的对象（Objects in this class cannot be updated outside an edit session）</para>
        /// <para>* 可能原因：数据库与该图层存在关联的拓扑/注记层/几何网络等；License权限不足；是否注册版本；空间索引是否缺失；</para>
        /// </summary>
        /// <param name="featureClass">操作的要素类</param>
        /// <param name="doActionByFeature">在保存要素前，对要素执行的操作</param>
        public static void InsertOneFeature(this IFeatureClass featureClass, Action<IFeatureBuffer> doActionByFeature)
        {
            featureClass.UseFeatureClassLoad(() =>
            {
                var featureCursor = featureClass.Insert(true);
                var featureBuffer = featureClass.CreateFeatureBuffer();

                doActionByFeature(featureBuffer);
                featureCursor.InsertFeature(featureBuffer);

                Marshal.ReleaseComObject(featureBuffer);
                Marshal.ReleaseComObject(featureCursor);
            });
        }

        /// <summary>
        /// 在要素类中创建若干条新要素，遍历新要素并在委托中对其内容执行赋值操作，最后保存全部新要素并释放资源(使用IFeatureClassLoad提高效率，仅用于SDE或FileGDB)
        /// </summary>
        /// <param name="featureClass">操作的要素类</param>
        /// <param name="insertCount">创建新要素的数量</param>
        /// <param name="doActionByFeatures">在保存要素前，对要素执行的操作，整型参数是新增要素的索引</param>
        public static void InsertFeaturesEx(this IFeatureClass featureClass, int insertCount, Action<IFeatureBuffer, int> doActionByFeatures)
        {
            var featureClassLoad = featureClass as IFeatureClassLoad;
            featureClass.UseFeatureClassLoad(() =>
                InsertFeatures(featureClass, insertCount, doActionByFeatures)
            );
        }
        /// <summary>
        /// 在要素类中创建若干条新要素，将游标提供给委托以在委托中指定具体操作，最后保存全部新要素并释放资源(使用IFeatureClassLoad提高效率，仅用于SDE或FileGDB)
        /// </summary>
        /// <param name="featureClass">操作的要素类</param>
        /// <param name="doActionByFeatures">在保存要素前，对要素执行的操作，整型参数是新增要素的索引</param>
        public static void InsertFeaturesEx(this IFeatureClass featureClass, Action<IFeatureCursor, IFeatureBuffer> doActionByFeatures)
        {
            var featureClassLoad = (IFeatureClassLoad)featureClass;
            featureClass.UseFeatureClassLoad(() =>
                InsertFeatures(featureClass, doActionByFeatures)
            );
        }
        /// <summary>
        /// 使用IFeatureClassLoad提高效率（仅在SDE或FileGDB中有效）
        /// </summary>
        /// <param name="featureClass"></param>
        /// <param name="action"></param>
        private static void UseFeatureClassLoad(this IFeatureClass featureClass, Action action)
        {
            var featureClassLoad = featureClass as IFeatureClassLoad;
            var schemaLock = featureClass as ISchemaLock;
            schemaLock?.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);

            if (featureClassLoad != null)
                featureClassLoad.LoadOnlyMode = true;

            action();

            if (featureClassLoad != null)
                featureClassLoad.LoadOnlyMode = false;

            schemaLock?.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);
        }
        #endregion


        #region 删除要素
        /// <summary>
        /// 删除所有符合查询条件的要素（使用Update游标方式删除，此方法执行速度较Search方法快）
        /// </summary>
        /// <param name="featureClass">操作的要素类</param>
        /// <param name="whereClause">查询条件，注意如果值为空则删除所有要素</param>
        public static void DeleteFeatures(this IFeatureClass featureClass, string whereClause)
        {
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = whereClause;
            //使用Update游标的方式删除数据，相对于Search方法要快，参考：http://blog.sina.com.cn/s/blog_5e4c933d010116n5.html
            var featureCursor = featureClass.Update(queryFilter, false);
            var feature = featureCursor.NextFeature();
            while (feature != null)
            {
                featureCursor.DeleteFeature();
                feature = featureCursor.NextFeature();
            }
            Marshal.ReleaseComObject(queryFilter);
            Marshal.ReleaseComObject(featureCursor);
        }
        /// <summary>
        /// 根据查询条件查询要素，按判断条件执行删除操作（使用Update游标方式删除，此方法执行速度较Search方法快）
        /// </summary>
        /// <param name="featureClass">操作的要素类</param>
        /// <param name="whereClause">查询条件，注意如果值为空则删除所有要素</param>
        /// <param name="isDeleteFunc">判断函数，根据返回值确定是否删除要素（返回值为True则删除）</param>
        public static void DeleteFeatures(this IFeatureClass featureClass, string whereClause, Func<IFeature, bool> isDeleteFunc)
        {
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = whereClause;
            //使用Update游标的方式删除数据，相对于Search方法要快，参考：http://blog.sina.com.cn/s/blog_5e4c933d010116n5.html
            var featureCursor = featureClass.Update(queryFilter, false);
            var feature = featureCursor.NextFeature();
            while (feature != null)
            {
                if (isDeleteFunc(feature))
                    featureCursor.DeleteFeature();
                feature = featureCursor.NextFeature();
            }
            Marshal.ReleaseComObject(queryFilter);
            Marshal.ReleaseComObject(featureCursor);
        }
        /// <summary>  
        /// 删除所有符合查询条件的要素（使用ITable.DeleteSearchedRows，此方法执行速度较Update游标更快）
        /// </summary>  
        /// <param name="featureClass">操作的要素类</param>  
        /// <param name="whereClause">查询条件，注意如果值为空则删除所有要素</param>  
        public static void DeleteFeatures2(this IFeatureClass featureClass, string whereClause)
        {
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = whereClause;
            var table = (ITable)featureClass;
            table.DeleteSearchedRows(queryFilter);
        }
        /// <summary>
        /// 删除所有符合查询条件的要素（执行sql，此方法执行速度较ITable.DeleteSearchedRows更快）
        /// </summary>
        /// <param name="featureClass">操作的要素类</param>
        /// <param name="whereClause">查询条件，注意如果值为空则删除所有要素</param>
        public static void DeleteFeatures3(this IFeatureClass featureClass, string whereClause)
        {
            var dataset = (IDataset)featureClass;
            whereClause = string.IsNullOrEmpty(whereClause) ? "1=1" : whereClause;
            var sql = $"delete from {dataset.Name} where {whereClause}";
            dataset.Workspace.ExecuteSQL(sql);
        }
        #endregion


        #region 更新要素
        /// <summary>
        /// 根据查询条件查询要素，对查询获取的要素执行更新操作
        /// </summary>
        /// <param name="featureClass">查询的要素类</param>
        /// <param name="whereClause">查询条件，此值为null时查询所有要素</param>
        /// <param name="doActionByFeatures">针对要素执行的操作（对feature执行赋值）</param>
        /// <param name="nullRecordException">在查询不到记录时是否抛出异常，默认false</param>
        public static void UpdateFeatures(this IFeatureClass featureClass, string whereClause, Action<IFeature> doActionByFeatures, bool nullRecordException = false)
        {
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = whereClause;

            var featureCursor = featureClass.Update(queryFilter, false);
            IFeature feature = null;

            if (nullRecordException && featureClass.FeatureCount(queryFilter) == 0)
                CheckNullToThrowException(featureClass, null, whereClause);

            try
            {
                while ((feature = featureCursor.NextFeature()) != null)
                {
                    doActionByFeatures(feature);
                    featureCursor.UpdateFeature(feature);
                }
            }
            catch (Exception ex)//抛出更具体的异常信息
            {
                var msgOid = feature == null ? null : $"“OID = {feature.OID}”的";
                var msgWhereClause = string.IsNullOrEmpty(whereClause) ? null : $"根据条件“{whereClause}”";
                throw new Exception($"在{featureClass.AliasName}图层中，{msgWhereClause}更新{msgOid}记录时出错：{ex.Message}");
            }
            finally
            {
                Marshal.ReleaseComObject(featureCursor);
            }
        }
        /// <summary>
        /// 根据查询条件查询要素，对查询获取的要素执行更新操作
        /// </summary>
        /// <param name="featureClass">查询的要素类</param>
        /// <param name="whereClause">查询条件，此值为null时查询所有要素</param>
        /// <param name="doActionByFeatures">针对每条要素执行的操作（对feature执行赋值，并使用featureCursor.UpdateFeature(feature)的操作）</param>
        /// <param name="nullRecordException">在查询不到记录时是否抛出异常，默认false</param>
        public static void UpdateFeatures(this IFeatureClass featureClass, string whereClause, Action<IFeature, IFeatureCursor> doActionByFeatures, bool nullRecordException = false)
        {
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = whereClause;

            var featureCursor = featureClass.Update(queryFilter, true);
            IFeature feature = null;

            if (nullRecordException && featureClass.FeatureCount(queryFilter) == 0)
                CheckNullToThrowException(featureClass, null, whereClause);

            try
            {
                while ((feature = featureCursor.NextFeature()) != null)
                {
                    doActionByFeatures(feature, featureCursor);
                }
            }
            catch (Exception ex)
            {
                var msgOid = feature == null ? null : $"“OID = {feature.OID}”的";
                var msgWhereClause = string.IsNullOrEmpty(whereClause) ? null : $"根据条件“{whereClause}”";
                throw new Exception($"在{featureClass.AliasName}图层中，{msgWhereClause}更新{msgOid}记录时出错：{ex.Message}");
            }
            Marshal.ReleaseComObject(featureCursor);
        }
        /// <summary>
        /// 根据查询条件查询要素，对查询获取的要素执行更新操作
        /// </summary>
        /// <param name="featureClass">查询的要素类</param>
        /// <param name="whereClause">查询条件，此值为null时查询所有要素</param>
        /// <param name="doActionByFeatures">针对要素执行的操作，返回值代表是否立即停止更新操作</param>
        /// <param name="nullRecordException">在查询不到记录时是否抛出异常，默认false</param>
        public static void UpdateFeatures(this IFeatureClass featureClass, string whereClause, Func<IFeature, bool> doActionByFeatures, bool nullRecordException = false)
        {
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = whereClause;

            var featureCursor = featureClass.Update(queryFilter, false);
            IFeature feature = null;

            if (nullRecordException && featureClass.FeatureCount(queryFilter) == 0)
                CheckNullToThrowException(featureClass, null, whereClause);

            try
            {
                while ((feature = featureCursor.NextFeature()) != null)
                {
                    var isStopped = doActionByFeatures(feature);
                    featureCursor.UpdateFeature(feature);
                    if (isStopped)
                        break;
                }
            }
            catch (Exception ex)//抛出更具体的异常信息
            {
                var msgOid = feature == null ? null : $"“OID = {feature.OID}”的";
                var msgWhereClause = string.IsNullOrEmpty(whereClause) ? null : $"根据条件“{whereClause}”";
                throw new Exception($"在{featureClass.AliasName}图层中，{msgWhereClause}更新{msgOid}记录时出错：{ex.Message}");
            }
            finally
            {
                Marshal.ReleaseComObject(featureCursor);
            }
        }
        #endregion


        #region 查询要素
        /// <summary>
        /// 查询符合条件的要素的数量
        /// </summary>
        /// <param name="featureClass">查询的要素类</param>
        /// <param name="whereClause">查询条件</param>
        /// <returns></returns>
        public static int QueryCount(this IFeatureClass featureClass, string whereClause = null)
        {
            IQueryFilter filter = new QueryFilterClass();
            filter.WhereClause = whereClause;
            return featureClass.FeatureCount(filter);
        }
        /// <summary>
        /// 查询获取要素
        /// </summary>
        /// <param name="featureClass">查询的要素类</param>
        /// <param name="whereClause">查询条件</param>
        /// <param name="subFields"></param>
        /// <returns></returns>
        public static IEnumerable<IFeature> QueryFeatures(this IFeatureClass featureClass, string whereClause = null, string subFields = null)
        {
            var featureCursor = GetSearchCursor(featureClass, whereClause, subFields);
            IFeature feature;
            while ((feature = featureCursor.NextFeature()) != null)
            {
                yield return feature;
            }
            Marshal.ReleaseComObject(featureCursor);
        }
        /// <summary>
        /// 查找符合条件的第一个要素
        /// </summary>
        /// <param name="featureClass">查询要素类</param>
        /// <param name="whereClause">查询条件</param>
        /// <param name="subFields"></param>
        /// <returns></returns>
        public static IFeature QueryFirstFeature(this IFeatureClass featureClass, string whereClause = null, string subFields = null)
        {
            if (featureClass == null) return null;
            var cursor = GetSearchCursor(featureClass, whereClause, subFields);
            var feature = cursor.NextFeature();
            Marshal.ReleaseComObject(cursor);
            return feature;
        }
        /// <summary>
        ///  根据查询条件查询要素，对查询获取的要素执行指定操作
        /// </summary>
        /// <param name="featureClass">查询的要素类</param>
        /// <param name="whereClause">查询条件，此值为null时查询所有要素</param>
        /// <param name="doActionByFeatures">针对要素执行的操作</param>
        /// <param name="nullRecordException">在查询不到记录时是否抛出异常，默认false</param>
        /// <param name="stopOperation">根据情况中止查询的操作，返回true则中止查询，返回false则继续查询</param>
        public static void QueryFeatures(this IFeatureClass featureClass, string whereClause, Action<IFeature> doActionByFeatures, bool nullRecordException = false, Func<bool> stopOperation = null)
        {
            var featureCursor = GetSearchCursor(featureClass, whereClause);
            var feature = featureCursor.NextFeature();

            if (nullRecordException && feature == null)//找不到记录时，抛出异常
            {
                var msg = string.IsNullOrEmpty(whereClause) ? null : $"“{whereClause}”的";
                throw new Exception($"在{featureClass.AliasName}图层中，找不到{msg}记录");
            }

            if (stopOperation == null)
            {
                while (feature != null)
                {
                    doActionByFeatures(feature);
                    feature = featureCursor.NextFeature();
                }
            }
            else
            {
                while (feature != null)
                {
                    if (stopOperation()) break;
                    doActionByFeatures(feature);
                    feature = featureCursor.NextFeature();
                }
            }
            Marshal.ReleaseComObject(featureCursor);
        }
        /// <summary>
        ///  根据查询条件查询要素，对查询获取的要素执行指定操作
        /// </summary>
        /// <param name="featureClass">查询的要素类</param>
        /// <param name="whereClause">查询条件，此值为null时查询所有要素</param>
        /// <param name="doActionByFeatures">针对要素执行的操作（<see cref="IFeature"/>参数代表执行操作的要素，<see cref="int"/>参数代表当前是从0开始的第几条要素）</param>
        /// <param name="nullRecordException">在查询不到记录时是否抛出异常，默认false</param>
        /// <param name="stopOperation">根据情况中止查询的操作，返回true则中止查询，返回false则继续查询</param>
        public static void QueryFeatures(this IFeatureClass featureClass, string whereClause, Action<IFeature, int> doActionByFeatures, bool nullRecordException = false, Func<bool> stopOperation = null)
        {
            var featureCursor = GetSearchCursor(featureClass, whereClause);
            var feature = featureCursor.NextFeature();

            if (nullRecordException && feature == null)//找不到记录时，抛出异常
            {
                var msg = string.IsNullOrEmpty(whereClause) ? null : $"“{whereClause}”的";
                throw new Exception($"在{featureClass.AliasName}图层中，找不到{msg}记录");
            }

            int index = -1;
            if (stopOperation == null)
            {
                while (feature != null)
                {
                    doActionByFeatures(feature, ++index);
                    feature = featureCursor.NextFeature();
                }
            }
            else
            {
                while (feature != null)
                {
                    if (stopOperation()) break;
                    doActionByFeatures(feature, ++index);
                    feature = featureCursor.NextFeature();
                }
            }

            Marshal.ReleaseComObject(featureCursor);
        }
        /// <summary>
        ///  根据查询条件查询要素，对查询获取的要素执行指定操作
        /// </summary>
        /// <param name="featureClass">查询的要素类</param>
        /// <param name="whereClause">查询条件，此值为null时查询所有要素</param>
        /// <param name="doActionByFeatures">针对要素执行的操作</param>
        /// <param name="nullRecordException">在查询不到记录时是否抛出异常，默认false</param>
        public static Dictionary<TKey, TValue> QueryFeatures<TKey, TValue>(this IFeatureClass featureClass, string whereClause,
            Func<IFeature, TKey> keyFunc, Func<IFeature, TValue> valueFunc, bool nullRecordException = false)
        {
            var pairs = new Dictionary<TKey, TValue>();
            QueryFeatures(featureClass, whereClause, feature =>
            {
                pairs.Add(keyFunc(feature), valueFunc(feature));
            }, nullRecordException);

            return pairs;
        }
        /// <summary>
        ///  根据查询条件查询要素，对查询获取的要素执行指定操作
        /// </summary>
        /// <param name="featureClass">查询的要素类</param>
        /// <param name="whereClause">查询条件，此值为null时查询所有要素</param>
        /// <param name="doFuncByFeatures">针对要素执行的操作</param>
        /// <param name="nullRecordException">在查询不到记录时是否抛出异常，默认false</param>
        /// <param name="stopOperation">根据情况中止查询的操作，返回true则中止查询，返回false则继续查询</param>
        public static IEnumerable<TResult> QueryFeatures<TResult>(this IFeatureClass featureClass, string whereClause, Func<IFeature, TResult> doFuncByFeatures, bool nullRecordException = false, Func<bool> stopOperation = null)
        {
            var featureCursor = GetSearchCursor(featureClass, whereClause);
            var feature = featureCursor.NextFeature();

            if (nullRecordException && feature == null)//找不到记录时，抛出异常
            {
                var msg = string.IsNullOrEmpty(whereClause) ? null : $"“{whereClause}”的";
                throw new Exception($"在{featureClass.AliasName}图层中，找不到{msg}记录");
            }
            if (stopOperation == null)
            {
                while (feature != null)
                {
                    yield return doFuncByFeatures(feature);
                    feature = featureCursor.NextFeature();
                }
            }
            else
            {
                while (feature != null)
                {
                    if (stopOperation()) break;
                    yield return doFuncByFeatures(feature);
                    feature = featureCursor.NextFeature();
                }
            }

            Marshal.ReleaseComObject(featureCursor);
        }
        #endregion


        #region 查询图斑
        /// <summary>
        /// 查询符合条件的第一个图形
        /// </summary>
        /// <param name="featureClass">查询的要素类</param>
        /// <param name="whereClause">查询条件</param>
        /// <returns></returns>
        public static IGeometry QueryFirstGeometry(this IFeatureClass featureClass, string whereClause = null)
        {
            var feature = QueryFirstFeature(featureClass, whereClause);
            return feature?.Shape;
        }
        /// <summary>
        /// 查询符合条件的图斑，如果查询到多个图斑则取其union后的范围，若查询到0个图斑则返回null
        /// </summary>
        /// <param name="featureClass"></param>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        public static IGeometry QueryUnionGeometry(this IFeatureClass featureClass, string whereClause = null)
        {
            var geometries = featureClass.QueryGeometries(whereClause);
            return TopologicalOpt.UnionGeometryEx(geometries);
        }
        /// <summary>
        /// 查询符合条件的图形
        /// </summary>
        /// <param name="featureClass">查询的要素类</param>
        /// <param name="whereClause">查询条件</param>
        /// <returns></returns>
        public static IEnumerable<IGeometry> QueryGeometries(this IFeatureClass featureClass, string whereClause = null)
        {
            var featureCursor = featureClass.GetSearchCursor(whereClause);
            IFeature feature;
            while ((feature = featureCursor.NextFeature()) != null)
                yield return feature.Shape;

            Marshal.ReleaseComObject(featureCursor);
        }
        /// <summary>
        /// 根据查询条件查询矢量图层的图斑，获取图斑的副本
        /// </summary>
        /// <param name="featureClass"></param>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        public static IEnumerable<IGeometry> QueryGeometriesCopy(this IFeatureClass featureClass, string whereClause = null)
        {
            var cursor = featureClass.GetSearchCursor(whereClause);
            IFeature feature;
            while ((feature = cursor.NextFeature()) != null)
                yield return feature.ShapeCopy;

            Marshal.ReleaseComObject(cursor);
        }
        #endregion


        #region 查询字段值
        /// <summary>
        /// 获取要素类指定字段的唯一值（全部不重复的值）
        /// </summary>
        /// <param name="featureClass">被查询的要素类</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="whereClause">条件语句</param>
        /// <returns></returns>
        public static IEnumerable<object> GetUniqueValues(this IFeatureClass featureClass, string fieldName, string whereClause = null)
        {
            var featureCursor = GetSearchCursor(featureClass, whereClause, fieldName, true);

            IDataStatistics dataStatistics = new DataStatisticsClass();
            dataStatistics.Field = fieldName;
            dataStatistics.Cursor = featureCursor as ICursor;
            var enumerator = dataStatistics.UniqueValues;
            enumerator.Reset();
            while (enumerator.MoveNext())
                yield return enumerator.Current;
        }
        /// <summary>
        /// 获取要素类指定字段的唯一字符串值（全部不重复的值）
        /// </summary>
        /// <param name="featureClass">被查询的要素类</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="whereClause">条件语句</param>
        /// <returns></returns>
        public static IEnumerable<string> GetUniqueStrValues(this IFeatureClass featureClass, string fieldName, string whereClause = null)
        {
            return GetUniqueValues(featureClass, fieldName, whereClause).Select(v => v.ToString());
        }
        /// <summary>
        ///  查找符合条件的第一条记录，并返回记录中指定字段的值
        /// </summary>
        /// <param name="featureClass">查询的要素类</param>
        /// <param name="queryFiledName">查询的字段（该字段必须存在否则抛出异常）</param>
        /// <param name="whereClause">查询条件</param>
        public static object QueryFirstValue(this IFeatureClass featureClass, string queryFiledName, string whereClause = null)
        {
            var fieldIndex = featureClass.FindField(queryFiledName);
            if (fieldIndex < 0)
                throw new Exception("找不到字段：" + queryFiledName);

            object value = null;
            var featureCursor = GetSearchCursor(featureClass, whereClause, queryFiledName, true);
            var feature = featureCursor.NextFeature();
            if (feature != null)
            {
                value = feature.get_Value(fieldIndex);
            }
            Marshal.ReleaseComObject(featureCursor);
            return value;
        }
        /// <summary>
        /// 查找符合条件的第一条记录，并返回记录中指定字段的值，此值为不包含前后空白的字符串或null
        /// </summary>
        /// <param name="featureClass">查询的要素类</param>
        /// <param name="queryFiledName">查询的字段（该字段必须存在否则抛出异常）</param>
        /// <param name="whereClause">查询条件</param>
        /// <returns>查询结果的第一条记录的指定字段的转成字符串的值，找不到则返回null</returns>
        public static string QueryFirstStrValue(this IFeatureClass featureClass, string queryFiledName, string whereClause = null)
        {
            var value = QueryFirstValue(featureClass, queryFiledName, whereClause);
            return (value == null || value == DBNull.Value) ? null : value.ToString().Trim();
        }

        /// <summary>
        /// 查找符合条件的第一条记录，并返回记录中指定若干个字段的值
        /// </summary>
        /// <param name="featureClass"></param>
        /// <param name="queryFiledNames">查询的字段集合（当此数组为null时返回记录的全部字段值，否则字段必须存在，不存在则会出现异常）</param>
        /// <param name="whereClause">查询条件</param>
        /// <returns></returns>
        public static IEnumerable<object> QueryFirstFeatureValues(this IFeatureClass featureClass, string[] queryFiledNames = null, string whereClause = null)
        {
            var feature = QueryFirstFeature(featureClass, whereClause);
            if (queryFiledNames == null)
            {
                for (var i = 0; i < feature.Fields.FieldCount; i++)
                    yield return feature.get_Value(i);
            }
            else
            {
                foreach (var fieldName in queryFiledNames)
                    yield return feature.get_Value(feature.Fields.FindField(fieldName));
            }
            Marshal.ReleaseComObject(feature);
        }
        /// <summary>
        /// 查询符合条件的指定字段值
        /// </summary>
        /// <param name="featureClass">查询的要素类</param>
        /// <param name="queryFiledName">查询的字段（该字段必须存在否则抛出异常）</param>
        /// <param name="whereClause">查询条件</param>
        /// <returns></returns>
        public static IEnumerable<object> QueryValues(this IFeatureClass featureClass, string queryFiledName, string whereClause = null)
        {
            var fieldIndex = featureClass.FindField(queryFiledName);
            if (fieldIndex < 0)
                throw new Exception("找不到字段：" + queryFiledName);

            var featureCursor = GetSearchCursor(featureClass, whereClause, queryFiledName, true);
            IFeature feature;
            while ((feature = featureCursor.NextFeature()) != null)
                yield return feature.get_Value(fieldIndex);

            Marshal.ReleaseComObject(featureCursor);
        }
        /// <summary>
        /// 查询符合条件的指定字段值，并转化成字符串
        /// </summary>
        /// <param name="featureClass">查询的要素类</param>
        /// <param name="queryFiledName">查询的字段（该字段必须存在否则抛出异常）</param>
        /// <param name="whereClause">查询条件</param>
        /// <returns></returns>
        public static IEnumerable<string> QueryStrValues(this IFeatureClass featureClass, string queryFiledName, string whereClause = null)
        {
            return QueryValues(featureClass, queryFiledName, whereClause).Select(v => v.ToString());
        }
        /// <summary>
        /// 查询符合条件的指定字段值，并转化成整型
        /// <para>注意要查询的字段应当为非空整型字段，否则类型转换失败时会抛出异常</para>
        /// </summary>
        /// <param name="featureClass">查询的要素类</param>
        /// <param name="queryFiledName">查询的字段（该字段必须存在否则抛出异常）</param>
        /// <param name="whereClause">查询条件</param>
        /// <returns></returns>
        public static IEnumerable<int> QueryIntValues(this IFeatureClass featureClass, string queryFiledName, string whereClause = null)
        {
            return QueryValues(featureClass, queryFiledName, whereClause).Select(v => Convert.ToInt32(v));
        }

        /// <summary>
        /// 查询符合条件的字段值组，组合成键值对（注意key字段符合唯一值规范，包括不能存在多个null、Empty或空格的值，否则抛出异常）
        /// </summary>
        /// <param name="featureClass">查询的要素类</param>
        /// <param name="keyFiledName">查询的字段，此作为键值对key值，查询前必须确定此字段值符合唯一值规范</param>
        /// <param name="valueFiledName">查询的字段，此作为键值对value值</param>
        /// <param name="whereClause">查询条件</param>
        /// <returns></returns>
        public static Dictionary<object, object> QueryValueDict(this IFeatureClass featureClass, string keyFiledName, string valueFiledName, string whereClause = null)
        {
            var keyFieldIndex = featureClass.FindField(keyFiledName);
            if (keyFieldIndex < 0) throw new Exception(featureClass.AliasName + "图层找不到字段：" + keyFiledName);
            var valueFieldIndex = featureClass.FindField(valueFiledName);
            if (valueFieldIndex < 0) throw new Exception(featureClass.AliasName + "图层找不到字段：" + valueFiledName);

            var values = new Dictionary<object, object>();
            var featureCursor = GetSearchCursor(featureClass, whereClause, keyFiledName + "," + valueFiledName, true);
            IFeature feature;
            while ((feature = featureCursor.NextFeature()) != null)
            {
                var key = feature.get_Value(keyFieldIndex);
                if (values.ContainsKey(key))
                {
                    if (key.ToString().Trim() == string.Empty)
                        throw new Exception($"{featureClass.AliasName}图层中，字段“{keyFiledName}”存在空值或空格，该字段的值要求不能重复，也不应为空！");
                    else
                        throw new Exception($"{featureClass.AliasName}图层中，要求值不能重复的字段“{keyFiledName}”，出现了重复的值“{key}”！");
                }
                values.Add(key, feature.get_Value(valueFieldIndex));
            }
            Marshal.ReleaseComObject(featureCursor);
            return values;
        }
        /// <summary>
        /// 查询符合条件的字段值组，组合成键值对（注意key字段符合唯一值规范，包括不能存在多个null、Empty或空格的值，否则抛出异常）
        /// </summary>
        /// <param name="featureClass">查询的表</param>
        /// <param name="keyFiledName">查询的字段，此作为键值对key值，查询前必须确定此字段值符合唯一值规范</param>
        /// <param name="valueFiledName">查询的字段，此作为键值对value值</param>
        /// <param name="whereClause">查询条件</param>
        /// <returns></returns>
        public static Dictionary<string, string> QueryValueStrDict(this IFeatureClass featureClass, string keyFiledName, string valueFiledName, string whereClause = null)
        {
            var keyFieldIndex = featureClass.FindField(keyFiledName);
            if (keyFieldIndex < 0) throw new Exception("找不到字段：" + keyFiledName);
            var valueFieldIndex = featureClass.FindField(valueFiledName);
            if (valueFieldIndex < 0) throw new Exception("找不到字段：" + valueFiledName);

            var values = new Dictionary<string, string>();
            var featureCursor = GetSearchCursor(featureClass, whereClause, keyFiledName + "," + valueFiledName, true);
            IFeature feature;
            while ((feature = featureCursor.NextFeature()) != null)
            {
                var key = feature.get_Value(keyFieldIndex).ToString();
                if (values.ContainsKey(key))
                {
                    if (key.Trim() == string.Empty)
                        throw new Exception($"{featureClass.AliasName}图层中，字段“{keyFiledName}”存在空值或空格，该字段的值要求不能重复，也不应为空！");
                    else
                        throw new Exception($"{featureClass.AliasName}图层中，要求值不能重复的字段“{keyFiledName}”，出现了重复的值“{key}”！");
                }
                values.Add(key, feature.get_Value(valueFieldIndex).ToString());
            }
            Marshal.ReleaseComObject(featureCursor);
            return values;
        }
        #endregion


        #region 获取数据源
        /// <summary>
        /// 获取要素类所属的工作空间（IFeatureWorkspace）
        /// </summary>
        /// <param name="featureClass">要素类</param>
        /// <returns></returns>
        public static IFeatureWorkspace GetFeatureWorkspace(this IFeatureClass featureClass)
        {
            var featureDataset = featureClass.FeatureDataset;

            if (featureDataset == null)
            {
                featureDataset = featureClass as IFeatureDataset;
            }
            if (featureDataset == null)
            {
                var dataset = featureClass as IDataset;
                featureDataset = dataset as IFeatureDataset;
            }
            if (featureDataset == null)
            {
                var dataset2 = featureClass as IDataset;
                var featureWorkspace = dataset2.Workspace as IFeatureWorkspace;
                if (featureWorkspace != null)
                    return featureWorkspace;
            }
            if (featureDataset == null)
                throw new Exception($"图层{featureClass.AliasName}不在要素数据集（Feature Dataset）中");

            return (featureDataset.Workspace as IFeatureWorkspace);
        }
        /// <summary>
        ///  获取要素类所属的工作空间的路径（IFeatureWorkspace）
        /// </summary>
        /// <param name="featureClass"></param>
        /// <returns></returns>
        public static string GetWorkspacePathName(this IFeatureClass featureClass)
        {
            var workspace = (IWorkspace)GetFeatureWorkspace(featureClass);
            return workspace.PathName;
        }
        /// <summary>
        /// 获取要素类的完整路径（eg: C:\xxx.mdb\xx要素数据集\xx要素类）
        /// </summary>
        /// <param name="featureClass"></param>
        /// <returns></returns>
        public static string GetSourcePath(this IFeatureClass featureClass)
        {
            var sourceName = ((IDataset)featureClass).Name;
            if (featureClass.FeatureDataset != null)
                sourceName = System.IO.Path.Combine(featureClass.FeatureDataset.Name, sourceName);

            return System.IO.Path.Combine(GetWorkspacePathName(featureClass), sourceName);
        }
        #endregion


        #region 修改要素类名称、别名
        /// <summary>
        /// 修改要素类别名
        /// </summary>
        /// <param name="featureClass">要素类</param>
        /// <param name="newAliasName">新要素类别名</param>
        public static void RenameAliasName(this IFeatureClass featureClass, string newAliasName)
        {
            var classSchemaEdit2 = featureClass as IClassSchemaEdit2;
            classSchemaEdit2.AlterAliasName(newAliasName);
        }
        /// <summary>
        /// 修改要素类名称以及别名
        /// <para>修改成功的条件：①需要Advanced级别的License权限，②要素类不能被其他程序锁定，③符合命名规范、在工作空间中不能有同名要素类</para>
        /// </summary> 
        /// <param name="featureClass">要素类</param>
        ///<param name="newName">新要素类名</param>
        ///<param name="newAliasName">新要素类别名</param>
        ///<returns>修改成功返回True,否则False</returns>
        public static bool Rename(this IFeatureClass featureClass, string newName, string newAliasName = null)
        {
            var dataset = featureClass as IDataset;
            var isRename = false;
            string oldAliasName = featureClass.AliasName, oldName = dataset.Name;
            try
            {
                if (!string.IsNullOrEmpty(newAliasName))
                    RenameAliasName(featureClass, newAliasName);

                if (dataset.CanRename())
                {
                    dataset.Rename(newName);
                    isRename = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"修改要素类名称失败（{oldName}，别名：{oldAliasName}，改为：{newName}，别名：{newAliasName}）：\r\n{ex.Message}");
            }
            return isRename;
        }
        #endregion


        #region 点/线/面要素类排序、筛选
        /// <summary>
        /// 将要素类集合按点、线、面的顺序排序并返回（仅返回点、线、面图层，其他类型图层不返回）
        /// </summary>
        /// <param name="featureClasses">要素类集合</param>
        /// <returns></returns>
        public static List<IFeatureClass> SortByShapeType(this IEnumerable<IFeatureClass> featureClasses)
        {
            var newClasses = new List<IFeatureClass>();
            newClasses.AddRange(featureClasses.Where(v => v.ShapeType == esriGeometryType.esriGeometryPoint || v.ShapeType == esriGeometryType.esriGeometryMultipoint));
            newClasses.AddRange(featureClasses.Where(v => v.ShapeType == esriGeometryType.esriGeometryPolyline));
            newClasses.AddRange(featureClasses.Where(v => v.ShapeType == esriGeometryType.esriGeometryPolygon));
            return newClasses;
        }
        /// <summary>
        /// 根据指定的几何类型按顺序从要素类集合中筛选要素类
        /// </summary>
        /// <param name="featureClasses">要素类集合</param>
        /// <param name="geometryTypes">几何类型</param>
        /// <returns></returns>
        public static List<IFeatureClass> FilterByShapeType(this IEnumerable<IFeatureClass> featureClasses, params esriGeometryType[] geometryTypes)
        {
            var newClasses = new List<IFeatureClass>();
            foreach (var geometryType in geometryTypes)
            {
                newClasses.AddRange(featureClasses.Where(v => v.ShapeType == geometryType));
            }
            return newClasses;
        }
        #endregion


        #region 校验WhereClause
        /// <summary>
        /// 校验并返回与数据源一致的条件查询语句
        /// （例如table的数据源是mdb，条件查询语句为 BH like '440101%'，则返回结果为 BH like '440101*'）
        /// </summary>
        /// <param name="featureClass">执行条件查询的要素类</param>
        /// <param name="whereClause">需要校验的条件查询语句</param>
        public static string ValidateWhereClause(this IFeatureClass featureClass, string whereClause) =>
            TableEx.ValidateWhereClause(featureClass as ITable, whereClause);
        /// <summary>
        /// 校验并返回与数据源一致的条件查询语句
        /// <para>例如eType == <see cref="EWorkspaceType.Access"/>，条件查询语句为 BH like '440101%'，则返回结果为 BH like '440101*'</para>
        /// </summary>
        /// <param name="eType">数据源类型</param>
        /// <param name="whereClause">需要校验的条件查询语句</param>
        public static string ValidateWhereClause(EWorkspaceType eType, string whereClause) =>
            TableEx.ValidateWhereClause(eType, whereClause);
        #endregion


        #region 获取字段
        /// <summary>
        /// 获取字段
        /// </summary>
        /// <param name="featureClass"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static IField GetField(this IFeatureClass featureClass, string fieldName) => featureClass.Fields.get_Field(featureClass.FindField(fieldName));
        /// <summary>
        /// 获取字段
        /// </summary>
        /// <param name="featureClass"></param>
        /// <param name="fieldIndex"></param>
        /// <returns></returns>
        public static IField GetField(this IFeatureClass featureClass, int fieldIndex) => featureClass.Fields.get_Field(fieldIndex);
        #endregion


        #region 创建、删除索引
        /// <summary>
        /// 创建要素类的字段索引
        /// </summary>
        /// <param name="featureClass">要创建索引的要素类</param>
        /// <param name="fieldName">要创建索引的字段</param>
        /// <param name="indexName">要创建的索引的名称，值为null则索引名称为字段名后加上“_Index”：即“{<paramref name="fieldName"/>}_Index”</param>
        public static void CreateIndex(this IFeatureClass featureClass, string fieldName, string indexName = null)
        {
            int fieldIndex = featureClass.FindField(fieldName);
            if (fieldIndex < 0)
                throw new Exception($"找不到字段{fieldName}");

            IField field = featureClass.Fields.Field[fieldIndex];
            IIndex index = TableEx.CreateIndex(field, indexName);
            featureClass.AddIndex(index);
        }
        /// <summary>
        /// 创建要素类的空间索引
        /// <para>参考：https://desktop.arcgis.com/en/arcobjects/latest/net/CreateIndexes.htm </para>
        /// <para>无法在个人地理数据库(mdb)中的要素类上重新创建空间索引</para>
        /// </summary>
        /// <param name="featureClass"></param>
        /// <param name="gridOneSize"></param>
        /// <param name="gridTwoSize"></param>
        /// <param name="gridThreeSize"></param>
        public static void CreateSpatialIndex(this IFeatureClass featureClass, double gridOneSize = 0, double gridTwoSize = 0, double gridThreeSize = 0)
        {
            //从索引集合中查找和删除已有的空间索引（Shape字段索引）
            // Get an enumerator for indexes based on the shape field.
            IIndexes indexes = featureClass.Indexes;
            var shapeFieldName = featureClass.ShapeFieldName;
            IEnumIndex enumIndex = indexes.FindIndexesByFieldName(shapeFieldName);
            enumIndex.Reset();
            // Get the index based on the shape field (should only be one) and delete it.
            IIndex index = enumIndex.Next();
            if (index != null)
                featureClass.DeleteIndex(index);

            //复制Shape字段
            // Clone the shape field from the feature class.
            int shapeFieldIndex = featureClass.FindField(shapeFieldName);
            IField sourceField = featureClass.Fields.get_Field(shapeFieldIndex);
            IClone sourceFieldClone = (IClone)sourceField;
            IClone targetFieldClone = sourceFieldClone.Clone();
            IField targetField = (IField)targetFieldClone;

            //修改新的Shape字段的几何定义
            // Open the geometry definition from the cloned field and modify it.
            IGeometryDef geometryDef = targetField.GeometryDef;
            IGeometryDefEdit geometryDefEdit = (IGeometryDefEdit)geometryDef;
            geometryDefEdit.GridCount_2 = 3;
            geometryDefEdit.set_GridSize(0, gridOneSize);
            geometryDefEdit.set_GridSize(1, gridTwoSize);
            geometryDefEdit.set_GridSize(2, gridThreeSize);

            //创建空间索引
            // Create a spatial index and set the required attributes.
            IIndex newIndex = new IndexClass();
            IIndexEdit indexEdit = (IIndexEdit)newIndex;
            indexEdit.Name_2 = string.Concat(shapeFieldName, "_Index");
            indexEdit.IsAscending_2 = true;
            indexEdit.IsUnique_2 = false;
            // Create a fields collection and assign it to the new index.
            IFields newIndexFields = new FieldsClass();
            IFieldsEdit newIndexFieldsEdit = (IFieldsEdit)newIndexFields;
            newIndexFieldsEdit.AddField(targetField);
            indexEdit.Fields_2 = newIndexFields;
            // Add the spatial index back into the feature class.
            featureClass.AddIndex(newIndex);
        }
        /// <summary>
        /// 删除要素类的索引
        /// </summary>
        /// <param name="featureClass"></param>
        /// <param name="indexName">要删除的索引的名称</param>
        public static void DeleteIndexByName(this IFeatureClass featureClass, String indexName)
        {
            IIndexes indexes = featureClass.Indexes;
            indexes.FindIndex(indexName, out var indexPos);
            if (indexPos < 0)
                throw new ArgumentException($"找不到名称为“{indexName}”的索引");

            IIndex index = indexes.get_Index(indexPos);
            featureClass.DeleteIndex(index);
        }
        #endregion


        /// <summary>
        /// 获取要素类名称
        /// </summary>
        /// <param name="featureClass"></param>
        /// <returns></returns>
        public static string GetName(this IFeatureClass featureClass) => (featureClass as IDataset)?.Name;
        /// <summary>
        /// 创建查询要素的游标
        /// </summary>
        /// <param name="featureClass">查询的要素类</param>
        /// <param name="whereClause">查询条件</param>
        /// <param name="subFields">查询所返回的字段，多个字段用逗号隔开：e.g. "OBJECTID，NAME"</param>
        /// <param name="recyling"></param>
        /// <returns></returns>
        public static IFeatureCursor GetSearchCursor(this IFeatureClass featureClass,
            string whereClause = null, string subFields = null, bool recyling = false)
        {
            IQueryFilter filter = new QueryFilterClass();
            filter.WhereClause = whereClause;
            if (!string.IsNullOrEmpty(subFields))
                filter.SubFields = subFields;

            return featureClass.Search(filter, recyling);
        }
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="featureClass"></param>
        /// <param name="sql"></param>
        public static void ExecuteSql(this IFeatureClass featureClass, string sql)
        {
            var workspace = (featureClass as IDataset).Workspace;
            var workspaceProperties = workspace as IWorkspaceProperties;
            var wor = workspaceProperties.get_Property(esriWorkspacePropertyGroupType.esriWorkspacePropertyGroup,
                (int)esriWorkspacePropertyType.esriWorkspacePropCanExecuteSQL);
            if (!wor.IsSupported)
                throw new Exception("当前数据源不支持执行Workspace.ExecuteSQL的方式处理数据");
            workspace.ExecuteSQL(sql);
        }
        /// <summary>
        /// 记录(feature)为空时，抛出包含具体提示信息的异常
        /// </summary>
        /// <param name="featureClass"></param>
        /// <param name="whereClause"></param>
        /// <param name="feature"></param>
        /// <returns></returns>
        public static void CheckNullToThrowException(this IFeatureClass featureClass, IFeature feature, string whereClause)
        {
            if (feature == null)//找不到记录时，抛出异常
            {
                if (string.IsNullOrEmpty(whereClause))
                    throw new Exception($"在{featureClass.AliasName}图层中，找不到记录！");
                else
                    throw new Exception($"在{featureClass.AliasName}图层中，找不到“{whereClause}”的记录！");
            }
        }
        /// <summary>
        /// 要素类是否包含Z值
        /// </summary>
        /// <param name="featureClass">要素类</param>
        /// <returns></returns>
        public static bool IsExistZ(this IFeatureClass featureClass)
        {
            var geoDataSet = featureClass as IGeoDataset;
            var zAware = geoDataSet.Extent as IZAware;
            return zAware.ZAware;
        }
        /// <summary>
        /// 判断要素类是否已被启用编辑
        /// </summary>
        /// <param name="featureClass"></param>
        /// <returns></returns>
        public static bool IsEdit(this IFeatureClass featureClass)
        {
            return (featureClass as IDatasetEdit).IsBeingEdited();
        }
        /// <summary>
        /// 获取几何定义
        /// </summary>
        /// <param name="featureClass"></param>
        /// <returns></returns>
        public static IGeometryDef GetGeometryDef(this IFeatureClass featureClass)
        {
            return FieldEx.GetShapeField(featureClass).GeometryDef;
        }
        /// <summary>
        /// 获取要素类范围
        /// </summary>
        /// <param name="featureClass"></param>
        public static IEnvelope GetExtent(this IFeatureClass featureClass) => ((IGeoDataset)featureClass).Extent;
        /// <summary>
        /// 根据当前要素类创建要素图层
        /// </summary>
        /// <param name="featureClass"></param>
        /// <returns></returns>
        public static IFeatureLayer CreateToLayer(this IFeatureClass featureClass)
        {
            if (featureClass == null)
                throw new Exception("创建图层时，图层对应的要素类不能为空！");
            return new FeatureLayer()
            {
                FeatureClass = featureClass,
                Name = featureClass.AliasName
            };
        }
        /// <summary>
        /// 判断要素类是否被占用
        /// </summary>
        /// <param name="featureClass"></param>
        /// <param name="message">被占用情况信息，未被占用则值为null</param>
        /// <returns>对象被占用返回True，未被占用返回False</returns>
        public static bool IsLock(this IFeatureClass featureClass, out string message) => TableEx.IsLock(featureClass as IObjectClass, out message);
        /// <summary>
        ///  将<see cref="IFeatureClassContainer"/>转成可枚举数组，获取全部要素类
        /// </summary>
        /// <param name="featureClassContainer"></param>
        /// <returns></returns>
        public static IEnumerable<IFeatureClass> ToEnumerable(this IFeatureClassContainer featureClassContainer)
        {
            return featureClassContainer.Classes.ToEnumerable();
        }
        /// <summary>
        /// 将<see cref="IEnumFeatureClass"/>转成可枚举数组，获取全部要素类
        /// </summary>
        /// <param name="enumFeatureClass"></param>
        /// <returns></returns>
        public static IEnumerable<IFeatureClass> ToEnumerable(this IEnumFeatureClass enumFeatureClass)
        {
            enumFeatureClass.Reset();
            IFeatureClass featureClass = null;
            while ((featureClass = enumFeatureClass.Next()) != null)
                yield return featureClass;
        }
    }
}
