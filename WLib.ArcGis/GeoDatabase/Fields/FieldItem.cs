﻿/*---------------------------------------------------------------- 
// auth： Windragon
// date： 2017/1/6 16:20:43
// desc： 由于ArcGIS的IField是COM组件对象，不能反射且难于调试，通过将IField信息转入此类，以方便反射和调试等操作
// mdfy:  None
//----------------------------------------------------------------*/

using ESRI.ArcGIS.Geodatabase;
using System.ComponentModel;

namespace WLib.ArcGis.GeoDatabase.Fields
{
    /// <summary>
    /// 表示字段
    /// <para>包含字段名、别名、类型</para>
    /// </summary>
    public class FieldItem
    {
        /// <summary>
        /// 字段名
        /// </summary>
        [Description("名称")]
        public string Name { get; set; }
        /// <summary>
        /// 字段别名
        /// </summary>
        [Description("别名")]
        public string AliasName { get; set; }
        /// <summary>
        /// 字段类型
        /// </summary>
        [Description("类型")]
        public esriFieldType FieldType { get; set; }
        /// <summary>
        /// 确定ToString方法输出的内容
        /// <para>N-字段名，A-字段别名，F-字段类型，例如Format="N,A,(F)"，则ToString()结果为"字段名,字段别名,(字段类型)"</para>
        /// </summary>
        [Description("输出格式")]
        public string Format { get; set; } = "A";
        /// <summary>
        /// 获得字段类型的文字描述
        /// </summary>
        /// <returns></returns>
        public string FieldTypeDesciption => FieldType.GetFieldTypeDesciption();
        /// <summary>
        /// 获得字段类型的中文文字描述
        /// </summary>
        /// <returns></returns>
        public string FieldTypeDesciptionCn => FieldType.GetFieldTypeDesciptionCn();


        /// <summary>
        /// 表示字段
        /// </summary>
        public FieldItem() { }
        /// <summary>
        /// 表示字段
        /// </summary>
        /// <param name="name">字段名</param>
        /// <param name="aliasName">字段别名</param>
        /// <param name="fieldType">字段类型</param>
        public FieldItem(string name, string aliasName, esriFieldType fieldType)
        {
            Name = name;
            AliasName = aliasName;
            FieldType = fieldType;
        }


        /// <summary>
        /// 按照Format指定的格式，格式化输出字段信息
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Format.Replace("N", Name).Replace("A", AliasName).Replace("F", FieldTypeDesciption);
        }
    }
}
