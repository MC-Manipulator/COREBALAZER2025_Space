#region

//文件创建者：Egg
//创建时间：04-02 10:15

#endregion

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace EggFramework.Util.Excel
{
    [Serializable]
    internal sealed class ExcelReloadConfig
    {
        public string                  TableName;
        public Dictionary<string, int> ConfigColDic;
        public string                  LastFilePath;
    }
}
#endif