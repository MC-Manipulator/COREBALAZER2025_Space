using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    private bool isDialoging = false;
    private void Awake()
    {
        EventCenter.GetInstance().AddEventListener<DialogueSO>("对话开始", OnDialogueStart);
        EventCenter.GetInstance().AddEventListener<DialogueSO>("对话结束", OnDialogueEnd);
    }

    private void OnDialogueStart(DialogueSO dialogue) 
    {
        if(isDialoging) return;
        GameObject dialoguePanel = ResMgr.GetInstance().Load<GameObject>("UI/DialoguePanel");
        dialoguePanel.transform.SetParent(transform, false);
        isDialoging = true;
    }

    private void OnDialogueEnd(DialogueSO dialogue)
    {
        isDialoging = false;
    }
    private void OnDestroy()
    {
        EventCenter.GetInstance().RemoveEventListener<DialogueSO>("对话开始", OnDialogueStart);
        EventCenter.GetInstance().RemoveEventListener<DialogueSO>("对话结束", OnDialogueEnd);
    }
}
