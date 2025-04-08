using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对话节点抽象基类
/// </summary>
[Serializable]
public abstract class DialogueNode : SerializedScriptableObject
{
    public string m_text;
    public Sprite m_sprite;
    [SerializeField] public NarrationCharacter speaker;
    public int nextNodeIndex = 0;

    public abstract void Accept(DialogueNodeVisitor visitor);
    public abstract bool CanBeFollowedByNode(int _nextNodeIndex);
}
