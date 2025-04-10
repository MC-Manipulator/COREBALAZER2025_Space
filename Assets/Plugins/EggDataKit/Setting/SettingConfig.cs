#region

//文件创建者：Egg
//创建时间：10-25 11:00

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EggFramework.Util
{
    [CreateAssetMenu(menuName = "Data/SettingConfig")]
    public sealed class SettingConfig : ScriptableObject
    {
        [ValueDropdown("GetFileNames")]
        [LabelText("当前的配置文件")]
        public string CurrentSettingFile;
        public IEnumerable<string> GetFileNames()
        {
            return SettingFiles.Select(file => file.name);
        }
        [LabelText("配置文件列表")]
        public List<TextAsset> SettingFiles = new();
    }
}