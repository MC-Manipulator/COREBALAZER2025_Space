using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveDialog : MonoBehaviour
{
    public Button confirmButton;
    public Button cancelButton;
    public TMP_Text text;

    protected virtual void OnEnable()
    {
        cancelButton.onClick.AddListener(() => gameObject.SetActive(false));
    }

    protected virtual void OnDisable()
    {
        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
    }
}
