using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Task")]
public class Task : ScriptableObject
{
    public TaskState taskState = TaskState.NoStart;
    public string taskName;
    public string taskInformation;

    public bool isComoeleted {  get; private set; }

    public void CompeleteTask()
    {
        isComoeleted = true;
    }
}

public enum TaskState
{
    NoStart,
    Progress,
    Finish
}
