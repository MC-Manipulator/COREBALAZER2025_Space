#region

//文件创建者：Egg
//创建时间：10-25 11:07

#endregion

#if UNITY_EDITOR


using System;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EggFramework.Util
{
    public sealed class SettingEditorWindow : OdinEditorWindow
    {
        [MenuItem("EggFramework/设置配置面板")]
        public static void OpenWindow()
        {
            var window = GetWindow<SettingEditorWindow>();
            window.titleContent = new GUIContent("设置配置面板");
            window.Show();
        }

        [ShowInInspector] [LabelText("设置文件路径")]
        public const string SETTING_FILE_PATH = "Assets/Setting";

        [LabelText("配置设置文件")] public SettingConfig SettingConfig;

        [ValueDropdown("@SettingConfig.GetFileNames()")]
        [ShowInInspector]
        [LabelText("当前配置文件")]
        public string CurrentSettingFile
        {
            get => SettingConfig.CurrentSettingFile;
            set => SettingConfig.CurrentSettingFile = value;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SettingConfig = ResUtil.GetAsset<SettingConfig>();
            if (SettingConfig != null) return;
            SettingConfig = InitSettingFile();
        }

        public static SettingConfig InitSettingFile()
        {
            var savePath = Path.Combine(SETTING_FILE_PATH, "SettingConfig.asset");
            var config   = ResUtil.GetOrCreateAsset<SettingConfig>(savePath);
            ResUtil.AddAssetToGroup("SettingConfig", savePath, true, true);
            config.CurrentSettingFile = "Default";
            EditorUtility.SetDirty(config);
            GenerateNewSettingFile("Default");
            return config;
        }

        [Button("生成新配置文件")]
        public static void GenerateNewSettingFile([LabelText("文件名")] string fileName)
        {
            var targetPath = Path.Combine(SETTING_FILE_PATH, fileName + ".json");

            DirectoryUtil.MakeSureDirectory(SETTING_FILE_PATH);

            if (File.Exists(targetPath))
            {
                Debug.LogError("已有同名配置文件");
                return;
            }

            File.WriteAllText(targetPath, "{}");
            AssetDatabase.ImportAsset(targetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            RefreshSettingFile(ResUtil.GetAsset<SettingConfig>());
        }

        public static void RefreshSettingFile(SettingConfig config)
        {
            config.SettingFiles.Clear();
            config.SettingFiles.AddRange(ResUtil.GetAssetsInFolder<TextAsset>(SETTING_FILE_PATH));
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [Button("删除本地存档", ButtonSizes.Large)]
        public static void DeleteFile()
        {
            FileUtil.DeleteFileOrDirectory(Application.persistentDataPath);
        }
    }
}

#endif