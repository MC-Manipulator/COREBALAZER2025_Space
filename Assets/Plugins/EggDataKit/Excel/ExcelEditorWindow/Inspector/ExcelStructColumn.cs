#region

//文件创建者：Egg
//创建时间：11-19 11:49

#endregion

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace EggFramework.Util.Excel
{
    [Serializable]
    public sealed class ExcelStructColumn
    {
        private readonly ExcelStructConfig       _config;
        private          int                     _index;
        private          ExcelSetting            _setting;
        private          ExcelStructModifyWindow _window;

        [ShowInInspector, HorizontalGroup("Button"), LabelText("结构名")]
        public string Name => _config.TypeName;

        public ExcelStructColumn(int index, ExcelStructConfig config)
        {
            _index   = index;
            _config  = config;
            _setting = StorageUtil.LoadFromSettingFile("ExcelSetting", new ExcelSetting());
        }

        [HorizontalGroup("Button", 50f), Button("删除")]
        public void Delete()
        {
            _setting.Configs.RemoveAt(_index);
            FileUtil.DeleteFileOrDirectory($"{_setting.CodePathRoot}/{_config.TypeName}.ExcelStruct.cs");
            FileUtil.DeleteFileOrDirectory($"{_setting.CodePathRoot}/{_config.TypeName}.ExcelStruct.cs.meta");
            AssetDatabase.Refresh();
            StorageUtil.SaveToSettingFile("ExcelSetting", _setting);
        }

        [HorizontalGroup("Button", 50f), Button("修改")]
        public void Modify()
        {
            _window              = EditorWindow.GetWindow<ExcelStructModifyWindow>();
            _window.titleContent = new GUIContent("修改结构体");
            _window.Config       = _config;
            _window.Host         = this;
            _window.Show();
        }

        public void Save(ExcelStructConfig config)
        {
            _window.Close();
            EditorWindow.GetWindow<ExcelEditorWindow>().Close();
            //缓存修改之前的结构体
            ExcelUtil.CacheExcelStructConfig();
            ExcelUtil.CacheExcelTableConfig();
            var origin = _setting.Configs[_index];
            _setting.Configs[_index] = config;
            FileUtil.DeleteFileOrDirectory($"{_setting.CodePathRoot}/ExcelStruct/{origin.TypeName}.cs");
            FileUtil.DeleteFileOrDirectory($"{_setting.CodePathRoot}/ExcelStruct/{origin.TypeName}.cs.meta");
            StorageUtil.SaveToSettingFile("ExcelSetting", _setting);
            new ExcelEditorWindowView(_setting, null, false)
                .GenerateNewStruct(config);
            if (origin.TypeName != config.TypeName)
            {
                StorageUtil.SaveByJson(nameof(ExcelStructRenameConfig), new ExcelStructRenameConfig
                {
                    OriginName = origin.TypeName,
                    NewName    = config.TypeName
                });
                var modifiedTableNames = RenameReference(origin, config);
                RegenerateTables(modifiedTableNames);
            }
        }

        private void RegenerateTables(List<string> modifiedTableNames)
        {
            var configs = StorageUtil.LoadFromSettingFile(nameof(ExcelTableConfig) + "s",
                new List<ExcelTableConfig>());
            var view = new ExcelEditorWindowView(_setting, configs, false);
            foreach (var modifiedTableName in modifiedTableNames)
            {
                view.GenerateExcel(modifiedTableName);
            }
        }

        private List<string> RenameReference(ExcelStructConfig origin, ExcelStructConfig now)
        {
            var ret = new List<string>();
            var configs = StorageUtil.LoadFromSettingFile(nameof(ExcelTableConfig) + "s",
                new List<ExcelTableConfig>());
            foreach (var excelTableConfig in configs)
            {
                var renameList = new List<int>();
                for (var index = 0; index < excelTableConfig.Columns.Count; index++)
                {
                    var excelColConfig = excelTableConfig.Columns[index];
                    if (excelColConfig.IsStruct && excelColConfig.ExcelColType == origin.TypeName)
                    {
                        renameList.Add(index);
                        ret.AddIfNotExist(excelTableConfig.ConfigName);
                    }
                }

                foreach (var i in renameList)
                {
                    var clone = CloneMapUtil<ExcelColConfig>.Clone(excelTableConfig.Columns[i]);
                    clone.ExcelColType          = now.TypeName;
                    excelTableConfig.Columns[i] = clone;
                }
            }

            StorageUtil.SaveToSettingFile(nameof(ExcelTableConfig) + "s", configs);
            return ret;
        }
    }
}
#endif