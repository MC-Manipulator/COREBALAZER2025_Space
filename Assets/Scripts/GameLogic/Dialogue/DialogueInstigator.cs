using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对话发起者，一般挂在Player上，接受对话请求并通过DialogueSequencer开始对话
/// </summary>
public class DialogueInstigator : MonoBehaviour
{
    private DialogueSequencer m_DialogueSequencer;

    private void Awake()
    {
        m_DialogueSequencer = new DialogueSequencer();

        EventCenter.GetInstance().AddEventListener<DialogueSO>("对话请求", m_DialogueSequencer.StartDialogue);
        EventCenter.GetInstance().AddEventListener("对话节点请求", m_DialogueSequencer.StartDialogueNode);
        EventCenter.GetInstance().AddEventListener<int>("对话节点请求带参", m_DialogueSequencer.StartDialogueNode);
    }

    private void OnDestroy()
    {
        EventCenter.GetInstance().RemoveEventListener<DialogueSO>("对话请求", m_DialogueSequencer.StartDialogue);
        EventCenter.GetInstance().RemoveEventListener("对话节点请求", m_DialogueSequencer.StartDialogueNode);
        EventCenter.GetInstance().RemoveEventListener<int>("对话节点请求带参", m_DialogueSequencer.StartDialogueNode);
        m_DialogueSequencer = null;
    }
}
