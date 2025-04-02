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

    public Item selectedItem;

    public delegate void UseItemAction(int id);
    public delegate void OfferItemAction(int id);

    public event UseItemAction OnUseItem;
    public event OfferItemAction OnOfferItem;
    public event Action OnCloseBackpack;

    public bool isOffering = false;

    private void Awake()
    {
        ItemSystem.Instance.backpackPanel = this;
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

    public void ShowItemDetail(Item item)
    {
        detailImage.sprite = item.image;
        detailName.text = item.itemName;
        detailDescription.text = item.itemDescription;
    }

    public void SelectItem(int id)
    {
        Item item = GetItemInBackpack(id);
        selectedItem = item;
        ShowItemDetail(item);
    }

    public void OfferItem()
    {
        OnOfferItem?.Invoke(selectedItem.id);
    }

    public void UseItem()
    {
        OnUseItem?.Invoke(selectedItem.id);
    }

    public void AddItemButton(Item item)
    {
        AddItemButton(item.id, item.itemName, item.itemDescription, item.image);
    }

    public void AddItemButton(int id, string name, string description, Sprite sprite)
    {
        Item item = new Item();

        item.id = id;
        item.itemName = name;
        item.itemDescription = description;
        item.image = sprite;

        GameObject itemButton = new GameObject();

        itemButton.transform.SetParent(content.transform);

        itemButton.AddComponent<Button>();
        itemButton.AddComponent<Image>();
        ItemButton buttonScript = itemButton.AddComponent<ItemButton>();

        itemButton.name = name;

        itemButton.GetComponent<Button>().targetGraphic = itemButton.GetComponent<Image>();
        itemButton.GetComponent<Button>().onClick.AddListener(buttonScript.SelectItem);
        itemButton.GetComponent<Image>().sprite = sprite;
        buttonScript.id = id;
    }

    public void RemoveItemButton(int id)
    {
        Item item = GetItemInBackpack(id);

        ItemButton[] buttonList = content.transform.GetComponentsInChildren<ItemButton>(true);
        foreach (ItemButton ib in buttonList)
        {
            if (ib.id == id)
            {
                Destroy(ib.gameObject);
                break;
            }
        }

        return;
    }

    public Item GetItemInBackpack(int id)
    {
        return ItemSystem.Instance.GetItemInBackpack(id);
    }
}
