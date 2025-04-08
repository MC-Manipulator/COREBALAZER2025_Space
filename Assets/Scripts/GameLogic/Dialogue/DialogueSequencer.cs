using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Diagnostics;

public class DialogueException : System.Exception
{
    public DialogueException(string message)
        : base(message)
    {
    }
}

/// <summary>
/// 对话序列，负责执行一整套对话逻辑
/// </summary>
public class DialogueSequencer
{
    public delegate void DialogueCallback(DialogueSO dialogue);
    public delegate void DialogueNodeCallback(DialogueNode node);

    public DialogueCallback OnDialogueStart;
    public DialogueCallback OnDialogueEnd;
    public DialogueNodeCallback OnDialogueNodeStart;
    public DialogueNodeCallback OnDialogueNodeEnd;

    private DialogueSO m_CurrentDialogue;
    private DialogueNode m_CurrentNode;

    public void StartDialogue(DialogueSO dialogue)
    {
        if (m_CurrentDialogue == null)
        {
            m_CurrentDialogue = dialogue;
            EventCenter.GetInstance().EventTrigger("对话开始", dialogue);
            StartDialogueNode();
        }
        else
        {
            throw new DialogueException("Can't start a dialogue when another is already running.");
        }
    }

    public void EndDialogue(DialogueSO dialogue)
    {
        if (m_CurrentDialogue == dialogue)
        {
            StopDialogueNode(m_CurrentNode);
            EventCenter.GetInstance().EventTrigger("对话结束", dialogue);
            m_CurrentDialogue = null;
        }
        else
        {
            throw new DialogueException("Trying to stop a dialogue that ins't running.");
        }
    }

    public void StartDialogueNode(int nextNodeIndex)
    {
        if (nextNodeIndex > m_CurrentDialogue.nodes.Count - 1 || !m_CurrentNode.CanBeFollowedByNode(nextNodeIndex))
        {
            EndDialogue(m_CurrentDialogue);
            return;
        }
        DialogueNode node = m_CurrentDialogue.nodes[nextNodeIndex];
        StopDialogueNode(m_CurrentNode);

        m_CurrentNode = node;
        EventCenter.GetInstance().EventTrigger("对话节点开始", m_CurrentNode);
    }

    public void StartDialogueNode()
    {
        if(m_CurrentNode!= null)
        {
            if (m_CurrentNode.nextNodeIndex > m_CurrentDialogue.nodes.Count - 1)
            {
                EndDialogue(m_CurrentDialogue);
                return;
            }
        }
        DialogueNode node;
        if (m_CurrentNode == null) node = m_CurrentDialogue.nodes[0];
        else node = m_CurrentDialogue.nodes[m_CurrentNode.nextNodeIndex];
        StopDialogueNode(m_CurrentNode);

        m_CurrentNode = node;
        EventCenter.GetInstance().EventTrigger("对话节点开始", m_CurrentNode);
    }

    private void StopDialogueNode(DialogueNode node)
    {
        if (m_CurrentNode == node)
        {
            m_CurrentNode = null;
        }
        else
        {
            throw new DialogueException("Trying to stop a dialogue node that ins't running.");
        }
    }
}
