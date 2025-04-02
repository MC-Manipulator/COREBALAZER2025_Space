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
    [Button("��ȡItemData")]
    [DisableInPlayMode] // ���� Edit ģʽ�¿���
    public void GatherScriptableObjects()
    {
        // ȷ��ֻ�ڱ༭����ִ��
        if (EditorApplication.isPlaying)
        {
            Debug.LogWarning("�˹���ֻ���� Edit ģʽ��ʹ�ã�");
            return;
        }

        Debug.Log($"��ʼ���ļ��� {targetFolderPath} ��ȡ ItemData...");

        string[] guids = AssetDatabase.FindAssets("ItemData", new[] { targetFolderPath });

        if (guids.Length == 0)
        {
            Debug.Log($"δ�ҵ�ItemData");
        }
        else
        {
            Debug.Log($"��ȡ�ɹ�");
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            _itemData = (ItemData)AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);


            Debug.Log($"��ʼ���ļ��� {targetFolderPath} ��ȡ ItemOS...");

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

            Debug.Log($"ScriptableObjects �ռ���ɣ����ҵ� {_itemData.itemList.Count} ������");
        }


        // ��Ƕ���Ϊ���Ա㱣��
        EditorUtility.SetDirty(this);
    }
#endif

}