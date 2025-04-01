using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �Ի�UI���
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
        EventCenter.GetInstance().AddEventListener<DialogueNode>("�Ի��ڵ㿪ʼ", OnDialogueNodeStart);
        EventCenter.GetInstance().AddEventListener<DialogueSO>("�Ի�����", OnDialogueEnd);
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
            EventCenter.GetInstance().EventTrigger("�Ի��ڵ�����");
        }
    }

    private void OnDialogueEnd(DialogueSO dialogue)
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        EventCenter.GetInstance().RemoveEventListener<DialogueNode>("�Ի��ڵ㿪ʼ", OnDialogueNodeStart);
        EventCenter.GetInstance().RemoveEventListener<DialogueSO>("�Ի�����", OnDialogueEnd);
    }
}
