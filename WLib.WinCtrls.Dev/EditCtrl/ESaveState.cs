﻿/*---------------------------------------------------------------- 
// auth： Windragon
// date： 2019/2
// desc： None
// mdfy:  None
//----------------------------------------------------------------*/

using WLib.Attributes.Description;

namespace WLib.WinCtrls.Dev.EditCtrl
{
    /// <summary>
    /// 保存结果状态（0-保存失败，1-保存成功）
    /// </summary>
    public enum ESavedState
    {
        /// <summary>
        /// 未执行保存
        /// </summary>
        [DescriptionEx("未执行保存")]
        UnSaved = -1,
        /// <summary>
        /// 保存失败
        /// </summary>
        [DescriptionEx("保存失败")]
        Fail = 0,
        /// <summary>
        /// 保存成功
        /// </summary>
        [DescriptionEx("保存成功")]
        Success = 1
    }
}
