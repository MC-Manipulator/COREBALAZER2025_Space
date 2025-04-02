using System.Collections;
using System.Collections.Generic;
using Test;
using UnityEngine;

public class BackpackMgr : BaseManager<BackpackMgr>
{
    public Dictionary<ItemType, ItemSO> itemDic = new Dictionary<ItemType, ItemSO>();
    public ItemSO GetItemInBackpack(ItemType name)
    {
        if (itemDic.ContainsKey(name)) return itemDic[name];

        return null;
    }
    public void AddItem(ItemSO item)
    {
        if (GetItemInBackpack(item.itemName) != null)
        {
            Debug.Log("Already has item under id:" + item.name);
            return;
        }

        itemDic.Add(item.itemName, item);
        EventCenter.GetInstance().EventTrigger("添加道具", item);
    }

    public void RemoveItem(ItemType id)
    {
        itemDic.Remove(id);
        EventCenter.GetInstance().EventTrigger("移除道具", id);
    }

    public void OnSave()
    {
        int[] itemIDList = new int[itemDic.Count];
        int it = 0;
        foreach (ItemSO i in itemDic.Values)
        {
            itemIDList[it++] = (int)i.itemName;
        }
        ES3.Save("Backpack", itemIDList);
    }

    public void OnLoad()
    {
        //foreach (Transform child in content.transform)
        //{
        //    Destroy(child.gameObject);
        //}
        ItemType[] itemTypes = (ItemType[])System.Enum.GetValues(typeof(ItemType));
        this.itemDic = new Dictionary<ItemType, ItemSO>();

        int[] itemIDList = ES3.Load("Backpack", new int[0]);
        foreach (int i in itemIDList)
        {
            ItemSystem.Instance.AddItemToBackpack(itemTypes[i]);
        }
    }
}
