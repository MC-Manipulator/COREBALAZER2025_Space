#region

//文件创建者：Egg
//创建时间：10-23 11:28

#endregion

using EggFramework.Storage;
using Newtonsoft.Json;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace EggFramework.Util
{
    public static class StorageUtil
    {
#if UNITY_EDITOR

        private static SettingConfig _settingConfigInst;

        private static SettingConfig _settingConfig
        {
            get
            {
                if (_settingConfigInst != null) return _settingConfigInst;
                _settingConfigInst = ResUtil.GetAsset<SettingConfig>();
                if (_settingConfigInst != null) return _settingConfigInst;
                _settingConfigInst = SettingEditorWindow.InitSettingFile();
                
                return _settingConfigInst;
            }
        }

#endif
        private static JsonStorage _jsonStorage;
        private static IStorage    _storage => _jsonStorage ??= new JsonStorage();

        public static T LoadByJson<T>(string key, T defaultValue, string fileName)
        {
            return _storage.LoadByJson(key, defaultValue, fileName);
        }

        public static T LoadByJson<T>(string key, T defaultValue)
        {
            return _storage.LoadByJson(key, defaultValue);
        }
#if UNITY_EDITOR

        public static void AssignSettingFile(string fileName)
        {
            if (_settingConfig.SettingFiles.Exists(file => file.name == fileName))
            {
                _settingConfig.CurrentSettingFile = fileName;
            }

            EditorUtility.SetDirty(_settingConfig);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void SaveToSettingFile<T>(string key, T value)
        {
            var fileName  = _settingConfig.CurrentSettingFile;
            var textAsset = _settingConfig.SettingFiles.Find(file => file.name == fileName);
            var path      = AssetDatabase.GetAssetPath(textAsset);
            _storage.SaveToSettingFile(key, value, path.Replace(".json", ""));
            EditorUtility.SetDirty(textAsset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
#endif
        public static T LoadFromSettingFile<T>(string key, T defaultValue)
        {
#if UNITY_EDITOR
            var fileName  = _settingConfig.CurrentSettingFile;
            var textAsset = _settingConfig.SettingFiles.Find(file => file.name == fileName);
#else
            var fileName  = ResUtil.SettingConfig.CurrentSettingFile;
            var textAsset = ResUtil.SettingConfig.SettingFiles.Find(file => file.name == fileName);
#endif
            return _storage.LoadFromSettingFile(key, defaultValue, textAsset);
        }

        public static void SaveByJson<T>(string key, T value, string fileName)
        {
            _storage.SaveByJson(key, value, fileName);
        }

        public static void SaveByJson<T>(string key, T value)
        {
            _storage.SaveByJson(key, value);
        }

        public static T LoadByPlayerPref<T>(string key, T defaultValue)
        {
            var json = PlayerPrefs.GetString(key, JsonConvert.SerializeObject(defaultValue));
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static void SaveByPlayerPref<T>(string key, T value)
        {
            var json = JsonConvert.SerializeObject(value);
            PlayerPrefs.SetString(key, json);
        }
    }
}