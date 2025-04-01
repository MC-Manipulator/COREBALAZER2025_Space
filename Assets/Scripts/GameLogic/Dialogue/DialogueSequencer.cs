
public class DialogueException : System.Exception
{
    public DialogueException(string message)
        : base(message)
    {
    }
}

/// <summary>
/// �Ի����У�����ִ��һ���׶Ի��߼�
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
    private int index = 0;

    //1
    public void StartDialogue(DialogueSO dialogue)
    {
        if (m_CurrentDialogue == null)
        {
            m_CurrentDialogue = dialogue;
            EventCenter.GetInstance().EventTrigger("�Ի���ʼ", dialogue);
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
            index = 0;
            EventCenter.GetInstance().EventTrigger("�Ի�����", dialogue);
            m_CurrentDialogue = null;
        }
        else
        {
            throw new DialogueException("Trying to stop a dialogue that ins't running.");
        }
    }

    //2
    public void StartDialogueNode()
    {
        if (index > m_CurrentDialogue.nodes.Count - 1)
        {
            EndDialogue(m_CurrentDialogue);
            return;
        }
        DialogueNode node = m_CurrentDialogue.nodes[index];
        index++;
        StopDialogueNode(m_CurrentNode);

        m_CurrentNode = node;
        EventCenter.GetInstance().EventTrigger("�Ի��ڵ㿪ʼ", m_CurrentNode);
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
