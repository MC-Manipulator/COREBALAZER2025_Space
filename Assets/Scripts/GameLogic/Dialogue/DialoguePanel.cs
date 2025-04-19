using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �Ի�UI���
/// </summary>
public class DialoguePanel : MonoBehaviour, DialogueNodeVisitor
{
    public TextMeshProUGUI m_text; //��ʾ���ı�����
    public TextMeshProUGUI nameOfCharacter;  //��ʾ�Ľ�ɫ����
    public Image illustration;  //��ʾ�Ľ�ɫ����
    public float charDelay;  //�ı���ʾ�ַ����
    private bool isPrinting = false;  //�Ƿ��ڴ�ӡ�ı�
    private Tweener textTween;
    public Transform m_ChoicesBoxTransform;  //�Ի�ѡ��
    private bool isPause = false;
    private ShowCondituon showCondituon;  //��ǰ�ĳ�ʾ��������
    private delegate void OnDialogueNodeEnd();
    private event OnDialogueNodeEnd DoOnEnd;

    private void Awake()
    {
        EventCenter.GetInstance().AddEventListener<DialogueNode>("�Ի��ڵ㿪ʼ", OnDialogueNodeStart);
        EventCenter.GetInstance().AddEventListener<DialogueSO>("�Ի�����", OnDialogueEnd);
        EventCenter.GetInstance().AddEventListener<ItemSO>("��ʾ����", CheckItem);
    }
    public void CheckItem(ItemSO _item)
    {
        string _text = "";
        if (showCondituon.IsRightItem(_item, ref _text))
        {
            isPause = false;
        }
        else
        {
            DoOnEnd = () =>
            {
                EventCenter.GetInstance().EventTrigger("��������");
            };
        }
        m_text.text = "";
        isPrinting = true;
        textTween = m_text.DOText(_text, charDelay * _text.Length)
        .OnComplete(() => {
            isPrinting = false;
        });
    }
    private void OnDialogueNodeStart(DialogueNode dialogue)
    {
        dialogue.Accept(this);
    }

    public void Visit(BasicDialogueNode node)
    {
        m_text.text = "";
        isPrinting = true;
        illustration.sprite = node.m_sprite;
        textTween = m_text.DOText(node.m_text, charDelay * node.m_text.Length)
        .OnComplete(() => {
         isPrinting = false;
        });
        nameOfCharacter.text = node.speaker.characterName;

        if(node.showCondituon != null)
        {
            showCondituon = node.showCondituon;
            isPause = true;

            DoOnEnd = () =>
            {
                EventCenter.GetInstance().EventTrigger("��������");
            };
        }
    }

    public void Visit(ChoiceDialogueNode node)
    {
        m_ChoicesBoxTransform.gameObject.SetActive(true);

        foreach (Transform child in m_ChoicesBoxTransform)
        {
            Destroy(child.gameObject);
        }
        foreach (DialogueChoice choice in node.choices)
        {
            if (!choice.IsMetConditions()) continue;
            ResMgr.GetInstance().LoadAsync<GameObject>("UI/Option", (obj) =>
            {
                UIDialogueChoiceController newChoice = obj.GetComponent<UIDialogueChoiceController>();
                newChoice.Initialization(choice.nextNodeIndex, choice.choicePreview, choice.statValuePairs);
                obj.transform.SetParent(m_ChoicesBoxTransform);
            });
        }
        //    DoChoice = () =>
        //{
            
        //    }
        //    DoChoice = null;
        //};
    }

    public void Visit(BranchDialogueNode node)
    {
        for (int i = 0; i < node.branchs.Length; i++)
        {
            if (i < node.branchs.Length - 1)
            {
                if (!node.branchs[i].IsMetConditions()) continue;
            }
            EventCenter.GetInstance().EventTrigger<int>("�Ի��ڵ��������", node.branchs[i].nextNodeIndex);
            if (node.branchs[i].statValuePairs != null)
            {
                if (node.branchs[i].statValuePairs.Length > 0)
                {
                    foreach (StatValuePair statValuePair in node.branchs[i].statValuePairs)
                    {
                        statValuePair.ApplyValueToStat();
                    }
                }
            }
            Debug.Log(i + "��֧������");
            break;
        }
        //DoChoice = () =>
        //{

        //    DoChoice = null;
        //};
    }

    public void NextNode()
    {
        if (isPrinting)
        {
            textTween.Complete(this);
        }
        else
        {
            if (!isPause && DoOnEnd == null)
            {
                EventCenter.GetInstance().EventTrigger("�Ի��ڵ�����");
            }
            else
            {
                DoOnEnd?.Invoke();
                DoOnEnd = null;
            }
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
