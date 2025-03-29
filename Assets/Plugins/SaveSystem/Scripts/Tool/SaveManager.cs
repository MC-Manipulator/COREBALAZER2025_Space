using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class SaveManager : MonoBehaviour
{
    public GameObject confirmDialog;
    public GameObject saveBoard;

    public StorageState state;

    public GameObject saveSlotBoard;

    public List<SaveSlot> saveSlotList;

    public static SaveManager instance;
    public UnityEvent onSave;
    public UnityEvent onLoad;

    private void Awake()
    {
        if (instance == null || instance != this)
            instance = this;
        else
            Destroy(gameObject);

        if (saveSlotBoard != null)
        {
            for (int i = 0;i < saveSlotBoard.transform.childCount;i++)
            {
                saveSlotList.Add(saveSlotBoard.transform.GetChild(i).gameObject.GetComponent<SaveSlot>());
                saveSlotBoard.transform.GetChild(i).gameObject.GetComponent<SaveSlot>().slotNumber.text = "No." + (i + 1).ToString();
                saveSlotBoard.transform.GetChild(i).gameObject.GetComponent<ES3Slot>().mgr = gameObject.GetComponent<ES3SlotManager>();
            }
        }
        if (ES3.FileExists("SlotData.es3"))
        {
            LoadSlotsData();
        }
        CloseSaveBoard();
    }

    public void SaveSlotsData()
    {
        for (int i = 0;i < saveSlotList.Count;i++)
        {
            if (saveSlotList[i].slotState == SlotState.Full)
                ES3.Save(saveSlotList[i].slotNumber.text + "-" + DateTime.Now, i, "SlotData.es3");
        }
    }

    public void LoadSlotsData()
    {
        string[] slotList = ES3.GetKeys("SlotData.es3");
        foreach (string slotName in slotList)
        {
            if (ES3.FileExists(slotName + ".es3"))
            {
                saveSlotList[ES3.Load<int>("slotName")].SwitchToFullState();
                saveSlotList[ES3.Load<int>("slotName")].slotName.text = ES3.Load<string>("Day " + "day", slotName + ".es3");
            }
            else
            {
                saveSlotList[ES3.Load<int>("slotName")].SwitchToEmptyState();
                ES3.DeleteKey(slotName, "SlotData.es3");
            }
        }
    }

    public void UpdateSlot()
    {

    }

    public void StartLoad()
    {
        state = StorageState.Load;
        OpenSaveBoard();
    }

    public void StartSave()
    {
        state = StorageState.Save;
        OpenSaveBoard();
    }

    public void OpenSaveBoard()
    {
        saveBoard.SetActive(true);
    }

    public void CloseSaveBoard()
    {
        saveBoard.SetActive(false);
    }

    public void OnSelectSlot()
    {
        if (state == StorageState.Save)
        {
            Save();
            SaveSlotsData();
            LoadSlotsData();
        }
        else
        {
            Load();
        }
    }

    public void Save()
    {
        onSave?.Invoke();
    }

    public void Load()
    {
        onLoad?.Invoke();
    }
}
