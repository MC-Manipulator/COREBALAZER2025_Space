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
        detailInfoList.Add("��1����Ϣ", "������������������������������������������������");
        detailInfoList.Add("��2����Ϣ", "������������������������������������������������");
        detailInfoList.Add("��3����Ϣ", "������������������������������������������������");
        detailInfoList.Add("��4����Ϣ", "������������������������������������������������");
        detailInfoList.Add("��5����Ϣ", "������������������������������������������������");
        detailInfoList.Add("��6����Ϣ", "������������������������������������������������");
        detailInfoList.Add("��7����Ϣ", "������������������������������������������������");
        detailInfoList.Add("��8����Ϣ", "������������������������������������������������");
        p.AddInfoPage(icon, cname, description, detailInfoList);
    }
}
