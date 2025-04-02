using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using Test;
using UnityEngine;

public class ItemSystem : MonoBehaviour
{
    public static ItemSystem Instance { get; private set; }
    public List<ItemSO> itemList = new List<ItemSO>();

    public ItemType requiredItemID;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void AddItemToBackpack(ItemType id)
    {
        ItemSO item = itemList.Find(i => i.itemName == id);
        BackpackSystem.instance.AddItem(item);
    }

    public void RequireItem(ItemType id)
    {
        requiredItemID = id;
        BackpackSystem.instance.SelectItemForHanding();
        BackpackSystem.instance.OnHandItem += OfferItem;
        BackpackSystem.instance.OnCloseBackpack += Cancel;
        DialogueLua.SetVariable("FinishSelected", false);
    }

    public void Cancel()
    {
        DialogueLua.SetVariable("SelectItem", false);
        EndRequiring();
    }

    public void OfferItem(ItemType id)
    {
        DialogueLua.SetVariable("SelectItem", true);
        if (id == requiredItemID)
        {
            DialogueLua.SetVariable("SelectCorrectOrWrong", true);
        }
        else
        {
            DialogueLua.SetVariable("SelectCorrectOrWrong", false);
        }
        EndRequiring();
        BackpackSystem.instance.CloseBackpack();
    }

    public void EndRequiring()
    {
        BackpackSystem.instance.OnHandItem -= OfferItem;
        BackpackSystem.instance.OnCloseBackpack -= Cancel;
        DialogueLua.SetVariable("FinishSelected", true);
    }


}
