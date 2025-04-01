#region

//文件创建者：Egg
//创建时间：10-30 10:45

#endregion

#if UNITY_EDITOR


using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EggFramework.Util.Excel
{
    public sealed partial class ExcelEditorWindowView
    {
        private TextAsset WriteExcel(ExcelTableConfig tableConfig)
        {
            var colNames = new List<string>();
            GetFlattenColCount(tableConfig.Columns, colNames, "");
            var builder        = new StringBuilder();
            var lastColContent = "";
            var depth          = colNames.Max(colName => colName.Split("->").Length);
            for (var i = 0; i < depth; i++)
            {
                for (var index = 0; index < colNames.Count; index++)
                {
                    var colName = colNames[index];
                    var colTree = colName.Split("->");

                    if (colTree.Length < depth - i ||
                        colTree[i - depth + colTree.Length] == lastColContent)
                    {
                        builder.Append(index == colNames.Count - 1 ? "" : ",");
                        continue;
                    }

                    var sampleContent = colTree[i - depth + colTree.Length];

                    lastColContent = sampleContent;
                    if (index == colNames.Count - 1)
                        builder.Append(sampleContent);
                    else builder.Append(sampleContent + ",");
                }

                builder.Append("\n");
            }

            var path = $"{Setting.ExcelDataPathRoot}/{tableConfig.ConfigName}/{tableConfig.ConfigName}.csv";
            DirectoryUtil.MakeSureDirectory($"{Setting.ExcelDataPathRoot}/{tableConfig.ConfigName}");
            if (File.Exists(path))
            {
                var newPath = path.Replace(".csv", "_") +
                              DateTime.Now.ToString(CultureInfo.InvariantCulture).Replace("/", "").Replace(" ", "")
                                  .Replace(":", "") +
                              ".csv";
                var currentDirectory = Directory.GetCurrentDirectory();
                File.Move(Path.Combine(currentDirectory, path), Path.Combine(currentDirectory, newPath));
            }

            File.WriteAllText(path, builder.ToString());
            AssetDatabase.ImportAsset(path);
            AssetDatabase.Refresh();
            return AssetDatabase.LoadAssetAtPath<TextAsset>(path);
        }
    }
}
#endif