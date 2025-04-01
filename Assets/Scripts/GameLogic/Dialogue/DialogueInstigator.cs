using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �Ի������ߣ�һ�����Player�ϣ����ܶԻ�����ͨ��DialogueSequencer��ʼ�Ի�
/// </summary>
public class DialogueInstigator : MonoBehaviour
{
    private DialogueSequencer m_DialogueSequencer;

    private void Awake()
    {
        m_DialogueSequencer = new DialogueSequencer();

        EventCenter.GetInstance().AddEventListener<DialogueSO>("�Ի�����", m_DialogueSequencer.StartDialogue);
        EventCenter.GetInstance().AddEventListener("�Ի��ڵ�����", m_DialogueSequencer.StartDialogueNode);
    }

    private void OnDestroy()
    {
        EventCenter.GetInstance().RemoveEventListener<DialogueSO>("�Ի�����", m_DialogueSequencer.StartDialogue);
        EventCenter.GetInstance().RemoveEventListener("�Ի��ڵ�����", m_DialogueSequencer.StartDialogueNode);

        m_DialogueSequencer = null;
    }
}
