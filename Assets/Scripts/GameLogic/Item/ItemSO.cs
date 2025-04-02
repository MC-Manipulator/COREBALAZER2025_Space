using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public Sprite image;
    public ItemType itemName;
    public string itemDescription;

    public string GetName()
    {
        return itemName.ToString();
    }

    public string GetDescription()
    {
        return itemDescription;

    }
}

public enum ItemType
{
    Item1,
    Item2
}
