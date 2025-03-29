using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Test
{
    public class BackpackSystem : MonoBehaviour
    {
        public static BackpackSystem instance;
        public GameObject backpack;
        public GameObject content;
        public GameObject useButton;
        public GameObject  handButton;
        public Image bigImage;
        public TMP_Text detailName;
        public TMP_Text detailDescription;

        public Item selectedItem;

        public List<Item> itemList = new List<Item>();

        public delegate void UseItemAction(int id);
        public delegate void HandItemAction(int id);
        public event UseItemAction OnUseItem;
        public event HandItemAction OnHandItem;
        public event Action OnCloseBackpack;

        public bool isHanding = false;


        private void Awake()
        {
            if (BackpackSystem.instance == null)
            {
                instance = this;
            }
            else if (BackpackSystem.instance != null && BackpackSystem.instance != this)
            {
                Destroy(gameObject);
            }

        }
        private void Start()
        {

            AVGSaveSystem.instance.onLoad.AddListener(OnLoad);
            AVGSaveSystem.instance.onSave.AddListener(OnSave);
        }

        public void SelectItemForHanding()
        {
            isHanding = true;
            backpack.SetActive(true);
            useButton.SetActive(false);
            handButton.SetActive(true);
        }

        public void OpenBackpack()
        {
            isHanding = false;
            backpack.SetActive(true);
            useButton.SetActive(true);
            handButton.SetActive(false);
        }

        public void CloseBackpack()
        {
            isHanding = false;
            backpack.SetActive(false);
            OnCloseBackpack?.Invoke();
        }

        public void ShowItemDetail(Item item)
        {
            bigImage.sprite = item.image;
            detailName.text = item.itemName;
            detailDescription.text = item.itemDescription;
        }

        public void SelectItem(int id)
        {
            Item item = GetItemInBackpack(id);
            selectedItem = item;
            ShowItemDetail(item);
        }

        public void HandItem()
        {
            OnHandItem?.Invoke(selectedItem.id);
        }

        public void UseItem(int id)
        {
            OnUseItem?.Invoke(id);
        }

        public void AddItem(Item item)
        {
            AddItem(item.id, item.itemName, item.itemDescription, item.image);
        }

        public void AddItem(int id, string name, string description, Sprite sprite)
        {
            Item item = new Item();

            if (GetItemInBackpack(id) != null)
            {
                Debug.Log("Already has item under id:" + id);
                return;
            }

            itemList.Add(item);

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

        public void RemoveItem(int id)
        {
            Item item = GetItemInBackpack(id);
            itemList.Remove(item);

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
            foreach (Item item in itemList)
            {
                if (item.id == id)
                {
                    return item;
                }
            }

            return null;
        }

        public void OnSave()
        {
            int[] itemIDList = new int[itemList.Count];
            int it = 0;
            foreach (Item i in itemList)
            {
                itemIDList[it++] = i.id;
            }
            ES3.Save("Backpack", itemIDList);
        }

        public void OnLoad()
        {
            foreach (Transform child in content.transform)
            {
                Destroy(child.gameObject);
            }

            this.itemList = new List<Item>();

            int[] itemIDList = ES3.Load("Backpack", new int[0]);
            foreach (int i in itemIDList)
            {
                ItemSystem.Instance.AddItemToBackpack(i);
            }
        }
    }


}