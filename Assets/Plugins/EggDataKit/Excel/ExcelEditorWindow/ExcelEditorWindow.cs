#region

//文件创建者：Egg
//创建时间：10-23 11:00

#endregion

#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace EggFramework.Util.Excel
{
    public sealed partial class ExcelEditorWindow : OdinMenuEditorWindow
    {
        [MenuItem("EggFramework/Excel管理面板  #&e")]
        public static void OpenWindow()
        {
            var window = GetWindow<ExcelEditorWindow>();
            window.Show();
            window.titleContent = new GUIContent("Excel管理面板");
        }

        public ExcelSetting           Setting;
        public List<ExcelTableConfig> Configs = new();

        protected override void OnEnable()
        {
            base.OnEnable();
            Setting = StorageUtil.LoadFromSettingFile(nameof(ExcelSetting),           new ExcelSetting());
            Configs = StorageUtil.LoadFromSettingFile(nameof(ExcelTableConfig) + "s", new List<ExcelTableConfig>());
            if (!Setting.Inited)
            {
                InitExcel();
                Setting.Inited = true;
            }

            ReloadExcelTables();

            // ClearUnUsedFile();
        }

        private void ReloadExcelTables()
        {
            var reloadConfigs = StorageUtil.LoadByJson(nameof(ExcelReloadConfig) + "s", new List<ExcelReloadConfig>());
            if (reloadConfigs.Count <= 0) return;
            var structRenameConfig =
                StorageUtil.LoadByJson(nameof(ExcelStructRenameConfig), new ExcelStructRenameConfig());
            foreach (var reloadConfig in reloadConfigs)
            {
                if (string.IsNullOrEmpty(reloadConfig.LastFilePath)) continue;
                var tableName    = reloadConfig.TableName;
                var tableConfig  = Configs.Find(config => config.ConfigName == tableName);
                var configColDic = ExcelUtil.GenerateConfigColDic(tableConfig, ExcelUtil.GetExcelStructConfigByName);
                var pickIndexDic =
                    ExcelUtil.DiffWithColDic(reloadConfig.ConfigColDic, configColDic, structRenameConfig);
                var rawDataBefore = ExcelUtil.LoadFromCustomFile(reloadConfig.LastFilePath);
                var rawDataAfter =
                    ExcelUtil.PickDataByIndex(rawDataBefore, pickIndexDic, configColDic.Count);
                ExcelUtil.WriteTableData(tableConfig, rawDataAfter);
            }

            reloadConfigs.Clear();
            StorageUtil.SaveByJson(nameof(ExcelReloadConfig) + "s", reloadConfigs);
        }

        private void ClearUnUsedFile()
        {
            var excelDataPathRoot = Setting.ExcelDataPathRoot;
            var subDirectories    = Directory.GetDirectories(excelDataPathRoot);
            foreach (var subDirectory in subDirectories)
            {
                if (Configs.All(config => config.ConfigName != DirectoryUtil.ExtractName(subDirectory)))
                {
                    FileUtil.DeleteFileOrDirectory(subDirectory);
                    FileUtil.DeleteFileOrDirectory(subDirectory + ".meta");
                }
            }

            var soDataPathRoot = Setting.DataPathRoot;
            var files          = Directory.GetFiles(soDataPathRoot);
            foreach (var file in files)
            {
                var fileName = DirectoryUtil.ExtractName(file);
                if (fileName is "ExcelRefData" or "ExcelRefData.asset")
                {
                    continue;
                }

                if (Configs.All(config =>
                        config.ConfigName + "SO" != fileName && config.ConfigName + "SO.asset" != fileName))
                {
                    FileUtil.DeleteFileOrDirectory(file);
                }
            }

            var excelDataRef = ResUtil.GetAsset<ExcelRefData>();
            if (!excelDataRef) return;
            var tableTypes = TypeUtil.GetDerivedClasses(typeof(IExcelEntity));
            var deleteList = new List<ExcelDataRef>();
            foreach (var dataRef in excelDataRef.ExcelDataRefs)
            {
                if (tableTypes.Any(tp => tp.Name == dataRef.Name))
                {
                    continue;
                }

                deleteList.Add(dataRef);
            }

            foreach (var dataRef in deleteList)
            {
                excelDataRef.ExcelDataRefs.Remove(dataRef);
            }

            EditorUtility.SetDirty(excelDataRef);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        private void InitExcel()
        {
            DirectoryUtil.MakeSureDirectory(Setting.CodePathRoot);
            DirectoryUtil.MakeSureDirectory(Setting.ExcelDataPathRoot);
            DirectoryUtil.MakeSureDirectory(Setting.DataPathRoot);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            StorageUtil.SaveToSettingFile(nameof(ExcelSetting),           Setting);
            StorageUtil.SaveToSettingFile(nameof(ExcelTableConfig) + "s", Configs);
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree  = new OdinMenuTree { { "控制面板", new ExcelEditorWindowView(Setting, Configs) } };
            var types = TypeUtil.GetDerivedClasses(typeof(IExcelEntitySO));
            foreach (var type in types)
            {
                var data = ResUtil.GetAsset(type);
                tree.Add(type.Name[..^2], data ? data : new DefaultExcelDataView(type));
            }

            var dateViewTypes = TypeUtil.GetDerivedClassesFromGenericClass(typeof(ExcelDataView<,>));
            foreach (var dateViewType in dateViewTypes)
            {
                var data = ResUtil.GetAsset(dateViewType);
                tree.Add(dateViewType.Name[..^8] + "/Preview", data);
            }

            return tree;
        }
    }

    public class DefaultExcelDataView
    {
        private Type _dataType;

        public DefaultExcelDataView(Type type)
        {
            _dataType = type;
        }

        [Button("读取数据", ButtonSizes.Large)]
        public void ReadData()
        {
            ExcelUtil.ReadDataByExcelEntitySOType(_dataType);
        }
    }

    public partial class ExcelEditorWindowView
    {
        [BoxGroup("帮助信息")]
        [ShowInInspector, LabelText("表头标识符")]
        public string TableHeadSign => ExcelUtil.TABLE_HEAD_SIGN;

        [BoxGroup("帮助信息")]
        [ShowInInspector, LabelText("列表数据分割符")]
        public string ListSplitSign => ExcelUtil.LIST_SPLIT_SIGN;

        [BoxGroup("帮助信息")]
        [ShowInInspector, LabelText("Vector类型分割符")]
        public string VectorSplitSign => ExcelUtil.VECTOR_SPLIT_SIGN;

        [BoxGroup("帮助信息/颜色"), ShowInInspector, LabelText("Color类型表示示例")]
        public string ColorStringExample => "#FF8899";

        [BoxGroup("帮助信息/颜色"), ShowInInspector, LabelText("对应颜色")]
        public Color ColorExample => ColorUtil.ParseHexColor(ColorStringExample);

        public void LoadInStructData()
        {
            var types   = TypeUtil.GetDerivedStructs(typeof(IExcelStruct));
            Setting.Configs = types.Select(type =>
            {
                var fields = type.GetSerializeFieldInfos();
                var excelStructConfig = new ExcelStructConfig
                {
                    TypeName = type.Name,
                    Columns  = new List<ExcelColConfig>(),
                };
                foreach (var field in fields)
                {
                    string fieldName;
                    if (field.FieldType.IsGenericType)
                    {
                        fieldName = field.FieldType.GetGenericTypeDefinition() == typeof(List<>)
                            ? field.FieldType.GenericTypeArguments[0].Name
                            : field.FieldType.Name;
                    }
                    else
                    {
                        fieldName = field.FieldType.Name;
                    }

                    var attributes  = field.GetCustomAttributes(typeof(LabelTextAttribute), false);
                    var chineseName = "";
                    if (attributes.Length > 0)
                    {
                        chineseName = ((LabelTextAttribute)attributes[0]).Text;
                    }

                    excelStructConfig.Columns.Add(new ExcelColConfig
                    {
                        Name         = field.Name,
                        ChineseName  = chineseName,
                        ExcelColType = fieldName,
                        IsList       = field.FieldType.IsGenericType
                    });
                }

                return excelStructConfig;
            }).ToList();
            StorageUtil.SaveToSettingFile(nameof(ExcelSetting), Setting);
        }

        public void GenerateExcel(string tableName)
        {
            EditorWindow.GetWindow<ExcelEditorWindow>().Close();
            InnerGenerateExcel(tableName);
        }

        private void InnerGenerateExcel(string tableName)
        {
            var savePath = $"{Setting.DataPathRoot}/{nameof(ExcelRefData)}.asset";
            var data = ResUtil.GetOrCreateAsset<ExcelRefData>(savePath);
            var configs = StorageUtil.LoadFromSettingFile(nameof(ExcelTableConfig) + "s", new List<ExcelTableConfig>());
            if (string.IsNullOrEmpty(tableName))
            {
                data.ExcelDataRefs.Clear();
                foreach (var tableConfig in configs)
                {
                    var excelTextAsset = WriteExcel(tableConfig);

                    WriteCode(tableConfig);

                    data.ExcelDataRefs.Add(new ExcelDataRef
                    {
                        Name      = tableConfig.ConfigName,
                        TextAsset = excelTextAsset
                    });
                }
            }
            else
            {
                var tableConfig = configs.Find(tb => tb.ConfigName == tableName);
                GenerateReloadConfig(tableName);
                if (string.IsNullOrEmpty(tableConfig.ConfigName))
                {
                    Debug.LogError("没有找到对应的表");
                    return;
                }

                var excelTextAsset = WriteExcel(tableConfig);

                WriteCode(tableConfig);

                var index = data.ExcelDataRefs.FindIndex(ef => ef.Name == tableName);
                var ef = new ExcelDataRef
                {
                    Name      = tableConfig.ConfigName,
                    TextAsset = excelTextAsset
                };
                if (index != -1)
                {
                    data.ExcelDataRefs[index] = ef;
                }
                else
                {
                    data.ExcelDataRefs.Add(ef);
                }
            }

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            ResUtil.AddAssetToGroup(Setting.GenerateAddressableDataGroupName, savePath, true, true);
        }

        private void GenerateReloadConfig(string tableName)
        {
            var reloadConfig = StorageUtil.LoadByJson(nameof(ExcelReloadConfig) + "s", new List<ExcelReloadConfig>());

            var cachedTableConfig = ExcelUtil.GetCachedExcelTableConfigByName(tableName);
            //没有获取到缓存的表，说明是新表
            if (string.IsNullOrEmpty(cachedTableConfig.ConfigName)) return;

            var colReloadConfigDic =
                ExcelUtil.GenerateConfigColDic(cachedTableConfig, ExcelUtil.GetCachedExcelStructConfigByName);

            reloadConfig.Add(new ExcelReloadConfig
            {
                TableName    = tableName,
                ConfigColDic = colReloadConfigDic,
                LastFilePath = null
            });
            StorageUtil.SaveByJson(nameof(ExcelReloadConfig) + "s", reloadConfig);
        }


        private void WriteCode(ExcelTableConfig tableConfig)
        {
            WriteTypeCode(tableConfig);

            WriteDataCode(tableConfig);

            WriteDataRefCode(tableConfig);

            WriteDataViewCode(tableConfig);
        }

        private string GetExcelTypeNameSpace(string excelType)
        {
            var resType = TypeUtil.GetResTypeByTypeName(excelType);
            if (resType != null)
            {
                return resType.Namespace;
            }

            if (!ExcelUtil.ExcelTypes.Contains(excelType)) return "EggFramework";
            return "EggFramework.Generator";
        }

        [LabelText("Excel设置"), OnValueChanged("Save", IncludeChildren = true)]
        public ExcelSetting Setting;

        private void Save()
        {
            StorageUtil.SaveToSettingFile(nameof(ExcelSetting), Setting);
        }

        [HideInInspector, LabelText("Excel表格数据")]
        public List<ExcelTableConfig> Configs;

        [ShowInInspector, LabelText("表格列表"),
         ListDrawerSettings(DraggableItems = false, HideAddButton = true, HideRemoveButton = true)]
        public List<ExcelDataColumn> DataColumns;

        [Button("新建表格", ButtonSizes.Large)]
        public void AddNewTable()
        {
            var dataColumn = new ExcelDataColumn(-1, new ExcelTableConfig
            {
                ConfigName = "",
                Columns    = new List<ExcelColConfig>()
            }, this);
            var window = EditorWindow.GetWindow<ExcelDataColumnWindow>();
            window.titleContent = new GUIContent("新建表格");
            window.Host         = dataColumn;
            window.IsNewTable   = true;
            window.Show();
        }

        public void DeleteTable(int index)
        {
            if (index < 0 || index >= Configs.Count) return;
            var target = Configs[index];
            Configs.RemoveAt(index);
            DataColumns.RemoveAt(index);
            EditorWindow.GetWindow<ExcelEditorWindow>().Close();
            Clear(target);
        }

        private void Clear(ExcelTableConfig target)
        {
            var data = (ExcelRefData)ResUtil.GetAsset(typeof(ExcelRefData));
            data.ExcelDataRefs.RemoveAll(refData => refData.Name == target.ConfigName);
            FileUtil.DeleteFileOrDirectory($"{Setting.CodePathRoot}/{target.ConfigName}.ExcelEntity.cs");
            FileUtil.DeleteFileOrDirectory($"{Setting.CodePathRoot}/{target.ConfigName}.ExcelEntity.cs.meta");
            FileUtil.DeleteFileOrDirectory($"{Setting.CodePathRoot}/{target.ConfigName}SO.cs");
            FileUtil.DeleteFileOrDirectory($"{Setting.CodePathRoot}/{target.ConfigName}SO.cs.meta");
            AssetDatabase.Refresh();
        }

        public void SaveTable(int index, ExcelTableConfig data)
        {
            ExcelUtil.CacheExcelTableConfig();
            if (index == -1)
            {
                Configs.Add(data);
            }

            if (index >= 0 && index < Configs.Count)
            {
                Configs[index] = data;
            }

            GenerateExcel(data.ConfigName);
        }

        public ExcelEditorWindowView(ExcelSetting setting, List<ExcelTableConfig> configs, bool loadStructData = true)
        {
            Setting     = setting;
            Configs     = configs;
            DataColumns = Configs?.Select((con, index) => new ExcelDataColumn(index, con, this)).ToList();
            if (loadStructData)
            {
                LoadInStructData();
            }
        }
    }
}
#endif