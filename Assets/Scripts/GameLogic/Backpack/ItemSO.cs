using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Item/Item")]
public class ItemSO : SerializedScriptableObject
{
    public ItemType itemName;
    public Sprite image;
    public string itemDescription;
}

public enum ItemType
{
    ����,
    ����,
}