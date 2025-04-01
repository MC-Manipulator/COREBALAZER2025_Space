#region

//文件创建者：Egg
//创建时间：10-30 10:44

#endregion

#if UNITY_EDITOR


using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EggFramework.CodeGenKit;
using UnityEditor;
using StringWriter = EggFramework.CodeGenKit.Writer.StringWriter;

namespace EggFramework.Util.Excel
{
    public sealed partial class ExcelEditorWindowView
    {
        private void WriteTypeCode(ExcelTableConfig tableConfig)
        {
            var code = new RootCode();
            code.Custom("//代码使用工具生成，请勿随意修改");
            code.Using("System");
            code.Using("UnityEngine");
            code.Using("EggFramework");
            code.Using("EggFramework.Util.Excel");
            code.Using("System.Collections.Generic");
            code.Using("System.Linq");
            code.Using("Sirenix.OdinInspector");
            var nameSpaces = new HashSet<string>();
            foreach (var nameSpace in from tableConfigColumn in tableConfig.Columns
                     where tableConfigColumn.IsStruct || tableConfigColumn.IsRes
                     select GetExcelTypeNameSpace(tableConfigColumn.ExcelColType))
            {
                if (!string.IsNullOrEmpty(nameSpace.Trim()))
                    nameSpaces.Add(nameSpace);
            }

            foreach (var nameSpace in nameSpaces)
            {
                code.Using(nameSpace);
            }

            code.NameSpace(Setting.GenerateDataClassNameSpace, cope =>
            {
                cope.Custom("[Serializable]");
                cope.Class($"{tableConfig.ConfigName}", false, false, true, "IExcelEntity", (cs) =>
                {
                    foreach (var tableConfigColumn in tableConfig.Columns)
                    {
                        var typeName = VariableUtil.BasicTypeName2KeyWord(tableConfigColumn.ExcelColType);
                        cs.Custom(
                            $"[LabelText(\"{(string.IsNullOrEmpty(tableConfigColumn.ChineseName) ? tableConfigColumn.Name : tableConfigColumn.ChineseName)}\")]");
                        cs.Custom(
                            $"public {(tableConfigColumn.IsList ? $"List<{typeName}>" : typeName)} {tableConfigColumn.Name};\n");
                    }

                    cs.Custom($"public {tableConfig.ConfigName} Clone(){{");
                    cs.Custom($"    return new {tableConfig.ConfigName}{{");
                    foreach (var tableConfigColumn in tableConfig.Columns)
                    {
                        cs.Custom(
                            $"      {tableConfigColumn.Name} = {(tableConfigColumn.IsList ? $"{tableConfigColumn.Name}.ToList()" : tableConfigColumn.Name)},");
                    }

                    cs.Custom("     };");
                    cs.Custom("}");
                });
            });
            var codeBuilder = new StringBuilder();
            var writer      = new StringWriter(codeBuilder);
            code.Gen(writer);
            var path = $"{Setting.CodePathRoot}/{tableConfig.ConfigName}.ExcelEntity.cs";
            File.WriteAllText(path, codeBuilder.ToString());
            AssetDatabase.ImportAsset(path);
            AssetDatabase.Refresh();
        }
    }
}
#endif