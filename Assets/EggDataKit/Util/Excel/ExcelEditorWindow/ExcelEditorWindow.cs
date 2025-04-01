#region

//文件创建者：Egg
//创建时间：10-23 11:00

#endregion

#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EggFramework.Util.Excel
{
    public sealed partial class ExcelEditorWindow : OdinMenuEditorWindow
    {
        [MenuItem("EggFramework/Excel管理面板")]
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

            ClearUnUsedFile();
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
            var tableTypes   = TypeUtil.GetDerivedClasses(typeof(IExcelEntity));
            var deleteList   = new List<ExcelDataRef>();
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
            var tree = new OdinMenuTree();
            tree.Add("控制面板", new ExcelEditorWindowView(Setting, Configs));
            var types = TypeUtil.GetDerivedClasses(typeof(IExcelEntitySO));
            foreach (var type in types)
            {
                var data = ResUtil.GetAsset(type);
                tree.Add(type.Name[..^2], data ? data : new DefaultExcelDataView(type));
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

        [Button("读取数据",ButtonSizes.Large)]
        public void ReadData()
        {
            ExcelUtil.ReadDataByExcelEntitySOType(_dataType);
        }
    }

    public partial class ExcelEditorWindowView
    {
        public void LoadInStructData()
        {
            var types = TypeUtil.GetDerivedStructs(typeof(IExcelStruct));
            Setting.Configs = types.Select(type =>
            {
                var fields = type.GetSerializeFieldInfos();
                var excelStructConfig = new ExcelStructConfig
                {
                    NameSpace = type.Namespace,
                    TypeName  = type.Name,
                    Columns   = new List<ExcelColConfig>(),
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
            StorageUtil.SaveToSettingFile("ExcelSetting", Setting);
        }

        public void GenerateExcel(string tableName)
        {
            EditorWindow.GetWindow<ExcelEditorWindow>().Close();
            InnerGenerateExcel(tableName);
        }

        private void InnerGenerateExcel(string tableName)
        {
            var savePath = $"{Setting.DataPathRoot}/{nameof(ExcelRefData)}.asset";
            var data     = ResUtil.GetOrCreateAsset<ExcelRefData>(savePath);
            if (string.IsNullOrEmpty(tableName))
            {
                data.ExcelDataRefs.Clear();
                foreach (var tableConfig in Configs)
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
                var tableConfig = Configs.Find(tb => tb.ConfigName == tableName);
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
            CompilationPipeline.RequestScriptCompilation();
        }

        private void WriteCode(ExcelTableConfig tableConfig)
        {
            WriteTypeCode(tableConfig);

            WriteDataCode(tableConfig);
        }

        //TODO:

        private string GetExcelTypeNameSpace(string excelType)
        {
            var resType = TypeUtil.GetResTypeByTypeName(excelType);
            if (resType != null)
            {
                return resType.Namespace;
            }

            if (!ExcelUtil.ExcelTypes.Contains(excelType)) return "";
            var config = Setting.Configs.Find(con => con.TypeName == excelType);
            return config.NameSpace;
        }

        private int GetFlattenColCount(List<ExcelColConfig> configs, List<string> colContents, string colPath)
        {
            var colCount = 0;
            foreach (var tableConfigColumn in configs)
            {
                if (TypeUtil.DefaultTypes.Contains(tableConfigColumn.ExcelColType) ||
                    TypeUtil.ResTypes.Contains(tableConfigColumn.ExcelColType) ||
                    TypeUtil.UnityStructTypes.Contains(tableConfigColumn.ExcelColType))
                {
                    colContents.Add(tableConfigColumn.IsList
                        ? $"{colPath}{ExcelUtil.TABLE_HEAD_SIGN}{tableConfigColumn.Name}(List<{tableConfigColumn.ExcelColType}>)"
                        : $"{colPath}{ExcelUtil.TABLE_HEAD_SIGN}{tableConfigColumn.Name}({tableConfigColumn.ExcelColType})");
                    colCount++;
                }
                else if (ExcelUtil.ExcelTypes.Contains(tableConfigColumn.ExcelColType))
                {
                    var structConfig = Setting.Configs.Find(con => con.TypeName == tableConfigColumn.ExcelColType);
                    colCount += GetFlattenColCount(structConfig.Columns, colContents,
                        colPath + $"{ExcelUtil.TABLE_HEAD_SIGN}{tableConfigColumn.Name}({structConfig.TypeName})->");
                }
                else
                {
                    Debug.LogError("没有找到对应类型的ExcelCol成员");
                }
            }

            return colCount;
        }

        [LabelText("Excel设置"), OnValueChanged("Save", IncludeChildren = true)]
        public ExcelSetting Setting;

        private void Save()
        {
            StorageUtil.SaveToSettingFile(nameof(ExcelSetting), Setting);
        }

        [HideInInspector] [LabelText("Excel表格数据")]
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

        public ExcelEditorWindowView(ExcelSetting setting, List<ExcelTableConfig> configs)
        {
            Setting     = setting;
            Configs     = configs;
            DataColumns = Configs?.Select((con, index) => new ExcelDataColumn(index, con, this)).ToList();
            LoadInStructData();
        }
    }
}
#endif