using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using Test;
using UnityEngine;

public class ItemMgr : BaseManager<ItemMgr>
{
    public ItemMgr()
    {
        ItemSO[] items = Resources.LoadAll<ItemSO>("ScriptableObject/Item");
        foreach (ItemSO item in items)
        {
            itemList.Add(item.itemName, item);
        }
    }

    public BackpackPanel backpackPanel;

    public Dictionary<ItemType,ItemSO> itemList = new Dictionary<ItemType, ItemSO> ();
    public Dictionary<ItemType, ItemSO> backpackList = new Dictionary<ItemType, ItemSO>();

    public delegate void OfferItemAction(ItemType id);
    public event OfferItemAction OnOfferItem;

    public void AddItemToBackpack(ItemType _itemName)
    {
        if (backpackList.ContainsKey(_itemName))
        {
            Debug.LogError("背包中已经有" +  _itemName + "道具，无法添加");
            return;
        }
        ItemSO item = itemList[_itemName];
        backpackList.Add(_itemName,item);
        backpackPanel.AddItemButton(item);
    }

    public void RemoveItemFromBackpack(ItemType _itemName)
    {
        if (!backpackList.ContainsKey(_itemName))
        {
            Debug.LogError("背包中没有" + _itemName + "道具，无法移除");
            return;
        }
        backpackPanel.RemoveItemButton(_itemName);
        backpackList.Remove(_itemName);
    }

    public ItemSO GetItemInBackpack(ItemType _itemName)
    {
        return backpackList[_itemName];
    }

    public void RequireItem()
    {
        backpackPanel.OnOfferItem += OfferItem;
        backpackPanel.OnCloseBackpack += CancelOffering;

        backpackPanel.SelectItemForOffering();
    }

    public void CancelOffering()
    {
        EndRequiring();
    }

    public void OfferItem(ItemType id)
    {
        OnOfferItem?.Invoke(id);

        EndRequiring();
    }

    public void EndRequiring()
    {
        backpackPanel.OnOfferItem -= OfferItem;
        backpackPanel.OnCloseBackpack -= CancelOffering;
    }
}