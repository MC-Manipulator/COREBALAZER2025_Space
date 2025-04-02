using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Item/Item")]
public class ItemSO : SerializedScriptableObject
{
    public int id;
    public Sprite image;
    public string itemName;
    public string itemDescription;
}
