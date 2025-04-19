using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPanel : MonoBehaviour
{

    public GameObject completedQuest;
    public GameObject uncompletedQuest;

    private void OnEnable()
    {

    }

    private void OnDisable()
    {
        
    }

    public void Open()
    {
        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }

    public void AddQuest()
    {

    }

    public void RemoveQuest()
    {

    }
}
