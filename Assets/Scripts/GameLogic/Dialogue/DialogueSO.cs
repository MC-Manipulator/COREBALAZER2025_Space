using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 一整套对话，包含多个对话节点
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Objects/Dialogue/Dialogue")]
public class DialogueSO : SerializedScriptableObject
{
    [SerializeField]
    public List<DialogueNode> nodes = new List<DialogueNode>();
}
