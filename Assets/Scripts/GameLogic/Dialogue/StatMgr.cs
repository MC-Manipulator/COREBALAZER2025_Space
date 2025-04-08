using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatMgr : BaseManager<StatMgr>
{
    public StatData statData;
    public StatMgr()
    {
        statData = ResMgr.GetInstance().Load<StatData>("ScriptableObject/New Stat Data");
    }

    public void ChangeStatValue(string index,Stat value)
    {
        statData.customProperties[index] = value;
    }

    public Stat GetStatValue(string index)
    {
        return statData.customProperties[index];
    }
}
