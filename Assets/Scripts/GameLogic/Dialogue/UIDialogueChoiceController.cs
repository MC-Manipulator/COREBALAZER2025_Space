using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDialogueChoiceController : MonoBehaviour
{
    private TextMeshProUGUI m_Choice;

    private int m_ChoiceNextNode;
    public Dictionary<string, Stat> dic;

    private void Awake()
    {
        m_Choice = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Initialization(int _choiceNode,string _choicePreview,Dictionary<string, Stat> _dic)
    {
        m_Choice.text = _choicePreview;
        m_ChoiceNextNode = _choiceNode;
        dic = _dic;
    }

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        EventCenter.GetInstance().EventTrigger<int>("对话节点请求带参", m_ChoiceNextNode);
        if (dic != null)
        {
            if (dic.Count > 0)
            {
                foreach (KeyValuePair<string, Stat> pair in dic)
                {
                    StatMgr.GetInstance().ChangeStatValue(pair.Key, pair.Value);
                }
            }
        }
        Destroy(transform.parent.gameObject);
    }
}
