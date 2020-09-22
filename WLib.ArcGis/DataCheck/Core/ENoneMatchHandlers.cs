﻿/*---------------------------------------------------------------- 
// auth： Windragon
// date： 2020
// desc： None
// mdfy:  None
//----------------------------------------------------------------*/

namespace WLib.ArcGis.DataCheck.Core
{
    /// <summary>
    /// 在数据检查时，数据查询或匹配失败的处理方式
    /// </summary>
    public enum ENoneMatchHandlers
    {
        /// <summary>
        /// 以异常信息处理
        /// </summary>
        Error,
        /// <summary>
        /// 以警告信息处理
        /// </summary>
        Warnning,
        /// <summary>
        /// 以提示信息处理
        /// </summary>
        Tips,
        /// <summary>
        /// 忽略，不进行处理
        /// </summary>
        Ignore,
    }
}
