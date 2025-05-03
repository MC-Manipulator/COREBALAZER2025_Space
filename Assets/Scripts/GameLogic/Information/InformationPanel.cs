using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InformationPanel : MonoBehaviour
{
    public Image bust;
    public TextMeshProUGUI textMeshProUGUI1;
    public TextMeshProUGUI textMeshProUGUI2;

    public void UpdatePanel(InformationSO _information)
    {
        textMeshProUGUI1.text = _information.texts1[_information.currentIndex];
        textMeshProUGUI2.text = _information.texts2[_information.currentIndex];
        bust = _information.bust;
    }
}
