#region

//文件创建者：Egg
//创建时间：10-30 10:49

#endregion

#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EggFramework.CodeGenKit;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using StringWriter = EggFramework.CodeGenKit.Writer.StringWriter;

namespace EggFramework.Util.Excel
{
    public sealed partial class ExcelEditorWindowView
    {
        [Button("新建结构体", ButtonSizes.Large)]
        public void ShowExcelStructCreateWindow()
        {
            var window = EditorWindow.GetWindow<ExcelStructCreateWindow>();
            window.titleContent = new GUIContent("结构体创建窗口");
            window.Host         = this;
            window.Show();
        }

        public void GenerateNewStruct([LabelText("结构体参数")] ExcelStructConfig config)
        {
            EditorWindow.GetWindow<ExcelEditorWindow>().Close();
            EditorWindow.GetWindow<ExcelStructCreateWindow>().Close();
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
            foreach (var nameSpace in from tableConfigColumn in config.Columns
                     where tableConfigColumn.IsStruct
                     select GetExcelTypeNameSpace(tableConfigColumn.ExcelColType))
            {
                if (!string.IsNullOrEmpty(nameSpace.Trim()))
                    nameSpaces.Add(nameSpace);
            }

            foreach (var nameSpace in nameSpaces)
            {
                code.Using(nameSpace);
            }

            code.NameSpace(config.NameSpace, cope =>
            {
                cope.Custom("[Serializable]");
                cope.Struct($"{config.TypeName}", false, "IExcelStruct", (cs) =>
                {
                    foreach (var tableConfigColumn in config.Columns)
                    {
                        var typeName = VariableUtil.BasicTypeName2KeyWord(tableConfigColumn.ExcelColType);
                        cs.Custom(
                            $"[LabelText(\"{(string.IsNullOrEmpty(tableConfigColumn.ChineseName) ? tableConfigColumn.Name : tableConfigColumn.ChineseName)}\")]");
                        cs.Custom(
                            $"public {(tableConfigColumn.IsList ? $"List<{typeName}>" : typeName)} {tableConfigColumn.Name};\n");
                    }
                });
            });
            var codeBuilder = new StringBuilder();
            var writer      = new StringWriter(codeBuilder);
            code.Gen(writer);
            var path = $"{Setting.CodePathRoot}/{config.TypeName}.ExcelStruct.cs";
            File.WriteAllText(path, codeBuilder.ToString());
            AssetDatabase.ImportAsset(path);
            AssetDatabase.Refresh();
            CompilationPipeline.RequestScriptCompilation();
        }
    }
}
#endif