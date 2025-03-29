using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ClueDetailPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Image clueImage;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;

    public void Show(Clue clue)
    {
        clueImage.sprite = clue.icon;
        titleText.text = clue.title;
        descriptionText.text = clue.description;
    }
}