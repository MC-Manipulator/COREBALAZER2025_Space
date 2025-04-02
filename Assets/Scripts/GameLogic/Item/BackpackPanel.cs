using System;
using System.Collections;
using System.Collections.Generic;
using Test;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BackpackPanel : MonoBehaviour
{
    public GameObject content;
    public GameObject useButton;
    public GameObject handButton;
    public Image bigImage;
    public TMP_Text detailName;
    public TMP_Text detailDescription;

    public ItemSO selectedItem;

    public delegate void UseItemAction(ItemType id);
    public delegate void HandItemAction(ItemType id);
    public event UseItemAction OnUseItem;
    public event HandItemAction OnHandItem;
    public event Action OnCloseBackpack;


    public bool isHanding = false;

    private void Awake()
    {
        EventCenter.GetInstance().AddEventListener<ItemSO>("添加道具", AddItem);
        EventCenter.GetInstance().AddEventListener<ItemType>("移除道具", RemoveItem);

    }
    public void SelectItemForHanding()
    {
        isHanding = true;
        //backpack.SetActive(true);
        useButton.SetActive(false);
        handButton.SetActive(true);
    }

    public void OpenBackpack()
    {
        isHanding = false;
        //backpack.SetActive(true);
        useButton.SetActive(true);
        handButton.SetActive(false);
    }

    public void CloseBackpack()
    {
        isHanding = false;
        //backpack.SetActive(false);
        OnCloseBackpack?.Invoke();
    }

    public void ShowItemDetail(ItemSO item)
    {
        bigImage.sprite = item.image;
        detailName.text = item.itemName.ToString();
        detailDescription.text = item.itemDescription;
    }

    public void SelectItem(ItemType name)
    {
        ItemSO item = BackpackMgr.GetInstance().GetItemInBackpack(name);
        selectedItem = item;
        ShowItemDetail(item);
    }

    public void HandItem()
    {
        OnHandItem?.Invoke(selectedItem.itemName);
    }

    public void UseItem(ItemType id)
    {
        OnUseItem?.Invoke(id);
    }

    public void AddItem(ItemSO item)
    {
        GameObject itemButton = new GameObject();

        itemButton.transform.SetParent(content.transform);

        itemButton.AddComponent<Button>();
        itemButton.AddComponent<Image>();
        ItemButton buttonScript = itemButton.AddComponent<ItemButton>();

        itemButton.name = name;

        itemButton.GetComponent<Button>().targetGraphic = itemButton.GetComponent<Image>();
        itemButton.GetComponent<Button>().onClick.AddListener(buttonScript.SelectItem);
        itemButton.GetComponent<Image>().sprite = item.image;
        buttonScript.id = item.itemName;
        buttonScript.OnItemSelected += SelectItem;
    }
    
    public void RemoveItem(ItemType id)
    {
        ItemButton[] buttonList = content.transform.GetComponentsInChildren<ItemButton>(true);
        foreach (ItemButton ib in buttonList)
        {
            if (ib.id == id)
            {
                Destroy(ib.gameObject);
                break;
            }
        }
    }

    private void OnDestroy()
    {
        EventCenter.GetInstance().RemoveEventListener<ItemSO>("添加道具", AddItem);
        EventCenter.GetInstance().RemoveEventListener<ItemType>("移除道具", RemoveItem);
    }
}
