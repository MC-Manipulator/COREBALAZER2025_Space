#region

//文件创建者：Egg
//创建时间：10-23 11:11

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EggFramework.Util.Excel
{
    [Serializable]
    public struct ExcelTableConfig
    {
        [LabelText("表名"), ValidateInput("CheckName")]
        public string ConfigName;

        [LabelText("表格头成员"), ValidateInput("CheckUnique")]
        public List<ExcelColConfig> Columns;
#if UNITY_EDITOR

        private bool CheckName(string name, ref string errMsg)
        {
            if (StartsWithDigitRegex(name))
            {
                errMsg = "不能以数字开头";
                return false;
            }

            return true;

            static bool StartsWithDigitRegex(string input)
            {
                return !string.IsNullOrEmpty(input) && Regex.IsMatch(input, @"^\d");
            }
        }

        private bool CheckUnique(List<ExcelColConfig> configs, ref string errMsg)
        {
            var list = new List<string>();
            foreach (var excelColConfig in configs)
            {
                if (!list.Contains(excelColConfig.Name))
                {
                    list.Add(excelColConfig.Name);
                }
                else
                {
                    errMsg = "字段名称不可以重复";
                    return false;
                }
            }

            return true;
        }
#endif
    }

    [Serializable]
    public struct ExcelColConfig
    {
#if UNITY_EDITOR
        private bool CheckValidate(string type, ref string errMsg)
        {
            if (!IsStruct || !IsList)
            {
                return true;
            }

            errMsg = "结构体不支持List";
            return false;
        }

        private bool CheckValidateBool(bool type, ref string errMsg)
        {
            if (!IsStruct || !IsList) return true;
            errMsg = "结构体不支持List";
            return false;
        }

        [ValueDropdown("@ExcelUtil.ExcelTypes")]
        [ValidateInput("CheckValidate", "结构体不支持List")]
#endif
        [LabelText("数据类型")]
        public string ExcelColType;
#if UNITY_EDITOR
        [ValidateInput("CheckValidateBool", "结构体不支持List")] [LabelText("是否为列表")]
#endif
        public bool IsList;

        [LabelText("变量名称"), ValidateInput("CheckName")]
        public string Name;

        [LabelText("中文名称")] public string ChineseName;
#if UNITY_EDITOR

        private bool CheckName(string name, ref string errMsg)
        {
            if (StartsWithDigitRegex(name))
            {
                errMsg = "不能以数字开头";
                return false;
            }

            return true;

            static bool StartsWithDigitRegex(string input)
            {
                return !string.IsNullOrEmpty(input) && Regex.IsMatch(input, @"^\d");
            }
        }

        [JsonIgnore]
        public bool IsStruct =>
            !TypeUtil.DefaultTypes.Contains(ExcelColType) && !TypeUtil.ResTypes.Contains(ExcelColType) &&
            !TypeUtil.UnityStructTypes.Contains(ExcelColType);

        [JsonIgnore] public bool IsRes => TypeUtil.ResTypes.Contains(ExcelColType);

#endif
    }

    [Serializable]
    public struct ExcelStructConfig
    {
        [LabelText("结构体名称"), ValidateInput("CheckName")]
        public string TypeName;

        [InfoBox("慎用结构体嵌套,结构体嵌套结构体可能会引发未知错误"), LabelText("结构体成员"),
         ValidateInput("CheckUnique")]
        public List<ExcelColConfig> Columns;

#if UNITY_EDITOR
        private bool CheckName(string name, ref string errMsg)
        {
            if (StartsWithDigitRegex(name))
            {
                errMsg = "不能以数字开头";
                return false;
            }

            return true;

            static bool StartsWithDigitRegex(string input)
            {
                return !string.IsNullOrEmpty(input) && Regex.IsMatch(input, @"^\d");
            }
        }

        private bool CheckUnique(List<ExcelColConfig> configs, ref string errMsg)
        {
            var list = new List<string>();
            foreach (var excelColConfig in configs)
            {
                if (!list.Contains(excelColConfig.Name))
                {
                    list.Add(excelColConfig.Name);
                }
                else
                {
                    errMsg = "字段名称不可以重复";
                    return false;
                }
            }

            return true;
        }
#endif
    }
}