#region

//文件创建者：Egg
//创建时间：10-24 01:28

#endregion

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace EggFramework.Util.Res
{
    [Serializable]
    public sealed class ResSetting
    {
        [ValueDropdown("@EggFramework.Util.DirectoryUtil.GetFilePaths()")] [LabelText("资源引用数据文件存放路径")]
        public string ResRefDataSavePath = "Assets/Data/ResRefData";

        [LabelText("资源引用文件Addressable资源组")] public string ResRefDataAddressableGroupName = "ResRefData";

        [ValueDropdown("@EggFramework.Util.DirectoryUtil.GetFilePaths()")] [LabelText("自定义资源引用文件生成代码路径")]
        public string CustomResRefCodePath = "Assets/Scripts/Generator/Res";
        
    }
}