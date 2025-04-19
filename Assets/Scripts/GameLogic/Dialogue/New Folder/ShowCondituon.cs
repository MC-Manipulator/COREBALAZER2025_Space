using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Condition/ShowCondituon")]
public class ShowCondituon : SerializedScriptableObject
{
    public Dictionary<ItemType,string> keyValuePairs = new Dictionary<ItemType,string>();
    public ItemType rightItem;

    public bool IsRightItem(ItemSO _item,ref string answer)
    {
        if (keyValuePairs.ContainsKey(_item.itemName))
        {
            answer = keyValuePairs[_item.itemName];
        }
        if(_item.itemName == rightItem)
        {
            return true;
        }
        return false;
    }
}
