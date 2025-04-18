using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAddInfo : MonoBehaviour
{
    public Sprite icon;
    public string cname;
    public string description;
    public Dictionary<string, string> detailInfoList;

    public NotebookUI p;

    public void Add()
    {
        detailInfoList = new Dictionary<string, string>();
        detailInfoList.Add("µÚ1ÌõÐÅÏ¢", "ÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊö");
        detailInfoList.Add("µÚ2ÌõÐÅÏ¢", "ÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊö");
        detailInfoList.Add("µÚ3ÌõÐÅÏ¢", "ÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊö");
        detailInfoList.Add("µÚ4ÌõÐÅÏ¢", "ÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊö");
        detailInfoList.Add("µÚ5ÌõÐÅÏ¢", "ÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊö");
        detailInfoList.Add("µÚ6ÌõÐÅÏ¢", "ÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊö");
        detailInfoList.Add("µÚ7ÌõÐÅÏ¢", "ÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊö");
        detailInfoList.Add("µÚ8ÌõÐÅÏ¢", "ÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊöÃèÊö");
        p.AddInfoPage(icon, cname, description, detailInfoList);
    }
}
