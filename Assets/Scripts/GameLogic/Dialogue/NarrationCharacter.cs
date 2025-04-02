using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �Ի���ɫ��أ�֮�����չ����ȹ���
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Objects/Dialogue/NarrationCharacter")]
public class NarrationCharacter : SerializedScriptableObject
{
    public string characterName;
    public Dictionary<string,Sprite> IllustrationOfCharacter = new Dictionary<string,Sprite>();
}
