using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// һ���׶Ի�����������Ի��ڵ�
/// </summary>
[CreateAssetMenu(menuName = "Dialogue/DialogueSO")]
public class DialogueSO : SerializedScriptableObject
{
    [SerializeField]
    [ListDrawerSettings(
        DraggableItems = true, 
        ShowItemCount = true, 
        HideAddButton = false
    )]
    public List<DialogueNode> nodes = new List<DialogueNode>();
}
