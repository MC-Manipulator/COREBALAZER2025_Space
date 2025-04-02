using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using Test;
using UnityEngine;

public class ItemSystem
{
    private static ItemSystem _instance;

    public static ItemSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ItemSystem();
            }
            return _instance;
        }
    }

    public BackpackPanel backpackPanel;

    public List<Item> itemList = new List<Item>();
    public List<Item> backpackList = new List<Item>();

    public delegate void OfferItemAction(int id);
    public event OfferItemAction OnOfferItem;

    public void AddItemToBackpack(int id)
    {
        Item item = itemList.Find(i => i.id == id);
        backpackList.Add(item);
        backpackPanel.AddItemButton(item);
    }

    public void RemoveItemFromBackpack(int id)
    {
        Item item = backpackList.Find(i => i.id == id);
        backpackList.Remove(item);
    }

    public Item GetItemInBackpack(int id)
    {
        return backpackList.Find(i => i.id == id);
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

    public void OfferItem(int id)
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