using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �Ի��ڵ���ʽӿ�
/// </summary>
public interface DialogueNodeVisitor
{
    void Visit(BasicDialogueNode node);
    //void Visit(ChoiceDialogueNode node);
}
