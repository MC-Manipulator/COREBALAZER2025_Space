using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveBoard : MonoBehaviour
{
    public GameObject saveBoard;
    public Button SaveButton;
    public Button LoadButton;
    public Button CloseButton;
    public GameObject Dialog;
    public List<SaveSlot> SaveSlotList;
    public List<PageButton> PageButtonList;
    public GameObject SaveSlotSet;
    public GameObject PageButtonSet;

    public int slotCountInaPage = 4;

    public Camera gameScreen;
    public Rect screenCaptureArea;

    private void OnEnable()
    {
        ScreenCaptureSystem.gameScreen = gameScreen;
        ScreenCaptureSystem.screenCaptureArea = screenCaptureArea;
        CloseButton.onClick.AddListener(Close);

        for (int i = 0; i < PageButtonSet.transform.childCount; i++)
        {
            PageButtonList.Add(PageButtonSet.transform.GetChild(i).gameObject.GetComponent<PageButton>());
            PageButtonList[i].pageNumber = i + 1;
            PageButtonList[i].sb = this;
            PageButtonList[i].sr = PageButtonList[i].GetComponent<SpriteRenderer>();
        }

        for (int i = 0; i < SaveSlotSet.transform.childCount; i++)
        {
            SaveSlotList.Add(SaveSlotSet.transform.GetChild(i).gameObject.GetComponent<SaveSlot>());
            SaveSlotList[i].slotNumber.text = "No." + (i + 1).ToString();
            SaveSlotList[i].saveNumber = i + 1;
            SaveSlotList[i].dialog = Dialog;
            SaveSlotList[i].SwitchToEmptyState();
        }
    }

    private void OnDisable()
    {

        CloseButton.onClick.RemoveAllListeners();
    }

    public void StartToSave()
    {
        ChangePage(1);
        saveBoard.SetActive(true);
        AVGSaveSystem.instance.state = StorageState.Save;
        AVGSaveSystem.instance.InstantiateSlots();

    }

    public void StartToLoad()
    {
        ChangePage(1);
        saveBoard.SetActive(true);
        AVGSaveSystem.instance.state = StorageState.Load;
        AVGSaveSystem.instance.InstantiateSlots();
    }

    public void Close()
    {
        saveBoard.SetActive(false);
    }

    public void ChangePage(int pagenumber)
    {
        foreach (var item in SaveSlotList)
        {
            item.gameObject.SetActive(false);
        }

        for (int i = ((pagenumber - 1) * slotCountInaPage);i <  pagenumber * slotCountInaPage;i++)
        {
            SaveSlotList[i].gameObject.SetActive(true);
        }
    }
}
