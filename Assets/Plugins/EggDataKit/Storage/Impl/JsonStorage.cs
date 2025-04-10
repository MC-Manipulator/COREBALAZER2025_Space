using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace EggFramework.Storage
{
    public sealed class JsonStorage : IStorage
    {
        private string _savePath = Path.Combine(Application.persistentDataPath, "saveData.json");

        private Dictionary<string, object> _saveData = new();

        public void ReadFromDisk()
        {
            if (File.Exists(_savePath))
            {
                var json = File.ReadAllText(_savePath);

                _saveData = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            }
            else
            {
                _saveData.Clear();
            }
        }

        public void ReadFromTextAsset(string textAsset)
        {
            _saveData = JsonConvert.DeserializeObject<Dictionary<string, object>>(textAsset);
        }

        public void WriteToDisk()
        {
            var json = JsonConvert.SerializeObject(_saveData);

            File.WriteAllText(_savePath, json);
        }

        public void Save<T>(string key, T saveData)
        {
            if (saveData == null)
            {
                Debug.LogWarning("Saving null data");
                return;
            }

            if (!_saveData.TryAdd(key, saveData))
            {
                _saveData[key] = saveData;
            }
        }

        public T Load<T>(string key, T defaultValue, out bool newValue)
        {
            newValue = false;

            if (!_saveData.TryGetValue(key, out var saveValue))
            {
                newValue = true;
                return defaultValue;
            }

            if (typeof(T) == typeof(bool) || typeof(T) == typeof(string))
            {
                return (T)saveValue;
            }

            if (typeof(IList).IsAssignableFrom(typeof(T)))
            {
                if (saveValue.ToString() == "[]")
                {
                    newValue = true;
                    return defaultValue;
                }

                if (saveValue is not JArray && saveValue is IList)
                {
                    return (T)saveValue;
                }
            }

            return JsonConvert.DeserializeObject<T>(saveValue.ToString());
        }
#if UNITY_EDITOR

        public void AssignEditorSavePath(string path)
        {
            _savePath = path + ".json";
        }
#endif

        public void AssignSavePath(string path = "saveData")
        {
            _savePath = Path.Combine(Application.persistentDataPath, path + ".json");
        }

        public void Clear()
        {
            _saveData.Clear();
        }
    }
}