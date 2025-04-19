using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Node/Branch")]
public class BranchDialogueNode : DialogueNode
{
    [ShowInInspector] public DialogueBranch[] branchs;
    public BranchDialogueNode(string _text, NarrationCharacter _speaker, Sprite _sprite)
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
        return branchs.Any(x => x.nextNodeIndex == _nextNodeIndex);
    }
}

public class DialogueBranch
{
    [ShowInInspector] public List<BranchCondition> conditions = new List<BranchCondition>();
    public StatValuePair[] statValuePairs;
    public DialogueBranch(int _nextNodeIndex)
    {
        nextNodeIndex = _nextNodeIndex;
    }

    public int nextNodeIndex;

    public bool IsMetConditions()
    {
        foreach (BranchCondition condition in conditions)
        {
            if (!condition.IsConditionTrue()) return false;
        }
        return true;
    }
}
