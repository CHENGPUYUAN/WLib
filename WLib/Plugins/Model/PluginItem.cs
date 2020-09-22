﻿/*---------------------------------------------------------------- 
// auth： Windragon
// date： 2019/7
// desc： None
// mdfy:  None
//----------------------------------------------------------------*/

using Newtonsoft.Json;
using System;
using WLib.Files;
using WLib.Plugins.Enum;
using WLib.Plugins.Interface;

namespace WLib.Plugins.Model
{
    /// <summary>
    /// 插件项
    /// <para>通常对应一个按钮、菜单项</para>
    /// </summary>
    [Serializable]
    public class PluginItem : IPluginItem
    {
        /// <summary>
        /// 插件ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 插件名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 插件标题
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 插件索引
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 插件对应的命令类型所在程序集文件路径
        /// </summary>
        public string AssemblyPath { get; set; }
        /// <summary>
        /// 插件对应的命令类型完全限定名（命名空间 + 类型名）
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 图标文件路径
        /// </summary>
        public string IconPath { get; set; }
        /// <summary>
        /// 提示信息
        /// </summary>
        public string Tips { get; set; }
        /// <summary>
        /// 快捷键
        /// </summary>
        public string ShortcutKeys { get; set; }
        /// <summary>
        /// 是否在该插件前面加入分隔符
        /// </summary>
        public bool AppendSplit { get; set; }
        /// <summary>
        /// 是否在界面上显示
        /// </summary>
        public bool Visible { get; set; } = true;
        /// <summary>
        /// 插件调用类型
        /// <para>在单击时调用插件、在视图加载时调用插件、在视图关闭时调用插件、自定义插件调用时机</para>
        /// </summary>
        public EPluginInvokeType InvokType { get; set; } = EPluginInvokeType.OnClick;
        /// <summary>
        /// 插件对应的命令
        /// </summary>
        [JsonIgnore]
        public ICommand Command { get; set; }


        /// <summary>
        /// 创建插件项，创建GUID赋值插件项Id
        /// </summary>
        public PluginItem() => Id = Guid.NewGuid().ToString();
        /// <summary>
        /// 将命令转为插件
        /// </summary>
        /// <param name="cmd">通过反射获取的功能命令</param>
        /// <param name="appPath">插件所属应用软件的位置，插件的命令位置将设置为对于应用程序的相对路径</param>
        /// <returns></returns>
        public static PluginItem FromCommand(ICommand cmd, string appPath)
        {
            var type = cmd.GetType();
            var assemblyPath = PathEx.GetRelativePath(type.Assembly.Location, appPath);
            return new PluginItem
            {
                Id = Guid.NewGuid().ToString(),
                Name = cmd.Name,
                Text = cmd.Text,
                AssemblyPath = assemblyPath,
                TypeName = type.FullName,
                Tips = cmd.Description,
                Visible = true,
                InvokType = EPluginInvokeType.OnClick,
                Command = cmd,
            };
        }
        /// <summary>
        /// 输出<see cref="Text"/>的值
        /// </summary>
        /// <returns></returns>
        public override string ToString() => AppendSplit ? Text + "（|）" : Text;
    }
}
