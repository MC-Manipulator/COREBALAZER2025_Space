#region

//文件创建者：Egg
//创建时间：10-29 08:21

#endregion

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EggFramework.Util.Excel
{
    public sealed class ExcelRefData : ScriptableObject
    {
        [LabelText("Excel文本数据引用")]
        public List<ExcelDataRef> ExcelDataRefs = new();
    }

    [Serializable]
    public struct ExcelDataRef
    {
        [LabelText("表名")]
        public string    Name;
        [LabelText("表资产")]
        public TextAsset TextAsset;
    }
}