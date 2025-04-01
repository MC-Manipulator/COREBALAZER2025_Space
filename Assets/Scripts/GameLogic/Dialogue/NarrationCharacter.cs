using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对话角色相关，之后可拓展立绘等功能
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Objects/Dialogue/NarrationCharacter")]
public class NarrationCharacter:ScriptableObject
{
    public string characterName;
}
