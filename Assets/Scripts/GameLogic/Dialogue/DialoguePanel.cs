using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 对话UI面板
/// </summary>
public class DialoguePanel : MonoBehaviour, DialogueNodeVisitor
{
    public TextMeshProUGUI m_text;
    public TextMeshProUGUI nameOfCharacter;
    public Image imageOfCharacter;
    public float charDelay;
    private bool isPrinting = false;
    private Tweener textTween;

    private void Awake()
    {
        EventCenter.GetInstance().AddEventListener<DialogueNode>("对话节点开始", OnDialogueNodeStart);
        EventCenter.GetInstance().AddEventListener<DialogueSO>("对话结束", OnDialogueEnd);
    }

    private void OnDialogueNodeStart(DialogueNode dialogue)
    {
        dialogue.Accept(this);
    }

    public void Visit(BasicDialogueNode node)
    {
        m_text.text = "";
        isPrinting = true;
        textTween = m_text.DOText(node.m_text, charDelay * node.m_text.Length)
        .OnComplete(() => {
         isPrinting = false;
        });
        nameOfCharacter.text = node.speaker.characterName;
    }

    public void NextNode()
    {
        if (isPrinting)
        {
            textTween.Complete(this);
        }
        else
        {
            EventCenter.GetInstance().EventTrigger("对话节点请求");
        }
    }

    private void OnDialogueEnd(DialogueSO dialogue)
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        EventCenter.GetInstance().RemoveEventListener<DialogueNode>("对话节点开始", OnDialogueNodeStart);
        EventCenter.GetInstance().RemoveEventListener<DialogueSO>("对话结束", OnDialogueEnd);
    }
}
