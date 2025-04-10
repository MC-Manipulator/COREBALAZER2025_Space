using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 基本的对话节点
/// </summary>
[CreateAssetMenu(menuName = "Dialogue/Node/Basic")]
public class BasicDialogueNode : DialogueNode
{
    //public Dictionary<string, Stat> dic;
    public StatValuePair[] statValuePairs;

    public BasicDialogueNode(string _text, NarrationCharacter _speaker,Sprite _sprite)
    {
        m_text = _text;
        speaker = _speaker;
        m_sprite = _sprite;
    }

    public override void Accept(DialogueNodeVisitor visitor)
    {
        visitor.Visit(this);
        if (statValuePairs != null)
        {
            if (statValuePairs.Length > 0)
            {
                foreach (StatValuePair pair in statValuePairs)
                {
                    pair.ApplyValueToStat();
                }
            }
        }
    }

    public override bool CanBeFollowedByNode(int _nextNodeIndex)
    {
        return nextNodeIndex == _nextNodeIndex;
    }
}
