#region

//文件创建者：Egg
//创建时间：11-10 01:03

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using EggFramework.Util.Localization;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EggFramework.Util
{
    public static class LocalizationUtil
    {
        private static          bool                                 _inited;
        private static          SystemLanguage                       _currentSystemLanguage;
        private static          LocalizationConfig                   _config;
        private static readonly Dictionary<string, LocalizationData> _localizationDatas = new();

        public static async UniTask Init()
        {
            if (_inited) return;
            _currentSystemLanguage = StorageUtil.LoadByJson("GameLanguage", SystemLanguage.ChineseSimplified);
            _config = StorageUtil.LoadFromSettingFile(nameof(LocalizationConfig), new LocalizationConfig());
            var tasks = Enumerable.Select(_config.LocalizationTables, LoadTable).ToList();
            await UniTask.WhenAll(tasks);
            _inited = true;
        }

        public static void ChangeLanguage(SystemLanguage language)
        {
            _currentSystemLanguage = language;
        }

        private static async UniTask LoadTable(string tableName)
        {
            _localizationDatas.TryAdd(tableName,
                await Addressables.LoadAssetAsync<LocalizationData>(tableName + "_LocalizationData"));
        }

        public static string GetLocalizationString(string tableName, string colName, string key)
        {
            return null;
        }

        public static string GetLocalizationString(string key)
        {
            var firstIndex  = key.IndexOf("_", StringComparison.Ordinal);
            var secondIndex = key.IndexOf("_", firstIndex + 1, StringComparison.Ordinal);
            var table       = key[..firstIndex];
            var colName     = key[(firstIndex + 1)..secondIndex];

            if (_localizationDatas.TryGetValue(table, out var data))
            {
                var targetPackage = data.Packages.Find(pa => pa.Language == _currentSystemLanguage);
                if (targetPackage == null)
                {
                    Debug.LogError($"不支持该语言{_currentSystemLanguage}");
                    return null;
                }

                var col = targetPackage.LocalizationCols.Find(col => col.ColName == colName);
                if (col == null)
                {
                    Debug.LogError($"没有找到对应字段{colName}");
                    return null;
                }

                return col.LocalizationItems.Find(item => item.EntryKey == key).Value;
            }

            Debug.LogError($"该表{table}没有创建本地化数据");

            return null;
        }
    }
}