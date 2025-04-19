using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BackpackPanel : MonoBehaviour
{
    public GameObject backpackPanel;
    public GameObject content;
    public GameObject useButton;
    public GameObject offerButton;

    public Image detailImage;
    public TMP_Text detailName;
    public TMP_Text detailDescription;

    public ItemSO selectedItem;
    public ItemSO currentSelectedItem;
    public NarrationCharacter character;

    public delegate void UseItemAction(ItemType id);
    public delegate void OfferItemAction(ItemType id);

    public event UseItemAction OnUseItem;
    public event OfferItemAction OnOfferItem;
    public event Action OnCloseBackpack;

    public bool isOffering = false;

    private void Awake()
    {
        ItemMgr.GetInstance().backpackPanel = this;
    }

    public void SelectItemForOffering()
    {
        isOffering = true;
        backpackPanel.SetActive(true);
        useButton.SetActive(false);
        offerButton.SetActive(true);
    }

    public void OpenBackpack()
    {
        isOffering = false;
        backpackPanel.SetActive(true);
        useButton.SetActive(true);
        offerButton.SetActive(false);
    }

    public void CloseBackpack()
    {
        isOffering = false;
        backpackPanel.SetActive(false);
        OnCloseBackpack?.Invoke();
    }

    public void ClearDetail()
    {
        detailImage.sprite = null;
        detailName.text = "";
        detailDescription.text = "";
    }

    public void ShowItemDetail(ItemSO _item)
    {
        detailImage.sprite = _item.image;
        detailName.text = _item.itemName.ToString();
        detailDescription.text = _item.itemDescription;
    }

    public void SelectItem(ItemType _itemName)
    {
        ItemSO item = GetItemInBackpack(_itemName);
        currentSelectedItem = item;
        selectedItem = item;
        ShowItemDetail(item);

    }

    public void OfferItem()
    {
        OnOfferItem?.Invoke(selectedItem.itemName);
    }

    public void UseItem()
    {
        //OnUseItem?.Invoke(selectedItem.itemName);
        EventCenter.GetInstance().EventTrigger<ItemSO>("出示道具", currentSelectedItem);
        Destroy(gameObject);
    }

    public void AddItemButton(ItemSO _item)
    {
        ItemSO item = new ItemSO();

        item.itemName = _item.itemName;
        item.itemDescription = _item.itemDescription;
        item.image = _item.image;

        GameObject itemButton = new GameObject();

        itemButton.transform.SetParent(content.transform);

        itemButton.AddComponent<Button>();
        itemButton.AddComponent<Image>();
        ItemButton buttonScript = itemButton.AddComponent<ItemButton>();

        itemButton.name = name;

        itemButton.GetComponent<Button>().targetGraphic = itemButton.GetComponent<Image>();
        itemButton.GetComponent<Button>().onClick.AddListener(buttonScript.SelectItem);
        itemButton.GetComponent<Image>().sprite = _item.image;
        buttonScript.itemName = _item.itemName;
        buttonScript.OnItemButtonSelected += SelectItem;
    }

    public void RemoveItemButton(ItemType _itemName)
    {
        ItemSO item = ItemMgr.GetInstance().GetItemInBackpack(_itemName);
        ItemButton[] buttonList = content.transform.GetComponentsInChildren<ItemButton>(true);
        foreach (ItemButton ib in buttonList)
        {
            if (ib.itemName == item.itemName)
            {
                Destroy(ib.gameObject);
                break;
            }
        }
        if(currentSelectedItem != null)
        {
            if (_itemName == currentSelectedItem.itemName)
            {
                ClearDetail();
                currentSelectedItem = null;
            }
        }
    }

    public ItemSO GetItemInBackpack(ItemType _itemName)
    {
        return ItemMgr.GetInstance().GetItemInBackpack(_itemName);
    }

    public void TestAddItemToBackpack(int id)
    {
        ItemMgr.GetInstance().AddItemToBackpack((ItemType)id);
    }

    public void TestRemoveItemFromBackpack(int id)
    {
        ItemMgr.GetInstance().RemoveItemFromBackpack((ItemType)id);
    }
}
