using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Test
{
    public class BackpackSystem : SerializedMonoBehaviour
    {
        public static BackpackSystem instance;
        public GameObject backpack;
        public GameObject content;
        public GameObject useButton;
        public GameObject handButton;
        public Image bigImage;
        public TMP_Text detailName;
        public TMP_Text detailDescription;

        public ItemSO selectedItem;

        public Dictionary<ItemType,ItemSO> itemDic = new Dictionary<ItemType,ItemSO>();

        public delegate void UseItemAction(ItemType id);
        public delegate void HandItemAction(ItemType id);
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
            if (AVGSaveSystem.instance != null)
            {
                AVGSaveSystem.instance.onLoad.AddListener(OnLoad);
                AVGSaveSystem.instance.onSave.AddListener(OnSave);
            }
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

        public void ShowItemDetail(ItemSO item)
        {
            bigImage.sprite = item.image;
            detailName.text = item.itemName.ToString();
            detailDescription.text = item.itemDescription;
        }

        public void SelectItem(ItemType name)
        {
            ItemSO item = GetItemInBackpack(name);
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
            if (GetItemInBackpack(item.itemName) != null)
            {
                Debug.Log("Already has item under id:" + item.name);
                return;
            }

            itemDic.Add(item.itemName,item);

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
        }

        public void RemoveItem(ItemType id)
        {
            itemDic.Remove(id);

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

        public ItemSO GetItemInBackpack(ItemType name)
        {
            if(itemDic.ContainsKey(name)) return itemDic[name];

            return null;
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
            foreach (Transform child in content.transform)
            {
                Destroy(child.gameObject);
            }
            ItemType[] itemTypes = (ItemType[])System.Enum.GetValues(typeof(ItemType));
            this.itemDic = new Dictionary<ItemType, ItemSO>();

            int[] itemIDList = ES3.Load("Backpack", new int[0]);
            foreach (int i in itemIDList)
            {
                ItemSystem.Instance.AddItemToBackpack(itemTypes[i]);
            }
        }
    }


}