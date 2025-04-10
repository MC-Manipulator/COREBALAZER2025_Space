#region

//文件创建者：Egg
//创建时间：04-04 03:22

#endregion

#if UNITY_EDITOR
using System;

namespace EggFramework.Util.Excel
{
    [Serializable]
    internal struct ExcelStructRenameConfig
    {
        public string OriginName;
        public string NewName;
    }
}
#endif