using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Clue
{
    public string clueID;
    public string title;
    [TextArea] public string description;
    public Sprite icon;
    public DateTime discoveryTime;
}