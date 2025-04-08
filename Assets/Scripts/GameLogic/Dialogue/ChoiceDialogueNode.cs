using PixelCrushers.DialogueSystem;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 选项对话节点，还未具体实现
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Objects/Narration/Dialogue/Node/Choice")]
public class ChoiceDialogueNode : DialogueNode
{
    [ShowInInspector] public DialogueChoice[] choices;
    public ChoiceDialogueNode(string _text, NarrationCharacter _speaker, Sprite _sprite)
    {
        m_text = _text;
        speaker = _speaker;
        m_sprite = _sprite;
    }

    public override void Accept(DialogueNodeVisitor visitor)
    {
        visitor.Visit(this);
    }

    public override bool CanBeFollowedByNode(int _nextNodeIndex)
    {
        return choices.Any(x => x.nextNodeIndex == _nextNodeIndex);
    }
}

[Serializable]
[ShowInInspector]
public class DialogueChoice
{

    [ShowInInspector] public List<Condition> conditions = new List<Condition>();
    public Dictionary<string, Stat> dic;
    public DialogueChoice(string _choicePreview,int _nextNodeIndex)
    {
        choicePreview = _choicePreview;
        nextNodeIndex = _nextNodeIndex;
    }
    public string choicePreview;

    public int nextNodeIndex;

    public bool IsMetConditions()
    {
        foreach (Condition condition in conditions)
        {
            if (!condition.IsConditionTrue()) return false;
        }
        return true;
    }
}
