﻿/*---------------------------------------------------------------- 
// auth： Windragon
// date： 2017/5/23 14:04:44
// desc： None
// mdfy:  None
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace WLib.Attributes.Description
{
    /// <summary>
    /// 获取对枚举描述特性的帮助类
    /// (使用反射机制，调用时注意效率优化)
    /// </summary> 
    public static class EnumDescriptionExHelper
    {
        #region 获取枚举的描述
        /// <summary>
        /// 获得指定枚举值的第一个描述，即获取附加在枚举值上的特性<see cref="DescriptionExAttribute.Description"/>属性
        /// </summary>
        /// <param name="value">枚举值</param>
        /// <param name="decriptionTag">对枚举的描述的分类标签，用于对描述进行分组</param>
        /// <returns></returns>
        public static string GetDescription(this Enum value, int decriptionTag = 0)
        {
            if (value == null)
                throw new ArgumentException($"枚举参数{nameof(value)}为空，请确认参数{nameof(value)}为枚举类型！");

            var name = value.ToString();
            var fieldInfo = value.GetType().GetField(name);
            var attributes = (DescriptionExAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionExAttribute), false);
            if (attributes.Length <= 0)
                return name;

            var attribute = attributes.FirstOrDefault(v => v.DescriptionTag == decriptionTag);
            return attribute?.Description;
        }
        /// <summary>
        /// 根据枚举值对应的常量值，获得对应枚举值的第一个描述
        /// <para>即获取附加在枚举值上的特性<see cref="DescriptionExAttribute.Description"/>属性，泛型T必须是枚举类型</para>
        /// </summary>
        /// <param name="value">某个枚举名对应的常量值</param>
        /// <param name="descriptionTag">对枚举的描述的分类标签</param>
        /// <returns></returns>
        public static string GetDescription<T>(this int value, int descriptionTag = 0) where T : struct
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new Exception($"类型{type.Name}不是枚举类型！");

            var enumName = Enum.GetName(type, value);
            var dict = GetNameAndDescriptionDict<T>(descriptionTag);
            if (dict.ContainsKey(enumName))
                return dict[enumName];

            throw new Exception($"对于枚举{type.Name}，无法根据枚举常量值{value}找到对应的枚举名！");
        }
        /// <summary>
        /// 获得枚举的所有枚举值对应的第一个描述
        /// <para>即获取附加在枚举值上的特性<see cref="DescriptionExAttribute.Description"/>属性，泛型T必须是枚举类型</para>
        /// </summary>
        /// <returns></returns>
        public static string[] GetDescriptions<T>(int descriptionTag = 0) where T : struct
        {
            return GetNameAndDescriptionDict<T>(descriptionTag).Values.ToArray();
        }
        /// <summary>
        /// 获得多个枚举值对应的第一个描述
        /// <para>即获取附加在枚举值上的特性<see cref="DescriptionExAttribute.Description"/>属性，泛型T必须是枚举类型</para>
        /// </summary>
        /// <param name="values">枚举值集合</param>
        /// <param name="descriptionTag">对枚举的描述的分类标签</param>
        /// <returns></returns>
        public static string[] GetDescriptions<T>(this IEnumerable<T> values, int descriptionTag = 0) where T : struct
        {
            return values.Select(v => GetDescription(v as Enum, descriptionTag)).ToArray();
        }
        /// <summary>
        /// 获得枚举的所有枚举值的名称和第一个描述的键值对
        /// <para>即获取枚举值名称和附加在枚举值上的特性<see cref="DescriptionExAttribute.Description"/>属性，泛型T必须是枚举类型</para>
        /// </summary>
        /// <param name="decriptionTag">对枚举的描述的分类标签</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetNameAndDescriptionDict<T>(int decriptionTag = 0) where T : struct
        {
            var enumType = typeof(T);
            if (!enumType.IsEnum) throw new Exception($"类型{enumType.Name}不是枚举类型！");

            var names = Enum.GetNames(enumType);
            var result = new Dictionary<string, string>();
            foreach (var name in names)
            {
                var fieldInfo = enumType.GetField(name);
                var attributes = (DescriptionExAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionExAttribute), false);
                if (attributes.Length > 0)
                {
                    var attribute = attributes.FirstOrDefault(v => v.DescriptionTag == decriptionTag);
                    result.Add(name, attribute == null ? name : attribute.Description);
                }
                else
                {
                    result.Add(name, name);
                }
            }
            return result;
        }
        /// <summary>
        /// 获得枚举的所有枚举值对应常量和第一个描述的键值对
        /// <para>即获取枚举值对应常量和附加在枚举值上的特性<see cref="DescriptionExAttribute.Description"/>属性，泛型T必须是枚举类型</para>
        /// </summary>
        /// <param name="descriptionTag">对枚举的描述的分类标签</param>
        /// <returns></returns>
        public static Dictionary<int, string> GetConstAndDescriptionDict<T>(int descriptionTag = 0) where T : struct
        {
            var constValues = GetEnumConst<T>();
            var descptValues = GetDescriptions<T>(descriptionTag);
            var dict = new Dictionary<int, string>();
            for (int i = 0; i < constValues.Length; i++)
                dict.Add(constValues[i], descptValues[i]);
            return dict;
        }
        /// <summary>
        /// 获得枚举的所有枚举值对应常量和第一个描述的键值对
        /// <para>即获取枚举值和附加在枚举值上的特性<see cref="DescriptionExAttribute.Description"/>属性，泛型 <typeparamref name="T"/> 必须是枚举类型</para>
        /// </summary>
        /// <param name="descriptionTag">对枚举的描述的分类标签</param>
        /// <returns></returns>
        public static Dictionary<T, string> GetValueAndDescriptionDict<T>(int descriptionTag = 0) where T : struct
        {
            var values = GetEnums<T>();
            var descptValues = GetDescriptions<T>(descriptionTag);
            var dict = new Dictionary<T, string>();
            for (int i = 0; i < values.Length; i++)
                dict.Add(values[i], descptValues[i]);
            return dict;
        }
        /// <summary>
        /// 获取附加在枚举值上的<see cref="DescriptionExAttribute"/>特性
        /// </summary>
        /// <param name="value">枚举值</param>
        /// <returns></returns>
        public static DescriptionExAttribute[] GetEnumDescriptionAttributes(this Enum value)
        {
            var name = value.ToString();
            var fieldInfo = value.GetType().GetField(name);
            return (DescriptionExAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionExAttribute), false);
        }
        #endregion


        #region 获取枚举值
        /// <summary>
        /// 根据枚举的第一个描述（即<see cref="DescriptionExAttribute.Description"/>属性），返回枚举值，T必须是枚举类型
        /// </summary>
        /// <param name="description">指定枚举值的描述，此值应是枚举的自定义特性EnumDescriptionAttribute的Description属性值</param>
        /// <param name="descriptionTag">对枚举的描述的分类标签</param>
        /// <returns>返回查找的枚举值</returns>
        public static T GetEnum<T>(this string description, int descriptionTag = 0) where T : struct
        {
            object result = null;
            var dict = GetNameAndDescriptionDict<T>(descriptionTag);
            if (dict.ContainsValue(description))
            {
                var enumName = dict.First(v => v.Value.Equals(description)).Key;
                result = Enum.Parse(typeof(T), enumName);
            }
            return (T)result;
        }
        /// <summary>
        /// 获取指定枚举的全部枚举值
        /// </summary>
        /// <returns>返回全部枚举值</returns>
        public static T[] GetEnums<T>() where T : struct
        {
            var enumNames = Enum.GetNames(typeof(T));
            T[] values = new T[enumNames.Length];
            for (int i = 0; i < enumNames.Length; i++)
                values[i] = (T)Enum.Parse(typeof(T), enumNames[i]);
            return values;
        }
        #endregion


        #region 获取枚举的常量值
        /// <summary>
        /// 获取枚举值的全部常量值，T必须是枚举类型
        /// </summary>
        /// <returns></returns>
        public static int[] GetEnumConst<T>() where T : struct
        {
            return Enum.GetValues(typeof(T)).Cast<int>().ToArray();
        }
        /// <summary>
        /// 获取枚举中大于指定值的常量值，T必须是枚举类型
        /// </summary>
        /// <param name="minValue">指定的最小常量值，返回枚举值的对应常量中，大于此值的常量</param>
        /// <returns></returns>
        public static int[] GetEnumConst<T>(int minValue) where T : struct
        {
            return Enum.GetValues(typeof(T)).Cast<int>().Where(v => v > minValue).ToArray();
        }
        /// <summary>
        /// 根据枚举的第一个描述获取枚举的常量值，T必须是枚举类型
        /// </summary>
        /// <param name="description">指定枚举值的描述，此值应是附加在枚举值上的自定义特性<see cref="DescriptionExAttribute.Description"/>属性值</param>
        /// <param name="descriptionTag">对枚举的描述的分类标签</param>
        /// <returns></returns>
        public static int GetEnumConst<T>(this string description, int descriptionTag = 0) where T : struct
        {
            object result = null;
            var dict = GetNameAndDescriptionDict<T>(descriptionTag);
            if (dict.ContainsValue(description))
            {
                var enumName = dict.First(v => v.Value.Equals(description)).Key;
                result = Enum.Parse(typeof(T), enumName);
            }
            return (int)result;
        }
        #endregion

        /// <summary>
        /// 获取枚举值的名称
        /// </summary>
        /// <returns></returns>
        public static string GetName<T>(this T t) where T : struct
        {
            return Enum.GetName(typeof(T), t);
        }
    }
}
