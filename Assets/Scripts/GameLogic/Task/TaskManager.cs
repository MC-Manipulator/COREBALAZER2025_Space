using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : BaseManager<TaskManager>
{
    private Dictionary<string,Task> tasks = new Dictionary<string,Task>();
    private TaskPanel taskPanel;
    private List<Task> activeTaskList = new List<Task>();

    public TaskManager()
    {
        Task[] allTasks = Resources.LoadAll<Task>("ScriptableObject/Task");
        foreach (Task task in allTasks)
        {
            tasks.Add(task.name, task);
            Debug.Log(task.name);
        }
    }

    public void StartTask(string taskName)
    {
        if (!tasks.ContainsKey(taskName))
        {
            Debug.Log("不存在任务" + taskName);
            return;
        }
        if (tasks[taskName].taskState != TaskState.NoStart)
        {
            Debug.Log("状态不符合");
            return;
        }
        tasks[taskName].taskState = TaskState.Progress;
        activeTaskList.Add(tasks[taskName]);
    }

    public void FinishTask(string taskName)
    {
        if (!tasks.ContainsKey(taskName))
        {
            Debug.Log("不存在任务" + taskName);
            return;
        }
        if (tasks[taskName].taskState != TaskState.Progress)
        {
            Debug.Log("状态不符合");
            return;
        }
        tasks[taskName].taskState = TaskState.Finish;
        activeTaskList.Remove(tasks[taskName]);
        EventCenter.GetInstance().EventTrigger("任务完成", tasks[taskName]);
    }

    public void GetTaskPanel(TaskPanel _taskPanel)
    {
        taskPanel = _taskPanel;
    }

    public void OnTaskPanelEnale()
    {
        foreach (Task task in activeTaskList)
        {
            taskPanel.AddTask(task);
        }
    }
}
