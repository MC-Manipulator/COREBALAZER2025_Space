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
    public StatValuePair[] statValuePairs;
    private void Awake()
    {
        m_Choice = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Initialization(int _choiceNode,string _choicePreview,StatValuePair[] _statValuePairs)
    {
        m_Choice.text = _choicePreview;
        m_ChoiceNextNode = _choiceNode;
        statValuePairs = _statValuePairs;
    }

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        EventCenter.GetInstance().EventTrigger<int>("对话节点请求带参", m_ChoiceNextNode);
        if (statValuePairs != null)
        {
            if (statValuePairs.Length > 0)
            {
                foreach (StatValuePair pair in statValuePairs)
                {
                    pair.ApplyValueToStat();
                }
            }
        }
        Destroy(transform.parent.gameObject);
    }
}
