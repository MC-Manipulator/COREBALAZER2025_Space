using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public GameObject taskPanel;
    private bool isDialoging = false;
    private void Awake()
    {
        EventCenter.GetInstance().AddEventListener<DialogueSO>("对话开始", OnDialogueStart);
        EventCenter.GetInstance().AddEventListener<DialogueSO>("对话结束", OnDialogueEnd);
        EventCenter.GetInstance().AddEventListener("开启背包", OpenBackpackPanel);
    }

    private void OnDialogueStart(DialogueSO dialogue) 
    {
        if(isDialoging) return;
        GameObject dialoguePanel = ResMgr.GetInstance().Load<GameObject>("UI/DialoguePanel");
        dialoguePanel.transform.SetParent(transform, false);
        isDialoging = true;
    }

    public void OpenBackpackPanel()
    {
        GameObject backpackPanel = ResMgr.GetInstance().Load<GameObject>("UI/BackpackPanel");
        backpackPanel.transform.SetParent(transform, false);
    }

    private void OnDialogueEnd(DialogueSO dialogue)
    {
        isDialoging = false;
    }
    private void OnDestroy()
    {
        EventCenter.GetInstance().RemoveEventListener<DialogueSO>("对话开始", OnDialogueStart);
        EventCenter.GetInstance().RemoveEventListener<DialogueSO>("对话结束", OnDialogueEnd);
        EventCenter.GetInstance().RemoveEventListener("开启背包", OpenBackpackPanel);
    }

    public void StartTask(string taskName)
    {
        TaskManager.GetInstance().StartTask(taskName);
    }

    public void OpenPanel()
    {
        taskPanel.SetActive(true);
    }
}
