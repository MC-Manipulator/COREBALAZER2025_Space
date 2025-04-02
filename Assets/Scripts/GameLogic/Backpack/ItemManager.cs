using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    private static string targetFolderPath = "Assets/Resources/ScriptableObject/Item";

    [SerializeField]
    private ItemData _itemData;

    private ItemSystem _itemSystem;

    private void Awake()
    {
        _itemSystem = ItemSystem.Instance;

        if (_itemData != null)
        {
            foreach (ItemSO i in _itemData.itemList)
            {
                Item item = new Item();
                item.id = i.id;
                item.image = i.image;
                item.itemName = i.itemName;
                item.itemDescription = i.itemDescription;
                _itemSystem.itemList.Add(item);
            }
        }
    }

#if UNITY_EDITOR
    [Button("读取ItemData")]
    [DisableInPlayMode] // 仅在 Edit 模式下可用
    public void GatherScriptableObjects()
    {
        // 确保只在编辑器下执行
        if (EditorApplication.isPlaying)
        {
            Debug.LogWarning("此功能只能在 Edit 模式下使用！");
            return;
        }

        Debug.Log($"开始从文件夹 {targetFolderPath} 读取 ItemData...");

        string[] guids = AssetDatabase.FindAssets("ItemData", new[] { targetFolderPath });

        if (guids.Length == 0)
        {
            Debug.Log($"未找到ItemData");
        }
        else
        {
            Debug.Log($"读取成功");
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            _itemData = (ItemData)AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);


            Debug.Log($"开始从文件夹 {targetFolderPath} 读取 ItemOS...");

            string[] guids_ = AssetDatabase.FindAssets("t:ScriptableObject", new[] { targetFolderPath });

            foreach (string guid in guids_)
            {
                string itemSOPath = AssetDatabase.GUIDToAssetPath(guid);
                ScriptableObject obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(itemSOPath);

                
                if (obj is ItemSO)
                {
                    Debug.Log(obj.name);
                    _itemData.itemList.Add((ItemSO)obj);
                }
            }

            Debug.Log($"ScriptableObjects 收集完成，共找到 {_itemData.itemList.Count} 个对象");
        }


        // 标记对象为脏以便保存
        EditorUtility.SetDirty(this);
    }
#endif

}