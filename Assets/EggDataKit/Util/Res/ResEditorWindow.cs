#region

//文件创建者：Egg
//创建时间：10-24 01:24

#endregion


#if UNITY_EDITOR

using System;
using System.IO;
using System.Linq;
using System.Text;
using EggFramework.CodeGenKit;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Object = UnityEngine.Object;
using StringWriter = EggFramework.CodeGenKit.Writer.StringWriter;

namespace EggFramework.Util.Res
{
    public sealed partial class ResEditorWindow : OdinMenuEditorWindow
    {
        [MenuItem("EggFramework/资源管理面板")]
        public static void OpenWindow()
        {
            var window = GetWindow<ResEditorWindow>();
            window.Show();
            window.titleContent = new GUIContent("资源管理面板");
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree  = new OdinMenuTree { { "控制面板", new ResEditorWindowView(Setting) } };
            var types = TypeUtil.GetDerivedClassesFromGenericClass(typeof(ResRefData<>));
            foreach (var type in types)
            {
                tree.Add(type.Name[6..], ResUtil.GetAsset(type));
            }

            return tree;
        }

        [LabelText("资源设置")] public ResSetting Setting;

        protected override void OnEnable()
        {
            base.OnEnable();
            Setting = StorageUtil.LoadFromSettingFile(nameof(ResSetting), new ResSetting());
            ClearUnUsedFiles();
            ResUtil.RefreshResRef();
        }

        private void ClearUnUsedFiles()
        {
            var pathRoot = Setting.ResRefDataSavePath;
            if (!Directory.Exists(pathRoot)) return;
            var files = Directory.GetFiles(pathRoot);
            var typeNames = TypeUtil.GetDerivedClassesFromGenericClass(typeof(ResRefData<>)).Select(type => type.Name)
                .ToList();
            foreach (var file in files)
            {
                var fileName = DirectoryUtil.ExtractName(file);
                if (typeNames.All(typeName => fileName != typeName && fileName != typeName + ".asset"))
                {
                    FileUtil.DeleteFileOrDirectory(file);
                }
            }

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            StorageUtil.SaveToSettingFile(nameof(ResSetting), Setting);
        }
    }

    public class ResEditorWindowView
    {
        [LabelText("资源设置")] public ResSetting Setting;

        public ResEditorWindowView(ResSetting setting)
        {
            Setting = setting;
        }

        [Button("生成自定义资源引用文件代码", ButtonSizes.Large)]
        public void GenerateResRefCode([LabelText("目标样本")] Object targetObject)
        {
            if (targetObject == null) return;
            var type = targetObject.GetType();
            EditorWindow.GetWindow<ResEditorWindow>().Close();

            WriteResRefDataCode(type);
            CompilationPipeline.RequestScriptCompilation();
        }

        private void WriteResRefDataCode(Type type)
        {
            var rootCode = new RootCode();
            rootCode.Custom("#if UNITY_EDITOR");
            rootCode.Using(type.Namespace);
            rootCode.NameSpace("EggFramework.Util.Res",
                ns => { ns.Class($"ResRef{type.Name}", false, false, true, $"ResRefData<{type.Name}>", cs => { }); });
            rootCode.Custom("#endif");
            var builder = new StringBuilder();
            var writer  = new StringWriter(builder);
            rootCode.Gen(writer);
            var path = $"{Setting.CustomResRefCodePath}/ResRef{type.Name}.cs";
            DirectoryUtil.MakeSureDirectory(Setting.CustomResRefCodePath);
            File.WriteAllText(path, builder.ToString());
            AssetDatabase.ImportAsset(path);
            AssetDatabase.Refresh();
        }
    }
}
#endif