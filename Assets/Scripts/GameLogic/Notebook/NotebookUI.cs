using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NotebookUI : MonoBehaviour
{
    public enum PageType
    {
        Quest,
        Info,
        Item,
        Clue,
        Close
    }

    public QuestPanel questPanel;
    public InfoPanel infoPanel;
    public CluePanel cluePanel;

    public Button questPanelButton;
    public Button infoPanelButton;
    public Button itemPanelButton;
    public Button cluePanelButton;

    public PageType currentPageType;

    public List<InfoPage> pageList = new List<InfoPage>();

    private void Awake()
    {
        pageList = new List<InfoPage>();
    }

    public void AddInfoPage(Sprite icon, string cname, string description, Dictionary<string, string> detailInfoList)
    {
        InfoPage newPage = new InfoPage();
        newPage.SetData(icon, cname, description, detailInfoList);
        pageList.Add(newPage);
        infoPanel.AddLabelButton(cname[0], pageList.Count - 1);
    }

    public void Open()
    {
        SwitchPage(PageType.Quest);
        questPanelButton.onClick.AddListener(delegate { SwitchPage(PageType.Quest); });
        infoPanelButton.onClick.AddListener(delegate { SwitchPage(PageType.Info); });
        //itemPanelButton.onClick.AddListener(delegate { SwitchPage(PageType.Item); });
        //cluePanelButton.onClick.AddListener(delegate { SwitchPage(PageType.Clue); });
        questPanelButton.gameObject.SetActive(true);
        infoPanelButton.gameObject.SetActive(true);
        itemPanelButton.gameObject.SetActive(true);
        cluePanelButton.gameObject.SetActive(true);

        infoPanel.pageList = this.pageList;
    }

    public void Close()
    {
        switch (this.currentPageType)
        {
            case PageType.Quest:
                questPanel.Close();
                break;
            case PageType.Info:
                infoPanel.Close();
                break;
            case PageType.Item:
                //
                break;
            case PageType.Clue:
                cluePanel.Close();
                break;
            default:
                break;
        }

        this.currentPageType = PageType.Close;

        questPanelButton.onClick.RemoveAllListeners();
        infoPanelButton.onClick.RemoveAllListeners();
        cluePanelButton.onClick.RemoveAllListeners();
        questPanelButton.gameObject.SetActive(false);
        infoPanelButton.gameObject.SetActive(false);
        itemPanelButton.gameObject.SetActive(false);
        cluePanelButton.gameObject.SetActive(false);
    }

    public void SwitchPage(PageType type)
    {
        if (type == currentPageType)
        {
            return;
        }

        switch (this.currentPageType)
        {
            case PageType.Quest:
                questPanel.Close();
                break;
            case PageType.Info:
                infoPanel.Close();
                break;
            case PageType.Item:
                //
                break;
            case PageType.Clue:
                cluePanel.Close();
                break;
            case PageType.Close:
                break;

        }

        this.currentPageType = type;

        switch (this.currentPageType)
        {
            case PageType.Quest:
                questPanel.Open();
                break;
            case PageType.Info:
                infoPanel.Open();
                break;
            case PageType.Item:
                //
                break;
            case PageType.Clue:
                cluePanel.Open();
                break;
        }
    }
}
