﻿/*---------------------------------------------------------------- 
// auth： Windragon
// date： 2018
// desc： None
// mdfy:  None
//----------------------------------------------------------------*/

using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Runtime.InteropServices;
using WLib.Attributes.Description;
using WLib.Files;

namespace WLib.ArcGis.GeoDatabase.WorkSpace
{
    /// <summary>
    /// 提供根据路径或连接字符串，获取工作空间的方法
    /// （sde的连接可参考 http://blog.csdn.net/mengdong_zy/article/details/8961390）
    /// （参考：http://edndoc.esri.com/arcobjects/9.2/ComponentHelp/esriGeoDatabase/IWorkspaceFactory.htm）
    /// （参考：http://blog.csdn.net/guangliang1102/article/details/51154893）
    /// （连接字符串：https://www.connectionstrings.com）
    /// </summary>
    public static partial class WorkspaceEx
    {
        /// <summary>
        /// 判断径是否为工作空间路径，任意已存在的目录、mdb文件、xls或xlsx文件均认为是工作空间
        /// </summary>
        /// <param name="path">工作空间路径，任意已存在的目录、mdb文件、xls或xlsx文件均认为是工作空间</param>
        /// <returns></returns>
        public static bool IsWorkspacePath(string path)
        {
            if (!System.IO.Path.IsPathRooted(path))
                path = AppDomain.CurrentDomain.BaseDirectory + path;

            if (System.IO.Directory.Exists(path)) return true;

            if (!System.IO.File.Exists(path)) return false;

            string extension = System.IO.Path.GetExtension(path)?.ToLower();
            return extension == ".mdb" || extension == ".xls" || extension == ".xlsx";
        }
        /// <summary>
        /// 判断字符串是否符合连接字符串规范（例如sde/sql/oledb连接字符串），不判断是否能成功连接
        /// </summary>
        /// <param name="str">需要判定的字符串</param>
        /// <returns></returns>
        public static bool IsConnectionString(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;
            str = str.TrimEnd(';');//去掉最后一个分号
            var strConnectArray = str.Split('=', ';');
            return strConnectArray.Length > 0 && strConnectArray.Length % 2 == 0;
        }
        /// <summary>
        /// 根据路径或连接参数，判断工作空间类型，只判断shp/gdb/mdb/sde/xls/xlsx，非gdb目录都将当成shp工作空间
        /// </summary>
        /// <param name="connStrOrPath">工作空间的路径或连接参数，可以是shp/gdb文件夹路径、mdb/xls/xlsx文件路径、sde连接字符串</param>
        /// <returns>若strConnOrPath不是连接字符串，且指示的不是gdb,mdb,shp路径或路径不存在，返回null</returns>
        public static EWorkspaceType GetDefaultWorkspaceType(string connStrOrPath)
        {
            var eWorkspaceType = EWorkspaceType.Default;
            if (IsConnectionString(connStrOrPath))
                eWorkspaceType = EWorkspaceType.Sde;
            else if (System.IO.File.Exists(connStrOrPath))
            {
                var extension = System.IO.Path.GetExtension(connStrOrPath).ToLower();
                if (extension == ".mdb") eWorkspaceType = EWorkspaceType.Access;
                else if (extension == ".xls" || extension == ".xlsx") eWorkspaceType = EWorkspaceType.Excel;
            }
            else if (System.IO.Directory.Exists(connStrOrPath))
                eWorkspaceType = connStrOrPath.ToLower().EndsWith(".gdb") ? EWorkspaceType.FileGDB : EWorkspaceType.ShapeFile;

            return eWorkspaceType;
        }
        /// <summary>
        /// 解析连接字符串，转成IPropertySet
        /// </summary>
        /// <param name="connStr">连接字符串</param>
        /// <returns></returns>
        public static IPropertySet ConnectStringToPropetySet(string connStr)
        {
            IPropertySet propertySet = new PropertySetClass();
            string[] strConnArray = connStr.Split('=', ';');
            for (int i = 0; i < (strConnArray.Length / 2); i++)
            {
                propertySet.SetProperty(strConnArray[2 * i], strConnArray[2 * i + 1]);
            }
            return propertySet;
        }


        /// <summary>
        /// 打开工作空间
        /// </summary>
        /// <param name="connStrOrPath">
        /// 工作空间路径或连接字符串，可以是以下情况：
        /// 1、shp、txt、dwg、栅格文件（GRID、TIFF、ERDAS IMAGE等）所在目录；
        /// 2、gdb文件夹自身路径；
        /// 3、mdb、xls、xlsx文件路径；
        /// 4、sde连接字符串（SERVER=ditu.test.com;INSTANCE=5151;DATABASE=sde_test;USER=sa;PASSWORD=sa;VERSION=dbo.DEFAULT）；
        /// 5、oledb连接字符串，包括连接Excel、Access、Oracle、SQLServer等（Provider=Microsoft.Jet.OLEDB.4.0;Data Source=x:\xxx.mdb;User Id=admin;Password=xxx;）；
        /// 6、直连sql连接字符串（server=localhost;uid=sa;pwd=sa;database=myDatabase）
        /// </param>
        /// <param name="eType">
        /// 标识优先将strConnOrPath作为打开哪种工作空间的参数，值为Default时，
        /// 根据strConnOrPath参数自动识别为shp/gdb/mdb/sde/oledb的其中一种工作空间</param>
        /// <returns></returns>
        public static IWorkspace GetWorkSpace(string connStrOrPath, EWorkspaceType eType = EWorkspaceType.Default)
        {
            IWorkspace workspace = null;
            if (IsConnectionString(connStrOrPath))//当参数是数据库连接字符串时
            {
                var tmpConnStr = connStrOrPath.ToLower();
                if (tmpConnStr.Contains("provider") && tmpConnStr.Contains("oledb"))
                    eType = EWorkspaceType.OleDb;
                else if (eType == EWorkspaceType.Default)
                    eType = EWorkspaceType.Sde;

                workspace = GetWorksapceFromConnStr(connStrOrPath, eType);
            }
            else if (IsWorkspacePath(connStrOrPath))
            {
                if (!System.IO.Path.IsPathRooted(connStrOrPath))
                    connStrOrPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, connStrOrPath);

                if (System.IO.Directory.Exists(connStrOrPath)) //当参数是文件夹路径时
                {
                    if (eType == EWorkspaceType.Default)
                        eType = connStrOrPath.ToLower().EndsWith(".gdb") ? EWorkspaceType.FileGDB : EWorkspaceType.ShapeFile;

                    workspace = GetWorkspaceFromFile(connStrOrPath, eType);
                }
                else if (System.IO.File.Exists(connStrOrPath)) //当参数是文件路径时
                {
                    var extension = System.IO.Path.GetExtension(connStrOrPath)?.ToLower();
                    if (extension == ".mdb")
                        eType = EWorkspaceType.Access;
                    else if (extension == ".xls" || extension == ".xlsx")
                        eType = EWorkspaceType.Excel;

                    workspace = GetWorkspaceFromFile(connStrOrPath, eType);
                }
            }
            return workspace;
        }
        /// <summary>
        /// 通过连接字符串获取工作空间，连接字符串参考：
        /// <para>①sde： SERVER=ditu.test.com;INSTANCE=5151;DATABASE=sde_test;USER=sa;PASSWORD=sa;VERSION=dbo.DEFAULT</para>
        /// <para>②sql： server=localhost;uid=sa;pwd=sa;database=myDatabase</para>
        /// <para>③oleDb： Provider=Microsoft.Jet.OLEDB.4.0;Data Source=x:\xxx.mdb;User Id=admin;Password=xxx;</para>
        /// </summary>
        /// <param name="eType">要打开的工作空间类别</param>
        /// <param name="connnectString">连接字符串</param>
        /// <returns></returns>
        public static IWorkspace GetWorksapceFromConnStr(string connnectString, EWorkspaceType eType)
        {
            try
            {
                IWorkspaceName workspaceName = new WorkspaceNameClass();
                workspaceName.WorkspaceFactoryProgID = eType.GetDescription(1);
                workspaceName.ConnectionProperties = ConnectStringToPropetySet(connnectString);
                IName iName = (IName)workspaceName;
                IWorkspace workspace = iName.Open() as IWorkspace;

                Marshal.ReleaseComObject(iName);
                Marshal.ReleaseComObject(workspaceName);
                return workspace;
            }
            catch (Exception ex)
            {
                throw new Exception($"打开{eType.GetDescription(2)}工作空间“{connnectString}”出错：{ex.Message}");
            }
        }
        /// <summary>
        /// 通过路径获取工作空间
        /// </summary>
        /// <param name="eType">要打开的工作空间类别</param>
        /// <param name="path">文件或文件夹路径</param>
        /// <returns></returns>
        public static IWorkspace GetWorkspaceFromFile(string path, EWorkspaceType eType)
        {
            try
            {
                IWorkspaceName workspaceName = new WorkspaceNameClass();
                workspaceName.WorkspaceFactoryProgID = eType.GetDescription(1);
                workspaceName.PathName = path;
                IName iName = (IName)workspaceName;
                IWorkspace workspace = iName.Open() as IWorkspace;

                Marshal.ReleaseComObject(iName);
                Marshal.ReleaseComObject(workspaceName);
                return workspace;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                //判断路径长度是否符合要求（Windows默认的字符的长度限制为260，一个中文字符长度为2）（路径超长不一定出错，mdb数据会出错，shp数据不一定出错）
                if (!PathEx.PathLengthValidate(path, out var length))
                    msg += $"\r\n 可能原因为：路径长度超出限制，无法识别“{path}”的数据，请修改数据存放路径\r\n（允许路径最大长度为260，该路径长度为{length}）\r\n";
                if (msg.Contains("WcsCommonProfileTypes}Text") || msg.Contains("未在 DTD/架构中声明"))
                    msg += $"\r\n 可能原因为：该路径不是{eType.GetDescription()}（{eType} Workspace）";
                throw new Exception($"打开{eType.GetDescription(2)}工作空间“{path}”出错：{msg}");
            }
        }
        /// <summary>
        /// 获取内存工作空间
        /// </summary>
        /// <returns></returns>
        public static IWorkspace GetWorkspaceInMemory() => GetWorkspaceFromFile("in_memory", EWorkspaceType.InMemory);

        /// <summary>
        /// 打开路径下的工作空间，对工作空间执行指定操作，最后释放工作空间
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="connStrOrPath">工作空间路径或连接字符串</param>
        /// <param name="func">打开工作空间后，要对工作空间执行的操作</param>
        /// <param name="eWorkspaceType">工作空间类型，标识优先将strConnOrPath作为打开哪种工作空间的参数</param>
        /// <returns></returns>
        internal static void ToWorkspace(this string connStrOrPath, Action<IWorkspace> action, EWorkspaceType eWorkspaceType = EWorkspaceType.Default)
        {
            var workspace = GetWorkSpace(connStrOrPath, eWorkspaceType);
            if (workspace == null) throw new Exception($"无法按照指定路径或连接字符串“{connStrOrPath}”打开工作空间！");

            action(workspace);
            Marshal.ReleaseComObject(workspace);
        }
        /// <summary>
        /// 打开路径下的工作空间，对工作空间执行指定操作，最后释放工作空间并返回操作结果
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="connStrOrPath">工作空间路径或连接字符串</param>
        /// <param name="func">打开工作空间后，要对工作空间执行的操作</param>
        /// <param name="eWorkspaceType">工作空间类型，标识优先将strConnOrPath作为打开哪种工作空间的参数</param>
        /// <returns></returns>
        internal static TResult ToWorkspace<TResult>(this string connStrOrPath, Func<IWorkspace, TResult> func, EWorkspaceType eWorkspaceType = EWorkspaceType.Default)
        {
            var workspace = GetWorkSpace(connStrOrPath, eWorkspaceType);
            if (workspace == null) throw new Exception($"无法按照指定路径或连接字符串“{connStrOrPath}”打开工作空间！");

            TResult result = func(workspace);
            Marshal.ReleaseComObject(workspace);
            return result;
        }
    }
}
