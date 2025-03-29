using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

public enum StorageState
{
    Save,
    Load
}

public class AVGSaveSystem : MonoBehaviour
{
    public static AVGSaveSystem instance;

    public UnityEvent onSave;
    public UnityEvent onLoad;

    public List<SaveSlot> saveSlotList;
    public string slotDirectory = "save/";
    public string slotExtension = "";

    public SaveBoard saveBoard;
    public StorageState state;

    public static string selectedSlotPath = null;

    private void OnEnable()
    {
        if (instance == null || instance != this)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void InstantiateSlots()
    {
        saveSlotList = saveBoard.SaveSlotList;

        if (!ES3.DirectoryExists(slotDirectory))
            return;

        List<string> slotNameList = new List<string>();
        foreach (var file in ES3.GetFiles(slotDirectory))
        {
            var slotName = Path.GetFileNameWithoutExtension(file);
            slotNameList.Add(slotName);
        }

        foreach (var slotName in slotNameList)
            InstantiateSlot(slotName);
    }

    public string GetSlotPath(string slotName)
    {
        return slotDirectory + Regex.Replace(slotName, @"\s+", "_") + slotExtension;
    }

    public void InstantiateSlot(string slotName)
    {
        int slotNumber = ES3.Load<int>("slotNumber", GetSlotPath(slotName));
        SaveSlot slot = saveSlotList[slotNumber - 1];

        Texture2D texture = new Texture2D(Screen.width, Screen.height);
        texture.LoadImage(ES3.Load<byte[]>("slotPicture", GetSlotPath(slotName)));
        slot.SetPicture(texture);

        //slot.slotName.text = ES3.Load<int>("day", GetSlotPath(slotName)).ToString();
        slot.saveName = slotName;
        slot.SwitchToFullState();
    }

    public void UpdateSaveSlots()
    {
        InstantiateSlots();
    }
}
