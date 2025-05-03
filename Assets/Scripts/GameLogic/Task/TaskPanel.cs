using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskPanel : MonoBehaviour
{
    public Transform taskGruop;
    private void Awake()
    {
        TaskManager.GetInstance().GetTaskPanel(this);
    }

    private void OnEnable()
    {
        TaskManager.GetInstance().OnTaskPanelEnale();
    }

    public void AddTask(Task task)
    {
        ResMgr.GetInstance().LoadAsync<GameObject>("UI/Task", (obj) =>
        {
            obj.GetComponent<TaskUI>().Initialization(task.taskName, task.taskInformation);
            obj.transform.SetParent(taskGruop, false);
        });
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}
