﻿/*---------------------------------------------------------------- 
// auth： Windragon
// date： 2020
// desc： None
// mdfy:  None
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Text;
using WLib.ArcGis.DataCheck.Compare.Item;
using WLib.ArcGis.DataCheck.Core;

namespace WLib.ArcGis.DataCheck.Compare.Plan
{
    /// <summary>
    /// 对比方案
    /// </summary>
    [Serializable]
    public abstract class ComparePlan
    {
        /// <summary>
        /// 对比方案名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 被对比表格（左表）的路径
        /// </summary>
        public string TablePath { get; set; }
        /// <summary>
        /// 被对比表格（左表）数据筛选条件
        /// </summary>
        public string WhereClause { get; set; }
        /// <summary>
        /// 被对比表格（左表）的作为ID标识的列（例如FID,单元编号等）
        /// </summary>
        public string IdField { get; set; }
        /// <summary>
        /// 对比方案的描述信息
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 不同字段与字段（或图斑与图斑、字段与常数等）的对比信息键值对
        /// </summary>
        public CompareItemCollection CompareItems { get; set; } = new CompareItemCollection();
        /// <summary>
        /// 在数据对比时，数据查询或匹配失败的各类情况和对应处理方式
        /// </summary>
        public Dictionary<ENoneMatchTypes, ENoneMatchHandlers> NoneMatchHandlers = new Dictionary<ENoneMatchTypes, ENoneMatchHandlers>();

        /// <summary>
        /// 添加字段对比项
        /// </summary>
        /// <param name="fieldExpression">表的对比列的表达式</param>
        public FieldCompare AddFieldCompare(string fieldExpression)
        {
            var fieldCompare = new FieldCompare(fieldExpression);
            CompareItems.Add(fieldCompare);
            return fieldCompare;
        }
        /// <summary>
        /// 添加字段对比项
        /// </summary>
        /// <param name="fieldExpression">表的对比列的表达式</param>
        /// <param name="description">不满足对比要求时的表述信息</param>
        public void AddFieldCompare(string fieldExpression, string description)
        {
            var fieldCompare = AddFieldCompare(fieldExpression);
            fieldCompare.Description = description;
        }
        /// <summary>
        /// 添加字段对比项
        /// </summary>
        /// <param name="fieldExpression">表的对比列的表达式</param>
        /// <param name="leftColumnName"></param>
        /// <param name="rightColumnName"></param>
        /// <param name="description">不满足对比要求时的表述信息</param>
        public void AddFieldCompare(string fieldExpression, string leftColumnName, string rightColumnName, string description)
        {
            CompareItems.Add(new FieldCompare()
            {
                Description = description,
                FieldExpression = fieldExpression,
                LeftColumnName = leftColumnName,
                RightColumnName = rightColumnName,
            });
        }
        /// <summary>
        /// 添加多个字段对比项
        /// </summary>
        /// <param name="fieldExpressions">表的对比列的表达式</param>
        public void AddFieldCompares(params string[] fieldExpressions)
        {
            if (fieldExpressions == null) return;
            foreach (var fieldExpression in fieldExpressions)
                AddFieldCompare(fieldExpression);
        }
        /// <summary>
        /// 获取对比方案涉及的字段
        /// </summary>
        /// <param name="leftFields">（表连接中的）左表的字段</param>
        /// <param name="rightFields">（表连接中的）右表的字段</param>
        public virtual void GetCompareFields(out List<string> leftFields, out List<string> rightFields)
        {
            if (string.IsNullOrWhiteSpace(IdField))
                throw new Exception($"ID字段（参数{nameof(IdField)}）不能为空！请对{nameof(IdField)}进行赋值！");

            CompareItems.GetCompareFields(out leftFields, out rightFields);
            leftFields.Insert(0, IdField);
            rightFields.Insert(0, IdField);
        }
        /// <summary>
        /// 显示对比方案名称
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Name;
        /// <summary>
        /// 获取对比方案中包含的全部信息
        /// </summary>
        /// <returns></returns>
        public virtual string GetAllInfo()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"--------{Name}---------");
            sb.AppendLine("左表：" + TablePath);
            sb.AppendLine("右表：");
            sb.AppendLine("左表筛选：" + WhereClause);
            sb.AppendLine("右表筛选：");
            sb.AppendLine("左表ID：" + IdField);
            sb.AppendLine("右表ID：");
            foreach (var line in CompareItems.GetAllInfo())
                sb.AppendLine(line);

            return sb.ToString();
        }
    }
}
