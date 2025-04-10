#region

//文件创建者：Egg
//创建时间：11-19 10:12

#endregion

#if UNITY_EDITOR

using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EggFramework.Util.Excel
{
    public sealed class ExcelDataColumnWindow : OdinEditorWindow
    {
        [LabelText("表配置信息"), ValidateInput("Validate")]
        public ExcelTableConfig Config;

        [HideInInspector] public bool IsNewTable;
        [HideInInspector] public ExcelDataColumn Host;
        [HideInInspector] public string          CacheTableName;

        private bool Validate(ExcelTableConfig config, ref string errString)
        {
            if (!IsNewTable && CacheTableName != Config.ConfigName)
            {
                Config.ConfigName = CacheTableName;
                EditorUtility.DisplayDialog("警告","非新建表格不可改名","我知道了");
                return true;
            }
            
            if (string.IsNullOrEmpty(config.ConfigName))
            {
                errString = "没有指定表名";
                return false;
            }

            if (config.Columns.Count <= 0)
            {
                errString = "至少指定一个成员";
                return false;
            }

            return true;
        }

        [Button("保存修改", ButtonSizes.Large)]
        public void Save()
        {
            string e = "";
            if (Validate(Config, ref e))
            {
                Close();
                Host.Save(Config);
            }
        }
    }
}
#endif