﻿/*---------------------------------------------------------------- 
// auth： Windragon
// date： 2019
// desc： None
// mdfy:  None
//----------------------------------------------------------------*/

using System;

namespace WLib.Attributes.Table
{
    /// <summary>
    /// 表示字段只读
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ReadOnlyAttribute : Attribute
    {
    }
}
