using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����ĶԻ��ڵ�
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Objects/Narration/Dialogue/Node/Basic")]
public class BasicDialogueNode : DialogueNode
{
    public BasicDialogueNode(string _text, NarrationCharacter _speaker)
    {
        m_text = _text;
        speaker = _speaker;
    }

    public override void Accept(DialogueNodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
