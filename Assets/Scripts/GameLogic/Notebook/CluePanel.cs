using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CluePanel : MonoBehaviour
{
    public List<ClueListButton> clueButtonList;
    public GameObject clueButtonPreb;

    public GameObject clueListContent;
    public ClueListButton currentDetailedClue;

    public Image detailIcon;
    public TMP_Text detailType;
    public TMP_Text detailDescription;

    private void OnEnable()
    {
        detailIcon.sprite = null;
        detailType.text = "";
        detailDescription.text = "";

        clueButtonList = new List<ClueListButton>();
    }

    private void OnDisable()
    {
        currentDetailedClue = null;
        detailIcon.sprite = null;
        detailType.text = "";
        detailDescription.text = "";

        for (int i = clueButtonList.Count - 1;i >= 0;i--)
        {
            Destroy(clueButtonList[i]);
        }
        clueButtonList = null;
    }

    public void Open()
    {
        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }

    public void SelectClue(ClueListButton clueButton)
    {
        this.currentDetailedClue = clueButton;

        this.detailIcon.sprite = clueButton.icon;
        this.detailType.text = clueButton.clueType;
        this.detailDescription.text = clueButton.clueDescription;

    }

    public void AddClue(Sprite image, string type, string description)
    {
        GameObject newClue = Instantiate(clueButtonPreb, clueListContent.transform);
        newClue.GetComponent<ClueListButton>().SetData(image, type, description);
        newClue.GetComponent<Button>().onClick.AddListener(delegate 
        {
            SelectClue(newClue.GetComponent<ClueListButton>());
        });
        clueButtonList.Add(newClue.GetComponent<ClueListButton>());
    }
}
