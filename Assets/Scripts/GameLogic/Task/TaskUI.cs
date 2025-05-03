using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TaskUI : MonoBehaviour
{
    public TextMeshProUGUI taskName;
    public TextMeshProUGUI taskDescription;

    private void OnEnable()
    {
        TaskManager.GetInstance().OnTaskPanelEnale();
    }
    public void Initialization(string _taskName,string _taskDescription)
    {
        taskName.text = _taskName;
        taskDescription.text = _taskDescription;
    }
}
