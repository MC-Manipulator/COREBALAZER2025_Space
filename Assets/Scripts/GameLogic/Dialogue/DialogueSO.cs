using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// һ���׶Ի�����������Ի��ڵ�
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Objects/Dialogue/Dialogue")]
public class DialogueSO : SerializedScriptableObject
{
    [SerializeField]
    public List<DialogueNode> nodes = new List<DialogueNode>();
}
