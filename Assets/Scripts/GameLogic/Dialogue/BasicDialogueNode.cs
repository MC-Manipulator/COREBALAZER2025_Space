using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����ĶԻ��ڵ�
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Objects/Narration/Dialogue/Node/Basic")]
public class BasicDialogueNode : DialogueNode
{
    public Dictionary<string, Stat> dic;

    public BasicDialogueNode(string _text, NarrationCharacter _speaker,Sprite _sprite)
    {
        m_text = _text;
        speaker = _speaker;
        m_sprite = _sprite;
    }

    public override void Accept(DialogueNodeVisitor visitor)
    {
        visitor.Visit(this);
        if(dic != null)
        {
            if(dic.Count > 0)
            {
                foreach (KeyValuePair<string, Stat> pair in dic)
                {
                    StatMgr.GetInstance().ChangeStatValue(pair.Key, pair.Value);
                }
            }
        }
    }

    public override bool CanBeFollowedByNode(int _nextNodeIndex)
    {
        return nextNodeIndex == _nextNodeIndex;
    }
}
