﻿/*---------------------------------------------------------------- 
// auth： Windragon
// date： 2018
// desc： None
// mdfy:  None
//----------------------------------------------------------------*/

using System.Collections.Generic;
using ESRI.ArcGIS.Geodatabase;

namespace WLib.ArcGis.GeoDatabase
{
    /// <summary>
    /// 子类型和属性域操作
    /// </summary>
    public static class SubTypeDomian
    {
        /// <summary>  
        /// 创建属性域  
        /// </summary>  
        /// <param name="workspace">工作空间</param>  
        /// <param name="strDomainName">属性域名称</param>  
        /// <param name="dicDomainItems">属性域的项</param>  
        public static void CreateDomain(this IWorkspace workspace, string strDomainName, Dictionary<string, string> dicDomainItems)
        {
            IWorkspaceDomains wsDomains = (IWorkspaceDomains)workspace;
            ICodedValueDomain codeValueDomain = new CodedValueDomainClass();
            foreach (var domainItem in dicDomainItems)
                codeValueDomain.AddCode(domainItem.Key, domainItem.Value);

            IDomain domain = (IDomain)codeValueDomain;
            domain.Name = strDomainName;
            domain.FieldType = esriFieldType.esriFieldTypeString;
            domain.SplitPolicy = esriSplitPolicyType.esriSPTDuplicate;
            domain.MergePolicy = esriMergePolicyType.esriMPTDefaultValue;

            wsDomains.AddDomain(domain);
        }
        /// <summary>  
        /// 创建子类  
        /// </summary>  
        /// <param name="featureClass">要创建子类的要素类</param>  
        /// <param name="subTypeFieldName">需创建子类的字段名</param>  
        /// <param name="dicSubtypeItems">子类项</param>  
        public static void CreateSubtypes(this IFeatureClass featureClass, string subTypeFieldName, Dictionary<int, string> dicSubtypeItems)
        {
            ISubtypes subtypes = featureClass as ISubtypes;
            subtypes.SubtypeFieldName = subTypeFieldName;
            foreach (var subtypeItem in dicSubtypeItems)
                subtypes.AddSubtype(subtypeItem.Key, subtypeItem.Value);

            subtypes.DefaultSubtypeCode = 0;
        }
    }
}
