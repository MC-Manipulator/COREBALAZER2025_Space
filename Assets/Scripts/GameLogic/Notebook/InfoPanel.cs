using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    public int currentPageNum;
    public InfoPage currentPage;
    public Image icon;
    public TMP_Text cname;
    public TMP_Text description;
    public List<GameObject> detailInfoList;

    public GameObject basicInfo;
    public GameObject lableList;
    public GameObject leftPage;
    public GameObject rightPage;
    public GameObject detailInfoPref;
    public GameObject labelButtonPref;

    private RectTransform _LPRect;
    private RectTransform _RPRect;
    private VerticalLayoutGroup _LPLayout;
    private VerticalLayoutGroup _RPLayout;

    public List<InfoPage> pageList;



    private void OnEnable()
    {
        _LPRect = leftPage.GetComponent<RectTransform>();
        _RPRect = rightPage.GetComponent<RectTransform>();
        _LPLayout = leftPage.GetComponent<VerticalLayoutGroup>();
        _RPLayout = rightPage.GetComponent<VerticalLayoutGroup>();
        detailInfoList = new List<GameObject>();
        SetPage(0);
    }

    private void OnDisable()
    {
        for (int i = detailInfoList.Count - 1; i >= 0; i--)
        {
            Destroy(detailInfoList[i]);
        }

        detailInfoList = null;
    }

    public void Open()
    {
        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }

    public void AddInfoPage(Sprite icon, string cname, string description, Dictionary<string, string> detailInfoList)
    {
        InfoPage newPage = new InfoPage();
        newPage.SetData(icon, cname, description, detailInfoList);
        pageList.Add(newPage);
        AddLabelButton(cname[0], pageList.Count - 1);
    }

    public void SetBasicInfo()
    {
        icon.sprite = currentPage.icon;
        cname.text = currentPage.cname;
        description.text = currentPage.description;
    }

    public void AddDetailInfo(string name, string description)
    {
        GameObject newInfo = Instantiate(detailInfoPref);

        newInfo.transform.Find("Name").GetComponent<TMP_Text>().text = name;
        newInfo.transform.Find("Description").GetComponent<TMP_Text>().text = description;

        int maxCountInLeft = (int)((_LPRect.rect.height - _LPLayout.padding.top - _LPLayout.padding.bottom - basicInfo.GetComponent<RectTransform>().rect.height) / detailInfoPref.GetComponent<RectTransform>().rect.height);


        bool isLeftFull = detailInfoList.Count + 1 > maxCountInLeft;
        if (!isLeftFull)
        {
            newInfo.transform.SetParent(leftPage.transform);
        }
        else
        {
            newInfo.transform.SetParent(rightPage.transform);
        }
        newInfo.transform.localScale = new Vector3(1, 1, 1);

        this.detailInfoList.Add(newInfo);
    }

    public void AddLabelButton(char character, int pageNum)
    {
        GameObject newLabel = Instantiate(labelButtonPref);

        newLabel.transform.Find("Character").GetComponent<TMP_Text>().text = character.ToString();
        newLabel.transform.SetParent(lableList.transform);

        newLabel.GetComponent<Button>().onClick.AddListener(delegate 
        {
            SetPage(pageNum);
        });

    }

    public void SetPage(int pageNum)
    {
        if (pageNum > pageList.Count)
        {
            return;
        }

        currentPage = pageList[pageNum];
        currentPageNum = pageNum;

        for (int i = detailInfoList.Count - 1; i >= 0; i--)
        {
            Destroy(detailInfoList[i]);
        }

        SetBasicInfo();

        this.detailInfoList = new List<GameObject>();
        foreach (string infoName in currentPage.detailInfoList.Keys)
        {
            Debug.Log(infoName);
            AddDetailInfo(infoName, currentPage.detailInfoList.GetValueOrDefault(infoName));
        }
    }

    public void LastPage()
    {
        if (currentPageNum - 1 > 0)
            SetPage(currentPageNum - 1);
    }

    public void NextPage()
    {
        if (currentPageNum + 1 < pageList.Count)
            SetPage(currentPageNum + 1);
    }

    public InfoPage FindPageByName(char firstCharacter)
    {
        foreach (InfoPage page in pageList)
        {
            if (page.cname[0] == firstCharacter)
            {
                return page;
            }
        }
        return null;
    }
}
