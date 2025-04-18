using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClueListButton : MonoBehaviour
{
    public Sprite icon;

    public string clueType;

    public string clueDescription;

    public void SetData(Sprite icon, string clueType, string clueDescription)
    {
        this.icon = icon;
        this.clueType = clueType;
        this.clueDescription = clueDescription;

        this.GetComponent<Image>().sprite = icon;
    }

    private void OnDisable()
    {
        this.GetComponent<Button>().onClick.RemoveAllListeners();
    }
}
