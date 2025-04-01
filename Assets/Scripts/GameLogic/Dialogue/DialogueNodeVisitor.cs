using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对话节点访问接口
/// </summary>
public interface DialogueNodeVisitor
{
    void Visit(BasicDialogueNode node);
    //void Visit(ChoiceDialogueNode node);
}
