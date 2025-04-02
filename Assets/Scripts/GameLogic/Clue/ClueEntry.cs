using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ClueEntry : MonoBehaviour
{
    [SerializeField] private Image clueIcon;
    [SerializeField] private TMP_Text clueTitle;
    [SerializeField] private Button entryButton;

    private Clue currentClue;

    public void Initialize(Clue clue, Action onClick)
    {
        currentClue = clue;
        clueIcon.sprite = clue.icon;
        clueTitle.text = clue.title;
        entryButton.onClick.AddListener(() => onClick());
        gameObject.name = clue.title;
    }
}