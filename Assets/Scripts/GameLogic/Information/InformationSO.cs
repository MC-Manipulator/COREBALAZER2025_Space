using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Information")]
public class InformationSO : ScriptableObject
{
    public Image bust;
    
    public List<string> texts1;
    public List<string> texts2;

    public int currentIndex = 0;
}
