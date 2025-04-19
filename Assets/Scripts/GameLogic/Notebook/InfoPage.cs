using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoPage
{
    public Sprite icon;
    public string cname;
    public string description;
    

    public Dictionary<string, string> detailInfoList;

    public void SetData(Sprite icon, string cname, string description, Dictionary<string, string> detailInfoList)
    {
        this.icon = icon;
        this.cname = cname;
        this.description = description;
        this.detailInfoList = new Dictionary<string, string>();

        foreach (string infoName in detailInfoList.Keys)
        {
            this.detailInfoList.Add(infoName, detailInfoList.GetValueOrDefault(infoName));
        }
    }
}
