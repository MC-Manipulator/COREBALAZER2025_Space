using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotePanel : MonoBehaviour
{
    public GameObject taskPanel;
    public GameObject informationPanel;
    public GameObject backpackPanel;
    private PanelOfNote currenDisplayPanel = PanelOfNote.Task;

    private void Awake()
    {
        taskPanel.gameObject.SetActive(true);
        informationPanel.gameObject.SetActive(false);
        backpackPanel.gameObject.SetActive(false);
    }
    public void SelectPanel(string selectPanel)
    {
        PanelOfNote result = (PanelOfNote)Enum.Parse(typeof(PanelOfNote), selectPanel);
        switch (result)
        {
            case PanelOfNote.Task:
                taskPanel.gameObject.SetActive(true);
                informationPanel.gameObject.SetActive(false);
                backpackPanel.gameObject.SetActive(false);
                currenDisplayPanel = PanelOfNote.Task;
                break;
            case PanelOfNote.Information:
                taskPanel.gameObject.SetActive(false);
                informationPanel.gameObject.SetActive(true);
                backpackPanel.gameObject.SetActive(false);
                currenDisplayPanel = PanelOfNote.Information;
                break;
            case PanelOfNote.BackPack:
                taskPanel.gameObject.SetActive(false);
                informationPanel.gameObject.SetActive(false);
                backpackPanel.gameObject.SetActive(true);
                currenDisplayPanel = PanelOfNote.BackPack;
                break;
        }
    }
}

public enum PanelOfNote
{
    Task,
    Information,
    BackPack,
}
