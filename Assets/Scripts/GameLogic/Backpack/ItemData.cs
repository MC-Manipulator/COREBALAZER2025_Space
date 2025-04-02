using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Item/ItemData")]
public class ItemData : SerializedScriptableObject
{
    public List<ItemSO> itemList;
}