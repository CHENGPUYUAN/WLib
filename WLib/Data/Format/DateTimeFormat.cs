﻿/*---------------------------------------------------------------- 
// auth： Windragon
// date： 2018
// desc： None
// mdfy:  None
//----------------------------------------------------------------*/

using System;

namespace WLib.Data.Format
{
    /// <summary>
    /// 提供对日期进行格式化输出的方法
    /// </summary>
    public static class DateTimeFormat
    {
        /// <summary>
        /// 分别在"yyyy年"、"M月"、"d日"的左侧用空白字符填充以达到指定的总长度，
        /// 最后整合并返回整个日期字符串
        /// </summary>
        /// <param name="dateTime">需要格式化输出的日期</param>
        /// <param name="totalWidth">填充年份或月份或天数的总长度</param>
        /// <returns></returns>
        public static string PadLeftWhiteSpace(this DateTime dateTime, int totalWidth)
        {
            string strDay = dateTime.Day.ToString().PadLeft(totalWidth);
            string strMonth = dateTime.Month.ToString().PadLeft(totalWidth);
            string strYear = dateTime.Year.ToString().PadLeft(totalWidth);

            return $"{strYear}年{strMonth}月{strDay}日";
        }

        /// <summary>
        /// 将<see cref="DateTime"/>对象转为Unix系统的时间戳（以1970/01/01为初始值的毫秒为单位的时间计数）
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long ToUnixTimeStamp(this DateTime dateTime)
        {
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
            return (long)(dateTime - startTime).TotalMilliseconds; // 相差秒数
        }
        /// <summary>
        /// 将Unix系统的时间戳（以毫秒为单位的时间计数）转为<see cref="DateTime"/>对象
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime UnixTimeToDateTime(this long timeStamp)
        {
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
            return startTime.AddMilliseconds(timeStamp);
        }
    }
}
